using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Entities;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloProjetoGeografico.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloProjetoGeografico.Bussiness
{
	public class ProjetoGeograficoInternoBus
	{
		#region Propriedades

		ProjetoGeograficoInternoDa _da;
		GerenciadorConfiguracao<ConfiguracaoProjetoGeo> _configPGeo;

		public List<SobreposicaoTipo> SobreposicaoTipo
		{
			get
			{
				return _configPGeo.Obter<List<SobreposicaoTipo>>(ConfiguracaoProjetoGeo.KeyTipoSobreposicao);
			}
		}

		#endregion

		public ProjetoGeograficoInternoBus()
		{
			_da = new ProjetoGeograficoInternoDa();
			_configPGeo = new GerenciadorConfiguracao<ConfiguracaoProjetoGeo>(new ConfiguracaoProjetoGeo());
		}

		#region Obter

		public ProjetoGeografico ObterProjeto(int id, bool simplificado = false)
		{
			ProjetoGeografico projeto = null;
			try
			{
				projeto = _da.Obter(id, null, simplificado, true);

				if (projeto.Id > 0 && !simplificado)
				{
					#region Sobreposicoes

					if (projeto.Sobreposicoes.Itens != null && projeto.Sobreposicoes.Itens.Count > 0)
					{
						foreach (var item in SobreposicaoTipo)
						{
							if (projeto.Sobreposicoes.Itens.Exists(x => x.Tipo == item.Id))
							{
								projeto.Sobreposicoes.Itens.First(x => x.Tipo == item.Id).TipoTexto = item.Texto;
							}
							else
							{
								projeto.Sobreposicoes.Itens.Add(new Sobreposicao()
								{
									Tipo = item.Id,
									TipoTexto = item.Texto,
									Base = (int)((item.Id == (int)eSobreposicaoTipo.OutrosEmpreendimento) ? eSobreposicaoBase.IDAF : eSobreposicaoBase.GeoBase),
									Identificacao = " - "
								});
							}
						}
					}

					#endregion
				}

				return projeto;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return projeto;
		}

		public int ExisteProjetoGeografico(int empreedimentoId, int caracterizacaoTipo)
		{
			try
			{
				return _da.ExisteProjetoGeografico(empreedimentoId, caracterizacaoTipo);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return 0;
		}

		#endregion
	}
}