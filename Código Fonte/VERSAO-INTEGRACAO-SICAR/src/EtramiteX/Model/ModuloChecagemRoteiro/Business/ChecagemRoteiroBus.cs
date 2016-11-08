using System;
using System.Collections.Generic;
using System.IO;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloChecagemRoteiro;
using Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloRoteiro;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloChecagemRoteiro.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloRoteiro.Business;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloChecagemRoteiro.Pdf.RoteiroPdf;
using CheckListRelatorio = Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloChecagemRoteiro;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloChecagemRoteiro.Business
{
	public class ChecagemRoteiroBus
	{
		#region Propriedades

		ChecagemValidar _validar = new ChecagemValidar();
		ChecagemRoteiroDa _daCheckListRoteiro = new ChecagemRoteiroDa();
		RoteiroBus _busRoteiro = new RoteiroBus();
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
		GerenciadorConfiguracao<ConfiguracaoChecagemRoteiro> _configCheckListRoteiro = new GerenciadorConfiguracao<ConfiguracaoChecagemRoteiro>(new ConfiguracaoChecagemRoteiro());

		#endregion

		public ChecagemRoteiroBus() { }

		#region Acoes DML

		public bool Salvar(ChecagemRoteiro checkListRoteiro)
		{
			try
			{
				if (_validar.Salvar(checkListRoteiro))
				{
					//lov_checagem_situacao
					checkListRoteiro.Situacao = 1;//Finalizada

					Mensagem msgSucesso = Mensagem.ChecagemRoteiro.Salvar;

					if (checkListRoteiro.Id > 0)
					{
						msgSucesso = Mensagem.ChecagemRoteiro.Editar;
					}

					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_daCheckListRoteiro.Salvar(checkListRoteiro, bancoDeDados);

						if (msgSucesso.Texto.Contains("#n#"))
						{
							msgSucesso.Texto = Mensagem.ChecagemRoteiro.Salvar.Texto.Replace("#n#", checkListRoteiro.Id.ToString());
						}

						bancoDeDados.Commit();
					}

					Validacao.Add(msgSucesso);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		internal void AlterarSituacao(int checkListId, int novaSituacao, BancoDeDados bancoDeDados = null)
		{
			try
			{
				_daCheckListRoteiro.AlterarSituacao(new ChecagemRoteiro() { Id = checkListId, Situacao = novaSituacao }, bancoDeDados);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		private ChecagemRoteiro AtualizarItensRoteiro(ChecagemRoteiro check)
		{
			try
			{
				for (int i = 0; i < check.Roteiros.Count; i++)
				{
					List<Item> itensRoteiroAtual = null;
					Roteiro roteiro = null;

					//Busca o tid de quando o roteiro foi associado
					string tidDeRelacao = check.Roteiros[i].Tid.Clone().ToString();

					if (check.Roteiros[i].Situacao.Equals(2))
					{
						Validacao.Add(Mensagem.ChecagemRoteiro.RoteiroDesativado(check.Roteiros[i].Numero, eTipoMensagem.Informacao));
					}

					//Buscar o roteiro atual
					roteiro = _busRoteiro.ObterSimplificado(check.Roteiros[i].Id);

					if (!_busRoteiro.VerificarTidAtual(check.Roteiros[i].Id, tidDeRelacao))
					{
						Validacao.Add(Mensagem.ChecagemRoteiro.RoteiroAtualizado(roteiro.Numero, roteiro.Versao.ToString()));
						int idRelacao = (check.Roteiros[i].IdRelacionamento ?? 0);
						check.Roteiros[i] = roteiro;
						check.Roteiros[i].IdRelacionamento = idRelacao;
					}

					itensRoteiroAtual = roteiro.Itens;

					for (int j = 0; j < itensRoteiroAtual.Count; j++)
					{
						bool isNew = true;

						for (int k = 0; k < check.Itens.Count; k++)
						{
							if (check.Itens[k].Id == itensRoteiroAtual[j].Id)
							{
								if (!check.Itens[k].Tid.Equals(itensRoteiroAtual[j].Tid, StringComparison.InvariantCultureIgnoreCase))
								{
									List<int> lstRoteiros = check.Itens[k].Roteiros;
									lstRoteiros.Add(check.Roteiros[i].Id);
									check.Itens[k] = itensRoteiroAtual[j];
									check.Itens[k].Roteiros = lstRoteiros;
								}
								else
								{
									check.Itens[k].Roteiros.Add(check.Roteiros[i].Id);
								}

								isNew = false;
								break;
							}
						}
						if (isNew)
						{
							itensRoteiroAtual[j].Roteiros.Add(check.Roteiros[i].Id);
							itensRoteiroAtual[j].Situacao = (int)eChecagemItemSituacao.Pendente;
							itensRoteiroAtual[j].SituacaoTexto = eChecagemItemSituacao.Pendente.ToString();
							check.Itens.Add(itensRoteiroAtual[j]);
						}
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return check;
		}

		private void SetarSituacaoChecagem(ChecagemRoteiro checkListRoteiro)
		{
			checkListRoteiro.Situacao = 1;

			foreach (var item in checkListRoteiro.Itens)
			{
				if (item.Situacao.Equals(1))
				{
					checkListRoteiro.Situacao = 2;
					break;
				}
			}
		}

		public bool Excluir(int id)
		{
			try
			{
				if (_validar.Excluir(id))
				{
					_daCheckListRoteiro.Excluir(id);
					Validacao.Add(Mensagem.ChecagemRoteiro.Excluir(id));
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

		public ChecagemRoteiro ObterSimplificado(int id)
		{
			ChecagemRoteiro ckList = new ChecagemRoteiro();
			try
			{
				ckList = _daCheckListRoteiro.ObterSimplificado(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return ckList;
		}

		public ChecagemRoteiro Obter(int id)
		{
			ChecagemRoteiro ckList = null;
			try
			{
				ckList = _daCheckListRoteiro.Obter(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return ckList;
		}

		public List<Roteiro> ObterRoteirosChecagem(int checagemId)
		{
			List<Roteiro> roteirosAtuais = new List<Roteiro>();

			ChecagemRoteiro checagem = Obter(checagemId);

			roteirosAtuais = checagem.Roteiros;

			if (roteirosAtuais.Count > 0)
			{
				roteirosAtuais[0].Itens = checagem.Itens;
			}
			return roteirosAtuais;
		}

		public List<Item> ObterItensRoteiro(int roteiro, string tid = null)
		{
			List<Item> lista = new List<Item>();

			try
			{
				lista = _busRoteiro.ObterSimplificado(roteiro, tid).Itens;
				lista.ForEach(x =>
				{
					x.Situacao = (int)eChecagemItemSituacao.Pendente;
					x.SituacaoTexto = eChecagemItemSituacao.Pendente.ToString();
				});
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return lista;
		}

		public Resultados<ChecagemRoteiro> Filtrar(ChecagemRoteiroListarFiltro filtrosListar, Paginacao paginacao)
		{
			try
			{
				Filtro<ChecagemRoteiroListarFiltro> filtro = new Filtro<ChecagemRoteiroListarFiltro>(filtrosListar, paginacao);
				Resultados<ChecagemRoteiro> resultados = _daCheckListRoteiro.Filtrar(filtro);

				if (resultados.Quantidade < 1)
				{
					Validacao.Add(Mensagem.Padrao.NaoEncontrouRegistros);
				}

				return resultados;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		#endregion

		#region PDF

		public MemoryStream GerarPdf(ChecagemRoteiro checkListRoteiro)
		{
			CheckListRelatorio.ChecagemRoteiroRelatorio checagem = new CheckListRelatorio.ChecagemRoteiroRelatorio()
			{
				Interessado = checkListRoteiro.Interessado
			};

			checkListRoteiro.Roteiros.ForEach(x =>
			{
				checagem.Roteiros.Add(new RoteiroRelatorio() { Nome = x.Nome, Versao = x.Versao });
			});

			ItemRelatorio itemPdf;
			checkListRoteiro.Itens.ForEach(x =>
			{
				itemPdf = new ItemRelatorio();
				itemPdf.SituacaoId = Convert.ToInt32(x.Situacao);
				itemPdf.Nome = x.Nome;
				itemPdf.Condicionante = x.Condicionante;
				itemPdf.Motivo = x.Motivo;
				checagem.Itens.Add(itemPdf);
			});

			return PdfCheckListRoteiro.GerarCheckListRoteiroPdf(checagem);
		}

		#endregion

		#region Validacao

		public bool ValidarCheckListRoteiroPdf(ChecagemRoteiro checkListRoteiro)
		{
			return _validar.ValidarCheckListRoteiroPdf(checkListRoteiro);
		}

		public bool ValidarAssociarCheckList(int checkListId, int idProtocoloDiferente, bool isProcesso, bool isConversao = false)
		{
			return _validar.ValidarAssociarCheckList(checkListId, idProtocoloDiferente, isProcesso, isConversao);
		}

		public bool ValidarAssociarRoteiro(int roteiroAssociado, List<int> roteirosAssociados)
		{
			return _validar.ValidarAssociarRoteiro(roteiroAssociado, roteirosAssociados);
		}

		#endregion
	}
}