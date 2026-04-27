using AgendamentoVacinacao.Utilities.UsuarioContexto;
using System.Collections;
using System.Security.Claims;

namespace AgendamentoVacinacao.Utilities.Extensions
{
    public static class UsuarioContextoExtensions
    {
        public static string Login(this IUsuarioContexto usuarioContexto)
        {
            var login = usuarioContexto.GetClaimValue<string>("login");

            return login ?? Environment.MachineName;
        }
        
        public static int Id(this IUsuarioContexto usuarioContexto)
        {
            int.TryParse(usuarioContexto.GetClaimValue<string>(ClaimTypes.Sid), out var id);

            return id;
        }

        public static void AddData<TValue>(this IUsuarioContexto usuarioContexto, string key, TValue data)
        {
            usuarioContexto.AdditionalData ??= new Hashtable();

            if (!usuarioContexto.AdditionalData.ContainsKey(key))
                usuarioContexto.AdditionalData.Add(key, data);
            else
                usuarioContexto.AdditionalData[key] = data;
        }

        private static TResult GetClaimValue<TResult>(this IUsuarioContexto usuarioContexto, string key)
        {
            if (usuarioContexto?.AdditionalData is Hashtable additionalData && additionalData.ContainsKey(key))
                try { return (TResult)additionalData[key]; } catch { return default; }

            return default;
        }
    }
}
