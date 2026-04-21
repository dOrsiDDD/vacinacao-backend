using AgendamentoVacinacao.Business.Business;
using AgendamentoVacinacao.Business.Interface.IBusiness;
using AgendamentoVacinacao.Entities.Entities;
using AgendamentoVacinacao.Entities.Model;
using AgendamentoVacinacao.Utilities.Exceptions; 
using AgendamentoVacinacao.Repository.Interface.IRepositories;
using AgendamentoVacinacao.Repository.Repositories;
using AgendamentoVacinacao.Utilities.Messages;
using NUnit.Framework;




namespace AgendamentoVacinacao.TestesUnitarios.Business
{
    public class PacienteBusinessTeste : TesteUnitarioBase
    {
        private IPacienteBusiness _negocio;
        private IPacienteRepository _repositorio;

        [SetUp]
        public void SetUp()
        {
            _repositorio = new PacienteRepository(_contexto);

            RegistrarObjeto(typeof(IPacienteRepository), _repositorio);
            RegistrarObjeto(typeof(IPacienteBusiness), new PacienteBusiness(_repositorio));

            _negocio = ObterServico<IPacienteBusiness>();
        }

        // --- TESTES DE CREATE (INSERIR) ---

        [Test]
        public void InserirPaciente_Sucesso()
        {
            // Arrange
            var novoPaciente = new CadastroPacienteModel
            {
                nome = "João da Silva",
                dataNascimento = new DateOnly(1990, 1, 1)
            };

            // Act
            async Task action() => await _negocio.Inserir(novoPaciente);

            // Assert
            Assert.DoesNotThrowAsync(action);
        }


        // --- TESTES DE READ (OBTER E LISTAR) ---

        [Test]
        public async Task ListarPacientes_Sucesso()
        {
            // Arrange
            _contexto.Add(new Paciente { nome = "Paciente 1", dataNascimento = new DateOnly(2000, 1, 1), dataCriacao = DateTime.Now });
            _contexto.Add(new Paciente { nome = "Paciente 2", dataNascimento = new DateOnly(2001, 1, 1), dataCriacao = DateTime.Now });
            await _contexto.SaveChangesAsync();

            // Act
            async Task action() => await _negocio.ListarPacientes();

            // Assert
            Assert.DoesNotThrowAsync(action);
        }

        [Test]
        public async Task ObterPacientePorId_Sucesso()
        {
            // Arrange
            var paciente = new Paciente { nome = "Carlos", dataNascimento = new DateOnly(1980, 10, 10), dataCriacao = DateTime.Now };
            _contexto.Add(paciente);
            await _contexto.SaveChangesAsync();

            // Act
            async Task action() => await _negocio.ObterPacientePorId(paciente.Id);

            // Assert
            Assert.DoesNotThrowAsync(action);
        }

        [TestCase(0)]
        [TestCase(999)]
        public void ObterPacientePorId_Inexistente_LancaExcecao(int idInvalido)
        {
            // Arrange

            // Act
            async Task action() => await _negocio.ObterPacientePorId(idInvalido);

            // Assert
            var exception = Assert.ThrowsAsync<BusinessException>(action);
            Assert.IsTrue(exception.Message == string.Format(BusinessMessages.IdInvalido, idInvalido));
        }

        [Test]
        public async Task ObterPacientePorNome_Sucesso()
        {
            // Arrange
            var nomeBusca = "Ana";
            var paciente = new Paciente { nome = nomeBusca, dataNascimento = new DateOnly(1992, 12, 12), dataCriacao = DateTime.Now };
            _contexto.Add(paciente);
            await _contexto.SaveChangesAsync();

            // Act
            var resultado = await _negocio.ObterPacientesPorNome(nomeBusca);

            // Assert
            Assert.IsNotNull(resultado);
            Assert.That(resultado[0].nome, Is.EqualTo(nomeBusca).IgnoreCase);
        }

        [TestCase("")]
        [TestCase("Paciente Fantasma")]
        public void ObterPacientePorNome_Inexistente_LancaExcecao(string nomeInvalido)
        {
            // Arrange
            _contexto.Add(new Paciente { nome = "Paciente Real", dataNascimento = new DateOnly(1990, 1, 1), dataCriacao = DateTime.Now });
            _contexto.SaveChanges();

            // Act
            async Task action() => await _negocio.ObterPacientesPorNome(nomeInvalido);

            // Assert
            var exception = Assert.ThrowsAsync<BusinessException>(action);
            Assert.IsTrue(exception.Message == string.Format(BusinessMessages.NomeInvalido, nomeInvalido));
        }

        [Test]
        public async Task ObterAgendamentosPorPaciente_Sucesso()
        {
            // Arrange
            var paciente = new Paciente
            {
                nome = "Roberto",
                dataNascimento = new DateOnly(1975, 8, 8),
                dataCriacao = DateTime.Now
            };

            paciente.agendamentos.Add(new Agendamento
            {
                dataAgendamento = new DateOnly(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.AddDays(1).Day),
                horaAgendamento = new TimeOnly(14, 0, 0),
                status = "Pendente",
                dataCriacao = DateTime.Now
            });

            _contexto.Add(paciente);
            await _contexto.SaveChangesAsync();

            // Act
            async Task action() => await _negocio.ObterAgendamentosPorPaciente(paciente.Id);

            // Assert
            Assert.DoesNotThrowAsync(action);
        }


