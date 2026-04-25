using AgendamentoVacinacao.Business.Business;
using AgendamentoVacinacao.Business.Interface.IBusiness;
using AgendamentoVacinacao.Entities.Entities;
using AgendamentoVacinacao.Entities.Enum;
using AgendamentoVacinacao.Entities.Model;
using AgendamentoVacinacao.Repository.Interface.IRepositories;
using AgendamentoVacinacao.Repository.Repositories;
using AgendamentoVacinacao.Utilities.Constants;
using AgendamentoVacinacao.Utilities.Exceptions;
using AgendamentoVacinacao.Utilities.Messages;
using NUnit.Framework;


namespace AgendamentoVacinacao.TestesUnitarios.Business
{
    public class AgendamentoBusinessTeste : TesteUnitarioBase
    {
        private IAgendamentoBusiness _negocio;
        private IAgendamentoRepository _repositorio;
        private Paciente _pacienteBase;

        [SetUp]
        public void SetUp()
        {
            _repositorio = new AgendamentoRepository(_contexto);

            RegistrarObjeto(typeof(IAgendamentoRepository), _repositorio);
            RegistrarObjeto(typeof(IAgendamentoBusiness), new AgendamentoBusiness(_repositorio));

            _negocio = ObterServico<IAgendamentoBusiness>();
        }

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            // Cria um paciente base para satisfazer a chave estrangeira dos agendamentos
            _pacienteBase = new Paciente { nome = "Paciente Padrão", cpf = "41445406420", dataNascimento = new DateOnly(1990, 1, 1), dataCriacao = DateTime.Now };
            _contexto.Add(_pacienteBase);
            _contexto.SaveChanges();
        }

        // --- TESTES DE CREATE (INSERIR E VALIDAÇÃO) ---

        [Test]
        public async Task InserirAgendamento_Sucesso()
        {
            // Arrange
            var novoAgendamento = new CadastroAgendamentoModel
            {
                idPaciente = _pacienteBase.Id,
                dataAgendamento = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                horaAgendamento = new TimeOnly(10, 0)
            };

            // Act
            async Task action() => await _negocio.Inserir(novoAgendamento);

            // Assert
            Assert.DoesNotThrowAsync(action);
        }

        [Test]
        public async Task InserirAgendamento_PacienteJaAgendadoNoMesmoHorario_LancaExcecao()
        {
            // Arrange
            var dataAgendamento = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
            var horaAgendamento = new TimeOnly(14, 0);

            var agendamentoExistente = new Agendamento
            {
                idPaciente = _pacienteBase.Id,
                dataAgendamento = dataAgendamento,
                horaAgendamento = horaAgendamento,
                status = "pendente",
                dataCriacao = DateTime.Now
            };

            _contexto.Add(agendamentoExistente);
            await _contexto.SaveChangesAsync();
            _contexto.ChangeTracker.Clear();

            var novoAgendamentoModel = new CadastroAgendamentoModel
            {
                idPaciente = _pacienteBase.Id,
                dataAgendamento = dataAgendamento,
                horaAgendamento = horaAgendamento
            };

            // Act
            async Task action() => await _negocio.Inserir(novoAgendamentoModel);

            // Assert
            var exception = Assert.ThrowsAsync<BusinessException>(action);
            Assert.IsTrue(exception.Message == string.Format(BusinessMessages.PacienteJaAgendado, _pacienteBase.Id));
        }

        [Test]
        public void ValidarAgendamento_DataPassada_LancaExcecao()
        {
            // Arrange
            var dataPassada = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
            var horaValida = new TimeOnly(10, 0);

            // Act
            async Task action() => await _negocio.ValidarAgendamento(dataPassada, horaValida, _pacienteBase.Id);

            // Assert
            var exception = Assert.ThrowsAsync<BusinessListException>(action);
            Assert.IsTrue(exception.Messages.Contains(string.Format(BusinessMessages.DataPassada)));
        }

        [Test]
        public void ValidarAgendamento_HorarioMinutosInvalido_LancaExcecao()
        {
            // Arrange
            var dataValida = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
            var horaInvalida = new TimeOnly(10, 30); 

            // Act
            async Task action() => await _negocio.ValidarAgendamento(dataValida, horaInvalida, _pacienteBase.Id);

            // Assert
            var exception = Assert.ThrowsAsync<BusinessListException>(action);
            Assert.IsTrue(exception.Messages.Contains(string.Format(BusinessMessages.HorarioInvalido, horaInvalida)));
        }

