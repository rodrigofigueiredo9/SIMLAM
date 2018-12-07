using System;
using System.Collections.Generic;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCadastro;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Data;
using Historico = Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Data.Historico;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloCadastro.Data
{
	class CadastroAmbientalRuralTituloDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		EspecificidadeDa _daEsp = new EspecificidadeDa();
		Consulta _consulta = new Consulta();

		internal Historico Historico { get { return _historico; } }
		internal EspecificidadeDa DaEsp { get { return _daEsp; } }
		internal Consulta Consulta { get { return _consulta; } }

		private string EsquemaBanco { get; set; }

		#endregion

		public CadastroAmbientalRuralTituloDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(CadastroAmbientalRuralTitulo cadastro, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Título de Cadastro Ambiental Rural

				eHistoricoAcao acao;
				object id;

				bancoDeDados.IniciarTransacao();
				//Verifica a existencia da especificidade
				Comando comando = bancoDeDados.CriarComando(@"select e.id from {0}esp_cad_ambiental_rural e where e.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", cadastro.Titulo.Id, DbType.Int32);
				id = bancoDeDados.ExecutarScalar(comando);

				if (id != null && !Convert.IsDBNull(id))
				{
					comando = bancoDeDados.CriarComando(@"update {0}esp_cad_ambiental_rural e set e.titulo = :titulo, e.protocolo = :protocolo, 
														e.destinatario = :destinatario, e.matricula = :matricula, e.tid = :tid where e.titulo = :titulo", EsquemaBanco);

					acao = eHistoricoAcao.atualizar;
					cadastro.Id = Convert.ToInt32(id);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}esp_cad_ambiental_rural e (id, titulo, protocolo, destinatario, matricula, tid) 
														values ({0}seq_esp_cad_ambiental_rural.nextval, :titulo, :protocolo, :destinatario, :matricula, :tid) returning e.id into :id", EsquemaBanco);
					acao = eHistoricoAcao.criar;
					comando.AdicionarParametroSaida("id", DbType.Int32);
				}

				comando.AdicionarParametroEntrada("titulo", cadastro.Titulo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("protocolo", cadastro.ProtocoloReq.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("destinatario", cadastro.Destinatario, DbType.Int32);
				comando.AdicionarParametroEntrada("matricula", cadastro.Matricula, DbType.String);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				if (id == null || Convert.IsDBNull(id))
				{
					cadastro = cadastro ?? new CadastroAmbientalRuralTitulo();
					cadastro.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
				}

				#endregion

				#region Histórico

				Historico.Gerar(Convert.ToInt32(cadastro.Titulo.Id), eHistoricoArtefatoEspecificidade.cadastroambientalruraltitulo, acao, bancoDeDados);

				#endregion

				#region Consulta

				Consulta.Gerar(cadastro.Titulo.Id, eHistoricoArtefato.carsolicitacaotitulo, bancoDeDados);

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

				Comando comando = bancoDeDados.CriarComando(@"update {0}esp_cad_ambiental_rural c set c.tid = :tid where c.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Histórico

				Historico.Gerar(titulo, eHistoricoArtefatoEspecificidade.cadastroambientalruraltitulo, eHistoricoAcao.excluir, bancoDeDados);

				#endregion

				#region Consulta

				Consulta.Deletar(titulo, eHistoricoArtefato.carsolicitacaotitulo, bancoDeDados);

				#endregion

				#region Apaga os dados da especificidade

				comando = bancoDeDados.CriarComando(@"delete from {0}esp_cad_ambiental_rural e where e.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion

		#region Obter

		internal CadastroAmbientalRuralTitulo Obter(int titulo, BancoDeDados banco = null)
		{
			CadastroAmbientalRuralTitulo especificidade = new CadastroAmbientalRuralTitulo();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Título de Cadastro Ambiental Rural

				Comando comando = bancoDeDados.CriarComando(@"select e.id, e.tid, e.protocolo, e.matricula, n.numero, n.ano, p.requerimento, p.protocolo protocolo_tipo, e.destinatario, 
				(select distinct nvl(pe.nome, pe.razao_social) from {0}hst_esp_cad_ambiental_rural he, {0}hst_pessoa pe where he.destinatario_id = pe.pessoa_id and he.destinatario_tid = pe.tid
				and pe.data_execucao = (select max(h.data_execucao) from {0}hst_pessoa h where h.pessoa_id = pe.pessoa_id and h.tid = pe.tid) and he.especificidade_id = e.id
				and not exists(select 1 from {0}lov_historico_artefatos_acoes l where l.id = he.acao_executada and l.acao = 3) 
				and he.titulo_tid = (select ht.tid from {0}hst_titulo ht where ht.titulo_id = e.titulo and ht.data_execucao = (select min(htt.data_execucao) from {0}hst_titulo htt where htt.titulo_id = e.titulo 
				and htt.data_execucao > (select max(httt.data_execucao) from {0}hst_titulo httt where httt.titulo_id = e.titulo and httt.situacao_id = 1)))) destinatario_nome_razao
				from {0}esp_cad_ambiental_rural e, {0}tab_titulo_numero n, {0}tab_protocolo p where n.titulo(+) = e.titulo and e.protocolo = p.id(+) and e.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						especificidade.Id = Convert.ToInt32(reader["id"]);
						especificidade.Titulo.Id = titulo;
						especificidade.Matricula = reader["matricula"].ToString();
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
					}

					reader.Close();
				}

				#endregion
			}

			return especificidade;
		}

		internal Cadastro ObterDadosPDF(int titulo, BancoDeDados banco)
		{
			Cadastro cadastro = new Cadastro();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados do Titulo

				DadosPDF dados = DaEsp.ObterDadosTitulo(titulo, bancoDeDados);

				cadastro.Titulo = dados.Titulo;
				cadastro.Titulo.SetorEndereco = DaEsp.ObterEndSetor(cadastro.Titulo.SetorId);
				cadastro.Protocolo = dados.Protocolo;
				cadastro.Empreendimento = dados.Empreendimento;

				#endregion

				#region Dados da Especificidade

				Comando comando = bancoDeDados.CriarComando(@"select e.destinatario, e.protocolo, e.matricula, 
															(select count(1) from tab_empreendimento_responsavel er, tab_protocolo p, tab_titulo t 
															where er.empreendimento = p.empreendimento
															and er.responsavel <> e.destinatario
															and p.id = t.protocolo
															and t.id = e.titulo) possui_outros 
															from esp_cad_ambiental_rural e where e.titulo = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						cadastro.Destinatario.Id = Convert.ToInt32(reader["destinatario"]);
						cadastro.Protocolo.Id = Convert.ToInt32(reader["protocolo"]);
						cadastro.DestinatarioPossuiOutros = reader.GetValue<Int32>("possui_outros") > 0;
						cadastro.Matricula = reader.GetValue<String>("matricula");
					}

					reader.Close();
				}

				#endregion

				cadastro.Destinatario = DaEsp.ObterDadosPessoa(cadastro.Destinatario.Id, cadastro.Empreendimento.Id, bancoDeDados);
			}

			return cadastro;
		}

		#endregion

		#region Validacoes

		internal String ObterNumeroTituloCARExistente(int empreendimentoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select tn.numero||'/'||tn.ano titulo_numero from tab_titulo t, tab_titulo_numero tn 
															where t.situacao in (2, 3, 4) /*Emitido, Concluído ou Assinado*/ 
															and t.modelo = (select id from tab_titulo_modelo where codigo = 49/*Cadastro Ambiental Rural*/)
															and t.empreendimento = :empreendimento and tn.titulo = t.id", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);

				return bancoDeDados.ExecutarScalar<String>(comando);

			}
		}

		internal bool EmpreendimentoProjetoGeograficoDominialidadeNaoFinalizado(int empreendimentoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select sum(contador) from (select count(1) contador from tmp_projeto_geo p 
				where p.caracterizacao = 1 and p.empreendimento = :empreendimentoId and p.situacao <> 2/*Finalizado*/
				union all
				select count(1) contador from tmp_projeto_geo p 
				where p.caracterizacao = 1 and p.empreendimento = :empreendimentoId and p.situacao <> 2/*Finalizado*/)", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimentoId", empreendimentoId, DbType.Int32);

				return bancoDeDados.ExecutarScalar<int>(comando) > 0;
			}
		}

		internal bool EmpreendimentoCaracterizacaoCARNaoFinalizada(int empreendimentoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from {0}tmp_cad_ambiental_rural where empreendimento = :empreendimento", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);

				return bancoDeDados.ExecutarScalar<int>(comando) > 0;
			}
		}

		#endregion Validacoes

		#region Auxiliares

		public List<String> ObterMatriculas(int protocoloId, BancoDeDados banco = null)
		{
			List<String> matriculas = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{

				Comando comando = bancoDeDados.CriarComando(@"select nvl(d.matricula, '-') from {0}crt_dominialidade_dominio d 
															where d.tipo = 1 and d.dominialidade = (select id from {0}crt_dominialidade 
															where empreendimento = (select empreendimento from {0}tab_protocolo 
															where id = :protocolo)) order by d.id", EsquemaBanco);

				comando.AdicionarParametroEntrada("protocolo", protocoloId, DbType.Int32);

				matriculas = bancoDeDados.ObterEntityList<String>(comando);
			}

			return matriculas;
		}

		#endregion

		internal SicarPDF ObterNumeroSICAR(int empreendimentoId, BancoDeDados banco)
		{
			SicarPDF sicar = null;
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				SELECT CODIGO_IMOVEL FROM (
					SELECT  CS.CODIGO_IMOVEL FROM TAB_CONTROLE_SICAR CS 
						WHERE CS.SOLICITACAO_CAR_ESQUEMA = 1 AND CS.EMPREENDIMENTO = :empreendimento
					UNION ALL 
					SELECT CS.CODIGO_IMOVEL FROM TAB_CONTROLE_SICAR CS 
							WHERE  CS.SOLICITACAO_CAR_ESQUEMA = 2 AND 
							CS.EMPREENDIMENTO IN (select e.id from IDAFCREDENCIADO.TAB_EMPREENDIMENTO e 
							where e.codigo IN (select ec.codigo from IDAF.TAB_EMPREENDIMENTO ec where ec.id = :empreendimento))
				)	WHERE ROWNUM <= 1", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);

				sicar = new SicarPDF();
				sicar.Numero = bancoDeDados.ExecutarScalar<string>(comando);
			}

			return sicar;
		}

		internal SicarPDF ObterSICARCredenciado(int empreendimentoId, BancoDeDados banco)
		{
			SicarPDF sicar = null;
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@" select l.codigo_imovel_sicar from lst_car_solicitacao_cred l 
					where l.situacao_id in (2,4,5) and l.empreendimento_id = :empreendimento", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);

				sicar = new SicarPDF();
				sicar.Numero = bancoDeDados.ExecutarScalar<string>(comando);
			}
			return sicar;
		}
	}
}