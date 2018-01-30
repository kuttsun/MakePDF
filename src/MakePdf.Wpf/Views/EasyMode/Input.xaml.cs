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

namespace MakePdf.Wpf.Views.EasyMode
{
    /// <summary>
    /// Interaction logic for Input.xaml
    /// </summary>
    public partial class Input : UserControl
    {
        readonly string exePath;
        readonly string exeFullPath;
        readonly string startupPath;

        public Input()
        {
            InitializeComponent();

            exePath = Environment.GetCommandLineArgs()[0];
            exeFullPath = Path.GetFullPath(exePath);
            startupPath = Path.GetDirectoryName(exeFullPath);
        }

        // ref: https://qiita.com/Fuhduki/items/447e5707c4fa4c8f532a
        dynamic VM
        {
            get { return DataContext; }
        }

        void ListView_Drop(object sender, DragEventArgs e)
        {
            var dropFileList = (e.Data.GetData(DataFormats.FileDrop) as string[]).ToList();

            VM.AddFiles(dropFileList);
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

        void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog
            {
                Title = "Select Output File",
                IsFolderPicker = false,
                InitialDirectory = startupPath,
                DefaultDirectory = startupPath,
            };

            dialog.Filters.Add(new CommonFileDialogFilter("PDF File", "*.pdf"));
            dialog.Filters.Add(new CommonFileDialogFilter("All File", "*.*"));

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                VM.AddOutputFile(dialog.FileName);
            }
        }
    }
}
