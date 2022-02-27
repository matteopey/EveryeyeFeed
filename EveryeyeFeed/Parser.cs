using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EveryeyeFeed
{
    public class Parser
    {
        private readonly string _urlTemplate;

        public Parser(string urlTemplate)
        {
            if (!urlTemplate.Contains("{0}"))
            {
                throw new ArgumentException("URL template must include {0}");
            }

            _urlTemplate = urlTemplate;
        }

        public List<Article> GetArticles(int page)
        {
            var url = string.Format(_urlTemplate, page);

            var web = new HtmlWeb();
            var document = web.Load(url);

            return document.DocumentNode.SelectNodes("//article[@class='fvideogioco']")
                .Select(article =>
                {
                    var category = article
                        .SelectSingleNode(".//div[@class='testi_notizia']/span")
                        .InnerText
                        .Split(' ')
                        .First()
                        .ToUpper();

                    var linkTitle = article
                        .SelectSingleNode(".//div[@class='testi_notizia']/a")
                        .GetAttributeValue("title", string.Empty);

                    var title = $"{category} | {linkTitle}";

                    var vote = article.SelectSingleNode(".//*[@class='ico-voto']");
                    if (vote != null)
                    {
                        title += $" | {vote.InnerText}";
                    }

                    var dateSplitted = article
                        .SelectSingleNode(".//div[@class='testi_notizia']/span/b")
                        .InnerText
                        .Trim()
                        .Replace(category, string.Empty)
                        .Split(' ');

                    var month = (int)Enum.Parse(typeof(Months), dateSplitted[1]);

                    var date = new DateTime(
                        int.Parse(dateSplitted[2]),
                        month,
                        int.Parse(dateSplitted[0]));

                    var link = article
                        .SelectSingleNode(".//div[@class='testi_notizia']/a")
                        .GetAttributeValue("href", string.Empty);

                    return new Article
                    {
                        Title = title,
                        Link = link,
                        Date = Helpers.GetRssDate(date),
                        Description = article.SelectSingleNode(".//div[@class='testi_notizia']/p").InnerText,
                    };
                })
                .ToList();
        }
    }
}
