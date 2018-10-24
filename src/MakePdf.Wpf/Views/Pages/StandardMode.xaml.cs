using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        Shell parentView;

        public StandardMode()
        {
            InitializeComponent();

            vm = DataContext as StandardModeViewModel;
            parentView = Application.Current.MainWindow as Shell;

            exePath = Environment.GetCommandLineArgs()[0];
            exeFullPath = Path.GetFullPath(exePath);
            startupPath = Path.GetDirectoryName(exeFullPath);
        }

        async void Page_Drop(object sender, DragEventArgs e)
        {
            var element = e.Source as FrameworkElement;
            if (element?.Name.Contains("DropAllowedControl") ?? false)
            {
                // ignore since the handler of the child control is being executed
                return;
            }

            var dialog = new TwoButtonDialog(
                     Properties.Resources.Dialog_DropSettingFile_Title,
                     Properties.Resources.Dialog_DropSettingFile_Message,
                     Properties.Resources.Common_Yes,
                     Properties.Resources.Common_No);
            var result = await parentView.dialogHostMain.ShowDialog(dialog) as Selected?;
            if (result == Selected.Negative)
            {
                return;
            }

            var dropFileList = (e.Data.GetData(DataFormats.FileDrop) as string[]).ToList();

            vm.LoadSetting(dropFileList.FirstOrDefault());
        }

        void InputDirectory_Drop(object sender, DragEventArgs e)
        {
            var dropFileList = (e.Data.GetData(DataFormats.FileDrop) as string[]).ToList();

            if (Directory.Exists(dropFileList[0]))
            {
                vm.WorkingDirectory = dropFileList[0];
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
                Title = Properties.Resources.Common_OpenFolderDialog_Title_WorkingDirectory,
                IsFolderPicker = true,
                InitialDirectory = startupPath,
                DefaultDirectory = startupPath,
            };

            if (vm.WorkingDirectory != string.Empty)
            {
                dialog.InitialDirectory = vm.WorkingDirectory;
            }

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                vm.WorkingDirectory = dialog.FileName;
            }
        }
        void SetOutputFileButton_Click(object sender, RoutedEventArgs e)
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

        async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog
            {
                Title = Properties.Resources.Common_OpenFileDialog_Title_SaveSetting,
                IsFolderPicker = false,
                InitialDirectory = startupPath,
                DefaultDirectory = startupPath,
            };

            dialog.Filters.Add(new CommonFileDialogFilter(Properties.Resources.Common_OpenFileDialog_FileType_Json, "*.json"));
            dialog.Filters.Add(new CommonFileDialogFilter(Properties.Resources.Common_OpenFileDialog_FileType_All, "*.*"));

            if (vm.WorkingDirectory != string.Empty)
            {
                dialog.InitialDirectory = vm.WorkingDirectory;
            }

            if (dialog.ShowDialog() != CommonFileDialogResult.Ok)
            {
                return;
            }

            // The setting file already exists
            if (File.Exists(dialog.FileName))
            {
                var overwriteDialog = new TwoButtonDialog(
                     Properties.Resources.Dialog_FileExists_Title,
                     Path.GetFileName(dialog.FileName) + " " + Properties.Resources.Dialog_FileExists_Message + Environment.NewLine + Properties.Resources.Dialog_FileExists_Message_Overwrite,
                     Properties.Resources.Common_Yes,
                     Properties.Resources.Common_No);
                var result = await parentView.dialogHostMain.ShowDialog(overwriteDialog) as Selected?;
                if (result == Selected.Negative)
                {
                    return;
                }
            }

            vm.SaveSetting(dialog.FileName);
        }

        async void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog
            {
                Title = Properties.Resources.Common_OpenFileDialog_Title_LoadSetting,
                IsFolderPicker = false,
                InitialDirectory = startupPath,
                DefaultDirectory = startupPath,
            };

            dialog.Filters.Add(new CommonFileDialogFilter(Properties.Resources.Common_OpenFileDialog_FileType_Json, "*.json"));
            dialog.Filters.Add(new CommonFileDialogFilter(Properties.Resources.Common_OpenFileDialog_FileType_All, "*.*"));

            if (vm.WorkingDirectory != string.Empty)
            {
                dialog.InitialDirectory = vm.WorkingDirectory;
            }

            if (dialog.ShowDialog() != CommonFileDialogResult.Ok)
            {
                return;
            }

            if (File.Exists(dialog.FileName))
            {
                vm.LoadSetting(dialog.FileName);
            }
            else
            {
                var overwriteDialog = new OneButtonDialog(Properties.Resources.Dialog_FileNotExists_Title, Properties.Resources.Dialog_FileNotExists_Message_SettingFile);
                await parentView.dialogHostMain.ShowDialog(overwriteDialog);
            }
        }

        async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            // Working directory is empty
            if (string.IsNullOrEmpty(vm.WorkingDirectory))
            {
                var overwriteDialog = new OneButtonDialog(Properties.Resources.Dialog_DirectoryEmpty_Title, Properties.Resources.Dialog_DirectoryEmpty_Message_WorkingDirectory);
                await parentView.dialogHostMain.ShowDialog(overwriteDialog);
                return;
            }

            // Output file is empty
            if (string.IsNullOrEmpty(vm.OutputFile))
            {
                var overwriteDialog = new OneButtonDialog(Properties.Resources.Dialog_FileNameEmpty_Title, Properties.Resources.Dialog_FileNameEmpty_Message_OutputFile);
                await parentView.dialogHostMain.ShowDialog(overwriteDialog);
                return;
            }

            // Check winword process
            if (Process.GetProcessesByName("winword").Length > 0)
            {
                var dialog = new OneButtonDialog(Properties.Resources.Dialog_ProcessExists_Title, Properties.Resources.Dialog_ProcessExists_Word);
                await parentView.dialogHostMain.ShowDialog(dialog);
                return;
            }
            // Check excel process
            if (Process.GetProcessesByName("excel").Length > 0)
            {
                var dialog = new OneButtonDialog(Properties.Resources.Dialog_ProcessExists_Title, Properties.Resources.Dialog_ProcessExists_Excel);
                await parentView.dialogHostMain.ShowDialog(dialog);
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
            var processingDialog = new ProcessingDialogDetail(Properties.Resources.Dialog_Processing_Title, Properties.Resources.Dialog_Processing_Started);
            var re = parentView.dialogHostMain.ShowDialog(processingDialog, async (object s, DialogOpenedEventArgs args) =>
            {
                processingDialog.Button.IsEnabled = false;
                processingDialog.Button.IsEnabled = false;
                await vm.StartAsync();

                processingDialog.Message.Content = Properties.Resources.Dialog_Processing_Completed;
                processingDialog.ProgressBar.IsIndeterminate = false;
                processingDialog.Button.IsEnabled = true;
            });
        }
    }
}
