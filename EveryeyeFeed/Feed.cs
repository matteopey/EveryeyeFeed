using EveryeyeFeed.Library;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EveryeyeFeed
{
    public static class Feed
    {
        [FunctionName("feed")]
        public async static Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var sw = Stopwatch.StartNew();

            var articlesUrl = "https://www.everyeye.it/articoli/pc/?pagina={0}";
            var newsUrl = "https://www.everyeye.it/notizie/pc/?pagina={0}";

            var pages = GetPages(req);

            log.LogInformation("Getting data for num. {Pages} pages", pages);

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
                .Aggregate(Enumerable.Empty<Article>(), (a, b) => a.Concat(b));

            var ret = new RssBuilder("TODO")
                .Generate(articles.OrderByDescending(x => x.Date).ToList());

            log.LogInformation("Building the RSS took: {Time}ms", sw.ElapsedMilliseconds);

            req.HttpContext.Response.ContentType = "application/xml";
            await req.HttpContext.Response.WriteAsync(ret, Encoding.UTF8);

            return new EmptyResult();
        }

        private static int GetPages(HttpRequest req)
        {
            if (req.Query.ContainsKey("pages"))
            {
                return int.Parse(req.Query["pages"]);
            }

            return 1;
        }
    }
}
