using Windows.UI.Xaml.Controls;
using MvvmSample.ViewModels;

namespace MvvmSample.Views
{
    public abstract class MainPageBase : PageBase<MainPageViewModel>
    {

    }


    public sealed partial class MainPage  : MainPageBase
    {
        public MainPage()
        {
            this.InitializeComponent();
        }
    }
}
