using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloLicenca;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloLicenca.Business
{
	public class LicencaOperacaoFomentoValidar : EspecificidadeValidarBase, IEspecificiadeValidar
	{
		EspecificidadeDa _daEspecificidade = new EspecificidadeDa();

		public bool Salvar(IEspecificidade especificidade)
		{
			LicencaOperacaoFomento esp = especificidade as LicencaOperacaoFomento;
			CaracterizacaoBus caracterizacaoBus = new CaracterizacaoBus();
			List<Caracterizacao> caracterizacoes = caracterizacaoBus.ObterCaracterizacoesEmpreendimento(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault());

			RequerimentoAtividade(esp, faseAnterior: false);

			int idCaracterizacao = caracterizacaoBus.Existe(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault(), eCaracterizacao.SilviculturaPPFF);

			if (idCaracterizacao > 0)
			{
				ICaracterizacaoBus busCaract = CaracterizacaoBusFactory.Criar(eCaracterizacao.SilviculturaPPFF);

				bool isPossui = false;

				busCaract.ObterAtividadesCaracterizacao(especificidade.Titulo.EmpreendimentoId.Value).ForEach(x =>
				{
					if (esp.Atividades[0].Id == x)
					{
						isPossui = true;
						return;
					}
				});

				if (!isPossui)
				{
					Validacao.Add(Mensagem.LicencaOperacaoFomentoMsg.CaracterizacaoAtividadeInexistente);
				}
			}
			else
			{
				Validacao.Add(Mensagem.LicencaOperacaoFomentoMsg.SilviculturaPPFFInexistente);
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
