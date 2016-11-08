using System;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Gerencial.ViewModels;

namespace Tecnomapas.EtramiteX.Gerencial.Controllers
{
	public class ArquivoController : DefaultController
	{
		[HttpPost]
		public string Arquivo(HttpPostedFileBase file)
		{
			string msg = string.Empty;
			Arquivo arquivo = null;

			try
			{
				ArquivoBus _bus = new ArquivoBus(eExecutorTipo.Interno);
				arquivo = _bus.SalvarTemp(file);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return ViewModelHelper.JsSerializer.Serialize(new { Msg = Validacao.Erros, Arquivo = arquivo });
		}

		public FileResult Baixar(int id)
		{
			return ViewModelHelper.BaixarArquivo(id);
		}

		public FileResult BaixarTemporario(string nomeTemporario, string contentType)
		{
			return ViewModelHelper.BaixarArquivoTemporario(nomeTemporario, contentType);
		}
	}
}