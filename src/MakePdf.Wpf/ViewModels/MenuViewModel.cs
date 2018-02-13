using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Prism.Commands;
using Prism.Mvvm;

namespace MakePdf.Wpf.ViewModels
{
    class MenuViewModel : BindableBase
    {
        public async Task<bool> CheckForUpdate()
        {
            await Task.Run(() => Task.Delay(3000));
            return false;
        }
    }
}
