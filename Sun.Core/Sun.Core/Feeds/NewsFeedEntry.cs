using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sun.Core.Feeds
{
    /// <summary>
    /// Represents a single news feed entry
    /// </summary>
    public class NewsFeedEntry
    {
        /// <summary>
        /// The ID of the feed entry
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// The title for the news feed
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The content of the news feed
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// A string that points to the source of the news feed
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// A list of all authors for this news item
        /// </summary>
        public List<string> Authors { get; set; }

        /// <summary>
        /// The date & time when the news entry was published
        /// </summary>
        public DateTime PublishDate { get; set; }
    }
}
