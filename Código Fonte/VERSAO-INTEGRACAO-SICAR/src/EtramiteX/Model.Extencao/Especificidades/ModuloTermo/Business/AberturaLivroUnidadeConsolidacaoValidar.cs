using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloTermo;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloTermo.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloTermo.Business
{
	public class AberturaLivroUnidadeConsolidacaoValidar : EspecificidadeValidarBase, IEspecificiadeValidar
	{
		AberturaLivroUnidadeConsolidacaoDa _daEspecificidade = new AberturaLivroUnidadeConsolidacaoDa();

		public bool Salvar(IEspecificidade especificidade)
		{
			AberturaLivroUnidadeConsolidacao esp = especificidade as AberturaLivroUnidadeConsolidacao;

			RequerimentoAtividade(esp, solicitado: true, jaAssociado: false, atividadeAndamento: false);

			if (esp.Culturas.Count <= 0)
			{
				Validacao.Add(Mensagem.AberturaLivroUnidadeConsolidacao.CulturaObrigatoria);
			}
			else
			{
				foreach (var item in esp.Culturas)
				{
					List<Lista> culturasCult = _daEspecificidade.ObterCulturas(esp.ProtocoloReq.Id);
					if (!culturasCult.Exists(x => x.Id == item.Id.ToString()))
					{
						Validacao.Add(Mensagem.AberturaLivroUnidadeConsolidacao.CulturaCultivarDesatualizada);
					}
					else
					{
						if (_daEspecificidade.ExisteTituloParaCulturaMesmoRequerimento(item.Id, esp.ProtocoloReq.Id, esp.Titulo.Id))
						{
							Validacao.Add(Mensagem.AberturaLivroUnidadeConsolidacao.CulturaAdicionadaOutroTitulo);
						}
					}
				}
			}

			#region TotalPaginasLivro

			if (String.IsNullOrWhiteSpace(esp.TotalPaginasLivro))
			{
				Validacao.Add(Mensagem.AberturaLivroUnidadeConsolidacao.TotalPaginasLivroObrigatorio);
			}
			else
			{
				Int32 aux = 0;

				if (Int32.TryParse(esp.TotalPaginasLivro, out aux))
				{
					if (aux <= 0)
					{
						Validacao.Add(Mensagem.AberturaLivroUnidadeConsolidacao.TotalPaginasLivroMaiorZero);
					}
				}
				else
				{
					Validacao.Add(Mensagem.AberturaLivroUnidadeConsolidacao.TotalPaginasLivroInvalido);
				}
			}

			#endregion

			#region PaginaInicial

			if (String.IsNullOrWhiteSpace(esp.PaginaInicial))
			{
				Validacao.Add(Mensagem.AberturaLivroUnidadeConsolidacao.PaginaInicialObrigatorio);
			}
			else
			{
				Int32 aux = 0;

				if (Int32.TryParse(esp.PaginaInicial, out aux))
				{
					if (aux <= 0)
					{
						Validacao.Add(Mensagem.AberturaLivroUnidadeConsolidacao.PaginaInicialMaiorZero);
					}
				}
				else
				{
					Validacao.Add(Mensagem.AberturaLivroUnidadeConsolidacao.PaginaInicialInvalido);
				}
			}

			#endregion

			#region PaginaFinal

			if (String.IsNullOrWhiteSpace(esp.PaginaFinal))
			{
				Validacao.Add(Mensagem.AberturaLivroUnidadeConsolidacao.PaginaFinalObrigatorio);
			}
			else
			{
				Int32 aux = 0;

				if (Int32.TryParse(esp.PaginaFinal, out aux))
				{
					if (aux <= 0)
					{
						Validacao.Add(Mensagem.AberturaLivroUnidadeConsolidacao.PaginaFinalMaiorZero);
					}
				}
				else
				{
					Validacao.Add(Mensagem.AberturaLivroUnidadeConsolidacao.PaginaFinalInvalido);
				}
			}

			#endregion

			#region Caracterizacao

			CaracterizacaoBus caracterizacaoBus = new CaracterizacaoBus();
			int caracterizacao = caracterizacaoBus.Existe(esp.Titulo.EmpreendimentoId.GetValueOrDefault(), eCaracterizacao.UnidadeConsolidacao);

			if (caracterizacao <= 0)
			{
				Validacao.Add(Mensagem.AberturaLivroUnidadeConsolidacao.CaracterizacaoNaoCadastrada);
			}

			#endregion

			return Validacao.EhValido;
		}

		public bool Emitir(IEspecificidade especificidade)
		{
			return Salvar(especificidade);
		}
	}
}