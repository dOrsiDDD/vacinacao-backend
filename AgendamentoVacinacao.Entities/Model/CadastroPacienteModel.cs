namespace AgendamentoVacinacao.Entities.Model
{
    public class CadastroPacienteModel
    {
        public string nome { get; set; }
        public string cpf { get; set; }
        public DateOnly dataNascimento { get; set; }
    }
}
