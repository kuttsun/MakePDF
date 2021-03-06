﻿using System;
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
    /// <summary>
    /// Interaction logic for ProcessingDialog.xaml
    /// </summary>
    public partial class ProcessingDialog : UserControl
    {
        public ProcessingDialog()
        {
            InitializeComponent();
        }

        public ProcessingDialog(string title, string message) : this()
        {
            labelTitle.Content = title;
            labelMessage.Content = message;
        }
    }
}
