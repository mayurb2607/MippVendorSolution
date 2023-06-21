using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using MippSamplePortal.Data;
using MippSamplePortal.Models;
using MippSamplePortal.Services;
using MippSamplePortal.ViewModel;
using Newtonsoft.Json;
using System.Text;

namespace MippSamplePortal.Controllers
{
    public class InviteController : Controller
    {

        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly EmailService _emailService;
        private MippTestContext _context;

        public InviteController(IConfiguration configuration, IHostEnvironment hostEnvironment, EmailService emailService, MippTestContext context)
        {
            _configuration = configuration;
            _hostEnvironment = hostEnvironment;
            _emailService = emailService;
            _context = context;
        }

        [HttpPost]
        public async Task<string> ValidateInvite(string email, string clientId, string rootVendorId)
        {
            InviteViewModel invite = new InviteViewModel()
            {
                ClientId = int.Parse(clientId),
                Email = email,
                RootVendorId = int.Parse(rootVendorId)
            };

            string fromEmail = _context.Clients.FirstOrDefault(x => x.ClientId == invite.ClientId).Email;

            VendorInvite vendorInvite = _context.VendorInvites.FirstOrDefault(x => x.VendorEmail == invite.Email);
            if (vendorInvite == null)
            {
                return await _emailService.SendInviteForNewVendor(_context, new SendEmailViewModel { ClientID = int.Parse(clientId), VendorID = invite.RootVendorId, ToEmail=invite.Email, Body="", Cc=null, FromEmail = fromEmail, Subject="" });
                //Call SendEmail API endpoint for valid invite

            }
            //validate invite using Service class method
            return "This Vendor has already joined Vendor Portal. Do you wish to send him an invite?";
           
        }

      
    }
}
