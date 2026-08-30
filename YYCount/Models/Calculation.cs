namespace YYCount.Models;

public class Calculation
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime Date { get; set; }
    public List<CalculationPosition> Positions { get; set; } = new();
    public double TotalSum => Positions.Sum(p => p.Total);
}