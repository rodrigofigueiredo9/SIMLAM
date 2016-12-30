
select PROPRIEDADE_CODIGO as antes_CRT_UNID_PROD from CRT_UNIDADE_PRODUCAO where rownum <= 10 order by id;
alter table 
   CRT_UNIDADE_PRODUCAO
modify 
( 
   PROPRIEDADE_CODIGO Number(19,0)
);
select PROPRIEDADE_CODIGO as depois_CRT_UNID_PROD from CRT_UNIDADE_PRODUCAO where rownum <= 10 order by id;
------------------------

select PROPRIEDADE_CODIGO as antes_HST_UNID_PROD from HST_CRT_UNIDADE_PRODUCAO where rownum <= 10 order by id;
alter table 
   HST_CRT_UNIDADE_PRODUCAO
modify 
( 
   PROPRIEDADE_CODIGO Number(19,0)
); 
select PROPRIEDADE_CODIGO as depois_HST_UNID_PROD from HST_CRT_UNIDADE_PRODUCAO where rownum <= 10 order by id;
------------------------

select CODIGO_UP as antes_CRT_UP_UNID from CRT_UNIDADE_PRODUCAO_UNIDADE where rownum <= 10 order by id;
alter table 
   CRT_UNIDADE_PRODUCAO_UNIDADE
modify 
( 
   CODIGO_UP Number(19,0)
);
select CODIGO_UP as depois_CRT_UP_UNID from CRT_UNIDADE_PRODUCAO_UNIDADE where rownum <= 10 order by id;
------------------------

select CODIGO_UP as antes_HST_UP_UNID from HST_CRT_UNIDADE_PROD_UNIDADE where rownum <= 10 order by id;
alter table
   HST_CRT_UNIDADE_PROD_UNIDADE
modify
(
   CODIGO_UP Number(19,0)
);
select CODIGO_UP as depois_HST_UP_UNID from HST_CRT_UNIDADE_PROD_UNIDADE where rownum <= 10 order by id;
