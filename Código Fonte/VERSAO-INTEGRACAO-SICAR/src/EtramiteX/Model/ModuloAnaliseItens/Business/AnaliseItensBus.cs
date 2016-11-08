using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.ModuloAnaliseItens;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloChecagemRoteiro;
using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloProjetoDigital.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAnaliseItens.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloChecagemRoteiro.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFuncionario.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProjetoDigital.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloRequerimento.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloRoteiro.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Business;
using ProjetoDigitalCredenciadoBus = Tecnomapas.EtramiteX.Credenciado.Model.ModuloProjetoDigital.Business.ProjetoDigitalCredenciadoBus;
using CaracterizacaoCredenciadoBus = Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Bussiness.CaracterizacaoBus;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloAnaliseItens.Business
{
	public class AnaliseItensBus
	{
		#region Propriedades

		AnaliseItensValidar _validar = null;
		AnaliseItensDa _da = new AnaliseItensDa();
		ListaBus _busLista = new ListaBus();		
		ProtocoloDa _daProtocolo = new ProtocoloDa();
		RequerimentoBus _busRequerimento = new RequerimentoBus();
		ChecagemRoteiroBus _busChecagem = new ChecagemRoteiroBus();
		TituloBus _busTitulo = new TituloBus();
		RoteiroBus _busRoteiro = new RoteiroBus();
		FuncionarioBus _busFuncionario = new FuncionarioBus(new FuncionarioValidar());
		CaracterizacaoCredenciadoBus _busCaracterizacaoCredenciado = new CaracterizacaoCredenciadoBus();
		GerenciadorConfiguracao _config;

		String EsquemaBancoCredenciado
		{
			get { return _config.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		#endregion
		
		public AnaliseItensBus() 
		{
			_config = new GerenciadorConfiguracao(new ConfiguracaoSistema());
		}

		public AnaliseItensBus(AnaliseItensValidar validar)
		{			
			_validar = validar;
			_config = new GerenciadorConfiguracao(new ConfiguracaoSistema());
		}

		#region Ações DML

		public void CriarAnalise(AnaliseItem analise)
		{
			try
			{
				List<Roteiro> roteirosChecagem = _busChecagem.ObterRoteirosChecagem(analise.Checagem.Id);
				analise.Roteiros = roteirosChecagem;
				List<Situacao> situacoes = _busLista.SituacoesItemAnalise;
				int setorId = _daProtocolo.ObterSetor(analise.Protocolo.Id ?? 0);

				foreach (Roteiro roteiroAtual in roteirosChecagem)
				{
					foreach (Item itemAtual in roteiroAtual.Itens)
					{
						switch ((eChecagemItemSituacao)itemAtual.Situacao)
						{
							case eChecagemItemSituacao.Pendente:
								itemAtual.Situacao = (int)eAnaliseItemSituacao.Pendente;
								break;
							case eChecagemItemSituacao.Conferido:
								itemAtual.Situacao = (int)eAnaliseItemSituacao.Recebido;
								break;
							case eChecagemItemSituacao.Dispensado:
								itemAtual.Situacao = (int)eAnaliseItemSituacao.Dispensado;
								break;
						}

						if (itemAtual.Situacao != (int)eAnaliseItemSituacao.Reprovado &&
							itemAtual.Situacao != (int)eAnaliseItemSituacao.Dispensado)
						{
							itemAtual.Motivo = String.Empty;
						}

						itemAtual.SetorId = setorId;
						itemAtual.Recebido = itemAtual.Situacao == (int)eAnaliseItemSituacao.Recebido;
						itemAtual.SituacaoTexto = situacoes.SingleOrDefault(x => x.Id == itemAtual.Situacao).Texto;
						analise.Itens.Add(itemAtual);
					}
				}

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
				{
					GerenciadorTransacao.ObterIDAtual();
					bancoDeDados.IniciarTransacao();
					_da.Salvar(analise, bancoDeDados);

					bancoDeDados.Commit();
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void Salvar(AnaliseItem analise, bool atualizarRoteiro = false)
		{
			try
			{
				//Validação proposta pela analista
				if (!atualizarRoteiro)
				{
					VerificarProtocolo(analise.ProtocoloPai);
				}

				if (Validacao.EhValido && _validar.Salvar(analise, atualizarRoteiro))
				{
					int setorId = _daProtocolo.ObterSetor(analise.Protocolo.Id ?? 0);
					Funcionario func = _busFuncionario.Obter(_busFuncionario.User.FuncionarioId);

					// para cada item que foi alterado da análise, atualiza sua DataAnalise para NOW
					foreach (Item item in analise.Itens)
					{
						if (item.Editado || item.IsAtualizado)
						{
							item.Analista = func.Nome;
							item.DataAnalise = DateTime.Now.ToString();
							item.SetorId = setorId;
						}

						if (item.Situacao != (int)eAnaliseItemSituacao.Reprovado &&
							item.Situacao != (int)eAnaliseItemSituacao.Dispensado)
						{
							item.Motivo = String.Empty;
						}

						if (!item.Avulso && item.Tipo != (int)eRoteiroItemTipo.ProjetoDigital && (item.IdRelacionamento < 1 || item.IsAtualizado))
						{
							item.Motivo = "Atualização de versão do roteiro orientativo.";
						}

						item.IsAtualizado = false;
						item.Editado = false;
					}

					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.Salvar(analise, bancoDeDados);

						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void AlterarSituacao(AnaliseItem analise, BancoDeDados banco = null)
		{
			try
			{
				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
				{
					bancoDeDados.IniciarTransacao();

					_da.AlterarSituacao(analise, bancoDeDados);

					bancoDeDados.Commit();
				}				
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public bool ImportarProjetoDigital(int analiseId, int projetoDigitalId)
		{
			try
			{
				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
				{
					bancoDeDados.IniciarTransacao();

					using (BancoDeDados bancoCredenciado = BancoDeDados.ObterInstancia(EsquemaBancoCredenciado))
					{
						bancoCredenciado.IniciarTransacao();
						AnaliseItem analise = Obter(analiseId);
						analise.Situacao = (int)eAnaliseSituacao.Finalizado;

						if (_validar.Analise(analise))
						{
							CaracterizacaoBus caracterizacaoBus = new CaracterizacaoBus();

							caracterizacaoBus.CopiarDadosCredenciado(projetoDigitalId, bancoDeDados, bancoCredenciado);

							_da.AlterarSituacao(analise, bancoDeDados);

							ProjetoDigitalCredenciadoBus projetoDigitalCredenciadoBus = new ProjetoDigitalCredenciadoBus();
							projetoDigitalCredenciadoBus.AlterarEtapaTemporario(projetoDigitalId, eProjetoDigitalEtapaImportacao.Finalizado, bancoDeDados);

							if (!Validacao.EhValido)
							{
								bancoDeDados.Rollback();
								bancoCredenciado.Rollback();
								return false;
							}
						}

						bancoCredenciado.Commit();
					}

					bancoDeDados.Commit();
				}

				return true;
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return false;
		}

		#endregion

		#region Obter

		public AnaliseItem Obter(int id, BancoDeDados banco = null) 
		{
			AnaliseItem analise = null;
			try
			{
				analise = _da.Obter(id, banco: banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return analise;
		}

		public AnaliseItem ObterPorChecagem(int checagem, bool simplificado = false, BancoDeDados banco = null)
		{
			AnaliseItem analise = null;
			try
			{
				analise = _da.ObterPorChecagem(checagem, simplificado: simplificado, banco: banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return analise;
		}

		public AnaliseItem Obter(Protocolo protocolo)
		{
			AnaliseItem analise = null;
			try
			{
				analise = _da.Obter(protocolo);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return analise;
		}

		public AnaliseItem ObterAnaliseProtocolo(Requerimento req, int protocoloId, bool atualizar)
		{
			AnaliseItem analise = null;
			try
			{
				int analiseId = _da.ExisteAnalise(req.Checagem);

				if (analiseId != 0)
				{
					analise = _da.Obter(analiseId);
					if (atualizar)
					{
						analise.Atualizado = atualizar;
						AtualizarAnalise(analise);
					}
				}
				else
				{
					analise = new AnaliseItem();
					analise.Checagem.Id = req.Checagem;
					analise.Protocolo.Id = protocoloId;
					CriarAnalise(analise);
				}

				if (req.IsRequerimentoDigital && !analise.Itens.Exists(x => x.Tipo == (int)eRoteiroItemTipo.ProjetoDigital))
				{
					analise.Itens.AddRange(ObterItensRequerimentoDigital(req));
				}

				int projetoDigitalId = new ProjetoDigitalCredenciadoBus().ObterProjetoDigitalId(req.Id);

				List<Caracterizacao> caracterizacoes = _busCaracterizacaoCredenciado.ObterCaracterizacoesAssociadasProjetoDigital(projetoDigitalId);

				analise.Itens.Where(x => x.Tipo == (int)eRoteiroItemTipo.ProjetoDigital).ToList().ForEach(x => {
					caracterizacoes.Where(c => c.Tipo == (eCaracterizacao)x.CaracterizacaoTipoId).ToList().ForEach(c => {
						x.TemProjetoGeografico = c.ProjetoId > 0;
					});
				
				});

				ValidarSituacaoRoteiros(analise);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return analise;
		}

		public AnaliseItem ObterSimplificado(Protocolo protocolo)
		{
			AnaliseItem resultado = null;

			try
			{
				resultado = _da.Obter(protocolo, true);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return resultado;
		}

		public AnaliseItem ObterHistorico(int id, string tid = null, bool simplificado = false, BancoDeDados banco = null)
		{
			AnaliseItem analise = null;
			try
			{
				analise = _da.ObterHistorico(id, tid, simplificado, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return analise;
		}

		public List<Item> ObterHistoricoAnalise(int itemId, int checagemId)
		{
			List<Item> listaAnalise = new List<Item>();

			try
			{
				listaAnalise = _da.ObterHistoricoAnalise(itemId, checagemId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return listaAnalise;
		}

		public void AtualizarAnalise(AnaliseItem analise)
		{
			try
			{
				List<Roteiro> roteirosAtuais = _busRoteiro.ObterRoteirosAtuais(analise);

				//Atualiza o TID do roteiro da análise com o TID do roteiro atual
				roteirosAtuais.ForEach(atual =>
				{
					analise.Roteiros.SingleOrDefault(rot => rot.Id == atual.Id).Tid = atual.Tid;
				});

				//Atualiza o TID do item da análise com o TID do item atual
				List<Item> itens = roteirosAtuais.SelectMany(item => item.Itens).ToList();
				Item aux = null;
				itens.ForEach(atual =>
				{
					aux = analise.Itens.SingleOrDefault(x => x.Id == atual.Id);
					if (aux != null)
					{
						if (aux.Tid != atual.Tid && aux.Situacao != (int)eAnaliseItemSituacao.Dispensado)
						{
							aux.Editado = true;
							aux.Situacao = (int)eAnaliseItemSituacao.Recebido;
							aux.Recebido = true;
							aux.SituacaoTexto = _busLista.SituacoesItemAnalise.Find(x => x.Id == (int)eAnaliseItemSituacao.Recebido).Texto;
							aux.Motivo = String.Empty;
						}

						aux.Nome = atual.Nome;
						aux.Condicionante = atual.Condicionante;
						aux.Tid = atual.Tid;
					}
					else
					{
						atual.Motivo = string.Empty;
						atual.Editado = true;
						atual.Situacao = (int)eAnaliseItemSituacao.Pendente;
						atual.SituacaoTexto = _busLista.SituacoesItemAnalise.Find(x => x.Id == (int)eAnaliseItemSituacao.Pendente).Texto;
						analise.Itens.Add(atual);
					}
				});
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public MergeItens MergeItens(int checagemId)
		{
			MergeItens merge = new MergeItens();

			try
			{
				int analiseId = _da.ExisteAnalise(checagemId);

				if (analiseId == 0)
				{
					return merge;
				}

				AnaliseItem analise = _da.Obter(analiseId);

				merge.Itens = analise.Itens;
				merge.ItensAtuais.AddRange(analise.Itens.Where(item => !item.Avulso && item.Tipo != (int)eRoteiroItemTipo.ProjetoDigital).ToList());

				List<Roteiro> roteirosAtuais =  _busRoteiro.ObterRoteirosAtuais(analise);

				//Atualiza o TID do roteiro da análise com o TID do roteiro atual
				roteirosAtuais.ForEach(atual =>
				{
					atual.IdRelacionamento = analise.Roteiros.SingleOrDefault(rot => rot.Id == atual.Id).IdRelacionamento;
				});
				merge.Roteiros.AddRange(roteirosAtuais);

				#region Merge de Itens

				//Itens dos roteiros antigos
				List<Item> itensRoteirosAnalise = new List<Item>();
				foreach (var item in analise.Roteiros)
				{
					itensRoteirosAnalise.AddRange(_busRoteiro.Obter(item.Id, item.Tid).Itens);
				}

				//Não duplicar linhas do historico
				foreach (Item item in merge.Itens)
				{
					item.Analista = string.Empty;
					item.DataAnalise = string.Empty;
					//item.SetorId = 0;
				}

				foreach (Item item in merge.Itens.Where(item => !item.Avulso && item.Tipo != (int)eRoteiroItemTipo.ProjetoDigital))
				{
					if (itensRoteirosAnalise.Exists(itemRotAntigo => itemRotAntigo.Id == item.Id))
					{
						item.StatusId = 3;
						item.StatusTela = "Removido do Roteiro";
					}
				}

				foreach (Item itemAtual in roteirosAtuais.SelectMany(item => item.Itens))
				{
					Item aux = merge.Itens.SingleOrDefault(x => x.Id == itemAtual.Id);
					int posicao = merge.Itens.IndexOf(aux);
					bool existiaRoteiroAntigo = itensRoteirosAnalise.Exists(itemRotAntigo => itemRotAntigo.Id == itemAtual.Id);

					if (aux != null && existiaRoteiroAntigo)
					{
						if (itemAtual.Tid != aux.Tid)
						{
							itemAtual.IdRelacionamento = aux.IdRelacionamento;
							itemAtual.Situacao = (int)eAnaliseItemSituacao.Recebido;
							itemAtual.IsAtualizado = true;
							itemAtual.Recebido = true;

							aux = itemAtual;
							aux.StatusId = 2;
							aux.StatusTela = "Atualizado";
						}
						else
						{
							aux.StatusId = 1;
							aux.StatusTela = "Não Atualizado";
						}

						if (aux.Avulso)
						{
							aux.IsAtualizado = true;
							aux.Avulso = false;
							aux.StatusId = 4;
							aux.StatusTela = "Adicionado no Roteiro";
							aux.Situacao = (int)eAnaliseItemSituacao.Pendente;
						}

						merge.Itens[posicao] = aux;
					}
					else
					{
						if (existiaRoteiroAntigo || aux != null)
						{
							itemAtual.IdRelacionamento = aux.IdRelacionamento;
						}
						else
						{
							itemAtual.IdRelacionamento = 0;
						}

						itemAtual.Situacao = (int)eAnaliseItemSituacao.Pendente;
						itemAtual.IsAtualizado = true;
						itemAtual.StatusId = 4;
						itemAtual.StatusTela = "Adicionado no Roteiro";
						merge.Itens.Add(itemAtual);
					}
				}

				#endregion
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return merge;
		}

		public AnaliseItem ObterAnaliseTitulo(int tituloId, BancoDeDados banco = null)
		{
			try
			{
				return _da.ObterAnaliseTitulo(tituloId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

		public List<Item> ObterItensRequerimentoDigital(Requerimento requerimento)
		{
			List<Item> itens = new List<Item>();

			if (requerimento.IsRequerimentoDigital)
			{
				try
				{
					int projetoDigitalId = new ProjetoDigitalCredenciadoBus().ObterProjetoDigitalId(requerimento.Id);
					List<Caracterizacao> caracterizacoes = _busCaracterizacaoCredenciado.ObterCaracterizacoesAssociadasProjetoDigital(projetoDigitalId);
					itens = _da.ObterItensProjetoDigital(caracterizacoes);
				}
				catch (Exception exc)
				{
					Validacao.AddErro(exc);
				}
			}

			return itens;

		}

		#endregion

		#region Validações

		public AnaliseItem VerificarProtocolo(string numero)
		{
			AnaliseItem analiseItem = new AnaliseItem();

			ProtocoloNumero protocolo = _daProtocolo.ObterProtocolo(numero);
			analiseItem.IsProcesso = protocolo.IsProcesso;

			try
			{
				if (_validar.VerificarProtocolo(protocolo))
				{
					analiseItem.Requerimentos = _daProtocolo.ObterProtocoloRequerimentos(protocolo.Id);
					analiseItem.Protocolo.Id = protocolo.Id;
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return analiseItem;

		}

		public bool ExisteTituloPendencia(Protocolo protocolo)
		{
			return _busTitulo.ExisteTituloPendencia(protocolo);
		}

		public bool ExisteAnalise(int checagem)
		{
			try
			{
				return _da.ExisteAnalise(checagem) > 0;
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return false;
		}

		public bool IsImportadoCaracterizacao(int projetoDigitalId)
		{
			try
			{
				return _da.IsImportadoCaracterizacao(projetoDigitalId);
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return false;
		}

		public void ValidarSituacaoRoteiros(AnaliseItem analise)
		{
			List<Roteiro> roteirosAtuais = _busRoteiro.ObterRoteirosAtuais(analise);

			_validar.ValidarSituacaoRoteiro(roteirosAtuais);
		}

		#endregion 
	}
}