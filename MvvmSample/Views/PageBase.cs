using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight;

namespace MvvmSample.Views
{
    public abstract class PageBase<T> : Page
        where T : ViewModelBase
    {
        protected PageBase()
        {
            DataContextChanged += OnDataContextChanged;

        }

        public T ViewModel { get; private set; }

        protected virtual void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs e)
        {
            var dataContext = e.NewValue as T;
            ViewModel = dataContext;
        }
    }
}
