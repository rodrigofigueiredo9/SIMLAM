using System;
using System.Linq;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloTermo;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloTermo.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloTermo.Business
{
	public class AberturaLivroUnidadeProducaoValidar : EspecificidadeValidarBase, IEspecificiadeValidar
	{
		AberturaLivroUnidadeProducaoDa _daEspecificidade = new AberturaLivroUnidadeProducaoDa();

		public bool Salvar(IEspecificidade especificidade)
		{
			AberturaLivroUnidadeProducao esp = especificidade as AberturaLivroUnidadeProducao;

			RequerimentoAtividade(esp, solicitado: true, jaAssociado: false, atividadeAndamento: false);

			#region TotalPaginasLivro

			if (String.IsNullOrWhiteSpace(esp.TotalPaginasLivro))
			{
				Validacao.Add(Mensagem.AberturaLivroUnidadeProducao.TotalPaginasLivroObrigatorio);
			}
			else
			{
				Int32 aux = 0;

				if (Int32.TryParse(esp.TotalPaginasLivro, out aux))
				{
					if (aux <= 0)
					{
						Validacao.Add(Mensagem.AberturaLivroUnidadeProducao.TotalPaginasLivroMaiorZero);
					}
				}
				else
				{
					Validacao.Add(Mensagem.AberturaLivroUnidadeProducao.TotalPaginasLivroInvalido);
				}
			}

			#endregion

			#region PaginaInicial

			if (String.IsNullOrWhiteSpace(esp.PaginaInicial))
			{
				Validacao.Add(Mensagem.AberturaLivroUnidadeProducao.PaginaInicialObrigatorio);
			}
			else
			{
				Int32 aux = 0;

				if (Int32.TryParse(esp.PaginaInicial, out aux))
				{
					if (aux <= 0)
					{
						Validacao.Add(Mensagem.AberturaLivroUnidadeProducao.PaginaInicialMaiorZero);
					}
				}
				else
				{
					Validacao.Add(Mensagem.AberturaLivroUnidadeProducao.PaginaInicialInvalido);
				}
			}

			#endregion

			#region PaginaFinal

			if (String.IsNullOrWhiteSpace(esp.PaginaFinal))
			{
				Validacao.Add(Mensagem.AberturaLivroUnidadeProducao.PaginaFinalObrigatorio);
			}
			else
			{
				Int32 aux = 0;

				if (Int32.TryParse(esp.PaginaFinal, out aux))
				{
					if (aux <= 0)
					{
						Validacao.Add(Mensagem.AberturaLivroUnidadeProducao.PaginaFinalMaiorZero);
					}
				}
				else
				{
					Validacao.Add(Mensagem.AberturaLivroUnidadeProducao.PaginaFinalInvalido);
				}
			}

			#endregion

			#region UnidadeProducaoUnidade

			if (esp.Unidades.Count <= 0)
			{
				Validacao.Add(Mensagem.AberturaLivroUnidadeProducao.UnidadeProducaoObrigatorio);
			}
			else
			{
				esp.Unidades.ForEach(unidade =>
				{
					if (unidade.Id <= 0)
					{
						Validacao.Add(Mensagem.AberturaLivroUnidadeProducao.UnidadeProducaoObrigatorio);
					}
					else
					{
						if (!_daEspecificidade.ExisteUnidadeProducao(unidade.Id))
						{
							Validacao.Add(Mensagem.AberturaLivroUnidadeProducao.UnidadeProducaoUnidadeNaoAssociadaCaracterizacaoEmpreendimento(unidade.CodigoUP.ToString()));
						}
						else
						{
							String titulo = _daEspecificidade.ObterTituloAssociado(unidade.Id, esp.Titulo.Id);
							if (!String.IsNullOrWhiteSpace(titulo))
							{
								Validacao.Add(Mensagem.AberturaLivroUnidadeProducao.UnidadeProducaoUnidadePossuiTituloAssociado(unidade.CodigoUP.ToString(), titulo));
							}
						}
					}
				});
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