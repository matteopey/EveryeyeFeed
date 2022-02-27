using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EveryeyeFeed.Library
{
    public class Parser
    {
        public async Task<IEnumerable<Article>> GetArticles(string urlTemplate, int page)
        {
            var articles = (await GetDocument(urlTemplate, page))
                .DocumentNode
                .SelectNodes("//article[@class='fvideogioco']")
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

                    var date = GetDate(article, category);

                    var link = article
                        .SelectSingleNode(".//div[@class='testi_notizia']/a")
                        .GetAttributeValue("href", string.Empty);

                    return new Article
                    {
                        Title = title,
                        Link = link,
                        Date = date,
                        Description = article.SelectSingleNode(".//div[@class='testi_notizia']/p").InnerText,
                    };
                });

            return articles;
        }

        private DateTime GetDate(HtmlNode article, string category)
        {
            var dateString = article
                .SelectSingleNode(".//div[@class='testi_notizia']/span/b")
                .InnerText
                .Trim()
                .Replace(category, string.Empty);

            return Helpers.GetEveryeyeDate(dateString);
        }

        private async Task<HtmlDocument> GetDocument(string urlTemplate, int page)
        {
            if (!urlTemplate.Contains("{0}"))
            {
                throw new ArgumentException("URL template must include {0}");
            }

            var url = string.Format(urlTemplate, page);

            var web = new HtmlWeb();
            return await web.LoadFromWebAsync(url);
        }
    }
}
