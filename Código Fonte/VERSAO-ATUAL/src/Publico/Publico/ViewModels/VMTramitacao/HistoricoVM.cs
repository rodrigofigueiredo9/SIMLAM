using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao;

namespace Tecnomapas.EtramiteX.Publico.ViewModels.VMTramitacao
{
	public class HistoricoVM
	{
		public List<SelectListItem> TipoTexto { get; set; }
		public List<Int32> AcaoHistoricoMostrarPdf { get; set; }
		public String Numero { get; set; }
		public Int32 TipoHistoricoId { get; set; } // 1 - processo
		public String TipoHistorico { get; set; }
		public string Localizacao { get; set; }

		public List<HistoricoVME> Historico { get; set; }

		public void CarregarHistorico(List<Tramitacao> hstTramitacao, Resultados<HistoricoProtocolo> ApensadoJuntado, string numero, string tipoTexto)
		{
			Numero = numero;
			TipoTexto = new List<SelectListItem>();
			TipoTexto.Add(new SelectListItem() { Text = tipoTexto });

			Historico = new List<HistoricoVME>();
			HistoricoVME historico;

			foreach (var item in hstTramitacao)
			{
				historico = new HistoricoVME();
				historico.Id = item.Id;
				historico.HistoricoId = item.HistoricoId;
				historico.SituacaoId = item.SituacaoId;
				historico.Remetente = item.Remetente.Nome;
				historico.RemetenteSetor = item.RemetenteSetor;
				historico.Destinatario = item.Destinatario.Nome;
				historico.DestinatarioSetor = item.DestinatarioSetor;
				historico.AcaoData = item.DataExecucao;
				historico.Acao = item.AcaoExecutada;
				historico.Objetivo = item.Objetivo.Texto;
				historico.IsProcesso = item.Protocolo.IsProcesso;
				
				//id da lov_historico_artefatos_acoes
				historico.MostrarPdf = (this.AcaoHistoricoMostrarPdf.Exists(x => x == item.AcaoId));

				Historico.Add(historico);
			}

			foreach (var item in ApensadoJuntado.Itens)
			{
				historico = new HistoricoVME();
				string acao = string.Empty;

				switch (item.AcaoEnumerado)
				{
					case eHistoricoAcao.apensar:
						acao = " apensado";

						historico.StrHistoricoTramitacao =
							item.ProtocoloTipoNome +
							acao +
							" ao processo " +
							item.Numero +
							" em " + item.AcaoData.Data.Value.ToString() +
							" por " + item.Setor.Sigla +
							" - " + item.Executor.Nome;

						break;

					case eHistoricoAcao.juntar:
						acao = " juntado";

						historico.StrHistoricoTramitacao =
							item.ProtocoloTipoNome +
							acao +
							" ao processo " +
							item.Numero +
							" em " + item.AcaoData.Data.Value.ToString() +
							" por " + item.Setor.Sigla +
							" - " + item.Executor.Nome;

						break;

					case eHistoricoAcao.desapensar:
						acao = " desapensado";

						historico.StrHistoricoTramitacao =
							item.ProtocoloTipoNome +
							acao +
							" do processo " +
							item.Numero +
							" em " + item.AcaoData.Data.Value.ToString() +
							" por " + item.Setor.Sigla +
							" - " + item.Executor.Nome;

						break;

					case eHistoricoAcao.desentranhar:
						acao = " desentranhado";

						historico.StrHistoricoTramitacao =
							item.ProtocoloTipoNome +
							acao +
							" do processo " +
							item.Numero +
							" em " + item.AcaoData.Data.Value.ToString() +
							" por " + item.Setor.Sigla +
							" - " + item.Executor.Nome;

						break;

					case eHistoricoAcao.converter:
						acao = " convertido";

						string protocoloDestino = item.ProtocoloTipoNome;
						item.ProtocoloTipoId = item.ProtocoloTipoId == 2 ? 1 : 2;
						string protocoloOrigem = item.ProtocoloTipoNome;

						historico.StrHistoricoTramitacao =
							protocoloOrigem + " foi " +
							acao +
							" em " + protocoloDestino.ToLower() + " " +
							item.Numero +
							" em " + item.AcaoData.Data.Value.ToString() +
							" por " + item.Setor.Sigla +
							" - " + item.Executor.Nome;

						break;

				}

				historico.AcaoData = item.AcaoData;
				/*
						historico.StrHistoricoTramitacao =
							TipoHistorico +
							acao +
							((item.AcaoEnumerado == eHistoricoAcao.apensar || item.AcaoEnumerado == eHistoricoAcao.juntar) ? " ao processo " : " do processo ") +
							item.ApensadorNumero +
							" em " + item.AcaoData.Data.Value.ToString() +
							" por " + item.Setor.Sigla +
							" - " + item.Executor.Nome;
				 
				 */
				Historico.Add(historico);
			}

			Historico = Historico.OrderByDescending(p => p.AcaoData.Data.Value).ToList();
		}
	}
}