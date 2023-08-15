using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Project.Model;
using Project.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Project.ViewModel
{
    public class OverviewVM : ObservableObject
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

        private List<Game> _games;
        public List<Game> Games
        {
            get { return _games; }
            set
            {
                _games = value;
                OnPropertyChanged(nameof(Games));
            }
        }

        private Game _selectedGame;
        public Game SelectedGame
        {
            get { return _selectedGame; }
            set { _selectedGame = value; }
        }

        private List<Store> _stores;
        public List<Store> Stores
        {
            get { return _stores; }
            private set
            {
                _stores = value;
                OnPropertyChanged(nameof(Stores));
            }
        }

        public Store _selectedStore;
        public Store SelectedStore
        {
            get { return _selectedStore; }
            set
            {
                _selectedStore = value;
                OnPropertyChanged(nameof(SelectedStore));
            }
        }

        public List<string> ComparisonOperators { get; private set; }
            = new List<string>()
            {
                ">",
                "<",
                "="
            };

        private string _selectedComparisonOperator;
        public string SelectedComparisonOperator
        {
            get { return _selectedComparisonOperator; }
            set
            {
                _selectedComparisonOperator = value;
                OnPropertyChanged(nameof(SelectedComparisonOperator));
            }
        }

        public List<string> ComparisonTypes { get; private set; }
            = new List<string>()
            {
                "USD",
                "%"
            };

        private string _selectedComparisionType;
        public string SelectedComparisonType
        {
            get { return _selectedComparisionType; }
            set
            {
                _selectedComparisionType = value;
                OnPropertyChanged(nameof(SelectedComparisonType));
            }
        }

        //this number is given by the user in a TextBox
        private float givenToCompareNumber;
        public float GivenToCompareNumber
        {
            get { return givenToCompareNumber; }
            set
            {
                givenToCompareNumber = value;
                OnPropertyChanged(nameof(GivenToCompareNumber));
            }
        }

        public RelayCommand LoadGamesCommand { get; private set; }

        public OverviewVM()
        {
            LoadGamesCommand = new RelayCommand(Load100Games);
        }

        private void Initialize()
        {
            if (UseAPI)
            {
                ApiGameRepository = new APIGameRepository();

                LoadGames(100);
                LoadStores();
            }
            else
            {
                Games = LocalGameRepository.GetGames();
                Stores = LocalGameRepository.GetStores();
                Stores.Add(new Store() { Name = "<all stores>", Id = "" });
                SelectedStore = Stores.Last();
                SelectedComparisonOperator = ComparisonOperators[0];
                SelectedComparisonType = ComparisonTypes[0];
                GivenToCompareNumber = 0.00f;
            }
        }

        private async void LoadGames(int minAmount)
        {
            Games = await ApiGameRepository.LoadGamesAsync(minAmount);
        }

        private async void Load100Games()
        {
            if(!UseAPI)
            {
                MessageBox.Show("Failed to load more games this feature is only available when using the api", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Games = await ApiGameRepository.LoadGamesAsync(100).ConfigureAwait(false);
            UpdateGames();
        }

        private async void LoadStores()
        {
            Stores = await ApiGameRepository.GetStoresAsync();
            Stores.Add(new Store() { Name = "<all stores>", Id = "" });
            SelectedStore = Stores.Last();
            SelectedComparisonOperator = ComparisonOperators[0];
            SelectedComparisonType = ComparisonTypes[0];
            GivenToCompareNumber = 0.00f;
        }

        //makes sure that only the games that comply to the filters are shown
        public async void UpdateGames()
        {
            if (UseAPI)
                Games = await ApiGameRepository.GetGamesAsync(SelectedStore.Name, SelectedComparisonOperator, SelectedComparisonType, GivenToCompareNumber);
            else
                Games = LocalGameRepository.GetGames(SelectedStore.Name, SelectedComparisonOperator, SelectedComparisonType, GivenToCompareNumber);
        }
    }
}
