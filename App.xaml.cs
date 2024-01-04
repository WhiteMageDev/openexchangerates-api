using Newtonsoft.Json;
using openexchangerates_api.Model;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace openexchangerates_api
{
    public partial class App : Application
    {
        private static readonly string filePath = "saves.json";
        private static readonly string filePathSettings = "settings.json";

        private static readonly string AppID = "YOU_APP_ID";

        private static readonly string BaseURI = "https://openexchangerates.org/api/";
        private static readonly string EndURI = "latest.json?app_id=" + AppID;


        public static JsonData Data;
        public static Settings SettingsData;
        public static DateTime LastUpdate;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            bool success = ReadDataFromFile<JsonData>(filePath, out Data);
            if (!success)
            {
                using FileStream fs = File.Create(filePath);
                Data = new JsonData();
            }

            DateTime lastDate = UnixTimeStampToDateTime(Data.Timestamp);

            if (CheckLastUpdate(DateTime.Today, lastDate))
            {
                UpdateDataObject();
            }
            LastUpdate = lastDate;

            bool check = ReadDataFromFile<Settings>(filePathSettings, out SettingsData);
            if (!check)
            {
                using FileStream fs = File.Create(filePathSettings);
                SettingsData = new Settings(true);
            }
        }
        public static void Save()
        {
            SaveUpdatedData(filePathSettings, SettingsData);
        }
        private bool ReadDataFromFile<T>(string path, out T? output) where T : class
        {
            output = default(T);
            try
            {
                string json;
                if (File.Exists(path))
                {
                    using StreamReader file = new(path);
                    {
                        json = file.ReadToEnd();
                    }

                    output = JsonConvert.DeserializeObject<T>(json);

                    if (output != null)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                Trace.WriteLine(ex.Message);
                return false;
            }
            return false;
        }
        private bool CheckLastUpdate(DateTime now, DateTime last)
        {
            if (now.CompareTo(last) == 1)
            {
                return true;
            }
            return false;
        }
        private static async void UpdateDataObject()
        {
            (JsonData? jsonData, string msg) court = await HttpGetAsync(BaseURI + EndURI);
            if (court.jsonData != null)
            {
                Data = court.jsonData;
                SaveUpdatedData(filePath, court.jsonData);
            }
        }
        private static void SaveUpdatedData<T>(string path, T jsonData)
        {
            string jsonString = JsonConvert.SerializeObject(jsonData);
            File.WriteAllText(path, jsonString);
        }
        private static async Task<(JsonData?, string)> HttpGetAsync(string link)
        {
            string msg = string.Empty;
            JsonData? jsonData = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(link);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        jsonData = JsonConvert.DeserializeObject<JsonData>(json);
                    }
                    else
                    {
                        msg = $"HTTP error: {response.StatusCode}";
                    }
                }
            }
            catch (Exception ex)
            {
                msg = $"Exception: {ex.Message}";
            }

            return (jsonData, msg);
        }
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime.Date;
        }
        public static async Task<bool> TryUpdateDataAsync()
        {
            (JsonData? jsonData, string msg) court = await HttpGetAsync(BaseURI + EndURI);
            if (court.jsonData != null)
            {
                Data = court.jsonData;
                SaveUpdatedData(filePath, court.jsonData);
                RestartApplication();
                return true;
            }
            else
            {
                MessageBox.Show($"{court.msg}");
                return false;
            }
        }
        private static void RestartApplication()
        {
            Current.Shutdown();
            System.Windows.Forms.Application.Restart();
        }
    }
}
