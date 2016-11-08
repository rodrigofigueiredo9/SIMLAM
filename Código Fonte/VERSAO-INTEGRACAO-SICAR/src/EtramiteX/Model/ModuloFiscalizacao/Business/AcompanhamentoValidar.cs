using System;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business
{
	public class AcompanhamentoValidar
	{
		#region Propriedades

		AcompanhamentoDa _da = new AcompanhamentoDa();
		ProtocoloDa _protocoloDa = new ProtocoloDa();
		FiscalizacaoBus _fiscalizacaoBus = new FiscalizacaoBus();

		public EtramiteIdentity User
		{
			get
			{
				if (HttpContext.Current.User == null)
				{
					return null;
				}
				return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity;
			}
		}

		#endregion

		internal bool Salvar(Acompanhamento entidade) 
		{
			ValidacoesGenericasBus.DataMensagem(entidade.DataVistoria, "Acompanhamento_DataVistoria_DataTexto", "vistoria");

			if (entidade.SetorId <= 0) {
				Validacao.Add(Mensagem.Acompanhamento.SetorObrigatorio);
			}

			if (entidade.ReservalegalTipo <= 0) {
				Validacao.Add(Mensagem.Acompanhamento.ReservalegalObrigatoria);
			}

			if (entidade.InfracaoResultouErosao == 1 && String.IsNullOrWhiteSpace(entidade.InfracaoResultouErosaoEspecificar))
			{
				Validacao.Add(Mensagem.Acompanhamento.InfracaoResultouErosaoEspecificarObrigatorio);
			}

			if (entidade.PossuiAreaEmbargadaOuAtividadeInterditada.HasValue && entidade.PossuiAreaEmbargadaOuAtividadeInterditada.Value)
			{
				#region AtividadeAreaEmbargada

				if (!entidade.AtividadeAreaEmbargada.HasValue)
				{
					Validacao.Add(Mensagem.Acompanhamento.AtividadeAreaEmbargadaObrigatorio);
				}
				else
				{
					if (entidade.AtividadeAreaEmbargada.Value == 1 && String.IsNullOrWhiteSpace(entidade.AtividadeAreaEmbargadaEspecificarTexto))
					{
						Validacao.Add(Mensagem.Acompanhamento.AtividadeAreaEmbargadaEspecificarTextoObrigatorio);
					}
				}

				#endregion
			}

			if (entidade.HouveApreensaoMaterial.HasValue && entidade.HouveApreensaoMaterial.Value)
			{
				#region HouveDesrespeitoTAD

				if (!entidade.HouveDesrespeitoTAD.HasValue)
				{
					Validacao.Add(Mensagem.Acompanhamento.HouveDesrespeitoTADObrigatorio);
				}
				else
				{
					if (entidade.HouveDesrespeitoTAD.Value == 1 && String.IsNullOrWhiteSpace(entidade.HouveDesrespeitoTADEspecificar))
					{
						Validacao.Add(Mensagem.Acompanhamento.HouveDesrespeitoTADEspecificarObrigatorio);
					}
				}

				#endregion
			}

			#region RepararDanoAmbiental

			if (!entidade.RepararDanoAmbiental.HasValue)
			{
				Validacao.Add(Mensagem.Acompanhamento.RepararDanoAmbientalObrigatorio);
			}
			else
			{
				if (entidade.RepararDanoAmbiental.Value == 1 && String.IsNullOrWhiteSpace(entidade.RepararDanoAmbientalEspecificar))
				{
					Validacao.Add(Mensagem.Acompanhamento.RepararDanoAmbientalEspecificarObrigatorio);
				}
			}

			#endregion

			#region FirmouTermoRepararDanoAmbiental

			if (!entidade.FirmouTermoRepararDanoAmbiental.HasValue)
			{
				Validacao.Add(Mensagem.Acompanhamento.FirmouTermoRepararDanoAmbientalObrigatorio);
			}
			else
			{
				if (entidade.FirmouTermoRepararDanoAmbiental.Value == 0 && String.IsNullOrWhiteSpace(entidade.FirmouTermoRepararDanoAmbientalEspecificar))
				{
					Validacao.Add(Mensagem.Acompanhamento.FirmouTermoRepararDanoAmbientalEspecificarObrigatorio);
				}
			}

			#endregion

			#region Assinantes

			if (entidade.Assinantes == null || entidade.Assinantes.Count == 0)
			{
				Validacao.Add(Mensagem.Acompanhamento.AssinanteObrigatorio);
			}
			else
			{
				if (!entidade.Assinantes.Exists(x => x.FuncionarioId == User.FuncionarioId))
				{
					Validacao.Add(Mensagem.Acompanhamento.AssinanteFuncionarioLogado);
				}

				if (entidade.Assinantes.GroupBy(x => new { FuncionarioId = x.FuncionarioId, FuncionarioCargoId = x.FuncionarioCargoId }).Where(x => x.Count() > 1).Count() > 0)
				{
					Validacao.Add(Mensagem.Acompanhamento.AssinanteFuncionarioUnico);
				}
			}

			#endregion

			return Validacao.EhValido;
		}

		public bool ValidarAcesso(Fiscalizacao fiscalizacao)
		{
			if (fiscalizacao.Situacao == eFiscalizacaoSituacao.EmAndamento ||
				fiscalizacao.Situacao == eFiscalizacaoSituacao.CadastroConcluido ||
				fiscalizacao.Situacao == eFiscalizacaoSituacao.CancelarConclusao)
			{
				Validacao.Add(Mensagem.Acompanhamento.Acompanhamentos);
				return false;
			}

			if (!_protocoloDa.EmPosse(fiscalizacao.ProtocoloId))
			{
				Validacao.Add(Mensagem.Fiscalizacao.PosseProcessoNecessaria);
				return false;
			}

			return Validacao.EhValido;
		}

		public bool PodeAlterarSituacao(Acompanhamento acompanhamento, Fiscalizacao fiscalizacao)
		{
			if (!_protocoloDa.EmPosse(fiscalizacao.ProtocoloId))
			{
				Validacao.Add(Mensagem.Acompanhamento.PosseProcessoAlterarSituacao);
				return false;
			}

			if (acompanhamento.SituacaoId == (int)eAcompanhamentoSituacao.Nulo || acompanhamento.SituacaoId == (int)eAcompanhamentoSituacao.Cancelado)
			{
				Validacao.Add(Mensagem.Acompanhamento.SituacaoInvalidaAlterarSituacao(acompanhamento.SituacaoTexto));
				return false;
			}

			return Validacao.EhValido;
		}

		public bool PodeEditar(Acompanhamento acompanhamento)
		{

			if (acompanhamento.SituacaoId != (int)eAcompanhamentoSituacao.EmCadastro)
			{
				Validacao.Add(Mensagem.Acompanhamento.SituacaoInvalidaEditar(acompanhamento.SituacaoTexto));
				return false;
			}

			return Validacao.EhValido;
		}

		internal bool AlterarSituacao(Acompanhamento entidade)
		{
			if (!_protocoloDa.EmPosse(_fiscalizacaoBus.Obter(entidade.FiscalizacaoId).ProtocoloId))
			{
				Validacao.Add(Mensagem.Acompanhamento.PosseProcessoAlterarSituacao);
				return false;
			}

			if (entidade.SituacaoId <= 0)
			{
				Validacao.Add(Mensagem.Acompanhamento.SituacaoNovaObrigatorio);
			}

			ValidacoesGenericasBus.DataMensagem(entidade.DataSituacao, "DataSituacaoNova", "Data da nova situação");

			if (entidade.SituacaoId == (int)eAcompanhamentoSituacao.Cancelado)
			{
				if (string.IsNullOrWhiteSpace(entidade.Motivo))
				{
					Validacao.Add(Mensagem.Acompanhamento.SituacaoMotivoObrigatorio);
				}
			}
			else
			{
				entidade.Motivo = string.Empty;
			}

			Acompanhamento auxilio = _da.Obter(entidade.Id, true);
			if (entidade.DataSituacao.Data < auxilio.DataSituacao.Data)
			{
				Validacao.Add(Mensagem.Acompanhamento.SituacaoDataAntigaMaiorNova);
			}

			entidade.PdfLaudo = auxilio.PdfLaudo;

			return Validacao.EhValido;
		}

		public bool Acompanhamentos(Fiscalizacao fiscalizacao)
		{
			if (fiscalizacao.Situacao == eFiscalizacaoSituacao.EmAndamento ||
				fiscalizacao.Situacao == eFiscalizacaoSituacao.CadastroConcluido || 
				fiscalizacao.Situacao == eFiscalizacaoSituacao.CancelarConclusao)
			{
				Validacao.Add(Mensagem.Acompanhamento.Acompanhamentos);
				return false;
			}

			if (!_protocoloDa.EmPosse(fiscalizacao.ProtocoloId))
			{
				Validacao.Add(Mensagem.Fiscalizacao.PosseProcessoNecessaria);
				return false;
			}

			return Validacao.EhValido;
		}

		internal bool Excluir(Acompanhamento entidade)
		{
			if (entidade.SituacaoId != (int)eAcompanhamentoSituacao.EmCadastro)
			{
				Validacao.Add(Mensagem.Acompanhamento.ExcluirInvalido(entidade.Numero, entidade.SituacaoTexto));
				return false;
			}

			if (!_protocoloDa.EmPosse(_fiscalizacaoBus.Obter(entidade.FiscalizacaoId).ProtocoloId))
			{
				Validacao.Add(Mensagem.Acompanhamento.PosseProcessoAlterarSituacao);
				return false;
			}

			return Validacao.EhValido;
		}
	}
}