# Validation in Universal Windows Platform

In Windows UWP XAML/C# apps, developers will discover that input controls do not support `DataAnnotation` or `ExceptionValidationRule` or `IDataErrorInfo` or `INotifyDataErrorInfo` or `BindingValidationError`. Even if the platform included these capabilities, there are significant limitations to each that make them a limiting option for application with significantreal-world data validation requirements.

![nuget](https://github.com/Windows-XAML/Template10.Validation/raw/master/Assets/Nuget.png)

## Introducing Template10 Validation

These validation libraries provide UWP developers a comprehensive solution to data validation that solves problems in a realistic and usable way. The associated sample application demonstrates the use and syntax. If you have feedback or a bug to report, do it here: https://github.com/Windows-XAML/Template10/issues. 

## Get started

To get started with validation, your models need to inerit `Template10.Validation.ValidatableModelBase`. This will include an implementation of `INotifyPropertyChanged` in case you need it in your logic. Should your poco classes (models) not support inheriting from a new base class, then this validation library will not work for you. The enhancements to your model(s) will include:

### ValidatableModelBase properties

* `public ObservableCollection<string> Errors { get; }`
* `public bool IsDirty { get; }`
* `public bool IsValid { get; }`
* `public ObservableDictionary<string, IProperty> Properties { get; }`
* `public Action<IValidatableModel> Validator { get; set; }`

### ValidatableModelBase methods

* `public void MarkAsClean();`
* `public void RaisePropertyChanged([CallerMemberName] string propertyName = null);`
* `public void Revert();`
* `public bool Validate();`
* `protected T Read<T>([CallerMemberName] string propertyName = null);`
* `protected void Write<T>(T value, [CallerMemberName] string propertyName = null, bool validateAfter = true);`

In addition to ainheriting from `ValidatableModelBase` both the setter and getters of your model properties will need to change to the following syntax in order to take part in the underlying change tracking mechanism.

#### Your model before

````csharp
public class User 
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public override string ToString() => $"{FirstName} {LastName}";
}
````

#### Your model after

````csharp
public class User : Template10.Validation.ValidatableModelBase
{
    public int Id { get; set; }
    public string FirstName { get { return Read<string>(); } set { Write(value); } }
    public string LastName { get { return Read<string>(); } set { Write(value); } }
    public override string ToString() => $"{FirstName} {LastName}";
}
````

## Adding validation logic

Once you have implemented these changes, you need to set the `Validator` property of your model - typically when they are created or just before they are exposed in your view-model. (Note: Itit is not required that you use the MVVP design pattern to use this library - but if you don't you're probably building your app all wrong). You would set your validator something like this:

````csharp
var user = new User
{
    FirstName = "Jerry",
    LastName = "Nixon",
    Validator = i =>
    {
        var u = i as User;
        if (string.IsNullOrEmpty(u.FirstName))
        {
            u.Properties[nameof(u.FirstName)].Errors.Add("First name is required");
        }
        if (string.IsNullOrEmpty(u.LastName))
        {
            u.Properties[nameof(u.LastName)].Errors.Add("Last name is required");
        }
    },
};
````

Notice in the code above how you are adding one or more errors to the `Errors` property of the model's property; you are not adding to the `Errors` property of the model. This allows errors on a field level. Property errors are automatically progated to the model-level property `Errors`. Any changes to that list will be overwritten.

## Calling validation logic

Once the validation logic is injected into the model, you can call the model's `Validate()` method to assess the current state of the model. You can repeatively call `Validate()` as the workflow in your app requires. If the validation logic has significant cost, calling `Validate()` too frequently could have a performance cost to your app. 

## New properties of your properties

Above you saw how each property has an `Errors` property. In addition, there is a `Value` property and these others:

### Properties

* `ObservableCollection <string> Errors { get; }`
* `bool IsDirty { get; }`
* `bool IsOriginalSet { get; }`
* `bool IsValid { get; }`
* `T Value { get; set; }`
* `T OriginalValue { get; set; }`

### Event(s)

* `event EventHandler ValueChanged;`

### Methods

* `void MarkAsClean();`
* `void Revert();`

## Error indicator

You will see that most of these properties are managed autoamtically by the base class. Calling `MarkAsClean()` or `Revert()` impacts `IsDirty` and `Value` respectively. That being said, you can also use them in tandem with the control wrapper that ships in the library. You would use it like this:

````xaml
<validate:ControlWrapper PropertyName="FirstName">
    <TextBox Width="{StaticResource FieldWidth}"
        Header="First Name"
        Text="{Binding FirstName, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}" />
</validate:ControlWrapper>
````

By default the visual looks like this:

![default look](https://github.com/Windows-XAML/Template10.Validation/raw/master/Assets/DefaultLook.png)

## Customized error indicator

The visual definition of the wrapper is defined in the library (https://github.com/Windows-XAML/Template10.Validation/blob/master/Library/Themes/Generic.xaml) and you can override it simply by changing the `Template` property of the wrapper. Here's how you might do it:

````xaml
<validate:ControlWrapper PropertyName="FirstName">
    <validate:ControlWrapper.Template>
        <ControlTemplate TargetType="validate:ControlWrapper">
            <StackPanel DataContext="{TemplateBinding Property}">
                <TextBlock Text="IsValid" />
                <TextBlock Text="{Binding IsValid}" />
                <TextBlock Text="Errors" />
                <TextBlock Text="{Binding Errors.Count}" />
                <ContentPresenter Content="{TemplateBinding Content}" />
            </StackPanel>
        </ControlTemplate>
    </validate:ControlWrapper.Template>
    <TextBox Width="{StaticResource FieldWidth}"
                Header="First Name"
                Text="{Binding FirstName, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}" />
</validate:ControlWrapper>
````

In the XAML above the `ControlTemplate` is created in-line, but could easily be moved to a resource and re-used in multiple controls. It also demonstrates who every control *could* have a separate look, if desired. The XAML above would render a look something like this: 

![custom look](https://github.com/Windows-XAML/Template10.Validation/raw/master/Assets/CustomLook.png)

I realize that's a silly use of the control, but it demonstrates an easy starting place.

I hope you enjoy Template 10 Validation!
