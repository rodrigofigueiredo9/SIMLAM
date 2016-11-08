using System;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo
{
	public class HistoricoProtocolo
	{
		public Int32 ApensadorId { get; set; }
		public String Numero { get; set; }
		public Int32 AcaoId { get; set; }
		public String AcaoTexto { get; set; }
		public Int32 ProtocoloTipoId { get; set; }
		public string ProtocoloTipoNome 
		{
			get
			{
				string strTipoNome = "";

				if (this.ProtocoloTipoId == 1)
				{
					strTipoNome = "Processo";
				}

				if (this.ProtocoloTipoId == 2)
				{
					strTipoNome = "Documento";
				}

				return strTipoNome;
			}
		}

		public eHistoricoAcao AcaoEnumerado
		{
			get { return (eHistoricoAcao)this.AcaoId; }
		}

		private DateTecno _acaoData = new DateTecno();
		public DateTecno AcaoData
		{
			get { return _acaoData; }
			set { _acaoData = value; }
		}

		private Funcionario _executor = new Funcionario();
		public Funcionario Executor
		{
			get { return _executor; }
			set { _executor = value; }
		}

		private Setor _setor = new Setor();
		public Setor Setor
		{
			get { return _setor; }
			set { _setor = value; }
		}
	}
}