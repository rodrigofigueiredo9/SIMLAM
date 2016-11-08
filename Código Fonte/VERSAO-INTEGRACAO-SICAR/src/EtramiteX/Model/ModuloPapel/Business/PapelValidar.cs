using System;
using Tecnomapas.Blocos.Entities.Interno.ModuloPapel;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloPapel.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloPapel.Business
{
	public class PapelValidar
	{
		PapelDa _daPapel = new PapelDa();

		public bool Salvar(Papel papel)
		{
			if (String.IsNullOrEmpty(papel.Nome))
			{
				Validacao.Add(Mensagem.Papel.NomeObrigatorio);
			}

			if (papel.Permissoes.Count == 0)
			{
				Validacao.Add(Mensagem.Papel.PermissaoObrigatorio);
			}

			return Validacao.EhValido;
		}

		internal bool VerificarExcluir(int id)
		{
			string funcionarios = string.Empty;

			if (_daPapel.PossuiFuncionario(id, out funcionarios))
			{
				Validacao.Add(Mensagem.Papel.PossuiFuncionario(funcionarios));
			}

			return Validacao.EhValido;
		}
	}
}