using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.RelatorioIndividual.ModuloProjetoDigital.Data
{
	public class ProjetoDigitalDa
	{
		#region Propriedades

		private string EsquemaBanco { get; set; }

		#endregion

		public ProjetoDigitalDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		public ProjetoDigital Obter(int idProjeto = 0, int idRequerimento = 0, bool simplificado = false, BancoDeDados banco = null)
		{
			ProjetoDigital projeto = new ProjetoDigital();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaBanco))
			{
				#region Projeto Digital

				Comando comando = bancoDeDados.CriarComando(@"select p.id, p.tid, p.etapa, p.situacao, l.texto situacao_texto, p.requerimento, p.empreendimento, 
				p.data_criacao, p.data_envio, p.credenciado, p.motivo_recusa from {0}tab_projeto_digital p, {0}lov_projeto_digital_situacao l where p.situacao = l.id", EsquemaBanco);

				if (idRequerimento > 0)
				{
					comando.DbCommand.CommandText += comando.FiltroAnd("p.requerimento", "requerimento", idRequerimento);
				}
				else
				{
					comando.DbCommand.CommandText += comando.FiltroAnd("p.id", "id", idProjeto);
				}

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{						
						projeto.Id = reader.GetValue<int>("id");
						projeto.Tid = reader.GetValue<string>("tid");
						projeto.Situacao = reader.GetValue<int>("situacao");
						projeto.SituacaoTexto = reader.GetValue<string>("situacao_texto");
						projeto.Etapa = reader.GetValue<int>("etapa");
						projeto.RequerimentoId = reader.GetValue<int>("requerimento");
						projeto.EmpreendimentoId = reader.GetValue<int>("empreendimento");
						projeto.DataCriacao.Data = reader.GetValue<DateTime>("data_criacao");
						projeto.DataEnvio.Data = reader.GetValue<DateTime>("data_envio");
						projeto.CredenciadoId = reader.GetValue<int>("credenciado");
						projeto.MotivoRecusa = reader.GetValue<String>("motivo_recusa");
					}

					reader.Close();
				}

				#endregion

				#region Dependencias

				if (projeto.Id > 0 && !simplificado)
				{
					projeto.Dependencias = ObterDependencias(projeto.Id);
				}

				#endregion
			}

			return projeto;
		}

		public List<Dependencia> ObterDependencias(int projetoDigitalID, BancoDeDados banco = null)
		{
			List<Dependencia> lista = new List<Dependencia>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select d.dependencia_tipo, d.dependencia_caracterizacao, lc.texto dependencia_carac_texto, d.dependencia_id, d.dependencia_tid 
				from {0}tab_proj_digital_dependencias d, {0}lov_caracterizacao_tipo lc where d.dependencia_caracterizacao = lc.id and d.projeto_digital_id = :projeto_digital_id", EsquemaBanco);

				comando.AdicionarParametroEntrada("projeto_digital_id", projetoDigitalID, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Dependencia dependencia = null;

					while (reader.Read())
					{
						dependencia = new Dependencia();

						dependencia.DependenciaTipo = reader.GetValue<int>("dependencia_tipo");
						dependencia.DependenciaCaracterizacao = reader.GetValue<int>("dependencia_caracterizacao");
						dependencia.DependenciaCaracterizacaoTexto = reader.GetValue<string>("dependencia_carac_texto");
						dependencia.DependenciaId = reader.GetValue<int>("dependencia_id");
						dependencia.DependenciaTid = reader.GetValue<string>("dependencia_tid");

						lista.Add(dependencia);
					}

					reader.Close();
				}
			}

			return lista;
		}

	}
}
