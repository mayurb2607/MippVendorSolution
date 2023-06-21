using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using MippPortalWebAPI.Helpers;
using MippSamplePortal.ViewModel;

namespace MippPortalWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendEmailController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly MailHelper _mailHelper;

        public SendEmailController(IConfiguration configuration, IHostEnvironment hostEnvironment, MailHelper mailHelper)
        {
            _configuration = configuration;
            _hostEnvironment = hostEnvironment;
            _mailHelper = mailHelper;
        }

        [EnableCors("AllowOrigin")]
        [HttpPost("Send")]
        public async Task<IActionResult> Send(SendEmailViewModel sendEmail)
        {
            //MippVendorPortalUrl
            var env = _hostEnvironment.EnvironmentName;
            string apiUrl = string.Empty;
            if (env == "Development")
            {
                // Configure services for development environment
                apiUrl = _configuration.GetValue<string>("MippVendorPortalUrl");
                //apiUrl = _configuration.GetValue<string>("DevEnvironmentAPIUrl");
            }
            else
            {
                // Configure services for local environment
                apiUrl = _configuration.GetValue<string>("MippVendorPortalUrl");
            }
            try
            {
                string url = apiUrl + "/Join/Index";
                CryptographyHelper cryptographyHelper = new CryptographyHelper();
                var mail = cryptographyHelper.EncryptString(sendEmail.ToEmail);
                var param = new Dictionary<string, string>() { { "e", mail }, { "cId", sendEmail.ClientID.ToString() }, { "rvId", sendEmail.VendorID.ToString() } };


                var newUrl = new Uri(QueryHelpers.AddQueryString(url, param));
                SendEmailViewModel request = new SendEmailViewModel();
                request.Body = "Registration Link:" + " " + newUrl;
                request.ToEmail = sendEmail.ToEmail;
                request.Subject = "Invitation: Mipp Vendor Portal";
                await _mailHelper.SendEmailAsync(request);
                return Ok();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
