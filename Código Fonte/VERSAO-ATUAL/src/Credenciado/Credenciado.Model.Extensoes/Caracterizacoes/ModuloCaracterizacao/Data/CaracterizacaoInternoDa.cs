using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using System.Web;
using Tecnomapas.Blocos.Data;
using System.Data;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Data
{
	public class CaracterizacaoInternoDa
	{
		#region Propriedades

		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());

		private string EsquemaBanco { get; set; }

		public String EsquemaBancoGeo
		{
			get { return _config.Obter<String>(ConfiguracaoSistema.KeyUsuarioGeo); }
		}

		#endregion

		public CaracterizacaoInternoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Obter / Filtrar

		public List<Caracterizacao> ObterCaracterizacoesAtuais(int empreendimento, List<CaracterizacaoLst> caracterizacoes, BancoDeDados banco = null)
		{
			List<Caracterizacao> lista = new List<Caracterizacao>();
			String select = String.Empty;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Lista das caracterizações

				Comando comando = bancoDeDados.CriarComando(@"select 'select ' || t.id || ' tipo, c.id caracterizacao_id, c.tid caracterizacao_tid from {0}' ||
				t.tabela || ' c where c.empreendimento = :empreendimento union all ' resultado from {0}lov_caracterizacao_tipo t ", EsquemaBanco);

				comando.DbCommand.CommandText += comando.AdicionarIn("where", "t.id", DbType.Int32, caracterizacoes.Select(x => (int)x.Id).ToList());

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						select += reader.GetValue<string>("resultado");
					}

					reader.Close();
				}

				if (!string.IsNullOrEmpty(select))
				{
					comando = bancoDeDados.CriarComando(@"
					select lc.id tipo, lc.texto tipo_texto, c.caracterizacao_id, c.caracterizacao_tid, pg.id projeto_id, pg.tid projeto_tid
					from {0}lov_caracterizacao_tipo lc, (" + select.Substring(0, select.Length - 10) + @") c,
						(select p.id, p.tid, p.empreendimento, p.caracterizacao from {0}crt_projeto_geo p where p.empreendimento = :empreendimento) pg
					where lc.id = pg.caracterizacao(+) and lc.id = c.tipo order by lc.id", EsquemaBanco);

					comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						Caracterizacao caracterizacao = null;

						while (reader.Read())
						{
							caracterizacao = new Caracterizacao();

							caracterizacao.Tipo = (eCaracterizacao)reader.GetValue<int>("tipo");
							caracterizacao.Nome = reader.GetValue<string>("tipo_texto");
							caracterizacao.Id = reader.GetValue<int>("caracterizacao_id");
							caracterizacao.Tid = reader.GetValue<string>("caracterizacao_tid");
							caracterizacao.ProjetoId = reader.GetValue<int>("projeto_id");
							caracterizacao.ProjetoTid = reader.GetValue<string>("projeto_tid");

							lista.Add(caracterizacao);
						}

						reader.Close();
					}
				}

				#endregion
			}

			return lista;
		}

		#endregion


		internal EmpreendimentoCaracterizacao ObterEmpreendimentoSimplificado(int empreendimentoInternoId, BancoDeDados banco = null)
		{
			EmpreendimentoCaracterizacao empreendimento = new EmpreendimentoCaracterizacao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Empreendimento

				Comando comando = bancoDeDados.CriarComando(@"select 
				e.id, e.codigo, ls.texto segmento_texto, ls.denominador, e.cnpj, e.denominador denominador_nome, e.tid, ee.zona, ee.municipio municipio_id,
				(select m.texto from {0}lov_municipio m where m.id = ee.municipio) municipio, (select m.ibge from {0}lov_municipio m where m.id = ee.municipio) municipio_ibge, 
				cm.id modulo_id, cm.modulo_ha, (select es.sigla from {0}lov_estado es where es.id = ee.estado) estado,
				case when ee.zona = 1 then 'Urbana' else 'Rural' end zona_texto
				from {0}tab_empreendimento e, {0}tab_empreendimento_atividade a, {0}lov_empreendimento_segmento ls, {0}tab_empreendimento_endereco ee, {0}cnf_municipio_mod_fiscal cm
				where e.atividade = a.id(+) and e.segmento = ls.id and ee.correspondencia = 0 and ee.empreendimento = e.id and ee.municipio = cm.municipio(+) and e.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", empreendimentoInternoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						empreendimento.Id = empreendimentoInternoId;
						empreendimento.Tid = reader.GetValue<string>("tid");
						empreendimento.DenominadorTipo = reader.GetValue<string>("denominador");
						empreendimento.Denominador = reader.GetValue<string>("denominador_nome");
						empreendimento.MunicipioId = reader.GetValue<int>("municipio_id");
						empreendimento.MunicipioIBGE = reader.GetValue<int>("municipio_ibge");
						empreendimento.Municipio = reader.GetValue<string>("municipio");
						empreendimento.ModuloFiscalId = reader.GetValue<int>("modulo_id");
						empreendimento.ModuloFiscalHA = reader.GetValue<decimal>("modulo_ha");
						empreendimento.Uf = reader.GetValue<string>("estado");
						empreendimento.ZonaLocalizacao = (eZonaLocalizacao)reader.GetValue<int>("zona");
						empreendimento.ZonaLocalizacaoTexto = reader.GetValue<string>("zona_texto");
						empreendimento.CNPJ = reader.GetValue<string>("cnpj");
						empreendimento.Codigo = reader.GetValue<int>("codigo");
					}

					reader.Close();
				}

				#endregion
			}

			return empreendimento;
		}
	}
}