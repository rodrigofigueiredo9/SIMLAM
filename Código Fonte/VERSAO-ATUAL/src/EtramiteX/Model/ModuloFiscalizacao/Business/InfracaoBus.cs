using System;
using System.Collections.Generic;
using System.Web;
using Tecnomapas.Blocos.Arquivo.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business
{
	public class InfracaoBus
	{
		#region Propriedades

		InfracaoValidar _validar = null;
		InfracaoDa _da = new InfracaoDa();

		private static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		#endregion

		public InfracaoBus()
		{
			_validar = new InfracaoValidar();
		}

		public InfracaoBus(InfracaoValidar validar)
		{
			_validar = validar;
		}

		#region Comandos DML

		public bool Salvar(Infracao entidade)
		{
			try
			{
				if (_validar.Salvar(entidade))
				{
					if (entidade.Id < 1)
					{
						entidade.Id = _da.ObterID(entidade.FiscalizacaoId);
					}

					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.Salvar(entidade, bancoDeDados);

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

		#endregion

		#region Obter

		public Infracao Obter(int fiscalizacaoId, bool isVerificarConfig = false, BancoDeDados banco = null)
		{
			Infracao entidadeAtual = new Infracao();
			Infracao entidadeNovoConfig = new Infracao();

			try
			{
				entidadeAtual = _da.Obter(fiscalizacaoId, banco);

				entidadeAtual.ConfigAlterou = _da.ConfigAlterada(entidadeAtual.ConfiguracaoId, entidadeAtual.ConfiguracaoTid, banco);
				entidadeAtual.ConfigAlterou = _da.PerguntaRespostaAlterada(entidadeAtual.Id) || entidadeAtual.ConfigAlterou;


				if (isVerificarConfig)
				{
					if (entidadeAtual.ConfigAlterou)
					{
						entidadeNovoConfig = _da.ObterConfig(entidadeAtual.ConfiguracaoId, banco);
						Mergear(entidadeAtual, entidadeNovoConfig);
					}
				}
				else
				{
					if (entidadeAtual.ConfigAlterou)
					{
						entidadeAtual = _da.ObterHistorico(entidadeAtual.Id, banco);
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return entidadeAtual;
		}

		public Infracao ObterHistoricoPorFiscalizacao(int fiscalizacaoId, BancoDeDados banco = null)
		{
			return _da.ObterHistoricoPorFiscalizacao(fiscalizacaoId, banco);
		}

        public bool PossuiIUFBloco(int fiscalizacaoId, BancoDeDados banco = null)
        {
            bool retorno = false;

            try
            {
                retorno = _da.PossuiIUFBloco(fiscalizacaoId, banco);
            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
            }

            return retorno;
        }

		#endregion

		public void Mergear(Infracao entidadeAtual, Infracao entidadeNovoConfig)
		{
			if (entidadeAtual.ClassificacaoId != entidadeNovoConfig.ClassificacaoId)
			{
				entidadeAtual.Campos = new List<InfracaoCampo>();
				entidadeAtual.Perguntas = new List<InfracaoPergunta>();
				entidadeAtual.ClassificacaoId = 0;
				entidadeAtual.TipoId = 0;
				entidadeAtual.ItemId = 0;
				return;				
			}
			else if (entidadeAtual.TipoId != entidadeNovoConfig.TipoId)
			{
				entidadeAtual.Campos = new List<InfracaoCampo>();
				entidadeAtual.Perguntas = new List<InfracaoPergunta>();
				entidadeAtual.TipoId = 0;
				entidadeAtual.ItemId = 0;
				return;				
			}
			else if (entidadeAtual.ItemId != entidadeNovoConfig.ItemId)
			{
				entidadeAtual.Campos = new List<InfracaoCampo>();
				entidadeAtual.Perguntas = new List<InfracaoPergunta>();
				entidadeAtual.ItemId = 0;
				return;				
			}

			entidadeNovoConfig.Campos.ForEach(x => 
			{
				var itemCampo = entidadeAtual.Campos.Find(z => z.CampoId == x.CampoId);

				if (itemCampo != null)
				{
					x.Id = itemCampo.Id;
					x.Identificacao = itemCampo.Identificacao;
					x.Texto = itemCampo.Texto;
					x.Tipo = itemCampo.Tipo;
					x.TipoTexto = itemCampo.TipoTexto;
					x.Unidade = itemCampo.Unidade;
					x.UnidadeTexto = itemCampo.UnidadeTexto;
				}
			});

			entidadeNovoConfig.Perguntas.ForEach(x =>
			{
				var itemPergunta = entidadeAtual.Perguntas.Find(z => z.PerguntaId == x.PerguntaId);

				if (itemPergunta != null)
				{
					x.Id = itemPergunta.Id;
					x.Especificacao = itemPergunta.Especificacao;
					x.Identificacao = itemPergunta.Identificacao;
					x.IsEspecificar = itemPergunta.IsEspecificar;
					x.RespostaId = itemPergunta.RespostaId;
					x.Respostas = itemPergunta.Respostas;
				}
			});

			entidadeAtual.Campos = entidadeNovoConfig.Campos;
			entidadeAtual.Perguntas = entidadeNovoConfig.Perguntas;
			entidadeAtual.ConfiguracaoTid = entidadeNovoConfig.ConfiguracaoTid;
		}

		public bool ConfigAlterada(int configuracaoId, string tid, BancoDeDados banco = null)
		{
			var configAlterada = false;
			try
			{
				configAlterada = _da.ConfigAlterada(configuracaoId, tid, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return configAlterada;
		}
	}
}
