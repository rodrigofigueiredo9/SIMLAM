using System;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business
{
	public class ComplementacaoDadosValidar
	{
		public bool Salvar(ComplementacaoDados complementacao)
		{
			#region Dados do Responsavel

			if (complementacao.AutuadoTipo == (int)eTipoAutuado.Empreendimento) 
			{
				if (complementacao.AutuadoId <= 0)
				{
					Validacao.Add(Mensagem.ComplementacaoDados.ResponsavelObrigatorio);
				}
			}

			if (complementacao.ResidePropriedadeTipo <= 0) 
			{
				Validacao.Add(Mensagem.ComplementacaoDados.ResidePropriedadeObrigatorio);
			}

			if (complementacao.RendaMensalFamiliarTipo <= 0)
			{
				Validacao.Add(Mensagem.ComplementacaoDados.RendaMensalFamiliarObrigatoria);
			}

			if (complementacao.NivelEscolaridadeTipo <= 0)
			{
				Validacao.Add(Mensagem.ComplementacaoDados.NivelEscolaridadeObrigatorio);
			}

			if (complementacao.VinculoComPropriedadeTipo <= 0)
			{
				Validacao.Add(Mensagem.ComplementacaoDados.VinculoComPropriedadeObrigatorio);
			}
			else 
			{
				if (complementacao.VinculoComPropriedadeTipo == (int)eVinculoPropriedade.Outro) 
				{
					if (String.IsNullOrWhiteSpace(complementacao.VinculoComPropriedadeEspecificarTexto))
					{
						Validacao.Add(Mensagem.ComplementacaoDados.VinculoComPropriedadeEspecificarObrigatorio);
					}
				}
			}

			if (complementacao.ConhecimentoLegislacaoTipo <= 0)
			{
				Validacao.Add(Mensagem.ComplementacaoDados.ConhecimentoLegislacaoObrigatorio);
			}
			else 
			{
				if (complementacao.ConhecimentoLegislacaoTipo != (int)eRespostasDefault.NaoSeAplica) 
				{
					if (String.IsNullOrWhiteSpace(complementacao.Justificativa)) 
					{
						Validacao.Add(Mensagem.ComplementacaoDados.JustificativaObrigatoria);
					}
				}
			}

			#endregion

			#region Dados da propriedade

			if (complementacao.AutuadoTipo == (int)eTipoAutuado.Empreendimento)
			{
				#region AreaTotalInformada

				if (!String.IsNullOrWhiteSpace(complementacao.AreaTotalInformada))
				{
					Decimal aux = 0;
					if (Decimal.TryParse(complementacao.AreaTotalInformada, out aux))
					{
						if (aux <= 0)
						{
							Validacao.Add(Mensagem.ComplementacaoDados.AreaTotalInformadaMaiorZero);
						}
					}
					else
					{
						Validacao.Add(Mensagem.ComplementacaoDados.AreaTotalInformadaInvalida);
					}
				}

				#endregion

				#region AreaCoberturaFlorestalNativa

				if (!String.IsNullOrWhiteSpace(complementacao.AreaCoberturaFlorestalNativa))
				{
					Decimal aux = 0;
					if (Decimal.TryParse(complementacao.AreaCoberturaFlorestalNativa, out aux))
					{
						if (aux <= 0)
						{
							Validacao.Add(Mensagem.ComplementacaoDados.AreaCoberturaFlorestalNativaMaiorZero);
						}
					}
					else
					{
						Validacao.Add(Mensagem.ComplementacaoDados.AreaCoberturaFlorestalNativaInvalida);
					}
				}

				#endregion


				if (complementacao.ReservalegalTipo <= 0)
				{
					Validacao.Add(Mensagem.ComplementacaoDados.ReservalegalObrigatoria);
				}

			}

			#endregion

			return Validacao.EhValido;
		}
	}
}
