using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloProjetoDigital.Business;
using Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloDominialidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloEmpreendimento.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using CaracterizacaoCredenciadoValidar = Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Bussiness.CaracterizacaoValidar;
using DominialidadeCredenciadoBus = Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloDominialidade.Business.DominialidadeBus;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class DominialidadeController : DefaultController
	{
		#region Propriedades

		ListaBus _listaBus = new ListaBus();
		DominialidadeBus _bus = new DominialidadeBus(new DominialidadeValidar());
		DominialidadeCredenciadoBus _busCredenciado = new DominialidadeCredenciadoBus();
		CaracterizacaoCredenciadoValidar _caracterizacaoCredValidar = new CaracterizacaoCredenciadoValidar();
		DominialidadeValidar _validar = new DominialidadeValidar();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
		CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus();
		EmpreendimentoBus _busEmpreendimento = new EmpreendimentoBus();

        public static double globalModuloFiscal;

		#endregion

		#region Criar

		[Permite(RoleArray = new Object[] { ePermissao.DominialidadeCriar })]
		public ActionResult Criar(int id)
		{
			if (!_caracterizacaoValidar.Basicas(id))
			{
				return RedirectToAction("Index", "../Empreendimento", Validacao.QueryParamSerializer());
			}

			Dominialidade caracterizacao = _bus.ObterDadosGeo(id);
			if (!_caracterizacaoValidar.Dependencias(id, (int)eCaracterizacao.Dominialidade) || !_validar.Dominios(caracterizacao.Dominios))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			}

			int zona = _busEmpreendimento.ObterEnderecoZona(id);
			caracterizacao.Dominios.ForEach(x => x.EmpreendimentoLocalizacao = zona);

			caracterizacao.EmpreendimentoId = id;
			caracterizacao.Dependencias = _caracterizacaoBus.ObterDependenciasAtual(id, eCaracterizacao.Dominialidade, eCaracterizacaoDependenciaTipo.Caracterizacao);
			DominialidadeVM vm = new DominialidadeVM(caracterizacao, _listaBus.BooleanLista);
			vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;
			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.DominialidadeCriar })]
		public ActionResult Criar(Dominialidade caracterizacao)
		{
			string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.Dominialidade,
				eCaracterizacaoDependenciaTipo.Caracterizacao,
				caracterizacao.Dependencias);

			if (!string.IsNullOrEmpty(textoMerge))
			{
				return Json(new { @TextoMerge = textoMerge }, JsonRequestBehavior.AllowGet);
			}

			_bus.Salvar(caracterizacao);
			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@UrlRedirecionar = Url.Action("", "Caracterizacao", new { id = caracterizacao.EmpreendimentoId, Msg = Validacao.QueryParam() })
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Editar

		[Permite(RoleArray = new Object[] { ePermissao.DominialidadeEditar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizar, Artefato = (int)eHistoricoArtefatoCaracterizacao.dominialidade)]
		public ActionResult Editar(int id)
		{
			if (!_caracterizacaoValidar.Basicas(id))
			{
				return RedirectToAction("Index", "../Empreendimento", Validacao.QueryParamSerializer());
			}

			if (!_caracterizacaoValidar.Dependencias(id, (int)eCaracterizacao.Dominialidade))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			}

			if (!_validar.ProjetoFinalizado(id))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			}

			Dominialidade caracterizacao = _bus.ObterPorEmpreendimento(id);
			string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.Dominialidade,
				eCaracterizacaoDependenciaTipo.Caracterizacao,
				caracterizacao.Dependencias);

			if (!string.IsNullOrEmpty(textoMerge))
			{
				caracterizacao = _bus.MergiarGeo(caracterizacao);

				if (!_validar.Dominios(caracterizacao.Dominios))
				{
					return RedirectToAction("", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
				}
			}

			foreach (var dominio in caracterizacao.Dominios)
			{
				foreach (var reserva in dominio.ReservasLegais)
				{
					if (!LocalizacoesReserva(reserva, (int)dominio.Tipo).Any(x => x.Id == reserva.LocalizacaoId.ToString()))
					{
						reserva.LocalizacaoId = 0;
					}
				}
			}

			DominialidadeVM vm = new DominialidadeVM(caracterizacao, _listaBus.BooleanLista);
			vm.TextoMerge = textoMerge;
			vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.DominialidadeEditar })]
		public ActionResult Editar(Dominialidade caracterizacao)
		{
			string textoMerge = _caracterizacaoValidar.DependenciasAlteradas(
				caracterizacao.EmpreendimentoId,
				(int)eCaracterizacao.Dominialidade,
				eCaracterizacaoDependenciaTipo.Caracterizacao,
				caracterizacao.Dependencias);

			if (!string.IsNullOrEmpty(textoMerge))
			{
				return Json(new { @TextoMerge = textoMerge }, JsonRequestBehavior.AllowGet);
			}

			_bus.Salvar(caracterizacao);
			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@UrlRedirecionar = Url.Action("", "Caracterizacao", new { id = caracterizacao.EmpreendimentoId, Msg = Validacao.QueryParam() })
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Visualizar

		[Permite(RoleArray = new Object[] { ePermissao.DominialidadeVisualizar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizar, Artefato = (int)eHistoricoArtefatoCaracterizacao.dominialidade)]
		public ActionResult Visualizar(int id)
		{
			if (!_caracterizacaoValidar.Dependencias(id, (int)eCaracterizacao.Dominialidade))
			{
				return RedirectToAction("", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			}

			Dominialidade caracterizacao = _bus.ObterPorEmpreendimento(id, isVisualizar: true);
			DominialidadeVM vm = new DominialidadeVM(caracterizacao, _listaBus.BooleanLista, true);
			vm.UrlRetorno = Url.Action("", "Caracterizacao", new { id = caracterizacao.EmpreendimentoId });

			return View(vm);
		}

		#endregion

		#region Excluir

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.DominialidadeExcluir })]
		public ActionResult ExcluirConfirm(int id)
		{
			ExcluirVM vm = new ExcluirVM();
			vm.Id = id;
			vm.Mensagem = Mensagem.Dominialidade.ExcluirMensagem;
			vm.Titulo = "Excluir Dominialidade";

			return PartialView("Excluir", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.DominialidadeExcluir })]
		public ActionResult Excluir(int id)
		{
			string urlRedireciona = string.Empty;

			if (_bus.Excluir(id))
			{
				urlRedireciona = Url.Action("Index", "Caracterizacao", new { id = id, Msg = Validacao.QueryParam() });
			}

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, urlRedireciona = urlRedireciona }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Dominio/Reserva Legal

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.DominialidadeCriar, ePermissao.DominialidadeEditar })]
		public ActionResult Dominio(Dominio dominio, int empreendimento)
		{
			DominioVM vm = new DominioVM(_listaBus.DominialidadeComprovacoes, dominio, dominio.ComprovacaoId);

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                #region Select QTD Modulo Fiscal GLOBAL
                Comando comando = bancoDeDados.CriarComando(@"SELECT ATP_QTD_MODULO_FISCAL FROM CRT_CAD_AMBIENTAL_RURAL WHERE EMPREENDIMENTO = :empreendimentoID");//, EsquemaBanco);

                comando.AdicionarParametroEntrada("empreendimentoID", empreendimento, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        //SET VARIAVEL GLOBAL
                        globalModuloFiscal = Convert.ToDouble(reader["ATP_QTD_MODULO_FISCAL"]);
                    }

                    reader.Close();
                }
                #endregion
            }
            
			return PartialView("DominioPartial", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.DominialidadeVisualizar })]
		public ActionResult DominioVisualizar(Dominio dominio)
		{
			DominioVM vm = new DominioVM(_listaBus.DominialidadeComprovacoes, dominio, dominio.ComprovacaoId, true);
			return PartialView("DominioPartial", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.DominialidadeCriar, ePermissao.DominialidadeEditar })]
		public ActionResult ReservaLegal(ReservaLegal reservaLegal, int? dominioTipo)
		{
			List<Lista> dominiosCompensacao = new List<Lista>();
			List<Lista> lstIdentificacaoARL = new List<Lista>();

			if (reservaLegal.EmpreendimentoCompensacao.Id > 0)
			{
				dominiosCompensacao = _bus.ObterDominiosLista(reservaLegal.EmpreendimentoCompensacao.Id);
				lstIdentificacaoARL = _bus.ObterARLCompensacaoLista(reservaLegal.EmpreendimentoId, reservaLegal.MatriculaId.GetValueOrDefault());
			}

			ReservaLegalVM vm = new ReservaLegalVM(_listaBus.DominialidadeReservaSituacoes,
													_listaBus.DominialidadeReservaLocalizacoes,
													_listaBus.DominialidadeReservaCartorios,
													reservaLegal, dominioTipo: dominioTipo.GetValueOrDefault(0),
													lstTiposCoordenada: _listaBus.TiposCoordenada,
													lstDatuns: _listaBus.Datuns,
													lstMatriculaCompensacao: dominiosCompensacao,
													lstIdentificacaoARLCompensacao: lstIdentificacaoARL,
													booleanLista: _listaBus.BooleanLista,
													lstSituacoesVegetal: _listaBus.DominialidadeReservaSituacaoVegetacao);
			return PartialView("ReservaLegalPartial", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.DominialidadeVisualizar })]
		public ActionResult ReservaLegalVisualizar(ReservaLegal reservaLegal)
		{
			List<Lista> dominiosCompensacao = new List<Lista>();
			List<Lista> lstIdentificacaoARL = new List<Lista>();

			if (reservaLegal.EmpreendimentoCompensacao.Id > 0)
			{
				dominiosCompensacao = _bus.ObterDominiosLista(reservaLegal.EmpreendimentoCompensacao.Id);
				lstIdentificacaoARL = _bus.ObterARLCompensacaoLista(reservaLegal.EmpreendimentoId, reservaLegal.MatriculaId.GetValueOrDefault());
			}

			ReservaLegalVM vm = new ReservaLegalVM(_listaBus.DominialidadeReservaSituacoes,
												   _listaBus.DominialidadeReservaLocalizacoes,
												   _listaBus.DominialidadeReservaCartorios,
												   reservaLegal,
												   true,
												   0,
												   _listaBus.TiposCoordenada,
												   _listaBus.Datuns,
												   booleanLista: _listaBus.BooleanLista,
												   lstMatriculaCompensacao: dominiosCompensacao,
												   lstIdentificacaoARLCompensacao: lstIdentificacaoARL,
												   lstSituacoesVegetal: _listaBus.DominialidadeReservaSituacaoVegetacao);
			return PartialView("ReservaLegalPartial", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.DominialidadeCriar, ePermissao.DominialidadeEditar })]
		public ActionResult DominioValidarSalvar(Dominio dominio)
		{
			_validar.DominioSalvar(dominio);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.DominialidadeCriar, ePermissao.DominialidadeEditar })]
		public ActionResult ReservaLegalValidarSalvar(ReservaLegal reserva, List<ReservaLegal> reservasAdicionadas)
		{
			if (reservasAdicionadas != null && reservasAdicionadas.Count > 0)
			{
				if (reserva.IdentificacaoARLCedente > 0 && reservasAdicionadas.Exists(r => r.IdentificacaoARLCedente == reserva.IdentificacaoARLCedente))
				{
					Validacao.Add(Mensagem.Dominialidade.RLAssociadaEmOutroEmpreendimentoCedente(0, ""));
				}
			}

			_validar.ReservaLegalSalvar(reserva);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.DominialidadeCriar, ePermissao.DominialidadeEditar })]
		public ActionResult ObterDadosEmpreendimentoCompensacao(int empreendimentoId)
		{
			List<Lista> dominios = _bus.ObterDominiosLista(empreendimentoId);
			return Json(new { @Valido = Validacao.EhValido, @Msg = Validacao.Erros, @Dominios = dominios });
		}

		[Permite(RoleArray = new Object[] { ePermissao.DominialidadeCriar, ePermissao.DominialidadeEditar })]
		public ActionResult ObterCompensacaoARL(int empreendimentoId, int dominio)
		{
			List<Lista> compensacoesARL = _bus.ObterARLCompensacaoLista(empreendimentoId, dominio);
			return Json(new { @Valido = Validacao.EhValido, @Msg = Validacao.Erros, @Compensacoes = compensacoesARL });
		}

		[Permite(RoleArray = new Object[] { ePermissao.DominialidadeCriar, ePermissao.DominialidadeEditar })]
		public ActionResult ObterARLCedente(int reservaCedenteId) 
		{

			Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade.ReservaLegal reservaCedente = null;

			reservaCedente = _bus.ObterReservaLegal(reservaCedenteId);

			if (reservaCedente == null)
			{
				return Json(new { @Valido = Validacao.EhValido, @Msg = Validacao.Erros });
			}

			var reservaJson = new
			{
				@SituacaoVegetalTexto = reservaCedente.SituacaoVegetalTexto,
				@SituacaoVegetalId = reservaCedente.SituacaoVegetalId,
				@Area = reservaCedente.ARLCroqui
			};

			return Json(new { @Valido = Validacao.EhValido, @Msg = Validacao.Erros, @ReservaCedenteJson = ViewModelHelper.Json(reservaJson) });
		}

		[Permite(RoleArray = new Object[] { ePermissao.DominialidadeCriar, ePermissao.DominialidadeEditar })]
		public ActionResult AtualizarGrupoARL(Dominialidade caracterizacao)
		{
			_bus.ObterDominialidadeARL(caracterizacao);
			DominialidadeVM vm = new DominialidadeVM(caracterizacao, new List<Lista>());

           /* var qtdModuloFiscal = 0.0;
            #region Carga das tabelas APP Caculada e APP Escadinha
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                #region Select QTD Modulo Fiscal
                Comando comando = bancoDeDados.CriarComando(@"SELECT ATP_QTD_MODULO_FISCAL FROM CRT_CAD_AMBIENTAL_RURAL WHERE EMPREENDIMENTO = :empreendimentoID");//, EsquemaBanco);

                comando.AdicionarParametroEntrada("empreendimentoID", caracterizacao.EmpreendimentoId, DbType.Int32);
                
                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        qtdModuloFiscal = Convert.ToDouble(reader["ATP_QTD_MODULO_FISCAL"]);
                    }

                    reader.Close();
                }
                #endregion
            }
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia("idafgeo"))
            {
                #region Chamada Procedure
                bancoDeDados.IniciarTransacao();
                Comando command = bancoDeDados.CriarComando(@"begin OPERACOESPROCESSAMENTOGEO.CalcularAppClassificadaCAR(:emp); end;");

                command.AdicionarParametroEntrada("emp", caracterizacao.EmpreendimentoId, System.Data.DbType.Int32);
                
                bancoDeDados.ExecutarNonQuery(command);
                bancoDeDados.Commit();
                #endregion
            }
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia("idafgeo"))
            {
                #region Chamada Procedure
                bancoDeDados.IniciarTransacao();
                Comando com = bancoDeDados.CriarComando(@"begin OPERACOESPROCESSAMENTOGEO.CalcularEscadinhaCAR(:emp, :moduloFiscal); end;");

                com.AdicionarParametroEntrada("emp", caracterizacao.EmpreendimentoId, System.Data.DbType.Int32);
                com.AdicionarParametroEntrada("moduloFiscal", qtdModuloFiscal, System.Data.DbType.Double);

                bancoDeDados.ExecutarNonQuery(com);
                bancoDeDados.Commit();
                #endregion
            }
            #endregion

            if(globalModuloFiscal != qtdModuloFiscal)
            {
                //var textoMerge = "ENTROU AQUI";
                //return Json(new { @TextoMerge = textoMerge }, JsonRequestBehavior.AllowGet);
                ConfirmarVM mv = new ConfirmarVM();

                mv.Id = caracterizacao.EmpreendimentoId;
                mv.Mensagem = Mensagem.Caracterizacao.retificacaoCAR(" ");
                mv.Titulo = "Confirmação da Retificação";
                //return PartialView("Confirmar", mv);
                return Json(new
                {
                    @EhValido = Validacao.EhValido,
                    @Empty = true,  //Variavel para abrir o modal
                    @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "Confirmar", mv)
                }, JsonRequestBehavior.AllowGet);
            }*/
            ConfirmarVM mv = new ConfirmarVM();
            
            mv.Id = caracterizacao.EmpreendimentoId;
            mv.Mensagem = Mensagem.Retificacao.msgCred1(123, 666);
            mv.Titulo = "Confirmação da Retificação";
            //return PartialView("Confirmar", mv);
            return Json(new
            {
                @EhValido = Validacao.EhValido,
                @Empty = true,  //Variavel para abrir o modal
                @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "Confirmar", mv)
            }, JsonRequestBehavior.AllowGet);
            /*
            return Json(new
            {
                @EhValido = Validacao.EhValido,
                @Msg = Validacao.Erros,
                @Empty = false,
                @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "DominialidadeARLPartial", vm)
            }, JsonRequestBehavior.AllowGet);
            */
        }

		[Permite(RoleArray = new Object[] { ePermissao.DominialidadeCriar, ePermissao.DominialidadeEditar })]
		private List<Lista> LocalizacoesReserva(ReservaLegal reserva, int dominioTipo = 0)
		{
			List<Lista> localizacoes = _listaBus.DominialidadeReservaLocalizacoes;

			if (!string.IsNullOrEmpty(reserva.Identificacao))
			{
				if (reserva.Compensada)
				{
					localizacoes = localizacoes.Where(x => (new string[] { 
					((int)eReservaLegalLocalizacao.CompensacaoEmpreendimentoCedente).ToString() }).Contains(x.Id)).ToList(); ;
				}
				else
				{
					if (dominioTipo == (int)eDominioTipo.Posse)
					{
						localizacoes = localizacoes.Where(x => x.Id == ((int)eReservaLegalLocalizacao.NestaPosse).ToString()).ToList(); ;
					}
					else if (dominioTipo == (int)eDominioTipo.Matricula)
					{
						localizacoes = localizacoes.Where(x => x.Id == ((int)eReservaLegalLocalizacao.NestaMatricula).ToString()).ToList(); ;
					}
				}
			}
			else
			{
				localizacoes = localizacoes.Where(x => (new string[] { 
					((int)eReservaLegalLocalizacao.CompensacaoEmpreendimentoReceptora).ToString() }).Contains(x.Id)).ToList();
			}

			return localizacoes;
		}

		#endregion

		#region Merge

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.DominialidadeCriar, ePermissao.DominialidadeEditar })]
		public ActionResult GeoMergiar(Dominialidade caracterizacao)
		{
			DominialidadeVM vm = new DominialidadeVM(_bus.MergiarGeo(caracterizacao), _listaBus.BooleanLista);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "DominialidadePartial", vm),
				@Dependencias = ViewModelHelper.Json(vm.Caracterizacao.Dependencias)
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Visualizar Credenciado

		[Permite(RoleArray = new Object[] { ePermissao.AnaliseItensCriar })]
		public ActionResult VisualizarCredenciado(int projetoDigitalId, int protocoloId = 0)
		{
			ProjetoDigitalCredenciadoBus _busProjetoDigitalCredenciado = new ProjetoDigitalCredenciadoBus();
			ProjetoDigital projeto = _busProjetoDigitalCredenciado.Obter(projetoDigitalId);

			Dominialidade caracterizacao = _busCredenciado.ObterPorEmpreendimento(projeto.EmpreendimentoId.GetValueOrDefault(), projetoDigitalId);
			DominialidadeVM vm = new DominialidadeVM(caracterizacao, ListaCredenciadoBus.BooleanLista, true);

			vm.ProtocoloId = protocoloId;
			vm.ProjetoDigitalId = projeto.Id;
			vm.RequerimentoId = projeto.RequerimentoId;
			vm.UrlRetorno = Url.Action("Analisar", "../AnaliseItens", new { protocoloId = protocoloId, requerimentoId = projeto.RequerimentoId });

			return View("Visualizar", vm);
		}

		#endregion
	}
}