using System;
using System.Windows.Media.Imaging;

namespace openexchangerates_api.Model
{
    public class Country
    {
        public string Name { get; set; }
        public CCode Code { get; set; }
        public BitmapImage Bitmap { get; set; }
        public double Rate { get; set; }
        public Country(CCode code)
        {
            Name = code.ToString();
            Code = code;
            Rate = GetRate(code);
            Bitmap = UpdateFlagImage(code);
        }
        private static BitmapImage UpdateFlagImage(CCode code)
        {
            var countryCode = code.ToString();
            var flag = "/Resources/Flags-png/" + countryCode + ".png";
            var bitmapImage = new BitmapImage(new Uri(flag, UriKind.Relative));

            return bitmapImage;
        }
        private static double GetRate(CCode code)
        {
            App.Data.Rates.TryGetValue(code.ToString(), out double rate);
            return rate;
        }
    }
}
