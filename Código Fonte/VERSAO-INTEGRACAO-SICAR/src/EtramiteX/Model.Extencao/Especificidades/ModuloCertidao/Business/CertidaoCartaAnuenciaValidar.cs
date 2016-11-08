using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCertidao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloCertidao.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloCertidao.Business
{
	public class CertidaoCartaAnuenciaValidar : EspecificidadeValidarBase, IEspecificiadeValidar
	{
		CertidaoCartaAnuenciaDa _da = new CertidaoCartaAnuenciaDa();

		public bool Salvar(IEspecificidade especificidade)
		{
			CertidaoCartaAnuencia esp = especificidade as CertidaoCartaAnuencia;

			RequerimentoAtividade(esp);

			Destinatario(especificidade.ProtocoloReq.Id, esp.Destinatarios);

			if (esp.Dominio == 0)
			{
				Validacao.Add(Mensagem.CertidaoCartaAnuenciaMsg.DominioObrigatorio);
			}
			else if (!_da.ValidarDominio(esp.Dominio))
			{
				Validacao.Add(Mensagem.CertidaoCartaAnuenciaMsg.DominioInexistente);
			}

			if (string.IsNullOrWhiteSpace(esp.Descricao))
			{
				Validacao.Add(Mensagem.CertidaoCartaAnuenciaMsg.DescricaoObrigatoria);
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