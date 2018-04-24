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
    public enum SupportedCulture
    {
        Auto,
        en,
        ja,
    }

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

        public void ChangeCulture(SupportedCulture culture)
        {
            if (culture == SupportedCulture.Auto)
            {
                Resources.Culture = CultureInfo.DefaultThreadCurrentCulture;
            }
            else
            {
                Resources.Culture = CultureInfo.GetCultureInfo(culture.ToString());
            }
            RaisePropertyChanged(nameof(Resources));
        }

        public SupportedCulture GetCulture()
        {
            return (SupportedCulture)Enum.Parse(typeof(SupportedCulture), Resources.Culture.Name, true);
        }
    }
}
