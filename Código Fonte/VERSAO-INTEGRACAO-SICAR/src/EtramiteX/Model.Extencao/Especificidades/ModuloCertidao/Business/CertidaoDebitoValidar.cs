using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCertidao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloCertidao.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloCertidao.Business
{
	public class CertidaoDebitoValidar : EspecificidadeValidarBase, IEspecificiadeValidar
	{
		CertidaoDebitoDa _da = new CertidaoDebitoDa();

		public bool Salvar(IEspecificidade especificidade)
		{
			CertidaoDebito esp = especificidade as CertidaoDebito;

			RequerimentoAtividade(esp);

			Destinatario(especificidade.ProtocoloReq.Id, esp.Destinatario, "CertidaoDebito_Destinatario");

			if (esp.Atividades[0].Id != ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.ConsultaDebitoAmbientalFlorestal))
			{
				Validacao.Add(Mensagem.CertidaoDebito.AtividadeInvalida(esp.Atividades[0].NomeAtividade));
			}

			return Validacao.EhValido;
		}

		public bool Emitir(IEspecificidade especificidade)
		{
			Salvar(especificidade);

			CertidaoDebito esp = especificidade as CertidaoDebito;

			List<Fiscalizacao> fiscalizacoesAtual = _da.ObterFiscalizacoesPorAutuado(esp.Destinatario).Where(x => x.InfracaoAutuada).ToList();


			if (esp.Fiscalizacoes.Count != fiscalizacoesAtual.Count) 
			{
				Validacao.Add(Mensagem.CertidaoDebito.EspecificidadeInvalida);
				return Validacao.EhValido;
			}


			Int32 QtdFiscalizacao = 0;
			fiscalizacoesAtual.ForEach(x => { esp.Fiscalizacoes.ForEach(y => { if (x.Id == y.Id) { QtdFiscalizacao++; } }); });

			if (esp.Fiscalizacoes.Count != QtdFiscalizacao) 
			{
				Validacao.Add(Mensagem.CertidaoDebito.EspecificidadeInvalida);
				return Validacao.EhValido;
			}


			foreach (Fiscalizacao fiscalizacaoEsp in esp.Fiscalizacoes)
			{
				foreach (Fiscalizacao fiscalizacaoAtual in fiscalizacoesAtual)
				{
					if (fiscalizacaoEsp.Id == fiscalizacaoAtual.Id) 
					{

						if (fiscalizacaoEsp.Tid != fiscalizacaoAtual.Tid)
						{
							Validacao.Add(Mensagem.CertidaoDebito.EspecificidadeInvalida);
							return Validacao.EhValido;
						}

						if (fiscalizacaoEsp.SituacaoId != fiscalizacaoAtual.SituacaoId)
						{
							Validacao.Add(Mensagem.CertidaoDebito.EspecificidadeInvalida);
							return Validacao.EhValido;
						}
					}
				}
			}

			return Validacao.EhValido;
		}
	}
}