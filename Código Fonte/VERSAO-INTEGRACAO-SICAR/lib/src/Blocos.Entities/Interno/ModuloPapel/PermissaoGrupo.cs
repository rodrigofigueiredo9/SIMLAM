

using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloPapel
{
    public class PermissaoGrupo
    {
        public string Nome { get; set; }
        public List<Permissao> PermissaoColecao { get; set; }
    }
}