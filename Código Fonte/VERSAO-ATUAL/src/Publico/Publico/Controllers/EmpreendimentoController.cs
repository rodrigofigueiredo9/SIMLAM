using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Publico.Model.ModuloEmpreendimento.Business;
using Tecnomapas.EtramiteX.Publico.ViewModels;
using Tecnomapas.EtramiteX.Publico.ViewModels.VMEmpreendimento;

namespace Tecnomapas.EtramiteX.Publico.Controllers
{
	public class EmpreendimentoController : DefaultController
	{
		ListaBus _busLista = new ListaBus();
		EmpreendimentoBus _bus = new EmpreendimentoBus();

		#region Filtrar

		public ActionResult Index()
		{
			ListarVM vm = new ListarVM(_bus.Atividades, _busLista.Segmentos, _busLista.QuantPaginacao);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			return PartialView(vm);
		}

		public ActionResult Filtrar(ListarVM vm, Paginacao paginacao)
		{
			if (!String.IsNullOrEmpty(vm.UltimaBusca))
			{
				vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<ListarVM>(vm.UltimaBusca).Filtros;
			}

			vm.Paginacao = paginacao;
			vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.SetListItens(_busLista.QuantPaginacao, vm.Paginacao.QuantPaginacao);

			Resultados<Empreendimento> resultados = _bus.Filtrar(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Visualizar

		public ActionResult Visualizar(int? id, bool mostrarTituloTela = true)
		{
			return View("Visualizar", new SalvarVM());

			/*Empreendimento emp = _bus.Obter(id.GetValueOrDefault());
			if (emp.Id > 0)
			{
				if (emp.Enderecos.Count <= 1)
				{
					emp.Enderecos.Add(new Endereco());
				}

				SalvarVM vm = new SalvarVM(_busLista.Estados,
					_busLista.Municipios(emp.Enderecos[0].EstadoId),
					_busLista.Municipios(emp.Enderecos[1].EstadoId),
					_busLista.Segmentos, _busLista.TiposCoordenada,
					_busLista.Datuns,
					_busLista.Fusos,
					_busLista.Hemisferios,
					_busLista.TiposResponsavel,
					_busLista.LocalColetaPonto,
					_busLista.FormaColetaPonto,
					emp.Enderecos[0].EstadoId,
					emp.Enderecos[0].MunicipioId,
					emp.Enderecos[1].EstadoId,
					emp.Enderecos[1].MunicipioId,
					emp.Coordenada.LocalColeta.GetValueOrDefault(),
					emp.Coordenada.FormaColeta.GetValueOrDefault());

				vm.Empreendimento = emp;
				vm.MostrarTituloTela = mostrarTituloTela;
				PreencherSalvar(vm);

				if (Request.IsAjaxRequest())
				{
					return PartialView("VisualizarPartial", vm);
				}
				else
				{
					return View("Visualizar", vm);
				}
			}
			else
			{
				Validacao.Add(Mensagem.Empreendimento.NaoEncontrouRegistros);
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}*/
		}

		#endregion

		#region Auxiliares

		private void PreencherSalvar(SalvarVM vm)
		{
			if (vm.Empreendimento != null)
			{
				#region meios de contato

				if (vm.Empreendimento.MeiosContatos == null || vm.Empreendimento.MeiosContatos.Count == 0)
				{
					vm.Empreendimento.MeiosContatos = PreencheContato(vm.Contato);
				}

				if (vm.Empreendimento.MeiosContatos != null && vm.Empreendimento.MeiosContatos.Count > 0)
				{
					vm.Contato = CarregaMeiosContatos(vm.Empreendimento.MeiosContatos);
				}

				#endregion

				if (vm.Segmentos == null)
				{
					vm.Segmentos = new List<SelectListItem>();
				}

				#region responsáveis

				if (vm.Empreendimento.Responsaveis != null && vm.Empreendimento.Responsaveis.Count > 0)
				{
					foreach (Responsavel resp in vm.Empreendimento.Responsaveis)
					{
						if (resp.Tipo == 3)
						{
							if (resp.DataVencimento != null)
							{
								resp.DataVencimentoTexto = resp.DataVencimento.Value.ToShortDateString();
							}
							else
							{
								resp.DataVencimento = ValidacoesGenericasBus.ParseData(resp.DataVencimentoTexto);
							}
						}
						else
						{
							resp.DataVencimento = null;
						}
					}
				}

				#endregion
			}
		}

		private List<Contato> PreencheContato(ContatoVME contato)
		{
			List<Contato> meiosContatos = new List<Contato>();

			try
			{
				if (!string.IsNullOrEmpty(contato.Telefone))
				{
					meiosContatos.Add(new Contato { Valor = contato.Telefone, Id = contato.TelefoneId, TipoContato = eTipoContato.TelefoneResidencial });
				}

				if (!string.IsNullOrEmpty(contato.TelefoneFax))
				{
					meiosContatos.Add(new Contato { Valor = contato.TelefoneFax, Id = contato.TelefoneFaxId, TipoContato = eTipoContato.TelefoneFax });
				}

				if (!string.IsNullOrEmpty(contato.Email))
				{
					meiosContatos.Add(new Contato { Valor = contato.Email, Id = contato.EmailId, TipoContato = eTipoContato.Email });
				}

				if (!string.IsNullOrEmpty(contato.NomeContato))
				{
					meiosContatos.Add(new Contato { Valor = contato.NomeContato, Id = contato.NomeContatoId, TipoContato = eTipoContato.NomeContato });
				}

				return meiosContatos;
			}
			catch (Exception exc)
			{
				Validacao.AddAdvertencia(exc.Message);
			}
			return null;
		}

		private ContatoVME CarregaMeiosContatos(List<Contato> meiosContatos)
		{
			ContatoVME contatoVME = new ContatoVME();

			if (meiosContatos != null && meiosContatos.Count > 0)
			{
				foreach (Contato cont in meiosContatos)
				{
					switch (cont.TipoContato)
					{
						case eTipoContato.TelefoneResidencial:
							contatoVME.Telefone = cont.Valor;
							contatoVME.TelefoneId = cont.Id;
							break;

						case eTipoContato.TelefoneFax:
							contatoVME.TelefoneFax = cont.Valor;
							contatoVME.TelefoneFaxId = cont.Id;
							break;

						case eTipoContato.Email:
							contatoVME.Email = cont.Valor;
							contatoVME.EmailId = cont.Id;
							break;

						case eTipoContato.NomeContato:
							contatoVME.NomeContato = cont.Valor;
							contatoVME.NomeContatoId = cont.Id;
							break;
					}
				}
			}

			return contatoVME;
		}

		#endregion
	}
}