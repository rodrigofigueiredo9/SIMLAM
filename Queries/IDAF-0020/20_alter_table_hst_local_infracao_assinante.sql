ALTER TABLE HST_FISC_LOCAL_INFRACAO ADD ASSINANTE_ID NUMBER(38,0);
ALTER TABLE HST_FISC_LOCAL_INFRACAO ADD ASSINANTE_TID VARCHAR2(36 BYTE);

COMMENT ON COLUMN "IDAF"."HST_FISC_LOCAL_INFRACAO"."ASSINANTE_ID" IS 'Chave estrangeira para tab_pessoa. Campo(ID).';
COMMENT ON COLUMN "IDAF"."HST_FISC_LOCAL_INFRACAO"."ASSINANTE_TID" IS 'Referencia ao campo (TID) da tabela tab_pessoa.';


CREATE INDEX "IDAF"."IDX_HST_FISC_LOCALINFASSP" ON "IDAF"."HST_FISC_LOCAL_INFRACAO" ("ASSINANTE_ID") 
PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
STORAGE(INITIAL 196608 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
TABLESPACE "IDAF_TBS" ;


CREATE INDEX "IDAF"."IDX_HST_FISC_LOCALINFASSTID" ON "IDAF"."HST_FISC_LOCAL_INFRACAO" ("ASSINANTE_TID") 
PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
STORAGE(INITIAL 524288 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
TABLESPACE "IDAF_TBS" ;

COMMIT;