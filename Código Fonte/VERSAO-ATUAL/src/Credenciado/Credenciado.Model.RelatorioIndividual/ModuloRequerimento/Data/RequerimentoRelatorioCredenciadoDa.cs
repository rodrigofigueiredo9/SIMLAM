using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.Entities;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloRequerimento;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Credenciado.Model.RelatorioIndividual.ModuloRequerimento.Data
{
	public class RequerimentoRelatorioCredenciadoDa
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		public String EsquemaBanco
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		public String EsquemaInterno
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioInterno); }
		}

		public Int32 RoteiroPadrao
		{
			get { return _configSys.Obter<Int32>(ConfiguracaoSistema.KeyRoteiroPadrao); }
		}

		#endregion

		public RequerimentoRelatorio Obter(int id)
		{
			RequerimentoRelatorio requerimento = new RequerimentoRelatorio();

			#region Banco do Credenciado

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaBanco))
			{
				#region Requerimento

				Comando comando = bancoDeDados.CriarComando(@"
												select r.id,
												r.credenciado,
												trunc(r.data_criacao) data_criacao,
												r.interessado,
												r.empreendimento,
												r.situacao,
												r.agendamento agendamento,
												to_char(r.data_criacao, 'dd') dia,
												to_char(r.data_criacao, 'MM') mes,
												to_char(r.data_criacao, 'yyyy') ano,
												r.setor,
												r.informacoes,
												p.situacao projeto_digital_situacao,
												(select pe.municipio from {0}tab_pessoa_endereco pe, {0}tab_credenciado c where pe.pessoa = c.pessoa and c.id = r.credenciado) municipio
											from {0}tab_requerimento r, tab_projeto_digital p 
											where r.id = p.requerimento and r.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						requerimento.Id = id;
						requerimento.AutorId = reader.GetValue<int>("credenciado");
						requerimento.DataCadastro = Convert.ToDateTime(reader["data_criacao"]);
						requerimento.DiaCadastro = reader["dia"].ToString();
						int mes = reader.GetValue<int>("mes");
						requerimento.MesCadastro = _configSys.Obter<List<String>>(ConfiguracaoSistema.KeyMeses).ElementAt(mes - 1);
						requerimento.AnoCadastro = reader["ano"].ToString();
						requerimento.MunicipioId =reader.GetValue<int>("municipio");
						requerimento.Interessado.Id = reader.GetValue<int>("interessado");
						requerimento.Empreendimento.Id = reader.GetValue<int>("empreendimento");
						requerimento.ProjetoDigitalSituacaoId = reader.GetValue<int>("projeto_digital_situacao");
						requerimento.SituacaoId = reader.GetValue<int>("situacao");
						requerimento.AgendamentoVistoria = reader["agendamento"].ToString();
						requerimento.SetorId = reader.GetValue<int>("setor");
						requerimento.Informacoes = reader["informacoes"].ToString();
						
					}

					reader.Close();
				}

				#endregion

				#region Atividades

				comando = bancoDeDados.CriarComando(@"
									select a.id, a.atividade, a.tid
									  from {0}tab_requerimento_atividade a
									 where a.requerimento = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					RequerimentoAtividadeRelatorio atividade;

					while (reader.Read())
					{
						atividade = new RequerimentoAtividadeRelatorio();
						atividade.Id = Convert.ToInt32(reader["atividade"]);
						atividade.IdRelacionamento = Convert.ToInt32(reader["id"]);

						#region Atividades/Finalidades/Modelos
						comando = bancoDeDados.CriarComando(@"
												select a.id,
													   a.finalidade,
													   a.modelo,
													   a.titulo_anterior_tipo,
													   a.titulo_anterior_id,
													   a.titulo_anterior_numero,
													   a.modelo_anterior_id,
													   a.modelo_anterior_nome,
													   a.modelo_anterior_sigla,
													   a.orgao_expedidor
												  from {0}tab_requerimento_ativ_finalida a       
												 where a.requerimento_ativ = :id", EsquemaBanco);

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

				#region Interessado

				comando = bancoDeDados.CriarComando(@"
											select p.id,
												   p.tipo,
												   p.nome,
												   p.cpf,
												   p.rg,
												   p.estado_civil,
												   p.cnpj,
												   p.razao_social,
												   p.nome_fantasia,
												   p.ie,
												   p.apelido,
												   tpp.profissao
											  from {0}tab_pessoa p, {0}tab_pessoa_profissao tpp
											 where p.id = tpp.pessoa(+)
											   and p.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", requerimento.Interessado.Id, DbType.Int32);
				PessoaRelatorio pessoa = new PessoaRelatorio();

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						pessoa.Id = requerimento.Interessado.Id;
						pessoa.Tipo = Convert.ToInt32(reader["tipo"]);

						if (pessoa.IsFisica)
						{
							pessoa.Fisica.Nome = reader["nome"].ToString();
							pessoa.Fisica.CPF = reader["cpf"].ToString();
							pessoa.Fisica.RG = reader["rg"].ToString();
							pessoa.Fisica.Apelido = reader["apelido"].ToString();
						}
						else // juridica
						{
							pessoa.Juridica.CNPJ = reader["cnpj"].ToString();
							pessoa.Juridica.RazaoSocial = reader["razao_social"].ToString();
							pessoa.Juridica.NomeFantasia = reader["nome_fantasia"].ToString();
							pessoa.Juridica.IE = reader["ie"].ToString();
						}
					}
					reader.Close();
				}

				#region Meio de Contato

				comando = bancoDeDados.CriarComando(@"select a.id, a.pessoa, a.meio_contato tipo_contato_id, a.valor
					from {0}tab_pessoa_meio_contato a where a.pessoa = :pessoa", EsquemaBanco);

				comando.AdicionarParametroEntrada("pessoa", pessoa.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					ContatoRelatorio contato;
					while (reader.Read())
					{
						contato = new ContatoRelatorio();
						contato.Id = Convert.ToInt32(reader["id"]);
						contato.PessoaId = Convert.ToInt32(reader["pessoa"]);
						contato.TipoContato = (eTipoContato)Enum.Parse(contato.TipoContato.GetType(), reader["tipo_contato_id"].ToString());
						contato.Valor = reader["valor"].ToString();
						pessoa.MeiosContatos.Add(contato);
					}
					reader.Close();
				}

				#endregion

				#region Endereços

				comando = bancoDeDados.CriarComando(@"
													select te.id,
														   te.pessoa,
														   te.cep,
														   te.logradouro,
														   te.bairro,
														   te.estado      estado_id,
														   te.municipio   municipio_id,
														   te.numero,
														   te.complemento,
														   te.distrito
													  from {0}tab_pessoa_endereco te
													 where te.pessoa = :pessoa", EsquemaBanco);

				comando.AdicionarParametroEntrada("pessoa", pessoa.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						pessoa.Endereco.Id = Convert.ToInt32(reader["id"]);
						pessoa.Endereco.Cep = reader["cep"].ToString();
						pessoa.Endereco.Logradouro = reader["logradouro"].ToString();
						pessoa.Endereco.Bairro = reader["bairro"].ToString();
						pessoa.Endereco.EstadoId = Convert.IsDBNull(reader["estado_id"]) ? 0 : Convert.ToInt32(reader["estado_id"]);
						pessoa.Endereco.MunicipioId = Convert.IsDBNull(reader["municipio_id"]) ? 0 : Convert.ToInt32(reader["municipio_id"]);
						pessoa.Endereco.Numero = reader["numero"].ToString();
						pessoa.Endereco.Complemento = reader["complemento"].ToString();
						pessoa.Endereco.Distrito = reader["distrito"].ToString();
					}
					reader.Close();
				}

				#endregion

				requerimento.Interessado = pessoa;

				#endregion

				#region Responsáveis

				comando = bancoDeDados.CriarComando(@"
											select pr.id,
												   pr.responsavel,
												   pr.funcao funcao,
												   pr.numero_art,
												   nvl(p.nome, p.razao_social) nome,
												   nvl(p.cpf, p.cnpj) cpf_cnpj,
												   nvl(p.rg, p.ie) rg_ie,
												   p.tipo,
												   trunc(p.data_nascimento) data_nascimento
											  from {0}tab_requerimento_responsavel pr, {0}tab_pessoa p
											 where pr.responsavel = p.id
											   and pr.requerimento = :requerimento", EsquemaBanco);

				comando.AdicionarParametroEntrada("requerimento", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					ResponsavelTecnicoRelatorio responsavel;
					while (reader.Read())
					{
						responsavel = new ResponsavelTecnicoRelatorio();
						responsavel.Id = Convert.ToInt32(reader["responsavel"]);
						responsavel.FuncaoTexto = reader["funcao"].ToString();
						responsavel.CpfCnpj = reader["cpf_cnpj"].ToString();
						responsavel.RgIe = reader["rg_ie"].ToString();
						responsavel.NomeRazao = reader["nome"].ToString();
						responsavel.NumeroArt = reader["numero_art"].ToString();

						if (reader["data_nascimento"] != null && !Convert.IsDBNull(reader["data_nascimento"]))
						{
							responsavel.DataNascimento = Convert.ToDateTime(reader["data_nascimento"]).ToShortDateString();
						}

						responsavel.DataVencimento = "Falta";

						#region Meio de Contato

						comando = bancoDeDados.CriarComando(@"select a.id, a.pessoa, a.meio_contato tipo_contato_id, a.valor
							from tab_pessoa_meio_contato a
							where a.pessoa = :pessoa", EsquemaBanco);

						comando.AdicionarParametroEntrada("pessoa", responsavel.Id, DbType.Int32);

						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							ContatoRelatorio contato;
							while (readerAux.Read())
							{
								contato = new ContatoRelatorio();
								contato.Id = Convert.ToInt32(readerAux["id"]);
								contato.PessoaId = Convert.ToInt32(readerAux["pessoa"]);
								contato.TipoContato = (eTipoContato)Enum.Parse(contato.TipoContato.GetType(), readerAux["tipo_contato_id"].ToString());
								contato.Valor = readerAux["valor"].ToString();
								responsavel.MeiosContatos.Add(contato);
							}
							readerAux.Close();
						}

						#endregion

						#region Endereços

						comando = bancoDeDados.CriarComando(@"
														select te.id,
															   te.pessoa,
															   te.cep,
															   te.logradouro,
															   te.bairro,
															   te.estado      estado_id,
															   te.municipio   municipio_id,
															   te.numero,
															   te.complemento,
															   te.distrito
														  from {0}tab_pessoa_endereco te
														 where te.pessoa = :pessoa", EsquemaBanco);

						comando.AdicionarParametroEntrada("pessoa", responsavel.Id, DbType.Int32);

						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							while (readerAux.Read())
							{
								responsavel.Endereco.Id = Convert.ToInt32(readerAux["id"]);
								responsavel.Endereco.Cep = readerAux["cep"].ToString();
								responsavel.Endereco.Logradouro = readerAux["logradouro"].ToString();
								responsavel.Endereco.Bairro = readerAux["bairro"].ToString();
								responsavel.Endereco.EstadoId = Convert.IsDBNull(readerAux["estado_id"]) ? 0 : Convert.ToInt32(readerAux["estado_id"]);
								responsavel.Endereco.MunicipioId = Convert.IsDBNull(readerAux["municipio_id"]) ? 0 : Convert.ToInt32(readerAux["municipio_id"]);
								responsavel.Endereco.Numero = readerAux["numero"].ToString();
								responsavel.Endereco.Complemento = readerAux["complemento"].ToString();
								responsavel.Endereco.Distrito = readerAux["distrito"].ToString();
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

				if (requerimento.Empreendimento.Id > 0)
				{
					comando = bancoDeDados.CriarComando(@"
					select e.id,
							e.codigo,
							e.segmento,
							e.denominador,
							e.cnpj,
							e.atividade,
							e.nome_fantasia,
							e.denominador razao_social
					from tab_empreendimento e
					where e.id = :id", EsquemaBanco);

					comando.AdicionarParametroEntrada("id", requerimento.Empreendimento.Id, DbType.Int32);
					EmpreendimentoRelatorio empreendimento = new EmpreendimentoRelatorio();

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						if (reader.Read())
						{
							empreendimento.Id = requerimento.Empreendimento.Id;
							empreendimento.Codigo = reader.GetValue<int>("codigo");
							empreendimento.CNPJ = reader["cnpj"].ToString();
							empreendimento.NomeFantasia = reader["nome_fantasia"].ToString();
							empreendimento.Denominador = reader["denominador"].ToString();
							empreendimento.SegmentoTexto = reader["segmento"].ToString();
							empreendimento.AtividadeTexto = Convert.ToString(reader["atividade"]);
							empreendimento.NomeRazao = Convert.ToString(reader["razao_social"]);
						}

						reader.Close();
					}

					#region Endereços

					comando = bancoDeDados.CriarComando(@"
											select te.id,
													te.empreendimento,
													te.correspondencia,
													te.cep,
													te.logradouro,
													te.bairro,
													te.estado estado_id,
													te.municipio municipio_id,
													te.numero,
													te.complemento,
													(case
														when te.zona = 1 then
														'Urbana'
														else
														'Rural'
													end) zona,
													te.distrito,
													te.corrego,
													te.complemento
												from tab_empreendimento_endereco te
												where te.empreendimento = :empreendimento", EsquemaBanco);

					comando.AdicionarParametroEntrada("empreendimento", empreendimento.Id, DbType.Int32);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						EnderecoRelatorio end;
						while (reader.Read())
						{
							end = new EnderecoRelatorio();
							end.Id = Convert.ToInt32(reader["id"]);
							end.Correspondencia = Convert.IsDBNull(reader["correspondencia"]) ? 0 : Convert.ToInt32(reader["correspondencia"]);
							end.Cep = reader["cep"].ToString();
							end.Logradouro = reader["logradouro"].ToString();
							end.Bairro = reader["bairro"].ToString();
							end.EstadoId = Convert.IsDBNull(reader["estado_id"]) ? 0 : Convert.ToInt32(reader["estado_id"]);
							end.MunicipioId = Convert.IsDBNull(reader["municipio_id"]) ? 0 : Convert.ToInt32(reader["municipio_id"]);
							end.Numero = reader["numero"].ToString();
							end.Complemento = reader["complemento"].ToString();
							end.Corrego = reader["corrego"].ToString();
							end.Zona = reader["zona"].ToString();
							end.Distrito = reader["distrito"].ToString();
							end.Complemento = reader["complemento"].ToString();
							empreendimento.Enderecos.Add(end);
						}
						reader.Close();
					}

					#endregion

					#region Coordenada

					comando = bancoDeDados.CriarComando(@"
													select aec.id,
														   aec.tipo_coordenada,
														   aec.datum,
														   aec.easting_utm,
														   aec.northing_utm,
														   aec.fuso_utm,
														   aec.hemisferio_utm,
														   aec.latitude_gms,
														   aec.longitude_gms,
														   aec.latitude_gdec,
														   aec.longitude_gdec,
														   aec.forma_coleta    forma_coleta,
														   aec.local_coleta    local_coleta,
														   aec.datum           datum_texto,
														   aec.hemisferio_utm  hemisferio_utm_texto
													  from {0}tab_empreendimento_coord aec
													 where aec.empreendimento = :empreendimentoid", EsquemaBanco);

					comando.AdicionarParametroEntrada("empreendimentoid", empreendimento.Id, DbType.Int32);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						if (reader.Read())
						{
							empreendimento.Coordenada.Id = Convert.ToInt32(reader["id"]);
							empreendimento.Coordenada.EastingUtmTexto = Convert.ToString(reader["easting_utm"]);
							empreendimento.Coordenada.NorthingUtmTexto = Convert.ToString(reader["northing_utm"]);
							empreendimento.Coordenada.FusoUtm = Convert.IsDBNull(reader["fuso_utm"]) ? 0 : Convert.ToInt32(reader["fuso_utm"]);
							empreendimento.Coordenada.HemisferioUtm = Convert.IsDBNull(reader["hemisferio_utm"]) ? 0 : Convert.ToInt32(reader["hemisferio_utm"]);
							empreendimento.Coordenada.LatitudeGms = reader["latitude_gms"].ToString();
							empreendimento.Coordenada.LongitudeGms = reader["longitude_gms"].ToString();
							empreendimento.Coordenada.LatitudeGdec = Convert.IsDBNull(reader["latitude_gdec"]) ? 0 : Convert.ToDouble(reader["latitude_gdec"]);
							empreendimento.Coordenada.LongitudeGdec = Convert.IsDBNull(reader["longitude_gdec"]) ? 0 : Convert.ToDouble(reader["longitude_gdec"]);
							empreendimento.Coordenada.Datum.Id = Convert.ToInt32(reader["datum"]);
							empreendimento.Coordenada.Tipo.Id = Convert.ToInt32(reader["tipo_coordenada"]);
							empreendimento.Coordenada.FormaColetaTexto = Convert.ToString(reader["forma_coleta"]);
							empreendimento.Coordenada.LocalColetaTexto = Convert.ToString(reader["local_coleta"]);
						}
						reader.Close();
					}

					#endregion

					#region Meio de Contato

					comando = bancoDeDados.CriarComando(@"select a.id, a.empreendimento, a.meio_contato tipo_contato_id, a.valor
				  from {0}tab_empreendimento_contato a
				 where a.empreendimento = :empreendimento", EsquemaBanco);

					comando.AdicionarParametroEntrada("empreendimento", empreendimento.Id, DbType.Int32);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						ContatoRelatorio contato;
						while (reader.Read())
						{
							contato = new ContatoRelatorio();
							contato.Id = Convert.ToInt32(reader["id"]);
							contato.PessoaId = Convert.ToInt32(reader["empreendimento"]);
							contato.TipoContato = (eTipoContato)Enum.Parse(contato.TipoContato.GetType(), reader["tipo_contato_id"].ToString());
							contato.Valor = reader["valor"].ToString();
							empreendimento.MeiosContatos.Add(contato);
						}
						reader.Close();
					}

					#endregion

					requerimento.Empreendimento = empreendimento;
				}
				
				#endregion

				#region Elaborador

				comando = bancoDeDados.CriarComando(@"
				select nvl(p.nome, p.razao_social) NomeRazaoSocial, lc.texto TipoTexto 
				from {0}tab_credenciado c, {0}tab_pessoa p, {0}lov_credenciado_tipo lc 
				where c.pessoa = p.id and c.tipo = lc.id and c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", requerimento.AutorId, DbType.Int32);
				requerimento.UsuarioCredenciado = bancoDeDados.ObterEntity<CredenciadoRelatorio>(comando);

				#endregion
			}

			#endregion

			#region Banco do Interno

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Requerimento

				requerimento.Municipio = ObterMunicipio(requerimento.MunicipioId, bancoDeDados);

				#region Situacao

				Comando comando = bancoDeDados.CriarComando(@"select l.texto situacao_texto from {0}lov_requerimento_situacao l where l.id = :id", EsquemaInterno);

				comando.AdicionarParametroEntrada("id", requerimento.SituacaoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						requerimento.SituacaoTexto = reader["situacao_texto"].ToString();
					}
					reader.Close();
				}

				#endregion

				#region Agendamento

				if (!string.IsNullOrWhiteSpace(requerimento.AgendamentoVistoria))
				{

					comando = bancoDeDados.CriarComando(@"select l.texto agendamento from {0}lov_requerimento_agendamento l where l.id = :id", EsquemaInterno);

					comando.AdicionarParametroEntrada("id", Convert.ToInt32(requerimento.AgendamentoVistoria), DbType.Int32);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						if (reader.Read())
						{
							requerimento.AgendamentoVistoria = reader["agendamento"].ToString();
						}
						reader.Close();
					}
				}

				#endregion

				#region Setor

				if (requerimento.SetorId > 0)
				{

					comando = bancoDeDados.CriarComando(@"select m.texto from tab_setor_endereco se, lov_municipio m 
						where se.municipio = m.id (+) and se.setor = :setor", EsquemaInterno);

					comando.AdicionarParametroEntrada("setor", Convert.ToInt32(requerimento.SetorId), DbType.Int32);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						if (reader.Read())
						{
							requerimento.Municipio = reader["texto"].ToString();
						}
						reader.Close();
					}
				}

				#endregion

				#endregion

				if (requerimento.Atividades.Count > 0)
				{
					#region Atividades

					comando = bancoDeDados.CriarComando(@"select b.id, b.atividade atividade_texto, b.conclusao from {0}tab_atividade b", EsquemaInterno);

					comando.DbCommand.CommandText += comando.AdicionarIn("where", "b.id", DbType.Int32, requerimento.Atividades.Select(x => x.Id).ToList());

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						while (reader.Read())
						{
							int idr = Convert.ToInt32(reader["id"]);
							RequerimentoAtividadeRelatorio atividade = requerimento.Atividades.FirstOrDefault(x => x.Id == idr);
							atividade.NomeAtividade = reader["atividade_texto"].ToString();
							atividade.Conclusao = reader.GetValue<string>("conclusao");
						}

						reader.Close();
					}

					#endregion

					#region Atividades/Finalidades/Modelos

					comando = bancoDeDados.CriarComando(@"
							select ltf.texto finalidade_texto, tm.nome modelo_nome
							  from {0}tab_titulo_modelo tm, {0}lov_titulo_finalidade ltf
							 where tm.id = :modelo and ltf.id = :fin", EsquemaInterno);

					comando.AdicionarParametroEntrada("fin", DbType.Int32);
					comando.AdicionarParametroEntrada("modelo", DbType.Int32);

					var finalidades = requerimento.Atividades.SelectMany(x => x.Finalidades);

					foreach (Finalidade f in finalidades)
					{
						comando.SetarValorParametro("fin", f.Id);
						comando.SetarValorParametro("modelo", f.TituloModelo);

						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							if (readerAux.Read())
							{
								f.Texto = readerAux["finalidade_texto"].ToString();
								f.TituloModeloTexto = readerAux["modelo_nome"].ToString();
							}

							readerAux.Close();
						}

					}

					#endregion
				}

				#region Interessado

				#region Profissao

				if (!string.IsNullOrWhiteSpace(requerimento.Interessado.Fisica.Profissao))
				{
					comando = bancoDeDados.CriarComando(@"select texto from {0}tab_profissao where id = :id", EsquemaInterno);

					comando.AdicionarParametroEntrada("id", Convert.ToInt32(requerimento.Interessado.Fisica.Profissao), DbType.Int32);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						if (reader.Read())
						{
							requerimento.Interessado.Fisica.Profissao = reader["texto"].ToString();
						}
						reader.Close();
					}
				}

				#endregion

				#region Meios de Contato

				comando = bancoDeDados.CriarComando(@"select b.id, b.texto, b.mascara from {0}tab_meio_contato b", EsquemaInterno);

				comando.DbCommand.CommandText += comando.AdicionarIn("where","b.id", DbType.Int32, requerimento.Interessado.MeiosContatos.Select(x => x.Id).ToList());

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						int idr = Convert.ToInt32(reader["id"]);
						var contatos = requerimento.Interessado.MeiosContatos.Where(x => x.Id == idr).ToList();
						contatos.ForEach(x =>
						{
							x.TipoTexto = reader["texto"].ToString();
							x.Mascara = reader.GetValue<string>("mascara");
						});
					}

					reader.Close();
				}

				#endregion

				#region Endereco

				EnderecoRelatorio end = requerimento.Interessado.Endereco;
				end.MunicipioTexto = ObterMunicipio(end.MunicipioId, bancoDeDados);
				end.EstadoSigla = ObterEstado(end.EstadoId, bancoDeDados);

				#endregion

				#endregion

				#region Responsaveis

				#region Funcao

				comando = bancoDeDados.CriarComando(@"select lf.texto funcao from {0}lov_protocolo_resp_funcao lf where lf.id = :id", EsquemaInterno);
				comando.AdicionarParametroEntrada("id", DbType.Int32);

				foreach (var resp in requerimento.Responsaveis)
				{
					comando.SetarValorParametro("id", int.Parse(resp.FuncaoTexto));
					resp.FuncaoTexto = bancoDeDados.ExecutarScalar<string>(comando);
				}

				#endregion

				#region Meios de Contato

				comando = bancoDeDados.CriarComando(@"select b.id, b.texto, b.mascara from {0}tab_meio_contato b", EsquemaInterno);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						int idr = Convert.ToInt32(reader["id"]);
						var contatos = requerimento.Responsaveis.SelectMany(x=>x.MeiosContatos).Where(x => x.Id == idr).ToList();
						contatos.ForEach(x =>
						{
							x.TipoTexto = reader["texto"].ToString();
							x.Mascara = reader.GetValue<string>("mascara");
						});
					}

					reader.Close();
				}

				#endregion

				#region Endereços

				foreach (var endResp in requerimento.Responsaveis.Select(x => x.Endereco))
				{
					endResp.MunicipioTexto = ObterMunicipio(endResp.MunicipioId, bancoDeDados);
					endResp.EstadoSigla = ObterEstado(endResp.EstadoId, bancoDeDados);
				}

				#endregion

				#endregion

				#region Empreendimento
				
				EmpreendimentoRelatorio emp = requerimento.Empreendimento;

				#region Atividade

				if (!string.IsNullOrWhiteSpace(emp.AtividadeTexto))
				{
					comando = bancoDeDados.CriarComando(@"select a.atividade from {0}tab_empreendimento_atividade a where a.id = :id", EsquemaInterno);
					comando.AdicionarParametroEntrada("id", Convert.ToInt32(emp.AtividadeTexto), DbType.Int32);
					requerimento.Empreendimento.AtividadeTexto = bancoDeDados.ExecutarScalar<string>(comando);
				}

				#endregion

				#region Segmento

				if (!string.IsNullOrWhiteSpace(emp.SegmentoTexto))
				{
					comando = bancoDeDados.CriarComando(@"select les.texto from {0}lov_empreendimento_segmento les where les.id = :id", EsquemaInterno);
					comando.AdicionarParametroEntrada("id", Convert.ToInt32(emp.SegmentoTexto), DbType.Int32);
					requerimento.Empreendimento.SegmentoTexto = bancoDeDados.ExecutarScalar<string>(comando);
				}

				#endregion

				#region Endereços

				foreach (var endEmp in emp.Enderecos)
				{
					endEmp.MunicipioTexto = ObterMunicipio(endEmp.MunicipioId, bancoDeDados);
					endEmp.EstadoSigla = ObterEstado(endEmp.EstadoId, bancoDeDados);
				}

				#endregion

				#region Meios de Contato

				comando = bancoDeDados.CriarComando(@"select b.id, b.texto, b.mascara from tab_meio_contato b", EsquemaInterno);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						int idr = Convert.ToInt32(reader["id"]);
						var contatos = emp.MeiosContatos.Where(x => x.Id == idr).ToList();
						contatos.ForEach(x =>
						{
							x.TipoTexto = reader["texto"].ToString();
							x.Mascara = reader.GetValue<string>("mascara");
						});
					}

					reader.Close();
				}

				#endregion

				#region Coordenada

				ListaBus bus = new ListaBus();

				emp.Coordenada.DatumTexto = ObterDatum(emp.Coordenada.Datum.Id, bancoDeDados);
				emp.Coordenada.Tipo.Texto = ObterTipoCoordenada(emp.Coordenada.Tipo.Id, bancoDeDados);
				
				if (!string.IsNullOrWhiteSpace(emp.Coordenada.HemisferioUtmTexto))
				{
					emp.Coordenada.HemisferioUtmTexto = bus.Hemisferios.FirstOrDefault(x => x.Id == Convert.ToInt32(emp.Coordenada.HemisferioUtmTexto)).Texto;
				}

				if (!string.IsNullOrWhiteSpace(emp.Coordenada.FormaColetaTexto))
				{
					comando = bancoDeDados.CriarComando(@"select c.texto from {0}lov_empreendimento_forma_colet c where c.id = :id", EsquemaInterno);
					comando.AdicionarParametroEntrada("id", Convert.ToInt32(emp.Coordenada.FormaColetaTexto), DbType.Int32);
					emp.Coordenada.FormaColetaTexto = bancoDeDados.ExecutarScalar<string>(comando);
				}

				if (!string.IsNullOrWhiteSpace(emp.Coordenada.LocalColetaTexto))
				{
					comando = bancoDeDados.CriarComando(@"select c.texto from {0}lov_empreendimento_local_colet c where c.id = :id", EsquemaInterno);
					comando.AdicionarParametroEntrada("id", Convert.ToInt32(emp.Coordenada.LocalColetaTexto), DbType.Int32);
					emp.Coordenada.LocalColetaTexto = bancoDeDados.ExecutarScalar<string>(comando);
				}

				#endregion

				#endregion
			}

			#endregion

			return requerimento;
		}

		public string ObterMunicipio(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(
				"select lm.texto from {0}lov_municipio lm where lm.id = :id", EsquemaInterno);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return bancoDeDados.ExecutarScalar<string>(comando);
			}
		}

		public string ObterEstado(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(
				"select lm.sigla from {0}lov_estado lm where lm.id = :id", EsquemaInterno);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return bancoDeDados.ExecutarScalar<string>(comando);
			}
		}

		public string ObterDatum(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(
				"select lm.texto from {0}lov_coordenada_datum lm where lm.id = :id", EsquemaInterno);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return bancoDeDados.ExecutarScalar<string>(comando);
			}
		}

		public string ObterTipoCoordenada(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(
				"select lm.texto from {0}lov_coordenada_tipo lm where lm.id = :id", EsquemaInterno);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return bancoDeDados.ExecutarScalar<string>(comando);
			}
		}
	}
}