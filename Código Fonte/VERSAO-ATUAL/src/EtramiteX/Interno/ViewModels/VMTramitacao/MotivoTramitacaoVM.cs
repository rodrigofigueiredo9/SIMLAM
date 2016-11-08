using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMTramitacao
{
	public class MotivoTramitacaoVM
	{
		private List<Motivo> _motivos = new List<Motivo>();
		public List<Motivo> Motivos
		{
			get { return _motivos; }
			set { _motivos = value; }
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@NomeObrigatorio = Mensagem.Tramitacao.MotivoObrigatorio,
					@NomeJaAdicionado = Mensagem.Tramitacao.NomeJaAdicionado
				});
			}
		}
	}
}