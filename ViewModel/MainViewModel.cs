using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpfApp.Model;
using WpfApp.View;

namespace WpfApp.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private UserControl _currentView;


        private readonly BaseViewModel _baseViewModel;


        public MainViewModel()
        {
            _baseViewModel = new BaseViewModel();

            ShowMainCommand = new RelayCommand(() => CurrentView = new BaseView {DataContext = _baseViewModel });
            ShowRecipeCommand = new RelayCommand(() => CurrentView = new UserControl { Content = "Recipe View" });
            ShowAuthViewCommand = new RelayCommand(() => CurrentView = new AuthView { DataContext = new AuthViewModel(this) });

            // Default view in main screen
            CurrentView = new BaseView { DataContext = _baseViewModel };


        }

        // Navigation buttons on main window
        public IRelayCommand ShowMainCommand { get; }
        public IRelayCommand ShowRecipeCommand { get; }
        //public IRelayCommand ShowSettingsCommand { get; }
        public IRelayCommand ShowAuthViewCommand { get; }




    }
}
