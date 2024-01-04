using openexchangerates_api.Model;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace openexchangerates_api.ViewModel
{
    public class TabPick : INotifyPropertyChanged
    {
        private bool _isTextBoxFocused;
        public bool IsTextBoxFocused
        {
            get => _isTextBoxFocused;
            set
            {
                if (_isTextBoxFocused != value)
                {
                    _isTextBoxFocused = value;
                    OnPropertyChanged(nameof(IsTextBoxFocused));
                }
            }
        }
        public bool eventGate = false;
        public static event Action<TabPick> OnValueChanged;
        private double lastRate;
        private Country? _selected;
        public Country? Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                if (ValueString != null)
                {
                    eventGate = true;
                    Value = (_value / lastRate) * Selected?.Rate;
                }
                lastRate = value.Rate;
                OnPropertyChanged(nameof(Selected));
            }
        }
        private string? _valueString;
        public string? ValueString
        {
            get { return _valueString; }
            set
            {
                _valueString = value;
                if (IsTextBoxFocused)
                    Value = StringToDoubleValue(value);
                eventGate = false;
                OnPropertyChanged(nameof(ValueString));
            }
        }
        private double? _value;
        public double? Value
        {
            get { return _value; }
            set
            {
                _value = value;

                if (!eventGate) OnValueChanged?.Invoke(this);
                if (!IsTextBoxFocused)
                    ValueString = DoubleToStringValue(value);

            }
        }
        public TabPick(Country country)
        {
            Selected = country;
        }

        private double StringToDoubleValue(string? value)
        {
            double outVal;
            bool success = double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out outVal);
            if (success)
            {
                return outVal;
            }
            else
            {
                _valueString = "0.00";
                return 0;
            }
        }
        private string? DoubleToStringValue(double? value)
        {
            return value?.ToString("F2", CultureInfo.InvariantCulture);
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
