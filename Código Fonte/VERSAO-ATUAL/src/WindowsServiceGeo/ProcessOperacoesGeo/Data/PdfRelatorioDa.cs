using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;

namespace Tecnomapas.EtramiteX.WindowsService.ProcessOperacoesGeo.Data
{
	class PdfRelatorioDa
	{
		public BancoDeDados banco { get; set; }

		private string _esquemaOficial = "";

		internal string EsquemaOficial
		{
			get
			{
				if (String.IsNullOrEmpty(_esquemaOficial))
				{
					string strSQL = @"begin select to_char(c.valor) into :ESQUEMA_OFICIAL from cnf_sistema_geo c where c.campo = 'ESQUEMA_OFICIAL'; end;";
					using (Comando comando = this.banco.CriarComando(strSQL))
					{
						comando.AdicionarParametroSaida("ESQUEMA_OFICIAL", DbType.String, 100);
						this.banco.ExecutarNonQuery(comando);

						string valor = comando.ObterValorParametro("ESQUEMA_OFICIAL").ToString();
						_esquemaOficial = String.IsNullOrWhiteSpace(valor) ? " " : valor;
					}
				}

				return _esquemaOficial;
			}
		}

		private string EsquemaOficialComPonto
		{
			get { return (EsquemaOficial != " ") ? EsquemaOficial + "." : ""; }
		}

		public PdfRelatorioDa(BancoDeDados banco)
		{
			this.banco = banco;
		}

		internal Hashtable BuscarDadosCabecalhoRodapePDF(int ticketID, int ticketType)
		{
			Hashtable result = null;

			string strSQL = @"begin
	                    select upper(to_char(c.valor)) into :GOVERNO_NOME from {0}cnf_sistema c where c.campo = 'orgaogoverno';
	                    select upper(to_char(c.valor)) into :SECRETARIA_NOME from {0}cnf_sistema c where c.campo = 'orgaosecretaria';
                        select '' into :SETOR_NOME from dual;
	                    select to_char(c.valor) into :ORGAO_CEP from {0}cnf_sistema c where c.campo = 'orgaocep';
                        select to_char(c.valor) into :ORGAO_CONTATO from {0}cnf_sistema c where c.campo = 'orgaotelefone';
                        select to_char(c.valor) into :ORGAO_ENDERECO from {0}cnf_sistema c where c.campo = 'orgaoendereco';
                        select to_char(c.valor) into :ORGAO_MUNICIPIO from {0}cnf_sistema c where c.campo = 'orgaomunicipio';
                        select to_char(c.valor) into :ORGAO_NOME from {0}cnf_sistema c where c.campo = 'orgaonome';
                        select to_char(c.valor) into :ORGAO_SIGLA from {0}cnf_sistema c where c.campo = 'orgaosigla';
                        select to_char(c.valor) into :ORGAO_UF from {0}cnf_sistema c where c.campo = 'orgaouf';
                    end;";

			strSQL = strSQL.Replace("\r", "").Replace("\n", "");
			strSQL = String.Format(strSQL, EsquemaOficialComPonto);

			using (Comando comando = this.banco.CriarComando(strSQL))
			{
				comando.AdicionarParametroSaida("GOVERNO_NOME", DbType.String, 100);
				comando.AdicionarParametroSaida("ORGAO_CEP", DbType.String, 100);
				comando.AdicionarParametroSaida("ORGAO_CONTATO", DbType.String, 100);
				comando.AdicionarParametroSaida("ORGAO_ENDERECO", DbType.String, 100);
				comando.AdicionarParametroSaida("ORGAO_MUNICIPIO", DbType.String, 100);
				comando.AdicionarParametroSaida("ORGAO_NOME", DbType.String, 100);
				comando.AdicionarParametroSaida("ORGAO_SIGLA", DbType.String, 100);
				comando.AdicionarParametroSaida("ORGAO_UF", DbType.String, 100);
				comando.AdicionarParametroSaida("SECRETARIA_NOME", DbType.String, 100);
				comando.AdicionarParametroSaida("SETOR_NOME", DbType.String, 100);

				//this.Conexao.Comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);
				//this.Conexao.Comando.AdicionarParametroEntrada("tipo", ticketType, DbType.String);

				this.banco.ExecutarNonQuery(comando);

				result = new Hashtable();

				result["GOVERNO_NOME"] = comando.ObterValorParametro("GOVERNO_NOME");
				result["ORGAO_CEP"] = comando.ObterValorParametro("ORGAO_CEP");
				result["ORGAO_CONTATO"] = comando.ObterValorParametro("ORGAO_CONTATO");
				result["ORGAO_ENDERECO"] = comando.ObterValorParametro("ORGAO_ENDERECO");
				result["ORGAO_MUNICIPIO"] = comando.ObterValorParametro("ORGAO_MUNICIPIO");
				result["ORGAO_NOME"] = comando.ObterValorParametro("ORGAO_NOME");
				result["ORGAO_SIGLA"] = comando.ObterValorParametro("ORGAO_SIGLA");
				result["ORGAO_UF"] = comando.ObterValorParametro("ORGAO_UF");
				result["SECRETARIA_NOME"] = comando.ObterValorParametro("SECRETARIA_NOME");
				result["SETOR_NOME"] = comando.ObterValorParametro("SETOR_NOME");

			}

			return result;
		}

		internal Hashtable BuscarDadosPDF(int ticketID, int ticketType, int ticketStep)
		{
			Hashtable result = new Hashtable();
			string strSQL;

			if (ticketType == OperacoesGeoDa.OPERACAO_CADASTRO_PROPRIEDADE)
			{
				#region CADASTRO_PROPRIEDADE

				strSQL = @"select 'ATP' classe, 'Área Total da Propriedade' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from tmp_atp where projeto=:projeto
                    union all select 'ACONSTRUIDA' classe, 'Área Construída' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from tmp_aconstruida where projeto=:projeto
                    union all select 'AFD' classe, 'Área de Faixa de Domínio' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from tmp_afd where projeto=:projeto
                    union all select 'APMP' classe, 'Área da Propriedade por Matrícula ou Posse' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from tmp_apmp where projeto=:projeto
                    union all select 'AFS' classe, 'Área de Faixa de Servidão' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from tmp_afs where projeto=:projeto
                    union all select 'ROCHA' classe, 'Área de rocha' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from tmp_rocha where projeto=:projeto
                    union all select 'MASSA_DAGUA' classe, 'Área de massa de água (represa, lagoa e curso de água)' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from tmp_areas_calculadas where tipo='MASSA_DAGUA_APMP' and projeto=:projeto
                    union all select 'RPPN' classe, 'Reserva Particular do Patrimônio Natural' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from tmp_rppn where projeto=:projeto
                    union all select 'AVN' classe, 'Total (Inicial + Médio + Avançado + Não caracterizado)' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from tmp_avn where projeto=:projeto
                    union all select * from (select classe, descricao, subtipo, sum(area_m2) area_m2 from(
                        select 'AVN' classe, 'Em Estágio Inicial de Regeneração' descricao, vegetacao subtipo, nvl(sum(area_m2),0) area_m2 from tmp_avn where estagio='I' and projeto=:projeto group by vegetacao
                        union all select 'AVN' classe, 'Em Estágio Inicial de Regeneração' descricao, null subtipo, 0 area_m2 from dual) group by classe, descricao, subtipo order by subtipo)
                    union all select * from (select classe, descricao, subtipo, sum(area_m2) area_m2 from(
                        select 'AVN' classe, 'Em Estágio Médio de Regeneração' descricao, vegetacao subtipo, nvl(sum(area_m2),0) area_m2 from tmp_avn where estagio='M' and projeto=:projeto group by vegetacao
                        union all select 'AVN' classe, 'Em Estágio Médio de Regeneração' descricao, null subtipo, 0 area_m2 from dual) group by classe, descricao, subtipo order by subtipo)
                    union all select * from (select classe, descricao, subtipo, sum(area_m2) area_m2 from(
                        select 'AVN' classe, 'Em Estágio Avançado de Regeneração' descricao, vegetacao subtipo, nvl(sum(area_m2),0) area_m2 from tmp_avn where estagio='A' and projeto=:projeto group by vegetacao
                        union all select 'AVN' classe, 'Em Estágio Avançado de Regeneração' descricao, null subtipo, 0 area_m2 from dual) group by classe, descricao, subtipo order by subtipo)
                    union all select * from (select classe, descricao, subtipo, sum(area_m2) area_m2 from(
                        select 'AVN' classe, 'Em Estágio de Regeneração não caracterizado' descricao, vegetacao subtipo, nvl(sum(area_m2),0) area_m2 from tmp_avn where estagio='D' and projeto=:projeto group by vegetacao
                        union all select 'AVN' classe, 'Em Estágio de Regeneração não caracterizado' descricao, null subtipo, 0 area_m2 from dual) group by classe, descricao, subtipo order by subtipo)
                    union all select 'AA' classe, 'Total (Em Recuperação + Em Uso + Não caracterizada)' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from tmp_aa where projeto=:projeto
                    union all select 'AA' classe, 'Em Recuperação' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from tmp_aa where tipo='REC' and projeto=:projeto
                    union all select * from (select classe, descricao, subtipo, sum(area_m2) area_m2 from(
                        select 'AA' classe, 'Em Uso' descricao, vegetacao subtipo, nvl(sum(area_m2),0) area_m2 from tmp_aa where tipo='USO' and projeto=:projeto group by vegetacao
                        union all select 'AA' classe, 'Em Uso' descricao, null subtipo, 0 area_m2 from dual) group by classe, descricao, subtipo order by subtipo)
                    union all select 'AA' classe, 'Não caracterizada' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from tmp_aa where tipo='D' and projeto=:projeto
                    union all select 'ARL' classe, 'Total (Preservada + Em Recuperação + Em Uso + Não caracterizada)' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from tmp_arl where projeto=:projeto
                    union all select 'ARL' classe, 'Preservada' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from tmp_arl where situacao='PRESERV' and projeto=:projeto
                    union all select 'ARL' classe, 'Em Recuperação' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from tmp_arl where situacao='REC' and projeto=:projeto
                    union all select 'ARL' classe, 'Em Uso' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from tmp_arl where situacao='USO' and projeto=:projeto
					union all select 'ARL' classe, 'Não caracterizada' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from tmp_arl where situacao='D' and projeto=:projeto					
					union all select 'ARL' classe, 'Em área de preservação permanente' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from tmp_areas_calculadas where tipo='APP_ARL' and projeto=:projeto 
					union all select 'APP' classe, 'Total (Preservada + Em Recuperação + Em Uso + Não caracterizada)' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from tmp_areas_calculadas where tipo='APP_APMP' and projeto=:projeto
                    union all select 'APP' classe, 'Preservada' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from tmp_areas_calculadas where tipo='APP_AVN' and projeto=:projeto
                    union all select 'APP' classe, 'Em Recuperação' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from tmp_areas_calculadas where tipo='APP_AA_REC' and projeto=:projeto
                    union all select 'APP' classe, 'Em Uso' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from tmp_areas_calculadas where tipo='APP_AA_USO' and projeto=:projeto					
					union all select 'APP' classe, 'Não caracterizada' descricao, '' subtipo,  
						nvl((select sum(a.area_m2) total from tmp_areas_calculadas a where a.tipo='APP_APMP' and a.projeto=:projeto ),0) -
						nvl((select sum(a.area_m2) total from tmp_areas_calculadas a where a.tipo in ('APP_AVN','APP_AA_REC','APP_AA_USO') and a.projeto=:projeto ),0)
						area_m2 from dual  ";

				strSQL = strSQL.Replace("\r", "").Replace("\n", "");

				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);

					result["QUADRO_TOTAL"] = this.banco.ExecutarHashtable(comando);
				}

