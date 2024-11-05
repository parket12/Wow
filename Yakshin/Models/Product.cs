using Yakshin.Models;

public record class Product
{
    public int ID { get; set; }
    public string NameOfProduct { get; set; }
    public decimal Cost { get; set; }
    public int ID_ProductCategory { get; set; }
    public string img { get; set; }
}

