using System.Windows;
using YYCount.Models;
using YYCount.ViewModels;

namespace YYCount.Views
{
    public partial class EquipmentSelectorDialog : Window
    {
        public EquipmentItem SelectedEquipment { get; private set; }

        public EquipmentSelectorDialog(EquipmentSelectorViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            viewModel.ItemSelected += (s, item) =>
            {
                SelectedEquipment = item;
                DialogResult = true;
                Close();
            };

            viewModel.CancelCommand = new Infrastructure.RelayCommand(() =>
            {
                DialogResult = false;
                Close();
            });

            this.Loaded += (s, e) =>
            {
                ItemsListBox.MouseDoubleClick += (sender, args) =>
                {
                    if (viewModel.SelectedItem != null)
                        viewModel.SelectCommand.Execute(viewModel.SelectedItem);
                };
            };
        }
    }
}