// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Hosting;

using MippVendorPortal.Areas.Identity.Data;
using MippVendorPortal.ViewModel;
using Newtonsoft.Json;

namespace MippVendorPortal.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<MippVendorPortalUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _hostEnvironment;

        public ForgotPasswordModel(UserManager<MippVendorPortalUser> userManager, IEmailSender emailSender , IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _configuration = configuration;
            _hostEnvironment = hostEnvironment;
    
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code },
                protocol: Request.Scheme);


                 SendForgotPasswordEmail(
                    new Login { Email = Input.Email, CallbackUrl = HtmlEncoder.Default.Encode(callbackUrl) });


                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }


        public async void SendForgotPasswordEmail(Login loginModel)
        {
            try
            {
                SendEmailViewModel request = new SendEmailViewModel();
                request.FromEmail = "bhavsarmayur26@gmail.com";
                request.Cc = new List<string>();
                request.Body = "Reset password, click here: <div> <a href ='" + loginModel.CallbackUrl + "'>Reset Password </a> </div> ";
                request.ToEmail = loginModel.Email;
                request.Subject = "Credentials for Vendor Portal";
                var env = _hostEnvironment.EnvironmentName;
                string apiUrl = string.Empty;
                if (env == "Development")
                {
                    // Configure services for development environment
                    apiUrl = _configuration.GetValue<string>("LocalEnvironmentAPIUrl");
                    //apiUrl = _configuration.GetValue<string>("DevEnvironmentAPIUrl");
                }
                else
                {
                    // Configure services for local environment
                    apiUrl = _configuration.GetValue<string>("ProdEnvironmentAPIUrl");
                }
                using (var httpClient1 = new HttpClient())
                {
                    StringContent content1 = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

                    using (var response1 = await httpClient1.PostAsync(apiUrl + "SendEmail/Send", content1))
                    {
                        string apiResponse1 = await response1.Content.ReadAsStringAsync();
                        
                       
                    }
                }
                //await _mailHelper.SendEmailAsync(request);
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    public class Login
    {
        public string Email { get; set; }
        public string CallbackUrl { get; set; }
    }
}
