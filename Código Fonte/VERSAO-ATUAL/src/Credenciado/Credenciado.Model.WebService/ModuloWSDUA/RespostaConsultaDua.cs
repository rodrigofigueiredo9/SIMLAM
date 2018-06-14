using System.Xml.Serialization;

namespace Tecnomapas.EtramiteX.Credenciado.Model.WebService.ModuloWSDUA
{
	[XmlRoot(ElementName="ide", Namespace="http://www.sefaz.es.gov.br/duae")]
	public class Ide {
		[XmlElement(ElementName="nDUA", Namespace="http://www.sefaz.es.gov.br/duae")]
		public string NDUA { get; set; }
		[XmlElement(ElementName="dEmis", Namespace="http://www.sefaz.es.gov.br/duae")]
		public string DEmis { get; set; }
		[XmlElement(ElementName="cBarra", Namespace="http://www.sefaz.es.gov.br/duae")]
		public string CBarra { get; set; }
	}

	[XmlRoot(ElementName="orgao", Namespace="http://www.sefaz.es.gov.br/duae")]
	public class Orgao {
		[XmlElement(ElementName="cnpj", Namespace="http://www.sefaz.es.gov.br/duae")]
		public string Cnpj { get; set; }
		[XmlElement(ElementName="cOrg", Namespace="http://www.sefaz.es.gov.br/duae")]
		public string COrg { get; set; }
		[XmlElement(ElementName="xNome", Namespace="http://www.sefaz.es.gov.br/duae")]
		public string XNome { get; set; }
		[XmlElement(ElementName="xSigla", Namespace="http://www.sefaz.es.gov.br/duae")]
		public string XSigla { get; set; }
	}

	[XmlRoot(ElementName="area", Namespace="http://www.sefaz.es.gov.br/duae")]
	public class Area {
		[XmlElement(ElementName="cArea", Namespace="http://www.sefaz.es.gov.br/duae")]
		public string CArea { get; set; }
		[XmlElement(ElementName="xNome", Namespace="http://www.sefaz.es.gov.br/duae")]
		public string XNome { get; set; }
	}

	[XmlRoot(ElementName="serv", Namespace="http://www.sefaz.es.gov.br/duae")]
	public class Serv {
		[XmlElement(ElementName="cServ", Namespace="http://www.sefaz.es.gov.br/duae")]
		public string CServ { get; set; }
		[XmlElement(ElementName="xNome", Namespace="http://www.sefaz.es.gov.br/duae")]
		public string XNome { get; set; }
	}

	[XmlRoot(ElementName="rece", Namespace="http://www.sefaz.es.gov.br/duae")]
	public class Rece {
		[XmlElement(ElementName="cRece", Namespace="http://www.sefaz.es.gov.br/duae")]
		public string CRece { get; set; }
		[XmlElement(ElementName="vRece", Namespace="http://www.sefaz.es.gov.br/duae")]
		public float VRece { get; set; }
	}

	[XmlRoot(ElementName="contri", Namespace="http://www.sefaz.es.gov.br/duae")]
	public class Contri {
		[XmlElement(ElementName="cpf", Namespace="http://www.sefaz.es.gov.br/duae")]
		public string Cpf { get; set; }

        [XmlElement(ElementName = "cnpj", Namespace = "http://www.sefaz.es.gov.br/duae")]
        public string Cnpj { get; set; }

		[XmlElement(ElementName="xNome", Namespace="http://www.sefaz.es.gov.br/duae")]
		public string XNome { get; set; }
	}

	[XmlRoot(ElementName="data", Namespace="http://www.sefaz.es.gov.br/duae")]
	public class Data {
		[XmlElement(ElementName="dRef", Namespace="http://www.sefaz.es.gov.br/duae")]
		public string DRef { get; set; }
		[XmlElement(ElementName="dVen", Namespace="http://www.sefaz.es.gov.br/duae")]
		public string DVen { get; set; }
		[XmlElement(ElementName="dPag", Namespace="http://www.sefaz.es.gov.br/duae")]
		public string DPag { get; set; }
	}

	[XmlRoot(ElementName="valor", Namespace="http://www.sefaz.es.gov.br/duae")]
	public class Valor {
		[XmlElement(ElementName="vMul", Namespace="http://www.sefaz.es.gov.br/duae")]
		public string VMul { get; set; }

		[XmlElement(ElementName="vJur", Namespace="http://www.sefaz.es.gov.br/duae")]
		public string VJur { get; set; }

		[XmlElement(ElementName="vAtu", Namespace="http://www.sefaz.es.gov.br/duae")]
		public string VAtu { get; set; }

		[XmlElement(ElementName="vCred", Namespace="http://www.sefaz.es.gov.br/duae")]
		public string VCred { get; set; }

		[XmlElement(ElementName="vTot", Namespace="http://www.sefaz.es.gov.br/duae")]
		public float VTot { get; set; }
	}

	[XmlRoot(ElementName="pgto", Namespace="http://www.sefaz.es.gov.br/duae")]
	public class Pgto {
		[XmlElement(ElementName="cPgto", Namespace="http://www.sefaz.es.gov.br/duae")]
		public string CPgto { get; set; }
		[XmlElement(ElementName="dAut", Namespace="http://www.sefaz.es.gov.br/duae")]
		public string DAut { get; set; }
		[XmlElement(ElementName="cAge", Namespace="http://www.sefaz.es.gov.br/duae")]
		public string CAge { get; set; }
		[XmlElement(ElementName="cAut", Namespace="http://www.sefaz.es.gov.br/duae")]
		public string CAut { get; set; }
		[XmlElement(ElementName="fPgto", Namespace="http://www.sefaz.es.gov.br/duae")]
		public string FPgto { get; set; }
		[XmlElement(ElementName="tCap", Namespace="http://www.sefaz.es.gov.br/duae")]
		public string TCap { get; set; }
		[XmlElement(ElementName="cBan", Namespace="http://www.sefaz.es.gov.br/duae")]
		public string CBan { get; set; }
	}

