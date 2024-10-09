namespace Labys.DTO
{
    public class InvoiceResponseDTO
    {
        public int InvoiceId { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public int MaintenanceCost { get; set; }
        public DateTime Date { get; set; }
        public decimal WeightOfPiece { get; set; }
        public int NumberOfPiece { get; set; }
        public string Notice { get; set; }
        public string BranchName { get; set; }
        public string ServiceType { get; set; }
        public string AgreedDuration { get; set; }
        public string ImageBase64 { get; set; }
    }
}
