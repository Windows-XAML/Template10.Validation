using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using Template10.Common;
using Template10.Services.NavigationService;

namespace MvvmSample.ViewModels
{
    public abstract class ViewModelBase : GalaSoft.MvvmLight.ViewModelBase, INavigable
    {
        public virtual Task OnNavigatedToAsync (object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnNavigatedFromAsync (IDictionary<string, object> suspensionState, bool suspending)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnNavigatingFromAsync (NavigatingEventArgs args)
        {
            return Task.CompletedTask;
        }

        public INavigationService NavigationService { get; set; }
        public IDispatcherWrapper Dispatcher { get; set; }
        public IStateItems SessionState { get; set; }

        protected virtual void LockUi()
        {
        }

        protected virtual void UnlockUi()
        {
        }

        private bool isUiLock;
        [JsonIgnore]
        public bool IsUiLock
        {
            get { return isUiLock; }
            set
            {
                if (Set(ref isUiLock, value))
                {
                    if (isUiLock)
                        LockUi();
                    else
                        UnlockUi();
                }
            }
        }
    }
}
