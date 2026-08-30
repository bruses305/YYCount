using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using YYCount.Models;

namespace YYCount.Services
{
    public class JsonDataService : IDataService
    {
        private readonly string _equipmentPath;
        private readonly string _calculationsPath;

        public JsonDataService()
        {
            var appData = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "CalculatorApp");
            Directory.CreateDirectory(appData);
            _equipmentPath = Path.Combine(appData, "equipment.json");
            _calculationsPath = Path.Combine(appData, "calculations.json");

            // Если файл оборудования отсутствует – создаём с тестовыми данными
            if (!File.Exists(_equipmentPath))
            {
                var defaultEquipment = new List<EquipmentItem>
                {
                    new EquipmentItem { Id = 1, Name = "Станок А", UnitValue = 2.5 },
                    new EquipmentItem { Id = 2, Name = "Станок Б", UnitValue = 1.8 },
                    new EquipmentItem { Id = 3, Name = "Конвейер", UnitValue = 0.7 },
                };
                SaveEquipment(defaultEquipment);
            }
        }

        public List<EquipmentItem> LoadEquipment()
        {
            if (!File.Exists(_equipmentPath))
                return new List<EquipmentItem>();
            var json = File.ReadAllText(_equipmentPath);
            return JsonSerializer.Deserialize<List<EquipmentItem>>(json) ?? new List<EquipmentItem>();
        }

        public void SaveEquipment(List<EquipmentItem> items)
        {
            var json = JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_equipmentPath, json);
        }

        public List<Calculation> LoadCalculations()
        {
            if (!File.Exists(_calculationsPath))
                return new List<Calculation>();
            var json = File.ReadAllText(_calculationsPath);
            return JsonSerializer.Deserialize<List<Calculation>>(json) ?? new List<Calculation>();
        }

        public void SaveCalculations(List<Calculation> calculations)
        {
            var json = JsonSerializer.Serialize(calculations, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_calculationsPath, json);
        }
    }
}