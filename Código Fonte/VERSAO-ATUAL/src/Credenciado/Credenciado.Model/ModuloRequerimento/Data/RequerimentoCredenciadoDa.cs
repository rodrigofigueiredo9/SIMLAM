using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloRequerimento.Business;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloRequerimento.Data
{
	public class RequerimentoCredenciadoDa
	{
		#region Requerimento

		private Historico Historico { get; set; }
		private RequerimentoInternoBus _busInterno { get; set; }

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys;

		public Int32 RoteiroPadrao
		{
			get { return _configSys.Obter<Int32>(ConfiguracaoSistema.KeyRoteiroPadrao); }
		}

		public String UsuarioCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		private static EtramitePrincipal User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal); }
		}

		#endregion

		public RequerimentoCredenciadoDa()
		{
			Historico = new Historico();
			_configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
			_busInterno = new RequerimentoInternoBus();
		}

		#region Ações de DML

		public void Salvar(Requerimento requerimento, BancoDeDados banco = null)
		{
			if (requerimento == null)
			{
				throw new Exception("Requerimento é nulo.");
			}

			if (requerimento.Id <= 0)
			{
				requerimento.Id = _busInterno.ObterNovoID();
				Criar(requerimento, banco);
			}
			else
			{
				Editar(requerimento, banco);
			}
		}

		internal int? Criar(Requerimento requerimento, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{
				#region Requerimento

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}tab_requerimento e (id, numero, data_criacao, situacao, tid, agendamento, setor, informacoes, credenciado) values 
				(:requerimento, :requerimento, sysdate, :situacao, :tid, :agendamento, :setor, :informacoes, :credenciado) returning e.id into :id", UsuarioCredenciado);

				comando.AdicionarParametroEntrada("situacao", 1, DbType.Int32); // 1 - Em andamento
				comando.AdicionarParametroEntrada("requerimento", requerimento.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("agendamento", requerimento.AgendamentoVistoria, DbType.Int32);
				comando.AdicionarParametroEntrada("setor", DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("informacoes", DbType.String, 500, requerimento.Informacoes);
				comando.AdicionarParametroEntrada("credenciado", requerimento.CredenciadoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				requerimento.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#endregion

				#region Barragem
				if (requerimento.ResponsabilidadeRT != null || requerimento.ResponsabilidadeRT > 0)
				{
					comando = bancoDeDados.CriarComando(@"
						insert into tab_requerimento_barragem 
							(id, requerimento, rt_elaboracao, possui_barragem_contigua)      
							values(seq_requerimento_barragem.nextval, :requerimento, :rt_elaboracao, :possui_barragem_contigua)", UsuarioCredenciado);

					comando.AdicionarParametroEntrada("requerimento", requerimento.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("rt_elaboracao", requerimento.ResponsabilidadeRT, DbType.Int32);
					comando.AdicionarParametroEntrada("possui_barragem_contigua", requerimento.BarragensContiguas, DbType.Int32);

					bancoDeDados.ExecutarNonQuery(comando);

				}
				#endregion

				#region Atividades
				if (requerimento.Atividades != null && requerimento.Atividades.Count > 0)
				{
					foreach (Atividade item in requerimento.Atividades)
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}tab_requerimento_atividade t (id, requerimento, atividade, tid)
						values ({0}seq_requerimento_atividade.nextval, :requerimento, :atividade, :tid) returning t.id into :id", UsuarioCredenciado);
						comando.AdicionarParametroSaida("id", DbType.Int32);

						comando.AdicionarParametroEntrada("requerimento", requerimento.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("atividade", item.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);

						item.IdRelacionamento = Convert.ToInt32(comando.ObterValorParametro("id"));

						#region Modelo de Título

						if (item.Finalidades.Count > 0)
						{
							foreach (Finalidade itemAux in item.Finalidades)
							{
								comando = bancoDeDados.CriarComando(@"insert into {0}tab_requerimento_ativ_finalida (id, requerimento_ativ, modelo,
								titulo_anterior_tipo, titulo_anterior_id, titulo_anterior_numero, modelo_anterior_id, modelo_anterior_nome, modelo_anterior_sigla, orgao_expedidor, finalidade, tid) values 
								({0}seq_requerimento_ativ_fin.nextval, :requerimento_ativ, :modelo, :titulo_anterior_tipo, :titulo_anterior_id, :titulo_anterior_numero, :modelo_anterior_id, 
								:modelo_anterior_nome, :modelo_anterior_sigla, :orgao_expedidor, :finalidade, :tid)", UsuarioCredenciado);

								comando.AdicionarParametroEntrada("requerimento_ativ", item.IdRelacionamento, DbType.Int32);
								comando.AdicionarParametroEntrada("modelo", itemAux.TituloModelo, DbType.Int32);
								comando.AdicionarParametroEntrada("titulo_anterior_tipo", itemAux.TituloAnteriorTipo, DbType.Int32);
								comando.AdicionarParametroEntrada("titulo_anterior_id", itemAux.TituloAnteriorId, DbType.Int32);
								comando.AdicionarParametroEntrada("titulo_anterior_numero", DbType.String, 20, itemAux.TituloAnteriorNumero);
								comando.AdicionarParametroEntrada("modelo_anterior_id", itemAux.TituloModeloAnteriorId, DbType.Int32);
								comando.AdicionarParametroEntrada("modelo_anterior_nome", DbType.String, 100, itemAux.TituloModeloAnteriorTexto);
								comando.AdicionarParametroEntrada("modelo_anterior_sigla", DbType.String, 100, itemAux.TituloModeloAnteriorSigla);
								comando.AdicionarParametroEntrada("orgao_expedidor", DbType.String, 100, itemAux.OrgaoExpedidor);
								comando.AdicionarParametroEntrada("finalidade", itemAux.Id, DbType.Int32);
								comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
								comando.AdicionarParametroEntrada("modelo_anterior_sigla", DbType.String, 100, itemAux.TituloModeloAnteriorSigla);

								bancoDeDados.ExecutarNonQuery(comando);
							}
						}
						#endregion
					}
				}

				#endregion

				#region Histórico

				Historico.Gerar(requerimento.Id, eHistoricoArtefato.requerimento, eHistoricoAcao.criar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();

				return requerimento.Id;
			}
		}

		internal void Editar(Requerimento requerimento, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{
				bancoDeDados.IniciarTransacao();

				#region Requerimento

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_requerimento r set r.interessado = :interessado, r.empreendimento = :empreendimento, 
				r.situacao = :situacao, r.tid = :tid, r.agendamento = :agendamento, r.informacoes = :informacoes where r.id = :id", UsuarioCredenciado);

				comando.AdicionarParametroEntrada("id", requerimento.Id, DbType.Int32);

				if (requerimento.Interessado.Id > 0)
				{
					comando.AdicionarParametroEntrada("interessado", requerimento.Interessado.Id, DbType.Int32);
				}
				else
				{
					comando.AdicionarParametroEntrada("interessado", null, DbType.Int32);
				}

				if (requerimento.Empreendimento.Id > 0)
				{
					comando.AdicionarParametroEntrada("empreendimento", requerimento.Empreendimento.Id, DbType.Int32);
				}
				else
				{
					comando.AdicionarParametroEntrada("empreendimento", null, DbType.Int32);
				}

				comando.AdicionarParametroEntrada("agendamento", requerimento.AgendamentoVistoria, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", requerimento.SituacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("informacoes", DbType.String, 500, requerimento.Informacoes);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Barragem
				if (requerimento.ResponsabilidadeRT != null || requerimento.ResponsabilidadeRT > 0)
				{
					Comando cmd = bancoDeDados.CriarComando(@"
					select coalesce(id, 0) id_barragem from tab_requerimento_barragem where requerimento = :requerimento", UsuarioCredenciado);

					cmd.AdicionarParametroEntrada("requerimento", requerimento.Id, DbType.Int32);

					using (IDataReader reader = bancoDeDados.ExecutarReader(cmd))
					{
						if (reader.Read())
						{
							comando = bancoDeDados.CriarComando(@"
							update tab_requerimento_barragem
								set rt_elaboracao = :rt_elaboracao, possui_barragem_contigua = :possui_barragem_contigua
							where requerimento = :requerimento", UsuarioCredenciado);

							comando.AdicionarParametroEntrada("requerimento", requerimento.Id, DbType.Int32);
							comando.AdicionarParametroEntrada("rt_elaboracao", requerimento.ResponsabilidadeRT, DbType.Int32);
							comando.AdicionarParametroEntrada("possui_barragem_contigua", requerimento.BarragensContiguas, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"
							insert into tab_requerimento_barragem 
								(id, requerimento, rt_elaboracao, possui_barragem_contigua)      
								values(seq_requerimento_barragem.nextval, :requerimento, :rt_elaboracao, :possui_barragem_contigua)", UsuarioCredenciado);

							comando.AdicionarParametroEntrada("requerimento", requerimento.Id, DbType.Int32);
							comando.AdicionarParametroEntrada("rt_elaboracao", requerimento.ResponsabilidadeRT, DbType.Int32);
							comando.AdicionarParametroEntrada("possui_barragem_contigua", requerimento.BarragensContiguas, DbType.Int32);
						}
					}
					bancoDeDados.ExecutarNonQuery(comando);
				}
				#endregion

				#region Limpar os dados do banco

				#region Atividade/Modelos

				comando = bancoDeDados.CriarComando(@"delete from {0}tab_requerimento_ativ_finalida c 
				where c.requerimento_ativ in (select a.id from {0}tab_requerimento_atividade a where a.requerimento = :requerimento ", UsuarioCredenciado);

				comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "a.atividade", DbType.Int32, requerimento.Atividades.Select(x => x.Id).ToList());

				comando.DbCommand.CommandText += ")";

				comando.AdicionarParametroEntrada("requerimento", requerimento.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				if (requerimento.Atividades != null && requerimento.Atividades.Count > 0)
				{
					foreach (Atividade item in requerimento.Atividades)
					{
						comando = bancoDeDados.CriarComando(@"delete from {0}tab_requerimento_ativ_finalida c 
						where c.requerimento_ativ in (select a.id from {0}tab_requerimento_atividade a where a.requerimento = :requerimento and a.atividade = :atividade)", UsuarioCredenciado);
						comando.DbCommand.CommandText += String.Format(" {0}", comando.AdicionarNotIn("and", "c.id", DbType.Int32, item.Finalidades.Select(x => x.IdRelacionamento).ToList()));

						comando.AdicionarParametroEntrada("atividade", item.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("requerimento", requerimento.Id, DbType.Int32);

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				//Atividades
				comando = bancoDeDados.CriarComando("delete from {0}tab_requerimento_atividade c ", UsuarioCredenciado);

				comando.DbCommand.CommandText += String.Format("where c.requerimento = :requerimento{0}",
				comando.AdicionarNotIn("and", "c.id", DbType.Int32, requerimento.Atividades.Select(x => x.IdRelacionamento).ToList()));

				comando.AdicionarParametroEntrada("requerimento", requerimento.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);
				#endregion

				//Responsáveis
				comando = bancoDeDados.CriarComando("delete from {0}tab_requerimento_responsavel c ", UsuarioCredenciado);
				comando.DbCommand.CommandText += String.Format("where c.requerimento = :requerimento{0}",
				comando.AdicionarNotIn("and", "c.id", DbType.Int32, requerimento.Responsaveis.Select(x => x.IdRelacionamento).ToList()));
				comando.AdicionarParametroEntrada("requerimento", requerimento.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);
				#endregion

				#region Atividades
				if (requerimento.Atividades != null && requerimento.Atividades.Count > 0)
				{
					foreach (Atividade item in requerimento.Atividades)
					{
						if (item.IdRelacionamento > 0)
						{
							comando = bancoDeDados.CriarComando(@"update {0}tab_requerimento_atividade e set e.requerimento = :requerimento, e.atividade = :atividade, e.tid =:tid where e.id = :id", UsuarioCredenciado);
							comando.AdicionarParametroEntrada("id", item.IdRelacionamento, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}tab_requerimento_atividade t (id, requerimento, atividade, tid)
							values ({0}seq_requerimento_atividade.nextval, :requerimento, :atividade, :tid) returning t.id into :id", UsuarioCredenciado);
							comando.AdicionarParametroSaida("id", DbType.Int32);
						}

						comando.AdicionarParametroEntrada("requerimento", requerimento.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("atividade", item.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);

						if (item.IdRelacionamento <= 0)
						{
							item.IdRelacionamento = Convert.ToInt32(comando.ObterValorParametro("id"));
						}

						#region Modelo de Título

						if (item.Finalidades != null)
						{
							foreach (Finalidade itemAux in item.Finalidades)
							{
								if (itemAux.IdRelacionamento > 0)
								{
									comando = bancoDeDados.CriarComando(@"update {0}tab_requerimento_ativ_finalida a set requerimento_ativ = :requerimento_ativ,
									modelo = :modelo, titulo_anterior_tipo = :titulo_anterior_tipo, titulo_anterior_id = :titulo_anterior_id, titulo_anterior_numero = :titulo_anterior_numero, 
									modelo_anterior_id = :modelo_anterior_id, modelo_anterior_nome = :modelo_anterior_nome, modelo_anterior_sigla = :modelo_anterior_sigla, orgao_expedidor = :orgao_expedidor, finalidade = :finalidade, 
									tid = :tid where id = :id", UsuarioCredenciado);
									comando.AdicionarParametroEntrada("id", itemAux.IdRelacionamento, DbType.Int32);
								}
								else
								{
									comando = bancoDeDados.CriarComando(@"insert into {0}tab_requerimento_ativ_finalida (id, requerimento_ativ, modelo,
									titulo_anterior_tipo, titulo_anterior_id, titulo_anterior_numero, modelo_anterior_id, modelo_anterior_nome, modelo_anterior_sigla, orgao_expedidor, finalidade, tid) values 
									({0}seq_requerimento_ativ_fin.nextval, :requerimento_ativ, :modelo, :titulo_anterior_tipo, :titulo_anterior_id, :titulo_anterior_numero, :modelo_anterior_id, 
									:modelo_anterior_nome, :modelo_anterior_sigla, :orgao_expedidor, :finalidade, :tid)", UsuarioCredenciado);
								}

								comando.AdicionarParametroEntrada("finalidade", itemAux.Id, DbType.Int32);
								comando.AdicionarParametroEntrada("requerimento_ativ", item.IdRelacionamento, DbType.Int32);
								comando.AdicionarParametroEntrada("modelo", itemAux.TituloModelo, DbType.Int32);
								comando.AdicionarParametroEntrada("titulo_anterior_tipo", itemAux.TituloAnteriorTipo, DbType.Int32);
								comando.AdicionarParametroEntrada("titulo_anterior_id", itemAux.TituloAnteriorId, DbType.Int32);
								comando.AdicionarParametroEntrada("titulo_anterior_numero", DbType.String, 20, itemAux.TituloAnteriorNumero);
								comando.AdicionarParametroEntrada("modelo_anterior_id", itemAux.TituloModeloAnteriorId, DbType.Int32);
								comando.AdicionarParametroEntrada("modelo_anterior_nome", DbType.String, 100, itemAux.TituloModeloAnteriorTexto);
								comando.AdicionarParametroEntrada("modelo_anterior_sigla", DbType.String, 100, itemAux.TituloModeloAnteriorSigla);
								comando.AdicionarParametroEntrada("orgao_expedidor", DbType.String, 100, itemAux.OrgaoExpedidor);
								comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
								bancoDeDados.ExecutarNonQuery(comando);
							}
						}
						#endregion
					}
				}

				#endregion

				#region Responsáveis

				if (requerimento.Responsaveis != null && requerimento.Responsaveis.Count > 0)
				{
					foreach (ResponsavelTecnico responsavel in requerimento.Responsaveis)
					{
						if (responsavel.IdRelacionamento > 0)
						{
							comando = bancoDeDados.CriarComando(@"update {0}tab_requerimento_responsavel r set r.requerimento = :requerimento, r.responsavel = :responsavel, 
							r.funcao = :funcao, r.numero_art = :numero_art, r.tid = :tid where r.id = :id", UsuarioCredenciado);
							comando.AdicionarParametroEntrada("id", responsavel.IdRelacionamento, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}tab_requerimento_responsavel(id, requerimento, responsavel, funcao, numero_art, tid) values
							({0}seq_requerimento_responsavel.nextval, :requerimento, :responsavel, :funcao, :numero_art, :tid )", UsuarioCredenciado);
						}

						comando.AdicionarParametroEntrada("requerimento", requerimento.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("responsavel", responsavel.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("funcao", responsavel.Funcao, DbType.Int32);
						comando.AdicionarParametroEntrada("numero_art", responsavel.NumeroArt ?? "", DbType.String);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());


						bancoDeDados.ExecutarNonQuery(comando);
					}
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"delete {0}tab_requerimento_responsavel p where p.requerimento = :id", UsuarioCredenciado);
					comando.AdicionarParametroEntrada("id", requerimento.Id, DbType.Int32);
					bancoDeDados.ExecutarNonQuery(comando);
				}
				#endregion

				#region Histórico

				Historico.Gerar(requerimento.Id, eHistoricoArtefato.requerimento, eHistoricoAcao.atualizar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{
				bancoDeDados.IniciarTransacao();

				#region Atualizar o tid para a nova ação

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_requerimento c set c.tid = :tid where c.id = :id", UsuarioCredenciado);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);
				#endregion

				#region Histórico

				Historico.Gerar(id, eHistoricoArtefato.requerimento, eHistoricoAcao.excluir, bancoDeDados);

				#endregion

				#region Apaga os dados de requerimento

				List<String> lista = new List<string>();

				lista.Add(@"delete from {0}tab_requerimento_ativ_finalida a where a.requerimento_ativ in (select b.id from {0}tab_requerimento_atividade b where b.requerimento = :requerimento);");
				lista.Add("delete from {0}tab_requerimento_atividade b where b.requerimento = :requerimento;");
				lista.Add("delete from {0}tab_requerimento_responsavel c where c.requerimento = :requerimento;");
				lista.Add("delete from {0}tab_requerimento_barragem c where c.requerimento = :requerimento;");
				lista.Add("delete from {0}tab_requerimento e where e.id = :requerimento;");

				comando = bancoDeDados.CriarComando(@" begin " + string.Join(" ", lista) + " end;", UsuarioCredenciado);
				comando.AdicionarParametroEntrada("requerimento", id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion

		#region Obter / Filtrar

		internal int ObterPessoaId(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select id from {0}tab_pessoa p where p.credenciado = :id and p.usuario = 1", UsuarioCredenciado);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				object retorno = bancoDeDados.ExecutarScalar(comando);

				return (retorno != null && !Convert.IsDBNull(retorno)) ? Convert.ToInt32(retorno) : 0;
			}
		}

		internal int ObterPessoaIdPorUsuario(int usuarioId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select pessoa from {0}tab_credenciado p where p.usuario = :id", UsuarioCredenciado);
				comando.AdicionarParametroEntrada("id", usuarioId, DbType.Int32);

				object retorno = bancoDeDados.ExecutarScalar(comando);

				return (retorno != null && !Convert.IsDBNull(retorno)) ? Convert.ToInt32(retorno) : 0;
			}
		}

		internal Requerimento Obter(int id, BancoDeDados banco = null, bool simplificado = false)
		{
			Requerimento requerimento = new Requerimento();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{
				#region Requerimento

				Comando comando = bancoDeDados.CriarComando(@"select r.id, r.numero, trunc(r.data_criacao) data_criacao, r.interessado, r.tid,
				nvl(p.nome, p.razao_social) interessado_nome, nvl(p.cpf, p.cnpj) interessado_cpf_cnpj, p.tipo interessado_tipo, r.empreendimento, e.codigo empreendimento_codigo, 
				e.cnpj empreendimento_cnpj, e.denominador, e.interno empreendimento_interno, e.credenciado empreendimento_credenciado, r.situacao, r.agendamento, r.setor, r.informacoes, r.credenciado 
				from {0}tab_requerimento r, {0}tab_pessoa p, {0}tab_empreendimento e where r.interessado = p.id(+) and r.empreendimento = e.id(+) and r.id = :id", UsuarioCredenciado);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						requerimento.Id = id;
						requerimento.Tid = reader["tid"].ToString();
						requerimento.DataCadastro = Convert.ToDateTime(reader["data_criacao"]);
						requerimento.IsCredenciado = true;
						requerimento.CredenciadoId = Convert.ToInt32(reader["credenciado"]);

						if (reader["setor"] != null && !Convert.IsDBNull(reader["setor"]))
						{
							requerimento.SetorId = Convert.ToInt32(reader["setor"]);
						}

						if (reader["agendamento"] != null && !Convert.IsDBNull(reader["agendamento"]))
						{
							requerimento.AgendamentoVistoria = Convert.ToInt32(reader["agendamento"]);
						}

						if (reader["interessado"] != null && !Convert.IsDBNull(reader["interessado"]))
						{
							requerimento.Interessado.SelecaoTipo = (int)eExecutorTipo.Credenciado;
							requerimento.Interessado.Id = Convert.ToInt32(reader["interessado"]);
							requerimento.Interessado.Tipo = Convert.ToInt32(reader["interessado_tipo"]);

							if (reader["interessado_tipo"].ToString() == "1")
							{
								requerimento.Interessado.Fisica.Nome = reader["interessado_nome"].ToString();
								requerimento.Interessado.Fisica.CPF = reader["interessado_cpf_cnpj"].ToString();
							}
							else
							{
								requerimento.Interessado.Juridica.RazaoSocial = reader["interessado_nome"].ToString();
								requerimento.Interessado.Juridica.CNPJ = reader["interessado_cpf_cnpj"].ToString();
							}
						}

						if (reader["empreendimento"] != null && !Convert.IsDBNull(reader["empreendimento"]))
						{
							requerimento.Empreendimento.SelecaoTipo = (int)eExecutorTipo.Credenciado;
							requerimento.Empreendimento.Id = Convert.ToInt32(reader["empreendimento"]);
							requerimento.Empreendimento.Codigo = reader.GetValue<int>("empreendimento_codigo");
							requerimento.Empreendimento.Denominador = reader["denominador"].ToString();
							requerimento.Empreendimento.CNPJ = reader["empreendimento_cnpj"].ToString();
							requerimento.Empreendimento.InternoId = reader.GetValue<int?>("empreendimento_interno");
							requerimento.Empreendimento.CredenciadoId = reader.GetValue<int?>("empreendimento_credenciado");
						}

						if (reader["situacao"] != null && !Convert.IsDBNull(reader["situacao"]))
						{
							requerimento.SituacaoId = Convert.ToInt32(reader["situacao"]);
						}

						requerimento.Informacoes = reader["informacoes"].ToString();

					}

					reader.Close();
				}

				#endregion

				if (requerimento.Id <= 0 || simplificado)
				{
					return requerimento;
				}

				#region Atividades

				comando = bancoDeDados.CriarComando(@"select a.id, a.atividade, ta.situacao, a.tid from {0}tab_requerimento_atividade a, tab_atividade ta where ta.id = a.atividade and a.requerimento = :id", UsuarioCredenciado);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Atividade atividade;

					while (reader.Read())
					{
						atividade = new Atividade();
						atividade.Id = Convert.ToInt32(reader["atividade"]);
						atividade.SituacaoId = Convert.ToInt32(reader["situacao"]);
						atividade.IdRelacionamento = Convert.ToInt32(reader["id"]);
						atividade.Tid = reader["tid"].ToString();

						#region Atividades/Finalidades/Modelos
						comando = bancoDeDados.CriarComando(@"select a.id, a.finalidade, a.modelo, a.titulo_anterior_tipo, a.titulo_anterior_id,
						a.titulo_anterior_numero, a.modelo_anterior_id, a.modelo_anterior_nome, a.modelo_anterior_sigla, a.orgao_expedidor 
						from {0}tab_requerimento_ativ_finalida a where a.requerimento_ativ = :id", UsuarioCredenciado);

						comando.AdicionarParametroEntrada("id", atividade.IdRelacionamento, DbType.Int32);

						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							Finalidade fin;

							while (readerAux.Read())
							{
								fin = new Finalidade();

								fin.IdRelacionamento = Convert.ToInt32(readerAux["id"]);


								fin.OrgaoExpedidor = readerAux["orgao_expedidor"].ToString();

								if (readerAux["finalidade"] != DBNull.Value)
								{
									fin.Id = Convert.ToInt32(readerAux["finalidade"]);
								}

								if (readerAux["modelo"] != DBNull.Value)
								{
									fin.TituloModelo = Convert.ToInt32(readerAux["modelo"]);
								}

								if (readerAux["modelo_anterior_id"] != DBNull.Value)
								{
									fin.TituloModeloAnteriorId = Convert.ToInt32(readerAux["modelo_anterior_id"]);
								}

								fin.TituloModeloAnteriorTexto = readerAux["modelo_anterior_nome"].ToString();
								fin.TituloModeloAnteriorSigla = readerAux["modelo_anterior_sigla"].ToString();

								if (readerAux["titulo_anterior_tipo"] != DBNull.Value)
								{
									fin.TituloAnteriorTipo = Convert.ToInt32(readerAux["titulo_anterior_tipo"]);
								}

								if (readerAux["titulo_anterior_id"] != DBNull.Value)
								{
									fin.TituloAnteriorId = Convert.ToInt32(readerAux["titulo_anterior_id"]);
								}

								fin.TituloAnteriorNumero = readerAux["titulo_anterior_numero"].ToString();
								fin.EmitidoPorInterno = (fin.TituloAnteriorTipo != 3);
								atividade.Finalidades.Add(fin);
							}
							readerAux.Close();
						}
						#endregion

						requerimento.Atividades.Add(atividade);
					}

					reader.Close();
				}

				#endregion

				#region Responsáveis

				comando = bancoDeDados.CriarComando(@"select pr.id, pr.responsavel, pr.funcao, nvl(p.nome, p.razao_social) nome, pr.numero_art, 
				nvl(p.cpf, p.cnpj) cpf_cnpj, p.tipo from {0}tab_requerimento_responsavel pr, {0}tab_pessoa p where pr.responsavel = p.id and pr.requerimento = :requerimento", UsuarioCredenciado);

				comando.AdicionarParametroEntrada("requerimento", requerimento.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					ResponsavelTecnico responsavel;
					while (reader.Read())
					{
						responsavel = new ResponsavelTecnico();
						responsavel.IdRelacionamento = reader.GetValue<int>("id");
						responsavel.Id = reader.GetValue<int>("responsavel");
						responsavel.Funcao = reader.GetValue<int>("funcao");
						responsavel.CpfCnpj = reader.GetValue<string>("cpf_cnpj");
						responsavel.NomeRazao = reader.GetValue<string>("nome");
						responsavel.NumeroArt = reader.GetValue<string>("numero_art");

						if (reader.GetValue<int>("tipo") == PessoaTipo.JURIDICA)
						{
							comando = bancoDeDados.CriarComando(@"select pr.id, pr.pessoa, pr.representante, p.nome, p.cpf, p.tipo 
							from {0}tab_pessoa_representante pr, {0}tab_pessoa p where pr.representante = p.id and pr.pessoa = :pessoa", UsuarioCredenciado);

							comando.AdicionarParametroEntrada("pessoa", responsavel.Id, DbType.Int32);

							using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
							{
								Pessoa representante;
								responsavel.Representantes = new List<Pessoa>();
								while (readerAux.Read())
								{
									representante = new Pessoa();
									representante.Id = readerAux.GetValue<int>("representante");
									representante.Tipo = readerAux.GetValue<int>("tipo");
									representante.Fisica.Nome = readerAux.GetValue<string>("nome");
									representante.Fisica.CPF = readerAux.GetValue<string>("cpf");
									responsavel.Representantes.Add(representante);
								}
								readerAux.Close();
							}
						}
						requerimento.Responsaveis.Add(responsavel);
					}
					reader.Close();
				}

				/* Se a atividade associada ao requerimento for Barragem (id == 327), o responsável necessariamente
				 * deverá ser o RT que está fazendo o cadastro do requerimento. Não poderá haver outros RTs associados
				 * ao requerimento.
				 */
				#region Barragem

				if (requerimento.Atividades.FirstOrDefault(x => x.Id == 327) != null)
				{
					CredenciadoBus _busCredenciado = new CredenciadoBus(new CredenciadoValidar());
					CredenciadoPessoa usuarioLogado = _busCredenciado.Obter(User.EtramiteIdentity.FuncionarioId);
					usuarioLogado.Pessoa = _busCredenciado.ObterPessoaCredenciado(usuarioLogado.Pessoa.Id);

					if (requerimento.Responsaveis.Count() > 1 || requerimento.Responsaveis.Count() == 0 || requerimento.Responsaveis.FirstOrDefault(x => x.Id == usuarioLogado.Pessoa.Id) == null)
					{
						requerimento.Responsaveis = new List<ResponsavelTecnico>();

						ResponsavelTecnico rt = new ResponsavelTecnico();
						rt.IdRelacionamento = 0;
						rt.Id = usuarioLogado.Pessoa.Id;
						rt.Funcao = 1;  //Elaborador
						rt.CpfCnpj = usuarioLogado.Pessoa.CPFCNPJ;
						rt.NomeRazao = usuarioLogado.Pessoa.NomeRazaoSocial;
						rt.NumeroArt = string.Empty;

						requerimento.Responsaveis.Add(rt);
					}
				}

				#endregion Barragem

				#endregion
			}

			return requerimento;
		}

		internal Resultados<Requerimento> Filtrar(Filtro<RequerimentoListarFiltro> filtros, BancoDeDados banco = null)
		{
			Resultados<Requerimento> retorno = new Resultados<Requerimento>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{
				string comandtxt = string.Empty;
				string esquema = (string.IsNullOrEmpty(UsuarioCredenciado) ? "" : UsuarioCredenciado + ".");
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAnd("e.numero", "numero", filtros.Dados.Numero);
				comandtxt += comando.FiltroAnd("e.situacao", "situacao", filtros.Dados.Situacao);
				comandtxt += comando.FiltroAnd("e.credenciado", "credenciado", filtros.Dados.Credenciado);

				if (filtros.Dados.ProjetoDigitalSituacoes != null && filtros.Dados.ProjetoDigitalSituacoes.Count > 0)
				{
					comandtxt += String.Format(" and e.id in (select tp.requerimento from tab_projeto_digital tp where {1})", esquema, comando.AdicionarIn("", "tp.situacao", DbType.Int32, filtros.Dados.ProjetoDigitalSituacoes));
				}

				if (!ValidacoesGenericasBus.Cpf(filtros.Dados.InteressadoCpfCnpj) && !ValidacoesGenericasBus.Cnpj(filtros.Dados.InteressadoCpfCnpj))
				{
					if (!string.IsNullOrEmpty(filtros.Dados.InteressadoNomeRazao))
					{
						comandtxt += String.Format(" and e.interessado in (select p.id from {0}tab_pessoa p where upper(p.{1}) like upper(:nome_razao)||'%')", esquema, filtros.Dados.IsCpf ? "nome" : "razao_social");
						comando.AdicionarParametroEntrada("nome_razao", filtros.Dados.InteressadoNomeRazao, DbType.String);
					}
				}
				else
				{
					comandtxt += String.Format(" and e.interessado in (select p.id from {0}tab_pessoa p where p.{1} = :cpf_cnpj)", esquema, filtros.Dados.IsCpf ? "cpf" : "cnpj");
					comando.AdicionarParametroEntrada("cpf_cnpj", filtros.Dados.InteressadoCpfCnpj, DbType.String);
				}

				if (!string.IsNullOrEmpty(filtros.Dados.EmpreendimentoCnpj))
				{
					comandtxt += String.Format(" and e.empreendimento in (select p.id from {0}tab_empreendimento p where p.cnpj like :cnpj||'%')", esquema);
					comando.AdicionarParametroEntrada("cnpj", filtros.Dados.EmpreendimentoCnpj, DbType.String);
				}

				if (!string.IsNullOrEmpty(filtros.Dados.EmpreendimentoDenominador))
				{
					comandtxt += String.Format(" and e.empreendimento in (select p.id from {0}tab_empreendimento p where upper(p.denominador) like upper(:denominador)||'%')", esquema);
					comando.AdicionarParametroEntrada("denominador", filtros.Dados.EmpreendimentoDenominador, DbType.String);
				}

				if (filtros.Dados.IsRemoverTituloDeclaratorio)
				{
					comandtxt += String.Format(@" and e.id not in (select ra.requerimento from tab_requerimento_atividade ra, tab_requerimento_ativ_finalida rf
                                                                   where rf.requerimento_ativ = ra.id and rf.modelo in (select m.id from tab_titulo_modelo m where m.documento = 2))", esquema);
				}

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "numero", "interessado_nome_razao", "empreendimento_denominador", "situacao_texto" };

				if (filtros.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("numero");
				}

				#endregion

				#region Quantidade de registro do resultado

				comando.DbCommand.CommandText = String.Format("select count(*) from {0}tab_requerimento e where e.id > 0" + comandtxt, esquema);

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = String.Format(@"select tp.id projeto_id, tp.situacao projeto_situacao, e.id requerimento_id, e.numero, e.data_criacao, e.interessado interessado_id, p.tipo interessado_tipo, 
				nvl(p.nome,p.razao_social) interessado_nome_razao, nvl(p.cpf,p.cnpj) interessado_cpf_cnpj, nvl(p.rg,p.ie) interessado_rg_ie, 
				e.empreendimento empreendimento_id, r.denominador empreendimento_denominador, r.cnpj empreendimento_cnpj, e.situacao situacao_id, ls.texto situacao_texto from 
				{0}tab_requerimento e, {0}tab_pessoa p, {0}tab_empreendimento r, {0}tab_projeto_digital tp, {0}lov_requerimento_situacao ls where e.id = tp.requerimento and e.interessado = p.id(+)
				and e.empreendimento = r.id(+) and e.situacao = ls.id"
				+ comandtxt + DaHelper.Ordenar(colunas, ordenar), esquema);

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					Requerimento req;

					while (reader.Read())
					{
						req = new Requerimento();
						req.Id = Convert.ToInt32(reader["requerimento_id"]);
						req.DataCadastro = Convert.ToDateTime(reader["data_criacao"]);
						req.ProjetoDigital.Id = reader.GetValue<int>("projeto_id");
						req.ProjetoDigital.Situacao = reader.GetValue<int>("projeto_situacao");

						if (reader["interessado_id"] != null && !Convert.IsDBNull(reader["interessado_id"]))
						{
							req.Interessado.Id = Convert.ToInt32(reader["interessado_id"]);
							req.Interessado.Tipo = Convert.ToInt32(reader["interessado_tipo"]);

							if (reader["interessado_tipo"].ToString() == "1")
							{
								req.Interessado.Fisica.Nome = reader["interessado_nome_razao"].ToString();
							}
							else
							{
								req.Interessado.Juridica.RazaoSocial = reader["interessado_nome_razao"].ToString();
							}
						}

						if (reader["empreendimento_id"] != null && !Convert.IsDBNull(reader["empreendimento_id"]))
						{
							req.Empreendimento.Id = Convert.ToInt32(reader["empreendimento_id"]);
							req.Empreendimento.Denominador = reader["empreendimento_denominador"].ToString();
						}

						if (reader["situacao_id"] != null && !Convert.IsDBNull(reader["situacao_id"]))
						{
							req.SituacaoId = Convert.ToInt32(reader["situacao_id"]);
						}

						retorno.Itens.Add(req);
					}

					reader.Close();

					#endregion
				}
			}

			return retorno;
		}

		internal List<Roteiro> ObterRequerimentoRoteirosHistorico(int requerimento, int situacao, BancoDeDados banco = null)
		{
			List<Roteiro> roteiros = new List<Roteiro>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{
				Comando comando;

				comando = bancoDeDados.CriarComando(@"select r.roteiro_id roteiro, r.nome roteiro_nome, r.versao, a.atividade_id atividade, ha.atividade atividade_nome, r.tid
				from {0}hst_roteiro r, {0}hst_roteiro_atividades a, {0}hst_roteiro_modelos m, {0}hst_atividade ha, (select lf.codigo finalidade, fa.modelo_id, a.atividade_id
				from {0}hst_requerimento_atividade a, {0}hst_requerimento_ativ_finalida fa, {0}lov_titulo_finalidade lf  where a.id = fa.id_hst and fa.finalidade = lf.id
				and a.id_hst = (select max(h.id) from {0}hst_requerimento h where h.requerimento_id = :requerimento)) x where r.id = a.id_hst and r.id = m.id_hst
				and a.atividade_id = x.atividade_id and bitand(r.finalidade, x.finalidade) > 0 and m.modelo_id = x.modelo_id and a.atividade_tid = ha.tid
				and a.atividade_id = ha.atividade_id and r.id in (select max(h.id) from {0}hst_roteiro h, {0}hst_roteiro_atividades aa, {0}hst_roteiro_modelos mm
				where h.id = aa.id_hst and h.id = mm.id_hst and aa.atividade_id = x.atividade_id and mm.modelo_id = x.modelo_id and bitand(h.finalidade, x.finalidade) > 0 
				and h.data_execucao <= (select max(hr.data_execucao) from {0}hst_requerimento hr where hr.requerimento_id = :requerimento)) order by r.roteiro_id, a.atividade_id", UsuarioCredenciado);

				comando.AdicionarParametroEntrada("requerimento", requerimento, DbType.Int32);

				#region Buscar todas as atividades que estão configuradas com um roteiro
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						roteiros.Add(new Roteiro()
						{
							Id = Convert.ToInt32(reader["roteiro"]),
							VersaoAtual = Convert.ToInt32(reader["versao"]),
							Nome = reader["roteiro_nome"].ToString(),
							AtividadeId = Convert.ToInt32(reader["atividade"]),
							AtividadeTexto = reader["atividade_nome"].ToString(),
							Tid = reader["tid"].ToString()
						});
					}
				}
				#endregion

				comando = bancoDeDados.CriarComando(@"
									select ta.id atividade_id, ta.atividade atividade_nome, r.roteiro_id roteiro, r.versao, r.nome roteiro_nome, r.roteiro_tid
									  from {0}tab_requerimento_atividade tra,
										   {0}tab_atividade              ta", UsuarioCredenciado);

				comando.DbCommand.CommandText += String.Format(
							@", (select hr.roteiro_id, hr.tid, hr.versao, hr.numero, hr.nome, hr.tid roteiro_tid
									from {0}tab_checagem_roteiro t, {0}hst_roteiro hr
								where t.roteiro_tid = hr.tid
									and t.roteiro = hr.roteiro_id
									and t.checagem = (select t.checagem from {0}tab_protocolo t where t.requerimento = :requerimento) {1}) r 
									where tra.atividade = ta.id and tra.requerimento = :requerimento {2}",
							UsuarioCredenciado, comando.AdicionarNotIn("and", "t.roteiro", DbType.Int32, roteiros.Select(x => x.Id).ToList()),
							comando.AdicionarNotIn("and", "tra.atividade", DbType.Int32, roteiros.Select(x => x.AtividadeId).ToList()));

				comando.AdicionarParametroEntrada("requerimento", requerimento, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						roteiros.Add(new Roteiro()
						{
							Id = Convert.ToInt32(reader["roteiro"]),
							VersaoAtual = Convert.ToInt32(reader["versao"]),
							Nome = reader["roteiro_nome"].ToString(),
							AtividadeId = Convert.ToInt32(reader["atividade_id"]),
							AtividadeTexto = reader["atividade_nome"].ToString(),
							Tid = reader["roteiro_tid"].ToString()
						});
					}
				}
			}
			return roteiros;
		}

		public List<Pessoa> ObterPessoas(int requerimento)
		{
			List<Pessoa> retorno = new List<Pessoa>();
			List<int> ids = new List<int>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{
				#region Pessoas relacionadas com o Projeto Digital

				Comando comando = bancoDeDados.CriarComando(@"select r.interessado id from {0}tab_requerimento r where r.id = :requerimento union all
				select er.responsavel id from {0}tab_requerimento r, {0}tab_empreendimento_responsavel er where r.id = :requerimento and r.empreendimento = er.empreendimento union all 
				select rr.responsavel id from {0}tab_requerimento_responsavel rr where rr.requerimento = :requerimento", UsuarioCredenciado);

				comando.AdicionarParametroEntrada("requerimento", requerimento, DbType.Int32);

				ids = bancoDeDados.ExecutarList<int>(comando);

				#region Responsáveis/Representantes

				ids = ids.Where(x => x > 0).ToList();
				List<string> conj = new List<string>();

				if (ids != null && ids.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"select p.tipo Tipo, p.id Id, p.interno InternoId, nvl(p.nome, p.razao_social) NomeRazaoSocial,
					nvl(p.cpf, p.cnpj) CPFCNPJ, (select c.conjuge||'@'||c.pessoa from {0}tab_pessoa_conjuge c where c.pessoa = p.id or c.conjuge = p.id) conjuge 
					from {0}tab_pessoa p ", UsuarioCredenciado);
					comando.DbCommand.CommandText += comando.AdicionarIn("where", "p.id", DbType.Int32, ids);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						Pessoa pes;
						while (reader.Read())
						{
							pes = new Pessoa();
							pes.Id = reader.GetValue<int>("Id");
							pes.Tipo = reader.GetValue<int>("Tipo");
							pes.InternoId = reader.GetValue<int>("InternoId");
							pes.NomeRazaoSocial = reader.GetValue<string>("NomeRazaoSocial");
							pes.CPFCNPJ = reader.GetValue<string>("CPFCNPJ");
							if (!string.IsNullOrEmpty(reader.GetValue<string>("conjuge")))
							{
								conj = reader.GetValue<string>("conjuge").Split('@').ToList();
								pes.Fisica.ConjugeId = (Convert.ToInt32(conj[0]) == pes.Id) ? Convert.ToInt32(conj[1]) : Convert.ToInt32(conj[0]);
							}
							retorno.Add(pes);
						}

						reader.Close();
					}
				}

				if (retorno.Count > 0)
				{
					if (retorno.Exists(x => x.IsJuridica))
					{
						//Representantes
						comando = bancoDeDados.CriarComando(@"select pc.pessoa, p.tipo Tipo, p.id Id, p.interno InternoId, nvl(p.nome, p.razao_social) NomeRazaoSocial,
						nvl(p.cpf, p.cnpj) CPFCNPJ, (select c.conjuge||'@'||c.pessoa from {0}tab_pessoa_conjuge c where c.pessoa = p.id or c.conjuge = p.id) conjuge 
						from {0}tab_pessoa p, {0}tab_pessoa_representante pc where p.id = pc.representante", UsuarioCredenciado);

						comando.DbCommand.CommandText += comando.AdicionarIn("and", "pc.pessoa", DbType.Int32, retorno.Where(x => x.IsJuridica).Select(x => x.Id).ToList());

						using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
						{
							Pessoa pes;
							while (reader.Read())
							{
								pes = new Pessoa();
								pes.Id = reader.GetValue<int>("Id");
								pes.Tipo = reader.GetValue<int>("Tipo");
								pes.InternoId = reader.GetValue<int>("InternoId");
								pes.IdRelacionamento = reader.GetValue<int>("pessoa");
								pes.NomeRazaoSocial = reader.GetValue<string>("NomeRazaoSocial");
								pes.CPFCNPJ = reader.GetValue<string>("CPFCNPJ");
								if (!string.IsNullOrEmpty(reader.GetValue<string>("conjuge")))
								{
									conj = reader.GetValue<string>("conjuge").Split('@').ToList();
									pes.Fisica.ConjugeId = (Convert.ToInt32(conj[0]) == pes.Id) ? Convert.ToInt32(conj[1]) : Convert.ToInt32(conj[0]);
								}
								retorno.Add(pes);
							}

							reader.Close();
						}

						retorno.Where(x => x.IsJuridica).ToList().ForEach(x => {
							x.Juridica.Representantes = retorno.Where(y => y.IdRelacionamento == x.Id).ToList();
						});
					}

					if (retorno.Exists(x => x.IsFisica))
					{
						//Conjuges
						comando = bancoDeDados.CriarComando(@"select p.tipo Tipo, p.id Id, p.interno InternoId, nvl(p.nome, p.razao_social) NomeRazaoSocial,
						nvl(p.cpf, p.cnpj) CPFCNPJ, (select c.conjuge||'@'||c.pessoa from {0}tab_pessoa_conjuge c where c.pessoa = p.id or c.conjuge = p.id) conjuge 
						from {0}tab_pessoa p, {0}tab_pessoa_conjuge pc where p.id = pc.conjuge", UsuarioCredenciado);

						comando.DbCommand.CommandText += comando.AdicionarIn("and", "pc.pessoa", DbType.Int32, retorno.Where(x => x.IsFisica).Select(x => x.Id).ToList());

						using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
						{
							Pessoa pes;
							while (reader.Read())
							{
								pes = new Pessoa();
								pes.Id = reader.GetValue<int>("Id");
								pes.Tipo = reader.GetValue<int>("Tipo");
								pes.InternoId = reader.GetValue<int>("InternoId");
								pes.NomeRazaoSocial = reader.GetValue<string>("NomeRazaoSocial");
								pes.CPFCNPJ = reader.GetValue<string>("CPFCNPJ");
								if (!string.IsNullOrEmpty(reader.GetValue<string>("conjuge")))
								{
									conj = reader.GetValue<string>("conjuge").Split('@').ToList();
									pes.Fisica.ConjugeId = (Convert.ToInt32(conj[0]) == pes.Id) ? Convert.ToInt32(conj[1]) : Convert.ToInt32(conj[0]);
								}
								retorno.Add(pes);
							}

							reader.Close();
						}

						//Obter CPF Conjuges
						comando = bancoDeDados.CriarComando(@"select p.id, p.cpf from {0}tab_pessoa p ", UsuarioCredenciado);
						comando.DbCommand.CommandText += comando.AdicionarIn("where", "p.id", DbType.Int32, retorno.Where(x => x.Fisica.ConjugeId > 0).Select(x => x.Fisica.ConjugeId).ToList());

						using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
						{
							Pessoa pes;

							while (reader.Read())
							{
								pes = retorno.FirstOrDefault(x => x.Fisica.ConjugeId == reader.GetValue<int>("id"));

								if (pes != null)
								{
									pes.Fisica.ConjugeCPF = reader.GetValue<string>("cpf");
								}
							}

							reader.Close();
						}
					}
				}

				#endregion

				#endregion
			}

			retorno = retorno.GroupBy(x => x.Id).Select(y => new Pessoa
			{
				Id = y.First().Id,
				Tipo = y.First().Tipo,
				InternoId = y.First().InternoId,
				NomeRazaoSocial = y.First().NomeRazaoSocial,
				CPFCNPJ = y.First().CPFCNPJ,
				Fisica = y.First().Fisica,
				Juridica = y.First().Juridica,
			}).ToList();

			return retorno;
		}

		public List<int> ObterResponsavelTecnico(int requerimento)
		{
			List<int> responsaveis = new List<int>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{
				/*Comando comando = bancoDeDados.CriarComando(@"SELECT P.CREDENCIADO FROM TAB_REQUERIMENTO_RESPONSAVEL R
																	INNER JOIN TAB_PESSOA P ON R.RESPONSAVEL = P.ID
																WHERE R.REQUERIMENTO = :requerimento", UsuarioCredenciado);
				comando.AdicionarParametroEntrada("requerimento", requerimento, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						responsaveis.Add(reader.GetValue<int>("CREDENCIADO"));
					}

					reader.Close();
				}*/

				Comando comando = bancoDeDados.CriarComando(@"SELECT * FROM TAB_CREDENCIADO C
														INNER JOIN TAB_PESSOA P ON P.ID = C.PESSOA 
													WHERE P.CPF IN (
														SELECT P.CPF FROM TAB_REQUERIMENTO_RESPONSAVEL R INNER JOIN TAB_PESSOA P ON R.RESPONSAVEL = P.ID
															WHERE R.REQUERIMENTO = :requerimento )", UsuarioCredenciado);
				comando.AdicionarParametroEntrada("requerimento", requerimento, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						responsaveis.Add(reader.GetValue<int>("CREDENCIADO"));
					}

					reader.Close();
				}

				comando = bancoDeDados.CriarComando(@"SELECT * FROM TAB_CREDENCIADO C
														INNER JOIN TAB_PESSOA P ON P.ID = C.PESSOA 
													WHERE P.CNPJ IN (
														SELECT P.CPF FROM TAB_REQUERIMENTO_RESPONSAVEL R INNER JOIN TAB_PESSOA P ON R.RESPONSAVEL = P.ID
															WHERE R.REQUERIMENTO = :requerimento )", UsuarioCredenciado);
				comando.AdicionarParametroEntrada("requerimento", requerimento, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						responsaveis.Add(reader.GetValue<int>("CREDENCIADO"));
					}

					reader.Close();
				}

				return responsaveis;
			}
		}
		public List<String> ObterAtividadesEmpreendimentoObrigatorio(int requerimentoId)
		{
			List<String> atividades = new List<String>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select a.atividade from tab_atividade a where a.empreendimento_obrigatorio = 1
				and a.id in (select ra.atividade from tab_requerimento_atividade ra where ra.requerimento = :requerimento)", UsuarioCredenciado);
				comando.AdicionarParametroEntrada("requerimento", requerimentoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						atividades.Add(reader.GetValue<String>("atividade"));
					}

					reader.Close();
				}

				return atividades;
			}
		}

		#endregion

		#region Validações

		internal bool ValidarModeloAnteriorNumero(int tituloAnteriorId, int tituloAnteriorTipo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from {0}tab_requerimento_ativ_finalida t where t.titulo_anterior_id = :tituloAnteriorId and t.titulo_anterior_tipo = :tituloAnteriorTipo", UsuarioCredenciado);

				comando.AdicionarParametroEntrada("tituloAnteriorId", tituloAnteriorId, DbType.Int32);
				comando.AdicionarParametroEntrada("tituloAnteriorTipo", tituloAnteriorTipo, DbType.Int32);
				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		public bool ValidarVersaoAtual(int id, string tid)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from {0}tab_requerimento r where r.id = :id and r.tid= :tid", UsuarioCredenciado);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", tid, DbType.String);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		internal bool ValidarNumeroSemAnoExistente(string numero, int modeloId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from {0}tab_titulo tt, {0}tab_titulo_numero ttn
															where tt.id = ttn.titulo and tt.situacao in (3, 6) -- Concluído e Prorrogado
															and ttn.numero = :numero and ttn.modelo = :modelo", String.Empty);

				comando.AdicionarParametroEntrada("numero", numero, DbType.String);
				comando.AdicionarParametroEntrada("modelo", modeloId, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		public bool Existe(int requerimentoId)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from {0}tab_requerimento r where r.id = :id", UsuarioCredenciado);
				comando.AdicionarParametroEntrada("id", requerimentoId, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		internal int PossuiSolicitacaoCARValidaSuspensaPendente(int requerimentoId)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id from tab_car_solicitacao t where t.situacao in (2, 4, 6) and t.requerimento = :id", UsuarioCredenciado);
				comando.AdicionarParametroEntrada("id", requerimentoId, DbType.Int32);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));
			}
		}

		internal bool PossuiSolicitacaoCAREnviadaSicar(int requerimentoId)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(t.id) from tab_car_solicitacao t, tab_controle_sicar c where t.id=c.solicitacao_car and t.situacao = 2 and c.situacao_envio = 6 and t.requerimento = :id", UsuarioCredenciado);
				comando.AdicionarParametroEntrada("id", requerimentoId, DbType.Int32);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0;
			}
		}


		internal bool PossuiSolicitacaoCAREmProcessamento(int requerimentoId)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(t.id) from tab_car_solicitacao t, tab_controle_sicar c where t.id=c.solicitacao_car and t.situacao in ( 1, 6 ) and c.situacao_envio in ( 1, 2, 3, 5 ) and t.requerimento = :id", UsuarioCredenciado);
				comando.AdicionarParametroEntrada("id", requerimentoId, DbType.Int32);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0;
			}
		}

		internal bool RequerimentoDeclaratorio(int requerimentoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) 
				from tab_requerimento_atividade ra, tab_requerimento_ativ_finalida rf
				where rf.requerimento_ativ = ra.id and rf.modelo in (select m.id from tab_titulo_modelo m where m.documento = 2)
				and ra.requerimento = :requerimento");

				comando.AdicionarParametroEntrada("requerimento", requerimentoId, DbType.Int32);
				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		internal bool VerificarRequerimentoPossuiModelo(int modeloId, int requerimento)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(ta.id) qtd from {0}tab_requerimento_atividade ta, {0}tab_requerimento_ativ_finalida taf
				where ta.requerimento = :id and ta.id = taf.requerimento_ativ and taf.modelo = :modelo", UsuarioCredenciado);

				comando.AdicionarParametroEntrada("id", requerimento, DbType.Int32);
				comando.AdicionarParametroEntrada("modelo", modeloId, DbType.Int32);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0;
			}
		}

		#endregion
	}
}