using System;
using System.Collections;
using System.Collections.Generic;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Arquivo.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.EtramiteX.WindowsService.ProcessOperacoesGeo.Data;
using Tecnomapas.EtramiteX.WindowsService.Utilitarios;

namespace Tecnomapas.EtramiteX.WindowsService.ProcessOperacoesGeo
{
	public class ProjetoBus
	{
		private BancoDeDados _banco;
		private OperacoesGeoDa _dataAccess;

		public ProjetoBus(BancoDeDados banco)
		{
			_banco = banco;
			_dataAccess = new OperacoesGeoDa(banco);
		}

		#region Arquivo

		private GerenciadorArquivo _gerenciadorArquivo;
		private ArquivoDa _arquivoDataAccess;

		private ArquivoDa arquivoDataAccess
		{
			get
			{
				if (_arquivoDataAccess == null)
				{
					_arquivoDataAccess = new ArquivoDa(_banco.Conexao);
				}

				return _arquivoDataAccess;
			}
		}

		private GerenciadorArquivo gerenciadorArquivo
		{
			get
			{
				if (_gerenciadorArquivo == null)
				{
					_gerenciadorArquivo = new GerenciadorArquivo(_dataAccess.DiretorioArquivo, _banco.Conexao);
				}

				return _gerenciadorArquivo;
			}
		}

		private void SalvarArquivo(Arquivo arquivo)
		{
			if (arquivo != null)
			{
				arquivo.Nome = System.IO.Path.GetFileName(arquivo.Nome);
				gerenciadorArquivo.Salvar(arquivo, _dataAccess.DiretorioArquivo, arquivoDataAccess.ObterSeparadorQtd().ToString());

				string oldFileUrl = ((arquivo.Id ?? 0) <= 0) ? "" : arquivoDataAccess.ObterCaminho(arquivo.Id ?? 0, _banco);

				arquivoDataAccess.Salvar(arquivo, null, "Serviço de Processamento de Geometrias", "SVCOperacoesGeo", null, GerenciadorTransacao.ObterIDAtual(), _banco);

				if (!String.IsNullOrWhiteSpace(oldFileUrl))
				{
					try
					{
						gerenciadorArquivo.Deletar(oldFileUrl);
					}
					catch
					{
						arquivoDataAccess.MarcarDeletado((arquivo.Id ?? 0), oldFileUrl, _banco);
					}
				}
			}
		}

		public void SalvarArquivo(Arquivo arquivo, int projeto, int tipoProjeto, int tipoArquivo)
		{
			_banco.IniciarTransacao();
			string[] fileNames = ",BaseDeReferencia.zip,BaseDeReferenciaGEOBASES.zip,ArquivoEnviado.zip,RelatorioImportacao.pdf,ArquivoProcessado.zip,ArquivoProcessadoTrackMaker.zip,PdfDoMapa.pdf,PecaTecnica.pdf".Split(',');

			arquivo.Nome = System.IO.Path.GetFileName(fileNames[tipoArquivo]);
			arquivo.Extensao = System.IO.Path.GetExtension(fileNames[tipoArquivo]);
			arquivo.TemporarioPathNome = string.Empty;
			arquivo.ContentLength = (int)arquivo.Buffer.Length;
			arquivo.ContentType = (arquivo.Extensao == ".zip") ? "application / octet - stream" : "application / pdf";

			SalvarArquivo(arquivo);

			_dataAccess.SetArquivoDisponivel(arquivo.Id ?? 0, projeto, tipoProjeto, tipoArquivo);
			_banco.Commit();
		}

		public string ObterCaminhoDoArquivoEnviado(int projeto)
		{
			List<int> ids = _dataAccess.BuscarIdsArquivo(projeto, 3);

			return (ids != null && ids.Count > 0) ? arquivoDataAccess.ObterCaminho(ids[1], _banco) : "";
		}

		public Arquivo ObterArquivo(int arquivoId)
		{
			return arquivoDataAccess.Obter(arquivoId, _banco);
		}

