using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloProcesso;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Publico.Model.ModuloProtocolo.Data
{
	class ProtocoloDa
	{
		#region Protocolo

		private string EsquemaBanco { get; set; }

		#endregion

		#region Obter / Filtrar

		internal Resultados<Protocolo> Filtrar(Filtro<ListarProtocoloFiltro> filtros, BancoDeDados banco = null)
		{
			Resultados<Protocolo> retorno = new Resultados<Protocolo>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAnd("e.protocolo", "protocolo", filtros.Dados.ProtocoloId);

				comandtxt += comando.FiltroAnd("e.numero", "numero", filtros.Dados.Protocolo.Numero);

				comandtxt += comando.FiltroAnd("e.ano", "ano", filtros.Dados.Protocolo.Ano);

				comandtxt += comando.FiltroAndLike("e.numero_autuacao", "numero_autuacao", filtros.Dados.NumeroAutuacao);

				comandtxt += comando.FiltroIn("e.setor_criacao_id", string.Format("select tse.setor from {0}tab_setor_endereco tse where tse.municipio = :municipio", (string.IsNullOrEmpty(EsquemaBanco) ? "" : ".")), "municipio", filtros.Dados.Municipio);

				if (!filtros.Dados.DataRegistro.IsEmpty && filtros.Dados.DataRegistro.IsValido)
				{
					comandtxt += comando.FiltroAnd("TO_DATE(e.data_criacao)", "data_criacao", filtros.Dados.DataRegistro.DataTexto);
				}

				if (!filtros.Dados.DataAutuacao.IsEmpty && filtros.Dados.DataAutuacao.IsValido)
				{
					comandtxt += comando.FiltroAnd("TO_DATE(e.data_autuacao)", "data_autuacao", filtros.Dados.DataAutuacao.DataTexto);
				}

				comandtxt += comando.FiltroAnd("e.tipo_id", "tipo", filtros.Dados.Tipo);

				comandtxt += comando.FiltroIn("e.protocolo_id", String.Format("select s.protocolo from {0}tab_protocolo_atividades s where s.situacao = :atividade_situacao_id", EsquemaBanco),
				"atividade_situacao_id", filtros.Dados.SituacaoAtividade);

				comandtxt += comando.FiltroIn("e.protocolo_id", String.Format("select a.protocolo from {0}tab_protocolo_atividades a where a.atividade = :atividade_id", EsquemaBanco),
				"atividade_id", filtros.Dados.AtividadeSolicitada);

				comandtxt += comando.FiltroAndLike("e.interessado_nome_razao", "interessado_nome_razao", filtros.Dados.InteressadoNomeRazao, true);

				if (!String.IsNullOrWhiteSpace(filtros.Dados.InteressadoCpfCnpj))
				{
					if (ValidacoesGenericasBus.Cpf(filtros.Dados.InteressadoCpfCnpj) ||
						ValidacoesGenericasBus.Cnpj(filtros.Dados.InteressadoCpfCnpj))
					{
						comandtxt += comando.FiltroAnd("e.interessado_cpf_cnpj", "interessado_cpf_cnpj", filtros.Dados.InteressadoCpfCnpj);
					}
					else
					{
						comandtxt += comando.FiltroAndLike("e.interessado_cpf_cnpj", "interessado_cpf_cnpj", filtros.Dados.InteressadoCpfCnpj);
					}
				}

				comandtxt += comando.FiltroAndLike("e.empreendimento_denominador", "empreendimento_denominador", filtros.Dados.EmpreendimentoRazaoDenominacao, true);

				if (!String.IsNullOrWhiteSpace(filtros.Dados.EmpreendimentoCnpj))
				{
					if (ValidacoesGenericasBus.Cnpj(filtros.Dados.EmpreendimentoCnpj))
					{
						comandtxt += comando.FiltroAnd("e.empreendimento_cnpj", "empreendimento_cnpj", filtros.Dados.EmpreendimentoCnpj);
					}
					else
					{
						comandtxt += comando.FiltroAndLike("e.empreendimento_cnpj", "empreendimento_cnpj", filtros.Dados.EmpreendimentoCnpj);
					}
				}

				comandtxt += " and e.tipo_id not in (14, 15) ";

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

				comando.DbCommand.CommandText = String.Format("select count(*) from {0}lst_protocolo e where e.id > 0 " + comandtxt, (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = String.Format(@"select id, protocolo_id, numero, ano, tipo_id, tipo_texto, data_criacao, interessado_id, interessado_tipo, 
					interessado_nome_razao, interessado_cpf_cnpj, interessado_rg_ie, empreendimento_id, numero_autuacao,
					empreendimento_denominador, empreendimento_cnpj, situacao_id, situacao_texto from {0}lst_protocolo e where e.id > 0"
				+ comandtxt + DaHelper.Ordenar(colunas, ordenar), (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					Protocolo item;

					while (reader.Read())
					{
						item = new Protocolo();
						item.Id = Convert.ToInt32(reader["protocolo_id"]);
						item.DataCadastro.Data = Convert.ToDateTime(reader["data_criacao"]);
						item.Tipo.Id = Convert.ToInt32(reader["tipo_id"]);
						item.NumeroAutuacao = reader["numero_autuacao"].ToString();

						if (reader["numero"] != null && !Convert.IsDBNull(reader["numero"]))
						{
							item.NumeroProtocolo = Convert.ToInt32(reader["numero"]);
						}

						if (reader["ano"] != null && !Convert.IsDBNull(reader["ano"]))
						{
							item.Ano = Convert.ToInt32(reader["ano"]);
						}

						item.Tipo.Texto = reader["tipo_texto"].ToString();

						if (reader["interessado_id"] != null && !Convert.IsDBNull(reader["interessado_id"]))
						{
							item.Interessado.Id = Convert.ToInt32(reader["interessado_id"]);
							item.Interessado.Tipo = Convert.ToInt32(reader["interessado_tipo"]);

							if (reader["interessado_tipo"].ToString() == "1")
							{
								item.Interessado.Fisica.Nome = reader["interessado_nome_razao"].ToString();
							}
							else
							{
								item.Interessado.Juridica.RazaoSocial = reader["interessado_nome_razao"].ToString();
							}
						}

						if (reader["empreendimento_id"] != null && !Convert.IsDBNull(reader["empreendimento_id"]))
						{
							item.Empreendimento.Id = Convert.ToInt32(reader["empreendimento_id"]);
							item.Empreendimento.Denominador = reader["empreendimento_denominador"].ToString();
						}

						if (reader["situacao_id"] != null && !Convert.IsDBNull(reader["situacao_id"]))
						{
							item.SituacaoId = Convert.ToInt32(reader["situacao_id"]);
							item.SituacaoTexto = reader["situacao_texto"].ToString();
						}

						retorno.Itens.Add(item);
					}

					reader.Close();

					#endregion
				}
			}
			return retorno;
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
						   lfs.texto fiscalizacao_sit_texto,
						   r.interessado_livre,
						   r.interessado_livre_telefone,
						   r.folhas, r.assunto, r.descricao, r.destinatario, r.destinatario_setor, d.id destinatario_id, 
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

		#endregion
	}
}
