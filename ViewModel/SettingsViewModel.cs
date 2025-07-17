using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using WpfApp.Model;

public partial class SettingsViewModel : ObservableObject
{


    private readonly SettingsModel _settings;


    [ObservableProperty] private string _ipPart1 = "";
    [ObservableProperty] private string _ipPart2 = "";
    [ObservableProperty] private string _ipPart3 = "";
    [ObservableProperty] private string _ipPart4 = "";
    [ObservableProperty] private string _port = "";

    public IRelayCommand SaveCommand { get; }

    public SettingsViewModel()
    {
        _settings = SettingsModel.Load(); 
        // Devide ip address to 4 parts

        var ipParts = _settings.IpAddress.Split('.');
        if(ipParts.Length == 4)
        {
            IpPart1 = ipParts[0];
            IpPart2 = ipParts[1];
            IpPart3 = ipParts[2];
            IpPart4 = ipParts[3];
        }
        Port = _settings.Port;

        SaveCommand = new RelayCommand(SaveSettings, CanSave);
        PropertyChanged += (s, e) => SaveCommand.NotifyCanExecuteChanged();
    }

    private void SaveSettings()
    {
        string ipAddress = $"{IpPart1}.{IpPart2}.{IpPart3}.{IpPart4}";
        string portToPrint = Port;

        _settings.IpAddress = ipAddress;
        _settings.Port = Port;
        _settings.Save();
        MessageBox.Show($"Saved IP: {ipAddress}:{portToPrint}. Restart TCP connection to device.", "Settings");
    }

    private bool CanSave()
    {
        return IsValidIpPart(IpPart1) &&
               IsValidIpPart(IpPart2) &&
               IsValidIpPart(IpPart3) &&
               IsValidIpPart(IpPart4) &&
               IsValidPort(Port);
    }

    private bool IsValidIpPart(string part)
    {
        if (int.TryParse(part, out int value))
        { 
            // First octet must be in range 1-255, others 0-255
            if (part == IpPart1)
            {
                return value >= 1 && value <= 255; 
            } else
            {
                return value >= 0 && value <= 255;
            }
        }
        return false;
    }

    private bool IsValidPort(string port)
    {
        if (int.TryParse(port, out int value))
            return value > 0 && value <= 65535; // Port w zakresie 1-65535
        return false;
    }
}
