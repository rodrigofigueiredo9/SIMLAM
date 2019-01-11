using System;
using System.Collections.Generic;
using System.Web;
using Tecnomapas.Blocos.Arquivo.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Etx.ModuloArquivo;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloEmail;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloEmail.Business;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAtividade.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFuncionario.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Business
{
	public class TituloDeclaratorioBus
	{
		#region Propriedades

		TituloDeclaratorioValidar _validar = null;
		FuncionarioBus _busFuncionario = new FuncionarioBus();
		ListaBus _busLista = new ListaBus();
		TituloDa _da = new TituloDa();
		TituloModeloBus _busModelo = new TituloModeloBus(null);
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		public EtramiteIdentity User
		{
			get
			{
				try
				{
					return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity;
				}
				catch (Exception exc)
				{
					Validacao.AddErro(exc);
					return null;
				}
			}
		}

		#endregion

		public TituloDeclaratorioBus() { _validar = new TituloDeclaratorioValidar(); }

		public TituloDeclaratorioBus(TituloDeclaratorioValidar validar)
		{
			_validar = validar;
		}

		#region Ações de DML

		public void Salvar(Titulo titulo)
		{
			try
			{
				titulo.Autor = new Funcionario() { Id = User.FuncionarioId };

				if (titulo.Id <= 0)
				{
					titulo.Situacao.Id = (int)eTituloSituacao.EmCadastro;
					titulo.DataCriacao.Data = DateTime.Now;
				}

				if (titulo.Modelo.Regra(eRegra.NumeracaoAutomatica))
				{
					titulo.Numero.Inteiro = null;
					titulo.Numero.Ano = null;
				}

				//Carrega o Modelo e suas regras do modelo
				titulo.Modelo = _busModelo.Obter(titulo.Modelo.Id);

				IEspecificidadeBus busEsp = EspecificiadadeBusFactory.Criar(titulo.Modelo.Codigo.GetValueOrDefault());

				string jsonEsp = (titulo.Especificidade != null) ? titulo.Especificidade.Json : null;
				titulo.Especificidade = (Especificidade)busEsp.Deserialize(jsonEsp);
				titulo.Especificidade = titulo.ToEspecificidade();

				//Delega a validação de especificidade
				_validar.ValidarEspecificidade = () =>
				{
					busEsp.Validar.Salvar(titulo.Especificidade);
				};

				if (_validar.Salvar(titulo))
				{
					#region Arquivos/Diretorio

					ArquivoBus _busArquivo = new ArquivoBus(eExecutorTipo.Interno);

					if (!titulo.Modelo.Regra(eRegra.PdfGeradoSistema) && titulo.ArquivoPdf.Id == 0)
					{
						titulo.ArquivoPdf = _busArquivo.Copiar(titulo.ArquivoPdf);
					}

					if (titulo.Anexos != null && titulo.Anexos.Count > 0)
					{
						foreach (Anexo anexo in titulo.Anexos)
						{
							if (anexo.Arquivo.Id == 0)
							{
								anexo.Arquivo = _busArquivo.Copiar(anexo.Arquivo);
							}
						}
					}

					#endregion

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						GerenciadorTransacao.ObterIDAtual();
						bancoDeDados.IniciarTransacao();

						#region Arquivos/Banco

						ArquivoDa arquivoDa = new ArquivoDa();

						if (!titulo.Modelo.Regra(eRegra.PdfGeradoSistema))
						{
							arquivoDa.Salvar(titulo.ArquivoPdf, User.FuncionarioId, User.Name, User.Login, (int)eExecutorTipo.Interno, User.FuncionarioTid, bancoDeDados);
						}

						if (titulo.Anexos != null && titulo.Anexos.Count > 0)
						{
							foreach (Anexo anexo in titulo.Anexos)
							{
								if (anexo.Arquivo.Id == 0)
								{
									arquivoDa.Salvar(anexo.Arquivo, User.FuncionarioId, User.Name, User.Login, (int)eExecutorTipo.Interno, User.FuncionarioTid, bancoDeDados);
								}
							}
						}

						#endregion

						_da.Salvar(titulo, bancoDeDados);

						//Atualiza os Ids em Especificidade
						titulo.Especificidade = titulo.ToEspecificidade();
						busEsp.Salvar(titulo.Especificidade, bancoDeDados);

						//Trata quando o catch do busEsp.Salvar silencia o erro.
						if (!Validacao.EhValido)
						{
							return;
						}

						bancoDeDados.Commit();
					}

					Validacao.Add(Mensagem.Titulo.Salvar);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void Excluir(Titulo titulo)
		{
			try
			{
				titulo = _da.ObterSimplificado(titulo.Id);
				titulo.Modelo = ObterModelo(titulo.Modelo.Id);

				if (!_validar.Excluir(titulo))
				{
					return;
				}

				IEspecificidadeBus busEsp = EspecificiadadeBusFactory.Criar(titulo.Modelo.Codigo.GetValueOrDefault());

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
				{
					GerenciadorTransacao.ObterIDAtual();
					bancoDeDados.IniciarTransacao();

					busEsp.Excluir(titulo.Id, bancoDeDados);

					_da.Excluir(titulo.Id, bancoDeDados);

					bancoDeDados.Commit();
				}

				Validacao.Add(Mensagem.Titulo.Excluir);

			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void AlterarSituacao(Titulo titulo, BancoDeDados banco = null)
		{
			Titulo atualTitulo = _da.ObterSimplificado(titulo.Id);

			bool isGerarNumero = false;
			bool isGerarPdf = false;

			if (!_validar.AlterarSituacao(titulo, atualTitulo.Situacao.Id == (int)eTituloSituacao.EmCadastro))
			{
				return;
			}

			#region Configurar Nova Situacao

			//Situação Nova
			switch ((eTituloSituacao)titulo.Situacao.Id)
			{
				#region 3 - Valido

				case eTituloSituacao.Valido:
					if (atualTitulo.Situacao.Id == (int)eTituloSituacao.EmCadastro)
					{
						isGerarNumero = true;
						isGerarPdf = true;
					}
					break;

				#endregion
			}

			#endregion

			#region Numero de Titulo

			if (isGerarNumero)
			{
				titulo.Numero.ReiniciaPorAno = titulo.Modelo.Regra(eRegra.NumeracaoReiniciada);

				if (titulo.Modelo.Regra(eRegra.NumeracaoAutomatica))
				{
					titulo.Numero.Automatico = true;
					TituloModeloResposta iniciarEm = titulo.Modelo.Resposta(eRegra.NumeracaoAutomatica, eResposta.InicioNumeracao);
					titulo.Numero.IniciaEm = null;
					titulo.Numero.IniciaEmAno = null;

					if (iniciarEm != null)
					{
						if (iniciarEm.Valor == null || !ValidacoesGenericasBus.ValidarMaskNumeroBarraAno(iniciarEm.Valor.ToString()))
						{
							Validacao.Add(Mensagem.Titulo.IniciarEmInvalido);
							return;
						}

						string[] iniciar = iniciarEm.Valor.ToString().Split('/');
						titulo.Numero.IniciaEm = Convert.ToInt32(iniciar[0]);
						titulo.Numero.IniciaEmAno = Convert.ToInt32(iniciar[1]);

						if (titulo.Numero.IniciaEmAno.GetValueOrDefault() != DateTime.Now.Year)
						{
							titulo.Numero.IniciaEm = null;
							titulo.Numero.IniciaEmAno = null;
						}
					}
				}

				titulo.Numero.Ano = DateTime.Today.Year;
			}

			#endregion

			GerenciadorTransacao.ObterIDAtual();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				_da.DeclaratorioAlterarSituacao(titulo, bancoDeDados);

				#region Atividades

				AtividadeBus atividadeBus = new AtividadeBus();

				#region Título Valido

				if (titulo.Situacao.Id == (int)eTituloSituacao.Valido)
				{
					if (titulo.Atividades != null && titulo.Atividades.Count > 0)
					{
						foreach (Atividade atividade in titulo.Atividades)
						{
							if (VerificarDeclaratorioSituacao(atividade, eTituloSituacao.Valido, titulo.EmpreendimentoId.GetValueOrDefault(), bancoDeDados))
							{
								atividade.SituacaoId = (int)eAtividadeSituacao.Regular;
								atividadeBus.AlterarSituacao(atividade, bancoDeDados);
							}
						}
					}
				}

				#endregion

				#region Título Suspenso

				if (titulo.Situacao.Id == (int)eTituloSituacao.Suspenso)
				{
					if (titulo.Atividades != null && titulo.Atividades.Count > 0)
					{
						foreach (Atividade atividade in titulo.Atividades)
						{
							if (VerificarDeclaratorioSituacao(atividade, eTituloSituacao.Suspenso, titulo.EmpreendimentoId.GetValueOrDefault(), bancoDeDados))
							{
								atividade.SituacaoId = (int)eAtividadeSituacao.Suspensa;
								atividadeBus.AlterarSituacao(atividade, bancoDeDados);
							}
						}
					}
				}

				#endregion

				#region Título Encerrado

				if (titulo.Situacao.Id == (int)eTituloSituacao.EncerradoDeclaratorio)
				{
					if (titulo.Atividades != null && titulo.Atividades.Count > 0)
					{
						foreach (Atividade atividade in titulo.Atividades)
						{
							if (VerificarDeclaratorioSituacao(atividade, eTituloSituacao.EncerradoDeclaratorio, titulo.EmpreendimentoId.GetValueOrDefault(), bancoDeDados))
							{
								atividade.SituacaoId = (int)eAtividadeSituacao.Irregular;
								atividadeBus.AlterarSituacao(atividade, bancoDeDados);
							}
						}
					}
				}

				#endregion

				#endregion

				#region Gerar Pdf de Titulo

				ArquivoBus arqBus = new ArquivoBus(eExecutorTipo.Interno);

				if (isGerarPdf && titulo.Modelo.Regra(eRegra.PdfGeradoSistema))
				{
					TituloBus bus = new TituloBus();

					titulo.ArquivoPdf.Nome = "Titulo.pdf";
					titulo.ArquivoPdf.Extensao = ".pdf";
					titulo.ArquivoPdf.ContentType = "application/pdf";
					titulo.ArquivoPdf.Buffer = bus.GerarPdf(titulo, bancoDeDados);

					if (titulo.ArquivoPdf.Buffer != null)
					{
						arqBus.Salvar(titulo.ArquivoPdf);

						ArquivoDa _arquivoDa = new ArquivoDa();
						_arquivoDa.Salvar(titulo.ArquivoPdf, User.FuncionarioId, User.Name,
							User.Login, (int)eExecutorTipo.Interno, User.FuncionarioTid, bancoDeDados);

						_da.SalvarPdfTitulo(titulo, bancoDeDados);
					}
				}

				#endregion

				#region Gerar/Enviar Email

				#region Gerar Email

				Email email = null;
				if (titulo.Situacao.Id == (int)eTituloSituacao.Valido && titulo.Modelo.Regra(eRegra.EnviarEmail))
				{
					if (titulo.Modelo.Resposta(eRegra.EnviarEmail, eResposta.TextoEmail).Valor != null)
					{
						string textoEmail = titulo.Modelo.Resposta(eRegra.EnviarEmail, eResposta.TextoEmail).Valor.ToString();

						if (!String.IsNullOrWhiteSpace(textoEmail))
						{
							Dictionary<String, String> emailKeys = new Dictionary<string, string>();

							emailKeys.Add("[orgão sigla]", _configSys.Obter<String>(ConfiguracaoSistema.KeyOrgaoSigla));
							emailKeys.Add("[data da conclusão]", titulo.Modelo.Regra(eRegra.Prazo) ? titulo.DataInicioPrazo.DataTexto : titulo.DataEmissao.DataTexto);
							emailKeys.Add("[nome do modelo]", titulo.Modelo.Nome);
							emailKeys.Add("[nome do subtipo]", titulo.Modelo.SubTipo);
							emailKeys.Add("[nº do título]", titulo.Numero.Texto);
							emailKeys.Add("[nº processo/documento do título]", titulo.Protocolo.Numero);
							emailKeys.Add("[nome do empreendimento]", titulo.EmpreendimentoTexto);

							foreach (string key in emailKeys.Keys)
							{
								textoEmail = textoEmail.Replace(key, emailKeys[key]);
							}

							email = new Email();
							email.Assunto = _configSys.Obter<String>(ConfiguracaoSistema.KeyOrgaoSigla);
							email.Texto = textoEmail;
							email.Tipo = eEmailTipo.TituloConcluir;
							email.Codigo = titulo.Id;
						}
					}
				}

				#endregion

				if (email != null)
				{
					List<String> lstEmail = _da.ObterEmails(titulo.Id, bancoDeDados);

					if (lstEmail != null && lstEmail.Count > 0)
					{
						email.Destinatario = String.Join(", ", lstEmail.ToArray());

						if (titulo.Modelo.Regra(eRegra.AnexarPDFTitulo))
						{
							email.Anexos.Add(titulo.ArquivoPdf);
						}

						EmailBus emailBus = new EmailBus();
						emailBus.Enviar(email, bancoDeDados);
					}
				}

				#endregion

				if (!Validacao.EhValido)
				{
					bancoDeDados.Rollback();
					return;
				}

				#region Salvar A especificidade

				if (EspecificiadadeBusFactory.Possui(titulo.Modelo.Codigo.GetValueOrDefault()))
				{
					IEspecificidadeBus busEsp = EspecificiadadeBusFactory.Criar(titulo.Modelo.Codigo.GetValueOrDefault());
					titulo.Especificidade = busEsp.Obter(titulo.Id) as Especificidade;
					titulo.Especificidade = titulo.ToEspecificidade();
					busEsp.Salvar(titulo.Especificidade, bancoDeDados);

					List<DependenciaLst> lstDependencias = busEsp.ObterDependencias(titulo.Especificidade);
					if (isGerarPdf && lstDependencias != null && lstDependencias.Count > 0)
					{
						if (!lstDependencias.Exists(x => x.TipoId == (int)eTituloDependenciaTipo.Caracterizacao && x.DependenciaTipo == (int)eCaracterizacao.Dominialidade))
						{
							lstDependencias.Add(new DependenciaLst() { TipoId = (int)eTituloDependenciaTipo.Caracterizacao, DependenciaTipo = (int)eCaracterizacao.Dominialidade });
						}
						_da.Dependencias(titulo.Id, titulo.Modelo.Id, titulo.EmpreendimentoId.GetValueOrDefault(), lstDependencias);
					}
				}

				#endregion

				#region Histórico

				_da.Historico.Gerar(titulo.Id, eHistoricoArtefato.titulo, eHistoricoAcao.alterarsituacao, bancoDeDados);
				_da.Consulta.Gerar(titulo.Id, eHistoricoArtefato.titulo, bancoDeDados);

				#endregion

				bancoDeDados.Commit();

				Validacao.Add(Mensagem.TituloAlterarSituacao.TituloAltSituacaoSucesso);
			}
		}

		public void AlterarSituacao(int tituloId, eTituloSituacao situacao, BancoDeDados banco = null)
		{
			try
			{
				_da.AlterarSituacao(tituloId, (int)situacao, banco);
			}
			catch (Exception ex)
			{
				Validacao.AddErro(ex);
			}
		}

		#endregion

		#region Obter / Filtrar

		public Resultados<Titulo> Filtrar(TituloFiltro filtrosListar, Paginacao paginacao)
		{
			try
			{
				if (!_validar.Filtrar(filtrosListar))
				{
					return new Resultados<Titulo>();
				}

				filtrosListar.IsDeclaratorio = true;
				Filtro<TituloFiltro> filtro = new Filtro<TituloFiltro>(filtrosListar, paginacao);
				Resultados<Titulo> resultados = _da.Filtrar(filtro);

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

		public Titulo Obter(int id, BancoDeDados banco = null)
		{
			try
			{
				return _da.Obter(id, banco: banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public Titulo ObterSimplificado(int id, BancoDeDados banco = null)
		{
			try
			{
				return _da.ObterSimplificado(id, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<TituloAssinante> ObterAssinantes(int id)
		{
			try
			{
				return _da.ObterAssinantes(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return new List<TituloAssinante>();
		}

		public List<Municipio> ObterLocais()
		{
			ListaBus busLista = new ListaBus();
			return busLista.Municipios(_busLista.EstadoDefault);
		}

		public List<Setor> ObterFuncionarioSetores()
		{
			return _busFuncionario.ObterSetoresFuncionario(User.FuncionarioId);
		}

		public List<Situacao> ObterSituacoes()
		{
			return _da.ObterSituacoes();
		}

		public TituloModelo ObterModelo(int modeloId)
		{
			return _busModelo.Obter(modeloId);
		}

		public List<TituloAssinante> ObterAssinantesCargos(List<TituloAssinante> lstAssinantes)
		{
			try
			{
				foreach (TituloAssinante assinante in lstAssinantes)
				{
					assinante.Cargos = _busFuncionario.ObterFuncionarioCargos(assinante.FuncionarioId);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return lstAssinantes;
		}

		#endregion

		#region Validações

		internal bool VerificarDeclaratorioSituacao(Atividade atividade, eTituloSituacao situacao, int empreendimentoID, BancoDeDados banco = null)
		{
			return _da.PossuiDeclaratorioForaSituacao(atividade, situacao, empreendimentoID, banco);
		}

		#endregion
	}
}