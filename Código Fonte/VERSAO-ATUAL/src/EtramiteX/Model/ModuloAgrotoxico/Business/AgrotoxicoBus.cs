using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.ModuloAgrotoxico;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAgrotoxico.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Arquivo.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using System.Web;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloAgrotoxico.Business
{
	public class AgrotoxicoBus
	{
		#region Propriedades

		AgrotoxicoDa _da = new AgrotoxicoDa();
		AgrotoxicoValidar _validar = new AgrotoxicoValidar();
		ListaBus _busLista = new ListaBus();

		private static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		#endregion

		#region Ações DML

		public bool Salvar(Agrotoxico agrotoxico)
		{
			try
			{
				agrotoxico.IngredientesAtivos.ForEach(item =>
				{
					if (item.UnidadeMedidaOutro != null)
					{
						item.UnidadeMedidaOutro = item.UnidadeMedidaOutro.DeixarApenasUmEspaco();
					}
				});

				if (_validar.Salvar(agrotoxico))
				{
					if (!agrotoxico.PossuiCadastro)
					{
						if (agrotoxico.Id < 1)
						{
							agrotoxico.NumeroCadastro = _da.ObterSequenciaNumeroCadastro();
						}
						else
						{
							agrotoxico.NumeroCadastro = _da.Obter(agrotoxico.Id, true).NumeroCadastro;
						}
					}

					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						#region Arquivo Bula

						if (agrotoxico.Bula != null)
						{
							if (!string.IsNullOrWhiteSpace(agrotoxico.Bula.Nome))
							{
								if (agrotoxico.Bula.Id != null && agrotoxico.Bula.Id == 0)
								{
									ArquivoBus _busArquivo = new ArquivoBus(eExecutorTipo.Interno);
									agrotoxico.Bula = _busArquivo.Copiar(agrotoxico.Bula);
								}

								if (agrotoxico.Bula.Id == 0)
								{
									ArquivoDa _arquivoDa = new ArquivoDa();
									_arquivoDa.Salvar(agrotoxico.Bula, User.FuncionarioId, User.Name, User.Login, (int)eExecutorTipo.Interno, User.FuncionarioTid, bancoDeDados);
								}
							}
							else
							{
								agrotoxico.Bula.Id = null;
							}
						}
						else
						{
							agrotoxico.Bula = new Blocos.Arquivo.Arquivo();
						}

						#endregion

						_da.Salvar(agrotoxico, bancoDeDados);

						bancoDeDados.Commit();

						Validacao.Add(Mensagem.Agrotoxico.SalvoSucesso(agrotoxico.NumeroCadastro.ToString()));
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public bool Excluir(int id)
		{
			try
			{
				if (_validar.Excluir(id))
				{
					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						GerenciadorTransacao.ObterIDAtual();
						bancoDeDados.IniciarTransacao();

						_da.Excluir(id, bancoDeDados);

						bancoDeDados.Commit();

						Validacao.Erros.Clear();

						Validacao.Add(Mensagem.Agrotoxico.Excluir);
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.Erros.RemoveAll(x => x.Tipo == eTipoMensagem.Sucesso);
				Validacao.AddErro(exc);
			}
			return Validacao.EhValido;
		}

		public void AlterarSituacao(int ingredienteAtivo, eIngredienteAtivoSituacao situacao, BancoDeDados banco)
		{
			#region Configurar

			AgrotoxicoFiltro filtro = new AgrotoxicoFiltro();
			List<Agrotoxico> lista = new List<Agrotoxico>();

			if (situacao == eIngredienteAtivoSituacao.Inativo || situacao == eIngredienteAtivoSituacao.Ativo_ANVISA_Inativo_Estado)
			{
				filtro = new AgrotoxicoFiltro()
				{
					Situacao = "1", //Cadastro Ativo
					IngredienteAtivoId = ingredienteAtivo
				};

				lista = _da.ObterLista(filtro, banco);

				foreach (var item in lista)
				{
					item.CadastroAtivo = false;
					item.MotivoId = (int)eAgrotoxicoDesativadoMotivo.IngredienteAtivoDesativado;
				}
			}
			else
			{
				filtro = new AgrotoxicoFiltro()
				{
					Situacao = "0", //Cadastro Inativo
					MotivoId = (int)eAgrotoxicoDesativadoMotivo.IngredienteAtivoDesativado,
					IngredienteAtivoId = ingredienteAtivo
				};

				lista = _da.ObterLista(filtro, banco);

				foreach (var item in lista)
				{
					item.CadastroAtivo = true;
					item.MotivoId = null;
				}
			}

			#endregion

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				foreach (var item in lista)
				{
					_da.AlterarSituacao(item, bancoDeDados);
				}

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter/Filtrar

		public Agrotoxico Obter(int id, BancoDeDados banco = null)
		{
			Agrotoxico agrotoxico = null;
			try
			{
				using (BancoDeDados bancoDedados = BancoDeDados.ObterInstancia(banco))
				{
					agrotoxico = _da.Obter(id);

					agrotoxico.Bula = new ArquivoDa().Obter(agrotoxico.Bula.Id.Value, bancoDedados);
				}

			}
			catch (Exception exc)
			{

				Validacao.AddErro(exc);
			}
			return agrotoxico;
		}

		public Resultados<AgrotoxicoFiltro> Filtrar(AgrotoxicoFiltro filtrosListar, Paginacao paginacao)
		{
			try
			{
				var filtros = new Filtro<AgrotoxicoFiltro>(filtrosListar, paginacao);
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

		#endregion

		#region Auxiliares

		public void CarregarMotivoAgrotoxicoDesativado(Agrotoxico agrotoxico)
		{
			try
			{
				foreach (var ingredienteAtivo in agrotoxico.IngredientesAtivos)
				{
					if (ingredienteAtivo.SituacaoId != (int)eIngredienteAtivoSituacao.Ativo)
					{
						agrotoxico.MotivoId = (int)eAgrotoxicoDesativadoMotivo.IngredienteAtivoDesativado;
						agrotoxico.MotivoTexto = _busLista.AgrotoxicoDesativadosMensagens.SingleOrDefault(x => x.Id == (int)eAgrotoxicoDesativadoMotivo.IngredienteAtivoDesativado).Texto;
						return;
					}
				}

				if (!agrotoxico.CadastroAtivo)
				{
					agrotoxico.MotivoId = (int)eAgrotoxicoDesativadoMotivo.AgrotoxicoDesativado;
					agrotoxico.MotivoTexto = _busLista.AgrotoxicoDesativadosMensagens.SingleOrDefault(x => x.Id == (int)eAgrotoxicoDesativadoMotivo.AgrotoxicoDesativado).Texto;
				}
				else
				{
					agrotoxico.MotivoId = null;
					agrotoxico.MotivoTexto = string.Empty;
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		#endregion
	}
}