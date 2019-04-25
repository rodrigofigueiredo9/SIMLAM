using Aspose.Words.Tables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Arquivo.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloArquivo;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;
using Tecnomapas.Blocos.Entities.Interno.ModuloProcesso;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloSetor;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Business;
using Tecnomapas.Blocos.Etx.ModuloRelatorio;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloProjetoGeografico.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFuncionario.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProcesso.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Business
{
	public class TituloBus
	{
		#region Propriedades

		TituloValidar _validar = null;
		FuncionarioBus _busFuncionario = new FuncionarioBus();
		ListaBus _busLista = new ListaBus();
		TituloDa _da = new TituloDa();
		CondicionanteDa _daCond = new CondicionanteDa();
		TituloModeloBus _busModelo = new TituloModeloBus(null);
		ProjetoGeograficoBus _busProjetoGeografico = new ProjetoGeograficoBus();
		GerenciadorConfiguracao<ConfiguracaoTituloModelo> _configTituloModelo = new GerenciadorConfiguracao<ConfiguracaoTituloModelo>(new ConfiguracaoTituloModelo());

		ProtocoloBus _busProtocolo = new ProtocoloBus();
		public ProtocoloBus BusProtocolo
		{
			get { return _busProtocolo; }
		}

		ProcessoBus _busProcesso = new ProcessoBus();
		public ProcessoBus BusProcesso_
		{
			get { return _busProcesso; }
		}

		DocumentoBus _busDocumento = new DocumentoBus();
		public DocumentoBus BusDocumento_
		{
			get { return _busDocumento; }
		}

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

		public List<int> ModeloCodigosPendencia
		{
			get { return _configTituloModelo.Obter<List<int>>(ConfiguracaoTituloModelo.KeyModeloCodigoPendencia); }
		}

		#endregion

		public TituloBus() { _validar = new TituloValidar(); }

		public TituloBus(TituloValidar validar)
		{
			_validar = validar;
		}
		public List<int> LstCadastroAmbientalRuralTituloCodigo
		{
			get { return _configTituloModelo.Obter<List<int>>(ConfiguracaoTituloModelo.KeyCadastroAmbientalRuralTituloCodigo); }
		}
		#region Ações de DML

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

		public void Salvar(Titulo titulo)
		{
			try
			{
				titulo.Autor = new Funcionario() { Id = User.FuncionarioId };

				if (titulo.Id <= 0)
				{
					titulo.Situacao.Id = (int)eTituloSituacao.Cadastrado;
					titulo.DataCriacao.Data = DateTime.Now;
				}

				if (titulo.Modelo.Regra(eRegra.NumeracaoAutomatica))
				{
					titulo.Numero.Inteiro = null;
					titulo.Numero.Ano = null;
				}

				//Carrega o Modelo e suas regras do modelo
				titulo.Modelo = _busModelo.Obter(titulo.Modelo.Id);

				if (titulo.Condicionantes != null && titulo.Condicionantes.Count > 0)
				{
					foreach (var cond in titulo.Condicionantes)
					{
						if (!cond.PossuiPrazo || !cond.PossuiPeriodicidade)
						{
							if (cond.Periodicidades != null)
							{
								cond.Periodicidades.Clear();
							}
							continue;
						}

						TituloCondicionantePeriodicidade periodicidade = new TituloCondicionantePeriodicidade();
						periodicidade.Situacao.Id = 1;
					}
				}

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
					if (LstCadastroAmbientalRuralTituloCodigo.Any(x => x == titulo.Modelo.Codigo))
					{
						Validacao.Add(Mensagem.Retificacao.msgInst4());
					}					
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public Arquivo GerarPdf(int id)
		{
			Titulo titulo = _da.ObterSimplificado(id);
			titulo.Modelo = ObterModelo(titulo.Modelo.Id);
			titulo.Anexos = _da.ObterAnexos(id);

			if (titulo.Modelo.Regra(eRegra.PdfGeradoSistema) && (titulo.Modelo.Arquivo.Id ?? 0) <= 0)
			{
				Validacao.Add(Mensagem.Titulo.ModeloNaoPossuiPdf);
				return null;
			}

			if (titulo.ArquivoPdf.Id > 0)
			{
				ArquivoBus busArquivo = new ArquivoBus(eExecutorTipo.Interno);
				titulo.ArquivoPdf = busArquivo.Obter(titulo.ArquivoPdf.Id.Value);
				string auxiliar = string.Empty;

				switch (titulo.Situacao.Id)
				{
					case (int)eTituloSituacao.Encerrado:
						auxiliar = _busLista.MotivosEncerramento.Single(x => x.Id == titulo.MotivoEncerramentoId).Texto;
						titulo.ArquivoPdf.Buffer = Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx.PdfMetodosAuxiliares.TarjaVermelha(titulo.ArquivoPdf.Buffer, auxiliar);
						break;

					case (int)eTituloSituacao.Prorrogado:
						auxiliar = String.Format("{0} até {1}", titulo.Situacao.Nome, titulo.DataVencimento.DataTexto);
						titulo.ArquivoPdf.Buffer = Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx.PdfMetodosAuxiliares.TarjaVerde(titulo.ArquivoPdf.Buffer, auxiliar);
						break;

					case (int)eTituloSituacao.Suspenso:
						titulo.ArquivoPdf.Buffer = Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx.PdfMetodosAuxiliares.TarjaLaranjaEscuro(titulo.ArquivoPdf.Buffer, "Consultado em " + DateTime.Now.ToShortDateString() + " às " + DateTime.Now.ToString(@"HH\hmm\min"), "Suspenso");
						break;

					case (int)eTituloSituacao.EncerradoDeclaratorio:
						titulo.ArquivoPdf.Buffer = Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx.PdfMetodosAuxiliares.TarjaVermelha(titulo.ArquivoPdf.Buffer, "Consultado em " + DateTime.Now.ToShortDateString() + " às " + DateTime.Now.ToString(@"HH\hmm\min"), "Encerrado");
						break;

					default:
						break;
				}

				titulo.ArquivoPdf.Nome = String.Concat(titulo.Modelo.Nome, "");
				return titulo.ArquivoPdf;
			}

			if ((titulo.Modelo.Codigo == (int)eTituloModeloCodigo.LaudoVistoriaFlorestal ||
				titulo.Modelo.Codigo == (int)eTituloModeloCodigo.AutorizacaoExploracaoFlorestal) &&
				!titulo.Anexos.Exists(x => x.Croqui == true))
			{
				this.AnexarCroqui(titulo);
				if (titulo.Modelo.Codigo == (int)eTituloModeloCodigo.LaudoVistoriaFlorestal)
				{
					titulo.Anexos = _da.ObterAnexos(id);
					if (!titulo.Anexos.Exists(x => x.Croqui == true))
					{
						Validacao.Add(Mensagem.Titulo.CroquiNaoGerado);
						return null;
					}
				}
				else
				{
					Validacao.Add(Mensagem.Titulo.CroquiNaoGerado);
					return null;
				}
			}

			titulo.ArquivoPdf.Nome = String.Concat(titulo.Modelo.Nome,"");
			titulo.ArquivoPdf.Extensao = "";
			titulo.ArquivoPdf.ContentType = "application/pdf";
			titulo.ArquivoPdf.Buffer = GerarPdf(titulo);

			return titulo.ArquivoPdf;
		}

		public MemoryStream GerarPdf(Titulo titulo, BancoDeDados banco = null)
		{
			if ((titulo.Modelo.Arquivo.Id ?? 0) <= 0)
			{
				return null;
			}

			ArquivoBus busArquivo = new ArquivoBus(eExecutorTipo.Interno);
			Arquivo templatePdf = busArquivo.Obter(titulo.Modelo.Arquivo.Id.Value);

			//Carrega as atividades para o ObterDadosPdf;
			if (titulo.Atividades == null || titulo.Atividades.Count == 0)
			{
				titulo.Atividades = _da.ObterAtividades(titulo.Id);
			}

			IEspecificidadeBus busEspecificiade = EspecificiadadeBusFactory.Criar(titulo.Modelo.Codigo.Value);

			titulo.Especificidade = busEspecificiade.Obter(titulo.Id) as Especificidade;
			titulo.ToEspecificidade();
			IConfiguradorPdf configurador = busEspecificiade.ObterConfiguradorPdf(titulo.Especificidade) ?? new ConfiguracaoDefault();

			configurador.ExibirSimplesConferencia = (titulo.Situacao.Id == (int)eTituloSituacao.Cadastrado) || (titulo.Situacao.Id == (int)eTituloSituacao.EmCadastro) || (titulo.Situacao.Id == (int)eTituloSituacao.AguardandoPagamento);

			Object dataSource = busEspecificiade.ObterDadosPdf(titulo.Especificidade, banco);

			#region Condicionantes

			var dataSourceTitulo = (((dynamic)dataSource).Titulo as DataSourceBase);

			if (titulo.Modelo.Regra(eRegra.Condicionantes))
			{
				if (dataSourceTitulo.Condicionantes == null || dataSourceTitulo.Condicionantes.Count == 0)
				{
					EspecificidadeDa daEsp = new EspecificidadeDa();
					dataSourceTitulo.Condicionantes = daEsp.ObterCondicionantes(titulo.Id);
				}
			}

			IConfiguracaoEvent cnfEvent = configurador as IConfiguracaoEvent;

			if (cnfEvent != null)
			{
				if (!titulo.Modelo.Regra(eRegra.Condicionantes) || dataSourceTitulo.Condicionantes == null || dataSourceTitulo.Condicionantes.Count == 0)
				{
					cnfEvent.AddLoadAcao((doc, dataSourceCnf) =>
					{
						Table tabela = doc.FindTable("«TableStart:Titulo.Condicionantes»");
						if (tabela != null)
						{
							if (configurador.CondicionanteRemovePageBreakAnterior)
							{
								tabela.RemovePageBreakAnterior();
							}

							AsposeExtensoes.RemoveTables(new List<Table>() { tabela });
						}
					});
				}
				else
				{
					cnfEvent.AddExecutedAcao((doc, dataSourceCnf) =>
					{
						Table tabela = doc.LastTable("«remover»");

						while (tabela != null)
						{
							AsposeExtensoes.RemoveTables(new List<Table> { tabela });
							tabela = doc.LastTable("«remover»");
						}

					});
				}
			}

			#endregion

			GeradorAspose gerador = new GeradorAspose(configurador);

			#region Assinantes

			List<TituloAssinante> assinantes = _da.ObterAssinantes(titulo.Id);

			if (busEspecificiade.CargosOrdenar != null && busEspecificiade.CargosOrdenar.Count > 0)
			{
				assinantes = assinantes.OrderByDescending(assinante => busEspecificiade.CargosOrdenar.IndexOf((eCargo)assinante.FuncionarioCargoCodigo)).ToList();
			}

			configurador.Assinantes = assinantes.Select(x =>
				(IAssinante)new AssinanteDefault() { Nome = x.FuncionarioNome, Cargo = x.FuncionarioCargoNome }
			).ToList();

			//Adiciona os assinantes da Especificidade
			configurador.Assinantes.AddRange((((dynamic)dataSource).Titulo as IAssinanteDataSource).AssinanteSource);

			#endregion

			MemoryStream msPdf = gerador.Pdf(templatePdf, dataSource);

			if (dataSource is Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio.IAnexoPdf)
			{
				Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio.IAnexoPdf dataAnexos = dataSource as Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio.IAnexoPdf;
				msPdf = GeradorAspose.AnexarPdf(msPdf, dataAnexos.AnexosPdfs);
			}

			return msPdf;
		}

		public void AnexarCroqui(Titulo titulo, BancoDeDados banco = null)
		{
			if (titulo.Modelo.Codigo == (int)eTituloModeloCodigo.AutorizacaoExploracaoFlorestal)
			{
				var projetoId = _busProjetoGeografico.ExisteProjetoGeografico(titulo.EmpreendimentoId.GetValueOrDefault(0), (int)eCaracterizacao.ExploracaoFlorestal, finalizado: true);
				_busProjetoGeografico.GerarCroquiTitulo(projetoId, titulo.Id, banco);
			}
			else if (titulo.Modelo.Codigo == (int)eTituloModeloCodigo.LaudoVistoriaFlorestal)
			{
				var projetoId = _busProjetoGeografico.ExisteProjetoGeografico(titulo.EmpreendimentoId.GetValueOrDefault(0), (int)eCaracterizacao.ExploracaoFlorestal, finalizado: false);
				var projeto = _busProjetoGeografico.ObterProjeto(projetoId);
				var croqui = projeto.Arquivos?.FirstOrDefault(x => x.Tipo == (int)eProjetoGeograficoArquivoTipo.Croqui);
				if (croqui?.Id > 0)
					_busProjetoGeografico.AnexarCroqui(titulo.Id, croqui.Id.GetValueOrDefault(0), banco);
			}
		}
		#endregion

		#region Obter / Filtrar

		public Resultados<Titulo> Filtrar(TituloFiltro filtrosListar, Paginacao paginacao)
		{
			try
			{
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

		public List<FuncionarioLst> ObterFuncionarioSetor(int id)
		{
			return _busFuncionario.ObterFuncionariosSetor(id);
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

		public Atividade ObterAtividade(int tituloId, int atividadeId, BancoDeDados banco = null)
		{
			try
			{
				return _da.ObterAtividades(tituloId, atividadeId, banco).FirstOrDefault();
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public ProtocoloEsp ObterProcDocReqEspecificidade(int tituloId, BancoDeDados banco = null)
		{
			try
			{
				return _da.ObterProcDocReqEspecificidade(tituloId, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return new ProtocoloEsp();
		}

		public List<Atividade> ObterAtividades(int id, BancoDeDados banco = null)
		{
			try
			{
				return _da.ObterAtividades(id, null, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return new List<Atividade>();
		}

		public List<AtividadeSolicitada> ObterAtividadesSimples(int id, BancoDeDados banco = null)
		{
			try
			{
				return _da.ObterAtividadesSimples(id, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return new List<AtividadeSolicitada>();
		}

		public List<Atividade> ObterAtividadesAtual(int id, bool isProcesso = true, BancoDeDados banco = null)
		{
			try
			{
				return _da.ObterAtividadesAtual(id, isProcesso, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return new List<Atividade>();
		}

		public List<Anexo> ObterAnexos(int id, BancoDeDados banco = null)
		{
			try
			{
				return _da.ObterAnexos(id, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return new List<Anexo>();
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

		public List<TituloCondicionante> ObterCondicionantes(int id)
		{
			try
			{
				return _da.ObterCondicionantes(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return new List<TituloCondicionante>();
		}

		public List<TituloExploracaoFlorestal> ObterExploracoes(int id)
		{
			try
			{
				return _da.ObterExploracoes(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return new List<TituloExploracaoFlorestal>();
		}

		public List<TituloExploracaoFlorestal> ObterExploracoesTituloAssociado(int id)
		{
			try
			{
				return _da.ObterExploracoesTituloAssociado(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return new List<TituloExploracaoFlorestal>();
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

		public List<Setor> ObterSetoresFuncContidoModelo(int id)
		{
			List<Setor> setoresFunc = _busFuncionario.ObterSetoresFuncionario(User.FuncionarioId);
			List<Setor> setoresModelo = _busModelo.ObterSetoresModeloPorTitulo(id);
			List<Setor> setoresIntersect = new List<Setor>();

			foreach (Setor item in setoresModelo)
			{
				if (setoresFunc.Any(x => x.Id == item.Id))
				{
					setoresIntersect.Add(item);
				}
			}

			return setoresIntersect;
		}

		public List<Protocolos> ObterProcessosDocumentos(int protocolo)
		{
			try
			{
				if (protocolo <= 0)
				{
					return new List<Protocolos>();
				}

				return _da.ObterProcessosDocumentos(protocolo);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public Protocolo ObterProtocolo(int requerimento)
		{
			try
			{
				return _da.ObterProtocolo(requerimento);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public void ObterProtocolo(int id, bool isProcesso, Titulo titulo)
		{
			IProtocolo protocolo = BusProtocolo.ObterSimplificado(id);

			if (protocolo.IsProcesso)
			{
				titulo.Protocolo = protocolo as Processo;
			}
			else
			{
				titulo.Protocolo = protocolo as Documento;
			}
		}

		public List<DestinatarioEmail> ObterDestinatariosEmailProtocolo(int protocolo)
		{
			return _da.ObterDestinatariosEmailProtocolo(protocolo);
		}

		public String ObterProtocoloRequerimentoKey(ProtocoloEsp procDocoReq)
		{
			return procDocoReq.Id.ToString() + (procDocoReq.IsProcesso ? "@1@" : "@2@") + procDocoReq.RequerimentoId.ToString();
		}

		public List<Setor> SetoresModeloFuncionario(List<Setor> setoresFunc, List<Setor> setoresModelo)
		{
			List<Setor> setoresIntersect = new List<Setor>();

			foreach (Setor item in setoresModelo)
			{
				if (setoresFunc.Any(x => x.Id == item.Id))
				{
					setoresIntersect.Add(item);
				}
			}

			if (setoresIntersect.Count <= 0)
			{
				Validacao.Add(Mensagem.Titulo.AutorSetor);
			}

			return setoresIntersect;
		}

		public Finalidade ObterTituloAnteriorAtividade(Protocolo protocolo, int atividadeId, int modeloCodigo)
		{
			try
			{
				return BusProtocolo.ObterTituloAnteriorAtividade(atividadeId, protocolo.Id.Value, modeloCodigo);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
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

		public Empreendimento EmpreendimentoProcDocAlterado(Titulo titulo, bool gerarMsg = true, eTipoMensagem tipoMsg = eTipoMensagem.Advertencia)
		{
			ProtocoloDa _protocoloDa = new ProtocoloDa();
			IProtocolo protocolo = _protocoloDa.ObterSimplificado(titulo.Protocolo.Id.Value);

			if (protocolo != null && (titulo.EmpreendimentoId.GetValueOrDefault(0) != protocolo.Empreendimento.Id))
			{
				if (gerarMsg)
				{
					Mensagem msg = Mensagem.Titulo.EmpreendimentoAlterado(protocolo.IsProcesso);
					msg.Tipo = tipoMsg;
					Validacao.Add(msg);
				}
				return protocolo.Empreendimento;
			}

			return null;
		}

		public List<PessoaLst> ObterInteressadoRepresentantes(Protocolo protocolo)
		{
			List<PessoaLst> lista = new List<PessoaLst>();

			lista = BusProtocolo.ObterInteressadoRepresentantes(protocolo.Id.Value);

			return lista;
		}

		public List<PessoaLst> ObterDestinatarios(int id)
		{
			List<PessoaLst> lstPessoas = new List<PessoaLst>();
			try
			{
				lstPessoas = _da.ObterDestinatarios(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return lstPessoas;
		}

		public Object ObterProtocoloHistorico()
		{
			return null;
		}
        
        public bool ExistePorEmpreendimento(int empreendimentoId)
        {
            try
            {
                Resultados<Titulo> titulos = ObterPorEmpreendimento(empreendimentoId);
                if (titulos == null)
                    return false;
                if (titulos.Itens.Count() > 0)
                    return true;
                else
                    return false;


            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
            }

            return false;
        }
        public Resultados<Titulo> ObterPorEmpreendimento(int empreendimentoId)
        {
            try
            {
                Resultados<Titulo> resultados = _da.ObterPorEmpreendimento(empreendimentoId);

                return resultados;
            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
            }

            return null;
        }

		public List<Titulo> Obter(TituloFiltro filtrosListar)
		{
			try
			{
				Filtro<TituloFiltro> filtro = new Filtro<TituloFiltro>(filtrosListar, new Paginacao());
				List<Titulo> resultados = _da.Filtrar(filtro).Itens;

				return resultados;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		#endregion

		#region Validações

		public object ValidarAbrirAlterarAutorSetor(int id)
		{
			Titulo titulo = ObterSimplificado(id);
			List<Setor> setoresFunc = _busFuncionario.ObterSetoresFuncionario(User.FuncionarioId);

			bool trocarAutor = false;
			bool trocarSetor = false;

			if (_validar.ProtocoloPosse(titulo))
			{
				if (titulo.Autor.Id != User.FuncionarioId)
				{
					trocarAutor = true;
				}

				if (!setoresFunc.Any(x => x.Id == titulo.Setor.Id))
				{
					trocarSetor = true;
					List<Setor> setoresModelo = _busModelo.ObterSetoresModeloPorTitulo(id);
					List<Setor> setoresIntersect = new List<Setor>();

					foreach (Setor item in setoresModelo)
					{
						if (setoresFunc.Any(x => x.Id == item.Id))
						{
							setoresIntersect.Add(item);
						}
					}

					if (setoresIntersect.Count <= 0)
					{
						Validacao.Add(Mensagem.Titulo.AutorSetor);
					}
				}
			}

			return new { @AbrirTela = (trocarAutor || trocarSetor), @TrocarAutor = trocarAutor, @TrocarSetor = trocarSetor };
		}

		public bool ExisteTituloPendencia(Protocolo protocolo, BancoDeDados banco = null)
		{
			try
			{
				Titulo titulo;
				foreach (int codigo in ModeloCodigosPendencia)
				{
					titulo = _da.TitulosProtocolo(protocolo, codigo, banco)
						.FirstOrDefault(x => x.Situacao.Id != (int)eTituloSituacao.Cadastrado && x.Situacao.Id != (int)eTituloSituacao.Encerrado);
					if (titulo != null && titulo.Id > 0)
					{
						return true;
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return false;
		}

		#endregion
	}
}