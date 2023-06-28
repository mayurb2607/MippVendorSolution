using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Hosting;
using MippSamplePortal.Models;
using MippSamplePortal.ViewModel;
using Newtonsoft.Json;
using System.Text;

namespace MippSamplePortal.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _hostEnvironment;

        public EmailService(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            _configuration = configuration;
            _hostEnvironment = hostEnvironment;
        }
        public EmailService() { }

        public async Task<string> SendInviteForNewVendor(MippTestContext context, SendEmailViewModel inviteModel)
        {
            VendorInvite vendor = new VendorInvite();
            vendor.VendorEmail = inviteModel.ToEmail;
            vendor.VendorId = context.Vendors.Count() + 1;
            vendor.ClientId = inviteModel.ClientID;
            vendor.InviteSentDate = DateTime.Now.ToShortDateString();
            vendor.JoinedDate = "";
            //vendor.Id = context.VendorInvites.Count() + 1;
            context.VendorInvites.Add(vendor);
            context.SaveChangesAsync();

            try
            {
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
                    apiUrl = _configuration.GetValue<string>("LocalEnvironmentAPIUrl");
                }

                using (var httpClient = new HttpClient())
                {
                    inviteModel.Cc = new List<string>();
                    StringContent content = new StringContent(JsonConvert.SerializeObject(inviteModel), Encoding.UTF8, "application/json");

                    using (var response = await httpClient.PostAsync(apiUrl + "SendEmail/Send", content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        //receivedReservation = JsonConvert.DeserializeObject<Reservation>(apiResponse);
                        return apiResponse;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> SendAcceptanceEmail(MippTestContext context, SendEmailViewModel inviteModel)
        {
            VendorInvite vendor = new VendorInvite();
            vendor.VendorEmail = inviteModel.ToEmail;
            vendor.VendorId = context.Vendors.Count() + 1;
            vendor.ClientId = inviteModel.ClientID;
            vendor.InviteSentDate = DateTime.Now.ToShortDateString();
            vendor.JoinedDate = "";
            //vendor.Id = context.VendorInvites.Count() + 1;
            context.VendorInvites.Add(vendor);
            context.SaveChangesAsync();

            try
            {
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
                    apiUrl = _configuration.GetValue<string>("LocalEnvironmentAPIUrl");
                }

                using (var httpClient = new HttpClient())
                {
                    inviteModel.Cc = new List<string>();
                    StringContent content = new StringContent(JsonConvert.SerializeObject(inviteModel), Encoding.UTF8, "application/json");

                    using (var response = await httpClient.PostAsync(apiUrl + "SendEmail/Send", content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        //receivedReservation = JsonConvert.DeserializeObject<Reservation>(apiResponse);
                        return apiResponse;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

}
