using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicenca;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels
{
	public class BarragemDispensaLicencaVM
	{
		public BarragemDispensaLicenca Caracterizacao { get; set; }
		public List<SelectListItem> Atividades { get; set; }
		public List<Lista> FinalidadesAtividade { set; get; }
		public List<Lista> FormacoesRTLst { set; get; }
		public List<Lista> BarragemTiposLst { get; set; }
		public List<Lista> FasesLst { get; set; }
		public List<SelectListItem> MongeTiposLst { get; set; }
		public List<SelectListItem> VertedouroTiposLst { get; set; }
		public List<List<SelectListItem>> profissoesLst { get; set; }
		public List<int> ProfissoesAutorizacao { get; set; }
		public bool IsVisualizar { get; set; }
        public int ProjetoDigitalId { get; set; }
        public int ProtocoloId { get; set; }
        public int RequerimentoId { get; set; }
		public string UrlRetorno { get; set; }
		public bool rtElaborador { get; set; }
		public List<BarragemDispensaLicenca> CaracterizacoesCadastradas { get; set; } = new List<BarragemDispensaLicenca>();
		public List<BarragemDispensaLicenca> CaracterizacoesAssociadas { get; set; } = new List<BarragemDispensaLicenca>();

		public string Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					FormacaoRTOutros = Mensagem.BarragemDispensaLicenca.FormacaoRTOutros,
					ArquivoObrigatorio = Mensagem.Arquivo.ArquivoObrigatorio
				});
			}
		}

		public string IdsTela
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					BarragemTipoTerra = eBarragemTipo.Terra,
					FaseConstruida = eFase.Construida,
					FaseAConstruir = eFase.AConstruir,
					MongeTipoOutros = eMongeTipo.Outros,
					VertedouroTipoOutros = eVertedouroTipo.Outros,
                    FormacaoRTEngenheiroCivil = eFormacaoRTCodigo.EngenheiroCivil,
					FormacaoRTOutros = eFormacaoRTCodigo.Outros
				});
			}
		}

		public BarragemDispensaLicencaVM(BarragemDispensaLicenca entidade, Atividade atividade, List<Lista> finalidades, List<Lista> formacoesRT, List<Lista> barragemTipos, List<Lista> fases, List<Lista> mongeTipos, List<Lista> vertedouroTipos, List<ProfissaoLst> profissoes)
		{
			Caracterizacao = entidade ?? new BarragemDispensaLicenca();
			List<Lista> atividades = new List<Lista>();
			profissoesLst = new List<List<SelectListItem>>();


			atividades.Add(new Lista() { Id = atividade.Id.ToString(), Texto = atividade.NomeAtividade });
			Atividades = ViewModelHelper.CriarSelectList(atividades, isFiltrarAtivo: false, itemTextoPadrao: false);

			FinalidadesAtividade = finalidades.Select(x => new Lista()
			{
				Codigo = x.Codigo,
				Id = x.Id,
				Texto = x.Texto,
				Tid = x.Tid,
				Tipo = x.Tipo,
				IsAtivo = entidade.finalidade.Exists(y => y == Convert.ToInt32(x.Id))
			}).ToList();
			barragemTipos.ForEach(x => {
				x.IsAtivo = (x.Id == ((int)Caracterizacao.BarragemTipo).ToString()) ? true : false;
			});
			ProfissoesAutorizacao = new List<int>() { 15, 37, 38 };

			FasesLst = fases;
			FormacoesRTLst = formacoesRT;
			BarragemTiposLst = barragemTipos;
			MongeTiposLst = ViewModelHelper.CriarSelectList(mongeTipos, isFiltrarAtivo: true, itemTextoPadrao: true, selecionado: Caracterizacao.construidaConstruir.vazaoMinTipo.ToString());
			VertedouroTiposLst = ViewModelHelper.CriarSelectList(vertedouroTipos, isFiltrarAtivo: true, itemTextoPadrao: true, selecionado: Caracterizacao.construidaConstruir.vazaoMaxTipo.ToString());
			profissoesLst.Add(ViewModelHelper.CriarSelectList(profissoes, selecionado: Caracterizacao.responsaveisTecnicos[0].profissao.Id.ToString()));
			profissoesLst.Add(ViewModelHelper.CriarSelectList(profissoes, selecionado: Caracterizacao.responsaveisTecnicos[1].profissao.Id.ToString()));
			profissoesLst.Add(ViewModelHelper.CriarSelectList(profissoes, selecionado: Caracterizacao.responsaveisTecnicos[2].profissao.Id.ToString()));
			profissoesLst.Add(ViewModelHelper.CriarSelectList(profissoes, selecionado: Caracterizacao.responsaveisTecnicos[3].profissao.Id.ToString()));
			profissoesLst.Add(ViewModelHelper.CriarSelectList(profissoes, selecionado: Caracterizacao.responsaveisTecnicos[4].profissao.Id.ToString()));
			profissoesLst.Add(ViewModelHelper.CriarSelectList(profissoes, selecionado: Caracterizacao.responsaveisTecnicos[5].profissao.Id.ToString()));
		}

		public string AutorizacaoJson
        {
            get
            {
                if (Caracterizacao.Autorizacao == null || Caracterizacao.Autorizacao.Id <= 0)
                {
                    return "";
                }

                return ViewModelHelper.Json(new
                {
                    @Id = Caracterizacao.Autorizacao.Id,
                    @Raiz = Caracterizacao.Autorizacao.Raiz,
                    @Nome = Caracterizacao.Autorizacao.Nome,
                    @Extensao = Caracterizacao.Autorizacao.Extensao,
                    @Caminho = Caracterizacao.Autorizacao.Caminho,
                    @Diretorio = Caracterizacao.Autorizacao.Diretorio,
                    @TemporarioPathNome = Caracterizacao.Autorizacao.TemporarioPathNome,
                    @ContentType = Caracterizacao.Autorizacao.ContentType,
                    @ContentLength = Caracterizacao.Autorizacao.ContentLength,
                    @Tid = Caracterizacao.Autorizacao.Tid,
                    @Apagar = Caracterizacao.Autorizacao.Apagar,
                    @Conteudo = Caracterizacao.Autorizacao.Conteudo,
                    @DiretorioConfiguracao = Caracterizacao.Autorizacao.DiretorioConfiguracao,
                    @TemporarioNome = Caracterizacao.Autorizacao.TemporarioNome
                });
            }
        }
	}
}