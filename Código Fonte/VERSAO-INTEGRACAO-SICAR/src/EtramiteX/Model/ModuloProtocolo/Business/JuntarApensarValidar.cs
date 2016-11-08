using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloProcesso;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProcesso.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloRequerimento.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Business
{
	public class JuntarApensarValidar
	{
		#region Propriedades

		ProtocoloDa _protocoloDa = new ProtocoloDa();
		ProcessoMsg Msg = new ProcessoMsg();
		ProcessoValidar _validarProcesso = new ProcessoValidar();
		RequerimentoValidar _requerimentoValidar = new RequerimentoValidar();

		#endregion

		public bool EditarApensadosJuntados(Processo processo)
		{
			if (!_validarProcesso.EmPosse(processo.Id.Value))
			{
				Validacao.Add(Mensagem.Processo.PosseProcessoNecessaria);
			}

			foreach (var item in processo.Documentos)
			{
				_validarProcesso.RequerimentoFinalizado(item.Requerimento.Id, item.Id.Value, false);

				if (item.Atividades.Count <= 0)
				{
					Validacao.Add(Msg.AtividadeObrigatoria);
					break;
				}
				else
				{
					_validarProcesso.ValidarAtividades(item.Atividades, false);
				}

				if (item.ChecagemRoteiro.Id > 0 && item.Requerimento.Id > 0)
				{
					_requerimentoValidar.RoteirosChecagemRequerimento(item.ChecagemRoteiro.Id, item.Requerimento.Id, item.Requerimento.SituacaoId);
				}

				RequerimentoAssociadoTitulo(item.Id.GetValueOrDefault(), false);
			}

			if (Validacao.EhValido)
			{
				foreach (Processo item in processo.Processos)
				{
					_validarProcesso.ValidarProcessoTodosCampos(item);

					RequerimentoAssociadoTitulo(item.Id.GetValueOrDefault(), true);
				}
			}

			return Validacao.EhValido;
		}

		public bool RequerimentoAssociadoTitulo(int id, bool isProcesso)
		{
			List<String> titulos = new List<string>();

			if (isProcesso)
			{
				titulos = _protocoloDa.VerificarAtividadeAssociadaTitulo(id);
			}
			else
			{
				titulos = _protocoloDa.VerificarAtividadeAssociadaTitulo(id);
			}

			if (titulos.Count > 0)
			{
				Validacao.Add(Mensagem.Documento.RequerimentoAssociadoTitulo());
			}

			return Validacao.EhValido;
		}

		public bool Apensado(int protocolo)
		{
			ProtocoloNumero retorno = _protocoloDa.VerificarProtocoloAssociado(protocolo);
			string numero = string.Empty;
			if (retorno != null)
			{
				numero = retorno.NumeroTexto;
			}
			
			if (!string.IsNullOrEmpty(numero))
			{
				Validacao.Add(Msg.EditarProcessoApensado(numero));
				return true;
			}

			return false;
		}

		public string Apensado(string numero)
		{
			ProtocoloNumero retorno = _protocoloDa.VerificarProtocoloAssociado(_protocoloDa.ExisteProtocolo(numero));
			if (retorno != null)
			{
				return retorno.NumeroTexto;
			}
			return string.Empty;
		}

		public string Juntado(string numero)
		{
			ProtocoloNumero retorno = _protocoloDa.VerificarProtocoloAssociado(_protocoloDa.ExisteProtocolo(numero));
			if (retorno != null)
			{
				return retorno.NumeroTexto;
			}
			return string.Empty;
		}
	}
}