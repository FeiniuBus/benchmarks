using Microsoft.Extensions.Configuration;

namespace HttpClientTestCore.DbContext
{
    public class MongoDbContext : FeiniuBus.MongoDB.MongoDbContext
    {
        public MongoDbContext(IConfigurationRoot configuration)
            : base(configuration.GetConnectionString("MongoConnectionString"))
        {
        }
    }
}