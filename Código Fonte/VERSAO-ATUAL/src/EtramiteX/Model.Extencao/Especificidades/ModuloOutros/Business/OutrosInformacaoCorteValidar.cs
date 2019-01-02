using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloOutros;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloOutros.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloOutros.Business
{
	public class OutrosInformacaoCorteValidar : EspecificidadeValidarBase, IEspecificiadeValidar
	{
		public bool Salvar(IEspecificidade especificidade)
		{

			OutrosInformacaoCorteDa _da = new OutrosInformacaoCorteDa();
			var esp = especificidade as OutrosInformacaoCorte;

			DeclaratorioRequerimentoAtividade(esp);

			if (esp.InformacaoCorte <= 0)
			{
				Validacao.Add(Mensagem.OutrosInformacaoCorte.InformacaoCorteObrigatorio);
			}

			if (esp.Validade <= 0)
			{
				Validacao.Add(Mensagem.OutrosInformacaoCorte.ValidadeObrigatoria);
			}


			return Validacao.EhValido;
		}

		public bool Emitir(IEspecificidade especificidade)
		{
			if (ExisteProcDocFilhoQueFoiDesassociado(especificidade.Titulo.Id))
			{
				Validacao.Add(Mensagem.Especificidade.ProtocoloReqFoiDesassociado);
			}

			return Salvar(especificidade);
		}
	}
}