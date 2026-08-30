// Models/CalculationPosition.cs
namespace YYCount.Models
{
    public class CalculationPosition
    {
        public int Id { get; set; }
        public string EquipmentName { get; set; } // копия на момент расчёта
        public double Quantity { get; set; }
        public double UnitValue { get; set; }     // копия на момент расчёта
        public double Total => Quantity * UnitValue;
    }
}