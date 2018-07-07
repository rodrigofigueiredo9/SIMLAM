alter table lov_fisc_infracao_codigo_rece
add ("DESCRICAO" VARCHAR2(100 BYTE),
      "ATIVO" NUMBER(1,0),
      "TID" VARCHAR2(36 BYTE))
;

update lov_fisc_infracao_codigo_rece
set ativo = 1,
    tid = '56C4B27E-AD4C-5591-E050-A8C06002328F'
;

commit;

alter table lov_fisc_infracao_codigo_rece
modify ("ATIVO" NUMBER(1,0) not null,
        "TID" VARCHAR2(36 BYTE) not null)
;

commit;