namespace BusuMatchProject.Models
{
    public class InvoiceData
    {
        public string? BusinessName { get; set; }
        public string? BusinessId { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? InvoiceDate { get; set; }
        public string? TotalWithVat { get; set; }
        public string? TotalBeforeVat { get; set; }
        public string? DocumentType { get; set; }
        public string? Language { get; set; }
        public string? RawText { get; set; }
        public string? Category { get; set; }

    }
}
