  select unidade_producao as ID_caracterizacao, id as Id_UP, codigo_up from crt_unidade_producao_unidade 
            where unidade_producao in (select id from crt_unidade_producao 
            where empreendimento = (select empreendimento from tab_protocolo 
            where numero = 40794	and ano = 2016 /*:protocolo*/ ))
    order by codigo_up, unidade_producao
  ;
 select unidade_producao as ID_caracterizacao, id as Id_UP, codigo_up from crt_unidade_producao_unidade 
          where unidade_producao = (select id from crt_unidade_producao 
          where PROPRIEDADE_CODIGO = 32032052923
          )
  order by codigo_up, unidade_producao
  ;
  select codigo_up nome from crt_unidade_producao_unidade 
        where unidade_producao in (select id from crt_unidade_producao 
        where empreendimento = (select empreendimento from tab_protocolo 
        where id = 158680 /*:protocolo*/ )) group by codigo_up
  ;
  -- Para encontrar dados de processo:
  select e.CODIGO, e.ID, e.DENOMINADOR, p.numero, p.ano, p.DATA_CRIACAO
    from tab_protocolo p inner join TAB_EMPREENDIMENTO e on e.ID = p.EMPREENDIMENTO
    where numero = 40794	and ano = 2016
  ;
  select * from tab_protocolo 
    where empreendimento in(158709); --Empreendimento ID 158709 e código 158690  
    
  --- empreendimento do Cod Propriedade 
  select c.PROPRIEDADE_CODIGO, c.id as ID_CARACT, e.CODIGO, e.ID, e.DENOMINADOR 
    from crt_unidade_producao c 
      inner join TAB_EMPREENDIMENTO e on c.EMPREENDIMENTO = e.ID
    where PROPRIEDADE_CODIGO = 32032052923;

 -- Caracterização de UP:
  select * from crt_unidade_producao 
    where empreendimento in(158709); --Empreendimento ID 158709 e código 158690

  -- UPs:
  select * from CRT_UNIDADE_PRODUCAO_UNIDADE un 
    where un.UNIDADE_PRODUCAO in ( select id from crt_unidade_producao where empreendimento in(158709) ); --Empreendimento ID 158709 e código 158690  
    
 -- Coordenadas de UP:
   select * from CRT_UNIDADE_PRODUCAO_UN_COORD 
    where UNIDADE_PRODUCAO_UNIDADE in 
      ( select ID from CRT_UNIDADE_PRODUCAO_UNIDADE un 
        where un.UNIDADE_PRODUCAO in (select id from crt_unidade_producao where empreendimento in(158709) ) ); --Empreendimento ID 158709 e código 158690  

 -- Produtor de UP:
   select * from CRT_UNIDADE_PROD_UN_PRODUTOR
    where UNIDADE_PRODUCAO_UNIDADE in 
      ( select ID from CRT_UNIDADE_PRODUCAO_UNIDADE un 
        where un.UNIDADE_PRODUCAO in (select id from crt_unidade_producao where empreendimento in(158709) ) ); --Empreendimento ID 158709 e código 158690  

 -- Resp. Técnico de UP:
   select * from CRT_UNIDADE_PROD_UN_RESP_TEC
    where UNIDADE_PRODUCAO_UNIDADE in 
      ( select ID from CRT_UNIDADE_PRODUCAO_UNIDADE un 
        where un.UNIDADE_PRODUCAO in (select id from crt_unidade_producao where empreendimento in(158709) ) ); --Empreendimento ID 158709 e código 158690  
    
  -- Empreendimento:
  select * from TAB_EMPREENDIMENTO
    where ID in(158709); --Empreendimento ID 158709 e código 158690  
    
  select codigo, --(select p.nome from idafcredenciado.tab_pessoa p inner join idafcredenciado.tab_credenciado c on c.pessoa = p.id where c.id = t.CREDENCIADO) as credenciado,
  t.* from TAB_EMPREENDIMENTO t
    where codigo in(158690); --Empreendimento ID 158709 e código 158690 ;
      
  -- Outras caracterizações duplicadas:
  select EMPREENDIMENTO ID_EMPREEND,
    min(PROPRIEDADE_CODIGO) as MIN_COD_PROPRIEDADE,
    min(PROPRIEDADE_CODIGO) as MAX_COD_PROPRIEDADE,
    max(id) as MAX_ID_CARACT,
    min(id) as MIN_ID_CARACT,
    count(1) as QTDE_CARACT_UP
    from crt_unidade_producao 
    group by empreendimento
    having count(1) > 1
    ;

-- Executando ajustes, removendo 1 item duplicado:
BEGIN
  savepoint Corr_UP_savepoint;
  begin
 -- Produtor de UP:
   delete from CRT_UNIDADE_PROD_UN_PRODUTOR
    where UNIDADE_PRODUCAO_UNIDADE in 
      ( select ID from CRT_UNIDADE_PRODUCAO_UNIDADE un 
        where un.UNIDADE_PRODUCAO in (select id from crt_unidade_producao where empreendimento in(158709) and id = 5203 ) ); --Empreendimento ID 158709 e código 158690  
 -- Resp. Técnico de UP:
   delete from CRT_UNIDADE_PROD_UN_RESP_TEC
    where UNIDADE_PRODUCAO_UNIDADE in 
      ( select ID from CRT_UNIDADE_PRODUCAO_UNIDADE un 
        where un.UNIDADE_PRODUCAO in (select id from crt_unidade_producao where empreendimento in(158709) and id = 5203 ) ); --Empreendimento ID 158709 e código 158690  
 -- Coordenadas de UP:
   delete from CRT_UNIDADE_PRODUCAO_UN_COORD 
    where UNIDADE_PRODUCAO_UNIDADE in 
      ( select ID from CRT_UNIDADE_PRODUCAO_UNIDADE un 
        where un.UNIDADE_PRODUCAO in (select id from crt_unidade_producao where empreendimento in(158709) and id = 5203) ); --Empreendimento ID 158709 e código 158690  
  -- UP:
    delete from CRT_UNIDADE_PRODUCAO_UNIDADE 
      where UNIDADE_PRODUCAO in ( select id from crt_unidade_producao where empreendimento in(158709) and id = 5203 ); --Empreendimento ID 158709 e código 158690  
  -- Caracterização de UP:
    delete from crt_unidade_producao 
      where empreendimento in(158709) and id = 5203; --Empreendimento ID 158709 e código 158690
  end;
  exception
     when others then
       rollback to Corr_UP_savepoint;
  commit;
END;