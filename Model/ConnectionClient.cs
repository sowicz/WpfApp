using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp.Model
{
    public class ConnectionClient
    {

        private TcpClient? _client;
        private NetworkStream? _stream;
        private CancellationTokenSource? _cts;


        public event Action<string>? DataReceived;
        public event Action? Disconnected;

        public string Address { get; private set; }
        public int Port { get; private set; }

        public ConnectionClient(string address, int port)
        {
            Address = address;
            Port = port;
        }


        public void UpdateConnectionInfo(string address, int port)
        {
            Address = address;
            Port = port;
        }

        public async Task<bool> Connect()
        {
            if (_client != null && _client.Connected)
                return true;  // Już połączone

            try
            {
                _client = new TcpClient();
                await _client.ConnectAsync(Address, Port);
                _stream = _client.GetStream();

                _cts = new CancellationTokenSource();
                _ = Task.Run(() => ReceiveDataAsync(_cts.Token));
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Connecting error: {ex.Message}");
                Disconnect();
                return false;
            }

        }

        public async Task SendAsync(string message)
        {
            if (_stream == null || _client == null || !_client.Connected) return;

            byte[] data = Encoding.UTF8.GetBytes(message);
            await _stream.WriteAsync(data, 0, data.Length);
        }

        private async Task ReceiveDataAsync(CancellationToken token)
        {
            if (_stream == null) return;

            byte[] buffer = new byte[1024];
            try
            {
                while (!token.IsCancellationRequested)
                {
                    int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length, token);
                    if (bytesRead == 0) break; // Serwer zamknął połączenie

                    string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    DataReceived?.Invoke(receivedMessage);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd odbierania danych: {ex.Message}");
            }
            finally
            {
                Disconnect();
            }
        }

        public void Disconnect()
        {
            _cts?.Cancel();
            _stream?.Close();
            _client?.Close();
            _client = null;
            _stream = null;
            Disconnected?.Invoke();
        }

    }
}
