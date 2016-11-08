using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.Blocos.Etx.ModuloGeo.Business;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmpreendimento.Data
{
	public class EmpreendimentoInternoDa
	{
		private string EsquemaBanco { get; set; }
		private string EsquemaBancoGeo { get; set; }

		public EmpreendimentoInternoDa(string strBancoDeDados = null, string strBancoDeDadosGeo = null)
		{
			EsquemaBanco = string.Empty;
			EsquemaBancoGeo = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}

			if (!string.IsNullOrEmpty(strBancoDeDadosGeo))
			{
				EsquemaBancoGeo = strBancoDeDadosGeo;
			}
		}

		public Empreendimento Obter(int id, BancoDeDados banco = null)
		{
			Empreendimento empreendimento = new Empreendimento();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Empreendimento

				Comando comando = bancoDeDados.CriarComando(@"select e.id, e.codigo, e.segmento, ls.texto segmento_texto, ls.denominador segmento_denominador, e.cnpj, e.denominador, 
				e.nome_fantasia, e.atividade, a.atividade atividade_texto, e.tid from {0}tab_empreendimento e, {0}tab_empreendimento_atividade a, {0}lov_empreendimento_segmento ls 
				where e.atividade = a.id(+) and e.segmento = ls.id and e.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						empreendimento.Id = id;
						empreendimento.Tid = reader.GetValue<string>("tid");
						empreendimento.Codigo = reader.GetValue<long>("codigo");

						if (reader["segmento"] != null && !Convert.IsDBNull(reader["segmento"]))
						{
							empreendimento.Segmento = Convert.ToInt32(reader["segmento"]);
							empreendimento.SegmentoTexto = reader["segmento_texto"].ToString();
							empreendimento.SegmentoDenominador = reader["segmento_denominador"].ToString();
						}

						empreendimento.CNPJ = reader["cnpj"].ToString();
						empreendimento.Denominador = reader["denominador"].ToString();
						empreendimento.NomeFantasia = reader["nome_fantasia"].ToString();

						if (reader["atividade"] != null && !Convert.IsDBNull(reader["atividade"]))
						{
							empreendimento.Atividade.Id = Convert.ToInt32(reader["atividade"]);
							empreendimento.Atividade.Atividade = reader["atividade_texto"].ToString();
						}
					}

					reader.Close();
				}

				#endregion

				#region Responsáveis

				comando = bancoDeDados.CriarComando(@"select pr.id, pr.empreendimento, pr.responsavel, nvl(p.nome, p.razao_social) nome, nvl(p.cpf, p.cnpj) cpf_cnpj, pr.tipo, 
				lr.texto tipo_texto, pr.data_vencimento, pr.especificar, pr.origem, pr.origem_texto, pr.credenciado_usuario_id 
				from {0}tab_empreendimento_responsavel pr, {0}tab_pessoa p, {0}lov_empreendimento_tipo_resp lr where pr.responsavel = p.id and pr.tipo = lr.id 
				and pr.empreendimento = :empreendimento", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", empreendimento.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Responsavel responsavel;

					while (reader.Read())
					{
						responsavel = new Responsavel();
						responsavel.Id = Convert.ToInt32(reader["responsavel"]);
						responsavel.IdRelacionamento = Convert.ToInt32(reader["id"]);
						responsavel.Tipo = Convert.ToInt32(reader["tipo"]);
						responsavel.TipoTexto = reader["tipo_texto"].ToString();

						if (reader["data_vencimento"] != null && !Convert.IsDBNull(reader["data_vencimento"]))
						{
							responsavel.DataVencimento = Convert.ToDateTime(reader["data_vencimento"]);
						}

						responsavel.NomeRazao = reader["nome"].ToString();
						responsavel.CpfCnpj = reader["cpf_cnpj"].ToString();
						responsavel.EspecificarTexto = reader["especificar"].ToString();

						responsavel.Origem = reader.GetValue<int>("origem");
						responsavel.OrigemTexto = reader.GetValue<string>("origem_texto");
						responsavel.CredenciadoUsuarioId = reader.GetValue<int>("credenciado_usuario_id");

						empreendimento.Responsaveis.Add(responsavel);
					}

					reader.Close();
				}

				#endregion

				#region Endereços

				comando = bancoDeDados.CriarComando(@"select te.id, te.empreendimento, te.correspondencia, te.zona, te.cep, te.logradouro, te.bairro, le.id estado_id, le.texto estado_texto, lm.id municipio_id, 
				lm.texto municipio_texto, te.numero, te.distrito, te.corrego, te.caixa_postal, te.complemento, te.tid from {0}tab_empreendimento_endereco te, {0}lov_estado le, {0}lov_municipio lm where te.estado = le.id(+) 
				and te.municipio = lm.id(+) and te.empreendimento = :empreendimento order by te.correspondencia", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", empreendimento.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Endereco end;

					while (reader.Read())
					{
						end = new Endereco();
						end.Id = Convert.ToInt32(reader["id"]);
						end.Tid = reader["tid"].ToString();
						end.Correspondencia = Convert.IsDBNull(reader["correspondencia"]) ? 0 : Convert.ToInt32(reader["correspondencia"]);
						end.ZonaLocalizacaoId = Convert.IsDBNull(reader["zona"]) ? 0 : Convert.ToInt32(reader["zona"]);
						end.Cep = reader["cep"].ToString();
						end.Logradouro = reader["logradouro"].ToString();
						end.Bairro = reader["bairro"].ToString();
						end.EstadoId = Convert.IsDBNull(reader["estado_id"]) ? 0 : Convert.ToInt32(reader["estado_id"]);
						end.EstadoTexto = reader["estado_texto"].ToString();
						end.MunicipioId = Convert.IsDBNull(reader["municipio_id"]) ? 0 : Convert.ToInt32(reader["municipio_id"]);
						end.MunicipioTexto = reader["municipio_texto"].ToString();
						end.Numero = reader["numero"].ToString();
						end.DistritoLocalizacao = reader["distrito"].ToString();
						end.Corrego = reader["corrego"].ToString();
						end.CaixaPostal = reader["caixa_postal"].ToString();
						end.Complemento = reader["complemento"].ToString();
						empreendimento.Enderecos.Add(end);
					}

					reader.Close();
				}

				#endregion

				#region Coordenada

				comando = bancoDeDados.CriarComando(@"select aec.id, aec.tid, aec.tipo_coordenada, lct.texto tipoTexto, aec.datum, lcd.texto datumTexto, aec.easting_utm,
				aec.northing_utm, aec.fuso_utm, aec.hemisferio_utm, lch.texto hemisferioTexto, aec.latitude_gms,aec.longitude_gms, aec.latitude_gdec, aec.longitude_gdec,
				aec.forma_coleta,   aec.local_coleta  from tab_empreendimento_coord aec, lov_coordenada_datum lcd, lov_coordenada_tipo lct, lov_coordenada_hemisferio lch
				where aec.datum = lcd.id(+) and aec.hemisferio_utm = lch.id(+) and aec.tipo_coordenada = lct.id(+) and aec.empreendimento = :empreendimentoid", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimentoid", empreendimento.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						empreendimento.Coordenada.Id = Convert.ToInt32(reader["id"]);
						empreendimento.Coordenada.Tid = reader["tid"].ToString();

						if (!Convert.IsDBNull(reader["easting_utm"]))
						{
							empreendimento.Coordenada.EastingUtm = Convert.ToDouble(reader["easting_utm"]);
							empreendimento.Coordenada.EastingUtmTexto = empreendimento.Coordenada.EastingUtm.ToString();
						}

						if (!Convert.IsDBNull(reader["northing_utm"]))
						{
							empreendimento.Coordenada.NorthingUtm = Convert.ToDouble(reader["northing_utm"]);
							empreendimento.Coordenada.NorthingUtmTexto = empreendimento.Coordenada.NorthingUtm.ToString();
						}

						empreendimento.Coordenada.FusoUtm = Convert.IsDBNull(reader["fuso_utm"]) ? 0 : Convert.ToInt32(reader["fuso_utm"]);
						empreendimento.Coordenada.HemisferioUtm = Convert.IsDBNull(reader["hemisferio_utm"]) ? 0 : Convert.ToInt32(reader["hemisferio_utm"]);
						empreendimento.Coordenada.LatitudeGms = reader["latitude_gms"].ToString();
						empreendimento.Coordenada.LongitudeGms = reader["longitude_gms"].ToString();

						if (!Convert.IsDBNull(reader["latitude_gdec"]))
						{
							empreendimento.Coordenada.LatitudeGdec = Convert.ToDouble(reader["latitude_gdec"]);
							empreendimento.Coordenada.LatitudeGdecTexto = empreendimento.Coordenada.LatitudeGdec.ToString();
						}

						if (!Convert.IsDBNull(reader["longitude_gdec"]))
						{
							empreendimento.Coordenada.LongitudeGdec = Convert.ToDouble(reader["longitude_gdec"]);
							empreendimento.Coordenada.LongitudeGdecTexto = empreendimento.Coordenada.LongitudeGdec.ToString();
						}

						empreendimento.Coordenada.Datum.Id = Convert.ToInt32(reader["datum"]);
						empreendimento.Coordenada.Tipo.Id = Convert.ToInt32(reader["tipo_coordenada"]);

						if (!Convert.IsDBNull(reader["forma_coleta"]))
						{
							empreendimento.Coordenada.FormaColeta = Convert.ToInt32(reader["forma_coleta"]);
						}

						if (!Convert.IsDBNull(reader["local_coleta"]))
						{
							empreendimento.Coordenada.LocalColeta = Convert.ToInt32(reader["local_coleta"]);
						}

						if (!Convert.IsDBNull(reader["datumtexto"]))
						{
							empreendimento.Coordenada.Datum.Texto = reader["datumtexto"].ToString();
						}

						if (!Convert.IsDBNull(reader["tipotexto"]))
						{
							empreendimento.Coordenada.Tipo.Texto = reader["tipotexto"].ToString();
						}

						if (!Convert.IsDBNull(reader["hemisferiotexto"]))
						{
							empreendimento.Coordenada.HemisferioUtmTexto = reader["hemisferiotexto"].ToString();
						}
					}

					reader.Close();
				}

				#endregion

				#region Meio de Contato

				comando = bancoDeDados.CriarComando(@"select a.id, b.mascara, a.empreendimento, b.id tipo_contato_id, b.texto contato_texto,
				a.valor, a.tid from {0}tab_empreendimento_contato a, {0}tab_meio_contato b where a.meio_contato = b.id and a.empreendimento = :empreendimento", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimento.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Contato contato;

					while (reader.Read())
					{
						contato = new Contato();
						contato.Id = Convert.ToInt32(reader["id"]);
						contato.PessoaId = Convert.ToInt32(reader["empreendimento"]);
						contato.TipoContato = (eTipoContato)Enum.Parse(contato.TipoContato.GetType(), reader["tipo_contato_id"].ToString());
						contato.TipoTexto = reader["contato_texto"].ToString();
						contato.Mascara = reader["mascara"].ToString();
						contato.Valor = reader["valor"].ToString();
						contato.Tid = reader["tid"].ToString();
						empreendimento.MeiosContatos.Add(contato);
					}

					reader.Close();
				}

				#endregion

				empreendimento.TemCorrespondencia = (empreendimento.Enderecos.Any(x => x.Correspondencia == 1) ? 1 : 0);
			}

			return empreendimento;
		}

		public Empreendimento ObterSimplificado(int id, BancoDeDados banco = null)
		{
			Empreendimento empreendimento = new Empreendimento();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Empreendimento

				Comando comando = bancoDeDados.CriarComando(@"select e.id, e.codigo, e.segmento, ls.texto segmento_texto, e.cnpj, e.denominador, e.nome_fantasia, e.atividade, 
				a.atividade atividade_texto, e.tid from {0}tab_empreendimento e, {0}tab_empreendimento_atividade a, {0}lov_empreendimento_segmento ls 
				where e.atividade = a.id(+) and e.segmento = ls.id and e.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						empreendimento.Id = id;
						empreendimento.Tid = reader.GetValue<string>("tid");
						empreendimento.Codigo = reader.GetValue<long>("codigo");

						if (reader["segmento"] != null && !Convert.IsDBNull(reader["segmento"]))
						{
							empreendimento.Segmento = Convert.ToInt32(reader["segmento"]);
							empreendimento.SegmentoTexto = reader["segmento"].ToString();
						}

						empreendimento.CNPJ = reader["cnpj"].ToString();
						empreendimento.Denominador = reader["denominador"].ToString();
						empreendimento.NomeFantasia = reader["nome_fantasia"].ToString();

						if (reader["atividade"] != null && !Convert.IsDBNull(reader["atividade"]))
						{
							empreendimento.Atividade.Id = Convert.ToInt32(reader["atividade"]);
							empreendimento.Atividade.Atividade = reader["atividade_texto"].ToString();
						}
					}

					reader.Close();
				}

				#endregion

				#region Coordenada

				comando = bancoDeDados.CriarComando(@"select aec.id, aec.tid, aec.tipo_coordenada, lct.texto tipoTexto, aec.datum, lcd.texto datumTexto, aec.easting_utm,
				aec.northing_utm, aec.fuso_utm, aec.hemisferio_utm, lch.texto hemisferioTexto, aec.latitude_gms,aec.longitude_gms, aec.latitude_gdec, aec.longitude_gdec,
				aec.forma_coleta,   aec.local_coleta  from tab_empreendimento_coord aec, lov_coordenada_datum lcd, lov_coordenada_tipo lct, lov_coordenada_hemisferio lch
				where aec.datum = lcd.id(+) and aec.hemisferio_utm = lch.id(+) and aec.tipo_coordenada = lct.id(+) and aec.empreendimento = :empreendimentoid", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimentoid", empreendimento.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						empreendimento.Coordenada.Id = Convert.ToInt32(reader["id"]);
						empreendimento.Coordenada.Tid = reader["tid"].ToString();

						if (!Convert.IsDBNull(reader["easting_utm"]))
						{
							empreendimento.Coordenada.EastingUtm = Convert.ToDouble(reader["easting_utm"]);
							empreendimento.Coordenada.EastingUtmTexto = empreendimento.Coordenada.EastingUtm.ToString();
						}

						if (!Convert.IsDBNull(reader["northing_utm"]))
						{
							empreendimento.Coordenada.NorthingUtm = Convert.ToDouble(reader["northing_utm"]);
							empreendimento.Coordenada.NorthingUtmTexto = empreendimento.Coordenada.NorthingUtm.ToString();
						}

						empreendimento.Coordenada.FusoUtm = Convert.IsDBNull(reader["fuso_utm"]) ? 0 : Convert.ToInt32(reader["fuso_utm"]);
						empreendimento.Coordenada.HemisferioUtm = Convert.IsDBNull(reader["hemisferio_utm"]) ? 0 : Convert.ToInt32(reader["hemisferio_utm"]);
						empreendimento.Coordenada.LatitudeGms = reader["latitude_gms"].ToString();
						empreendimento.Coordenada.LongitudeGms = reader["longitude_gms"].ToString();

						if (!Convert.IsDBNull(reader["latitude_gdec"]))
						{
							empreendimento.Coordenada.LatitudeGdec = Convert.ToDouble(reader["latitude_gdec"]);
							empreendimento.Coordenada.LatitudeGdecTexto = empreendimento.Coordenada.LatitudeGdec.ToString();
						}

						if (!Convert.IsDBNull(reader["longitude_gdec"]))
						{
							empreendimento.Coordenada.LongitudeGdec = Convert.ToDouble(reader["longitude_gdec"]);
							empreendimento.Coordenada.LongitudeGdecTexto = empreendimento.Coordenada.LongitudeGdec.ToString();
						}

						empreendimento.Coordenada.Datum.Id = Convert.ToInt32(reader["datum"]);
						empreendimento.Coordenada.Tipo.Id = Convert.ToInt32(reader["tipo_coordenada"]);

						if (!Convert.IsDBNull(reader["forma_coleta"]))
						{
							empreendimento.Coordenada.FormaColeta = Convert.ToInt32(reader["forma_coleta"]);
						}

						if (!Convert.IsDBNull(reader["local_coleta"]))
						{
							empreendimento.Coordenada.LocalColeta = Convert.ToInt32(reader["local_coleta"]);
						}

						if (!Convert.IsDBNull(reader["datumtexto"]))
						{
							empreendimento.Coordenada.Datum.Texto = reader["datumtexto"].ToString();
						}

						if (!Convert.IsDBNull(reader["tipotexto"]))
						{
							empreendimento.Coordenada.Tipo.Texto = reader["tipotexto"].ToString();
						}

						if (!Convert.IsDBNull(reader["hemisferiotexto"]))
						{
							empreendimento.Coordenada.HemisferioUtmTexto = reader["hemisferiotexto"].ToString();
						}
					}

					reader.Close();
				}

				#endregion
			}

			return empreendimento;
		}

		internal Empreendimento Obter(String cnpj, BancoDeDados banco = null)
		{
			Empreendimento empreendimento = new Empreendimento();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select id from {0}tab_empreendimento p where p.cnpj = :cnpj", EsquemaBanco);
				comando.AdicionarParametroEntrada("cnpj", cnpj, DbType.String);
				Object id = bancoDeDados.ExecutarScalar(comando);

				if (id != null && !Convert.IsDBNull(id))
				{
					empreendimento = Obter(Convert.ToInt32(id));
				}
			}

			return empreendimento;
		}

		internal Empreendimento ObterSimplificado(String cnpj, BancoDeDados banco = null)
		{
			Empreendimento empreendimento = new Empreendimento();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select id from {0}tab_empreendimento p where p.cnpj = :cnpj", EsquemaBanco);
				comando.AdicionarParametroEntrada("cnpj", cnpj, DbType.String);
				Object id = bancoDeDados.ExecutarScalar(comando);

				if (id != null && !Convert.IsDBNull(id))
				{
					empreendimento = ObterSimplificado(Convert.ToInt32(id));
				}
			}

			return empreendimento;
		}

		public Resultados<Empreendimento> Filtrar(Filtro<ListarEmpreendimentoFiltro> filtros, BancoDeDados banco = null)
		{
			Resultados<Empreendimento> retorno = new Resultados<Empreendimento>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAnd("e.codigo", "codigo", filtros.Dados.Codigo);

				comandtxt += comando.FiltroAndLike("e.denominador", "denominador", filtros.Dados.Denominador, true);

				comandtxt += comando.FiltroAndLike("e.cnpj", "cnpj", filtros.Dados.CNPJ);

				comandtxt += comando.FiltroIn("e.empreendimento_id", String.Format(@"select r.empreendimento from {0}tab_empreendimento_responsavel r, {0}lst_pessoa p where p.pessoa_id = r.responsavel 
				and p.cpf_cnpj like :responsavel_cpf_cnpj ||'%'", (string.IsNullOrEmpty(EsquemaBanco) ? "" : ".")), "responsavel_cpf_cnpj", filtros.Dados.Responsavel.CpfCnpj);

				if (!ValidacoesGenericasBus.Cpf(filtros.Dados.Responsavel.CpfCnpj) && !ValidacoesGenericasBus.Cnpj(filtros.Dados.Responsavel.CpfCnpj))
				{
					comandtxt += comando.FiltroIn("e.empreendimento_id", String.Format(@"select r.empreendimento from {0}tab_empreendimento_responsavel r, {0}lst_pessoa p  
					where p.pessoa_id = r.responsavel and upper(p.nome_razao_social) like upper('%'|| :responsavel_nome ||'%')", (string.IsNullOrEmpty(EsquemaBanco) ? "" : ".")), "responsavel_nome", filtros.Dados.Responsavel.NomeRazao);
				}

				comandtxt += comando.FiltroAnd("e.segmento_id", "segmento_id", filtros.Dados.Segmento);

				comandtxt += comando.FiltroAnd("e.municipio_id", "municipio_id", filtros.Dados.MunicipioId);

				if (filtros.Dados.MunicipioId.GetValueOrDefault() <= 0)
				{
					comandtxt += comando.FiltroAnd("e.estado_id", "estado_id", filtros.Dados.EstadoId);
				}

				comandtxt += comando.FiltroAnd("e.atividade_id", "atividade_id", filtros.Dados.Atividade.Id);

				comandtxt += comando.FiltroAnd("e.cnpj", "cnpj_empreemdimento", filtros.Dados.CnpjEmpreemdimento);

				comandtxt += comando.FiltroIn("e.empreendimento_id", String.Format(@"select p.empreendimento_id from {0}lst_protocolo p where p.numero_completo = :protocolo_numero", (string.IsNullOrEmpty(EsquemaBanco) ? "" : ".")), "protocolo_numero", filtros.Dados.Protocolo.NumeroTexto);

				//Filtro de abrangencia
				if (!string.IsNullOrEmpty(filtros.Dados.AreaAbrangencia) && filtros.Dados.Coordenada != null)
				{
					//Transforma as coordenadas
					filtros.Dados.Coordenada = CoordenadaBus.TransformarCoordenada(filtros.Dados.Coordenada);
					Double abrangencia = Convert.ToDouble(filtros.Dados.AreaAbrangencia);

					comandtxt += String.Format(@" and e.empreendimento_id in (select e.empreendimento from {0}geo_emp_localizacao e where sdo_relate(e.geometry, 
					{0}coordenada.utm2spatialrect(:x1, :y1, :f1, :x2, :y2, :f2, 0), 'MASK=ANYINTERACT QUERYTYPE=WINDOW')='TRUE')",
					(string.IsNullOrEmpty(EsquemaBancoGeo) ? "" : EsquemaBancoGeo + "."));

					comando.AdicionarParametroEntrada("x1", (filtros.Dados.Coordenada.EastingUtm - abrangencia), DbType.Double);
					comando.AdicionarParametroEntrada("y1", (filtros.Dados.Coordenada.NorthingUtm - abrangencia), DbType.Double);
					comando.AdicionarParametroEntrada("f1", filtros.Dados.Coordenada.FusoUtm, DbType.Int32);

					comando.AdicionarParametroEntrada("x2", (filtros.Dados.Coordenada.EastingUtm + abrangencia), DbType.Double);
					comando.AdicionarParametroEntrada("y2", (filtros.Dados.Coordenada.NorthingUtm + abrangencia), DbType.Double);
					comando.AdicionarParametroEntrada("f2", filtros.Dados.Coordenada.FusoUtm, DbType.Int32);
				}

				if (filtros.Dados.EmpreendimentoCompensacao > 0)
				{
					comandtxt += string.Format(@" and e.empreendimento_id in (select empreendimento from crt_dominialidade where id in (select dominialidade from 
					crt_dominialidade_dominio where id in (select dominio from crt_dominialidade_reserva where cedente_receptor = 1 and emp_compensacao = :emp_compensacao))) ", (string.IsNullOrEmpty(EsquemaBanco) ? "" : EsquemaBanco + "."));

					comando.AdicionarParametroEntrada("emp_compensacao", filtros.Dados.EmpreendimentoCompensacao, DbType.Int32);
				}

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "codigo", "segmento_texto", "denominador", "cnpj" };

				if (filtros.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("denominador");
				}

				#endregion

				#region Quantidade de registro do resultado

				comando.DbCommand.CommandText = String.Format("select count(*) from {0}lst_empreendimento e where e.id > 0" + comandtxt,
				(string.IsNullOrEmpty(EsquemaBanco) ? "" : EsquemaBanco + "."));

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = String.Format(@"select e.empreendimento_id id, e.codigo, e.denominador, e.cnpj, e.segmento_texto from {0}lst_empreendimento e where e.id > 0"
				+ comandtxt + DaHelper.Ordenar(colunas, ordenar), (string.IsNullOrEmpty(EsquemaBanco) ? "" : EsquemaBanco + "."));

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					Empreendimento empreendimento;

					while (reader.Read())
					{
						empreendimento = new Empreendimento();
						empreendimento.Id = reader.GetValue<Int32>("id");
						empreendimento.Codigo = reader.GetValue<Int64>("codigo");
						empreendimento.Denominador = reader.GetValue<String>("denominador");
						empreendimento.CNPJ = reader.GetValue<String>("cnpj");
						empreendimento.SegmentoTexto = reader.GetValue<String>("segmento_texto");
						retorno.Itens.Add(empreendimento);
					}

					reader.Close();

					#endregion
				}
			}

			return retorno;
		}

		public bool ExisteCnpj(string cnpj, Int32? id = null, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(cnpj) from {0}tab_empreendimento te where te.cnpj = :cnpj", EsquemaBanco);

				if ((id ?? 0) > 0)
				{
					comando.DbCommand.CommandText += " and te.id != :id";
					comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				}

				comando.AdicionarParametroEntrada("cnpj", cnpj, DbType.String);
				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}
	}
}