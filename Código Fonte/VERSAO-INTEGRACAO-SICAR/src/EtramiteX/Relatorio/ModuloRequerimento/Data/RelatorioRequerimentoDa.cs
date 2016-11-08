using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.Entities;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloRequerimento;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloRequerimento.Data
{
	class RelatorioRequerimentoDa : IRelatorioRequerimentoDa
	{
		#region Propriedades

		private String EsquemaBanco { get; set; }
		private String EsquemaBancoInterno { get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioInterno); } } 
		public String EsquemaBancoCredenciado { get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); } }

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		public Int32 RoteiroPadrao
		{
			get { return _configSys.Obter<Int32>(ConfiguracaoSistema.KeyRoteiroPadrao); }
		}

		#endregion

		public RelatorioRequerimentoDa(String strBancoDeDados = null)
		{
			EsquemaBanco = String.Empty;
			if (!String.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		public int ObterSituacao(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select r.situacao from tab_requerimento r where r.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return bancoDeDados.ExecutarScalar<int>(comando);
			}
		}

		public RequerimentoRelatorio Obter(int id, BancoDeDados banco = null)
		{
			RequerimentoRelatorio requerimento = new RequerimentoRelatorio();
			int aux = 0;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Requerimento

				Comando comando = bancoDeDados.CriarComando(@"
				select r.id,
					r.interessado interessado_id,
					r.empreendimento empreendimento_id,
					rs.id situacao_id,
					rs.texto situacao_texto,
					ra.texto agendamento_texto,
					sm.texto municipio_texto,
					s.id setor_id,
					r.informacoes,
					r.autor autor_id,
					trunc(r.data_criacao) data_criacao,
					to_char(r.data_criacao, 'dd') dia,
					to_char(r.data_criacao, 'MM') mes,
					to_char(r.data_criacao, 'yyyy') ano
				from {0}tab_requerimento             r,
					{0}lov_requerimento_situacao    rs,
					{0}lov_requerimento_agendamento ra,
					{0}tab_setor                    s,
					{0}tab_setor_endereco           se,
					{0}lov_municipio                sm
				where r.situacao = rs.id
				and r.agendamento = ra.id
				and r.setor = s.id
				and s.id = se.setor
				and se.municipio = sm.id
				and r.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						requerimento.Id = id;
						requerimento.Interessado.Id = reader.GetValue<int>("interessado_id");
						requerimento.Empreendimento.Id = reader.GetValue<int>("empreendimento_id");
						requerimento.SituacaoId = reader.GetValue<int>("situacao_id");
						requerimento.SituacaoTexto = reader.GetValue<string>("situacao_texto");
						requerimento.AgendamentoVistoria = reader.GetValue<string>("agendamento_texto");
						requerimento.Municipio = reader.GetValue<string>("municipio_texto");
						requerimento.SetorId = reader.GetValue<int>("setor_id");
						requerimento.Informacoes = reader.GetValue<string>("informacoes");
						requerimento.DataCadastro = reader.GetValue<DateTime>("data_criacao");
						requerimento.DiaCadastro = reader.GetValue<string>("dia");
						int mes = reader.GetValue<int>("mes");
						requerimento.MesCadastro = _configSys.Obter<List<String>>(ConfiguracaoSistema.KeyMeses).ElementAt(mes - 1);
						requerimento.AnoCadastro = reader.GetValue<string>("ano");
						requerimento.AutorId = reader.GetValue<int>("autor_id");
						
					}

					reader.Close();
				}

				#endregion

				#region Atividades

				comando = bancoDeDados.CriarComando(@"
									select a.id, a.atividade, a.tid, b.atividade atividade_texto, b.conclusao
									  from {0}tab_requerimento_atividade a, {0}tab_atividade b
									 where a.atividade = b.id
									   and a.requerimento = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					RequerimentoAtividadeRelatorio atividade;

					while (reader.Read())
					{
						atividade = new RequerimentoAtividadeRelatorio();
						atividade.IdRelacionamento = Convert.ToInt32(reader["id"]);
						atividade.Id = Convert.ToInt32(reader["atividade"]);
						atividade.NomeAtividade = reader["atividade_texto"].ToString();
						atividade.Conclusao = reader["conclusao"].ToString();

						#region Atividades/Finalidades/Modelos

						comando = bancoDeDados.CriarComando(@"
												select a.id,
														a.finalidade,
														ltf.texto                finalidade_texto,
														a.modelo,
														tm.nome                  modelo_nome,
														a.titulo_anterior_tipo,
														a.titulo_anterior_id,
														a.titulo_anterior_numero,
														a.modelo_anterior_id,
														a.modelo_anterior_nome,
														a.modelo_anterior_sigla,
														a.orgao_expedidor
													from {0}tab_requerimento_ativ_finalida a,
														 {0}tab_titulo_modelo              tm,
														 {0}lov_titulo_finalidade          ltf
													where a.modelo = tm.id
													and a.finalidade = ltf.id(+)
													and a.requerimento_ativ = :id", EsquemaBanco);

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

						requerimento.Atividades.Add(atividade);
					}

					reader.Close();
				}

				#endregion

				#region Interessado

				comando = bancoDeDados.CriarComando(@"
				select p.id,
					p.tipo,
					p.cpf,
					p.nome,
					p.rg,
					p.apelido,
					tp.texto profissao,
					p.cnpj,
					p.razao_social,
					p.ie,
					p.nome_fantasia
				from {0}tab_requerimento    r,
					{0}tab_pessoa           p,
					{0}tab_pessoa_profissao pp,
					{0}tab_profissao        tp
				where r.interessado = p.id
				and p.id = pp.pessoa(+)
				and pp.profissao = tp.id(+)
				and r.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						aux = reader.GetValue<int>("id");

						requerimento.Interessado.Id = requerimento.Interessado.Id;
						requerimento.Interessado.Tipo = reader.GetValue<int>("tipo");

						if (requerimento.Interessado.IsFisica)
						{
							requerimento.Interessado.Fisica.CPF = reader.GetValue<string>("cpf");
							requerimento.Interessado.Fisica.Nome = reader.GetValue<string>("nome");
							requerimento.Interessado.Fisica.RG = reader.GetValue<string>("rg");
							requerimento.Interessado.Fisica.Apelido = reader.GetValue<string>("apelido");
							requerimento.Interessado.Fisica.Profissao = reader.GetValue<string>("profissao");
						}
						else // juridica
						{
							requerimento.Interessado.Juridica.CNPJ = reader.GetValue<string>("cnpj");
							requerimento.Interessado.Juridica.RazaoSocial = reader.GetValue<string>("razao_social");
							requerimento.Interessado.Juridica.IE = reader.GetValue<string>("ie");
							requerimento.Interessado.Juridica.NomeFantasia = reader.GetValue<string>("nome_fantasia");
						}
					}

					#region Meio de Contato

					comando = bancoDeDados.CriarComando(@"select pc.id, pc.meio_contato meio_contato_id, c.texto contato_texto, pc.valor
					from {0}tab_pessoa_meio_contato pc, {0}tab_meio_contato c where pc.meio_contato = c.id and pc.pessoa = :pessoa", EsquemaBanco);

					comando.AdicionarParametroEntrada("pessoa", aux, DbType.Int32);

					using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
					{
						ContatoRelatorio contato;
						while (readerAux.Read())
						{
							contato = new ContatoRelatorio();
							contato.Id = readerAux.GetValue<int>("id");
							contato.PessoaId = requerimento.Interessado.Id;
							contato.TipoContato = (eTipoContato)Enum.Parse(contato.TipoContato.GetType(), readerAux.GetValue<string>("meio_contato_id"));
							contato.TipoTexto = readerAux.GetValue<string>("contato_texto");
							contato.Valor = readerAux.GetValue<string>("valor");
							requerimento.Interessado.MeiosContatos.Add(contato);
						}

						readerAux.Close();
					}

					#endregion

					#region Endereços

					comando = bancoDeDados.CriarComando(@"
					select e.id,
						e.cep,
						e.logradouro,
						e.bairro,
						ee.id estado_id,
						ee.sigla estado_sigla,
						em.id municipio_id,
						em.texto municipio_texto,
						e.numero,
						e.complemento,
						e.distrito
					from {0}tab_pessoa_endereco e, {0}lov_estado ee, {0}lov_municipio em
					where e.estado = ee.id
					and e.municipio = em.id
					and e.pessoa = :pessoa", EsquemaBanco);

					comando.AdicionarParametroEntrada("pessoa", aux, DbType.Int32);

					using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
					{
						if (readerAux.Read())
						{
							requerimento.Interessado.Endereco.Id = readerAux.GetValue<int>("id");
							requerimento.Interessado.Endereco.Cep = readerAux.GetValue<string>("cep");
							requerimento.Interessado.Endereco.Logradouro = readerAux.GetValue<string>("logradouro");
							requerimento.Interessado.Endereco.Bairro = readerAux.GetValue<string>("bairro");
							requerimento.Interessado.Endereco.EstadoId = readerAux.GetValue<int>("estado_id");
							requerimento.Interessado.Endereco.EstadoSigla = readerAux.GetValue<string>("estado_sigla");
							requerimento.Interessado.Endereco.MunicipioId = readerAux.GetValue<int>("municipio_id");
							requerimento.Interessado.Endereco.MunicipioTexto = readerAux.GetValue<string>("municipio_texto");
							requerimento.Interessado.Endereco.Numero = readerAux.GetValue<string>("numero");
							requerimento.Interessado.Endereco.Complemento = readerAux.GetValue<string>("complemento");
							requerimento.Interessado.Endereco.Distrito = readerAux.GetValue<string>("distrito");
						}

						readerAux.Close();
					}

					#endregion

					reader.Close();
				}

				#endregion

				#region Responsáveis

				comando = bancoDeDados.CriarComando(@"
				select p.id,
					r.responsavel responsavel_id,
					lf.texto funcao_texto,
					r.numero_art,
					nvl(p.nome, p.razao_social) nome,
					nvl(p.cpf, p.cnpj) cpf_cnpj,
					nvl(p.rg, p.ie) rg_ie,
					p.tipo,
					trunc(p.data_nascimento) data_nascimento
				from {0}tab_requerimento_responsavel r, {0}tab_pessoa p, {0}lov_protocolo_resp_funcao lf 
				where r.responsavel = p.id
				and r.funcao = lf.id
				and r.requerimento = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					ResponsavelTecnicoRelatorio responsavel;
					while (reader.Read())
					{
						aux = reader.GetValue<int>("id");

						responsavel = new ResponsavelTecnicoRelatorio();
						responsavel.Id = reader.GetValue<int>("responsavel_id");
						responsavel.FuncaoTexto = reader.GetValue<string>("funcao_texto");
						responsavel.CpfCnpj = reader.GetValue<string>("cpf_cnpj");
						responsavel.RgIe = reader.GetValue<string>("rg_ie");
						responsavel.NomeRazao = reader.GetValue<string>("nome");
						responsavel.NumeroArt = reader.GetValue<string>("numero_art");
						responsavel.DataVencimento = "Falta";

						if (reader["data_nascimento"] != null && !Convert.IsDBNull(reader["data_nascimento"]))
						{
							responsavel.DataNascimento = reader.GetValue<DateTime>("data_nascimento").ToShortDateString();
						}

						#region Meio de Contato

						comando = bancoDeDados.CriarComando(@"select pc.id, pc.meio_contato meio_contato_id, c.texto contato_texto, pc.valor
						from {0}tab_pessoa_meio_contato pc, {0}tab_meio_contato c where pc.meio_contato = c.id and pc.pessoa = :pessoa", EsquemaBanco);

						comando.AdicionarParametroEntrada("pessoa", aux, DbType.Int32);

						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							ContatoRelatorio contato;
							while (readerAux.Read())
							{
								contato = new ContatoRelatorio();
								contato.Id = readerAux.GetValue<int>("id");
								contato.PessoaId = responsavel.Id;
								contato.TipoContato = (eTipoContato)Enum.Parse(contato.TipoContato.GetType(), readerAux.GetValue<string>("meio_contato_id"));
								contato.TipoTexto = readerAux.GetValue<string>("contato_texto");
								contato.Valor = readerAux.GetValue<string>("valor");
								responsavel.MeiosContatos.Add(contato);
							}

							readerAux.Close();
						}

						#endregion

						#region Endereços

						comando = bancoDeDados.CriarComando(@"
						select e.id,
							e.cep,
							e.logradouro,
							e.bairro,
							ee.id estado_id,
							ee.sigla estado_sigla,
							em.id municipio_id,
							em.texto municipio_texto,
							e.numero,
							e.complemento,
							e.distrito
						from {0}tab_pessoa_endereco e, {0}lov_estado ee, {0}lov_municipio em
						where e.estado = ee.id
						and e.municipio = em.id
						and e.pessoa = :pessoa", EsquemaBanco);

						comando.AdicionarParametroEntrada("pessoa", aux, DbType.Int32);

						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							if (readerAux.Read())
							{
								responsavel.Endereco.Id = readerAux.GetValue<int>("id");
								responsavel.Endereco.Cep = readerAux.GetValue<string>("cep");
								responsavel.Endereco.Logradouro = readerAux.GetValue<string>("logradouro");
								responsavel.Endereco.Bairro = readerAux.GetValue<string>("bairro");
								responsavel.Endereco.EstadoId = readerAux.GetValue<int>("estado_id");
								responsavel.Endereco.EstadoSigla = readerAux.GetValue<string>("estado_sigla");
								responsavel.Endereco.MunicipioId = readerAux.GetValue<int>("municipio_id");
								responsavel.Endereco.MunicipioTexto = readerAux.GetValue<string>("municipio_texto");
								responsavel.Endereco.Numero = readerAux.GetValue<string>("numero");
								responsavel.Endereco.Complemento = readerAux.GetValue<string>("complemento");
								responsavel.Endereco.Distrito = readerAux.GetValue<string>("distrito");
							}

							readerAux.Close();
						}

						#endregion

						requerimento.Responsaveis.Add(responsavel);
					}

					reader.Close();
				}

				#endregion

				#region Pessoa Credenciado

				if (requerimento.AutorId > 0)
				{
					comando = bancoDeDados.CriarComando(@"select p.tipo, tc.id, tc.tipo credenciado_tipo, lt.texto credenciado_tipo_texto,p.cpf, p.nome, p.rg, p.apelido,
														p.cnpj, p.razao_social, p.ie, p.nome_fantasia, o.orgao_nome, o.orgao_sigla from {0}tab_pessoa p, {0}tab_credenciado tc,
														{0}lov_credenciado_tipo lt, {1}tab_orgao_parc_conv o where p.id = tc.pessoa and tc.id = :credenciado_id and lt.id = tc.tipo
														and tc.orgao_parc = o.id(+)", EsquemaBanco, EsquemaBancoCredenciado);

					comando.AdicionarParametroEntrada("credenciado_id", requerimento.AutorId, DbType.Int32);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						if (reader.Read())
						{
							requerimento.UsuarioCredenciado.Id = reader.GetValue<int>("id");
							requerimento.UsuarioCredenciado.PessoaTipo = reader.GetValue<int>("tipo");

							requerimento.UsuarioCredenciado.TipoId = reader.GetValue<int>("credenciado_tipo");
							requerimento.UsuarioCredenciado.TipoTexto = reader.GetValue<String>("credenciado_tipo_texto");
							requerimento.UsuarioCredenciado.OrgaoParceiroNome = reader.GetValue<String>("orgao_nome");
							requerimento.UsuarioCredenciado.OrgaoParceiroSigla = reader.GetValue<String>("orgao_sigla");

							if (requerimento.Interessado.IsFisica)
							{
								requerimento.UsuarioCredenciado.Fisica.CPF = reader.GetValue<string>("cpf");
								requerimento.UsuarioCredenciado.Fisica.Nome = reader.GetValue<string>("nome");
								requerimento.UsuarioCredenciado.Fisica.RG = reader.GetValue<string>("rg");
								requerimento.UsuarioCredenciado.Fisica.Apelido = reader.GetValue<string>("apelido");
							}
							else // juridica
							{
								requerimento.UsuarioCredenciado.Juridica.CNPJ = reader.GetValue<string>("cnpj");
								requerimento.UsuarioCredenciado.Juridica.RazaoSocial = reader.GetValue<string>("razao_social");
								requerimento.UsuarioCredenciado.Juridica.IE = reader.GetValue<string>("ie");
								requerimento.UsuarioCredenciado.Juridica.NomeFantasia = reader.GetValue<string>("nome_fantasia");
							}
														
						}

						reader.Close();

					}
				}

				#endregion

				#region Empreendimento

				comando = bancoDeDados.CriarComando(@"
				select e.id,
					e.cnpj,
					es.texto segmento_texto,
					e.denominador,
					e.nome_fantasia,
					a.atividade,
					e.codigo
				from tab_requerimento             r,
					tab_empreendimento           e,
					lov_empreendimento_segmento  es,
					tab_empreendimento_atividade a
				where r.empreendimento = e.id
				and e.segmento = es.id
				and e.atividade = a.id(+)
				and r.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						aux = reader.GetValue<int>("id");

						requerimento.Empreendimento.Id = requerimento.Empreendimento.Id;
						requerimento.Empreendimento.CNPJ = reader.GetValue<string>("cnpj");
						requerimento.Empreendimento.Codigo = reader.GetValue<int>("codigo");
						requerimento.Empreendimento.SegmentoTexto = reader.GetValue<string>("segmento_texto");
						requerimento.Empreendimento.NomeRazao = reader.GetValue<string>("denominador");
						requerimento.Empreendimento.NomeFantasia = reader.GetValue<string>("nome_fantasia");
						requerimento.Empreendimento.AtividadeTexto = reader.GetValue<string>("atividade");
					}

					#region Meio de Contato

					comando = bancoDeDados.CriarComando(@"select ec.id, c.id tipo_contato_id, c.texto contato_texto, ec.valor
					from {0}tab_empreendimento_contato ec, {0}tab_meio_contato c where ec.meio_contato = c.id and ec.empreendimento = :empreendimento", EsquemaBanco);

					comando.AdicionarParametroEntrada("empreendimento", aux, DbType.Int32);

					using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
					{
						ContatoRelatorio contato;
						while (readerAux.Read())
						{
							contato = new ContatoRelatorio();
							contato.Id = readerAux.GetValue<int>("id");
							contato.PessoaId = requerimento.Empreendimento.Id;
							contato.TipoContato = (eTipoContato)Enum.Parse(contato.TipoContato.GetType(), readerAux.GetValue<string>("tipo_contato_id"));
							contato.TipoTexto = readerAux.GetValue<string>("contato_texto");
							contato.Valor = readerAux.GetValue<string>("valor");
							requerimento.Empreendimento.MeiosContatos.Add(contato);
						}

						readerAux.Close();
					}

					#endregion

					#region Endereços

					comando = bancoDeDados.CriarComando(@"
					select ee.id,
						ee.correspondencia,
						ee.cep,
						ee.logradouro,
						ee.bairro,
						le.id estado_id,
						le.sigla estado_sigla,
						lm.id municipio_id,
						lm.texto municipio_texto,
						ee.numero,
						ee.complemento,
						ee.corrego,
						(case when ee.zona = 1 then 'Urbana' else 'Rural' end) zona,
						ee.distrito,
						ee.complemento
					from {0}tab_empreendimento          e,
						{0}tab_empreendimento_endereco ee,
						{0}lov_estado                  le,
						{0}lov_municipio               lm
					where e.id = ee.empreendimento
					and ee.estado = le.id(+)
					and ee.municipio = lm.id(+)
					and e.id = :empreendimento", EsquemaBanco);

					comando.AdicionarParametroEntrada("empreendimento", aux, DbType.Int32);

					using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
					{
						EnderecoRelatorio end;
						while (readerAux.Read())
						{
							end = new EnderecoRelatorio();
							end.Id = readerAux.GetValue<int>("id");
							end.Correspondencia = readerAux.GetValue<int>("correspondencia");
							end.Cep = readerAux.GetValue<string>("cep");
							end.Logradouro = readerAux.GetValue<string>("logradouro");
							end.Bairro = readerAux.GetValue<string>("bairro");
							end.EstadoId = readerAux.GetValue<int>("estado_id");
							end.EstadoSigla = readerAux.GetValue<string>("estado_sigla");
							end.MunicipioId = readerAux.GetValue<int>("municipio_id");
							end.MunicipioTexto = readerAux.GetValue<string>("municipio_texto");
							end.Numero = readerAux.GetValue<string>("numero");
							end.Complemento = readerAux.GetValue<string>("complemento");
							end.Corrego = readerAux.GetValue<string>("corrego");
							end.Zona = readerAux.GetValue<string>("zona");
							end.Distrito = readerAux.GetValue<string>("distrito");
							end.Complemento = readerAux.GetValue<string>("complemento");
							requerimento.Empreendimento.Enderecos.Add(end);
						}

						readerAux.Close();
					}

					#endregion

					#region Coordenada

					comando = bancoDeDados.CriarComando(@"
					select ec.id coordenada_id,
						ct.id tipo_coordenada_id,
						ct.texto tipo_coordenada_texto,
						cd.id datum_id,
						cd.texto datum_texto,
						ec.easting_utm,
						ec.northing_utm,
						ec.fuso_utm,
						ch.id hemisferio_utm_id,
						ch.texto hemisferio_utm_texto,
						ec.latitude_gms,
						ec.longitude_gms,
						ec.latitude_gdec,
						ec.longitude_gdec,
						efc.texto forma_coleta_texto,
						elc.texto local_coleta_texto
					from {0}tab_empreendimento_coord       ec,
						{0}lov_coordenada_tipo            ct,
						{0}lov_coordenada_datum           cd,
						{0}lov_coordenada_hemisferio      ch,
						{0}lov_empreendimento_forma_colet efc,
						{0}lov_empreendimento_local_colet elc
					where ec.tipo_coordenada = ct.id
					and ec.datum = cd.id
					and ec.hemisferio_utm = ch.id
					and ec.forma_coleta = efc.id
					and ec.local_coleta = elc.id
					and ec.empreendimento = :empreendimento", EsquemaBanco);

					comando.AdicionarParametroEntrada("empreendimento", aux, DbType.Int32);

					using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
					{
						if (readerAux.Read())
						{
							requerimento.Empreendimento.Coordenada.Id = readerAux.GetValue<int>("coordenada_id");
							requerimento.Empreendimento.Coordenada.Tipo.Id = readerAux.GetValue<int>("tipo_coordenada_id");
							requerimento.Empreendimento.Coordenada.Tipo.Texto = readerAux.GetValue<string>("tipo_coordenada_texto");
							requerimento.Empreendimento.Coordenada.Datum.Id = readerAux.GetValue<int>("datum_id");
							requerimento.Empreendimento.Coordenada.DatumTexto = readerAux.GetValue<string>("datum_texto");
							requerimento.Empreendimento.Coordenada.EastingUtm = readerAux.GetValue<double>("easting_utm");
							requerimento.Empreendimento.Coordenada.NorthingUtm = readerAux.GetValue<double>("northing_utm");
							requerimento.Empreendimento.Coordenada.FusoUtm = readerAux.GetValue<int>("fuso_utm");
							requerimento.Empreendimento.Coordenada.HemisferioUtm = readerAux.GetValue<int>("hemisferio_utm_id");
							requerimento.Empreendimento.Coordenada.HemisferioUtmTexto = readerAux.GetValue<string>("hemisferio_utm_texto");
							requerimento.Empreendimento.Coordenada.LatitudeGms = readerAux.GetValue<string>("latitude_gms");
							requerimento.Empreendimento.Coordenada.LongitudeGms = readerAux.GetValue<string>("longitude_gms");
							requerimento.Empreendimento.Coordenada.LatitudeGdec = readerAux.GetValue<double>("latitude_gdec");
							requerimento.Empreendimento.Coordenada.LongitudeGdec = readerAux.GetValue<double>("longitude_gdec");
							requerimento.Empreendimento.Coordenada.FormaColetaTexto = readerAux.GetValue<string>("forma_coleta_texto");
							requerimento.Empreendimento.Coordenada.LocalColetaTexto = readerAux.GetValue<string>("local_coleta_texto");
						}

						readerAux.Close();
					}

					#endregion

					reader.Close();
				}

				#endregion
			}

			return requerimento;
		}

		public RequerimentoRelatorio ObterHistorico(int id, BancoDeDados banco = null)
		{
			RequerimentoRelatorio requerimento = new RequerimentoRelatorio();
			string tid = string.Empty;
			int hst = 0;
			int aux = 0;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Obter TID

				Comando comando = bancoDeDados.CriarComando(@"select (case when r.situacao in (3, 4) then 
				(select h.tid from {0}hst_requerimento h where h.data_execucao = (select max(a.data_execucao) from {0}hst_requerimento a where a.requerimento_id = :id and a.situacao_id = 2)) 
				else r.tid end) tid from {0}tab_requerimento r where r.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				tid = bancoDeDados.ExecutarScalar<string>(comando);

				#endregion

				#region Requerimento

				comando = bancoDeDados.CriarComando(@"
				select r.id,
					r.interessado_id,
					r.empreendimento_id,
					r.situacao_id,
					r.situacao_texto,
					r.agendamento_texto,
					se.municipio_texto,
					r.setor_id,
					r.informacoes,
					r.autor_id,
					r.autor_tid,
					trunc(r.data_criacao) data_criacao,
					to_char(r.data_criacao, 'dd') dia,
					to_char(r.data_criacao, 'MM') mes,
					to_char(r.data_criacao, 'yyyy') ano
				from {0}hst_requerimento r, {0}hst_setor s, {0}hst_setor_endereco se
				where r.setor_id = s.setor_id(+)
				and r.setor_tid = s.tid(+)
				and s.id = se.id_hst
				and r.requerimento_id = :id
				and r.tid = :tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						hst = reader.GetValue<int>("id");

						requerimento.Id = id;
						requerimento.Interessado.Id = reader.GetValue<int>("interessado_id");
						requerimento.Empreendimento.Id = reader.GetValue<int>("empreendimento_id");
						requerimento.SituacaoId = reader.GetValue<int>("situacao_id");
						requerimento.SituacaoTexto = reader.GetValue<string>("situacao_texto");
						requerimento.AgendamentoVistoria = reader.GetValue<string>("agendamento_texto");
						requerimento.Municipio = reader.GetValue<string>("municipio_texto");
						requerimento.SetorId = reader.GetValue<int>("setor_id");
						requerimento.Informacoes = reader.GetValue<string>("informacoes");
						requerimento.DataCadastro = reader.GetValue<DateTime>("data_criacao");
						requerimento.DiaCadastro = reader.GetValue<string>("dia");
						int mes = reader.GetValue<int>("mes");
						requerimento.MesCadastro = _configSys.Obter<List<String>>(ConfiguracaoSistema.KeyMeses).ElementAt(mes - 1);
						requerimento.AnoCadastro = reader.GetValue<string>("ano");

						requerimento.AutorId = reader.GetValue<Int32>("autor_id");
						requerimento.AutorTid = reader.GetValue<String>("autor_tid");
					}

					reader.Close();
				}

				#endregion

				#region Atividades

				comando = bancoDeDados.CriarComando(@"
									select a.id, a.atividade, a.tid, b.atividade atividade_texto, b.conclusao
									  from {0}tab_requerimento_atividade a, {0}tab_atividade b
									 where a.atividade = b.id
									   and a.requerimento = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					RequerimentoAtividadeRelatorio atividade;

					while (reader.Read())
					{
						atividade = new RequerimentoAtividadeRelatorio();
						atividade.IdRelacionamento = Convert.ToInt32(reader["id"]);
						atividade.Id = Convert.ToInt32(reader["atividade"]);
						atividade.NomeAtividade = reader["atividade_texto"].ToString();
						atividade.Conclusao = reader["conclusao"].ToString();

						#region Atividades/Finalidades/Modelos

						comando = bancoDeDados.CriarComando(@"
												select a.id,
														a.finalidade,
														ltf.texto                finalidade_texto,
														a.modelo,
														tm.nome                  modelo_nome,
														a.titulo_anterior_tipo,
														a.titulo_anterior_id,
														a.titulo_anterior_numero,
														a.modelo_anterior_id,
														a.modelo_anterior_nome,
														a.modelo_anterior_sigla,
														a.orgao_expedidor
													from {0}tab_requerimento_ativ_finalida a,
														 {0}tab_titulo_modelo              tm,
														 {0}lov_titulo_finalidade          ltf
													where a.modelo = tm.id
													and a.finalidade = ltf.id(+)
													and a.requerimento_ativ = :id", EsquemaBanco);

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

						requerimento.Atividades.Add(atividade);
					}

					reader.Close();
				}

				#endregion

				#region Interessado

				comando = bancoDeDados.CriarComando(@"
				select p.id,
					p.tipo,
					p.cpf,
					p.nome,
					p.rg,
					p.apelido,
					tp.texto profissao,
					p.cnpj,
					p.razao_social,
					p.ie,
					p.nome_fantasia
				from {0}hst_requerimento    r,
					{0}hst_pessoa           p,
					{0}hst_pessoa_profissao pp,
					{0}tab_profissao        tp
				where r.interessado_id = p.pessoa_id
				and r.interessado_tid = p.tid
				and p.data_execucao = (select max(h.data_execucao) from {0}hst_pessoa h where h.pessoa_id = p.pessoa_id and h.tid = p.tid)
				and p.id = pp.id_hst(+)
				and pp.profissao_id = tp.id(+)
				and r.id = :hst", EsquemaBanco);

				comando.AdicionarParametroEntrada("hst", hst, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						aux = reader.GetValue<int>("id");

						requerimento.Interessado.Id = requerimento.Interessado.Id;
						requerimento.Interessado.Tipo = reader.GetValue<int>("tipo");

						if (requerimento.Interessado.IsFisica)
						{
							requerimento.Interessado.Fisica.CPF = reader.GetValue<string>("cpf");
							requerimento.Interessado.Fisica.Nome = reader.GetValue<string>("nome");
							requerimento.Interessado.Fisica.RG = reader.GetValue<string>("rg");
							requerimento.Interessado.Fisica.Apelido = reader.GetValue<string>("apelido");
							requerimento.Interessado.Fisica.Profissao = reader.GetValue<string>("profissao");
						}
						else // juridica
						{
							requerimento.Interessado.Juridica.CNPJ = reader.GetValue<string>("cnpj");
							requerimento.Interessado.Juridica.RazaoSocial = reader.GetValue<string>("razao_social");
							requerimento.Interessado.Juridica.IE = reader.GetValue<string>("ie");
							requerimento.Interessado.Juridica.NomeFantasia = reader.GetValue<string>("nome_fantasia");
						}
					}

					#region Meio de Contato

					comando = bancoDeDados.CriarComando(@"select pc.pes_meio_cont_id id, pc.meio_contato_id, c.texto contato_texto, pc.valor
					from {0}hst_pessoa_meio_contato pc, {0}tab_meio_contato c where pc.meio_contato_id = c.id and pc.id_hst = :hst", EsquemaBanco);

					comando.AdicionarParametroEntrada("hst", aux, DbType.Int32);

					using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
					{
						ContatoRelatorio contato;
						while (readerAux.Read())
						{
							contato = new ContatoRelatorio();
							contato.Id = readerAux.GetValue<int>("id");
							contato.PessoaId = requerimento.Interessado.Id;
							contato.TipoContato = (eTipoContato)Enum.Parse(contato.TipoContato.GetType(), readerAux.GetValue<string>("meio_contato_id"));
							contato.TipoTexto = readerAux.GetValue<string>("contato_texto");
							contato.Valor = readerAux.GetValue<string>("valor");
							requerimento.Interessado.MeiosContatos.Add(contato);
						}

						readerAux.Close();
					}

					#endregion

					#region Endereços

					comando = bancoDeDados.CriarComando(@"
					select e.endereco_id id,
						e.cep,
						e.logradouro,
						e.bairro,
						e.estado_id,
						le.sigla estado_sigla,
						e.municipio_id,
						e.municipio_texto,
						e.numero,
						e.complemento,
						e.distrito
					from {0}hst_pessoa_endereco e, {0}lov_estado le
					where e.estado_id = le.id
					and e.id_hst = :hst", EsquemaBanco);

					comando.AdicionarParametroEntrada("hst", aux, DbType.Int32);

					using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
					{
						if (readerAux.Read())
						{
							requerimento.Interessado.Endereco.Id = readerAux.GetValue<int>("id");
							requerimento.Interessado.Endereco.Cep = readerAux.GetValue<string>("cep");
							requerimento.Interessado.Endereco.Logradouro = readerAux.GetValue<string>("logradouro");
							requerimento.Interessado.Endereco.Bairro = readerAux.GetValue<string>("bairro");
							requerimento.Interessado.Endereco.EstadoId = readerAux.GetValue<int>("estado_id");
							requerimento.Interessado.Endereco.EstadoSigla = readerAux.GetValue<string>("estado_sigla");
							requerimento.Interessado.Endereco.MunicipioId = readerAux.GetValue<int>("municipio_id");
							requerimento.Interessado.Endereco.MunicipioTexto = readerAux.GetValue<string>("municipio_texto");
							requerimento.Interessado.Endereco.Numero = readerAux.GetValue<string>("numero");
							requerimento.Interessado.Endereco.Complemento = readerAux.GetValue<string>("complemento");
							requerimento.Interessado.Endereco.Distrito = readerAux.GetValue<string>("distrito");
						}

						readerAux.Close();
					}

					#endregion

					reader.Close();
				}

				#endregion

				#region Responsáveis

				comando = bancoDeDados.CriarComando(@"
				select p.id,
					r.responsavel_id,
					r.funcao_texto,
					r.numero_art,
					nvl(p.nome, p.razao_social) nome,
					nvl(p.cpf, p.cnpj) cpf_cnpj,
					nvl(p.rg, p.ie) rg_ie,
					p.tipo,
					trunc(p.data_nascimento) data_nascimento
				from {0}hst_requerimento_responsavel r, {0}hst_pessoa p
				where r.responsavel_id = p.pessoa_id
				and r.responsavel_tid = p.tid
				and p.data_execucao = (select max(h.data_execucao) from {0}hst_pessoa h where h.pessoa_id = p.pessoa_id and h.tid = p.tid)
				and r.id_hst = :hst", EsquemaBanco);

				comando.AdicionarParametroEntrada("hst", hst, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					ResponsavelTecnicoRelatorio responsavel;
					while (reader.Read())
					{
						aux = reader.GetValue<int>("id");

						responsavel = new ResponsavelTecnicoRelatorio();
						responsavel.Id = reader.GetValue<int>("responsavel_id");
						responsavel.FuncaoTexto = reader.GetValue<string>("funcao_texto");
						responsavel.CpfCnpj = reader.GetValue<string>("cpf_cnpj");
						responsavel.RgIe = reader.GetValue<string>("rg_ie");
						responsavel.NomeRazao = reader.GetValue<string>("nome");
						responsavel.NumeroArt = reader.GetValue<string>("numero_art");
						responsavel.DataVencimento = "Falta";

						if (reader["data_nascimento"] != null && !Convert.IsDBNull(reader["data_nascimento"]))
						{
							responsavel.DataNascimento = reader.GetValue<DateTime>("data_nascimento").ToShortDateString();
						}

						#region Meio de Contato

						comando = bancoDeDados.CriarComando(@"select pc.pes_meio_cont_id id, pc.meio_contato_id, c.texto contato_texto, pc.valor
						from {0}hst_pessoa_meio_contato pc, {0}tab_meio_contato c where pc.meio_contato_id = c.id and pc.id_hst = :hst", EsquemaBanco);

						comando.AdicionarParametroEntrada("hst", aux, DbType.Int32);

						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							ContatoRelatorio contato;
							while (readerAux.Read())
							{
								contato = new ContatoRelatorio();
								contato.Id = readerAux.GetValue<int>("id");
								contato.PessoaId = responsavel.Id;
								contato.TipoContato = (eTipoContato)Enum.Parse(contato.TipoContato.GetType(), readerAux.GetValue<string>("meio_contato_id"));
								contato.TipoTexto = readerAux.GetValue<string>("contato_texto");
								contato.Valor = readerAux.GetValue<string>("valor");
								responsavel.MeiosContatos.Add(contato);
							}

							readerAux.Close();
						}

						#endregion

						#region Endereços

						comando = bancoDeDados.CriarComando(@"
						select e.endereco_id id,
							e.cep,
							e.logradouro,
							e.bairro,
							e.estado_id,
							le.sigla estado_sigla,
							e.municipio_id,
							e.municipio_texto,
							e.numero,
							e.complemento,
							e.distrito
						from {0}hst_pessoa_endereco e, {0}lov_estado le
						where e.estado_id = le.id
						and e.id_hst = :hst", EsquemaBanco);

						comando.AdicionarParametroEntrada("hst", aux, DbType.Int32);

						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							if (readerAux.Read())
							{
								responsavel.Endereco.Id = readerAux.GetValue<int>("id");
								responsavel.Endereco.Cep = readerAux.GetValue<string>("cep");
								responsavel.Endereco.Logradouro = readerAux.GetValue<string>("logradouro");
								responsavel.Endereco.Bairro = readerAux.GetValue<string>("bairro");
								responsavel.Endereco.EstadoId = readerAux.GetValue<int>("estado_id");
								responsavel.Endereco.EstadoSigla = readerAux.GetValue<string>("estado_sigla");
								responsavel.Endereco.MunicipioId = readerAux.GetValue<int>("municipio_id");
								responsavel.Endereco.MunicipioTexto = readerAux.GetValue<string>("municipio_texto");
								responsavel.Endereco.Numero = readerAux.GetValue<string>("numero");
								responsavel.Endereco.Complemento = readerAux.GetValue<string>("complemento");
								responsavel.Endereco.Distrito = readerAux.GetValue<string>("distrito");
							}

							readerAux.Close();
						}

						#endregion

						requerimento.Responsaveis.Add(responsavel);
					}

					reader.Close();
				}

				#endregion

				#region Empreendimento

				comando = bancoDeDados.CriarComando(@"
				select e.id,
					e.cnpj,
					e.segmento_texto,
					e.denominador,
					e.nome_fantasia,
					a.atividade,
					e.codigo
				from {0}hst_requerimento            r,
					{0}hst_empreendimento           e,
					{0}hst_empreendimento_atividade a
				where r.empreendimento_id = e.empreendimento_id
				and r.empreendimento_tid = e.tid
				and e.atividade_id = a.atividade_id(+)
				and e.atividade_tid = a.tid(+)
				and r.id = :hst", EsquemaBanco);

				comando.AdicionarParametroEntrada("hst", hst, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						aux = reader.GetValue<int>("id");

						requerimento.Empreendimento.Id = requerimento.Empreendimento.Id;
						requerimento.Empreendimento.CNPJ = reader.GetValue<string>("cnpj");
						requerimento.Empreendimento.Codigo = reader.GetValue<int>("codigo");
						requerimento.Empreendimento.SegmentoTexto = reader.GetValue<string>("segmento_texto");
						requerimento.Empreendimento.NomeRazao = reader.GetValue<string>("denominador");
						requerimento.Empreendimento.NomeFantasia = reader.GetValue<string>("nome_fantasia");
						requerimento.Empreendimento.AtividadeTexto = reader.GetValue<string>("atividade");
					}

					#region Meio de Contato

					comando = bancoDeDados.CriarComando(@"select ec.emp_contato_id id, c.id tipo_contato_id, c.texto contato_texto, ec.valor
					from {0}hst_empreendimento_contato ec, {0}tab_meio_contato c where ec.meio_contato_id = c.id and ec.id_hst = :hst", EsquemaBanco);

					comando.AdicionarParametroEntrada("hst", aux, DbType.Int32);

					using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
					{
						ContatoRelatorio contato;
						while (readerAux.Read())
						{
							contato = new ContatoRelatorio();
							contato.Id = readerAux.GetValue<int>("id");
							contato.PessoaId = requerimento.Empreendimento.Id;
							contato.TipoContato = (eTipoContato)Enum.Parse(contato.TipoContato.GetType(), readerAux.GetValue<string>("tipo_contato_id"));
							contato.TipoTexto = readerAux.GetValue<string>("contato_texto");
							contato.Valor = readerAux.GetValue<string>("valor");
							requerimento.Empreendimento.MeiosContatos.Add(contato);
						}

						readerAux.Close();
					}

					#endregion

					#region Endereços

					comando = bancoDeDados.CriarComando(@"
					select e.endereco_id id,
						e.correspondencia,
						e.cep,
						e.logradouro,
						e.bairro,
						e.estado_id,
						le.sigla estado_sigla,
						e.municipio_id,
						e.municipio_texto,
						e.numero,
						e.complemento,
						e.corrego,
						(case when e.zona = 1 then 'Urbana' else 'Rural' end) zona,
						e.distrito,
						e.complemento
					from {0}hst_empreendimento_endereco e,
						{0}lov_estado                   le
					where e.estado_id = le.id(+)
					and e.id_hst = :hst", EsquemaBanco);

					comando.AdicionarParametroEntrada("hst", aux, DbType.Int32);

					using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
					{
						EnderecoRelatorio end;
						while (readerAux.Read())
						{
							end = new EnderecoRelatorio();
							end.Id = readerAux.GetValue<int>("id");
							end.Correspondencia = readerAux.GetValue<int>("correspondencia");
							end.Cep = readerAux.GetValue<string>("cep");
							end.Logradouro = readerAux.GetValue<string>("logradouro");
							end.Bairro = readerAux.GetValue<string>("bairro");
							end.EstadoId = readerAux.GetValue<int>("estado_id");
							end.EstadoSigla = readerAux.GetValue<string>("estado_sigla");
							end.MunicipioId = readerAux.GetValue<int>("municipio_id");
							end.MunicipioTexto = readerAux.GetValue<string>("municipio_texto");
							end.Numero = readerAux.GetValue<string>("numero");
							end.Complemento = readerAux.GetValue<string>("complemento");
							end.Corrego = readerAux.GetValue<string>("corrego");
							end.Zona = readerAux.GetValue<string>("zona");
							end.Distrito = readerAux.GetValue<string>("distrito");
							end.Complemento = readerAux.GetValue<string>("complemento");
							requerimento.Empreendimento.Enderecos.Add(end);
						}

						readerAux.Close();
					}

					#endregion

					#region Coordenada

					comando = bancoDeDados.CriarComando(@"
					select ec.coordenada_id,
						ec.tipo_coordenada_id,
						ec.tipo_coordenada_texto,
						ec.datum_id,
						ec.datum_texto,
						ec.easting_utm,
						ec.northing_utm,
						ec.fuso_utm,
						ec.hemisferio_utm_id,
						ec.hemisferio_utm_texto,
						ec.latitude_gms,
						ec.longitude_gms,
						ec.latitude_gdec,
						ec.longitude_gdec,
						ec.forma_coleta_texto,
						ec.local_coleta_texto
					from {0}hst_empreendimento_coord ec
					where ec.id_hst = :hst", EsquemaBanco);

					comando.AdicionarParametroEntrada("hst", aux, DbType.Int32);

					using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
					{
						if (readerAux.Read())
						{
							requerimento.Empreendimento.Coordenada.Id = readerAux.GetValue<int>("coordenada_id");
							requerimento.Empreendimento.Coordenada.Tipo.Id = readerAux.GetValue<int>("tipo_coordenada_id");
							requerimento.Empreendimento.Coordenada.Tipo.Texto = readerAux.GetValue<string>("tipo_coordenada_texto");
							requerimento.Empreendimento.Coordenada.Datum.Id = readerAux.GetValue<int>("datum_id");
							requerimento.Empreendimento.Coordenada.DatumTexto = readerAux.GetValue<string>("datum_texto");
							requerimento.Empreendimento.Coordenada.EastingUtm = readerAux.GetValue<double>("easting_utm");
							requerimento.Empreendimento.Coordenada.NorthingUtm = readerAux.GetValue<double>("northing_utm");
							requerimento.Empreendimento.Coordenada.FusoUtm = readerAux.GetValue<int>("fuso_utm");
							requerimento.Empreendimento.Coordenada.HemisferioUtm = readerAux.GetValue<int>("hemisferio_utm_id");
							requerimento.Empreendimento.Coordenada.HemisferioUtmTexto = readerAux.GetValue<string>("hemisferio_utm_texto");
							requerimento.Empreendimento.Coordenada.LatitudeGms = readerAux.GetValue<string>("latitude_gms");
							requerimento.Empreendimento.Coordenada.LongitudeGms = readerAux.GetValue<string>("longitude_gms");
							requerimento.Empreendimento.Coordenada.LatitudeGdec = readerAux.GetValue<double>("latitude_gdec");
							requerimento.Empreendimento.Coordenada.LongitudeGdec = readerAux.GetValue<double>("longitude_gdec");
							requerimento.Empreendimento.Coordenada.FormaColetaTexto = readerAux.GetValue<string>("forma_coleta_texto");
							requerimento.Empreendimento.Coordenada.LocalColetaTexto = readerAux.GetValue<string>("local_coleta_texto");
						}

						readerAux.Close();
					}

					#endregion

					reader.Close();
				}

				#endregion
			}

			#region Pessoa Credenciado

			if (requerimento.AutorId > 0)
			{
				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaBancoCredenciado))
				{

					Comando comando = bancoDeDados.CriarComando(@"select p.id,tc.credenciado_id,  p.tipo, tc.tipo_id credenciado_tipo, tc.tipo_texto credenciado_tipo_texto,
					p.cpf, p.nome, p.rg, p.apelido, p.cnpj, p.razao_social, p.ie, p.nome_fantasia, o.orgao_nome, o.orgao_sigla from {0}hst_pessoa p, 
					{0}hst_credenciado tc, {1}hst_orgao_parc_conv o where p.pessoa_id = tc.pessoa_id and p.tid = tc.pessoa_tid 
					and o.orgao_parc_conv_id(+) = tc.orgao_parc_id and o.tid(+) = tc.orgao_parc_tid and tc.credenciado_id = :credenciado_id 
					and tc.tid = :credenciado_tid", EsquemaBancoCredenciado, EsquemaBancoInterno);

					comando.AdicionarParametroEntrada("credenciado_id", requerimento.AutorId, DbType.Int32);
					comando.AdicionarParametroEntrada("credenciado_tid", requerimento.AutorTid, DbType.String);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						if (reader.Read())
						{
							aux = reader.GetValue<int>("id");

							requerimento.UsuarioCredenciado.Id = reader.GetValue<int>("credenciado_id");
							requerimento.UsuarioCredenciado.PessoaTipo = reader.GetValue<int>("tipo");

							requerimento.UsuarioCredenciado.TipoId = reader.GetValue<int>("credenciado_tipo");
							requerimento.UsuarioCredenciado.TipoTexto = reader.GetValue<String>("credenciado_tipo_texto");
							requerimento.UsuarioCredenciado.OrgaoParceiroNome = reader.GetValue<String>("orgao_nome");
							requerimento.UsuarioCredenciado.OrgaoParceiroSigla = reader.GetValue<String>("orgao_sigla");

							if (requerimento.UsuarioCredenciado.PessoaTipo == PessoaTipo.FISICA)
							{
								requerimento.UsuarioCredenciado.Fisica.CPF = reader.GetValue<string>("cpf");
								requerimento.UsuarioCredenciado.Fisica.Nome = reader.GetValue<string>("nome");
								requerimento.UsuarioCredenciado.Fisica.RG = reader.GetValue<string>("rg");
								requerimento.UsuarioCredenciado.Fisica.Apelido = reader.GetValue<string>("apelido");
							}
							else // juridica
							{
								requerimento.UsuarioCredenciado.Juridica.CNPJ = reader.GetValue<string>("cnpj");
								requerimento.UsuarioCredenciado.Juridica.RazaoSocial = reader.GetValue<string>("razao_social");
								requerimento.UsuarioCredenciado.Juridica.IE = reader.GetValue<string>("ie");
								requerimento.UsuarioCredenciado.Juridica.NomeFantasia = reader.GetValue<string>("nome_fantasia");
							}
						}

						reader.Close();

					}
				}
			}

			#endregion


			return requerimento;
		}
	}
}