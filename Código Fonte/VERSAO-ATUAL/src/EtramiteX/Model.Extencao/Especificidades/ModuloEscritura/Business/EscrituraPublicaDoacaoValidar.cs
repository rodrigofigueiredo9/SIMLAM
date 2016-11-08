using System;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEscritura;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEscritura.Business
{
	public class EscrituraPublicaDoacaoValidar : EspecificidadeValidarBase, IEspecificiadeValidar
	{
		public bool Salvar(IEspecificidade especificidade)
		{
			EscrituraPublicaDoacao esp = especificidade as EscrituraPublicaDoacao;

			RequerimentoAtividade(esp, false, true);
			Destinatario(especificidade.ProtocoloReq.Id, esp.Destinatario, "Escritura_Destinatario");

			if (String.IsNullOrWhiteSpace(esp.Livro))
			{
				Validacao.Add(Mensagem.EscrituraPublicaDoacao.LivroObrigatorio);
			}

			if (String.IsNullOrWhiteSpace(esp.Folhas))
			{
				Validacao.Add(Mensagem.EscrituraPublicaDoacao.FolhasObrigatorio);
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