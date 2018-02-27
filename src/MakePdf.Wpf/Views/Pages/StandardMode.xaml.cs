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
using System.IO;

using Microsoft.WindowsAPICodePack.Dialogs;

using MaterialDesignThemes.Wpf;

using MakePdf.Wpf.Views.Dialogs.Common;
using MakePdf.Wpf.ViewModels.Pages;

namespace MakePdf.Wpf.Views.Pages
{
    /// <summary>
    /// Interaction logic for StandardMode.xaml
    /// </summary>
    public partial class StandardMode : UserControl
    {
        readonly string exePath;
        readonly string exeFullPath;
        readonly string startupPath;

        StandardModeViewModel vm;

        public StandardMode()
        {
            InitializeComponent();

            vm = DataContext as StandardModeViewModel;

            exePath = Environment.GetCommandLineArgs()[0];
            exeFullPath = Path.GetFullPath(exePath);
            startupPath = Path.GetDirectoryName(exeFullPath);
        }

        void InputDirectory_Drop(object sender, DragEventArgs e)
        {
            var dropFileList = (e.Data.GetData(DataFormats.FileDrop) as string[]).ToList();

            if (Directory.Exists(dropFileList[0]))
            {
                vm.InputDirectory = dropFileList[0];
            }
        }

        void OutputFile_Drop(object sender, DragEventArgs e)
        {
            var dropFileList = (e.Data.GetData(DataFormats.FileDrop) as string[]).ToList();

            if (File.Exists(dropFileList[0]))
            {
                vm.OutputFile = dropFileList[0];
            }
        }

        void Item_PreviewDragOver(object sender, DragEventArgs e)
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

        void SetInputDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog
            {
                Title = "Select input directory",
                IsFolderPicker = true,
                InitialDirectory = startupPath,
                DefaultDirectory = startupPath,
            };

            if (vm.InputDirectory != string.Empty)
            {
                dialog.InitialDirectory = vm.InputDirectory;
            }

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                vm.InputDirectory = dialog.FileName;
            }
        }
        void SetOutputFileButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog
            {
                Title = "Select output file",
                IsFolderPicker = false,
                InitialDirectory = startupPath,
                DefaultDirectory = startupPath,
            };

            dialog.Filters.Add(new CommonFileDialogFilter("PDF File", "*.pdf"));
            dialog.Filters.Add(new CommonFileDialogFilter("All File", "*.*"));

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

            if (vm.InputDirectory == "")
            {
                var overwriteDialog = new OneButtonDialog("Input directory is empty", $"Please specify the intput directory.");
                await parentView.dialogHostMain.ShowDialog(overwriteDialog);
                return;
            }

            if (vm.OutputFile == "")
            {
                var overwriteDialog = new OneButtonDialog("Output file is empty", $"Please specify the output file.");
                await parentView.dialogHostMain.ShowDialog(overwriteDialog);
                return;
            }

            if (File.Exists(vm.OutputFile))
            {
                var overwriteDialog = new TwoButtonDialog("Output file exists", $"The output file already exists.{Environment.NewLine}Would you like to overwrite it?", "Yes", "No");
                var result = await parentView.dialogHostMain.ShowDialog(overwriteDialog) as Selected?;
                if (result == Selected.Negative)
                {
                    return;
                }
            }

            // Start
            var processingDialog = new ProcessingDialog("Processing", $"Please wait ...");
            var re = parentView.dialogHostMain.ShowDialog(processingDialog, async (object s, DialogOpenedEventArgs args) =>
            {
                await vm.StartAsync();
                args.Session.Close(false);
            });
        }
    }
}
