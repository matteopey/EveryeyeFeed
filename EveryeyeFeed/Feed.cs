using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
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
            var startUrl = "https://www.everyeye.it/articoli/pc/?pagina={0}";
            var pages = GetPages(req);

            log.LogInformation("Getting data for num. {Pages} pages", pages);

            var parser = new Parser(startUrl);

            var articles = new List<Article>();
            int page = 1;
            while (page <= pages)
            {
                articles.AddRange(parser.GetArticles(page));

                page++;
            }

            var ret = new RssBuilder("TODO")
                .Generate(articles);

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
