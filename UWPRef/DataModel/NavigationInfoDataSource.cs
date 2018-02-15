using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;

namespace UWPRef.Data
{
    public class NavigationInfo
    {
        public NavigationInfo(string uniqueId, string title, string subtitle, 
            string imagePath, string description, string content)
        {
            this.UniqueId = uniqueId;
            this.Title = title;
            this.Subtitle = subtitle;
            this.ImagePath = imagePath;
            this.Description = description;
            this.Content = content;
            this.Docs = new ObservableCollection<DocLink>();
            this.RelatedItems = new ObservableCollection<string>();
        }
        public string UniqueId { get; private set; }
        public string Title { get; private set; }
        public string Subtitle { get; private set; }
        public string Description { get; private set; }
        public string ImagePath { get; private set; }
        public string Content { get; private set; }
        public ObservableCollection<DocLink> Docs { get; private set; }
        public ObservableCollection<string> RelatedItems { get; private set; }

    }

    public class DocLink
    {
        public DocLink(string title, string uri)
        {
            this.Title = title;
            this.Uri = uri;
        }
        public string Title { get; private set; }
        public string Uri { get; private set; }
    }

    public class NavigationInfoDataSource
    {
        private static readonly object _lock = new object();

        #region Singleton

        private static NavigationInfoDataSource _instance;

        public static NavigationInfoDataSource Instance
        {
            get
            {
                return _instance;
            }
        }

        static NavigationInfoDataSource()
        {
            _instance = new NavigationInfoDataSource();
        }

        private NavigationInfoDataSource() { }

        #endregion

        private IList<NavigationInfo> _items = new List<NavigationInfo>();
        public IList<NavigationInfo> Items
        {
            get { return this._items; }
        }

        public async Task<IEnumerable<NavigationInfo>> GetItemsAsync()
        {
            await _instance.GetNavigationInfoDataAsync();
            return _instance.Items;
        }

        private async Task GetNavigationInfoDataAsync()
        {
            lock (_lock)
            {
                if (this.Items.Count() != 0)
                {
                    return;
                }
            }

            // Path to json file
            Uri dataUri = new Uri("ms-appx:///DataModel/NavigationInfoItems.json");

            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
            string jsonText = await FileIO.ReadTextAsync(file);

            JsonObject jsonObject = JsonObject.Parse(jsonText);
            JsonArray jsonArray = jsonObject["Items"].GetArray();

            lock (_lock)
            {
                foreach (JsonValue jsonItem in jsonArray)
                {
                    JsonObject jsonObj = jsonItem.GetObject();
                    var item = new NavigationInfo(
                        jsonObj["UniqueId"].GetString(),
                        jsonObj["Title"].GetString(),
                        jsonObj["Subtitle"].GetString(),
                        jsonObj["ImagePath"].GetString(),
                        jsonObj["Description"].GetString(),
                        jsonObj["Content"].GetString());

                    if(!_items.Any(i => i.UniqueId == item.UniqueId))
                        _items.Add(item);

                    if (jsonObj.ContainsKey("Docs"))
                    {
                        foreach (JsonValue docValue in jsonObj["Docs"].GetArray())
                        {
                            JsonObject docObject = docValue.GetObject();
                            item.Docs.Add(new DocLink(docObject["Title"].GetString(), docObject["Uri"].GetString()));
                        }
                    }

                    if (jsonObj.ContainsKey("Related"))
                    {
                        foreach (JsonValue relatedValue in jsonObj["Related"].GetArray())
                        {
                            item.RelatedItems.Add(relatedValue.GetString());
                        }
                    }
                }
            }
        }
    }
}
