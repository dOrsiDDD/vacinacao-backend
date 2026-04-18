namespace AgendamentoVacinacao.Entities.DTO
{
    public class PacienteDTO
    {
        public string nome { get; set; }
        public DateOnly dataNascimento { get; set; }
        public DateTime dataCriacao { get; set; }

        public ICollection<AgendamentoDTO> agendamentos { get; set; } = new List<AgendamentoDTO>();
    }
}
