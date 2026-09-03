using System;
using System.IO;
using System.Text.Json;

namespace YYCount.Services
{
    public static class AppSettings
    {
        private static readonly string SettingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "YYCount", "settings.json");

        public static string ExcelTemplatePath
        {
            get => GetValue<string>("ExcelTemplatePath");
            set => SetValue("ExcelTemplatePath", value);
        }

        public static int ExcelStartRow
        {
            get => GetValue<int>("ExcelStartRow", 33); // по умолчанию 33
            set => SetValue("ExcelStartRow", value);
        }

        public static int ExcelUnitColumn
        {
            get => GetValue<int>("ExcelUnitColumn", 17); // по умолчанию 17 (Q)
            set => SetValue("ExcelUnitColumn", value);
        }

        private static T GetValue<T>(string key, T defaultValue = default)
        {
            try
            {
                if (!File.Exists(SettingsPath)) return defaultValue;
                var json = File.ReadAllText(SettingsPath);
                var settings = JsonSerializer.Deserialize<SettingsData>(json);
                if (settings == null) return defaultValue;
                var prop = typeof(SettingsData).GetProperty(key);
                if (prop == null) return defaultValue;
                var val = prop.GetValue(settings);
                return val is T t ? t : defaultValue;
            }
            catch { return defaultValue; }
        }

        private static void SetValue<T>(string key, T value)
        {
            try
            {
                SettingsData settings;
                if (File.Exists(SettingsPath))
                {
                    var json = File.ReadAllText(SettingsPath);
                    settings = JsonSerializer.Deserialize<SettingsData>(json) ?? new SettingsData();
                }
                else
                {
                    settings = new SettingsData();
                }
                var prop = typeof(SettingsData).GetProperty(key);
                if (prop != null)
                {
                    prop.SetValue(settings, value);
                }
                var newJson = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath));
                File.WriteAllText(SettingsPath, newJson);
            }
            catch { }
        }

        private class SettingsData
        {
            public string ExcelTemplatePath { get; set; }
            public int ExcelStartRow { get; set; } = 33;
            public int ExcelUnitColumn { get; set; } = 17;
        }
    }
}