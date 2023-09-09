using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;
using Service.IService;
using Microsoft.Extensions.DependencyInjection;
using Castle.Core.Configuration;
using Entities.DTOs;

namespace Service
{
    public class SearchService : ISearchService
    {
        private readonly IElasticClient _client;

        public SearchService(IElasticClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<ReviewDTO>> Search(string wordToSearch)
        {
            var searchResponse = await _client.SearchAsync<ReviewDTO>(s => s
                .Query(
                    q => q.QueryString(
                        d => d.Query(wordToSearch)
                        )
                ));

            var matchingReviews = searchResponse.Documents.ToList();

            return matchingReviews;
        }
    }
}
