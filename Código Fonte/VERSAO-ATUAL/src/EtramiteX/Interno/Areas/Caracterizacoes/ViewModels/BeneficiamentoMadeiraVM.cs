using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBeneficiamentoMadeira;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels
{
	public class BeneficiamentoMadeiraVM
	{
		public bool IsVisualizar { get; set; }
		public bool IsEditar { get; set; }
		public String TextoAbrirModal { get; set; }
		public String TextoMerge { get; set; }
		public String AtualizarDependenciasModalTitulo { get; set; }

		private List<BeneficiamentoMadeiraBeneficiamentoVM> _beneficiamentoMadeiraBeneficiamentosVM = new List<BeneficiamentoMadeiraBeneficiamentoVM>();
		public List<BeneficiamentoMadeiraBeneficiamentoVM> BeneficiamentoMadeiraBeneficiamentosVM
		{
			get { return _beneficiamentoMadeiraBeneficiamentosVM; }
			set { _beneficiamentoMadeiraBeneficiamentosVM = value; }
		}

		public BeneficiamentoMadeiraBeneficiamentoVM BeneficiamentoMadeiraBeneficiamentosTemplateVM {get;set;}

		private BeneficiamentoMadeira _caracterizacao = new BeneficiamentoMadeira();
		public BeneficiamentoMadeira Caracterizacao
		{
			get { return _caracterizacao; }
			set { _caracterizacao = value; }
		}

		public Int32 QtdAtividade
		{
			get 
			{
				return 2;
			}
		}

		public String IdsTela
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@SerrariasQuandoNaoAssociadasAFabricacaoDeEstruturas = ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.SerrariasQuandoNaoAssociadasAFabricacaoDeEstruturas),
					@FabricacaoDeEstruturasDeMadeiraComAplicacaoRural = ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.FabricacaoDeEstruturasDeMadeiraComAplicacaoRural),
				});
			}
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@GeometriaTipoObrigatorio = Mensagem.CoordenadaAtividade.GeometriaTipoObrigatorio,
					@CoordenadaAtividadeObrigatoria = Mensagem.CoordenadaAtividade.CoordenadaAtividadeObrigatoria,

					@MateriaPrimaFlorestalConsumidaObrigatoria = Mensagem.MateriaPrimaFlorestalConsumida.MateriaPrimaFlorestalConsumidaObrigatoria,
					@MateriaPrimaFlorestalConsumidaDuplicada = Mensagem.MateriaPrimaFlorestalConsumida.MateriaPrimaFlorestalConsumidaDuplicada,
					@UnidadeMateriaPrimaObrigatoria = Mensagem.MateriaPrimaFlorestalConsumida.UnidadeMateriaPrimaObrigatoria,

					@QuantidadeMateriaPrimaObrigatoria = Mensagem.MateriaPrimaFlorestalConsumida.QuantidadeMateriaPrimaObrigatoria,
					@QuantidadeMateriaPrimaInvalida = Mensagem.MateriaPrimaFlorestalConsumida.QuantidadeMateriaPrimaInvalida,
					@QuantidadeMateriaPrimaMairZero = Mensagem.MateriaPrimaFlorestalConsumida.QuantidadeMateriaPrimaMaiorZero,

					@EspecificarMateriaPrimaObrigatorio = Mensagem.MateriaPrimaFlorestalConsumida.EspecificarMateriaPrimaObrigatorio,

					@ListaAtividadeObrigatoria = Mensagem.BeneficiamentoMadeira.ListaAtividadeObrigatoria
				});
			}
		}

		public BeneficiamentoMadeiraVM(BeneficiamentoMadeira caracterizacao, bool isVisualizar = false, bool isEditar = false)
		{
			Caracterizacao = caracterizacao;
			IsVisualizar = isVisualizar;
			IsEditar = isEditar;
		}
		
	}
}