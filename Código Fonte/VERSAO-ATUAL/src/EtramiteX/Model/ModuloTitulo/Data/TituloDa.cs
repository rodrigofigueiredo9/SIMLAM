using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Etx.ModuloArquivo;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicenca;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Entities;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicensa.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Data
{
	public class TituloDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		CondicionanteDa _daCondicionante = new CondicionanteDa();
		internal Historico Historico { get { return _historico; } }
		private static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}
		private string EsquemaBanco { get; set; }
		Consulta _consulta = new Consulta();
		internal Consulta Consulta { get { return _consulta; } }

		#endregion

		public TituloDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(Titulo titulo, BancoDeDados banco)
		{
			if (titulo == null)
			{
				throw new Exception("O Título é nulo.");
			}

			if (titulo.Id <= 0)
			{
				Criar(titulo, banco);
			}
			else
			{
				Editar(titulo, banco);
			}
		}

		internal int? Criar(Titulo titulo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Título

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"
                    insert into {0}tab_titulo t
                      (id,
                       autor,
                       setor,
                       modelo,
                       situacao,
                       local_emissao,
                       protocolo,
                       empreendimento,
                       data_criacao,
                       arquivo,
                       tid,
                       requerimento,
                       credenciado,
						data_situacao)
                    values
                      (seq_titulo.nextval,
                       :autor,
                       :setor,
                       :modelo,
                       :situacao,
                       :local_emissao,
                       :protocolo,
                       :empreendimento,
                       sysdate,
                       :arquivo,
                       :tid,
                       :requerimento,
                       :credenciado,
						:data_situacao)
                    returning t.id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("autor", User.FuncionarioId, DbType.Int32);
				comando.AdicionarParametroEntrada("setor", titulo.Setor.Id == 0 ? null : titulo.Setor.Id.ToString(), DbType.Int32);
				comando.AdicionarParametroEntrada("modelo", titulo.Modelo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", titulo.Situacao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("local_emissao", titulo.LocalEmissao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("protocolo", titulo.Protocolo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("empreendimento", titulo.EmpreendimentoId.GetValueOrDefault() > 0 ? titulo.EmpreendimentoId : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("arquivo", (titulo.ArquivoPdf != null && titulo.ArquivoPdf.Id.GetValueOrDefault() > 0) ? titulo.ArquivoPdf.Id : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("requerimento", titulo.RequerimetoId, DbType.Int32);
				comando.AdicionarParametroEntrada("credenciado", titulo.CredenciadoId, DbType.Int32);
				comando.AdicionarParametroEntrada("data_situacao", titulo.RequerimetoId.GetValueOrDefault() > 0 ? DateTime.Now : (object)DBNull.Value, DbType.DateTime);
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				titulo.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#endregion

				#region Número

				if (titulo.Numero.Inteiro > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}tab_titulo_numero (id, titulo, numero, ano, modelo, tid) 
					values (seq_titulo_numero.nextval, :titulo, :numero, :ano, :modelo, :tid)", EsquemaBanco);

					comando.AdicionarParametroEntrada("titulo", titulo.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("numero", titulo.Numero.Inteiro, DbType.Int32);
					comando.AdicionarParametroEntrada("ano", titulo.Numero.Ano, DbType.Int32);
					comando.AdicionarParametroEntrada("modelo", titulo.Modelo.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#region Setores

				if (titulo.Setores != null && titulo.Setores.Count > 0)
				{
					foreach (Setor item in titulo.Setores)
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}tab_titulo_setores s (id, titulo, setor, tid)
						values ({0}seq_titulo_setores.nextval, :titulo, :setor, :tid)", EsquemaBanco);

						comando.AdicionarParametroEntrada("titulo", titulo.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("setor", item.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Assinantes

				if (titulo.Assinantes != null && titulo.Assinantes.Count > 0)
				{
					foreach (TituloAssinante item in titulo.Assinantes)
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}tab_titulo_assinantes s (id, titulo, funcionario, cargo, tid)
						values ({0}seq_titulo_assinantes.nextval, :titulo, :funcionario, :cargo, :tid)", EsquemaBanco);

						comando.AdicionarParametroEntrada("titulo", titulo.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("funcionario", item.FuncionarioId, DbType.Int32);
						comando.AdicionarParametroEntrada("cargo", item.FuncionarioCargoId, DbType.Int32);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Associados

				if (titulo.Associados != null && titulo.Associados.Count > 0)
				{
					foreach (Titulo item in titulo.Associados)
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}tab_titulo_associados s (id, titulo, associado_id, associado_tid, tid)
						values ({0}seq_titulo_associados.nextval, :titulo, :associado_id, (select t.tid from {0}tab_titulo t where t.id = :associado_id), :tid)", EsquemaBanco);

						comando.AdicionarParametroEntrada("titulo", titulo.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("associado_id", item.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Atividades

				if (titulo.Atividades != null && titulo.Atividades.Count > 0)
				{
					foreach (Atividade item in titulo.Atividades)
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}tab_titulo_atividades s (id, titulo, atividade, protocolo, tid)
						values ({0}seq_titulo_atividades.nextval, :titulo, :atividade, :protocolo, :tid)", EsquemaBanco);

						comando.AdicionarParametroEntrada("titulo", titulo.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("atividade", item.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("protocolo", item.Protocolo.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Exploracoes

				if (titulo.Exploracoes != null && titulo.Exploracoes.Count > 0)
				{
					foreach (TituloExploracaoFlorestal item in titulo.Exploracoes)
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}tab_titulo_exp_florestal s (id, titulo, exploracao_florestal)
						values ({0}seq_titulo_exp_florestal.nextval, :titulo, :exploracao_florestal)
						returning s.id into :id", EsquemaBanco);

						comando.AdicionarParametroEntrada("titulo", titulo.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("exploracao_florestal", item.ExploracaoFlorestalId, DbType.Int32);
						comando.AdicionarParametroSaida("id", DbType.Int32);

						bancoDeDados.ExecutarNonQuery(comando);

						item.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

						if (item.TituloExploracaoFlorestalExploracaoList != null && item.TituloExploracaoFlorestalExploracaoList.Count > 0)
						{
							foreach (TituloExploracaoFlorestalExploracao itemDetalhe in item.TituloExploracaoFlorestalExploracaoList)
							{
								comando = bancoDeDados.CriarComando(@"insert into {0}tab_titulo_exp_flor_exp s (id, titulo_exploracao_florestal, exp_florestal_exploracao)
								values ({0}seq_titulo_exp_flor_exp.nextval, :titulo_exploracao_florestal, :exp_florestal_exploracao)", EsquemaBanco);

								comando.AdicionarParametroEntrada("titulo_exploracao_florestal", item.Id, DbType.Int32);
								comando.AdicionarParametroEntrada("exp_florestal_exploracao", itemDetalhe.ExploracaoFlorestalExploracaoId, DbType.Int32);

								bancoDeDados.ExecutarNonQuery(comando);
							}
						}
					}
				}

				#endregion

				#region Destinatário de e-mails

				if (titulo.DestinatarioEmails != null && titulo.DestinatarioEmails.Count > 0)
				{
					foreach (DestinatarioEmail item in titulo.DestinatarioEmails)
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}tab_titulo_pessoas s (id, titulo, pessoa, tipo, tid)
						values ({0}seq_titulo_pessoas.nextval, :titulo, :pessoa, 1, :tid)", EsquemaBanco);

						comando.AdicionarParametroEntrada("titulo", titulo.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("pessoa", item.PessoaId, DbType.Int32);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Representante

				if (titulo.Representante.Id > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}tab_titulo_pessoas s (id, titulo, pessoa, tipo, tid)
						values ({0}seq_titulo_pessoas.nextval, :titulo, :pessoa, 2, :tid)", EsquemaBanco);

					comando.AdicionarParametroEntrada("titulo", titulo.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("pessoa", titulo.Representante.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#region Condicionantes

				if (titulo.Condicionantes != null && titulo.Condicionantes.Count > 0)
				{
					for (int i = 0; i < titulo.Condicionantes.Count; i++)
					{
						titulo.Condicionantes[i].tituloId = titulo.Id;
						titulo.Condicionantes[i].Ordem = i + 1;
						_daCondicionante.Salvar(titulo.Condicionantes[i], bancoDeDados);
					}
				}

				#endregion

				#region Arquivos

				if (titulo.Anexos != null && titulo.Anexos.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}tab_titulo_arquivo a (id, titulo, arquivo, ordem, descricao, tid) 
					values ({0}seq_titulo_arquivo.nextval, :titulo, :arquivo, :ordem, :descricao, :tid)", EsquemaBanco);

					comando.AdicionarParametroEntrada("titulo", titulo.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("arquivo", DbType.Int32);
					comando.AdicionarParametroEntrada("ordem", DbType.Int32);
					comando.AdicionarParametroEntrada("descricao", DbType.String, 100);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					foreach (Anexo item in titulo.Anexos)
					{
						comando.SetarValorParametro("arquivo", item.Arquivo.Id);
						comando.SetarValorParametro("ordem", item.Ordem);
						comando.SetarValorParametro("descricao", item.Descricao);
						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Histórico

				Historico.Gerar(titulo.Id, eHistoricoArtefato.titulo, eHistoricoAcao.criar, bancoDeDados);
				Consulta.Gerar(titulo.Id, eHistoricoArtefato.titulo, bancoDeDados);

				#endregion

				bancoDeDados.Commit();

				return titulo.Id;
			}
		}

		internal void Editar(Titulo titulo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Título

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"
                    update {0}tab_titulo t
                       set t.autor             = :autor,
                           t.setor             = :setor,
                           t.modelo            = :modelo,
                           t.situacao          = :situacao,
                           t.local_emissao     = :local_emissao,
                           t.protocolo         = :protocolo,
                           t.empreendimento    = :empreendimento,
                           t.prazo             = :prazo,
                           t.dias_prorrogados  = :dias_prorrogados,
                           t.data_emissao      = :data_emissao,
                           t.data_assinatura   = :data_assinatura,
                           t.data_vencimento   = :data_vencimento,
                           t.data_encerramento = :data_encerramento,
                           t.arquivo           = :arquivo,
                           t.tid               = :tid,
                           t.requerimento      = :requerimento,
                           t.credenciado       = :credenciado
                     where t.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", titulo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("autor", User.FuncionarioId, DbType.Int32);
				comando.AdicionarParametroEntrada("setor", titulo.Setor.Id == 0 ? null : titulo.Setor.Id.ToString(), DbType.Int32);
				comando.AdicionarParametroEntrada("modelo", titulo.Modelo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", titulo.Situacao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("local_emissao", titulo.LocalEmissao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("protocolo", titulo.Protocolo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("empreendimento", titulo.EmpreendimentoId.GetValueOrDefault() > 0 ? titulo.EmpreendimentoId : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("prazo", (titulo.Prazo.HasValue) ? titulo.Prazo : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("dias_prorrogados", (titulo.DiasProrrogados.HasValue) ? titulo.DiasProrrogados : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("data_emissao", titulo.DataEmissao.IsValido ? titulo.DataEmissao.Data : (object)DBNull.Value, DbType.DateTime);
				comando.AdicionarParametroEntrada("data_assinatura", titulo.DataAssinatura.IsValido ? titulo.DataAssinatura.Data : (object)DBNull.Value, DbType.DateTime);
				comando.AdicionarParametroEntrada("data_vencimento", titulo.DataVencimento.IsValido ? titulo.DataVencimento.Data : (object)DBNull.Value, DbType.DateTime);
				comando.AdicionarParametroEntrada("data_encerramento", titulo.DataEncerramento.IsValido ? titulo.DataEncerramento.Data : (object)DBNull.Value, DbType.DateTime);
				comando.AdicionarParametroEntrada("arquivo", (titulo.ArquivoPdf != null && titulo.ArquivoPdf.Id.GetValueOrDefault() > 0) ? titulo.ArquivoPdf.Id : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("requerimento", titulo.RequerimetoId, DbType.Int32);
				comando.AdicionarParametroEntrada("credenciado", titulo.CredenciadoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Número

				if (titulo.Numero.Inteiro > 0)
				{
					comando = bancoDeDados.CriarComando(@"update {0}tab_titulo_numero tn set tn.numero = :numero, tn.ano = :ano, tn.modelo = :modelo, tn.tid = :tid 
					where tn.titulo = :titulo", EsquemaBanco);

					comando.AdicionarParametroEntrada("titulo", titulo.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("numero", titulo.Numero.Inteiro, DbType.Int32);
					comando.AdicionarParametroEntrada("ano", titulo.Numero.Ano, DbType.Int32);
					comando.AdicionarParametroEntrada("modelo", titulo.Modelo.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#region Limpar os dados do banco

				//Setores
				comando = bancoDeDados.CriarComando(@"delete {0}tab_titulo_setores s ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where s.titulo = :titulo{0}",
				comando.AdicionarNotIn("and", "s.id", DbType.Int32, titulo.Setores.Select(x => x.IdRelacao).ToList()));
				comando.AdicionarParametroEntrada("titulo", titulo.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				//Assinantes
				comando = bancoDeDados.CriarComando(@"delete {0}tab_titulo_assinantes a ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where a.titulo = :titulo{0}",
				comando.AdicionarNotIn("and", "a.id", DbType.Int32, titulo.Assinantes.Select(x => x.Id).ToList()));
				comando.AdicionarParametroEntrada("titulo", titulo.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				//Associados
				comando = bancoDeDados.CriarComando(@"delete {0}tab_titulo_associados a ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where a.titulo = :titulo{0}",
				comando.AdicionarNotIn("and", "a.id", DbType.Int32, titulo.Associados.Select(x => x.IdRelacionamento).ToList()));
				comando.AdicionarParametroEntrada("titulo", titulo.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				//Atividades
				comando = bancoDeDados.CriarComando(@"delete {0}tab_titulo_atividades a ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where a.titulo = :titulo{0}",
				comando.AdicionarNotIn("and", "a.id", DbType.Int32, titulo.Atividades.Select(x => x.IdRelacionamento).ToList()));
				comando.AdicionarParametroEntrada("titulo", titulo.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				//Exploracoes
				comando = bancoDeDados.CriarComando(@"delete {0}tab_integracao_sinaflor a
							where exists(select 1 from {0}tab_titulo_exp_florestal e
								where e.titulo = :titulo and e.id = a.titulo_exp_florestal)", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", titulo.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				comando = bancoDeDados.CriarComando(@"delete {0}tab_titulo_exp_flor_exp a
							where exists(select 1 from {0}tab_titulo_exp_florestal e
								where e.titulo = :titulo and e.id = a.titulo_exploracao_florestal)", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", titulo.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				comando = bancoDeDados.CriarComando(@"delete {0}tab_titulo_exp_florestal a ", EsquemaBanco);
				comando.DbCommand.CommandText += "where a.titulo = :titulo";
				comando.AdicionarParametroEntrada("titulo", titulo.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				//Destinatário de e-mails
				comando = bancoDeDados.CriarComando(@"delete {0}tab_titulo_pessoas a ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where a.tipo = 1 and a.titulo = :titulo{0}",
				comando.AdicionarNotIn("and", "a.id", DbType.Int32, titulo.DestinatarioEmails.Select(x => x.IdRelacionamento).ToList()));
				comando.AdicionarParametroEntrada("titulo", titulo.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				//Representante
				comando = bancoDeDados.CriarComando(@"delete {0}tab_titulo_pessoas a where a.tipo = 2 and a.titulo = :titulo and a.id != :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", titulo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("id", titulo.Representante.IdRelacionamento, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				//Condicionantes
				comando = bancoDeDados.CriarComando(@"delete {0}tab_titulo_condicionantes c ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where c.titulo = :titulo{0}",
				comando.AdicionarNotIn("and", "c.id", DbType.Int32, titulo.Condicionantes.Select(x => x.Id).ToList()));
				comando.AdicionarParametroEntrada("titulo", titulo.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				//Arquivos
				comando = bancoDeDados.CriarComando("delete from {0}tab_titulo_arquivo ra ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where ra.titulo = :titulo{0}",
					comando.AdicionarNotIn("and", "ra.id", DbType.Int32, titulo.Anexos.Select(x => x.Id).ToList()));
				comando.AdicionarParametroEntrada("titulo", titulo.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Setores

				if (titulo.Setores != null && titulo.Setores.Count > 0)
				{
					foreach (Setor item in titulo.Setores)
					{
						if (item.IdRelacao > 0)
						{
							comando = bancoDeDados.CriarComando(@"update {0}tab_titulo_setores e set e.titulo = :titulo, e.setor = :setor, 
							e.tid = :tid where e.id = :id", EsquemaBanco);
							comando.AdicionarParametroEntrada("id", item.IdRelacao, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}tab_titulo_setores s (id, titulo, setor, tid)
							values ({0}seq_titulo_setores.nextval, :titulo, :setor, :tid)", EsquemaBanco);
						}

						comando.AdicionarParametroEntrada("titulo", titulo.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("setor", item.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Assinantes

				if (titulo.Assinantes != null && titulo.Assinantes.Count > 0)
				{
					foreach (TituloAssinante item in titulo.Assinantes)
					{
						if (item.Id > 0)
						{
							comando = bancoDeDados.CriarComando(@"update {0}tab_titulo_assinantes e set e.titulo = :titulo, e.funcionario = :funcionario, e.cargo = :cargo,
							e.tid = :tid where e.id = :id", EsquemaBanco);
							comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}tab_titulo_assinantes s (id, titulo, funcionario, cargo, tid)
							values ({0}seq_titulo_assinantes.nextval, :titulo, :funcionario, :cargo, :tid)", EsquemaBanco);
						}

						comando.AdicionarParametroEntrada("titulo", titulo.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("funcionario", item.FuncionarioId, DbType.Int32);
						comando.AdicionarParametroEntrada("cargo", item.FuncionarioCargoId, DbType.Int32);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Associados

				if (titulo.Associados != null && titulo.Associados.Count > 0)
				{
					foreach (Titulo item in titulo.Associados)
					{
						if (item.IdRelacionamento > 0)
						{

							comando = bancoDeDados.CriarComando(@"update {0}tab_titulo_associados e set e.titulo = :titulo, e.associado_id = :associado_id,
							e.associado_tid = (select t.tid from {0}tab_titulo t where t.id = :associado_id), e.tid = :tid where e.id = :id", EsquemaBanco);

							comando.AdicionarParametroEntrada("id", item.IdRelacionamento, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}tab_titulo_associados s (id, titulo, associado_id, associado_tid, tid)
							values ({0}seq_titulo_associados.nextval, :titulo, :associado_id, (select t.tid from {0}tab_titulo t where t.id = :associado_id), :tid)", EsquemaBanco);
						}

						comando.AdicionarParametroEntrada("titulo", titulo.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("associado_id", item.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Atividades

				if (titulo.Atividades != null && titulo.Atividades.Count > 0)
				{
					foreach (Atividade item in titulo.Atividades)
					{
						if (item.IdRelacionamento > 0)
						{
							comando = bancoDeDados.CriarComando(@"update {0}tab_titulo_atividades e set e.titulo = :titulo, e.atividade = :atividade, e.protocolo = :protocolo, 
							e.tid = :tid where e.id = :id", EsquemaBanco);
							comando.AdicionarParametroEntrada("id", item.IdRelacionamento, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}tab_titulo_atividades s (id, titulo, atividade, protocolo, tid)
							values ({0}seq_titulo_atividades.nextval, :titulo, :atividade, :protocolo, :tid)", EsquemaBanco);
						}

						comando.AdicionarParametroEntrada("titulo", titulo.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("atividade", item.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("protocolo", item.Protocolo.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Exploracoes

				if (titulo.Exploracoes != null && titulo.Exploracoes.Count > 0)
				{
					foreach (TituloExploracaoFlorestal item in titulo.Exploracoes)
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}tab_titulo_exp_florestal s (id, titulo, exploracao_florestal)
							values ({0}seq_titulo_exp_florestal.nextval, :titulo, :exploracao_florestal)
							returning s.id into :id", EsquemaBanco);
						

						comando.AdicionarParametroEntrada("titulo", titulo.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("exploracao_florestal", item.ExploracaoFlorestalId, DbType.Int32);
						comando.AdicionarParametroSaida("id", DbType.Int32);

						bancoDeDados.ExecutarNonQuery(comando);

						item.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

						if (item.TituloExploracaoFlorestalExploracaoList?.Count > 0)
						{
							foreach (TituloExploracaoFlorestalExploracao itemDetalhe in item.TituloExploracaoFlorestalExploracaoList)
							{
								comando = bancoDeDados.CriarComando(@"insert into {0}tab_titulo_exp_flor_exp s (id, titulo_exploracao_florestal, exp_florestal_exploracao)
									values ({0}seq_titulo_exp_flor_exp.nextval, :titulo_exploracao_florestal, :exp_florestal_exploracao)", EsquemaBanco);

								comando.AdicionarParametroEntrada("titulo_exploracao_florestal", item.Id, DbType.Int32);
								comando.AdicionarParametroEntrada("exp_florestal_exploracao", itemDetalhe.ExploracaoFlorestalExploracaoId, DbType.Int32);

								bancoDeDados.ExecutarNonQuery(comando);
							}
						}

					}
				}

				#endregion

				#region Destinatário de e-mails

				if (titulo.DestinatarioEmails != null && titulo.DestinatarioEmails.Count > 0)
				{
					foreach (DestinatarioEmail item in titulo.DestinatarioEmails)
					{
						if (item.IdRelacionamento > 0)
						{
							comando = bancoDeDados.CriarComando(@"update {0}tab_titulo_pessoas e set e.titulo = :titulo, e.pessoa = :pessoa, 
							e.tid = :tid where e.id = :id", EsquemaBanco);
							comando.AdicionarParametroEntrada("id", item.IdRelacionamento, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}tab_titulo_pessoas s (id, titulo, pessoa, tipo, tid)
							values ({0}seq_titulo_pessoas.nextval, :titulo, :pessoa, 1, :tid)", EsquemaBanco);
						}

						comando.AdicionarParametroEntrada("titulo", titulo.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("pessoa", item.PessoaId, DbType.Int32);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Representante

				if (titulo.Representante.Id > 0)
				{
					if (titulo.Representante.IdRelacionamento > 0)
					{
						comando = bancoDeDados.CriarComando(@"update {0}tab_titulo_pessoas e set e.titulo = :titulo, e.pessoa = :pessoa, e.tid = :tid where e.id = :id", EsquemaBanco);
						comando.AdicionarParametroEntrada("id", titulo.Representante.IdRelacionamento, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}tab_titulo_pessoas s (id, titulo, pessoa, tipo, tid) values ({0}seq_titulo_pessoas.nextval, :titulo, :pessoa, 2, :tid)", EsquemaBanco);
					}

					comando.AdicionarParametroEntrada("titulo", titulo.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("pessoa", titulo.Representante.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#region Condicionantes

				if (titulo.Condicionantes != null && titulo.Condicionantes.Count > 0)
				{
					for (int i = 0; i < titulo.Condicionantes.Count; i++)
					{
						titulo.Condicionantes[i].tituloId = titulo.Id;
						titulo.Condicionantes[i].Ordem = i + 1;
						_daCondicionante.Salvar(titulo.Condicionantes[i], bancoDeDados);
					}
				}

				#endregion

				#region Arquivos

				if (titulo.Anexos != null && titulo.Anexos.Count > 0)
				{
					foreach (Anexo item in titulo.Anexos)
					{
						if (item.Id > 0)
						{
							comando = bancoDeDados.CriarComando(@"update {0}tab_titulo_arquivo a set a.titulo = :titulo, a.arquivo = :arquivo, 
							a.ordem = :ordem, a.descricao = :descricao, a.tid = :tid where a.id = :id", EsquemaBanco);
							comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}tab_titulo_arquivo a (id, titulo, arquivo, ordem, descricao, tid) 
							values ({0}seq_titulo_arquivo.nextval, :titulo, :arquivo, :ordem, :descricao, :tid)", EsquemaBanco);
						}

						comando.AdicionarParametroEntrada("titulo", titulo.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("arquivo", item.Arquivo.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("ordem", item.Ordem, DbType.Int32);
						comando.AdicionarParametroEntrada("descricao", DbType.String, 100, item.Descricao);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Histórico

				Historico.Gerar(titulo.Id, eHistoricoArtefato.titulo, eHistoricoAcao.atualizar, bancoDeDados);
				Consulta.Gerar(titulo.Id, eHistoricoArtefato.titulo, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		public void AlterarSituacao(Titulo titulo, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Alterar situação do Título

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_titulo t set t.situacao = :situacao, t.data_emissao = :data_emissao, 
				t.data_assinatura = :data_assinatura, t.data_inicio = :data_inicio, t.data_vencimento = :data_vencimento, t.data_encerramento = :data_encerramento,  t.situacao_motivo = :situacao_motivo, 
				t.prazo = :prazo, t.prazo_unidade = :prazo_unidade, t.dias_prorrogados = :dias_prorrogados, t.arquivo = :arquivo, t.tid = :tid where t.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", titulo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", titulo.Situacao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("data_emissao", (titulo.DataEmissao.Data.HasValue) ? titulo.DataEmissao.Data : (object)DBNull.Value, DbType.DateTime);
				comando.AdicionarParametroEntrada("data_assinatura", (titulo.DataAssinatura.Data.HasValue) ? titulo.DataAssinatura.Data : (object)DBNull.Value, DbType.DateTime);
				comando.AdicionarParametroEntrada("data_inicio", (titulo.DataInicioPrazo.Data.HasValue) ? titulo.DataInicioPrazo.Data : (object)DBNull.Value, DbType.DateTime);
				comando.AdicionarParametroEntrada("data_vencimento", (titulo.DataVencimento.Data.HasValue) ? titulo.DataVencimento.Data : (object)DBNull.Value, DbType.DateTime);
				comando.AdicionarParametroEntrada("data_encerramento", (titulo.DataEncerramento.Data.HasValue) ? titulo.DataEncerramento.Data : (object)DBNull.Value, DbType.DateTime);
				comando.AdicionarParametroEntrada("situacao_motivo", (titulo.MotivoEncerramentoId.GetValueOrDefault() > 0) ? titulo.MotivoEncerramentoId : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("prazo", (titulo.Prazo.HasValue) ? titulo.Prazo : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("prazo_unidade", DbType.String, 10, titulo.PrazoUnidade);
				comando.AdicionarParametroEntrada("dias_prorrogados", (titulo.DiasProrrogados.HasValue) ? titulo.DiasProrrogados : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("arquivo", (titulo.ArquivoPdf.Id.HasValue) ? titulo.ArquivoPdf.Id : (object)DBNull.Value, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Número

				if (titulo.Numero != null)
				{
					if (titulo.Numero.Automatico)
					{
						GerarNumeroAutomatico(titulo, banco);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"update {0}tab_titulo_numero t set t.ano = :ano, t.tid = :tid where t.titulo = :titulo", EsquemaBanco);
						comando.AdicionarParametroEntrada("ano", (titulo.Numero.Ano.HasValue) ? titulo.Numero.Ano : (object)DBNull.Value, DbType.Int32);
						comando.AdicionarParametroEntrada("titulo", titulo.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				//Limpar numero
				if (titulo.Numero == null)
				{
					comando = bancoDeDados.CriarComando(@"delete from {0}tab_titulo_numero t where t.titulo = :titulo", EsquemaBanco);
					comando.AdicionarParametroEntrada("titulo", titulo.Id, DbType.Int32);
					bancoDeDados.ExecutarNonQuery(comando);
				}
				#endregion

				bancoDeDados.Commit();
			}
		}

		public void DeclaratorioAlterarSituacao(Titulo titulo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Alterar situação do Título

				Comando comando = bancoDeDados.CriarComando(@"
				update {0}tab_titulo t
				set t.situacao         = :situacao,
					t.situacao_motivo  = :situacao_motivo,
					t.motivo_suspensao = :motivo_suspensao,
					t.data_vencimento  = :data_vencimento,
					t.dias_prorrogados = :dias_prorrogados,
					t.data_situacao    = sysdate,
					t.data_emissao     = (case when :situacao = 8 then sysdate else null end),/*Válido*/
					t.arquivo          = :arquivo,
					t.tid              = :tid
				where t.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", titulo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", titulo.Situacao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao_motivo", (titulo.MotivoEncerramentoId.GetValueOrDefault() > 0) ? titulo.MotivoEncerramentoId : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("motivo_suspensao", DbType.String, 1000, titulo.MotivoSuspensao);
				comando.AdicionarParametroEntrada("data_vencimento", (titulo.DataVencimento.Data.HasValue) ? titulo.DataVencimento.Data : (object)DBNull.Value, DbType.DateTime);
				comando.AdicionarParametroEntrada("dias_prorrogados", (titulo.DiasProrrogados.HasValue) ? titulo.DiasProrrogados : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("arquivo", (titulo.ArquivoPdf.Id.HasValue) ? titulo.ArquivoPdf.Id : (object)DBNull.Value, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Número

				if (titulo.Numero != null)
				{
					if (titulo.Numero.Automatico)
					{
						GerarNumeroAutomatico(titulo, banco);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"update {0}tab_titulo_numero t set t.ano = :ano, t.tid = :tid where t.titulo = :titulo", EsquemaBanco);
						comando.AdicionarParametroEntrada("ano", (titulo.Numero.Ano.HasValue) ? titulo.Numero.Ano : (object)DBNull.Value, DbType.Int32);
						comando.AdicionarParametroEntrada("titulo", titulo.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				bancoDeDados.Commit();
			}
		}

		public void SalvarPdfTitulo(Titulo titulo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Alterar situação do Título

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_titulo t set t.arquivo = :arquivo, t.tid = :tid where t.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", titulo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("arquivo", (titulo.ArquivoPdf.Id.HasValue) ? titulo.ArquivoPdf.Id : (object)DBNull.Value, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Atualizar o tid para a nova ação

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_titulo c set c.tid = :tid where c.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);
				#endregion

				#region Histórico

				#region Condicionantes

				comando = bancoDeDados.CriarComando(@"select c.id from {0}tab_titulo_condicionantes c where c.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						_daCondicionante.Excluir(Convert.ToInt32(reader["id"]));
					}
					reader.Close();
				}

				#endregion

				Historico.Gerar(id, eHistoricoArtefato.titulo, eHistoricoAcao.excluir, bancoDeDados);
				Consulta.Deletar(id, eHistoricoArtefato.titulo, bancoDeDados);

				#endregion

				#region Apaga os dados da titulo
				List<String> lista = new List<string>();
				lista.Add(@"delete from {0}tab_integracao_sinaflor f where exists
					(select 1 from {0}tab_titulo_exp_florestal e where e.titulo = :titulo and f.titulo_exp_florestal = e.id);");
				lista.Add(@"delete from {0}tab_titulo_exp_flor_exp f where exists
					(select 1 from {0}tab_titulo_exp_florestal e where e.titulo = :titulo and f.titulo_exploracao_florestal = e.id);");
				lista.Add("delete from {0}tab_titulo_exp_florestal e where e.titulo = :titulo;");
				lista.Add("delete from {0}tab_titulo_dependencia e where e.titulo = :titulo;");
				lista.Add("delete from {0}tab_titulo_associados e where e.titulo = :titulo;");
				lista.Add("delete from {0}tab_titulo_assinantes e where e.titulo = :titulo;");
				lista.Add("delete from {0}tab_titulo_setores e where e.titulo = :titulo;");
				lista.Add("delete from {0}tab_titulo_atividades e where e.titulo = :titulo;");
				lista.Add("delete from {0}tab_titulo_pessoas e where e.titulo = :titulo;");
				lista.Add("delete from {0}tab_titulo_condicionantes e where e.titulo = :titulo;");
				lista.Add("delete from {0}tab_titulo_numero e where e.titulo = :titulo;");
				lista.Add("delete from {0}tab_titulo_arquivo e where e.titulo = :titulo;");
				lista.Add("delete from {0}tab_titulo e where e.id = :titulo;");
				comando = bancoDeDados.CriarComando(@"begin " + string.Join(" ", lista) + "end;", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);
				bancoDeDados.Commit();
				#endregion
			}
		}

		private void GerarNumeroAutomatico(Titulo titulo, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				var comando = bancoDeDados.CriarComandoPlSql(@"
					declare 
						numeroc number;
					begin
						select 
						(case when maior.numero > :iniciarEm then maior.numero else :iniciarEm end) numero INTO numeroc
							from (select (nvl(max(t.numero),0)+1) numero 
									from {0}tab_titulo_numero t 
									where t.modelo = :modelo" + ((titulo.Numero.ReiniciaPorAno) ? " and t.ano = :ano" : String.Empty) + @") maior;

						update {0}tab_titulo_numero tt set tt.numero = numeroc, tt.ano = :ano, tt.tid = :tid where tt.titulo = :titulo returning tt.numero into :numero;
						if ( sql%rowcount = 0 ) then
							insert into {0}tab_titulo_numero e (id, titulo, modelo, numero, ano, tid) 
							values ({0}seq_titulo_numero.nextval, :titulo, :modelo, numeroc, :ano, :tid) returning numero into :numero;
						end if;
					end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("modelo", titulo.Modelo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("titulo", titulo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("iniciarEm", (titulo.Numero.IniciaEm ?? 0), DbType.Int32);
				comando.AdicionarParametroEntrada("ano", titulo.Numero.Ano, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				comando.AdicionarParametroSaida("numero", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				titulo.Numero.Inteiro = Convert.ToInt32(comando.ObterValorParametro("numero"));

				bancoDeDados.Commit();
			}
		}

		internal void Dependencias(int titulo, int modelo, int empreendimento, List<DependenciaLst> dependencias, BancoDeDados banco = null)
		{
			string select = string.Empty;
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Obter Dependecias

				Comando comando = bancoDeDados.CriarComando(@"select 'select c.id, c.tid, '|| lc.id ||' tipo from {0}'|| lc.tabela ||' c where c.empreendimento = :empreendimento union all ' retorno
				from {0}lov_caracterizacao_tipo lc", EsquemaBanco);

				comando.DbCommand.CommandText += comando.AdicionarIn("where", "lc.id", DbType.Int32, dependencias.Select(x => x.DependenciaTipo).ToList());

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						select += reader["retorno"].ToString();
					}

					reader.Close();
				}

				if (!string.IsNullOrEmpty(select))
				{
					comando = bancoDeDados.CriarComando(select.Substring(0, select.Length - 10), EsquemaBanco);
					comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						DependenciaLst aux = null;

						while (reader.Read())
						{
							aux = (dependencias.SingleOrDefault(x => x.DependenciaTipo == Convert.ToInt32(reader["tipo"])) ?? new DependenciaLst());
							aux.Id = Convert.ToInt32(reader["id"]);
							aux.DependenciaTid = reader["tid"].ToString();
						}

						reader.Close();
					}
				}

				#endregion

				#region Dependências da caracterização

				comando = bancoDeDados.CriarComando("delete from {0}tab_titulo_dependencia d where d.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				if (dependencias != null && dependencias.Count > 0)
				{
					foreach (DependenciaLst item in dependencias.Where(x => x.Id > 0))
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}tab_titulo_dependencia 
						(id, titulo, titulo_modelo, dependencia_tipo, dependencia_caracterizacao, dependencia_id, dependencia_tid, dependencia_projeto_id, dependencia_projeto_tid, tid) 
						values ({0}seq_titulo_dependencia.nextval, :titulo, :titulo_modelo, :dependencia_tipo, :dependencia_caracterizacao, :dependencia_id, :dependencia_tid, 
						(select prj.id from crt_projeto_geo prj where prj.caracterizacao = :dependencia_caracterizacao and prj.empreendimento = :empreendimento), 
						(select prj.tid from crt_projeto_geo prj where prj.caracterizacao = :dependencia_caracterizacao and prj.empreendimento = :empreendimento), :tid)", EsquemaBanco);

						comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);
						comando.AdicionarParametroEntrada("titulo_modelo", modelo, DbType.Int32);
						comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);

						comando.AdicionarParametroEntrada("dependencia_tipo", item.TipoId, DbType.Int32);
						comando.AdicionarParametroEntrada("dependencia_caracterizacao", item.DependenciaTipo, DbType.Int32);
						comando.AdicionarParametroEntrada("dependencia_id", item.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("dependencia_tid", DbType.String, 36, item.DependenciaTid);

						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void AlterarSituacao(int titulo, int situacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"
					update tab_titulo t set t.situacao = :situacao where t.id = :id");

				comando.AdicionarParametroEntrada("situacao", situacao, DbType.Int32);
				comando.AdicionarParametroEntrada("id", titulo, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(titulo, eHistoricoArtefato.titulo, eHistoricoAcao.atualizar, bancoDeDados);
				Consulta.Gerar(titulo, eHistoricoArtefato.titulo, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter / Filtrar

		public Resultados<Titulo> Filtrar(Filtro<TituloFiltro> filtros, BancoDeDados banco = null)
		{
			Resultados<Titulo> retorno = new Resultados<Titulo>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAnd("l.requerimento", "requerimento", filtros.Dados.RequerimentoID);

				if (!string.IsNullOrWhiteSpace(filtros.Dados.DataEmisssao))
				{
					comandtxt += " and exists (select null from tab_titulo t where t.id = l.titulo_id and trunc(t.data_emissao) = :data_emissao) ";
					comando.AdicionarParametroEntrada("data_emissao", filtros.Dados.DataEmisssao, DbType.DateTime);
				}

				comandtxt += comando.FiltroAnd("l.interessado_cpf_cnpj", "interessado_cpf_cnpj", filtros.Dados.InteressadoCPFCNPJ);

				comandtxt += comando.FiltroAndLike("l.interessado_nome_razao", "interessado_nome_razao", filtros.Dados.InteressadoNomeRazao, true, true);

				if (filtros.Dados.OrigemID == 1)//Institucional
				{
					comandtxt += " and l.credenciado is null ";
				}
				else if(filtros.Dados.OrigemID == 2)//Credenciado
				{
					comandtxt += " and l.credenciado is not null ";
				}

				comandtxt += comando.FiltroAnd("l.modelo_id", "modelo", filtros.Dados.Modelo);

				comandtxt += comando.FiltroAnd("l.situacao_id", "situacao", filtros.Dados.Situacao);

				comandtxt += comando.FiltroAnd("l.setor_id", "setor", filtros.Dados.Setor);

				comandtxt += comando.FiltroAndLike("l.numero || '/' || l.ano", "numero", filtros.Dados.Numero, true);

				comandtxt += comando.FiltroAndLike("l.protocolo_numero", "protocolo_numero", filtros.Dados.Protocolo.NumeroTexto, true);

				comandtxt += comando.FiltroAnd("l.empreendimento_codigo", "empreendimento_codigo", filtros.Dados.EmpreendimentoCodigo);

				comandtxt += comando.FiltroAndLike("l.empreendimento_denominador", "empreendimento", filtros.Dados.Empreendimento, true);

				if (filtros.Dados.Modelo <= 0 && filtros.Dados.ModeloFiltrar != null && filtros.Dados.ModeloFiltrar.Count > 0)
				{
					comandtxt += comando.AdicionarIn("and", "l.modelo_id", DbType.Int32, filtros.Dados.ModeloFiltrar.Select(x => x).ToList());
				}

				if (filtros.Dados.SituacoesFiltrar != null && filtros.Dados.SituacoesFiltrar.Count > 0)
				{
					comandtxt += comando.AdicionarIn("and", "l.situacao_id", DbType.Int32, filtros.Dados.SituacoesFiltrar.Select(x => x).ToList());
				}

				if (filtros.Dados.IsDeclaratorio)
				{
					comandtxt += " and l.requerimento is not null ";
				}
				else
				{
					comandtxt += " and l.requerimento is null ";
				}

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "numero", "modelo_sigla", "protocolo_numero", "empreendimento_denominador", "situacao_texto", "data_vencimento" };

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

				comando.DbCommand.CommandText = String.Format(@"
				select count(*) quantidade from lst_titulo l where l.credenciado is null " + comandtxt +
				"union all select count(*) quantidade from lst_titulo l where l.credenciado is not null and l.situacao_id != 7 " +
				"and exists (select 1 from tab_requerimento r where r.numero = l.requerimento) " + comandtxt, (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));


				comando.DbCommand.CommandText = "select sum(d.quantidade) from (" + comando.DbCommand.CommandText + ") d ";

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = String.Format(@"
				select titulo_id, titulo_tid, numero, numero_completo, data_vencimento, autor_id, autor_nome, modelo_sigla, situacao_texto, situacao_id,
					modelo_id, modelo_nome, modelo_codigo, protocolo_id, protocolo protocolo_tipo, protocolo_numero, empreendimento_codigo, empreendimento_denominador, requerimento 
					from lst_titulo l where l.credenciado is null " + comandtxt +
				@" union all 
				select titulo_id, titulo_tid, numero, numero_completo, data_vencimento, autor_id, autor_nome, modelo_sigla, situacao_texto, situacao_id,
					modelo_id, modelo_nome, modelo_codigo, protocolo_id, protocolo protocolo_tipo, protocolo_numero, empreendimento_codigo, empreendimento_denominador, requerimento 
					from lst_titulo l where l.credenciado is not null and l.situacao_id != 7 and exists (select 1 from tab_requerimento r where r.numero = l.requerimento) " + comandtxt, (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a " + DaHelper.Ordenar(colunas, ordenar) + ") where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					Titulo titulo;
					while (reader.Read())
					{
						titulo = new Titulo();
						titulo.Id = reader.GetValue<int>("titulo_id");
						titulo.Autor.Id = reader.GetValue<int>("autor_id");
						titulo.Autor.Nome = reader.GetValue<string>("autor_nome");
						titulo.Tid = reader.GetValue<string>("titulo_tid");
						titulo.Numero.Texto = reader.GetValue<string>("numero_completo");
						titulo.Modelo.Id = reader.GetValue<int>("modelo_id");
						titulo.Modelo.Sigla = reader.GetValue<string>("modelo_sigla");
						titulo.Modelo.Nome = reader.GetValue<string>("modelo_nome");
						titulo.Modelo.Codigo = reader.GetValue<int>("modelo_codigo");
						titulo.Situacao.Id = reader.GetValue<int>("situacao_id");
						titulo.Situacao.Nome = reader.GetValue<string>("situacao_texto");
						titulo.EmpreendimentoCodigo = reader.GetValue<long>("empreendimento_codigo");
						titulo.EmpreendimentoTexto = reader.GetValue<string>("empreendimento_denominador");
						titulo.DataVencimento.DataTexto = reader.GetValue<string>("data_vencimento");
						titulo.RequerimetoId = reader.GetValue<int>("requerimento");

						titulo.Protocolo.Id = reader.GetValue<int>("protocolo_id");
						if (titulo.Protocolo.Id.GetValueOrDefault() > 0)
						{
							ProtocoloNumero prot = new ProtocoloNumero(reader.GetValue<string>("protocolo_numero"));
							titulo.Protocolo.IsProcesso = (reader.GetValue<int>("protocolo_tipo") == 1);
							titulo.Protocolo.NumeroProtocolo = prot.Numero;
							titulo.Protocolo.Ano = prot.Ano;
						}
						if (titulo.Situacao.Id == (int)eTituloSituacao.Concluido && titulo.DataVencimento?.Data < DateTime.Now.Date)
						{
							if (titulo.Modelo.Codigo == (int)eTituloModeloCodigo.LaudoVistoriaFlorestal || titulo.Modelo.Codigo == (int)eTituloModeloCodigo.AutorizacaoExploracaoFlorestal)
								titulo.Situacao.Nome = "Vencido";
						}
						retorno.Itens.Add(titulo);
					}

					reader.Close();

					#endregion
				}
			}

			return retorno;
		}

		internal Titulo Obter(int id, bool simplificado = false, BancoDeDados banco = null)
		{
			Titulo titulo = new Titulo();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Título

				Comando comando = bancoDeDados.CriarComando(@"
                    select t.*, ta.*,
					  (select s.autorizacao_sinaflor from tab_integracao_sinaflor s where rownum = 1
						and s.autorizacao_sinaflor is not null and exists
						(select 1 from tab_titulo_exp_florestal tt
							where tt.titulo = t.id
							and tt.id = s.titulo_exp_florestal)) codigo_sinaflor,
						(select count(*) from HST_TITULO where titulo_id = t.id and SITUACAO_ID = :situacao_prorrogado) ja_prorrogado
                      from (select t.titulo_id id,
                                   t.titulo_tid tid,
                                   t.numero,
                                   t.ano,
                                   t.autor_id,
                                   t.autor_nome,
                                   t.setor_id,
                                   t.setor_nome,
                                   t.modelo_id,
                                   t.modelo_codigo,
                                   t.modelo_nome,
                                   t.modelo_sigla,
                                   t.situacao_id,
                                   t.situacao_texto,
                                   t.protocolo,
                                   t.protocolo_id,
                                   t.protocolo_numero,
                                   t.empreendimento_id,
                                   t.empreendimento_denominador,
                                   t.empreendimento_cnpj
                              from lst_titulo t
                             where t.titulo_id = :id) t,
                           (select (select p.requerimento
                                      from tab_protocolo p
                                     where p.id = ta.protocolo) requerimento,
                                   (select p.empreendimento
                                      from tab_protocolo p
                                     where p.id = ta.protocolo) protocolo_empreendimento,
                                   ta.requerimento requerimento_titulo,
                                   ta.credenciado,
                                   ta.situacao_motivo,
                                   ta.local_emissao local_emissao_id,
                                   (select m.texto
                                      from lov_municipio m
                                     where m.id = ta.local_emissao) local_emissao_texto,
                                   ta.prazo,
                                   ta.prazo_unidade,
                                   ta.dias_prorrogados,
                                   ta.data_criacao,
                                   ta.data_emissao,
                                   ta.data_assinatura,
                                   ta.data_inicio,
                                   ta.data_vencimento,
                                   ta.data_encerramento,
									ta.data_situacao,
                                   ta.arquivo,
                                   p.id representante,
                                   p.nome representante_nome,
                                   tp.id relacionamento_representante
                              from tab_titulo ta, 
                                   tab_titulo_pessoas tp, 
                                   tab_pessoa p
                             where ta.id = tp.titulo(+)
                               and tp.pessoa = p.id(+)
                               and tp.tipo(+) = 2
                               and ta.id = :id) ta", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao_prorrogado", (int)eTituloSituacao.ProrrogadoDeclaratorio, DbType.Int32);
				
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						#region Dados Base

						titulo.Id = id;
						titulo.Tid = reader["tid"].ToString();

						titulo.Autor.Id = Convert.ToInt32(reader["autor_id"]);
						titulo.Autor.Nome = reader["autor_nome"].ToString();

						titulo.Setor.Id = reader.GetValue<int>("setor_id");
						titulo.Setor.Nome = reader.GetValue<string>("setor_nome");

						titulo.Modelo.Id = Convert.ToInt32(reader["modelo_id"]);
						titulo.Modelo.Codigo = Convert.ToInt32(reader["modelo_codigo"]);
						titulo.Modelo.Sigla = reader["modelo_sigla"].ToString();
						titulo.Modelo.Nome = reader["modelo_nome"].ToString();

						titulo.Situacao.Id = Convert.ToInt32(reader["situacao_id"]);
						titulo.Situacao.Nome = reader["situacao_texto"].ToString();

						if (reader["situacao_motivo"] != null && !Convert.IsDBNull(reader["situacao_motivo"]))
						{
							titulo.MotivoEncerramentoId = Convert.ToInt32(reader["situacao_motivo"]);
						}

						titulo.LocalEmissao.Id = Convert.ToInt32(reader["local_emissao_id"]);
						titulo.LocalEmissao.Texto = reader["local_emissao_texto"].ToString();

						if (reader["numero"] != null && !Convert.IsDBNull(reader["numero"]))
						{
							titulo.Numero.Inteiro = Convert.ToInt32(reader["numero"]);
						}

						if (reader["ano"] != null && !Convert.IsDBNull(reader["ano"]))
						{
							titulo.Numero.Ano = Convert.ToInt32(reader["ano"]);
						}

						if (reader["protocolo_id"] != null && !Convert.IsDBNull(reader["protocolo_id"]))
						{
							ProtocoloNumero prot = new ProtocoloNumero(reader["protocolo_numero"].ToString());
							titulo.Protocolo.Id = Convert.ToInt32(reader["protocolo_id"]);
							titulo.Protocolo.NumeroProtocolo = prot.Numero;
							titulo.Protocolo.Ano = prot.Ano;
						}

						titulo.Protocolo.IsProcesso = (reader.GetValue<string>("protocolo") != null && reader.GetValue<int>("protocolo") == 1);

						if (reader["requerimento"] != null && !Convert.IsDBNull(reader["requerimento"]))
						{
							titulo.Protocolo.Requerimento.Id = Convert.ToInt32(reader["requerimento"]);
						}

						if (reader["protocolo_empreendimento"] != null && !Convert.IsDBNull(reader["protocolo_empreendimento"]))
						{
							titulo.Protocolo.Empreendimento.Id = Convert.ToInt32(reader["protocolo_empreendimento"]);
						}

						if (reader["empreendimento_id"] != null && !Convert.IsDBNull(reader["empreendimento_id"]))
						{
							titulo.EmpreendimentoId = Convert.ToInt32(reader["empreendimento_id"]);
							titulo.EmpreendimentoTexto = reader["empreendimento_denominador"].ToString();
						}

						if (reader["prazo"] != null && !Convert.IsDBNull(reader["prazo"]))
						{
							titulo.Prazo = Convert.ToInt32(reader["prazo"]);
							titulo.PrazoUnidade = reader["prazo_unidade"].ToString();
						}

						if (reader["dias_prorrogados"] != null && !Convert.IsDBNull(reader["dias_prorrogados"]))
						{
							titulo.DiasProrrogados = Convert.ToInt32(reader["dias_prorrogados"]);
						}

						titulo.DataCriacao.Data = Convert.ToDateTime(reader["data_criacao"]);

						if (reader["data_emissao"] != null && !Convert.IsDBNull(reader["data_emissao"]))
						{
							titulo.DataEmissao.Data = Convert.ToDateTime(reader["data_emissao"]);
						}

						if (reader["data_assinatura"] != null && !Convert.IsDBNull(reader["data_assinatura"]))
						{
							titulo.DataAssinatura.Data = Convert.ToDateTime(reader["data_assinatura"]);
						}

						if (reader["data_inicio"] != null && !Convert.IsDBNull(reader["data_inicio"]))
						{
							titulo.DataInicioPrazo.Data = Convert.ToDateTime(reader["data_inicio"]);
						}

						if (reader["data_vencimento"] != null && !Convert.IsDBNull(reader["data_vencimento"]))
						{
							titulo.DataVencimento.Data = Convert.ToDateTime(reader["data_vencimento"]);
						}

						if (reader["data_encerramento"] != null && !Convert.IsDBNull(reader["data_encerramento"]))
						{
							titulo.DataEncerramento.Data = Convert.ToDateTime(reader["data_encerramento"]);
						}

						if (reader["data_situacao"] != null && !Convert.IsDBNull(reader["data_situacao"]))
						{
							titulo.DataSituacao.Data = Convert.ToDateTime(reader["data_situacao"]);
						}

						if (reader["arquivo"] != null && !Convert.IsDBNull(reader["arquivo"]))
						{
							titulo.ArquivoPdf.Id = Convert.ToInt32(reader["arquivo"]);
						}

						if (reader["representante"] != null && !Convert.IsDBNull(reader["representante"]))
						{
							titulo.Representante.Id = Convert.ToInt32(reader["representante"]);
							titulo.Representante.Fisica.Nome = reader["representante_nome"].ToString();
							titulo.Representante.IdRelacionamento = Convert.ToInt32(reader["relacionamento_representante"]);
						}

						titulo.RequerimetoId = reader.GetValue<int?>("requerimento_titulo");
						titulo.CredenciadoId = reader.GetValue<int?>("credenciado");
						titulo.CodigoSinaflor = reader["codigo_sinaflor"].ToString();
						titulo.JaFoiProrrogado = reader.GetValue<bool>("ja_prorrogado");

						#endregion
					}
					reader.Close();
				}

				if (simplificado)
				{
					return titulo;
				}

				#endregion

				#region Setores

				comando = bancoDeDados.CriarComando(@"select id, titulo, setor, tid from {0}tab_titulo_setores s where s.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Setor item;
					while (reader.Read())
					{
						item = new Setor();
						item.IdRelacao = Convert.ToInt32(reader["id"]);
						item.Id = Convert.ToInt32(reader["setor"]);
						titulo.Setores.Add(item);
					}
					reader.Close();
				}

				#endregion

				#region Assinantes
				comando = bancoDeDados.CriarComando(@"select ta.id, ta.titulo, f.id func_id, f.nome func_nome, ta.cargo, c.nome cargo_nome, ta.tid from tab_titulo_assinantes ta, tab_funcionario f, tab_cargo c where 
					ta.funcionario = f.id and ta.cargo = c.id and ta.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					TituloAssinante item;
					while (reader.Read())
					{
						item = new TituloAssinante();
						item.Id = Convert.ToInt32(reader["id"]);
						item.Tid = reader["tid"].ToString();
						item.FuncionarioId = Convert.ToInt32(reader["func_id"]);
						item.FuncionarioNome = reader["func_nome"].ToString();
						item.FuncionarioCargoId = Convert.ToInt32(reader["cargo"]);
						item.FuncionarioCargoNome = reader["cargo_nome"].ToString();
						item.Selecionado = true;

						if (reader["cargo"] != null && !Convert.IsDBNull(reader["cargo"]))
						{
							item.FuncionarioCargoId = Convert.ToInt32(reader["cargo"]);
						}

						titulo.Assinantes.Add(item);
					}
					reader.Close();
				}
				#endregion

				#region Associados

				comando = bancoDeDados.CriarComando(@"select s.id, s.associado_id, s.associado_tid, s.tid, ttn.numero || '/' || ttn.ano numeroAno, ttm.nome modelo_nome,
				ttm.id modelo_id, ttm.sigla from {0}tab_titulo_associados s, {0}tab_titulo_numero ttn, {0}tab_titulo_modelo ttm where s.associado_id = ttn.titulo 
				and ttn.modelo = ttm.id and s.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Titulo item;
					while (reader.Read())
					{
						item = new Titulo();

						item.IdRelacionamento = Convert.ToInt32(reader["id"]);

						item.Tid = reader["tid"].ToString();

						item.Numero.Texto = reader["numeroAno"].ToString();

						item.Modelo.Nome = reader["modelo_nome"].ToString();
						item.Modelo.Id = Convert.ToInt32(reader["modelo_id"]);
						item.Modelo.Sigla = reader["sigla"].ToString();

						if (reader["associado_id"] != null && !Convert.IsDBNull(reader["associado_id"]))
						{
							item.Id = Convert.ToInt32(reader["associado_id"]);
						}

						item.Tid = reader["associado_tid"].ToString();

						titulo.Associados.Add(item);
					}
					reader.Close();
				}

				#endregion

				#region Atividades

				comando = bancoDeDados.CriarComando(@"select s.id, pa.codigo, s.titulo, s.atividade, pa.atividade atividade_nome, s.protocolo, s.tid, pa.situacao 
				from {0}tab_titulo_atividades s, {0}tab_atividade pa where s.atividade = pa.id and s.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Atividade item;
					while (reader.Read())
					{
						item = new Atividade();
						item.Codigo = reader.GetValue<int>("codigo");
						item.IdRelacionamento = Convert.ToInt32(reader["id"]);
						item.Tid = reader["tid"].ToString();
						item.NomeAtividade = reader["atividade_nome"].ToString();
						item.Ativada = Convert.ToBoolean(reader.GetValue<int>("situacao"));

						if (reader["atividade"] != null && !Convert.IsDBNull(reader["atividade"]))
						{
							item.Id = Convert.ToInt32(reader["atividade"]);
						}

						if (!Convert.IsDBNull(reader["protocolo"]))
						{
							item.Protocolo.Id = Convert.ToInt32(reader["protocolo"]);
						}

						titulo.Atividades.Add(item);
					}
					reader.Close();
				}

				#endregion

				#region Exploracoes

				comando = bancoDeDados.CriarComando(@"select s.id, s.titulo, s.exploracao_florestal,
				concat(concat(concat(lv.chave, lpad(to_char(e.codigo_exploracao), 3, '0')), '-'), to_char(e.data_cadastro, 'ddMMyyyy')) localizador
				from {0}tab_titulo_exp_florestal s
				inner join crt_exploracao_florestal e
					on (s.exploracao_florestal = e.id)
				left join idafgeo.lov_tipo_exploracao lv
					on (e.tipo_exploracao = lv.tipo_atividade)
				where s.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					TituloExploracaoFlorestal item;
					while (reader.Read())
					{
						item = new TituloExploracaoFlorestal();
						item.Id = Convert.ToInt32(reader["id"]);
						item.TituloId = Convert.ToInt32(reader["titulo"]);
						item.ExploracaoFlorestalId = Convert.ToInt32(reader["exploracao_florestal"]);

						if (reader["localizador"] != null && !Convert.IsDBNull(reader["localizador"]))
							item.ExploracaoFlorestalTexto = reader["localizador"].ToString();

						titulo.Exploracoes.Add(item);
					}
					reader.Close();
				}

				#endregion

				#region Destinatário de e-mails

				comando = bancoDeDados.CriarComando(@"select s.id, s.titulo, s.pessoa, nvl(p.nome,p.razao_social) pessoa_nome, s.tid from {0}tab_titulo_pessoas s, {0}tab_pessoa p 
				where s.pessoa = p.id and s.tipo = 1 and s.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						titulo.DestinatarioEmails.Add(new DestinatarioEmail()
						{
							IdRelacionamento = Convert.ToInt32(reader["id"]),
							PessoaId = Convert.ToInt32(reader["pessoa"]),
							PessoaNome = reader["pessoa_nome"].ToString(),
							Selecionado = true
						});
					}
					reader.Close();
				}

				#endregion

				#region Representante

				comando = bancoDeDados.CriarComando(@"select s.id, s.titulo, s.pessoa, nvl(p.nome,p.razao_social) pessoa_nome, s.tid from {0}tab_titulo_pessoas s, {0}tab_pessoa p 
				where s.pessoa = p.id and s.titulo = :titulo and s.tipo = 2", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						titulo.Representante.Id = Convert.ToInt32(reader["pessoa"]);
						titulo.Representante.IdRelacionamento = Convert.ToInt32(reader["id"]);
						titulo.Representante.Fisica.Nome = reader["pessoa_nome"].ToString();
					}

					reader.Close();
				}

				#endregion

				#region Condicionantes

				comando = bancoDeDados.CriarComando(@"select c.id, c.titulo, c.situacao, c.descricao, c.possui_prazo, c.prazo, c.periodicidade, c.periodo, c.periodo_tipo, c.tid, ls.texto situacao_texto
				from {0}tab_titulo_condicionantes c, lov_titulo_cond_situacao ls where c.titulo = :titulo and ls.id = c.situacao order by c.ordem", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					TituloCondicionante item;
					while (reader.Read())
					{
						item = new TituloCondicionante();
						item.Id = Convert.ToInt32(reader["id"]);
						item.Tid = reader["tid"].ToString();
						item.tituloId = titulo.Id;
						item.Situacao.Id = Convert.ToInt32(reader["situacao"]);
						item.Situacao.Texto = reader["situacao_texto"].ToString();
						item.Descricao = reader["descricao"].ToString();

						item.PossuiPrazo = Convert.ToInt32(reader["possui_prazo"]) > 0;

						if (reader["prazo"] != null && !Convert.IsDBNull(reader["prazo"]))
						{
							item.Prazo = Convert.ToInt32(reader["prazo"]);
						}

						item.PossuiPeriodicidade = Convert.ToInt32(reader["periodicidade"]) > 0;

						if (reader["periodo"] != null && !Convert.IsDBNull(reader["periodo"]))
						{
							item.PeriodicidadeValor = Convert.ToInt32(reader["periodo"]);
						}

						if (reader["periodo_tipo"] != null && !Convert.IsDBNull(reader["periodo_tipo"]))
						{
							item.PeriodicidadeTipo.Id = Convert.ToInt32(reader["periodo_tipo"]);
						}

						titulo.Condicionantes.Add(item);
					}
					reader.Close();
				}

				#endregion

				#region Arquivos
				comando = bancoDeDados.CriarComando(@"select a.id, a.ordem, a.descricao, b.nome, b.extensao, b.id arquivo_id, b.caminho,
				a.tid from {0}tab_titulo_arquivo a, {0}tab_arquivo b where a.arquivo = b.id and a.titulo = :titulo order by a.ordem", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Anexo item;
					while (reader.Read())
					{
						item = new Anexo();
						item.Id = Convert.ToInt32(reader["id"]);
						item.Ordem = Convert.ToInt32(reader["ordem"]);
						item.Descricao = reader["descricao"].ToString();

						item.Arquivo.Id = Convert.ToInt32(reader["arquivo_id"]);
						item.Arquivo.Caminho = reader["caminho"].ToString();
						item.Arquivo.Nome = reader["nome"].ToString();
						item.Arquivo.Extensao = reader["extensao"].ToString();

						item.Tid = reader["tid"].ToString();

						titulo.Anexos.Add(item);
					}
					reader.Close();
				}
				#endregion
			}
			return titulo;
		}

		internal Titulo ObterSimplificado(int id, BancoDeDados banco = null)
		{
			return Obter(id, true, banco);
		}

		internal Protocolo ObterProtocolo(int requerimento, BancoDeDados banco = null)
		{
			Protocolo protocolo = new Protocolo();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Título

				Comando comando = comando = bancoDeDados.CriarComando(@"select p.id, p.numero||'/'||p.ano numero, p.protocolo tipo from {0}tab_protocolo p where p.requerimento = :requerimento", EsquemaBanco);

				comando.AdicionarParametroEntrada("requerimento", requerimento, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						protocolo.Id = Convert.ToInt32(reader["id"]);
						protocolo.NumeroProtocolo = Convert.ToInt32(reader["numero"]);
						protocolo.IsProcesso = reader["tipo"].ToString() == "1";
					}
					reader.Close();
				}

				#endregion
			}
			return protocolo;
		}

		internal List<Titulo> TitulosProtocolo(int protocolo)
		{
			List<Titulo> retorno = new List<Titulo>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id, t.tid, tn.numero, tn.ano, t.modelo modelo_id, m.nome modelo_nome, t.situacao situacao_id, ls.texto situacao_texto
				from {0}tab_titulo t, {0}tab_titulo_numero tn, {0}lov_titulo_situacao ls, {0}tab_titulo_modelo m where t.modelo = m.id and t.situacao = ls.id and t.id = tn.titulo 
				and t.protocolo in (select :protocolo id from dual union all select pa.associado id from {0}tab_protocolo_associado pa where pa.protocolo = :protocolo)", EsquemaBanco);

				comando.AdicionarParametroEntrada("protocolo", protocolo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					Titulo titulo;
					while (reader.Read())
					{
						titulo = new Titulo();
						titulo.Id = Convert.ToInt32(reader["id"]);
						titulo.Tid = reader["tid"].ToString();

						if (reader["situacao_id"] != null && !Convert.IsDBNull(reader["situacao_id"]))
						{
							titulo.Situacao.Id = Convert.ToInt32(reader["situacao_id"]);
							titulo.Situacao.Nome = reader["situacao_texto"].ToString();
						}

						if (reader["numero"] != null && !Convert.IsDBNull(reader["numero"]))
						{
							titulo.Numero.Inteiro = Convert.ToInt32(reader["numero"]);
						}

						if (reader["ano"] != null && !Convert.IsDBNull(reader["ano"]))
						{
							titulo.Numero.Ano = Convert.ToInt32(reader["ano"]);
						}

						if (reader["modelo_nome"] != null && !Convert.IsDBNull(reader["modelo_nome"]))
						{
							titulo.Modelo.Nome = reader["modelo_nome"].ToString();
						}

						if (reader["modelo_id"] != null && !Convert.IsDBNull(reader["modelo_id"]))
						{
							titulo.Modelo.Id = Convert.ToInt32(reader["modelo_id"]);
						}

						retorno.Add(titulo);
					}

					reader.Close();

					#endregion
				}
			}

			return retorno;
		}

		internal List<Situacao> ObterSituacoes()
		{
			List<Situacao> lst = new List<Situacao>();

			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto from lov_titulo_situacao t order by texto");

			foreach (var item in daReader)
			{
				lst.Add(new Situacao()
				{
					Id = Convert.ToInt32(item["id"]),
					Nome = item["texto"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<Titulo> TitulosProtocolo(Protocolo protocolo, int codigoModelo, BancoDeDados banco = null)
		{
			List<Titulo> retorno = new List<Titulo>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id, t.tid, t.situacao, t.modelo modelo_id, tm.codigo modelo_codigo, 
				t.protocolo from {0}tab_titulo t, {0}tab_titulo_atividades a, {0}tab_titulo_modelo tm where a.titulo = t.id
				and t.modelo = tm.id and tm.codigo = :codigo and a.protocolo = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", protocolo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("codigo", codigoModelo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					Titulo titulo;
					while (reader.Read())
					{
						titulo = new Titulo();
						titulo.Id = Convert.ToInt32(reader["id"]);
						titulo.Tid = reader["tid"].ToString();

						if (reader["situacao"] != null && !Convert.IsDBNull(reader["situacao"]))
						{
							titulo.Situacao.Id = Convert.ToInt32(reader["situacao"]);
						}

						if (reader["modelo_id"] != null && !Convert.IsDBNull(reader["modelo_id"]))
						{
							titulo.Modelo.Id = Convert.ToInt32(reader["modelo_id"]);
						}

						if (reader["modelo_codigo"] != null && !Convert.IsDBNull(reader["modelo_codigo"]))
						{
							titulo.Modelo.Codigo = Convert.ToInt32(reader["modelo_codigo"]);
						}

						if (reader["protocolo"] != null && !Convert.IsDBNull(reader["protocolo"]))
						{
							titulo.Protocolo.Id = Convert.ToInt32(reader["protocolo"]);
						}

						retorno.Add(titulo);
					}

					reader.Close();

					#endregion
				}
			}

			return retorno;
		}

		internal List<Protocolos> ObterProcessosDocumentos(int protocolo)
		{
			List<Protocolos> lst = new List<Protocolos>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select p.id, p.id || '@'|| p.protocolo ||'@' || r.id valor, r.numero || ' - ' || r.data_criacao texto, 
															(case p.protocolo when 1 then 'P' else 'D' end) tipo from {0}tab_protocolo p, {0}tab_requerimento r
															where r.id = p.requerimento and (p.id = :id or p.id in (select a.associado from {0}tab_protocolo_associado a
															where a.tipo = p.protocolo and  a.protocolo = :id))", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", protocolo);
				IEnumerable<IDataReader> daReader = DaHelper.ObterLista(comando, bancoDeDados);

				foreach (var item in daReader)
				{
					lst.Add(new Protocolos()
					{
						Id = item["valor"].ToString(),
						Texto = item["texto"].ToString(),
						IsAtivo = true
					});
				}
			}

			return lst;
		}

		internal List<DestinatarioEmail> ObterDestinatariosEmailProtocolo(int protocolo)
		{
			List<DestinatarioEmail> lst = new List<DestinatarioEmail>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select p.id, nvl(p.nome,p.razao_social) nome from {0}tab_pessoa p, {0}tab_pessoa_meio_contato pm where pm.pessoa = p.id and pm.meio_contato = 5 and p.id in
				(select p.interessado id from {0}tab_protocolo p where p.id = :id union select r.responsavel id from {0}tab_protocolo_responsavel r where r.protocolo = :id union
				select o.responsavel id from {0}tab_empreendimento_responsavel o, {0}tab_protocolo p where o.empreendimento = p.empreendimento and p.id = :id)", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", protocolo);

				IEnumerable<IDataReader> daReader = DaHelper.ObterLista(comando, bancoDeDados);

				foreach (var item in daReader)
				{
					lst.Add(new DestinatarioEmail()
					{
						PessoaId = Convert.ToInt32(item["id"]),
						PessoaNome = item["nome"].ToString()
					});
				}
			}

			return lst;
		}

		internal List<DestinatarioEmail> ObterDestinatariosEmailTitulo(int id)
		{
			List<DestinatarioEmail> lst = new List<DestinatarioEmail>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select tp.id, p.id pessoa, nvl(p.nome,p.razao_social) nome pessoa_nome from {0}tab_titulo t, {0}tab_titulo_pessoas tp, {0}tab_pessoa p
				where t.id = tp.titulo and tp.tipo = 1 and tp.pessoa = p.id and t.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id);

				IEnumerable<IDataReader> daReader = DaHelper.ObterLista(comando, bancoDeDados);

				foreach (var item in daReader)
				{
					lst.Add(new DestinatarioEmail()
					{
						IdRelacionamento = Convert.ToInt32(item["id"]),
						PessoaId = Convert.ToInt32(item["pessoa"]),
						PessoaNome = item["pessoa_nome"].ToString(),
						Selecionado = true
					});
				}
			}
			return lst;
		}

		internal List<TituloCondicionante> ObterCondicionantes(int tituloId)
		{
			List<TituloCondicionante> lst = new List<TituloCondicionante>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select 
					(select min(htc.data_execucao) from {0}hst_titulo_condicionantes htc where htc.titulo_id = c.titulo) cond_data_criacao,
					c.id, c.titulo, t.data_criacao, (case when tn.numero is not null then tn.numero || '/' || tn.ano else null end) titulo_numero, 
					ls.id situacao_id, ls.texto situacao_texto, c.descricao, c.possui_prazo, c.prazo, c.periodicidade, c.periodo, c.data_vencimento, c.data_inicio, c.dias_prorrogados,
					lp.id periodo_tipo_id, lp.texto periodo_tipo_texto, c.tid
					from {0}tab_titulo_condicionantes c, {0}tab_titulo t, {0}tab_titulo_numero tn, {0}lov_titulo_cond_situacao ls, {0}lov_titulo_cond_periodo_tipo lp
					where tn.titulo(+) = c.titulo and t.id = c.titulo and ls.id = c.situacao and lp.id(+) = c.periodo_tipo and c.titulo = :tituloId order by c.ordem", EsquemaBanco);

				comando.AdicionarParametroEntrada("tituloId", tituloId);

				IEnumerable<IDataReader> daReader = DaHelper.ObterLista(comando, bancoDeDados);

				foreach (var item in daReader)
				{
					TituloCondicionante condicionante = new TituloCondicionante();
					condicionante.Id = Convert.ToInt32(item["id"]);
					condicionante.Tid = item["tid"].ToString();
					condicionante.tituloId = Convert.ToInt32(item["titulo"]);
					condicionante.tituloNumero = item["titulo_numero"].ToString();

					condicionante.Situacao.Id = Convert.ToInt32(item["situacao_id"]);
					condicionante.Situacao.Texto = item["situacao_texto"].ToString();
					condicionante.Descricao = item["descricao"].ToString();

					condicionante.PossuiPrazo = Convert.ToInt32(item["possui_prazo"]) > 0;

					if (item["prazo"] != null && !Convert.IsDBNull(item["prazo"]))
					{
						condicionante.Prazo = Convert.ToInt32(item["prazo"]);
					}

					if (item["dias_prorrogados"] != null && !Convert.IsDBNull(item["dias_prorrogados"]))
					{
						condicionante.DiasProrrogados = Convert.ToInt32(item["dias_prorrogados"]);
					}

					if (item["cond_data_criacao"] != null && !Convert.IsDBNull(item["cond_data_criacao"]))
					{
						condicionante.DataCriacao.Data = Convert.ToDateTime(item["cond_data_criacao"]);
					}

					if (item["data_vencimento"] != null && !Convert.IsDBNull(item["data_vencimento"]))
					{
						condicionante.DataVencimento.Data = Convert.ToDateTime(item["data_vencimento"]);
					}

					if (item["periodo"] != null && !Convert.IsDBNull(item["periodo"]))
					{
						condicionante.PeriodicidadeValor = Convert.ToInt32(item["periodo"]);
					}

					condicionante.PossuiPeriodicidade = Convert.ToInt32(item["periodicidade"]) > 0;

					if (item["periodo"] != null && !Convert.IsDBNull(item["periodo"]))
					{
						condicionante.PeriodicidadeValor = Convert.ToInt32(item["periodo"]);
					}

					if (item["periodo_tipo_id"] != null && !Convert.IsDBNull(item["periodo_tipo_id"]))
					{
						condicionante.PeriodicidadeTipo.Id = Convert.ToInt32(item["periodo_tipo_id"]);
						condicionante.PeriodicidadeTipo.Texto = item["periodo_tipo_texto"].ToString();
					}

					comando = bancoDeDados.CriarComando(@"select p.id, p.situacao, ls.texto situacao_texto, p.dias_prorrogados, p.data_inicio, p.data_vencimento, p.tid
					from {0}tab_titulo_condicionantes_peri p, {0}lov_titulo_cond_situacao ls where p.situacao = ls.id and p.condicionante = :id", EsquemaBanco);

					comando.AdicionarParametroEntrada("id", condicionante.Id, DbType.Int32);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						TituloCondicionantePeriodicidade periodicidade = null;
						while (reader.Read())
						{
							periodicidade = new TituloCondicionantePeriodicidade();
							periodicidade.Id = Convert.ToInt32(reader["id"]);
							periodicidade.Tid = reader["tid"].ToString();
							periodicidade.Situacao.Id = Convert.ToInt32(reader["situacao"]);
							periodicidade.Situacao.Texto = reader["situacao_texto"].ToString();

							if (reader["dias_prorrogados"] != null && !Convert.IsDBNull(reader["dias_prorrogados"]))
							{
								periodicidade.DiasProrrogados = Convert.ToInt32(reader["dias_prorrogados"]);
							}

							if (reader["data_inicio"] != null && !Convert.IsDBNull(reader["data_inicio"]))
							{
								periodicidade.DataInicioPrazo.Data = Convert.ToDateTime(reader["data_inicio"]);
							}

							if (reader["data_vencimento"] != null && !Convert.IsDBNull(reader["data_vencimento"]))
							{
								periodicidade.DataVencimento.Data = Convert.ToDateTime(reader["data_vencimento"]);
							}

							condicionante.Periodicidades.Add(periodicidade);
						}

						reader.Close();
					}

					lst.Add(condicionante);
				}
			}
			return lst;
		}

		internal List<TituloExploracaoFlorestal> ObterExploracoes(int tituloId)
		{
			List<TituloExploracaoFlorestal> lst = new List<TituloExploracaoFlorestal>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select
						te.id, te.titulo, te.exploracao_florestal,
						concat(concat(concat(lv.chave, lpad(to_char(e.codigo_exploracao), 3, '0')), '-'), to_char(e.data_cadastro, 'ddMMyyyy')) localizador
					from {0}tab_titulo_exp_florestal te
					inner join {0}crt_exploracao_florestal e
						on (te.exploracao_florestal = e.id)
					left join idafgeo.lov_tipo_exploracao lv
						on (e.tipo_exploracao = lv.tipo_atividade)
					where te.titulo = :tituloId
					", EsquemaBanco);

				comando.AdicionarParametroEntrada("tituloId", tituloId);

				IEnumerable<IDataReader> daReader = DaHelper.ObterLista(comando, bancoDeDados);

				foreach (var item in daReader)
				{
					TituloExploracaoFlorestal tituloExploracao = new TituloExploracaoFlorestal();
					tituloExploracao.Id = Convert.ToInt32(item["id"]);
					tituloExploracao.TituloId = Convert.ToInt32(item["titulo"]);
					tituloExploracao.ExploracaoFlorestalId = Convert.ToInt32(item["exploracao_florestal"]);

					if (item["localizador"] != null && !Convert.IsDBNull(item["localizador"]))
						tituloExploracao.ExploracaoFlorestalTexto = item["localizador"].ToString();

					comando = bancoDeDados.CriarComando(@"select tp.id, tp.titulo_exploracao_florestal,
								tp.exp_florestal_exploracao, ep.identificacao, ep.autorizacao_sinaflor_id
							from {0}tab_titulo_exp_flor_exp tp
							inner join {0}crt_exp_florestal_exploracao ep
								on (tp.exp_florestal_exploracao = ep.id)
                            where tp.titulo_exploracao_florestal = :titulo_exploracao_id", EsquemaBanco);

					comando.AdicionarParametroEntrada("titulo_exploracao_id", tituloExploracao.Id, DbType.Int32);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						TituloExploracaoFlorestalExploracao tituloExploracaoFlorestalExploracao = null;
						while (reader.Read())
						{
							tituloExploracaoFlorestalExploracao = new TituloExploracaoFlorestalExploracao();
							tituloExploracaoFlorestalExploracao.Id = Convert.ToInt32(reader["id"]);
							tituloExploracaoFlorestalExploracao.TituloExploracaoFlorestalId = Convert.ToInt32(reader["titulo_exploracao_florestal"]);
							tituloExploracaoFlorestalExploracao.ExploracaoFlorestalExploracaoId = Convert.ToInt32(reader["exp_florestal_exploracao"]);

							if (reader["identificacao"] != null && !Convert.IsDBNull(reader["identificacao"]))
								tituloExploracaoFlorestalExploracao.ExploracaoFlorestalExploracaoTexto = reader["identificacao"].ToString();
							if (reader["autorizacao_sinaflor_id"] != null && !Convert.IsDBNull(reader["autorizacao_sinaflor_id"]))
								tituloExploracaoFlorestalExploracao.AutorizacaoSinaflorId = Convert.ToInt32(reader["autorizacao_sinaflor_id"]);

							tituloExploracao.TituloExploracaoFlorestalExploracaoList.Add(tituloExploracaoFlorestalExploracao);
						}

						reader.Close();
					}

					lst.Add(tituloExploracao);
				}
			}
			return lst;
		}

		internal List<TituloExploracaoFlorestal> ObterExploracoesTituloAssociado(int tituloId)
		{
			List<TituloExploracaoFlorestal> lst = new List<TituloExploracaoFlorestal>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select
						te.id, te.titulo, te.exploracao_florestal,
						concat(concat(concat(lv.chave, lpad(to_char(e.codigo_exploracao), 3, '0')), '-'), to_char(e.data_cadastro, 'ddMMyyyy')) localizador
					from {0}tab_titulo_exp_florestal te
					inner join {0}crt_exploracao_florestal e
						on (te.exploracao_florestal = e.id)
					left join idafgeo.lov_tipo_exploracao lv
						on (e.tipo_exploracao = lv.tipo_atividade)
					where te.titulo = :tituloId
					", EsquemaBanco);

				comando.AdicionarParametroEntrada("tituloId", tituloId);

				IEnumerable<IDataReader> daReader = DaHelper.ObterLista(comando, bancoDeDados);

				foreach (var item in daReader)
				{
					TituloExploracaoFlorestal tituloExploracao = new TituloExploracaoFlorestal();
					tituloExploracao.Id = Convert.ToInt32(item["id"]);
					tituloExploracao.TituloId = Convert.ToInt32(item["titulo"]);
					tituloExploracao.ExploracaoFlorestalId = Convert.ToInt32(item["exploracao_florestal"]);

					if (item["localizador"] != null && !Convert.IsDBNull(item["localizador"]))
						tituloExploracao.ExploracaoFlorestalTexto = item["localizador"].ToString();

					comando = bancoDeDados.CriarComando(@"select ep.id, ep.identificacao
							from {0}crt_exp_florestal_exploracao ep
                            where ep.parecer_favoravel = :parecer_favoravel
							and ep.exploracao_florestal = :exploracao_florestal
							and not exists (select 1 from tab_titulo_exp_flor_exp t
							where t.exp_florestal_exploracao = ep.id)", EsquemaBanco);

					comando.AdicionarParametroEntrada("exploracao_florestal", tituloExploracao.ExploracaoFlorestalId, DbType.Int32);
					comando.AdicionarParametroEntrada("parecer_favoravel", true, DbType.Boolean);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						TituloExploracaoFlorestalExploracao tituloExploracaoFlorestalExploracao = null;
						while (reader.Read())
						{
							tituloExploracaoFlorestalExploracao = new TituloExploracaoFlorestalExploracao();
							tituloExploracaoFlorestalExploracao.ExploracaoFlorestalExploracaoId = Convert.ToInt32(reader["id"]);
							if (reader["identificacao"] != null && !Convert.IsDBNull(reader["identificacao"]))
								tituloExploracaoFlorestalExploracao.ExploracaoFlorestalExploracaoTexto = reader["identificacao"].ToString();

							tituloExploracao.TituloExploracaoFlorestalExploracaoList.Add(tituloExploracaoFlorestalExploracao);
						}

						reader.Close();
					}

					if(tituloExploracao?.TituloExploracaoFlorestalExploracaoList?.Count() > 0)
						lst.Add(tituloExploracao);
				}
			}
			return lst;
		}

		internal List<TituloAssinante> ObterAssinantes(int id)
		{
			List<TituloAssinante> lst = new List<TituloAssinante>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id, f.id funcionario_id, f.nome funcionario_nome, c.id cargo_id, c.codigo cargo_codigo, c.nome cargo_nome 
				from {0}tab_titulo_assinantes t, {0}tab_funcionario f, {0}tab_cargo c where t.funcionario = f.id and t.cargo = c.id and t.titulo = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id);

				IEnumerable<IDataReader> daReader = DaHelper.ObterLista(comando, bancoDeDados);

				foreach (var item in daReader)
				{
					lst.Add(new TituloAssinante()
					{
						Id = Convert.ToInt32(item["id"]),
						FuncionarioId = Convert.ToInt32(item["funcionario_id"]),
						FuncionarioNome = item["funcionario_nome"].ToString(),
						FuncionarioCargoId = Convert.ToInt32(item["cargo_id"]),
						FuncionarioCargoCodigo = Convert.ToInt32(item["cargo_codigo"]),
						FuncionarioCargoNome = item["cargo_nome"].ToString()
					});
				}
			}
			return lst;
		}

		internal List<Titulo> ObterAssociados(int id, BancoDeDados banco = null)
		{
			var lst = new List<Titulo>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				var comando = bancoDeDados.CriarComando(@"select s.id, s.associado_id, s.associado_tid, s.tid, ttn.numero || '/' || ttn.ano numeroAno, ttm.nome modelo_nome,
				ttm.id modelo_id, ttm.sigla, t.data_vencimento from {0}tab_titulo t, {0}tab_titulo_associados s, {0}tab_titulo_numero ttn, {0}tab_titulo_modelo ttm where s.associado_id = ttn.titulo 
				and ttn.modelo = ttm.id and t.id = s.associado_id and s.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Titulo item;
					while (reader.Read())
					{
						item = new Titulo();

						item.IdRelacionamento = Convert.ToInt32(reader["id"]);

						item.Tid = reader["tid"].ToString();

						item.Numero.Texto = reader["numeroAno"].ToString();

						item.Modelo.Nome = reader["modelo_nome"].ToString();
						item.Modelo.Id = Convert.ToInt32(reader["modelo_id"]);
						item.Modelo.Sigla = reader["sigla"].ToString();

						if (reader["associado_id"] != null && !Convert.IsDBNull(reader["associado_id"]))
						{
							item.Id = Convert.ToInt32(reader["associado_id"]);
						}

						item.Tid = reader["associado_tid"].ToString();

						item.DataVencimento = new DateTecno() { Data = reader.GetValue<DateTime>("data_vencimento") };

						lst.Add(item);
					}
					reader.Close();
				}
			}

			return lst;
		}

		public List<AtividadeSolicitada> ObterAtividadesSimples(int tituloId, BancoDeDados banco = null)
		{
			List<AtividadeSolicitada> lst = new List<AtividadeSolicitada>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select s.id, s.atividade, pa.atividade atividade_nome, s.protocolo 
				from {0}tab_titulo_atividades s, {0}tab_atividade pa where s.atividade = pa.id and s.titulo = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", tituloId);

				IEnumerable<IDataReader> daReader = DaHelper.ObterLista(comando, bancoDeDados);

				foreach (var item in daReader)
				{
					lst.Add(new AtividadeSolicitada()
					{
						Id = Convert.ToInt32(item["atividade"]),
						IdRelacionamento = Convert.ToInt32(item["id"]),
						Texto = item["atividade_nome"].ToString(),
						IsAtivo = true
					});
				}
			}
			return lst;
		}

		public List<Atividade> ObterAtividades(int tituloId, int? atividadeId = null, BancoDeDados banco = null)
		{
			List<Atividade> lst = new List<Atividade>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select s.id, s.atividade, pa.atividade atividade_nome, s.protocolo
				from {0}tab_titulo_atividades s, {0}tab_atividade pa where s.atividade = pa.id and s.titulo = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", tituloId);

				if (atividadeId != null)
				{
					comando.DbCommand.CommandText += " and pa.id = :atividade ";
					comando.AdicionarParametroEntrada("atividade", atividadeId);
				}

				IEnumerable<IDataReader> daReader = DaHelper.ObterLista(comando, bancoDeDados);

				Atividade atividade = null;

				foreach (var item in daReader)
				{
					atividade = new Atividade();

					atividade.Id = Convert.ToInt32(item["atividade"]);
					atividade.NomeAtividade = item["atividade_nome"].ToString();

					if (!Convert.IsDBNull(item["protocolo"]))
					{
						atividade.Protocolo.Id = Convert.ToInt32(item["protocolo"]);
					}

					lst.Add(atividade);
				}
			}
			return lst;
		}

		internal ProtocoloEsp ObterProcDocReqEspecificidade(int tituloId, BancoDeDados banco = null)
		{
			ProtocoloEsp procotolo = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select ta.atividade, ta.protocolo, p.requerimento, p.protocolo protocolo_tipo 
															from {0}tab_titulo_atividades ta, {0}tab_protocolo p where ta.protocolo = p.id 
															and ta.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", tituloId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					if (reader.Read())
					{
						procotolo = new ProtocoloEsp();
						procotolo.Id = Convert.ToInt32(reader["protocolo"]);
						procotolo.IsProcesso = Convert.ToInt32(reader["protocolo_tipo"]) == 1;
						procotolo.RequerimentoId = Convert.ToInt32(reader["requerimento"]);
					}

					reader.Close();

					#endregion
				}
			}

			return procotolo;

		}

		public List<Atividade> ObterAtividadesAtual(int titulo, bool isProcesso = true, BancoDeDados banco = null)
		{
			List<Atividade> lst = new List<Atividade>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select s.id, s.atividade, ta.atividade atividade_nome, s.protocolo
				from {0}tab_protocolo_atividades pa, {0}tab_titulo_atividades s, {0}tab_atividade ta, {0}tab_titulo t 
				where t.protocolo = pa.protocolo and pa.atividade = s.atividade and s.titulo = t.id  and s.atividade = ta.id and t.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", titulo);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					Atividade atividade;
					while (reader.Read())
					{
						atividade = new Atividade();
						atividade.Id = Convert.ToInt32(reader["atividade"]);
						atividade.IdRelacionamento = Convert.ToInt32(reader["id"]);
						atividade.NomeAtividade = reader["atividade_nome"].ToString();

						if (!Convert.IsDBNull(reader["protocolo"]))
						{
							atividade.Protocolo.Id = Convert.ToInt32(reader["protocolo"]);
							atividade.Protocolo.IsProcesso = isProcesso;
						}

						lst.Add(atividade);
					}

					reader.Close();

					#endregion
				}
			}
			return lst;
		}

		public List<Anexo> ObterAnexos(int titulo, BancoDeDados banco = null)
		{
			List<Anexo> lst = new List<Anexo>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select a.id, a.ordem, a.descricao, a.croqui, b.nome, b.extensao, b.id arquivo_id, b.caminho,
				a.tid from {0}tab_titulo_arquivo a, {0}tab_arquivo b where a.arquivo = b.id and a.titulo = :titulo order by a.ordem", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					Anexo anexo;
					while (reader.Read())
					{
						anexo = new Anexo();
						anexo.Id = Convert.ToInt32(reader["id"]);
						anexo.Ordem = Convert.ToInt32(reader["ordem"]);
						anexo.Descricao = reader["descricao"].ToString();
						anexo.Croqui = Convert.ToBoolean(Convert.IsDBNull(reader["croqui"]) ? false : reader["croqui"]);

						anexo.Arquivo.Id = Convert.ToInt32(reader["arquivo_id"]);
						anexo.Arquivo.Caminho = reader["caminho"].ToString();
						anexo.Arquivo.Nome = reader["nome"].ToString();
						anexo.Arquivo.Extensao = reader["extensao"].ToString();

						anexo.Tid = reader["tid"].ToString();

						lst.Add(anexo);
					}

					reader.Close();

					#endregion
				}
			}
			return lst;
		}

		internal List<String> ObterEmails(int id, BancoDeDados banco = null)
		{
			List<String> lst = new List<String>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select c.valor email from {0}tab_titulo_pessoas s, {0}tab_pessoa p, {0}tab_pessoa_meio_contato c 
					where s.pessoa = p.id and s.tipo = 1 and s.titulo = :id and p.id = c.pessoa and c.meio_contato = 5", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id);

				IEnumerable<IDataReader> daReader = DaHelper.ObterLista(comando, bancoDeDados);

				foreach (var item in daReader)
				{
					lst.Add(item["email"].ToString());
				}
			}
			return lst;
		}

		internal List<PessoaLst> ObterDestinatarios(int id)
		{
			List<PessoaLst> lst = new List<PessoaLst>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Interessados do título

				Comando comando = bancoDeDados.CriarComando(@"select pe.id, nvl(pe.nome, pe.razao_social) nome_razao from 
				(select r.responsavel id from {0}tab_protocolo p, {0}tab_empreendimento_responsavel r where p.empreendimento = r.empreendimento and p.id = :id
				union select p.interessado id  from {0}tab_protocolo p where p.id = :id) p, tab_pessoa pe where pe.id = p.id order by nvl(pe.nome, pe.razao_social)", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					PessoaLst item;

					while (reader.Read())
					{
						item = new PessoaLst();
						item.Id = Convert.ToInt32(reader["id"]);
						item.Texto = reader["nome_razao"].ToString();
						item.IsAtivo = true;
						lst.Add(item);
					}

					reader.Close();
				}

				#endregion
			}

			return lst;
		}

        internal Resultados<Titulo> ObterPorEmpreendimento(int empreendimentoId, BancoDeDados banco = null)
        {
            Resultados<Titulo> retorno = new Resultados<Titulo>();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando = bancoDeDados.CriarComando(@"
                        SELECT ID, SITUACAO, EMPREENDIMENTO FROM TAB_TITULO WHERE MODELO = (select id from tab_titulo_modelo where codigo = 49 /*Cadastro Ambiental Rural*/)
                                        AND SITUACAO != 5 /*Encerrado*/ AND SITUACAO = 3/*Concluido*/ AND EMPREENDIMENTO = :empreendimento", EsquemaBanco);

                comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        Titulo titulo = new Titulo();
                        titulo.Id = reader.GetValue<int>("ID");
                        titulo.Situacao.Id = reader.GetValue<int>("SITUACAO");
                        titulo.EmpreendimentoId = reader.GetValue<int>("EMPREENDIMENTO");
                        retorno.Itens.Add(titulo);
                    }

                    reader.Close();
                }
            }

            return retorno;
        }

		internal bool ExistsTituloAssociadoNaoEncerrado(int laudoId)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(1) from {0}tab_titulo_associados s
				left join {0}tab_titulo t
				on (t.id = s.titulo)
				where s.associado_id = :laudo_id
				and t.situacao <> :situacao", EsquemaBanco);

				comando.AdicionarParametroEntrada("laudo_id", laudoId, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", (int)eTituloSituacao.Encerrado, DbType.Int32);

				return (bancoDeDados.ExecutarScalar<int>(comando) > 0);
			}
		}
		internal List<List<string>> GerarRelatorio(TituloRelatorioFiltro filtro, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				BarragemDispensaLicencaDa _barragemBus = new BarragemDispensaLicencaDa();
				List<List<string>> retorno = new List<List<string>>();
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				if(filtro.modelo > 0)
					comandtxt += comando.FiltroAnd("tm.id", "modelo", filtro.modelo);

				if (filtro.municipio > 0)
					comandtxt += $@"AND EXISTS (SELECT 1 FROM TAB_EMPREENDIMENTO_ENDERECO EE WHERE EE.CORRESPONDENCIA = 0 AND EE.EMPREENDIMENTO = BD.EMPREENDIMENTO AND EE.MUNICIPIO = {filtro.municipio} )";
					//	comando.FiltroAnd("mp.id", "", "municipio", filtro.municipio.ToString());

				if (!string.IsNullOrWhiteSpace(filtro.inicioPeriodo) && !string.IsNullOrWhiteSpace(filtro.fimPeriodo))
					comandtxt += $@"AND tt.data_criacao BETWEEN TO_DATE ('{filtro.inicioPeriodo}', 'DD/MM/YYYY') - 1 + INTERVAL '1' SECOND
										AND TO_DATE ('{filtro.fimPeriodo}', 'DD/MM/YYYY') + 1 - INTERVAL '1' SECOND OR
									tt.data_situacao BETWEEN TO_DATE ('{filtro.inicioPeriodo}', 'DD/MM/YYYY') - 1 + INTERVAL '1' SECOND
										AND TO_DATE ('{filtro.fimPeriodo}', 'DD/MM/YYYY') + 1 - INTERVAL '1' SECOND";


				if (!string.IsNullOrWhiteSpace(filtro.nomeRazaoSocial))
					comandtxt += comando.FiltroAndLike("pc.nome", "nomeRazaoSocial", filtro.nomeRazaoSocial, upper: true, likeInicio: true);
								
				if (!string.IsNullOrWhiteSpace(filtro.cpfCnpj))
				{
					if(filtro.isCpf)
						comandtxt += comando.FiltroIn("rq.interessado", "select p.id from tab_pessoa p where p.CPF = :cpfCnpj", "cpfCnpj", filtro.cpfCnpj);
					else
						comandtxt += comando.FiltroIn("rq.interessado", "select p.id from tab_pessoa p where p.CNPJ = :cpfCnpj", "cpfCnpj", filtro.cpfCnpj);
				}

				#endregion

				#region colunas
				List<string> colunas = new List<string>() {
					"numeto_titulo",
					"requerimento",
					"autor_nome",
					"autor_cpf",
					"setor_nome",
					"setor_sigla",
					"modelo_nome",
					"modelo_sigla",
					"situacao",
					"situacao_motivo",
					"local_emissao",
					"codigo_empreendimento",
					"empreendimento",
					"prazo",
					"prazo_unidade",
					"dias_prorrogados",
					"data_criacao",
					"data_inicio",
					"data_emissao",
					"data_assinatura",
					"data_vencimento",
					"data_encerramento",
					"email",
					"motivo_suspensao",
					"fase_barragem",
					"tipo_barragem",
					"area_alagada",
					"volume_armazenado",
					"altura_barramento",
					"largura_base_barramento",
					"comprimento_barramento",
					"largura_crista_barramento",
					"coordenada_barramento",
					"coordenada_area_bota_fora",
					"coordenada_area_emprestimo",
					"curso_hidrico",
					"area_bacia_contribuicao",
					"precipitacao",
					"fonte_precipitacao",
					"periodo_retorno",
					"coeficiente_escoamento",
					"fonte_coeficiente_escoamento",
					"tempo_concentracao",
					"equacao_tempo_concentracao",
					"vazao_enchente",
					"fonte_vazao_enchente",
					"supressao_app_implant_barragem",
					"fx_demarc_app_entorno_reserv",
					"largura_demarcada",
					"largura_demarcada_legislacao",
					"faixa_cercada",
					"descricao_estagio_desenv",
					"barragem_dentro_das_normas",
					"barramento_adequacoes",
					"vazao_min_tipo",
					"vazao_min_diametro",
					"vazao_min_instalado",
					"vazao_min_normas",
					"vazao_min_adequacoes",
					"vazao_max_tipo",
					"vazao_max_larg_alt_diametro",
					"vazao_max_instalado",
					"vazao_max_normas",
					"vazao_max_adequacoes",
					"supressao_app_implant_barragem",
					//"vazao_min_tipo",
					//"vazao_min_diametro",
					//"vazao_max_tipo",
					//"vazao_max_larg_alt_diametro",
					"periodo_inicio_obra",
				   "periodo_termino_obra",
					"atividades",
					"rt_" + eTipoRT.ElaboracaoDeclaracao.ToDescription() + "_nome",
					"rt_" + eTipoRT.ElaboracaoDeclaracao.ToDescription() + "_profissao",
					"rt_" + eTipoRT.ElaboracaoDeclaracao.ToDescription() + "_registro_crea",
					"rt_" + eTipoRT.ElaboracaoDeclaracao.ToDescription() + "_numero_art",

					"rt_" + eTipoRT.ElaboracaoProjeto.ToDescription() + "_nome",
					"rt_" + eTipoRT.ElaboracaoProjeto.ToDescription() + "_profissao",
					"rt_" + eTipoRT.ElaboracaoProjeto.ToDescription() + "_registro_crea",
					"rt_" + eTipoRT.ElaboracaoProjeto.ToDescription() + "_numero_art",

					"rt_" + eTipoRT.ExecucaoBarragem.ToDescription() + "_nome",
					"rt_" + eTipoRT.ExecucaoBarragem.ToDescription() + "_profissao",
					"rt_" + eTipoRT.ExecucaoBarragem.ToDescription() + "_registro_crea",
					"rt_" + eTipoRT.ExecucaoBarragem.ToDescription() + "_numero_art",

					"rt_" + eTipoRT.ElaboracaoEstudoAmbiental.ToDescription() + "_nome",
					"rt_" + eTipoRT.ElaboracaoEstudoAmbiental.ToDescription() + "_profissao",
					"rt_" + eTipoRT.ElaboracaoEstudoAmbiental.ToDescription() + "_registro_crea",
					"rt_" + eTipoRT.ElaboracaoEstudoAmbiental.ToDescription() + "_numero_art",

					"rt_" + eTipoRT.ElaboracaoPlanoRecuperacao.ToDescription() + "_nome",
					"rt_" + eTipoRT.ElaboracaoPlanoRecuperacao.ToDescription() + "_profissao",
					"rt_" + eTipoRT.ElaboracaoPlanoRecuperacao.ToDescription() + "_registro_crea",
					"rt_" + eTipoRT.ElaboracaoPlanoRecuperacao.ToDescription() + "_numero_art",

					"rt_" + eTipoRT.ExecucaoPlanoRecuperacao.ToDescription() + "_nome",
					"rt_" + eTipoRT.ExecucaoPlanoRecuperacao.ToDescription() + "_profissao",
					"rt_" + eTipoRT.ExecucaoPlanoRecuperacao.ToDescription() + "_registro_crea",
					"rt_" + eTipoRT.ExecucaoPlanoRecuperacao.ToDescription() + "_numero_art"
				};
				retorno.Add(colunas);
				#endregion

				#region consulta
				comando.DbCommand.CommandText = String.Format(@"
				select
						bd.id barragem_id,
						tn.numero || '/' || tn.ano numeto_titulo,
						tt.requerimento,
						pc.nome autor_nome,
						pc.cpf autor_cpf,
						se.nome setor_nome,
						se.sigla setor_sigla,
						tm.nome modelo_nome,
						tm.sigla modelo_sigla,
						ls.texto situacao,
						lm.texto situacao_motivo,
						mp.texto local_emissao,
						ee.codigo codigo_empreendimento,
						ee.denominador empreendimento,
						tt.prazo,
						tt.prazo_unidade,
						tt.dias_prorrogados,
						tt.data_criacao,
						tt.data_inicio,
						tt.data_emissao,
						tt.data_assinatura,
						tt.data_vencimento,
						tt.data_encerramento, 
						tt.email, 
						tt.motivo_suspensao,
						lf.texto fase_barragem,
						lt.texto tipo_barragem,
						bd.area_alagada,
						bd.volume_armazenado,
						bd.altura_barramento,
						bd.largura_base_barramento,
						bd.comprimento_barramento,
						bd.largura_crista_barramento,
						(select 'N: ' || c1.northing || '  E: ' || c1.easting from crt_barragem_coordenada c1 where c1.barragem = bd.id and c1.tipo = 1) coordenada_barramento,
						(select 'N: ' || c1.northing || '  E: ' || c1.easting from crt_barragem_coordenada c1 where c1.barragem = bd.id and c1.tipo = 2) coordenada_area_bota_fora,
						(select 'N: ' || c1.northing || '  E: ' || c1.easting from crt_barragem_coordenada c1 where c1.barragem = bd.id and c1.tipo = 3) coordenada_area_emprestimo,
						bd.curso_hidrico,
						bd.area_bacia_contribuicao,
						bd.precipitacao,
						bd.fonte_precipitacao,
						bd.periodo_retorno,
						bd.coeficiente_escoamento,
						bd.fonte_coeficiente_escoamento,
						bd.tempo_concentracao,
						bd.equacao_calculo equacao_tempo_concentracao,
						bd.vazao_enchente,
						bd.fonte_vazao_enchente,
						(case bc.supressao_app when 1 then 'Sim' when 0 then 'Não' else '' end) supressao_app_implant_barragem,
						(case bc.demarcacao_app when 1 then 'Sim' when 0 then 'Não' when 2 then 'Não se aplica' else '' end) fx_demarc_app_entorno_reserv,
						bc.largura_demarcada,
						(case bc.largura_demarcada_legislacao when 1 then 'Sim' when 0 then 'Não' else '' end) largura_demarcada_legislacao,
						(case bc.faixa_cercada when 1 then 'Sim' when 0 then 'Não' when 2 then 'Parcialmente' else '' end) faixa_cercada,
						bc.descricao_desen_app descricao_estagio_desenv,
						(case bc.barramento_normas when 1 then 'Sim'when 0 then 'Não' else 'null' end) barragem_dentro_das_normas,
						bc.barramento_adequacoes,
						li.texto vazao_min_tipo,
						bc.vazao_min_diametro,
						(case bc.vazao_min_instalado when 1 then 'Sim' when 0 then 'Não' else 'null' end) vazao_min_instalado,
						(case bc.vazao_min_normas when 1 then 'Sim' when 0 then 'Não' else 'null' end) vazao_min_normas,
						bc.vazao_min_adequacoes,
						la.texto vazao_max_tipo,
						bc.vazao_max_diametro vazao_max_larg_alt_diametro,
						(case bc.vazao_max_instalado when 1 then 'Sim' when 0 then 'Não' else 'null' end) vazao_max_instalado,
						(case bc.vazao_max_normas when 1 then 'Sim' when 0 then 'Não' else 'null' end) vazao_max_normas,
						bc.vazao_max_adequacoes,
						(case bc.supressao_app when 1 then 'Sim'when 0 then 'Não' else 'null' end) supressao_app_implant_barragem,
						/*li.texto vazao_min_tipo,
						bc.vazao_min_diametro,
						la.texto vazao_max_tipo,
						bc.vazao_max_diametro vazao_max_larg_alt_diametro,*/
						bc.periodo_inicio_obra,
						bc.periodo_termino_obra
        
        
				from tab_titulo tt 
					inner join tab_titulo_modelo                    tm on tm.id = tt.modelo
					left join  tab_setor                            se on tt.setor = se.id
					left join  tab_credenciado                      cr on tt.autor = cr.id
					left join  idafcredenciado.tab_pessoa           pc on cr.pessoa = pc.id 
					left join  lov_titulo_situacao                  ls on ls.id = tt.situacao
					left join  lov_titulo_situacao_motivo           lm on tt.situacao_motivo = lm.id
					left join tab_titulo_numero                    tn on tt.id = tn.titulo
					inner join lov_municipio                        mp on mp.id = tt.local_emissao
					inner join idafcredenciado.tab_empreendimento   ee on ee.id = tt.empreendimento
					inner join tab_requerimento                     rq on rq.id = tt.requerimento
					left join tab_requerimento_barragem            rb on rb.requerimento = rq.id
					left join crt_barragem_dispensa_lic            bd on bd.id = rb.barragem
					left join lov_crt_bdla_fase                    lf on lf.id = bd.fase
					left join lov_crt_bdla_barragem_tipo           lt on lt.id = bd.tipo_barragem
					left join crt_barragem_construida_con          bc on bd.id = bc.barragem
					left join LOV_CRT_BDLA_MONGE_TIPO              li on li.id = bc.vazao_min_tipo
					left join LOV_CRT_BDLA_VERTEDOURO_TIPO         la on la.id = bc.vazao_max_tipo 
    
				where 1 = 1 and tm.id = 72  " + comandtxt, (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));
				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						#region campos
						List<string> resultados = new List<string>();
						int barragemId = reader.GetValue<int>("barragem_id");
						BarragemDispensaLicenca caracterizacao = new BarragemDispensaLicenca();

						resultados.Add(reader.GetValue<string>("numeto_titulo"));
						resultados.Add(reader.GetValue<string>("requerimento"));
						resultados.Add(reader.GetValue<string>("autor_nome"));
						resultados.Add(reader.GetValue<string>("autor_cpf"));
						resultados.Add(reader.GetValue<string>("setor_nome"));
						resultados.Add(reader.GetValue<string>("setor_sigla"));
						resultados.Add(reader.GetValue<string>("modelo_nome"));
						resultados.Add(reader.GetValue<string>("modelo_sigla"));
						resultados.Add(reader.GetValue<string>("situacao"));
						resultados.Add(reader.GetValue<string>("situacao_motivo"));
						resultados.Add(reader.GetValue<string>("local_emissao"));
						resultados.Add(reader.GetValue<string>("codigo_empreendimento"));
						resultados.Add(reader.GetValue<string>("empreendimento"));
						resultados.Add(reader.GetValue<string>("prazo"));
						resultados.Add(reader.GetValue<string>("prazo_unidade"));
						resultados.Add(reader.GetValue<string>("dias_prorrogados"));
						resultados.Add(reader.GetValue<string>("data_criacao"));
						resultados.Add(reader.GetValue<string>("data_inicio"));
						resultados.Add(reader.GetValue<string>("data_emissao"));
						resultados.Add(reader.GetValue<string>("data_assinatura"));
						resultados.Add(reader.GetValue<string>("data_vencimento"));
						resultados.Add(reader.GetValue<string>("data_encerramento"));
						resultados.Add(reader.GetValue<string>("email"));
						resultados.Add(reader.GetValue<string>("motivo_suspensao"));
						resultados.Add(reader.GetValue<string>("fase_barragem"));
						resultados.Add(reader.GetValue<string>("tipo_barragem"));
						resultados.Add(reader.GetValue<string>("area_alagada"));
						resultados.Add(reader.GetValue<string>("volume_armazenado"));
						resultados.Add(reader.GetValue<string>("altura_barramento"));
						resultados.Add(reader.GetValue<string>("largura_base_barramento"));
						resultados.Add(reader.GetValue<string>("comprimento_barramento"));
						resultados.Add(reader.GetValue<string>("largura_crista_barramento"));
						resultados.Add(reader.GetValue<string>("coordenada_barramento"));
						resultados.Add(reader.GetValue<string>("coordenada_area_bota_fora"));
						resultados.Add(reader.GetValue<string>("coordenada_area_emprestimo"));
						resultados.Add(reader.GetValue<string>("curso_hidrico"));
						resultados.Add(reader.GetValue<string>("area_bacia_contribuicao"));
						resultados.Add(reader.GetValue<string>("precipitacao"));
						resultados.Add(reader.GetValue<string>("fonte_precipitacao"));
						resultados.Add(reader.GetValue<string>("periodo_retorno"));
						resultados.Add(reader.GetValue<string>("coeficiente_escoamento"));
						resultados.Add(reader.GetValue<string>("fonte_coeficiente_escoamento"));
						resultados.Add(reader.GetValue<string>("tempo_concentracao"));
						resultados.Add(reader.GetValue<string>("equacao_tempo_concentracao"));
						resultados.Add(reader.GetValue<string>("vazao_enchente"));
						resultados.Add(reader.GetValue<string>("fonte_vazao_enchente"));
						resultados.Add(reader.GetValue<string>("supressao_app_implant_barragem"));
						resultados.Add(reader.GetValue<string>("fx_demarc_app_entorno_reserv"));
						resultados.Add(reader.GetValue<string>("largura_demarcada"));
						resultados.Add(reader.GetValue<string>("largura_demarcada_legislacao"));
						resultados.Add(reader.GetValue<string>("faixa_cercada"));
						var descricao_estagio_desenv = reader.GetValue<string>("descricao_estagio_desenv");
						descricao_estagio_desenv = String.IsNullOrWhiteSpace(descricao_estagio_desenv) ?
							descricao_estagio_desenv : descricao_estagio_desenv.Replace("\n", String.Empty).Replace("\r", String.Empty);
						resultados.Add(descricao_estagio_desenv);
						resultados.Add(reader.GetValue<string>("barragem_dentro_das_normas"));
						var barramento_adequacoes  = reader.GetValue<string>("barramento_adequacoes");
						barramento_adequacoes = String.IsNullOrWhiteSpace(barramento_adequacoes) ?
							barramento_adequacoes : barramento_adequacoes.Replace("\n", String.Empty).Replace("\r", String.Empty);
							resultados.Add(barramento_adequacoes);
						resultados.Add(reader.GetValue<string>("vazao_min_tipo"));
						resultados.Add(reader.GetValue<string>("vazao_min_diametro"));
						resultados.Add(reader.GetValue<string>("vazao_min_instalado"));
						resultados.Add(reader.GetValue<string>("vazao_min_normas"));
						var vazao_min_adequacoes = reader.GetValue<string>("vazao_min_adequacoes");
						vazao_min_adequacoes = String.IsNullOrWhiteSpace(vazao_min_adequacoes) ?
							vazao_min_adequacoes : vazao_min_adequacoes.Replace("\n", String.Empty).Replace("\r", String.Empty);
							resultados.Add(vazao_min_adequacoes);
						resultados.Add(reader.GetValue<string>("vazao_max_tipo"));
						resultados.Add(reader.GetValue<string>("vazao_max_larg_alt_diametro"));
						resultados.Add(reader.GetValue<string>("vazao_max_instalado"));
						resultados.Add(reader.GetValue<string>("vazao_max_normas"));
						var vazao_max_adequacoes  = reader.GetValue<string>("vazao_max_adequacoes");
						vazao_max_adequacoes = String.IsNullOrWhiteSpace(vazao_max_adequacoes) ?
							vazao_max_adequacoes : vazao_max_adequacoes.Replace("\n", String.Empty).Replace("\r", String.Empty);
						resultados.Add(vazao_max_adequacoes);
						resultados.Add(reader.GetValue<string>("supressao_app_implant_barragem"));
						resultados.Add(reader.GetValue<string>("periodo_inicio_obra"));
						resultados.Add(reader.GetValue<string>("periodo_termino_obra"));

						var atividades = String.Join(" / ", _barragemBus.ObterListaFinalidadeAtividade(barragemId));
						if (String.IsNullOrWhiteSpace(atividades))
							resultados.Add("(null)");
						else
							resultados.Add(atividades);

						#region Responsaveis Tecnicos
						Comando cmd = bancoDeDados.CriarComando(@"
						select r.id, r.tipo, r.nome, p.texto profissao, r.registro_crea, r.numero_art, r.autorizacao_crea, r.proprio_declarante
							from crt_barragem_responsavel r  inner join tab_profissao p on p.id = r.profissao
						where r.barragem = :barragem");

						cmd.AdicionarParametroEntrada("barragem", barragemId, DbType.Int32);

						using (IDataReader rd = bancoDeDados.ExecutarReader(cmd))
						{
							while (rd.Read())
							{
								var obj = caracterizacao.responsaveisTecnicos.FirstOrDefault(x => (int)x.tipo == rd.GetValue<int>("tipo"));
								obj.id = rd.GetValue<int>("id");
								obj.tipo = (eTipoRT)rd.GetValue<int>("tipo");
								obj.nome = rd.GetValue<string>("nome");
								obj.profissao.Texto = rd.GetValue<string>("profissao");
								obj.registroCREA = rd.GetValue<string>("registro_crea");
								obj.numeroART = rd.GetValue<string>("numero_art");
								if (obj.tipo == eTipoRT.ElaboracaoProjeto)
									obj.autorizacaoCREA.Id = rd.GetValue<int>("autorizacao_crea");
								obj.proprioDeclarante = rd.GetValue<bool>("proprio_declarante");
							}
						}
						caracterizacao.responsaveisTecnicos.ForEach(x => {
							resultados.Add(x.nome);
							resultados.Add(x.profissao.Texto);
							resultados.Add(x.registroCREA);
							resultados.Add(x.numeroART);
						});
						retorno.Add(resultados);
						#endregion

						#endregion
					}

					reader.Close();
				}
				return retorno;
			}
		}

		#endregion

		#region Validações

		internal bool PossuiDeclaratorioForaSituacao(Atividade atividade, eTituloSituacao situacao, int empreendimentoID, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from tab_titulo t 
				where exists (select 1 from tab_titulo_atividades a where a.titulo = t.id and a.id = :atividade)
				and t.empreendimento = :empreendimento and t.situacao != :situacao", EsquemaBanco);

				comando.AdicionarParametroEntrada("atividade", atividade.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", (int)situacao, DbType.Int32);
				comando.AdicionarParametroEntrada("empreendimento", empreendimentoID, DbType.Int32);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0;
			}
		}

		internal bool VerificarNumero(Titulo titulo)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from {0}tab_titulo_numero tn where tn.modelo = :modelo and tn.numero = :numero and tn.ano = :ano", EsquemaBanco);
				comando.AdicionarParametroEntrada("modelo", titulo.Modelo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("numero", titulo.Numero.Inteiro, DbType.Int32);
				comando.AdicionarParametroEntrada("ano", titulo.Numero.Ano, DbType.Int32);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0;
			}
		}

		internal bool VerificarModeloEmitidoProcDoc(int modeloCodigo, int tituloId, int protocoloId)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id, t.situacao, t.situacao_motivo from tab_titulo t, tab_titulo_modelo tm, esp_oficio_notificacao e
				where tm.codigo = :modeloCodigo and tm.id = t.modelo and t.id = e.titulo and t.situacao <> 5 and e.protocolo = :protocoloId and t.id <> :tituloId", EsquemaBanco);

				comando.AdicionarParametroEntrada("tituloId", tituloId, DbType.Int32);
				comando.AdicionarParametroEntrada("protocoloId", protocoloId, DbType.Int32);
				comando.AdicionarParametroEntrada("modeloCodigo", modeloCodigo, DbType.Int32);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0;
			}
		}

		internal bool VerificarProcDocPossuiModelo(int modeloId, int protocolo, int? atividade = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select (select count(ta.id) qtd from {0}tab_protocolo_atividades ta, {0}tab_protocolo_ativ_finalida taf
				where ta.protocolo = :id and ta.id = taf.protocolo_ativ and taf.modelo = :modelo) +
				(select count(ta.id) qtd from {0}tab_protocolo_atividades ta, {0}tab_protocolo_ativ_finalida taf, {0}tab_protocolo_associado ap
				where ta.protocolo = ap.associado and ta.id = taf.protocolo_ativ and ap.protocolo = :id and taf.modelo = :modelo ) valor from dual", EsquemaBanco);

				if (atividade.GetValueOrDefault() > 0)
				{
					comando = bancoDeDados.CriarComando(@"select (select count(ta.id) qtd from {0}tab_protocolo_atividades ta, {0}tab_protocolo_ativ_finalida taf
					where ta.protocolo = :id and ta.id = taf.protocolo_ativ and taf.modelo = :modelo and ta.atividade = :atividade ) +
					(select count(ta.id) qtd from {0}tab_protocolo_atividades ta, {0}tab_protocolo_ativ_finalida taf, {0}tab_protocolo_associado ap
					where ta.protocolo = ap.associado and ta.id = taf.protocolo_ativ and ap.protocolo = :id and taf.modelo = :modelo and ta.atividade = :atividade )
					valor from dual", EsquemaBanco);

					comando.AdicionarParametroEntrada("atividade", atividade, DbType.Int32);
				}

				comando.AdicionarParametroEntrada("id", protocolo, DbType.Int32);
				comando.AdicionarParametroEntrada("modelo", modeloId, DbType.Int32);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0;
			}
		}

		internal bool VerificarRequerimentoPossuiModelo(int modeloId, int requerimento)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select (select count(ta.id) qtd from {0}tab_requerimento_atividade ta, {0}tab_requerimento_ativ_finalida taf
				where ta.requerimento = :id and ta.id = taf.requerimento_ativ and taf.modelo = :modelo) +
				(select count(ta.id) qtd from {0}tab_protocolo_atividades ta, {0}tab_protocolo_ativ_finalida taf, {0}tab_protocolo_associado ap
				where ta.protocolo = ap.associado and ta.id = taf.protocolo_ativ and ap.protocolo = :id and taf.modelo = :modelo ) valor from dual", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", requerimento, DbType.Int32);
				comando.AdicionarParametroEntrada("modelo", modeloId, DbType.Int32);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0;
			}
		}

		internal bool ProtocoloNoSetorDoModelo(int protocolo, int setorId)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(p.id) from {0}tab_protocolo p where p.id = :id and p.setor = :setor", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", protocolo, DbType.Int32);
				comando.AdicionarParametroEntrada("setor", setorId, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		public bool ValidarSituacaoTitulo(int tituloId, int situacao)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select situacao from {0}tab_titulo where id = :tituloId", EsquemaBanco);
				comando.AdicionarParametroEntrada("tituloId", tituloId, DbType.Int32);
				object obj = bancoDeDados.ExecutarScalar(comando);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) == situacao;
			}
		}

		internal int VerificarEhTituloAnterior(Titulo titulo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select nvl(max(af.finalidade),0) finalidade from (
					select f.finalidade 
					from {0}tab_protocolo_ativ_finalida f 
					where f.modelo = :modelo and f.titulo_anterior_tipo = 1 and f.titulo_anterior_id = :id
					union all
					select f.finalidade 
					from {0}tab_protocolo_ativ_finalida f 
					where f.modelo = :modelo and f.titulo_anterior_tipo = 1 and f.titulo_anterior_id = :id ) af", EsquemaBanco);

				comando.AdicionarParametroEntrada("modelo", titulo.Modelo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("id", titulo.Id, DbType.Int32);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));
			}
		}

		internal bool VerificarEhModeloCodigo(int titulo, int modeloCodigo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select m.codigo from tab_titulo t, tab_titulo_modelo m where t.id = :titulo and t.modelo = m.id", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) == modeloCodigo;
			}
		}

		#endregion
	}
}