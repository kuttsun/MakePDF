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
using MakePdf.Wpf.Views.Dialogs.Common;
using MakePdf.Wpf.ViewModels.Pages;

namespace MakePdf.Wpf.Views.Pages
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        HomeViewModel vm;
        Shell parentView;

        public Home()
        {
            InitializeComponent();

            vm = DataContext as HomeViewModel;
            parentView = Application.Current.MainWindow as Shell;
        }

        async void Page_Drop(object sender, DragEventArgs e)
        {
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

            vm.ReadSettingFile(dropFileList.FirstOrDefault());
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
    }
}
