using System.Windows;
using YYCount.ViewModels;
using YYCount.Infrastructure;

namespace YYCount.Views
{
    public partial class EquipmentEditorDialog : Window
    {
        public EquipmentEditorDialog(EquipmentEditorViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            // Переопределяем команды для установки DialogResult
            viewModel.SaveCommand = new RelayCommand(() =>
            {
                viewModel.Save(); // Вызываем сохранение
                DialogResult = true;
                Close();
            });

            viewModel.CancelCommand = new RelayCommand(() =>
            {
                DialogResult = false;
                Close();
            });
        }
    }
}