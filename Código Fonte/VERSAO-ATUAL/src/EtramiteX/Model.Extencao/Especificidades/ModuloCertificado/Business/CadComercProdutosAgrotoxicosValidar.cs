using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCertificado;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloCertificado.Business
{
	public class CadComercProdutosAgrotoxicosValidar : EspecificidadeValidarBase, IEspecificiadeValidar
	{
		public bool Salvar(IEspecificidade especificidade)
		{
			CadComercProdutosAgrotoxicos esp = especificidade as CadComercProdutosAgrotoxicos;

			RequerimentoAtividade(esp, false, true);
			Destinatario(especificidade.ProtocoloReq.Id, esp.Destinatario, "CadComercProdutosAgrotoxicos_Destinatario");

			return Validacao.EhValido;
		}

		public bool Emitir(IEspecificidade especificidade)
		{
			Salvar(especificidade);

			return Validacao.EhValido;
		}
	}
}