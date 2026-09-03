// Views/TemplateEditorWindow.xaml.cs
using System.Windows;
using YYCount.ViewModels;

namespace YYCount.Views
{
    public partial class TemplateEditorWindow : Window
    {
        public TemplateEditorWindow(TemplateEditorViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            viewModel.CloseDialog += (result) =>
            {
                DialogResult = result;
                Close();
            };
        }
    }
}