        [Test]
        public async Task ValidarAgendamento_DiaEsgotado_LancaExcecao()
        {
            // Arrange
            var dataAgendamento = DateOnly.FromDateTime(DateTime.Today.AddDays(2));

            // Adiciona agendamentos até o limite diário
            for (int i = 0; i < BusinessConstants.MAX_AGENDAMENTOS_POR_DIA; i++)
            {
                // Espalha as horas para não esgotar o limite por horário
                var hora = new TimeOnly(8 + (i % 10), 0);
                _contexto.Add(new Agendamento
                {
                    idPaciente = _pacienteBase.Id,
                    dataAgendamento = dataAgendamento,
                    horaAgendamento = hora,
                    status = "pendente",
                    dataCriacao = DateTime.Now
                });
            }
            await _contexto.SaveChangesAsync();

            var horaNovoAgendamento = new TimeOnly(18, 0);

            // Act
            async Task action() => await _negocio.ValidarAgendamento(dataAgendamento, horaNovoAgendamento, _pacienteBase.Id);

            // Assert
            var exception = Assert.ThrowsAsync<BusinessListException>(action);
            Assert.IsTrue(exception.Messages.Contains(string.Format(BusinessMessages.DiaEsgotado, BusinessConstants.MAX_AGENDAMENTOS_POR_DIA)));
        }

        [Test]
        public async Task ValidarAgendamento_HorarioEsgotado_LancaExcecao()
        {
            // Arrange
            var dataAgendamento = DateOnly.FromDateTime(DateTime.Today.AddDays(4));
            var horaEsgotada = new TimeOnly(10, 0);

            // Adiciona agendamentos até o limite do horário
            for (int i = 0; i < BusinessConstants.MAX_AGENDAMENTOS_POR_HORARIO; i++)
            {
                _contexto.Add(new Agendamento
                {
                    idPaciente = _pacienteBase.Id,
                    dataAgendamento = dataAgendamento,
                    horaAgendamento = horaEsgotada,
                    status = "pendente",
                    dataCriacao = DateTime.Now
                });
            }
            await _contexto.SaveChangesAsync();

            // Act
            async Task action() => await _negocio.ValidarAgendamento(dataAgendamento, horaEsgotada, _pacienteBase.Id);

            // Assert
            var exception = Assert.ThrowsAsync<BusinessListException>(action);
            Assert.IsTrue(exception.Messages.Contains(string.Format(BusinessMessages.HorarioEsgotado, BusinessConstants.MAX_AGENDAMENTOS_POR_HORARIO)));
        }

        // --- TESTES DE READ (OBTER, LISTAR, CONSULTAR, FILTRAR) ---

        [Test]
        public async Task ListarAgendamentos_Sucesso()
        {
            // Arrange
            _contexto.Add(new Agendamento { idPaciente = _pacienteBase.Id, dataAgendamento = DateOnly.FromDateTime(DateTime.Today), horaAgendamento = new TimeOnly(10, 0), status = "pendente" });
            await _contexto.SaveChangesAsync();

            // Act
            async Task action() => await _negocio.ListarAgendamentos();

            // Assert
            Assert.DoesNotThrowAsync(action);
        }

        [Test]
        public async Task ConsultarAgendamentosPorDia_Sucesso()
        {
            // Arrange
            var dataBusca = DateOnly.FromDateTime(DateTime.Today.AddDays(5));
            _contexto.Add(new Agendamento { idPaciente = _pacienteBase.Id, dataAgendamento = dataBusca, horaAgendamento = new TimeOnly(10, 0), status = "pendente" });
            await _contexto.SaveChangesAsync();

            // Act
            async Task action() => await _negocio.ConsultarAgendamentosPorDia(dataBusca);

            // Assert
            Assert.DoesNotThrowAsync(action);
        }

        [Test]
        public async Task ConsultarAgendamentosPorHorario_Sucesso()
        {
            // Arrange
            var data = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
            var hora = new TimeOnly(15, 0);
            var dateTimeBusca = data.ToDateTime(hora);

            _contexto.Add(new Agendamento { idPaciente = _pacienteBase.Id, dataAgendamento = data, horaAgendamento = hora, status = "pendente" });
            await _contexto.SaveChangesAsync();

            // Act
            async Task action() => await _negocio.ConsultarAgendamentosPorHorario(dateTimeBusca);

            // Assert
            Assert.DoesNotThrowAsync(action);
        }

