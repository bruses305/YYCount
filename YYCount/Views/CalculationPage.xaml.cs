using System.Windows.Controls;
using YYCount.ViewModels;

namespace YYCount.Views
{
    public partial class CalculationPage : Page
    {
        public CalculationPage()
        {
            InitializeComponent();

            this.Loaded += (s, e) =>
            {
                if (DataContext is CalculationPageViewModel vm)
                    vm.RequestFocusQuantity += () => QuantityTextBox.Focus();
            };
        }

        private void QuantityTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                if (DataContext is CalculationPageViewModel vm && vm.AddPositionCommand.CanExecute(null))
                    vm.AddPositionCommand.Execute(null);
                e.Handled = true;
            }
        }
    }
}