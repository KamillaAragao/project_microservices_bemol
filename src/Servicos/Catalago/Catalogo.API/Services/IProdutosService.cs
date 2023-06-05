using Catalogo.API.Models;

namespace Catalogo.API.Services
{
    public interface IProdutosService
    {
        Task AddProduto(Produto produto);
    }
}
