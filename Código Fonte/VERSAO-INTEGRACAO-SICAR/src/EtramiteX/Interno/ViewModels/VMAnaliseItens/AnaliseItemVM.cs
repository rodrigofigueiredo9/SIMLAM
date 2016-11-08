using System;
using System.Linq;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.ModuloAnaliseItens;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMAnaliseItens
{
	public class AnaliseItemVM
	{
		public int Situacao { set; get; }
		public int CheckListId { set; get; }
		public bool Atualizado { get; set; }
		public string ProtocoloNumero { get; set; }
		public int RequerimentoSelecionado { get; set; }
		public int AnaliseId { get; set; }

		public bool IsPendente { get { return Situacao == (int)eAnaliseSituacao.ComPendencia; } }

		public Int32 ProtocoloId { get; set; }
		public Int32 ProjetoDigitalId { get; set; }
		public Boolean IsProjetoDigitalImportado { get; set; }

		public Boolean ExisteItensPendentes
		{
			get
			{
				return ListarItens.Exists(x => x.Situacao == (int)eAnaliseItemSituacao.Pendente);
			}
		}

		public bool ImportarDados
		{
			get
			{
				Requerimento requerimento = Requerimentos.FirstOrDefault(x=> x.Id == RequerimentoSelecionado);

				return (!ItensProjetoDigital.Exists(x => x.Situacao == (int)eAnaliseItemSituacao.Pendente || 
						x.Situacao == (int)eAnaliseItemSituacao.Recebido)) &&
						requerimento.EtapaImportacao != (int)eProjetoDigitalEtapaImportacao.Finalizado;
			}
		}
		
		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					ItemExistente = Mensagem.Roteiro.ItemExistente,
					ItemObrigatorio = Mensagem.Roteiro.ItemObrigatorio,
					ItemEditadoSucesso = Mensagem.Item.ItemEditado,
					ItemAdicionado = Mensagem.Item.ItemAdicionado,
					ItemEditado = Mensagem.Item.Editar,
					Editar = Mensagem.AnaliseItem.Editar,
					ItensAtualizados = Mensagem.AnaliseItem.ItensAtualizados,
					ItemJaAdicionado = Mensagem.Item.ItemJaAdicionado,
					MotivoObrigatorio = Mensagem.AnaliseItem.MotivoObrigatorio,
					SituacaoObrigatorio = Mensagem.AnaliseItem.SituacaoObrigatorio,
					ImportarProjetoDigital = Mensagem.AnaliseItem.ImportarProjetoDigital,
					SairSemSalvarDados = Mensagem.AnaliseItem.SairSemSalvarDados,
					SalvarDadosObrigatorio = Mensagem.AnaliseItem.SalvarDadosObrigatorio
				});
			}
		}

		public String Tipos
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					Administrativo = (int)eRoteiroItemTipo.Administrativo,
					Tecnico = (int)eRoteiroItemTipo.Tecnico,
					ProjetoDigital = (int)eRoteiroItemTipo.ProjetoDigital
				});
			}
		}

		public String SituacoesItem
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					Aprovado = (int)eAnaliseItemSituacao.Aprovado,
					AprovadoImportacao = (int)eAnaliseItemSituacao.AprovadoImportacao,
					Dispensado = (int)eAnaliseItemSituacao.Dispensado,
					Pendente = (int)eAnaliseItemSituacao.Pendente,
					Recebido = (int)eAnaliseItemSituacao.Recebido,
					Reprovado = (int)eAnaliseItemSituacao.Reprovado
				});
			}
		}

		public String UrlsCaracterizacoes { get; set; }

		private List<Requerimento> _requerimentos = new List<Requerimento>();
		public List<Requerimento> Requerimentos { set { _requerimentos = value; } get { return _requerimentos; } }

		private List<Roteiro> _roteiros = new List<Roteiro>();
		public List<Roteiro> Roteiros { get { return _roteiros; } set { _roteiros = value; } }

		List<Item> _listarItens= new List<Item>();
		public List<Item> ListarItens { set { _listarItens = value; } get { return _listarItens; } }

		public List<Item> ItensAdmin { get { return ListarItens.FindAll(x => x.Tipo == (int)eRoteiroItemTipo.Administrativo); } }

		public List<Item> ItensTecnico { get { return ListarItens.FindAll(x => x.Tipo == (int)eRoteiroItemTipo.Tecnico); } }

		public List<Item> ItensProjetoDigital { get { return ListarItens.FindAll(x => x.Tipo == (int)eRoteiroItemTipo.ProjetoDigital); } }

		public AnaliseItemVM() { }
	}
}