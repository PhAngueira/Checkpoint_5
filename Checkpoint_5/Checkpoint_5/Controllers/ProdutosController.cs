using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using CP_5.Models;

namespace MinhaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutosController : ControllerBase
    {
        private readonly IMongoCollection<Produto> _produtos;

        public ProdutosController(IMongoCollection<Produto> produtos)
        {
            _produtos = produtos;
        }

        // Endpoint para listar todos os produtos
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var produtos = await _produtos.Find(_ => true).ToListAsync();
            return Ok(produtos);
        }

        // Endpoint para obter um produto pelo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var produto = await _produtos.Find(p => p.Id == id).FirstOrDefaultAsync();
            if (produto == null)
            {
                return NotFound();
            }
            return Ok(produto);
        }

        // Endpoint para criar um novo produto
        [HttpPost]
        public async Task<IActionResult> Create(Produto novoProduto)
        {
            await _produtos.InsertOneAsync(novoProduto);
            return CreatedAtAction(nameof(GetById), new { id = novoProduto.Id }, novoProduto);
        }

        // Endpoint para atualizar um produto
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, Produto produtoAtualizado)
        {
            var result = await _produtos.ReplaceOneAsync(p => p.Id == id, produtoAtualizado);
            if (result.IsAcknowledged && result.MatchedCount > 0)
            {
                return NoContent();
            }
            return NotFound();
        }

        // Endpoint para deletar um produto
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _produtos.DeleteOneAsync(p => p.Id == id);
            if (result.DeletedCount > 0)
            {
                return NoContent();
            }
            return NotFound();
        }
    }
}
