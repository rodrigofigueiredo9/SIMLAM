using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloOutros;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Especificidades.ModuloOutros.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Especificidades.ModuloOutros.Business
{
	public class OutrosInformacaoCorteValidar : EspecificidadeValidarBase, IEspecificiadeValidar
	{
		OutrosInformacaoCorteDa _da = new OutrosInformacaoCorteDa();

		public bool Salvar(IEspecificidade especificidade)
		{
			var esp = especificidade as OutrosInformacaoCorte;
			
			DeclaratorioRequerimentoAtividade(esp);

			if (especificidade.RequerimentoId <= 0)
				Validacao.Add(Mensagem.Especificidade.RequerimentoPradroObrigatoria);

			var tituloAssociadoRequerimento = _da.ObterTituloDeclaratorioAssociadoARequerimento(especificidade.RequerimentoId, especificidade.Titulo.Id);
			if (!string.IsNullOrWhiteSpace(tituloAssociadoRequerimento))
				Validacao.Add(Mensagem.OutrosInformacaoCorte.RequerimentoJaAssociado(tituloAssociadoRequerimento));
			else if(_da.RequerimentoJaAssociado(especificidade.RequerimentoId, especificidade.Titulo.Id))
				Validacao.Add(Mensagem.OutrosInformacaoCorte.RequerimentoJaAssociadoTituloNaoEmitido);

			if (esp.InformacaoCorte <= 0)
				Validacao.Add(Mensagem.OutrosInformacaoCorte.InformacaoCorteObrigatorio);

			if (esp.Validade <= 0)
				Validacao.Add(Mensagem.OutrosInformacaoCorte.ValidadeObrigatoria);
			if (esp.Validade < 20 || esp.Validade > 180)
				Validacao.Add(Mensagem.OutrosInformacaoCorte.ValidadeIntervalo);

			return Validacao.EhValido;
		}

		public bool Emitir(IEspecificidade especificidade)
		{
			Salvar(especificidade);

			return Validacao.EhValido;
		}
	}
}
