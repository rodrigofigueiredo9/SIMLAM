using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;

namespace Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital
{
	public class ProjetoDigital
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Int32 Etapa { get; set; }
		public Int32 RequerimentoId { get; set; }
		public String RequerimentoTid { get; set; }
		public Int32? EmpreendimentoId { get; set; }
		public String EmpreendimentoTid { get; set; }
		public String EmpreendimentoTexto { get; set; }
		public String Interessado { get; set; }
		public Int32 Situacao { get; set; }
		public String SituacaoTexto { get; set; }
		public Int32 CredenciadoId { get; set; }
		public String CredenciadoTid { get; set; }
		public List<Pessoa> Pessoas { get; set; }
		public DateTecno DataCriacao { get; set; }
		public DateTecno DataEnvio { get; set; }
		public String MotivoRecusa { get; set; }
		public List<Dependencia> Dependencias { get; set; }
		
		public ProjetoDigital()
		{
			Pessoas = new List<Pessoa>();
			DataCriacao = new DateTecno();
			DataEnvio = new DateTecno();
			Dependencias = new List<Dependencia>();
		}
	}
}