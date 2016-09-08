using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Utils;
using Windows.UI.Xaml.Navigation;

namespace Template10.Samples.ValidationSample.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        Services.UserService.UserService _userService;

        public MainPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                // designtime
            }
            else
            {
                _userService = new Services.UserService.UserService();
            }
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            Users.AddRange(_userService.GetUsers());
            await Task.CompletedTask;
        }

        public ObservableCollection<Models.User> Users { get; } = new ObservableCollection<Models.User>();

        Models.User _Selected = default(Models.User);
        public Models.User Selected { get { return _Selected; } set { Set(ref _Selected, value); } }

        public void DeleteSelectedUser()
        {
            if (Selected != null)
            {
                _userService.DeleteUsers(Selected.Id);
                Users.Remove(Selected);
                Selected = null;
            }
        }

        public void CreateAndSelectUser()
        {
            var id = Users.Max(x => x.Id);
            Users.Add(Selected = _userService.CreateUser(id + 1));
        }
    }
}
