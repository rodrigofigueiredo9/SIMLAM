BEGIN

  savepoint RQF_21_revert_savepoint;

  begin
      ----------------------------------------------------------------------------------------------------------
      /*select count(1) as crt_PROP_COD, min(length(crt.PROPRIEDADE_CODIGO)) as min_tamanho, max(length(crt.PROPRIEDADE_CODIGO)) as max_tamanho
       from CRT_UNIDADE_PRODUCAO crt;*/
      
      update CRT_UNIDADE_PRODUCAO set PROPRIEDADE_CODIGO = substr(PROPRIEDADE_CODIGO, -4, 4) 
        where length(PROPRIEDADE_CODIGO) > 4;
      ----------------------------------------------------------------------------------------------------------
      
      /*select count(1) as hst_crt_PROP_COD, min(length(hst_crt.PROPRIEDADE_CODIGO)) as min_tamanho, max(length(hst_crt.PROPRIEDADE_CODIGO)) as max_tamanho
       from HST_CRT_UNIDADE_PRODUCAO hst_crt;*/
      
      update HST_CRT_UNIDADE_PRODUCAO set PROPRIEDADE_CODIGO = substr(PROPRIEDADE_CODIGO, -4, 4) 
        where length(PROPRIEDADE_CODIGO) > 4;
      
      ----------------------------------------------------------------------------------------------------------
      /*select count(1) as unid_CODIGO_UP, min(length(unid.CODIGO_UP)) as min_tamanho, max(length(unid.CODIGO_UP)) as max_tamanho
       from CRT_UNIDADE_PRODUCAO_UNIDADE unid;*/
      
      update CRT_UNIDADE_PRODUCAO_UNIDADE set CODIGO_UP = substr(CODIGO_UP, 1, length(CODIGO_UP) -4) || substr(CODIGO_UP, -2, 2) 
        where length(CODIGO_UP) > 15;

      ----------------------------------------------------------------------------------------------------------
      /*select count(1) as hst_unid_CODIGO_UP, min(length(hst_unid.CODIGO_UP)) as min_tamanho, max(length(hst_unid.CODIGO_UP)) as max_tamanho
        from HST_CRT_UNIDADE_PROD_UNIDADE hst_unid;*/
      
      update HST_CRT_UNIDADE_PROD_UNIDADE set CODIGO_UP = substr(CODIGO_UP, 1, length(CODIGO_UP) -4) || substr(CODIGO_UP, -2, 2) 
        where length(CODIGO_UP) > 15;

      ----------------------------------------------------------------------------------------------------------
  end;
  exception
     when others then
       rollback to RQF_21_revert_savepoint;
	   
  commit;
END;  
