using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragem;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels
{
	public class BarragemVM
	{
		public Barragem Barragem { get; set; }
		public List<SelectListItem> Atividades { set; get; }

		public String TextoAbrirModal { get; set; }
		public String TextoMerge { get; set; }
		public string AtualizarDependenciasModalTitulo { get; set; }
		public bool TemARL { get; set; }
		public bool TemARLDesconhecida { get; set; }

		public bool IsVisualizar { get; set; }
		public bool IsEditar { get; set; }

		public string Mensagens
		{
			get
			{
				return this.GetJSON(new
				{
					@SelecioneAtividade = Mensagem.BarragemMsg.SelecioneAtividade,
					@AddBarragem = Mensagem.BarragemMsg.AddBarragem,
					@AddBarragemDadosItem = Mensagem.BarragemMsg.AddBarragemDadosItem,
					@InformeQuantidade = Mensagem.BarragemMsg.InformeQuantidade,
					@QuantidadeInvalida = Mensagem.BarragemMsg.QuantidadeInvalida,
					@InformeQuantidadeZero = Mensagem.BarragemMsg.InformeQuantidadeZero,
					@SelecioneFinalidade = Mensagem.BarragemMsg.SelecioneFinalidade,
					@InformeOutroFinalidade = Mensagem.BarragemMsg.InformeOutroFinalidade,
					@InformeLamina = Mensagem.BarragemMsg.InformeLamina,
					@InformeArmazenado = Mensagem.BarragemMsg.InformeArmazenado,
					@SelecioneOutorga = Mensagem.BarragemMsg.SelecioneOutorga,
					@InformeNumero = Mensagem.BarragemMsg.InformeNumero,
					@Salvar = Mensagem.BarragemMsg.Salvar,
					@Excluir = Mensagem.BarragemMsg.Excluir,
					@ExcluirMensagem = Mensagem.BarragemMsg.ExcluirMensagem,
					@SemARLConfirm = Mensagem.BarragemMsg.SemARLConfirm,
					@ARLDesconhecidaConfirm = Mensagem.BarragemMsg.ARLDesconhecidaConfirm,
                    @BarragemNaoSalva = Mensagem.BarragemMsg.BarragemNaoSalva
				});
			}
		}

		public BarragemVM()
		{
			this.Barragem = new Barragem();
			this.Atividades = new List<SelectListItem>();
		}

		public String GetJSON(object obj)
		{
			return ViewModelHelper.Json(obj);
		}
	}
}