﻿using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao.Configuracoes;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Data;
using System.Linq;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business
{
	public class ConfigFiscalizacaoBus
	{
		#region Propriedades

		ConfigFiscalizacaoDa _da = new ConfigFiscalizacaoDa();
		ConfigFiscalizacaoValidar _validar = new ConfigFiscalizacaoValidar();

		#endregion

		#region Comandos DML

		public bool Salvar(ConfigFiscalizacao configuracao)
		{
			var isCadastrar = configuracao.Id == 0;
			try
			{
				if (_validar.Salvar(configuracao))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.Salvar(configuracao, bancoDeDados);

						Validacao.Add(isCadastrar ? Mensagem.FiscalizacaoConfiguracao.Cadastrar(configuracao.Id.ToString()) : Mensagem.FiscalizacaoConfiguracao.Editar(configuracao.Id.ToString()));

						bancoDeDados.Commit();
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
				if (_validar.Excluir(id))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.Excluir(id, bancoDeDados);

						Validacao.Add(Mensagem.FiscalizacaoConfiguracao.Excluir(id));

						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		#region Tipo Infração

		public bool SalvarTipoInfracao(Item entidade)
		{
			try
			{
				if (_validar.SalvarTipoInfracao(entidade))
				{

					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						int tipoId = _da.SalvarTipoInfracao(entidade, bancoDeDados);

						_da.GerarConsultaTipoInfracao(tipoId, bancoDeDados);

						Validacao.Add(Mensagem.FiscalizacaoConfiguracao.SalvarTipoInfracao);

						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return Validacao.EhValido;
		}

		public bool ExcluirTipoInfracao(int id)
		{
			try
			{
				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
				{

					bancoDeDados.IniciarTransacao();

					if (_validar.ExcluirTipoInfracao(id))
					{
						_da.ExcluirTipoInfracao(id, bancoDeDados);
						Validacao.Add(Mensagem.FiscalizacaoConfiguracao.ExcluirTipoInfracao);
					}

					bancoDeDados.Commit();
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		#endregion

		#region Item Infracao

		public bool SalvarItemInfracao(Item entidade)
		{
			try
			{
				if (_validar.SalvarItemInfracao(entidade))
				{

					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						int itemId = _da.SalvarItemInfracao(entidade, bancoDeDados);

						_da.GerarConsultaItem(itemId, bancoDeDados);

						Validacao.Add(Mensagem.FiscalizacaoConfiguracao.SalvarItemInfracao);

						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return Validacao.EhValido;
		}

		public bool ExcluirItemInfracao(int id)
		{
			try
			{
				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
				{
					bancoDeDados.IniciarTransacao();

					if (_validar.ExcluirItemInfracao(id))
					{
						_da.ExcluirItemInfracao(id, bancoDeDados);
						Validacao.Add(Mensagem.FiscalizacaoConfiguracao.ExcluirItemInfracao);
					}

					bancoDeDados.Commit();
				}

			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		#endregion

		#region SubItem Infracao

		public bool SalvarSubItemInfracao(Item entidade)
		{
			try
			{
				if (_validar.SalvarSubItemInfracao(entidade))
				{

					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.SalvarSubItemInfracao(entidade, bancoDeDados);

						Validacao.Add(Mensagem.FiscalizacaoConfiguracao.SalvarSubItemInfracao);

						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return Validacao.EhValido;
		}

		public bool ExcluirSubItemInfracao(int id)
		{
			try
			{
				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
				{
					bancoDeDados.IniciarTransacao();

					if (_validar.ExcluirSubItemInfracao(id))
					{
						_da.ExcluirSubItemInfracao(id, bancoDeDados);
						Validacao.Add(Mensagem.FiscalizacaoConfiguracao.ExcluirSubItemInfracao);
					}

					bancoDeDados.Commit();
				}

			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		#endregion

		#region Penalidade

		public bool ExcluirPenalidade(int id)
		{
			try
			{
				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
				{
					bancoDeDados.IniciarTransacao();


					_da.ExcluirPenalidade(id, bancoDeDados);
					Validacao.Add(Mensagem.FiscalizacaoConfiguracao.ExcluirPenalidade);


					bancoDeDados.Commit();
				}

			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public bool SalvarPenalidade(List<Penalidade> penalidades)
		{
			try
			{
				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
				{
					bancoDeDados.IniciarTransacao();
					_da.ExcluirPenalidades(penalidades.Where(p => p.Id != "0").ToList(), bancoDeDados);
					bancoDeDados.Commit();
				}

				foreach (Penalidade entidade in penalidades)
				{
					if (_validar.SalvarPenalidade(entidade))
					{

						GerenciadorTransacao.ObterIDAtual();

						using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
						{
							bancoDeDados.IniciarTransacao();

							_da.SalvarPenalidade(entidade, bancoDeDados);

							Validacao.Add(Mensagem.FiscalizacaoConfiguracao.SalvarPenalidade);

							bancoDeDados.Commit();
						}
					}
				}
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return Validacao.EhValido;

		}

		#endregion

		#region Campo Infração
		public bool SalvarCampoInfracao(Item entidade)
		{
			try
			{
				if (_validar.SalvarCampoInfracao(entidade))
				{

					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.SalvarCampoInfracao(entidade, bancoDeDados);

						Validacao.Add(Mensagem.FiscalizacaoConfiguracao.SalvarCampoInfracao);

						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return Validacao.EhValido;
		}

		public bool ExcluirCampoInfracao(int id)
		{
			try
			{
				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
				{
					bancoDeDados.IniciarTransacao();

					if (_validar.ExcluirCampoInfracao(id))
					{
						_da.ExcluirCampoInfracao(id, bancoDeDados);
						Validacao.Add(Mensagem.FiscalizacaoConfiguracao.ExcluirCampoInfracao);
					}

					bancoDeDados.Commit();
				}

			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		#endregion

		#region Pergunta Infracao

		public bool SalvarPerguntaInfracao(PerguntaInfracao entidade)
		{
			try
			{
				if (_validar.SalvarPerguntaInfracao(entidade))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						int id = _da.SalvarPerguntaInfracao(entidade, bancoDeDados);

						Validacao.Add(Mensagem.FiscalizacaoConfiguracao.SalvarPerguntaInfracao(id.ToString()));

						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return Validacao.EhValido;
		}

		public bool ExcluirPerguntaInfracao(int id)
		{
			try
			{
				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
				{
					bancoDeDados.IniciarTransacao();

					if (_validar.ExcluirPerguntaInfracao(id))
					{
						_da.ExcluirPerguntaInfracao(id, bancoDeDados);
						Validacao.Add(Mensagem.FiscalizacaoConfiguracao.ExcluirPerguntaInfracao);
					}

					bancoDeDados.Commit();
				}

			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		#endregion

		#region Resposta Infracao

		public bool SalvarRespostaInfracao(Item entidade)
		{
			try
			{
				if (_validar.SalvarRespostaInfracao(entidade))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.SalvarRespostaInfracao(entidade, bancoDeDados);

						Validacao.Add(Mensagem.FiscalizacaoConfiguracao.SalvarRespostaInfracao);

						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return Validacao.EhValido;
		}

		public bool ExcluirRespostaInfracao(int id)
		{
			try
			{
				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
				{
					bancoDeDados.IniciarTransacao();

					if (_validar.ExcluirRespostaInfracao(id))
					{
						_da.ExcluirRespostaInfracao(id, bancoDeDados);
						Validacao.Add(Mensagem.FiscalizacaoConfiguracao.ExcluirRespostaInfracao);
					}

					bancoDeDados.Commit();
				}

			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		#endregion

		#region Produtos Apreendidos / Destinação

		public bool SalvarProdutosDestinacao(List<ProdutoApreendido> listaProdutos, List<DestinacaoProduto> listaDestinos)
		{
			try
			{
				if (_validar.SalvarProdutosApreendidos(listaProdutos)
					&& _validar.SalvarDestinacao(listaDestinos))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.SalvarProdutosApreendidos(listaProdutos, bancoDeDados);

						_da.SalvarDestinacao(listaDestinos, bancoDeDados);

						Validacao.Add(Mensagem.FiscalizacaoConfiguracao.SalvarProdutosDestinos);

						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return Validacao.EhValido;
		}

		#endregion Produtos Apreendidos / Destinação

		#region Códigos da Receita

		public bool SalvarCodigosReceita(List<CodigoReceita> listaCodigosReceita)
		{
			try
			{
				if (_validar.SalvarCodigosReceita(listaCodigosReceita))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.SalvarCodigosReceita(listaCodigosReceita, bancoDeDados);

						Validacao.Add(Mensagem.FiscalizacaoConfiguracao.SalvarCodigoReceita);

						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception e)
			{
				//TODO: alterar isso quando o resto das telas estiver pronto 
				if (e.Message.Contains("integrity constraint"))
				{
					Validacao.Add(Mensagem.FiscalizacaoConfiguracao.ErroCodigoUsado);
				}
				else
				{
					Validacao.AddErro(e);
				}
			}

			return Validacao.EhValido;
		}

		public List<CodigoReceita> ObterCodigosReceita()
		{
			List<CodigoReceita> listaCodigosReceita = new List<CodigoReceita>();

			try
			{
				listaCodigosReceita = _da.ObterCodigosReceita();
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return listaCodigosReceita;
		}

		public bool PermiteExcluirCodigo(CodigoReceita codigo)
		{
			bool podeExcluir = false;

			try
			{
				podeExcluir = _da.PermiteExcluirCodigo(codigo);

				if (!podeExcluir)
				{
					Validacao.Add(Mensagem.FiscalizacaoConfiguracao.ErroCodigoUsado);
				}
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return podeExcluir;
		}

		#endregion Códigos da Receita

		#region Vrte

		public bool SalvarVrte(List<Vrte> listaVrte)
		{
			try
			{
				if (_validar.SalvarVrte(listaVrte))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.SalvarVrte(listaVrte, bancoDeDados);

						Validacao.Add(Mensagem.FiscalizacaoConfiguracao.SalvarVrte);

						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return Validacao.EhValido;
		}

		public Vrte ObterVrte(int ano)
		{
			var vrte = new Vrte();

			try
			{
				vrte = _da.ObterVrte(ano);
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return vrte;
		}

		public List<Vrte> ObterVrte()
		{
			List<Vrte> listaVrte = new List<Vrte>();

			try
			{
				listaVrte = _da.ObterVrte();
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return listaVrte;
		}

		public bool PermiteExcluirVrte(Vrte vrte)
		{
			bool podeExcluir = false;

			try
			{
				podeExcluir = _da.PermiteExcluirVrte(vrte);
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return podeExcluir;
		}

		#endregion Vrte

		#region Parametrizacao 

		public bool SalvarParametrizacao(Parametrizacao entidade)
		{
			try
			{
				if (_validar.SalvarParametrizacao(entidade))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						int id = _da.SalvarParametrizacao(entidade, bancoDeDados);
						foreach (var detalhe in entidade.ParametrizacaoDetalhes)
							detalhe.ParametrizacaoId = id;
						_da.SalvarParametrizacaoDetalhe(entidade.ParametrizacaoDetalhes, bancoDeDados);

						Validacao.Add(Mensagem.FiscalizacaoConfiguracao.SalvarParametrizacao);

						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return Validacao.EhValido;
		}

		public bool ExcluirParametrizacao(int id)
		{
			try
			{
				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
				{
					bancoDeDados.IniciarTransacao();

					_da.ExcluirParametrizacao(id, bancoDeDados);
					Validacao.Add(Mensagem.FiscalizacaoConfiguracao.ExcluirParametrizacao);

					bancoDeDados.Commit();
				}

			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		#endregion

		#endregion

		#region Obter


		public ConfigFiscalizacao Obter(int id, BancoDeDados banco = null)
		{
			ConfigFiscalizacao entidade = null;

			try
			{
				entidade = _da.Obter(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return entidade;
		}

		public Dictionary<string, object> Obter(int classificacaoId, int tipoId, int itemId, BancoDeDados banco = null)
		{
			try
			{
				return _da.Obter(classificacaoId, tipoId, itemId, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}


		public List<Item> ObterTipoInfracao()
		{
			List<Item> lista = null;
			try
			{
				lista = _da.ObterTipoInfracao();
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return lista;
		}


		public List<Penalidade> ObterPenalidades()
		{
			List<Penalidade> lista = null;
			try
			{
				lista = _da.ObterPenalidades();
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return lista;
		}

		public List<Item> ObterItemInfracao()
		{
			List<Item> lista = null;
			try
			{
				lista = _da.ObterItemInfracao();
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return lista;
		}

		public List<Item> ObterSubItemInfracao()
		{
			List<Item> lista = null;
			try
			{
				lista = _da.ObterSubItemInfracao();
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return lista;
		}

		public List<Item> ObterCampoInfracao()
		{
			List<Item> lista = null;
			try
			{
				lista = _da.ObterCampoInfracao();
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return lista;
		}

		public List<Item> ObterPerguntaInfracao()
		{
			List<Item> lista = null;
			try
			{
				lista = _da.ObterPerguntaInfracao();
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return lista;
		}

		public PerguntaInfracao ObterPerguntaRespostasInfracao(int id)
		{
			PerguntaInfracao pergunta = null;
			try
			{
				pergunta = _da.ObterPerguntaRespostasInfracao(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return pergunta;
		}

		public List<Item> ObterRespostaInfracao()
		{
			List<Item> lista = null;
			try
			{
				lista = _da.ObterRespostaInfracao();
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return lista;
		}


		public List<Item> ObterTipos(int classificacaoId = 0)
		{
			List<Item> lista = null;
			try
			{
				lista = _da.ObterTipos(classificacaoId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return lista;
		}

		public List<Lista> ObterItens(int classificacaoId, int tipoId)
		{
			List<Lista> lista = null;
			try
			{
				lista = _da.ObterItens(classificacaoId, tipoId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return lista;
		}

		public List<Lista> ObterSubitens(int classificacaoId, int tipoId, int itemId)
		{
			List<Lista> lista = null;
			try
			{
				lista = _da.ObterSubitens(classificacaoId, tipoId, itemId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return lista;
		}

		public List<Lista> ObterPenalidadesLista()
		{
			List<Lista> lista = null;
			try
			{
				lista = _da.ObterPenalidadesLista();
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return lista;
		}

		public List<Lista> ObterSeries(bool isSim)
		{
			List<Lista> lista = null;
			try
			{
				lista = _da.ObterSeries(isSim);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return lista;
		}

		public List<InfracaoCampo> ObterCampos(int classificacaoId, int tipoId, int itemId)
		{
			List<InfracaoCampo> lista = null;
			try
			{
				lista = _da.ObterCampos(classificacaoId, tipoId, itemId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return lista;
		}

		public List<InfracaoPergunta> ObterQuestionarios(int classificacaoId, int tipoId, int itemId)
		{
			List<InfracaoPergunta> lista = null;
			try
			{
				lista = _da.ObterPerguntas(classificacaoId, tipoId, itemId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return lista;
		}

		public List<ProdutoApreendido> ObterProdutosApreendidos()
		{
			List<ProdutoApreendido> listaProdutos = new List<ProdutoApreendido>();

			try
			{
				listaProdutos = _da.ObterProdutosApreendidos();
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return listaProdutos;
		}

		public List<DestinacaoProduto> ObterDestinacao()
		{
			List<DestinacaoProduto> listaDestinacao = new List<DestinacaoProduto>();

			try
			{
				listaDestinacao = _da.ObterDestinacao();
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return listaDestinacao;
		}

		public List<Lista> ObterItensConfig(bool? isAtivo)
		{
			List<Lista> lista = null;
			try
			{
				lista = _da.ObterItensConfig(isAtivo);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return lista;
		}

		public List<Lista> ObterTiposConfig(bool? isAtivo)
		{
			List<Lista> lista = null;
			try
			{
				lista = _da.ObterTiposConfig(isAtivo);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return lista;
		}

		public List<Lista> ObterSubitensConfig(bool? isAtivo)
		{
			List<Lista> lista = null;
			try
			{
				lista = _da.ObterSubitensConfig(isAtivo);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return lista;
		}

		public List<Lista> ObterPerguntasConfig()
		{
			List<Lista> lista = null;
			try
			{
				lista = _da.ObterPerguntasConfig();
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return lista;
		}

		public List<Lista> ObterCamposConfig(bool? isAtivo)
		{
			List<Lista> lista = null;
			try
			{
				lista = _da.ObterCamposConfig(isAtivo);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return lista;
		}

		public Resultados<PerguntaInfracaoListarResultado> PerguntasFiltrar(PerguntaInfracaoListarFiltro filtrosListar, Paginacao paginacao)
		{
			try
			{
				Filtro<PerguntaInfracaoListarFiltro> filtro = new Filtro<PerguntaInfracaoListarFiltro>(filtrosListar, paginacao);
				Resultados<PerguntaInfracaoListarResultado> resultados = _da.PerguntasFiltrar(filtro);

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

		#region Parametrizacao
		public List<Parametrizacao> ObterParametrizacao()
		{
			List<Parametrizacao> lista = null;
			try
			{
				lista = _da.ObterParametrizacao();
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return lista;
		}
		
		public Resultados<ParametrizacaoListarResultado> ParametrizacaoFiltrar(ParametrizacaoListarFiltro filtrosListar, Paginacao paginacao)
		{
			try
			{
				Filtro<ParametrizacaoListarFiltro> filtro = new Filtro<ParametrizacaoListarFiltro>(filtrosListar, paginacao);
				Resultados<ParametrizacaoListarResultado> resultados = _da.ParametrizacaoFiltrar(filtro);

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

		public Parametrizacao ObterParametrizacao(int id)
		{
			Parametrizacao parametrizacao = null;
			try
			{
				parametrizacao = _da.ObterParametrizacao(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return parametrizacao;
		}

		public Parametrizacao ObterParametrizacao(int codigoReceita, DateTime data)
		{
			Parametrizacao parametrizacao = null;
			try
			{
				parametrizacao = _da.ObterParametrizacao(codigoReceita, data);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return parametrizacao;
		}

		#endregion Parametrizacao

		public Resultados<ConfigFiscalizacao> Filtrar(ConfigFiscalizacaoListarFiltro filtrosListar, Paginacao paginacao)
		{
			try
			{
				Filtro<ConfigFiscalizacaoListarFiltro> filtro = new Filtro<ConfigFiscalizacaoListarFiltro>(filtrosListar, paginacao);
				Resultados<ConfigFiscalizacao> resultados = _da.Filtrar(filtro);

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

		#region Auxiliares


		public void AlterarSituacaoTipoInfracao(int tipoId, int situacaoNova)
		{
			List<String> configAssociadas = _da.ObterIdsConfiguracoesAssociadasTipoInfracao(tipoId);

			if (configAssociadas.Count > 0 && situacaoNova == 0)
			{
				Validacao.Add(Mensagem.FiscalizacaoConfiguracao.TipoInfracaoNaoPodeDesativar(Mensagem.Concatenar(configAssociadas)));
			}
			else
			{
				_da.AlterarSituacaoTipoInfracao(tipoId, situacaoNova);
				Validacao.Add(Mensagem.FiscalizacaoConfiguracao.TipoInfracaoSituacaoAlterada);
			}
		}

		public void AlterarSituacaoItemInfracao(int ItemId, int situacaoNova)
		{
			List<String> configAssociadas = _da.ObterIdsConfiguracoesAssociadasItemInfracao(ItemId);

			if (configAssociadas.Count > 0 && situacaoNova == 0)
			{
				Validacao.Add(Mensagem.FiscalizacaoConfiguracao.ItemInfracaoNaoPodeDesativar(Mensagem.Concatenar(configAssociadas)));
			}
			else
			{
				_da.AlterarSituacaoItemInfracao(ItemId, situacaoNova);
				Validacao.Add(Mensagem.FiscalizacaoConfiguracao.ItemSituacaoAlterada);
			}
		}

		public void AlterarSituacaoSubItemInfracao(int SubItemId, int situacaoNova)
		{
			List<String> configAssociadas = _da.ObterIdsConfiguracoesAssociadasSubItemInfracao(SubItemId);

			if (configAssociadas.Count > 0 && situacaoNova == 0)
			{
				Validacao.Add(Mensagem.FiscalizacaoConfiguracao.SubItemInfracaoNaoPodeDesativar(Mensagem.Concatenar(configAssociadas)));
			}
			else
			{
				_da.AlterarSituacaoSubItemInfracao(SubItemId, situacaoNova);
				Validacao.Add(Mensagem.FiscalizacaoConfiguracao.SubItemSituacaoAlterada);
			}
		}

		public void AlterarSituacaoPenalidade(int Id, int situacaoNova)
		{
			_da.AlterarSituacaoPenalidade(Id, situacaoNova);
		}

		public void AlterarSituacaoCampoInfracao(int campoId, int situacaoNova)
		{
			List<String> configAssociadas = _da.ObterIdsConfiguracoesAssociadasCampoInfracao(campoId);

			if (configAssociadas.Count > 0 && situacaoNova == 0)
			{
				Validacao.Add(Mensagem.FiscalizacaoConfiguracao.CampoInfracaoNaoPodeDesativar(Mensagem.Concatenar(configAssociadas)));
			}
			else
			{
				_da.AlterarSituacaoCampoInfracao(campoId, situacaoNova);
				Validacao.Add(Mensagem.FiscalizacaoConfiguracao.CampoSituacaoAlterada);
			}
		}

		public void AlterarSituacaoRespostaInfracao(int RespostaId, int situacaoNova)
		{
			List<String> configAssociadas = _da.ObterIdsPerguntasAssociadasRespostaInfracao(RespostaId);

			if (configAssociadas.Count > 0 && situacaoNova == 0)
			{
				Validacao.Add(Mensagem.FiscalizacaoConfiguracao.RespostaInfracaoNaoPodeDesativar(Mensagem.Concatenar(configAssociadas)));
			}
			else
			{
				_da.AlterarSituacaoRespostaInfracao(RespostaId, situacaoNova);
				Validacao.Add(Mensagem.FiscalizacaoConfiguracao.RespostaSituacaoAlterada);
			}
		}

		public void AlterarSituacaoPerguntaInfracao(int perguntaId, int situacaoNova)
		{
			List<String> configAssociadas = _da.ObterIdsConfiguracoesAssociadasPerguntaInfracao(perguntaId);

			if (configAssociadas.Count > 0 && situacaoNova == 0)
			{
				Validacao.Add(Mensagem.FiscalizacaoConfiguracao.PerguntaInfracaoNaoPodeDesativar(Mensagem.Concatenar(configAssociadas)));
			}
			else
			{
				_da.AlterarSituacaoPerguntaInfracao(perguntaId, situacaoNova);
				Validacao.Add(Mensagem.FiscalizacaoConfiguracao.PerguntaNumSituacaoAlterada(perguntaId.ToString()));
			}
		}


		public bool PodeEditarTipoInfracao(int id)
		{
			if (_da.TipoIsAssociadoFiscalizacao(id))
			{
				Validacao.Add(Mensagem.FiscalizacaoConfiguracao.EditarTipoInfracaoJaAssociado);
			}
			else
			{
				List<String> ConfiguracoesAssociadas = _da.ObterIdsConfiguracoesAssociadasTipoInfracao(id);
				if (ConfiguracoesAssociadas.Count > 0)
				{
					Validacao.Add(Mensagem.FiscalizacaoConfiguracao.TipoInfracaoNaoPodeEditar(Mensagem.Concatenar(ConfiguracoesAssociadas)));
				}
			}

			return Validacao.EhValido;

		}

		public bool PodeEditarItemInfracao(int id)
		{
			if (_da.ItemIsAssociadoFiscalizacao(id))
			{
				Validacao.Add(Mensagem.FiscalizacaoConfiguracao.EditarItemInfracaoJaAssociado);
			}
			else
			{
				List<String> ConfiguracoesAssociadas = _da.ObterIdsConfiguracoesAssociadasItemInfracao(id);
				if (ConfiguracoesAssociadas.Count > 0)
				{
					Validacao.Add(Mensagem.FiscalizacaoConfiguracao.ItemInfracaoNaoPodeEditar(Mensagem.Concatenar(ConfiguracoesAssociadas)));
				}
			}

			return Validacao.EhValido;

		}

		public bool PodeEditarSubItemInfracao(int id)
		{
			if (_da.SubItemIsAssociadoFiscalizacao(id))
			{
				Validacao.Add(Mensagem.FiscalizacaoConfiguracao.EditarSubItemInfracaoJaAssociado);
			}
			else
			{
				List<String> ConfiguracoesAssociadas = _da.ObterIdsConfiguracoesAssociadasSubItemInfracao(id);
				if (ConfiguracoesAssociadas.Count > 0)
				{
					Validacao.Add(Mensagem.FiscalizacaoConfiguracao.SubItemInfracaoNaoPodeEditar(Mensagem.Concatenar(ConfiguracoesAssociadas)));
				}
			}

			return Validacao.EhValido;

		}

		public bool PodeEditarCampoInfracao(int id)
		{
			if (_da.CampoIsAssociadoFiscalizacao(id))
			{
				Validacao.Add(Mensagem.FiscalizacaoConfiguracao.EditarCampoInfracaoJaAssociado);
			}
			else
			{
				List<String> ConfiguracoesAssociadas = _da.ObterIdsConfiguracoesAssociadasCampoInfracao(id);
				if (ConfiguracoesAssociadas.Count > 0)
				{
					Validacao.Add(Mensagem.FiscalizacaoConfiguracao.CampoInfracaoNaoPodeEditar(Mensagem.Concatenar(ConfiguracoesAssociadas)));
				}
			}

			return Validacao.EhValido;

		}

		public bool PodeEditarRespostaInfracao(int id)
		{
			if (_da.RespostaIsAssociadoFiscalizacao(id))
			{
				Validacao.Add(Mensagem.FiscalizacaoConfiguracao.EditarRespostaInfracaoJaAssociado);
			}
			else
			{
				List<String> PerguntasAssociadas = _da.ObterIdsPerguntasAssociadasRespostaInfracao(id);
				if (PerguntasAssociadas.Count > 0)
				{
					Validacao.Add(Mensagem.FiscalizacaoConfiguracao.RespostaInfracaoNaoPodeEditar(Mensagem.Concatenar(PerguntasAssociadas)));
				}
			}

			return Validacao.EhValido;

		}


		#endregion
	}
}
