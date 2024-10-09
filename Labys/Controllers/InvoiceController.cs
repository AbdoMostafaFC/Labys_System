using Labys.DTO;
using Labys.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;

namespace Labys.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InvoiceController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public InvoiceController(ApplicationDbContext context)
        {
            this.context = context;
        }
        
        [HttpPost]
        public async Task <IActionResult> AddInvoice([FromForm] InvoiceDTO invoiceDTO)
        {
            string imagePath = null;

            if (invoiceDTO.ProductImage != null)
            {
                
                var imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

                
                if (!Directory.Exists(imagesPath))
                {
                    Directory.CreateDirectory(imagesPath);  
                }

                
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(invoiceDTO.ProductImage.FileName);
                var filePath = Path.Combine(imagesPath, fileName);

                
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await invoiceDTO.ProductImage.CopyToAsync(stream);
                }

                
                imagePath = Path.Combine("images", fileName);
            }
            Invoice invoice = new Invoice()
            {
                Price = invoiceDTO.Price,
                Stauts = invoiceDTO.Stauts,
                SecondaryInvoiceID = invoiceDTO.SecondaryInvoiceID,
                CustomerName = invoiceDTO.CustomerName,
                PhoneNumber = invoiceDTO.PhoneNumber,
                MaintenanceCost = invoiceDTO.MaintenanceCost,
                DateOfMantanance=invoiceDTO.DateOfMantanance,
                WeightOfPiece = invoiceDTO.WeightOfPiece,
                NumberOfPiece = invoiceDTO.NumberOfPiece,
                Notice = invoiceDTO.Notice,
                BranchName = invoiceDTO.BranchName,
                ServiceType = invoiceDTO.ServiceType,
                AgreedDuration = invoiceDTO.AgreedDuration,
                ProductImage=imagePath,
                InvoiceType=invoiceDTO.InvoiceType,
                Address=invoiceDTO.Address,
                InitDate=DateTime.Now,
                MaintenanceCostType=invoiceDTO.MaintenanceCostType,
                EmployeeName=invoiceDTO.EmployeeName,

            };

            context.Invoices.Add(invoice);
            context.SaveChanges();


            return CreatedAtAction("GenerateInvoiceImage", new {id=invoice.InvoiceId },invoice);



        }

        [HttpGet("id")]
        public IActionResult GetBYID(int id)
        {

            var invoice = context.Invoices.FirstOrDefault(invo=>invo.InvoiceId == id);
            if (invoice != null)
            {
                invoice.ProductImage= $"{Request.Scheme}://{Request.Host}/{invoice.ProductImage}";
                return Ok(invoice);
            }
            return BadRequest("Invlaid ID!!!!");



        }
        [HttpGet("all")]
       public async Task<IActionResult> GetAll(
    int pageNumber = 1, 
    int pageSize = 10, 
    string? customerName = null, 
    string? phoneNumber = null, 
    string? branchName = null, 
    int? invoiceId = null, 
    string? secondaryInvoiceId = null)
{
    
    if (pageNumber <= 0) pageNumber = 1;
    if (pageSize <= 0 || pageSize > 100) pageSize = 10;

 
    var query = context.Invoices.AsQueryable();

    
    if (!string.IsNullOrWhiteSpace(customerName))
    {
        query = query.Where(inv => inv.CustomerName.Contains(customerName));
    }

    if (!string.IsNullOrWhiteSpace(phoneNumber))
    {
        query = query.Where(inv => inv.PhoneNumber.Contains(phoneNumber));
    }

    if (!string.IsNullOrWhiteSpace(branchName))
    {
        query = query.Where(inv => inv.BranchName.Contains(branchName));
    }

    
    if (invoiceId.HasValue)
    {
        query = query.Where(inv => inv.InvoiceId == invoiceId.Value);
    }

    if (!string.IsNullOrWhiteSpace(secondaryInvoiceId))
    {
       query = query.Where(inv => inv.SecondaryInvoiceID.Contains(secondaryInvoiceId));
    }

            
            var totalRecords = await query.CountAsync();

 
    var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

    
    var invoices = await query
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

   
    foreach (var invoice in invoices)
    {
        invoice.ProductImage = $"{Request.Scheme}://{Request.Host}/{invoice.ProductImage}";
    }

    
    return Ok(new
    {
        totalRecords,
        totalPages,
        currentPage = pageNumber,
        pageSize,
        data = invoices
    });
}


        [HttpGet("count")]
        public IActionResult GetCount()
        {
            var count = context.Invoices.Count();


            return Ok(count);


        }

         [HttpPatch]
        public IActionResult update(int id,InvoiceDTO invoiceDTO)
        {
            if(id == null || id==0)
            {
                return BadRequest("Inavlide ID");

            }
            else
            {
                var invoice= context.Invoices.FirstOrDefault(invo=>invo.InvoiceId==id);
                if(invoice != null)
                {
                    invoice.AgreedDuration= invoiceDTO.AgreedDuration;
                    invoice.PhoneNumber= invoiceDTO.PhoneNumber;
                    invoice.BranchName= invoiceDTO.BranchName;
                    invoice.CustomerName= invoiceDTO.CustomerName;
                    invoice.NumberOfPiece= invoiceDTO.NumberOfPiece;
                    invoice.WeightOfPiece= invoiceDTO.WeightOfPiece;
                    invoice.Notice= invoiceDTO.Notice;
                    
                    invoice.MaintenanceCost= invoiceDTO.MaintenanceCost;
                    invoice.DateOfMantanance= invoiceDTO.DateOfMantanance;
                    invoice.Price= invoiceDTO.Price;
                    invoice.InvoiceType= invoiceDTO.InvoiceType;
                    invoice.Stauts= invoiceDTO.Stauts;
                    invoice.Address = invoiceDTO.Address;
                    invoice.SecondaryInvoiceID= invoiceDTO.SecondaryInvoiceID;
                    invoice.ServiceType= invoiceDTO.ServiceType;
                    invoice.MaintenanceCostType= invoiceDTO.MaintenanceCostType;
                    invoice.EmployeeName= invoiceDTO.EmployeeName;
                    
                    context.Invoices.Update(invoice);
                    context.SaveChanges();
                    return NoContent();
                }

            }
            return BadRequest("invlide ID");

        }
        [HttpGet("ByBranchName")]
        public async Task< IActionResult> FilterBYbranch(string branchName, int pageNumber = 1, int pageSize = 10)
        {

            
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0 || pageSize > 100) pageSize = 10;

           
            var query = context.Invoices.Where(inv => inv.BranchName == branchName);

           
            var totalRecords = await query.CountAsync();

           
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            
            var invoices = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

           
            if (invoices.Count == 0)
            {
                return Ok("No invoices available.");
            }

            
            return Ok(new
            {
                totalRecords,
                totalPages,
                currentPage = pageNumber,
                pageSize,
                data = invoices
            });


        }
        [HttpDelete]
        public IActionResult delete(int id)
        {
            if(id==null || id==0)
            {
                return BadRequest("invlaide ID");

            }
            else
            {
                var invoce=context.Invoices.FirstOrDefault(invo=>invo.InvoiceId==id);
                if (invoce!=null)
                {
                context.Invoices.Remove(invoce);
                    context.SaveChanges();
                    return Ok("Deleted");
                }
                return BadRequest("invice  id not exist in database");
            }




        }



        [HttpGet("invoice-image/{id}")]
        public IActionResult GenerateInvoiceImage(int id)
        {
            // Fetch the invoice from the database
            var invoice = context.Invoices.FirstOrDefault(invo => invo.InvoiceId == id);

            if (invoice == null)
            {
                return BadRequest("Invalid invoice ID.");
            }

            // Set image dimensions
            var imageWidth = 600;
            var imageHeight = 850;  // Adjust height to accommodate all data

            using (var surface = SKSurface.Create(new SKImageInfo(imageWidth, imageHeight)))
            {
                var canvas = surface.Canvas;
                canvas.Clear(SKColors.White); // Set background color to white

                var paint = new SKPaint
                {
                    Color = SKColors.Black,
                    TextSize = 24,
                    IsAntialias = true
                };

                // Set text coordinates and spacing between rows
                float x = 50;
                float y = 50;
                float lineSpacing = 50;

                // Draw invoice details on the image
                canvas.DrawText($"Invoice ID: {invoice.InvoiceId}", x, y, paint);
                y += lineSpacing;
                canvas.DrawText($"Customer Name: {invoice.CustomerName}", x, y, paint);
                y += lineSpacing;
                canvas.DrawText($"Phone Number: {invoice.PhoneNumber}", x, y, paint);
                y += lineSpacing;
                canvas.DrawText($"Maintenance Cost: {invoice.MaintenanceCost} EGP", x, y, paint);
                y += lineSpacing;
                canvas.DrawText($"Date: {invoice.DateOfMantanance.ToShortDateString()}", x, y, paint);
                y += lineSpacing;
                canvas.DrawText($"Weight of Piece: {invoice.WeightOfPiece} kg", x, y, paint);
                y += lineSpacing;
                canvas.DrawText($"Number of Pieces: {invoice.NumberOfPiece}", x, y, paint);
                y += lineSpacing;
                canvas.DrawText($"Branch: {invoice.BranchName}", x, y, paint);
                y += lineSpacing;
                canvas.DrawText($"Service Type: {invoice.ServiceType}", x, y, paint);
                y += lineSpacing;
                canvas.DrawText($"Agreed Duration: {invoice.AgreedDuration}", x, y, paint);
                y += lineSpacing;

                // Draw a separator line before notice
                var linePaint = new SKPaint
                {
                    Color = SKColors.Gray,
                    StrokeWidth = 2
                };
                canvas.DrawLine(x, y + 10, imageWidth - 50, y + 10, linePaint);
                y += lineSpacing;

                // Draw the notice
                canvas.DrawText($"Notice: {invoice.Notice}", x, y, paint);
                y += lineSpacing;

                // Draw product image if available
                if (!string.IsNullOrEmpty(invoice.ProductImage))
                {
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", invoice.ProductImage);
                    if (System.IO.File.Exists(imagePath))
                    {
                        using (var skImage = SKBitmap.Decode(imagePath))
                        {
                            // Resize and draw the image
                            var imageRect = new SKRect(x, y, x + 150, y + 150); // 150x150 image size
                            canvas.DrawBitmap(skImage, imageRect);
                        }
                    }
                }

                // Save the canvas as an image and return it
                using (var image = surface.Snapshot())
                using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                {
                    var imageBytes = data.ToArray();

                    // Return the image as a file result
                    return File(imageBytes, "image/png", "invoice.png");
                }
            }
        }

    }
}
