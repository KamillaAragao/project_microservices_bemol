using Catalogo.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using System.Text;
using System.Text.Json;

namespace Cliente.API.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class ProdutoController : ControllerBase
    {
        private readonly IConfiguration config;
        private readonly string connectionString;

        public ProdutoController(IConfiguration config)
        {
            this.config = config;
            connectionString = this.config.GetValue<string>("AzureServiceBus");
        }

        [HttpPost("queue")]
        public async Task<IActionResult> PostQueue(Produto produto)
        {
            await SendMessageQueue(produto);
            return Ok(produto);
        }


        private async Task SendMessageQueue(Produto produto)
        {
            {
                string queueName = "product";
                var client = new QueueClient(connectionString, queueName, ReceiveMode.PeekLock);
                string messageBody = JsonSerializer.Serialize(produto);
                var message = new Message(Encoding.UTF8.GetBytes(messageBody));

                await client.SendAsync(message);
                await client.CloseAsync();
            }
        }
    }
}

