using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
using Tecnomapas.Blocos.Entities.Etx.ModuloArquivo;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;

namespace Tecnomapas.EtramiteX.Credenciado.ViewModels.VMProjetoDigital
{
	public class DocumentosGeradosVM
	{
		public Int32 ProjetoDigitalID { get; set; }
		public Boolean MostrarTitulo { get; set; }

		public List<DocumentoGerado> DocumentosGerados { get; set; }
		public List<DocumentoGerado> RoteirosOrientativos { get; set; }

		public String TiposPDF
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					Roteiro = (int)eProjetoDigitalDocGeradosTipo.Roteiro,
					Requerimento = (int)eProjetoDigitalDocGeradosTipo.Requerimento,
					RelatorioTecnico = (int)eProjetoDigitalDocGeradosTipo.RelatorioCaracterizacao,
                    FichaInscricaoUnidadeProducao = (int)eProjetoDigitalDocGeradosTipo.FichaInscricaoUnidadeProducao,
                    FichaInscricaoUnidadeConsolidacao = (int)eProjetoDigitalDocGeradosTipo.FichaInscricaoUnidadeConsolidacao

				});
			}
		}

		public DocumentosGeradosVM()
		{
			DocumentosGerados = new List<DocumentoGerado>();
			RoteirosOrientativos = new List<DocumentoGerado>();
		}

		public void CarregarDocumentos(Requerimento requerimento, ProjetoDigital projetoDigital)
		{
			DocumentoGerado documento;
			if (projetoDigital != null)
			{
				ProjetoDigitalID = projetoDigital.Id;
			}

			foreach (var item in requerimento.Roteiros)
			{
				documento = new DocumentoGerado()
				{
					Id = item.Id,
					Texto = item.Nome+" - Nº: " + item.Numero,
					Tipo = (int)eProjetoDigitalDocGeradosTipo.Roteiro
				};

				RoteirosOrientativos.Add(documento);
			}

			documento = new DocumentoGerado()
			{
				Id = requerimento.Id,
				Texto = "Requerimento Digital",
				Tipo = (int)eProjetoDigitalDocGeradosTipo.Requerimento
			};
			DocumentosGerados.Add(documento);

            foreach (var item in projetoDigital.Dependencias.Where(x => x.DependenciaTipo == (int)eCaracterizacaoDependenciaTipo.Caracterizacao && x.DependenciaCaracterizacao != (int)eCaracterizacao.BarragemDispensaLicenca))
			{
				documento = new DocumentoGerado()
				{
					Id = item.DependenciaCaracterizacao,
					Texto = "Relatório de Caracterização - " + item.DependenciaCaracterizacaoTexto,
					Tipo = (int)eProjetoDigitalDocGeradosTipo.RelatorioCaracterizacao
				};
				var anexo = projetoDigital.Dependencias.SingleOrDefault(x => x.DependenciaTipo == (int)eCaracterizacaoDependenciaTipo.ProjetoGeografico
																			 && x.DependenciaCaracterizacao == item.DependenciaCaracterizacao) ?? new Dependencia();
				if (anexo.Id > 0)
				{
					documento.Anexos.Add(new Lista()
				   {
					   Id = anexo.Id.ToString(),
					   Texto = "Croqui - " + anexo.DependenciaCaracterizacaoTexto,
					   Tipo = (int)eProjetoDigitalDocGeradosTipo.Croqui
				   });
				}
                
				DocumentosGerados.Add(documento);
			}

            //Fichas de inscrição

            #region Unidade de Produção

            var unidade = projetoDigital.Dependencias
                            .SingleOrDefault(x => x.DependenciaTipo == (int)eCaracterizacaoDependenciaTipo.Caracterizacao
                                                 && x.DependenciaCaracterizacao == (int)eCaracterizacao.UnidadeProducao);

            if (unidade != null && unidade.DependenciaId > 0)
            {
                documento = new DocumentoGerado() { Id = unidade.DependenciaId, Texto = String.Format("Ficha de Inscrição - {0}", unidade.DependenciaCaracterizacaoTexto), Tipo = (int)eProjetoDigitalDocGeradosTipo.FichaInscricaoUnidadeProducao };
                DocumentosGerados.Add(documento);
            }
            
            #endregion

            #region Unidade de Consolidação

            unidade = projetoDigital.Dependencias
                            .SingleOrDefault(x => x.DependenciaTipo == (int)eCaracterizacaoDependenciaTipo.Caracterizacao
                                                 && x.DependenciaCaracterizacao == (int)eCaracterizacao.UnidadeConsolidacao);

            if (unidade != null && unidade.DependenciaId > 0)
            {
                documento = new DocumentoGerado() { Id = unidade.DependenciaId, Texto = String.Format("Ficha de Inscrição - {0}", unidade.DependenciaCaracterizacaoTexto), Tipo = (int)eProjetoDigitalDocGeradosTipo.FichaInscricaoUnidadeConsolidacao };
                DocumentosGerados.Add(documento);
            }
            
            #endregion
		}

		public String ObterJSon(DocumentoGerado item)
		{
			return HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(new { Id = item.Id, Tipo = item.Tipo }));
		}
	}
}