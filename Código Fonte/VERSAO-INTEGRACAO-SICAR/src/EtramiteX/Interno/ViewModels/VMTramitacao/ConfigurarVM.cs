using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMTramitacao
{
	public class ConfigurarVM
	{
		private List<Setor> _setores = new List<Setor>();
		public List<Setor> Setores
		{
			get { return _setores; }
			set { _setores = value; }
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					FuncionarioJaAdicionado = Mensagem.Tramitacao.FuncionarioJaAdicionado,
					FuncionarioObrigatorio = Mensagem.Tramitacao.FuncionarioObrigatorio
				});
			}
		}
		
	}
}