        [Test]
        public async Task FiltrarAgendamentos_Sucesso()
        {
            // Arrange
            _contexto.Add(new Agendamento { idPaciente = _pacienteBase.Id, dataAgendamento = DateOnly.FromDateTime(DateTime.Today), horaAgendamento = new TimeOnly(10, 0), status = StatusEnum.Pendente.ToString().ToLower() });
            await _contexto.SaveChangesAsync();

            // Act
            async Task action() => await _negocio.FiltrarAgendamentos(StatusEnum.Pendente);

            // Assert
            Assert.DoesNotThrowAsync(action);
        }

        // --- TESTES DE UPDATE (ATUALIZAR) ---

        [Test]
        public async Task AtualizarData_Sucesso()
        {
            // Arrange
            var agendamento = new Agendamento { idPaciente = _pacienteBase.Id, dataAgendamento = DateOnly.FromDateTime(DateTime.Today), horaAgendamento = new TimeOnly(9, 0), status = "pendente" };
            _contexto.Add(agendamento);
            await _contexto.SaveChangesAsync();

            var novaData = DateTime.Today.AddDays(5).AddHours(10);

            // Act
            async Task action() => await _negocio.AtualizarData(agendamento.Id, novaData);

            // Assert
            Assert.DoesNotThrowAsync(action);
        }

        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(999)]
        public void AtualizarData_Inexistente_LancaExcecao(int idInvalido)
        {
            // Arrange
            var novaData = DateTime.Today.AddDays(1);

            // Act
            async Task action() => await _negocio.AtualizarData(idInvalido, novaData);

            // Assert
            var exception = Assert.ThrowsAsync<BusinessException>(action);
            Assert.IsTrue(exception.Message == string.Format(BusinessMessages.IdInvalido, idInvalido));
        }

        [Test]
        public async Task AtualizarStatus_Sucesso()
        {
            // Arrange
            var agendamento = new Agendamento { idPaciente = _pacienteBase.Id, dataAgendamento = DateOnly.FromDateTime(DateTime.Today), horaAgendamento = new TimeOnly(9, 0), status = "pendente" };
            _contexto.Add(agendamento);
            await _contexto.SaveChangesAsync();

            var novoStatus = StatusEnum.Concluido;

            // Act
            async Task action() => await _negocio.AtualizarStatus(agendamento.Id, novoStatus);

            // Assert
            Assert.DoesNotThrowAsync(action);
        }


        // --- TESTES DE DELETE (DELETAR) ---

        [Test]
        public async Task Deletar_Unico_Sucesso()
        {
            // Arrange
            var agendamento = new Agendamento { idPaciente = _pacienteBase.Id, dataAgendamento = DateOnly.FromDateTime(DateTime.Today), horaAgendamento = new TimeOnly(9, 0), status = "pendente" };
            _contexto.Add(agendamento);
            await _contexto.SaveChangesAsync();

            // Act
            async Task action() => await _negocio.Deletar(agendamento.Id);

            // Assert
            Assert.DoesNotThrowAsync(action);
        }

        [Test]
        public async Task Deletar_Lote_Sucesso()
        {
            // Arrange
            var agendamento1 = new Agendamento { idPaciente = _pacienteBase.Id, dataAgendamento = DateOnly.FromDateTime(DateTime.Today), horaAgendamento = new TimeOnly(9, 0), status = "pendente" };
            var agendamento2 = new Agendamento { idPaciente = _pacienteBase.Id, dataAgendamento = DateOnly.FromDateTime(DateTime.Today), horaAgendamento = new TimeOnly(10, 0), status = "pendente" };

            _contexto.AddRange(agendamento1, agendamento2);
            await _contexto.SaveChangesAsync();

            var idsParaDeletar = new List<int> { agendamento1.Id, agendamento2.Id };

            // Act
            async Task action() => await _negocio.Deletar(idsParaDeletar);

            // Assert
            Assert.DoesNotThrowAsync(action);
        }

        [Test]
        public void Deletar_Lote_ContendoIdInexistente_LancaExcecao()
        {
            // Arrange
            var agendamento = new Agendamento { idPaciente = _pacienteBase.Id, dataAgendamento = DateOnly.FromDateTime(DateTime.Today), horaAgendamento = new TimeOnly(9, 0), status = "pendente" };
            _contexto.Add(agendamento);
            _contexto.SaveChanges();

            var idsParaDeletar = new List<int> { agendamento.Id, 999 };

            // Act
            async Task action() => await _negocio.Deletar(idsParaDeletar);

            // Assert
            var exception = Assert.ThrowsAsync<BusinessException>(action);
            Assert.IsTrue(exception.Message == string.Format(BusinessMessages.IdInvalido, 999));
        }
    }
}
