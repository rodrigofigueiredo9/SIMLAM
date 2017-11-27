ALTER TABLE idaf.tab_liberacao_cfo_cfoc
  ADD( serie varchar1(1) );
  
ALTER TABLE idaf.tab_numero_cfo_cfoc
  ADD( serie varchar1(1) );
  
ALTER TABLE idaf.cnf_doc_fito_intervalo
  ADD( serie varchar1(1) );

ALTER TABLE idaf.hst_cnf_doc_fito_intervalo
  ADD( serie varchar1(1) );
  
ALTER TABLE idaf.tab_ptv_outrouf_produto
  ADD( numero_origem_str varchar2(20) );
  
commit;


UPDATE tab_ptv_outrouf_produto
   SET numero_origem_str = to_char(numero_origem);
   
commit;

ALTER TABLE tab_ptv_outrouf_produto
 DROP COLUMN numero_origem;

commit;

 ALTER TABLE tab_ptv_outrouf_produto
 RENAME COLUMN numero_origem_str TO numero_origem;
 
 
 commit;
 
 -- Histórico
 
 ALTER TABLE idaf.hst_ptv_outrouf_prod
  ADD( numero_origem_str varchar2(20) );
  
commit;

UPDATE hst_ptv_outrouf_prod
   SET numero_origem_str = to_char(numero_origem);
   
commit;

ALTER TABLE hst_ptv_outrouf_prod
 DROP COLUMN numero_origem;

commit;

 ALTER TABLE hst_ptv_outrouf_prod
 RENAME COLUMN numero_origem_str TO numero_origem;
 
 
 commit;