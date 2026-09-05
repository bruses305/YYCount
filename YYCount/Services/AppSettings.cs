using System;
using System.IO;
using System.Text.Json;
using YYCount.Models;

namespace YYCount.Services
{
    public static class AppSettings
    {
        private static readonly string SettingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "YYCount", "settings.json");

        public static string ExcelTemplatePath
        {
            get => LoadSettings()?.TemplatePath ?? "";
            set
            {
                var settings = LoadSettings() ?? new TemplateSettings();
                settings.TemplatePath = value;
                SaveSettings(settings);
            }
        }

        public static TemplateSettings GetTemplateSettings()
        {
            return LoadSettings() ?? new TemplateSettings();
        }

        public static void SaveTemplateSettings(TemplateSettings settings)
        {
            SaveSettings(settings);
        }

        private static TemplateSettings LoadSettings()
        {
            try
            {
                if (!File.Exists(SettingsPath)) return null;
                var json = File.ReadAllText(SettingsPath);
                return JsonSerializer.Deserialize<TemplateSettings>(json);
            }
            catch { return null; }
        }

        private static void SaveSettings(TemplateSettings settings)
        {
            try
            {
                var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath));
                File.WriteAllText(SettingsPath, json);
            }
            catch { }
        }
    }
}