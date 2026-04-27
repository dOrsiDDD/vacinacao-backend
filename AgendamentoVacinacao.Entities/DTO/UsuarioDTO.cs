using AgendamentoVacinacao.Entities.Enum;

namespace AgendamentoVacinacao.Entities.DTO
{
    public class UsuarioDTO
    {
        public string nome { get; set; }
        public string email { get; set; }
        public PerfilEnum perfil { get; set; }
        public DateTime dataCriacao { get; set; }
    }
}
