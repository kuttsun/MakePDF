using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Prism.Commands;
using Prism.Mvvm;

using MakePdf.Wpf.Models;

namespace MakePdf.Wpf.ViewModels
{
    class MenuViewModel : BindableBase
    {
        Updater updater;
        public DelegateCommand LoadedCommand { get; }

        public MenuViewModel()
        {
            updater = Updater.Instance;

            LoadedCommand = new DelegateCommand(async () =>
            {
                var ret = await updater.CheckForUpdate();

                Debug.WriteLine(ret);
            });
        }

        public async Task<string> CheckForUpdate()
        {
            return await updater.CheckForUpdate();
        }

        public async Task<bool> PrepareForUpdate()
        {
            return await updater.PrepareForUpdate();
        }
    }
}
