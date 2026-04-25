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
                cpf = "41445406420",
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
            _contexto.Add(new Paciente { nome = "Paciente 1", cpf = "31081862807", dataNascimento = new DateOnly(2000, 1, 1), dataCriacao = DateTime.Now });
            _contexto.Add(new Paciente { nome = "Paciente 2", cpf = "04777445402", dataNascimento = new DateOnly(2001, 1, 1), dataCriacao = DateTime.Now });
            await _contexto.SaveChangesAsync();

            // Act
            async Task action() => await _negocio.ListarPacientes();

            // Assert
            Assert.DoesNotThrowAsync(action);
        }

        [Test]
        public async Task ObterPacientePorCPF_Sucesso()
        {
            // Arrange
            var paciente = new Paciente { nome = "Carlos", cpf = "26208784620", dataNascimento = new DateOnly(1980, 10, 10), dataCriacao = DateTime.Now };
            _contexto.Add(paciente);
            await _contexto.SaveChangesAsync();

            // Act
            async Task action() => await _negocio.ObterPacientePorCPF(paciente.cpf);

            // Assert
            Assert.DoesNotThrowAsync(action);
        }

        [TestCase("06631299748")]
        [TestCase("00000000000")]
        [TestCase("CPF Invalido")]
        [TestCase("81322222222")]
        public void ObterPacientePorCPF_Inexistente_LancaExcecao(string cpfInvalido)
        {
            // Arrange

            // Act
            async Task action() => await _negocio.ObterPacientePorCPF(cpfInvalido);

            // Assert
            var exception = Assert.ThrowsAsync<BusinessException>(action);
            Assert.IsTrue(exception.Message == string.Format(BusinessMessages.CPFInvalido, cpfInvalido));
        }

        [Test]
        public async Task ObterPacientePorNome_Sucesso()
        {
            // Arrange
            var nomeBusca = "Ana";
            var paciente = new Paciente { nome = nomeBusca, cpf = "13049932643", dataNascimento = new DateOnly(1992, 12, 12), dataCriacao = DateTime.Now };
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
            _contexto.Add(new Paciente { nome = "Paciente Real", cpf = "15607725812", dataNascimento = new DateOnly(1990, 1, 1), dataCriacao = DateTime.Now });
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
                cpf = "04364885665",
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
            async Task action() => await _negocio.ObterAgendamentosPorPaciente(paciente.cpf);

            // Assert
            Assert.DoesNotThrowAsync(action);
        }


        [TestCase("06631299748")]
        [TestCase("00000000000")]
        [TestCase("CPF Invalido")]
        [TestCase("81322222222")]
        public void ObterAgendamentosPorPaciente_Inexistente_LancaExcecao(string cpfInvalido)
        {
            // Arrange
            var paciente = new Paciente
            {
                nome = "Paciente",
                cpf = "140.581.438-19",
                dataNascimento = new DateOnly(1995, 5, 5),
                dataCriacao = DateTime.Now
            };
            _contexto.Add(paciente);
            _contexto.SaveChanges();

            // Act
            async Task action() => await _negocio.ObterAgendamentosPorPaciente(cpfInvalido);

            // Assert
            var exception = Assert.ThrowsAsync<BusinessException>(action);
            Assert.IsTrue(exception.Message == string.Format(BusinessMessages.CPFInvalido, cpfInvalido));
        }

        // --- TESTES DE UPDATE (ATUALIZAR) ---

        [Test]
        public async Task AtualizarNome_Sucesso()
        {
            // Arrange
            var nomeAntigo = "Fernanda";
            var nomeNovo = "Fernanda Silva";
            var cpf = "04173598610";
            var paciente = new Paciente { nome = nomeAntigo, cpf = cpf, dataNascimento = new DateOnly(1988, 4, 4), dataCriacao = DateTime.Now };

            _contexto.Add(paciente);
            await _contexto.SaveChangesAsync();

            // Act
            async Task action() => await _negocio.AtualizarNome(paciente.cpf, nomeNovo);

            // Assert
            Assert.DoesNotThrowAsync(action);
        }

        [TestCase("06631299748")]
        [TestCase("00000000000")]
        [TestCase("CPF Invalido")]
        [TestCase("81322222222")]
        public void AtualizarNome_Inexistente_LancaExcecao(string cpf)
        {
            // Arrange
            var nomeNovo = "Novo Nome";

            // Act
            async Task action() => await _negocio.AtualizarNome(cpf, nomeNovo);

            // Assert
            var exception = Assert.ThrowsAsync<BusinessException>(action);
            Assert.IsTrue(exception.Message == string.Format(BusinessMessages.CPFInvalido, cpf));
        }

        [Test]
        public async Task AtualizarDataNascimento_Sucesso()
        {
            // Arrange
            var nomePaciente = "Lucas";
            var cpf = "06752340692";
            var dataAntiga = new DateOnly(1990, 1, 1);
            var dataNova = new DateOnly(1991, 2, 2);
            var paciente = new Paciente { nome = nomePaciente, cpf = cpf, dataNascimento = dataAntiga, dataCriacao = DateTime.Now };

            _contexto.Add(paciente);
            await _contexto.SaveChangesAsync();

            // Act
            async Task action() => await _negocio.AtualizarDataNascimento(paciente.cpf, dataNova);

            // Assert
            Assert.DoesNotThrowAsync(action);
        }

        [TestCase("06631299748")]
        [TestCase("00000000000")]
        [TestCase("CPF Invalido")]
        [TestCase("81322222222")]
        public void AtualizarDataNascimento_cpfInvalido_LancaExcecao(string cpf)
        {
            // Arrange
            var dataNova = new DateOnly(2000, 1, 1);

            // Act
            async Task action() => await _negocio.AtualizarDataNascimento(cpf, dataNova);

            // Assert
            var exception = Assert.ThrowsAsync<BusinessException>(action);
            Assert.IsTrue(exception.Message == string.Format(BusinessMessages.CPFInvalido, cpf));
        }

        [Test]
        public void AtualizarDataNascimento_DataFutura_LancaExcecao()
        {
            // Arrange
            var nomePaciente = "Mariana";
            var dataAntiga = new DateOnly(1990, 1, 1);
            var cpf = "07325503601";
            var dataFutura = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
            var paciente = new Paciente { nome = nomePaciente, cpf = cpf, dataNascimento = dataAntiga, dataCriacao = DateTime.Now };
            _contexto.Add(paciente);
            _contexto.SaveChanges();
            // Act
            async Task action() => await _negocio.AtualizarDataNascimento(paciente.cpf, dataFutura);
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
            var cpf = "42335388649";
            var dataInvalida = new DateOnly(1899, 12, 31);
            var paciente = new Paciente { nome = nomePaciente, cpf = cpf, dataNascimento = dataAntiga, dataCriacao = DateTime.Now };
            _contexto.Add(paciente);
            _contexto.SaveChanges();

            // Act
            async Task action() => await _negocio.AtualizarDataNascimento(paciente.cpf, dataInvalida);

            // Assert
            var exception = Assert.ThrowsAsync<BusinessException>(action);
            Assert.IsTrue(exception.Message == BusinessMessages.DataNascimentoInvalida);
        }

        // --- TESTES DE DELETE (DELETAR) ---

        [Test]
        public async Task Deletar_Sucesso()
        {
            // Arrange
            var paciente = new Paciente { nome = "Paciente Deletar", cpf = "70200360663", dataNascimento = new DateOnly(1980, 1, 1), dataCriacao = DateTime.Now };
            _contexto.Add(paciente);
            await _contexto.SaveChangesAsync();

            // Act
            async Task action() => await _negocio.Deletar(paciente.cpf);

            // Assert
            Assert.DoesNotThrowAsync(action);
        }

        [TestCase("06631299748")]
        [TestCase("00000000000")]
        [TestCase("CPF Invalido")]
        [TestCase("81322222222")]
        public void Deletar_Inexistente_LancaExcecao(string cpfInvalido)
        {
            // Arrange
            var paciente = new Paciente { nome = "Paciente", cpf = "04516584697", dataNascimento = new DateOnly(1980, 1, 1), dataCriacao = DateTime.Now };
            _contexto.Add(paciente);
            _contexto.SaveChanges();

            // Act
            async Task action() => await _negocio.Deletar(cpfInvalido);

            // Assert
            var exception = Assert.ThrowsAsync<BusinessException>(action);
            Assert.IsTrue(exception.Message == string.Format(BusinessMessages.CPFInvalido, cpfInvalido));
        }
    }
}
