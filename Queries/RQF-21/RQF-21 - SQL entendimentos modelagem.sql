select m.IBGE, crt.PROPRIEDADE_CODIGO, 
  m.IBGE || crt.PROPRIEDADE_CODIGO as novo_codigo
from
  CRT_UNIDADE_PRODUCAO crt
    inner join TAB_EMPREENDIMENTO emp
      on crt.EMPREENDIMENTO = emp.ID
    inner join TAB_EMPREENDIMENTO_ENDERECO en
      on en.EMPREENDIMENTO = emp.id
    inner join LOV_MUNICIPIO m
      on m.ID = en.MUNICIPIO
;
select unid.ID, unid.CODIGO_UP, 
  count(distinct coord.MUNICIPIO) as mun
from
  CRT_UNIDADE_PRODUCAO_UNIDADE unid
  join CRT_UNIDADE_PRODUCAO_UN_COORD coord
    on unid.ID = coord.UNIDADE_PRODUCAO_UNIDADE
group by unid.ID, unid.CODIGO_UP
having count(distinct coord.MUNICIPIO) > 1
;

 