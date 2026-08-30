// Models/EquipmentItem.cs
namespace YYCount.Models
{
    public class EquipmentItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double UnitValue { get; set; } // условных установок на единицу
    }
}