using Catalogo.API.Models;
using Catalogo.API.Models.ProdutoDTO;
using Catalogo.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using System.Text;
using System.Text.Json;

namespace Catalogo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutoController : ControllerBase
    {
        private readonly IConfiguration config;
        private readonly string connectionString;
        private readonly IProdutosService _produtosService;

        public ProdutoController(IConfiguration config, IProdutosService produtosService)
        {
            this.config = config;
            this.connectionString = this.config.GetValue<string>("AzureServiceBus");
            this._produtosService = produtosService;
        }

        [HttpPost]
        public async Task<IActionResult> Add(Produto produto)
        {
            try
            {
                await ArmazenaProduto(produto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            await EnviaMsg(produto);   
            return Ok();
        }
        private async Task ArmazenaProduto(Produto produto)
        {
            await _produtosService.AddProduto(produto);
        }

        private async Task EnviaMsg(Produto produto)
        {
            string queueName = "product";
            var client = new QueueClient(connectionString, queueName, ReceiveMode.PeekLock);

            var produtoDTO = new ProdutoDTO
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Preco = produto.Preco
            };

            string messageBody = JsonSerializer.Serialize(produtoDTO);
            var message = new Message(Encoding.UTF8.GetBytes(messageBody));

            await client.SendAsync(message);
            await client.CloseAsync();
        }
    }
}
