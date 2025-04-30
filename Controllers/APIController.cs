using Microsoft.AspNetCore.Mvc;
using BusuMatchProject.Models;
using Microsoft.EntityFrameworkCore;
using BusuMatchProject.Data;

namespace BusuMatchProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseseController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ExpenseseController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /api/expensese
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Expense>>> GetExpenses(
            string? businessName,
            string? category,
            decimal? minTotal,
            decimal? maxTotal,
            DateTime? fromDate,
            DateTime? toDate)
        {
            var query = _context.Expenses.AsQueryable();

            if (!string.IsNullOrWhiteSpace(businessName))
                query = query.Where(e => e.BusinessName != null && e.BusinessName.Contains(businessName));

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(e => e.Category == category);

            if (minTotal.HasValue)
                query = query.Where(e => e.TotalWithVat.HasValue && e.TotalWithVat >= minTotal);

            if (maxTotal.HasValue)
                query = query.Where(e => e.TotalWithVat.HasValue && e.TotalWithVat <= maxTotal);

            if (fromDate.HasValue)
                query = query.Where(e => e.InvoiceDate.HasValue && e.InvoiceDate >= fromDate);

            if (toDate.HasValue)
                query = query.Where(e => e.InvoiceDate.HasValue && e.InvoiceDate <= toDate);

            return await query
                .OrderByDescending(e => e.InvoiceDate ?? DateTime.MinValue)
                .ToListAsync();
        }

        // POST: /api/expensese
        [HttpPost]
        public async Task<IActionResult> AddExpense([FromBody] Expense expense)
        {
            if (expense == null)
                return BadRequest("Expense is null");

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetExpenses), new { id = expense.Id }, expense);
        }
    }
}
