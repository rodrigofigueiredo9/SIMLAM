using System;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloTerraplanagem;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloTerraplanagem.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloTerraplanagem.Business
{
	public class TerraplanagemValidar
	{
		#region Propriedades

		TerraplanagemDa _da = new TerraplanagemDa();
		CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
		CoordenadaAtividadeValidar _coordenadaValidar = new CoordenadaAtividadeValidar();

		#endregion

		internal bool Salvar(Terraplanagem caracterizacao)
		{
			if (!_caracterizacaoValidar.Basicas(caracterizacao.EmpreendimentoId))
			{
				return false;
			}

			if (caracterizacao.Id <= 0 && (_da.ObterPorEmpreendimento(caracterizacao.EmpreendimentoId, true) ?? new Terraplanagem()).Id > 0)
			{
				Validacao.Add(Mensagem.Caracterizacao.EmpreendimentoCaracterizacaoJaCriada);
				return false;
			}

			if (!Acessar(caracterizacao.EmpreendimentoId))
			{
				return false;
			}

			if (caracterizacao.Atividade <= 0)
			{
				Validacao.Add(Mensagem.Terraplanagem.AtividadeObrigatoria);
			}

			_coordenadaValidar.Salvar(caracterizacao.CoordenadaAtividade);

			#region Área de terraplanagem 

			if (!String.IsNullOrWhiteSpace(caracterizacao.Area))
			{
				decimal aux = 0;
				if (!Decimal.TryParse(caracterizacao.Area, out aux))
				{
					Validacao.Add(Mensagem.Terraplanagem.AreaInvalida);
				}
				else if (aux <= 0)
				{
					Validacao.Add(Mensagem.Terraplanagem.AreaMaiorZero);
				}
			}
			else
			{
				Validacao.Add(Mensagem.Terraplanagem.AreaObrigatoria);
			}

			#endregion

			#region Volume de terra movimentado

			if (!String.IsNullOrWhiteSpace(caracterizacao.VolumeMovimentado))
			{
				decimal aux = 0;
				if (!Decimal.TryParse(caracterizacao.VolumeMovimentado, out aux))
				{
					Validacao.Add(Mensagem.Terraplanagem.VolumeInvalido);
				}
				else if (aux <= 0)
				{
					Validacao.Add(Mensagem.Terraplanagem.VolumeMaiorZero);
				}
			}

			#endregion

			return Validacao.EhValido;
		}

		public bool Acessar(int empreendimentoId)
		{
			if (!_caracterizacaoValidar.Dependencias(empreendimentoId, (int)eCaracterizacao.Terraplanagem))
			{
				return false;
			}

			return Validacao.EhValido;
		}
	}
}
