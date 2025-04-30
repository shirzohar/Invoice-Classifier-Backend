using Microsoft.AspNetCore.Mvc;
using BusuMatchProject.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BusuMatchProject.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReportsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Missing user ID in token");

            var expenses = await _context.Expenses
                .Where(e => e.UserId == userId)
                .ToListAsync();

            var byCategory = expenses
                .GroupBy(e => e.Category)
                .Select(g => new
                {
                    category = g.Key ?? "ללא קטגוריה",
                    total = g.Sum(e => e.TotalWithVat ?? 0)
                })
                .ToList();

            var byMonth = expenses
                .Where(e => e.InvoiceDate.HasValue)
                .GroupBy(e => e.InvoiceDate!.Value.Month)
                .Select(g => new
                {
                    month = g.Key,
                    total = g.Sum(e => e.TotalWithVat ?? 0)
                })
                .ToList();

            return Ok(new { byCategory, byMonth });
        }
    }
}
