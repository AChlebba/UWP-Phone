using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeroExplorer.Models
{

    public class EventCharacter
    {
        public string resourceURI { get; set; }
        public string name { get; set; }
    }

    public class EventCharacters
    {
        public int available { get; set; }
        public string collectionURI { get; set; }
        public List<EventCharacter> items { get; set; }
        public int returned { get; set; }
    }

    public class Next
    {
        public string resourceURI { get; set; }
        public string name { get; set; }
    }

    public class Previous
    {
        public string resourceURI { get; set; }
        public string name { get; set; }
    }

    public class ComicEvent
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string resourceURI { get; set; }
        public List<Url> urls { get; set; }
        public string modified { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public Thumbnail thumbnail { get; set; }
        public Creators creators { get; set; }
        public EventCharacters characters { get; set; }
        public Stories stories { get; set; }
        public Comics comics { get; set; }
        public Series series { get; set; }
        public Next next { get; set; }
        public Previous previous { get; set; }
    }

    public class EventDataContainer
    {
        public int offset { get; set; }
        public int limit { get; set; }
        public int total { get; set; }
        public int count { get; set; }
        public List<ComicEvent> results { get; set; }
    }

    public class EventDataWrapper
    {
        public int code { get; set; }
        public string status { get; set; }
        public string copyright { get; set; }
        public string attributionText { get; set; }
        public string attributionHTML { get; set; }
        public string etag { get; set; }
        public EventDataContainer data { get; set; }
    }
}
