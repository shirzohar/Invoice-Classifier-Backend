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

        //  Get filtered expenses for the logged-in user
        [HttpGet]
        public async Task<IActionResult> GetFilteredExpenses(
            string? name,
            string? category,
            decimal? min,
            decimal? max,
            DateTime? from,
            DateTime? to)
        {
            // Extract user ID from JWT token
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            // Start query with only this user's expenses
            var query = _context.Expenses.Where(e => e.UserId == userId).AsQueryable();

            // Apply optional filters from query string
            if (!string.IsNullOrEmpty(name))
                query = query.Where(e => e.BusinessName.Contains(name));

            if (!string.IsNullOrEmpty(category))
                query = query.Where(e => e.Category == category);

            if (min.HasValue)
                query = query.Where(e => e.TotalWithVat >= min);

            if (max.HasValue)
                query = query.Where(e => e.TotalWithVat <= max);

            if (from.HasValue)
                query = query.Where(e => e.InvoiceDate >= from);

            if (to.HasValue)
                query = query.Where(e => e.InvoiceDate <= to);

            // Order results by date and return them
            var results = await query.OrderByDescending(e => e.InvoiceDate).ToListAsync();
            return Ok(results);
        }

        //  Delete an expense by ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            // Try to find the expense
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null) return NotFound();

            // Remove it from the database
            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();

            // Return success with no content
            return NoContent();
        }

        //  Update an expense by ID
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExpense(int id, [FromBody] Expense updated)
        {
            // Find the expense to update
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null) return NotFound();

            // Update its fields with the provided data
            expense.BusinessName = updated.BusinessName;
            expense.InvoiceDate = updated.InvoiceDate;
            expense.TotalBeforeVat = updated.TotalBeforeVat;
            expense.TotalWithVat = updated.TotalWithVat;
            expense.Category = updated.Category;

            // Save changes and return the updated object
            await _context.SaveChangesAsync();
            return Ok(expense);
        }
    }
}
