using System.Windows;
using System.Windows.Controls;
using WpfApp.ViewModel;

namespace WpfApp.View
{
    /// <summary>
    /// Logika interakcji dla klasy AuthView.xaml
    /// </summary>
    public partial class AuthView : UserControl
    {
        public AuthView()
        {
            InitializeComponent();
        }
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is AuthViewModel viewModel)
            {
                viewModel.Password = ((PasswordBox)sender).Password;
            }
        }
    }
}
