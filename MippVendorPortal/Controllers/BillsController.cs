using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using dotless.Core.Parser.Functions;
using dotless.Core.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MippPortalWebAPI.Helpers;
using MippPortalWebAPI.Models;
using MippSamplePortal.Models;
using MippSamplePortal.ViewModel;
using MippVendorPortal.Models;
using MippVendorPortal.ViewModel;
using Newtonsoft.Json;
using Syncfusion.EJ2.Charts;
using Syncfusion.EJ2.Linq;


namespace MippVendorPortal.Controllers
{
    [Authorize]
    public class BillsController : Controller
    {
        private readonly MippVendorTestContext _context;
        private readonly MippPortalWebAPI.Models.MippTestContext _context1;
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _hostEnvironment;

        public BillsController(MippVendorTestContext context, MippPortalWebAPI.Models.MippTestContext context1, IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            _context = context;
            _context1 = context1;
            _configuration = configuration;
            _hostEnvironment = hostEnvironment;

        }



        [HttpPost]
        public IActionResult Upload(IFormFile file1)
        {
            var files = Request.Form.Files; // Access the uploaded files here
            List<string> filesUrl = new List<string>();
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var filePath = Path.Combine("C:\\MiPP\\MippVendor\\MippVendorPortal", fileName); // Specify the desired directory path

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    string connectionString = "DefaultEndpointsProtocol=https;AccountName=mippbills;AccountKey=WV5m77LeyX2X21hEhLov5gZ6rn0RX7goEXxIGK9/ju/7i07oGX+i/P/XI/e4aKFVraPxyjaKwMBl+AStR305aw==;EndpointSuffix=core.windows.net"; // Replace with your Azure Blob Storage connection string
                    string containerName = "mipp-bill-accounts"; // Replace with your container name

                    //CloudBlobClient blobClient = account.CreateCloudBlobClient();
                    //CloudBlobContainer container = blobClient.GetContainerReference("images");

                    // Create a BlobServiceClient object
                    BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

                    // Get a reference to the container
                    BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                    // Generate a unique blob name
                    string blobName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    //CloudBlockBlob blockBlob = containerClient.GetBlockBlobReference("/JamesDon//1//" + blobName);


                    // Create a new blob in the container
                    BlobClient blobClient = containerClient.GetBlobClient(blobName);

                    // Open a FileStream to read the file
                    using (FileStream fileStream = new FileStream(file.FileName, FileMode.Open))
                    {
                        // Upload the file to Azure Blob Storage
                        blobClient.Upload(fileStream, true);
                    }
                    // Get the public URL of the uploaded image
                    var fileUrl = blobClient.Uri.ToString();
                    filesUrl.Add(fileUrl);
                    //TempData["fileUrl"] = fileUrl;
                    ViewBag.fileUrl = fileUrl;
                    // Optionally, you can delete the local file after uploading it to Azure Blob Storage
                    //System.IO.File.Delete(file.FileName);



                }
            }

            // If no file was selected or the file is empty, display an error message
            ModelState.AddModelError("file", "Please select a file to upload.");

            //int clientId = 1;

            return Json(filesUrl);


