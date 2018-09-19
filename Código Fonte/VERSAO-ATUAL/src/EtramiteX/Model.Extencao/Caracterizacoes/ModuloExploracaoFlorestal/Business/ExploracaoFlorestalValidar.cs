using System;
using System.Linq;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloExploracaoFlorestal;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloDominialidade.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloExploracaoFlorestal.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloExploracaoFlorestal.Business
{
	public class ExploracaoFlorestalValidar
	{
		#region Propriedades

		ExploracaoFlorestalDa _da = new ExploracaoFlorestalDa();
		CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();

		#endregion

		internal bool Salvar(ExploracaoFlorestal caracterizacao)
		{
			if (!_caracterizacaoValidar.Basicas(caracterizacao.EmpreendimentoId)) return false;

			if (caracterizacao.Id <= 0 && (_da.ObterPorEmpreendimento(caracterizacao.EmpreendimentoId, true) ?? new ExploracaoFlorestal()).Id > 0)
			{
				Validacao.Add(Mensagem.Caracterizacao.EmpreendimentoCaracterizacaoJaCriada);
				return false;
			}

			if (!Acessar(caracterizacao.EmpreendimentoId)) return false;
						
			if (caracterizacao.TipoAtividade <= 0)
				Validacao.Add(Mensagem.ExploracaoFlorestal.ExploracaoTipoObrigatorio);

			foreach (ExploracaoFlorestalExploracao item in caracterizacao.Exploracoes)
			{
				if (item.GeometriaTipoId == (int)eExploracaoFlorestalGeometria.Poligono)
				{
					if (!String.IsNullOrWhiteSpace(item.AreaRequeridaTexto))
					{
						if (!ValidacoesGenericasBus.ValidarDecimal(DecimalEtx.ClearMask(item.AreaRequeridaTexto), 7, 2))
							Validacao.Add(Mensagem.ExploracaoFlorestal.AreaRequiridaInvalida(item.Identificacao));
						else if (item.AreaRequerida <= 0)
							Validacao.Add(Mensagem.ExploracaoFlorestal.AreaRequiridaMaiorZero(item.Identificacao));
					}
					else
						Validacao.Add(Mensagem.ExploracaoFlorestal.AreaRequiridaObrigatoria(item.Identificacao));

				}
				else
				{
					if (item.GeometriaTipoId == (int)eExploracaoFlorestalGeometria.Ponto || item.GeometriaTipoId == (int)eExploracaoFlorestalGeometria.Linha)
					{
						#region Arvores Requeridas

						if (!String.IsNullOrWhiteSpace(item.ArvoresRequeridas))
						{
							if (Convert.ToDecimal(item.ArvoresRequeridas) <= 0)
								Validacao.Add(Mensagem.ExploracaoFlorestal.ArvoresRequeridasMaiorZero(item.Identificacao));
						}
						else
							Validacao.Add(Mensagem.ExploracaoFlorestal.ArvoresRequeridasObrigatoria(item.Identificacao));

						#endregion

						#region Numero de Arvores

						if (!String.IsNullOrWhiteSpace(item.QuantidadeArvores))
						{
							bool existeProdutoSemRendimento = item.Produtos.Where(x => x.ProdutoId == (int)eProduto.SemRendimento).ToList().Count() > 0;

							if (!existeProdutoSemRendimento)
							{
								if (Convert.ToInt32(item.QuantidadeArvores) <= 0)
									Validacao.Add(Mensagem.ExploracaoFlorestal.QdeArvoresRequeridasMaiorZero(item.Identificacao));
							}
						}
						else
							Validacao.Add(Mensagem.ExploracaoFlorestal.QdeArvoresRequeridasObrigatoria(item.Identificacao));

						#endregion
					}
				}

				if (item.FinalidadeExploracao <= 0)
					Validacao.Add(Mensagem.ExploracaoFlorestal.FinalidadeExploracaoObrigatorio);
				else
				{
					if (item.FinalidadeExploracao == (int)eExploracaoFlorestalFinalidade.Outros && String.IsNullOrWhiteSpace(item.FinalidadeEspecificar))
						Validacao.Add(Mensagem.ExploracaoFlorestal.FinalidadeExploracaoEspecificarObrigatorio(item.Identificacao));
				}

				if (item.ClassificacaoVegetacaoId <= 0)
					Validacao.Add(Mensagem.ExploracaoFlorestal.ClassificacaoVegetacaoObrigatoria(item.Identificacao));

				if (Convert.ToBoolean(item.ParecerFavoravel))
				{
					if (item.Produtos.Count == 0)
						Validacao.Add(Mensagem.ExploracaoFlorestal.ProdutoObrigatorio(item.Identificacao));
					else
					{
						foreach (ExploracaoFlorestalProduto produto in item.Produtos)
						{
							if (produto.ProdutoId == (int)eProduto.SemRendimento) continue;

							if (!String.IsNullOrWhiteSpace(produto.Quantidade))
							{
								if (!ValidacoesGenericasBus.ValidarDecimal(DecimalEtx.ClearMask(produto.Quantidade), 7, 2))
									Validacao.Add(Mensagem.Dominialidade.AreaInvalida("exploracao" + item.Identificacao, "Quantidade"));
								else if (DecimalEtx.ToDecimalMask(produto.Quantidade).GetValueOrDefault() <= 0)
									Validacao.Add(Mensagem.Dominialidade.AreaMaiorZero("exploracao" + item.Identificacao, "Quantidade"));
							}
							else
								Validacao.Add(Mensagem.Dominialidade.AreaObrigatoria("exploracao" + item.Identificacao, "Quantidade"));
						}
					}
				}
			}

			return Validacao.EhValido;
		}

		public bool Acessar(int empreendimentoId)
		{
			if (!_caracterizacaoValidar.Dependencias(empreendimentoId, (int)eCaracterizacao.ExploracaoFlorestal))
			{
				return false;
			}

			if (_da.NaoPossuiAVNOuAA(empreendimentoId))
			{
				Validacao.Add(Mensagem.ExploracaoFlorestal.NaoPossuiAVNOuAA);
				return false;
			}

			if (_da.PossuiAVNNaoCaracterizada(empreendimentoId))
			{
				Validacao.Add(Mensagem.ExploracaoFlorestal.PossuiAVNNaoCaracterizada);
				return false;
			}

			return Validacao.EhValido;
		}

		public string AbrirModalAcessar(ExploracaoFlorestal caracterizacao)
		{
			EmpreendimentoCaracterizacao empreendimento = _caracterizacaoBus.ObterEmpreendimentoSimplificado(caracterizacao.EmpreendimentoId);

			if (empreendimento.ZonaLocalizacao == eZonaLocalizacao.Rural)
			{
				DominialidadeDa dominialidadeDa = new DominialidadeDa();
				Dominialidade dominialidade = dominialidadeDa.ObterPorEmpreendimento(caracterizacao.EmpreendimentoId);

				foreach (Dominio dominio in dominialidade.Dominios)
				{
					if (dominio.ReservasLegais.Exists(x => x.SituacaoId == (int)eReservaLegalSituacao.NaoInformada))
					{
						return Mensagem.ExploracaoFlorestal.EmpreendimentoRuralReservaIndefinida.Texto;
					}
				}
			}

			return string.Empty;
		}
	}
}