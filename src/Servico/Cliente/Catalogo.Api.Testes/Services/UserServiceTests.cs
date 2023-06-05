using AutoMapper;
using Catalogo.API.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalogo.Api.Testes.Services
{
    public class UserServiceTests
    {
        private ProdutosService produtoService;

        public UserServiceTests()
        {
            produtoService = new ProdutosService(new Mock<IProdutosService>().Object, new Mock<IMapper>;
        }

    }
}
