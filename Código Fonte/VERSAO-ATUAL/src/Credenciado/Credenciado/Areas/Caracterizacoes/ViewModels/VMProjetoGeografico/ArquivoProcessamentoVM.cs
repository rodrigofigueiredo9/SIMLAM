using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Entities.Etx.ModuloGeo;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico;

namespace Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels.VMProjetoGeografico
{
	public class ArquivoProcessamentoVM
	{
		private ArquivoProjeto _arquivo = new ArquivoProjeto();
		public ArquivoProjeto ArquivoProcessamento
		{
			get { return _arquivo; }
			set { _arquivo = value; }
		}

		public int ArquivoEnviadoTipo { get; set; }
		public int ArquivoEnviadoFilaTipo { get; set; }
		public Int32 Id { get { return ArquivoProcessamento.Id.GetValueOrDefault(); } set { ArquivoProcessamento.Id = value; } }
		public Int32 IdRelacionamento { get { return ArquivoProcessamento.IdRelacionamento; } set { ArquivoProcessamento.IdRelacionamento = value; } }
		public Int32 ProjetoId { get { return ArquivoProcessamento.ProjetoId; } set { ArquivoProcessamento.ProjetoId = value; } }
		public Int32 Tipo { get { return ArquivoProcessamento.Tipo; } set { ArquivoProcessamento.Tipo = value; } }
		public Int32 FilaTipo { get { return ArquivoProcessamento.Processamento.FilaTipo; } set { ArquivoProcessamento.Processamento.FilaTipo = value; } }

		public Int32 ProcessamentoId { get { return ArquivoProcessamento.Processamento.Id; } set { ArquivoProcessamento.Processamento.Id = value; } }
		public Int32 Etapa { get { return ArquivoProcessamento.Processamento.Etapa; } set { ArquivoProcessamento.Processamento.Etapa = value; } }
		public Int32 Situacao { get { return ArquivoProcessamento.Processamento.Situacao; } set { ArquivoProcessamento.Processamento.Situacao = value; } }
		public String SituacaoTexto { get { return ArquivoProcessamento.Processamento.SituacaoTexto; } set { ArquivoProcessamento.Processamento.SituacaoTexto = value; } }
		public String Caminho { get { return ArquivoProcessamento.Caminho; } set { ArquivoProcessamento.Caminho = value; } }
		public String Chave { get { return ArquivoProcessamento.Chave; } set { ArquivoProcessamento.Chave = value; } }
		public String ChaveData { get { return ArquivoProcessamento.ChaveData.ToString(); } set { ArquivoProcessamento.Chave = value; } }
		public bool IsPDF { get { return ArquivoProcessamento.Tipo == 7; } }
		public Int32 Mecanismo { get { return ArquivoProcessamento.Processamento.Mecanismo; } set { ArquivoProcessamento.Processamento.Mecanismo = value; } }
		public Int32 EmpreendimentoId { get; set; }

		public String Texto
		{
			set { ArquivoProcessamento.Nome = value; }

			get
			{
				if (Tipo == (int)eProjetoGeograficoArquivoTipo.Croqui)
				{
					if (ArquivoEnviadoFilaTipo == (int)eFilaTipoGeo.Dominialidade)
					{
						return ArquivoProcessamento.Nome + " da dominialidade (PDF)";
					}
					else
					{
						if (ArquivoEnviadoFilaTipo == (int)eFilaTipoGeo.CAR)
						{
							return ArquivoProcessamento.Nome + " do CAR (PDF)";
						}

						return ArquivoProcessamento.Nome + " da atividade (PDF)";

					}
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

		public ArquivoProcessamentoVM(ArquivoProjeto arquivo)
		{
			this.ArquivoProcessamento = arquivo;
		}

		public ArquivoProcessamentoVM(ArquivoProjeto arquivo, int ArquivoEnviadoTipo, int ArquivoEnviadoFilaTipo, eProjetoGeograficoSituacao situacao = eProjetoGeograficoSituacao.EmElaboracao)
		{
			this.ArquivoEnviadoTipo = ArquivoEnviadoTipo;
			this.ArquivoEnviadoFilaTipo = ArquivoEnviadoFilaTipo;

			this.ArquivoProcessamento = arquivo;

			if (situacao != eProjetoGeograficoSituacao.Finalizado)
			{
				this.MostrarReenviar = true;
				this.MostrarReprocessar = true;
				this.MostrarGerar = true;
			}
		}

		public ArquivoProcessamentoVM(ArquivoProjeto arquivo, eProjetoGeograficoSituacao situacao = eProjetoGeograficoSituacao.EmElaboracao)
		{
			if (situacao != eProjetoGeograficoSituacao.Finalizado)
			{
				this.MostrarReenviar = true;
				this.MostrarReprocessar = true;
				this.MostrarGerar = true;
			}
		}

		public ArquivoProcessamentoVM() { }

	}
}
