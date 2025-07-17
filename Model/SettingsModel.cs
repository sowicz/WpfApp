using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WpfApp.Model
{
    class SettingsModel
    {
        public string IpAddress { get; set; } = "192.168.1.100";
        public string Port { get; set; } = "8080";

        private static readonly string ConfigFilePath = "settings.json";


        public static event Action? SettingsChanged;

        // Load settings from file
        public static SettingsModel Load()
        {
            if(!File.Exists(ConfigFilePath))
                return new SettingsModel();
            // Returning default values

            var json = File.ReadAllText(ConfigFilePath);
            return JsonSerializer.Deserialize<SettingsModel>(json) ?? new SettingsModel();
        
        }


        public void Save()
        {
            var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigFilePath, json);
            SettingsChanged?.Invoke();
        }

    }
}