        [TestCase(999)]
        [TestCase(0)]
        public void ObterAgendamentosPorPaciente_Inexistente_LancaExcecao(int idInvalido)
        {
            // Arrange
            var paciente = new Paciente
            {
                nome = "Paciente",
                dataNascimento = new DateOnly(1995, 5, 5),
                dataCriacao = DateTime.Now
            };
            _contexto.Add(paciente);
            _contexto.SaveChanges();

            // Act
            async Task action() => await _negocio.ObterAgendamentosPorPaciente(idInvalido);

            // Assert
            var exception = Assert.ThrowsAsync<BusinessException>(action);
            Assert.IsTrue(exception.Message == string.Format(BusinessMessages.IdInvalido, idInvalido));
        }

        // --- TESTES DE UPDATE (ATUALIZAR) ---

        [Test]
        public async Task AtualizarNome_Sucesso()
        {
            // Arrange
            var nomeAntigo = "Fernanda";
            var nomeNovo = "Fernanda Silva";
            var paciente = new Paciente { nome = nomeAntigo, dataNascimento = new DateOnly(1988, 4, 4), dataCriacao = DateTime.Now };

            _contexto.Add(paciente);
            await _contexto.SaveChangesAsync();

            // Act
            async Task action() => await _negocio.AtualizarNome(paciente.Id, nomeNovo);

            // Assert
            Assert.DoesNotThrowAsync(action);
        }

        [TestCase(0)]
        [TestCase(999)]
        //[TestCase(null)]
        public void AtualizarNome_Inexistente_LancaExcecao(int pacienteId)
        {
            // Arrange
            var nomeNovo = "Novo Nome";

            // Act
            async Task action() => await _negocio.AtualizarNome(pacienteId, nomeNovo);

            // Assert
            var exception = Assert.ThrowsAsync<BusinessException>(action);
            Assert.IsTrue(exception.Message == string.Format(BusinessMessages.IdInvalido, pacienteId));
        }

        [Test]
        public async Task AtualizarDataNascimento_Sucesso()
        {
            // Arrange
            var nomePaciente = "Lucas";
            var dataAntiga = new DateOnly(1990, 1, 1);
            var dataNova = new DateOnly(1991, 2, 2);
            var paciente = new Paciente { nome = nomePaciente, dataNascimento = dataAntiga, dataCriacao = DateTime.Now };

            _contexto.Add(paciente);
            await _contexto.SaveChangesAsync();

            // Act
            async Task action() => await _negocio.AtualizarDataNascimento(paciente.Id, dataNova);

            // Assert
            Assert.DoesNotThrowAsync(action);
        }

        [TestCase(0)]
        [TestCase(999)]
        //[TestCase(null)]
        public void AtualizarDataNascimento_IdInvalido_LancaExcecao(int pacienteId)
        {
            // Arrange
            var dataNova = new DateOnly(2000, 1, 1);

            // Act
            async Task action() => await _negocio.AtualizarDataNascimento(pacienteId, dataNova);

            // Assert
            var exception = Assert.ThrowsAsync<BusinessException>(action);
            Assert.IsTrue(exception.Message == string.Format(BusinessMessages.IdInvalido, pacienteId));
        }

        [Test]
        public void AtualizarDataNascimento_DataFutura_LancaExcecao()
        {
            // Arrange
            var nomePaciente = "Mariana";
            var dataAntiga = new DateOnly(1990, 1, 1);
            var dataFutura = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
            var paciente = new Paciente { nome = nomePaciente, dataNascimento = dataAntiga, dataCriacao = DateTime.Now };
            _contexto.Add(paciente);
            _contexto.SaveChanges();
            // Act
            async Task action() => await _negocio.AtualizarDataNascimento(paciente.Id, dataFutura);
            // Assert
            var exception = Assert.ThrowsAsync<BusinessException>(action);
            Assert.IsTrue(exception.Message == BusinessMessages.DataNascimentoFuturo);
        }

        [Test]
        public void AtualizarDataNascimento_DataInvalida_LancaExcecao()
        {
            // Arrange
            var nomePaciente = "Mariana";
            var dataAntiga = new DateOnly(1990, 1, 1);
            var dataInvalida = new DateOnly(1899, 12, 31);
            var paciente = new Paciente { nome = nomePaciente, dataNascimento = dataAntiga, dataCriacao = DateTime.Now };
            _contexto.Add(paciente);
            _contexto.SaveChanges();

            // Act
            async Task action() => await _negocio.AtualizarDataNascimento(paciente.Id, dataInvalida);

            // Assert
            var exception = Assert.ThrowsAsync<BusinessException>(action);
            Assert.IsTrue(exception.Message == BusinessMessages.DataNascimentoInvalida);
        }

        // --- TESTES DE DELETE (DELETAR) ---

        [Test]
        public async Task Deletar_Sucesso()
        {
            // Arrange
            var paciente = new Paciente { nome = "Paciente Deletar", dataNascimento = new DateOnly(1980, 1, 1), dataCriacao = DateTime.Now };
            _contexto.Add(paciente);
            await _contexto.SaveChangesAsync();

            // Act
            async Task action() => await _negocio.Deletar(paciente.Id);

            // Assert
            Assert.DoesNotThrowAsync(action);
        }

        [TestCase(0)]
        [TestCase(999)]
        public void Deletar_Inexistente_LancaExcecao(int idInvalido)
        {
            // Arrange
            var paciente = new Paciente { nome = "Paciente", dataNascimento = new DateOnly(1980, 1, 1), dataCriacao = DateTime.Now };
            _contexto.Add(paciente);
            _contexto.SaveChanges();

            // Act
            async Task action() => await _negocio.Deletar(idInvalido);

            // Assert
            var exception = Assert.ThrowsAsync<BusinessException>(action);
            Assert.IsTrue(exception.Message == string.Format(BusinessMessages.IdInvalido, idInvalido));
        }
    }
}
