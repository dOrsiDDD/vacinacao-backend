using AgendamentoVacinacao.Business.Interface.IBusiness;
using AgendamentoVacinacao.Entities.DTO;
using AgendamentoVacinacao.Entities.Entities;
using AgendamentoVacinacao.Entities.Filter;
using AgendamentoVacinacao.Entities.Model;
using AgendamentoVacinacao.Repository.Interface.IRepositories;
using AgendamentoVacinacao.Utilities.Exceptions;
using AgendamentoVacinacao.Utilities.Messages;
using log4net;
using System.Security.Cryptography;
using System.Text;

namespace AgendamentoVacinacao.Business.Business
{
    public class UsuarioBusiness : IUsuarioBusiness
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(UsuarioBusiness));
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioBusiness(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }
        public async Task<UsuarioDTO> InserirUsuario(CadastroUsuarioModel novoUsuario)
        {
            var usuario = await _usuarioRepository.ObterUsuario(new UsuarioFilter() { email = novoUsuario.email });

            if (usuario != null)
            {
                _log.InfoFormat(BusinessMessages.RegistroExistente, usuario.email);
                throw new BusinessException(string.Format(BusinessMessages.RegistroExistente, usuario.email));
            }

            usuario = CriarUsuario(novoUsuario);

            var usuarioInserido = await _usuarioRepository.Inserir(usuario);

            var usuarioInseridoDTO = new UsuarioDTO { nome = usuarioInserido.nome, 
                                                      email = usuarioInserido.email, 
                                                      perfil = usuarioInserido.perfil ,
                                                      dataCriacao = usuarioInserido.dataCriacao};

            return usuarioInseridoDTO;
        }

        private static Usuario CriarUsuario(CadastroUsuarioModel novoUsuario)
        {
            var usuario = new Usuario
            {
                email = novoUsuario.email,
                nome = novoUsuario.nome,
                dataCriacao = DateTime.Now,
                perfil = novoUsuario.perfil,
                login = novoUsuario.login
            };

            using var hmac = new HMACSHA512();
            usuario.passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(novoUsuario.senha));
            usuario.passwordSalt = hmac.Key;

            return usuario;
        }

        public async Task<List<UsuarioDTO>> ListarTodos()
        {
            return await _usuarioRepository.ListarTodos();
        }

        public async Task<UsuarioDTO> ConsultarUsuario(UsuarioFilter filtro)
        {
            var usuario = await _usuarioRepository.ObterUsuario(filtro);
            if (usuario == null)
            {
                _log.InfoFormat(string.Format(BusinessMessages.RegistroInexistente, "Usuario", filtro));
                throw new BusinessException(string.Format(BusinessMessages.RegistroInexistente, "Usuario", filtro));
            }

            var usuarioDTO = new UsuarioDTO { nome = usuario.nome, 
                                              dataCriacao = usuario.dataCriacao, 
                                              email = usuario.email,
                                              perfil = usuario.perfil};
            return usuarioDTO;
        }
    }
}
