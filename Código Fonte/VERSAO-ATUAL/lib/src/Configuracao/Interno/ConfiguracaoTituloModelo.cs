using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Tecnomapas.EtramiteX.Configuracao.Interno.Data;

namespace Tecnomapas.EtramiteX.Configuracao.Interno
{
	public class ConfiguracaoTituloModelo : ConfiguracaoBase
	{
		private ListaValoresDa _daLista = new ListaValoresDa();
		private JavaScriptSerializer _jss = new JavaScriptSerializer();

		public const string KeyModeloCodigoPendencia = "ModeloCodigoPendencia";
		public List<int> ModeloCodigoPendencia 
		{ 
			get 
			{ 
				return new List<int> { Convert.ToInt32(_daLista.BuscarConfiguracaoSistema("ModeloCodigoPendencia"))}; 
			} 
		}

		public const string KeyModeloCodigoIndeferido = "ModeloCodigoIndeferido";
		public List<int> ModeloCodigoIndeferido 
		{ 
			get 
			{ 
				return new List<int> { Convert.ToInt32(_daLista.BuscarConfiguracaoSistema("TituloTituloindeferimentoCodigo")) }; 
			} 
		}

		public const string KeyCadastroAmbientalRuralTituloCodigo = "CadastroAmbientalRuralTituloCodigo";
		public List<int> CadastroAmbientalRuralTituloCodigo 
		{ 
			get 
			{
				return new List<int> {Convert.ToInt32(_daLista.BuscarConfiguracaoSistema("CadastroAmbientalRuralTituloCodigo"))}; 
			} 
		}

		public const String KeyTituloCompensacaoCodigo = "TituloCompensacaoCodigo";
		public String TituloCompensacaoCodigo { get { return _daLista.BuscarConfiguracaoSistema("TituloCompensacaoCodigo"); } }

		public const String KeyAtividadeEspecificidadeCaracterizacao = "AtividadeEspecificidadeCaracterizacao";
		public Dictionary<string, object> AtividadeEspecificidadeCaracterizacao
		{ 
			get 
			{
				var configModelo = new Dictionary<string, object>();
				configModelo["AtividadeEspecificidadeCaracterizacao"] = _jss.Deserialize<List<Dictionary<string, object>>>(_daLista.BuscarConfiguracaoSistema("atividadeespecificidadecaracterizacao"));
				return configModelo; 
			} 
		}
	}
}