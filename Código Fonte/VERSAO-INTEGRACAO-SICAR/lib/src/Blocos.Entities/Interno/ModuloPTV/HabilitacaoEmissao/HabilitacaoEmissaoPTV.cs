using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloPTV.HabilitacaoEmissao
{
	public class HabilitacaoEmissaoPTV
	{
		public int Id { get; set; }
		public string Tid { get; set; }
		public int SituacaoId { get; set; }
		public string NumeroHabilitacao { get; set; }
		public string RG { get; set; }
		public string NumeroMatricula { get; set; }

		public string NumeroVistoCREA { get; set; }
		public string NumeroCREA { get; set; }
		public int EstadoRegistro { get; set; }
		public Funcionario Funcionario { get; set; }
		public Arquivo.Arquivo Arquivo { get; set; }
		public Profissao Profissao { get; set; }
		public List<Contato> Telefones { get; set; }
		public Endereco Endereco { get; set; }
		public List<HabilitacaoEmissaoPTVOperador> Operadores { get; set; }

		public string TelefoneResidencial
		{
			get
			{
				return Telefones.Exists(x => x.TipoContato == eTipoContato.TelefoneResidencial) ? Telefones.Single(x => x.TipoContato == eTipoContato.TelefoneResidencial).Valor : string.Empty;
			}
		}
		public string TelefoneComercial
		{
			get
			{
				return Telefones.Exists(x => x.TipoContato == eTipoContato.TelefoneComercial) ? Telefones.Single(x => x.TipoContato == eTipoContato.TelefoneComercial).Valor : string.Empty;
			}
		}
		public string TelefoneCelular
		{
			get
			{
				return Telefones.Exists(x => x.TipoContato == eTipoContato.TelefoneCelular) ? Telefones.Single(x => x.TipoContato == eTipoContato.TelefoneCelular).Valor : string.Empty;
			}
		}

		public HabilitacaoEmissaoPTV()
		{
			Funcionario = new ModuloFuncionario.Funcionario();
			Profissao = new Profissao();
			Telefones = new List<Contato>();
			Endereco = new Endereco();
			Arquivo = new Blocos.Arquivo.Arquivo(); //Arquivo.Arquivo();
			Operadores = new List<HabilitacaoEmissaoPTVOperador>();
		}
	}
}