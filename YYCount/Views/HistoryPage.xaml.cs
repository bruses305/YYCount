using System.Windows.Controls;
using YYCount.ViewModels;

namespace YYCount.Views
{
    public partial class HistoryPage : Page
    {
        public HistoryPage()
        {
            InitializeComponent();
            // DataContext задаётся через навигацию (в MainWindowViewModel)
        }
    }
}