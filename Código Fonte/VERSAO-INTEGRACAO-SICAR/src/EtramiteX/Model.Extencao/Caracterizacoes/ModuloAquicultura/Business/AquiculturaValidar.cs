using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloAquicultura;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloAquicultura.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloAquicultura.Business
{
	public class AquiculturaValidar
	{
		#region Propriedades

		AquiculturaDa _da = new AquiculturaDa();
		CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
		CoordenadaAtividadeValidar _coordenadaValidar = new CoordenadaAtividadeValidar();

		#endregion

		internal bool Salvar(Aquicultura caracterizacao)
		{
			if (!_caracterizacaoValidar.Basicas(caracterizacao.EmpreendimentoId))
			{
				return false;
			}

			if (caracterizacao.Id <= 0 && (_da.ObterPorEmpreendimento(caracterizacao.EmpreendimentoId, true) ?? new Aquicultura()).Id > 0)
			{
				Validacao.Add(Mensagem.Caracterizacao.EmpreendimentoCaracterizacaoJaCriada);
				return false;
			}

			if (!Acessar(caracterizacao.EmpreendimentoId))
			{
				return false;
			}

			List<String> atividadesDuplicadas = new List<String>();

			List<int> atividadesIdGrupo1 = ConfiguracaoAtividade.ObterId(
				new[]{ (int)eAtividadeCodigo.PisciculturaCarciniculturaEspeciesAguaDoceViveirosEscavadosInclusivePolicultivo01,
						(int)eAtividadeCodigo.PisciculturaEspeciesAguaDoceViveirosEscavadosUnidadePescaEsportivatipo02 , 
						(int)eAtividadeCodigo.CarciniculturaEspeciesAguaDoceViveirosEscavados03}).ToList();
			int atividadeIdGrupo3 = ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.CriacaoAnimaisConfinadosPequenoPorteAmbienteAquatico10);

			foreach (var item in caracterizacao.AquiculturasAquicult)
			{
				#region Atividade

				if (item.Atividade <= 0)
				{
					Validacao.Add(Mensagem.Aquicultura.AtividadeObrigatoria(item.Identificador));
				}
				else
				{

					if (caracterizacao.AquiculturasAquicult.Where(x => x.Atividade == item.Atividade).ToList().Count >= 2)
					{
						atividadesDuplicadas.Add(item.Identificador);
					}

					Boolean atividadeGrupo01 = atividadesIdGrupo1.Any(x => x == item.Atividade);
					Boolean atividadeGrupo03 = atividadeIdGrupo3 == item.Atividade;
					Boolean atividadeGrupo02 = (!atividadeGrupo01 && !atividadeGrupo03);

					#region Grupo de Atividade 01

					if (atividadeGrupo01)
					{
						#region Area Total Inundada

						if (!String.IsNullOrWhiteSpace(item.AreaInundadaTotal))
						{
							decimal aux = 0;

							if (Decimal.TryParse(item.AreaInundadaTotal, out aux))
							{
								if (aux <= 0)
								{
									Validacao.Add(Mensagem.Aquicultura.AreaInundadaTotalMaiorZero(item.Identificador));
								}
							}
							else
							{
								Validacao.Add(Mensagem.Aquicultura.AreaInundadaTotalInvalido(item.Identificador));
							}
						}
						else
						{
							Validacao.Add(Mensagem.Aquicultura.AreaInundadaTotalObrigatorio(item.Identificador));
						}

						#endregion

						#region Nº de viveiros escavados

						if (!String.IsNullOrWhiteSpace(item.NumViveiros))
						{
							decimal aux = 0;

							if (Decimal.TryParse(item.NumViveiros, out aux))
							{
								if (aux <= 0)
								{
									Validacao.Add(Mensagem.Aquicultura.NumViveiroMaiorZero(item.Identificador));
								}
							}
							else
							{
								Validacao.Add(Mensagem.Aquicultura.NumViveiroInvalido(item.Identificador));
							}
						}
						else
						{
							Validacao.Add(Mensagem.Aquicultura.NumViveiroObrigatorio(item.Identificador));
						}

						#endregion

						item.NumUnidadeCultivos = string.Empty;
						item.Cultivos.Clear();
						item.AreaCultivo = string.Empty;
					}

					#endregion

					#region Grupo de Atividade 02

					if (atividadeGrupo02)
					{
						#region Cultivos

						if (item.Cultivos.Count <= 0)
						{
							Validacao.Add(Mensagem.Aquicultura.CultivosObrigatorio);
						}

						if (!String.IsNullOrWhiteSpace(item.NumUnidadeCultivos))
						{
							int aux = 0;
							Int32.TryParse(item.NumUnidadeCultivos, out aux);

							if (aux <= 0)
							{
								Validacao.Add(Mensagem.Aquicultura.NumUnidadeCultivosMaiorZero(item.Identificador));
							}
						}
						else
						{
							Validacao.Add(Mensagem.Aquicultura.NumUnidadeCultivosObrigatorio(item.Identificador));
						}

						int numeroUnidadeCultivos = 0;
						Int32.TryParse(item.NumUnidadeCultivos, out numeroUnidadeCultivos);

						if (numeroUnidadeCultivos < item.Cultivos.Count)
						{
							Validacao.Add(Mensagem.Aquicultura.NumUnidadeCultivosMenorQueCultivosAdicionados(item.Identificador));
						}

						#endregion

						item.AreaInundadaTotal = string.Empty;
						item.NumViveiros = string.Empty;
						item.AreaCultivo = string.Empty;
					}

					#endregion

					#region Grupo de Atividade 03

					if (atividadeGrupo03)
					{
						#region Área do Cultivo

						if (!String.IsNullOrWhiteSpace(item.AreaCultivo))
						{
							decimal aux = 0;

							if (Decimal.TryParse(item.AreaCultivo, out aux))
							{
								if (aux <= 0)
								{
									Validacao.Add(Mensagem.Aquicultura.AreaCultivoMaiorZero(item.Identificador));
								}
							}
							else
							{
								Validacao.Add(Mensagem.Aquicultura.AreaCultivoInvalido(item.Identificador));
							}
						}
						else
						{
							Validacao.Add(Mensagem.Aquicultura.AreaCultivoObrigatorio(item.Identificador));
						}

						#endregion

						item.AreaInundadaTotal = string.Empty;
						item.NumViveiros = string.Empty;
						item.NumUnidadeCultivos = string.Empty;
						item.Cultivos.Clear();
					}

					#endregion
				}

				#endregion

				#region Coordenadas

				if (item.CoordenadaAtividade.Id <= 0)
				{
					Validacao.Add(Mensagem.Aquicultura.CoordenadaAtividadeObrigatoria(item.Identificador));
				}

				if (item.CoordenadaAtividade.Tipo <= 0)
				{
					Validacao.Add(Mensagem.Aquicultura.GeometriaTipoObrigatorio(item.Identificador));
				}

				#endregion
			}

			if (atividadesDuplicadas.Count > 0)
			{
				Validacao.Add(Mensagem.Aquicultura.AtividadeDuplicada(atividadesDuplicadas));
			}

			return Validacao.EhValido;

		}

		public bool Acessar(int empreendimentoId)
		{
			if (!_caracterizacaoValidar.Dependencias(empreendimentoId, (int)eCaracterizacao.Aquicultura))
			{
				return false;
			}

			return Validacao.EhValido;
		}
	}
}