	[XmlRoot(ElementName="infDUAe", Namespace="http://www.sefaz.es.gov.br/duae")]
	public class InfDUAe {
		[XmlElement(ElementName="ide", Namespace="http://www.sefaz.es.gov.br/duae")]
		public Ide Ide { get; set; }
		[XmlElement(ElementName="orgao", Namespace="http://www.sefaz.es.gov.br/duae")]
		public Orgao Orgao { get; set; }
		[XmlElement(ElementName="area", Namespace="http://www.sefaz.es.gov.br/duae")]
		public Area Area { get; set; }
		[XmlElement(ElementName="serv", Namespace="http://www.sefaz.es.gov.br/duae")]
		public Serv Serv { get; set; }
		[XmlElement(ElementName="rece", Namespace="http://www.sefaz.es.gov.br/duae")]
		public Rece Rece { get; set; }
		[XmlElement(ElementName="contri", Namespace="http://www.sefaz.es.gov.br/duae")]
		public Contri Contri { get; set; }
		[XmlElement(ElementName="data", Namespace="http://www.sefaz.es.gov.br/duae")]
		public Data Data { get; set; }
		[XmlElement(ElementName="valor", Namespace="http://www.sefaz.es.gov.br/duae")]
		public Valor Valor { get; set; }
		[XmlElement(ElementName="infComp", Namespace="http://www.sefaz.es.gov.br/duae")]
		public string InfComp { get; set; }
		[XmlElement(ElementName="pgto", Namespace="http://www.sefaz.es.gov.br/duae")]
		public Pgto Pgto { get; set; }
		[XmlAttribute(AttributeName="versao")]
		public string Versao { get; set; }
	}

	[XmlRoot(ElementName="dua", Namespace="http://www.sefaz.es.gov.br/duae")]
	public class Dua {
		[XmlElement(ElementName="infDUAe", Namespace="http://www.sefaz.es.gov.br/duae")]
		public InfDUAe InfDUAe { get; set; }
	}

	[XmlRoot(ElementName="retConsDua", Namespace="http://www.sefaz.es.gov.br/duae")]
	public class RetConsDua {
		[XmlElement(ElementName="tpAmb", Namespace="http://www.sefaz.es.gov.br/duae")]
		public string TpAmb { get; set; }
		[XmlElement(ElementName="dRet", Namespace="http://www.sefaz.es.gov.br/duae")]
		public string DRet { get; set; }
		[XmlElement(ElementName="nProt", Namespace="http://www.sefaz.es.gov.br/duae")]
		public string NProt { get; set; }
		[XmlElement(ElementName="tProc", Namespace="http://www.sefaz.es.gov.br/duae")]
		public string TProc { get; set; }
		[XmlElement(ElementName="cStat", Namespace="http://www.sefaz.es.gov.br/duae")]
		public string CStat { get; set; }
		[XmlElement(ElementName="xMotivo", Namespace="http://www.sefaz.es.gov.br/duae")]
		public string XMotivo { get; set; }
		[XmlElement(ElementName="dua", Namespace="http://www.sefaz.es.gov.br/duae")]
		public Dua Dua { get; set; }
		[XmlAttribute(AttributeName="xmlns")]
		public string Xmlns { get; set; }
		[XmlAttribute(AttributeName="versao")]
		public string Versao { get; set; }
	}

	[XmlRoot(ElementName="duaConsultaResult", Namespace="http://www.sefaz.es.gov.br/duae")]
	public class DuaConsultaResult {
		[XmlElement(ElementName="retConsDua", Namespace="http://www.sefaz.es.gov.br/duae")]
		public RetConsDua RetConsDua { get; set; }
	}

	[XmlRoot(ElementName="duaConsultaResponse", Namespace="http://www.sefaz.es.gov.br/duae")]
	public class DuaConsultaResponse {
		[XmlElement(ElementName="duaConsultaResult", Namespace="http://www.sefaz.es.gov.br/duae")]
		public DuaConsultaResult DuaConsultaResult { get; set; }
		[XmlAttribute(AttributeName="xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName="Body", Namespace="http://www.w3.org/2003/05/soap-envelope")]
	public class Body {
		[XmlElement(ElementName="duaConsultaResponse", Namespace="http://www.sefaz.es.gov.br/duae")]
		public DuaConsultaResponse DuaConsultaResponse { get; set; }
	}

	[XmlRoot(ElementName="Envelope", Namespace="http://www.w3.org/2003/05/soap-envelope")]
	public class RespostaConsultaDua {
		[XmlElement(ElementName="Body", Namespace="http://www.w3.org/2003/05/soap-envelope")]
		public Body Body { get; set; }
		[XmlAttribute(AttributeName="soap", Namespace="http://www.w3.org/2000/xmlns/")]
		public string Soap { get; set; }
		[XmlAttribute(AttributeName="xsi", Namespace="http://www.w3.org/2000/xmlns/")]
		public string Xsi { get; set; }
		[XmlAttribute(AttributeName="xsd", Namespace="http://www.w3.org/2000/xmlns/")]
		public string Xsd { get; set; }
	}
}