using Microsoft.AspNetCore.Mvc;
using Test.API.DataAccess.Interfaces;
using Test.API.DataAccess;
using MongoDB.Bson;
using System.Text;

namespace Test.API.Controllers
{
    /// <summary>
    /// Product Controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> logger;
        private readonly IDataAccessLayer dataAccessLayer;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="logger"></param>
        public ProductController(ILogger<ProductController> logger)
        {
            this.logger = logger;
            dataAccessLayer = new DataAccessLayer();
        }

        /// <summary>
        /// Get Product By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetProductById")]
        public async Task<IActionResult> GetProductById(long id)
        {
            logger.LogInformation($"Getting product by id: {id}");

            List<BsonDocument> result = dataAccessLayer.GetProductById(id);

            if (result.Count == 0)
            {
                return NotFound("No product been found");
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
        /// Get Product By Category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpGet("GetProductByCategory")]
        public async Task<IActionResult> GetProductByCategory(string category)
        {
            logger.LogInformation($"Getting product by category: {category}");

            List<BsonDocument> result = dataAccessLayer.GetProductByCategory(category);

            if (result.Count == 0)
            {
                return NotFound("No products been found");
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
        /// Get All Products
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllProducts")]
        public IActionResult GetAllProducts()
        {
            logger.LogInformation($"Getting all products");

            List<BsonDocument> result = dataAccessLayer.GetAllProducts();

            return Ok(result.ToJson());
        }

        //Expiry Date Format: MM/DD/YYYY
        /// <summary>
        /// Create New Product
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="price"></param>
        /// <param name="catergory"></param>
        /// <param name="isActive"></param>
        /// <param name="ExpiryDate"></param>
        /// <param name="voltage"></param>
        /// <param name="socket"></param>
        /// <returns></returns>
        [HttpGet("CreateNewProduct")]
        public IActionResult CreateNewProduct(long id, string title, string description, string price, string catergory, bool isActive,
            DateTime ExpiryDate, string voltage, string socket)
        {
            int n;
            if (!int.TryParse(price, out n))
            {
                return BadRequest("price is invalid");
            }

            if (dataAccessLayer.IsProductExists(id))
            {
                string msg = $"Product with id: {id} already exists";
                logger.LogInformation(msg);

                return BadRequest(msg);
            }

            if (string.Compare(catergory, "Fresh", true) == 0)
            {
                if (!IsExpiryDateValid(ExpiryDate, 7))
                {
                    string msg = $"Product id {id} expiry date is not valid";
                    logger.LogInformation(msg);

                    return BadRequest(msg);
                }
            }
            else if (string.Compare(catergory, "Electric", true) == 0)
            {
                if (!IsVoltageMatchSocket(voltage, socket))
                {
                    string msg = $"Product id {id} voltage does not match socket";
                    logger.LogInformation(msg);

                    return BadRequest(msg);
                }
            }
            else
            {
                return BadRequest("Not valid category");
            }

            logger.LogInformation($"Creating new product id: {id}");

            dataAccessLayer.CreateNewProduct(id, title, description, price, catergory, isActive,
                ExpiryDate, voltage, socket);

            return Ok();
        }

        /// <summary>
        /// Get Product By Price
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        [HttpGet("GetProductByPrice")]
        public async Task<IActionResult> GetProductByPrice(string price)
        {
            logger.LogInformation($"Getting product by price: {price}");

            List<BsonDocument> result = dataAccessLayer.GetProductByPrice(price);

            if (result.Count == 0)
            {
                return NotFound("No products been found");
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
        /// Update Product
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="price"></param>
        /// <param name="category"></param>
        /// <param name="isactive"></param>
        /// <param name="expiryDate"></param>
        /// <param name="voltage"></param>
        /// <param name="socket"></param>
        /// <returns></returns>
        [HttpGet("UpdateProduct")]
        public async Task<IActionResult> UpdateProduct(long id,
            string title, string description, string price, string category, bool isactive, DateTime expiryDate,
            string voltage, string socket)
        {
            logger.LogInformation($"Updating product with id: {id}");

            int n;
            if (!int.TryParse(price, out n))
            {
                return BadRequest("price is invalid");
            }

            bool result = dataAccessLayer.UpdateProduct(id, title, description, price, category,
                isactive, expiryDate, voltage, socket);

            if (!result)
            {
                return NotFound("No product been found for update");
            }

            return Ok();
        }

        /// <summary>
        /// Delete Product
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("DeleteProduct")]
        public async Task<IActionResult> DeleteProduct(long id)
        {
            logger.LogInformation($"Deleting product with id: {id}");

            bool result = dataAccessLayer.DeleteProduct(id);

            if (!result)
            {
                return NotFound("No product been found for deletion");
            }

            return Ok();
        }

        private bool IsExpiryDateValid(DateTime inputDate, int days)
        {
            TimeSpan diffDate = inputDate - DateTime.Now;

            if (diffDate.Days > days)
            {
                return true;
            }

            return false;
        }

        private bool IsVoltageMatchSocket(string voltage, string socket)
        {
            if (string.Compare(voltage, "220", true) == 0)
            {
                if (string.Compare(socket, "UK", true) == 0 || string.Compare(socket, "EU", true) == 0)
                {
                    return true;
                }
            }
            else if (string.Compare(voltage, "110", true) == 0)
            {
                if (string.Compare(socket, "US", true) == 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
