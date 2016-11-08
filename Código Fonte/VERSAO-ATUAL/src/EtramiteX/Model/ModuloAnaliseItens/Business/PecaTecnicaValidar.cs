using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico;
using Tecnomapas.Blocos.Entities.Interno.ModuloAnaliseItens;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloProjetoGeografico.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAnaliseItens.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloRequerimento.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloAnaliseItens.Business
{
	public class PecaTecnicaValidar
	{
		#region Propriedades

		ProtocoloBus _busProtocolo = new ProtocoloBus();
		RequerimentoBus _busRequerimento = new RequerimentoBus();
		AnaliseItensDa _daAnalise = new AnaliseItensDa();
		
		#endregion

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
				Validacao.Add(protocolo.IsProcesso ? Mensagem.AnaliseItem.PecaTecnicaProcessoApensado(retorno) : Mensagem.AnaliseItem.PecaTecnicaDocumentoJuntado(retorno));
				return false;
			}

			if (!_busProtocolo.ExisteRequerimento(protocolo.Id))
			{
				Validacao.Add(Mensagem.AnaliseItem.ExisteRequerimento);
				return false;
			}

			return Validacao.EhValido;
		}

		public bool Salvar(PecaTecnica pecaTecnica)
		{

			if (pecaTecnica.Elaborador <= 0)
			{
				Validacao.Add(Mensagem.AnaliseItem.PecaTecnicaElaboradorObrigatorio);
			}

			if (pecaTecnica.Atividade <= 0)
			{
				Validacao.Add(Mensagem.AnaliseItem.PecaTecnicaAtividadeObrigatorio);
			}

			if (pecaTecnica.ResponsaveisEmpreendimento.Count <= 0)
			{
				Validacao.Add(Mensagem.AnaliseItem.PecaTecnicaRespEmpreendimentoObrigatorio);
			}

			if (pecaTecnica.ElaboradorTipoEnum == eElaboradorTipo.TecnicoIdaf && pecaTecnica.SetorCadastro <= 0)
			{
				Validacao.Add(Mensagem.AnaliseItem.PecaTecnicaSetorObrigatorio);
			}

			if (!Validacao.EhValido)
			{
				return false;
			}

			IProtocolo protocolo =  _busProtocolo.ObterSimplificado(pecaTecnica.Protocolo.Id.GetValueOrDefault());

			if (protocolo.Empreendimento.Id <= 0)
			{
				Validacao.Add(Mensagem.AnaliseItem.PecaTecnicaNaoPossuiEmpreendimento);
				return false;
			}

			List<Requerimento> requerimentos = _busProtocolo.ObterProtocoloRequerimentos(pecaTecnica.ProtocoloPai.GetValueOrDefault());

			if (!requerimentos.Exists(x => x.ProtocoloId == pecaTecnica.Protocolo.Id))
			{
				Validacao.Add(Mensagem.AnaliseItem.PecaTecnicaRequerimentoNaoAssociado);
				return false;
			}

			ProjetoGeograficoBus busProjetoGeo = new ProjetoGeograficoBus();

			int projetoGeoId = busProjetoGeo.ExisteProjetoGeografico(pecaTecnica.Protocolo.Empreendimento.Id,(int) eCaracterizacao.Dominialidade);

			if (busProjetoGeo.ObterSitacaoProjetoGeografico(projetoGeoId) != (int)eProjetoGeograficoSituacao.Finalizado)
			{
				Validacao.Add(Mensagem.AnaliseItem.PecaTecnicaProjetoDeveSerFinalizado);
			}


			return Validacao.EhValido;
		}
	}
}
