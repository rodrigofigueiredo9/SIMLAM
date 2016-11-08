using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao
{
	public class AlterarSituacaoVM
	{
		public Boolean IsVisualizar { get; set; }

		private Fiscalizacao _fiscalizacao = new Fiscalizacao();
		public Fiscalizacao Fiscalizacao
		{
			get { return _fiscalizacao; }
			set { _fiscalizacao = value; }
		}

		private List<SelectListItem> _setores = new List<SelectListItem>();
		public List<SelectListItem> Setores
		{
			get { return _setores; }
			set { _setores = value; }
		}

		private List<SelectListItem> _segmentos = new List<SelectListItem>();
		public List<SelectListItem> Segmentos
		{
			get { return _segmentos; }
			set { _segmentos = value; }
		}

		private List<SelectListItem> _situacaoAtual = new List<SelectListItem>();
		public List<SelectListItem> SituacaoAtual
		{
			get { return _situacaoAtual; }
			set { _situacaoAtual = value; }
		}

		private List<SelectListItem> _situacaoNova = new List<SelectListItem>();
		public List<SelectListItem> SituacaoNova
		{
			get { return _situacaoNova; }
			set { _situacaoNova = value; }
		}

		public AlterarSituacaoVM(Fiscalizacao fiscalizacao, List<Setor> setores, List<Segmento> segmentos, List<Lista> situacoes)
		{
			Fiscalizacao = fiscalizacao;

			Setores = ViewModelHelper.CriarSelectList(setores, true, true, selecionado: Fiscalizacao.LocalInfracao.SetorId.ToString());
			Segmentos = ViewModelHelper.CriarSelectList(segmentos, true, true, selecionado: Fiscalizacao.AutuadoEmpreendimento.Segmento.GetValueOrDefault().ToString());
			SituacaoAtual = ViewModelHelper.CriarSelectList(situacoes, true, true, selecionado: Fiscalizacao.SituacaoId.ToString());

			List<Lista> _situacaonovalst = new List<Lista>();
			if (Fiscalizacao.SituacaoId == (int)eFiscalizacaoSituacao.CadastroConcluido)
			{
				_situacaonovalst = situacoes.Where(x => x.Id == ((int)eFiscalizacaoSituacao.CancelarConclusao).ToString()).ToList();
			}
			else 
			{
				if (Fiscalizacao.SituacaoId == (int)eFiscalizacaoSituacao.Protocolado)
				{
					_situacaonovalst = situacoes.Where(x => x.Id != ((int)eFiscalizacaoSituacao.Protocolado).ToString() &&
										 x.Id != ((int)eFiscalizacaoSituacao.EmAndamento).ToString() &&
										 x.Id != ((int)eFiscalizacaoSituacao.CadastroConcluido).ToString() &&
										 x.Id != ((int)eFiscalizacaoSituacao.CancelarConclusao).ToString()).ToList();
				}
				else 
				{
					_situacaonovalst = situacoes.Where(x => x.Id != ((int)eFiscalizacaoSituacao.Protocolado).ToString() &&
															x.Id != ((int)eFiscalizacaoSituacao.CadastroConcluido).ToString() &&
															x.Id != ((int)eFiscalizacaoSituacao.CancelarConclusao).ToString() &&
															x.Id != ((int)eFiscalizacaoSituacao.EmAndamento).ToString() &&
															x.Id != Fiscalizacao.SituacaoId.ToString()).ToList();
				}
			}

			SituacaoNova = ViewModelHelper.CriarSelectList(_situacaonovalst, true, true);
		}
		
	}
}