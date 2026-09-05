namespace YYCount.Models
{
    public class TemplateSettings
    {
        public string TemplatePath { get; set; } = "";
        public string NameTag { get; set; } = "{Name}";
        public string SumTag { get; set; } = "{Sum}";
        public string TableStartTag { get; set; } = "{TableStart}";
        public string IdTag { get; set; } = "{Id}";
        public string EquipmentNameTag { get; set; } = "{EquipmentName}";
        public string QuantityTag { get; set; } = "{Quantity}";
        public string UnitValueTag { get; set; } = "{UnitValue}";
        public string TotalTag { get; set; } = "{Total}";
    }
}