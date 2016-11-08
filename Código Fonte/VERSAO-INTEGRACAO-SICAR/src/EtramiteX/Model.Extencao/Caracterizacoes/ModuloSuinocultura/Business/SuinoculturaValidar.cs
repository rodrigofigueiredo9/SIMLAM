using System;
using System.Linq;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloSuinocultura;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloSuinocultura.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloSuinocultura.Business
{
	public class SuinoculturaValidar
	{

		#region Propriedades

		SuinoculturaDa _da = new SuinoculturaDa();
		CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
		CoordenadaAtividadeValidar _coordenadaValidar = new CoordenadaAtividadeValidar();

		#endregion

		internal bool Salvar(Suinocultura caracterizacao)
		{
			if (!_caracterizacaoValidar.Basicas(caracterizacao.EmpreendimentoId))
			{
				return false;
			}

			if (caracterizacao.Id <= 0 && (_da.ObterPorEmpreendimento(caracterizacao.EmpreendimentoId, true) ?? new Suinocultura()).Id > 0)
			{
				Validacao.Add(Mensagem.Caracterizacao.EmpreendimentoCaracterizacaoJaCriada);
				return false;
			}

			if (!Acessar(caracterizacao.EmpreendimentoId))
			{
				return false;
			}

			#region Atividade

			if (caracterizacao.Atividade <= 0)
			{
				Validacao.Add(Mensagem.Suinocultura.AtividadeObrigatoria);
			}
			else
			{
				if (ConfiguracaoAtividade.ObterId(new[]{ 
					(int)eAtividadeCodigo.SuinoculturaCicloCompleto, 
					(int)eAtividadeCodigo.SuinoculturaComLançamentoEfluenteLiquidos, 
					(int)eAtividadeCodigo.SuinoculturaExclusivoTerminacao}).Any(x => x == caracterizacao.Atividade))
				{

					if (!String.IsNullOrWhiteSpace(caracterizacao.NumeroCabecas))
					{
						decimal aux = 0;
						if (Decimal.TryParse(caracterizacao.NumeroCabecas, out aux))
						{
							if (aux <= 0)
							{
								Validacao.Add(Mensagem.Suinocultura.NumeroCabecaMaiorZero);
							}
						}
						else
						{
							Validacao.Add(Mensagem.Suinocultura.NumeroCabecaInvalido);
						}
					}
					else
					{
						Validacao.Add(Mensagem.Suinocultura.NumeroCabecaObrigatorio);
					}

				}

				if (caracterizacao.Atividade == ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.SuinoculturaExclusivoProducaoLeitoes))
				{
					if (!String.IsNullOrWhiteSpace(caracterizacao.NumeroMatrizes))
					{
						decimal aux = 0;
						if (Decimal.TryParse(caracterizacao.NumeroMatrizes, out aux))
						{
							if (aux <= 0)
							{
								Validacao.Add(Mensagem.Suinocultura.NumeroMatrizesMaiorZero);
							}
						}
						else
						{
							Validacao.Add(Mensagem.Suinocultura.NumeroMatrizesInvalido);
						}
					}
					else
					{
						Validacao.Add(Mensagem.Suinocultura.NumeroMatrizesObrigatorio);
					}

				}

			}

			#endregion

			if (caracterizacao.Fase <= 0) {
				Validacao.Add(Mensagem.Suinocultura.FaseObrigatoria);
			}


			if (caracterizacao.ExisteBiodigestor == null) 
			{
				Validacao.Add(Mensagem.Suinocultura.ExisteBiodigestorObrigatorio);
			}

			if (caracterizacao.PossuiFabricaRacao == null)
			{
				Validacao.Add(Mensagem.Suinocultura.PossuiFabricaRacaoObrigatorio);
			}

			_coordenadaValidar.Salvar(caracterizacao.CoordenadaAtividade);

			return Validacao.EhValido;

		}

		public bool Acessar(int empreendimentoId)
		{
			if (!_caracterizacaoValidar.Dependencias(empreendimentoId, (int)eCaracterizacao.Suinocultura))
			{
				return false;
			}

			return Validacao.EhValido;
		}
	}
}
