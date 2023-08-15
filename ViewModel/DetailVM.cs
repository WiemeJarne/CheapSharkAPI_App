using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Project.Model;
using Project.Repository;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;

namespace Project.ViewModel
{
    public class DetailVM : ObservableObject
    {
        private APIGameRepository ApiGameRepository { get; set; }

        private bool _useAPI;
        public bool UseAPI
        {
            get { return _useAPI; }
            set
            {
                _useAPI = value;
                Initialize();
            }
        }

        private Game _currentGame;
        public Game CurrentGame
        {
            get { return _currentGame; }
            set
            {
                _currentGame = value;
                OnPropertyChanged(nameof(CurrentGame));
            }
        }

        private string _userEmail = "(enter email here)";
        public string UserEmail
        {
            get { return _userEmail; }
            set
            {
                _userEmail = value;
                OnPropertyChanged(nameof(UserEmail));
            }
        }

        private string _priceToReach = "(enter price to reach here)";
        public string PriceToReach
        {
            get { return _priceToReach; }
            set
            {
                _priceToReach = value;
                OnPropertyChanged(nameof(PriceToReach));
            }
        }

        public List<Store> Stores { get; private set; }

        private Store _selectedStore;
        public Store SelectedStore
        {
            get { return _selectedStore; }
            set
            {
                _selectedStore = value;
                CalculateShowingDeals();
            }
        }

        private string _selectedComparisonOperator;
        public string SelectedComparisonOperator
        {
            get { return _selectedComparisonOperator; }
            set
            {
                _selectedComparisonOperator = value;
                CalculateShowingDeals();
            }
        }

        private string _selectedComparisonType;
        public string SelectedComparisonType
        {
            get { return _selectedComparisonType; }
            set
            {
                _selectedComparisonType = value;
                CalculateShowingDeals();
            }
        }

        private float _givenToCompareNumber;
        public float GivenToCompareNumber
        {
            get { return _givenToCompareNumber; }
            set
            {
                _givenToCompareNumber = value;
                CalculateShowingDeals();
            }
        }

        private List<Deal> _showingDeals;
        public List<Deal> ShowingDeals
        {
            get { return _showingDeals; }
            set
            {
                _showingDeals = value;
                OnPropertyChanged(nameof(ShowingDeals));
            }
        }

        public Deal SelectedDeal { get; set; }

        public RelayCommand BrowseToSelectedDealCommand { get; private set; }
        public RelayCommand SetPriceAlertCommand { get; private set; }
        public RelayCommand DeletePriceAlertCommand { get; private set; }

        public DetailVM()
        {
            BrowseToSelectedDealCommand = new RelayCommand(BrowseToSelectedDeal);
            SetPriceAlertCommand = new RelayCommand(SetPriceAlert);
            DeletePriceAlertCommand = new RelayCommand(DeletePriceAlert);
        }

        private void Initialize()
        {            
            if (UseAPI)
            {
                ApiGameRepository = new APIGameRepository();
                LoadStores();
            }
            else
            {
                Stores = LocalGameRepository.GetStores();
            }
        }

        private async void LoadStores()
        {
            Stores = await ApiGameRepository.GetStoresAsync();
        }

        //looks at all the deals of the current game and makes sure only the deals that comply to the filters are visible
        public void CalculateShowingDeals()
        {
            List<Deal> showingDeals = new List<Deal>();
            //determine the store index of the SelectedStoreName
            string selectedStoreId = "";

            foreach (var store in Stores)
            {
                if (store.Name.Equals(SelectedStore.Name))
                {
                    selectedStoreId = store.Id;
                    break;
                }
            }

            if (SelectedComparisonOperator == null) return;
            if (SelectedComparisonType == null) return;
            if (CurrentGame == null) return;

            foreach (var deal in CurrentGame.Deals)
            {

                if (!UseAPI && LocalGameRepository.CheckDeal(deal, selectedStoreId, SelectedComparisonOperator, SelectedComparisonType, GivenToCompareNumber))
                    showingDeals.Add(deal);
                else if (UseAPI && ApiGameRepository.CheckDeal(deal, selectedStoreId, SelectedComparisonOperator, SelectedComparisonType, GivenToCompareNumber))
                    showingDeals.Add(deal);
            }

            ShowingDeals = showingDeals;
        }

        //Opens the default browser and browses to the SelectedDeal this only works when the api is in use when it is not in use an error window will be shown
        public void BrowseToSelectedDeal()
        {
            if (SelectedDeal == null)
            {
                MessageBox.Show($"please select a deal", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string url = $"https://www.cheapshark.com/redirect?dealID={SelectedDeal.DealId}";

            Process.Start(url);
        }

        //this function only work when the api is in use
        public async void SetPriceAlert()
        {
            if (!UseAPI)
                MessageBox.Show("Failed set alert this feature is only available when using the api", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                await ApiGameRepository.SetPriceAlertAsync(UserEmail, CurrentGame.Id, PriceToReach);
            }
        }

        //this function only work when the api is in use
        public async void DeletePriceAlert()
        {
            if (!UseAPI)
                MessageBox.Show("Failed delete alert this feature is only available when using the api", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                await ApiGameRepository.DeletePriceAlertAsync(UserEmail, CurrentGame.Id, PriceToReach);
            }
        }
    }
}