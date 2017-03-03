using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using MvvmSample.TypeExtentions;

namespace BankOk.Controls.MaskedTextBox
{
    public class MaskedTextBox : TextBox
    {
        #region DependencyProperties

        public static readonly DependencyProperty UnmaskedTextProperty = DependencyProperty.Register(
            "UnmaskedText", typeof(string), typeof(MaskedTextBox), new PropertyMetadata(default(string), UnmaskedTextPropertyChangedCallback));

        private static void UnmaskedTextPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var mtb = dependencyObject as MaskedTextBox;
            if (mtb == null)
                return;

            if (mtb.Provider == null)
            {
                mtb.InitProvider();
            }

            var newText = e.NewValue.ToString();
            
            if (newText == mtb.Provider.ToString(false, false))
            {
                if (string.IsNullOrEmpty(newText)) // remove formatting if has not space symbols in mask
                    mtb.InternalText = string.Empty;
                return;
            }

            mtb.Provider.Set(newText);
            if (string.IsNullOrEmpty(newText)) // remove formatting if has not space symbols in mask
                mtb.InternalText = string.Empty;
            else
                mtb.Text = mtb.Provider.ToDisplayString();
        }

        public string UnmaskedText
        {
            get { return (string)GetValue(UnmaskedTextProperty); }
            set { SetValue(UnmaskedTextProperty, value); }
        }

        public static readonly DependencyProperty InputMaskProperty =
         DependencyProperty.Register("InputMask", typeof(string), typeof(MaskedTextBox), null);

        public string InputMask
        {
            get { return (string)GetValue(InputMaskProperty); }
            set { SetValue(InputMaskProperty, value); }
        }

        public static readonly DependencyProperty PromptCharProperty =
         DependencyProperty.Register("PromptChar", typeof(string), typeof(MaskedTextBox),
         new PropertyMetadata('_'));

        public string PromptChar
        {
            get { return (string)GetValue(PromptCharProperty); }
            set { SetValue(PromptCharProperty, value); }
        }

        #endregion

        private MaskedTextProvider Provider;
        private string internalText;
        protected string InternalText
        {
            get { return internalText; }
            set
            {
                internalText = value;
                Text = internalText;
            }
        }

        public MaskedTextBox()
        {
            Loaded += MaskedTextBox_Loaded;
            KeyDown += OnKeyDown;
            TextChanging += OnTextChanging;
            GotFocus += OnGotFocus;
            SelectionChanged += OnSelectionChanged;
        }

        private int internalSelectionStart;
        private int internalSelectionLength;
        private void OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            internalSelectionStart = SelectionStart;
            internalSelectionLength = SelectionLength;
        }

        private void OnGotFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            var index = string.IsNullOrEmpty(InternalText) ? 0 : InternalText.Length;
            var position = GetNextCharacterPosition(index);
            SelectionStart = position;
        }

        private void OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            
            switch (e.Key)
            {
                case VirtualKey.Space:
                    TreatSelectedText();
                    var position = this.GetNextCharacterPosition(SelectionStart);
                    if (this.Provider.InsertAt(" ", position))
                        this.RefreshText(position);
                    e.Handled = true;
                    break;
                case VirtualKey.Back:
                    if(SelectionStart <= 0 || string.IsNullOrEmpty(UnmaskedText))
                        break;

                    if (internalSelectionLength > 0) // has selected text
                    {
                        Provider.RemoveAt(SelectionStart, SelectionStart + internalSelectionLength - 2);
                        RefreshText(SelectionStart);
                        e.Handled = true;
                        break;
                    }

                    if (!Provider.IsEditPosition(internalSelectionStart - 1)) // remove not editable symbol
                    {
                        var pos = Provider.FindEditPositionFrom(internalSelectionStart - 1, false);
                        Provider.RemoveAt(pos);
                        RefreshText(pos);
                        e.Handled = true;
                    }
                    break;
            }
        }

        private void OnTextChanging(TextBox sender, TextBoxTextChangingEventArgs e)
        {
            if (InternalText.Equals(Text))
            {
                return;
            }

            if (Text == string.Empty) // clear control data
            {
                Provider.Set(string.Empty);
                RefreshText(0);
                return;
            }

            var delta = Text.GetDiff(internalText);
            
            var position = GetNextCharacterPosition(Math.Max(SelectionStart - delta.Length, 0));
            if (Provider.Replace(delta, position))
                position += delta.Length;

            position = GetNextCharacterPosition(position);
            RefreshText(position);
        }

        void MaskedTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            InitProvider();
        }

        private void InitProvider()
        {
            if(Provider != null)
                return;

            Provider = new MaskedTextProvider(InputMask, CultureInfo.CurrentCulture);
            Provider.PromptChar = PromptChar[0];

            if (string.IsNullOrWhiteSpace(UnmaskedText))
            {
                Provider.Set(string.Empty);
                InternalText = string.Empty;
            }
            else
            {
                Provider.Set(UnmaskedText);
                InternalText = Provider.ToDisplayString();
            }
        }

        private bool TreatSelectedText()
        {
            if (internalSelectionLength > 0)
            {
                return Provider.RemoveAt(SelectionStart,
                                              SelectionStart + internalSelectionLength - 1);
            }
            return false;
        }

        private void RefreshText(int position)
        {
            SetText(this.Provider.ToDisplayString());
            UnmaskedText = Provider.ToString(false, false);
            SelectionStart = Math.Max(position, 0);
        }

        private void SetText(string text)
        {
            InternalText = string.IsNullOrWhiteSpace(text) ? string.Empty : text;
        }

        private int GetNextCharacterPosition(int startPosition)
        {
            var position = this.Provider.FindEditPositionFrom(startPosition, true);

            if (position == -1)
                return startPosition;
            else
                return position;
        }
    }
}
