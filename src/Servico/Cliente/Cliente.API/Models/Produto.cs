using Newtonsoft.Json;

namespace Catalogo.API.Models
{
    public class Produto
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string? Nome { get; set; }
        [JsonProperty("preco")]
        public double Preco { get; set; }
    }
}