		public Arquivo ObterArquivo(int projeto, int tipoArquivo)
		{
			List<int> ids = _dataAccess.BuscarIdsArquivo(projeto, tipoArquivo);

			return (ids != null && ids.Count > 0) ? arquivoDataAccess.Obter(ids[1], _banco) : new Arquivo();
		}

		#endregion

		public int ObterMecanismoDeEnvio(int projeto, int tipo)
		{
			return _dataAccess.BuscarMecanismoDeEnvio(projeto, tipo);
		}

		public List<int> ObterEnvelope(int projeto)
		{
			return _dataAccess.BuscarEnvelope(projeto);
		}

		public Hashtable ObterConfiguracoesBaseRef()
		{
			return _dataAccess.ObterConfiguracoesBaseRef();
		}

		public void SetAguardandoEtapaNaFila(int projeto, int tipo, int etapa)
		{
			_banco.IniciarTransacao();
			if (!_dataAccess.VerificarCancelado(projeto, tipo))
			{
				_dataAccess.SetAguardandoEtapaNaFila(projeto, tipo, etapa);
			}
			_banco.Commit();
		}

		public void SetFalhaNaFila(int projeto, int tipo)
		{
			try
			{
				_dataAccess.SetFalhaNaFila(projeto, tipo);
			}
			catch (Exception exc)
			{
				Log.GerarLog(exc);
			}
		}

		public void SetConcluidoNaFila(int projeto, int tipo, int etapa)
		{
			_banco.IniciarTransacao();
			if (!_dataAccess.VerificarCancelado(projeto, tipo))
			{
				_dataAccess.SetConcluidoNaFila(projeto, tipo, etapa);
			}
			_banco.Commit();
		}

		public void SalvarLogOperacoes(int projeto, int tipo, int etapa, List<Hashtable> listLog)
		{
			_banco.IniciarTransacao();
			_dataAccess.SalvarLogOperacoes(projeto, tipo, etapa, listLog);
			_banco.Commit();
		}

		public void ApagarGeometriasTemporariasTrackmaker(int projeto, int tipo)
		{
			_banco.IniciarTransacao();
			_dataAccess.ApagarGeometriasTemporariasTrackmaker(projeto, tipo);
			_banco.Commit();
		}

		public void FinalizarImportacaoDesenhador(int projeto, int tipo)
		{
			_banco.IniciarTransacao();
			_dataAccess.FinalizarImportacaoDesenhador(projeto, tipo);
			_banco.Commit();
		}

		public void FinalizarImportacaoTrackmaker(int projeto, int tipo)
		{
			_banco.IniciarTransacao();
			_dataAccess.FinalizarImportacaoTrackmaker(projeto, tipo);
			_banco.Commit();
		}

		public void ProcessarGeometrias(int projeto, int tipo)
		{
			_banco.IniciarTransacao();
			_dataAccess.ProcessarGeometrias(projeto, tipo);
			_banco.Commit();
		}

		public void ProcessarValidacao(int projeto, int tipo)
		{
			_banco.IniciarTransacao();
			_dataAccess.ProcessarValidacao(projeto, tipo);
			_banco.Commit();
		}

		public List<Hashtable> ValidarErrosEspaciais(int projeto, int tipo)
		{
			return _dataAccess.ValidarErrosEspaciais(projeto, tipo);
		}

		public List<Hashtable> ValidarObrigatoriedades(int projeto, int tipo)
		{
			return _dataAccess.ValidarObrigatoriedades(projeto, tipo);
		}

		public List<Hashtable> ValidarAtributos(int projeto, int tipo)
		{
			return _dataAccess.ValidarAtributos(projeto, tipo);
		}

		public List<Hashtable> ContabilizarGeometrias(int projeto, int tipo)
		{
			return _dataAccess.ContabilizarGeometrias(projeto, tipo);
		}

		internal void LimparErrosGeometricos(int projectID, int projectType)
		{
			_dataAccess.LimparErrosGeometricos(projectID, projectType);
		}

		internal int ObterProjetoIdDominialidade(int projetoId)
		{
			return _dataAccess.ObterProjetoIdDominialidade(projetoId);
		}
	}
}