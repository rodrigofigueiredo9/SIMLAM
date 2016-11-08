using System;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloLaudo;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloLaudo.Business
{
	public class LaudoVistoriaLicenciamentoValidar : EspecificidadeValidarBase, IEspecificiadeValidar
	{
		public bool Salvar(IEspecificidade especificidade)
		{
			LaudoVistoriaLicenciamento esp = especificidade as LaudoVistoriaLicenciamento;

			//RequerimentoAtividade(esp);
			#region validações requerimento
			if (especificidade.ProtocoloReq.RequerimentoId <= 0)
			{
				Validacao.Add(Mensagem.Especificidade.RequerimentoPradroObrigatoria);
			}

			if (especificidade.Atividades == null || especificidade.Atividades.Count == 0 || especificidade.Atividades[0].Id == 0)
			{
				Validacao.Add(Mensagem.Especificidade.AtividadeObrigatoria);
			}
			#endregion

			Destinatario(especificidade.ProtocoloReq.Id, esp.Destinatario, "Laudo_Destinatario");

			ValidacoesGenericasBus.DataMensagem(esp.DataVistoria, "Laudo_DataVistoria_DataTexto", "vistoria");

			if (String.IsNullOrWhiteSpace(esp.Objetivo))
			{
				Validacao.Add(Mensagem.LaudoVistoriaLicenciamentoMsg.ObjetivoObrigatorio);
			}

			if (String.IsNullOrWhiteSpace(esp.Consideracao))
			{
				Validacao.Add(Mensagem.LaudoVistoriaLicenciamentoMsg.ConsideracoesObrigatorio);
			}

			if (String.IsNullOrWhiteSpace(esp.ParecerDescricao))
			{
				Validacao.Add(Mensagem.LaudoVistoriaLicenciamentoMsg.ParecerTecnicoDescricaoObrigatorio);
			}

			if (esp.Conclusao <= 0)
			{
				Validacao.Add(Mensagem.LaudoVistoriaLicenciamentoMsg.ConclusaoObrigatoria);
			}

			if (esp.Responsavel <= 0)
			{
				Validacao.Add(Mensagem.LaudoVistoriaLicenciamentoMsg.ResponsavelTecnicoObrigatorio);
			}

			return Validacao.EhValido;
		}

		public bool Emitir(IEspecificidade especificidade)
		{
			Salvar(especificidade);

			return Validacao.EhValido;
		}
	}
}