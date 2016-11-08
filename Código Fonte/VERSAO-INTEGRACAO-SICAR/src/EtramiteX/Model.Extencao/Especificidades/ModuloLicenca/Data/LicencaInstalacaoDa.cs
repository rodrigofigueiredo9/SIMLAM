using System;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloLicenca;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloLicenca.Data
{
	public class LicencaInstalacaoDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		EspecificidadeDa _daEsp = new EspecificidadeDa();

		internal Historico Historico { get { return _historico; } }

		internal EspecificidadeDa DaEsp { get { return _daEsp; } }

		private string EsquemaBanco { get; set; }

		#endregion

		public LicencaInstalacaoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(LicencaInstalacao Licenca, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Cadastro do Titulo

				eHistoricoAcao acao;
				object id;

				//Verifica a existencia da especificidade
				Comando comando = bancoDeDados.CriarComando(@"select e.id from {0}esp_licenca_Instalacao e where e.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", Licenca.Titulo.Id, DbType.Int32);
				id = bancoDeDados.ExecutarScalar(comando);

				if (id != null && !Convert.IsDBNull(id))
				{
					comando = bancoDeDados.CriarComando(@"update {0}esp_licenca_Instalacao e set e.titulo = :titulo, e.protocolo = :protocolo, e.destinatario = :destinatario, e.barragem_id = :barragem_id, e.caracterizacao_id = (select distinct t.barragem from {0}crt_barragem_barragens t where t.id = :barragem_id), e.tid = :tid where e.titulo = 
                        :titulo", EsquemaBanco);

					acao = eHistoricoAcao.atualizar;
					Licenca.Id = Convert.ToInt32(id);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}esp_licenca_Instalacao(id, titulo, protocolo, destinatario, tid, barragem_id, caracterizacao_id) values ({0}seq_esp_licenca_instalacao.nextval, :titulo, 
                        :protocolo, :destinatario, :tid, :barragem_id, (select distinct t.barragem from {0}crt_barragem_barragens t where t.id = :barragem_id)) returning id into :id", EsquemaBanco);

					acao = eHistoricoAcao.criar;
					comando.AdicionarParametroSaida("id", DbType.Int32);
				}

				comando.AdicionarParametroEntrada("titulo", Licenca.Titulo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("protocolo", Licenca.ProtocoloReq.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("destinatario", Licenca.Destinatario, DbType.Int32);
				comando.AdicionarParametroEntrada("barragem_id", Licenca.BarragemId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				if (id == null || Convert.IsDBNull(id))
				{
					Licenca = Licenca ?? new LicencaInstalacao();
					Licenca.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
				}

				#endregion

				#region Histórico

				Historico.Gerar(Convert.ToInt32(Licenca.Titulo.Id), eHistoricoArtefatoEspecificidade.licencainstalacao, acao, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int titulo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Atualizar o tid para a nova ação

				Comando comando = bancoDeDados.CriarComando(@"update {0}esp_licenca_Instalacao c set c.tid = :tid where c.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Histórico

				Historico.Gerar(titulo, eHistoricoArtefatoEspecificidade.licencainstalacao, eHistoricoAcao.excluir, bancoDeDados);

				#endregion

				#region Apaga os dados da especificidade

				comando = bancoDeDados.CriarComando(@"delete from {0}esp_licenca_Instalacao e where e.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion

		#region Obter

		internal LicencaInstalacao Obter(int titulo, BancoDeDados banco = null)
		{
			LicencaInstalacao especificidade = new LicencaInstalacao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados da Especificidade

				Comando comando = bancoDeDados.CriarComando(@"select e.id, e.tid, e.protocolo, n.numero, n.ano, p.requerimento, p.protocolo protocolo_tipo, e.barragem_id, e.destinatario, 
				(select distinct nvl(pe.nome, pe.razao_social) from {0}hst_esp_licenca_instalacao he, {0}hst_pessoa pe where he.destinatario_id = pe.pessoa_id and he.destinatario_tid = pe.tid
				and pe.data_execucao = (select max(h.data_execucao) from {0}hst_pessoa h where h.pessoa_id = pe.pessoa_id and h.tid = pe.tid) and he.especificidade_id = e.id
				and not exists(select 1 from {0}lov_historico_artefatos_acoes l where l.id = he.acao_executada and l.acao = 3) 
				and he.titulo_tid = (select ht.tid from {0}hst_titulo ht where ht.titulo_id = e.titulo and ht.data_execucao = (select min(htt.data_execucao) from {0}hst_titulo htt where htt.titulo_id = e.titulo 
				and htt.data_execucao > (select max(httt.data_execucao) from {0}hst_titulo httt where httt.titulo_id = e.titulo and httt.situacao_id = 1)))) destinatario_nome_razao, 
				(select t.caracterizacao_tid from {0}hst_esp_licenca_instalacao t where t.id = (select max(id) from {0}hst_esp_licenca_instalacao where especificidade_id = e.id)) caracterizacao_tid 
				from {0}esp_licenca_Instalacao e, {0}tab_titulo_numero n, {0}tab_protocolo p where n.titulo(+) = e.titulo and e.protocolo = p.id(+) and e.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						especificidade.Titulo.Id = titulo;
						especificidade.Id = Convert.ToInt32(reader["id"]);
						especificidade.Tid = reader["tid"].ToString();

						if (reader["protocolo"] != null && !Convert.IsDBNull(reader["protocolo"]))
						{
							especificidade.ProtocoloReq.IsProcesso = (reader["protocolo_tipo"] != null && Convert.ToInt32(reader["protocolo_tipo"]) == 1);
							especificidade.ProtocoloReq.RequerimentoId = Convert.ToInt32(reader["requerimento"]);
							especificidade.ProtocoloReq.Id = Convert.ToInt32(reader["protocolo"]);
						}

						if (reader["destinatario"] != null && !Convert.IsDBNull(reader["destinatario"]))
						{
							especificidade.Destinatario = Convert.ToInt32(reader["destinatario"]);
							especificidade.DestinatarioNomeRazao = Convert.ToString(reader["destinatario_nome_razao"]);
						}

						if (reader["numero"] != null && !Convert.IsDBNull(reader["numero"]))
						{
							especificidade.Titulo.Numero.Inteiro = Convert.ToInt32(reader["numero"]);
						}

						if (reader["ano"] != null && !Convert.IsDBNull(reader["ano"]))
						{
							especificidade.Titulo.Numero.Ano = Convert.ToInt32(reader["ano"]);
						}
						especificidade.BarragemId = reader.GetValue<int>("barragem_id");
						especificidade.DadosEspAtivCaractObj = especificidade.BarragemId;
						especificidade.CaracterizacaoTid = reader.GetValue<string>("caracterizacao_tid");
					}

					reader.Close();
				}

				#endregion
			}

			return especificidade;
		}

		internal Licenca ObterDadosPDF(int titulo, BancoDeDados banco = null)
		{
			Licenca Licenca = new Licenca();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados do Titulo

				DadosPDF dados = DaEsp.ObterDadosTitulo(titulo, bancoDeDados);

				Licenca.Titulo = dados.Titulo;
				Licenca.Titulo.SetorEndereco = DaEsp.ObterEndSetor(Licenca.Titulo.SetorId);
				Licenca.Protocolo = dados.Protocolo;
				Licenca.Empreendimento = dados.Empreendimento;

				#endregion

				#region Dados da Especificidade

				Comando comando = bancoDeDados.CriarComando(@"select e.destinatario from {0}esp_licenca_Instalacao e where titulo = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						Licenca.Destinatario.Id = Convert.ToInt32(reader["destinatario"]);
					}

					reader.Close();
				}

				#endregion

				Licenca.Destinatario = DaEsp.ObterDadosPessoa(Licenca.Destinatario.Id, Licenca.Empreendimento.Id, bancoDeDados);
			}

			return Licenca;
		}

		#endregion
	}
}