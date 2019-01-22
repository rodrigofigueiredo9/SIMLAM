using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloRegularizacaoFundiaria;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloRegularizacaoFundiaria.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloRegularizacaoFundiaria.Business
{
	public class RegularizacaoFundiariaValidar
	{
		#region Propriedades

		RegularizacaoFundiariaDa _da;
		CaracterizacaoValidar _caracterizacaoValidar;

		public RegularizacaoFundiariaValidar()
		{
			_da = new RegularizacaoFundiariaDa();
			_caracterizacaoValidar = new CaracterizacaoValidar();
		}

		#endregion

		public bool Salvar(RegularizacaoFundiaria caracterizacao)
		{
			List<string> regularizacoesInvalidas = new List<string>();
			foreach (Posse posse in caracterizacao.Posses)
			{
				if (!Posse(posse, caracterizacao.EmpreendimentoId))
				{
					regularizacoesInvalidas.Add(posse.Identificacao);
				}
			}
			Validacao.Erros.Clear();

			if (!Acessar(caracterizacao))
			{
				return false;
			}

			if (caracterizacao.Id <= 0)
			{
				if ((_da.ObterPorEmpreendimento(caracterizacao.EmpreendimentoId) ?? new RegularizacaoFundiaria()).Id > 0)
				{
					Validacao.Add(Mensagem.Caracterizacao.EmpreendimentoCaracterizacaoJaCriada);
					return false;
				}
			}

			if (regularizacoesInvalidas.Count > 0)
			{
				foreach (var item in regularizacoesInvalidas)
				{
					Validacao.Add(Mensagem.RegularizacaoFundiaria.RegularizacaoInvalida(item));
				}
			}

			return Validacao.EhValido;
		}

		public bool Posse(Posse posse, int empreendimentoId)
		{
			if (string.IsNullOrWhiteSpace(posse.Identificacao))
			{
				Validacao.Add(Mensagem.RegularizacaoFundiaria.CampoObrigatorio);
			}

			if (posse.AreaCroqui <= 0)
			{
				Validacao.Add(Mensagem.RegularizacaoFundiaria.CampoObrigatorio);
			}

			if (posse.AreaRequerida <= 0)
			{
				Validacao.Add(Mensagem.RegularizacaoFundiaria.AreaRequeridaObrigatoria);
			}

			if (posse.RegularizacaoTipo <= 0)
			{
				Validacao.Add(Mensagem.RegularizacaoFundiaria.TipoRegularizacaoObrigatorio);
			}

			#region Dominios Avulsos

			if (!posse.PossuiDominioAvulso.HasValue)
			{
				Validacao.Add(Mensagem.RegularizacaoFundiaria.PossuiDominioAvulsoObrigatorio);
			}

			if (posse.PossuiDominioAvulsoBool)
			{
				if (posse.DominiosAvulsos == null || posse.DominiosAvulsos.Count <= 0)
				{
					Validacao.Add(Mensagem.RegularizacaoFundiaria.DominioAvulsoObrigatorio);
				}
				else
				{
					posse.DominiosAvulsos.ForEach(dominio => {
						DominioAvulso(posse.DominiosAvulsos.Where(x => x != dominio).ToList(), dominio);
					});
				}
			}

			#endregion

			#region Transmitentes

			if (posse.Transmitentes.Count(x => x.TempoOcupacao <= 0) > 0)
			{
				Validacao.Add(Mensagem.RegularizacaoFundiaria.TempoDeOcupacaoObrigatorio);
			}

			if (posse.Transmitentes.Count(x => x.Transmitente.Id <= 0) > 0)
			{
				Validacao.Add(Mensagem.RegularizacaoFundiaria.TrasmitenteObrigatorio);
			}

			if (posse.Transmitentes.Any(x => posse.Transmitentes.Count(y => y.Transmitente.Id == x.Transmitente.Id) > 1))
			{
				Validacao.Add(Mensagem.RegularizacaoFundiaria.TrasmitenteJaAdicionado);
			}

			#endregion

			if (_da.EmpreendimentoZonaAlterada(empreendimentoId, posse.Zona))
			{
				Validacao.Add(Mensagem.Caracterizacao.SegmentoEmpreendimentoAlterado);
				return Validacao.EhValido;
			}

			
			#region Zona Rural

			if (posse.Zona == (int)eZonaLocalizacao.Rural)
			{
				if (posse.UsoAtualSolo.Count <= 0)
				{
					Validacao.Add(Mensagem.RegularizacaoFundiaria.UsoSoloObrigatorio);
				}

				if (posse.UsoAtualSolo.Count(x => x.AreaPorcentagem <= 0) > 0)
				{
					Validacao.Add(Mensagem.RegularizacaoFundiaria.UsoSoloTipoObrigatorio);
				}

				int porcentagem = posse.UsoAtualSolo.Sum(x => x.AreaPorcentagem);
				if (porcentagem > 100)
				{
					Validacao.Add(Mensagem.RegularizacaoFundiaria.UsoSoloLimitePorcentagem);
				}

				if (posse.RelacaoTrabalho <= 0)
				{
					Validacao.Add(Mensagem.RegularizacaoFundiaria.RelacaoTrabalhoObrigatorio);
				}

				if (string.IsNullOrWhiteSpace(posse.Benfeitorias))
				{
					Validacao.Add(Mensagem.RegularizacaoFundiaria.BenfeitoriasEdificacoesObrigatorio);
				}
			}

			#endregion

			#region Opções

			foreach (Opcao opcao in posse.Opcoes)
			{
				switch (opcao.TipoEnum)
				{
					case eTipoOpcao.TerrenoDevoluto:

						if (!opcao.Valor.HasValue)
						{
							Validacao.Add(Mensagem.RegularizacaoFundiaria.TerrenoDevolutoObrigatorio);
							break;
						}

						if (String.IsNullOrEmpty(opcao.Outro) || opcao.Outro == "0")
						{
							if (opcao.Valor == 0)
							{
								Validacao.Add(Mensagem.RegularizacaoFundiaria.EspecificarDominialidadeObrigatorio);
							}
							else
							{
								Validacao.Add(Mensagem.RegularizacaoFundiaria.HomologacaoAprovadaObrigatoria);
							}
						}
						break;

					case eTipoOpcao.RequerenteResideNaPosse:

						if (!opcao.Valor.HasValue)
						{
							Validacao.Add(Mensagem.RegularizacaoFundiaria.RequerenteResidePosseObrigatorio);
						}

						ValidacoesGenericasBus.DataMensagem(new DateTecno() { DataTexto = opcao.Outro }, "DataArquisicao", "aquisição ou ocupação");
						break;

					case eTipoOpcao.ExisteLitigio:

						if (!opcao.Valor.HasValue)
						{
							Validacao.Add(Mensagem.RegularizacaoFundiaria.ExisteLitigioObrigatorio);
							break;
						}

						if (opcao.Valor == 1 && String.IsNullOrEmpty(opcao.Outro))
						{
							Validacao.Add(Mensagem.RegularizacaoFundiaria.NomeLitigioObrigatorio);
						}
						break;

					case eTipoOpcao.SobrepoeSeDivisa:

						if (!opcao.Valor.HasValue)
						{
							Validacao.Add(Mensagem.RegularizacaoFundiaria.SobrepoeFaixaDivisaObrigatorio);
							break;
						}

						if (opcao.Valor == 1 && opcao.Outro == "0")
						{
							Validacao.Add(Mensagem.RegularizacaoFundiaria.AQuemPertenceLimiteObrigatorio);
						}
						break;

					case eTipoOpcao.BanhadoPorRioCorrego:

						if (!opcao.Valor.HasValue)
						{
							Validacao.Add(Mensagem.RegularizacaoFundiaria.BanhadoRioCorregoObrigatorio);
							break;
						}

						if (opcao.Valor == 1 && String.IsNullOrEmpty(opcao.Outro))
						{
							Validacao.Add(Mensagem.RegularizacaoFundiaria.NomeRioCorregoObrigatorio);
						}
						break;

					case eTipoOpcao.PossuiNascente:

						if (!opcao.Valor.HasValue)
						{
							Validacao.Add(Mensagem.RegularizacaoFundiaria.PossuiNascenteObrigatorio);
						}
						break;

					case eTipoOpcao.RedeAgua:

						if (!opcao.Valor.HasValue)
						{
							Validacao.Add(Mensagem.RegularizacaoFundiaria.RedeAguaObrigatorio);
						}
						break;

					case eTipoOpcao.RedeEsgoto:

						if (!opcao.Valor.HasValue)
						{
							Validacao.Add(Mensagem.RegularizacaoFundiaria.RedeEsgotoObrigatorio);
						}
						break;

					case eTipoOpcao.LuzEletrica:

						if (!opcao.Valor.HasValue)
						{
							Validacao.Add(Mensagem.RegularizacaoFundiaria.LuzEletricaDomiciliarObrigatorio);
						}
						break;

					case eTipoOpcao.IluminacaoPublica:

						if (!opcao.Valor.HasValue)
						{
							Validacao.Add(Mensagem.RegularizacaoFundiaria.IluminacaoViaPublicaObrigatorio);
						}
						break;

					case eTipoOpcao.RedeTelefonica:

						if (!opcao.Valor.HasValue)
						{
							Validacao.Add(Mensagem.RegularizacaoFundiaria.RedeTelefonicaObrigatorio);
						}
						break;

					case eTipoOpcao.Calcada:

						if (!opcao.Valor.HasValue)
						{
							Validacao.Add(Mensagem.RegularizacaoFundiaria.CalcadaObrigatorio);
						}
						break;

					case eTipoOpcao.Pavimentacao:

						if (!opcao.Valor.HasValue)
						{
							Validacao.Add(Mensagem.RegularizacaoFundiaria.PavimentacaoObrigatorio);
							break;
						}

						if (opcao.Valor == 1 && String.IsNullOrEmpty(opcao.Outro))
						{
							Validacao.Add(Mensagem.RegularizacaoFundiaria.TipoPavimentacaoObrigatorio);
						}
						break;
				}
			}

			#endregion

			return Validacao.EhValido;
		}

		public bool Acessar(RegularizacaoFundiaria caracterizacao, bool isVisualizar = false)
		{
			if (!_caracterizacaoValidar.Basicas(caracterizacao.EmpreendimentoId, isVisualizar))
			{
				return false;
			}

			if (!_caracterizacaoValidar.Dependencias(caracterizacao.EmpreendimentoId, (int)eCaracterizacao.RegularizacaoFundiaria))
			{
				return false;
			}

			//if (caracterizacao.Posses == null || caracterizacao.Posses.Count <= 0)
			//{
			//	Validacao.Add(Mensagem.RegularizacaoFundiaria.PossesObrigatorio);
			//}

			return Validacao.EhValido;
		}

		public void DominioAvulso(List<Dominio> dominioLista, Dominio dominio)
		{
			dominioLista = dominioLista ?? new List<Dominio>();

			if (string.IsNullOrWhiteSpace(dominio.Matricula))
			{
				Validacao.Add(Mensagem.RegularizacaoFundiaria.MatriculaObrigatoria);
			}

			if (string.IsNullOrWhiteSpace(dominio.Folha))
			{
				Validacao.Add(Mensagem.RegularizacaoFundiaria.FolhaObrigatoria);
			}

			if (string.IsNullOrWhiteSpace(dominio.Livro))
			{
				Validacao.Add(Mensagem.RegularizacaoFundiaria.LivroObrigatoria);
			}

			if(dominio.AreaDocumento <= 0)
			{
				Validacao.Add(Mensagem.RegularizacaoFundiaria.AreaDocumentoObrigatoria);
			}

			if (string.IsNullOrWhiteSpace(dominio.Cartorio))
			{
				Validacao.Add(Mensagem.RegularizacaoFundiaria.CartorioObrigatorio);
			}

			if (!dominio.DataUltimaAtualizacao.IsEmpty)
			{
				ValidacoesGenericasBus.DataMensagem(new DateTecno() { DataTexto = dominio.DataUltimaAtualizacao.DataTexto }, "DataUltimaAtualizacao_DataTexo", "ultima atualização");
			}

			if (dominioLista.Count(x => x.Matricula == dominio.Matricula) > 0)
			{
				Validacao.Add(Mensagem.RegularizacaoFundiaria.MatriculaJaAdicionada);
			}
		}

		public void Transmitente(List<TransmitentePosse> transmitenteLista, TransmitentePosse transmitente)
		{
			transmitenteLista = transmitenteLista ?? new List<TransmitentePosse>();

			if (transmitente.TempoOcupacao < 1)
			{
				Validacao.Add(Mensagem.RegularizacaoFundiaria.TempoDeOcupacaoObrigatorio);
			}

			if (transmitente.Transmitente.Id < 1)
			{
				Validacao.Add(Mensagem.RegularizacaoFundiaria.TrasmitenteObrigatorio);
			}

			if (transmitenteLista.Count(x => x.Transmitente.Id == transmitente.Id) > 0)
			{
				Validacao.Add(Mensagem.RegularizacaoFundiaria.TrasmitenteJaAdicionado);
			}
		}

		public void UsoAtualSolo(List<UsoAtualSolo> usoAtualSoloLista, UsoAtualSolo usoAtualSolo)
		{
			usoAtualSoloLista = usoAtualSoloLista ?? new List<UsoAtualSolo>();

			if (usoAtualSolo.TipoDeUso < 1)
			{
				Validacao.Add(Mensagem.RegularizacaoFundiaria.UsoSoloTipoObrigatorio);
			}

			if (usoAtualSolo.AreaPorcentagem < 1)
			{
				Validacao.Add(Mensagem.RegularizacaoFundiaria.UsoSoloAreaObrigatorio);
			}

			int porcentagem = usoAtualSoloLista.Sum(x => x.AreaPorcentagem) + usoAtualSolo.AreaPorcentagem;
			if (porcentagem > 100)
			{
				Validacao.Add(Mensagem.RegularizacaoFundiaria.UsoSoloLimitePorcentagem);
			}

			if (usoAtualSoloLista.Count(x => x.TipoDeUso == usoAtualSolo.TipoDeUso) > 0)
			{
				Validacao.Add(Mensagem.RegularizacaoFundiaria.UsoSoloTipoJaAdicionado);
			}
		}

		public void Edificacao(Edificacao edificacao)
		{
			if (string.IsNullOrWhiteSpace(edificacao.Tipo))
			{
				Validacao.Add(Mensagem.RegularizacaoFundiaria.TipoEdificacaoObrigatorio);
			}

			if (edificacao.Quantidade < 1)
			{
				Validacao.Add(Mensagem.RegularizacaoFundiaria.QuantidadeEdificacaoObrigatorio);
			}
		}

		public bool EmpreendimentoZonaAlterada(int empreendimentoId)
		{
			return _da.EmpreendimentoZonaAlterada(empreendimentoId);
		}

		public bool ValidarProjetoGeo(int empreendimento)
		{
			RegularizacaoFundiariaBus bus = new RegularizacaoFundiariaBus();
			RegularizacaoFundiaria caracterizacao = bus.ObterDadosGeo(empreendimento);
			caracterizacao.EmpreendimentoId = empreendimento;
			if (Acessar(caracterizacao))
			{
				return true;
			}
			return false;
		}
	}
}