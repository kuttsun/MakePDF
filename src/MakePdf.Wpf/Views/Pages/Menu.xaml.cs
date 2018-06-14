using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using System.Reflection;
using System.IO;

using Microsoft.Extensions.DependencyInjection;

using MaterialDesignThemes.Wpf;

using Prism.Events;

using MakePdf.Wpf.Views.Dialogs;
using MakePdf.Wpf.Views.Dialogs.Common;
using MakePdf.Wpf.ViewModels.Pages;
using MakePdf.Wpf.Models;

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

            Messenger.Instance[MessengerType.NewVersionFound].GetEvent<PubSubEvent<string>>().Subscribe(x =>
            {
                Runner runner = Service.Provider.GetService<Runner>();
                if (runner.IsProcessing == false)
                {
                    NewVersionFound(x);
                }
            });
        }

        void OpenLogFolder_Click(object sender, RoutedEventArgs e)
        {
            var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Process.Start($@"{location}\logs");
        }

        void Exit_Click(object sender, RoutedEventArgs e)
        {
            parentView.Close();
        }

        void Usage_Click(object sender, RoutedEventArgs e)
        {
            switch (ResourceService.Current.GetCulture())
            {
                case SupportedCulture.ja:
                    Process.Start("https://github.com/kuttsun/MakePdf/wiki/Home（日本語）");
                    break;
                default:
                    Process.Start("https://github.com/kuttsun/MakePdf/wiki");
                    break;
            }
        }

        async void About_Click(object sender, RoutedEventArgs e)
        {
            var aboutDialog = new AboutDialog();
            await parentView.dialogHostMain.ShowDialog(aboutDialog);
        }


        async void CheckForUpdate_Click(object sender, RoutedEventArgs e)
        {
            string newVersion = null;

            var processingDialog = new ProcessingDialog(Properties.Resources.MsgBox_CheckForUpdates_Title, Properties.Resources.MsgBox_CheckForUpdates_Message);
            await parentView.dialogHostMain.ShowDialog(processingDialog, async (object s, DialogOpenedEventArgs args) =>
            {
                newVersion = await vm.CheckForUpdate();
                args.Session.Close(false);
            });

            if (newVersion == null)
            {
                var okdialog = new OneButtonDialog(Properties.Resources.MsgBox_NotFound_Title, Properties.Resources.MsgBox_NotFound_Message);
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
            var processingDialog = new ProcessingDialog(Properties.Resources.MsgBox_Downloading_Title, Properties.Resources.MsgBox_Downloading_Message);
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

        void ChangeCulture_Auto(object sender, RoutedEventArgs e) => ResourceService.Current.ChangeCulture(SupportedCulture.Auto);
        void ChangeCulture_English(object sender, RoutedEventArgs e) => ResourceService.Current.ChangeCulture(SupportedCulture.en);
        void ChangeCulture_Japanese(object sender, RoutedEventArgs e) => ResourceService.Current.ChangeCulture(SupportedCulture.ja);
    }
}
