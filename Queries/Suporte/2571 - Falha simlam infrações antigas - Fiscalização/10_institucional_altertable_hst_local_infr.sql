alter table HST_FISC_LOCAL_INFRACAO
add AREA_FISCALIZACAO	NUMBER(1,0)
;

COMMENT ON COLUMN "IDAF"."HST_FISC_LOCAL_INFRACAO"."AREA_FISCALIZACAO" IS 'Área da fiscalização. 0 - DDSIA; 1 - DDSIV; 2 - DRNRE';

begin

for i in ( select tfli.id,
                  tfli.AREA_FISCALIZACAO
           from tab_fisc_local_infracao tfli
           where tfli.AREA_FISCALIZACAO is not null
          ) loop
          update hst_fisc_local_infracao
          set AREA_FISCALIZACAO = i.area_fiscalizacao
          where area_fiscalizacao is null
                and LOCAL_INFRACAO_ID = i.id;
end loop;

end;