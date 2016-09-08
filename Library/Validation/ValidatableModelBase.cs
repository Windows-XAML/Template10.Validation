using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Template10.Controls.Validation;
using Template10.Interfaces.Validation;
using Windows.Foundation.Collections;

namespace Template10.Validation
{
    public abstract class ValidatableModelBase : IValidatableModel, INotifyPropertyChanged
    {
        public ValidatableModelBase()
        {
            Properties.MapChanged += (s, e) =>
            {
                if (e.CollectionChange.Equals(CollectionChange.ItemInserted))
                    Properties[e.Key].ValueChanged += (sender, args) =>
                    {
                        RaisePropertyChanged(e.Key);
                        Validate();
                        RaisePropertyChanged(nameof(IsDirty));
                        RaisePropertyChanged(nameof(IsValid));
                    };
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected T Read<T>([CallerMemberName]string propertyName = null)
        {
            if (!Properties.ContainsKey(propertyName))
                Properties.Add(propertyName, new Property<T>());
            return (Properties[propertyName] as IProperty<T>).Value;
        }

        protected void Write<T>(T value, [CallerMemberName]string propertyName = null, bool validateAfter = true)
        {
            if (!Properties.ContainsKey(propertyName))
                Properties.Add(propertyName, new Property<T>());
            var property = (Properties[propertyName] as IProperty<T>);
            var previous = property.Value;
            if (!property.IsOriginalSet || !Equals(value, previous))
            {
                property.Value = value;
                if (validateAfter) Validate();
            }
        }

        public bool Validate()
        {
            foreach (var property in Properties)
            {
                property.Value.Errors.Clear();
            }
            Validator?.Invoke(this);
            Errors.Clear();
            foreach (var error in Properties.Values.SelectMany(x => x.Errors))
            {
                Errors.Add(error);
            }
            RaisePropertyChanged(nameof(IsValid));
            return IsValid;
        }

        public void Revert()
        {
            foreach (var property in Properties)
            {
                property.Value.Revert();
            }
            Validate();
        }

        public void MarkAsClean()
        {
            foreach (var property in Properties)
            {
                property.Value.MarkAsClean();
            }
            Validate();
        }

        [JsonIgnore]
        public ObservableDictionary<string, IProperty> Properties { get; }
            = new ObservableDictionary<string, IProperty>();

        [JsonIgnore]
        public ObservableCollection<string> Errors { get; }
            = new ObservableCollection<string>();

        [JsonIgnore]
        public Action<IValidatableModel> Validator { set; get; }

        [JsonIgnore]
        public bool IsValid => !Errors.Any();

        [JsonIgnore]
        public bool IsDirty { get; }

        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
