namespace ApíRecognizer.Models
{
  public class Invoice
  {
    public string Id { get; set; } = string.Empty;
    public  Data CustomerData { get; set; } = new Data();
    public  Data VendorData { get; set; } = new Data();
    public string InvoiceNumber { get; set; } = string.Empty;
    public List<Product> Products { get; set; }
    public double Total { get; set; }

    public Invoice()
        {
              Products = new List<Product>();
        }
    }
  public class Data
  {
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
  }
  public class Product
  { 
    public double Quantity { get; set; }
    public double Price { get; set; }
    public string Name { get; set; } = string.Empty;

  }
}
