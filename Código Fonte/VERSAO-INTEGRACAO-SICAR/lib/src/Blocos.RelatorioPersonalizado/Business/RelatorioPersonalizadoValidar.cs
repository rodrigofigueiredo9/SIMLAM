using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.Blocos.RelatorioPersonalizado.Entities;

namespace Tecnomapas.Blocos.RelatorioPersonalizado.Business
{
	public class RelatorioPersonalizadoValidar
	{
		internal bool Salvar(Relatorio relatorio)
		{
			VerificarConfiguracao(relatorio.ConfiguracaoRelatorio);

			return Validacao.EhValido;
		}

		private void VerificarConfiguracao(ConfiguracaoRelatorio configuracao)
		{
			if (configuracao.CamposSelecionados.Count == 0)
			{
				Validacao.Add(Mensagem.RelatorioPersonalizado.CampoSelecionarObrigatorio);
			}

			if (string.IsNullOrWhiteSpace(configuracao.Nome))
			{
				Validacao.Add(Mensagem.RelatorioPersonalizado.NomeObrigatorio);
			}

			if (string.IsNullOrWhiteSpace(configuracao.Descricao))
			{
				Validacao.Add(Mensagem.RelatorioPersonalizado.DescricaoObrigatoria);
			}

			AnalisarTermos(configuracao.Termos);
		}

		public void AnalisarTermos(List<Termo> termos)
		{
			AnalisadorSintatico sintatico = new AnalisadorSintatico(termos);
			sintatico.Analisar();
			if (sintatico.Erros.Count > 0)
			{
				foreach (var item in sintatico.Erros)
				{
					Validacao.AddAdvertencia(item.Mensagem);
				}
			}
		}

		public Relatorio Deserializar(Relatorio relatorio)
		{
			Relatorio rel = null;
			try
			{
				JavaScriptSerializer serializer = new JavaScriptSerializer();
				rel = serializer.Deserialize<Relatorio>(relatorio.ConfiguracaoSerializada);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return rel;
		}

		public bool Importar(Relatorio relatorio, int executor)
		{
			Fato fato = new RelatorioPersonalizadoBus().ObterFato(relatorio.ConfiguracaoRelatorio.FonteDadosId);

			if (fato == null || fato.Id == 0)
			{
				Validacao.Add(Mensagem.RelatorioPersonalizado.FonteDadosNaoEncontrada);
				return false;
			}

			if (relatorio.ConfiguracaoRelatorio.CamposSelecionados.Exists(x => x.CampoCodigo <= 0)
				|| relatorio.ConfiguracaoRelatorio.Ordenacoes.Exists(x => x.CampoCodigo <= 0)
				|| relatorio.ConfiguracaoRelatorio.Termos.Exists(x => (x.Tipo == (int)eTipoTermo.Filtro) && x.CampoCodigo <= 0)
				|| relatorio.ConfiguracaoRelatorio.Sumarios.Exists(x => x.CampoCodigo <= 0)
				|| relatorio.ConfiguracaoRelatorio.Agrupamentos.Exists(x => x.CampoCodigo <= 0))
			{
				Validacao.Add(Mensagem.RelatorioPersonalizado.CamposFiscalizacaoNaoImportado);
			}

			return Validacao.EhValido;
		}

		internal bool Executar(Relatorio relatorio, int executor)
		{
			if (!relatorio.UsuariosPermitidos.Exists(x => x.Id == executor))
			{
				Validacao.Add(Mensagem.RelatorioPersonalizado.SemPermissaoExecutar);
				return false;
			}

			if (relatorio.ConfiguracaoRelatorio.CamposSelecionados.Where(c => c.Campo != null).Count() <= 0)
			{
				Validacao.Add(Mensagem.RelatorioPersonalizado.ConfiguracaoInvalida);
				return false;
			}

			Fato fato = new RelatorioPersonalizadoBus().ObterFato(relatorio.ConfiguracaoRelatorio.FonteDadosId);

			if (fato == null || fato.Id == 0)
			{
				Validacao.Add(Mensagem.RelatorioPersonalizado.FonteDadosNaoEncontrada);
				return false;
			}
			else if (fato.Tid != relatorio.ConfiguracaoRelatorio.FonteDadosTid)
			{
				Validacao.Add(Mensagem.RelatorioPersonalizado.FonteDadosDesatualizada);
				return false;
			}

			if (relatorio.Setor <= 0)
			{
				Validacao.Add(Mensagem.RelatorioPersonalizado.SetorObrigatorio);
				return false;
			}

			int i = 0;
			DateTime data;

			relatorio.ConfiguracaoRelatorio.Termos.Where(x => x.DefinirExecucao).ToList().ForEach(x =>
			{
				i++;
				if (x.Campo.PossuiListaDeValores)
				{
					if (x.Valor == "0")
					{
						Validacao.Add(Mensagem.RelatorioPersonalizado.CampoObrigatorio(x.Campo.Alias, i));
					}
				}
				else if (string.IsNullOrEmpty(x.Valor))
				{
					Validacao.Add(Mensagem.RelatorioPersonalizado.CampoObrigatorio(x.Campo.Alias, i));
				}
				else if (x.Campo.TipoDadosEnum == eTipoDados.Data && !DateTime.TryParse(x.Valor, out data))
				{
					Validacao.Add(Mensagem.RelatorioPersonalizado.FiltroInvalido(x.Campo.Alias, i));
				}
			});

			relatorio.ConfiguracaoRelatorio.Termos.Where(x => !string.IsNullOrEmpty(x.Valor) && !x.DefinirExecucao).ToList().ForEach(r =>
			{
				if (r.Campo.TipoDadosEnum == eTipoDados.Data && !DateTime.TryParse(r.Valor, out data))
				{
					Validacao.Add(Mensagem.RelatorioPersonalizado.FiltroInvalido(r.Campo.Alias));
				}
			});

			if (relatorio.ConfiguracaoRelatorio.CamposSelecionados == null || relatorio.ConfiguracaoRelatorio.CamposSelecionados.Exists(x => x.Campo == null))
			{
				Validacao.Add(Mensagem.RelatorioPersonalizado.FonteDadosDesatualizada);
			}

			return Validacao.EhValido;
		}
	}
}