using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloSilvicultura;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloSilvicultura.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloSilvicultura.Business
{
	public class SilviculturaValidar
	{

		#region Propriedades

		SilviculturaDa _da = new SilviculturaDa();
		CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();

		#endregion

		internal bool Salvar(Silvicultura caracterizacao)
		{
			if (!_caracterizacaoValidar.Basicas(caracterizacao.EmpreendimentoId))
			{
				return false;
			}

			if (caracterizacao.Id <= 0 && (_da.ObterPorEmpreendimento(caracterizacao.EmpreendimentoId, true) ?? new Silvicultura()).Id > 0)
			{
				Validacao.Add(Mensagem.Caracterizacao.EmpreendimentoCaracterizacaoJaCriada);
				return false;
			}

			if (!Acessar(caracterizacao.EmpreendimentoId))
			{
				return false;
			}

			#region Silviculturas

			foreach (var silvicultura in caracterizacao.Silviculturas)
			{
				if (silvicultura.Culturas.Count <= 0) 
				{
					Validacao.Add(Mensagem.Silvicultura.CulturaObrigatorio);
				}
			}

			#endregion

			return Validacao.EhValido;
		}

		public bool Acessar(int empreendimentoId)
		{
			if (!_caracterizacaoValidar.Dependencias(empreendimentoId, (int)eCaracterizacao.Silvicultura))
			{
				return false;
			}

			return Validacao.EhValido;
		}

		public string AbrirModalAcessar(Silvicultura caracterizacao)
		{
			EmpreendimentoCaracterizacao empreendimento = _caracterizacaoBus.ObterEmpreendimentoSimplificado(caracterizacao.EmpreendimentoId);

			/*if (empreendimento.ZonaLocalizacao == eZonaLocalizacao.Rural)
			{
				DominialidadeDa dominialidadeDa = new DominialidadeDa();
				Dominialidade dominialidade = dominialidadeDa.ObterPorEmpreendimento(caracterizacao.EmpreendimentoId);

				foreach (Dominio dominio in dominialidade.Dominios)
				{
					if (dominio.ReservasLegais.Exists(x => x.SituacaoId == (int)eReservaLegalSituacao.NaoInformada))
					{
						return Mensagem.Silvicultura.EmpreendimentoRuralReservaIndefinida.Texto;
					}
				}
			}*/

			return string.Empty;
		}
	}
}
