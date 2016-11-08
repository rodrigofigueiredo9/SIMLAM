using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Credenciado.ViewModels.VMRoteiro
{
	public class SalvarVM
	{
		public Int32? IdPdf { get; set; }
		public Boolean CadastrandoItens { get; set; }
		public Roteiro Roteiro { get; set; }
		public Item Item { get; set; }
		public Arquivo Arquivo { get; set; }
		public String ArquivoTexto { get; set; }
		public String Descricao { get; set; }
		public String PalavraChave { get; set; }
		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					ArquivoAnexoExistente = Mensagem.Roteiro.ArquivoAnexoExistente,
					ArquivoAnexoObrigatorio = Mensagem.Roteiro.ArquivoAnexoObrigatorio,
					DescricaoAnexoObrigatorio = Mensagem.Roteiro.DescricaoAnexoObrigatorio,
					ItemExistente = Mensagem.Roteiro.ItemExistente,
					ItemObrigatorio = Mensagem.Roteiro.ItemObrigatorio,
					ObrigatorioNome = Mensagem.Roteiro.ObrigatorioNome,
					ObrigatorioNumero = Mensagem.Roteiro.ObrigatorioNumero,
					ObrigatorioSetor = Mensagem.Roteiro.ObrigatorioSetor,
					PalavraChaveExistente = Mensagem.Roteiro.PalavraChaveExistente,
					PalavraChaveObrigatoria = Mensagem.Roteiro.PalavraChaveObrigatoria,
					ArquivoAnexoNaoEhPDF = Mensagem.Roteiro.ArquivoAnexoNaoEhPDF,
					TransferenciaAndamento = Mensagem.Arquivo.TransferenciaAndamento,
					ArquivoExistente = Mensagem.Arquivo.ArquivoExistente,
					ItemAdicionado = Mensagem.Item.ItemAdicionado,
					ItemEditado = Mensagem.Item.Editar,
					@ItemEditadoSucesso = Mensagem.Item.ItemEditado,
					@AtividadeJaAdicionado = Mensagem.AtividadeConfiguracao.AtividadeJaAssociada,
					@AtividadeAssociada = Mensagem.AtividadeConfiguracao.AtividadeAssociada,
					@RoteiroDesativo = Mensagem.Roteiro.RoteiroDesativo
				});
			}
		}
		public Boolean IsVisualizar { get; set; }

		public List<SelectListItem> Setores { get; set; }
		
		private List<Finalidade> _finalidades = new List<Finalidade>();
		public List<Finalidade> Finalidades
		{
			get { return _finalidades; }
			set { _finalidades = value; }
		}
		
		public SalvarVM() 
		{
			Roteiro = new Roteiro();
			Item = new Item();
			Arquivo = new Arquivo();
		}

		public SalvarVM(Roteiro roteiro, List<Finalidade> finalidades) 
		{
			this.Roteiro = roteiro;
			Finalidades = finalidades;
		}

		public SalvarVM(List<Setor> setores, List<Finalidade> finalidades)
		{
			Roteiro = new Roteiro();
			Item = new Item();
			Item.Tipo = 1;
			Arquivo = new Arquivo();
			Setores = ViewModelHelper.CriarSelectList(setores, true);
			PalavraChave = "";
			Finalidades = finalidades;
		}

		public SalvarVM(List<Setor> setores, Roteiro roteiro, List<Finalidade> finalidades)
		{
			Setores = ViewModelHelper.CriarSelectList(setores, true);
			Finalidades = finalidades;
			Roteiro = roteiro;
		}
	}
}