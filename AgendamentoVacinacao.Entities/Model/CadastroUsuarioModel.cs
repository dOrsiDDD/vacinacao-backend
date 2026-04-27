using AgendamentoVacinacao.Entities.Enum;

namespace AgendamentoVacinacao.Entities.Model
{
    public class CadastroUsuarioModel
    {
        public string nome { get; set; }
        public string email { get; set; }
        public PerfilEnum perfil { get; set; }
        public string login { get; set; }
        public string senha { get; set; }
    }
}
