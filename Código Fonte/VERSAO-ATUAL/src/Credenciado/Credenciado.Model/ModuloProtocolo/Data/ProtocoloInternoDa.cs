using System;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloProcesso;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloProtocolo.Data
{
	public class ProtocoloInternoDa
	{
		#region Propriedades

		private string EsquemaBanco { get; set; }

		#endregion

		public ProtocoloInternoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

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
						   lfs.texto fiscalizacao_sit_texto
					  from {0}tab_protocolo          r,
						   {0}tab_pessoa             p,
						   {0}tab_fiscalizacao       f,
						   {0}tab_empreendimento     e,
						   {0}lov_protocolo_situacao ls,
						   {0}lov_protocolo_tipo     lpt,
						   {0}tab_requerimento       tr,
						   {0}lov_fiscalizacao_situacao lfs
					 where r.interessado = p.id(+)
					   and r.empreendimento = e.id(+)
					   and r.situacao = ls.id
					   and lpt.id = r.tipo
					   and r.requerimento = tr.id(+)
					   and r.fiscalizacao = f.id(+)
					   and f.situacao = lfs.id(+)
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

		public IProtocolo ObterAtividades(int id, BancoDeDados banco = null)
		{
			return Obter(id, atividades: true, banco: banco);
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

		public bool ExisteProtocolo(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(id) from {0}tab_protocolo p where p.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
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
	}
}