using Project.Model;
using Project.Repository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Project.View.Converters
{
    public class StoreIdToBannerUrlConverter : IValueConverter
    {
        public static bool UseAPI { get; set; }

        private APIGameRepository ApiGameRepository { get; set; }
        private List<Store> Stores { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (ApiGameRepository == null)
                ApiGameRepository = new APIGameRepository();

            string id = value.ToString();

            GetStores();

            foreach (var store in Stores)
            {
                if (store.Id == id)
                    return store.BannerUrl;
            }

            return "invalid storeId";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private async void GetStores()
        {
            if (Stores != null)
                return;

            if (UseAPI)
                Stores = await ApiGameRepository.GetStoresAsync();
            else
                Stores = LocalGameRepository.GetStores();
        }
    }
}
