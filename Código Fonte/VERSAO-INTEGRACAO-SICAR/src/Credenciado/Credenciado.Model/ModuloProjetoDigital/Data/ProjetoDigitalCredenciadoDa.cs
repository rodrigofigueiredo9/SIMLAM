using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloProjetoDigital.Data
{
	class ProjetoDigitalCredenciadoDa
	{
		#region Propriedades

		private string EsquemaBanco { get; set; }

		Historico _historico = new Historico();
		public Historico Historico { get { return _historico; } }
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		public String UsuarioCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		#endregion

		public ProjetoDigitalCredenciadoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		public void Salvar(ProjetoDigital projeto, BancoDeDados banco = null, bool gerarHistorico = false)
		{
			if (projeto == null)
			{
				throw new Exception("Projeto digital é nulo.");
			}

			if (projeto.Id == 0)
			{
				Criar(projeto, banco);
			}
			else
			{
				Editar(projeto, banco, gerarHistorico);
			}
		}

		internal void Criar(ProjetoDigital projeto, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}tab_projeto_digital a (id, tid, etapa, situacao, requerimento, data_criacao, credenciado) 
				values ({0}seq_projeto_digital.nextval, :tid, :etapa, :situacao, :requerimento, sysdate, :credenciado) returning a.id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("etapa", (int)eProjetoDigitalEtapa.Requerimento, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", (int)eProjetoDigitalSituacao.EmElaboracao, DbType.Int32);
				comando.AdicionarParametroEntrada("requerimento", projeto.RequerimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("credenciado", projeto.CredenciadoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				projeto.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				Historico.Gerar(projeto.Id, eHistoricoArtefato.projetodigital, eHistoricoAcao.criar, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		internal void Editar(ProjetoDigital projeto, BancoDeDados banco = null, bool gerarHistorico = false)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_projeto_digital r set r.etapa = :etapa, r.situacao = :situacao, 
				r.data_envio = null, r.empreendimento = :empreendimento, r.tid = :tid where r.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", projeto.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("etapa", projeto.Etapa, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", projeto.Situacao, DbType.Int32);
				comando.AdicionarParametroEntrada("empreendimento", (projeto.EmpreendimentoId.GetValueOrDefault() > 0 ? projeto.EmpreendimentoId : null), DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				if (gerarHistorico)
				{
					Historico.Gerar(projeto.Id, eHistoricoArtefato.projetodigital, eHistoricoAcao.atualizar, bancoDeDados);
				}

				bancoDeDados.Commit();
			}
		}

		internal void AlterarSituacao(ProjetoDigital projeto, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_projeto_digital r set r.situacao = :situacao, r.tid = :tid where r.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", projeto.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", projeto.Situacao, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(projeto.Id, eHistoricoArtefato.projetodigital, eHistoricoAcao.alterarsituacao, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		internal void Enviar(ProjetoDigital projeto, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_projeto_digital r set r.situacao = :situacao, r.etapa = :etapa, 
                r.data_envio = sysdate, r.motivo_recusa = null, r.tid = :tid where r.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", projeto.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", projeto.Situacao, DbType.Int32);
				comando.AdicionarParametroEntrada("etapa", projeto.Etapa, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(projeto.Id, eHistoricoArtefato.projetodigital, eHistoricoAcao.enviar, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		internal void CancelarEnvio(ProjetoDigital projeto, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_projeto_digital r set r.situacao = :situacao, r.etapa = :etapa, 
				r.data_envio = null, r.tid = :tid where r.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", projeto.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", projeto.Situacao, DbType.Int32);
				comando.AdicionarParametroEntrada("etapa", projeto.Etapa, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(projeto.Id, eHistoricoArtefato.projetodigital, eHistoricoAcao.cancelarenvio, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_projeto_digital c set c.tid = :tid where c.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(id, eHistoricoArtefato.projetodigital, eHistoricoAcao.excluir, bancoDeDados);

				comando = bancoDeDados.CriarComandoPlSql(@"
				begin
					delete {0}tab_proj_digital_dependencias where projeto_digital_id = :id;
					delete {0}tab_projeto_digital a where a.id = :id;
				end;", EsquemaBanco);
				
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}

		internal void Recusar(ProjetoDigital projeto, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_projeto_digital r set r.situacao = :situacao, r.motivo_recusa = :motivo_recusa, r.tid = :tid where r.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", projeto.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", (int)eProjetoDigitalSituacao.AguardandoCorrecao, DbType.Int32);
				comando.AdicionarParametroEntrada("motivo_recusa", DbType.String, 3000, projeto.MotivoRecusa);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(projeto.Id, eHistoricoArtefato.projetodigital, eHistoricoAcao.alterarsituacao, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		internal void AssociarDependencias(ProjetoDigital projeto, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();
				Comando comando;

				if (projeto.Dependencias != null && projeto.Dependencias.Count > 0)
				{
					#region Obter ID

					foreach (var item in projeto.Dependencias)
					{
						comando = bancoDeDados.CriarComando(@"
						select d.id from tab_proj_digital_dependencias d
						where d.projeto_digital_id = :projeto_digital_id 
						and d.dependencia_tipo = :dependencia_tipo
						and d.dependencia_caracterizacao = :dependencia_caracterizacao", EsquemaBanco);

						comando.AdicionarParametroEntrada("projeto_digital_id", projeto.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("dependencia_tipo", item.DependenciaTipo, DbType.Int32);
						comando.AdicionarParametroEntrada("dependencia_caracterizacao", item.DependenciaCaracterizacao, DbType.Int32);

						object retorno = bancoDeDados.ExecutarScalar(comando);
						item.Id = (retorno != null && !Convert.IsDBNull(retorno)) ? Convert.ToInt32(retorno) : 0;
					}

					#endregion

					#region Salvar

					foreach (var item in projeto.Dependencias)
					{
						if (item.Id > 0)
						{
							comando = bancoDeDados.CriarComando(@"
							update {0}tab_proj_digital_dependencias d set 
							d.dependencia_id = :dependencia_id, 
							d.dependencia_tid = :dependencia_tid, 
							d.tid = :tid 
							where d.id = :id", EsquemaBanco);

							comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"
							insert into {0}tab_proj_digital_dependencias d 
							(id, projeto_digital_id, dependencia_tipo, dependencia_caracterizacao, dependencia_id, dependencia_tid, tid) values 
							({0}seq_proj_digital_dependencias.nextval, :projeto_digital_id, :dependencia_tipo, :dependencia_caracterizacao, :dependencia_id, :dependencia_tid, :tid) 
							returning id into :id_dependencia", EsquemaBanco);

							comando.AdicionarParametroEntrada("projeto_digital_id", projeto.Id, DbType.Int32);
							comando.AdicionarParametroSaida("id_dependencia", DbType.Int32);
							comando.AdicionarParametroEntrada("dependencia_tipo", item.DependenciaTipo, DbType.Int32);
							comando.AdicionarParametroEntrada("dependencia_caracterizacao", item.DependenciaCaracterizacao, DbType.Int32);
						}

						comando.AdicionarParametroEntrada("dependencia_id", item.DependenciaId, DbType.Int32);
						comando.AdicionarParametroEntrada("dependencia_tid", DbType.String, 36, item.DependenciaTid);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);

						if (item.Id <= 0)
						{
							item.Id = Convert.ToInt32(comando.ObterValorParametro("id_dependencia"));
						}

						//VERIFICAR
						//Historico.Gerar(item.Id, eHistoricoArtefatoCaracterizacao.dependencia, eHistoricoAcao.atualizar, bancoDeDados, executor: null);
					}

					#endregion
				}

				bancoDeDados.Commit();
			}
		}

		internal void DesassociarDependencias(ProjetoDigital projeto, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando;

				if (projeto.EmpreendimentoId > 0)
				{
					comando = bancoDeDados.CriarComando(@"delete from {0}tab_proj_digital_dependencias d 
                    where exists (select 1 from {0}tab_projeto_digital p where p.id = d.projeto_digital_id and p.situacao in (1, 3))
					and d.projeto_digital_id in (select p.id from {0}tab_projeto_digital p where p.empreendimento = :empreendimento) ", EsquemaBanco);
					comando.AdicionarParametroEntrada("empreendimento", projeto.EmpreendimentoId, DbType.Int32);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"delete from {0}tab_proj_digital_dependencias d
                    where exists (select 1 from {0}tab_projeto_digital p where p.id = d.projeto_digital_id and p.situacao in (1, 3))", EsquemaBanco);
				}

				comando.DbCommand.CommandText += comando.FiltroAnd("d.projeto_digital_id", "projeto_digital_id", projeto.Id);

				if (projeto.Dependencias != null && projeto.Dependencias.Count > 0)
				{
					comando.DbCommand.CommandText += comando.FiltroAnd("d.dependencia_caracterizacao", "dependencia_caracterizacao", projeto.Dependencias.First().DependenciaCaracterizacao);
				}

				bancoDeDados.ExecutarNonQuery(comando);
				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Ações DML Temporario

		internal void CriarTemporario(ProjetoDigital projeto, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into tmp_projeto_digital (
				id, tid, etapa_importacao, situacao_id, requerimento_id, requerimento_tid, empreendimento_id, 
				empreendimento_tid, data_criacao, data_envio, credenciado_id, credenciado_tid, data_execucao) 
				values 
				(:id, :tid, :etapa_importacao, :situacao_id, :requerimento_id, :requerimento_tid, :empreendimento_id, 
				:empreendimento_tid, :data_criacao, :data_envio, :credenciado_id, :credenciado_tid, sysdate)");

				comando.AdicionarParametroEntrada("id", projeto.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, projeto.Tid);
				comando.AdicionarParametroEntrada("etapa_importacao", (int)eProjetoDigitalEtapa.Requerimento, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao_id", projeto.Situacao, DbType.Int32);
				comando.AdicionarParametroEntrada("requerimento_id", projeto.RequerimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("requerimento_tid", DbType.String, 36, projeto.RequerimentoTid);
				comando.AdicionarParametroEntrada("empreendimento_id", projeto.EmpreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("empreendimento_tid", DbType.String, 36, projeto.EmpreendimentoTid);
				comando.AdicionarParametroEntrada("credenciado_id", projeto.CredenciadoId, DbType.Int32);
				comando.AdicionarParametroEntrada("credenciado_tid", DbType.String, 36, projeto.CredenciadoTid);
				comando.AdicionarParametroEntrada("data_criacao", projeto.DataCriacao.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("data_envio", projeto.DataEnvio.Data, DbType.DateTime);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}

		internal void ExcluirTemporario(int id, BancoDeDados banco)
		{

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"delete tmp_projeto_digital a where a.id = :id");
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}

		internal void AlterarEtapaTemporario(int projetoDigitalId, eProjetoDigitalEtapaImportacao etapaImportacao, BancoDeDados banco)
		{
			//Não atualiza o Tid da tmp_projeto_digital
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update tmp_projeto_digital t set t.etapa_importacao = :etapa_importacao where t.id = :id");

				comando.AdicionarParametroEntrada("id", projetoDigitalId, DbType.Int32);
				comando.AdicionarParametroEntrada("etapa_importacao", (int)etapaImportacao, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter / Filtrar

		public ProjetoDigital Obter(int id, BancoDeDados banco = null, string tid = null)
		{
			ProjetoDigital projeto = new ProjetoDigital();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
			{
				#region Projeto Digital

				if (String.IsNullOrWhiteSpace(tid))
				{
					projeto = Obter(idProjeto: id, banco: bancoDeDados);
				}
				else
				{
					Comando comando = bancoDeDados.CriarComando(@"select count(r.id) existe from {0}tab_projeto_digital r where r.id = :id and r.tid = :tid", EsquemaBanco);

					comando.AdicionarParametroEntrada("id", id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

					if (Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando)))
					{
						projeto = Obter(idProjeto: id, banco: bancoDeDados);
					}
					else
					{
						projeto = ObterHistorico(id, tid, bancoDeDados);
					}
				}

				#endregion
			}

			return projeto;
		}

		internal ProjetoDigital Obter(int idProjeto = 0, int idRequerimento = 0, BancoDeDados banco = null)
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

				if (projeto.Id > 0)
				{
					projeto.Dependencias = ObterDependencias(projeto.Id);
				}

				#endregion
			}

			return projeto;
		}

		internal ProjetoDigital ObterHistorico(int id, string tid, BancoDeDados banco = null)
		{
			ProjetoDigital projeto = new ProjetoDigital();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
			{
				#region Projeto Digital

				Comando comando = bancoDeDados.CriarComando(@"select p.id, p.projeto_id, p.tid, p.etapa, p.situacao_id, p.requerimento_id, p.requerimento_tid, 
				p.empreendimento_id, p.empreendimento_tid, p.data_criacao, p.data_envio, p.credenciado_id, p.credenciado_tid, p.motivo_recusa 
				from {0}hst_projeto_digital p where p.projeto_id = :id and p.tid = :tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						projeto.Id = reader.GetValue<int>("projeto_id");
						projeto.Tid = reader.GetValue<string>("tid");
						projeto.Etapa = reader.GetValue<int>("etapa");
						projeto.Situacao = reader.GetValue<int>("situacao_id");
						projeto.RequerimentoId = reader.GetValue<int>("requerimento_id");
						projeto.RequerimentoTid = reader.GetValue<string>("requerimento_tid");
						projeto.EmpreendimentoId = reader.GetValue<int>("empreendimento_id");
						projeto.EmpreendimentoTid = reader.GetValue<string>("empreendimento_tid");
						projeto.DataCriacao.Data = reader.GetValue<DateTime>("data_criacao");
						projeto.DataEnvio.Data = reader.GetValue<DateTime>("data_envio");
						projeto.CredenciadoId = reader.GetValue<int>("credenciado_id");
						projeto.CredenciadoTid = reader.GetValue<string>("credenciado_tid");
						projeto.MotivoRecusa = reader.GetValue<string>("motivo_recusa");
					}

					reader.Close();
				}

				#endregion
			}

			return projeto;
		}

		public Resultados<ProjetoDigital> Filtrar(Filtro<ProjetoDigitalListarFiltro> filtros)
		{
			Resultados<ProjetoDigital> retorno = new Resultados<ProjetoDigital>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaBanco))
			{
				string comandtxt = string.Empty;
				string esquemaBanco = (string.IsNullOrEmpty(EsquemaBanco) ? "" : EsquemaBanco + ".");
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAnd("p.credenciado", "credenciado", filtros.Dados.Credenciado);

				comandtxt += comando.FiltroAnd("p.requerimento", "requerimento", filtros.Dados.Requerimento);

				comandtxt += comando.FiltroAnd("p.situacao", "situacao", filtros.Dados.Situacao);

				comandtxt += comando.FiltroAnd("p.empreendimento", "empreendimento", filtros.Dados.EmpreendimentoID);

				if (!string.IsNullOrEmpty(filtros.Dados.DataEnvio.DataTexto))
				{
					comandtxt += comando.FiltroAnd("p.data_envio", "data_envio", filtros.Dados.DataEnvio.Data);
				}

				if (filtros.Dados.IsCpf)
				{
					comandtxt += comando.FiltroAndLike("pe.cpf", "interessadocpfcnpj", filtros.Dados.InteressadoCpfCnpj);
				}
				else
				{
					comandtxt += comando.FiltroAndLike("pe.cnpj", "interessadocpfcnpj", filtros.Dados.InteressadoCpfCnpj);
				}

				if (!String.IsNullOrWhiteSpace(filtros.Dados.InteressadoNomeRazaoSocial) && (!ValidacoesGenericasBus.Cpf(filtros.Dados.InteressadoCpfCnpj)
					&& !ValidacoesGenericasBus.Cnpj(filtros.Dados.InteressadoCpfCnpj)))
				{
					comandtxt += " and (upper(pe.nome) like upper(:interessadonomerazaosocial)||'%' or upper(pe.razao_social) like upper(:interessadonomerazaosocial)||'%')";
					comando.AdicionarParametroEntrada("interessadonomerazaosocial", filtros.Dados.InteressadoNomeRazaoSocial, DbType.String);
				}

				comandtxt += comando.FiltroAndLike("e.cnpj", "empreendimentocnpj", filtros.Dados.EmpreendimentoCnpj);

				comandtxt += comando.FiltroAndLike("e.denominador", "empreendimentonomerazaosocial", filtros.Dados.EmpreendimentoNomeRazaoSocial, true);

				string sqlClassificacao = string.Empty;

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "requerimento", "interessado", "empreendimento", "situacao" };

				if (filtros.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("requerimento");
				}

				#endregion

				#region Quantidade de registro do resultado

				comando.DbCommand.CommandText = String.Format(@"select count(*) from {0}tab_projeto_digital p, {0}tab_empreendimento e, {0}tab_requerimento r, {0}tab_pessoa pe 
				where p.empreendimento = e.id(+) and p.requerimento = r.id and r.interessado = pe.id(+) " + comandtxt, esquemaBanco);

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = String.Format(@"select p.id, p.requerimento, p.tid, p.etapa, p.empreendimento, e.denominador, nvl(pe.nome, pe.razao_social) interessado, p.situacao, ls.texto situacao_texto, 
				p.motivo_recusa from {0}tab_projeto_digital p, {0}tab_empreendimento e, {0}tab_requerimento r, {0}tab_pessoa pe, {0}lov_projeto_digital_situacao ls where p.empreendimento = e.id(+) 
				and p.requerimento = r.id and r.interessado = pe.id(+) and ls.id = p.situacao " + comandtxt + DaHelper.Ordenar(colunas, ordenar), esquemaBanco);

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					ProjetoDigital projeto;

					while (reader.Read())
					{
						projeto = new ProjetoDigital();
						projeto.Id = reader.GetValue<int>("id");
						projeto.RequerimentoId = reader.GetValue<int>("requerimento");
						projeto.Tid = reader.GetValue<string>("tid");
						projeto.Etapa = reader.GetValue<int>("etapa");
						projeto.EmpreendimentoId = reader.GetValue<int>("empreendimento");
						projeto.EmpreendimentoTexto = reader.GetValue<string>("denominador");
						projeto.Interessado = reader.GetValue<string>("interessado");
						projeto.Situacao = reader.GetValue<int>("situacao");
						projeto.SituacaoTexto = reader.GetValue<string>("situacao_texto");
						projeto.MotivoRecusa = reader.GetValue<string>("motivo_recusa");
						retorno.Itens.Add(projeto);
					}

					reader.Close();

					#endregion
				}
			}

			return retorno;
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

		internal Int32 ObterProjetoDigitalId(int requerimentoId, BancoDeDados banco = null)
		{
			int retorno = 0;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select id from {0}tab_projeto_digital where requerimento = :requerimento", UsuarioCredenciado);
				comando.AdicionarParametroEntrada("requerimento", requerimentoId, DbType.Int32);
				object obj = bancoDeDados.ExecutarScalar(comando);

				if (obj != null && !Convert.IsDBNull(obj))
				{
					retorno = Convert.ToInt32(obj);
				}
			}

			return retorno;
		}

		#endregion

		#region Validações

		internal bool PossuiAtividadeCAR(int projetoDigitalID)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaBanco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from tab_projeto_digital p, tab_requerimento r, tab_requerimento_atividade ra
				where p.requerimento = r.id and r.id = ra.requerimento and ra.atividade = (select a.id from tab_atividade a where a.codigo = 
				(select to_number(c.valor) from cnf_sistema c where c.campo = 'cadastroambientalruralatividadecodigo')) and p.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", projetoDigitalID, DbType.Int32);
				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		#endregion

		internal List<Blocos.Entities.Interno.ModuloAtividade.Atividade> ObterAtividades(int projetoDigitalId)
		{
			List<Atividade> atividades = new List<Atividade>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaBanco))
			{
				Comando comando = null;
				#region Atividades

				comando = bancoDeDados.CriarComando(@"select ta.id, ta.atividade, ta.tid from {0}tab_atividade ta, {0}tab_requerimento_atividade tr ,{0}tab_projeto_digital tp 
                where  tr.atividade = ta.id and tp.requerimento = tr.requerimento and tp.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", projetoDigitalId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Atividade atividade;

					while (reader.Read())
					{
						atividade = new Atividade();
						atividade.Id = Convert.ToInt32(reader["id"]);
						atividade.NomeAtividade = reader.GetValue<string>("atividade");
						atividade.Tid = reader.GetValue<string>("tid");
						atividades.Add(atividade);
					}

					reader.Close();
				}

				#endregion
			}


			return atividades;
		}

		internal int ObterArquivoCroquiId(int idProjetoGeo, BancoDeDados banco = null)
		{
			int arquivoId = 0;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
			{
				Comando comando = null;
				comando = bancoDeDados.CriarComando(@"select c.arquivo from crt_projeto_geo_arquivos c where c.tipo = 7 and  c.projeto = :idProjeto", EsquemaBanco);
				comando.AdicionarParametroEntrada("idProjeto", idProjetoGeo, DbType.Int32);

				arquivoId = bancoDeDados.ExecutarScalar<int>(comando);

			}
			return arquivoId;
		}
	}
}