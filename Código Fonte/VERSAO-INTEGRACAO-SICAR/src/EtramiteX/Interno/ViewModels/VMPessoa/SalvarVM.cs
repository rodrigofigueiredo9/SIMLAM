using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Etx.ModuloCore.View;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMPessoa
{
	public class SalvarVM
	{
		public Boolean CpfCnpjValido { get; set; }
		public Boolean IsVisualizar { get; set; }
		public String DataNascimento { get; set; }
		public ContatoVME Contato { get; set; }
		public int TipoCadastro { get; set; }

		private Pessoa _pessoa;
		private List<SelectListItem> _estadosCivis = new List<SelectListItem>();
		private List<SelectListItem> _sexos = new List<SelectListItem>();
		private List<SelectListItem> _profissoes = new List<SelectListItem>();
		private List<SelectListItem> _orgaoClasses = new List<SelectListItem>();
		private List<SelectListItem> _estados = new List<SelectListItem>();
		private List<SelectListItem> _municipios = new List<SelectListItem>();
		private List<SelectListItem> _portes = new List<SelectListItem>();

		public List<Sessao> PessoaInterno { get; set; }
		public List<Sessao> PessoaCredenciado { get; set; }

		public Pessoa Pessoa
		{
			get { return _pessoa; }
			set { _pessoa = value; }
		}
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

		public SalvarVM(List<EstadoCivil> estadosCivis, List<Sexo> sexos, List<ProfissaoLst> profissoes, List<OrgaoClasse> orgaoClasses, List<Estado> estados)
		{
			Pessoa = new Pessoa();
			Contato = new ContatoVME();
			PessoaInterno = new List<Sessao>();
			PessoaCredenciado = new List<Sessao>();
			EstadosCivis = ViewModelHelper.CriarSelectList(estadosCivis, true);
			Sexos = ViewModelHelper.CriarSelectList(sexos, true);
			Profissoes = ViewModelHelper.CriarSelectList(profissoes, true);
			OrgaoClasses = ViewModelHelper.CriarSelectList(orgaoClasses, true);
			Estados = ViewModelHelper.CriarSelectList(estados, true);
			IsVisualizar = false;
			ExibirLimparPessoa = true;
		}

		public SalvarVM()
		{
			Pessoa = new Pessoa();
			Contato = new ContatoVME();
			PessoaInterno = new List<Sessao>();
			PessoaCredenciado = new List<Sessao>();
			IsVisualizar = false;
			ExibirLimparPessoa = true;
		}

		public String UrlAcao { get; set; }
		public bool ExibirBotoes { get; set; }
		public bool ExibirMensagensPartial { get; set; }
		public bool ExibirLimparPessoa { get; set; }

		public bool MostarConjuge(int? estadoCivilId)
		{
			return estadoCivilId.HasValue && (estadoCivilId == 2 || estadoCivilId == 5);
		}

		public bool IsCredenciado { get; set; }

		public bool OcultarLimparPessoa { get; set; }
	}
}