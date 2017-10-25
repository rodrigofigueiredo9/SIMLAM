alter table tab_fisc_obj_infracao
add ("INTERDITADO" number(1, 0),
     "NUMERO_LACRE" varchar2(150),
     "IUF_DIGITAL" number(1, 0),
     "IUF_NUMERO" NUMBER(38,0),
     "IUF_DATA" DATE,
     "SERIE" NUMBER(38,0))
;

ALTER TABLE tab_fisc_obj_infracao
ADD CONSTRAINT FK_FIS_OBJ_SERIE
  FOREIGN KEY (SERIE)
  REFERENCES "IDAF"."LOV_FISCALIZACAO_SERIE" ("ID") ENABLE;