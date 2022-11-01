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
                var restRequest = new RestRequest(GetURI($"{PetEndpoint}/{data.id}"));
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
                id = 1998,
                category = new Category()
                {
                    id = 0115,
                    name = "Takeshi"
                },


                name = "Isobe",
                photoUrls = new string[]
                {
                    "facebook.com"
                },

                tags = new Tag[]
                {
                    new Tag()
                    {
                     id = 111,
                     name = "Tag of Pet"

                    }

                },
                status = "available"
            };



            var temp = GetURI(PetEndpoint);
            var postRestRequest = new RestRequest(GetURI(PetEndpoint)).AddJsonBody(newPet);
            var postRestResponse = await restClient.ExecutePostAsync(postRestRequest);

            //Verify POST request status code
            Assert.AreEqual(HttpStatusCode.OK, postRestResponse.StatusCode, "Status code is not equal to 200");
            #endregion

            #region GetUser
            var restRequest = new RestRequest(GetURI($"{PetEndpoint}/{newPet.id}"), Method.Get);
            var restResponse = await restClient.ExecuteAsync<Petmodel>(restRequest);
            #endregion

            #region Assertions
            Assert.AreEqual(HttpStatusCode.OK, restResponse.StatusCode, "Status code is not equal to 200");
            Assert.AreEqual(newPet.name, restResponse.Data.name, "Pet Name did not match.");
            Assert.AreEqual(newPet.category.id, restResponse.Data.category.id, "Pet Category ID did not match.");
            Assert.AreEqual(newPet.category.name, restResponse.Data.category.name, "Pet Category ID did not match.");
            Assert.AreEqual(newPet.photoUrls[0], restResponse.Data.photoUrls[0], "PhotoURLs did not match.");
            Assert.AreEqual(newPet.tags[0].id, restResponse.Data.tags[0].id, "Pet Tag ID did not match.");
            Assert.AreEqual(newPet.tags[0].name, restResponse.Data.tags[0].name, "Pet Tag ID did not match.");
            #endregion

            #region CleanUp
            cleanUpList.Add(newPet);
            #endregion

           
        }
    }
}       
