----------------------------------------------------------------------------------------------------------
  select 
    emp.id as ID_Empreend,
    emp.codigo as Cod_Empreend,
    crt.ID as ID_Caract_UP,
    crt.PROPRIEDADE_CODIGO as Cod_Propriedade, 
    m.IBGE || lpad(crt.PROPRIEDADE_CODIGO, 4, '0') as NOVO_CODIGO,
    m.TEXTO as Municipio
  from 
    CRT_UNIDADE_PRODUCAO crt
      inner join TAB_EMPREENDIMENTO emp
        on crt.EMPREENDIMENTO = emp.ID
      inner join TAB_EMPREENDIMENTO_ENDERECO en
        on en.EMPREENDIMENTO = emp.id
      inner join LOV_MUNICIPIO m
        on m.ID = en.MUNICIPIO
  where en.CORRESPONDENCIA = 0
    and length(crt.PROPRIEDADE_CODIGO) <= 4
;

----------------------------------------------------------------------------------------------------------
  select
    emp.id as ID_Empreend,
    emp.codigo as Cod_Empreend,
    crt.ID as ID_Caract_UP,
    crt.PROPRIEDADE_CODIGO as Cod_Propriedade,
    unid.ID as ID_UP, 
    unid.CODIGO_UP, 
    substr(unid.CODIGO_UP, 1, length(unid.CODIGO_UP) -2) ||'00'||substr(unid.CODIGO_UP, -2, 2) as NOVO_CODIGO
  from 
    CRT_UNIDADE_PRODUCAO_UNIDADE unid 
      inner join CRT_UNIDADE_PRODUCAO crt
        on unid.UNIDADE_PRODUCAO = crt.ID
      inner join TAB_EMPREENDIMENTO emp
        on crt.EMPREENDIMENTO = emp.ID
  where length(unid.CODIGO_UP) <= 15
;

----------------------------------------------------------------------------------------------------------
