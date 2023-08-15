using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Project.Model;
using Project.View;
using Project.View.Converters;
using System.Windows;
using System.Windows.Controls;

namespace Project.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        private bool UseAPI { get; set; } = true;

        //determines what the text is of a button
        public string CommandText
        {
            get
            {
                if (CurrentPage is OverviewPage)
                    return "show deals";

                return "back";
            }
        }

        public Page CurrentPage { get; set; }

        public OverviewPage OverviewPage { get; }
        = new OverviewPage();

        public DetailPage DetailPage { get; }
        = new DetailPage();

        //the visibility for the search button the button should only be visible when the OverViewPage is the CurrentPage
        public Visibility IsSearchButtonVisible { get; set; } = Visibility.Visible;

        public RelayCommand SwitchPageCommand { get; private set; }
        public RelayCommand SearchDealsCommand { get; private set; }

        public MainViewModel()
        {
            CurrentPage = OverviewPage;
            SwitchPageCommand = new RelayCommand(SwitchPage);
            SearchDealsCommand = new RelayCommand(SearchDeals);

            (OverviewPage.DataContext as OverviewVM).UseAPI = UseAPI;
            (DetailPage.DataContext as DetailVM).UseAPI = UseAPI;
            StoreIdToBannerUrlConverter.UseAPI = UseAPI;
        }

        public void SwitchPage()
        {
            if (CurrentPage is OverviewPage)
            {
                Game selectedGame = (OverviewPage.DataContext as OverviewVM).SelectedGame;
                if (selectedGame == null) return;

                Store selectedStore = (OverviewPage.DataContext as OverviewVM).SelectedStore;
                if (selectedStore == null) return;

                string selectedComparisonOperator = (OverviewPage.DataContext as OverviewVM).SelectedComparisonOperator;
                if (selectedComparisonOperator == null) return;

                string selectedComparisonType = (OverviewPage.DataContext as OverviewVM).SelectedComparisonType;
                if (selectedComparisonType == null) return;

                float givenToCompareNumber = (OverviewPage.DataContext as OverviewVM).GivenToCompareNumber;

                (DetailPage.DataContext as DetailVM).CurrentGame = selectedGame;
                (DetailPage.DataContext as DetailVM).SelectedStore = selectedStore;
                (DetailPage.DataContext as DetailVM).SelectedComparisonOperator = selectedComparisonOperator;
                (DetailPage.DataContext as DetailVM).SelectedComparisonType = selectedComparisonType;
                (DetailPage.DataContext as DetailVM).GivenToCompareNumber = givenToCompareNumber;

                CurrentPage = DetailPage;
                IsSearchButtonVisible = Visibility.Hidden;
            }
            else
            {
                CurrentPage = OverviewPage;
                IsSearchButtonVisible = Visibility.Visible;
            }

            OnPropertyChanged(nameof(CurrentPage));
            OnPropertyChanged(nameof(CommandText));
            OnPropertyChanged(nameof(IsSearchButtonVisible));
        }

        //this function is called when the search button is pressed
        public void SearchDeals()
        {
            if (CurrentPage is OverviewPage)
            {
                Store selectedStore = (OverviewPage.DataContext as OverviewVM).SelectedStore;
                if (selectedStore == null) return;

                string selectedComparisonOperator = (OverviewPage.DataContext as OverviewVM).SelectedComparisonOperator;
                if (selectedComparisonOperator == null) return;

                string selectedComparisonType = (OverviewPage.DataContext as OverviewVM).SelectedComparisonType;
                if (selectedComparisonType == null) return;

                float givenToCompareNumber = (OverviewPage.DataContext as OverviewVM).GivenToCompareNumber;

                (DetailPage.DataContext as DetailVM).SelectedStore = selectedStore;
                (DetailPage.DataContext as DetailVM).SelectedComparisonOperator = selectedComparisonOperator;
                (DetailPage.DataContext as DetailVM).SelectedComparisonType = selectedComparisonType;
                (DetailPage.DataContext as DetailVM).GivenToCompareNumber = givenToCompareNumber;

                (OverviewPage.DataContext as OverviewVM).UpdateGames();
            }
        }
    }
}