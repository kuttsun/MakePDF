using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

using MakePdf.Wpf.Properties;

namespace MakePdf.Wpf
{
    // see. http://grabacr.net/archives/1647
    public class ResourceService : INotifyPropertyChanged
    {
        public static ResourceService Current { get; } = new ResourceService();
        public Resources Resources { get; } = new Resources();

        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
        
        public void ChangeCulture(string name)
        {
            if (name == null) {
                Resources.Culture = CultureInfo.DefaultThreadCurrentCulture;
            }
            else { 
                Resources.Culture = CultureInfo.GetCultureInfo(name);
            }
            RaisePropertyChanged(nameof(Resources));
        }
    }
}
