using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Windows;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        

        private static HttpClient client;

        static void InitiateHttpClient()
        {
            client = new HttpClient();
        }

        public MainWindow()
        {
            InitiateHttpClient();
            InitializeComponent();
            txtRequestHeaders.Text = "ContentType: application/json";
            txtRequestHeaders.IsEnabled = false;
        }


        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            if (txtUrl.Text.Trim() == "")
                MessageBox.Show("URL Error");
            else
                Send(txtUrl.Text.Trim());
        }

        private async void Send(string uri)
        {
            try
            {
                var requestMessage = new HttpRequestMessage
                {
                    RequestUri = new Uri(uri),
                    Method = (HttpMethod)comboMethodType.SelectedItem,
                    Headers = { { HttpRequestHeader.ContentType.ToString(), "application/json" } }
                };
                if ((HttpMethod)comboMethodType.SelectedItem != HttpMethod.Get) { 
                    requestMessage.Content = new StringContent(txtRequestBody.Text.Trim(), Encoding.UTF8, "application/json");
                }
                HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);
                
                txtResponseHeaders.Text = responseMessage.Headers.ToString();
                string responseBody = await responseMessage.Content.ReadAsStringAsync();
                if (responseBody.Trim() != "")
                    txtResponseBody.Text = JToken.Parse(responseBody).ToString(Formatting.Indented);
                else
                    txtResponseBody.Text = "";
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                StringBuilder builder = new StringBuilder();
                txtResponseBody.Text = builder.Append("Message : " + e.Message).ToString();
            }
        }

        private void PopulateMethodTypes(object sender, RoutedEventArgs e)
        {
            //List<string> methodTypes = new List<string> {"GET", "POST", "PUT", "DELETE"};
            List<HttpMethod> methodTypes = new List<HttpMethod> { HttpMethod.Get, HttpMethod.Post, HttpMethod.Put, HttpMethod.Delete };
            comboMethodType.ItemsSource = methodTypes;
            comboMethodType.SelectedIndex = 0;
        }
    }
}