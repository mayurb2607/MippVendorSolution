using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MippPortalWebAPI.Helpers;
using MippPortalWebAPI.Models;
using MippVendorPortal.Models;
using MippVendorPortal.ViewModel;
using Newtonsoft.Json;

namespace MippVendorPortal.Controllers
{
    public class BillsController : Controller
    {
        private readonly MippVendorTestContext _context;
        private readonly MippTestContext _context1;

        public BillsController(MippVendorTestContext context, MippTestContext context1)
        {
            _context = context;
            _context1 = context1;
        }



        [HttpPost("Upload")]
        public async Task<IActionResult> Upload()
        {
            int clientId = 1;
            var files = Request.Form.Files; // Access the uploaded files here
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
                        await blobClient.UploadAsync(fileStream, true);
                    }
                    // Get the public URL of the uploaded image
                    var fileUrl = blobClient.Uri.ToString();

                    // Optionally, you can delete the local file after uploading it to Azure Blob Storage
                    System.IO.File.Delete(file.FileName);

                    return RedirectToAction("Index", new { clientID = clientId });


                }
            }

            // Process the uploaded files

            return RedirectToAction("Bills", new { clientID = clientId });
        }

        public IActionResult OverlayPartial()
        {
            return PartialView("_CommentsView");
            //return PartialView("_FeedbackPartial");
        }

        public async Task<IActionResult> Index(int vendorID)
        {
            try
            {
                Setting settings = _context.Settings.FirstOrDefault(x => x.VendorId == vendorID);

                List<Setting> lst = new List<Setting>();
                lst.Add(settings);
                //DateTime dt = DateTime.ParseExact(settings.BillDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //DateTime dt2 = DateTime.ParseExact(settings.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                //settings.BillDate = dt;
                //settings.DueDate = dt2;
                IEnumerable<Setting> enumerable = lst;
                List<string> taxList = new List<string>();
                var taxes = _context1.Taxes.Where(x => x.ClientId == _context.VendorClients.FirstOrDefault(x => x.VendorId == vendorID).ClientId);
                foreach (var tax in taxes)
                {
                    taxList.Add(tax.TaxRate);
                }
                ViewBag.taxList = taxList;
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
            WorkorderRequest workorderRequest = new WorkorderRequest();
            workorderRequest.VendorID = vendorId;
            workorderRequest.Status = "";
            workorderRequest.AdditionalComments = "";

            IEnumerable<Bill> bill;
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(workorderRequest), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync("https://localhost:7026/api/Bills/GetBills", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    try
                    {
                        bill = JsonConvert.DeserializeObject<IEnumerable<Bill>>(apiResponse);
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
        public async Task<IActionResult> Insert(string item, string description, string quantity, int unit, string price, string tax, string subtotal, string total,
            string title,
            string summary,
            string invoice,
            string billTo,
            string careOf,
            string woId,
            string addressLine,
            string addressLine2,
            string city,
            string province,
            string zip,
            string billDate,
            string dueDate,
            int clientId)
        {
            clientId = 1;
            Setting setting = _context.Settings.FirstOrDefault(x => x.VendorId == _context.VendorClients.FirstOrDefault(x=> x.ClientId== clientId).VendorId);
            setting.AddressLine1 = addressLine;
            setting.AddressLine2 = addressLine2;
            setting.City = city;
            setting.BillDate = billDate;
            setting.DueDate = dueDate;
            setting.BusinessName = billTo;
            setting.CareOf = careOf;
            setting.Province = province;
            setting.Zip = zip;
            _context.SaveChanges();

            return Ok();

        }

        [HttpPost]

        public async Task<IActionResult> InsertBillData(List<BillDataViewModel> billDataList)
        {
            foreach (var item in billDataList)
            {
                var existingBillId = _context1.Bills.FirstOrDefault(x => x.ClientId == item.clientId.ToString());
                BillItem billItems = new BillItem();
                if (billItems != null)
                {
                    billItems.Total = item.total;
                    billItems.Name = item.item;
                    billItems.Description = item.description;
                    billItems.Quantity = item.quantity;
                    billItems.Unit = item.unit;
                    billItems.Price = item.price;
                    billItems.Subtotal = item.subtotal;

                    // Add record in BillItem
                    // Add record in bills with corresponding billItemID
                    var billcount = _context1.Bills.Count() + 1;
                    var billItemCount = _context1.BillItems.Count() + 1;

                    billItems.BillId = existingBillId.Id.ToString();
                    billItems.Id = _context1.BillItems.Count() + 1;
                    Bill bill = new Bill();
                    bill.ClientId = billItems.Id.ToString();
                    bill.Id = billcount;

                    Bill bill1 = new Bill
                    {
                        Title = item.title,
                        Summary = item.summary,
                        AddressLine1 = item.addressLine,
                        AddressLine2 = item.addressLine2,
                        AddressLine3 = item.city,
                        BillDate = item.billDate,
                        InvoiceDate = item.dueDate,
                        BillTo = item.billTo,
                        CareOf = item.careOf,
                        Province = item.province,
                        Zip = item.zip,
                        BillItemId = billItems.Id.ToString(),
                        SubTotal = item.subtotal,
                        TaxAmount = item.tax,
                        Total = item.total,
                        Wonumber = item.invoice

                    };
                    _context1.Bills.Add(bill1);
                    _context1.BillItems.Add(billItems);

                    await _context1.SaveChangesAsync();
                    return Ok();


                }
                return Ok();


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

            var bill = await _context1.Bills
                .FirstOrDefaultAsync(m => m.Id
                == id);
            if (bill == null)
            {
                return NotFound();
            }
            var cid = int.Parse(bill.ClientId);
            var client = _context1.Clients.FirstOrDefault(x => x.ClientId == cid);
            var vid = int.Parse(bill.VendorId);
            var vendor = _context1.Vendors.FirstOrDefault(x => x.Id == vid);

            var billrecords = _context1.BillItems.Where(x => x.BillId == bill.Id.ToString()).AsEnumerable();

            ViewBag.VendorAddressLine1 = vendor.FirstName + vendor.LastName;
            ViewBag.VendorPhone = vendor.BusinessName;
            ViewBag.VendorEmail = vendor.Email;

            ViewBag.CareOf = bill.CareOf;
            ViewBag.ClientAddressLine1 = bill.AddressLine1;
            ViewBag.ClientAddressLine2 = bill.AddressLine2;
            ViewBag.ClientPhone = "";
            ViewBag.ClientEmail = client.Email;

            var subtotal = 0.00;
            var tax = 0.00;
            var total = 0.00;
            foreach (var item in billrecords)
            {
                subtotal = subtotal + float.Parse(item.Subtotal);
                tax = tax + float.Parse(item.Total) - float.Parse(item.Subtotal);
                total = total + float.Parse(item.Total);
            }
            ViewBag.Subtotal = (float)Math.Round(subtotal, 2); ;
            ViewBag.Tax = (float)Math.Round(tax, 2); ;
            ViewBag.Total = (float)Math.Round(total, 2); ;


            return View(billrecords);
            //billItems
        }

        // GET: BillSettings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BillSettings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClientId")] BillSettingsViewModel billSettingsViewModel)
        {
            if (billSettingsViewModel is null)
            {
                throw new ArgumentNullException(nameof(billSettingsViewModel));
            }
            Setting setting = new Setting();
            setting.VendorId = billSettingsViewModel.settings.VendorId;
            setting.AddressLine1 = billSettingsViewModel.settings.Address;
            setting.AddressLine2 = billSettingsViewModel.settings.Address2;
            setting.BusinessName = billSettingsViewModel.settings.BusinessName;
            setting.CareOf = billSettingsViewModel.settings.CareOf;
            setting.Email = billSettingsViewModel.settings.Email;
            setting.City = billSettingsViewModel.settings.City;
            setting.Province = billSettingsViewModel.settings.Province;
            setting.Zip = billSettingsViewModel.settings.Zip;
            setting.Phone = billSettingsViewModel.settings.Phone;
            setting.BillDate = billSettingsViewModel.settings.BillDate;
            setting.DueDate = billSettingsViewModel.settings.DueDate;

            if (ModelState.IsValid)
            {
                _context.Settings.Add(setting);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(billSettingsViewModel);
        }

        // GET: BillSettings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Settings == null)
            {
                return NotFound();
            }

            var billSettingsViewModel = await _context.Settings.FindAsync(id);
            if (billSettingsViewModel == null)
            {
                return NotFound();
            }
            return View(billSettingsViewModel);
        }

        // POST: BillSettings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("ClientId")] BillSettingsViewModel billSettingsViewModel)
        {
            if (id != billSettingsViewModel.ClientId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(billSettingsViewModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BillSettingsViewModelExists(billSettingsViewModel.ClientId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(billSettingsViewModel);
        }

        // GET: BillSettings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Settings == null)
            {
                return NotFound();
            }

            var billSettingsViewModel = await _context.Settings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (billSettingsViewModel == null)
            {
                return NotFound();
            }

            return View(billSettingsViewModel);
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
