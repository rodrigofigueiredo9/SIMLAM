using System;
using System.Collections;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao
{
	public class ConsideracaoFinalVM
	{
		public ConsideracaoFinal ConsideracaoFinal { get; set; }		
		public bool IsVisualizar { get; set; }
		private List<ConsideracaoFinalTestemunhaVM> _consideracaoFinalTestemunhaVM;
		public List<ConsideracaoFinalTestemunhaVM> ConsideracaoFinalTestemunhaVM { get { return _consideracaoFinalTestemunhaVM; } }
		public ArquivoVM ArquivoVM { get; set; }

        public ArquivoVM ArquivoIUFVM { get; set; }
		public FiscalizacaoAssinanteVM AssinantesVM { get; set; }

		public ConsideracaoFinalVM()
		{
			ConsideracaoFinal = new ConsideracaoFinal();
			_consideracaoFinalTestemunhaVM = new List<ConsideracaoFinalTestemunhaVM>(2);
			_consideracaoFinalTestemunhaVM.Add(new ConsideracaoFinalTestemunhaVM { Testemunha = new ConsideracaoFinalTestemunha { Colocacao = 1 } });
			_consideracaoFinalTestemunhaVM.Add(new ConsideracaoFinalTestemunhaVM { Testemunha = new ConsideracaoFinalTestemunha { Colocacao = 2 } });
			ArquivoVM = new ArquivoVM();
            ArquivoIUFVM = new ArquivoVM();
			ArquivoJSon = string.Empty;
			AssinantesVM = new FiscalizacaoAssinanteVM();
		}

		public String GetJson(object obj)
		{
			return ViewModelHelper.Json(obj);
		}

		public String ArquivoJSon { get; set; }
		public String TiposArquivoValido { get { return ViewModelHelper.Json(new ArrayList { ".pdf" }); } }
		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@ArquivoObrigatorio = Mensagem.ConsideracaoFinalMsg.ArquivoObrigatorio,
					@ArquivoNaoEhPdf = Mensagem.ConsideracaoFinalMsg.ArquivoNaoEhPdf,
					@AssinanteJaAdicionado = Mensagem.ConsideracaoFinalMsg.AssinanteJaAdicionado,
					@AssinanteObrigatorio = Mensagem.ConsideracaoFinalMsg.AssinanteObrigatorio,
					@AssinanteSetorObrigatorio = Mensagem.ConsideracaoFinalMsg.AssinanteSetorObrigatorio,
					@AssinanteFuncionarioObrigatorio = Mensagem.ConsideracaoFinalMsg.AssinanteFuncionarioObrigatorio,
					@AssinanteCargoObrigatorio = Mensagem.ConsideracaoFinalMsg.AssinanteCargoObrigatorio
				});
			}
		}
	}
}
