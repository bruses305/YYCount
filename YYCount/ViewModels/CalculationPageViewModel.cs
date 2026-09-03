using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using YYCount.Infrastructure;
using YYCount.Models;
using YYCount.Services;
using YYCount.Views;

namespace YYCount.ViewModels
{
    public class CalculationPageViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel _mainVm;
        private readonly IDataService _dataService;
        private readonly Calculation _calculation;
        private bool _isNew;

        public string CalculationName
        {
            get => _calculation.Name;
            set
            {
                _calculation.Name = value;
                OnPropertyChanged();
            }
        }

        public DateTime CalculationDate => _calculation.Date;

        public ObservableCollection<EquipmentItem> EquipmentItems { get; }
        public ObservableCollection<PositionViewModel> Positions { get; }

        private EquipmentItem _selectedEquipment;
        public EquipmentItem SelectedEquipment
        {
            get => _selectedEquipment;
            set
            {
                if (Set(ref _selectedEquipment, value))
                {
                    OnPropertyChanged(nameof(SelectedEquipmentName));
                    RequestFocusQuantity?.Invoke();
                }
            }
        }

        public string SelectedEquipmentName => SelectedEquipment?.Name ?? "Выбрать оборудование";

        private string _quantityToAddText = "";
        public string QuantityToAddText
        {
            get => _quantityToAddText;
            set => Set(ref _quantityToAddText, value);
        }

        public double TotalSum => _calculation.TotalSum;

        // Команды
        public ICommand AddPositionCommand { get; }
        public ICommand RemovePositionCommand { get; }
        public ICommand SaveCalculationCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand EditEquipmentCommand { get; }
        public ICommand OpenSelectorCommand { get; }
        public ICommand ExportToExcelCommand { get; }

        public event Action RequestFocusQuantity;

        public CalculationPageViewModel(MainWindowViewModel mainVm, Calculation existingCalc)
        {
            _mainVm = mainVm;
            _dataService = new JsonDataService();

            var equip = _dataService.LoadEquipment();
            EquipmentItems = new ObservableCollection<EquipmentItem>(equip);

            if (existingCalc == null)
            {
                _calculation = new Calculation
                {
                    Id = GetNextId(),
                    Date = DateTime.Now,
                    Name = "Новый расчёт"
                };
                _isNew = true;
            }
            else
            {
                _calculation = existingCalc;
                _isNew = false;
            }

            Positions = new ObservableCollection<PositionViewModel>(
                _calculation.Positions.Select(p => new PositionViewModel(p)));

            AddPositionCommand = new RelayCommand(AddPosition, CanAddPosition);
            RemovePositionCommand = new RelayCommand<PositionViewModel>(RemovePosition);
            SaveCalculationCommand = new RelayCommand(SaveCalculation);
            CancelCommand = new RelayCommand(() => _mainVm.NavigateToHistory());
            EditEquipmentCommand = new RelayCommand(EditEquipment);
            OpenSelectorCommand = new RelayCommand(OpenSelector);
            ExportToExcelCommand = new RelayCommand(ExportToExcel);

            Positions.CollectionChanged += (s, e) => OnPropertyChanged(nameof(TotalSum));
            foreach (var pos in Positions)
                pos.PropertyChanged += (s, e) => OnPropertyChanged(nameof(TotalSum));
        }

        private int GetNextId()
        {
            var all = _dataService.LoadCalculations();
            return all.Any() ? all.Max(c => c.Id) + 1 : 1;
        }

        private bool CanAddPosition()
        {
            return SelectedEquipment != null && 
                   double.TryParse(QuantityToAddText, out double q) && q > 0;
        }

        private void AddPosition()
        {
            if (!double.TryParse(QuantityToAddText, out double quantity) || quantity <= 0)
                return;

            var pos = new CalculationPosition
            {
                Id = Positions.Any() ? Positions.Max(p => p.Model.Id) + 1 : 1,
                EquipmentName = SelectedEquipment.Name,
                Quantity = quantity,
                UnitValue = SelectedEquipment.UnitValue
            };
            var vm = new PositionViewModel(pos);
            vm.PropertyChanged += (s, e) => OnPropertyChanged(nameof(TotalSum));
            Positions.Add(vm);
            _calculation.Positions.Add(pos);

            QuantityToAddText = "";
            OnPropertyChanged(nameof(TotalSum));
            RequestFocusQuantity?.Invoke();
        }

        private void RemovePosition(PositionViewModel pos)
        {
            if (pos == null) return;
            Positions.Remove(pos);
            _calculation.Positions.Remove(pos.Model);
            OnPropertyChanged(nameof(TotalSum));
        }

        private void SaveCalculation()
        {
            var all = _dataService.LoadCalculations();
            if (_isNew)
                all.Add(_calculation);
            else
            {
                var existing = all.FirstOrDefault(c => c.Id == _calculation.Id);
                if (existing != null)
                {
                    existing.Name = _calculation.Name;
                    existing.Date = _calculation.Date;
                    existing.Positions = _calculation.Positions;
                }
            }
            _dataService.SaveCalculations(all);
            _mainVm.NavigateToHistory();
            if (_mainVm.CurrentPage is HistoryPageViewModel historyVm)
                historyVm.Refresh();
        }

        private void EditEquipment()
        {
            var dialog = new EquipmentEditorDialog(
                new EquipmentEditorViewModel(EquipmentItems, _dataService));
            if (dialog.ShowDialog() == true)
            {
                EquipmentItems.Clear();
                var updated = _dataService.LoadEquipment();
                foreach (var item in updated)
                    EquipmentItems.Add(item);
                if (SelectedEquipment != null && !EquipmentItems.Any(e => e.Id == SelectedEquipment.Id))
                    SelectedEquipment = null;
            }
        }

        private void OpenSelector()
        {
            var selectorVm = new EquipmentSelectorViewModel(_dataService);
            var dialog = new EquipmentSelectorDialog(selectorVm);
            if (dialog.ShowDialog() == true && dialog.SelectedEquipment != null)
            {
                SelectedEquipment = dialog.SelectedEquipment;
                RequestFocusQuantity?.Invoke();
            }
        }

        private void ExportToExcel()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Excel файлы (*.xlsx)|*.xlsx",
                FileName = $"{CalculationName}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            };
            if (dialog.ShowDialog() == true)
            {
                var service = new ExcelService();
                string templatePath = AppSettings.ExcelTemplatePath;
                if (!string.IsNullOrEmpty(templatePath) && File.Exists(templatePath))
                    service.ExportCalculationToExcel(_calculation, dialog.FileName, templatePath);
                else
                    service.ExportCalculationToExcel(_calculation, dialog.FileName);
                MessageBox.Show("Экспорт завершён!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }

    // Вспомогательная ViewModel для позиции
    public class PositionViewModel : ViewModelBase
    {
        private readonly CalculationPosition _model;
        public CalculationPosition Model => _model;

        public int Id => _model.Id;

        public string EquipmentName
        {
            get => _model.EquipmentName;
            set
            {
                if (_model.EquipmentName != value)
                {
                    _model.EquipmentName = value;
                    OnPropertyChanged();
                }
            }
        }

        public double Quantity
        {
            get => _model.Quantity;
            set
            {
                _model.Quantity = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Total));
            }
        }

        public double UnitValue
        {
            get => _model.UnitValue;
            set
            {
                _model.UnitValue = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Total));
            }
        }

        public double Total => _model.Total;

        public PositionViewModel(CalculationPosition model)
        {
            _model = model;
        }
    }
}