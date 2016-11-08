using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloAgrotoxico;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV.HabilitacaoEmissao;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMHabilitacaoEmissaoPTV
{
	public class HabilitacaoEmissaoPTVVM
	{
		public bool IsVisualizar { get; set; }

		private HabilitacaoEmissaoPTV _habilitacao = new HabilitacaoEmissaoPTV();

		public HabilitacaoEmissaoPTV Habilitacao
		{
			get { return _habilitacao; }
			set { _habilitacao = value; }
		}

		public List<SelectListItem> UF { get; set; }

		public List<SelectListItem> Municipio { get; set; }

		public List<SelectListItem> OrgaoClasse { get; set; }

		public String TiposArquivoValido { get { return ViewModelHelper.Json(new ArrayList { ".pdf", ".jpg", ".gif", ".bmp" }); } }

		public string ArquivoJson 
		{
			get 
			{
				if (Habilitacao.Arquivo == null || Habilitacao.Arquivo.Id <=0)
				{
					return ""; 
				}

				return ViewModelHelper.Json(new { 
					@Id = Habilitacao.Arquivo.Id,
					@Raiz = Habilitacao.Arquivo.Raiz,
					@Nome = Habilitacao.Arquivo.Nome,
					@Extensao = Habilitacao.Arquivo.Extensao,
					@Caminho = Habilitacao.Arquivo.Caminho,
					@Diretorio = Habilitacao.Arquivo.Diretorio,
					@TemporarioPathNome=Habilitacao.Arquivo.TemporarioPathNome,
					@ContentType= Habilitacao.Arquivo.ContentType,
					@ContentLength = Habilitacao.Arquivo.ContentLength,
					@Tid = Habilitacao.Arquivo.Tid,
					@Apagar = Habilitacao.Arquivo.Apagar,
					@Conteudo = Habilitacao.Arquivo.Conteudo,
					@DiretorioConfiguracao = Habilitacao.Arquivo.DiretorioConfiguracao,
					@TemporarioNome = Habilitacao.Arquivo.TemporarioNome
				});
			}
		}

		public string Mensagens 
		{ 
			get 
			{
				return ViewModelHelper.Json(new { 
					@ArquivoInvalido = Mensagem.HabilitacaoEmissaoPTV.ArquivoInvalido,
					@OperadorAdicionado = Mensagem.HabilitacaoEmissaoPTV.OperadorAdicionado,
					@SituacaoFuncionarioDeveSerAtivo = Mensagem.HabilitacaoEmissaoPTV.SituacaoOperadorDeveEstarAtivo,
					@FuncionarioJaOperador = Mensagem.HabilitacaoEmissaoPTV.FuncionarioJaOperador,
					@Ativa = Mensagem.HabilitacaoEmissaoPTV.Ativada,
					@Desativada = Mensagem.HabilitacaoEmissaoPTV.Desativada
				});
			} 
		}

		public HabilitacaoEmissaoPTVVM(HabilitacaoEmissaoPTV habilitacao, List<Estado> listaUF, List<Municipio> listaMunicipio, List<OrgaoClasse> orgaoClasse, bool isVisualizar = false)
		{
			IsVisualizar = isVisualizar;
			this.Habilitacao = habilitacao;

			UF = ViewModelHelper.CriarSelectList(listaUF, null, true, habilitacao.Endereco.EstadoId.ToString());
			Municipio = ViewModelHelper.CriarSelectList(listaMunicipio, null, true, habilitacao.Endereco.MunicipioId.ToString());
			OrgaoClasse = ViewModelHelper.CriarSelectList(orgaoClasse, null, true, habilitacao.Profissao.OrgaoClasseId.ToString());
		}
	}
}