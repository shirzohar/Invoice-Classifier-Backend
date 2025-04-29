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
        // Returns a filtered list of expenses based on query parameters
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Expense>>> GetExpenses(
            string? businessName,
            string? category,
            decimal? minTotal,
            decimal? maxTotal,
            DateTime? fromDate,
            DateTime? toDate)
        {
            // Start with a queryable collection of all expenses
            var query = _context.Expenses.AsQueryable();

            // Filter by business name (contains, case-sensitive)
            if (!string.IsNullOrWhiteSpace(businessName))
                query = query.Where(e => e.BusinessName.Contains(businessName));

            // Filter by exact category match
            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(e => e.Category == category);

            // Filter by minimum total amount
            if (minTotal.HasValue)
                query = query.Where(e => e.TotalWithVat >= minTotal);

            // Filter by maximum total amount
            if (maxTotal.HasValue)
                query = query.Where(e => e.TotalWithVat <= maxTotal);

            // Filter by minimum invoice date
            if (fromDate.HasValue)
                query = query.Where(e => e.InvoiceDate >= fromDate);

            // Filter by maximum invoice date
            if (toDate.HasValue)
                query = query.Where(e => e.InvoiceDate <= toDate);

            // Order results by date descending and execute the query
            return await query.OrderByDescending(e => e.InvoiceDate).ToListAsync();
        }

        // POST: /api/expensese
        // Adds a new expense to the database
        [HttpPost]
        public async Task<IActionResult> AddExpense([FromBody] Expense expense)
        {
            // Add the new expense to the context
            _context.Expenses.Add(expense);

            // Save changes to the database asynchronously
            await _context.SaveChangesAsync();

            // Return 201 Created with location of the new resource
            return CreatedAtAction(nameof(GetExpenses), new { id = expense.Id }, expense);
        }
    }
}
