using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.ViewModels;

namespace Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels.VMDominialidade
{
	public class DominioVM
	{
		public Boolean IsVisualizar { get; set; }

		private Dominio _dominio = new Dominio();
		public Dominio Dominio
		{
			get { return _dominio; }
			set { _dominio = value; }
		}

		private List<SelectListItem> _comprovacoes = new List<SelectListItem>();
		public List<SelectListItem> Comprovacoes
		{
			get { return _comprovacoes; }
			set { _comprovacoes = value; }
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@ReservaLegalExcluir = Mensagem.Dominialidade.ReservaLegalExcluir
				});
			}
		}

		public String IdsTela
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@DominioComprovacaoPosseiroPrimitivo = eDominioComprovacao.PosseiroPrimitivo,
					@DominioComprovacaoRecibo = eDominioComprovacao.Recibo,
					@DominioComprovacaoCertidaoPrefeitura = eDominioComprovacao.CertidaoPrefeitura,
					@DominioComprovacaoContratoCompraVenda = eDominioComprovacao.ContratoCompraVenda,
					@DominioComprovacaoDeclaracao = eDominioComprovacao.Declaracao,
					@DominioComprovacaoOutros = eDominioComprovacao.Outros,
					@DominioCertificadoCadastroImovelRuralCCIR = eDominioComprovacao.CertificadoCadastroImovelRuralCCIR
				});
			}
		}

		public DominioVM(List<Lista> comprovacoes, Dominio dominio, int comprovacaoId = 0, bool isVisualizar = false)
		{
			Comprovacoes = ViewModelHelper.CriarSelectList(comprovacoes, isFiltrarAtivo: true, selecionado: comprovacaoId.ToString());

			Dominio = dominio;
			IsVisualizar = isVisualizar;
		}

		public string ObterTermoMatriculaCompensada(ReservaLegal reserva)
		{
			string retorno = string.Empty;

			if (!string.IsNullOrEmpty(reserva.NumeroTermo))
			{
				retorno = reserva.NumeroTermo;
			}

			if (!string.IsNullOrEmpty(reserva.MatriculaIdentificacao))
			{
				if (!string.IsNullOrEmpty(retorno))
				{
					retorno += "/" + reserva.MatriculaTexto;
				}
				else
				{
					retorno = reserva.MatriculaTexto;
				}
			}

			return retorno;
		}
	}
}