using Microsoft.AspNetCore.Mvc;
using BusuMatchProject.Data;
using BusuMatchProject.Models;
using Microsoft.EntityFrameworkCore;

namespace BusuMatchProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExpensesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ExpensesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetFilteredExpenses(
            string? name,
            string? category,
            decimal? min,
            decimal? max,
            DateTime? from,
            DateTime? to)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token");

            var query = _context.Expenses.Where(e => e.UserId == userId).AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(e => e.BusinessName != null && e.BusinessName.Contains(name));

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(e => e.Category == category);

            if (min.HasValue)
                query = query.Where(e => e.TotalWithVat.HasValue && e.TotalWithVat >= min);

            if (max.HasValue)
                query = query.Where(e => e.TotalWithVat.HasValue && e.TotalWithVat <= max);

            if (from.HasValue)
                query = query.Where(e => e.InvoiceDate.HasValue && e.InvoiceDate >= from);

            if (to.HasValue)
                query = query.Where(e => e.InvoiceDate.HasValue && e.InvoiceDate <= to);

            var results = await query.OrderByDescending(e => e.InvoiceDate ?? DateTime.MinValue).ToListAsync();
            return Ok(results);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null) return NotFound();

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExpense(int id, [FromBody] Expense updated)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null) return NotFound();

            expense.BusinessName = updated.BusinessName;
            expense.InvoiceDate = updated.InvoiceDate;
            expense.TotalBeforeVat = updated.TotalBeforeVat;
            expense.TotalWithVat = updated.TotalWithVat;
            expense.Category = updated.Category;

            await _context.SaveChangesAsync();
            return Ok(expense);
        }
    }
}
