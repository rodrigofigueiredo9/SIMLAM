﻿using System.Collections.Generic;
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
		public bool IsVisualizar { get; set; }
        public int ProjetoDigitalId { get; set; }
        public int ProtocoloId { get; set; }
        public int RequerimentoId { get; set; }
        public string UrlRetorno { get; set; }
        
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

		public BarragemDispensaLicencaVM(BarragemDispensaLicenca entidade, Atividade atividade, List<Lista> finalidades, List<Lista> formacoesRT, List<Lista> barragemTipos, List<Lista> fases, List<Lista> mongeTipos, List<Lista> vertedouroTipos)
		{
			Caracterizacao = entidade ?? new BarragemDispensaLicenca();

			List<Lista> atividades  = new List<Lista>();
			atividades.Add(new Lista() { Id = atividade.Id.ToString(), Texto = atividade.NomeAtividade });
			Atividades = ViewModelHelper.CriarSelectList(atividades, isFiltrarAtivo: false, itemTextoPadrao: false);

			FinalidadesAtividade = finalidades;
			FormacoesRTLst = formacoesRT;
			BarragemTiposLst = barragemTipos;
			FasesLst = fases;
			MongeTiposLst = ViewModelHelper.CriarSelectList(mongeTipos, isFiltrarAtivo: true, itemTextoPadrao: true, selecionado: Caracterizacao.MongeTipo.ToString());
			VertedouroTiposLst = ViewModelHelper.CriarSelectList(vertedouroTipos, isFiltrarAtivo: true, itemTextoPadrao: true, selecionado: Caracterizacao.VertedouroTipo.ToString());
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