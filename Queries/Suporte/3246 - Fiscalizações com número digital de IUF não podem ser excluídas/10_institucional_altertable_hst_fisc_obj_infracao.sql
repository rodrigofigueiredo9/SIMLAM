alter table HST_FISC_OBJ_INFRACAO
add ( INTERDITADO	NUMBER(1,0),
	  NUMERO_LACRE	VARCHAR2(150 BYTE),
	  IUF_DIGITAL	NUMBER(1,0),
	  IUF_NUMERO	NUMBER(38,0),
	  IUF_DATA	DATE,
	  SERIE	NUMBER(38,0) )
;

COMMENT ON COLUMN "IDAF"."HST_FISC_OBJ_INFRACAO"."INTERDITADO" IS '1 - Interditado; 0 - Embargado.';
COMMENT ON COLUMN "IDAF"."TAB_FISC_OBJ_INFRACAO"."INTERDITADO" IS '1 - Interditado; 0 - Embargado.';
COMMENT ON COLUMN "IDAF"."HST_FISC_OBJ_INFRACAO"."NUMERO_LACRE" IS 'String com o número do lacre de interdição.';
COMMENT ON COLUMN "IDAF"."TAB_FISC_OBJ_INFRACAO"."NUMERO_LACRE" IS 'String com o número do lacre de interdição.';
COMMENT ON COLUMN "IDAF"."HST_FISC_OBJ_INFRACAO"."IUF_DIGITAL" IS '1 - IUF digital; 0 - IUF bloco';
COMMENT ON COLUMN "IDAF"."TAB_FISC_OBJ_INFRACAO"."IUF_DIGITAL" IS '1 - IUF digital; 0 - IUF bloco';
COMMENT ON COLUMN "IDAF"."HST_FISC_OBJ_INFRACAO"."IUF_NUMERO" IS 'Número do IUF.';
COMMENT ON COLUMN "IDAF"."TAB_FISC_OBJ_INFRACAO"."IUF_NUMERO" IS 'Número do IUF.';
COMMENT ON COLUMN "IDAF"."HST_FISC_OBJ_INFRACAO"."IUF_DATA" IS 'Data do IUF.';
COMMENT ON COLUMN "IDAF"."TAB_FISC_OBJ_INFRACAO"."IUF_DATA" IS 'Data do IUF.';
COMMENT ON COLUMN "IDAF"."HST_FISC_OBJ_INFRACAO"."SERIE" IS 'Série do IUF';
COMMENT ON COLUMN "IDAF"."TAB_FISC_OBJ_INFRACAO"."SERIE" IS 'Série do IUF';


begin

for i in ( select tfoi.id,
                  tfoi.interditado,
				  tfoi.numero_lacre,
				  tfoi.iuf_digital,
				  tfoi.iuf_numero,
				  tfoi.iuf_data,
				  tfoi.serie
           from tab_fisc_obj_infracao tfoi
           where tfoi.interditado is not null
				 or tfoi.numero_lacre is not null
				 or tfoi.iuf_digital is not null
				 or tfoi.iuf_numero is not null
				 or tfoi.iuf_data is not null
				 or tfoi.serie is not null
          ) loop
		  
          update hst_fisc_obj_infracao
          set interditado = i.interditado,
			  numero_lacre = i.numero_lacre,
			  iuf_digital = i.iuf_digital,
			  iuf_numero = i.iuf_numero,
			  iuf_data = i.iuf_data,
			  serie = i.serie
          where obj_infracao_id = i.id;
end loop;

--commit;

end;
