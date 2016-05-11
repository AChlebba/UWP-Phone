using HeroExplorer.Models;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace HeroExplorer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public ObservableCollection<Character> MarvelCharacters { get; set; }
        public ObservableCollection<Character> MarvelCharacter { get; set; }
        public ObservableCollection<ComicBook> MarvelComics { get; set; }
        public ObservableCollection<ComicBook> MarvelComic { get; set; }
        public ObservableCollection<ComicEvent> MarvelEvents { get; set; }

        public MainPage()
        {
            this.InitializeComponent();

            MarvelCharacters = new ObservableCollection<Character>();
            MarvelCharacter = new ObservableCollection<Character>();
            MarvelComics = new ObservableCollection<ComicBook>();
            MarvelComic = new ObservableCollection<ComicBook>();
            MarvelEvents = new ObservableCollection<ComicEvent>();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MyProgressRing.IsActive = true;
            MyProgressRing.Visibility = Visibility.Visible;

            while (MarvelCharacters.Count < 10)
            {
               Task t =  MarvelFacade.PopulateMarvelCharactersAsync(MarvelCharacters);
               await t;
            }

            
            MyProgressRing.IsActive = false;
            MyProgressRing.Visibility = Visibility.Collapsed;
        }

        private async void MasterListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            MyProgressRing.IsActive = true;
            MyProgressRing.Visibility = Visibility.Visible;

            ComicDetailNameTextBlock.Text = "";
            ComicDetailDescriptionTextBlock.Text = "";
            ComicBuyTextBlock.Text = "";
            ComicDetailImage.Source = null;

            var selectedCharacter = (Character)e.ClickedItem;

            DetailNameTextBlock.Text = selectedCharacter.name;
            DetailDescriptionTextBlock.Text = selectedCharacter.description;

            var largeImage = new BitmapImage();
            Uri uri = new Uri(selectedCharacter.thumbnail.large, UriKind.Absolute);
            largeImage.UriSource = uri;
            DetailImage.Source = largeImage;

            MarvelComics.Clear();

            await MarvelFacade.PopulateMarvelComicsAsync(selectedCharacter.id, MarvelComics);

            MyProgressRing.IsActive = false;
            MyProgressRing.Visibility = Visibility.Collapsed;
        }

        private void ComicsGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var selectedComic = (ComicBook)e.ClickedItem;

            ComicDetailNameTextBlock.Text = "";
            ComicDetailDescriptionTextBlock.Text = "";
            ComicBuyTextBlock.Text = "";
            ComicDetailImage.Source = null;

            ComicDetailNameTextBlock.Text = selectedComic.title;

            if (selectedComic.description != null)
                ComicDetailDescriptionTextBlock.Text = selectedComic.description;

            try
            {
                ComicBuyTextBlock.Text = selectedComic.urls[1].url;
            }

            catch (Exception)
            {
                return;
            }

            var largeImage = new BitmapImage();
            Uri uri = new Uri(selectedComic.thumbnail.large, UriKind.Absolute);
            largeImage.UriSource = uri;
            ComicDetailImage.Source = largeImage;
        }

        private async void HyperlinkButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var selectedLink = ComicBuyTextBlock.Text;

            string uriToLaunch = @selectedLink;

            var uri = new Uri(uriToLaunch);

            var success = await Windows.System.Launcher.LaunchUriAsync(uri);
        }

        private async void buttonSearch_Click(object sender, RoutedEventArgs e)
        {
            
            MarvelCharacter.Clear();
            MarvelComic.Clear();
            MarvelEvents.Clear();
            loadComics.Visibility = Visibility.Collapsed;
            hideComics.Visibility = Visibility.Collapsed;
            loadEvents.Visibility = Visibility.Collapsed;
            hideEvents.Visibility = Visibility.Collapsed;
            SearchedComicDetailNameTextBlock.Text = "";
            SearchedComicDetailDescriptionTextBlock.Text = "";
            SearchedComicBuyTextBlock.Text = "";
            SearchedComicDetailImage.Source = null;

            var searchedCharacter = textBoxSearch.Text;

            Task t = MarvelFacade.FindMarvelCharactersAsync(MarvelCharacter, searchedCharacter);
            await t;

            try
            {
                if (MarvelCharacter[0].id > 100)
                {
                    loadComics.Visibility = Visibility.Visible;
                    loadEvents.Visibility = Visibility.Visible;
                }
            }
            catch (Exception)
            {
                return;
            }



        }

        private async void loadSearchedComics_Click(object sender, RoutedEventArgs e)
        {
            ProgressRing.IsActive = true;
            ProgressRing.Visibility = Visibility.Visible;

            var searchedCharacterId = MarvelCharacter[0].id;

            loadComics.Visibility = Visibility.Collapsed;
            hideComics.Visibility = Visibility.Visible;

            MarvelComic.Clear();

            await MarvelFacade.PopulateMarvelComicsAsync(searchedCharacterId, MarvelComic);

            ProgressRing.IsActive = false;
            ProgressRing.Visibility = Visibility.Collapsed;
        }

        

        private void SearchedComicsGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ProgressRing.IsActive = true;
            ProgressRing.Visibility = Visibility.Visible;

            var selectedComic = (ComicBook)e.ClickedItem;

            SearchedComicDetailNameTextBlock.Text = "";
            SearchedComicDetailDescriptionTextBlock.Text = "";
            SearchedComicBuyTextBlock.Text = "";
            SearchedComicDetailImage.Source = null;

            SearchedComicDetailNameTextBlock.Text = selectedComic.title;

            if (selectedComic.description != null)
                SearchedComicDetailDescriptionTextBlock.Text = selectedComic.description;

            try
            {
                SearchedComicBuyTextBlock.Text = selectedComic.urls[1].url;
            }

            catch (Exception)
            {
                return;
            }

            var largeImage = new BitmapImage();
            Uri uri = new Uri(selectedComic.thumbnail.large, UriKind.Absolute);
            largeImage.UriSource = uri;
            SearchedComicDetailImage.Source = largeImage;

            ProgressRing.IsActive = false;
            ProgressRing.Visibility = Visibility.Collapsed;
        }

        private async void loadEvents_Click(object sender, RoutedEventArgs e)
        {
            ProgressRing.IsActive = true;
            ProgressRing.Visibility = Visibility.Visible;

            var searchedCharacterId = MarvelCharacter[0].id;

            loadEvents.Visibility = Visibility.Collapsed;
            hideEvents.Visibility = Visibility.Visible;

            MarvelEvents.Clear();

            await MarvelFacade.FindMarvelEventsAsync(MarvelEvents, searchedCharacterId);
            var searchedCharacterId2 = MarvelCharacter[0].id;

            ProgressRing.IsActive = false;
            ProgressRing.Visibility = Visibility.Collapsed;
        }

        private void SearchedEventsGridView_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void hideEvents_Click(object sender, RoutedEventArgs e)
        {
            MarvelEvents.Clear();
            loadEvents.Visibility = Visibility.Visible;
            hideEvents.Visibility = Visibility.Collapsed;
        }

        private void hideSearchedComics_Click(object sender, RoutedEventArgs e)
        {
            MarvelComic.Clear();
            loadComics.Visibility = Visibility.Visible;
            hideComics.Visibility = Visibility.Collapsed;
            SearchedComicDetailNameTextBlock.Text = "";
            SearchedComicDetailDescriptionTextBlock.Text = "";
            SearchedComicBuyTextBlock.Text = "";
            SearchedComicDetailImage.Source = null;
        }

        private async void buttonRefresh_Click(object sender, RoutedEventArgs e)
        {
            MarvelCharacters.Clear();
            MarvelComics.Clear();
            DetailNameTextBlock.Text = "";
            DetailDescriptionTextBlock.Text = "";
            DetailImage.Source = null;
            ComicDetailNameTextBlock.Text = "";
            ComicDetailDescriptionTextBlock.Text = "";
            ComicBuyTextBlock.Text = "";
            ComicDetailImage.Source = null;

            MyProgressRing.IsActive = true;
            MyProgressRing.Visibility = Visibility.Visible;

            while (MarvelCharacters.Count < 10)
            {
                Task t = MarvelFacade.PopulateMarvelCharactersAsync(MarvelCharacters);
                await t;
            }


            MyProgressRing.IsActive = false;
            MyProgressRing.Visibility = Visibility.Collapsed;
        }
    }
    
}
