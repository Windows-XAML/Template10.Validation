using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Template10.Controls.Validation;
using Template10.Interfaces.Validation;
using Windows.Foundation.Collections;
using System.ComponentModel.DataAnnotations.Schema;

namespace Template10.Validation
{
    public abstract class ValidatableModelBase : IValidatableModel, INotifyPropertyChanged
    {
        /*I think this is where the bug is found because this ctor. keeps calling Validate Method
         wheter you set the validateAfter property, so I suggest to remove it from here.*/
        public ValidatableModelBase()
        {
            Properties.MapChanged += (s, e) =>
            {
                if (e.CollectionChange.Equals(CollectionChange.ItemInserted))
                    Properties[e.Key].ValueChanged += (sender, args) =>
                    {
                        RaisePropertyChanged(e.Key);
                        //Validate(); //You call RaisePropertyChanged inside Validate Method why Twice?
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
                Validate(validateAfter);
            }
        }

        public bool Validate(bool validateAfter = true)
        {
            if (validateAfter)
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
            }
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

        [NotMapped]
        [JsonIgnore]
        public ObservableDictionary<string, IProperty> Properties { get; }
            = new ObservableDictionary<string, IProperty>();

        [NotMapped]
        [JsonIgnore]
        public ObservableCollection<string> Errors { get; }
            = new ObservableCollection<string>();

        [NotMapped]
        [JsonIgnore]
        public Action<IValidatableModel> Validator { set; get; }

        [NotMapped]
        [JsonIgnore]
        public bool IsValid => !Errors.Any();

        [NotMapped]
        [JsonIgnore]
        public bool IsDirty => Properties.Any(x => x.Value.IsDirty);

        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
