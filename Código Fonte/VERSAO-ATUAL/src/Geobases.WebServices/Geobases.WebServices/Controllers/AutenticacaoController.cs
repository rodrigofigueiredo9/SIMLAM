using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using Tecnomapas.Geobases.WebServices.Models.Business;
using Tecnomapas.Geobases.WebServices.Models.Entities;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.Blocos.Etx.Criptografia;
using System.Configuration;

namespace Tecnomapas.Geobases.WebServices.Controllers
{
	public class AutenticacaoController : Controller
	{
		[HttpGet]
		public JsonResult LogOn(string chaveAutenticacao)
		{
			//string chave = "sistema%externo$geobases.webservices";
			//string result = Encrypt.Executar(chave); 
			try
			{
				if (!AutenticacaoBus.Logar(chaveAutenticacao))
				{
					Validacao.Add(new Mensagem() { Tipo = eTipoMensagem.Erro, Texto = "Usuário não associado as regras do sistema. Contate o Administrador do sistema." });
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return Json(new { Erros = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		public JsonResult Deslogar()
		{
			FormsAuthentication.SignOut();
			return Json(true, JsonRequestBehavior.AllowGet);
		}
	}
}