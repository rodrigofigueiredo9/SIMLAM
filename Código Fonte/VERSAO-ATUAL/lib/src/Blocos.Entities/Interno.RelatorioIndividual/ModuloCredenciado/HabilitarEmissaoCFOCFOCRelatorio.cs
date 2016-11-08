using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloPessoa;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloCredenciado
{
	public class HabilitarEmissaoCFOCFOCRelatorio
	{
		public int Id { get; set; }
		public String Tid { get; set; }
		public String Numero
		{
			get
			{
				if(NumeroHabilitacao.Length < 6)
				{
					return string.Empty;
				}

				return NumeroHabilitacao.Substring(NumeroHabilitacao.Length - 4) + "/" + NumeroHabilitacao.Substring(2, 2);
			}
		}
		public PessoaRelatorio Responsavel { get; set; }
		public ProfissaoRelatorio Profissao { get; set; }
		public Arquivo.Arquivo Foto { get; set; }
		public String NumeroHabilitacao { get; set; }
		public String NumeroHabilitacaoPDF
		{
			get
			{
				if (ExtensaoHabilitacaoBool)
				{
					return NumeroHabilitacao + " - ES";
				}

				return NumeroHabilitacao;
			}
			set { NumeroHabilitacao = value; }
		}
		public String ValidadeRegistro { get; set; }
		public String SituacaoData { get; set; }
		public String SituacaoTexto { get; set; }
		public String MotivoTexto { get; set; }
		public String NumeroDua { get; set; }
		public Boolean ExtensaoHabilitacaoBool { get; set; }
		public String ExtensaoHabilitacao { get; set; }
		public String Observacao { get; set; }
		public String NumeroHabilitacaoOrigem { get; set; }
		public String RegistroCrea { get; set; }
		public String NumeroVistoCrea { get; set; }
		public String UFTexto { get; set; }
		public String NumeroPaginasAnexo { get; set; }
		public DateTime DataCadastro { get; set; }
		public List<PragaHabilitarEmissaoRelatorio> Pragas { get; set; }

		public HabilitarEmissaoCFOCFOCRelatorio()
		{
			Responsavel = new PessoaRelatorio();
			Foto = new Blocos.Arquivo.Arquivo();
			Foto.Conteudo = new byte[0];
			Profissao = new ProfissaoRelatorio();
			DataCadastro = new DateTime();
			Pragas = new List<PragaHabilitarEmissaoRelatorio>();
			SituacaoTexto = "Ativo";
		}
	}
}