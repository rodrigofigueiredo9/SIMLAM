BEGIN

  savepoint RQF_21_savepoint;

  begin
      ----------------------------------------------------------------------------------------------------------
      update CRT_UNIDADE_PRODUCAO SET CRT_UNIDADE_PRODUCAO.PROPRIEDADE_CODIGO = 
        (
          select m.IBGE || lpad(CRT_UNIDADE_PRODUCAO.PROPRIEDADE_CODIGO, 4, '0')
          from TAB_EMPREENDIMENTO emp
              inner join TAB_EMPREENDIMENTO_ENDERECO en
                on en.EMPREENDIMENTO = emp.id
              inner join LOV_MUNICIPIO m
                on m.ID = en.MUNICIPIO
          where en.CORRESPONDENCIA = 0
          and CRT_UNIDADE_PRODUCAO.EMPREENDIMENTO = emp.ID
        ) 
        where length(CRT_UNIDADE_PRODUCAO.PROPRIEDADE_CODIGO) <= 4
      ;
      
      ----------------------------------------------------------------------------------------------------------
      update HST_CRT_UNIDADE_PRODUCAO SET HST_CRT_UNIDADE_PRODUCAO.PROPRIEDADE_CODIGO = 
        (
          select m.IBGE || lpad(HST_CRT_UNIDADE_PRODUCAO.PROPRIEDADE_CODIGO, 4, '0')
          from TAB_EMPREENDIMENTO emp
              inner join TAB_EMPREENDIMENTO_ENDERECO en
                on en.EMPREENDIMENTO = emp.id
              inner join LOV_MUNICIPIO m
                on m.ID = en.MUNICIPIO
          where en.CORRESPONDENCIA = 0
          and HST_CRT_UNIDADE_PRODUCAO.EMPREENDIMENTO_ID = emp.ID
        ) 
        where length(HST_CRT_UNIDADE_PRODUCAO.PROPRIEDADE_CODIGO) <= 4
      ;
      
      ----------------------------------------------------------------------------------------------------------
      update CRT_UNIDADE_PRODUCAO_UNIDADE unid set unid.CODIGO_UP = substr(unid.CODIGO_UP, 1, length(unid.CODIGO_UP) -2) ||'00'||substr(unid.CODIGO_UP, -2, 2) 
      -- select count(1), min(length(unid.CODIGO_UP)) as min_tamanho, max(length(unid.CODIGO_UP)) as max_tamanho from CRT_UNIDADE_PRODUCAO_UNIDADE unid
        where length(unid.CODIGO_UP) <= 15
      ;
      
      ----------------------------------------------------------------------------------------------------------
      update HST_CRT_UNIDADE_PROD_UNIDADE hst_unid set hst_unid.CODIGO_UP = substr(hst_unid.CODIGO_UP, 1, length(hst_unid.CODIGO_UP) -2) ||'00'||substr(hst_unid.CODIGO_UP, -2, 2)
      -- select count(1), min(length(hst_unid.CODIGO_UP)) as min_tamanho, max(length(hst_unid.CODIGO_UP)) as max_tamanho from HST_CRT_UNIDADE_PROD_UNIDADE hst_unid
        where length(hst_unid.CODIGO_UP) <= 15
      ;
      
      ----------------------------------------------------------------------------------------------------------
  end;
  exception
     when others then
       rollback to RQF_21_savepoint;
	   
  commit;
END;  
