using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Gerencial.Controllers
{
	public class ListaController : DefaultController
	{
		GerenciadorConfiguracao<ConfiguracaoEndereco> _configEnd = new GerenciadorConfiguracao<ConfiguracaoEndereco>(new ConfiguracaoEndereco());

		public List<Estado> Estados
		{
			get { return _configEnd.Obter<List<Estado>>(ConfiguracaoEndereco.KeyEstados); }
		}

		public ActionResult Municipios(int EstadoId)
		{
			Dictionary<int, List<Municipio>> lstMun = _configEnd.Obter<Dictionary<int, List<Municipio>>>(ConfiguracaoEndereco.KeyMunicipios);

			return Json(
				lstMun[EstadoId],
				JsonRequestBehavior.AllowGet
			);
		}

		public ActionResult MunicipiosUf(String uf)
		{
			Estado estado = Estados.SingleOrDefault(x => String.Equals(x.Texto, uf, StringComparison.InvariantCultureIgnoreCase));
			int idEstado = 1;

			if (estado != null)
			{
				idEstado = estado.Id;
			}

			Dictionary<int, List<Municipio>> lstMun = _configEnd.Obter<Dictionary<int, List<Municipio>>>(ConfiguracaoEndereco.KeyMunicipios);

			return Json(
				lstMun[idEstado],
				JsonRequestBehavior.AllowGet
			);
		}
	}
}