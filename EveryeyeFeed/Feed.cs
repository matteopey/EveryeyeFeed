using EveryeyeFeed.Library;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EveryeyeFeed
{
    public class Feed(ILogger<Feed> logger)
    {
        private readonly ILogger<Feed> _logger = logger;

        [Function(nameof(Feed))]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = null)] HttpRequestData req)
        {
            var sw = Stopwatch.StartNew();

            var articlesUrl = "https://www.everyeye.it/articoli/pc/?pagina={0}";
            var newsUrl = "https://www.everyeye.it/notizie/pc/?pagina={0}";

            var pages = GetPages(req);

            _logger.LogInformation("Getting data for num. {Pages} pages", pages);

            var parser = new Parser();
            int page = 1;
            var tasks = new List<Task<IEnumerable<Article>>>();
            while (page <= pages)
            {
                tasks.Add(parser.GetArticles(articlesUrl, page));
                tasks.Add(parser.GetArticles(newsUrl, page));

                page++;
            }

            var articles = (await Task.WhenAll(tasks))
                .Aggregate(Enumerable.Empty<Article>(), (a, b) => a.Concat(b))
                .Where(ShouldBeAdded)
                .OrderByDescending(x => x.Date);

            var ret = new RssBuilder(req.Url.ToString()).Generate(articles);

            _logger.LogInformation("Building the RSS took: {Time}ms", sw.ElapsedMilliseconds);
            sw.Stop();

            var response = req.CreateResponse();
            response.StatusCode = HttpStatusCode.OK;
            response.Headers.Add("Content-Type", "application/xml");
            await response.WriteStringAsync(ret, Encoding.UTF8);

            return response;
        }

        private static int GetPages(HttpRequestData req)
        {
            var pagesQueryExists = req.Query.AllKeys.Contains("pages");

            if (pagesQueryExists && int.TryParse(req.Query["pages"], out int pages))
            {
                return pages;
            }

            return 1;
        }

        private static bool ShouldBeAdded(Article article)
        {
            return !article.Title.Contains("OFFERTE");
        }
    }
}
