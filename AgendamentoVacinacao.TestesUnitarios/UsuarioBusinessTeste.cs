using AgendamentoVacinacao.Business.Interface.IBusiness;
using AgendamentoVacinacao.Business.Business;
using AgendamentoVacinacao.Entities.Entities;
using AgendamentoVacinacao.Entities.Enum;
using AgendamentoVacinacao.Entities.Model;
using AgendamentoVacinacao.Repository.Interface.IRepositories;
using AgendamentoVacinacao.Repository.Repositories;
using AgendamentoVacinacao.Utilities.Exceptions;
using AgendamentoVacinacao.Utilities.Messages;
using NUnit.Framework;

namespace AgendamentoVacinacao.TestesUnitarios
{
    public class UsuarioBusinessTeste : TesteUnitarioBase
    {
        private IUsuarioBusiness _business;
        private IUsuarioRepository _repository;

        [SetUp]
        public void SetUp()
        {
            _repository = new UsuarioRepository(_contexto);

            RegistrarObjeto(typeof(IUsuarioRepository), _repository);

            Registrar<IUsuarioBusiness, UsuarioBusiness>();

            _business = ObterServico<IUsuarioBusiness>();
        }

        [TestCase(PerfilEnum.Admin, "email1")]
        [TestCase(PerfilEnum.Medico, "email2")]
        public void InserirUsuario_Sucesso(PerfilEnum perfil, string email)
        {
            var novoUsuario = new CadastroUsuarioModel
            {
                email = email,
                login = "login",
                nome = "nome",
                perfil = perfil,
                senha = "senha"
            };

            async Task action() => await _business.InserirUsuario(novoUsuario);

            Assert.DoesNotThrowAsync(action);
        }
    }
}
