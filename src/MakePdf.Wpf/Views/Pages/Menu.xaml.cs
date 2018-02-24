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
using System.Diagnostics;

using MaterialDesignThemes.Wpf;

using Prism.Events;

using MakePdf.Wpf.Views.Dialogs;
using MakePdf.Wpf.Views.Dialogs.Common;
using MakePdf.Wpf.ViewModels.Pages;

namespace MakePdf.Wpf.Views.Pages
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : UserControl
    {
        MenuViewModel vm;
        Shell parentView;

        public Menu()
        {
            InitializeComponent();

            vm = DataContext as MenuViewModel;
            parentView = Application.Current.MainWindow as Shell;

            Messenger.Instance.GetEvent<PubSubEvent<string>>().Subscribe(x => NewVersionFound(x));
        }

        void GitHub_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/kuttsun/MakePdf");
        }

        void Usage_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/kuttsun/MakePdf/wiki");
        }

        void About_Click(object sender, RoutedEventArgs e)
        {
            
        }


        async void CheckForUpdate_Click(object sender, RoutedEventArgs e)
        {
            string newVersion = null;

            var processingDialog = new ProcessingDialog("Check for updates...", $"Please wait a minute.");
            await parentView.dialogHostMain.ShowDialog(processingDialog, async (object s, DialogOpenedEventArgs args) =>
            {
                newVersion = await vm.CheckForUpdate();
                args.Session.Close(false);
            });

            if (newVersion == null)
            {
                var okdialog = new OneButtonDialog("Not found", $"You are using the latest version.");
                await parentView.dialogHostMain.ShowDialog(okdialog);
                return;
            }

            NewVersionFound(newVersion);
        }

        async void NewVersionFound(string newVersion)
        {
            YesNo needsUpdate = YesNo.No;
            var newVersionFoundDialog = new NewVersionFoundDialog(newVersion);
            await parentView.dialogHostMain.ShowDialog(newVersionFoundDialog, null, (object s, DialogClosingEventArgs args) =>
            {
                needsUpdate = (YesNo)args.Parameter;
            });

            if (needsUpdate == YesNo.No)
            {
                return;
            }

            // Download and decompress
            bool result = false;
            var processingDialog = new ProcessingDialog("Downloading...", $"Please wait a minute.");
            await parentView.dialogHostMain.ShowDialog(processingDialog, async (object s, DialogOpenedEventArgs args) =>
            {
                result = await vm.PrepareForUpdate();
                args.Session.Close(false);
            });
            if (result)
            {
                parentView.Close();
            }
        }
    }
}
