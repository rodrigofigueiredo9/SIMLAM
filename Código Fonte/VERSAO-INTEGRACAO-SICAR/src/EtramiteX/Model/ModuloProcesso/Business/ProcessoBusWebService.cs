using System;
using Tecnomapas.Blocos.Entities.Interno.ModuloProcesso;

//using Interno.Model.WebService.WSProcesso;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloProcesso.Business
{
	public class ProcessoBusWebService
	{
		//Este metodo é obrigatorio ainda tem que fazer seu real implementamento
		public Processo ObterProcesso(string numero, int tipo)
		{
			//ProcessoWS webService = new ProcessoWS();
			Processo processo = null;

			//if (String.IsNullOrEmpty(webService.UrlConsultarProcesso))
			//{
			//    processo = new Processo();
			//    return processo;
			//}
			
			//ProcessoSemasa processoSemasa = webService.ConsultarProcesso(ObterTipoWs(tipo), numero);

			//if (processoSemasa == null)
			//{
			//    return null;
			//}

			processo = new Processo();
			processo.Tipo.Id = tipo;
			processo.NumeroAutuacao = new Random().Next(500, 9999).ToString() + "/" + DateTime.Now.Year.ToString();
			processo.DataAutuacao.Data = DateTime.Now;
			
			return processo;
		}

		/*public eTipoProcessoWS ObterTipoWs(int tipo)
		{
			
			//lov_protocolo_tipo
			switch (tipo)
			{
				case 1://Ambiental
					return eTipoProcessoWS.Ambiental;

				case 2://Administrativo
					return eTipoProcessoWS.Administrativo;
				
				default:
					throw new Exception("Tipo no webService não implementado");
			}
		}*/
	}
}