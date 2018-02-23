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

namespace MakePdf.Wpf.Views.Dialogs.Common
{
    /// /// <summary>
    /// Interaction logic for OneButtonDialog.xaml
    /// </summary>
    public partial class OneButtonDialog : UserControl
    {
        public OneButtonDialog()
        {
            InitializeComponent();
        }

        public OneButtonDialog(string title, string message, string buttonText = "OK") : this()
        {
            labelTitle.Content = title;
            labelMessage.Content = message;
            button.Content = buttonText;
        }
    }
}
