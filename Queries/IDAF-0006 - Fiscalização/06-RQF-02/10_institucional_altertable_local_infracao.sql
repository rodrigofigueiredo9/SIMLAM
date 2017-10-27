alter table tab_fisc_local_infracao
modify DATA date null;

alter table tab_fisc_local_infracao
add AREA_FISCALIZACAO number(1, 0)
;

COMMENT ON COLUMN "IDAF"."TAB_FISC_LOCAL_INFRACAO"."AREA_FISCALIZACAO" IS 'Área da fiscalização. 0 - DDSIA; 1 - DDSIV; 2 - DRNRE';