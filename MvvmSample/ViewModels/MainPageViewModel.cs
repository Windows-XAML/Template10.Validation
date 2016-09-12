using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Command;
using MvvmSample.Helpers;
using MvvmSample.Models;
using Template10.Common;
using Template10.Services.NavigationService;

namespace MvvmSample.ViewModels
{
    public sealed class MainPageViewModel : ViewModelBase
    {
        public override Task OnNavigatedToAsync (object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            // Model validation complete 
            NameModel.OnValidateCompleted += NameModelOnOnValidateCompleted;

            if (state.Any())
            {
                NameModel.LastName = state[nameof(NameModel.LastName)]?.ToString();
                NameModel.FirstName = state[nameof(NameModel.FirstName)]?.ToString();
                NameModel.MidName = state[nameof(NameModel.MidName)]?.ToString();
                NameModel.Gender = SerializeHelpers.Deserialize(state[nameof(NameModel.Gender)]?.ToString(), Gender.Male);
                NameModel.IsSimpleIdentify = SerializeHelpers.Deserialize(state[nameof(NameModel.IsSimpleIdentify)]?.ToString(), true);
                NameModel.BirthDate = SerializeHelpers.Deserialize(state[nameof(NameModel.BirthDate)]?.ToString(), DateTimeOffset.Now.AddYears(-25));
                NameModel.PasportSeriesAndNumber = state[nameof(NameModel.PasportSeriesAndNumber)]?.ToString();
                NameModel.PassportIssueDate = SerializeHelpers.Deserialize(state[nameof(NameModel.PassportIssueDate)]?.ToString(), DateTimeOffset.Now);
                NameModel.SnilsNumber = state[nameof(NameModel.SnilsNumber)]?.ToString();
            }
            else
            {
                // force check model
                NameModel.Validate();
            }

            return Task.CompletedTask;
        }
        
        public override Task OnNavigatedFromAsync (IDictionary<string, object> suspensionState, bool suspending)
        {
            if (suspending)
            {
                suspensionState[nameof(NameModel.LastName)] = NameModel.LastName;
                suspensionState[nameof(NameModel.FirstName)] = NameModel.FirstName;
                suspensionState[nameof(NameModel.MidName)] = NameModel.MidName;
                suspensionState[nameof(NameModel.Gender)] = SerializeHelpers.Serialize(NameModel.Gender);
                suspensionState[nameof(NameModel.IsSimpleIdentify)] = SerializeHelpers.Serialize(NameModel.IsSimpleIdentify);
                suspensionState[nameof(NameModel.BirthDate)] = SerializeHelpers.Serialize(NameModel.BirthDate);
                suspensionState[nameof(NameModel.PasportSeriesAndNumber)] = NameModel.PasportSeriesAndNumber;
                suspensionState[nameof(NameModel.PassportIssueDate)] = SerializeHelpers.Serialize(NameModel.PassportIssueDate);
                suspensionState[nameof(NameModel.SnilsNumber)] = NameModel.SnilsNumber;
            }

            NameModel.OnValidateCompleted -= NameModelOnOnValidateCompleted;

            return Task.CompletedTask;
        }

        public DateTimeOffset MaxDate => DateTimeOffset.Now;

        private void NameModelOnOnValidateCompleted(object sender, bool e)
        {
            RegisterUserCommand.RaiseCanExecuteChanged();
        }

        public SetNameModel NameModel { get; } = SetNameModel.Instance();

        protected override void LockUi()
        {
            IsRegisterUserCommandCanExecute = false;
        }

        protected override void UnlockUi()
        {
            IsRegisterUserCommandCanExecute = true;
        }

        #region RegisterUserCommand

        private bool isRegisterUserCommandCanExecute = true;

        private bool IsRegisterUserCommandCanExecute
        {
            get { return isRegisterUserCommandCanExecute; }
            set
            {
                if (isRegisterUserCommandCanExecute != value)
                {
                    isRegisterUserCommandCanExecute = value;
                    RegisterUserCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private RelayCommand registerUserCommand;
        public RelayCommand RegisterUserCommand => registerUserCommand ?? (registerUserCommand = new RelayCommand(async () => await ExecuteRegisterUserAsync(), CanExecuteRegisterUser));

        private async Task ExecuteRegisterUserAsync()
        {
            LockUi();

            // long running register command
            await Task.Delay(3000);

            UnlockUi();
        }

        private bool CanExecuteRegisterUser()
        {
            var retVal = isRegisterUserCommandCanExecute && NameModel.IsValid; // command can execute && all input values are valid
            IsCommandBarOpen = retVal;
            return retVal;
        }

        #endregion

        private bool isCommandBarOpen;

        public bool IsCommandBarOpen
        {
            get { return isCommandBarOpen; }
            set { Set(ref isCommandBarOpen, value); }
        }

        
    }
}
