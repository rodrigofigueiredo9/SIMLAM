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
  end;
  exception
     when others then
       rollback to RQF_21_savepoint;
	   
  commit;
END;  
