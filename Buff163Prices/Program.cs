using System;
using System.IO;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace Buff163Prices
{
    class Program
    {
        private static readonly double conversion = 11.2271715;
        private static readonly string url = "https://buff.163.com/api/market/goods/sell_order?game=csgo&goods_id=";

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            string[] itemNames = { "Stockholm 2021 Legends Sticker Capsule", "Stockholm 2021 Contenders Sticker Capsule", "Stockholm 2021 Challengers Sticker Capsule" };

            foreach (string itemName in itemNames)
            {
                string itemID = FindId(itemName, "IDs.txt");

                if (itemID != null)
                {
                    double itemPrice = GetPrice(itemID);
                    Console.WriteLine($"{itemName}: {itemPrice}¥ ({(itemPrice * conversion).ToString("C2")})");
                }
                else
                {
                    Console.WriteLine($"{itemName} not found.");
                }
            }

            Console.ReadKey();
        }

        static string FindId(string itemName, string filePath)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] values = line.Split(';');

                    if (values[1].ToLower().Contains(itemName.ToLower()))
                    {
                        return values[0];
                    }
                }
            }

            return null;
        }

        static double GetPrice(string id)
        {
            using (HttpClient client = new HttpClient())
            {
                string responseString = client.GetStringAsync(url + id).Result;
                JObject responseJson = JObject.Parse(responseString);
                JObject dataJson = responseJson["data"]["items"][0] as JObject;
                double price = (double)dataJson["price"];

                return price;
            }
        }
    }
}
