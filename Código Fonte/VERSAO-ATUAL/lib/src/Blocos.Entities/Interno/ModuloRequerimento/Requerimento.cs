using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento
{
	public class Requerimento
	{
		public int Id { get; set; }
		public int ProjetoDigitalId { get; set; }
		public int CredenciadoId { get; set; }
		public bool IsCredenciado { get; set; }
		public int IdRelacionamento { get; set; }
		public int Numero { get { return Id; } }
		public int Checagem { get; set; }
		public String Tid { get; set; }
		public DateTime DataCadastro { get; set; }
		public int ProtocoloId { get; set; }
		public int ProtocoloTipo { get; set; }
		public int AgendamentoVistoria { get; set; }
		public int SetorId { get; set; }

		public int? BarragensContiguas { get; set; }
		public int? ResponsabilidadeRT { get; set; }

		//utilizado somente para requerimentos que possuam título declaratório de barragem
		#region Barragem

		public bool? InfoPreenchidas { get; set; }

		#endregion Barragem

		public string DataCadastroTexto { get { return DataCadastro.ToShortDateString(); } }

		public int SituacaoId { get; set; }
		public String SituacaoTexto { get; set; }

		public String Informacoes { get; set; }

		public Pessoa Interessado { get; set; }
		public Int32 AutorId { get; set; }

		public String Origem
		{
			get
			{
				return IsRequerimentoDigital ? "Credenciado" : "Institucional";
			}
		}

		public Boolean IsRequerimentoDigital 
		{
			get {
				return AutorId > 0;
			}
		}

		public Int32 EtapaImportacao { get; set; }

		public Empreendimento Empreendimento { get; set; }
		public ProjetoDigital ProjetoDigital { get; set; }
		public List<Roteiro> Roteiros { get; set; }
		public List<Atividade> Atividades { get; set; }
		public List<ResponsavelTecnico> Responsaveis { get; set; }
		public List<Pessoa> Pessoas { get; set; }

		public Requerimento()
		{
			Empreendimento = new Empreendimento();
			ProjetoDigital = new ProjetoDigital();
			Roteiros = new List<Roteiro>();
			Atividades = new List<Atividade>();
			Responsaveis = new List<ResponsavelTecnico>();
			Pessoas = new List<Pessoa>();
			Interessado = new Pessoa();
		}
	}
}
