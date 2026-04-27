using AgendamentoVacinacao.Entities.Enum;

namespace AgendamentoVacinacao.Entities.Entities
{
    public class Usuario : IdEntity<int>
    {
        public string nome { get; set; }
        public string login { get; set; }
        public string email { get; set; }
        public byte[] passwordHash { get; set; }
        public byte[] passwordSalt { get; set; }
        public PerfilEnum perfil { get; set; }
        public DateTime dataCriacao { get; set; }
        public Usuario()
        {

        }
    }
}

