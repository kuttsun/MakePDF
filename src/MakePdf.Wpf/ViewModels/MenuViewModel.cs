using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Prism.Commands;
using Prism.Mvvm;

using MakePdf.Wpf.Models;

namespace MakePdf.Wpf.ViewModels
{
    class MenuViewModel : BindableBase
    {
        Updater updater;

        public MenuViewModel()
        {
            updater = Updater.Instance;
        }

        public async Task<string> CheckForUpdate()
        {
            return await updater.CheckForUpdate();
        }
    }
}
