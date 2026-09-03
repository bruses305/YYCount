using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using YYCount.Infrastructure;
using YYCount.Models;
using YYCount.Services;

namespace YYCount.ViewModels
{
    public class HistoryPageViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel _mainVm;
        private readonly IDataService _dataService;

        public ObservableCollection<Calculation> Calculations { get; }

        public ICommand OpenCalculationCommand { get; }
        public ICommand CreateNewCommand { get; }
        public ICommand DeleteCalculationCommand { get; } 
        public ICommand OpenTemplateEditorCommand { get; }
        public ICommand OpenAboutCommand { get; } 

        public HistoryPageViewModel(MainWindowViewModel mainVm)
        {
            _mainVm = mainVm;
            _dataService = new JsonDataService();

            var list = _dataService.LoadCalculations();
            Calculations = new ObservableCollection<Calculation>(list.OrderByDescending(c => c.Date));

            OpenCalculationCommand = new RelayCommand<Calculation>(calc => _mainVm.NavigateToCalculation(calc));
            CreateNewCommand = new RelayCommand(() => _mainVm.NavigateToCalculation(null));
            DeleteCalculationCommand = new RelayCommand<Calculation>(DeleteCalculation, CanDeleteCalculation);

            OpenTemplateEditorCommand = new RelayCommand(()=>_mainVm.OpenTemplateEditor());
            //OpenAboutCommand = new RelayCommand(()=>_mainVm.);
        }

        private bool CanDeleteCalculation(Calculation calc) => calc != null;

        private void DeleteCalculation(Calculation calc)
        {
            if (calc == null) return;

            var result = MessageBox.Show(
                $"Удалить расчёт \"{calc.Name}\"?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            var all = _dataService.LoadCalculations();
            var toRemove = all.FirstOrDefault(c => c.Id == calc.Id);
            if (toRemove != null)
            {
                all.Remove(toRemove);
                _dataService.SaveCalculations(all);
                Refresh(); // обновить список
            }
        }

        public void Refresh()
        {
            var list = _dataService.LoadCalculations();
            Calculations.Clear();
            foreach (var calc in list.OrderByDescending(c => c.Date))
                Calculations.Add(calc);
        }
    }
}