using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Template10.Mvvm.Validation
{
    public interface IBindable : INotifyPropertyChanged
    {
        void RaisePropertyChanged([CallerMemberName]string propertyName = null);
    }
}
