using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Arquivo.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Data;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloFiscalizacao.Pdf;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business
{
	public class AcompanhamentoBus
	{
		#region Propriedades

		AcompanhamentoValidar _validar = null;
		AcompanhamentoDa _da = null;
		Historico _historico = new Historico();

		private static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		#endregion

		public AcompanhamentoBus() : this(new AcompanhamentoValidar()) { }

		public AcompanhamentoBus(AcompanhamentoValidar validar)
		{
			_validar = validar;
			_da = new AcompanhamentoDa();
		}

		#region Comandos DML

		public bool Salvar(Acompanhamento entidade)
		{
			try
			{
				if (_validar.Salvar(entidade))
				{
					#region Arquivos/Diretorio

					ArquivoBus _busArquivo = new ArquivoBus(eExecutorTipo.Interno);

					if (entidade.Arquivo != null && !string.IsNullOrWhiteSpace(entidade.Arquivo.Nome) && entidade.Arquivo.Id.GetValueOrDefault() == 0)
					{
						entidade.Arquivo = _busArquivo.Copiar(entidade.Arquivo);
					}

					foreach (var anexo in entidade.Anexos)
					{
						if (anexo.Arquivo.Id == 0)
						{
							anexo.Arquivo = _busArquivo.Copiar(anexo.Arquivo);
						}
					}

					#endregion

					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						#region Arquivos/Banco

						ArquivoDa _arquivoDa = new ArquivoDa();

						if (entidade.Arquivo != null && entidade.Arquivo.Id.GetValueOrDefault() == 0)
						{
							_arquivoDa.Salvar(entidade.Arquivo, User.FuncionarioId, User.Name, User.Login, (int)eExecutorTipo.Interno, User.FuncionarioTid, bancoDeDados);
						}

						foreach (var anexo in entidade.Anexos)
						{
							if (anexo.Arquivo.Id == 0)
							{
								_arquivoDa.Salvar(anexo.Arquivo, User.FuncionarioId, User.Name, User.Login, (int)eExecutorTipo.Interno, User.FuncionarioTid, bancoDeDados);
							}
						}

						#endregion

						if (entidade.Id == 0)
						{
							entidade.NumeroSufixo = ObterNumeroAcompanhamento(entidade);
						}

						_da.Salvar(entidade, bancoDeDados);

						bancoDeDados.Commit();

						Validacao.Add(Mensagem.Acompanhamento.Salvar(entidade.Numero));
					}
				}
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return Validacao.EhValido;
		}

		public bool AlterarSituacao(Acompanhamento entidade)
		{
			try
			{
				if (_validar.AlterarSituacao(entidade))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.AlterarSituacao(entidade, bancoDeDados);

						if (entidade.PdfLaudo.Id.GetValueOrDefault() <= 0)
						{
							ArquivoBus arquivoBus = new ArquivoBus(eExecutorTipo.Interno);
							ArquivoDa arquivoDa = new ArquivoDa();

							entidade.PdfLaudo.Nome = "LaudoAcompanhamentoFiscalizacao";
							entidade.PdfLaudo.Extensao = ".pdf";
							entidade.PdfLaudo.ContentType = "application/pdf";
							entidade.PdfLaudo.Buffer = new PdfFiscalizacao().GerarLaudoAcompanhamentoFiscalizacao(entidade.Id, false, banco: bancoDeDados);
							arquivoBus.Salvar(entidade.PdfLaudo);

							arquivoDa.Salvar(entidade.PdfLaudo, User.FuncionarioId, User.Name, User.Login, (int)eExecutorTipo.Interno, User.FuncionarioTid, bancoDeDados);

							_da.SalvarArquivo(entidade, bancoDeDados);
						}

						_historico.Gerar(entidade.Id, eHistoricoArtefato.acompanhamento, eHistoricoAcao.alterarsituacao, bancoDeDados);

						bancoDeDados.Commit();

						Validacao.Add(Mensagem.Acompanhamento.SituacaoSalvar);
					}
				}
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return Validacao.EhValido;
		}

		public bool Excluir(int id)
		{
			try
			{
				Acompanhamento entidade = Obter(id, true);
				if (_validar.Excluir(entidade))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.Excluir(id, bancoDeDados);

						bancoDeDados.Commit();

						Validacao.Add(Mensagem.Acompanhamento.Excluir(entidade.Numero));
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		#endregion

		#region Obter

		public Resultados<Acompanhamento> ObterAcompanhamentos(int fiscalizacao, BancoDeDados banco = null)
		{
			try
			{
				return _da.ObterAcompanhamentos(fiscalizacao, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public Acompanhamento Obter(int id, bool simplificado = false, BancoDeDados banco = null)
		{
			try
			{
				return _da.Obter(id, simplificado, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		#endregion

		#region Pdf

		public Stream LaudoAcompanhamentoFiscalizacaoPdf(int id, BancoDeDados banco = null)
		{
			try
			{
				PdfFiscalizacao _pdf = new PdfFiscalizacao();
				Acompanhamento acompanhamento = Obter(id, simplificado: true);

				if (acompanhamento.SituacaoId == (int)eAcompanhamentoSituacao.EmCadastro) 
				{
					return _pdf.GerarLaudoAcompanhamentoFiscalizacao(id, banco: banco);
				}


				if (acompanhamento.SituacaoId > (int)eAcompanhamentoSituacao.EmCadastro)
				{
					if (acompanhamento.PdfLaudo.Id.GetValueOrDefault() == 0)
					{
						Validacao.Add(Mensagem.Acompanhamento.ArquivoNaoEncontrado);
						return null;
					}

					ArquivoBus arquivoBus = new ArquivoBus(eExecutorTipo.Interno);
					Arquivo pdf = arquivoBus.Obter(acompanhamento.PdfLaudo.Id.GetValueOrDefault());

					if (acompanhamento.SituacaoId == (int)eAcompanhamentoSituacao.Cancelado)
					{
						pdf.Buffer = PdfMetodosAuxiliares.TarjaVermelha(pdf.Buffer, "CANCELADO " + acompanhamento.DataSituacao.DataTexto);
					}

					return pdf.Buffer;

				}

			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		#endregion

		#region Auxiliares

		public String ObterNumeroAcompanhamento(Acompanhamento entidade)
		{
			List<Acompanhamento> acompanhamentos = ObterAcompanhamentos(entidade.FiscalizacaoId).Itens;

			if (acompanhamentos != null && acompanhamentos.Count <= 0)
			{
				return ASCIIEncoding.ASCII.GetString(new byte[] { 65 });
			}

			Acompanhamento auxilio = acompanhamentos.Single(x => x.Id == acompanhamentos.Max(y => y.Id));

			char[] sufixoChars = auxilio.NumeroSufixo.ToCharArray();
			byte[] sufixoBytes = ASCIIEncoding.ASCII.GetBytes(sufixoChars);

			if (sufixoBytes.Last() == 90)
			{
				if (sufixoBytes.Length == 1 || sufixoBytes.Count(x => x != 90) <= 0)
				{
					byte[] sufixoBytesAux = new byte[sufixoBytes.Length + 1];

					for (int i = 0; i < sufixoBytesAux.Length; i++)
					{
						sufixoBytesAux[i] = 65;
					}

					return ASCIIEncoding.ASCII.GetString(sufixoBytesAux);
				}
				else
				{
					for (int i = sufixoBytes.Length - 1; i >= 0; i--)
					{
						if (sufixoBytes[i] == 90)
						{
							sufixoBytes[i] = 65;
							continue;
						}

						sufixoBytes[i] = (byte)(Convert.ToInt32(sufixoBytes[i]) + 1);
						break;
					}

					return ASCIIEncoding.ASCII.GetString(sufixoBytes);
				}
			}
			else
			{
				sufixoBytes[sufixoBytes.Length - 1] = (byte)(Convert.ToInt32(sufixoBytes[sufixoBytes.Length - 1]) + 1);
				return ASCIIEncoding.ASCII.GetString(sufixoBytes);
			}
		}

		public List<Lista> ObterSituacoes(eAcompanhamentoSituacao situacao, List<Lista> situacoes)
		{
			switch (situacao)
			{
				case eAcompanhamentoSituacao.EmCadastro:
					return situacoes.Where(x => Convert.ToInt32(x.Id) == (int)eAcompanhamentoSituacao.CadastroConcluido).ToList();

				case eAcompanhamentoSituacao.CadastroConcluido:
					return situacoes.Where(x => Convert.ToInt32(x.Id) == (int)eAcompanhamentoSituacao.Cancelado).ToList();

				default:
					return new List<Lista>();
			}
		}

		public bool ExisteAcompanhamento(int fiscalizacao, BancoDeDados banco = null)
		{
			return _da.ExisteAcompanhamento(fiscalizacao, banco);
		}

		#endregion
	}
}