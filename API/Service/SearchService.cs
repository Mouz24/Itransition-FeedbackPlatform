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
using Elasticsearch.Net;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;

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
                .Query(q => q
                    .MultiMatch(m => m
                        .Query(wordToSearch)
                        .Fields(f => f
                            .Field(fld => fld.Title)
                            .Field(fld => fld.Text)
                            .Field(fld => fld.Artwork.Name)
                            .Field(fld => fld.Tags.Select(tag => tag.Text))
                            .Field(fld => fld.Group.Name)
                            .Field(fld => fld.User.UserName)
                        )
                        .Type(TextQueryType.MostFields)
                        .Operator(Operator.Or)
                        .Fuzziness(Fuzziness.Auto)
                    )
                )
            );

            var matchingReviews = searchResponse.Documents.ToList();

            return matchingReviews;
        }
    }
}
