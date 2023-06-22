using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using MippPortalWebAPI.Helpers;
using MippPortalWebAPI.Models;
using MippSamplePortal.Areas.Identity.Pages.Account;
using MippSamplePortal.ViewModel;
using System.Runtime.CompilerServices;
using System.Web;

namespace MippPortalWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendEmailController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly MailHelper _mailHelper;
        private readonly MippTestContext _context;

        public SendEmailController(IConfiguration configuration, IHostEnvironment hostEnvironment, MailHelper mailHelper, MippTestContext context)
        {
            _configuration = configuration;
            _hostEnvironment = hostEnvironment;
            _mailHelper = mailHelper;
            _context = context;
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



        [EnableCors("AllowOrigin")]
        [HttpPost("SendForgotPasswordEmail")]
        public async Task<IActionResult> SendForgotPasswordEmail(LoginModel loginModel)
        {
            try
            {
                SendEmailViewModel request = new SendEmailViewModel();
                request.Body = "Email:" + " " + loginModel.Input.Email + "<br/>" + "Password: " + " " + loginModel.Input.Password;
                request.ToEmail = loginModel.Input.Email;
                request.Subject = "Credentials for Vendor Portal";
                await _mailHelper.SendEmailAsync(request);
                return Ok();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [EnableCors("AllowOrigin")]
        [HttpPost("SendPasswordResetEmail")]
        public async Task<IActionResult> SendPasswordResetEmail(LoginModel loginModel)
        {
            try
            {
                SendEmailViewModel request = new SendEmailViewModel();
                request.Body = "Your password has been resetted successfully! ";
                request.ToEmail = loginModel.Input.Email;
                request.Subject = "Mipp Vendor Portal: Password reset";
                await _mailHelper.SendEmailAsync(request);
                return Ok();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [EnableCors("AllowOrigin")]
        [HttpPost("SendAcceptance")]
        public async Task<IActionResult> SendAcceptance(InviteModel invitemodel)
        {
            try
            {
                const string url = "https://localhost:7013/Join/Accept";
                CryptographyHelper cryptographyHelper = new CryptographyHelper();
                var mail = cryptographyHelper.EncryptString(invitemodel.Email);

                var param = new Dictionary<string, string>() { { "VendorEmail", mail }, { "clientID", invitemodel.clientId.ToString() } };

                var newUrl = new Uri(QueryHelpers.AddQueryString(url, param));
                SendEmailViewModel request = new SendEmailViewModel();
                request.Body = "Invite Link:" + " " + newUrl;
                request.ToEmail = invitemodel.Email;
                request.Subject = "Invitation: You have an invite!";
                await _mailHelper.SendEmailAsync(request);
                return Ok();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [EnableCors("AllowOrigin")]
        [HttpPost("SendWorkorderUpdateEmail")]
        public async Task<IActionResult> SendWorkorderUpdateEmail(Workorder workorder)
        {
            try
            {

                SendEmailViewModel request = new SendEmailViewModel();
                var uriBuilder = new UriBuilder("https://localhost:7179/Workorders");
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                query["VendorId"] = workorder.VendorId.ToString();
                uriBuilder.Query = query.ToString();


                request.Body = "Workorder with Id: " + workorder.Id + " assigned to" + " " + workorder.AssignedTo + " " + "with comments:" + " " + workorder.AdditionalComments + "<br/>" +
                    "Click here to access your dashboard: " + uriBuilder;
                request.ToEmail = workorder.AssignedToEmailAddress;
                //request.Cc.Add(workorder.PropertyManagerEmail);
                //request.ToEmail = workorder.PropertyManagerEmail;
                request.Subject = "You have a new Workorder assigned to you, Id:" + " " + workorder.Id;
                await _mailHelper.SendEmailAsync(request);
                return Ok();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [EnableCors("AllowOrigin")]
        [HttpPost("Sen_contextillUpdateEmail")]
        public async Task<IActionResult> Sen_contextillUpdateEmail(Bill bill)
        {
            try
            {
                SendEmailViewModel request = new SendEmailViewModel();
                request.Body = "Bill" + " " + bill.Id + " " + "needs to be updated for Workorder" + " " + bill.Wonumber + " " + "by" + " " + bill.VendorId;
                //request.ToEmail = bill.vendoremail;
                //request.Cc = bill.PropertyManagerEmail;
                request.Subject = "Bill requested for update for workorder:" + " " + bill.Wonumber + "by" + bill.CareOf;
                await _mailHelper.SendEmailAsync(request);
                return Ok();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [EnableCors("AllowOrigin")]
        [HttpPost("Sen_contextillCreatedEmail")]
        public async Task<IActionResult> Sen_contextillCreatedEmail(Bill bill)
        {
            try
            {
                SendEmailViewModel request = new SendEmailViewModel();
                request.Body = "Bill" + " " + bill.Id + " " + "has been created for Workorder" + " " + bill.Wonumber + " " + "by" + " " + bill.VendorId;
                //request.ToEmail = bill.vendoremail;
                //request.Cc = bill.PropertyManagerEmail;
                request.Subject = "Bill created for workorder:" + " " + bill.Wonumber;
                await _mailHelper.SendEmailAsync(request);
                return Ok();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [EnableCors("AllowOrigin")]
        [HttpPost("SendWorkorderStatusUpdateEmail")]
        public async Task<IActionResult> SendWorkorderStatusUpdateEmail(Workorder workorder)
        {
            try
            {

                SendEmailViewModel request = new SendEmailViewModel();
                request.Body = "Workorder" + " " + workorder.Id + " " + "updated with status" + " " + workorder.Status + " " + "by" + " " + workorder.AssignedTo;
                request.ToEmail = workorder.AssignedToEmailAddress;
                request.Cc.Add(workorder.PropertyManagerEmail);
                request.Subject = "Workorder Update for:" + " " + workorder.Id;
                await _mailHelper.SendEmailAsync(request);
                return Ok();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        [EnableCors("AllowOrigin")]
        [HttpPost("ValidateInvite")]
        public bool ValidateInvite(InviteModel invitemodel)
        {
            VendorInvite vendorInvite = _context.VendorInvites.FirstOrDefault(x => x.VendorEmail == invitemodel.Email);
            if (vendorInvite == null)
            {
                return true;
            }
            return false;
        }

        [EnableCors("AllowOrigin")]
        [HttpPost("UpdateVendorInvite")]
        //call this API on Join from Vendor portal
        public async Task<Vendor> UpdateVendorInvite(VendorInvite vendorInvite)
        {
            if (vendorInvite != null)
            {
                Vendor vendor = new Vendor();
                //vendor.Id = _context.Vendors.Count() + 1;
                vendor.Email = vendorInvite.VendorEmail;
                vendor.BusinessName = "";
                vendor.FirstName = vendorInvite.VendorEmail.Split("@")[0].ToString();
                if (_context.Vendors.FirstOrDefault(x => x.Email == vendorInvite.VendorEmail) == null)
                {
                    _context.Vendors.Add(vendor);
                     _context.SaveChanges();
                }

                var vi = _context.VendorInvites.FirstOrDefault(x => x.VendorEmail == vendorInvite.VendorEmail);
                vi.JoinedDate = DateTime.Now.ToShortDateString();
                _context.VendorInvites.Update(vi);


                _context.SaveChanges();
                return vendor;

            }
            return null;

        }
    }
}
