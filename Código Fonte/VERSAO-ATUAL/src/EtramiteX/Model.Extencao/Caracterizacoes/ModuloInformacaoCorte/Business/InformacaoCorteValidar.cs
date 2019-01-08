using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloInformacaoCorte;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloInformacaoCorte.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloInformacaoCorte.Business
{
	public class InformacaoCorteValidar
	{
		#region Propriedades

		InformacaoCorteDa _da = new InformacaoCorteDa();
		CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();

		#endregion

		public bool Salvar(InformacaoCorte caracterizacao) 
		{
			if (!_caracterizacaoValidar.Basicas(caracterizacao.Empreendimento.Id))
				return false;

			if (!Acessar(caracterizacao.Empreendimento.Id))
				return false;

			if (caracterizacao.AreaFlorestaPlantada > 100)
			{
				if (caracterizacao.InformacaoCorteLicenca.Count < 1)
					Validacao.Add(Mensagem.InformacaoCorte.LicencaObrigatoria);
			}

			if (caracterizacao.InformacaoCorteTipo.Count < 1)
				Validacao.Add(Mensagem.InformacaoCorte.InformacaoCorteListaObrigatorio);

			foreach(var item in caracterizacao.InformacaoCorteLicenca)
			{
				if (!item.DataVencimento.IsValido)
					Validacao.Add(Mensagem.InformacaoCorte.DataVencimentoInvalida(item.NumeroLicenca));
			}

			return Validacao.EhValido;
		}

		public bool Acessar(int empreendimentoId)
		{
			if (!_caracterizacaoValidar.Dependencias(empreendimentoId, (int)eCaracterizacao.InformacaoCorte))
				return false;

			return Validacao.EhValido;
		}
	}
}