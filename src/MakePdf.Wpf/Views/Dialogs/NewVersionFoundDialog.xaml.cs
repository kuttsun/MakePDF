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
    public enum YesNo
    {
        Yes,
        No
    }

    /// <summary>
    /// Interaction logic for TwoButtonDialog.xaml
    /// </summary>
    public partial class NewVersionFoundDialog : UserControl
    {
        public NewVersionFoundDialog()
        {
            InitializeComponent();
        }

        public NewVersionFoundDialog(string newVersion) : this()
        {
            labelCurrentVersion.Content = MyInformation.Instance.AssemblyInformationalVersion;
            labelNewVersion.Content = newVersion;
        }
    }
}
