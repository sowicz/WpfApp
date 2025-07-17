using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using WpfApp.Model;

namespace WpfApp.ViewModel
{
    public partial class BaseViewModel : ObservableObject
    {

        private readonly ConnectionClient _connection;
        private readonly SettingsModel _settings;


        // Observable properties for connection settings
        [ObservableProperty]
        private string _ipAddress;
        [ObservableProperty]
        private string _port;
        [ObservableProperty]
        private Brush _connectionStatus = Brushes.LightCoral;
        [ObservableProperty]
        //private string _logs;
        ObservableCollection<string> _logs = new();


        public BaseViewModel()
        {
            // load to BaseView connection settings
            _settings = SettingsModel.Load();
            IpAddress = _settings.IpAddress;
            Port = _settings.Port;


            if (int.TryParse(Port, out int portNumber))
            {
                _connection = new ConnectionClient(IpAddress, portNumber);
            }
            else
            {
                Debug.WriteLine("Port number is invalid");
                //_connection = new ConnectionClient(IpAddress, 23); // Default port
            }

            //_connection.DataReceived += OnDataReceived;
            //_connection.Disconnected += OnDisconnected;

            ConnectionCommand = new RelayCommand(async () => await Connect());
            DisconnectionCommand = new RelayCommand(Disconnect);
            ClearLogsCommand = new RelayCommand(ClearLogs);


            // Subscribe in SettingsModel
            SettingsModel.SettingsChanged += OnSettingsChanged;
        }

        private async Task Connect()
        {
            bool isConnected = await _connection.Connect();

            if (isConnected)
            {
                ConnectionStatus = Brushes.LightGreen;
                AddLog("Connected to device");
                // Subscribe event (to listen on message)
                _connection.DataReceived += OnDataReceived;
            }
            else
            {
                ConnectionStatus = Brushes.LightCoral;
            }
        }

        private void Disconnect()
        {
            if (_connection != null)
            {
                // unsubscribe event (to listen on message)
                _connection.DataReceived -= OnDataReceived;
                _connection.Disconnect();
            }

            AddLog("Disconnected from device");
            ConnectionStatus = Brushes.LightCoral;
        }


        private void OnDataReceived(string message)
        {
            if (_connection == null) return;
            AddLog(message);
            //Debug.WriteLine($"Receiver message: {message}");
        }


        private void OnSettingsChanged()
        {
            var updatedSettings = SettingsModel.Load();
            IpAddress = updatedSettings.IpAddress;
            Port = updatedSettings.Port;
            // Disconnect after changing settings (need to connect maunally again)
            Disconnect();
            // Update ip-address and port in connection
            _connection.UpdateConnectionInfo(IpAddress, int.Parse(Port));
        }


        private void AddLog(string log)
        {
            string message = $"{DateTime.Now:HH:mm:ss} - {log}";
            App.Current.Dispatcher.Invoke(() =>
            {
                Logs.Add(message);
            });
        }

        private void ClearLogs()
        {
            Logs.Clear();

        }

        public IRelayCommand ConnectionCommand { get; }
        public IRelayCommand DisconnectionCommand { get; }

        public IRelayCommand ClearLogsCommand { get; }
    }
}
