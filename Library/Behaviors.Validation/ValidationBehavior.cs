using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Interfaces.Validation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace Template10.Validation.Behaviors
{
    [TypeConstraint(typeof(Control))]
    public class ValidationBehavior : DependencyObject, IBehavior
    {
        public DependencyObject AssociatedObject { get; private set; }
        Control Control => AssociatedObject as Control;

        public void Attach(DependencyObject associatedObject)
        {
            AssociatedObject = associatedObject;
            Control.DataContextChanged += Control_DataContextChanged;
            if (Control.DataContext != null)
            {
                Setup();
            }
        }

        public void Detach() => TearDown();

        private void Control_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args) => Setup();

        private void Setup()
        {
            TearDown();
            GetProperty().PropertyChanged += Property_PropertyChanged;
            ExecuteActions();
        }

        private void TearDown()
        {
            GetProperty(false).PropertyChanged -= Property_PropertyChanged;
            Control.DataContextChanged -= Control_DataContextChanged;
        }

        private void Property_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ExecuteActions();
        }

        private void ExecuteActions()
        {
            if (GetProperty().IsValid)
            {
                Interaction.ExecuteActions(AssociatedObject, WhenValidActions, null);
            }
            else
            {
                Interaction.ExecuteActions(AssociatedObject, WhenInvalidActions, null);
            }
        }

        IProperty _property;
        IProperty GetProperty(bool throwException = true)
        {
            if (_property != null)
                return _property;
            var context = (AssociatedObject as Control)?.DataContext;
            if (context == null)
                if (throwException) throw new NullReferenceException("AssociatedObject is not valid");
                else return null;
            if (string.IsNullOrEmpty(PropertyName))
                if (throwException) throw new NullReferenceException("PropertyName is not set");
                else return null;
            var model = context as IValidatableModel;
            if (model == null)
                if (throwException) throw new NullReferenceException("DataContext is not IModel");
                else return null;
            if (!model.Properties.ContainsKey(PropertyName))
                if (throwException) throw new KeyNotFoundException("PropertyName is not found");
                else return null;
            try
            {
                return _property = model.Properties[PropertyName];
            }
            catch
            {
                if (throwException) throw;
                else return null;
            }
        }

        public string PropertyName
        {
            get { return (string)GetValue(PropertyNameProperty); }
            set { SetValue(PropertyNameProperty, value); }
        }
        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.Register(nameof(PropertyName), typeof(string),
                typeof(ValidationBehavior), new PropertyMetadata(string.Empty, PropertyNameChanged));
        private static void PropertyNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                if (!string.IsNullOrEmpty(e.OldValue?.ToString() ?? string.Empty))
                {
                    throw new InvalidOperationException("PropertyName cannot be changed once set.");
                }
            }
        }

        public ActionCollection WhenValidActions
        {
            get
            {
                var actions = GetValue(WhenValidActionsProperty) as ActionCollection;
                if (actions == null)
                {
                    SetValue(WhenValidActionsProperty, actions = new ActionCollection());
                }
                return actions;
            }
        }
        public static readonly DependencyProperty WhenValidActionsProperty =
            DependencyProperty.Register(nameof(WhenValidActions), typeof(ActionCollection),
                typeof(ValidationBehavior), new PropertyMetadata(null));

        public ActionCollection WhenInvalidActions
        {
            get
            {
                var actions = GetValue(WhenInvalidActionsProperty) as ActionCollection;
                if (actions == null)
                {
                    SetValue(WhenInvalidActionsProperty, actions = new ActionCollection());
                }
                return actions;
            }
        }
        public static readonly DependencyProperty WhenInvalidActionsProperty =
            DependencyProperty.Register(nameof(WhenInvalidActions), typeof(ActionCollection),
                typeof(ValidationBehavior), new PropertyMetadata(null));
    }
}
