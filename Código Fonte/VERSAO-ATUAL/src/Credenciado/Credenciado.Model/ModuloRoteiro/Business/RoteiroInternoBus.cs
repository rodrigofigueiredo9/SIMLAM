using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloAtividade.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloRoteiro.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloRoteiro.Business
{
	public class RoteiroInternoBus
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys;
		RoteiroInternoDa _da;

		public String UsuarioInterno
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioInterno); }
		}

		#endregion

		public RoteiroInternoBus()
		{
			_configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
			_da = new RoteiroInternoDa(UsuarioInterno);
		}

		public Roteiro Obter(int id, string tid = null)
		{
			Roteiro roteiro = null;

			try
			{
				if (tid == null || _da.VerificarTidAtual(id, tid))
				{
					roteiro = _da.Obter(id);
				}
				else
				{
					roteiro = _da.ObterHistorico(id, tid);
				}

				if (roteiro.Atividades.Count > 0)
				{
					roteiro.ModelosAtuais = ObterModelosAtividades(roteiro.Atividades);
				}

				if (roteiro.ModelosAtuais.Count > 0)
				{
					roteiro.Modelos.ForEach(x =>
					{
						TituloModeloLst modelo = roteiro.ModelosAtuais.SingleOrDefault(y => y.Id == x.Id);
						if (modelo != null)
						{
							modelo.IsAtivo = true;
						}
					});
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return roteiro;
		}

		public Roteiro ObterSimplificado(int id, string tid = null)
		{
			Roteiro roteiro = null;

			try
			{
				if (tid == null || _da.VerificarTidAtual(id, tid))
				{
					roteiro = _da.ObterSimplificado(id);
				}
				else
				{
					roteiro = _da.ObterHistoricoSimplificado(id, tid);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return roteiro;
		}

		public int ObterSituacao(int id)
		{
			return _da.ObterSituacao(id);
		}

		public Atividade ObterAtividade(Atividade atividades)
		{
			return _da.ObterAtividade(atividades);
		}

		public List<Roteiro> ObterRoteirosPorAtividades(List<Atividade> atividades)
		{
			List<Roteiro> roteiros = new List<Roteiro>();

			if (atividades == null || atividades.Count <= 0)
			{
				return roteiros;
			}

			Roteiro roteiroPadrao = ListaCredenciadoBus.RoteiroPadrao.FirstOrDefault(x => x.Setor == atividades[0].SetorId);

			if (roteiroPadrao != null)
			{
				roteiroPadrao = ObterSimplificado(roteiroPadrao.Id);
			}

			List<String> titulos = new List<String>();

			foreach (var atividade in atividades)
			{
				foreach (var finalidade in atividade.Finalidades)
				{
					finalidade.AtividadeId = atividade.Id;
					finalidade.AtividadeNome = atividade.NomeAtividade;
					finalidade.AtividadeSetorId = atividade.SetorId;

					String modeloTituloNaoAdicionadoRoteiro = _da.ModeloTituloNaoAdicionadoRoteiro(finalidade);
					if (!String.IsNullOrWhiteSpace(modeloTituloNaoAdicionadoRoteiro))
					{
						titulos.Add("\"" + modeloTituloNaoAdicionadoRoteiro + "\"");
						continue;
					}

					Roteiro roteiroAux = _da.ObterRoteirosPorAtividades(finalidade);
					if (roteiroAux == null)
					{
						roteiroPadrao.AtividadeTexto = atividade.NomeAtividade;
						roteiros.Add(roteiroPadrao);
						continue;
					}

					roteiros.Add(roteiroAux);

				}
			}

			if (titulos.Count > 0)
			{
				Validacao.Add(Mensagem.Roteiro.TituloNaoAdicionadoRoteiroCredenciado(Mensagem.Concatenar(titulos)));
			}

			#region Faz a magica de agrupar os resultados

			roteiros = roteiros.GroupBy(x => x.Id).Select(y => new Roteiro
			{
				Id = y.First().Id,
				Nome = y.First().Nome,
				VersaoAtual = y.First().VersaoAtual,
				Tid = y.First().Tid,
				AtividadeTexto = y.Select(w => w.AtividadeTexto).Distinct().Aggregate((total, atual) => total + " / " + atual)
			}).ToList();

			#endregion

			return roteiros;
		}

		public Finalidade ObterFinalidade(Finalidade finalidade)
		{
			return _da.ObterFinalidade(finalidade);
		}

		public int ObterFinalidadeCodigo(int finalidadeId)
		{
			return _da.ObterFinalidadeCodigo(finalidadeId);
		}

		public List<TituloModeloLst> ObterModelosAtividades(List<AtividadeSolicitada> atividades)
		{
			AtividadeConfiguracaoInternoBus atividadeBus = new AtividadeConfiguracaoInternoBus();
			return atividadeBus.ObterModelosAtividades(atividades);
		}

	}
}