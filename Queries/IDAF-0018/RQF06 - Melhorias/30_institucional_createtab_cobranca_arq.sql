
  CREATE TABLE "IDAF"."TAB_FISC_COBRANCA_ARQ" 
   (	"ID" NUMBER(38,0) NOT NULL ENABLE, 
	"COBRANCA" NUMBER(38,0) NOT NULL ENABLE, 
	"ARQUIVO" NUMBER(38,0) NOT NULL ENABLE, 
	"ORDEM" NUMBER(3,0) NOT NULL ENABLE, 
	"DESCRICAO" VARCHAR2(100 BYTE) NOT NULL ENABLE, 
	"TID" VARCHAR2(36 BYTE) NOT NULL ENABLE, 
	 CONSTRAINT "PK_COBRANCA_ARQ" PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
  TABLESPACE "IDAF_TBS"  ENABLE, 
	 CONSTRAINT "FK_COBRANCA_ARQ" FOREIGN KEY ("COBRANCA")
	  REFERENCES "IDAF"."TAB_FISC_COBRANCA" ("ID") ENABLE, 
	 CONSTRAINT "FK_COBRANCA_ARQ_ARQ_ID" FOREIGN KEY ("ARQUIVO")
	  REFERENCES "IDAF"."TAB_ARQUIVO" ("ID") ENABLE
   ) PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 NOCOMPRESS LOGGING
  STORAGE(INITIAL 196608 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
  TABLESPACE "IDAF_TBS" ;
 

   COMMENT ON COLUMN "IDAF"."TAB_FISC_COBRANCA_ARQ"."ID" IS 'Chave primaria. Utiliza a sequencia seq_fisc_cobranca_arq.';
 
   COMMENT ON COLUMN "IDAF"."TAB_FISC_COBRANCA_ARQ"."COBRANCA" IS 'Chave estrangeira para tab_fisc_cobranca. Campo(ID).';
 
   COMMENT ON COLUMN "IDAF"."TAB_FISC_COBRANCA_ARQ"."ARQUIVO" IS 'Chave estrangeira para tab_arquivo. Campo(ID).';
 
   COMMENT ON COLUMN "IDAF"."TAB_FISC_COBRANCA_ARQ"."ORDEM" IS 'Ordem de inserc?o do item na lista.';
 
   COMMENT ON COLUMN "IDAF"."TAB_FISC_COBRANCA_ARQ"."DESCRICAO" IS 'Descric?o do arquivo inserido na lista.';
 
   COMMENT ON COLUMN "IDAF"."TAB_FISC_COBRANCA_ARQ"."TID" IS 'Id transacional. Esse valor garante a ligac?o entre todas as tabelas relacionadas com essa transaction.';
 
   COMMENT ON TABLE "IDAF"."TAB_FISC_COBRANCA_ARQ"  IS 'Tabela de relacionamento do roteiro e seus arquivos.';
 

  CREATE INDEX "IDAF"."IDX_COBRANCA_ARQ_CONS" ON "IDAF"."TAB_FISC_COBRANCA_ARQ" ("COBRANCA") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
  TABLESPACE "IDAF_TBS" ;
 

  CREATE INDEX "IDAF"."IDX_COBRANCA_ARQ_TID" ON "IDAF"."TAB_FISC_COBRANCA_ARQ" ("TID") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 131072 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
  TABLESPACE "IDAF_TBS" ;
 

  CREATE INDEX "IDAF"."IDX_COBRANCA_ARQ_TIPO" ON "IDAF"."TAB_FISC_COBRANCA_ARQ" ("ARQUIVO") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
  TABLESPACE "IDAF_TBS" ;
 
