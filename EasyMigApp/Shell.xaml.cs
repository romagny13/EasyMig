using EasyMigApp.ViewModels;
using System.Windows;

namespace EasyMigApp
{
    public partial class Shell : Window
    {
        public Shell()
        {
            InitializeComponent();

            this.DataContext = new ShellViewModel();
        }
    }
}
