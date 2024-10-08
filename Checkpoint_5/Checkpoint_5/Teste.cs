using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Checkpoint_5.Controllers; 
using Checkpoint_5.Models;      
using MongoDB.Driver;

public class ProdutoTests
{
    [Fact]
    public async Task Get_ReturnsAllProducts()
    {
        // Arrange
        var mockCollection = new Mock<IMongoCollection<Produto>>();
        var mockCursor = new Mock<IAsyncCursor<Produto>>();

        // Simula o comportamento do MongoDB ao retornar produtos
        mockCursor.Setup(c => c.Current).Returns(new List<Produto> { new Produto { Id = "1", Nome = "Produto A", Preco = 19.99M } });
        mockCursor.SetupSequence(c => c.MoveNext(It.IsAny<CancellationToken>())).Returns(true).Returns(false);
        mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true).ReturnsAsync(false);

        mockCollection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<Produto>>(), null, default)).ReturnsAsync(mockCursor.Object);

        var controller = new ProdutosController(mockCollection.Object);

        // Act
        var result = await controller.Get();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var produtos = Assert.IsAssignableFrom<IEnumerable<Produto>>(okResult.Value);
        Assert.Single(produtos);  // Espera que haja um único produto na lista
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_ForInvalidId()
    {
        // Arrange
        var mockCollection = new Mock<IMongoCollection<Produto>>();
        var controller = new ProdutosController(mockCollection.Object);

        // Act
        var result = await controller.GetById("invalid_id");

        // Assert
        Assert.IsType<NotFoundResult>(result);  // Espera que o resultado seja NotFound
    }
}
