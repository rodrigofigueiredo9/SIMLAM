using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloRegistroAtividadeFlorestal;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels
{
	public class RegistroAtividadeFlorestalVM
	{
		public bool IsVisualizar { get; set; }
		public RegistroAtividadeFlorestal Caracterizacao { get; set; }
		public List<ConsumoVM> ConsumosVM { get; set; }
		public ConsumoVM ConsumoTemplateVM { get; set; }

		public String IdsTela
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@FabricanteMotosserra = ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.FabricanteMotosserra),
					@ComercianteMotosserra = ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.ComercianteMotosserra)
				});
			}
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					CamposObrigatorio = Mensagem.RegistroAtividadeFlorestal.CamposObrigatorio(-1),
					ExcluirConsumo = Mensagem.RegistroAtividadeFlorestal.ExcluirConsumo
				});
			}
		}

		public RegistroAtividadeFlorestalVM(RegistroAtividadeFlorestal caracterizacao, List<ListaValor> atividades, List<ListaValor> fonteTipos, List<ListaValor> unidades, List<TituloModeloLst> modelosLicenca, bool isVisualizar = false)
		{
			ConsumosVM = new List<ConsumoVM>();

			IsVisualizar = isVisualizar;
			Caracterizacao = caracterizacao;

			if (caracterizacao.Id > 0)
			{
				int count = 0;
				caracterizacao.Consumos.ForEach(x => {
					ConsumosVM.Add(new ConsumoVM(x, atividades, fonteTipos, unidades, modelosLicenca, count.ToString(), isVisualizar));
					count++;
				});
			}
			else
			{
				ConsumosVM.Add(new ConsumoVM(new Consumo(), atividades, fonteTipos, unidades, modelosLicenca, "0", isVisualizar));
			}

			ConsumoTemplateVM = new ConsumoVM(new Consumo { Id = -1 }, atividades, fonteTipos, unidades, modelosLicenca, null, isVisualizar);
		}
	}
}