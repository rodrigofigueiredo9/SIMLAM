using System;
using System.Collections.Generic;
using System.Web;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloChecagemRoteiro;
using Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAtividade.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAtividade.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloChecagemRoteiro.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProcesso.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloRoteiro.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloRoteiro.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloChecagemRoteiro.Business
{
	public class ChecagemValidar
	{
		RoteiroBus _bus = new RoteiroBus();
		ChecagemRoteiroDa _da = new ChecagemRoteiroDa();
		RoteiroDa _daRoteiro = new RoteiroDa();
		ChecagemRoteiroMsg Msg = new ChecagemRoteiroMsg();
		AtividadeDa _daAtividade = new AtividadeDa();
		private EtramitePrincipal User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal); }
		}

		public bool Salvar(ChecagemRoteiro checkListRoteiro)
		{
			if (VerificarChecagem(checkListRoteiro))
			{
				// Todos itens devem estar marcados ou dispensados
				if (!User.IsInRole(ePermissao.ChecagemItemRoteiroCriarPendente.ToString()) && checkListRoteiro.Itens.Exists(x => x.Situacao == 1))
				{
					Validacao.AddAdvertencia(Mensagem.ChecagemRoteiro.ItemPendente.Texto);
				}
			}

			return Validacao.EhValido;
		}

		private bool VerificarChecagem(ChecagemRoteiro checkListRoteiro)
		{
			if (String.IsNullOrWhiteSpace(checkListRoteiro.Interessado))
			{
				Validacao.AddAdvertencia(Msg.NomeInteressadoObrigatorio.Texto);
			}

			if (checkListRoteiro.Roteiros == null || checkListRoteiro.Roteiros.Count <= 0)
			{
				Validacao.AddAdvertencia(Msg.RoteiroObrigatorio.Texto);
			}

			// checa se as atividades dos roteiros estão no mesmo setor
			// apenas está checando os setor de cada roteiro pois isto já basta
			if (!_daRoteiro.RoteirosMesmoSetor(checkListRoteiro.Roteiros))
			{
				Validacao.Add(Mensagem.ChecagemRoteiro.RoteiroSetorDiferente);
			}

			int ri = 0;
			foreach (var item in checkListRoteiro.Roteiros)
			{
				ri++;
				Roteiro roteiro = _bus.ObterSimplificado(item.Id, item.Tid);

				if (roteiro == null)
				{
					Validacao.AddAdvertencia(Msg.RoteiroInvalidoNaoEncontrado.Texto);
					return Validacao.EhValido;
				}

				if (_daRoteiro.RoteiroDesativado(roteiro.Id))
				{
					Validacao.Add(Msg.RoteiroDesativado(roteiro.Numero, eTipoMensagem.Advertencia));
					return Validacao.EhValido;
				}

				if (!VerificarAtividadesAtivadas(roteiro.Id))
				{
					return Validacao.EhValido;
				}

				if (!_daRoteiro.VerificarTidAtual(roteiro.Id, roteiro.Tid))
				{
					Validacao.AddAdvertencia(Msg.RoteiroAtualizado(roteiro.Numero, roteiro.VersaoAtual.ToString()).Texto);
					return Validacao.EhValido;
				}

				var existe = false;

				foreach (var itemRoteiro in roteiro.Itens)
				{
					foreach (var itemCheckList in checkListRoteiro.Itens)
					{
						if (itemCheckList.Nome.ToUpper().Trim().Equals(itemRoteiro.Nome.ToUpper().Trim()))
						{
							existe = true;
							break;
						}

						if (itemCheckList.Situacao == 3 && string.IsNullOrWhiteSpace(itemCheckList.Motivo))
						{
							Validacao.Add(Mensagem.ChecagemRoteiro.MotivoObrigatorio(itemCheckList.Nome));
							break;
						}
					}

					if (!existe)
					{
						Validacao.AddAdvertencia(Msg.ItemRoteiroNaoAssociado(roteiro.Numero, itemRoteiro.Nome).Texto);
						return Validacao.EhValido;
					}
				}

				foreach (var itemCheckList in checkListRoteiro.Itens)
				{
					if (itemCheckList.Situacao == 3 && string.IsNullOrWhiteSpace(itemCheckList.Motivo))
					{
						Validacao.Add(Mensagem.ChecagemRoteiro.MotivoObrigatorio(itemCheckList.Nome));
						break;
					}
				}

			}
			return Validacao.EhValido;
		}

		public bool Excluir(int id)
		{
			List<String> listString = new List<string>();
			listString = _da.ObterAssociacoesCheckList(id);

			if (listString != null && listString.Count > 0)
			{
				listString.ForEach(x => Validacao.Add(Msg.ErroExcluir(x)));
			}

			return Validacao.EhValido;
		}

		internal bool ValidarCheckListRoteiroPdf(ChecagemRoteiro checkListRoteiro)
		{
			if (String.IsNullOrWhiteSpace(checkListRoteiro.Interessado))
			{
				Validacao.Add(Mensagem.ChecagemRoteiro.NomeInteressadoObrigatorio);
			}

			return Validacao.EhValido;
		}

		internal bool ValidarAssociarCheckList(int checkListId, int idDiferente, bool isProcesso, bool isConversao = false)
		{
			if (checkListId != 0)
			{
				ChecagemRoteiro checkList = _da.Obter(checkListId);

				if (idDiferente != 0)
				{
					if (isProcesso)
					{
						ProcessoBus _bus = new ProcessoBus();
						if (_bus.Obter(idDiferente).ChecagemRoteiro.Id == checkListId)
							return true;
					}
					else
					{
						DocumentoBus _bus = new DocumentoBus();
						if (_bus.Obter(idDiferente).ChecagemRoteiro.Id == checkListId)
							return true;
					}
				}

				if (checkList.Situacao == 3) // se situação for inválida, descobrir o motivo e mostrar mensagem
				{
					Validacao.Add(Mensagem.ChecagemRoteiro.AssociarChecagemSituacaoInvalida);
				}
				else if (checkList.Situacao != 1) // diferente de finalizada
				{
					if (!isConversao)
					{
						Validacao.Add(Mensagem.ChecagemRoteiro.AssociarChecagemSituacaoNaoFinalizada);
					}
				}

				checkList.Roteiros.ForEach(roteiro =>
				{
					if (!VerificarAtividadesAtivadas(roteiro.Id))
					{
						Validacao.Erros.RemoveAll(x => x.Texto == Mensagem.ChecagemRoteiro.PossuiAtividadeDesativada.Texto);
						Validacao.Add(Mensagem.ChecagemRoteiro.ChecagemSelecionadaPossuiRoteirosAtividadesDesativadas);
						return;
					}
				});
			}

			return Validacao.EhValido;
		}

		internal bool ValidarAssociarRoteiro(int roteiroAssociado, List<int> roteirosAssociados)
		{
			if (roteirosAssociados != null && roteirosAssociados.Exists(x => x == roteiroAssociado))
			{
				Validacao.Add(Mensagem.ChecagemRoteiro.RoteiroJaAdicionado);
				return Validacao.EhValido;
			}

			// validar se o roteiro está desativado
			if (_daRoteiro.RoteiroDesativado(roteiroAssociado))
			{
				Validacao.Add(Mensagem.ChecagemRoteiro.RoteiroDesativado(roteiroAssociado, eTipoMensagem.Advertencia));
				return Validacao.EhValido;
			}

			if (!VerificarAtividadesAtivadas(roteiroAssociado))
			{
				return Validacao.EhValido;
			}

			// valida roteiros de setores diferentes
			if (roteirosAssociados != null && roteirosAssociados.Count > 0)
			{
				List<int> todosRoteiros = new List<int>(roteirosAssociados.ToArray());
				todosRoteiros.Add(roteiroAssociado);
				if (!_daRoteiro.RoteirosMesmoSetor(todosRoteiros))
				{
					Validacao.Add(Mensagem.ChecagemRoteiro.RoteiroAssociarSetorDiferente);
				}
			}

			return Validacao.EhValido;
		}

		public bool VerificarAtividadesAtivadas(int id)
		{
			if (!_daRoteiro.ValidarAtividadesAtivadas(id))
			{
				Validacao.Add(Mensagem.ChecagemRoteiro.PossuiAtividadeDesativada);
				return false;
			}

			return true;
		}
	}
}