using Test.API.DataAccess.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Test.API.DataAccess
{
    public class DataAccessLayer : IDataAccessLayer
    {
        private IMongoClient dbClient;
        private IMongoDatabase database;
        private IMongoCollection<BsonDocument> productCollection;
        private IMongoCollection<BsonDocument> catalogCollection;

        public DataAccessLayer()
        {
            dbClient = new MongoClient(new MongoUrl("mongodb://localhost:27017"));
            database = dbClient.GetDatabase("Jifiti");
            productCollection = database.GetCollection<BsonDocument>("Products");
            catalogCollection = database.GetCollection<BsonDocument>("Catalogs");
        }

        public List<BsonDocument> GetAllProducts()
        {
            var documents = productCollection.Find(new BsonDocument()).ToList();

            return documents;
        }

        public void CreateNewProduct(long id, string title, string description, string price, string catergory, bool isActive,
            DateTime expiryDate, string voltage, string socket)
        {
            var document = new BsonDocument
{
                { "id", id },
                { "title", title },
                { "description", description },
                { "price", price },
                { "category", catergory },
                { "isactive", isActive },
                { "ExpiryDate", expiryDate },
                { "voltage", voltage },
                { "socket", socket } 
};

            productCollection.InsertOne(document);
        }

        public List<BsonDocument> GetProductById(long id)
        {
            var document = new BsonDocument
{
                { "id", id }
};
            var documents = productCollection.Find(document).ToList();

            return documents;
        }

        public List<BsonDocument> GetProductByCategory(string category)
        {
            var document = new BsonDocument
{
                { "category", category }
};
            var documents = productCollection.Find(document).ToList();

            return documents;
        }

        public List<BsonDocument> GetProductByPrice(string price)
        {
            var filter = Builders<BsonDocument>.Filter.AnyLte("price", price);

            var documents = productCollection.Find(filter).ToList();

            return documents;
        }

        public bool IsProductExists(long id)
        {
            var document = new BsonDocument
{
                { "id", id }
};
            var documents = productCollection.Find(document).ToList();

            if (documents == null || documents.Count == 0)
            {
                return false;
            }

            return true;
        }

        public bool UpdateProduct(long id, string title, string description, string price, string category,
            bool isactive, DateTime expiryDate, string voltage, string socket)
        {
            if (IsProductExists(id))
            {
                var filter = Builders<BsonDocument>.Filter.Eq("id", id);
                var update = Builders<BsonDocument>.Update
                    .Set("title", title)
                    .Set("description", description)
                    .Set("price", price)
                    .Set("category", category)
                    .Set("isactive", isactive)
                    .Set("ExpiryDate", expiryDate)
                    .Set("voltage", voltage)
                    .Set("socket", socket);

                productCollection.UpdateOne(filter, update);

                return true;
            }

            return false;
        }

        public bool DeleteProduct(long id)
        {
            if (IsProductExists(id))
            {
                var filter = Builders<BsonDocument>.Filter.Eq("id", id);
                productCollection.DeleteOne(filter);

                return true;
            }

            return false;
        }

        public List<BsonDocument> GetAllCatalogs()
        {
            var documents = catalogCollection.Find(new BsonDocument()).ToList();

            return documents;
        }

        public List<BsonDocument> GetCatalogById(long id)
        {
            var document = new BsonDocument
{
                { "id", id }
};
            var documents = catalogCollection.Find(document).ToList();

            return documents;
        }

        private bool IsValueExistInString(string input, string val)
        {
            string [] result = input.Split(new char[]{ ',' });

            foreach (string s in result)
            {
                if (string.Compare(s, val, true) == 0)
                { 
                    return true;
                }
            }

            return false;
        }

        public List<BsonDocument> GetCatalogByProductId(long id)
        {
            List<BsonDocument> catalogList = GetAllCatalogs();
            List<BsonDocument> catalogWwithProductList = new List<BsonDocument>();

            string strVal = id.ToString();

            foreach (BsonDocument item in catalogList)
            {
                BsonValue val = item.Values.ElementAt(3);

                string? valString = val.ToString();

                bool isProductIdInCatalog = IsValueExistInString(valString, strVal);

                if (isProductIdInCatalog)
                {
                    catalogWwithProductList.Add(item);
                }
            }

            return catalogWwithProductList;
        }

        public bool IsCatalogExists(long id)
        {
            var document = new BsonDocument
            {
                { "id", id }
            };
            var documents = catalogCollection.Find(document).ToList();

            if (documents == null || documents.Count == 0)
            {
                return false;
            }

            return true;
        }

        public bool UpdateCatalog(long id, string title, string products)
        {
            if (IsCatalogExists(id))
            {

                var filter = Builders<BsonDocument>.Filter.Eq("id", id);
                var update = Builders<BsonDocument>.Update.Set("title", title).Set("productsIds", products);
                catalogCollection.UpdateOne(filter, update);

                return true;
            }

            return false;
        }

        public bool DeleteCatalog(long id)
        {
            if (IsCatalogExists(id))
            {
                var filter = Builders<BsonDocument>.Filter.Eq("id", id);
                catalogCollection.DeleteOne(filter);

                return true;
            }

            return false;
        }

        public void CreateNewCatalog(long id, string title, string products)
        {
            var document = new BsonDocument
            {
                { "id", id },
                { "title", title },
                { "products", products }
            };

            catalogCollection.InsertOne(document);
        }
    }
}