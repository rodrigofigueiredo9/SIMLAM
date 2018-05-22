﻿using System;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao
{
	public class FiscalizacaoListarFiltro
	{
		public String NumeroFiscalizacao { get; set; }
		public String NumeroAIIUFBloco { get; set; }
		public String NumeroTEITADBloco { get; set; }

		private ProtocoloNumero _protocolo = new ProtocoloNumero();
		public ProtocoloNumero Protocolo
		{
			get { return _protocolo; }
			set { _protocolo = value; }
		}

		public String AutuadoNomeRazao { get; set; }
		public String AutuadoCpfCnpj { get; set; }

		public String EmpreendimentoDenominador { get; set; }
		public String EmpreendimentoCnpj { get; set; }


		//Data de Vistoria
		private DateTecno _dataFiscalizacao = new DateTecno();
		public DateTecno DataFiscalizacao
		{
			get { return _dataFiscalizacao; }
			set { _dataFiscalizacao = value; }
		}

		public String NumeroSEP { get; set; }

		public Int32 InfracaoTipo { get; set; }
		public String InfracaoTipoTexto { get; set; }

		public Int32 ItemTipo { get; set; }
		public String ItemTipoTexto { get; set; }

		public String AgenteFiscal { get; set; }

		public Int32 SituacaoTipo { get; set; }
		public String SituacaoTipoTexto { get; set; }

		public Int32 SetorTipo { get; set; }
		public String SetorTipoTexto { get; set; }

		public Int32 Serie { get; set; }
		public String SerieTexto { get; set; }

		public Int32 AreaFiscalizacao { get; set; }
		public String AreaFiscalizacaoTexto { get; set; }

		public Int32 Classificacao { get; set; }
		public String ClassificacaoTexto { get; set; }
	}
}
