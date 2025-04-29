using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BusuMatchProject.Services;
using BusuMatchProject.Models;
using BusuMatchProject.Data;

namespace BusuMatchProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UploadController(AppDbContext context)
        {
            _context = context;
        }

        // Endpoint for authenticated users to upload an invoice file
        [Authorize]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            // Validate file
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            // Ensure Uploads directory exists
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            // Save uploaded file locally
            var filePath = Path.Combine(uploadsPath, file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            try
            {
                // Run OCR on the uploaded file
                var ocr = new OcrService();
                var text = ocr.ExtractTextFromFile(filePath);
                if (string.IsNullOrWhiteSpace(text))
                    return StatusCode(500, "OCR returned empty result");

                // Parse structured data from raw OCR text
                var parsed = InvoiceParser.Parse(text);
                if (parsed == null)
                    return StatusCode(500, "Failed to parse invoice");

                // Extract current user's ID from JWT token
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                // Map parsed invoice fields to Expense entity
                var expense = new Expense
                {
                    BusinessName = parsed.BusinessName ?? "לא ידוע",
                    BusinessId = parsed.BusinessId ?? "",
                    InvoiceNumber = string.IsNullOrWhiteSpace(parsed.InvoiceNumber) ? "לא צויין" : parsed.InvoiceNumber,
                    InvoiceDate = DateTime.TryParse(parsed.InvoiceDate, out var date) ? date : (DateTime?)null,
                    DocumentType = parsed.DocumentType ?? "לא מוגדר",
                    TotalBeforeVat = decimal.TryParse(parsed.TotalBeforeVat, out var beforeVat) ? beforeVat : (decimal?)null,
                    TotalWithVat = decimal.TryParse(parsed.TotalWithVat, out var withVat) ? withVat : (decimal?)null,
                    Category = string.IsNullOrWhiteSpace(parsed.Category) ? "לא מסווג" : parsed.Category,
                    CreatedAt = DateTime.UtcNow,
                    UserId = userId //  Link expense to the logged-in user
                };

                // Save expense to database
                _context.Expenses.Add(expense);
                await _context.SaveChangesAsync();

                return Ok(expense);
            }
            catch (Exception ex)
            {
                // Catch all errors from OCR, parsing or DB and return a generic error
                return StatusCode(500, $"OCR or Parsing failed: {ex.InnerException?.Message ?? ex.Message}");
            }
        }
    }
}
