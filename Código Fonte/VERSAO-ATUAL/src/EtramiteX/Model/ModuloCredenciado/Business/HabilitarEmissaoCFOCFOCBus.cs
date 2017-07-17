using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Arquivo.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloCredenciado.Data;
using Cred = Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;


namespace Tecnomapas.EtramiteX.Interno.Model.ModuloCredenciado.Business
{
	public class HabilitarEmissaoCFOCFOCBus
	{
		#region Propriedades

		HabilitarEmissaoCFOCFOCValidar _validar = new HabilitarEmissaoCFOCFOCValidar();

		HabilitarEmissaoCFOCFOCDa _da = new HabilitarEmissaoCFOCFOCDa();
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		private static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		#endregion

		#region Ações de DML

		public HabilitarEmissaoCFOCFOCBus(string Esquema = null)
		{
			_da = new HabilitarEmissaoCFOCFOCDa(Esquema);
		}

		public HabilitarEmissaoCFOCFOCBus(HabilitarEmissaoCFOCFOCValidar pessoaValidar, string Esquema = null)
		{
			_validar = pessoaValidar;
			_da = new HabilitarEmissaoCFOCFOCDa(Esquema);
		}

		public bool Salvar(HabilitarEmissaoCFOCFOC habilitar)
		{
			try
			{
				if (_validar.Salvar(habilitar))
				{
					#region Arquivos/Diretorio
					ArquivoBus _busArquivo = new ArquivoBus(eExecutorTipo.Interno);

					if (habilitar.Arquivo != null && !String.IsNullOrWhiteSpace(habilitar.Arquivo.TemporarioNome))
					{
						if (habilitar.Arquivo.Id == 0)
						{
							habilitar.Arquivo = _busArquivo.Copiar(habilitar.Arquivo);
						}
					}
					#endregion

					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						#region Arquivos/Banco
						ArquivoDa arquivoDa = new ArquivoDa();

						if (habilitar.Arquivo != null && !String.IsNullOrWhiteSpace(habilitar.Arquivo.TemporarioNome))
						{
							if (habilitar.Arquivo.Id == 0)
							{
								arquivoDa.Salvar(habilitar.Arquivo, User.FuncionarioId, User.Name, User.Login, (int)eExecutorTipo.Interno, User.FuncionarioTid, bancoDeDados);
							}
						}
						#endregion

						_da.Salvar(habilitar, bancoDeDados);

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

		public bool ValidarPraga(HabilitarEmissaoCFOCFOC habilitar, PragaHabilitarEmissao praga)
		{
			try
			{
				_validar.VerificarAdicionarPraga(habilitar, praga);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return Validacao.EhValido;
		}

		public bool AlterarSituacao(HabilitarEmissaoCFOCFOC habilitar)
		{
			try
			{
                if (habilitar.Situacao == 1 && !_da.ValidarPodeAtivar(habilitar.Id))
                {
                    Validacao.Add(Mensagem.HabilitarEmissaoCFOCFOC.SituacaoAnteriorVigente);
                    return Validacao.EhValido;
                }
				if (_validar.AlterarSituacao(habilitar))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.AlterarSituacao(habilitar, bancoDeDados);

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

		public bool RenovarData(PragaHabilitarEmissao praga)
		{
			try
			{
				_validar.RenovarData(praga);
				return Validacao.EhValido;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return Validacao.EhValido;
		}

		#endregion

		#region Obter/Filtrar

		public Resultados<Cred.ListarFiltro> Filtrar(Cred.ListarFiltro filtrosListar, Paginacao paginacao)
		{
			try
			{
				var filtros = new Filtro<Cred.ListarFiltro>(filtrosListar, paginacao);
				var resultados = _da.Filtrar(filtros);

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

		public HabilitarEmissaoCFOCFOC Obter(int id, Boolean isCredenciado = false, Boolean isEditar = false)
		{
			HabilitarEmissaoCFOCFOC retorno = _da.Obter(id, isCredenciado: isCredenciado);

			if (retorno == null && isCredenciado)
			{
				retorno = ObterCredenciado(id, isCredenciado);
			}

			if (retorno != null && retorno.Responsavel.Id > 0)
			{
				CredenciadoBus _busCredenciado = new CredenciadoBus();

				var pessoa = _busCredenciado.Obter(retorno.Responsavel.Pessoa.Fisica.CPF);

				retorno.Responsavel.Pessoa = pessoa.Pessoa;

				if (_da.ValidarResponsavelHabilitado(retorno.Id) && !isEditar && !isCredenciado)
				{
					Validacao.Add(Mensagem.HabilitarEmissaoCFOCFOC.ResponsavelHabilitado);
				}

				if (retorno.Responsavel.Pessoa.Fisica.Profissao.Id == 0 || String.IsNullOrWhiteSpace(retorno.Responsavel.Pessoa.Fisica.Profissao.Registro))
				{
					Validacao.Add(Mensagem.HabilitarEmissaoCFOCFOC.SemProfissaoRegistro);
				}

				return retorno;
			}
			else
			{
				Validacao.Add(Mensagem.HabilitarEmissaoCFOCFOC.CpfNaoCadastrado);
			}

			return retorno;
		}

        public List<HistoricoEmissaoCFOCFOC> ObterHistoricoHabilitacoes(int id)
        {
            List<HistoricoEmissaoCFOCFOC> retorno = _da.ObterHistoricoHabilitacoes(id);

            return retorno;
        }

		#endregion

		public HabilitarEmissaoCFOCFOC ObterCredenciado(int credenciadoId, Boolean isCredenciado = false)
		{
			HabilitarEmissaoCFOCFOC retorno = null;

			try
			{
				retorno = _da.ObterCredenciado(credenciadoId);
				if ((retorno == null || retorno.Id < 1) && !isCredenciado)
				{
					Validacao.Add(Mensagem.HabilitarEmissaoCFOCFOC.NaoExisteHabilitacaoParaEsteCredenciado);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return retorno;
		}

		public HabilitarEmissaoCFOCFOC ObterPorCredenciado(int credenciadoId, bool simplificado = false)
		{
			HabilitarEmissaoCFOCFOC retorno = null;

			try
			{
				retorno = _da.ObterPorCredenciado(credenciadoId, simplificado);
				if (retorno == null || retorno.Id < 1)
				{
					Validacao.Add(Mensagem.HabilitarEmissaoCFOCFOC.NaoExisteHabilitacaoParaEsteCredenciado);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return retorno;
		}
	}
}