using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloAgrotoxico;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMAgrotoxico
{
	public class AgrotoxicoCulturaVM
	{
		public bool IsVisualizar { get; set; }

		private List<ConfiguracaoVegetalItem> _modalidadesAplicacoes = new List<ConfiguracaoVegetalItem>();
		public List<ConfiguracaoVegetalItem> ModalidadesAplicacoes
		{
			get { return _modalidadesAplicacoes; }
			set { _modalidadesAplicacoes = value; }
		}

		private AgrotoxicoCultura _agrotoxicoCultura = new AgrotoxicoCultura();
		public AgrotoxicoCultura AgrotoxicoCultura
		{
			get { return _agrotoxicoCultura; }
			set { _agrotoxicoCultura = value; }
		}

		public string Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@PragaJaAdicionada = Mensagem.Agrotoxico.PragaJaAdicionada,
					@CulturaObrigatorio = Mensagem.Agrotoxico.CulturaObrigatoria,
					@PragaObrigatorio = Mensagem.Agrotoxico.PragaObrigatoria,
					@ModalidadeAplicacaoObrigatorio = Mensagem.Agrotoxico.ModalidadeAplicacaoObrigatoria,
					@IntervaloSegurancaObrigatorio = Mensagem.Agrotoxico.IntervaloSegurancaObrigatorio
				});
			}
		}

		public AgrotoxicoCulturaVM(){}

		public AgrotoxicoCulturaVM(AgrotoxicoCultura agrotoxicoCultura, List<ConfiguracaoVegetalItem> modalidadesAplicacoes, bool isVisualizar = false)
		{
			IsVisualizar = isVisualizar;
			AgrotoxicoCultura = agrotoxicoCultura;
			ModalidadesAplicacoes = modalidadesAplicacoes;
		}
	}
}