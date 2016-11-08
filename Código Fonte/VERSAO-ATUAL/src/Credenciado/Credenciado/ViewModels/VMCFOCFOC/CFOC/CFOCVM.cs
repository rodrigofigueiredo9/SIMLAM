using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloCFOCFOC;
using Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Credenciado.ViewModels.VMCFOCFOC.CFOC
{
	public class CFOCVM
	{
		public EmissaoCFOC CFOC { get; set; }
		public bool IsVisualizar { get; set; }

		public List<SelectListItem> Produtores { get; set; }
		public List<SelectListItem> Empreendimentos { get; set; }
		public List<SelectListItem> Pragas { get; set; }
		public List<SelectListItem> Estados { get; set; }
		public List<SelectListItem> Municipios { get; set; }
		public List<Lista> ProdutosEspecificacoes { get; set; }
		public List<SelectListItem> Situacoes { get; set; }
		public List<SelectListItem> UnidadesProducao { get; set; }
		public List<SelectListItem> EstadosEmissao { get; set; }
		public List<SelectListItem> MunicipiosEmissao { get; set; }
		public string HoraServidor { get { return DateTime.Today.ToShortDateString(); } }

		public string IdsTela
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@EmElaboracao = eDocumentoFitossanitarioSituacao.EmElaboracao,
					@Valido = eDocumentoFitossanitarioSituacao.Valido,
					@tipoNumeroBloco = eDocumentoFitossanitarioTipoNumero.Bloco,
					@tipoNumeroDigital = eDocumentoFitossanitarioTipoNumero.Digital
				});
			}
		}

		public string Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					EmpreendimentoObrigatorio = Mensagem.EmissaoCFOC.EmpreendimentoObrigatorio
				});
			}
		}

		public CFOCVM(EmissaoCFOC entidade, List<Estado> lstEstados, List<Municipio> lstMunicipios, List<Lista> lstEmpreendimentos,
			List<Lista> lstPragas, List<Lista> lstProdutosEspecificacoes, List<Lista> lstSituacoes, List<Municipio> lstMunicipiosEmissao)
		{
			CFOC = entidade ?? new EmissaoCFOC();

			if (CFOC.Id <= 0)
			{
				if (lstEmpreendimentos != null && lstEmpreendimentos.Count == 1)
				{
					CFOC.EmpreendimentoId = Convert.ToInt32(lstEmpreendimentos.First().Id);
				}
			}

			Empreendimentos = ViewModelHelper.CriarSelectList(lstEmpreendimentos, false, true, entidade.EmpreendimentoId.ToString());
			Situacoes = ViewModelHelper.CriarSelectList(lstSituacoes, false, true, entidade.SituacaoId.ToString());
			ProdutosEspecificacoes = lstProdutosEspecificacoes;
			Pragas = ViewModelHelper.CriarSelectList(lstPragas);
			Estados = ViewModelHelper.CriarSelectList(lstEstados);
			Municipios = ViewModelHelper.CriarSelectList(lstMunicipios);
			EstadosEmissao = ViewModelHelper.CriarSelectList(lstEstados, selecionado: CFOC.EstadoEmissaoId.ToString());
			MunicipiosEmissao = ViewModelHelper.CriarSelectList(lstMunicipiosEmissao, selecionado: CFOC.MunicipioEmissaoId.ToString());

			if (CFOC.EstadoId > 0)
			{
				Estados.Single(x => x.Value == CFOC.EstadoId.ToString()).Selected = true;
			}

			if (CFOC.MunicipioId > 0)
			{
				Municipios.Single(x => x.Value == CFOC.MunicipioId.ToString()).Selected = true;
			}
		}
	}
}