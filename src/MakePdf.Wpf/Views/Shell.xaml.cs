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
using System.Windows.Shapes;

using Microsoft.Extensions.DependencyInjection;

using MakePdf.Wpf.Models;

namespace MakePdf.Wpf.Views
{
    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    public partial class Shell : Window
    {
        public Shell()
        {
            InitializeComponent();
        }

        void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Runner runner = Service.Provider.GetService<Runner>();
            if (runner.IsProcessing)
            {
                e.Cancel = true;
                return;
            }
        }
    }
}
