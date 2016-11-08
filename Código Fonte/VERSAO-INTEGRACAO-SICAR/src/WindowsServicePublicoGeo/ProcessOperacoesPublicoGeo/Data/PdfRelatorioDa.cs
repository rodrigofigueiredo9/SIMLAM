﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;

namespace Tecnomapas.EtramiteX.WindowsService.ProcessOperacoesPublicoGeo.Data
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

			string strSQL = @"
			begin 
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

		internal Hashtable BuscarDadosPDF(int ticketID, int ticketType)
		{
			Hashtable result = new Hashtable();
			string strSQL;

			if (ticketType == OperacoesGeoDa.OPERACAO_CADASTRO_PROPRIEDADE)
			{
				strSQL = @"
				select 'ATP' classe, 'Área Total da Propriedade' descricao, '' subtipo, nvl(sum(area_m2),0) area_m2 from tmp_atp where projeto=:projeto
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
				//--------------------------------------------------------------------

				strSQL = @"
				select 
					upper(a.nome) apmp_nome,
					(case a.tipo when 'M' then 'Matrícula' when 'P' then 'Posse' else 'Desconhecido' end) apmp_tipo,
					a.area_m2 apmp_area,
					sdo_geom.sdo_length(a.geometry, 0.0001) apmp_perimeter,
					nvl( (select sum(b.area_m2) from tmp_afs b where b.cod_apmp = a.id), 0) afs_area,
					nvl( (select sum(b.area_m2) from tmp_rocha b where b.cod_apmp = a.id), 0) rocha_area,
					nvl( (select sum(b.area_m2) from tmp_areas_calculadas b where b.cod_apmp = a.id and b.tipo='MASSA_DAGUA_APMP'), 0) massa_dagua_area,
					nvl( (select sum(b.area_m2) from tmp_avn b where b.cod_apmp = a.id and b.estagio='I'), 0) avn_i_area,
					nvl( (select sum(b.area_m2) from tmp_avn b where b.cod_apmp = a.id and b.estagio='M'), 0) avn_m_area,
					nvl( (select sum(b.area_m2) from tmp_avn b where b.cod_apmp = a.id and b.estagio='A'), 0) avn_a_area,
					nvl( (select sum(b.area_m2) from tmp_avn b where b.cod_apmp = a.id and b.estagio='D'), 0) avn_d_area,
					nvl( (select sum(b.area_m2) from tmp_aa b where b.cod_apmp = a.id and b.tipo='REC'), 0) aa_rec_area,
					nvl( (select sum(b.area_m2) from tmp_aa b where b.cod_apmp = a.id and b.tipo='USO'), 0) aa_uso_area,
					nvl( (select sum(b.area_m2) from tmp_aa b where b.cod_apmp = a.id and b.tipo='D'), 0) aa_d_area,
					nvl( (select sum(b.area_m2) from tmp_arl b where b.cod_apmp = a.id and b.situacao='PRESERV'), 0) arl_preserv_area,
					nvl( (select sum(b.area_m2) from tmp_arl b where b.cod_apmp = a.id and b.situacao='REC'), 0) arl_rec_area,
					nvl( (select sum(b.area_m2) from tmp_arl b where b.cod_apmp = a.id and b.situacao='USO'), 0) arl_uso_area,
					nvl( (select sum(b.area_m2) from tmp_arl b where b.cod_apmp = a.id and b.situacao='D'), 0) arl_d_area,
					nvl( (select sum(b.area_m2) from tmp_rppn b where b.cod_apmp = a.id), 0) rppn_area,
					nvl( (select sum(b.area_m2) from tmp_areas_calculadas b where b.cod_apmp = a.id and b.tipo='APP_APMP'), 0) app_apmp_area,
					nvl( (select sum(b.area_m2) from tmp_areas_calculadas b where b.cod_apmp = a.id and b.tipo='APP_AVN'), 0) app_avn_area,
					nvl( (select sum(b.area_m2) from tmp_areas_calculadas b where b.cod_apmp = a.id and b.tipo='APP_AA_REC'), 0) app_aa_rec_area,
					nvl( (select sum(b.area_m2) from tmp_areas_calculadas b where b.cod_apmp = a.id and b.tipo='APP_AA_USO'), 0) app_aa_uso_area,
					nvl( (select sum(b.area_m2) from tmp_areas_calculadas b where b.cod_apmp = a.id and b.tipo='APP_ARL'), 0) app_arl_area
				from tmp_apmp a where a.projeto=:projeto order by 1";

				strSQL = strSQL.Replace("\r", "").Replace("\n", "");

				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);

					result["QUADRO_DE_AREAS"] = this.banco.ExecutarHashtable(comando);
				}

			}
			else if (ticketType == OperacoesGeoDa.OPERACAO_ATIVIDADE)
			{
				strSQL = @"
				select * from (select t.nome apmp_nome,
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
			}

			strSQL = @"
			select 
				0 empreendimento, 
				' ' precisao, 
				' ' uf, 
				' ' municipio,
				' ' atividade,
				p.id dominialidade,
				(select round(a.geometry.sdo_point.x) ||';'|| round(a.geometry.sdo_point.y) from geo_emp_localizacao a where a.projeto=p.id) coordenada 
			from 
				{0}tmp_projeto_geo p 
			where 
				p.id=:projeto";

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
					}

					reader.Close();
				}
			}

			return result;
		}
	}
}