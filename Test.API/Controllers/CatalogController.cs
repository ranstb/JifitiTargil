using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Text;
using Test.API.DataAccess.Interfaces;
using Test.API.DataAccess;

namespace Test.API.Controllers
{
    /// <summary>
    /// Catalog Controller
    /// </summary>
    public class CatalogController : Controller
    {
        private readonly ILogger<ProductController> logger;
        private readonly IDataAccessLayer dataAccessLayer;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="logger"></param>
        public CatalogController(ILogger<ProductController> logger)
        {
            this.logger = logger;
            dataAccessLayer = new DataAccessLayer();
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Create New Catalog
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title"></param>
        /// <param name="products"></param>
        /// <returns></returns>
        [HttpGet("CreateNewCatalog")]
        public IActionResult CreateNewCatalog(long id, string title, string products)
        {
            if (dataAccessLayer.IsCatalogExists(id))
            {
                string msg = $"Catalog with id: {id} already exists";
                logger.LogInformation(msg);

                return BadRequest(msg);
            }

            logger.LogInformation($"Creating new catalog id: {id}");

            string productsNoDuplication = EliminateDuplicateProduct(products);

            if (!string.IsNullOrEmpty(productsNoDuplication) && productsNoDuplication.Length > 0)
            {
                string productsExisting = GetExistingProducts(productsNoDuplication);
                dataAccessLayer.CreateNewCatalog(id, title, productsExisting);
            }
            else 
            {
                dataAccessLayer.CreateNewCatalog(id, title, productsNoDuplication);
            }

            return Ok();
        }

        /// <summary>
        /// Get All Catalogs
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllCalalogs")]
        public async Task<IActionResult> GetAllCalalogs()
        {
            logger.LogInformation($"Getting all catalogs");

            List<BsonDocument> result = dataAccessLayer.GetAllCatalogs();

            if (result.Count == 0)
            {
                return NotFound("No catalogs been found");
            }

            StringBuilder stringBuilder = new StringBuilder();

            foreach (BsonDocument item in result)
            {
                stringBuilder.Append(item.Names.ElementAt(1));
                stringBuilder.Append(" : ");
                stringBuilder.Append(item.Values.ElementAt(1));
                stringBuilder.AppendLine();

                stringBuilder.Append(" ");

                stringBuilder.Append(item.Names.ElementAt(2));
                stringBuilder.Append(" : ");
                stringBuilder.Append(item.Values.ElementAt(2));

                stringBuilder.AppendLine();
            }

            return Ok(stringBuilder.ToString());
        }

        /// <summary>
        /// Get Catalog By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetCatalogById")]
        public async Task<IActionResult> GetCatalogById(long id)
        {
            logger.LogInformation($"Getting catalog by id: {id}");

            List<BsonDocument> result = dataAccessLayer.GetCatalogById(id);

            if (result.Count == 0)
            {
                return NotFound("No catalog been found");
            }

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(result[0].Names.ElementAt(1));
            stringBuilder.Append(" : ");
            stringBuilder.Append(result[0].Values.ElementAt(1));

            stringBuilder.Append(" ");

            stringBuilder.Append(result[0].Names.ElementAt(2));
            stringBuilder.Append(" : ");
            stringBuilder.Append(result[0].Values.ElementAt(2));

            return Ok(stringBuilder.ToString());
        }

        /// <summary>
        /// Get Catalog By Product Id
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        [HttpGet("GetCatalogByProductId")]
        public async Task<IActionResult> GetCatalogByProductId(long ProductId)
        {
            logger.LogInformation($"Getting catalog by product id: {ProductId}");

            List<BsonDocument> result = dataAccessLayer.GetCatalogByProductId(ProductId);

            if (result.Count == 0)
            {
                return NotFound("No catalogs been found");
            }

            StringBuilder stringBuilder = new StringBuilder();

            foreach (BsonDocument item in result)
            {
                stringBuilder.Append(item.Names.ElementAt(1));
                stringBuilder.Append(" : ");
                stringBuilder.Append(item.Values.ElementAt(1));
                stringBuilder.AppendLine();

                stringBuilder.Append(" ");

                stringBuilder.Append(item.Names.ElementAt(2));
                stringBuilder.Append(" : ");
                stringBuilder.Append(item.Values.ElementAt(2));

                stringBuilder.AppendLine();
            }

            return Ok(stringBuilder.ToString());
        }

        /// <summary>
        /// Update Catalog
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title"></param>
        /// <param name="productIds"></param>
        /// <returns></returns>
        [HttpGet("UpdateCatalog")]
        public async Task<IActionResult> UpdateCatalog(long id, string title, string productIds)
        {
            logger.LogInformation($"Updating catalog with id: {id}");

            var productsWithNoDuplications = EliminateDuplicateProduct(productIds);

            bool result = dataAccessLayer.UpdateCatalog(id, title, productsWithNoDuplications);

            if (!result)
            {
                return NotFound("No catalog been found for update");
            }

            return Ok();
        }

        /// <summary>
        /// Delete Catalog
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("DeleteCatalog")]
        public async Task<IActionResult> DeleteCatalog(long id)
        {
            logger.LogInformation($"Deleting catalog with id: {id}");

            bool result = dataAccessLayer.DeleteCatalog(id);

            if (!result)
            {
                return NotFound("No catalog been found for deletion");
            }

            return Ok();
        }

        private string EliminateDuplicateProduct(string input)
        {
            if (input == null || string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            string[] result = input.Split(new char[] { ',' });

            string[] NoDuplicationArray = result.Distinct().ToArray();

            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < NoDuplicationArray.Length; i++)
            {
                stringBuilder.Append(NoDuplicationArray[i]);

                if (i < NoDuplicationArray.Length - 1)
                {
                    stringBuilder.Append(",");
                }
            }

            return stringBuilder.ToString();
        }

        private string GetExistingProducts(string input)
        {
            var ExistingProductList = dataAccessLayer.GetAllProducts();

            List<int> ExistingProductIdsList = new List<int>();

            foreach (var product in ExistingProductList)
            {
                ExistingProductIdsList.Add(product.ElementAt(1).Value.ToInt32());
            }

            if (ExistingProductIdsList != null && ExistingProductIdsList.Count > 0)
            {
                string[] result = input.Split(new char[] { ',' });

                StringBuilder stringBuilder = new StringBuilder();

                for (int i = 0; i < result.Length; i++)
                {
                    bool isProductExists = (ExistingProductIdsList.Contains(Int32.Parse(result[i])));

                    if (isProductExists)
                    {
                        if (i > 0)
                        {
                            stringBuilder.Append(",");
                        }
                        stringBuilder.Append(result[i]);
                    }
                }

                return stringBuilder.ToString();
            }

            return string.Empty;
        }
    }
}
