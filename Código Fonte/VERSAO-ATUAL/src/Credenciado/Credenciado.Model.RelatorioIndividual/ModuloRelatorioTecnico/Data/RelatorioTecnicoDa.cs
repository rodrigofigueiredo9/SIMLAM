using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloProjetoDigital;
using Tecnomapas.Blocos.Data;
using System.Data;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloRequerimento;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.Entities;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;

namespace Tecnomapas.EtramiteX.Credenciado.Model.RelatorioIndividual.ModuloRelatorioTecnico.Data
{
	public class RelatorioTecnicoDa
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

		#endregion

		public RelatorioTecnico Obter(int projetoDigitalId, BancoDeDados banco = null) 
		{
			RelatorioTecnico relatorio = new RelatorioTecnico();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
			{
				#region Projeto Digital

				Comando comando = bancoDeDados.CriarComando(@"
				select p.id Id, p.requerimento RequerimentoId, p.data_criacao DataCadastro, p.situacao Situacao, lps.texto SituacaoTexto 
				from {0}tab_projeto_digital p, {0}lov_projeto_digital_situacao lps where p.situacao = lps.id and p.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", projetoDigitalId,DbType.Int32);

				relatorio.ProjetoDigital = bancoDeDados.ObterEntity<ProjetoDigitalRelatorio>(comando);

				relatorio.ProjetoDigital.Dependencias = ObterDependencias(projetoDigitalId, bancoDeDados);

				#endregion

				#region Requerimento

				comando = bancoDeDados.CriarComando(@"
				select r.id, r.interessado, r.credenciado, trunc(r.data_criacao) data_cadastro, r.empreendimento, r.situacao, lrs.texto situacao_texto,
				(select pe.municipio from {0}tab_pessoa_endereco pe, {0}tab_credenciado c where pe.pessoa = c.pessoa and c.id = r.credenciado) municipio from {0}tab_requerimento r, 
				tab_projeto_digital p, lov_requerimento_situacao lrs where r.id = p.requerimento and r.situacao = lrs.id and r.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", relatorio.ProjetoDigital.RequerimentoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						relatorio.RequerimentoDigital.Id = reader.GetValue<int>("id");
						relatorio.RequerimentoDigital.Interessado.Id = reader.GetValue<int>("interessado");
						relatorio.RequerimentoDigital.AutorId = reader.GetValue<int>("credenciado");
						relatorio.RequerimentoDigital.EmpreendimentoId = reader.GetValue<int>("empreendimento");
						relatorio.RequerimentoDigital.SituacaoId = reader.GetValue<int>("situacao");
						relatorio.RequerimentoDigital.SituacaoTexto = reader.GetValue<string>("situacao_texto");
						relatorio.RequerimentoDigital.MunicipioId = reader.GetValue<int>("municipio");
						relatorio.RequerimentoDigital.Municipio = ObterMunicipio(relatorio.RequerimentoDigital.MunicipioId);
						relatorio.RequerimentoDigital.DataCadastro = reader.GetValue<DateTime>("data_cadastro");
						relatorio.RequerimentoDigital.DataCadastroExtenso = relatorio.RequerimentoDigital.DataCadastro.ToLongDateString().Split(',').GetValue(1).ToString();
					}

					reader.Close();
				}

				#region Atividades

				comando = bancoDeDados.CriarComando(@"
									select a.id ,a.atividade, a.tid, ta.atividade nome
									  from tab_requerimento_atividade a, tab_atividade ta
									 where ta.id = a.atividade and a.requerimento = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", relatorio.ProjetoDigital.RequerimentoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					RequerimentoAtividadeRelatorio atividade;

					while (reader.Read())
					{
						atividade = new RequerimentoAtividadeRelatorio();
						atividade.Id = reader.GetValue<int>("atividade");
						atividade.IdRelacionamento = reader.GetValue<int>("id");
						atividade.NomeAtividade = reader.GetValue<string>("nome");

						#region Atividades/Finalidades/Modelos

						comando = bancoDeDados.CriarComando(@"
												select a.id,
													   a.finalidade,
													   ltf.texto,
													   a.modelo,
													   tm.nome titulo_modelo,
													   a.titulo_anterior_tipo,
													   a.titulo_anterior_id,
													   a.titulo_anterior_numero,
													   a.modelo_anterior_id,
													   a.modelo_anterior_nome,
													   a.modelo_anterior_sigla,
													   a.orgao_expedidor
												 from {0}tab_requerimento_ativ_finalida a ,{1}tab_titulo_modelo tm, {1}lov_titulo_finalidade ltf      
												 where a.finalidade = ltf.id and a.modelo = tm.id and a.requerimento_ativ =:id", EsquemaBanco, EsquemaInterno);

						comando.AdicionarParametroEntrada("id", atividade.IdRelacionamento, DbType.Int32);

						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							Finalidade fin;

							while (readerAux.Read())
							{
								fin = new Finalidade();

								fin.IdRelacionamento = Convert.ToInt32(readerAux["id"]);
								fin.TituloModeloTexto = readerAux.GetValue<string>("titulo_modelo");
								fin.Texto = readerAux.GetValue<string>("texto");
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

						relatorio.RequerimentoDigital.Atividades.Add(atividade);
					}

					reader.Close();
				}

				#endregion

				#region Empreendimento

				if (relatorio.RequerimentoDigital.EmpreendimentoId > 0)
				{
					comando = bancoDeDados.CriarComando(@"
					select e.id, e.codigo, e.segmento, les.texto segmento_texto, e.denominador, e.cnpj, e.atividade, e.nome_fantasia, e.denominador razao_social
					from {0}tab_empreendimento e, {1}lov_empreendimento_segmento les where e.segmento = les.id and e.id = :id", EsquemaBanco, EsquemaInterno);

					comando.AdicionarParametroEntrada("id", relatorio.RequerimentoDigital.EmpreendimentoId, DbType.Int32);
					EmpreendimentoRelatorio empreendimento = new EmpreendimentoRelatorio();

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						if (reader.Read())
						{
							empreendimento.Id = relatorio.RequerimentoDigital.EmpreendimentoId;
							empreendimento.Codigo = reader.GetValue<int>("codigo");
							empreendimento.CNPJ = reader["cnpj"].ToString();
							empreendimento.NomeFantasia = reader["nome_fantasia"].ToString();
							empreendimento.Denominador = reader["denominador"].ToString();
							empreendimento.SegmentoTexto = reader["segmento_texto"].ToString();
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
													  le.sigla estado_sigla,
													  te.municipio municipio_id,
													  te.numero,
													  te.complemento,
													  lm.texto municipio,
													  (case
														when te.zona = 1 then
														'Urbana'
														else
														'Rural'
													end) zona,
													te.distrito,
													te.corrego,
													te.complemento
												from {0}tab_empreendimento_endereco te, {1}lov_municipio lm, {1}lov_estado le
												where te.estado = le.id and te.municipio = lm.id and te.empreendimento = :empreendimento", EsquemaBanco, EsquemaInterno);

					comando.AdicionarParametroEntrada("empreendimento", empreendimento.Id, DbType.Int32);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						EnderecoRelatorio end;
						while (reader.Read())
						{
							end = new EnderecoRelatorio();
							end.Id = Convert.ToInt32(reader["id"]);
							end.MunicipioTexto = reader.GetValue<string>("municipio");
							end.Correspondencia = Convert.IsDBNull(reader["correspondencia"]) ? 0 : Convert.ToInt32(reader["correspondencia"]);
							end.Cep = reader["cep"].ToString();
							end.Logradouro = reader["logradouro"].ToString();
							end.Bairro = reader["bairro"].ToString();
							end.EstadoId = Convert.IsDBNull(reader["estado_id"]) ? 0 : Convert.ToInt32(reader["estado_id"]);
							end.EstadoSigla = reader["estado_sigla"].ToString();
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
													select ec.id,
														 ec.tipo_coordenada,
														 lct.texto coordenada_tipo_texto,
														 ec.datum,
														 ld.texto datum_texto,
														 ec.easting_utm,
														 ec.northing_utm,
														 ec.fuso_utm,
														 ec.hemisferio_utm,
														 ec.latitude_gms,
														 ec.longitude_gms,
														 ec.latitude_gdec,
														 ec.longitude_gdec,
														 ec.forma_coleta    forma_coleta,
														 lef.texto			 forma_coleta_texto,
														 ec.local_coleta    local_coleta,
														 lelc.texto          local_coleta_texto,
														 ec.datum           datum_texto,
														 ec.hemisferio_utm  hemisferio_utm_texto
													from {0}tab_empreendimento_coord ec, {1}lov_coordenada_datum ld, {1}lov_coordenada_tipo lct, {1}lov_empreendimento_local_colet lelc ,{1}lov_empreendimento_forma_colet lef
													where ec.forma_coleta = lef.id and ec.local_coleta = lelc.id and ec.datum = ld.id and ec.tipo_coordenada = lct.id and ec.empreendimento = :empreendimentoid", EsquemaBanco, EsquemaInterno);

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
							empreendimento.Coordenada.DatumTexto = reader.GetValue<string>("datum_texto");
							empreendimento.Coordenada.Tipo.Id = Convert.ToInt32(reader["tipo_coordenada"]);
							empreendimento.Coordenada.Tipo.Texto = reader.GetValue<string>("coordenada_tipo_texto");
							empreendimento.Coordenada.FormaColetaTexto = Convert.ToString(reader["forma_coleta_texto"]);
							empreendimento.Coordenada.LocalColetaTexto = Convert.ToString(reader["local_coleta_texto"]);
						}
						reader.Close();
					}

					#endregion
					
					relatorio.RequerimentoDigital.Empreendimento = empreendimento;
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

				comando.AdicionarParametroEntrada("id", relatorio.RequerimentoDigital.Interessado.Id, DbType.Int32);
				PessoaRelatorio pessoa = new PessoaRelatorio();

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						pessoa.Id = relatorio.RequerimentoDigital.Interessado.Id;
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

				relatorio.RequerimentoDigital.Interessado = pessoa;

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

				comando.AdicionarParametroEntrada("requerimento", relatorio.RequerimentoDigital.Id, DbType.Int32);

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

						relatorio.RequerimentoDigital.Responsaveis.Add(responsavel);
					}
					reader.Close();
				}

				#endregion

				#endregion

				#region Elaborador

				comando = bancoDeDados.CriarComando(@"
				select nvl(p.nome, p.razao_social) NomeRazaoSocial, lc.texto TipoTexto 
				from {0}tab_credenciado c, {0}tab_pessoa p, {0}lov_credenciado_tipo lc 
				where c.pessoa = p.id and c.tipo = lc.id and c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", relatorio.RequerimentoDigital.AutorId, DbType.Int32);
				relatorio.UsuarioCredenciado = bancoDeDados.ObterEntity<CredenciadoRelatorio>(comando);

				#endregion
			}

			return relatorio;
		}

		public RelatorioTecnico ObterHistorico(int projetoDigitalId, eProjetoDigitalSituacao situacao, BancoDeDados banco = null)
		{
			RelatorioTecnico relatorio = new RelatorioTecnico();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
			{

				#region Projeto Digital

				Comando comando = bancoDeDados.CriarComando(@"
				select p.projeto_id      Id,
				  p.tid             Tid,
				  p.requerimento_id RequerimentoId,
				  p.requerimento_tid RequerimentoTid,
				  p.data_criacao    DataCadastro,
				  p.situacao_id     Situacao,
				  p.situacao_texto  SituacaoTexto
				from {0}hst_projeto_digital p
				where p.projeto_id = :id
				and p.data_execucao = (select max(h.data_execucao) from {0}hst_projeto_digital h where h.projeto_id = :id and h.situacao_id = :situacao_id)", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", projetoDigitalId, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao_id", (int)situacao, DbType.Int32);
				

				relatorio.ProjetoDigital = bancoDeDados.ObterEntity<ProjetoDigitalRelatorio>(comando);
				
				relatorio.ProjetoDigital.Dependencias = ObterDependencias(projetoDigitalId, bancoDeDados);
				
				#endregion

				#region Requerimento

				comando = bancoDeDados.CriarComando(@"
				select r.id id_hst, 
					r.requerimento_id,
					r.interessado_id,
					r.interessado_tid,
					r.credenciado_id,
					r.credenciado_tid,
					trunc(r.data_criacao) data_cadastro,
					r.empreendimento_id,
					r.empreendimento_tid,
					r.situacao_id,
					r.situacao_texto,
					(select distinct pe.municipio_texto
						from hst_credenciado c, hst_pessoa p, hst_pessoa_endereco pe
						where c.pessoa_id = p.pessoa_id
						and c.pessoa_tid = p.tid
						and p.id = pe.id_hst
						and c.credenciado_id = r.credenciado_id
						and c.tid = r.credenciado_tid
						and c.data_execucao = (select max(h.data_execucao) from hst_credenciado h 
						where h.credenciado_id = c.credenciado_id and h.data_execucao < r.data_execucao)) municipio_texto
				from {0}hst_requerimento r
				where r.requerimento_id = :requerimento_id
				and r.tid = :requerimento_tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("requerimento_id", relatorio.ProjetoDigital.RequerimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("requerimento_tid", relatorio.ProjetoDigital.RequerimentoTid, DbType.String);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						relatorio.RequerimentoDigital.Id = reader.GetValue<int>("requerimento_id");
						relatorio.RequerimentoDigital.HistoricoId = reader.GetValue<int>("id_hst");
						relatorio.RequerimentoDigital.Interessado.Id = reader.GetValue<int>("interessado_id");
						relatorio.RequerimentoDigital.Interessado.Tid = reader.GetValue<String>("interessado_tid");
						relatorio.RequerimentoDigital.AutorId = reader.GetValue<int>("credenciado_id");
						relatorio.RequerimentoDigital.AutorTid = reader.GetValue<String>("credenciado_tid");
						relatorio.RequerimentoDigital.EmpreendimentoId = reader.GetValue<int>("empreendimento_id");
						relatorio.RequerimentoDigital.EmpreendimentoTid = reader.GetValue<String>("empreendimento_tid");
						relatorio.RequerimentoDigital.SituacaoId = reader.GetValue<int>("situacao_id");
						relatorio.RequerimentoDigital.SituacaoTexto = reader.GetValue<string>("situacao_texto");
						relatorio.RequerimentoDigital.Municipio = reader.GetValue<String>("municipio_texto");						
						relatorio.RequerimentoDigital.DataCadastro = reader.GetValue<DateTime>("data_cadastro");
						relatorio.RequerimentoDigital.DataCadastroExtenso = relatorio.RequerimentoDigital.DataCadastro.ToLongDateString().Split(',').GetValue(1).ToString();
					}

					reader.Close();
				}

				#region Atividades

				comando = bancoDeDados.CriarComando(@"
				select a.requeri_ativ_id, 
					a.atividade_id, 
					ta.atividade atividade_nome,
					a.tid,
					a.id id_hst
				from {0}hst_requerimento_atividade a, {0}hst_atividade ta
				where a.atividade_id = ta.atividade_id
				and a.atividade_tid = ta.tid
				and a.requerimento_id = :requerimento_id
				and a.requerimento_tid = :requerimento_tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("requerimento_id", relatorio.ProjetoDigital.RequerimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("requerimento_tid", relatorio.ProjetoDigital.RequerimentoTid, DbType.String);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					RequerimentoAtividadeRelatorio atividade;

					while (reader.Read())
					{
						atividade = new RequerimentoAtividadeRelatorio();
						atividade.Id = reader.GetValue<int>("atividade_id");						
						atividade.NomeAtividade = reader.GetValue<string>("atividade_nome");
						atividade.Tid = reader.GetValue<String>("tid");
						atividade.HistoricoId = reader.GetValue<int>("id_hst"); 						

						#region Atividades/Finalidades/Modelos

						comando = bancoDeDados.CriarComando(@"
						select a.requerimento_ativ_fin_id id,
							a.finalidade,
							ltf.texto,
							a.modelo_id,
							tm.nome titulo_modelo,
							a.titulo_anterior_tipo_id,
							a.titulo_anterior_id,
							a.titulo_anterior_numero,
							a.modelo_anterior_id,
							a.modelo_anterior_nome,
							a.modelo_anterior_sigla,
							a.orgao_expedidor
						from {0}hst_requerimento_ativ_finalida a,
							{1}hst_titulo_modelo         tm,
							{1}lov_titulo_finalidade     ltf
						where a.finalidade = ltf.id
						and a.modelo_id = tm.modelo_id
						and a.modelo_tid = tm.tid
						and a.id_hst = :id_hst
						and a.tid = :tid", EsquemaBanco, EsquemaInterno);

						comando.AdicionarParametroEntrada("id_hst", atividade.HistoricoId, DbType.Int32);
						comando.AdicionarParametroEntrada("tid", atividade.Tid, DbType.String);

						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							Finalidade fin;

							while (readerAux.Read())
							{
								fin = new Finalidade();

								fin.IdRelacionamento = Convert.ToInt32(readerAux["id"]);
								fin.TituloModeloTexto = readerAux.GetValue<string>("titulo_modelo");
								fin.Texto = readerAux.GetValue<string>("texto");
								fin.OrgaoExpedidor = readerAux["orgao_expedidor"].ToString();

								if (readerAux["finalidade"] != DBNull.Value)
								{
									fin.Id = Convert.ToInt32(readerAux["finalidade"]);
								}

								if (readerAux["modelo_id"] != DBNull.Value)
								{
									fin.TituloModelo = Convert.ToInt32(readerAux["modelo_id"]);
								}

								if (readerAux["modelo_anterior_id"] != DBNull.Value)
								{
									fin.TituloModeloAnteriorId = Convert.ToInt32(readerAux["modelo_anterior_id"]);
								}

								fin.TituloModeloAnteriorTexto = readerAux["modelo_anterior_nome"].ToString();
								fin.TituloModeloAnteriorSigla = readerAux["modelo_anterior_sigla"].ToString();

								if (readerAux["titulo_anterior_tipo_id"] != DBNull.Value)
								{
									fin.TituloAnteriorTipo = Convert.ToInt32(readerAux["titulo_anterior_tipo_id"]);
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

						relatorio.RequerimentoDigital.Atividades.Add(atividade);
					}

					reader.Close();
				}

				#endregion

				#region Empreendimento

				if (relatorio.RequerimentoDigital.EmpreendimentoId > 0)
				{
					comando = bancoDeDados.CriarComando(@"
					select e.id id_hst,
						e.empreendimento_id,
						e.codigo,
						e.segmento_id,
						e.segmento_texto,
						e.denominador,
						e.cnpj,
						e.atividade_id,
						e.nome_fantasia,
						e.denominador razao_social
					from {0}hst_empreendimento e
					where e.empreendimento_id = :id
					and e.tid = :tid", EsquemaBanco);

					comando.AdicionarParametroEntrada("id", relatorio.RequerimentoDigital.EmpreendimentoId, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", relatorio.RequerimentoDigital.EmpreendimentoTid, DbType.String);

					EmpreendimentoRelatorio empreendimento = new EmpreendimentoRelatorio();

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						if (reader.Read())
						{
							empreendimento.Id = relatorio.RequerimentoDigital.EmpreendimentoId;
							empreendimento.Tid = relatorio.RequerimentoDigital.EmpreendimentoTid;
							empreendimento.HistoricoId = reader.GetValue<int>("id_hst");							
							empreendimento.Codigo = reader.GetValue<int>("codigo");
							empreendimento.CNPJ = reader["cnpj"].ToString();
							empreendimento.NomeFantasia = reader["nome_fantasia"].ToString();
							empreendimento.Denominador = reader["denominador"].ToString();
							empreendimento.SegmentoTexto = reader["segmento_texto"].ToString();
							empreendimento.AtividadeTexto = Convert.ToString(reader["atividade_id"]);
							empreendimento.NomeRazao = Convert.ToString(reader["razao_social"]);
						}

						reader.Close();
					}

					#region Endereços

					comando = bancoDeDados.CriarComando(@"
					select te.endereco_id,
						te.correspondencia,
						te.cep,
						te.logradouro,
						te.bairro,
						te.estado_id,
						le.sigla estado_sigla,
						te.municipio_id,
						te.numero,
						te.complemento,
						municipio_texto,
						(case
							when te.zona = 1 then
							'Urbana'
							else
							'Rural'
						end) zona,
						te.distrito,
						te.corrego,
						te.complemento
					from {0}hst_empreendimento_endereco te, {1}lov_estado le
					where te.estado_id = le.id
					and te.id_hst = :id_hst
					and te.tid = :tid", EsquemaBanco, EsquemaInterno);

					comando.AdicionarParametroEntrada("id_hst", empreendimento.HistoricoId, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", empreendimento.Tid, DbType.String);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						EnderecoRelatorio end;
						while (reader.Read())
						{
							end = new EnderecoRelatorio();
							end.Id = Convert.ToInt32(reader["endereco_id"]);
							end.MunicipioId = Convert.IsDBNull(reader["municipio_id"]) ? 0 : Convert.ToInt32(reader["municipio_id"]);
							end.MunicipioTexto = reader.GetValue<string>("municipio_texto");
							end.Correspondencia = Convert.IsDBNull(reader["correspondencia"]) ? 0 : Convert.ToInt32(reader["correspondencia"]);
							end.Cep = reader["cep"].ToString();
							end.Logradouro = reader["logradouro"].ToString();
							end.Bairro = reader["bairro"].ToString();
							end.EstadoId = Convert.IsDBNull(reader["estado_id"]) ? 0 : Convert.ToInt32(reader["estado_id"]);
							end.EstadoSigla = reader["estado_sigla"].ToString();							
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
					select ec.coordenada_id,
							ec.tipo_coordenada_id,
							ec.tipo_coordenada_texto,
							ec.datum_id,
							ec.datum_texto,
							ec.easting_utm,
							ec.northing_utm,
							ec.fuso_utm,
							ec.hemisferio_utm_id,
							ec.latitude_gms,
							ec.longitude_gms,
							ec.latitude_gdec,
							ec.longitude_gdec,
							ec.forma_coleta_id,
							ec.forma_coleta_texto,
							ec.local_coleta_id,
							ec.local_coleta_texto,							
							ec.hemisferio_utm_texto
					from {0}hst_empreendimento_coord ec
					where ec.id_hst = :id_hst
						and ec.tid = :tid", EsquemaBanco);

					comando.AdicionarParametroEntrada("id_hst", empreendimento.HistoricoId, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", empreendimento.Tid, DbType.String);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						if (reader.Read())
						{
							empreendimento.Coordenada.Id = Convert.ToInt32(reader["coordenada_id"]);
							empreendimento.Coordenada.EastingUtmTexto = Convert.ToString(reader["easting_utm"]);
							empreendimento.Coordenada.NorthingUtmTexto = Convert.ToString(reader["northing_utm"]);
							empreendimento.Coordenada.FusoUtm = Convert.IsDBNull(reader["fuso_utm"]) ? 0 : Convert.ToInt32(reader["fuso_utm"]);
							empreendimento.Coordenada.HemisferioUtm = Convert.IsDBNull(reader["hemisferio_utm_id"]) ? 0 : Convert.ToInt32(reader["hemisferio_utm_id"]);
							empreendimento.Coordenada.LatitudeGms = reader["latitude_gms"].ToString();
							empreendimento.Coordenada.LongitudeGms = reader["longitude_gms"].ToString();
							empreendimento.Coordenada.LatitudeGdec = Convert.IsDBNull(reader["latitude_gdec"]) ? 0 : Convert.ToDouble(reader["latitude_gdec"]);
							empreendimento.Coordenada.LongitudeGdec = Convert.IsDBNull(reader["longitude_gdec"]) ? 0 : Convert.ToDouble(reader["longitude_gdec"]);
							empreendimento.Coordenada.Datum.Id = Convert.ToInt32(reader["datum_id"]);
							empreendimento.Coordenada.DatumTexto = reader.GetValue<string>("datum_texto");
							empreendimento.Coordenada.Tipo.Id = Convert.ToInt32(reader["tipo_coordenada_id"]);
							empreendimento.Coordenada.Tipo.Texto = reader.GetValue<string>("tipo_coordenada_texto");
							empreendimento.Coordenada.FormaColetaTexto = Convert.ToString(reader["forma_coleta_texto"]);
							empreendimento.Coordenada.LocalColetaTexto = Convert.ToString(reader["local_coleta_texto"]);
						}
						reader.Close();
					}

					#endregion

					relatorio.RequerimentoDigital.Empreendimento = empreendimento;
				}

				#endregion

				#region Interessado

				comando = bancoDeDados.CriarComando(@"
				select p.pessoa_id,
					p.tipo,
					p.nome,
					p.cpf,
					p.rg,					
					p.cnpj,
					p.razao_social,
					p.nome_fantasia,
					p.ie,
					p.apelido,
					tpp.profissao_id
				from {0}hst_pessoa p, {0}hst_pessoa_profissao tpp
				where p.id = tpp.id_hst(+)
				and p.tid = tpp.tid(+)
				and p.pessoa_id = :id
				and p.tid = :tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", relatorio.RequerimentoDigital.Interessado.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", relatorio.RequerimentoDigital.Interessado.Tid, DbType.String);
				PessoaRelatorio pessoa = new PessoaRelatorio();

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						pessoa.Id = relatorio.RequerimentoDigital.Interessado.Id;
						pessoa.Tid = relatorio.RequerimentoDigital.Interessado.Tid;
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

				comando = bancoDeDados.CriarComando(@"select a.id, p.pessoa_id, a.meio_contato_id, a.valor
													from {0}hst_pessoa p, {0}hst_pessoa_meio_contato a
													where p.id = a.id_hst
													and p.tid = a.tid
													and p.pessoa_id = :pessoa_id
													and p.tid = :tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("pessoa_id", pessoa.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", pessoa.Tid, DbType.String);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					ContatoRelatorio contato;
					while (reader.Read())
					{
						contato = new ContatoRelatorio();
						contato.Id = Convert.ToInt32(reader["id"]);
						contato.PessoaId = Convert.ToInt32(reader["pessoa_id"]);
						contato.TipoContato = (eTipoContato)Enum.Parse(contato.TipoContato.GetType(), reader["meio_contato_id"].ToString());
						contato.Valor = reader["valor"].ToString();
						pessoa.MeiosContatos.Add(contato);
					}
					reader.Close();
				}

				#endregion

				#region Endereços

				comando = bancoDeDados.CriarComando(@"
				select te.endereco_id,
					p.pessoa_id,
					te.cep,
					te.logradouro,
					te.bairro,
					te.estado_id,
					te.municipio_id,
					te.numero,
					te.complemento,
					te.distrito
				from {0}hst_pessoa p, {0}hst_pessoa_endereco te
				where p.id = te.id_hst
				and p.tid = te.tid
				and p.pessoa_id = :pessoa_id
				and p.tid = :tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("pessoa_id", pessoa.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", pessoa.Tid, DbType.String);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						pessoa.Endereco.Id = Convert.ToInt32(reader["endereco_id"]);
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

				relatorio.RequerimentoDigital.Interessado = pessoa;

				#endregion

				#region Responsáveis

				comando = bancoDeDados.CriarComando(@"
				select pr.requeri_resp_id,
					pr.responsavel_id,
					pr.funcao_id,
					pr.numero_art,
					pr.responsavel_tid,
					nvl(p.nome, p.razao_social) nome,
					nvl(p.cpf, p.cnpj) cpf_cnpj,
					nvl(p.rg, p.ie) rg_ie,
					p.tipo,
					trunc(p.data_nascimento) data_nascimento
				from {0}hst_requerimento_responsavel pr, {0}hst_pessoa p
				where pr.responsavel_id = p.pessoa_id
				and pr.responsavel_tid = p.tid
				and pr.id_hst = :id_hst 
				and pr.tid = :tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("id_hst", relatorio.RequerimentoDigital.HistoricoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", relatorio.RequerimentoDigital.Tid, DbType.String);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					ResponsavelTecnicoRelatorio responsavel;
					while (reader.Read())
					{
						responsavel = new ResponsavelTecnicoRelatorio();
						responsavel.Id = Convert.ToInt32(reader["responsavel_id"]);
						responsavel.Tid = reader["responsavel_tid"].ToString();
						responsavel.FuncaoTexto = reader["funcao_id"].ToString();
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

						comando = bancoDeDados.CriarComando(@"select a.pes_meio_cont_id, p.pessoa_id, a.meio_contato_id, a.valor
						from {0}hst_pessoa p, {0}hst_pessoa_meio_contato a
						where p.id = a.id_hst
						and p.tid = a.tid
						and p.pessoa_id = :pessoa_id
						and p.tid = :pessoa_tid", EsquemaBanco);

						comando.AdicionarParametroEntrada("pessoa_id", responsavel.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("pessoa_tid", responsavel.Tid, DbType.Int32);

						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							ContatoRelatorio contato;
							while (readerAux.Read())
							{
								contato = new ContatoRelatorio();
								contato.Id = Convert.ToInt32(readerAux["pes_meio_cont_id"]);
								contato.PessoaId = Convert.ToInt32(readerAux["pessoa_id"]);
								contato.TipoContato = (eTipoContato)Enum.Parse(contato.TipoContato.GetType(), readerAux["meio_contato_id"].ToString());
								contato.Valor = readerAux["valor"].ToString();
								responsavel.MeiosContatos.Add(contato);
							}
							readerAux.Close();
						}

						#endregion

						#region Endereços

						comando = bancoDeDados.CriarComando(@"
						select te.endereco_id,
							p.pessoa_id,
							te.cep,
							te.logradouro,
							te.bairro,
							te.estado_id,
							te.municipio_id,
							te.numero,
							te.complemento,
							te.distrito
						from {0}hst_pessoa p, {0}hst_pessoa_endereco te
						where p.id = te.id_hst
						and p.tid = te.tid
						and p.pessoa_id = :pessoa_id
						and p.tid = :tid", EsquemaBanco);

						comando.AdicionarParametroEntrada("pessoa_id", responsavel.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("tid", responsavel.Tid, DbType.String);

						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							while (readerAux.Read())
							{
								responsavel.Endereco.Id = Convert.ToInt32(readerAux["endereco_id"]);
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

						relatorio.RequerimentoDigital.Responsaveis.Add(responsavel);
					}
					reader.Close();
				}

				#endregion

				#endregion

				#region Elaborador

				comando = bancoDeDados.CriarComando(@"
				select nvl(p.nome, p.razao_social) NomeRazaoSocial, c.tipo_texto TipoTexto
				from {0}hst_credenciado c, {0}hst_pessoa p
				 where c.pessoa_id = p.pessoa_id
				   and c.pessoa_tid = p.tid
				   and c.tid = :tid 
				   and c.credenciado_id = :credenciado_id", EsquemaBanco);

				comando.AdicionarParametroEntrada("tid", relatorio.RequerimentoDigital.AutorTid, DbType.String);
				comando.AdicionarParametroEntrada("credenciado_id", relatorio.RequerimentoDigital.AutorId, DbType.Int32);				
				relatorio.UsuarioCredenciado = bancoDeDados.ObterEntity<CredenciadoRelatorio>(comando);

				#endregion
			}

			return relatorio;
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

		public List<Dependencia> ObterDependencias(int projetoDigitalID, BancoDeDados banco = null)
		{
			List<Dependencia> lista = new List<Dependencia>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select d.dependencia_tipo, d.dependencia_caracterizacao, d.dependencia_id, d.tid, d.dependencia_tid 
				from {0}tab_proj_digital_dependencias d where d.projeto_digital_id = :projeto_digital_id", EsquemaBanco);

				comando.AdicionarParametroEntrada("projeto_digital_id", projetoDigitalID, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Dependencia dependencia = null;

					while (reader.Read())
					{
						dependencia = new Dependencia();

						dependencia.DependenciaTipo = reader.GetValue<int>("dependencia_tipo");
						dependencia.DependenciaCaracterizacao = reader.GetValue<int>("dependencia_caracterizacao");
						dependencia.DependenciaId = reader.GetValue<int>("dependencia_id");
						dependencia.DependenciaTid = reader.GetValue<string>("dependencia_tid");
						dependencia.Tid = reader.GetValue<string>("tid");
						lista.Add(dependencia);
					}

					reader.Close();
				}
			}

			return lista;
		}

	}
}
