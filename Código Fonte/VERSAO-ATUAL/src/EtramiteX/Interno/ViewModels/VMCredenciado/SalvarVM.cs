using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMPessoa;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMCredenciado
{
	public class SalvarVM
	{
		public ContatoVME Contatos { get; set; }

		private CredenciadoPessoa _credenciado = new CredenciadoPessoa();
		public CredenciadoPessoa Credenciado
		{
			get { return _credenciado; }
			set
			{
				_credenciado = value;
				CarregaMeiosContatos(_credenciado.Pessoa.MeiosContatos); 
			}
		}
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
		public List<SelectListItem> Portes
		{
			get { return _portes; }
			set { _portes = value; }
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					RepresentanteExistente = Mensagem.Pessoa.RepresentanteExistente,
					RegerarChave = Mensagem.Credenciado.RegerarChave
				});
			}
		}

		public SalvarVM(List<EstadoCivil> estadosCivis, List<Sexo> sexos, List<ProfissaoLst> profissoes, List<OrgaoClasse> orgaoClasses, List<Estado> estados)
		{
			EstadosCivis = ViewModelHelper.CriarSelectList(estadosCivis, true);
			Sexos = ViewModelHelper.CriarSelectList(sexos, true);
			Profissoes = ViewModelHelper.CriarSelectList(profissoes, true);
			OrgaoClasses = ViewModelHelper.CriarSelectList(orgaoClasses, true);
			Estados = ViewModelHelper.CriarSelectList(estados, true);
		}

		private void CarregaMeiosContatos(List<Contato> meiosContatos)
		{
			Contatos = new ContatoVME();
			try
			{
				foreach (eTipoContato tipoContato in Enum.GetValues(typeof(eTipoContato)))
				{
					Contato contato = meiosContatos.FirstOrDefault(x => x.TipoContato == tipoContato);
					if (contato == null) continue;
					switch (tipoContato)
					{
						case eTipoContato.TelefoneResidencial:
							Contatos.TelefoneResidencial = contato.Valor;
							break;
						case eTipoContato.TelefoneComercial:
							Contatos.TelefoneComercial = contato.Valor;
							break;
						case eTipoContato.TelefoneCelular:
							Contatos.TelefoneCelular = contato.Valor;
							break;
						case eTipoContato.TelefoneFax:
							Contatos.TelefoneFax = contato.Valor;
							break;
						case eTipoContato.Email:
							Contatos.Email = contato.Valor;
							break;
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddAdvertencia(exc.Message);
			}
		}

		public void CarregaMunicipios(List<Municipio> municipios)
		{
			Municipios = ViewModelHelper.CriarSelectList(municipios, true);
		}

		public SalvarVM()
		{
		}
	}
}