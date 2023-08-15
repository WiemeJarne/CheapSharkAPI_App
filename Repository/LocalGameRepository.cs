using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Project.Model;

namespace Project.Repository
{
    public class LocalGameRepository
    {
        private static List<Game> _games;//a list of all the games that have been loaded in

        public static List<Game> GetGames()
        {
            if (_games != null) return _games;

            _games = new List<Game>();

            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            var resourceName = "Project.Resources.Data.games.json";

            using(Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using(var reader = new StreamReader(stream))
                {
                    string json = reader.ReadToEnd();
                    
                    var obj = JsonConvert.DeserializeObject<Dictionary<string, JObject>>(json);

                    foreach(var item in obj)
                    {
                        Game game = new Game();
                        var info = item.Value.SelectToken("info");
                        game.Title = info.SelectToken("title").ToString();
                        game.ImageUrl = info.SelectToken("thumb").ToString();
                        game.Deals = item.Value.SelectToken("deals").ToObject<List<Deal>>();
                        _games.Add(game);
                    }
                }
            }

            return _games;
        }

        //returns a list of games that comply to the filters
        public static List<Game> GetGames(string storeName, string comparisonOperator, string comparisonType, float toCompareNumber)
        {
            bool shouldCheckStoreId = false;
            if (!storeName.Equals("<all stores>"))
                shouldCheckStoreId = true;

            List<Game> filteredGames = new List<Game>();

            string storeId = "";
            if (shouldCheckStoreId)
            {
                storeId = GetStoreId(storeName);
            }

            if (comparisonOperator == null)
                return GetGames();

            if (comparisonType == null)
                return GetGames();

            //loop over all the games
            foreach(Game game in _games)
            {
                //loop over all the deals from the game, when 1 deal complies to the filters the game is added to the filteredGames list
                foreach (Deal deal in game.Deals)
                {
                    if (CheckDeal(deal, storeId, comparisonOperator, comparisonType, toCompareNumber))
                    {
                        filteredGames.Add(game);
                        break;
                    }
                }
            }

            return filteredGames;
        }

        //checks the deal complies to the filters
        public static bool CheckDeal(Deal deal, string storeId, string comparisonOperator, string comparisonType, float toCompareNumber)
        {
            bool shouldCheckStoreId = true;
            if(storeId.Equals(""))
                shouldCheckStoreId = false;

            if (shouldCheckStoreId && deal.StoreId.Equals(storeId))
            {
                if (comparisonType.Equals("USD") && CheckDealSalePrice(deal.SalePrice, comparisonOperator, toCompareNumber))
                    return true;
                else if (comparisonType.Equals("%") && CheckDealSalePercentage(deal.SavingPercentage, comparisonOperator, toCompareNumber))
                    return true;
            }
            else if (!shouldCheckStoreId)
            {
                if (comparisonType.Equals("USD") && CheckDealSalePrice(deal.SalePrice, comparisonOperator, toCompareNumber))
                    return true;
                else if (comparisonType.Equals("%") && CheckDealSalePercentage(deal.SavingPercentage, comparisonOperator, toCompareNumber))
                    return true;
            }

            return false;
        }

        //checks if the price complies to the price filter
        private static bool CheckDealSalePrice(float price, string comparisonOperator, float toCompareNumber)
        {
            if (comparisonOperator.Equals("<") && price < toCompareNumber)
                return true;

            if (comparisonOperator.Equals(">") && price > toCompareNumber)
                return true;

            if (comparisonOperator.Equals("=") && price == toCompareNumber)
                return true;

            return false;
        }

        //checks if the percentage complies to the percentage filter
        private static bool CheckDealSalePercentage(float pertencate, string comparisonOperator, float toCompareNumber)
        {
            if (comparisonOperator.Equals("<") && pertencate < toCompareNumber)
                return true;

            if (comparisonOperator.Equals(">") && pertencate > toCompareNumber)
                return true;

            if (comparisonOperator.Equals("=") && pertencate == toCompareNumber)
                return true;

            return false;
        }

        private static List<Store> _stores; //a list of all the stores that have been loaded in

        public static List<Store> GetStores()
        {
            if (_stores != null) return _stores;

            _stores = new List<Store>();

            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            var resourceName = "Project.Resources.Data.stores.json";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (var reader = new StreamReader(stream))
                {
                    string json = reader.ReadToEnd();

                    var obj = JsonConvert.DeserializeObject<List<JObject>>(json);

                    foreach (var item in obj)
                    {
                        if (!item.SelectToken("isActive").ToObject<bool>())
                            continue;

                        Store store = new Store();
                        store.Id = item.SelectToken("storeID").ToString();
                        store.Name = item.SelectToken("storeName").ToString();
                        var images = item.SelectToken("images");
                        string imagesUrlStart = "https://www.cheapshark.com/";
                        store.BannerUrl = $"{imagesUrlStart}{images.SelectToken("banner")}";
                        store.IconUrl = $"{imagesUrlStart}{images.SelectToken("icon")}";
                        _stores.Add(store);
                    }
                }
            }

            return _stores;
        }

        private static string GetStoreId(string storeName)
        {
            GetStores();

            string storeId = "";
            foreach (var store in _stores)
            {
                if (store.Name.Equals(storeName))
                {
                    storeId = store.Id;
                    break;
                }
            }

            return storeId;
        }
    }
}