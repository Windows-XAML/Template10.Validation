using System;
using System.Collections.ObjectModel;
using Template10.Controls.Validation;
using Template10.Mvvm.Validation;

namespace Template10.Interfaces.Validation
{
    public interface IValidatableModel : IBindable
    {
        bool Validate(bool validateAfter);

        void Revert();

        void MarkAsClean();

        ObservableDictionary<string, IProperty> Properties { get; }

        ObservableCollection<string> Errors { get; }

        Action<IValidatableModel> Validator { set; get; }

        bool IsValid { get; }

        bool IsDirty { get; }
    }
}
