using System;
using System.Collections.Generic;
using System.Web;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloAnaliseItens;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAnaliseItens.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloRequerimento.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloAnaliseItens.Business
{
	public class AnaliseItensValidar
	{
		#region Propriedades

		ProtocoloBus _busProtocolo = new ProtocoloBus();
		RequerimentoBus _busRequerimento = new RequerimentoBus();
		AnaliseItensDa _daAnalise = new AnaliseItensDa();

		private static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		#endregion

		public bool Salvar(AnaliseItem analise, bool atualizarRoteiro = false)
		{
			bool finalizar = true;

			//Validação proposta pela analista
			if (atualizarRoteiro)
			{
				foreach (var item in analise.Itens)
				{
					if (item.Situacao == (int)eAnaliseItemSituacao.Recebido ||
						item.Situacao == (int)eAnaliseItemSituacao.Pendente ||
						item.Situacao == (int)eAnaliseItemSituacao.Reprovado)
					{
						finalizar = false;
						break;
					}
				}

				analise.Situacao = finalizar ? (int)eAnaliseSituacao.Finalizado : (int)eAnaliseSituacao.EmAndamento;

				return true;
			}

			bool isAnalisado = false;

			AnaliseItem analiseBanco = _daAnalise.ObterSimplificado(analise.Id);

			if (analiseBanco.Situacao == (int)eAnaliseItemSituacao.Pendente)
			{
				Validacao.Add(Mensagem.AnaliseItem.ExistePendencia(analise.Protocolo.Id.HasValue));
			}

			if (!ValidarSituacaoAtividadeProtocolo(analise))
			{
				return Validacao.EhValido;
			}

			if (!_busProtocolo.EmPosse(analise.Protocolo.Id.GetValueOrDefault()))
			{
				Validacao.Add(Mensagem.AnaliseItem.ProtocoloSemPosse);
				return false;
			}


			foreach (var item in analise.Itens)
			{
				if (item.Tipo <= 0)
				{
					Validacao.Add(Mensagem.AnaliseItem.AnaliseItemTipoObrigatorio);
				}

				if (string.IsNullOrWhiteSpace(item.Nome))
				{
					Validacao.Add(Mensagem.AnaliseItem.AnaliseItemNomeObrigatorio);
				}

				if (item.Situacao <= (int)eAnaliseItemSituacao.Nulo)
				{
					Validacao.Add(Mensagem.AnaliseItem.AnaliseItemSituacaoObrigatorio(item.Nome));
				}

				switch (item.Situacao)
				{
					case (int)eAnaliseItemSituacao.Reprovado:
					case (int)eAnaliseItemSituacao.Dispensado:
						MotivoItem(item);
						break;
				}

				if (item.Situacao != (int)eAnaliseItemSituacao.Recebido)
				{
					isAnalisado = true;
				}

				if (item.Situacao == (int)eAnaliseItemSituacao.Recebido ||
					item.Situacao == (int)eAnaliseItemSituacao.Pendente ||
					item.Situacao == (int)eAnaliseItemSituacao.Reprovado)
				{
					finalizar = false;
				}
			}

			analise.Situacao = finalizar ? (int)eAnaliseSituacao.Finalizado : (int)eAnaliseSituacao.EmAndamento;

			if (!isAnalisado)
			{
				Validacao.Add(Mensagem.AnaliseItem.ItemAnalisadoObrigatorio);
			}

			return Validacao.EhValido;
		}

		#region Auxiliares

		public bool Analise(AnaliseItem analise)
		{
			if (analise != null && analise.Situacao == (int)eAnaliseItemSituacao.Pendente)
			{
				Validacao.Add(Mensagem.AnaliseItem.AnaliseSituacaoComPendencia);
			}

			if (!_busProtocolo.EmPosse(analise.Protocolo.Id.GetValueOrDefault()))
			{
				Validacao.Add(Mensagem.AnaliseItem.ProtocoloSemPosse);
				return false;
			}

			return Validacao.EhValido;
		}

		internal bool VerificarProtocolo(ProtocoloNumero protocolo)
		{
			if (String.IsNullOrWhiteSpace(protocolo.NumeroTexto))
			{
				Validacao.Add(Mensagem.AnaliseItem.NumeroObrigatorio);
				return false;
			}

			if (!ValidacoesGenericasBus.ValidarMaskNumeroBarraAno(protocolo.NumeroTexto))
			{
				Validacao.Add(Mensagem.AnaliseItem.NumeroInvalido);
				return false;
			}

			if (protocolo.Id == 0)
			{
				Validacao.Add(Mensagem.AnaliseItem.NumeroInexistente);
				return false;
			}

			string retorno = _busProtocolo.VerificarProtocoloAssociadoNumero(_busProtocolo.ExisteProtocolo(protocolo.NumeroTexto));

			if (!String.IsNullOrEmpty(retorno))
			{
				Validacao.Add(protocolo.IsProcesso ? Mensagem.AnaliseItem.ProcessoApensado(retorno) : Mensagem.AnaliseItem.DocumentoJuntado(retorno));
				return false;
			}

			if (!_busProtocolo.EmPosse(protocolo.Id))
			{
				Validacao.Add(Mensagem.AnaliseItem.ProtocoloSemPosse);
				return false;
			}

			if (!_busProtocolo.ExisteRequerimento(protocolo.Id, exibirMsg: false))
			{
				Validacao.Add(Mensagem.AnaliseItem.ExisteRequerimento);
				return false;
			}

			return Validacao.EhValido;
		}

		internal bool Itens(List<Item> itens)
		{
			for (int i = 0; i < itens.Count; i++)
			{
				if (itens[i].Tipo <= 0)
				{
					Validacao.Add(Mensagem.AnaliseItem.AnaliseItemTipoObrigatorio);
				}

				if (string.IsNullOrWhiteSpace(itens[i].Nome))
				{
					Validacao.Add(Mensagem.AnaliseItem.AnaliseItemNomeObrigatorio);
				}

				if (string.IsNullOrWhiteSpace(itens[i].ProcedimentoAnalise))
				{
					Validacao.Add(Mensagem.AnaliseItem.AnaliseItemProcedimentoAnaliseObrigatorio);
				}
			}

			return Validacao.EhValido;
		}

		private void MotivoItem(Item item)
		{
			if (string.IsNullOrWhiteSpace(item.Motivo))
			{
				Validacao.Add(Mensagem.AnaliseItem.MotivoObrigatorio);
			}
			else if (item.Motivo.Length > 4000)
			{
				Validacao.Add(Mensagem.AnaliseItem.MotivoMaximoCaracter(item.Nome, 4000));
			}
		}

		public static bool ValidarData(String data)
		{
			try
			{
				DateTime.Parse(data).ToString("dd/MM/yyyy");
				return true;
			}
			catch
			{
				return false;
			}
		}

		internal void ValidarSituacaoRoteiro(List<Roteiro> analiseRoteiros)
		{
			for (int i = 0; i < analiseRoteiros.Count; i++)
			{
				if (analiseRoteiros[i].Situacao == 2)
				{
					Validacao.Add(Mensagem.AnaliseItem.RoteiroDesativado(analiseRoteiros[i].Numero));
				}
			}
		}

		internal bool ValidarSituacaoAtividadeProtocolo(AnaliseItem analise)
		{
			List<Atividade> atividades = _busProtocolo.ObterListAtividadesProtocolo(analise.Protocolo.Id.Value);

			return ValidarSituacaoAtividadeProtocolo(atividades);
		}

		internal bool ValidarSituacaoAtividadeProtocolo(List<Atividade> atividades)
		{
			if (atividades != null && atividades.Count > 0)
			{
				bool todasEncerradas = true;
				foreach (Atividade atividade in atividades)
				{
					if (atividade.SituacaoId != (int)eAtividadeSituacao.Encerrada)
					{
						todasEncerradas = false;
						break;
					}
				}

				if (todasEncerradas)
				{
					Validacao.Add(Mensagem.AnaliseItem.AtividadeRequerimentoEncerrada);
				}
			}

			return Validacao.EhValido;
		}

		public bool ValidarProtocoloAnalisar(int checagemId, int requerimentoId, bool isProcesso)
		{
			if (checagemId == 0)
			{
				Validacao.Add(Mensagem.AnaliseItem.AnaliseProtocoloSemChecagem(isProcesso));
				return Validacao.EhValido;
			}

			if (_daAnalise.ExisteTituloPendencia(requerimentoId))
			{
				Validacao.Add(Mensagem.AnaliseItem.AnaliseSituacaoComPendencia);
				return Validacao.EhValido;
			}

			List<Atividade> atividades = _busProtocolo.ObterProtocoloAtividades(requerimentoId);
			return ValidarSituacaoAtividadeProtocolo(atividades);
		}

		public void ValidarVersaoRoteiro(int checagemId)
		{
			int analiseId = _daAnalise.ExisteAnalise(checagemId);

			if (analiseId <= 0) return;

			AnaliseItem analise = _daAnalise.Obter(analiseId);

			for (int i = 0; i < analise.Roteiros.Count; i++)
			{
				if (analise.Roteiros[i].Versao != analise.Roteiros[i].VersaoAtual)
				{
					Validacao.Add(Mensagem.AnaliseItem.VersaoAtualizada(analise.Roteiros[i].Numero, analise.Roteiros[i].VersaoAtual, analise.Roteiros[i].Versao, analise.Roteiros[i].DataAtualizacao));
				}
			}
		}

		#endregion
	}
}