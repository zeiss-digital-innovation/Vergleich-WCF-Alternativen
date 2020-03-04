using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using SharedLibrary;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading;

namespace WPFClient.ViewModel
{
    public class MainViewModel : ViewModelBase, IDataErrorInfo
    {
        // Connecting to server
        static HttpClient client = new HttpClient();

        // With Nuget Package PropertyChanged.Fody is no need to define RaisePropertyChanged
        public string Username { get; set; }
        public string ChatText { get; set; }
        public string Userlist { get; set; } = "Teilnehmer";
        public ObservableCollection<Message> Messages { get; set; }
        public string WindowTitle { get; set; }
        public Visibility LoginVisibility { get; set; }
        public Visibility ChatViewVisibility { get; set; }
        public ICommand LoginCommand { get; private set; }
        public ICommand SendCommand { get; private set; }
        public ICommand LogOutCommand { get; private set; }

        // Error Handler
        public string Error => string.Empty;

        public string this[string propertyName]
        {
            get
            {
                switch (propertyName)
                {
                    case nameof(Username):
                        if (Username == "")
                            return "Username is required!";
                        break;
                }
                return string.Empty;
            }
        }

        public MainViewModel()
        {
            LoginVisibility = Visibility.Visible;
            ChatViewVisibility = Visibility.Collapsed;

            LoginCommand = new RelayCommand(LoginMethod);
            SendCommand = new RelayCommand(SendMethod);

            // Setup Server Address
            client.BaseAddress = new Uri("https://localhost:5001");
            MediaTypeWithQualityHeaderValue contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);

            if (IsInDesignMode)
            {
                WindowTitle = "Chat Application (Design)";
            }
            else
            {
                WindowTitle = "Chat Application";
            }

            Application.Current.MainWindow.Closing += new CancelEventHandler(MainWindow_Closing);
        }

        public void LoginMethod()
        {
            if (!string.IsNullOrEmpty(Username))
            {
                LoginVisibility = Visibility.Collapsed;
                ChatViewVisibility = Visibility.Visible;

                Join();

                Task.Run(() => GetMessagesAsync());
                Task.Run(() => GetUsersAsync());
            }
        }

        private void Join()
        {
            var data = JsonConvert.SerializeObject(Username);
            var content = new StringContent(data, System.Text.Encoding.UTF8, "application/json");

            client.PostAsync("api/Users", content);
        }

        public async Task GetMessagesAsync()
        {
            while (true)
            {
                var response = client.GetAsync("/api/Messages").Result;
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    var history = JsonConvert.DeserializeObject<List<Message>>(data);
                    Messages = new ObservableCollection<Message>(history);
                }
                // Refresh Chat List every 0.5 second
                await Task.Delay(TimeSpan.FromSeconds(0.5), CancellationToken.None);
            }
        }

        public async Task GetUsersAsync()
        {
            while (true)
            {
                var response = client.GetAsync("/api/Users").Result;
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    var users = JsonConvert.DeserializeObject<List<User>>(data);
                    Userlist = users.Count + " Teilnehmer: ";
                    foreach (var user in users)
                        Userlist += user.Name + " ";
                }
                // Refresh User List every 2 second
                await Task.Delay(TimeSpan.FromSeconds(2), CancellationToken.None);
            }
        }

        public void SendMethod()
        {
            if (!string.IsNullOrEmpty(ChatText))
            {
                var s = Username + "," + ChatText;
                var data = JsonConvert.SerializeObject(s);
                var content = new StringContent(data, System.Text.Encoding.UTF8, "application/json");

                client.PostAsync("api/Messages", content);
            }
            ChatText = string.Empty;
        }

        public void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to close the application?", "Chat Application", MessageBoxButton.YesNo) == MessageBoxResult.No)
                e.Cancel = true;
            else
            {
                if (!string.IsNullOrEmpty(Username))
                {
                    client.DeleteAsync("api/Users/" + Username);
                }
            }
        }
    }
}
