using HeroExplorer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace HeroExplorer
{
    public class MarvelFacade
    {
        private const string PrivateKey = "211450af3fe536cc940dcf1cd07a7c0b2bba92c6";
        private const string PublicKey = "2b7c954a99975b015406229e8465fdde";
        private const string ImageNotAvailablePath = "http://i.annihil.us/u/prod/marvel/i/mg/b/40/image_not_available";
        private const int MaxCharacters = 1500;

        public static async Task PopulateMarvelCharactersAsync(ObservableCollection<Character> marvelCharacters)
        {
            try
            {
                var characterDataWrapper = await GetCharacterDataWrapperAsync();

                var characters = characterDataWrapper.data.results;

                foreach (var character in characters)
                {
                    // Filter characters that are missing thumbnail images

                    if (character.thumbnail != null
                        && character.thumbnail.path != ""
                        && character.thumbnail.path != ImageNotAvailablePath)
                    {

                        character.thumbnail.small = String.Format("{0}/standard_small.{1}",
                            character.thumbnail.path,
                            character.thumbnail.extension);

                        character.thumbnail.large = String.Format("{0}/portrait_xlarge.{1}",
                            character.thumbnail.path,
                            character.thumbnail.extension);

                        marvelCharacters.Add(character);
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        public static async Task PopulateMarvelComicsAsync(int characterId, ObservableCollection<ComicBook> marvelComics)
        {
            try
            {
                var comicDataWrapper = await GetComicDataWrapperAsync(characterId);

                var comics = comicDataWrapper.data.results;

                foreach (var comic in comics)
                {
                    // Filter characters that are missing thumbnail images

                    if (comic.thumbnail != null
                        && comic.thumbnail.path != ""
                        && comic.thumbnail.path != ImageNotAvailablePath)
                    {

                        comic.thumbnail.small = String.Format("{0}/portrait_medium.{1}",
                            comic.thumbnail.path,
                            comic.thumbnail.extension);

                        comic.thumbnail.large = String.Format("{0}/portrait_xlarge.{1}",
                            comic.thumbnail.path,
                            comic.thumbnail.extension);

                        marvelComics.Add(comic);
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        public static async Task FindMarvelCharactersAsync(ObservableCollection<Character> marvelCharacters, string searchedCharacter)
        {
            try
            {
                var characterDataWrapper = await GetSearchedCharacterDataWrapperAsync(searchedCharacter);

                var characters = characterDataWrapper.data.results;

                foreach (var character in characters)
                {
                    // Filter characters that are missing thumbnail images

                    if (character.thumbnail != null
                        && character.thumbnail.path != ""
                        && character.thumbnail.path != ImageNotAvailablePath)
                    {

                        character.thumbnail.small = String.Format("{0}/standard_small.{1}",
                            character.thumbnail.path,
                            character.thumbnail.extension);

                        character.thumbnail.large = String.Format("{0}/portrait_xlarge.{1}",
                            character.thumbnail.path,
                            character.thumbnail.extension);

                        marvelCharacters.Add(character);
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        public static async Task FindMarvelEventsAsync(ObservableCollection<ComicEvent> marvelEvents, int characterId)
        {
            try
            {
                var eventDataWrapper = await GetSearchedEventsDataWrapperAsync(characterId);

                var events = eventDataWrapper.data.results;

                foreach (var ComicEvent in events)
                {
                    // Filter characters that are missing thumbnail images

                    if (ComicEvent.thumbnail != null
                        && ComicEvent.thumbnail.path != ""
                        && ComicEvent.thumbnail.path != ImageNotAvailablePath)
                    {

                        ComicEvent.thumbnail.small = String.Format("{0}/standard_small.{1}",
                            ComicEvent.thumbnail.path,
                            ComicEvent.thumbnail.extension);

                        ComicEvent.thumbnail.large = String.Format("{0}/portrait_xlarge.{1}",
                            ComicEvent.thumbnail.path,
                            ComicEvent.thumbnail.extension);

                        marvelEvents.Add(ComicEvent);
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
        }
        
        /// FUNKCJE TWORZACE URL I DE/SERIALIZUJACE///////////////////////////////////////////////////////////////////////////////////////
        
        private static async Task<CharacterDataWrapper> GetCharacterDataWrapperAsync()
        {
            // Assemble the URL
            Random random = new Random();
            var offset = random.Next(MaxCharacters);

            string url = String.Format("http://gateway.marvel.com:80/v1/public/characters?limit=10&offset={0}",
                offset);

            var jsonMessage = await CallMarvelAsync(url);

            // Response -> string / json -> deserialize
            var serializer = new DataContractJsonSerializer(typeof(CharacterDataWrapper));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonMessage));

            var result = (CharacterDataWrapper)serializer.ReadObject(ms);
            return result;
        }

        private static async Task<CharacterDataWrapper> GetSearchedCharacterDataWrapperAsync(string searchedCharacter)
        {
        
            string url = String.Format("http://gateway.marvel.com:80/v1/public/characters?name={0}&limit=1",
                searchedCharacter);

            var jsonMessage = await CallMarvelAsync(url);

            // Response -> string / json -> deserialize
            var serializer = new DataContractJsonSerializer(typeof(CharacterDataWrapper));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonMessage));

            var result = (CharacterDataWrapper)serializer.ReadObject(ms);
            return result;
        }

        private static async Task<ComicDataWrapper> GetComicDataWrapperAsync(int characterId)
        {
            var url = String.Format("http://gateway.marvel.com:80/v1/public/comics?characters={0}&limit=10",
                characterId);

            var jsonMessage = await CallMarvelAsync(url);

            // Response -> string / json -> deserialize
            var serializer = new DataContractJsonSerializer(typeof(ComicDataWrapper));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonMessage));

            var result = (ComicDataWrapper)serializer.ReadObject(ms);
            return result;
        }

       
        private static async Task<EventDataWrapper> GetSearchedEventsDataWrapperAsync(int characterId)
        {
            var url = String.Format("http://gateway.marvel.com:80/v1/public/events?characters={0}&limit=10",
                characterId);

            var jsonMessage = await CallMarvelAsync(url);

            // Response -> string / json -> deserialize
            var serializer = new DataContractJsonSerializer(typeof(EventDataWrapper));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonMessage));

            var result = (EventDataWrapper)serializer.ReadObject(ms);
            return result;
        }

        /// DOKONCZENIE URL I ZAPYTANIE DO API//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        private async static Task<string> CallMarvelAsync(string url)
        {
            // Get the MD5 Hash
            var timeStamp = DateTime.Now.Ticks.ToString();
            var hash = CreateHash(timeStamp);

            string completeUrl = String.Format("{0}&apikey={1}&ts={2}&hash={3}", url, PublicKey, timeStamp, hash);

            // Call out to Marvel
            HttpClient http = new HttpClient();
            var response = await http.GetAsync(completeUrl);
            return await response.Content.ReadAsStringAsync();
        }


        /// TWORZENIE HASZU I SZYFROWANIE///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
        
        private static string CreateHash(string timeStamp)
        {

            var toBeHashed = timeStamp + PrivateKey + PublicKey;
            var hashedMessage = ComputeMD5(toBeHashed);
            return hashedMessage;
        }
        
        // From:
        // http://stackoverflow.com/questions/8299142/how-to-generate-md5-hash-code-for-my-winrt-app-using-c
        private static string ComputeMD5(string str)
        {
            var alg = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
            IBuffer buff = CryptographicBuffer.ConvertStringToBinary(str, BinaryStringEncoding.Utf8);
            var hashed = alg.HashData(buff);
            var res = CryptographicBuffer.EncodeToHexString(hashed);
            return res;
        }
    }
}
