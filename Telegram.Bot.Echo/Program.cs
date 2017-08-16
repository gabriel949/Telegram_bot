using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using File = System.IO.File;
using System.Collections.Generic;
using System.Configuration;
using Google.Apis.Customsearch.v1;
using Google.Apis.Customsearch.v1.Data;

namespace Telegram.Bot.Echo
{
    class Program
    {
        
        
        private static Api Bot = new Api(ConfigurationManager.AppSettings["AccessToken"]);
        private static DataBase db = new DataBase();

        static void Main(string[] args)
        {


            Run().Wait();
            
            
        }

        static string[] Search(string key)
        {
            const string apiKey = "AIzaSyDIm9ZOWD8Zd-2tHy5r3c0R-_XjdEFaXGE";
            const string searchEngineId = "003470263288780838160:ty47piyybua";
            string query = key;
            CustomsearchService customSearchService = new CustomsearchService(new Google.Apis.Services.BaseClientService.Initializer() { ApiKey = apiKey });
            CseResource.ListRequest listRequest = customSearchService.Cse.List(query);
            listRequest.Cx = searchEngineId;
            Search search = listRequest.Execute();
            string[] results = new string[3];
            for (int i = 0; i < 3; i++)
            {
                results[i] = "Title : " + search.Items[i].Title + Environment.NewLine + "Link : " + search.Items[i].Link + Environment.NewLine + Environment.NewLine;
            }
            return results;
        }

        static async Task Run()
        {
            //var Bot = new Api(ConfigurationManager.AppSettings["AccessToken"]);
            //var me = await Bot.GetMe();
            //Console.WriteLine("Hello my name is {0}", me.Username);
            //Bot.SendTextMessage(171731119, "ola");

            var offset = 0;
            var keyb = new ReplyKeyboardMarkup()
            {
                Keyboard = new[] { new[] { "Conceitos adm?" }, new[] { "/SAD" }, new[] { "/SPT" }, new[] { "/SIG" }, new[] { "/CADEIA DE VALOR" }, new[] { "/MODELO DE NEGOCIO" }, new[] { "/MODELO DE GESTAO" }, new[] { "/DESAFIOS DA TI" }, new[] { "/INFLUENCIAS DA TI" }, new[] { "/Activity based costing" }, new[] { "/Aliancas estrategicas" }, new[] { "/Analise SWOT" }, new[] { "/Analise de valor" }, new[] { "/Benchmarking" }, new[] { "/Brainstorming" }, new[] { "/Brand management" }, new[] { "/Breakeven" }, new[] { "/Core competence" }, new[] { "/Cultura organizacional" }, new[] { "/Horizontal Oganization" } },
                OneTimeKeyboard = true,
                ResizeKeyboard = true
            };


            while (true)
            {
                var updates = await Bot.GetUpdates(offset);
                
               
                foreach (var update in updates)
                {
                    if (update.Message.Type == MessageType.TextMessage)
                    {
                        string resposta = db.GetAnswer(update.Message.Text.ToLower());
                        if (!string.IsNullOrEmpty(resposta))
                        {
                            await Bot.SendTextMessage(update.Message.Chat.Id, resposta);
                        }
                        else if (update.Message.Text == "/start")
                        {
                            await Bot.SendTextMessage(update.Message.Chat.Id, "Olá, eu sou o Robo de Consulta sobre Administração.", false, 0, keyb);
                        }

                       

                        else
                        {
                            string[] messages = Search(update.Message.Text.ToString());
                            await Bot.SendTextMessage(update.Message.Chat.Id, "Hm! Ainda não tenho esse conhecimento. Mas talvez estes links ajudem, dê uma olhada:");
                            await Bot.SendTextMessage(update.Message.Chat.Id, messages[0]);
                            await Bot.SendTextMessage(update.Message.Chat.Id, messages[1]);
                            await Bot.SendTextMessage(update.Message.Chat.Id, messages[2]);
                        }


                        await Task.Delay(2000);
                        Console.WriteLine("Echo Message: {0}", update.Message.Text);

                    }

                    offset = update.Id + 1;
                }

                await Task.Delay(1000);
            }
        }
    }
}
