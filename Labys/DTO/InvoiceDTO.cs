namespace Labys.DTO
{
    public class InvoiceDTO
    {
        
        public string? SecondaryInvoiceID { get; set; }
        public decimal Price { get; set; }
        public string? Stauts { get; set; }
        public string? Address { get; set; }
        public string? CustomerName { get; set; }
    public decimal MaintenanceCost { get; set; }
    public string? PhoneNumber { get; set; }
    public IFormFile? ProductImage { get; set; }  // Image file from form-data
    public string? AgreedDuration { get; set; }
    public decimal WeightOfPiece { get; set; }
    public int NumberOfPiece { get; set; }
    public string? Notice { get; set; }
    public string? BranchName { get; set; }
    public string? ServiceType { get; set; }
    public DateTime DateOfMantanance { get; set; }
     public int InvoiceType { get; set; }
     public string? MaintenanceCostType { get; set; }
     public string? EmployeeName { get; set; }



    }
}
