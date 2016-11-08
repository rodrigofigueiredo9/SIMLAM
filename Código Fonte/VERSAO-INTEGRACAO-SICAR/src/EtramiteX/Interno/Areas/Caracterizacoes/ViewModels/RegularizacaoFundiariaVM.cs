using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloRegularizacaoFundiaria;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels
{
	public class RegularizacaoFundiariaVM
	{
		public Boolean IsVisualizar { get; set; }
		public String TextoMerge { get; set; }
		public String AtualizarDependenciasModalTitulo { get; set; }

		private RegularizacaoFundiaria _caracterizacao = new RegularizacaoFundiaria();
		public RegularizacaoFundiaria Caracterizacao
		{
			get { return _caracterizacao; }
			set { _caracterizacao = value; }
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@TempoDeOcupacaoObrigatorio = Mensagem.RegularizacaoFundiaria.TempoDeOcupacaoObrigatorio,
					@TempoDeOcupacaoMaiorZero = Mensagem.RegularizacaoFundiaria.TempoDeOcupacaoMaiorZero,
					@TrasmitenteObrigatorio = Mensagem.RegularizacaoFundiaria.TrasmitenteObrigatorio,
					@AreaRequeridaObrigatoria = Mensagem.RegularizacaoFundiaria.AreaRequeridaObrigatoria,
					@AreaRequeridaInvalida = Mensagem.RegularizacaoFundiaria.AreaRequeridaInvalida,
					@AreaRequiridaMaiorZero = Mensagem.RegularizacaoFundiaria.AreaRequeridaMaiorZero,
					@TipoRegularizacaoObrigatorio = Mensagem.RegularizacaoFundiaria.TipoRegularizacaoObrigatorio,
					@CentroComercialObrigatorio = Mensagem.RegularizacaoFundiaria.CentroComercialObrigatorio,
					@CampoObrigatorio = Mensagem.RegularizacaoFundiaria.CampoObrigatorio,
					@TipoEdificacaoObrigatorio = Mensagem.RegularizacaoFundiaria.TipoEdificacaoObrigatorio,
					@QuantidadeEdificacaoObrigatorio = Mensagem.RegularizacaoFundiaria.QuantidadeEdificacaoObrigatorio,
					@BenfeitoriasEdificacoesObrigatorio = Mensagem.RegularizacaoFundiaria.BenfeitoriasEdificacoesObrigatorio,
					@UsoSoloObrigatorio = Mensagem.RegularizacaoFundiaria.UsoSoloObrigatorio,
					@UsoSoloTipoJaAdicionado = Mensagem.RegularizacaoFundiaria.UsoSoloTipoJaAdicionado,
					@UsoSoloAreaObrigatorio = Mensagem.RegularizacaoFundiaria.UsoSoloAreaObrigatorio,
					@UsoSoloAreaMaiorZero = Mensagem.RegularizacaoFundiaria.UsoSoloAreaMaiorZero,
					@UsoSoloTipoObrigatorio =  Mensagem.RegularizacaoFundiaria.UsoSoloTipoObrigatorio,
					@UsoSoloLimitePorcentagem = Mensagem.RegularizacaoFundiaria.UsoSoloLimitePorcentagem,
					@TrasmitenteJaAdicionado = Mensagem.RegularizacaoFundiaria.TrasmitenteJaAdicionado,
					@RelacaoTrabalhoObrigatorio = Mensagem.RegularizacaoFundiaria.RelacaoTrabalhoObrigatorio
				});
			}
		}

		public String IdsTela
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@TipoMatriculaId = eDominioTipo.Matricula,
					@ZonaLocalizacaoRural = eZonaLocalizacao.Rural,
					@ZonaLocalizacaoUrbana = eZonaLocalizacao.Urbana
				});
			}
		}

		public List<SelectListItem> Homologacoes { set; get; }
		public List<RelacaoTrabalho> RelacoesTrabalho { get; set; }
		public List<SelectListItem> TipoLimite { set; get; }
		public List<SelectListItem> TipoRegularizacao { set; get; }
		public List<SelectListItem> TipoUso { set; get; }

		public RegularizacaoFundiariaVM(RegularizacaoFundiaria regularizacao, bool isVisualizar = false)
		{
			Caracterizacao = regularizacao;
			IsVisualizar = isVisualizar;
		}

		public RegularizacaoFundiariaVM(RegularizacaoFundiaria regularizacao, List<RelacaoTrabalho> relacoesTrabalho, List<Lista> limite,
			List<Lista> regularizacaoTipo, List<UsoAtualSoloLst> tipoUso, List<Lista> homologacoes, bool isVisualizar = false)
		{
			Caracterizacao = regularizacao;
			IsVisualizar = isVisualizar;
			Opcao opcaoAux = null;
			string homologacaoSelecionada = string.Empty;
			string limiteSelecionado = string.Empty;

			if (Caracterizacao != null)
			{
				homologacaoSelecionada = MontarRadioCheck(eTipoOpcao.TerrenoDevoluto).Outro;
				opcaoAux = MontarRadioCheck(eTipoOpcao.SobrepoeSeDivisa);

				if (Convert.ToBoolean(opcaoAux.Valor))
				{
					limiteSelecionado = opcaoAux.Outro;
				}
			}

			RelacoesTrabalho = relacoesTrabalho;
			Homologacoes = ViewModelHelper.CriarSelectList(homologacoes, true, selecionado: homologacaoSelecionada);
			TipoLimite = ViewModelHelper.CriarSelectList(limite, true, selecionado: limiteSelecionado);
			TipoUso = ViewModelHelper.CriarSelectList(tipoUso, true);
			TipoRegularizacao = ViewModelHelper.CriarSelectList(regularizacaoTipo, true);
		}

		public Opcao MontarRadioCheck(eTipoOpcao tipo)
		{
			Opcao opcao = Caracterizacao.Posse.Opcoes.SingleOrDefault(x => x.TipoEnum == tipo) ?? new Opcao() { Tipo = (int)tipo };
			return opcao;
		}
	}
}