using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Credenciado.ViewModels.VMPessoa
{
	public class PessoaVM
	{
		GerenciadorConfiguracao<ConfiguracaoEndereco> _configEnd = new GerenciadorConfiguracao<ConfiguracaoEndereco>(new ConfiguracaoEndereco());

		public Boolean CpfCnpjValido { get; set; }
		public Boolean IsVisualizar { get; set; }

		public String DataNascimento { get; set; }
		public int TipoCadastro { get; set; }
		public bool IsCredenciado { get; set; }
		public bool IsAssociarConjuge { get; set; }
		public bool IsConjuge { get; set; }
		public bool OcultarLimparPessoa { get; set; }
		public bool ExisteCredenciado { get; set; }
		public bool ExisteInterno { get; set; }
		public String UrlAcao { get; set; }
		public bool ExibirMensagensPartial { get; set; }		

		public Pessoa Pessoa { get; set; }
		public ContatoVME Contato { get; set; }

		private List<SelectListItem> _estadosCivis = new List<SelectListItem>();
		private List<SelectListItem> _sexos = new List<SelectListItem>();
		private List<SelectListItem> _profissoes = new List<SelectListItem>();
		private List<SelectListItem> _orgaoClasses = new List<SelectListItem>();
		private List<SelectListItem> _estados = new List<SelectListItem>();
		private List<SelectListItem> _municipios = new List<SelectListItem>();
		private List<SelectListItem> _portes = new List<SelectListItem>();

		public List<SelectListItem> EstadosCivis
		{
			get { return _estadosCivis; }
			set { _estadosCivis = value; }
		}

		public List<SelectListItem> Sexos
		{
			get { return _sexos; }
			set { _sexos = value; }
		}

		public List<SelectListItem> Profissoes
		{
			get { return _profissoes; }
			set { _profissoes = value; }
		}

		public List<SelectListItem> OrgaoClasses
		{
			get { return _orgaoClasses; }
			set { _orgaoClasses = value; }
		}

		public List<SelectListItem> Estados
		{
			get { return _estados; }
			set { _estados = value; }
		}

		public List<SelectListItem> Municipios
		{
			get { return _municipios; }
			set { _municipios = value; }
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					RepresentanteExistente = Mensagem.Pessoa.RepresentanteExistente,
					PessoaConjugeSaoIguais = Mensagem.Pessoa.PessoaConjugeSaoIguais
				});
			}
		}

		public PessoaVM()
		{
			Pessoa = new Pessoa();
			Contato = new ContatoVME();
		}

		public PessoaVM(List<EstadoCivil> estadosCivis, List<Sexo> sexos, List<ProfissaoLst> profissoes, List<OrgaoClasse> orgaoClasses, List<Estado> estados)
		{
			Pessoa = new Pessoa();
			Contato = new ContatoVME();

			EstadosCivis = ViewModelHelper.CriarSelectList(estadosCivis, true);
			Sexos = ViewModelHelper.CriarSelectList(sexos, true);
			Profissoes = ViewModelHelper.CriarSelectList(profissoes, true);
			OrgaoClasses = ViewModelHelper.CriarSelectList(orgaoClasses, true);
			Estados = ViewModelHelper.CriarSelectList(estados, true);
		}

		public bool MostarConjuge(int? estadoCivilId)
		{
			return estadoCivilId.HasValue && (estadoCivilId == 2 || estadoCivilId == 5);
		}

		public void CarregarMunicipios()
		{
			if (Pessoa.Endereco.EstadoId > 0)
			{
				Municipios = ViewModelHelper.CriarSelectList(_configEnd.Obter<Dictionary<int, List<Municipio>>>(ConfiguracaoEndereco.KeyMunicipios)[Pessoa.Endereco.EstadoId], true);
			}
			else
			{
				Municipios = ViewModelHelper.CriarSelectList(new List<Municipio>(), true);
			}
		}

		public bool OcultarIsCopiado { get; set; }
	}
}