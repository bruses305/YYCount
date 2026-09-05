using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Win32;
using OfficeOpenXml;
using YYCount.Infrastructure;
using YYCount.Models;
using YYCount.Services;

namespace YYCount.ViewModels
{
    public class TemplateEditorViewModel : ViewModelBase
    {
        private string _templatePath;
        public string TemplatePath
        {
            get => _templatePath;
            set
            {
                if (Set(ref _templatePath, value))
                {
                    CheckTemplate();
                }
            }
        }

        public ObservableCollection<TagStatus> Tags { get; }

        public ICommand BrowseCommand { get; }
        public ICommand CheckCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand CopyTagCommand { get; }

        private string _copyMessage;
        public string CopyMessage
        {
            get => _copyMessage;
            set => Set(ref _copyMessage, value);
        }

        private DispatcherTimer _copyMessageTimer;

        public TemplateEditorViewModel()
        {
            TemplateSettings ts = AppSettings.GetTemplateSettings();
            Tags = new ObservableCollection<TagStatus>
            {
                new TagStatus(ts.TableStartTag),
                new TagStatus(ts.IdTag),
                new TagStatus(ts.EquipmentNameTag),
                new TagStatus(ts.QuantityTag),
                new TagStatus(ts.UnitValueTag),
                new TagStatus(ts.TotalTag),
                new TagStatus(ts.SumTag),
            };

            TemplatePath = AppSettings.ExcelTemplatePath ?? "";

            BrowseCommand = new RelayCommand(Browse);
            CheckCommand = new RelayCommand(CheckTemplate);
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
            CopyTagCommand = new RelayCommand<string>(CopyTag);

            _copyMessageTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1.5)
            };
            _copyMessageTimer.Tick += (s, e) =>
            {
                CopyMessage = "";
                _copyMessageTimer.Stop();
            };
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

        private void CheckTemplate()
        {
            foreach (var tag in Tags)
                tag.IsFound = false;

            if (string.IsNullOrWhiteSpace(TemplatePath) || !File.Exists(TemplatePath))
                return;

            try
            {
                ExcelPackage.License.SetNonCommercialPersonal("Local");
                using var package = new ExcelPackage(new FileInfo(TemplatePath));
                foreach (var worksheet in package.Workbook.Worksheets)
                {
                    foreach (var cell in worksheet.Cells)
                    {
                        if (cell.Value is string text)
                        {
                            foreach (var tag in Tags)
                            {
                                if (text.Contains(tag.TagName))
                                    tag.IsFound = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при проверке шаблона: {ex.Message}", 
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CopyTag(string tag)
        {
            if (string.IsNullOrEmpty(tag)) return;
            Clipboard.SetText(tag);
            CopyMessage = $"Тег {tag} скопирован";
            _copyMessageTimer.Stop();
            _copyMessageTimer.Start();
        }

        private void Save()
        {
            AppSettings.ExcelTemplatePath = TemplatePath;
            CloseDialog?.Invoke(true);
        }

        private void Cancel()
        {
            CloseDialog?.Invoke(false);
        }

        public event Action<bool> CloseDialog;
    }

    public class TagStatus : ViewModelBase
    {
        private string _tagName;
        public string TagName
        {
            get => _tagName;
            set => Set(ref _tagName, value);
        }

        private bool _isFound;
        public bool IsFound
        {
            get => _isFound;
            set => Set(ref _isFound, value);
        }

        public TagStatus(string tagName, bool isFound = false)
        {
            TagName = tagName;
            IsFound = isFound;
        }
    }
}