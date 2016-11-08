using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMProjetoGeografico
{
	public class ProjetoGeograficoVM
	{
		public ProjetoGeograficoVM()
		{	
		}

		public int SituacaoId { get; set; }

		public bool IsVisualizar { get; set; }
		public bool IsEditar { get { return Projeto != null && Projeto.Id > 0; } }
		public bool IsFinalizado { get { return SituacaoId == (int)eFiscalizacaoProjetoGeoSituacao.Finalizado; } }
		public bool IsImportadorShape { get { return Projeto != null && Projeto.MecanismoElaboracaoId == (int)eProjetoGeograficoMecanismo.ImportadorShapes; } }
		public bool IsDesenhador { get { return Projeto != null && Projeto.MecanismoElaboracaoId == (int)eProjetoGeograficoMecanismo.Desenhador; } }

		public String MenorX { get { return (Projeto == null || Projeto.MenorX == 0) ? String.Empty : ((int)Projeto.MenorX).ToString(); } }
		public String MenorY { get { return (Projeto == null || Projeto.MenorY == 0) ? String.Empty : ((int)Projeto.MenorY).ToString(); } }
		public String MaiorX { get { return (Projeto == null || Projeto.MaiorX == 0) ? String.Empty : ((int)Projeto.MaiorX).ToString(); } }
		public String MaiorY { get { return (Projeto == null || Projeto.MaiorY == 0) ? String.Empty : ((int)Projeto.MaiorY).ToString(); } }

		public int TipoMecanismo { get; set; }
		
		public int ArquivoEnviadoTipo { get; set; }
		public int BaseReferenciaDelay { get; set; }
		public string UrlAvancar { get; set; }
		public string TextoMerge { get; set; }
		public string AtualizarDependenciasModalTitulo { get; set; }
		public bool isCadastrarCaracterizacao { get; set; }
		public string UrlBaixarOrtofoto { get; set; }
		public string UrlValidarOrtofoto { get; set; }

		private int _delayProcessamento = 1;
		public int DelayRequisicao
		{
			get { return _delayProcessamento; }
			set { _delayProcessamento = value; }
		}

		private ProjetoGeografico _projeto = new ProjetoGeografico();
		public ProjetoGeografico Projeto
		{
			get { return _projeto; }
			set { _projeto = value; }
		}

		private List<SelectListItem> _sistemasCoordenada = new List<SelectListItem>();
		public List<SelectListItem> SistemaCoordenada
		{
			get { return _sistemasCoordenada; }
			set { _sistemasCoordenada = value; }
		}

		private List<SelectListItem> _niveisPrecisao = new List<SelectListItem>();
		public List<SelectListItem> NiveisPrecisao
		{
			get { return _niveisPrecisao; }
			set { _niveisPrecisao = value; }
		}

		private EnviarProjetoVM _enviarProjeto = new EnviarProjetoVM();
		public EnviarProjetoVM EnviarProjeto
		{
			get { return _enviarProjeto; }
			set { _enviarProjeto = value; }
		}

		private DesenhadorVM _desenhador = new DesenhadorVM();
		public DesenhadorVM Desenhador
		{
			get { return _desenhador; }
			set { _desenhador = value; }
		}

		private BaseReferenciaVM _baseReferencia = new BaseReferenciaVM();
		public BaseReferenciaVM BaseReferencia
		{
			get { return _baseReferencia; }
			set { _baseReferencia = value; }
		}

		public List<ArquivoProcessamentoVM> ArquivosProcessados
		{
			get
			{
				if (EnviarProjeto != null && EnviarProjeto.ArquivosProcessados.Count > 0)
				{
					return EnviarProjeto.ArquivosProcessados;
				}

				if (Desenhador != null && Desenhador.ArquivosProcessados.Count > 0)
				{
					return Desenhador.ArquivosProcessados;
				}

				return new List<ArquivoProcessamentoVM>();
			}
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					NivelPrecisaoObrigatorio = Mensagem.ProjetoGeografico.NivelPrecisaoObrigatorio,
					AreaAbrangenciaObrigatorio = Mensagem.ProjetoGeografico.AreaDeAbrangenciaObrigatorio,
					MecanismoObrigatorio = Mensagem.ProjetoGeografico.MecanismoObrigatorio,
					ConfirmarExcluir = Mensagem.ProjetoGeografico.ConfirmarExcluir,
					ConfirmarAreaAbrangencia = Mensagem.ProjetoGeografico.ConfirmarAreaAbrangencia,
					//ConfirmacaoFinalizar = Mensagem.ProjetoGeografico.ConfirmacaoFinalizar(Dependentes.Select(x => x.Nome).ToList(), Projeto.SituacaoId == (int)eProjetoGeograficoSituacao.EmElaboracao || Dependentes.Count <= 0),
					ConfirmacaoRecarregar = Mensagem.ProjetoGeografico.ConfirmacaoRecarregar(),
					ConfirmacaoRefazer = Mensagem.ProjetoGeografico.ConfirmacaoRefazer(),
					ConfirmacaoReenviar = Mensagem.ProjetoGeografico.ConfirmacaoReenviar,
					EmpreendimentoForaAbrangencia = Mensagem.ProjetoGeografico.EmpreendimentoForaAbrangencia
				});
			}
		}

		public String MensagensDesenhador
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					CnpjJaExistente = Mensagem.Empreendimento.CnpjJaExistente
				});
			}
		}

		public String MensagensImportador
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					ArquivoAnexoNaoEhZip = Mensagem.ProjetoGeografico.ArquivoAnexoNaoEhZip
				});
			}
		}

		public String SituacoesValidasJson
		{
			get { return ViewModelHelper.Json(new List<int>() { 1, 2, 6, 7, 11, 12 }); }
		}

		public void CarregarVMs()
		{
			EnviarProjeto.SituacaoProjeto = Projeto.SituacaoId;
			Desenhador.SituacaoProjeto = Projeto.SituacaoId;
			BaseReferencia.SituacaoProjeto = Projeto.SituacaoId;

			NiveisPrecisao.ForEach(x => x.Selected = false);
			NiveisPrecisao.SingleOrDefault(x => x.Value == Projeto.NivelPrecisaoId.ToString()).Selected = true;


			#region Base de referência

			List<ArquivoProjeto> arquivos = Projeto.Arquivos.Where(x => x.Tipo == (int)eProjetoGeograficoArquivoTipo.DadosIDAF || x.Tipo == (int)eProjetoGeograficoArquivoTipo.DadosGEOBASES).ToList();
			ArquivoProcessamentoVM arqProcessamento = new ArquivoProcessamentoVM();

			if (arquivos != null && arquivos.Count > 0)
			{
				foreach (ArquivoProjeto item in arquivos)
				{
					for (int i = 0; i < BaseReferencia.ArquivosVetoriais.Count; i++)
					{
						if (BaseReferencia.ArquivosVetoriais[i].Tipo == item.Tipo)
						{
							BaseReferencia.ArquivosVetoriais.RemoveAt(i);
							break;
						}
					}

					if (item.Situacao <= 0)
					{
						item.Situacao = 9;
						item.SituacaoTexto = "Processado";
					}

					arqProcessamento = new ArquivoProcessamentoVM(item);
					BaseReferencia.ArquivosVetoriais.Add(arqProcessamento);
				}
			}

			#endregion

			#region Ortofotos

			if (Projeto.ArquivosOrtofotos.Count > 0)
			{
				foreach (ArquivoProjeto item in Projeto.ArquivosOrtofotos)
				{
					BaseReferencia.ArquivosOrtoFotoMosaico.Add(new ArquivoProcessamentoVM(item));
				}
			}

			#endregion

			#region Arquivos da Fiscalizacao

			if (Projeto.ArquivosDominio.Count > 0)
			{
				foreach (ArquivoProjeto item in Projeto.ArquivosDominio)
				{
					ArquivoProcessamentoVM arquivo = new ArquivoProcessamentoVM(item);
					if (item.Tipo == (int)eProjetoGeograficoArquivoTipo.Croqui)
					{
						arquivo.ArquivoEnviadoTipo = 3;
					}
					BaseReferencia.DadosDominio.Add(arquivo);
				}
			}

			#endregion

			#region Arquivos processados desse projeto

			arquivos = Projeto.Arquivos.Where(x =>
				x.Tipo != (int)eProjetoGeograficoArquivoTipo.DadosIDAF).ToList();

			if (arquivos != null && arquivos.Count > 0)
			{
				foreach (ArquivoProjeto item in arquivos)
				{
					if (item.Tipo == (int)eProjetoGeograficoArquivoTipo.ArquivoEnviado && item.Situacao <= 0)
					{
						ArquivoProjeto arquivoAux = arquivos.FirstOrDefault(x => x.Tipo == 4);
						if (arquivoAux != null)
						{
							item.Situacao = arquivoAux.Situacao;
							item.SituacaoTexto = arquivoAux.SituacaoTexto;
						}
					}

					arqProcessamento = new ArquivoProcessamentoVM(item, ArquivoEnviadoTipo, (eProjetoGeograficoSituacao)Projeto.SituacaoId);

					if (Projeto.MecanismoElaboracaoId == 1)
					{

						EnviarProjeto.ArquivosProcessados.Add(arqProcessamento);
					}
					else
					{
						Desenhador.ArquivosProcessados.Add(arqProcessamento);
					}
				}
			}

			#endregion

			Desenhador.ArquivoEnviado = new ArquivoProcessamentoVM(Projeto.ArquivoEnviadoDesenhador);
		}
	}
}