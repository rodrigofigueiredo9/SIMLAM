using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloArquivo;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels
{
	public class ArquivoVM
	{
		private List<Anexo> _anexos = new List<Anexo>();
		public List<Anexo> Anexos {
			get { return _anexos; }
			set { _anexos = value; }
		}

		public Boolean UsarOrdenacao = true;
		public Boolean IsVisualizar = false;

		private List<String> _arquivosPermitidos = new List<String>();
		public List<String> ArquivosPermitidos
		{
			get { return _arquivosPermitidos; }
			set { _arquivosPermitidos = value; }
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@ArquivoDescricaoObrigatorio = Mensagem.Arquivo.DescricaoObrigatorio,
					@ArquivoNenhumSelecionado = Mensagem.Arquivo.NenhumArquivoSelecionadoParaEnviar,
					@ArquivoExistente = Mensagem.Arquivo.ArquivoExistente,
					@ArquivoTipoInvalidoJs = Mensagem.Arquivo.ArquivoTipoInvalido("#arquivo#", ArquivosPermitidos)
				});
			}
		}

		public ArquivoVM() { }
	}
}
