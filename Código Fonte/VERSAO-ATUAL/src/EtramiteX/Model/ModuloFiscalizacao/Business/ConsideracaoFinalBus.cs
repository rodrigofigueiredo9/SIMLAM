using System;
using System.Collections.Generic;
using System.Web;
using Tecnomapas.Blocos.Arquivo.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloSetor.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business
{
	public class ConsideracaoFinalBus
	{
		#region Propriedades

		ConsideracaoFinalValidar _validar = null;
		ConsideracaoFinalDa _da = new ConsideracaoFinalDa();
		FiscalizacaoDa _daFiscalizacao = new FiscalizacaoDa();
		SetorLocalizacaoBus _busSetorLocalizacao = new SetorLocalizacaoBus(new SetorLocalizacaoValidar());

		private static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		#endregion

		#region Comandos DML

		public bool Salvar(ConsideracaoFinal entidade)
		{
			try
			{
				if (_validar.Salvar(entidade))
				{
					if (entidade.Id < 1)
					{
						entidade.Id = _da.ObterID(entidade.FiscalizacaoId);
					}

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

						entidade.Testemunhas.ForEach(x => 
						{
							x.TestemunhaEndereco = x.TestemunhaIDAF.GetValueOrDefault() ? string.Empty : x.TestemunhaEndereco;
						});

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

		public ConsideracaoFinal Obter(int id, BancoDeDados banco = null) 
		{
			ConsideracaoFinal entidade = null;

			try
			{
				entidade = _da.Obter(id);
				entidade.Testemunhas.ForEach(x => 
				{
					if (x.TestemunhaIDAF.Value)
					{
						var setorLocalizacao = _busSetorLocalizacao.Obter(x.TestemunhaSetorId.Value);
						x.TestemunhaEndereco = setorLocalizacao.FormatarEndereco();
					}
				});
				
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return entidade;
		}

		public List<PessoaLst> ObterFuncionarios()
		{
			return _da.ObterFuncionarios();
		}

		#endregion 

		public ConsideracaoFinalBus()
		{
			_validar = new ConsideracaoFinalValidar();
		}

		public ConsideracaoFinalBus(ConsideracaoFinalValidar validar)
		{
			_validar = validar;
		}
	}
}