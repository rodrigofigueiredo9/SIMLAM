using System;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEscritura;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEscritura.Business
{
	public class EscrituraPublicaCompraVendaValidar : EspecificidadeValidarBase, IEspecificiadeValidar
	{
		public bool Salvar(IEspecificidade especificidade)
		{
			EscrituraPublicaCompraVenda esp = especificidade as EscrituraPublicaCompraVenda;

			RequerimentoAtividade(esp, false, true);
			Destinatario(especificidade.ProtocoloReq.Id, esp.Destinatario, "Escritura_Destinatario");

			if (String.IsNullOrWhiteSpace(esp.Livro))
			{
				Validacao.Add(Mensagem.EscrituraPublicaCompraVenda.LivroObrigatorio);
			}

			if (String.IsNullOrWhiteSpace(esp.Folhas))
			{
				Validacao.Add(Mensagem.EscrituraPublicaCompraVenda.FolhasObrigatorio);
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