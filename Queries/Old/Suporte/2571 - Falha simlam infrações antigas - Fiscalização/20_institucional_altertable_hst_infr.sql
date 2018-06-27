alter table HST_FISC_INFRACAO
add ( POSSUI_INFRACAO	NUMBER(1,0),
      DATA_CONSTATACAO	DATE,
      HORA_CONSTATACAO	VARCHAR2(5 BYTE),
      CLASSIFICACAO_INFRACAO	NUMBER(1,0) )
;

COMMENT ON COLUMN "IDAF"."HST_FISC_INFRACAO"."POSSUI_INFRACAO" IS '1 - possui infração; 0 - não possui infração';
COMMENT ON COLUMN "IDAF"."HST_FISC_INFRACAO"."DATA_CONSTATACAO" IS 'Data da constatação/vistoria';
COMMENT ON COLUMN "IDAF"."HST_FISC_INFRACAO"."HORA_CONSTATACAO" IS 'Hora da constatação/vistoria';
COMMENT ON COLUMN "IDAF"."HST_FISC_INFRACAO"."CLASSIFICACAO_INFRACAO" IS '0-leve; 1-média; 2-grave; 3-gravíssima';

begin

for i in ( select tfi.id,
                  tfi.possui_infracao,
				  tfi.data_constatacao,
				  tfi.hora_constatacao,
				  tfi.classificacao_infracao
           from tab_fisc_infracao tfi
           where tfi.possui_infracao is not null
				 or tfi.data_constatacao is not null
				 or tfi.hora_constatacao is not null
				 or tfi.classificacao_infracao is not null
          ) loop
		  
          update hst_fisc_infracao
          set possui_infracao = i.possui_infracao,
			  data_constatacao = i.data_constatacao,
			  hora_constatacao = i.hora_constatacao,
			  classificacao_infracao = i.classificacao_infracao
          where ( possui_infracao is null
				  or data_constatacao is null
				  or hora_constatacao is null
				  or classificacao_infracao is null )
                and infracao_id = i.id;
end loop;

--commit;

end;
