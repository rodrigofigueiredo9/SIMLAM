using System;
using Tecnomapas.EtramiteX.Publico.Model.ModuloGeoProcessamento.Entities;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels.VMSimuladorGeo
{
	public class ArquivoItemGridVM
	{
		private SimuladorGeoArquivo _arquivo = new SimuladorGeoArquivo();
		public SimuladorGeoArquivo ArquivoProcessamento
		{
			get { return _arquivo; }
			set { _arquivo = value; }
		}

		private SimuladorGeoArquivo _arquivoEnviado = new SimuladorGeoArquivo();
		public SimuladorGeoArquivo ArquivoEnviado
		{
			get { return _arquivoEnviado; }
			set { _arquivoEnviado = value; }
		}

		public int ArquivoEnviadoTipo { get; set; }
		public Int32 Id { get { return ArquivoProcessamento.Id.GetValueOrDefault(); } set { ArquivoProcessamento.Id = value; } }
		public Int32 IdRelacionamento { get { return ArquivoProcessamento.IdRelacionamento; } set { ArquivoProcessamento.IdRelacionamento = value; } }
		public Int32 ProjetoId { get { return ArquivoProcessamento.ProjetoId; } set { ArquivoProcessamento.ProjetoId = value; } }
		public Int32 Tipo { get { return ArquivoProcessamento.Tipo; } set { ArquivoProcessamento.Tipo = value; } }
		
		public Int32 Etapa { get { return ArquivoProcessamento.Etapa; } set { ArquivoProcessamento.Etapa = value; } }
		public Int32 Situacao { get { return ArquivoProcessamento.Situacao; } set { ArquivoProcessamento.Situacao = value; } }
		public String SituacaoTexto { get { return ArquivoProcessamento.SituacaoTexto; } set { ArquivoProcessamento.SituacaoTexto = value; } }
		public String Caminho { get { return ArquivoProcessamento.Caminho; } set { ArquivoProcessamento.Caminho = value; } }
		public String Chave { get { return ArquivoProcessamento.Chave; } set { ArquivoProcessamento.Chave = value; } }
		public bool IsPDF { get { return ArquivoProcessamento.Tipo == 7; } }
		public Int32 Mecanismo { get { return ArquivoProcessamento.Mecanismo; } set { ArquivoProcessamento.Mecanismo = value; } }
		public Int32 EmpreendimentoId { get; set; }
		
		public String Texto
		{
			set { ArquivoProcessamento.Nome = value; }

			get
			{
				if (Tipo == (int)eSimuladorGeoArquivoTipo.Croqui)
				{
					/*if (ArquivoEnviadoTipo == (int)eSimuladorGeoArquivoTipo.ArquivoEnviado)
					{
						return ArquivoProcessamento.Nome + " da dominialidade (PDF)";
					}
					else
					{
						return ArquivoProcessamento.Nome + " da atividade (PDF)";
					}*/
					return ArquivoProcessamento.Nome + " da dominialidade (PDF)";
				}
				return ArquivoProcessamento.Nome;
			}
		}

		public bool MostrarGerar { get; set; }
		public bool MostrarBaixar { get; set; }
		public bool MostrarRegerar { get; set; }

		public bool MostrarReenviar { get; set; }
		public bool MostrarReprocessar { get; set; }
		public bool MostrarCancelar { get; set; }

		public ArquivoItemGridVM(SimuladorGeoArquivo arquivo)
		{
		    this.ArquivoProcessamento = arquivo;
		    this.RegraBotoesGridVetoriais();
		}

		public ArquivoItemGridVM(SimuladorGeoArquivo arquivo, int ArquivoEnviadoTipo, eSimuladorGeoSituacao situacao = eSimuladorGeoSituacao.EmElaboracao) 
		{
		    this.ArquivoEnviadoTipo = ArquivoEnviadoTipo;
		    this.ArquivoProcessamento = arquivo;
		    this.RegraBotoesGridVetoriais();

			if (situacao != eSimuladorGeoSituacao.Finalizado)
		    {
		        this.MostrarReenviar = true;
		        this.MostrarReprocessar = true;
		        this.MostrarGerar = true;
		    }
		}

		public ArquivoItemGridVM(SimuladorGeoArquivo arquivo, eSimuladorGeoSituacao situacao = eSimuladorGeoSituacao.EmElaboracao)
		{
			if (situacao != eSimuladorGeoSituacao.Finalizado)
		    {
		        this.MostrarReenviar = true;
		        this.MostrarReprocessar = true;
		        this.MostrarGerar = true;
		    }
		}

		public ArquivoItemGridVM() { }

		public void RegraBotoesGridVetoriais()
		{
			/*
				 Aguardando a validação 1
				Executanto a validação 2
				Erro na validação 3
				Reprovado 4
				Cancelada 5
				Aguardando processamento 6
				Processando 7
				Erro no processamento 8
				Processado 9
				Cancelado 10
				Aguardando geração do PDF 11
				Gerando o PDF 12
				Erro ao gerar o PDF 13
				Processado 14
				Cancelada 15
				 14; 4, Falha na validação 3, falha no processamento 8 ou Falha na geração do PDF 13. 
				 */

			switch (Situacao)
			{
				case 1:
				case 6:
				case 11:
					ConfigurarBotoes(cancelar: true);
					break;

				case 2:
				case 7:
				case 12:
					ConfigurarBotoes();
					break;

				case 8:
					ConfigurarBotoes(reenviar: true, reprocessar: true, regerar: true);
					break;

				case 3:
				case 4:
				case 5:
				case 9:
				case 10:
				case 13:
				case 15:
					ConfigurarBotoes(baixar: true, reenviar: true, reprocessar: true, regerar: true);
					break;

				case 14:
					ConfigurarBotoes(baixar: true, reenviar: true, regerar: true);
					break;
			}
		}

		public void ConfigurarBotoes(bool gerar = false, bool baixar = false, bool regerar = false, bool reenviar = false, bool reprocessar = false, bool cancelar = false)
		{
			this.MostrarGerar = gerar;
			this.MostrarBaixar = baixar;
			this.MostrarRegerar = regerar;
			this.MostrarReenviar = reenviar;
			this.MostrarReprocessar = reprocessar;
			this.MostrarCancelar = cancelar;
		}
	}
}