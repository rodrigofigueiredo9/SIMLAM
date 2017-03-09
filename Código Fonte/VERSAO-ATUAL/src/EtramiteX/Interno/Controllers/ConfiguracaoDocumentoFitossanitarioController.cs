using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloConfiguracaoDocumentoFitossanitario.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMConfiguracaoDocumentoFitossanitario;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class ConfiguracaoDocumentoFitossanitarioController : DefaultController
	{
		#region Propriedades

		ConfiguracaoDocumentoFitossanitarioBus _bus = new ConfiguracaoDocumentoFitossanitarioBus();
		ConfiguracaoDocumentoFitossanitarioValidar _validar = new ConfiguracaoDocumentoFitossanitarioValidar();
		ListaBus _listaBus = new ListaBus();

		#endregion

		[Permite(RoleArray = new Object[] { ePermissao.ConfigDocumentoFitossanitario })]
		public ActionResult Configurar()
		{
            ConfiguracaoDocumentoFitossanitarioVM vm = new ConfiguracaoDocumentoFitossanitarioVM(
				_bus.Obter(),
				_listaBus.DocumentosFitossanitario.Where(x => 
					Convert.ToInt32(x.Id) == (int)eDocumentoFitossanitarioTipo.CFO || 
					Convert.ToInt32(x.Id) == (int)eDocumentoFitossanitarioTipo.CFOC || 
					Convert.ToInt32(x.Id) == (int)eDocumentoFitossanitarioTipo.PTV).ToList());

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ConfigDocumentoFitossanitario })]
		public ActionResult Configurar(ConfiguracaoDocumentoFitossanitario configuracao)
		{
			_bus.Salvar(configuracao);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@Url = Url.Action("Configurar", "ConfiguracaoDocumentoFitossanitario", new { Msg = Validacao.QueryParam() })
			}, JsonRequestBehavior.AllowGet);
		}

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.ConfigDocumentoFitossanitario })]
        public ActionResult SalvarEdicao(ConfiguracaoDocumentoFitossanitario configuracao, string idString, string novoNumInicial, string novoNumFinal)
        {
            int id = Convert.ToInt32(idString);
            DocumentoFitossanitario editar = configuracao.DocumentoFitossanitarioIntervalos.FirstOrDefault(x => x.ID == id);

            long inicioOriginal = editar.NumeroInicial;
            long fimOriginal = editar.NumeroFinal;

            editar.NumeroInicial = Convert.ToInt64(novoNumInicial);
            editar.NumeroFinal = Convert.ToInt64(novoNumFinal);
            var intervalos = configuracao.DocumentoFitossanitarioIntervalos.Where(x => x.ID != id).ToList();

            //Faz as verificações para ver se o novo intervalo é válido
            _validar.ValidarIntervalo(editar, intervalos);  //Verificações normais relativas a um intervalo

            //Trazer a lista de todos os numeros já LIBERADOS (Institucional lívia -> Credenciado -> Consultar número de cfo/cfoc liberado)
            var lista = _bus.ObterLiberadosIntervalo(editar.TipoDocumentoID, inicioOriginal, fimOriginal);

            //Verificar se existem números liberados dentro do intervalo modificado
            //Se existe, verificar se a mudança de range deixa de incluir os números liberados
            if (lista.Count(x => x < editar.NumeroInicial || x > editar.NumeroFinal) > 0)
            {
                Validacao.Add(Mensagem.ConfiguracaoDocumentoFitossanitario.IntervaloUtilizado());
            }
            else
            {
                //Se for válido, remove o item original de Configuração
                configuracao.DocumentoFitossanitarioIntervalos = intervalos;

                //Coloca o item com os novos valores em Configuração
                configuracao.DocumentoFitossanitarioIntervalos.Add(editar);

                //Chama a função de salvar, normalmente
                _bus.Editar(configuracao, id);
            }
            return Json(new
            {
                @EhValido = Validacao.EhValido,
                @Msg = Validacao.Erros,
                @Url = Url.Action("Configurar", "ConfiguracaoDocumentoFitossanitario", new { Msg = Validacao.QueryParam() })
            }, JsonRequestBehavior.AllowGet);
        }

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ConfigDocumentoFitossanitario })]
		public ActionResult ValidarIntervalo(DocumentoFitossanitario intervalo, List<DocumentoFitossanitario> intervalos)
		{
			_validar.ValidarIntervalo(intervalo, intervalos);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
			}, JsonRequestBehavior.AllowGet);
		}

        [Permite(RoleArray = new Object[] {ePermissao.ConfigDocumentoFitossanitario })]
        public ActionResult EditarNumeracao(int id)
        {
            int i = 2;

            ConfiguracaoDocumentoFitossanitario _listaCompletaIntervalos = _bus.Obter();
            DocumentoFitossanitario intervaloSelecionado = _listaCompletaIntervalos.DocumentoFitossanitarioIntervalos.FirstOrDefault(x => x.ID == id);

            return View("EditarNumeracao", intervaloSelecionado);
        }
	}
}