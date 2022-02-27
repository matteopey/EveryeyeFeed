using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EveryeyeFeed
{
    public class RssBuilder
    {
        private readonly string _link;

        public RssBuilder(string link)
        {
            _link = link;
        }

        public string Generate(List<Article> articles)
        {
            using var stream = new MemoryStream();

            var settings = new XmlWriterSettings { Encoding = Encoding.UTF8 };

            using var feedWriter = XmlWriter.Create(stream, settings);

            feedWriter.WriteStartDocument();
            feedWriter.WriteStartElement("rss");
            feedWriter.WriteAttributeString("version", "2.0");
            feedWriter.WriteAttributeString("xmlns", "atom", null, "http://www.w3.org/2005/Atom");

            feedWriter.WriteStartElement("channel");
            feedWriter.WriteElementString("title", "Everyeye Feed");
            feedWriter.WriteElementString("link", _link);
            feedWriter.WriteElementString("description", "Everyeye Feed");
            feedWriter.WriteElementString("lastBuildDate", Helpers.GetRssDate(DateTime.UtcNow));

            // ATOM element
            feedWriter.WriteStartElement("atom", "link", null);
            feedWriter.WriteAttributeString("href", _link);
            feedWriter.WriteAttributeString("rel", "self");
            feedWriter.WriteAttributeString("type", "application/rss+xml");
            feedWriter.WriteEndElement();
            // End ATOM

            foreach (var article in articles)
            {
                feedWriter.WriteStartElement("item");

                feedWriter.WriteStartElement("guid");
                feedWriter.WriteAttributeString("isPermaLink", "true");
                feedWriter.WriteString(article.Link);
                feedWriter.WriteEndElement();

                feedWriter.WriteStartElement("title");
                feedWriter.WriteCData(article.Title);
                feedWriter.WriteEndElement();

                feedWriter.WriteStartElement("description");
                feedWriter.WriteCData(article.Description);
                feedWriter.WriteEndElement();

                feedWriter.WriteElementString("link", article.Link);
                feedWriter.WriteElementString("pubDate", Helpers.GetRssDate(article.Date));

                feedWriter.WriteEndElement();
            }

            feedWriter.WriteEndElement();   //channel
            feedWriter.WriteEndElement();   //rss

            feedWriter.WriteEndDocument();
            feedWriter.Flush();
            feedWriter.Close();

            return Encoding.UTF8.GetString(stream.ToArray());
        }
    }
}
