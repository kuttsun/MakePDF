using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using MaterialDesignThemes.Wpf;

using MakePdf.Wpf.Views.Dialogs;
using MakePdf.Wpf.ViewModels;

namespace MakePdf.Wpf.Views
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : UserControl
    {
        MenuViewModel vm;

        public Menu()
        {
            InitializeComponent();

            vm = DataContext as MenuViewModel;
        }

        void CheckForUpdate_Click(object sender, RoutedEventArgs e)
        {
            var parentView = Application.Current.MainWindow as Shell;

            var processingDialog = new ProcessingDialog("Processing", $"Check for Update ...");
            var re = parentView.dialogHostMain.ShowDialog(processingDialog, async (object s, DialogOpenedEventArgs args) =>
            {
                await vm.CheckForUpdate();
                args.Session.Close(false);
            });

            return;
        }
    }
}
