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
    public enum Selected
    {
        // e.g. Yes, OK
        Affirmative,
        // e.g. No, Cancel
        Negative
    }

    /// <summary>
    /// Interaction logic for TwoButtonDialog.xaml
    /// </summary>
    public partial class TwoButtonDialog : UserControl
    {
        public TwoButtonDialog()
        {
            InitializeComponent();
        }

        public TwoButtonDialog(string title, string message, string affirmativeButtonText, string negativeButtonText) : this()
        {
            labelTitle.Content = title;
            labelMessage.Content = message;
            affirmativeButton.Content = affirmativeButtonText;
            negativeButton.Content = negativeButtonText;
        }
    }
}
