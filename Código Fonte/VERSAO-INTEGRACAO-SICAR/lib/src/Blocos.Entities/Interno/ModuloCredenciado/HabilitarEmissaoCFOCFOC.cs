using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Autenticacao;
using Tecnomapas.Blocos.Entities.Interno.ModuloPapel;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado
{
	public class HabilitarEmissaoCFOCFOC
	{
		public int Id { get; set; }
		public String Tid { get; set; }
		public CredenciadoIntEnt Responsavel { get; set; }
		public Arquivo.Arquivo Arquivo { get; set; }
		public String NumeroHabilitacao { get; set; }
		public String NumeroHabilitacaoES 
		{
			get
			{
				if (Convert.ToBoolean(ExtensaoHabilitacao))
				{
					return NumeroHabilitacao + "-ES";
				}
				return NumeroHabilitacao;
			}
		}
		public String ValidadeRegistro { get; set; }
		public String SituacaoData { get; set; }
		public Int32 Situacao { get; set; }
		public String SituacaoTexto { get; set; }
		public Int32? Motivo { get; set; }
		public String MotivoTexto { get; set; }
		public String NumeroDua { get; set; }
		public Int32 ExtensaoHabilitacao { get; set; }
		public String Observacao { get; set; }
		public String NumeroHabilitacaoOrigem { get; set; }
		public String RegistroCrea { get; set; }
		public String NumeroVistoCrea { get; set; }
		public Int32 UF { get; set; }
		public String UFTexto { get; set; }

		#region Meio de contato

		public String TelefoneResidencial { get; set; }
		public String TelefoneCelular { get; set; }
		public String TelefoneFax { get; set; }
		public String TelefoneComercial { get; set; }
		public String Email { get; set; }

		#endregion

		public List<PragaHabilitarEmissao> Pragas { get; set; }

		public HabilitarEmissaoCFOCFOC()
		{
			Responsavel = new CredenciadoIntEnt();
			Arquivo = new Blocos.Arquivo.Arquivo();
			Pragas = new List<PragaHabilitarEmissao>();
			Situacao = 1;
			SituacaoTexto = "Ativo";
		}
	}
}