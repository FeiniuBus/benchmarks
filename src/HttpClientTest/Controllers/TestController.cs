using System;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using FeiniuBus.MongoDB;
using HttpClientTestCore.DbContext;
using HttpClientTestCore.Models.Ef;
using HttpClientTestCore.Models.Mongo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace HttpClientTestCore.Controllers
{
    [Route("test")]
    public class TestController : Controller
    {
        private static readonly AmazonSQSClient AwsClient = new AmazonSQSClient("AKIAOQNUVJ3TBNLUFQ2Q",
            "sx7sutcsjzJtzjxxjdwEjcSwAzaAtnaJmit26UOy", RegionEndpoint.CNNorth1);

        private readonly EfDbContext _efContext;

        private readonly HttpClient _httpClient;
        private readonly IMongoDbContext _mongoContext;

        private readonly IDistributedCache _redisCache;

        private readonly string _redisKey = "4953e7c5-5add-4d61-bc94-351f0722af86";

        private readonly string _testUrl;
        private readonly string _testUrl1;

        public TestController(HttpClient httpClient, IConfigurationRoot configuration, IMongoDbContext mongoDbContext,
            EfDbContext efDbContext, IDistributedCache redisCache)
        {
            _httpClient = httpClient;
            _mongoContext = mongoDbContext;
            _efContext = efDbContext;
            _redisCache = redisCache;
            _testUrl = configuration["TestUrl"];
            _testUrl1 = configuration["TestUrl1"];
        }

        [HttpGet("sqs")]
        public async Task<IActionResult> Sqs()
        {
            var request =
                new SendMessageRequest("https://sqs.cn-north-1.amazonaws.com.cn/696761199786/FeiniuBus-Logstash",
                    "TestLog");
            var response = await AwsClient.SendMessageAsync(request).ConfigureAwait(false);
            return Ok(response.MessageId);
        }

        [HttpGet("whenAllThree")]
        public async Task<IActionResult> WhenAllThree()
        {
            var request = _httpClient.GetAsync(_testUrl);
            var request1 = _httpClient.GetAsync(_testUrl1);
            var request2 = _httpClient.GetAsync(_testUrl);
            await Task.WhenAll(request, request1, request2);

            var response1 = request.Result.Content.ReadAsStringAsync();
            var response2 = request1.Result.Content.ReadAsStringAsync();
            var response3 = request2.Result.Content.ReadAsStringAsync();
            await Task.WhenAll(response1, response2, response3);

            return Ok($"response1: {response1.Result}   response2: {response2.Result}   response3: {response3.Result}");
        }

        [HttpGet("three")]
        public async Task<IActionResult> Three()
        {
            var request = await _httpClient.GetAsync(_testUrl);
            var response1 = await request.Content.ReadAsStringAsync();

            request = await _httpClient.GetAsync(_testUrl1);
            var response2 = await request.Content.ReadAsStringAsync();

            request = await _httpClient.GetAsync(_testUrl);
            var response3 = await request.Content.ReadAsStringAsync();

            return Ok($"response1: {response1}   response2: {response2}   response3: {response3}");
        }

        [HttpGet("whenAllTwo")]
        public async Task<IActionResult> WhenAllTwo()
        {
            var request = _httpClient.GetAsync(_testUrl);
            var request1 = _httpClient.GetAsync(_testUrl1);
            await Task.WhenAll(request, request1);

            var response1 = request.Result.Content.ReadAsStringAsync();
            var response2 = request1.Result.Content.ReadAsStringAsync();
            await Task.WhenAll(response1, response2);

            return Ok($"response1: {response1.Result}   response2: {response2.Result} ");
        }

        [HttpGet("two")]
        public async Task<IActionResult> Two()
        {
            var request = await _httpClient.GetAsync(_testUrl);
            var response = await request.Content.ReadAsStringAsync();

            request = await _httpClient.GetAsync(_testUrl1);
            var response1 = await request.Content.ReadAsStringAsync();

            return Ok($"response1: {response}   response2: {response1}");
        }

        [HttpGet("one")]
        public async Task<IActionResult> Static()
        {
            var request = await _httpClient.GetAsync(_testUrl);
            var response = await request.Content.ReadAsStringAsync();
            return Ok(response);
        }

        [HttpGet("ef")]
        public async Task<IActionResult> Ef()
        {
            try
            {
                var cities = await _efContext.Set<EfOpenCity>().AsNoTracking().ToListAsync();
                return Ok(cities);
            }
            catch (Exception ex)
            {
                var message = $"Message:{ex.Message} \n $InnerMessage: {ex.InnerException.Message}";
                return BadRequest(message);
            }
        }

        [HttpGet("mongo")]
        public async Task<IActionResult> Mongo()
        {
            try
            {
                var collection = _mongoContext.GetCollection<OpenCity>();
                var cities = await collection.Find(it => true).ToListAsync();
                return Ok(cities);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("redis")]
        public async Task<IActionResult> Redis()
        {
            try
            {
                var res = await _redisCache.GetStringAsync(_redisKey);
                if (res == null)
                {
                    var collection = _mongoContext.GetCollection<OpenCity>();
                    var cities = await collection.Find(it => true).ToListAsync();
                    var value = JsonConvert.SerializeObject(cities);
                    await _redisCache.SetStringAsync(_redisKey, value, new DistributedCacheEntryOptions());

                    return Ok(cities);
                }

                return Ok(JsonConvert.DeserializeObject(res));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("hello")]
        public IActionResult Index()
        {
            return Ok(new[] {"X Art", "X Art", "X Art", "X Art", "X Art", "X Art", "X Art"});
        }
    }
}