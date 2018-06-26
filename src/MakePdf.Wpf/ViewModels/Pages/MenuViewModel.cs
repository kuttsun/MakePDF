using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Microsoft.Extensions.DependencyInjection;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Events;
using Prism.Regions;

using MakePdf.Wpf.Models;
using MakePdf.Wpf.Views.Pages;

namespace MakePdf.Wpf.ViewModels.Pages
{
    class MenuViewModel : BindableBase
    {
        readonly IRegionManager _regionManager;

        Updater updater;
        public DelegateCommand LoadedCommand { get; }

        public ObservableCollection<MenuItem> RecentFiles { get; set; } = new ObservableCollection<MenuItem>();

        public MenuViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            Messenger.Instance[MessengerType.UpdateRecentFiles].GetEvent<PubSubEvent<string>>().Subscribe(x =>
            {
                UpdateRecentFiles();
            });

            UpdateRecentFiles();

            updater = Service.Provider.GetService<Updater>();

            LoadedCommand = new DelegateCommand(async () =>
            {
                var ret = await updater.CheckForUpdates();

                if (ret != null)
                {
                    Messenger.Instance[MessengerType.NewVersionFound].GetEvent<PubSubEvent<string>>().Publish(ret);
                }
            });
        }

        public void UpdateRecentFiles()
        {
            var options = Service.Provider.GetService<Options>();

            if (options?.UserSetting?.RecentFiles?.Count > 0)
            {
                RecentFiles.Clear();

                foreach (var file in options.UserSetting.RecentFiles)
                {
                    RecentFiles.Insert(0,
                        new MenuItem
                        {
                            Title = file,
                            Command = new DelegateCommand(() =>
                            {
                                Messenger.Instance[MessengerType.ReadSettingFile].GetEvent<PubSubEvent<string>>().Publish(file);
                            })
                        });
                }
            }
        }

        public async Task<string> CheckForUpdate()
        {
            return await updater.CheckForUpdates();
        }

        public async Task<bool> PrepareForUpdate()
        {
            return await updater.PrepareForUpdates();
        }

        public void ReadSettingFile(string file)
        {
            var parameters = new NavigationParameters
            {
                { "file", file }
            };
            // see https://field-notes.sakura.ne.jp/pgmemo/microsoft/dotnet/wpf/samples#k74p16
            _regionManager.RequestNavigate(nameof(Region.MainRegion), nameof(StandardMode), parameters);
        }
    }

    public class MenuItem
    {
        public string Title { get; set; }
        public DelegateCommand Command { get; set; }
    }
}
