using System;
using Tecnomapas.Blocos.Arquivo.Data;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloProcesso;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloProtocolo.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloTramitacao.Business;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloProtocolo.Business
{
	public class ProtocoloInternoBus
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys;
		ProtocoloInternoDa _da;
		ProtocoloInternoValidar _validar;

		public String UsuarioInterno
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioInterno); }
		}

		#endregion

		public ProtocoloInternoBus()
		{
			_configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
			_da = new ProtocoloInternoDa(UsuarioInterno);
			_validar = new ProtocoloInternoValidar();
		}

		public IProtocolo Obter(int id)
		{
			IProtocolo protocolo = null;

			try
			{
				protocolo = _da.Obter(id);

				if ((protocolo.Id ?? 0) <= 0)
				{
					Validacao.Add(Mensagem.Processo.Inexistente);
				}
				else
				{
					if (protocolo.Arquivo != null && protocolo.Arquivo.Id > 0)
					{
						ArquivoDa _arquivoDa = new ArquivoDa();
						protocolo.Arquivo = _arquivoDa.Obter(protocolo.Arquivo.Id.Value);
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return protocolo;
		}

		public IProtocolo ObterSimplificado(int id, bool suprimirMensagemInexistente = false)
		{
			IProtocolo protocolo = null;

			try
			{
				protocolo = _da.ObterSimplificado(id);

				if (!suprimirMensagemInexistente && (protocolo.Id ?? 0) <= 0)
				{
					Validacao.Add(Mensagem.Processo.Inexistente);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return protocolo;
		}

		public IProtocolo ObterProcessosDocumentos(int processo)
		{
			try
			{
				return _da.ObterProcessosDocumentos(processo);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public bool ExisteProtocolo(int id)
		{
			return _validar.ExisteProtocolo(id);
		}

		public ProtocoloLocalizacao ObterLocalizacao(int protocoloId, ProtocoloLocalizacao localizacao = null)
		{
			ProtocoloLocalizacao loc = localizacao == null ? new ProtocoloLocalizacao() : localizacao;
			IProtocolo protocolo = ObterSimplificado(protocoloId);

			if (loc.ProcessoPaiId <= 0)
			{
				int ApensadoEmProcessoId = ProcessoApensado(protocoloId);
				if (ApensadoEmProcessoId > 0)
				{
					loc.ProcessoPaiId = ApensadoEmProcessoId;
					loc.ProcessoPaiNumero = ObterSimplificado(loc.ProcessoPaiId).Numero;
					return ObterLocalizacao(loc.ProcessoPaiId, loc);
				}
			}

			TramitacaoInternoBus _busTramitacao = new TramitacaoInternoBus();

			loc.Tramitacao.Id = _busTramitacao.ObterTramitacaoProtocolo(protocoloId);
			if (loc.Tramitacao.Id > 0)
			{
				loc.Tramitacao = _busTramitacao.Obter(loc.Tramitacao.Id);
				if (loc.Tramitacao.SituacaoId == (int)eTramitacaoSituacao.Arquivado)
				{
					loc.Localizacao = eLocalizacaoProtocolo.Arquivado;
					ArquivarInternoBus _arquivarBus = new ArquivarInternoBus();
					Arquivar arquivamento = _arquivarBus.ObterArquivamento(loc.Tramitacao.Id);
					loc.ArquivoNome = arquivamento.ArquivoNome;
				}
				else if (loc.Tramitacao.SituacaoId == (int)eTramitacaoSituacao.ParaOrgaoExterno)
				{
					loc.Localizacao = eLocalizacaoProtocolo.OrgaoExterno;
					loc.OrgaoExternoNome = loc.Tramitacao.OrgaoExterno.Texto;
				}
				else if (loc.Tramitacao.SituacaoId == (int)eTramitacaoSituacao.Tramitando)
				{
					if (loc.Tramitacao.Destinatario.Id == 0)
					{
						loc.Localizacao = eLocalizacaoProtocolo.EnviadoParaSetor;
					}
					else
					{
						loc.Localizacao = eLocalizacaoProtocolo.EnviadoParaFuncionario;
						loc.FuncionarioDestinatarioNome = loc.Tramitacao.Destinatario.Nome;
					}
				}
			}
			else // se não existir tramitação, ele está na posse de algum funcionário
			{
				loc.Localizacao = eLocalizacaoProtocolo.PosseFuncionario;
				TramitacaoPosse posse = _busTramitacao.ObterProtocoloPosse(protocoloId);
				loc.FuncionarioDestinatarioNome = posse.FuncionarioNome;
				loc.SetorDestinatarioNome = posse.SetorNome;
			}
			return loc;
		}

		public bool ExisteAtividade(int protocolo)
		{
			return _validar.ExisteAtividade(protocolo);
		}

		public int ProcessoApensado(int protocolo)
		{
			return _validar.ProtocoloAssociado(protocolo);
		}
	}
}