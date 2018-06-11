using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

using Prism.Events;

using MakePdf.Wpf.ViewModels.Dialogs.Common;

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

            Messenger.Instance[MessengerType.Processing].GetEvent<PubSubEvent<string>>().Subscribe(x =>
            {
                Dispatcher.Invoke(() =>
                {
                    ListBox.Items.Add(x);

                    // see: http://kenzauros.com/blog/follow-last-item-of-wpf-listbox/
                    //var lastIndex = ListBox.Items.Count - 1;
                    //ListBox.ScrollIntoView(ListBox.Items[lastIndex]);

                    // Get ScrollViewer of ListBox
                    // see http://1010029.blogspot.com/2013/01/wpflistbox_26.html
                    if (VisualTreeHelper.GetChild(ListBox, 0) is Border border)
                    {
                        if (border.Child is ScrollViewer listBoxScroll)
                        {
                            listBoxScroll.ScrollToEnd();
                        }
                    }
                });
            });
        }

        public ProcessingDialog(string title, string message) : this()
        {
            labelTitle.Content = title;
            labelMessage.Content = message;
        }
    }
}
