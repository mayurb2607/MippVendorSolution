// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using MimeKit;
using MippSamplePortal.Areas.Identity.Data;
using MippSamplePortal.ViewModel;

namespace MippSamplePortal.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<MippSamplePortalUser> _userManager;
        private readonly IEmailSender _emailSender;


        public ForgotPasswordModel(UserManager<MippSamplePortalUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
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

                //await _emailSender.SendEmailAsync(
                //    Input.Email,
                //    "Reset Password",
                //    $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

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
                request.Body = "Reset password, click here:" + loginModel.CallbackUrl + "<br/>";
                request.ToEmail = loginModel.Email;
                request.Subject = "Credentials for Vendor Portal";
                await SendEmailAsync(request);

            }
            catch (Exception ex)
            {
                throw ex;
            }



        }

        public async Task SendEmailAsync(SendEmailViewModel mailRequest)
        {

            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse("bhavsarmayur26@gmail.com");
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            email.Subject = mailRequest.Subject;
            if (mailRequest.Cc != null && mailRequest.Cc.Count != 0)
            {
                foreach (var item in mailRequest.Cc)
                {
                    email.Cc.Add(MailboxAddress.Parse(item));

                }
            }

            var builder = new BodyBuilder();
            builder.HtmlBody = mailRequest.Body;

            email.Body = builder.ToMessageBody();
            var smtp = new MailKit.Net.Smtp.SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("bhavsarmayur26@gmail.com", "osgbupqipgyqagrk");
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }

        public class Login
        {
            public string Email { get; set; }
            public string CallbackUrl { get; set; }
        }

    }
}
