namespace BarnameNevis1401.Data.Entities;

public class Product:BaseEntity
{
  public string Name { get; set; }
  public int Price { get; set; }
  public int Discount { get; set; }

  public int FinalPrice => Price - (Price * Discount / 100);
}