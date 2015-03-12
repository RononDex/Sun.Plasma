using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;

namespace Sun.Plasma.ViewModel
{
    /// <summary>
    /// ViewModel for the news control
    /// </summary>
    public class ViewModelNews : ViewModelBase
    {
        public ViewModelNews()
        {
            
        }

        private IEnumerable<Core.Feeds.NewsFeedEntry> _newsFeeds;
        public IEnumerable<Core.Feeds.NewsFeedEntry> NewsFeeds 
        { 
            get
            {
                return this._newsFeeds;
            }
            set
            {
                this._newsFeeds = value;
                OnPropertyChanged("NewsFeeds");
            }
        }

        /// <summary>
        /// Loads the news feeds from the SUN forum
        /// </summary>
        public void LoadNewsFeeds()
        {
            var url = ConfigurationManager.AppSettings["NewsFeedUrl"];
            if (string.IsNullOrEmpty(url))
            {
                ViewModelTools.Logger.ErrorFormat("No Rss feed url defined in the application configuration file!");
                throw new ConfigurationErrorsException("No Rss feed url defined in the application configuraito file!");
            }

            var reader = new Core.Feeds.RssNewsFeedReader();
            this.NewsFeeds = reader.GetNewsFeeds(url).OrderByDescending(x => x.PublishDate).Take(10);
        }
    }
}
