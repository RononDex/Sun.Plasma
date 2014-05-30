using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sun.Core.Feeds
{
    /// <summary>
    /// Base class for news feed readers
    /// </summary>
    public abstract class NewsFeedReader
    {
        public abstract IEnumerable<NewsFeedEntry> GetNewsFeeds(string source);
    }
}
