using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno;

namespace Tecnomapas.EtramiteX.Credenciado.ViewModels.VMDUA
{
	public class DUAVM
	{
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		private Titulo _titulo = new Titulo();
		public Titulo Titulo { get { return _titulo; } set { _titulo = value; } }

		public List<SelectListItem> LstModelos { get; set; }
		public List<SelectListItem> LstLocalEmissao { get; set; }
		public List<SelectListItem> LstSetores { get; set; }

		public string LabelTipoPrazo { get; set; }
		public bool TemEmpreendimento { get; set; }
		public bool SetoresEditar { get; set; }
		public bool IsEditar { get; set; }
		public bool IsVisualizar { get; set; }
		public bool CarregarEspecificidade { get; set; }

		public Int32? ArquivoId { get; set; }
		public String ArquivoTexto { get; set; }
		public String ArquivoJSon { get; set; }
		public String AtividadeEspecificidadeCaracterizacaoJSON { get; set; }
		public String AtividadesIDJSON
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					BarragemID = ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.Barragem)
				});
			}
		}


		public bool IsExibirAnexoTituloPdf
		{
			get
			{
				if (Modelo.Regras == null || Modelo.Regras.Count == 0)
				{
					return false;
				}
				return !Modelo.Regra(eRegra.PdfGeradoSistema);
			}
		}

		
		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					ProcDocSemEmpAssociado = Mensagem.Titulo.ProcDocSemEmpAssociado,
					ArquivoObrigatorio = Mensagem.Titulo.ArquivoObrigatorio,
					ArquivoTipoPdf = Mensagem.Titulo.ArquivoTipoPdf,
					AssinanteJaAdicionado = Mensagem.Titulo.AssinanteJaAdicionado,
					AssinanteObrigatorio = Mensagem.Titulo.AssinanteObrigatorio,
					AssinanteSetorObrigatorio = Mensagem.Titulo.AssinanteSetorObrigatorio,
					AssinanteFuncionarioObrigatorio = Mensagem.Titulo.AssinanteFuncionarioObrigatorio,
					AssinanteCargoObrigatorio = Mensagem.Titulo.AssinanteCargoObrigatorio,
					RequerimentoSemEmpreendimento = Mensagem.Titulo.RequerimentoSemEmpreendimento,
				});
			}
		}

		

		private TituloModelo _modelo = new TituloModelo();
		public TituloModelo Modelo
		{
			get { return _modelo; }
			set { _modelo = value; }
		}

		public DUAVM()
		{
		}

		public void SetaSetores(List<Setor> setores, int setorSelecionado = 0)
		{
			LstSetores = ViewModelHelper.CriarSelectList(setores, true, (setores.Count > 1), setorSelecionado.ToString());
		}

	
	}
}