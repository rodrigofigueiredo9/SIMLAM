alter table tab_fisc_infracao
add ("POSSUI_INFRACAO" number(1, 0),
     "DATA_CONSTATACAO" date,
     "HORA_CONSTATACAO" varchar2(5 byte),
     "CLASSIFICACAO_INFRACAO" number(1, 0))
;