using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using SharedLibrary;
using Microsoft.AspNetCore.SignalR.Client;

namespace WPFClient.ViewModel
{
    public class MainViewModel : ViewModelBase, IDataErrorInfo
    {
        // Connecting to server
        HubConnection connection;

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
            connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:5001/ChatHub")
                .WithAutomaticReconnect()
                .Build();

            // Manual Reconnect if the connection undesirable closed
            connection.Closed += async (error) =>
            {
                MessageBox.Show(error.Message);
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };

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
                InvokeHandler();
                Task.Run(() => Join());
            }
        }

        private void InvokeHandler()
        {
            connection.On<Message>("ReceiveMessage", (message) =>
            {
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Messages.Add(message);
                });
            });

            connection.On<List<Message>>("GetChats", (history) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Messages = new ObservableCollection<Message>(history);
                });
            });

            connection.On<List<User>>("UserlistUpdated", (users) =>
            {
                Userlist = users.Count + " Teilnehmer: ";
                foreach (var user in users)
                    Userlist += user.Name + " ";
            });

        }

        private async void Join()
        {
            try
            {
                await connection.StartAsync();
                LoginVisibility = Visibility.Collapsed;
                ChatViewVisibility = Visibility.Visible;
                await connection.InvokeAsync("Join", Username);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public async void SendMethod()
        {
            if (!string.IsNullOrEmpty(ChatText))
            {
                try
                {
                    await connection.InvokeAsync("SendMessage", Username, ChatText);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
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
                    Task.Run(async () => { await connection.StopAsync(); });
                }
            }
        }
    }
}
