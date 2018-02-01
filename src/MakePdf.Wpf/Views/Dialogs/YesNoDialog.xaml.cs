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

namespace MakePdf.Wpf.Views.Dialogs
{
    /// <summary>
    /// OverwriteDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class YesNoDialog : UserControl
    {
        public YesNoDialog()
        {
            InitializeComponent();
        }

        public YesNoDialog(string title, string message) : this()
        {
            labelTitle.Content = title;
            labelMessage.Content = message;
        }
    }
}
