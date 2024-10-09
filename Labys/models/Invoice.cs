using Microsoft.EntityFrameworkCore;

namespace Labys.models
{
    public class Invoice
    {
        
        public int InvoiceId { get; set; }
        public string? SecondaryInvoiceID { get; set; }
        public decimal? Price { get; set; }
        public string ? Stauts { get; set; }
        public string ? CustomerName { get; set; }
        public string? Address { get; set; }
        public DateTime DateOfMantanance { get; set; }
        public DateTime InitDate { get; set; }
        public decimal ? MaintenanceCost { get; set; }
        public string ?PhoneNumber { get; set; }
        public string ?ProductImage { get; set; }  
        public string ?AgreedDuration { get; set; }
        public decimal? WeightOfPiece { get; set; }
        public int? NumberOfPiece { get; set; }
        public string ?Notice { get; set; }
        public string ?BranchName { get; set; }
        public string? ServiceType { get; set; }
        public int? InvoiceType { get; set; }
        public string? MaintenanceCostType { get; set; }
        public string? EmployeeName { get; set; }
       
    }
}
