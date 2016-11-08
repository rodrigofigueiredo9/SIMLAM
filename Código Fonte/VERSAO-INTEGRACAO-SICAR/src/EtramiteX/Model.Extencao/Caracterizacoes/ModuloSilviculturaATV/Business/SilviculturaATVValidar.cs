using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloSilviculturaATV;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloSilviculturaATV.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloSilviculturaATV.Business
{
	public class SilviculturaATVValidar
	{

		#region Propriedades

		SilviculturaATVDa _da = new SilviculturaATVDa();
		CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();

		#endregion

		internal bool Salvar(SilviculturaATV caracterizacao)
		{
			if (!_caracterizacaoValidar.Basicas(caracterizacao.EmpreendimentoId))
			{
				return false;
			}

			if (caracterizacao.Id <= 0 && (_da.ObterPorEmpreendimento(caracterizacao.EmpreendimentoId, true) ?? new SilviculturaATV()).Id > 0)
			{
				Validacao.Add(Mensagem.Caracterizacao.EmpreendimentoCaracterizacaoJaCriada);
				return false;
			}

			if (!Acessar(caracterizacao.EmpreendimentoId))
			{
				return false;
			}
			
			if (caracterizacao.Areas.Exists(x => x.Tipo == (int)eSilviculturaAreaATV.DECLIVIDADE && (x.Valor == 0)))
			{
				Validacao.Add(Mensagem.SilviculturaAtvMsg.AreaDeclividadeObrigatoria);
			}

			#region Caracteristicas

			foreach (var item in caracterizacao.Caracteristicas)
			{
				if (item.Fomento == eFomentoTipoATV.Nulo)
				{
					this.AddFormarMsg(Mensagem.SilviculturaAtvMsg.SelecioneFomento, item.Identificacao);
				}

				if (!item.DeclividadeToDecimal.HasValue || item.DeclividadeToDecimal.GetValueOrDefault(0) == 0)
				{
					this.AddFormarMsg(Mensagem.SilviculturaAtvMsg.DeclividadeObrigatoria, item.Identificacao);
				}

				if (!item.TotalRequeridaToDecimal.HasValue || item.TotalRequeridaToDecimal.GetValueOrDefault(0) == 0)
				{
					this.AddFormarMsg(Mensagem.SilviculturaAtvMsg.TotalRequeridaObrigatoria, item.Identificacao);
				}

				if (!item.TotalPlantadaComEucaliptoToDecimal.HasValue || item.TotalPlantadaComEucaliptoToDecimal.GetValueOrDefault(0) == 0)
				{
					this.AddFormarMsg(Mensagem.SilviculturaAtvMsg.TotalPlantadaComEucaliptoObrigatoria, item.Identificacao);
				}

				if (item.Culturas.Count <= 0)
				{
					Validacao.Add(Mensagem.SilviculturaAtvMsg.CulturaObrigatorio);
				}			
			}

			#endregion

			return Validacao.EhValido;
		}

		public bool Acessar(int empreendimentoId)
		{
			if (!_caracterizacaoValidar.Dependencias(empreendimentoId, (int)eCaracterizacao.SilviculturaATV))
			{
				return false;
			}

			return Validacao.EhValido;
		}

		public string AbrirModalAcessar(SilviculturaATV caracterizacao)
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

		internal void AddFormarMsg(Mensagem msg, string complemento)
		{
			int index = Validacao.Erros.FindIndex(x => !string.IsNullOrEmpty(x.Campo) && x.Campo.Contains(msg.Campo));

			if (index > -1)
			{
				Validacao.Erros[index].Campo += "," + msg.Campo + complemento;
			}
			else
			{
				Validacao.Add(new Mensagem { Tipo = msg.Tipo, Texto = msg.Texto, Campo = msg.Campo + complemento });
			}
		}
	}
}
