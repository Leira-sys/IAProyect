namespace ApíRecognizer.Models
{
    public class Receipt
    {
        public string MerchantName { get; set; } = string.Empty;
        public string ReceiptNumber { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public string MerchantAddress {get;set;} = string.Empty;
    }
}
