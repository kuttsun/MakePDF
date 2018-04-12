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
//using System.Windows.Shapes;
using System.IO;

using Microsoft.WindowsAPICodePack.Dialogs;

using MaterialDesignThemes.Wpf;

using MakePdf.Wpf.Views.Dialogs.Common;
using MakePdf.Wpf.ViewModels.Pages;

namespace MakePdf.Wpf.Views.Pages
{
    /// <summary>
    /// Interaction logic for EasyMode.xaml
    /// </summary>
    public partial class EasyMode : UserControl
    {
        readonly string exePath;
        readonly string exeFullPath;
        readonly string startupPath;

        EasyModeViewModel vm;

        public EasyMode()
        {
            InitializeComponent();

            vm = DataContext as EasyModeViewModel;

            exePath = Environment.GetCommandLineArgs()[0];
            exeFullPath = Path.GetFullPath(exePath);
            startupPath = Path.GetDirectoryName(exeFullPath);
        }

        // ref: https://qiita.com/Fuhduki/items/447e5707c4fa4c8f532a
        //dynamic VM
        //{
        //    get { return DataContext; }
        //}

        void ListView_Drop(object sender, DragEventArgs e)
        {
            var dropFileList = (e.Data.GetData(DataFormats.FileDrop) as string[]).ToList();

            vm.AddFiles(dropFileList);
        }

        void ListView_PreviewDragOver(object sender, DragEventArgs e)
        {
            // Only drop event
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        void OutputFile_Drop(object sender, DragEventArgs e)
        {
            var dropFileList = (e.Data.GetData(DataFormats.FileDrop) as string[]).ToList();

            vm.OutputFile = dropFileList[0];
        }

        void OutputFile_PreviewDragOver(object sender, DragEventArgs e)
        {
            // Only drop event
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog
            {
                Title = Properties.Resources.Common_OpenFileDialog_Title_OutputFile,
                IsFolderPicker = false,
                InitialDirectory = startupPath,
                DefaultDirectory = startupPath,
            };

            dialog.Filters.Add(new CommonFileDialogFilter(Properties.Resources.Common_OpenFileDialog_FileType_Pdf, "*.pdf"));
            dialog.Filters.Add(new CommonFileDialogFilter(Properties.Resources.Common_OpenFileDialog_FileType_All, "*.*"));

            if (vm.OutputFile != string.Empty)
            {
                dialog.InitialDirectory = Path.GetDirectoryName(vm.OutputFile);
            }

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                vm.OutputFile = dialog.FileName;
            }
        }

        async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            var parentView = Application.Current.MainWindow as Shell;

            // Output file is empty
            if (vm.OutputFile == "")
            {
                var overwriteDialog = new OneButtonDialog(Properties.Resources.Dialog_FileNameEmpty_Title, Properties.Resources.Dialog_FileNameEmpty_Message_OutputFile);
                await parentView.dialogHostMain.ShowDialog(overwriteDialog);
                return;
            }

            // Output file already exists
            if (File.Exists(vm.OutputFile))
            {
                var overwriteDialog = new TwoButtonDialog(
                    Properties.Resources.Dialog_FileExists_Title,
                    Path.GetFileName(vm.OutputFile) + " " + Properties.Resources.Dialog_FileExists_Message + Environment.NewLine + Properties.Resources.Dialog_FileExists_Message_Overwrite,
                    Properties.Resources.Common_Yes,
                    Properties.Resources.Common_No);
                var result = await parentView.dialogHostMain.ShowDialog(overwriteDialog) as Selected?;
                if (result == Selected.Negative)
                {
                    return;
                }
            }

            // Start
            var processingDialog = new ProcessingDialog(Properties.Resources.Dialog_Processing_Title, Properties.Resources.Dialog_Processing_Message);
            var re = parentView.dialogHostMain.ShowDialog(processingDialog, async (object s, DialogOpenedEventArgs args) =>
            {
                await vm.StartAsync();
                args.Session.Close(false);
            });
        }
    }
}
