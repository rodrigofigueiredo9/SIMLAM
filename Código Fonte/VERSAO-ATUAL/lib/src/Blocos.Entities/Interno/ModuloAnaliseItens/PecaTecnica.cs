using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloAnaliseItens
{
	public class PecaTecnica
	{
		public Int32? Id { get; set; }
		public String Tid { get; set; }
		public IProtocolo Protocolo { get; set; }
		public Int32 Atividade { get; set; }
		public Int32 ElaboradorTipo { get; set; }
		public Int32 Elaborador { get; set; }
		public Int32? SetorCadastro { get; set; }
		public Int32? ProtocoloPai { set; get; }

		private List<Responsavel> _responsaveisEmpreendimento = new List<Responsavel>();
		public List<Responsavel> ResponsaveisEmpreendimento
		{
			get { return _responsaveisEmpreendimento; }
			set { _responsaveisEmpreendimento = value; }
		}

		public eElaboradorTipo ElaboradorTipoEnum { get { return (eElaboradorTipo)ElaboradorTipo; } }

		public PecaTecnica()
		{
			Protocolo = new Protocolo();
		}
	}
}