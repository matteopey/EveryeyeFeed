using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace EveryeyeFeed.Library
{
    public class RssBuilder
    {
        private readonly string _selfRef;

        public RssBuilder(string selfRef)
        {
            _selfRef = selfRef;
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
            feedWriter.WriteElementString("link", _selfRef);
            feedWriter.WriteElementString("description", "Everyeye Feed");
            feedWriter.WriteElementString("lastBuildDate", Helpers.GetRssDate(DateTime.UtcNow));

            // ATOM element
            feedWriter.WriteStartElement("atom", "link", null);
            feedWriter.WriteAttributeString("href", _selfRef);
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
