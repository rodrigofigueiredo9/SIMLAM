using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao.Configuracoes;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Entities;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business
{
	public class ConfigFiscalizacaoValidar
	{
		ConfigFiscalizacaoDa _da = new ConfigFiscalizacaoDa();

		#region Tipo

		public bool SalvarTipoInfracao(Item tipoInfracao)
		{
			return Validacao.EhValido;
		}

		public bool ExcluirTipoInfracao(int id) 
		{
			if (!_da.TipoIsAtivo(id))
			{
				Validacao.Add(Mensagem.FiscalizacaoConfiguracao.ExcluirTipoInfracaoDesativado);
			}else
			{
				if (_da.TipoIsAssociadoFiscalizacao(id))
				{
					Validacao.Add(Mensagem.FiscalizacaoConfiguracao.ExcluirTipoInfracaoJaAssociado);
				}
				else
				{
					List<String> ConfiguracoesAssociadas = _da.ObterIdsConfiguracoesAssociadasTipoInfracao(id);
					if (ConfiguracoesAssociadas.Count > 0)
					{
						Validacao.Add(Mensagem.FiscalizacaoConfiguracao.TipoInfracaoNaoPodeExcluir(Mensagem.Concatenar(ConfiguracoesAssociadas)));
					}
				}
			}

			return Validacao.EhValido;
		}

		#endregion

		#region Item

		public bool SalvarItemInfracao(Item itemInfracao)
		{
			return Validacao.EhValido;
		}

		public bool ExcluirItemInfracao(int id)
		{
			if (!_da.ItemIsAtivo(id))
			{
				Validacao.Add(Mensagem.FiscalizacaoConfiguracao.ExcluirItemInfracaoDesativado);
			}
			else
			{
				if (_da.ItemIsAssociadoFiscalizacao(id))
				{
					Validacao.Add(Mensagem.FiscalizacaoConfiguracao.ExcluirItemInfracaoJaAssociado);
				}
				else
				{
					List<String> ConfiguracoesAssociadas = _da.ObterIdsConfiguracoesAssociadasItemInfracao(id);
					if (ConfiguracoesAssociadas.Count > 0)
					{
						Validacao.Add(Mensagem.FiscalizacaoConfiguracao.ItemInfracaoNaoPodeExcluir(Mensagem.Concatenar(ConfiguracoesAssociadas)));
					}
				}
			}

			return Validacao.EhValido;
		}

		#endregion

		#region SubItem

		public bool SalvarSubItemInfracao(Item subItem)
		{
			return Validacao.EhValido;
		}

		public bool ExcluirSubItemInfracao(int id)
		{
			if (!_da.SubitemIsAtivo(id))
			{
				Validacao.Add(Mensagem.FiscalizacaoConfiguracao.ExcluirSubItemInfracaoDesativado);
			}
			else
			{
				if (_da.SubItemIsAssociadoFiscalizacao(id))
				{
					Validacao.Add(Mensagem.FiscalizacaoConfiguracao.ExcluirSubItemInfracaoJaAssociado);
				}
				else
				{
					List<String> ConfiguracoesAssociadas = _da.ObterIdsConfiguracoesAssociadasSubItemInfracao(id);
					if (ConfiguracoesAssociadas.Count > 0)
					{
						Validacao.Add(Mensagem.FiscalizacaoConfiguracao.SubItemInfracaoNaoPodeExcluir(Mensagem.Concatenar(ConfiguracoesAssociadas)));
					}
				}
			}

			return Validacao.EhValido;
		}

		#endregion

        #region Penalidade
        public bool SalvarPenalidade(Penalidade penalidade)
        {
            return Validacao.EhValido;
        }


        #endregion

        #region Campo

        public bool SalvarCampoInfracao(Item campo)
		{
			return Validacao.EhValido;
		}

		public bool ExcluirCampoInfracao(int id)
		{
			if (!_da.CampoIsAtivo(id))
			{
				Validacao.Add(Mensagem.FiscalizacaoConfiguracao.ExcluirCampoInfracaoDesativado);
			}
			else
			{
				if (_da.CampoIsAssociadoFiscalizacao(id))
				{
					Validacao.Add(Mensagem.FiscalizacaoConfiguracao.ExcluirCampoInfracaoJaAssociado);
				}
				else
				{
					List<String> ConfiguracoesAssociadas = _da.ObterIdsConfiguracoesAssociadasCampoInfracao(id);
					if (ConfiguracoesAssociadas.Count > 0)
					{
						Validacao.Add(Mensagem.FiscalizacaoConfiguracao.CampoInfracaoNaoPodeExcluir(Mensagem.Concatenar(ConfiguracoesAssociadas)));
					}
				}
			}

			return Validacao.EhValido;
		}

		#endregion

		#region Pergunta

		public bool SalvarPerguntaInfracao(PerguntaInfracao pergunta)
		{
			if (String.IsNullOrWhiteSpace(pergunta.Texto)) 
			{
				Validacao.Add(Mensagem.FiscalizacaoConfiguracao.PerguntaNomeObrigatorio);
			}

			if (pergunta.Respostas.Count <= 0) 
			{
				Validacao.Add(Mensagem.FiscalizacaoConfiguracao.RespostaListaObrigatoria);
			}

			return Validacao.EhValido;
		}

		public bool ExcluirPerguntaInfracao(int id)
		{
			if (!_da.PerguntaIsAtivo(id))
			{
				Validacao.Add(Mensagem.FiscalizacaoConfiguracao.ExcluirPerguntaInfracaoDesativado);
			}
			else
			{
				if (_da.PerguntaIsAssociadoFiscalizacao(id))
				{
					Validacao.Add(Mensagem.FiscalizacaoConfiguracao.ExcluirPerguntaInfracaoJaAssociado);
				}
				else
				{
					List<String> ConfiguracoesAssociadas = _da.ObterIdsConfiguracoesAssociadasPerguntaInfracao(id);
					if (ConfiguracoesAssociadas.Count > 0)
					{
						Validacao.Add(Mensagem.FiscalizacaoConfiguracao.PerguntaInfracaoNaoPodeExcluir(Mensagem.Concatenar(ConfiguracoesAssociadas)));
					}
				}
			}

			return Validacao.EhValido;
		}

		#endregion

		#region Resposta

		public bool SalvarRespostaInfracao(Item subItem)
		{
			return Validacao.EhValido;
		}

		public bool ExcluirRespostaInfracao(int id)
		{
			if (!_da.RespostaIsAtivo(id))
			{
				Validacao.Add(Mensagem.FiscalizacaoConfiguracao.ExcluirRespostaInfracaoDesativado);
			}
			else
			{
				if (_da.RespostaIsAssociadoFiscalizacao(id))
				{
					Validacao.Add(Mensagem.FiscalizacaoConfiguracao.ExcluirRespostaInfracaoJaAssociado);
				}
				else
				{
					List<String> ConfiguracoesAssociadas = _da.ObterIdsPerguntasAssociadasRespostaInfracao(id);
					if (ConfiguracoesAssociadas.Count > 0)
					{
						Validacao.Add(Mensagem.FiscalizacaoConfiguracao.RespostaInfracaoNaoPodeExcluir(Mensagem.Concatenar(ConfiguracoesAssociadas)));
					}
				}
			}

			return Validacao.EhValido;
		}

		#endregion

        #region Produtos Apreendidos / Destinação

        public bool SalvarProdutosApreendidos(List<ProdutoApreendido> listaProdutos)
        {
            if (listaProdutos == null)
            {
                return Validacao.EhValido;
            }

            foreach (var item in listaProdutos)
            {
                if (item.Excluir == false)
                {
                    if (String.IsNullOrWhiteSpace(item.Item))
                    {
                        Validacao.Add(Mensagem.FiscalizacaoConfiguracao.ItemProdutoObrigatorio);
                    }

                    if (String.IsNullOrWhiteSpace(item.Unidade))
                    {
                        Validacao.Add(Mensagem.FiscalizacaoConfiguracao.UnidadeProdutoObrigatoria);
                    }
                }
            }


            return Validacao.EhValido;
        }

        public bool SalvarDestinacao(List<DestinacaoProduto> listaDestinacao)
        {
            if (listaDestinacao == null)
            {
                return Validacao.EhValido;
            }

            foreach (var item in listaDestinacao)
            {
                if (item.Excluir == false)
                {
                    if (String.IsNullOrWhiteSpace(item.Destino))
                    {
                        Validacao.Add(Mensagem.FiscalizacaoConfiguracao.DestinoObrigatorio);
                    }
                }
            }


            return Validacao.EhValido;
        }

        #endregion Produtos Apreendidos / Destinação

        #region Códigos da Receita

        public bool SalvarCodigosReceita(List<CodigoReceita> listaCodigosReceita)
        {
            if (listaCodigosReceita == null)
            {
                return Validacao.EhValido;
            }

            foreach (var item in listaCodigosReceita)
            {
                if (item.Excluir == false)
                {
                    if (String.IsNullOrWhiteSpace(item.Codigo))
                    {
                        Validacao.Add(Mensagem.FiscalizacaoConfiguracao.CodigoReceitaObrigatorio);
                    }

                    //Não estou verificando a descrição para não obrigar que os códigos já cadastrados sejam todos editados
                    //if (String.IsNullOrWhiteSpace(item.Descricao))
                    //{
                    //    Validacao.Add(Mensagem.FiscalizacaoConfiguracao.DescricaoCodigoObrigatoria);
                    //}
                }
            }


            return Validacao.EhValido;
        } 
 

        #endregion Códigos da Receita

        public bool Salvar(ConfigFiscalizacao configuracao)
		{
			Mensagem msg = null;
			List<string> lstMsg = new List<string>();

			if (configuracao.ClassificacaoId == 0)
			{
				Validacao.Add(Mensagem.FiscalizacaoConfiguracao.ClassificacaoObrigatorio);
			}

			if (configuracao.TipoId == 0)
			{
				Validacao.Add(Mensagem.FiscalizacaoConfiguracao.TipoObrigatorio);
			}
			else if (!_da.TipoIsAtivo(configuracao.TipoId))
			{
				msg = Mensagem.FiscalizacaoConfiguracao.ItemDesativado("Tipo de infração", configuracao.TipoTexto);
				msg.Campo = "Configuracao_Tipo";
				Validacao.Add(msg);
			}

			if (configuracao.ItemId == 0)
			{
				Validacao.Add(Mensagem.FiscalizacaoConfiguracao.ItemObrigatorio);				
			} 
			else if (!_da.ItemIsAtivo(configuracao.ItemId))
			{
				msg = Mensagem.FiscalizacaoConfiguracao.ItemDesativado("Item", configuracao.ItemTexto);
				msg.Campo = "Configuracao_Item";
				Validacao.Add(msg);
			}

			if (configuracao.Id == 0)
			{
				var config = _da.Obter(configuracao.ClassificacaoId, configuracao.TipoId, configuracao.ItemId);

				if (config.GetValue<int>("Id") > 0)
				{
					Validacao.Add(Mensagem.FiscalizacaoConfiguracao.ConfiguracaoCadastrada);
				}
			}

			configuracao.Subitens.ForEach(x => 
			{
				if (!_da.SubitemIsAtivo(x.SubItemId))
				{
					lstMsg.Add(x.SubItemTexto);
				}				
			});

			if (lstMsg.Count == 1)
			{
				msg = Mensagem.FiscalizacaoConfiguracao.ItemDesativado("Subitem", lstMsg[0]);
				msg.Campo = "fsSubitens";
				Validacao.Add(msg);
			}
			else if (lstMsg.Count > 1)
			{
				msg = Mensagem.FiscalizacaoConfiguracao.ItemDesativado("Subitens", Mensagem.Concatenar(lstMsg), true);
				msg.Campo = "fsSubitens";
				Validacao.Add(msg);
			}

			lstMsg = new List<string>();

			configuracao.Perguntas.ForEach(x => 
			{
				if (!_da.PerguntaIsAtivo(x.PerguntaId))
				{
					lstMsg.Add(x.PerguntaTexto);
				}
			});

			if (lstMsg.Count == 1)
			{
				msg = Mensagem.FiscalizacaoConfiguracao.ItemDesativado("Pergunta", lstMsg[0]);
				msg.Campo = "fsPerguntas";
				Validacao.Add(msg);
			}
			else if (lstMsg.Count > 1)
			{
				msg = Mensagem.FiscalizacaoConfiguracao.ItemDesativado("Perguntas", Mensagem.Concatenar(lstMsg), true);
				msg.Campo = "fsPerguntas";
				Validacao.Add(msg);
			}

			lstMsg = new List<string>();

			configuracao.Campos.ForEach(x => 
			{
				if (!_da.CampoIsAtivo(x.CampoId))
				{
					lstMsg.Add(x.CampoTexto);
				}
			});

			if (lstMsg.Count == 1)
			{
				msg = Mensagem.FiscalizacaoConfiguracao.ItemDesativado("Campo", lstMsg[0]);
				msg.Campo = "fsCampos";
				Validacao.Add(msg);
			}
			else if (lstMsg.Count > 1)
			{
				msg = Mensagem.FiscalizacaoConfiguracao.ItemDesativado("Campos", Mensagem.Concatenar(lstMsg), true);
				msg.Campo = "fsCampos";
				Validacao.Add(msg);
			}

			return Validacao.EhValido;
		}

		public bool Excluir(int configuracaoId)
		{
			var lstFiscalizacao = _da.ConfiguracaoEmUso(configuracaoId);

			if (lstFiscalizacao.Count > 0)
			{
				if (lstFiscalizacao.Count == 1)
				{
					Validacao.Add(Mensagem.FiscalizacaoConfiguracao.ExcluirInvalidoSingular(Mensagem.Concatenar(lstFiscalizacao)));
				}
				else
				{
					Validacao.Add(Mensagem.FiscalizacaoConfiguracao.ExcluirInvalidoPlural(Mensagem.Concatenar(lstFiscalizacao)));
				}				
			}
			return Validacao.EhValido;
		}
	}
}
