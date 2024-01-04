using openexchangerates_api.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;

namespace openexchangerates_api.ViewModel
{
    public class MainVM : INotifyPropertyChanged
    {
        public List<Country> Countries { get; set; }
        public List<TabPick> Tabs { get; set; }
        public double? ValueUSD {  get; set; }
        public string LastUpdate { get; set; }
        private bool _isHidden = false;
        public bool IsHidden
        {
            get { return _isHidden; }
            set
            {
                _isHidden = value;
                OnPropertyChanged(nameof(IsHidden));
            }
        }

        public Brush UpdateColor { get; set; }
        public bool IsUpdate { get; set; }
        public MainVM()
        {
            Countries = new List<Country>();
            LastUpdate = $"Last update: {App.LastUpdate.ToShortDateString()}";

            if (App.LastUpdate.Date.CompareTo(DateTime.Today) == 0){
                UpdateColor = Brushes.Green;
                IsUpdate = false;
            }
            else
            {
                UpdateColor = Brushes.Red;
                IsUpdate = true;
            }

            var list = new List<CCode>((CCode[])Enum.GetValues(typeof(CCode)));
            foreach (var item in list)
            {
                Countries.Add(new Country(item));
            }

            List<CCode> ints = App.SettingsData.Ints;
            Tabs = new List<TabPick>()
            {
                new TabPick(Countries.FirstOrDefault(p => p.Code == ints[0])),
                new TabPick(Countries.FirstOrDefault(p => p.Code == ints[1])),
                new TabPick(Countries.FirstOrDefault(p => p.Code == ints[2])),
                new TabPick(Countries.FirstOrDefault(p => p.Code == ints[3])),
                new TabPick(Countries.FirstOrDefault(p => p.Code == ints[4])),
            };

            TabPick.OnValueChanged += TabPick_OnValueChanged;
        }
        public void HandleButtonClick()
        {
            IsHidden = !IsHidden;
        }
        private void TabPick_OnValueChanged(TabPick obj)
        {
            ValueUSD = obj.Value / obj.Selected?.Rate;

            foreach (var other in Tabs)
            {
                if (!other.Equals(obj))
                {
                    if (other.Selected == null) continue;
                    other.eventGate = true;
                    other.Value = ValueUSD * other.Selected?.Rate;
                }
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        internal static async void TryUpdateData()
        {
            await App.TryUpdateDataAsync();
        }
        #endregion
    }
}
