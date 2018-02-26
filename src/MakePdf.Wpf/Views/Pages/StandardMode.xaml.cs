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

using MakePdf.Wpf.ViewModels.Pages;

namespace MakePdf.Wpf.Views.Pages
{
    /// <summary>
    /// Interaction logic for StandardMode.xaml
    /// </summary>
    public partial class StandardMode : UserControl
    {
        StandardModeViewModel vm;

        public StandardMode()
        {
            InitializeComponent();

            vm = DataContext as StandardModeViewModel;
        }

        void OutputFile_Drop(object sender, DragEventArgs e)
        {
            var dropFileList = (e.Data.GetData(DataFormats.FileDrop) as string[]).ToList();

            //vm.OutputFile = dropFileList[0];
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
        }

        void StartButton_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
