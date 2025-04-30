using System;
using System.ComponentModel.DataAnnotations;

namespace BusuMatchProject.Models
{
    public class Expense
    {
        [Key]
        public int Id { get; set; }

        public string? BusinessName { get; set; }

        public string? InvoiceNumber { get; set; }

        public string? BusinessId { get; set; }

        public string? DocumentType { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public decimal? TotalBeforeVat { get; set; }

        public decimal? TotalWithVat { get; set; }

        public string? Category { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? UserId { get; set; }
    }
}
