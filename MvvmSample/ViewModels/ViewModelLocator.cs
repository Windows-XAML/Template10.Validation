using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

namespace MvvmSample.ViewModels
{
    public class ViewModelLocator
    {
        private IUnityContainer unityContainer;

        public ViewModelLocator()
        {
            unityContainer = new UnityContainer();
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(unityContainer));

            unityContainer.RegisterType<MainPageViewModel>();
        }

        public MainPageViewModel MainPageViewModel => unityContainer.Resolve<MainPageViewModel>();
    }
}
