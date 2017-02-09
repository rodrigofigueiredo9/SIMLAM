select 12345 || lpad(67, 4, '0') from dual;
select 12345 || lpad(67654321, 4, '0') from dual;
select campo, substr(campo, 1, length(campo) -2) ||'00'||substr(campo, -2, 2) as transform from (select 32045002916 as campo from dual) tcampo;
----------------------------------------------------------------------------------------------------------
select m.IBGE, crt.PROPRIEDADE_CODIGO, m.IBGE || lpad(crt.PROPRIEDADE_CODIGO, 4, '0') as novo_codigo, length(crt.PROPRIEDADE_CODIGO) as tamanho
-- select emp.*, '||||' as separador, en.*
-- select count(1), min(length(crt.PROPRIEDADE_CODIGO)) as min_tamanho, max(length(crt.PROPRIEDADE_CODIGO)) as max_tamanho
-- select count(distinct crt.id) qtde_crt_UP, count(distinct emp.id) qtde_empreend, count(distinct en.id) qtde_end_empreend, count(distinct en.municipio) qtde_mun_end_empreend, count(distinct m.id) qtde_municipio 
from
  CRT_UNIDADE_PRODUCAO crt
    inner join TAB_EMPREENDIMENTO emp
      on crt.EMPREENDIMENTO = emp.ID
    inner join TAB_EMPREENDIMENTO_ENDERECO en
      on en.EMPREENDIMENTO = emp.id
    inner join LOV_MUNICIPIO m
      on m.ID = en.MUNICIPIO
where en.CORRESPONDENCIA = 0
--and length(crt.PROPRIEDADE_CODIGO) <= 4
--order by crt.PROPRIEDADE_CODIGO asc
;
/*
-- fazendo GROUP BY de parte da query acima
-- caminho: CRT_UNIDADE_PRODUCAO x TAB_EMPREENDIMENTO x TAB_EMPREENDIMENTO_ENDERECO
select crt.ID as UP, crt.PROPRIEDADE_CODIGO, emp.ID as Empreend, count(distinct en.MUNICIPIO) as mun, min(en.municipio) as mun_1, max(en.municipio) as mun_ult
from
  CRT_UNIDADE_PRODUCAO crt
    inner join TAB_EMPREENDIMENTO emp
      on crt.EMPREENDIMENTO = emp.ID
    inner join TAB_EMPREENDIMENTO_ENDERECO en
      on en.EMPREENDIMENTO = emp.id
group by crt.ID, crt.PROPRIEDADE_CODIGO, emp.ID
having count(distinct en.MUNICIPIO) > 1
;*/
/*
-- caminho (que n�o tem a ver com o escopo): CRT_UNIDADE_PRODUCAO_UNIDADE x CRT_UNIDADE_PRODUCAO_UN_COORD
select unid.ID, unid.CODIGO_UP, count(distinct coord.MUNICIPIO) as mun
from
  CRT_UNIDADE_PRODUCAO_UNIDADE unid
  join CRT_UNIDADE_PRODUCAO_UN_COORD coord
    on unid.ID = coord.UNIDADE_PRODUCAO_UNIDADE
group by unid.ID, unid.CODIGO_UP
having count(distinct coord.MUNICIPIO) > 1
;*/

----------------------------------------------------------------------------------------------------------

select m.IBGE, hst_crt.PROPRIEDADE_CODIGO, m.IBGE || lpad(hst_crt.PROPRIEDADE_CODIGO, 4, '0') as novo_codigo, length(hst_crt.PROPRIEDADE_CODIGO) as tamanho
-- select emp.*, '||||' as separador, en.*
-- select count(1), min(length(hst_crt.PROPRIEDADE_CODIGO)) as min_tamanho, max(length(hst_crt.PROPRIEDADE_CODIGO)) as max_tamanho
-- select count(distinct hst_crt.id) qtde_hst_crt_UP, count(distinct emp.id) qtde_empreend, count(distinct en.id) qtde_end_empreend, count(distinct en.municipio) qtde_mun_end_empreend, count(distinct m.id) qtde_municipio 
from
  HST_CRT_UNIDADE_PRODUCAO hst_crt
    inner join TAB_EMPREENDIMENTO emp
      on hst_crt.EMPREENDIMENTO_ID = emp.ID
    inner join TAB_EMPREENDIMENTO_ENDERECO en
      on en.EMPREENDIMENTO = emp.id
    inner join LOV_MUNICIPIO m
      on m.ID = en.MUNICIPIO
