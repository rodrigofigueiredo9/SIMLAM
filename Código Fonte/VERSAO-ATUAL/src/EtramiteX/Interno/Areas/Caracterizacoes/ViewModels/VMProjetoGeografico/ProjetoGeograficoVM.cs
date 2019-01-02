using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Etx.ModuloGeo;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels.VMProjetoGeografico
{
	public class ProjetoGeograficoVM
	{
		public int Id { get; set; }
		public int EmpreendimentoEasting { get; set; }
		public int EmpreendimentoNorthing { get; set; }
		public int TipoMecanismo { get; set; }
		public int EmpreendimentoId { get; set; }
		public int CaracterizacaoTipo { get; set; }

		public int ArquivoEnviadoTipo { get; set; }
		public int ArquivoEnviadoFilaTipo { get; set; }
		public int BaseReferenciaDelay { get; set; }
		public string UrlAvancar { get; set; }
        public string UrlVoltar { get; set; }
		public string TextoMerge { get; set; }
		public string AtualizarDependenciasModalTitulo { get; set; }
		public bool isCadastrarCaracterizacao { get; set; }
		public string UrlBaixarOrtofoto { get; set; }
		public string UrlValidarOrtofoto { get; set; }

		public bool PossuiAPPNaoCaracterizada { get; set; }
		public bool PossuiARLNaoCaracterizada { get; set; }

		public bool IsVisualizar { get; set; }
		public bool IsVisualizarCredenciado { get; set; }
		public bool IsEditar { get { return Projeto != null && Projeto.Id > 0; } }
		public bool IsImportadorShape { get { return Projeto != null && Projeto.MecanismoElaboracaoId == (int)eProjetoGeograficoMecanismo.ImportadorShapes; } }
		public bool IsDesenhador { get { return Projeto != null && Projeto.MecanismoElaboracaoId == (int)eProjetoGeograficoMecanismo.Desenhador; } }
		public bool IsProcessado { get; set; }
		public bool IsFinalizado
		{
			get
			{
				if (Projeto == null)
				{
					return false;
				}
				return Projeto.SituacaoId == (int)eProjetoGeograficoSituacao.Finalizado;
			}
		}

		public bool IsDominialidade { get { return Projeto != null && Projeto.CaracterizacaoId == (int)eCaracterizacao.Dominialidade || Projeto.CaracterizacaoId == (int)eCaracterizacao.RegularizacaoFundiaria ; } }
		public bool IsRegularizacaoFundiaria { get { return Projeto != null && Projeto.CaracterizacaoId == (int)eCaracterizacao.RegularizacaoFundiaria; } }

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

		private List<Caracterizacao> _dependentes = new List<Caracterizacao>();
		public List<Caracterizacao> Dependentes
		{
			get { return _dependentes; }
			set { _dependentes = value; }
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

		private SobreposicoesVM _sobreposicoes = new SobreposicoesVM();
		public SobreposicoesVM Sobreposicoes
		{
			get { return _sobreposicoes; }
			set { _sobreposicoes = value; }
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
					ConfirmacaoFinalizar = Mensagem.ProjetoGeografico.ConfirmacaoFinalizar(Dependentes.Select(x => x.Nome).ToList(), Projeto.SituacaoId == (int)eProjetoGeograficoSituacao.EmElaboracao || Dependentes.Count <= 0),
					ConfirmacaoRecarregar = Mensagem.ProjetoGeografico.ConfirmacaoRecarregar(),
					ConfirmacaoRefazer = Mensagem.ProjetoGeografico.ConfirmacaoRefazer(),
					ConfirmacaoAtualizar = Mensagem.ProjetoGeografico.ConfirmacaoAtualizar(),
					ConfirmacaoReenviar = Mensagem.ProjetoGeografico.ConfirmacaoReenviar,
					EmpreendimentoForaAbrangencia = Mensagem.ProjetoGeografico.EmpreendimentoForaAbrangencia,
					AlterarMecanismo = Mensagem.ProjetoGeografico.AlterarMecanismo
				});
			}
		}

		public String UrlsArquivo { get; set; }

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

		public ProjetoGeograficoVM() { }

		public void CarregarVMs()
		{
			EnviarProjeto.SituacaoProjeto = Projeto.SituacaoId;
			Desenhador.SituacaoProjeto = Projeto.SituacaoId;
			BaseReferencia.SituacaoProjeto = Projeto.SituacaoId;

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

			#region Arquivos da Dominialidade

			if (Projeto.ArquivosDominio.Count > 0)
			{
				foreach (ArquivoProjeto item in Projeto.ArquivosDominio)
				{
					ArquivoProcessamentoVM arquivo = new ArquivoProcessamentoVM(item);
					if (item.Tipo == (int)eProjetoGeograficoArquivoTipo.Croqui)
					{
						arquivo.ArquivoEnviadoTipo = (int)eProjetoGeograficoArquivoTipo.ArquivoEnviado;
						arquivo.ArquivoEnviadoFilaTipo = (int)((Projeto.CaracterizacaoId == (int)eCaracterizacao.Dominialidade) ? eFilaTipoGeo.Dominialidade : eFilaTipoGeo.Atividade);
					}
					BaseReferencia.DadosDominio.Add(arquivo);
				}
			}

			#endregion

			#region Arquivos processados desse projeto

			arquivos = Projeto.Arquivos.Where(x =>
				x.Tipo != (int)eProjetoGeograficoArquivoTipo.DadosIDAF &&
				x.Tipo != (int)eProjetoGeograficoArquivoTipo.DadosGEOBASES &&
				x.Tipo != (int)eProjetoGeograficoArquivoTipo.CroquiFinal).ToList();

			if (arquivos != null && arquivos.Count > 0)
			{
				foreach (ArquivoProjeto item in arquivos)
				{
					if (item.Tipo == (int)eProjetoGeograficoArquivoTipo.ArquivoEnviado && item.Processamento.Situacao <= 0)
					{
						ArquivoProjeto arquivoAux = arquivos.FirstOrDefault(x => x.Tipo == (int)eProjetoGeograficoArquivoTipo.RelatorioImportacao);
						if (arquivoAux != null && arquivoAux.Id > 0)
						{
							item.Processamento.Id = arquivoAux.Processamento.Id;
							item.Processamento.FilaTipo = arquivoAux.Processamento.FilaTipo;
							item.Processamento.Etapa = arquivoAux.Processamento.Etapa;
							item.Processamento.Situacao = arquivoAux.Processamento.Situacao;
							item.Processamento.SituacaoTexto = arquivoAux.Processamento.SituacaoTexto;
						}
					}

					arqProcessamento = new ArquivoProcessamentoVM(item, ArquivoEnviadoTipo, ArquivoEnviadoFilaTipo, (eProjetoGeograficoSituacao)Projeto.SituacaoId);

					if (Projeto.MecanismoElaboracaoId == (int)eProjetoGeograficoMecanismo.ImportadorShapes)
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

			Sobreposicoes = new SobreposicoesVM(Projeto.Sobreposicoes, (!IsFinalizado && !IsVisualizar));
		}

		//Prop da Analise
		private bool _isCredenciado = true;
		public bool IsCredenciado
		{
			get { return _isCredenciado; }
			set { _isCredenciado = value; }
		}

		public int ProtocoloId { get; set; }
		public int RequerimentoId { get; set; }
	}
}