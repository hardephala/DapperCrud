using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace DapperCrud.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ProductController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetAllProducts()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<Product> products = await SelectAllProducts(connection);
            return Ok(products);
        }

        [HttpGet("{productId}")]
        public async Task<ActionResult<Product>> GetProduct(int productId)
        {
            var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var product = await connection.QueryFirstAsync<Product>("select * from products where Id = @Id",
                new { Id = productId});
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<List<Product>>> CreateProduct(Product product)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("INSERT INTO products (Id,Name,Description,Status) VALUES (@Id, @Name, @Description, @Status)",product);
            return Ok(await SelectAllProducts(connection));
        }

        [HttpPut]
        public async Task<ActionResult<List<Product>>> UpdateProduct(Product product)
        {
            var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("UPDATE products SET name = @Name, description = @Description, status = @Status WHERE id = @Id", product);
            return Ok(await SelectAllProducts(connection));
        }

        [HttpDelete("{productId}")]
        public async Task<ActionResult<List<Product>>> DeleteProduct(int productId)
        {
            var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("DELETE FROM products WHERE id = @Id", new {Id = productId});
            return Ok(await SelectAllProducts(connection));
        }

        private static async Task<IEnumerable<Product>> SelectAllProducts(SqlConnection connection)
        {
            return await connection.QueryAsync<Product>("select * from products");
        }
    }
}
