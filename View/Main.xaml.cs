using openexchangerates_api.Model;
using openexchangerates_api.ViewModel;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace openexchangerates_api.View
{
    public partial class Main : Window
    {
        public MainVM VM;
        public Main()
        {
            InitializeComponent();
            VM = new MainVM();
            DataContext = VM;
        }
        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = (TextBox) sender;

            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = (TextBox) sender;
            string newText = textBox.Text.Insert(textBox.CaretIndex, e.Text);

            if (!IsValidInput(newText))
            {
                e.Handled = true;
            }
        }
        private bool IsValidInput(string input) => DoubleString().IsMatch(input);
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.DataContext is TabPick tabPick)
            {
                foreach (var tab in VM.Tabs)
                {
                    if(tab == tabPick)
                    {
                        tabPick.IsTextBoxFocused = true;
                    }
                }
            }
        }
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.DataContext is TabPick tabPick)
            {
                foreach (var tab in VM.Tabs)
                {
                    if (tab == tabPick)
                    {
                        tabPick.IsTextBoxFocused = false;
                    }
                }
            }
        }
        private void TextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.SelectAll();
                e.Handled = true;
            }
        }

        [GeneratedRegex("^(\\d+)?(\\.\\d*)?$|^(\\d*\\.)?\\d+$")]
        private static partial Regex DoubleString();

        private void ExtenderButton_Click(object sender, RoutedEventArgs e)
        {
            VM.HandleButtonClick();
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            List<CCode> ints = new();
            foreach (var item in VM.Tabs)
            {
                ints.Add(item.Selected.Code);
            }
            App.SettingsData.Ints = ints;
            App.Save();
        }
        private void TryUpdateData(object sender, RoutedEventArgs e)
        {
            MainVM.TryUpdateData();
        }
    }
}