            // Process the uploaded files


        }

        [HttpPost("Remove")]
        public async Task<IActionResult> Remove(IEnumerable<string> urls)
        {
            foreach (var item in urls)
            {
                string connectionString = "DefaultEndpointsProtocol=https;AccountName=mippbills;AccountKey=WV5m77LeyX2X21hEhLov5gZ6rn0RX7goEXxIGK9/ju/7i07oGX+i/P/XI/e4aKFVraPxyjaKwMBl+AStR305aw==;EndpointSuffix=core.windows.net"; // Replace with your Azure Blob Storage connection string
                string containerName = "mipp-bill-accounts"; // Replace with your container name

                //CloudBlobClient blobClient = account.CreateCloudBlobClient();
                //CloudBlobContainer container = blobClient.GetContainerReference("images");

                // Create a BlobServiceClient object
                BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

                // Get a reference to the container
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                // Extract the blob name from the URL
                var blobName = Path.GetFileName(item);

                // Get the blob client using the blob name
                var blobClient = containerClient.GetBlobClient(blobName);

                // Delete the blob
                blobClient.DeleteIfExists();
            }
           

            return Ok();
        }
        public async Task<IActionResult> Index(int vendorID, int woID)
        {
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


                Setting settings = _context.Settings.FirstOrDefault(x => x.VendorId == vendorID);

                List<Setting> lst = new List<Setting>();
                lst.Add(settings);
                //DateTime dt = DateTime.ParseExact(settings.BillDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //DateTime dt2 = DateTime.ParseExact(settings.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                //settings.BillDate = dt;
                //settings.DueDate = dt2;
                IEnumerable<Setting> enumerable = lst;
                var clientId = _context.VendorClients.FirstOrDefault(x => x.VendorId == vendorID).ClientId;
                List<string> taxList = new List<string>();


                //var taxes = _context1.Taxes.Where(x => x.ClientId == clientId);
              
                
                MippPortalWebAPI.Helpers.WorkorderRequest workorderRequest = new MippPortalWebAPI.Helpers.WorkorderRequest();
                workorderRequest.ClientID = clientId;
                IEnumerable<MippPortalWebAPI.Models.Tax> taxes = null;
                using (var httpClient1 = new HttpClient())
                {
                    StringContent content1 = new StringContent(JsonConvert.SerializeObject(workorderRequest), Encoding.UTF8, "application/json");

                    using (var response1 = await httpClient1.PostAsync(apiUrl + "Bills/GetTaxList", content1))
                    {
                        string apiResponse1 = await response1.Content.ReadAsStringAsync();
                        try
                        {
                            taxes = JsonConvert.DeserializeObject
                                <IEnumerable<MippPortalWebAPI.Models.Tax>>(apiResponse1);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
                ViewBag.taxList = taxes;


                ViewBag.woid = woID;
                return View(enumerable);
            }
            catch (Exception ex)
            {

                return View();
            }
            //return View(billSettingsViewModel);
        }



        [HttpGet("Bills")]
        public async Task<IActionResult> Bills(int vendorId, string msg)
        {
            MippPortalWebAPI.Helpers.WorkorderRequest workorderRequest = new MippPortalWebAPI.Helpers.WorkorderRequest();
            workorderRequest.VendorID = vendorId;
            workorderRequest.Status = "";
            workorderRequest.AdditionalComments = "";

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

            IEnumerable<MippPortalWebAPI.Models.Bill> bill;
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(workorderRequest), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync(apiUrl + "Bills/GetBills", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    try
                    {
                        bill = JsonConvert.DeserializeObject<IEnumerable<MippPortalWebAPI.Models.Bill>>(apiResponse);
                        ViewBag.VendorId = vendorId;
                        ViewBag.Workorder = bill;
                        ViewBag.msg = msg;


                        ViewData["GridData"] = bill;
                        ViewBag.GridData = bill;

                        return View(bill);

                    }
                    catch (Exception ex)
                    {

                    }
                }
            }



            return View(_context1.Bills);
        }

        [HttpPost]
        public async Task<IActionResult> Insert(string woId, IEnumerable<BillDataViewModel> testArr, IEnumerable<SettingsViewModel> testArrSettings)
        {
            MippPortalWebAPI.Models.Bill bill = new MippPortalWebAPI.Models.Bill();
            var clientID = _context1.Workorders.FirstOrDefault(x => x.Id == int.Parse(woId)).ClientId;
            var vendorID = _context1.Workorders.FirstOrDefault(x => x.Id == int.Parse(woId)).VendorId;
            MippPortalWebAPI.Helpers.WorkorderRequest workorderRequest = new MippPortalWebAPI.Helpers.WorkorderRequest();
            workorderRequest.ClientID = clientID;
            string clientEmail = "";



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

            using (var httpClient2 = new HttpClient())
            {
                StringContent content2 = new StringContent(JsonConvert.SerializeObject(workorderRequest), Encoding.UTF8, "application/json");

                using (var response2 = await httpClient2.PostAsync(apiUrl + "Bills/GetClientEmail", content2))
                {
                    clientEmail = await response2.Content.ReadAsStringAsync();
                }
            }


            foreach (var item in testArr)
            {
                MippPortalWebAPI.Models.BillItem billItem = new MippPortalWebAPI.Models.BillItem();
                billItem.Name = item.item;
                billItem.Description = item.description;
                billItem.Quantity = item.quantity;
                billItem.Unit = item.unit;
                billItem.Quantity = item.quantity;
                billItem.Price = item.price;
                billItem.Tax = item.tax;
                billItem.Total = item.total;
                billItem.Subtotal = item.subtotal;
                billItem.BillId = "1";

                using (var httpClient1 = new HttpClient())
                {
                    StringContent content1 = new StringContent(JsonConvert.SerializeObject(billItem), Encoding.UTF8, "application/json");

                    using (var response1 = await httpClient1.PostAsync(apiUrl + "Bills/AddBillItems", content1))
                    {
                        string apiResponse1 = await response1.Content.ReadAsStringAsync();
                        try
                        {
                            foreach (var item1 in testArrSettings)
                            {
                                string Id = "1";
                                bill.AddressLine1 = item1.AddressLine1;
                                bill.AddressLine2 = item1.AddressLine2;
                                bill.AddressLine3 = item1.City;
                                bill.City = item1.City;
                                bill.Province = item1.Province;
                                bill.Wonumber = woId;
                                bill.CareOf = item1.CareOf;
                                bill.BillTo = item1.BusinessName;
                                bill.BillDate = item1.BillDate;
                                bill.ClientId = _context1.Workorders.FirstOrDefault(x => x.Id == int.Parse(woId)).ClientId.ToString();
                                bill.ClientEmail = clientEmail;
                                bill.SubTotal = item1.Subtotal;
                                bill.TaxAmount = item1.Tax;
                                bill.Total = item1.Total;
                                bill.BillItemId = _context1.BillItems.FirstOrDefault(x => x.BillId == Id).Id.ToString(); ;
                                bill.Summary = item1.Summary;
                                bill.Title= item1.Title;
                                bill.PaymentDueOn = item1.DueDate;
                                bill.Note = item1.Note;
                                bill.InvoiceDate = item1.BillDate;
                                bill.VendorId = _context1.Workorders.FirstOrDefault(x => x.Id == int.Parse(woId)).VendorId.ToString();
                                bill.VendorEmail = _context.Vendors.FirstOrDefault(x => x.Id == int.Parse(bill.VendorId)).VendorEmail;
                                //bill.Documents
                                bill.Footer = item1.Footer;

                                using (var httpClient2 = new HttpClient())
                                {
                                    StringContent content2 = new StringContent(JsonConvert.SerializeObject(bill), Encoding.UTF8, "application/json");

                                    using (var response2 = await httpClient2.PostAsync(apiUrl + "Bills/AddBill", content2))
                                    {
                                        string apiResponse2 = await response2.Content.ReadAsStringAsync();
                                        try
                                        { }
                                        catch { }




                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }


               
            }
            workorderRequest.Id = int.Parse(woId);
            workorderRequest.Status = "Bill Created";
            using (var httpClient3 = new HttpClient())
            {
                StringContent content3 = new StringContent(JsonConvert.SerializeObject(workorderRequest), Encoding.UTF8, "application/json");

                using (var response3 = await httpClient3.PostAsync(apiUrl + "Workorders/UpdateWorkorderStatus", content3))
                {
                    var apiResponse3 = await response3.Content.ReadAsStringAsync();
                    if(apiResponse3 == "true")
                    {
                        using (var httpClient4 = new HttpClient())
                        {
                            StringContent content4 = new StringContent(JsonConvert.SerializeObject(bill), Encoding.UTF8, "application/json");

                            using (var response4 = await httpClient4.PostAsync(apiUrl + "SendEmail/SendBillCreatedEmail", content4))
                            {
                                string apiResponse4 = await response4.Content.ReadAsStringAsync();
                                //receivedReservation = JsonConvert.DeserializeObject<Reservation>(apiResponse);
                                //return true;
                                if (response4.IsSuccessStatusCode)
                                    return RedirectToAction("Index", new { rootVendorId = vendorID});
                            }
                        }
                    }
                }
            }





            return Ok();

        }

        
        // GET: BillSettings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Settings == null)
            {
                return NotFound();
            }

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
            BillRequest billRequest = new BillRequest();
            billRequest.Id = id;

            MippPortalWebAPI.Models.Bill bill = new MippPortalWebAPI.Models.Bill(); 
            using (var httpClient2 = new HttpClient())
            {
                StringContent content2 = new StringContent(JsonConvert.SerializeObject(billRequest), Encoding.UTF8, "application/json");

                using (var response2 = await httpClient2.PostAsync(apiUrl + "Bills/GetBillDetails", content2))
                {
                    var apiResponse = await response2.Content.ReadAsStringAsync();
                    bill = JsonConvert.DeserializeObject<MippPortalWebAPI.Models.Bill>(apiResponse);
                }
            }
            if(bill != null)
                return View(bill);
            return NotFound();
        }

        // GET: BillSettings/Create
        public IActionResult Create()
        {
            return View();
        }

     

        // POST: BillSettings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (_context.Settings == null)
            {
                return Problem("Entity set 'VendordbContext.BillSettingsViewModel'  is null.");
            }
            var billSettingsViewModel = await _context.Settings.FindAsync(id);
            if (billSettingsViewModel != null)
            {
                _context.Settings.Remove(billSettingsViewModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BillSettingsViewModelExists(int? id)
        {
            return (_context.Settings?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
