using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloProcesso;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Data
{
	public class ProtocoloDa
	{
		#region Protocolo

		Historico _historico;
		Consulta _consulta;

		internal Historico Historico { get { return _historico; } }
		internal Consulta Consulta { get { return _consulta; } }
		private static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		private string EsquemaBanco { get; set; }

		#endregion

		public ProtocoloDa()
		{
			_historico = new Historico();
			_consulta = new Consulta();
		}

		#region Ações de DML

		public void Salvar(IProtocolo protocolo, BancoDeDados banco = null)
		{
			if (protocolo == null)
			{
				throw new Exception("Protocolo é nulo.");
			}

			if (!protocolo.Id.HasValue || protocolo.Id <= 0)
			{
				Criar(protocolo, banco);
			}
			else
			{
				Editar(protocolo, banco);
			}
		}

		internal int? Criar(IProtocolo protocolo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Documento doc = (protocolo.IsProcesso) ? new Documento() : protocolo as Documento;

				#region Protocolo

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}tab_protocolo e (id, numero, ano, numero_autuacao, data_autuacao, nome, tipo,
				protocolo, data_criacao, volume, situacao, interessado, requerimento, empreendimento, checagem, checagem_pendencia, setor, setor_criacao,
				protocolo_associado, emposse, arquivo, fiscalizacao, tid, interessado_livre, interessado_livre_telefone, folhas, assunto, descricao,
				destinatario, destinatario_setor, orgao_destino, cargo_destinatario, nome_destinatario, endereco_destinatario) 
				values ({0}seq_protocolo.nextval, (select nvl(max(p.numero) + 1, 1) from {0}tab_protocolo p where p.ano = :ano),
				:ano, :numero_autuacao, :data_autuacao, :nome, :tipo, :protocolo, :data_criacao, :volume, 1, :interessado, :requerimento, :empreendimento,
				:checagem, :checagem_pendencia, :setor, :setor_criacao, :protocolo_associado, :emposse, :arquivo, :fiscalizacao, :tid, :interessadoLivre,
				:interessadoLivreTel, :folhas, :assunto, :descricao, :destinatario, :destinatario_setor, :orgao_destino, :cargo_destinatario, :nome_destinatario,
				:endereco_destinatario) returning e.id, e.numero into :id, :numero", EsquemaBanco);

				comando.AdicionarParametroEntrada("tipo", protocolo.Tipo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("protocolo", (protocolo.IsProcesso) ? 1 : 2, DbType.Int32);
				comando.AdicionarParametroEntrada("ano", protocolo.DataCadastro.Data.Value.Year, DbType.Int32);
				comando.AdicionarParametroEntrada("numero_autuacao", (String.IsNullOrWhiteSpace(protocolo.NumeroAutuacao)) ? (object)DBNull.Value : protocolo.NumeroAutuacao, DbType.String);
				comando.AdicionarParametroEntrada("data_autuacao", (protocolo.DataAutuacao == null || String.IsNullOrWhiteSpace(protocolo.DataAutuacao.DataTexto)) ? (object)DBNull.Value : protocolo.DataAutuacao.DataTexto, DbType.DateTime);
				comando.AdicionarParametroEntrada("data_criacao", protocolo.DataCadastro.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("volume", protocolo.Volume ?? 0, DbType.Int32);
				comando.AdicionarParametroEntrada("checagem", protocolo.ChecagemRoteiro.Id == 0 ? (object)DBNull.Value : protocolo.ChecagemRoteiro.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("interessado", protocolo.Interessado.Id == 0 ? (object)DBNull.Value : protocolo.Interessado.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("requerimento", protocolo.Requerimento.Id == 0 ? (object)DBNull.Value : protocolo.Requerimento.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("fiscalizacao", protocolo.Fiscalizacao.Id == 0 ? (object)DBNull.Value : protocolo.Fiscalizacao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("empreendimento", (protocolo.Empreendimento.Id == 0) ? (object)DBNull.Value : protocolo.Empreendimento.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("arquivo", protocolo.Arquivo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("emposse", protocolo.Emposse.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("setor", protocolo.SetorId, DbType.Int32);
				comando.AdicionarParametroEntrada("setor_criacao", protocolo.SetorId, DbType.Int32);

				comando.AdicionarParametroEntrada("interessadoLivre", protocolo.InteressadoLivre, DbType.String);
				comando.AdicionarParametroEntrada("interessadoLivreTel", protocolo.InteressadoLivreTelefone, DbType.String);
				comando.AdicionarParametroEntrada("folhas", protocolo.Folhas, DbType.Int32);

				//Apenas documento
				comando.AdicionarParametroEntrada("destinatario", doc.Destinatario.Id > 0 ? (int?)doc.Destinatario.Id : null, DbType.Int32);
				comando.AdicionarParametroEntrada("destinatario_setor", doc.DestinatarioSetor.Id > 0 ? (int?)doc.DestinatarioSetor.Id : null, DbType.Int32);
				comando.AdicionarParametroEntrada("nome", DbType.String, 80, doc.Nome);
				comando.AdicionarParametroEntrada("assunto", DbType.String, 150, doc.Assunto);
				comando.AdicionarParametroEntrada("descricao", DbType.String, 4000, doc.Descricao);
				comando.AdicionarParametroEntrada("checagem_pendencia", doc.ChecagemPendencia.Id == 0 ? (object)DBNull.Value : doc.ChecagemPendencia.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("protocolo_associado", doc.ProtocoloAssociado.Id.GetValueOrDefault() == 0 ? (object)DBNull.Value : doc.ProtocoloAssociado.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("orgao_destino", DbType.String, 100, doc.OrgaoDestino);
				comando.AdicionarParametroEntrada("cargo_destinatario", DbType.String, 100, doc.CargoFuncaoDestinatario);
				comando.AdicionarParametroEntrada("nome_destinatario", DbType.String, 100, doc.NomeDestinatario);
				comando.AdicionarParametroEntrada("endereco_destinatario", DbType.String, 100, doc.EnderecoDestinatario);

				comando.AdicionarParametroSaida("id", DbType.Int32);
				comando.AdicionarParametroSaida("numero", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				protocolo.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
				protocolo.NumeroProtocolo = Convert.ToInt32(comando.ObterValorParametro("numero"));
				protocolo.Ano = protocolo.DataCadastro.Data.Value.Year;

				#endregion

				#region Atividades

				if (protocolo.Atividades != null && protocolo.Atividades.Count > 0)
				{
					AtividadeProtocoloDa _daAtividade = new AtividadeProtocoloDa();

					foreach (Atividade item in protocolo.Atividades)
					{
						item.Protocolo.Id = protocolo.Id.Value;
						item.Protocolo.IsProcesso = protocolo.IsProcesso;
						item.Protocolo.Requerimento.Id = protocolo.Requerimento.Id;
						_daAtividade.Salvar(item, bancoDeDados);
					}
				}

				#endregion

				#region Responsáveis

				if (protocolo.Responsaveis != null && protocolo.Responsaveis.Count > 0)
				{
					foreach (ResponsavelTecnico responsavel in protocolo.Responsaveis)
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}tab_protocolo_responsavel(id, protocolo, responsavel, funcao, numero_art, tid) values
						({0}seq_protocolo_responsavel.nextval, :protocolo, :responsavel, :funcao, :numero_art, :tid )", EsquemaBanco);

						comando.AdicionarParametroEntrada("protocolo", protocolo.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("responsavel", responsavel.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("funcao", responsavel.Funcao, DbType.Int32);
						comando.AdicionarParametroEntrada("numero_art", responsavel.NumeroArt, DbType.String);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Assinantes

				if (protocolo.Assinantes != null && protocolo.Assinantes.Count > 0)
				{
					foreach (TituloAssinante item in protocolo.Assinantes)
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}tab_protocolo_assinantes s (id, protocolo, funcionario, cargo, tid)
						values ({0}seq_protocolo_assinantes.nextval, :protocolo, :funcionario, :cargo, :tid)", EsquemaBanco);

						comando.AdicionarParametroEntrada("protocolo", protocolo.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("funcionario", item.FuncionarioId, DbType.Int32);
						comando.AdicionarParametroEntrada("cargo", item.FuncionarioCargoId, DbType.Int32);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Histórico/Consulta/Posse

				Historico.Gerar(protocolo.Id.Value, eHistoricoArtefato.protocolo, eHistoricoAcao.criar, bancoDeDados);
				Consulta.Gerar(protocolo.Id.Value, eHistoricoArtefato.protocolo, bancoDeDados);

				#endregion

				bancoDeDados.Commit();

				return protocolo.Id;
			}
		}

		internal void Editar(IProtocolo protocolo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Protocolo

				Documento doc = (protocolo.IsProcesso) ? new Documento() : protocolo as Documento;

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_protocolo p set p.nome = :nome, p.volume = :volume, p.numero_autuacao =
				:numero_autuacao, p.data_autuacao = :data_autuacao, p.checagem = :checagem, p.requerimento = :requerimento, 
				p.interessado = :interessado, p.empreendimento = :empreendimento, p.protocolo = :protocolo, p.protocolo_associado = :protocolo_associado,
				p.arquivo = :arquivo, p.fiscalizacao = :fiscalizacao,  p.tid = :tid, p.interessado_livre = :interessadoLivre,
				p.interessado_livre_telefone = :interessadoLivreTel, p.folhas = :folhas, p.assunto = :assunto, p.descricao = :descricao,
				p.destinatario = :destinatario, p.destinatario_setor = :destinatario_setor, p.orgao_destino = :orgao_destino,
				p.cargo_destinatario = :cargo_destinatario, p.nome_destinatario = :nome_destinatario, p.endereco_destinatario = :endereco_destinatario
				where p.id = :id", EsquemaBanco);


				comando.AdicionarParametroEntrada("id", protocolo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("protocolo", (protocolo.IsProcesso) ? 1 : 2, DbType.Int32);
				comando.AdicionarParametroEntrada("volume", protocolo.Volume ?? 0, DbType.Int32);
				comando.AdicionarParametroEntrada("numero_autuacao", (String.IsNullOrWhiteSpace(protocolo.NumeroAutuacao)) ? (object)DBNull.Value : protocolo.NumeroAutuacao, DbType.String);
				comando.AdicionarParametroEntrada("data_autuacao", (protocolo.DataAutuacao == null || String.IsNullOrWhiteSpace(protocolo.DataAutuacao.DataTexto)) ? (object)DBNull.Value : protocolo.DataAutuacao.DataTexto, DbType.DateTime);
				comando.AdicionarParametroEntrada("checagem", protocolo.ChecagemRoteiro.Id == 0 ? (object)DBNull.Value : protocolo.ChecagemRoteiro.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("requerimento", protocolo.Requerimento.Id == 0 ? (object)DBNull.Value : protocolo.Requerimento.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("fiscalizacao", protocolo.Fiscalizacao.Id == 0 ? (object)DBNull.Value : protocolo.Fiscalizacao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("interessado", protocolo.Interessado.Id == 0 ? (Object)DBNull.Value : protocolo.Interessado.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("empreendimento", (protocolo.Empreendimento.Id == 0) ? (object)DBNull.Value : protocolo.Empreendimento.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("arquivo", protocolo.Arquivo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				comando.AdicionarParametroEntrada("interessadoLivre", protocolo.InteressadoLivre, DbType.String);
				comando.AdicionarParametroEntrada("interessadoLivreTel", protocolo.InteressadoLivreTelefone, DbType.String);
				comando.AdicionarParametroEntrada("folhas", protocolo.Folhas, DbType.Int32);
				//Apenas documento
				comando.AdicionarParametroEntrada("destinatario", doc.Destinatario.Id > 0 ? (int?)doc.Destinatario.Id : null, DbType.Int32);
				comando.AdicionarParametroEntrada("destinatario_setor", doc.DestinatarioSetor.Id > 0 ? (int?)doc.DestinatarioSetor.Id : null, DbType.Int32);
				comando.AdicionarParametroEntrada("nome", DbType.String, 80, doc.Nome);
				comando.AdicionarParametroEntrada("protocolo_associado", doc.ProtocoloAssociado.Id.GetValueOrDefault() == 0 ? (object)DBNull.Value : doc.ProtocoloAssociado.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("assunto", DbType.String, 150, doc.Assunto);
				comando.AdicionarParametroEntrada("descricao", DbType.String, 4000, doc.Descricao);
				comando.AdicionarParametroEntrada("orgao_destino", DbType.String, 100, doc.OrgaoDestino);
				comando.AdicionarParametroEntrada("cargo_destinatario", DbType.String, 100, doc.CargoFuncaoDestinatario);
				comando.AdicionarParametroEntrada("nome_destinatario", DbType.String, 100, doc.NomeDestinatario);
				comando.AdicionarParametroEntrada("endereco_destinatario", DbType.String, 100, doc.EnderecoDestinatario);

				bancoDeDados.ExecutarNonQuery(comando);


				#endregion

				#region Limpar os dados do banco

				#region Atividade/Modelos

				comando = bancoDeDados.CriarComando(@"delete from {0}tab_protocolo_ativ_finalida c 
				where c.protocolo_ativ in (select a.id from {0}tab_protocolo_atividades a where a.protocolo = :protocolo ", EsquemaBanco);

				comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "a.atividade", DbType.Int32, protocolo.Atividades.Select(x => x.Id).ToList());

				comando.DbCommand.CommandText += ")";

				comando.AdicionarParametroEntrada("protocolo", protocolo.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				if (protocolo.Atividades != null && protocolo.Atividades.Count > 0)
				{
					foreach (Atividade item in protocolo.Atividades)
					{
						comando = bancoDeDados.CriarComando(@"delete from {0}tab_protocolo_ativ_finalida c 
						where c.protocolo_ativ in (select a.id from {0}tab_protocolo_atividades a where a.protocolo = :protocolo and a.atividade = :atividade)", EsquemaBanco);
						comando.DbCommand.CommandText += String.Format(" {0}", comando.AdicionarNotIn("and", "c.id", DbType.Int32, item.Finalidades.Select(x => x.IdRelacionamento).ToList()));

						comando.AdicionarParametroEntrada("atividade", item.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("protocolo", protocolo.Id, DbType.Int32);

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				//Atividades
				comando = bancoDeDados.CriarComando("delete from {0}tab_protocolo_atividades c ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where c.protocolo = :protocolo{0}",
				comando.AdicionarNotIn("and", "c.id", DbType.Int32, protocolo.Atividades.Select(x => x.IdRelacionamento).ToList()));
				comando.AdicionarParametroEntrada("protocolo", protocolo.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				//Responsáveis
				comando = bancoDeDados.CriarComando("delete from {0}tab_protocolo_responsavel c ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where c.protocolo = :protocolo{0}",
				comando.AdicionarNotIn("and", "c.id", DbType.Int32, protocolo.Responsaveis.Select(x => x.IdRelacionamento).ToList()));
				comando.AdicionarParametroEntrada("protocolo", protocolo.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Atividades

				if (protocolo.Atividades != null && protocolo.Atividades.Count > 0)
				{
					AtividadeProtocoloDa _daAtividade = new AtividadeProtocoloDa();

					foreach (Atividade item in protocolo.Atividades)
					{
						item.Protocolo.Id = protocolo.Id.Value;
						item.Protocolo.IsProcesso = protocolo.IsProcesso;
						item.Protocolo.Requerimento.Id = protocolo.Requerimento.Id;
						_daAtividade.Salvar(item, bancoDeDados);
					}
				}

				#endregion

				#region Responsáveis

				if (protocolo.Responsaveis != null && protocolo.Responsaveis.Count > 0)
				{
					foreach (ResponsavelTecnico responsavel in protocolo.Responsaveis)
					{
						if (responsavel.IdRelacionamento > 0)
						{
							comando = bancoDeDados.CriarComando(@"update {0}tab_protocolo_responsavel r set r.protocolo = :protocolo, r.responsavel = :responsavel, 
							r.funcao = :funcao, r.numero_art = :numero_art, r.tid = :tid where r.id = :id", EsquemaBanco);
							comando.AdicionarParametroEntrada("id", responsavel.IdRelacionamento, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}tab_protocolo_responsavel(id, protocolo, responsavel, funcao, numero_art, tid) values
							({0}seq_protocolo_responsavel.nextval, :protocolo, :responsavel, :funcao, :numero_art, :tid )", EsquemaBanco);
						}

						comando.AdicionarParametroEntrada("protocolo", protocolo.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("responsavel", responsavel.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("funcao", responsavel.Funcao, DbType.Int32);
						comando.AdicionarParametroEntrada("numero_art", responsavel.NumeroArt, DbType.String);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());


						bancoDeDados.ExecutarNonQuery(comando);
					}
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"delete {0}tab_protocolo_responsavel p where p.protocolo = :id", EsquemaBanco);
					comando.AdicionarParametroEntrada("id", protocolo.Id, DbType.Int32);
					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#region Assinantes

				if (protocolo.Assinantes != null && protocolo.Assinantes.Count > 0)
				{
					foreach (TituloAssinante item in protocolo.Assinantes)
					{						
						if (item.Id > 0)
						{
							comando = bancoDeDados.CriarComando(@"update {0}tab_protocolo_assinantes e set e.protocolo = :protocolo, e.funcionario = :funcionario, e.cargo = :cargo,
							e.tid = :tid where e.id = :id", EsquemaBanco);
							comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}tab_protocolo_assinantes s (id, protocolo, funcionario, cargo, tid)
							values ({0}seq_protocolo_assinantes.nextval, :protocolo, :funcionario, :cargo, :tid)", EsquemaBanco);
						}

						comando.AdicionarParametroEntrada("protocolo", protocolo.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("funcionario", item.FuncionarioId, DbType.Int32);
						comando.AdicionarParametroEntrada("cargo", item.FuncionarioCargoId, DbType.Int32);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Histórico

				Historico.Gerar(protocolo.Id.Value, eHistoricoArtefato.protocolo, eHistoricoAcao.atualizar, bancoDeDados);
				Consulta.Gerar(protocolo.Id.Value, eHistoricoArtefato.protocolo, bancoDeDados);

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

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_protocolo c set c.tid = :tid where c.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Histórico
				Historico.Gerar(id, eHistoricoArtefato.protocolo, eHistoricoAcao.excluir, bancoDeDados);
				Consulta.Deletar(id, eHistoricoArtefato.protocolo, bancoDeDados);

				#endregion

				#region Apaga os dados de protocolo

				List<String> lista = new List<string>();
				lista.Add("delete from {0}tab_protocolo_ativ_finalida a where a.protocolo_ativ in (select b.id from {0}tab_protocolo_atividades b where b.protocolo = :protocolo);");
				lista.Add("delete from {0}tab_protocolo_atividades b where b.protocolo = :protocolo;");
				lista.Add("delete from {0}tab_protocolo_responsavel c where c.protocolo = :protocolo;");
				lista.Add("delete from {0}tab_protocolo e where e.id = :protocolo;");
				comando = bancoDeDados.CriarComando(@"begin " + string.Join(" ", lista) + "end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("protocolo", id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		public void Autuar(IProtocolo protocolo)
		{
			GerenciadorTransacao.ObterIDAtual();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				bancoDeDados.IniciarTransacao();

				#region Protocolo

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_protocolo p set p.numero_autuacao = :numero_autuacao, p.data_autuacao = :data_autuacao, 
				p.tid = :tid where p.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", protocolo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("numero_autuacao", DbType.String, 15, protocolo.NumeroAutuacao);
				comando.AdicionarParametroEntrada("data_autuacao", protocolo.DataAutuacao.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Histórico

				Historico.Gerar(protocolo.Id.Value, eHistoricoArtefato.protocolo, eHistoricoAcao.autuar, bancoDeDados);
				Consulta.Gerar(protocolo.Id.Value, eHistoricoArtefato.protocolo, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		public void AlterarAtividades(IProtocolo protocolo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Protocolo

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_protocolo p set p.requerimento = :requerimento, p.tid = :tid where p.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", protocolo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("requerimento", protocolo.Requerimento.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Limpar Atividade/Modelos

				comando = bancoDeDados.CriarComando(@"delete from {0}tab_protocolo_ativ_finalida c 
				where c.protocolo_ativ in (select a.id from {0}tab_protocolo_atividades a where a.protocolo = :protocolo ", EsquemaBanco);

				comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "a.atividade", DbType.Int32, protocolo.Atividades.Select(x => x.Id).ToList());

				comando.DbCommand.CommandText += ")";

				comando.AdicionarParametroEntrada("protocolo", protocolo.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				if (protocolo.Atividades != null && protocolo.Atividades.Count > 0)
				{
					foreach (Atividade item in protocolo.Atividades)
					{
						comando = bancoDeDados.CriarComando(@"delete from {0}tab_protocolo_ativ_finalida c 
						where c.protocolo_ativ in (select a.id from {0}tab_protocolo_atividades a where a.protocolo = :protocolo and a.atividade = :atividade)", EsquemaBanco);
						comando.DbCommand.CommandText += String.Format(" {0}", comando.AdicionarNotIn("and", "c.id", DbType.Int32, item.Finalidades.Select(x => x.IdRelacionamento).ToList()));

						comando.AdicionarParametroEntrada("atividade", item.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("protocolo", protocolo.Id, DbType.Int32);

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				//Atividades
				comando = bancoDeDados.CriarComando("delete from {0}tab_protocolo_atividades c ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where c.protocolo = :protocolo{0}",
				comando.AdicionarNotIn("and", "c.id", DbType.Int32, protocolo.Atividades.Select(x => x.IdRelacionamento).ToList()));
				comando.AdicionarParametroEntrada("protocolo", protocolo.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Atividades

				if (protocolo.Atividades != null && protocolo.Atividades.Count > 0)
				{
					AtividadeProtocoloDa _daAtividade = new AtividadeProtocoloDa();

					foreach (Atividade item in protocolo.Atividades)
					{
						item.Protocolo.Id = protocolo.Id.Value;
						item.Protocolo.IsProcesso = true;
						item.Protocolo.Requerimento.Id = protocolo.Requerimento.Id;
						_daAtividade.Salvar(item, bancoDeDados);
					}
				}

				#endregion

				#region Histórico

				Historico.Gerar(protocolo.Id.Value, eHistoricoArtefato.protocolo, eHistoricoAcao.atualizar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		protected void Apensar(Processo processo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando;

				#region Buscar os apensados que serão removidos

				comando = bancoDeDados.CriarComando("select p.associado, po.setor from {0}tab_protocolo_associado p, {0}tab_protocolo po where po.id = p.protocolo and p.tipo = 1 and p.protocolo = :protocolo ", EsquemaBanco);

				comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "p.associado", DbType.Int32, processo.Processos.Select(x => x.Id).ToList());
				comando.AdicionarParametroEntrada("protocolo", processo.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					comando = bancoDeDados.CriarComando(@"update {0}tab_protocolo p set p.emposse = :emposse, p.setor =:setor, p.tid = :tid where p.id = :associado", EsquemaBanco);
					comando.AdicionarParametroEntrada("associado", DbType.Int32);
					comando.AdicionarParametroEntrada("emposse", User.FuncionarioId, DbType.Int32);
					comando.AdicionarParametroEntrada("setor", DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					int apen = 0;
					int setor = 0;

					while (reader.Read())
					{
						apen = Convert.ToInt32(reader["associado"]);
						setor = Convert.ToInt32(reader["setor"]);
						comando.SetarValorParametro("associado", apen);
						comando.SetarValorParametro("setor", setor);
						bancoDeDados.ExecutarNonQuery(comando);

						Historico.Gerar(apen, eHistoricoArtefato.protocolo, eHistoricoAcao.atualizar, bancoDeDados);
						Historico.Gerar(apen, eHistoricoArtefato.apensar, eHistoricoAcao.desapensar, bancoDeDados);
						Consulta.Gerar(apen, eHistoricoArtefato.protocolo, bancoDeDados);
					}
					reader.Close();
				}

				//Apensados
				comando = bancoDeDados.CriarComando("delete from {0}tab_protocolo_associado p ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where p.tipo = 1 and p.protocolo = :protocolo{0}",
				comando.AdicionarNotIn("and", "p.associado", DbType.Int32, processo.Processos.Select(x => x.Id).ToList()));
				comando.AdicionarParametroEntrada("protocolo", processo.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Apensados

				if (processo.Processos != null && processo.Processos.Count > 0)
				{
					foreach (Processo apensado in processo.Processos)
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}tab_protocolo_associado(id, protocolo, associado, tipo, tid)
						(select {0}seq_protocolo_associado.nextval, :protocolo, :associado, 1, :tid from dual where not exists
						(select id from {0}tab_protocolo_associado r where r.protocolo = :protocolo and r.associado = :associado))", EsquemaBanco);

						comando.AdicionarParametroEntrada("protocolo", processo.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("associado", apensado.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);

						if (comando.LinhasAfetadas > 0)
						{
							comando = bancoDeDados.CriarComando(@"update {0}tab_protocolo p set p.setor = null, p.emposse = null, p.tid = :tid where p.id = :associado", EsquemaBanco);
							comando.AdicionarParametroEntrada("associado", apensado.Id, DbType.Int32);
							comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
							bancoDeDados.ExecutarNonQuery(comando);

							Historico.Gerar(apensado.Id.Value, eHistoricoArtefato.protocolo, eHistoricoAcao.atualizar, bancoDeDados);
							Historico.Gerar(apensado.Id.Value, eHistoricoArtefato.apensar, eHistoricoAcao.apensar, bancoDeDados);
							Consulta.Gerar(apensado.Id.Value, eHistoricoArtefato.protocolo, bancoDeDados);
						}
					}
				}

				#endregion

				bancoDeDados.Commit();
			}
		}

		protected void Juntar(Processo processo, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando;

				#region Buscar os juntados que serão removidos

				comando = bancoDeDados.CriarComando("select p.associado, po.setor from {0}tab_protocolo_associado p, {0}tab_protocolo po where po.id = p.protocolo and p.tipo = 2 and p.protocolo = :protocolo ", EsquemaBanco);
				comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "p.associado", DbType.Int32, processo.Documentos.Select(x => x.Id).ToList());
				comando.AdicionarParametroEntrada("protocolo", processo.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					comando = bancoDeDados.CriarComando(@"update {0}tab_protocolo p set p.emposse = :emposse, p.setor =:setor, p.tid = :tid where p.id = :associado", EsquemaBanco);
					comando.AdicionarParametroEntrada("associado", DbType.Int32);
					comando.AdicionarParametroEntrada("emposse", User.FuncionarioId, DbType.Int32);
					comando.AdicionarParametroEntrada("setor", DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					int junt = 0;
					int setor = 0;

					while (reader.Read())
					{
						junt = Convert.ToInt32(reader["associado"]);
						setor = Convert.ToInt32(reader["setor"]);
						comando.SetarValorParametro("associado", junt);
						comando.SetarValorParametro("setor", setor);

						bancoDeDados.ExecutarNonQuery(comando);

						Historico.Gerar(junt, eHistoricoArtefato.protocolo, eHistoricoAcao.atualizar, bancoDeDados);
						Historico.Gerar(junt, eHistoricoArtefato.juntar, eHistoricoAcao.desentranhar, bancoDeDados);
						Consulta.Gerar(junt, eHistoricoArtefato.protocolo, bancoDeDados);
					}
					reader.Close();
				}

				//Juntados
				comando = bancoDeDados.CriarComando("delete from {0}tab_protocolo_associado p ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where p.tipo = 2 and p.protocolo = :protocolo{0}",
				comando.AdicionarNotIn("and", "p.associado", DbType.Int32, processo.Documentos.Select(x => x.Id).ToList()));
				comando.AdicionarParametroEntrada("protocolo", processo.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Juntados

				if (processo.Documentos != null && processo.Documentos.Count > 0)
				{
					foreach (Documento juntado in processo.Documentos)
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}tab_protocolo_associado(id, protocolo, associado, tipo, tid)
						(select {0}seq_protocolo_associado.nextval, :protocolo, :associado, 2, :tid from dual where not exists
						(select id from {0}tab_protocolo_associado r where r.protocolo = :protocolo and r.associado = :associado))", EsquemaBanco);

						comando.AdicionarParametroEntrada("protocolo", processo.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("associado", juntado.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);

						if (comando.LinhasAfetadas > 0)
						{
							comando = bancoDeDados.CriarComando(@"update {0}tab_protocolo d set d.emposse = null, d.setor = null, d.tid = :tid where d.id = :associado", EsquemaBanco);
							comando.AdicionarParametroEntrada("associado", juntado.Id, DbType.Int32);
							comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
							bancoDeDados.ExecutarNonQuery(comando);

							Historico.Gerar(juntado.Id.Value, eHistoricoArtefato.protocolo, eHistoricoAcao.atualizar, bancoDeDados);
							Historico.Gerar(juntado.Id.Value, eHistoricoArtefato.juntar, eHistoricoAcao.juntar, bancoDeDados);
							Consulta.Gerar(juntado.Id.Value, eHistoricoArtefato.protocolo, bancoDeDados);
						}
					}
				}

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void JuntarApensar(Processo processo, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();
				//Apensar
				Apensar(processo, bancoDeDados);
				//Juntar
				Juntar(processo, bancoDeDados);

				#region Atualizar o tid para a nova ação

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_protocolo c set c.tid = :tid where c.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", processo.Id.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Histórico
				Historico.Gerar(processo.Id.Value, eHistoricoArtefato.protocolo, eHistoricoAcao.juntarapensar, bancoDeDados);
				#endregion
			}
		}

		internal void ConverterDocumento(ConverterDocumento convertDoc, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"
					update {0}tab_protocolo t
					   set t.tipo            = 3 /*Técnico*/,
						   t.protocolo       = 1 /*Processo*/,
						   t.numero_autuacao = :numero_autuacao,
						   t.data_autuacao   = :data_autuacao
					 where t.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("numero_autuacao", DbType.Int32, 15, convertDoc.NumeroAutuacao);
				comando.AdicionarParametroEntrada("data_autuacao", convertDoc.DataAutuacao, DbType.DateTime);
				comando.AdicionarParametroEntrada("id", convertDoc.DocumentoId, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#region Histórico

				Historico.Gerar(convertDoc.DocumentoId, eHistoricoArtefato.protocolo, eHistoricoAcao.converter, bancoDeDados);
				Consulta.Gerar(convertDoc.DocumentoId, eHistoricoArtefato.protocolo, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter / Filtrar

		public IProtocolo Obter(int id, bool simplificado = false, bool atividades = false, BancoDeDados banco = null)
		{
			Protocolo protocolo = new Protocolo();
			Documento documento = new Documento();

			if (id <= 0)
			{
				return new Protocolo();
			}

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Protocolo

				Comando comando = bancoDeDados.CriarComando(@"
					select r.id,
						   r.fiscalizacao,
						   f.situacao fiscalizacao_situacao,
						   r.numero,
						   r.ano,
						   r.nome,
						   r.tipo,
						   r.protocolo,
						   trunc(r.data_criacao) data_criacao,
						   r.numero_autuacao,
						   trunc(r.data_autuacao) data_autuacao,
						   r.volume,
						   r.checagem,
						   r.checagem_pendencia,
						   r.requerimento,
						   to_char(tr.data_criacao, 'dd/mm/yyyy') data_requerimento,
						   tr.situacao requerimento_situacao,
						   r.interessado,
						   nvl(p.nome, p.razao_social) interessado_nome,
						   nvl(p.cpf, p.cnpj) interessado_cpf_cnpj,
						   p.tipo interessado_tipo,
						   lpt.texto tipo_texto,
						   r.empreendimento,
						   e.cnpj empreendimento_cnpj,
						   e.denominador,
						   r.situacao,
						   ls.texto situacao_texto,
						   r.protocolo_associado,
						   r.emposse,
						   r.arquivo,
						   r.tid,
						   r.setor,
						   r.setor_criacao, 
						   lfs.texto fiscalizacao_sit_texto,
						   r.interessado_livre,
						   r.interessado_livre_telefone,
						   r.folhas, r.assunto, r.descricao, r.destinatario, r.destinatario_setor, r.orgao_destino, r.cargo_destinatario,
						   r.nome_destinatario, r.endereco_destinatario, d.id destinatario_id, 
						   d.tid destinatario_tid, d.nome destinatario_nome, s.id destinatario_setor_id, s.sigla destinatario_setor_sigla, s.nome destinatario_setor_nome
					  from {0}tab_protocolo          r,
						   {0}tab_pessoa             p,
						   {0}tab_fiscalizacao       f,
						   {0}tab_empreendimento     e,
						   {0}lov_protocolo_situacao ls,
						   {0}lov_protocolo_tipo     lpt,
						   {0}tab_requerimento       tr,
						   {0}lov_fiscalizacao_situacao lfs,
						   {0}tab_setor				 s,
						   {0}tab_funcionario		 d
					 where r.interessado = p.id(+)
					   and r.empreendimento = e.id(+)
					   and r.situacao = ls.id
					   and lpt.id = r.tipo
					   and r.requerimento = tr.id(+)
					   and r.fiscalizacao = f.id(+)
					   and f.situacao = lfs.id(+)
					   and r.destinatario_setor = s.id(+)
					   and r.destinatario = d.id(+)
					   and r.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						#region Dados base

						protocolo.Id = id;
						protocolo.Tid = reader["tid"].ToString();

						if (reader["fiscalizacao"] != null && !Convert.IsDBNull(reader["fiscalizacao"]))
						{
							protocolo.Fiscalizacao.Id = Convert.ToInt32(reader["fiscalizacao"]);
							protocolo.Fiscalizacao.SituacaoId = Convert.ToInt32(reader["fiscalizacao_situacao"]);
							protocolo.Fiscalizacao.SituacaoTexto = reader.GetValue<string>("fiscalizacao_sit_texto");
						}

						if (reader["numero"] != null && !Convert.IsDBNull(reader["numero"]))
						{
							protocolo.NumeroProtocolo = Convert.ToInt32(reader["numero"]);
						}

						if (reader["ano"] != null && !Convert.IsDBNull(reader["ano"]))
						{
							protocolo.Ano = Convert.ToInt32(reader["ano"]);
						}

						protocolo.NumeroAutuacao = reader["numero_autuacao"].ToString();

						if (reader["data_autuacao"] != null && !Convert.IsDBNull(reader["data_autuacao"]))
						{
							protocolo.DataAutuacao.Data = Convert.ToDateTime(reader["data_autuacao"]);
						}

						protocolo.DataCadastro.Data = Convert.ToDateTime(reader["data_criacao"]);

						if (reader["setor"] != null && !Convert.IsDBNull(reader["setor"]))
						{
							protocolo.SetorId = Convert.ToInt32(reader["setor"]);
						}

						if (reader["setor_criacao"] != null && !Convert.IsDBNull(reader["setor_criacao"]))
						{
							protocolo.SetorCriacaoId = Convert.ToInt32(reader["setor_criacao"]);
						}

						if (reader["tipo"] != null && !Convert.IsDBNull(reader["tipo"]))
						{
							protocolo.Tipo.Id = Convert.ToInt32(reader["tipo"]);
							protocolo.Tipo.Texto = reader["tipo_texto"].ToString();
						}

						if (reader["volume"] != null && !Convert.IsDBNull(reader["volume"]))
						{
							protocolo.Volume = Convert.ToInt32(reader["volume"]);
						}

						if (reader["checagem"] != null && !Convert.IsDBNull(reader["checagem"]))
						{
							protocolo.ChecagemRoteiro.Id = Convert.ToInt32(reader["checagem"]);
						}

						if (reader["requerimento"] != null && !Convert.IsDBNull(reader["requerimento"]))
						{
							protocolo.Requerimento.Id = Convert.ToInt32(reader["requerimento"]);
							protocolo.Requerimento.SituacaoId = Convert.ToInt32(reader["requerimento_situacao"]);
							protocolo.Requerimento.DataCadastro = Convert.ToDateTime(reader["data_requerimento"]);
							protocolo.Requerimento.ProtocoloId = protocolo.Id.Value;
							protocolo.Requerimento.ProtocoloTipo = 1;
						}

						if (reader["interessado"] != null && !Convert.IsDBNull(reader["interessado"]))
						{
							protocolo.Interessado.Id = Convert.ToInt32(reader["interessado"]);
							protocolo.Interessado.Tipo = Convert.ToInt32(reader["interessado_tipo"]);

							if (reader["interessado_tipo"].ToString() == "1")
							{
								protocolo.Interessado.Fisica.Nome = reader["interessado_nome"].ToString();
								protocolo.Interessado.Fisica.CPF = reader["interessado_cpf_cnpj"].ToString();
							}
							else
							{
								protocolo.Interessado.Juridica.RazaoSocial = reader["interessado_nome"].ToString();
								protocolo.Interessado.Juridica.CNPJ = reader["interessado_cpf_cnpj"].ToString();
							}
						}

						if (reader["interessado_livre"] != null && !Convert.IsDBNull(reader["interessado_livre"]))
							protocolo.InteressadoLivre = reader["interessado_livre"].ToString();
						if (reader["interessado_livre_telefone"] != null && !Convert.IsDBNull(reader["interessado_livre_telefone"]))
							protocolo.InteressadoLivreTelefone = reader["interessado_livre_telefone"].ToString();
						if (reader["folhas"] != null && !Convert.IsDBNull(reader["folhas"]))
							protocolo.Folhas = Convert.ToInt32(reader["folhas"]);

						if (reader["empreendimento"] != null && !Convert.IsDBNull(reader["empreendimento"]))
						{
							protocolo.Empreendimento.Id = Convert.ToInt32(reader["empreendimento"]);
							protocolo.Empreendimento.Denominador = reader["denominador"].ToString();
							protocolo.Empreendimento.CNPJ = reader["empreendimento_cnpj"].ToString();
						}

						if (reader["emposse"] != null && !Convert.IsDBNull(reader["emposse"]))
						{
							protocolo.Emposse.Id = Convert.ToInt32(reader["emposse"]);
						}

						if (reader["situacao"] != null && !Convert.IsDBNull(reader["situacao"]))
						{
							protocolo.SituacaoId = Convert.ToInt32(reader["situacao"]);
							protocolo.SituacaoTexto = reader["situacao_texto"].ToString();
						}

						if (reader["arquivo"] != null && !Convert.IsDBNull(reader["arquivo"]))
						{
							protocolo.Arquivo.Id = Convert.ToInt32(reader["arquivo"]);
						}

						if (reader["protocolo"] != null && !Convert.IsDBNull(reader["protocolo"]))
						{
							protocolo.IsProcesso = Convert.ToInt32(reader["protocolo"]) == 1;
						}

						#endregion

						if (!protocolo.IsProcesso)
						{
							documento = new Documento(protocolo);
							documento.Nome = reader["nome"].ToString();
							documento.Assunto = reader["assunto"].ToString();
							documento.Descricao = reader["descricao"].ToString();
							documento.OrgaoDestino = reader["orgao_destino"].ToString();
							documento.CargoFuncaoDestinatario = reader["cargo_destinatario"].ToString();
							documento.NomeDestinatario = reader["nome_destinatario"].ToString();
							documento.EnderecoDestinatario = reader["endereco_destinatario"].ToString();

							if (reader["destinatario_id"] != null && !Convert.IsDBNull(reader["destinatario_id"]))
							{
								documento.Destinatario.Id = Convert.ToInt32(reader["destinatario_id"]);
							}
							documento.Destinatario.Nome = reader["destinatario_nome"].ToString();

							if (reader["destinatario_setor_id"] != null && !Convert.IsDBNull(reader["destinatario_setor_id"]))
							{
								documento.DestinatarioSetor.Id = Convert.ToInt32(reader["destinatario_setor_id"]);
							}
							documento.DestinatarioSetor.Nome = reader["destinatario_setor_nome"].ToString();

							if (reader["protocolo_associado"] != null && !Convert.IsDBNull(reader["protocolo_associado"]))
							{
								documento.ProtocoloAssociado = new Protocolo(ObterProtocolo(Convert.ToInt32(reader["protocolo_associado"])));
							}

							if (reader["checagem_pendencia"] != null && !Convert.IsDBNull(reader["checagem_pendencia"]))
							{
								documento.ChecagemPendencia.Id = Convert.ToInt32(reader["checagem_pendencia"]);
							}
						}
					}

					reader.Close();
				}

				if (simplificado)
				{
					if (protocolo.IsProcesso)
					{
						return new Processo(protocolo);
					}
					else
					{
						return documento;
					}
				}

				#endregion

				#region Atividades

				comando = bancoDeDados.CriarComando(@"select b.id, b.atividade, a.atividade atividade_texto, b.situacao atividade_situacao_id,
				(select s.texto from lov_atividade_situacao s where s.id = b.situacao) atividade_situacao_texto, a.setor, b.motivo,
				b.tid from {0}tab_atividade a, {0}tab_protocolo_atividades b where a.id = b.atividade and b.protocolo = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Atividade atividade;

					while (reader.Read())
					{
						atividade = new Atividade();
						atividade.Id = Convert.ToInt32(reader["atividade"]);
						atividade.IdRelacionamento = Convert.ToInt32(reader["id"]);
						atividade.Tid = reader["tid"].ToString();
						atividade.NomeAtividade = reader["atividade_texto"].ToString();
						atividade.SituacaoId = Convert.ToInt32(reader["atividade_situacao_id"]);
						atividade.SituacaoTexto = reader["atividade_situacao_texto"].ToString();
						atividade.SetorId = Convert.ToInt32(reader["setor"]);
						atividade.Motivo = reader["motivo"].ToString();

						#region Atividades/Finalidades/Modelos

						comando = bancoDeDados.CriarComando(@"select a.id, a.finalidade, ltf.texto finalidade_texto, a.modelo, tm.nome modelo_nome, 
							a.titulo_anterior_tipo, a.titulo_anterior_id, a.titulo_anterior_numero, a.modelo_anterior_id, a.modelo_anterior_nome, a.modelo_anterior_sigla, a.orgao_expedidor
							from {0}tab_protocolo_ativ_finalida a, {0}tab_titulo_modelo tm, {0}lov_titulo_finalidade ltf where a.modelo = tm.id and a.finalidade = ltf.id(+) 
							and a.protocolo_ativ = :id", EsquemaBanco);

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
									fin.Texto = readerAux["finalidade_texto"].ToString();
								}

								if (readerAux["modelo"] != DBNull.Value)
								{
									fin.TituloModelo = Convert.ToInt32(readerAux["modelo"]);
									fin.TituloModeloTexto = readerAux["modelo_nome"].ToString();
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

						protocolo.Atividades.Add(atividade);
					}

					reader.Close();
				}

				if (atividades)
				{
					if (protocolo.IsProcesso)
					{
						return new Processo(protocolo);
					}
					else
					{
						return documento;
					}
				}

				#endregion

				#region Responsáveis

				comando = bancoDeDados.CriarComando(@"select pr.id, pr.responsavel, pr.funcao, nvl(p.nome, p.razao_social) nome, pr.numero_art, 
				nvl(p.cpf, p.cnpj) cpf_cnpj, p.tipo from {0}tab_protocolo_responsavel pr, {0}tab_pessoa p where pr.responsavel = p.id and pr.protocolo = :protocolo", EsquemaBanco);

				comando.AdicionarParametroEntrada("protocolo", protocolo.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					ResponsavelTecnico responsavel;
					while (reader.Read())
					{
						responsavel = new ResponsavelTecnico();
						responsavel.IdRelacionamento = Convert.ToInt32(reader["id"]);
						responsavel.Id = Convert.ToInt32(reader["responsavel"]);
						responsavel.Funcao = Convert.ToInt32(reader["funcao"]);
						responsavel.CpfCnpj = reader["cpf_cnpj"].ToString();
						responsavel.NomeRazao = reader["nome"].ToString();
						responsavel.NumeroArt = reader["numero_art"].ToString();
						protocolo.Responsaveis.Add(responsavel);
					}
					reader.Close();
				}

				#endregion

				#region Assinantes
				comando = bancoDeDados.CriarComando(@"select ta.id, ta.protocolo, f.id func_id, f.nome func_nome, ta.cargo, c.nome cargo_nome, ta.tid from tab_protocolo_assinantes ta, tab_funcionario f, tab_cargo c where 
					ta.funcionario = f.id and ta.cargo = c.id and ta.protocolo = :protocolo", EsquemaBanco);
				comando.AdicionarParametroEntrada("protocolo", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						TituloAssinante item = new TituloAssinante();
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

						documento.Assinantes.Add(item);
					}
					reader.Close();
				}
				#endregion
			}

			if (protocolo.IsProcesso)
			{
				return new Processo(protocolo);
			}
			else
			{
				return documento;
			}
		}

		public IProtocolo ObterSimplificado(int id, BancoDeDados banco = null)
		{
			return Obter(id, true, banco: banco);
		}

		public IProtocolo ObterAtividades(int id, BancoDeDados banco = null)
		{
			return Obter(id, atividades: true, banco: banco);
		}

		public IProtocolo ObterJuntadosApensados(int id, BancoDeDados banco = null)
		{
			Processo processo = ObterSimplificado(id, banco) as Processo;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Processos Apensados/Documentos Juntados

				Comando comando = bancoDeDados.CriarComando(@"select p.id, p.protocolo, p.associado, p.tipo, p.tid from {0}tab_protocolo_associado p 
				where p.protocolo = :protocolo order by p.associado", EsquemaBanco);

				comando.AdicionarParametroEntrada("protocolo", processo.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Processo proc;
					Documento doc;
					while (reader.Read())
					{
						if (Convert.ToInt32(reader["tipo"]) == 1)
						{
							proc = ObterSimplificado(Convert.ToInt32(reader["associado"]), bancoDeDados) as Processo;
							processo.Processos.Add(proc);
						}
						else
						{
							doc = ObterSimplificado(Convert.ToInt32(reader["associado"]), bancoDeDados) as Documento;
							processo.Documentos.Add(doc);
						}
					}
					reader.Close();
				}

				#endregion
			}
			return processo;
		}

		public IProtocolo ObterProcessosDocumentos(int id, BancoDeDados banco = null)
		{
			Processo processo = null;

			IProtocolo protocolo = ObterAtividades(id, banco);

			if (protocolo.IsProcesso)
			{
				processo = protocolo as Processo;

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
				{
					#region Processos Apensados/Documentos Juntados

					Comando comando = bancoDeDados.CriarComando(@"select p.id, p.protocolo, p.associado, p.tipo, p.tid, r.id from 
					{0}tab_protocolo_associado p, {0}tab_protocolo pa, {0}tab_requerimento r where p.protocolo = :protocolo and p.associado = pa.id 
					and pa.requerimento = r.id(+) order by r.data_criacao", EsquemaBanco);

					comando.AdicionarParametroEntrada("protocolo", processo.Id, DbType.Int32);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						Processo proc;
						Documento doc;
						while (reader.Read())
						{
							if (Convert.ToInt32(reader["tipo"]) == 1)
							{
								proc = ObterAtividades(Convert.ToInt32(reader["associado"]), bancoDeDados) as Processo;
								processo.Processos.Add(proc);
							}
							else
							{
								doc = ObterAtividades(Convert.ToInt32(reader["associado"]), bancoDeDados) as Documento;
								processo.Documentos.Add(doc);
							}
						}
						reader.Close();
					}

					#endregion
				}
				return processo;
			}
			else
			{
				return protocolo as Documento;
			}
		}

		public Resultados<Protocolo> Filtrar(Filtro<ListarProtocoloFiltro> filtros, BancoDeDados banco = null)
		{
			Resultados<Protocolo> retorno = new Resultados<Protocolo>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAnd("l.protocolo", "protocolo", filtros.Dados.ProtocoloId);

				comandtxt += comando.FiltroAnd("l.numero", "numero", filtros.Dados.Protocolo.Numero);

				comandtxt += comando.FiltroAnd("l.ano", "ano", filtros.Dados.Protocolo.Ano);

				comandtxt += comando.FiltroAndLike("l.nome", "nome", filtros.Dados.Nome, true, true);

				comandtxt += comando.FiltroAndLike("l.numero_autuacao", "numero_autuacao", filtros.Dados.NumeroAutuacao);

				comandtxt += comando.FiltroIn("l.protocolo_id", "select t.protocolo_id from hst_tramitacao t where t.numero_autuacao = :numero_autuacao and rownum = 1", "numero_autuacao", filtros.Dados.NumeroAutuacaoDoc);

				comandtxt += comando.FiltroIn("l.setor_criacao_id", string.Format("select tse.setor from {0}tab_setor_endereco tse where tse.municipio = :municipio", (string.IsNullOrEmpty(EsquemaBanco) ? "" : ".")), "municipio", filtros.Dados.Municipio);

				if (!filtros.Dados.DataRegistro.IsEmpty && filtros.Dados.DataRegistro.IsValido)
				{
					comandtxt += comando.FiltroAnd("TO_DATE(l.data_criacao)", "data_criacao", filtros.Dados.DataRegistro.DataTexto);
				}

				if (!filtros.Dados.DataAutuacao.IsEmpty && filtros.Dados.DataAutuacao.IsValido)
				{
					comandtxt += comando.FiltroAnd("TO_DATE(l.data_autuacao)", "data_autuacao", filtros.Dados.DataAutuacao.DataTexto);
				}

				comandtxt += comando.FiltroAnd("l.tipo_id", "tipo", filtros.Dados.Tipo);

				comandtxt += comando.FiltroIn("l.protocolo_id", String.Format("select s.protocolo from {0}tab_protocolo_atividades s where s.situacao = :atividade_situacao_id", EsquemaBanco),
				"atividade_situacao_id", filtros.Dados.SituacaoAtividade);

				comandtxt += comando.FiltroIn("l.protocolo_id", String.Format("select a.protocolo from {0}tab_protocolo_atividades a where a.atividade = :atividade_id", EsquemaBanco),
				"atividade_id", filtros.Dados.AtividadeSolicitada);

				comandtxt += comando.FiltroAndLike("l.interessado_nome_razao", "interessado_nome_razao", filtros.Dados.InteressadoNomeRazao, true);

				if (!String.IsNullOrWhiteSpace(filtros.Dados.InteressadoCpfCnpj))
				{
					if (ValidacoesGenericasBus.Cpf(filtros.Dados.InteressadoCpfCnpj) ||
						ValidacoesGenericasBus.Cnpj(filtros.Dados.InteressadoCpfCnpj))
					{
						comandtxt += comando.FiltroAnd("l.interessado_cpf_cnpj", "interessado_cpf_cnpj", filtros.Dados.InteressadoCpfCnpj);
					}
					else
					{
						comandtxt += comando.FiltroAndLike("l.interessado_cpf_cnpj", "interessado_cpf_cnpj", filtros.Dados.InteressadoCpfCnpj);
					}
				}

				comandtxt += comando.FiltroAnd("l.empreendimento_codigo", "empreendimento_codigo", filtros.Dados.EmpreendimentoCodigo);

				comandtxt += comando.FiltroAndLike("l.empreendimento_denominador", "empreendimento_denominador", filtros.Dados.EmpreendimentoRazaoDenominacao, true);

				if (!String.IsNullOrWhiteSpace(filtros.Dados.EmpreendimentoCnpj))
				{
					if (ValidacoesGenericasBus.Cnpj(filtros.Dados.EmpreendimentoCnpj))
					{
						comandtxt += comando.FiltroAnd("l.empreendimento_cnpj", "empreendimento_cnpj", filtros.Dados.EmpreendimentoCnpj);
					}
					else
					{
						comandtxt += comando.FiltroAndLike("l.empreendimento_cnpj", "empreendimento_cnpj", filtros.Dados.EmpreendimentoCnpj);
					}
				}

				comandtxt += comando.FiltroAndLike("l.assunto", "assunto", filtros.Dados.Assunto, true, true);

				comandtxt += $" and exists (select 1 from tab_funcionario_setor s where s.funcionario = {User.FuncionarioId} and (s.setor = l.setor_criacao_id or s.setor = l.setor_id) and l.tipo_id in (14, 15) or l.tipo_id not in (14, 15))";

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "numero,ano", "interessado_nome_razao", "empreendimento_denominador" };

				if (filtros.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("numero,ano");
				}

				#endregion

				#region Quantidade de registro do resultado

				comando.DbCommand.CommandText = String.Format("select count(*) from {0}lst_protocolo l where l.id > 0 " + comandtxt, (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = String.Format(@"select id, protocolo_id, numero, ano, nome, tipo_id, tipo_texto, data_criacao, interessado_id, interessado_tipo, coalesce(interessado_nome_razao, assunto) interessado_nome_razao, 
				interessado_cpf_cnpj, interessado_rg_ie, empreendimento_id, empreendimento_codigo, empreendimento_denominador, empreendimento_cnpj, situacao_id, situacao_texto 
				from {0}lst_protocolo l where l.id > 0" + comandtxt + DaHelper.Ordenar(colunas, ordenar), (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					Protocolo item;

					while (reader.Read())
					{
						item = new Protocolo();
						item.Id = reader.GetValue<int>("protocolo_id");
						item.DataCadastro.Data = reader.GetValue<DateTime>("data_criacao");
						item.Tipo.Id = reader.GetValue<int>("tipo_id");
						item.Nome = reader.GetValue<string>("nome");
						item.NumeroProtocolo = reader.GetValue<int>("numero");
						item.Ano = reader.GetValue<int>("ano");
						item.Tipo.Texto = reader.GetValue<string>("tipo_texto");
						item.Interessado.Id = reader.GetValue<int>("interessado_id");
						item.Interessado.Tipo = reader.GetValue<int>("interessado_tipo");
						item.Interessado.Fisica.Nome = reader.GetValue<string>("interessado_nome_razao");
						item.Interessado.Juridica.RazaoSocial = reader.GetValue<string>("interessado_nome_razao");
						item.Empreendimento.Id = reader.GetValue<int>("empreendimento_id");
						item.Empreendimento.Codigo = reader.GetValue<long>("empreendimento_codigo");
						item.Empreendimento.Denominador = reader.GetValue<string>("empreendimento_denominador");
						item.SituacaoId = reader.GetValue<int>("situacao_id");
						item.SituacaoTexto = reader.GetValue<string>("situacao_texto");

						retorno.Itens.Add(item);
					}

					reader.Close();

					#endregion
				}
			}
			return retorno;
		}

		internal Resultados<HistoricoProtocolo> FiltrarHistoricoAssociados(Filtro<ListarProtocoloFiltro> filtros, BancoDeDados banco = null)
		{
			Resultados<HistoricoProtocolo> retorno = new Resultados<HistoricoProtocolo>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAnd("associado_id", "associado_id", filtros.Dados.Id);

				comandtxt += comando.FiltroAnd("acao", "acao", filtros.Dados.Acao);

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "numero", "tipo_id", "interessado_nome_razao", "empreendimento_denominador" };

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
					select count(*)
					  from (select a.tipo_id, a.associado_id, la.id acao
							  from {0}hst_protocolo_associado       a,
								   {0}lov_historico_artefatos_acoes l,
								   {0}lov_historico_acao            la
							 where l.acao = la.id
							   and l.id = a.acao_executada
							union all
							select t.protocolo_id tipo_id, t.id_protocolo associado_id, la.id acao
							  from {0}hst_protocolo                 t,
								   {0}lov_historico_artefatos_acoes l,
								   {0}lov_historico_acao            la
							 where t.acao_executada = l.id(+)
							   and l.acao = la.id(+)
							   and la.id = 33/*Converter*/) where 0=0 " + comandtxt, (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.DbCommand.CommandText = String.Format(@"
					select *
					  from (select pa.numero || '/' || pa.ano numero,
								   to_char(a.data_execucao, 'dd/mm/yyyy hh24:mi:ss') data_execucao,
								   s.id setor_id,
								   s.sigla setor_sigla,
								   a.executor_id,
								   a.executor_nome executor_nome,
								   a.acao_executada,
								   la.id acao_id,
								   la.texto acao,
								   a.tipo_id,
								   a.tipo_texto protocolo_texto,
								   a.associado_id
							  from {0}hst_protocolo_associado       a,
								   {0}tab_protocolo                 p,
								   {0}tab_protocolo                 pa,
								   {0}tab_setor                     s,
								   {0}lov_historico_artefatos_acoes l,
								   {0}lov_historico_acao            la
							 where a.associado_id = p.id
							   and a.protocolo_id = pa.id
							   and p.setor = s.id(+)
							   and l.acao = la.id
							   and l.id = a.acao_executada

							union all

							select p.numero || '/' || p.ano numero,
								   to_char(a.data_execucao, 'dd/mm/yyyy hh24:mi:ss') data_execucao,
								   s.id setor_id,
								   s.sigla setor_sigla,
								   a.executor_id,
								   a.executor_nome executor_nome,
								   a.acao_executada,
								   la.id acao_id,
								   la.texto acao,
								   a.protocolo_id tipo_id,
								   a.protocolo_texto,
								   a.id_protocolo associado_id
							  from {0}hst_protocolo                 a,
								   {0}tab_protocolo                 p,
								   {0}tab_setor                     s,
								   {0}lov_historico_artefatos_acoes l,
								   {0}lov_historico_acao            la
							 where a.id_protocolo = p.id
							   and p.setor = s.id(+)
							   and a.acao_executada = l.id(+)
							   and l.acao = la.id(+)
							   and l.acao = 33/*Converter*/)
					 where 0 = 0" + comandtxt + DaHelper.Ordenar(colunas, ordenar), (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					HistoricoProtocolo historico;

					while (reader.Read())
					{
						historico = new HistoricoProtocolo();
						historico.Numero = reader["numero"].ToString();
						historico.AcaoId = Convert.ToInt32(reader["acao_id"]);
						historico.AcaoTexto = reader["acao"].ToString();
						historico.AcaoData.Data = Convert.ToDateTime(reader["data_execucao"]);

						if (reader["executor_id"] != null && !Convert.IsDBNull(reader["executor_id"]))
						{
							historico.Executor.Id = Convert.ToInt32(reader["executor_id"]);
							historico.Executor.Nome = reader["executor_nome"].ToString();
						}

						if (reader["setor_id"] != null && !Convert.IsDBNull(reader["setor_id"]))
						{
							historico.Setor.Id = Convert.ToInt32(reader["setor_id"]);
							historico.Setor.Sigla = reader["setor_sigla"].ToString();
						}

						historico.ProtocoloTipoId = reader.GetValue<Int32>("tipo_id");

						retorno.Itens.Add(historico);
					}

					reader.Close();

					#endregion
				}
			}

			return retorno;
		}

		internal string ObterNumeroProcessoPai(int? id)
		{
			// Verificar se o protocolo está apensado a algum protocolo
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select p.numero||'/'||p.ano numero_processo_pai from {0}tab_protocolo_associado a, {0}tab_protocolo p 
				where p.id = a.protocolo and a.associado = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				object retorno = bancoDeDados.ExecutarScalar(comando);
				if (retorno != null)
				{
					return retorno.ToString();
				}
				return string.Empty;
			}
		}

		internal List<string> ObterNumeroProcessoFilhos(int? id)
		{
			List<string> lstNumProcessos = new List<string>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select pf.numero||'/'||pf.ano numero from {0}tab_protocolo_associado p, {0}tab_protocolo pf where p.tipo = 1 and p.protocolo = :id and p.associado = pf.id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						lstNumProcessos.Add(reader["numero"].ToString());
					}

					reader.Close();
				}
				return lstNumProcessos;
			}
		}

		internal List<string> ObterNumeroDocumentosFilhos(int? id)
		{
			List<string> lstNumDocumentos = new List<string>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select d.numero||'/'||d.ano numero from {0}tab_protocolo_associado p, {0}tab_protocolo d where p.tipo = 2 and p.protocolo = :id and p.associado = d.id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						lstNumDocumentos.Add(reader["numero"].ToString());
					}

					reader.Close();
				}
				return lstNumDocumentos;
			}
		}

		internal List<Atividade> ObterAtividadesSolicitadas(int protocoloId, BancoDeDados banco = null)
		{
			List<Atividade> listaAtividades = new List<Atividade>();

			Protocolo protocolo = new Protocolo();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Atividades

				Comando comando = bancoDeDados.CriarComando(@"select tpas.*, tpa.setor, tpa.atividade atividade_texto, lpas.texto situacao_texto
					from {0}tab_protocolo_atividades tpas, {0}tab_atividade tpa, {0}lov_atividade_situacao lpas 
					where tpas.atividade = tpa.id and tpas.situacao = lpas.id and tpas.protocolo = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", protocoloId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Atividade atividade;

					while (reader.Read())
					{
						atividade = new Atividade();
						atividade.Id = Convert.ToInt32(reader["atividade"]);
						atividade.IdRelacionamento = Convert.ToInt32(reader["id"]);
						atividade.SituacaoId = Convert.ToInt32(reader["situacao"]);
						atividade.SituacaoTexto = reader["situacao_texto"].ToString();
						atividade.NomeAtividade = reader["atividade_texto"].ToString();
						atividade.SetorId = Convert.ToInt32(reader["setor"]);
						atividade.Tid = reader["tid"].ToString();
						listaAtividades.Add(atividade);
					}
					reader.Close();
				}

				#endregion
			}

			return listaAtividades;
		}

		public Finalidade ObterTituloAnteriorAtividade(int atividade, int protocolo, int modeloCodigo)
		{
			Finalidade finalidade = new Finalidade();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select tpaf.titulo_anterior_id, tpaf.titulo_anterior_numero numero_documento, tpaf.modelo_anterior_nome modelo_nome,
				tpaf.modelo_anterior_sigla  modelo_sigla, tpaf.orgao_expedidor from {0}tab_protocolo_atividades tpa, {0}tab_protocolo_ativ_finalida tpaf, {0}tab_titulo_modelo ttm
				where tpaf.titulo_anterior_numero is not null and tpaf.modelo = ttm.id and tpa.id = tpaf.protocolo_ativ and tpa.protocolo = :protocolo and tpa.atividade = :atividade
				and ttm.codigo = :modelo_codigo", EsquemaBanco);

				comando.AdicionarParametroEntrada("atividade", atividade, DbType.Int32);
				comando.AdicionarParametroEntrada("protocolo", protocolo, DbType.Int32);
				comando.AdicionarParametroEntrada("modelo_codigo", modeloCodigo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						if (reader["titulo_anterior_id"] != DBNull.Value && reader["titulo_anterior_id"] != null)
						{
							finalidade.TituloAnteriorId = Convert.ToInt32(reader["titulo_anterior_id"]);
						}
						finalidade.TituloAnteriorNumero = reader["numero_documento"].ToString();
						finalidade.TituloModeloAnteriorTexto = reader["modelo_nome"].ToString();
						finalidade.TituloModeloAnteriorSigla = reader["modelo_sigla"].ToString();
						finalidade.OrgaoExpedidor = reader["orgao_expedidor"].ToString();
					}
					reader.Close();
				}
			}

			return finalidade;
		}

		internal List<PessoaLst> ObterInteressadoRepresentantes(int id, BancoDeDados banco = null)
		{
			List<PessoaLst> lista = new List<PessoaLst>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Representantes Interessado

				Comando comando = bancoDeDados.CriarComando(@"select p.id, p.nome from {0}tab_protocolo t, {0}tab_pessoa_representante pr, {0}tab_pessoa p
				where t.interessado = pr.pessoa and pr.representante = p.id and t.id = :id order by p.nome", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					PessoaLst item;

					while (reader.Read())
					{
						item = new PessoaLst();
						item.IsAtivo = true;
						item.Id = Convert.ToInt32(reader["id"]);
						item.Texto = reader["nome"].ToString();
						lista.Add(item);
					}

					reader.Close();
				}

				#endregion
			}
			return lista;
		}

		internal int ObterSetor(int protocolo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
					select nvl(p.setor,
							   (select tp.setor
								  from {0}tab_protocolo_associado tpa, 
									   {0}tab_protocolo tp
								 where tp.id = tpa.protocolo
								   and tpa.associado = :protocolo)) setor
					  from {0}tab_protocolo p
					 where p.id = :protocolo", EsquemaBanco);

				comando.AdicionarParametroEntrada("protocolo", protocolo, DbType.Int32);
				object retorno = bancoDeDados.ExecutarScalar(comando);
				return (retorno != null && !Convert.IsDBNull(retorno)) ? Convert.ToInt32(retorno) : 0;
			}
		}

		internal ProtocoloNumero ObterProtocolo(string numero)
		{
			ProtocoloNumero protocolo = new ProtocoloNumero(numero);

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select tp.id, tp.numero || '/' || tp.ano numero_ano, tp.protocolo, tp.tipo,
					(select l.texto from {0}lov_protocolo_tipo l where l.id = tp.tipo) tipo_texto from {0}tab_protocolo tp 
				where tp.numero = :numero and tp.ano = :ano", EsquemaBanco);

				comando.AdicionarParametroEntrada("numero", protocolo.Numero, DbType.Int32);
				comando.AdicionarParametroEntrada("ano", protocolo.Ano, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						protocolo.Id = Convert.ToInt32(reader["id"]);
						protocolo.IsProcesso = Convert.ToInt32(reader["protocolo"]) == 1;
						protocolo.Tipo = Convert.ToInt32(reader["tipo"]);
						protocolo.TipoTexto = reader["tipo_texto"].ToString();
					}

					reader.Close();
				}
			}
			return protocolo;
		}

		internal ProtocoloNumero ObterProtocolo(int id)
		{
			ProtocoloNumero protocolo = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select tp.numero || '/' || tp.ano numero_ano, tp.protocolo, tp.tipo,
					(select l.texto from {0}lov_protocolo_tipo l where l.id = tp.tipo) tipo_texto from {0}tab_protocolo tp where tp.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						protocolo = new ProtocoloNumero(reader["numero_ano"].ToString());
						protocolo.Id = id;
						protocolo.IsProcesso = Convert.ToInt32(reader["protocolo"]) == 1;
						protocolo.Tipo = Convert.ToInt32(reader["tipo"]);
						protocolo.TipoTexto = reader["tipo_texto"].ToString();
					}

					reader.Close();
				}
			}
			return protocolo;
		}

		internal List<PessoaLst> ObterResposaveisTecnicos(int id, BancoDeDados banco = null)
		{
			List<PessoaLst> responsaveis = new List<PessoaLst>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select tp.id, nvl(tp.nome, tp.razao_social) nome_razao_social
				from {0}tab_protocolo_responsavel tpr, {0}tab_pessoa tp where tpr.responsavel = tp.id and tpr.protocolo = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						PessoaLst pessoa = new PessoaLst();
						pessoa.Id = Convert.ToInt32(reader["id"]);
						pessoa.Texto = reader["nome_razao_social"].ToString();
						pessoa.IsAtivo = true;
						responsaveis.Add(pessoa);
					}

					reader.Close();
				}
			}

			return responsaveis;
		}

		internal List<PessoaLst> ObterResponsaveisTecnicosPorRequerimento(int id, BancoDeDados banco = null)
		{
			List<PessoaLst> responsaveis = new List<PessoaLst>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select tp.id, nvl(tp.nome, tp.razao_social) nome_razao_social
				from {0}tab_requerimento_responsavel tpr, {0}tab_pessoa tp where tpr.responsavel = tp.id and tpr.requerimento = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						PessoaLst pessoa = new PessoaLst();
						pessoa.Id = Convert.ToInt32(reader["id"]);
						pessoa.Texto = reader["nome_razao_social"].ToString();
						pessoa.IsAtivo = true;
						responsaveis.Add(pessoa);
					}

					reader.Close();
				}
			}

			return responsaveis;
		}

		public List<Requerimento> ObterProtocoloRequerimentos(int protocolo)
		{
			List<Requerimento> requerimentos = new List<Requerimento>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Requerimentos
				Comando comando = bancoDeDados.CriarComando(@"select requerimento, tmp.etapa_importacao, r.data_criacao, a.checagem, a.id, tipo, r.autor
															from (select p.requerimento, p.checagem, p.id, 1 tipo from {0}tab_protocolo p where p.requerimento is not null
															and p.id = :protocolo  union all select p.requerimento, p.checagem, p.id, pa.tipo from {0}tab_protocolo_associado pa, 
															{0}tab_protocolo p where p.id = pa.associado and p.requerimento is not null and pa.protocolo = :protocolo) a,
															{0}tab_requerimento r, {0}tmp_projeto_digital tmp where r.id = a.requerimento and tmp.requerimento_id(+) = r.id
															order by 2", EsquemaBanco);

				comando.AdicionarParametroEntrada("protocolo", protocolo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Requerimento requerimento;
					while (reader.Read())
					{
						requerimento = new Requerimento();
						if (reader["requerimento"] != null && !Convert.IsDBNull(reader["requerimento"]))
						{
							requerimento.Id = Convert.ToInt32(reader["requerimento"]);
							requerimento.AutorId = reader.GetValue<Int32>("autor");
						}

						if (reader["checagem"] != null && !Convert.IsDBNull(reader["checagem"]))
						{
							requerimento.Checagem = Convert.ToInt32(reader["checagem"]);
						}

						if (reader["id"] != null && !Convert.IsDBNull(reader["id"]))
						{
							requerimento.ProtocoloId = Convert.ToInt32(reader["id"]);
						}

						if (reader["tipo"] != null && !Convert.IsDBNull(reader["tipo"]))
						{
							requerimento.ProtocoloTipo = Convert.ToInt32(reader["tipo"]);
						}

						requerimento.DataCadastro = Convert.ToDateTime(reader["data_criacao"]);
						requerimento.EtapaImportacao = reader.GetValue<Int32>("etapa_importacao");

						requerimentos.Add(requerimento);
					}
					reader.Close();
				}
				#endregion
			}
			return requerimentos;
		}

		public List<Atividade> ObterProtocoloAtividades(int requerimento, BancoDeDados banco = null)
		{
			List<Atividade> atividades = new List<Atividade>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Requerimentos
				Comando comando = bancoDeDados.CriarComando(@"select tpa.situacao from {0}tab_protocolo_atividades tpa where tpa.requerimento = :requerimento_id", EsquemaBanco);

				comando.AdicionarParametroEntrada("requerimento_id", requerimento, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Atividade ativ;
					while (reader.Read())
					{
						ativ = new Atividade();
						ativ.SituacaoId = Convert.ToInt32(reader["situacao"]);
						atividades.Add(ativ);
					}
					reader.Close();
				}
				#endregion
			}
			return atividades;
		}

		public List<Atividade> ObterAtividadesProtocolo(int processoId, BancoDeDados banco = null)
		{
			List<Atividade> atividades = new List<Atividade>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Requerimentos
				Comando comando = bancoDeDados.CriarComando(@"select tpa.situacao from {0}tab_protocolo_atividades tpa where tpa.protocolo = :protocolo_id", EsquemaBanco);

				comando.AdicionarParametroEntrada("protocolo_id", processoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Atividade ativ;
					while (reader.Read())
					{
						ativ = new Atividade();
						ativ.SituacaoId = Convert.ToInt32(reader["situacao"]);
						atividades.Add(ativ);
					}
					reader.Close();
				}
				#endregion
			}
			return atividades;
		}

		internal List<ListaValor> ObterAssinanteFuncionarios(int setorId, int cargoId, BancoDeDados banco = null)
		{
			List<ListaValor> lst = new List<ListaValor>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@" select distinct t.* from ( select tf.id, tf.nome from {0}tab_funcionario tf, {0}tab_setor ts, {0}tab_funcionario_cargo tfc, 
					{0}tab_titulo_modelo_assinantes ttma where ts.responsavel = tf.id and tf.id = tfc.funcionario and ttma.setor = ts.id and ttma.tipo_assinante = 1 and ts.id 
					= :setorId and tfc.cargo = :cargoId union all select tf.id, tf.nome from {0}tab_funcionario tf, {0}tab_funcionario_setor ts, {0}tab_funcionario_cargo tfc, {0}tab_titulo_modelo_assinantes ttma
					where ts.funcionario = tf.id and tf.id = tfc.funcionario and ttma.setor = ts.setor and ttma.tipo_assinante = 2 and ts.setor = :setorId and tfc.cargo = 
					:cargoId ) t order by t.nome ", EsquemaBanco);

				comando.AdicionarParametroEntrada("setorId", setorId, DbType.Int32);
				comando.AdicionarParametroEntrada("cargoId", cargoId, DbType.Int32);

				IEnumerable<IDataReader> daReader = DaHelper.ObterLista(comando, bancoDeDados);

				foreach (var item in daReader)
				{
					lst.Add(new ListaValor()
					{
						Id = Convert.ToInt32(item["id"]),
						Texto = item["nome"].ToString(),
						IsAtivo = true
					});
				}
			}

			return lst;
		}

		internal List<ListaValor> ObterAssinanteCargos(int setorId, BancoDeDados banco = null)
		{
			List<ListaValor> lst = new List<ListaValor>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@" select distinct t.* from ( select  tc.id, tc.nome from {0}tab_funcionario tf, {0}tab_setor ts, {0}tab_funcionario_setor tse, 
															{0}tab_funcionario_cargo tfc, {0}tab_cargo tc, {0}tab_titulo_modelo_assinantes ttma 
															where ts.responsavel = tf.id and tf.id = tfc.funcionario and ttma.setor = ts.id and tfc.cargo = tc.id
															and ttma.tipo_assinante = 1 and ts.id = :setorId 
															union all           
															select tc.id, tc.nome from {0}tab_funcionario tf, {0}tab_funcionario_setor ts, 
															{0}tab_funcionario_cargo tfc, {0}tab_cargo tc, {0}tab_titulo_modelo_assinantes ttma 
															where ts.funcionario = tf.id and tf.id = tfc.funcionario and ttma.setor = ts.setor and tfc.cargo = 
															tc.id and ttma.tipo_assinante = 2
															and ts.setor = :setorId ) t order by t.nome ", EsquemaBanco);

				comando.AdicionarParametroEntrada("setorId", setorId, DbType.Int32);

				IEnumerable<IDataReader> daReader = DaHelper.ObterLista(comando, bancoDeDados);

				foreach (var item in daReader)
				{
					lst.Add(new ListaValor()
					{
						Id = Convert.ToInt32(item["id"]),
						Texto = item["nome"].ToString(),
						IsAtivo = true
					});
				}
			}

			return lst;
		}

		#endregion

		#region Validações

		public ProtocoloNumero VerificarProtocoloAssociado(int associado)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				ProtocoloNumero protocolo = null;

				#region Protocolo

				Comando comando = bancoDeDados.CriarComando(@"
				select p.id, p.numero || '/' || p.ano numero, p.tipo, p.protocolo
				  from tab_protocolo_associado a, tab_protocolo p
				 where a.protocolo = p.id
				   and a.associado = :associado", EsquemaBanco);

				comando.AdicionarParametroEntrada("associado", associado, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						protocolo = new ProtocoloNumero(reader["numero"].ToString());
						protocolo.Id = Convert.ToInt32(reader["id"]);
						protocolo.Tipo = Convert.ToInt32(reader["tipo"]);
						protocolo.IsProcesso = Convert.ToInt32(reader["protocolo"]) == (int)eTipoProtocolo.Processo;
					}
					reader.Close();
				}

				#endregion

				return protocolo;
			}
		}

		public List<String> VerificarProtocoloAssociadoOutroProtocolo(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				List<String> lista = new List<String>();

				Comando comando = bancoDeDados.CriarComando(@"select p.id, p.numero || '/' || p.ano numero from {0}tab_protocolo p where p.protocolo_associado = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						lista.Add(reader["numero"].ToString());
					}
					reader.Close();
				}

				return lista;
			}
		}

		public int ExisteProtocolo(string numero, int excetoId = 0, int? protocoloTipo = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select id from {0}tab_protocolo a where a.numero = :numero and a.ano = :ano and a.id != :excetoId", EsquemaBanco);

				ProtocoloNumero protocolo = new ProtocoloNumero(numero);
				comando.AdicionarParametroEntrada("numero", protocolo.Numero, DbType.Int32);
				comando.AdicionarParametroEntrada("ano", protocolo.Ano, DbType.Int32);
				comando.AdicionarParametroEntrada("excetoId", excetoId, DbType.Int32);

				if (protocoloTipo != null)
				{
					comando.DbCommand.CommandText += comando.FiltroAnd("a.protocolo", "protocolo", protocoloTipo);
				}

				object retorno = bancoDeDados.ExecutarScalar(comando);
				return (retorno != null && !Convert.IsDBNull(retorno)) ? Convert.ToInt32(retorno) : 0;
			}
		}

		public bool ExisteProtocolo(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(id) from {0}tab_protocolo p where p.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		public bool ExisteRequerimento(int protocolo, bool somenteFilhos = false)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Existe Requerimentos

				Comando comando = bancoDeDados.CriarComando(@"select (select count(*) from {0}tab_protocolo p where p.id = :protocolo and p.requerimento is not null and p.id != :excetoId)
				+ (select count(*) from {0}tab_protocolo p, {0}tab_protocolo_associado pa where pa.associado = p.id
				and pa.protocolo = :protocolo and p.requerimento is not null) valor from dual", EsquemaBanco);

				comando.AdicionarParametroEntrada("protocolo", protocolo, DbType.Int32);
				comando.AdicionarParametroEntrada("excetoId", somenteFilhos ? protocolo : 0, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));

				#endregion
			}
		}

		internal bool ExisteAtividade(int protocolo)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Existe Atividades de protocolo

				Comando comando = bancoDeDados.CriarComando(@"select (select count(*) qtd from {0}tab_protocolo_atividades t where t.protocolo = :protocolo)
				+ (select count(*) qtd from {0}tab_protocolo_atividades t, {0}tab_protocolo_associado pp where pp.protocolo = :protocolo and pp.associado = t.protocolo)
				valor from dual", EsquemaBanco);

				comando.AdicionarParametroEntrada("protocolo", protocolo, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));

				#endregion
			}
		}

		public bool EmPosse(int processoId, int usuarioId)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select  
				(select count(*) from {0}tab_protocolo p where p.id = :protocolo and p.emposse = :funcionario and 
				not exists (select 1 from {0}tab_tramitacao t where t.protocolo = p.id)) 
				+ (select count(*) from {0}tab_protocolo_associado pa, {0}tab_protocolo p where pa.protocolo = p.id and 				
				not exists (select 1 from {0}tab_tramitacao t where t.protocolo = p.id) and p.emposse = :funcionario 
				and pa.associado = :protocolo) valor from dual", EsquemaBanco);

				comando.AdicionarParametroEntrada("protocolo", processoId, DbType.Int32);
				comando.AdicionarParametroEntrada("funcionario", usuarioId, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		public bool EmPosse(int processoId)
		{
			return EmPosse(processoId, User.FuncionarioId);
		}

		internal string ChecagemPendenciaJaAssociada(int checagem, int excetoId = 0)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select a.numero||'/'||a.ano numero from {0}tab_protocolo a where a.checagem_pendencia = :checagem_pendencia and a.id != :excetoId", EsquemaBanco);
				comando.AdicionarParametroEntrada("checagem_pendencia", checagem, DbType.Int32);
				comando.AdicionarParametroEntrada("excetoId", excetoId, DbType.Int32);
				object resultado = bancoDeDados.ExecutarScalar(comando);
				return ((resultado != DBNull.Value && resultado != null) ? resultado.ToString() : string.Empty);
			}
		}

		internal bool VerificarPossuiAnalise(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from {0}tab_analise a where a.protocolo = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		internal List<Situacao> VerificarPossuiCARSolicitacao(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select s.situacao Id, ls.texto Nome from tab_car_solicitacao s, lov_car_solicitacao_situacao ls where s.situacao = ls.id and s.protocolo = :id 
				union all 
				select s.situacao Id, ls.texto Nome from tab_car_solicitacao s, lov_car_solicitacao_situacao ls where s.situacao = ls.id and s.protocolo_selecionado = :id ", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return bancoDeDados.ObterEntityList<Situacao>(comando);
			}
		}

		internal List<String> VerificarAtividadeAssociadaTitulo(int protocolo)
		{
			List<String> titulos = new List<String>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				//Tabelas filhas de especificidade que serao ignoradas
				List<String> tabelas = new List<String>();

				tabelas.Add("esp_certidao_deb_fisc");

				Comando comando = bancoDeDados.CriarComando(@"select 'select e.titulo from '||c.table_name||' e where e.'||cc.column_name||' = :protocolo union' comando, c.table_name tabela from all_constraints c, all_cons_columns cc
				where cc.constraint_name = c.constraint_name and c.r_constraint_name = 'PK_PROTOCOLO' and c.table_name like 'ESP_%'");

				string select = string.Empty;
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						if (!tabelas.Contains(reader["tabela"].ToString().ToLower()))
						{
							select += reader["comando"].ToString() + " ";
						}
					}
					reader.Close();
				}

				if (select != string.Empty)
				{
					select = String.Format("select tn.numero|| (case when tn.ano is not null then '/'||tn.ano else null end) titulo_numero from {0}tab_titulo t, {0}tab_titulo_numero tn, {0}tab_titulo_atividades ta, ("
					+ select.Substring(0, select.Length - 6) + @") e where t.id = ta.titulo and t.id = e.titulo and t.id = tn.titulo and ((t.situacao = 5 and t.situacao_motivo not in(1, 4)) or (t.situacao not in (1,5))) 
					group by tn.numero || (case when tn.ano is not null then '/' || tn.ano else null end)", EsquemaBanco);

					comando = bancoDeDados.CriarComando(select);
					comando.AdicionarParametroEntrada("protocolo", protocolo, DbType.Int32);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						while (reader.Read())
						{
							titulos.Add(reader["titulo_numero"].ToString());
						}
						reader.Close();
					}
				}
			}
			return titulos;
		}

		internal List<String> VerificarAssociadoTitulo(int protocolo)
		{
			List<String> titulos = new List<String>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				//Tabelas filhas de especificidade que serao ignoradas
				List<String> tabelas = new List<String>();

				tabelas.Add("esp_certidao_deb_fisc");

				Comando comando = bancoDeDados.CriarComando(@"select 'select e.titulo from {0}'||c.table_name||' e where e.'||cc.column_name||' = :protocolo union' comando, c.table_name tabela from all_constraints c, all_cons_columns cc
				where cc.constraint_name = c.constraint_name and c.r_constraint_name = 'PK_PROTOCOLO' and c.table_name like 'ESP_%'", EsquemaBanco);

				string select = string.Empty;
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						if (!tabelas.Contains(reader["tabela"].ToString().ToLower()))
						{
							select += reader["comando"].ToString() + " ";
						}
					}
					reader.Close();
				}

				if (select != string.Empty)
				{
					select += " select tt.id from tab_titulo tt where tt.protocolo = :protocolo";
					select = String.Format(@"select (case when tn.numero is not null then tn.numero||'/'||tn.ano||' - '||tm.nome else tm.nome end) numero_modelo 
					from tab_titulo t, tab_titulo_numero tn, tab_titulo_modelo tm, (" + select + @") aux where t.id = aux.titulo 
					and tm.id = t.modelo and t.id = tn.titulo(+)", EsquemaBanco);

					comando = bancoDeDados.CriarComando(select);
					comando.AdicionarParametroEntrada("protocolo", protocolo, DbType.Int32);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						while (reader.Read())
						{
							titulos.Add(reader["numero_modelo"].ToString());
						}
						reader.Close();
					}
				}
			}
			return titulos;
		}

		internal List<String> VerificarChecagemTemTituloPendencia(List<int> codigosTitulosPendencia, int id)
		{
			List<String> titulos = new List<String>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@" select 'select e.titulo from {0}'||tm.tabela||' e where e.protocolo = :protocolo union' tabela from {0}tab_titulo_modelo tm", EsquemaBanco);
				comando.DbCommand.CommandText += comando.AdicionarIn("where", "tm.codigo", DbType.Int32, codigosTitulosPendencia);

				string select = string.Empty;
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						select += reader["tabela"].ToString();
					}

					reader.Close();
				}

				if (select != string.Empty)
				{
					select = String.Format("select tn.numero|| (case when tn.ano is not null then '/'||tn.ano else null end) titulo_numero from {0}tab_titulo t, {0}tab_titulo_numero tn, {0}tab_titulo_atividades ta, ("
					+ select.Remove(select.LastIndexOf("union")) + @") e where t.id = ta.titulo and t.id = e.titulo and t.id = tn.titulo and t.situacao <> 1 
					group by tn.numero || (case when tn.ano is not null then '/' || tn.ano else null end)", EsquemaBanco);

					comando = bancoDeDados.CriarComando(select);
					comando.AdicionarParametroEntrada("protocolo", id, DbType.Int32);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						while (reader.Read())
						{
							titulos.Add(reader["titulo_numero"].ToString());
						}

						reader.Close();
					}
				}
			}

			return titulos;
		}

		internal bool VerificarPossuiAtividadesNaoEncerrada(int protocolo)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select (select count(*) qtd from {0}tab_protocolo_atividades t where t.protocolo = :protocolo and t.situacao != 6 )
				+ (select count(*) qtd from {0}tab_protocolo_atividades t, {0}tab_protocolo_associado pp where pp.protocolo = :protocolo and t.situacao != 6 and pp.associado = t.protocolo)
				valor from dual", EsquemaBanco);

				comando.AdicionarParametroEntrada("protocolo", protocolo, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		internal bool VerificarNumeroSEP(Processo processo, out String numeroProcesso)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select p.numero||'/'||p.ano processo_numero, p.id processo_id from 
															{0}tab_protocolo p where p.numero_autuacao = :numeroAutuacao", EsquemaBanco);

				comando.AdicionarParametroEntrada("numeroAutuacao", processo.NumeroAutuacao, DbType.String);

				numeroProcesso = String.Empty;

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						if (processo.Id == Convert.ToInt32(reader["processo_id"]))
						{
							return false;
						}
						numeroProcesso = reader["processo_numero"].ToString();
						return true;
					}
					reader.Close();
				}
			}
			return false;
		}

		internal String ValidarFiscalizacaoAssociadaOutroProtocolo(int protocoloId, int fiscalizacaoId)
		{

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{

				Comando comando = bancoDeDados.CriarComando(@"select p.numero||'/'||p.ano numero, p.id protocolo from {0}tab_protocolo p where p.fiscalizacao = :fiscalizacao", EsquemaBanco);
				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacaoId, DbType.String);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					String numero = String.Empty;

					if (reader.Read())
					{
						if (Convert.ToInt32(reader["protocolo"]) == protocoloId)
						{
							numero = String.Empty;
						}
						else
						{
							numero = reader["numero"].ToString();
						}
					}
					else
					{
						numero = String.Empty;
					}

					reader.Close();

					return numero;
				}
			}
		}

		#endregion
	}
}