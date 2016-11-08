using System;
using System.Web;
using Tecnomapas.Blocos.Arquivo.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV.HabilitacaoEmissao;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFuncionario.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloPTV.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloPTV.Business
{
	public class HabilitacaoEmissaoPTVBus
	{
		#region Propriedades

		HabilitacaoEmissaoPTVDa _da = new HabilitacaoEmissaoPTVDa();
		HabilitacaoEmissaoPTVValidar _validar = new HabilitacaoEmissaoPTVValidar();
		ArquivoBus _busArquivo = new ArquivoBus(eExecutorTipo.Interno);
		FuncionarioBus _busFuncionario = new FuncionarioBus();
		private static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		#endregion

		public void Salvar(HabilitacaoEmissaoPTV habilitacao)
		{
			try
			{
				if (!_validar.Salvar(habilitacao))
				{
					return;
				}

				habilitacao.SituacaoId = (int)eHabilitacaoEmissaoPTV.Ativo;
				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados banco = BancoDeDados.ObterInstancia())
				{
					banco.IniciarTransacao();

					#region Arquivo

					if (habilitacao.Arquivo != null)
					{
						if (!string.IsNullOrWhiteSpace(habilitacao.Arquivo.Nome))
						{
							if (habilitacao.Arquivo.Id != null && habilitacao.Arquivo.Id == 0)
							{
								ArquivoBus _busArquivo = new ArquivoBus(eExecutorTipo.Interno);
								habilitacao.Arquivo = _busArquivo.Copiar(habilitacao.Arquivo);
							}

							if (habilitacao.Arquivo.Id == 0)
							{
								ArquivoDa _arquivoDa = new ArquivoDa();
								_arquivoDa.Salvar(habilitacao.Arquivo, User.FuncionarioId, User.Name, User.Login, (int)eExecutorTipo.Interno, User.FuncionarioTid, banco);
							}
						}
						else
						{
							habilitacao.Arquivo.Id = null;
						}
					}
					else
					{
						habilitacao.Arquivo = new Blocos.Arquivo.Arquivo();
					}

					#endregion

					_da.Salvar(habilitacao, banco);

					Validacao.Add(Mensagem.HabilitacaoEmissaoPTV.SalvoSucesso);

					banco.Commit();
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void AlterarSituacao(int id, int situacao, BancoDeDados banco = null)
		{
			try
			{
				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
				{
					bancoDeDados.IniciarTransacao();

					_da.AlterarSituacao(id, situacao, bancoDeDados);

					if (situacao.Equals(1))
					{
						Validacao.Add(Mensagem.HabilitacaoEmissaoPTV.Ativada);
					}
					else if (situacao.Equals(0))
					{
						Validacao.Add(Mensagem.HabilitacaoEmissaoPTV.Desativada);
					}

					bancoDeDados.Commit();
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public Resultados<HabilitacaoEmissaoPTVFiltro> Filtrar(HabilitacaoEmissaoPTVFiltro filtrosListar, Paginacao paginacao)
		{
			try
			{
				var filtros = new Filtro<HabilitacaoEmissaoPTVFiltro>(filtrosListar, paginacao);
				Resultados<HabilitacaoEmissaoPTVFiltro> resultados = _da.Filtrar(filtros);

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

		public HabilitacaoEmissaoPTV Obter(int id)
		{
			HabilitacaoEmissaoPTV habilitacao = null;

			try
			{
				habilitacao = _da.Obter(id);

				habilitacao.Funcionario.Setores = _busFuncionario.ObterSetoresFuncionario(habilitacao.Funcionario.Id);

				if (habilitacao.Arquivo.Id > 0)
				{
					habilitacao.Arquivo = _busArquivo.Obter(habilitacao.Arquivo.Id.GetValueOrDefault());
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return habilitacao;
		}

		public bool VerificarCPF(string cpf)
		{
			return _validar.VerificarCPF(cpf);
		}

		public string ExisteOperador(int id)	
		{
			try
			{
				string operador = _da.ExisteOperador(id);

				if(!string.IsNullOrEmpty(operador))
				{
					return operador;
				}
			}
			catch(Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}
	}
}