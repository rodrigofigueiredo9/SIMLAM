alter table tab_fisc_infracao
add ("POSSUI_INFRACAO" number(1, 0),
     "DATA_CONSTATACAO" date,
     "HORA_CONSTATACAO" varchar2(5 byte),
     "CLASSIFICACAO_INFRACAO" number(1, 0))
;

alter table tab_fisc_infracao
modify DESCRICAO_INFRACAO VARCHAR2(3000 BYTE);

alter table hst_fisc_infracao 
modify DESCRICAO_INFRACAO VARCHAR2(3000 BYTE);