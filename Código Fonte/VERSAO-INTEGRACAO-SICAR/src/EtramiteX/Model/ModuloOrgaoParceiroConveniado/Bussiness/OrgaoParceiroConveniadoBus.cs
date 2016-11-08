using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloOrgaoParceiroConveniado;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloOrgaoParceiroConveniado.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloOrgaoParceiroConveniado.Bussiness
{
	public class OrgaoParceiroConveniadoBus
	{
		#region Propriedades

		private OrgaoParceiroConveniadoValidar _validar = new OrgaoParceiroConveniadoValidar();
		private OrgaoParceiroConveniadoDa _da = new OrgaoParceiroConveniadoDa();
		private CredenciadoBus _busCredenciado = new CredenciadoBus();
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
		public String UsuarioCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}
		#endregion

		#region Comandos DML

		public void Salvar(OrgaoParceiroConveniado orgaoParceiro)
		{
			try
			{
				using (BancoDeDados banco = BancoDeDados.ObterInstancia(UsuarioCredenciado))
				{
					if (!_validar.Salvar(orgaoParceiro, banco))
					{
						return;
					}
				}

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
				{
					bancoDeDados.IniciarTransacao();

					_da.Salvar(orgaoParceiro, bancoDeDados);

					bancoDeDados.Commit();

					Validacao.Add(Mensagem.OrgaoParceiroConveniado.SalvarOrgaoParceiroConveniado(""));
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void AlterarSituacao(OrgaoParceiroConveniado orgao)
		{
			try
			{
				if (!_validar.AlterarSituacao(orgao))
				{
					return;
				}

				List<CredenciadoPessoa> credenciadosOrgao = new List<CredenciadoPessoa>();

				switch ((eOrgaoParceiroConveniadoSituacao)orgao.SituacaoId)
				{
					case eOrgaoParceiroConveniadoSituacao.Ativo:
						orgao.SituacaoMotivo = string.Empty;
						break;

					case eOrgaoParceiroConveniadoSituacao.Bloqueado:
						credenciadosOrgao = ObterCredenciados(orgao.Id, 0).Where(x => x.Situacao != (int)eCredenciadoSituacao.Bloqueado && x.Situacao != (int)eCredenciadoSituacao.Cadastrado).ToList();
						Validacao.Erros.Clear();
						break;
				}

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
				{
					bancoDeDados.IniciarTransacao();

					_da.AlterarSituacao(orgao, bancoDeDados);

					if (credenciadosOrgao.Count > 0)
					{
						using (BancoDeDados bancoDeDadosCredenciado = BancoDeDados.ObterInstancia(UsuarioCredenciado))
						{
							foreach (CredenciadoPessoa credenciado in credenciadosOrgao)
							{
								_busCredenciado.AlterarSituacao(credenciado.Id, "", (int)eCredenciadoSituacao.Bloqueado, "Órgão Parceiro/ Conveniado Bloqueado", bancoDeDadosCredenciado);

								if (!Validacao.EhValido)
								{
									break;
								}
							}

							if (!Validacao.EhValido)
							{
								bancoDeDadosCredenciado.Rollback();
							}
						}
					}

					if (!Validacao.EhValido)
					{
						bancoDeDados.Rollback();
						return;
					}

					bancoDeDados.Commit();
				}

				Validacao.Add(Mensagem.OrgaoParceiroConveniado.SituacaoAlteradaSucesso);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public bool BloquearCredenciados(List<CredenciadoPessoa> credenciados)
		{
			bool retorno = true;
			try
			{
				if (!_validar.VerificarSituacao(credenciados.First().OrgaoParceiroId))
				{
					Validacao.Add(Mensagem.OrgaoParceiroConveniado.OrgaoParceiroBloqueado(Obter(credenciados[0].OrgaoParceiroId).SiglaNome));
					return false;
				}

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
				{
					_validar.VerificarCredenciadoAssociadoOrgao(credenciados, bancoDeDados);

					foreach (CredenciadoPessoa credenciado in credenciados)
					{
						credenciado.Situacao = (int)eCredenciadoSituacao.Bloqueado;

						_busCredenciado.AlterarSituacao(credenciado.Id, string.Empty, credenciado.Situacao, "Alterado pelo Gerenciar do interno", bancoDeDados);

						if (!Validacao.EhValido)
						{
							break;
						}
					}

					if (!Validacao.EhValido)
					{
						bancoDeDados.Rollback();
						return false;
					}

					bancoDeDados.Commit();
				}

				Validacao.Add(Mensagem.OrgaoParceiroConveniado.CredenciadosBloqueadosSucesso);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return retorno;
		}

		public bool DesbloquearCredenciados(List<CredenciadoPessoa> credenciados)
		{
			bool retorno = true;
			try
			{
				if (!_validar.VerificarSituacao(credenciados[0].OrgaoParceiroId))
				{
					Validacao.Add(Mensagem.OrgaoParceiroConveniado.OrgaoParceiroBloqueado(Obter(credenciados[0].OrgaoParceiroId).SiglaNome));
					return false;
				}

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
				{
					_validar.VerificarCredenciadoAssociadoOrgao(credenciados, bancoDeDados);
					
					foreach (CredenciadoPessoa credenciado in credenciados)
					{
						//Metodo especifico para Orgaos Parceiros
						eCredenciadoSituacao situacao = eCredenciadoSituacao.AguardandoChave;
						if (!_busCredenciado.IsCredenciadoAtivoAlgumaVez(credenciado.Id))
						{
							situacao = eCredenciadoSituacao.AguardandoAtivacao;
						}

						_busCredenciado.RegerarChave(credenciado.Id, bancoDeDados, situacao);

						if (!Validacao.EhValido)
						{
							break;
						}
					}

					if (!Validacao.EhValido)
					{
						bancoDeDados.Rollback();
						return false;
					}

					bancoDeDados.Commit();
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return retorno;
		}

		public void EnviarEmail(List<CredenciadoPessoa> credenciados, int orgaoParceiroId)
		{
			try
			{
				if (!_validar.VerificarSituacao(orgaoParceiroId))
				{
					return;
				}

				using (BancoDeDados bancoDeDadosCredenciado = BancoDeDados.ObterInstancia(UsuarioCredenciado))
				{
					_validar.VerificarCredenciadoAssociadoOrgao(credenciados, bancoDeDadosCredenciado);
				}

				if (credenciados.Count < 1)
				{
					return;
				}

				GerenciadorTransacao.ObterIDAtual();
				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
				{
					bancoDeDados.IniciarTransacao();

					foreach (CredenciadoPessoa credenciado in credenciados)
					{
						_busCredenciado.EnviarEmail(credenciado, credenciado.Email, cadastro: true, banco: bancoDeDados);
					}

					if (Validacao.EhValido)
					{
						using (BancoDeDados bancoCredenciado = BancoDeDados.ObterInstancia(UsuarioCredenciado))
						{
							bancoCredenciado.IniciarTransacao();

							foreach (CredenciadoPessoa credenciado in credenciados)
							{
								credenciado.Situacao = (int)eCredenciadoSituacao.AguardandoAtivacao;
								_busCredenciado.AlterarSituacao(credenciado, bancoCredenciado);

								if (!Validacao.EhValido)
								{
									break;
								}
							}

							if (!Validacao.EhValido)
							{
								bancoCredenciado.Rollback();
							}
							else
							{
								bancoCredenciado.Commit();
							}
						}
					}

					if (!Validacao.EhValido)
					{
						bancoDeDados.Rollback();
						return;
					}

					bancoDeDados.Commit();
					Validacao.Add(Mensagem.OrgaoParceiroConveniado.ChaveGeradaSucesso);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		#endregion

		#region Obter/ Filtrar

		public OrgaoParceiroConveniado Obter(int id)
		{
			OrgaoParceiroConveniado orgao = null;
			try
			{
				orgao = _da.Obter(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return orgao;
		}

		public List<Lista> ObterUnidadesLst(int orgao, List<Unidade> lista = null)
		{
			List<Lista> list = null;

			try
			{
				if (lista != null && lista.Count > 0)
				{
					list = new List<Lista>();
					lista.ForEach(x => list.Add(new Lista()
					{
						Texto = x.Sigla + " - " + x.Nome,
						Id = x.Id.ToString(),
						IsAtivo = true
					}));
				}
				else
				{
					list = _da.ObterUnidadesLst(orgao);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return list;
		}

		public List<Lista> ObterOrgaosParceirosLst()
		{
			List<Lista> list = null;

			try
			{
				list = _da.ObterOrgaosParceirosLst();
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return list;

		}

		public Resultados<OrgaoParceiroConveniadoListarResultados> Filtrar(OrgaoParceiroConveniadoListarFiltros filtrosListar, Paginacao paginacao)
		{
			Resultados<OrgaoParceiroConveniadoListarResultados> resultados = null;
			try
			{
				Filtro<OrgaoParceiroConveniadoListarFiltros> filtro = new Filtro<OrgaoParceiroConveniadoListarFiltros>(filtrosListar, paginacao);
				resultados = _da.Filtrar(filtro);

				if (resultados.Quantidade < 1)
				{
					Validacao.Add(Mensagem.Padrao.NaoEncontrouRegistros);
				}


			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return resultados;
		}

		public List<CredenciadoPessoa> ObterCredenciados(int idOrgaoParceiro, int idUnidade)
		{
			List<CredenciadoPessoa> retornos = null;

			try
			{
				if (!_validar.VerificarSituacao(idOrgaoParceiro))
				{
					Validacao.Add(Mensagem.OrgaoParceiroConveniado.OrgaoParceiroBloqueado(Obter(idOrgaoParceiro).SiglaNome));
					return null;
				}

				List<int> idsCredenciados = _busCredenciado.ObterIdsCredenciadosParceiros(idOrgaoParceiro, idUnidade);
				CredenciadoPessoa credenciado = null;
				retornos = new List<CredenciadoPessoa>();

				foreach (int id in idsCredenciados)
				{
					credenciado = _busCredenciado.Obter(id, true);
					retornos.Add(credenciado);
				}

				if (retornos.Count < 1)
				{
					Validacao.Add(Mensagem.Padrao.NaoEncontrouRegistros);
				}

			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return retornos;
		}

		#endregion

		#region Validacoes

		public void VerificarCredenciadoAssociado(Unidade unidade)
		{
			try
			{
				using (BancoDeDados banco = BancoDeDados.ObterInstancia(UsuarioCredenciado))
				{
					_validar.ExcluirUnidade(unidade, banco);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		#endregion

		#region Auxiliares

		internal static string GerarChaveAcesso(string email, string nome)
		{
			string strTexto = (email + string.Empty).ToLower() + "$" + DateTime.Now.Ticks.ToString() + "$" + nome;
			UTF8Encoding encoder = new UTF8Encoding();
			SHA512 sha512 = SHA512.Create();
			byte[] byteHash = sha512.ComputeHash(encoder.GetBytes(strTexto));

			return string.Join("", byteHash.Select(bin => bin.ToString("X2")).ToArray());
		}

		#endregion
	}
}