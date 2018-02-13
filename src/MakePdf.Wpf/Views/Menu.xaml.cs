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

        async void CheckForUpdate_Click(object sender, RoutedEventArgs e)
        {
            var parentView = Application.Current.MainWindow as Shell;

            string newVersion = null;

            var processingDialog = new ProcessingDialog("Check for updates...", $"Please wait a minute.");
            var ret = await parentView.dialogHostMain.ShowDialog(processingDialog, async (object s, DialogOpenedEventArgs args) =>
            {
                newVersion = await vm.CheckForUpdate();
                args.Session.Close(false);
            });

            if (newVersion == null)
            {
                var okdialog = new OkDialog("Not found", $"You are using the latest version.");
                await parentView.dialogHostMain.ShowDialog(okdialog);
                return;
            }

            var yesNoDialog = new YesNoDialog("New version found", $"The new version is available.{Environment.NewLine}Current version:{Environment.NewLine}New version: {newVersion + Environment.NewLine}Do you want to update now ?");
            await parentView.dialogHostMain.ShowDialog(yesNoDialog);

            // Update
        }
    }
}
