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
                .Aggregate(Enumerable.Empty<Article>(), (a, b) => a.Concat(b))
                .OrderByDescending(x => x.Date);

            var ret = new RssBuilder(GetSelf(req)).Generate(articles);

            log.LogInformation("Building the RSS took: {Time}ms", sw.ElapsedMilliseconds);
            sw.Stop();

            req.HttpContext.Response.ContentType = "application/xml";
            await req.HttpContext.Response.WriteAsync(ret, Encoding.UTF8);

            return new EmptyResult();
        }

        private static int GetPages(HttpRequest req)
        {
            if (req.Query.ContainsKey("pages") && !string.IsNullOrEmpty(req.Query["pages"]))
            {
                return int.Parse(req.Query["pages"]);
            }

            return 1;
        }

        private static string GetSelf(HttpRequest req)
        {
            var sb = new StringBuilder();

            if (req.IsHttps)
            {
                sb.Append("https://");
            }
            else
            {
                sb.Append("http://");
            }

            sb.Append(req.Host);
            sb.Append(req.Path);
            sb.Append(req.QueryString);

            return sb.ToString();
        }
    }
}
