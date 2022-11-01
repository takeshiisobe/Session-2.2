using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

[assembly: Parallelize(Workers = 10, Scope = ExecutionScope.MethodLevel)]
namespace DemoLearning2Part2
{
    [TestClass]
    public class Session2
    {
        private static RestClient restClient;

        private static readonly string BaseURL = "https://petstore.swagger.io/v2/";

        private static readonly string PetEndpoint = "pet";

        private static string GetURL(string enpoint) => $"{BaseURL}{enpoint}";

        private static Uri GetURI(string endpoint) => new Uri(GetURL(endpoint));

        private readonly List<Petmodel> cleanUpList = new List<Petmodel>();

        [TestInitialize]
        public async Task TestInitialize()
        {
            restClient = new RestClient();
        }

        [TestCleanup]
        public async Task TestCleanup()
        {
            foreach (var data in cleanUpList)
            {
                var restRequest = new RestRequest(GetURI($"{PetEndpoint}/{data.Id}"));
                var restResponse = await restClient.DeleteAsync(restRequest);
            }
        }

        [TestMethod]
        public async Task Post()
        {
            #region CreateUser
            //Create User
            var newPet = new Petmodel()
            {
                Id = 1998,
                Category = new Category()
                {
                    Id = 0115,
                    Name = "Takeshi"
                },


                Name = "Isobe",
                PhotoUrls = new string[]
                {
                    "facebook.com"
                },

                Tags = new Category[] { new Category { Id = 9999, Name = "Takeshi" } },

                Status = "available"
            };



            var temp = GetURI(PetEndpoint);
            var postRestRequest = new RestRequest(GetURI(PetEndpoint)).AddJsonBody(newPet);
            var postRestResponse = await restClient.ExecutePostAsync(postRestRequest);

            //Verify POST request status code
            Assert.AreEqual(HttpStatusCode.OK, postRestResponse.StatusCode, "Status code is not equal to 200");
            #endregion

            #region GetUser
            var restRequest = new RestRequest(GetURI($"{PetEndpoint}/{newPet.Id}"), Method.Get);
            var restResponse = await restClient.ExecuteAsync<Petmodel>(restRequest);
            #endregion

            #region Assertions
            Assert.AreEqual(HttpStatusCode.OK, restResponse.StatusCode, "Status code is not equal to 200");
            Assert.AreEqual(newPet.Name, restResponse.Data.Name, "Pet Name did not match.");
            Assert.AreEqual(newPet.Category.Id, restResponse.Data.Category.Id, "Pet Category ID did not match.");
            Assert.AreEqual(newPet.Category.Name, restResponse.Data.Category.Name, "Pet Category ID did not match.");
            Assert.AreEqual(newPet.PhotoUrls[0], restResponse.Data.PhotoUrls[0], "PhotoURLs did not match.");
            Assert.AreEqual(newPet.Tags[0].Id, restResponse.Data.Tags[0].Id, "Pet Tag ID did not match.");
            Assert.AreEqual(newPet.Tags[0].Name, restResponse.Data.Tags[0].Name, "Pet Tag ID did not match.");
            #endregion

            #region CleanUp
            cleanUpList.Add(newPet);
            #endregion

           
        }
    }
}       
