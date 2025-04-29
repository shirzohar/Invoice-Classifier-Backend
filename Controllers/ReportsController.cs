using Microsoft.AspNetCore.Mvc;
using BusuMatchProject.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BusuMatchProject.Controllers
{
    [Authorize] // Only allow authenticated users
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReportsController(AppDbContext context)
        {
            _context = context;
        }

        // Endpoint: GET /api/Reports/summary
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            // Extract user ID from JWT token
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Missing user ID in token");

            // Fetch all expenses belonging to the current user
            var expenses = await _context.Expenses
                .Where(e => e.UserId == userId)
                .ToListAsync();

            // Group expenses by category and sum the totals
            var byCategory = expenses
                .GroupBy(e => e.Category)
                .Select(g => new
                {
                    category = g.Key ?? "ללא קטגוריה", // Handle nulls
                    total = g.Sum(e => e.TotalWithVat ?? 0)
                })
                .ToList();

            // Group expenses by month and sum the totals
            var byMonth = expenses
                .Where(e => e.InvoiceDate.HasValue)
                .GroupBy(e => e.InvoiceDate.Value.Month)
                .Select(g => new
                {
                    month = g.Key,
                    total = g.Sum(e => e.TotalWithVat ?? 0)
                })
                .ToList();

            // Return both summaries in a single response
            return Ok(new { byCategory, byMonth });
        }
    }
}
