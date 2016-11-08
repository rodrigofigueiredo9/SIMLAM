using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCertificado;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloCertificado.Business
{
	public class CertificadoCadastroProdutoVegetalValidar : EspecificidadeValidarBase, IEspecificiadeValidar
	{
		public bool Salvar(IEspecificidade especificidade)
		{
			CertificadoCadastroProdutoVegetal esp = especificidade as CertificadoCadastroProdutoVegetal;

			RequerimentoAtividade(esp, false, true);

			Destinatario(especificidade.ProtocoloReq.Id, esp.Destinatario.GetValueOrDefault(0), "#ddlDestinatarios");

			if (string.IsNullOrEmpty(esp.Nome))
			{
				Validacao.Add(Mensagem.CertificadoCadastroProtudoVegetalMsg.InformeNomeProduto);
			}

			if (string.IsNullOrEmpty(esp.Fabricante))
			{
				Validacao.Add(Mensagem.CertificadoCadastroProtudoVegetalMsg.InformeFabricanteProduto);
			}

			if (string.IsNullOrEmpty(esp.ClasseToxicologica))
			{
				Validacao.Add(Mensagem.CertificadoCadastroProtudoVegetalMsg.InformeClasseToxilogica);
			}

			if (string.IsNullOrEmpty(esp.Classe))
			{
				Validacao.Add(Mensagem.CertificadoCadastroProtudoVegetalMsg.InformeClasse);
			}

			if (string.IsNullOrEmpty(esp.Ingrediente))
			{
				Validacao.Add(Mensagem.CertificadoCadastroProtudoVegetalMsg.InformeIngredienteAtivo);
			}

			if (string.IsNullOrEmpty(esp.Classificacao))
			{
				Validacao.Add(Mensagem.CertificadoCadastroProtudoVegetalMsg.InformeClassificacaoQtoPotPericAmbiental);
			}

			if (string.IsNullOrEmpty(esp.Cultura))
			{
				Validacao.Add(Mensagem.CertificadoCadastroProtudoVegetalMsg.InformeCulturaIndicada);
			}

			return Validacao.EhValido;
		}

		public bool Emitir(IEspecificidade especificidade)
		{
			return Salvar(especificidade);
		}
	}
}