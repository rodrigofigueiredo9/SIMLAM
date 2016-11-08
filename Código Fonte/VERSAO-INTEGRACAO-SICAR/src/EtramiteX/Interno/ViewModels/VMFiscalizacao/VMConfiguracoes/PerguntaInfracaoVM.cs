using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao.Configuracoes;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMConfiguracoes
{
	public class PerguntaInfracaoVM
	{
		public Boolean IsVisualizar { get; set; }

		private PerguntaInfracao _entidade = new PerguntaInfracao();
		public PerguntaInfracao Entidade
		{
			get { return _entidade; }
			set { _entidade = value; }
		}

		private List<SelectListItem> _respostas = new List<SelectListItem>();
		public List<SelectListItem> Respostas
		{
			get { return _respostas; }
			set { _respostas = value; }
		}
	
		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@NomeItemObrigatorio = Mensagem.FiscalizacaoConfiguracao.PerguntaNomeObrigatorio,
					@RespostaObrigatoria = Mensagem.FiscalizacaoConfiguracao.RespostaObrigatoria,
					@RespostaDuplicada = Mensagem.FiscalizacaoConfiguracao.RespostaDuplicada,
					@RespostaEspecificarObrigatorio = Mensagem.FiscalizacaoConfiguracao.RespostaEspecificarObrigatorio,
					@RespostaListaObrigatoria = Mensagem.FiscalizacaoConfiguracao.RespostaListaObrigatoria
					
				});
			}
		}

		public String IdsTela
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					/*@TipoPerguntaTexto = eTipoPergunta.Texto,
					@TipoPerguntaNumerico = eTipoPergunta.Numerico,*/
				});
			}
		}

		public PerguntaInfracaoVM(PerguntaInfracao entidade, List<Item> respostas, bool isVisualizar = false)
		{
			Entidade = entidade;
			Respostas = ViewModelHelper.CriarSelectList(respostas, true, true);
			IsVisualizar = isVisualizar;
		}
	}
}