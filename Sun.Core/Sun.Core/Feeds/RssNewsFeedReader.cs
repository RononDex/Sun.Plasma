using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;

namespace Sun.Core.Feeds
{
    /// <summary>
    /// This class can be used to read rss news feeds 
    /// </summary>
    public class RssNewsFeedReader : NewsFeedReader
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public RssNewsFeedReader()
        {

        }

        /// <summary>
        /// Get the news from the given RSS feed located at the given URL
        /// </summary>
        /// <param name="source">URL to RSS feed</param>
        /// <returns></returns>
        public override IEnumerable<NewsFeedEntry> GetNewsFeeds(string source)
        {
            try
            {
                var res = new List<NewsFeedEntry>();

                var reader = XmlReader.Create(source);
                var feed = SyndicationFeed.Load(reader);
                reader.Close();

                foreach (SyndicationItem newsEntry in feed.Items.OrderByDescending(news => news.PublishDate))
                {
                    res.Add(new NewsFeedEntry()
                        {
                            ID = newsEntry.Id,
                            Title = newsEntry.Title.Text,
                            Authors = newsEntry.Authors.Select(x => x.Email).ToList(),
                            Source = newsEntry.Links[0].Uri.ToString(),
                            PublishDate = newsEntry.PublishDate.LocalDateTime
                        });
                }

                return res;
            }
            catch (Exception ex)
            {
                CoreTools.Logger.ErrorFormat("Error loading RSS feed from {0}: {1}", source, ex.Message);
                throw ex;
            }
        }
    }
}
