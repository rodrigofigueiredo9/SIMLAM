﻿using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao
{
	public class Multa
	{
		public Int32 Id { get; set; }
		public Int32 FiscalizacaoId { get; set; }
        public Boolean? IsDigital { get; set; }
        public String NumeroIUF { get; set; }
        public Int32? SerieId { get; set; }
        public Int32? CodigoReceitaId { get; set; }
		public Decimal ValorMulta { get; set; }
		public String Justificativa { get; set; }
		public Int32 FiscalizacaoSituacaoId { get; set; }

        private DateTecno _dataLavratura = new DateTecno();
        public DateTecno DataLavratura
        {
            get { return _dataLavratura; }
            set { _dataLavratura = value; }
        }

		public Arquivo.Arquivo _arquivo = new Arquivo.Arquivo();
		public Arquivo.Arquivo Arquivo
		{
			get { return _arquivo; }
			set { _arquivo = value; }
		}
	}
}
