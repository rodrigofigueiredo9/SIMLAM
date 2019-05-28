using System;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloOutros;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloOutros.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloOutros.Business
{
	public class OutrosInformacaoCorteValidar : EspecificidadeValidarBase, IEspecificiadeValidar
	{
		OutrosInformacaoCorteDa _da = new OutrosInformacaoCorteDa();

		public bool Salvar(IEspecificidade especificidade)
		{
			var esp = especificidade as OutrosInformacaoCorte;

			if (esp.InformacaoCorte <= 0)
				Validacao.Add(Mensagem.OutrosInformacaoCorte.InformacaoCorteObrigatorio);

			var tituloAssociado = _da.ObterTituloDeclaratorioAssociadoACaracterizacao(esp.InformacaoCorte);
			if (!string.IsNullOrWhiteSpace(tituloAssociado))
				Validacao.Add(Mensagem.OutrosInformacaoCorte.CaracterizacaoJaAssociada(tituloAssociado));

			if (especificidade.RequerimentoId <= 0)
				Validacao.Add(Mensagem.Especificidade.RequerimentoPradroObrigatoria);

			var tituloAssociadoRequerimento = _da.ObterTituloDeclaratorioAssociadoARequerimento(especificidade.RequerimentoId, especificidade.Titulo.Id);
			if (!string.IsNullOrWhiteSpace(tituloAssociadoRequerimento))
				Validacao.Add(Mensagem.OutrosInformacaoCorte.RequerimentoJaAssociado(tituloAssociadoRequerimento));
			else if (_da.RequerimentoJaAssociado(especificidade.RequerimentoId, especificidade.Titulo.Id))
				Validacao.Add(Mensagem.OutrosInformacaoCorte.RequerimentoJaAssociadoTituloNaoEmitido);

			if (esp.Validade <= 0)
				Validacao.Add(Mensagem.OutrosInformacaoCorte.ValidadeObrigatoria);
			if (esp.Validade < 20 || esp.Validade > 180)
				Validacao.Add(Mensagem.OutrosInformacaoCorte.ValidadeIntervalo);

			return Validacao.EhValido;
		}

		public bool Emitir(IEspecificidade especificidade)
		{
			return Salvar(especificidade);
		}
	}
}