using Catalogo.API.Data;
using Catalogo.API.Models;

namespace Catalogo.API.Services
{
    public class ProdutosService : IProdutosService
    {
        private readonly NoSQLDatabase<Produto> _noSQLDataBase;
        public string container = "Produto";

        public ProdutosService() 
        {
            _noSQLDataBase = new NoSQLDatabase<Produto>();
        }

        public async Task AddProduto(Produto produto)
        {
            await _noSQLDataBase.Add(container, produto, produto.Id.ToString());
        }
    }
}
