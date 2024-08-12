using MongoDB.Bson;

namespace Test.API.DataAccess.Interfaces
{
    public interface IDataAccessLayer
    {
        void CreateNewProduct(long id, string title, string description, string price, string catergory, bool isActive,
            DateTime expiryDate, string voltage, string socket);
        List<BsonDocument> GetAllProducts();
        List<BsonDocument> GetProductById(long id);
        List<BsonDocument> GetProductByCategory(string category);
        List<BsonDocument> GetProductByPrice(string price);
        bool IsProductExists(long id);
        bool UpdateProduct(long id, string title, string description, string price, string category,
            bool isactive, DateTime expiryDate, string voltage, string socket);
        bool DeleteProduct(long id);
        
        /* Catalog */
        List<BsonDocument> GetAllCatalogs();
        List<BsonDocument> GetCatalogById(long id);
        List<BsonDocument> GetCatalogByProductId(long id);
        bool UpdateCatalog(long id, string title, string products);
        bool DeleteCatalog(long id);
        bool IsCatalogExists(long id);
        void CreateNewCatalog(long id, string title, string products);
    }
}
