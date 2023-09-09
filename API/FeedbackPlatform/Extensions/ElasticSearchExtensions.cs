using Entities.DTOs;
using Entities.Models;
using Nest;

namespace FeedbackPlatform.Extensions
{
    public static class ElasticSearchExtensions
    {
        public static void AddElasticSearch(this IServiceCollection services, IConfiguration configuration)
        {
            var url = configuration["ElasticSearch:Uri"];
            var defaultIndex = configuration["ElasticSearch:defaultIndex"];
            var username = configuration["ElasticSearch:username"];
            var password = configuration["ElasticSearch:password"];

            var settings = new ConnectionSettings(new Uri(url))
                .PrettyJson()
                .DefaultIndex(defaultIndex)
                .BasicAuthentication(username, password);

            var client = new ElasticClient(settings);

            services.AddSingleton<IElasticClient>(client);

            CreateIndex(client, defaultIndex);
        }

        private static void CreateIndex(IElasticClient client, string indexName)
        {
            client.Indices.Create(indexName, i => i.Map<ReviewDTO>(x => x.AutoMap()));
        }
    }
}