where en.CORRESPONDENCIA = 0
--and length(hst_crt.PROPRIEDADE_CODIGO) <= 4
--order by hst_crt.PROPRIEDADE_CODIGO asc
;
----------------------------------------------------------------------------------------------------------
select unid.CODIGO_UP, substr(unid.CODIGO_UP, 1, length(unid.CODIGO_UP) -2) ||'00'||substr(unid.CODIGO_UP, -2, 2) as novo_codigo, length(unid.CODIGO_UP) as tamanho
-- select count(1), min(length(unid.CODIGO_UP)) as min_tamanho, max(length(unid.CODIGO_UP)) as max_tamanho
from CRT_UNIDADE_PRODUCAO_UNIDADE unid
--where length(unid.CODIGO_UP) <= 15
order by unid.CODIGO_UP asc
;
----------------------------------------------------------------------------------------------------------
select hst_unid.CODIGO_UP, substr(hst_unid.CODIGO_UP, 1, length(hst_unid.CODIGO_UP) -2) ||'00'||substr(hst_unid.CODIGO_UP, -2, 2) as novo_codigo, length(hst_unid.CODIGO_UP) as tamanho
-- select count(1), min(length(hst_unid.CODIGO_UP)) as min_tamanho, max(length(hst_unid.CODIGO_UP)) as max_tamanho
from HST_CRT_UNIDADE_PROD_UNIDADE hst_unid
--where length(hst_unid.CODIGO_UP) <= 15
order by hst_unid.CODIGO_UP asc
;


----------------------------------------------------------------------------------------------------------
-- Conferindo UPs com Munic�pios diferentes dos Empreendimentos:
-- Obs.: tem que conferir o subtstr() dos c�digos, e tamb�m os munic�pios cadastrados para identificarmos erros.
select 
  '--- EMPRENDIMENTO: ---' AS SEPARADOR,
  emp.CODIGO, 
  --emp.CNPJ, 
  emp.DENOMINADOR,
  --emp.NOME_FANTASIA,
  '--- PROPRIEDADE: ---' AS SEPARADOR,
  crt.PROPRIEDADE_CODIGO, (select m.TEXTO from LOV_MUNICIPIO where IBGE = substr(crt.PROPRIEDADE_CODIGO, 1, 7)) as Mun_Substr_Prop,
  m.TEXTO as Mun_CADAST_PRO, m.IBGE as IBGE_CADAST_PROP,
  '--- UP: ---' AS SEPARADOR,
  unid.CODIGO_UP, (select m.TEXTO from LOV_MUNICIPIO where IBGE = substr(unid.CODIGO_UP, 1, 7)) as Mun_Substr_UP,
  m2.TEXTO as Mun_CADAST_UP, m2.IBGE as IBGE_CADAST_UP
from
  CRT_UNIDADE_PRODUCAO crt
    inner join TAB_EMPREENDIMENTO emp
      on crt.EMPREENDIMENTO = emp.ID
    inner join TAB_EMPREENDIMENTO_ENDERECO en
      on en.EMPREENDIMENTO = emp.ID
    inner join LOV_MUNICIPIO m
      on m.ID = en.MUNICIPIO
    inner join CRT_UNIDADE_PRODUCAO_UNIDADE unid
      on unid.UNIDADE_PRODUCAO = crt.ID
    inner join CRT_UNIDADE_PRODUCAO_UN_COORD coord
      on unid.ID = coord.UNIDADE_PRODUCAO_UNIDADE
    inner join LOV_MUNICIPIO m2
      on coord.MUNICIPIO = m2.ID
where en.CORRESPONDENCIA = 0
--and crt.PROPRIEDADE_CODIGO in(32019022929, 32031301712)
and substr(crt.PROPRIEDADE_CODIGO, 1, 7) <> substr(unid.CODIGO_UP, 1, 7)
order by crt.PROPRIEDADE_CODIGO asc, unid.CODIGO_UP
;

-----
-- public Endereco ObterEndereco(int empreendimentoId, BancoDeDados banco = null)
-- Credenciado
select (select to_char(m.id) ||'-'|| m.texto ||'-'|| m.ibge  from lov_municipio m where m.id = lm.id) as IBGE, te.id, te.empreendimento, te.correspondencia, te.zona, te.cep, te.logradouro, te.bairro, le.id estado_id, le.texto estado_texto,
				lm.id municipio_id, lm.texto municipio_texto, te.numero, te.distrito, te.corrego, te.caixa_postal, te.complemento, te.tid
				from tab_empreendimento_endereco te, lov_estado le, lov_municipio lm
				where te.estado = le.id(+) and te.municipio = lm.id(+) and te.empreendimento = 44827 /*:empreendimento*/ and te.correspondencia = 0
;

-- novo SQL:
select m.IBGE
from
TAB_EMPREENDIMENTO_ENDERECO en
    inner join LOV_MUNICIPIO m
      on m.ID = en.MUNICIPIO
where en.CORRESPONDENCIA = 0
and en.EMPREENDIMENTO = 44827
;


--- Analisando novos
select * from CRT_UNIDADE_PRODUCAO_UNIDADE where codigo_up = 32029000822110004;
select * from CRT_UNIDADE_PRODUCAO where id in (select UNIDADE_PRODUCAO from CRT_UNIDADE_PRODUCAO_UNIDADE where codigo_up = 32029000822110004);
--
select * from CRT_UNIDADE_PRODUCAO where id = 38235;
select * from CRT_UNIDADE_PRODUCAO where PROPRIEDADE_CODIGO = 32031631601;