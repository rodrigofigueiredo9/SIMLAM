alter table tab_fisc_local_infracao
modify DATA date null;

alter table tab_fisc_local_infracao
add AREA_FISCALIZACAO number(1, 0)
;

COMMENT ON COLUMN "IDAF"."TAB_FISC_LOCAL_INFRACAO"."AREA_FISCALIZACAO" IS 'Área da fiscalização. 0 - DDSIA; 1 - DDSIV; 2 - DRNRE';

alter table tab_fisc_local_infracao
modify (AREA_ABRANG number null,
        lat_northing number null,
        lon_easting number null,
        local varchar(150) null)
;

alter table tab_fisc_local_infracao
modify (local varchar(150) null)
;

alter table tab_fisc_local_infracao
modify(sis_coord number (38, 0) null,
       datum number(38, 0) null,
       fuso number(38, 0) null,
       hemisferio number(38, 0) null,
       municipio number(38, 0) null)
;       
