using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Interno.ModuloAnaliseItens;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.AnaliseItens.ViewModels
{
	public class PecaTecnicaVM
	{
		public PecaTecnica PecaTecnica { set; get; }

		public List<SelectListItem> RespEmpreendimento { get; set; }

		public List<SelectListItem> Setores { set; get; }

		public List<SelectListItem> Elaboradores { set; get; }

		public Requerimento Requerimento { get; set; }

		public List<Requerimento> Requerimentos { get; set; }

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@AtividadeObrigatorio = Mensagem.AnaliseItem.PecaTecnicaAtividadeObrigatorio,
					@RespEmpreendimentoObrigatorio = Mensagem.AnaliseItem.PecaTecnicaRespEmpreendimentoObrigatorio,
					@RespEmpreendimentoJaAdicionado = Mensagem.AnaliseItem.PecaTecnicaRespEmpreendimentoJaAdicionado,
					@ElaboradorObrigatorio = Mensagem.AnaliseItem.PecaTecnicaElaboradorObrigatorio,
					@SetorObrigatorio = Mensagem.AnaliseItem.PecaTecnicaSetorObrigatorio,
					@ProtocoloInvalido = Mensagem.AnaliseItem.NumeroInvalido
				});
			}
		}

		public String ElaboradorTipos
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@TecnicoIdaf = (int)eElaboradorTipo.TecnicoIdaf,
					@TecnicoTerceirizado = (int)eElaboradorTipo.TecnicoTerceirizado,
				});
			}
		}

		public PecaTecnicaVM(PecaTecnica pecaTecnica)
		{
			PecaTecnica = pecaTecnica;
			Elaboradores = new List<SelectListItem>() { new SelectListItem() { Text = " *** Selecione *** ", Value = "0" } };
			Setores = new List<SelectListItem>() { new SelectListItem() { Text = " *** Selecione *** ", Value = "0" } };
			RespEmpreendimento = new List<SelectListItem>() { new SelectListItem() { Text = " *** Selecione *** ", Value = "0" } };
		}
	}

}