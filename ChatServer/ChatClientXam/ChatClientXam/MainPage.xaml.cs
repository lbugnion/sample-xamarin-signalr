using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ChatClientXam
{
    public partial class MainPage : ContentPage
    {
        private ObservableCollection<Message> _messages;
        private HubConnection _connection;

        public MainPage()
        {
            InitializeComponent();

            _messages = new ObservableCollection<Message>();
        }

        private async Task ConnectToServerAsync()
        {
            try
            {
                var client = new HttpClient();
                var negotiateJson = await client.GetStringAsync("https://lbchatserver.azurewebsites.net/api/negotiate");

                var negotiateInfo = JsonConvert.DeserializeObject<NegotiateInfo>(negotiateJson);

                _connection = new HubConnectionBuilder()
                    .WithUrl(negotiateInfo.Url, options =>
                    {
                        options.AccessTokenProvider = async () => negotiateInfo.AccessToken;
                    })
                    //.ConfigureLogging(builder =>
                    //{
                    //    builder.AddConsole();
                    //})
                    .Build();

                _connection.On<JObject>("newMessage", AddNewMessage);

                await _connection.StartAsync();

                //await _connection.SendAsync("newMessage", "{\"name\" : \"LaurentXam\",\"text\" : \"This is a test\"");

                //if (_connection != null)
                //{
                //    _connection.Stop();
                //    _connection.Dispose();
                //}


            }
            catch (Exception ex)
            {

            }
        }

        private void AddNewMessage(JObject message)
        {
            var name = message.GetValue("name").ToString();
            var text = message.GetValue("text").ToString();

            Debug.WriteLine($"{name} says {text}");
        }

        private async void ConnectButtonClicked(object sender, EventArgs e)
        {
            await ConnectToServerAsync();
        }
    }
}
