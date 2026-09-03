using System;
using System.Windows.Input;
using Microsoft.Win32;
using YYCount.Infrastructure;
using YYCount.Services;

namespace YYCount.ViewModels
{
    public class TemplateEditorViewModel : ViewModelBase
    {
        private string _templatePath;
        public string TemplatePath
        {
            get => _templatePath;
            set => Set(ref _templatePath, value);
        }

        private int _startRow;
        public int StartRow
        {
            get => _startRow;
            set => Set(ref _startRow, value);
        }

        private int _unitColumn;
        public int UnitColumn
        {
            get => _unitColumn;
            set => Set(ref _unitColumn, value);
        }

        public ICommand BrowseCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public TemplateEditorViewModel()
        {
            TemplatePath = AppSettings.ExcelTemplatePath ?? "";
            StartRow = AppSettings.ExcelStartRow;
            UnitColumn = AppSettings.ExcelUnitColumn;

            BrowseCommand = new RelayCommand(Browse);
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void Browse()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Excel файлы (*.xlsx)|*.xlsx|Все файлы (*.*)|*.*",
                Title = "Выберите файл шаблона Excel"
            };
            if (dialog.ShowDialog() == true)
            {
                TemplatePath = dialog.FileName;
            }
        }

        private void Save()
        {
            AppSettings.ExcelTemplatePath = TemplatePath;
            AppSettings.ExcelStartRow = StartRow;
            AppSettings.ExcelUnitColumn = UnitColumn;
            CloseDialog?.Invoke(true);
        }

        private void Cancel()
        {
            CloseDialog?.Invoke(false);
        }

        public event Action<bool> CloseDialog;
    }
}