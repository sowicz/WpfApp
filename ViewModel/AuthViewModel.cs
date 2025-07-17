using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace WpfApp.ViewModel
{
    public partial class AuthViewModel : ObservableObject
    {
        private readonly MainViewModel _mainViewModel;

        [ObservableProperty]
        private string _password;

        public IRelayCommand LoginCommand { get; }

        public AuthViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            LoginCommand = new RelayCommand(Login);
        }

        private void Login()
        {
            if (Password == "1234") // Poprawne hasło
            {
                _mainViewModel.CurrentView = new View.SettingsView { DataContext = new SettingsViewModel() };
            } else
            {
                MessageBox.Show("Wrong password", "Auth error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
