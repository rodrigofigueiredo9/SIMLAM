using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTramitacao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloTramitacao.Business
{
	public class ArquivarValidar
	{
		ArquivarDa _da = new ArquivarDa();
		TramitacaoDa _daTram = new TramitacaoDa();
		ProtocoloBus _busProtocolo = new ProtocoloBus();

		public bool PreDesarquivar(int arquivo)
		{
			if (arquivo <= 0)
			{
				Validacao.Add(Mensagem.Arquivamento.ArquivarArquivoObrigatorio);
			}

			return Validacao.EhValido;
		}

		public bool PreDesarquivar(ListarTramitacaoFiltro filtros)
		{
            

			if (filtros.ArquivoId <= 0)
			{
				Validacao.Add(Mensagem.Arquivamento.DesarquivarArquivoObrigatorio);
				return Validacao.EhValido;
			}

			if (String.IsNullOrWhiteSpace(filtros.Protocolo.NumeroTexto))
			{
				Validacao.Add(Mensagem.Arquivamento.DesarquivarProtocoloNumeroObrigatorio);
				return Validacao.EhValido;
			}
			else
			{
				if (!filtros.Protocolo.IsValido)
				{
					Validacao.Add(Mensagem.Arquivamento.DesarquivarProtocoloNumeroInvalido);
					return Validacao.EhValido;
				}
			}

			IProtocolo proc = _busProtocolo.Obter(filtros.Protocolo.NumeroTexto);

			if (proc == null || proc.Id.GetValueOrDefault() <= 0)
			{
				Validacao.Add(Mensagem.Arquivamento.DesarquivarProtocoloNaoCadastrado);
			}
			else
			{
                if (filtros.Protocolo.IsValido)
                {
                    return Validacao.EhValido;
                }

				int setorProtocolo = proc.SetorId > 0 ? proc.SetorId : proc.SetorCriacaoId;

				if (setorProtocolo != filtros.RemetenteSetorId)
				{
					Validacao.Add(Mensagem.Arquivamento.DesarquivarProtocoloEmOutroSetor);
				}
			}

			return Validacao.EhValido;

		}

		public bool Desarquivar(int arquivoId, int destinoSetor, List<Tramitacao> tramitacoes)
		{

			if (destinoSetor <= 0)
			{
				Validacao.Add(Mensagem.Arquivamento.SetorDestinoObrigatorio);
			}

			if (arquivoId <= 0)
			{
				Validacao.Add(Mensagem.Arquivamento.DesarquivarArquivoObrigatorio);
			}

			if (tramitacoes == null || tramitacoes.Count <= 0)
			{
				Validacao.Add(Mensagem.Arquivamento.DesarquivarProtocoloObrigatorio);
			}

			if (!Validacao.EhValido) return Validacao.EhValido;

			foreach (Tramitacao tramitacao in tramitacoes)
			{
				bool existe = _daTram.ExisteTramitacao(tramitacao.Protocolo.Id.Value) > 0;
				
				if (!existe)
				{
					Validacao.Add(Mensagem.Arquivamento.ProtocoloJaDesarquivado(tramitacao.Protocolo.IsProcesso ? "processo" : "documento", tramitacao.Protocolo.Numero));
				}
			}

			return Validacao.EhValido;
		}

		internal bool Arquivar(Arquivar arquivar, List<Tramitacao> tramitacoes)
		{
			if (arquivar.SetorId <= 0)
			{
				Validacao.Add(Mensagem.Arquivamento.ArquivarSetorOrigemObrigatorio);
				return Validacao.EhValido;
			}

			Arquivamento(arquivar);

			if (!Validacao.EhValido) return Validacao.EhValido;

			if (tramitacoes.Count <= 0)
			{
				Validacao.Add(Mensagem.Arquivamento.ArquivarProtocoloObrigatorio);
			}

			foreach (Tramitacao tramitacao in tramitacoes)
			{
				Arquivar(tramitacao);
			}

			return Validacao.EhValido;
		}

		internal bool Arquivamento(Arquivar arquivar)
		{
			if (arquivar.SetorId <= 0)
			{
				Validacao.Add(Mensagem.Arquivamento.ArquivarSetorOrigemObrigatorio);
			}

			if (arquivar.ObjetivoId <= 0)
			{
				Validacao.Add(Mensagem.Arquivamento.ArquivarObjetivoObrigatorio);
			}

			if (arquivar.ArquivoId <= 0)
			{
				Validacao.Add(Mensagem.Arquivamento.ArquivarArquivoObrigatorio);
			}

			if (arquivar.EstanteId <= 0)
			{
				Validacao.Add(Mensagem.Arquivamento.ArquivarEstanteObrigatorio);
			}

			if (arquivar.PrateleiraModoId <= 0)
			{
				Validacao.Add(Mensagem.Arquivamento.ArquivarPrateleiraObrigatorio);
			}

			if (arquivar.PrateleiraId <= 0)
			{
				Validacao.Add(Mensagem.Arquivamento.ArquivarIdentificacaoObrigatoria);
			}

			return Validacao.EhValido;
		}

		internal bool Arquivar(Tramitacao tramitacao)
		{
			// Valida se processo está apensado/juntado
			if ( _busProtocolo.VerificarProtocoloAssociado(tramitacao.Protocolo.Id.Value))
			{
				Validacao.Add(tramitacao.Protocolo.IsProcesso?Mensagem.Arquivamento.ArquivarProcessoApensadoNaoPodeSerArquivado:Mensagem.Arquivamento.ArquivarDocumentoJuntadoNaoPodeSerArquivado);
			}

			return Validacao.EhValido;
		}
	}
}