				foreach (Hashtable hs in (List<Hashtable>)result["QUADRO_TOTAL"])
				{
					if (hs["CLASSE"].ToString() == "ARL")
					{
						result["AREA_ARL"] = hs["AREA_M2"];
						break;
					}
				}


				//Ordenadas TMP_ATP
				strSQL = @"select t.column_value ordenada from table(select t.geometry.sdo_ordinates from tmp_atp t where t.projeto=:projeto) t";

				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);

					result["ORDENADAS_ATP"] = this.banco.ExecutarList<Decimal>(comando);
				}

				//Ordenadas TMP_ARL
				//----------------------------------------------------------------------
				strSQL = @"select t.id, t.codigo from tmp_arl t where t.projeto=:projeto order by t.codigo ";

				Dictionary<String, List<Decimal>> lstCoord = new Dictionary<string, List<decimal>>();

				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);

					using (IDataReader reader = banco.ExecutarReader(comando))
					{
						while (reader.Read())
						{
							lstCoord.Add(reader["codigo"].ToString(), new List<decimal>());
						}

						reader.Close();
					}
				}

				//coordenadas
				strSQL = @"select t.column_value ordenada from table(select t.geometry.sdo_ordinates from tmp_arl t where t.projeto=:projeto and t.codigo=:codigo) t";
				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);
					comando.AdicionarParametroEntrada("codigo", "-", DbType.String);

					lstCoord.Keys.ToList().ForEach(key =>
					{
						comando.SetarValorParametro("codigo", key);
						lstCoord[key] = this.banco.ExecutarList<Decimal>(comando);
					});
				}

				result["ORDENADAS_ARL"] = lstCoord;
				//----------------------------------------------------------------------


				strSQL = @"select
                                upper(a.nome) apmp_nome,
                                (case a.tipo when 'M' then 'Matrícula' when 'P' then 'Posse' else 'Desconhecido' end) apmp_tipo,
                                a.area_m2 apmp_area,
                                sdo_geom.sdo_length(a.geometry, 0.0001) apmp_perimeter,
                                nvl( (select sum( b.area_m2 ) from tmp_afs b where b.projeto=a.projeto and b.cod_apmp = a.id), 0) afs_area,
                                nvl( (select sum( b.area_m2 ) from tmp_rocha b where b.projeto=a.projeto and b.cod_apmp = a.id), 0) rocha_area,
                                nvl( (select sum( b.area_m2 ) from tmp_areas_calculadas b where b.projeto=a.projeto and b.cod_apmp = a.id and b.tipo='MASSA_DAGUA_APMP'), 0) massa_dagua_area,
                                nvl( (select sum( b.area_m2 ) from tmp_avn b where b.projeto=a.projeto and b.cod_apmp = a.id and b.estagio='I'), 0) avn_i_area,
                                nvl( (select sum( b.area_m2 ) from tmp_avn b where b.projeto=a.projeto and b.cod_apmp = a.id and b.estagio='M'), 0) avn_m_area,
                                nvl( (select sum( b.area_m2 ) from tmp_avn b where b.projeto=a.projeto and b.cod_apmp = a.id and b.estagio='A'), 0) avn_a_area,
                                nvl( (select sum( b.area_m2 ) from tmp_avn b where b.projeto=a.projeto and b.cod_apmp = a.id and b.estagio='D'), 0) avn_d_area,
                                nvl( (select sum( b.area_m2 ) from tmp_aa b where b.projeto=a.projeto and b.cod_apmp = a.id and b.tipo='REC'), 0) aa_rec_area,
                                nvl( (select sum( b.area_m2 ) from tmp_aa b where b.projeto=a.projeto and b.cod_apmp = a.id and b.tipo='USO'), 0) aa_uso_area,
                                nvl( (select sum( b.area_m2 ) from tmp_aa b where b.projeto=a.projeto and b.cod_apmp = a.id and b.tipo='D'), 0) aa_d_area,
                                nvl( (select sum( b.area_m2 ) from tmp_arl b where b.projeto=a.projeto and b.cod_apmp = a.id and b.situacao='PRESERV'), 0) arl_preserv_area,
                                nvl( (select sum( b.area_m2 ) from tmp_arl b where b.projeto=a.projeto and b.cod_apmp = a.id and b.situacao='REC'), 0) arl_rec_area,
                                nvl( (select sum( b.area_m2 ) from tmp_arl b where b.projeto=a.projeto and b.cod_apmp = a.id and b.situacao='USO'), 0) arl_uso_area,
                                nvl( (select sum( b.area_m2 ) from tmp_arl b where b.projeto=a.projeto and b.cod_apmp = a.id and b.situacao='D'), 0) arl_d_area,
                                nvl( (select sum( b.area_m2 ) from tmp_rppn b where b.projeto=a.projeto and b.cod_apmp = a.id), 0) rppn_area,
                                nvl( (select sum( b.area_m2 ) from tmp_areas_calculadas b where b.projeto=a.projeto and b.cod_apmp = a.id and b.tipo='APP_APMP'), 0) app_apmp_area,
                                nvl( (select sum( b.area_m2 ) from tmp_areas_calculadas b where b.projeto=a.projeto and b.cod_apmp = a.id and b.tipo='APP_AVN'), 0) app_avn_area,
                                nvl( (select sum( b.area_m2 ) from tmp_areas_calculadas b where b.projeto=a.projeto and b.cod_apmp = a.id and b.tipo='APP_AA_REC'), 0) app_aa_rec_area,
                                nvl( (select sum( b.area_m2 ) from tmp_areas_calculadas b where b.projeto=a.projeto and b.cod_apmp = a.id and b.tipo='APP_AA_USO'), 0) app_aa_uso_area,
                                nvl( (select sum( b.area_m2 ) from tmp_areas_calculadas b where b.projeto=a.projeto and b.cod_apmp = a.id and b.tipo='APP_ARL'), 0) app_arl_area
                            from tmp_apmp a where a.projeto=:projeto order by 1";

				strSQL = strSQL.Replace("\r", "").Replace("\n", "");

				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);

					result["QUADRO_DE_AREAS"] = this.banco.ExecutarHashtable(comando);
				}


				strSQL = @"select 
                                p.empreendimento, 
                                (select a.texto from {0}lov_crt_projeto_geo_nivel a where a.id = p.nivel_precisao) precisao, 
                                (select a.sigla from {0}lov_estado a where a.id=t.estado) uf, 
                                (select a.texto from {0}lov_municipio a where a.id=t.municipio) municipio,
                                (select a.texto from {0}lov_caracterizacao_tipo a where a.id=p.caracterizacao) atividade,
                                (select a.id from {0}crt_projeto_geo a where a.empreendimento=p.empreendimento and a.caracterizacao=1) dominialidade,
                                (select round(a.geometry.sdo_point.x) ||';'|| round(a.geometry.sdo_point.y) from geo_emp_localizacao a where a.empreendimento=p.empreendimento) coordenada,
								(select sdo_geom.sdo_length(a.geometry, 0.0001) from tmp_atp a where a.projeto=p.id) atp_perimetro   
                            from 
                                {0}tmp_projeto_geo p, 
                                {0}tab_empreendimento_endereco t 
                            where 
                                p.id=:projeto and 
                                p.empreendimento=t.empreendimento(+) and
                                t.correspondencia(+)=0";

				strSQL = strSQL.Replace("\r", "").Replace("\n", "");
				strSQL = String.Format(strSQL, EsquemaOficialComPonto);

				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);

					using (IDataReader reader = banco.ExecutarReader(comando))
					{
						while (reader.Read())
						{
							result["EMPREENDIMENTO"] = reader["empreendimento"];
							result["PRECISAO"] = reader["precisao"];
							result["MUNICIPIO"] = (reader["municipio"] is DBNull) ? "" : reader["municipio"].ToString();
							result["ATIVIDADE"] = (reader["atividade"] is DBNull) ? "" : reader["atividade"].ToString();
							result["DOMINIALIDADE"] = reader["dominialidade"];
							result["UF"] = (reader["uf"] is DBNull) ? "" : reader["uf"].ToString();
							result["COORDENADA"] = reader["coordenada"];
							result["ATP_PERIMETRO"] = reader["ATP_PERIMETRO"];
						}

						reader.Close();
					}

				}
				#endregion
			}
			else if (ticketType == OperacoesGeoDa.OPERACAO_ATIVIDADE)
			{
				#region Atividade
				strSQL = @" select * from (select t.nome apmp_nome,
                                (case t.tipo when 'M' then 'Matrícula' when 'P' then 'Posse' else 'Desconhecido' end) apmp_tipo,
                                (select count(id) from tmp_pativ p where p.projeto=:projeto and p.cod_apmp=t.id) pativ_quantidade, 
                                (select nvl(sum(p.comprimento), 0) from tmp_lativ p where p.projeto=:projeto and p.cod_apmp=t.id) lativ_comprimento, 
                                (select nvl(sum(p.area_m2), 0) from tmp_aativ p where p.projeto=:projeto and p.cod_apmp=t.id) aativ_area_m2, 
                                (select nvl(sum(p.area_m2), 0) from tmp_aiativ p where p.projeto=:projeto and p.cod_apmp=t.id) aiativ_area_m2
                            from geo_apmp t, {0}crt_projeto_geo a, {0}tmp_projeto_geo b where t.projeto=a.id and a.empreendimento = b.empreendimento and b.id = :projeto order by 1)
                            union all
                            select * from(
                                select ' - ' apmp_nome, ' - ' apmp_tipo,
                                    (select count(id) from tmp_pativ p where p.projeto=:projeto and p.cod_apmp is null) pativ_quantidade, 
                                    (select nvl(sum(p.comprimento), 0) from tmp_lativ p where p.projeto=:projeto and p.cod_apmp is null) lativ_comprimento, 
                                    (select nvl(sum(p.area_m2), 0) from tmp_aativ p where p.projeto=:projeto and p.cod_apmp is null) aativ_area_m2, 
                                    (select nvl(sum(p.area_m2), 0) from tmp_aiativ p where p.projeto=:projeto and p.cod_apmp is null) aiativ_area_m2
                                from dual
                            ) where pativ_quantidade>0 or lativ_comprimento>0 or aativ_area_m2>0 or aiativ_area_m2>0";

				strSQL = strSQL.Replace("\r", "").Replace("\n", "");
				strSQL = String.Format(strSQL, EsquemaOficialComPonto);

				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);

					result["QUADRO_TOTAL"] = this.banco.ExecutarHashtable(comando);
				}

				//----------------------------------------------------------------------
				strSQL = @"select t.codigo, (select a.nome from geo_apmp a where a.id = t.cod_apmp) apmp_nome, t.rocha, t.massa_dagua, t.avn, t.aa, t.afs, t.rest_declividade, t.arl, t.rppn, t.app from tmp_pativ t where t.projeto=:projeto order by 1";
				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);

					result["QUADRO_PATIV"] = this.banco.ExecutarHashtable(comando);
				}

				//coordenadas
				strSQL = @"select t.column_value ordenada from table(select t.geometry.sdo_ordinates from tmp_pativ t where t.projeto=:projeto and t.codigo=:codigo) t";
				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);
					comando.AdicionarParametroEntrada("codigo", "-", DbType.String);
					foreach (Hashtable hs in (List<Hashtable>)result["QUADRO_PATIV"])
					{
						comando.SetarValorParametro("codigo", hs["CODIGO"]);
						hs["ORDENADAS"] = this.banco.ExecutarList<Decimal>(comando);
					}
				}
				//----------------------------------------------------------------------


				//----------------------------------------------------------------------
				strSQL = @"select t.codigo, (select a.nome from geo_apmp a where a.id = t.cod_apmp) apmp_nome, t.comprimento, t.rocha, t.massa_dagua, t.avn, t.aa, t.afs, t.rest_declividade, t.arl, t.rppn, t.app from tmp_lativ t where t.projeto=:projeto order by 1";
				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);

					result["QUADRO_LATIV"] = this.banco.ExecutarHashtable(comando);
				}

				//coordenadas
				strSQL = @"select t.column_value ordenada from table(select t.geometry.sdo_ordinates from tmp_lativ t where t.projeto=:projeto and t.codigo=:codigo) t";
				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);
					comando.AdicionarParametroEntrada("codigo", "-", DbType.String);
					foreach (Hashtable hs in (List<Hashtable>)result["QUADRO_LATIV"])
					{
						comando.SetarValorParametro("codigo", hs["CODIGO"]);
						hs["ORDENADAS"] = this.banco.ExecutarList<Decimal>(comando);
					}
				}
				//----------------------------------------------------------------------


				//----------------------------------------------------------------------
				strSQL = @"select t.codigo, (select a.nome from geo_apmp a where a.id = t.cod_apmp) apmp_nome, t.area_m2, t.rocha, t.massa_dagua, t.avn, t.aa, t.afs, t.rest_declividade, t.arl, t.rppn, t.app from tmp_aativ t where t.projeto=:projeto order by 1";
				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);

					result["QUADRO_AATIV"] = this.banco.ExecutarHashtable(comando);
				}

				//coordenadas
				strSQL = @"select t.column_value ordenada from table(select t.geometry.sdo_ordinates from tmp_aativ t where t.projeto=:projeto and t.codigo=:codigo) t";
				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);
					comando.AdicionarParametroEntrada("codigo", "-", DbType.String);
					foreach (Hashtable hs in (List<Hashtable>)result["QUADRO_AATIV"])
					{
						comando.SetarValorParametro("codigo", hs["CODIGO"]);
						hs["ORDENADAS"] = this.banco.ExecutarList<Decimal>(comando);
					}
				}
				//----------------------------------------------------------------------


				//----------------------------------------------------------------------
				strSQL = @"select t.codigo, (select a.nome from geo_apmp a where a.id = t.cod_apmp) apmp_nome, t.area_m2, t.rocha, t.massa_dagua, t.avn, t.aa, t.afs, t.rest_declividade, t.arl, t.rppn, t.app from tmp_aiativ t where t.projeto=:projeto order by 1";
				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);

					result["QUADRO_AIATIV"] = this.banco.ExecutarHashtable(comando);
				}

				//coordenadas
				strSQL = @"select t.column_value ordenada from table(select t.geometry.sdo_ordinates from tmp_aiativ t where t.projeto=:projeto and t.codigo=:codigo) t";
				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);
					comando.AdicionarParametroEntrada("codigo", "-", DbType.String);
					foreach (Hashtable hs in (List<Hashtable>)result["QUADRO_AIATIV"])
					{
						comando.SetarValorParametro("codigo", hs["CODIGO"]);
						hs["ORDENADAS"] = this.banco.ExecutarList<Decimal>(comando);
					}
				}
				//----------------------------------------------------------------------


				strSQL = @"select 
                                p.empreendimento, 
                                (select a.texto from {0}lov_crt_projeto_geo_nivel a where a.id = p.nivel_precisao) precisao, 
                                (select a.sigla from {0}lov_estado a where a.id=t.estado) uf, 
                                (select a.texto from {0}lov_municipio a where a.id=t.municipio) municipio,
                                (select a.texto from {0}lov_caracterizacao_tipo a where a.id=p.caracterizacao) atividade,
                                (select a.id from {0}crt_projeto_geo a where a.empreendimento=p.empreendimento and a.caracterizacao=1) dominialidade,
                                (select round(a.geometry.sdo_point.x) ||';'|| round(a.geometry.sdo_point.y) from geo_emp_localizacao a where a.empreendimento=p.empreendimento) coordenada,
								(select sdo_geom.sdo_length(a.geometry, 0.0001) from tmp_atp a where a.projeto=p.id) atp_perimetro  
                            from 
                                {0}tmp_projeto_geo p, 
                                {0}tab_empreendimento_endereco t 
                            where 
                                p.id=:projeto and 
                                p.empreendimento=t.empreendimento(+) and
                                t.correspondencia(+)=0";

				strSQL = strSQL.Replace("\r", "").Replace("\n", "");
				strSQL = String.Format(strSQL, EsquemaOficialComPonto);

				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);

					using (IDataReader reader = banco.ExecutarReader(comando))
					{
						while (reader.Read())
						{
							result["EMPREENDIMENTO"] = reader["empreendimento"];
							result["PRECISAO"] = reader["precisao"];
							result["MUNICIPIO"] = (reader["municipio"] is DBNull) ? "" : reader["municipio"].ToString();
							result["ATIVIDADE"] = (reader["atividade"] is DBNull) ? "" : reader["atividade"].ToString();
							result["DOMINIALIDADE"] = reader["dominialidade"];
							result["UF"] = (reader["uf"] is DBNull) ? "" : reader["uf"].ToString();
							result["COORDENADA"] = reader["coordenada"];
							result["ATP_PERIMETRO"] = reader["ATP_PERIMETRO"];
						}

						reader.Close();
					}

				}
				#endregion
			}
			else if (ticketType == OperacoesGeoDa.OPERACAO_FISCALIZACAO)
			{
				#region Fiscalizacao
				//----------------------------------------------------------------------
				strSQL = @"select t.codigo from tmp_fiscal_ponto t where t.projeto=:projeto order by 1";
				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);

					result["QUADRO_PONTO"] = this.banco.ExecutarHashtable(comando);
				}

				//coordenadas
				strSQL = @"select t.column_value ordenada from table(select t.geometry.sdo_ordinates from tmp_fiscal_ponto t where t.projeto=:projeto and t.codigo=:codigo) t";
				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);
					comando.AdicionarParametroEntrada("codigo", "-", DbType.String);
					foreach (Hashtable hs in (List<Hashtable>)result["QUADRO_PONTO"])
					{
						comando.SetarValorParametro("codigo", hs["CODIGO"]);
						hs["ORDENADAS"] = this.banco.ExecutarList<Decimal>(comando);
					}
				}
				//----------------------------------------------------------------------


				//----------------------------------------------------------------------
				strSQL = @"select t.codigo, t.comprimento from tmp_fiscal_linha t where t.projeto=:projeto order by 1";
				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);

					result["QUADRO_LINHA"] = this.banco.ExecutarHashtable(comando);
				}

				//coordenadas
				strSQL = @"select t.column_value ordenada from table(select t.geometry.sdo_ordinates from tmp_fiscal_linha t where t.projeto=:projeto and t.codigo=:codigo) t";
				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);
					comando.AdicionarParametroEntrada("codigo", "-", DbType.String);
					foreach (Hashtable hs in (List<Hashtable>)result["QUADRO_LINHA"])
					{
						comando.SetarValorParametro("codigo", hs["CODIGO"]);
						hs["ORDENADAS"] = this.banco.ExecutarList<Decimal>(comando);
					}
				}
				//----------------------------------------------------------------------


				//----------------------------------------------------------------------
				strSQL = @"select t.codigo, t.area_m2 from tmp_fiscal_area t where t.projeto=:projeto order by 1";
				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);

					result["QUADRO_AREA"] = this.banco.ExecutarHashtable(comando);
				}

				//coordenadas
				strSQL = @"select t.column_value ordenada from table(select t.geometry.sdo_ordinates from tmp_fiscal_area t where t.projeto=:projeto and t.codigo=:codigo) t";
				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);
					comando.AdicionarParametroEntrada("codigo", "-", DbType.String);
					foreach (Hashtable hs in (List<Hashtable>)result["QUADRO_AREA"])
					{
						comando.SetarValorParametro("codigo", hs["CODIGO"]);
						hs["ORDENADAS"] = this.banco.ExecutarList<Decimal>(comando);
					}
				}
				//----------------------------------------------------------------------


				strSQL = @"select 
                                p.fiscalizacao, 
                                (select a.texto from {0}lov_crt_projeto_geo_nivel a where a.id = p.nivel_precisao) precisao, 
                                (select a.sigla from {0}lov_estado a, {0}lov_municipio b where b.id=t.municipio and a.id=b.estado) uf, 
                                (select a.texto from {0}lov_municipio a where a.id=t.municipio) municipio,
                                (select count(*) from tmp_fiscal_ponto a where a.projeto=p.id) total_pontos,
                                (select count(*) from tmp_fiscal_linha a where a.projeto=p.id) total_linhas,
                                (select count(*) from tmp_fiscal_area a where a.projeto=p.id) total_areas,
                                (select nvl(sum(a.comprimento),0) from tmp_fiscal_linha a where a.projeto=p.id) soma_linhas,
                                (select nvl(sum(a.area_m2),0) from tmp_fiscal_area a where a.projeto=p.id) soma_areas,
                                gerarEnvelopeFiscalizacao(p.id) envelope 
                            from 
                                {0}tmp_projeto_geo p, 
                                {0}tab_fisc_local_infracao t 
                            where 
                                p.id=:projeto and 
                                p.fiscalizacao=t.fiscalizacao";

				strSQL = strSQL.Replace("\r", "").Replace("\n", "");
				strSQL = String.Format(strSQL, EsquemaOficialComPonto);

				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);

					using (IDataReader reader = banco.ExecutarReader(comando))
					{
						while (reader.Read())
						{
							result["FISCALIZACAO"] = reader["fiscalizacao"];
							result["PRECISAO"] = reader["precisao"];
							result["MUNICIPIO"] = (reader["municipio"] is DBNull) ? "" : reader["municipio"].ToString();
							result["UF"] = (reader["uf"] is DBNull) ? "" : reader["uf"].ToString();

							result["TOTAL_PONTOS"] = reader["total_pontos"];
							result["TOTAL_LINHAS"] = reader["total_linhas"];
							result["TOTAL_AREAS"] = reader["total_areas"];
							result["SOMA_LINHAS"] = reader["soma_linhas"];
							result["SOMA_AREAS"] = reader["soma_areas"];

							result["ENVELOPE"] = reader["ENVELOPE"];
						}

						reader.Close();
					}

				}
				#endregion
			}
			else if (ticketType == OperacoesGeoDa.OPERACAO_CAR)
			{
				int projetoDomId = 0;
				#region CAR

				strSQL = @"select g.id prj_dom_id, lm.texto municipio, cc.atp_qtd_modulo_fiscal, cc.atp_documento_2008
				 from syn_idaf_crt_projeto_geo g, syn_idaf_tmp_crt_car cc, syn_idaf_lov_municipio lm
					  where g.caracterizacao = 1 
					  and g.empreendimento = cc.empreendimento
				and g.empreendimento = (select f.empreendimento from tab_fila f where f.projeto = :projetoId and f.tipo = :fila_tipo )
					  and cc.municipio = lm.id";

				strSQL = strSQL.Replace("\r", "").Replace("\n", "");

				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projetoId", ticketID, DbType.Int32);
					comando.AdicionarParametroEntrada("fila_tipo", OperacoesGeoDa.OPERACAO_CAR, DbType.Int32);

					result["CAR"] = this.banco.ExecutarHashtable(comando).First();
				}

				projetoDomId = Convert.ToInt32(((Hashtable)result["CAR"])["PRJ_DOM_ID"]);


				strSQL = @"select 'ATP' classe, 'Área Total da Propriedade' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from geo_atp where projeto=:projetoDomId
                    union all select 'ACONSTRUIDA' classe, 'Área Construída' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from geo_aconstruida where projeto=:projetoDomId
                    union all select 'AFD' classe, 'Área de Faixa de Domínio' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from geo_afd where projeto=:projetoDomId
                    union all select 'APMP' classe, 'Área da Propriedade por Matrícula ou Posse' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from geo_apmp where projeto=:projetoDomId
                    union all select 'AFS' classe, 'Área de Faixa de Servidão' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from geo_afs where projeto=:projetoDomId
                    union all select 'ROCHA' classe, 'Área de rocha' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from geo_rocha where projeto=:projetoDomId
                    union all select 'MASSA_DAGUA' classe, 'Área de massa de água (represa, lagoa e curso de água)' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from geo_areas_calculadas where tipo='MASSA_DAGUA_APMP' and projeto=:projetoDomId
                    union all select 'RPPN' classe, 'Reserva Particular do Patrimônio Natural' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from geo_rppn where projeto=:projetoDomId
                    union all select 'AVN' classe, 'Total (Inicial + Médio + Avançado + Não caracterizado)' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from geo_avn where projeto=:projetoDomId
                    union all select * from (select classe, descricao, subtipo, sum(area_m2) area_m2 from(
                        select 'AVN' classe, 'Em Estágio Inicial de Regeneração' descricao, vegetacao subtipo, nvl(sum(area_m2),0) area_m2 from geo_avn where estagio='I' and projeto=:projetoDomId group by vegetacao
                        union all select 'AVN' classe, 'Em Estágio Inicial de Regeneração' descricao, null subtipo, 0 area_m2 from dual) group by classe, descricao, subtipo order by subtipo)
                    union all select * from (select classe, descricao, subtipo, sum(area_m2) area_m2 from(
                        select 'AVN' classe, 'Em Estágio Médio de Regeneração' descricao, vegetacao subtipo, nvl(sum(area_m2),0) area_m2 from geo_avn where estagio='M' and projeto=:projetoDomId group by vegetacao
                        union all select 'AVN' classe, 'Em Estágio Médio de Regeneração' descricao, null subtipo, 0 area_m2 from dual) group by classe, descricao, subtipo order by subtipo)
                    union all select * from (select classe, descricao, subtipo, sum(area_m2) area_m2 from(
                        select 'AVN' classe, 'Em Estágio Avançado de Regeneração' descricao, vegetacao subtipo, nvl(sum(area_m2),0) area_m2 from geo_avn where estagio='A' and projeto=:projetoDomId group by vegetacao
                        union all select 'AVN' classe, 'Em Estágio Avançado de Regeneração' descricao, null subtipo, 0 area_m2 from dual) group by classe, descricao, subtipo order by subtipo)
                    union all select * from (select classe, descricao, subtipo, sum(area_m2) area_m2 from(
                        select 'AVN' classe, 'Em Estágio de Regeneração não caracterizado' descricao, vegetacao subtipo, nvl(sum(area_m2),0) area_m2 from geo_avn where estagio='D' and projeto=:projetoDomId group by vegetacao
                        union all select 'AVN' classe, 'Em Estágio de Regeneração não caracterizado' descricao, null subtipo, 0 area_m2 from dual) group by classe, descricao, subtipo order by subtipo)
                    union all select 'AA' classe, 'Total (Em Recuperação + Em Uso + Não caracterizada)' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from geo_aa where projeto=:projetoDomId
                    union all select 'AA' classe, 'Em Recuperação' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from geo_aa where tipo='REC' and projeto=:projetoDomId
                    union all select * from (select classe, descricao, subtipo, sum(area_m2) area_m2 from(
                        select 'AA' classe, 'Em Uso' descricao, vegetacao subtipo, nvl(sum(area_m2),0) area_m2 from geo_aa where tipo='USO' and projeto=:projetoDomId group by vegetacao
                        union all select 'AA' classe, 'Em Uso' descricao, null subtipo, 0 area_m2 from dual) group by classe, descricao, subtipo order by subtipo)
                    union all select 'AA' classe, 'Não caracterizada' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from geo_aa where tipo='D' and projeto=:projetoDomId
                    union all select 'ARL' classe, 'Total (Preservada + Em Recuperação + Em Uso)' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from geo_arl where situacao <> 'D' and projeto=:projetoDomId
                    union all select 'ARL' classe, 'Preservada' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from geo_arl where situacao='PRESERV' and projeto=:projetoDomId
                    union all select 'ARL' classe, 'Em Recuperação' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from geo_arl where situacao='REC' and projeto=:projetoDomId
                    union all select 'ARL' classe, 'Em Uso' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from geo_arl where situacao='USO' and projeto=:projetoDomId
					union all select 'ARL' classe, 'Em área de preservação permanente' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from geo_areas_calculadas where tipo='APP_ARL' and projeto=:projetoDomId 
					union all select 'APP' classe, 'Total (Preservada + Em Recuperação + Em Uso)' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from geo_areas_calculadas where tipo='APP_APMP' and projeto=:projetoDomId
                    union all select 'APP' classe, 'Preservada' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from geo_areas_calculadas where tipo='APP_AVN' and projeto=:projetoDomId
                    union all select 'APP' classe, 'Em Recuperação' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from geo_areas_calculadas where tipo='APP_AA_REC' and projeto=:projetoDomId
                    union all select 'APP' classe, 'Em Uso' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from geo_areas_calculadas where tipo='APP_AA_USO' and projeto=:projetoDomId					
					union all select 'APP' classe, 'A Recuperar (Calculado)' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from tmp_car_areas_calculadas where tipo='CAR_APP_AA_USO' and projeto=:projetoId		
					union all select 'APP' classe, 'Com Uso Consolidado' descricao, '' subtipo,  
						nvl((select sum(a.area_m2) total from geo_areas_calculadas a where a.tipo ='APP_AA_USO' and a.projeto=:projetoDomId ),0) -
						nvl((select sum(a.area_m2) total from tmp_car_areas_calculadas a where a.tipo ='CAR_APP_AA_USO' and a.projeto=:projetoId ),0)
						area_m2 from dual  ";

				strSQL = strSQL.Replace("\r", "").Replace("\n", "");

				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projetoId", ticketID, DbType.Int32);
					comando.AdicionarParametroEntrada("projetoDomId", projetoDomId, DbType.Int32);

					result["QUADRO_TOTAL"] = this.banco.ExecutarHashtable(comando);
				}

				foreach (Hashtable hs in (List<Hashtable>)result["QUADRO_TOTAL"])
				{
					if (hs["CLASSE"].ToString() == "ARL")
					{
						result["AREA_ARL"] = hs["AREA_M2"];
						break;
					}
				}


				//Ordenadas GEO_ATP
				strSQL = @"select t.column_value ordenada from table(select t.geometry.sdo_ordinates from geo_atp t where t.projeto=:projetoDomId) t";

				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projetoDomId", projetoDomId, DbType.Int32);

					result["ORDENADAS_ATP"] = this.banco.ExecutarList<Decimal>(comando);
				}

				//Ordenadas GEO_ARL
				//----------------------------------------------------------------------
				strSQL = @"select t.id, t.codigo from geo_arl t where t.projeto=:projetoDomId order by t.codigo ";

				Dictionary<String, List<Decimal>> lstCoord = new Dictionary<string, List<decimal>>();

				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projetoDomId", projetoDomId, DbType.Int32);

					using (IDataReader reader = banco.ExecutarReader(comando))
					{
						while (reader.Read())
						{
							lstCoord.Add(reader["codigo"].ToString(), new List<decimal>());
						}

						reader.Close();
					}
				}

				//coordenadas
				strSQL = @"select t.column_value ordenada from table(select t.geometry.sdo_ordinates from geo_arl t where t.projeto=:projetoDomId and t.codigo=:codigo) t";
				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projetoDomId", projetoDomId, DbType.Int32);
					comando.AdicionarParametroEntrada("codigo", "-", DbType.String);

					lstCoord.Keys.ToList().ForEach(key =>
					{
						comando.SetarValorParametro("codigo", key);
						lstCoord[key] = this.banco.ExecutarList<Decimal>(comando);
					});
				}

				result["ORDENADAS_ARL"] = lstCoord;
				//----------------------------------------------------------------------


				strSQL = @"select
                                upper(a.nome) apmp_nome,
                                (case a.tipo when 'M' then 'Matrícula' when 'P' then 'Posse' else 'Desconhecido' end) apmp_tipo,
                                a.area_m2 apmp_area,
                                sdo_geom.sdo_length(a.geometry, 0.0001) apmp_perimeter,
                                nvl( (select sum( b.area_m2 ) from geo_afs b where b.projeto=a.projeto and b.cod_apmp = a.id), 0) afs_area,
                                nvl( (select sum( b.area_m2 ) from geo_rocha b where b.projeto=a.projeto and b.cod_apmp = a.id), 0) rocha_area,
                                nvl( (select sum( b.area_m2 ) from geo_areas_calculadas b where b.projeto=a.projeto and b.cod_apmp = a.id and b.tipo='MASSA_DAGUA_APMP'), 0) massa_dagua_area,
                                nvl( (select sum( b.area_m2 ) from geo_avn b where b.projeto=a.projeto and b.cod_apmp = a.id and b.estagio='I'), 0) avn_i_area,
                                nvl( (select sum( b.area_m2 ) from geo_avn b where b.projeto=a.projeto and b.cod_apmp = a.id and b.estagio='M'), 0) avn_m_area,
                                nvl( (select sum( b.area_m2 ) from geo_avn b where b.projeto=a.projeto and b.cod_apmp = a.id and b.estagio='A'), 0) avn_a_area,
                                nvl( (select sum( b.area_m2 ) from geo_avn b where b.projeto=a.projeto and b.cod_apmp = a.id and b.estagio='D'), 0) avn_d_area,
                                nvl( (select sum( b.area_m2 ) from geo_aa b where b.projeto=a.projeto and b.cod_apmp = a.id and b.tipo='REC'), 0) aa_rec_area,
                                nvl( (select sum( b.area_m2 ) from geo_aa b where b.projeto=a.projeto and b.cod_apmp = a.id and b.tipo='USO'), 0) aa_uso_area,
                                nvl( (select sum( b.area_m2 ) from geo_aa b where b.projeto=a.projeto and b.cod_apmp = a.id and b.tipo='D'), 0) aa_d_area,
                                nvl( (select sum( b.area_m2 ) from geo_arl b where b.projeto=a.projeto and b.cod_apmp = a.id and b.situacao='PRESERV'), 0) arl_preserv_area,
                                nvl( (select sum( b.area_m2 ) from geo_arl b where b.projeto=a.projeto and b.cod_apmp = a.id and b.situacao='REC'), 0) arl_rec_area,
                                nvl( (select sum( b.area_m2 ) from geo_arl b where b.projeto=a.projeto and b.cod_apmp = a.id and b.situacao='USO'), 0) arl_uso_area,
                                nvl( (select sum( b.area_m2 ) from geo_arl b where b.projeto=a.projeto and b.cod_apmp = a.id and b.situacao='D'), 0) arl_d_area,
                                nvl( (select sum( b.area_m2 ) from geo_rppn b where b.projeto=a.projeto and b.cod_apmp = a.id), 0) rppn_area,
                                nvl( (select sum( b.area_m2 ) from geo_areas_calculadas b where b.projeto=a.projeto and b.cod_apmp = a.id and b.tipo='APP_APMP'), 0) app_apmp_area,
                                nvl( (select sum( b.area_m2 ) from geo_areas_calculadas b where b.projeto=a.projeto and b.cod_apmp = a.id and b.tipo='APP_AVN'), 0) app_avn_area,
                                nvl( (select sum( b.area_m2 ) from geo_areas_calculadas b where b.projeto=a.projeto and b.cod_apmp = a.id and b.tipo='APP_AA_REC'), 0) app_aa_rec_area,
                                nvl( (select sum( b.area_m2 ) from geo_areas_calculadas b where b.projeto=a.projeto and b.cod_apmp = a.id and b.tipo='APP_AA_USO'), 0) app_aa_uso_area,
                                nvl( (select sum( b.area_m2 ) from geo_areas_calculadas b where b.projeto=a.projeto and b.cod_apmp = a.id and b.tipo='APP_ARL'), 0) app_arl_area, 
								nvl( (select sum( b.area_m2 ) from tmp_car_areas_calculadas b where b.projeto=:projetoId and b.cod_apmp = a.id and b.tipo='CAR_APP_AA_USO'), 0) car_app_aa_uso_area 
                            from geo_apmp a where a.projeto=:projetoDomId order by 1";

				strSQL = strSQL.Replace("\r", "").Replace("\n", "");

				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projetoId", ticketID, DbType.Int32);
					comando.AdicionarParametroEntrada("projetoDomId", projetoDomId, DbType.Int32);

					result["QUADRO_DE_AREAS"] = this.banco.ExecutarHashtable(comando);
				}


				strSQL = @"select 
                                p.empreendimento, 
                                (select a.texto from {0}lov_crt_projeto_geo_nivel a where a.id = p.nivel_precisao) precisao, 
                                (select a.sigla from {0}lov_estado a where a.id=t.estado) uf, 
                                (select a.texto from {0}lov_municipio a where a.id=t.municipio) municipio,
                                (select a.texto from {0}lov_caracterizacao_tipo a where a.id=p.caracterizacao) atividade,
                                (select a.id from {0}crt_projeto_geo a where a.empreendimento=p.empreendimento and a.caracterizacao=1) dominialidade,
                                (select round(a.geometry.sdo_point.x) ||';'|| round(a.geometry.sdo_point.y) from geo_emp_localizacao a where a.empreendimento=p.empreendimento) coordenada,
								(select a.area_m2 from geo_atp a where a.projeto=p.id) atp_croqui_m2,   
								(select sdo_geom.sdo_length(a.geometry, 0.0001) from geo_atp a where a.projeto=p.id) atp_perimetro   
                            from 
                                {0}crt_projeto_geo p, 
                                {0}tab_empreendimento_endereco t 
                            where 
                                p.id=:projetoDomId and 
                                p.empreendimento=t.empreendimento(+) and
                                t.correspondencia(+)=0";

				strSQL = strSQL.Replace("\r", "").Replace("\n", "");
				strSQL = String.Format(strSQL, EsquemaOficialComPonto);

				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projetoDomId", projetoDomId, DbType.Int32);

					using (IDataReader reader = banco.ExecutarReader(comando))
					{
						while (reader.Read())
						{
							result["EMPREENDIMENTO"] = reader["empreendimento"];
							result["PRECISAO"] = reader["precisao"];
							result["MUNICIPIO"] = (reader["municipio"] is DBNull) ? "" : reader["municipio"].ToString();
							result["ATIVIDADE"] = (reader["atividade"] is DBNull) ? "" : reader["atividade"].ToString();
							result["DOMINIALIDADE"] = reader["dominialidade"];
							result["UF"] = (reader["uf"] is DBNull) ? "" : reader["uf"].ToString();
							result["COORDENADA"] = reader["coordenada"];
							result["ATP_PERIMETRO"] = reader["ATP_PERIMETRO"];
							result["ATP_CROQUI_M2"] = reader["ATP_CROQUI_M2"];
						}

						reader.Close();
					}

				}
				#endregion
			}
			else if (ticketType == OperacoesGeoDa.OPERACAO_ATIVIDADE_TITULO)
			{
				#region Atividade por título

				int titulo;

				strSQL = @"select titulo from tab_fila where projeto = :projeto and tipo = :tipo and etapa = :etapa and data_fim is null and rownum = 1";

				strSQL = strSQL.Replace("\r", "").Replace("\n", "");
				strSQL = String.Format(strSQL, EsquemaOficialComPonto);

				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);
					comando.AdicionarParametroEntrada("tipo", ticketType, DbType.Int32);
					comando.AdicionarParametroEntrada("etapa", ticketStep, DbType.Int32);

					titulo = int.Parse(this.banco.ExecutarScalar(comando).ToString());
				}

				result["TITULO"] = titulo;

				strSQL = @"	select t.nome apmp_nome,
							(case t.tipo when 'M' then 'Matrícula'	when 'P' then 'Posse' else 'Desconhecido' end) apmp_tipo,
							(select count(id) from geo_pativ g
											inner join {0}CRT_EXP_FLORESTAL_GEO ge
												on ge.GEO_PATIV_ID = g.id
											inner join {0}TAB_TITULO_EXP_FLOR_EXP ted
												on ted.EXP_FLORESTAL_EXPLORACAO = ge.EXP_FLORESTAL_EXPLORACAO
											inner join {0}TAB_TITULO_EXP_FLORESTAL te
												on te.id = ted.TITULO_EXPLORACAO_FLORESTAL
										where g.projeto = :projeto and g.cod_apmp=t.id) pativ_quantidade,
							(select	nvl(sum(p.comprimento), 0) from	geo_lativ p	where p.projeto =:projeto and p.cod_apmp = t.id) lativ_comprimento,
							(select nvl(sum(g.area_m2), 0) from	geo_aativ g	
											inner join {0}CRT_EXP_FLORESTAL_GEO ge
												on ge.GEO_AATIV_ID = g.id 
												inner join IDAF.TAB_TITULO_EXP_FLOR_EXP ted
												on ted.EXP_FLORESTAL_EXPLORACAO = ge.EXP_FLORESTAL_EXPLORACAO
											inner join {0}TAB_TITULO_EXP_FLORESTAL te
												on te.id = ted.TITULO_EXPLORACAO_FLORESTAL
											where g.projeto =:projeto and g.cod_apmp=t.id) aativ_area_m2,
							(select	nvl(sum(p.area_m2), 0) from	geo_aiativ p where	p.projeto =:projeto	and p.cod_apmp = t.id) aiativ_area_m2
						from geo_apmp t,{0}crt_projeto_geo a,	{0}crt_projeto_geo b
						where t.projeto = a.id and a.empreendimento = b.empreendimento	and b.id = :projeto
						order by 1";

				strSQL = strSQL.Replace("\r", "").Replace("\n", "");
				strSQL = String.Format(strSQL, EsquemaOficialComPonto);

				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);

					result["QUADRO_TOTAL"] = this.banco.ExecutarHashtable(comando);
				}

				//----------------------------------------------------------------------
				strSQL = @"select t.codigo, (select a.nome from geo_apmp a where a.id = t.cod_apmp) apmp_nome, t.rocha, t.massa_dagua, 
							t.avn, t.aa, t.afs, t.rest_declividade, t.arl, t.rppn, t.app 
							from geo_pativ t 
							inner join {0}CRT_EXP_FLORESTAL_GEO ge
									on ge.GEO_PATIV_ID = t.id
							inner join {0}TAB_TITULO_EXP_FLOR_EXP ted
									on ted.EXP_FLORESTAL_EXPLORACAO = ge.EXP_FLORESTAL_EXPLORACAO
							inner join {0}TAB_TITULO_EXP_FLORESTAL te
									on te.id = ted.TITULO_EXPLORACAO_FLORESTAL
							where t.projeto=:projeto and te.titulo = :titulo order by 1";

				strSQL = strSQL.Replace("\r", "").Replace("\n", "");
				strSQL = String.Format(strSQL, EsquemaOficialComPonto);
				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);
					comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

					result["QUADRO_PATIV"] = this.banco.ExecutarHashtable(comando);
				}

				//coordenadas
				strSQL = @"select t.column_value ordenada from table(select t.geometry.sdo_ordinates from geo_pativ t where t.projeto=:projeto and t.codigo=:codigo) t";
				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);
					comando.AdicionarParametroEntrada("codigo", "-", DbType.String);
					foreach (Hashtable hs in (List<Hashtable>)result["QUADRO_PATIV"])
					{
						comando.SetarValorParametro("codigo", hs["CODIGO"]);
						hs["ORDENADAS"] = this.banco.ExecutarList<Decimal>(comando);
					}
				}
				//----------------------------------------------------------------------


				//----------------------------------------------------------------------
				strSQL = @"select t.codigo, (select a.nome from geo_apmp a where a.id = t.cod_apmp) apmp_nome, t.comprimento, t.rocha, t.massa_dagua, t.avn, t.aa, t.afs, t.rest_declividade, t.arl, t.rppn, t.app from geo_lativ t where t.projeto=:projeto order by 1";
				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);

					result["QUADRO_LATIV"] = this.banco.ExecutarHashtable(comando);
				}

				//coordenadas
				strSQL = @"select t.column_value ordenada from table(select t.geometry.sdo_ordinates from geo_lativ t where t.projeto=:projeto and t.codigo=:codigo) t";
				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);
					comando.AdicionarParametroEntrada("codigo", "-", DbType.String);
					foreach (Hashtable hs in (List<Hashtable>)result["QUADRO_LATIV"])
					{
						comando.SetarValorParametro("codigo", hs["CODIGO"]);
						hs["ORDENADAS"] = this.banco.ExecutarList<Decimal>(comando);
					}
				}
				//----------------------------------------------------------------------


				//----------------------------------------------------------------------
				strSQL = @"select t.codigo, (select a.nome from geo_apmp a where a.id = t.cod_apmp) apmp_nome, t.area_m2, t.rocha, 
								t.massa_dagua, t.avn, t.aa, t.afs, t.rest_declividade, t.arl, t.rppn, t.app 
								from geo_aativ t
								inner join {0}CRT_EXP_FLORESTAL_GEO ge
									on ge.GEO_AATIV_ID = t.id 
								inner join {0}TAB_TITULO_EXP_FLOR_EXP ted
									on ted.EXP_FLORESTAL_EXPLORACAO = ge.EXP_FLORESTAL_EXPLORACAO
								inner join {0}TAB_TITULO_EXP_FLORESTAL te
									on te.id = ted.TITULO_EXPLORACAO_FLORESTAL
							   where t.projeto= :projeto and te.titulo = :titulo order by 1";

				strSQL = strSQL.Replace("\r", "").Replace("\n", "");
				strSQL = String.Format(strSQL, EsquemaOficialComPonto);
				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);
					comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

					result["QUADRO_AATIV"] = this.banco.ExecutarHashtable(comando);
				}

				//coordenadas
				strSQL = @"select t.column_value ordenada from table(select t.geometry.sdo_ordinates from geo_aativ t where t.projeto=:projeto and t.codigo=:codigo) t";
				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);
					comando.AdicionarParametroEntrada("codigo", "-", DbType.String);
					foreach (Hashtable hs in (List<Hashtable>)result["QUADRO_AATIV"])
					{
						comando.SetarValorParametro("codigo", hs["CODIGO"]);
						hs["ORDENADAS"] = this.banco.ExecutarList<Decimal>(comando);
					}
				}
				//----------------------------------------------------------------------


				//----------------------------------------------------------------------
				strSQL = @"select t.codigo, (select a.nome from geo_apmp a where a.id = t.cod_apmp) apmp_nome, t.area_m2, t.rocha, t.massa_dagua, t.avn, t.aa, t.afs, t.rest_declividade, t.arl, t.rppn, t.app from geo_aiativ t where t.projeto=:projeto order by 1";
				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);

					result["QUADRO_AIATIV"] = this.banco.ExecutarHashtable(comando);
				}

				//coordenadas
				strSQL = @"select t.column_value ordenada from table(select t.geometry.sdo_ordinates from geo_aiativ t where t.projeto=:projeto and t.codigo=:codigo) t";
				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);
					comando.AdicionarParametroEntrada("codigo", "-", DbType.String);
					foreach (Hashtable hs in (List<Hashtable>)result["QUADRO_AIATIV"])
					{
						comando.SetarValorParametro("codigo", hs["CODIGO"]);
						hs["ORDENADAS"] = this.banco.ExecutarList<Decimal>(comando);
					}
				}
				//----------------------------------------------------------------------


				strSQL = @"select 
                                p.empreendimento, 
                                (select a.texto from {0}lov_crt_projeto_geo_nivel a where a.id = p.nivel_precisao) precisao, 
                                (select a.sigla from {0}lov_estado a where a.id=t.estado) uf, 
                                (select a.texto from {0}lov_municipio a where a.id=t.municipio) municipio,
                                (select a.texto from {0}lov_caracterizacao_tipo a where a.id=p.caracterizacao) atividade,
                                (select a.id from {0}crt_projeto_geo a where a.empreendimento=p.empreendimento and a.caracterizacao=1) dominialidade,
                                (select round(a.geometry.sdo_point.x) ||';'|| round(a.geometry.sdo_point.y) from geo_emp_localizacao a where a.empreendimento=p.empreendimento) coordenada,
								(select sdo_geom.sdo_length(a.geometry, 0.0001) from tmp_atp a where a.projeto=p.id) atp_perimetro  
                            from 
                                {0}crt_projeto_geo p, 
                                {0}tab_empreendimento_endereco t 
                            where 
                                p.id=:projeto and 
                                p.empreendimento=t.empreendimento(+) and
                                t.correspondencia(+)=0";

				strSQL = strSQL.Replace("\r", "").Replace("\n", "");
				strSQL = String.Format(strSQL, EsquemaOficialComPonto);

				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);

					using (IDataReader reader = banco.ExecutarReader(comando))
					{
						while (reader.Read())
						{
							result["EMPREENDIMENTO"] = reader["empreendimento"];
							result["PRECISAO"] = reader["precisao"];
							result["MUNICIPIO"] = (reader["municipio"] is DBNull) ? "" : reader["municipio"].ToString();
							result["ATIVIDADE"] = (reader["atividade"] is DBNull) ? "" : reader["atividade"].ToString();
							result["DOMINIALIDADE"] = reader["dominialidade"];
							result["UF"] = (reader["uf"] is DBNull) ? "" : reader["uf"].ToString();
							result["COORDENADA"] = reader["coordenada"];
							result["ATP_PERIMETRO"] = reader["ATP_PERIMETRO"];
						}

						reader.Close();
					}

				}

				strSQL = @"select id from idafgeo.geo_pativ g where exists
							(
								select 1 from idaf.CRT_EXP_FLORESTAL_GEO ge
								where ge.GEO_PATIV_ID = g.id
								and exists
								(
									select 1 from idaf.TAB_TITULO_EXP_FLOR_EXP ted
									where ted.EXP_FLORESTAL_EXPLORACAO = ge.EXP_FLORESTAL_EXPLORACAO
									and exists
									(
										select 1 from idaf.TAB_TITULO_EXP_FLORESTAL te
										where te.id = ted.TITULO_EXPLORACAO_FLORESTAL
										and te.TITULO = :titulo
									)
								)
							)";

				strSQL = strSQL.Replace("\r", "").Replace("\n", "");
				strSQL = String.Format(strSQL, EsquemaOficialComPonto);

				ArrayList idsList = new ArrayList();

				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

					using (IDataReader reader = banco.ExecutarReader(comando))
					{
						while (reader.Read())
						{
							idsList.Add(reader["id"]);
						}

						if (idsList.Count > 0)
							result["PATIV"] = string.Join(",", idsList.ToArray());

						reader.Close();
					}
				}

				strSQL = @"select id from idafgeo.geo_aativ g where exists
							(
								select 1 from idaf.CRT_EXP_FLORESTAL_GEO ge
								where ge.GEO_AATIV_ID = g.id
								and exists
								(
									select 1 from idaf.TAB_TITULO_EXP_FLOR_EXP ted
									where ted.EXP_FLORESTAL_EXPLORACAO = ge.EXP_FLORESTAL_EXPLORACAO
									and exists
									(
										select 1 from idaf.TAB_TITULO_EXP_FLORESTAL te
										where te.id = ted.TITULO_EXPLORACAO_FLORESTAL
										and te.TITULO = :titulo
									)
								)
							)";

				strSQL = strSQL.Replace("\r", "").Replace("\n", "");
				strSQL = String.Format(strSQL, EsquemaOficialComPonto);

				idsList = new ArrayList();

				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

					using (IDataReader reader = banco.ExecutarReader(comando))
					{
						while (reader.Read())
						{
							idsList.Add(reader["id"]);
						}

						if (idsList.Count > 0)
							result["AATIV"] = string.Join(",", idsList.ToArray());

						reader.Close();
					}
				}

				#endregion
			}
			else if (ticketType == OperacoesGeoDa.OPERACAO_REGULARIZACAO)
			{
				#region CADASTRO_PROPRIEDADE

				strSQL = @"select 'ATP' classe, 'Área Total da Propriedade' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from tmp_atp where projeto=:projeto                    
                    union all select 'APMP' classe, 'Área da Propriedade por Matrícula ou Posse' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from tmp_apmp where projeto=:projeto";


				strSQL = strSQL.Replace("\r", "").Replace("\n", "");

				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);

					result["QUADRO_TOTAL"] = this.banco.ExecutarHashtable(comando);
				}

				//foreach (Hashtable hs in (List<Hashtable>)result["QUADRO_TOTAL"])
				//{
				//	if (hs["CLASSE"].ToString() == "ARL")
				//	{
				//		result["AREA_ARL"] = hs["AREA_M2"];
				//		break;
				//	}
				//}


				//Ordenadas TMP_ATP
				strSQL = @"select t.column_value ordenada from table(select t.geometry.sdo_ordinates from tmp_atp t where t.projeto=:projeto) t";

				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);

					result["ORDENADAS_ATP"] = this.banco.ExecutarList<Decimal>(comando);
				}

				strSQL = @"select t.codigo, (select a.nome from geo_apmp a where a.id = t.cod_apmp) apmp_nome, t.area_m2, t.rocha, t.massa_dagua, t.avn, t.aa, t.afs, t.rest_declividade, t.arl, t.rppn, t.app from tmp_aativ t where t.projeto=:projeto order by 1";
				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);

					result["QUADRO_AATIV"] = this.banco.ExecutarHashtable(comando);
				}

				//coordenadas
				strSQL = @"select t.column_value ordenada from table(select t.geometry.sdo_ordinates from tmp_aativ t where t.projeto=:projeto and t.codigo=:codigo) t";
				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);
					comando.AdicionarParametroEntrada("codigo", "-", DbType.String);
					foreach (Hashtable hs in (List<Hashtable>)result["QUADRO_AATIV"])
					{
						comando.SetarValorParametro("codigo", hs["CODIGO"]);
						hs["ORDENADAS"] = this.banco.ExecutarList<Decimal>(comando);
					}
				}

				//strSQL = @"select
				//                            upper(a.nome) apmp_nome,
				//                            (case a.tipo when 'M' then 'Matrícula' when 'P' then 'Posse' else 'Desconhecido' end) apmp_tipo,
				//                            a.area_m2 apmp_area,
				//                            sdo_geom.sdo_length(a.geometry, 0.0001) apmp_perimeter,
				//                            nvl( (select sum( b.area_m2 ) from tmp_afs b where b.projeto=a.projeto and b.cod_apmp = a.id), 0) afs_area,
				//                            nvl( (select sum( b.area_m2 ) from tmp_rocha b where b.projeto=a.projeto and b.cod_apmp = a.id), 0) rocha_area,
				//                            nvl( (select sum( b.area_m2 ) from tmp_areas_calculadas b where b.projeto=a.projeto and b.cod_apmp = a.id and b.tipo='MASSA_DAGUA_APMP'), 0) massa_dagua_area,
				//                            nvl( (select sum( b.area_m2 ) from tmp_avn b where b.projeto=a.projeto and b.cod_apmp = a.id and b.estagio='I'), 0) avn_i_area,
				//                            nvl( (select sum( b.area_m2 ) from tmp_avn b where b.projeto=a.projeto and b.cod_apmp = a.id and b.estagio='M'), 0) avn_m_area,
				//                            nvl( (select sum( b.area_m2 ) from tmp_avn b where b.projeto=a.projeto and b.cod_apmp = a.id and b.estagio='A'), 0) avn_a_area,
				//                            nvl( (select sum( b.area_m2 ) from tmp_avn b where b.projeto=a.projeto and b.cod_apmp = a.id and b.estagio='D'), 0) avn_d_area,
				//                            nvl( (select sum( b.area_m2 ) from tmp_aa b where b.projeto=a.projeto and b.cod_apmp = a.id and b.tipo='REC'), 0) aa_rec_area,
				//                            nvl( (select sum( b.area_m2 ) from tmp_aa b where b.projeto=a.projeto and b.cod_apmp = a.id and b.tipo='USO'), 0) aa_uso_area,
				//                            nvl( (select sum( b.area_m2 ) from tmp_aa b where b.projeto=a.projeto and b.cod_apmp = a.id and b.tipo='D'), 0) aa_d_area,
				//                            nvl( (select sum( b.area_m2 ) from tmp_arl b where b.projeto=a.projeto and b.cod_apmp = a.id and b.situacao='PRESERV'), 0) arl_preserv_area,
				//                            nvl( (select sum( b.area_m2 ) from tmp_arl b where b.projeto=a.projeto and b.cod_apmp = a.id and b.situacao='REC'), 0) arl_rec_area,
				//                            nvl( (select sum( b.area_m2 ) from tmp_arl b where b.projeto=a.projeto and b.cod_apmp = a.id and b.situacao='USO'), 0) arl_uso_area,
				//                            nvl( (select sum( b.area_m2 ) from tmp_arl b where b.projeto=a.projeto and b.cod_apmp = a.id and b.situacao='D'), 0) arl_d_area,
				//                            nvl( (select sum( b.area_m2 ) from tmp_rppn b where b.projeto=a.projeto and b.cod_apmp = a.id), 0) rppn_area,
				//                            nvl( (select sum( b.area_m2 ) from tmp_areas_calculadas b where b.projeto=a.projeto and b.cod_apmp = a.id and b.tipo='APP_APMP'), 0) app_apmp_area,
				//                            nvl( (select sum( b.area_m2 ) from tmp_areas_calculadas b where b.projeto=a.projeto and b.cod_apmp = a.id and b.tipo='APP_AVN'), 0) app_avn_area,
				//                            nvl( (select sum( b.area_m2 ) from tmp_areas_calculadas b where b.projeto=a.projeto and b.cod_apmp = a.id and b.tipo='APP_AA_REC'), 0) app_aa_rec_area,
				//                            nvl( (select sum( b.area_m2 ) from tmp_areas_calculadas b where b.projeto=a.projeto and b.cod_apmp = a.id and b.tipo='APP_AA_USO'), 0) app_aa_uso_area,
				//                            nvl( (select sum( b.area_m2 ) from tmp_areas_calculadas b where b.projeto=a.projeto and b.cod_apmp = a.id and b.tipo='APP_ARL'), 0) app_arl_area
				//                        from tmp_apmp a where a.projeto=:projeto order by 1";

				//strSQL = strSQL.Replace("\r", "").Replace("\n", "");

				//using (Comando comando = this.banco.CriarComando(strSQL))
				//{
				//	comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);

				//	result["QUADRO_DE_AREAS"] = this.banco.ExecutarHashtable(comando);
				//}


				strSQL = @"select 
                                p.empreendimento, 
                                (select a.texto from {0}lov_crt_projeto_geo_nivel a where a.id = p.nivel_precisao) precisao, 
                                (select a.sigla from {0}lov_estado a where a.id=t.estado) uf, 
                                (select a.texto from {0}lov_municipio a where a.id=t.municipio) municipio,
                                (select a.texto from {0}lov_caracterizacao_tipo a where a.id=p.caracterizacao) atividade,
                                (select a.id from {0}crt_projeto_geo a where a.empreendimento=p.empreendimento and a.caracterizacao=1) dominialidade,
                                (select round(a.geometry.sdo_point.x) ||';'|| round(a.geometry.sdo_point.y) from geo_emp_localizacao a where a.empreendimento=p.empreendimento) coordenada,
								(select sdo_geom.sdo_length(a.geometry, 0.0001) from tmp_atp a where a.projeto=p.id) atp_perimetro   
                            from 
                                {0}tmp_projeto_geo p, 
                                {0}tab_empreendimento_endereco t 
                            where 
                                p.id=:projeto and 
                                p.empreendimento=t.empreendimento(+) and
                                t.correspondencia(+)=0";

				strSQL = strSQL.Replace("\r", "").Replace("\n", "");
				strSQL = String.Format(strSQL, EsquemaOficialComPonto);

				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);

					using (IDataReader reader = banco.ExecutarReader(comando))
					{
						while (reader.Read())
						{
							result["EMPREENDIMENTO"] = reader["empreendimento"];
							result["PRECISAO"] = reader["precisao"];
							result["MUNICIPIO"] = (reader["municipio"] is DBNull) ? "" : reader["municipio"].ToString();
							result["ATIVIDADE"] = (reader["atividade"] is DBNull) ? "" : reader["atividade"].ToString();
							result["DOMINIALIDADE"] = reader["dominialidade"];
							result["UF"] = (reader["uf"] is DBNull) ? "" : reader["uf"].ToString();
							result["COORDENADA"] = reader["coordenada"];
							result["ATP_PERIMETRO"] = reader["ATP_PERIMETRO"];
						}

						reader.Close();
					}

				}
				#endregion
			}
			return result;
		}
	}
}