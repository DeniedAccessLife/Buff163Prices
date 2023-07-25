using System;
using System.IO;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Globalization;

namespace Buff163Prices
{
    class Program
    {
        private static readonly CultureInfo currentCulture = CultureInfo.CurrentCulture;
        private static readonly RegionInfo regionInfo = new RegionInfo(currentCulture.Name);

        private static readonly string url = "https://buff.163.com/api/market/goods/sell_order?game=csgo&goods_id=";
        private static readonly string val = "https://cdn.jsdelivr.net/gh/fawazahmed0/currency-api@1/latest/currencies/cny/";

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            using (StreamReader reader = new StreamReader("Items.txt"))
            {
                while (!reader.EndOfStream)
                {
                    string item = reader.ReadLine();
                    string id = FindId(item, "IDs.txt");

                    if (id != null)
                    {
                        double itemPrice = GetPrice(id);
                        double converted = Math.Round(itemPrice * GetConversion(), 2);

                        Console.WriteLine($"{item}: {itemPrice}¥ ({converted + regionInfo.CurrencySymbol})");
                        Thread.Sleep(3000);
                    }
                    else
                    {
                        Console.WriteLine($"{item} not found.");
                    }
                }
            }

            Console.ReadKey();
        }

        private static string FindId(string itemName, string filePath)
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

        private static double GetPrice(string id)
        {
            using (HttpClient client = new HttpClient())
            {
                string response = client.GetStringAsync(url + id).Result;
                JObject json = JObject.Parse(response);
                JObject data = json["data"]["items"][0] as JObject;
                double price = (double)data["price"];

                return price;
            }
        }

        private static double GetConversion()
        {
            string currency = regionInfo.ISOCurrencySymbol.ToLower();

            using (HttpClient client = new HttpClient())
            {
                string response = client.GetStringAsync(val + currency + ".json").Result;
                JObject json = JObject.Parse(response);
                double rate = (double)json[currency];

                return rate;
            }
        }
    }
}
