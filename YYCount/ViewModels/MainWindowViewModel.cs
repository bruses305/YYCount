using System.Windows.Input;
using YYCount.Infrastructure;
using YYCount.Models;
using YYCount.Views;

namespace YYCount.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private object _currentPage;
        public object CurrentPage
        {
            get => _currentPage;
            set => Set(ref _currentPage, value);
        }

        public ICommand NavigateCalculationCommand { get; }

        public MainWindowViewModel()
        {
            NavigateCalculationCommand = new RelayCommand(() => CurrentPage = CreateCalculationPage(null));
            CurrentPage = CreateHistoryPage(); // стартовая страница
        }

        private HistoryPage CreateHistoryPage()
        {
            var vm = new HistoryPageViewModel(this);
            var page = new HistoryPage();
            page.DataContext = vm;
            return page;
        }

        private CalculationPage CreateCalculationPage(Calculation calculation)
        {
            var vm = new CalculationPageViewModel(this, calculation);
            var page = new CalculationPage();
            page.DataContext = vm;
            return page;
        }

        public void NavigateToCalculation(Calculation calculation = null)
        {
            CurrentPage = CreateCalculationPage(calculation);
        }

        public void NavigateToHistory()
        {
            CurrentPage = CreateHistoryPage();
        }
        
        public void OpenTemplateEditor()
        {
            var vm = new TemplateEditorViewModel();
            var window = new TemplateEditorWindow(vm);
            window.ShowDialog();
        }

        // private void OpenAbout()
        // {
        //     var vm = new AboutViewModel();
        //     var window = new AboutWindow(vm);
        //     window.ShowDialog();
        // }
    }
}