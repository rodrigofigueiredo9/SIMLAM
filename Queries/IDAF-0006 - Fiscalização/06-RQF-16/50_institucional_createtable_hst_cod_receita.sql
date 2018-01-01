
  CREATE TABLE "IDAF"."HST_LOV_FISC_INFR_COD_RECE" 
   (	"ID" NUMBER(38,0) NOT NULL ENABLE, 
	"CODIGO_ID" NUMBER(38,0) NOT NULL ENABLE, 
	"TEXTO" VARCHAR2(50 BYTE) NOT NULL ENABLE, 
	"DESCRICAO" VARCHAR2(100 BYTE), 
	"ATIVO" NUMBER(1,0) NOT NULL ENABLE, 
	"TID" VARCHAR2(36 BYTE) NOT NULL ENABLE, 
	"EXECUTOR_ID" NUMBER(38,0), 
	"EXECUTOR_TID" VARCHAR2(36 BYTE), 
	"EXECUTOR_NOME" VARCHAR2(80 BYTE) NOT NULL ENABLE, 
	"EXECUTOR_LOGIN" VARCHAR2(30 BYTE) NOT NULL ENABLE, 
	"EXECUTOR_TIPO_ID" NUMBER(38,0), 
	"EXECUTOR_TIPO_TEXTO" VARCHAR2(30 BYTE), 
	"ACAO_EXECUTADA" NUMBER(38,0) NOT NULL ENABLE, 
	"DATA_EXECUCAO" TIMESTAMP (6) NOT NULL ENABLE, 
	 CONSTRAINT "PK_HST_LOV_FISC_INFR_COD_RECE" PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
  TABLESPACE "IDAF_TBS"  ENABLE, 
	 CONSTRAINT "FK_HST_LOV_FISC_INFR_CR_ACAO" FOREIGN KEY ("ACAO_EXECUTADA")
	  REFERENCES "IDAF"."LOV_HISTORICO_ARTEFATOS_ACOES" ("ID") ENABLE
   ) PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
  TABLESPACE "IDAF_TBS" ;
 

   COMMENT ON COLUMN "IDAF"."HST_LOV_FISC_INFR_COD_RECE"."ID" IS 'Chave prim�ria. Utiliza a sequ�ncia seq_hst_lov_fisc_infr_cod_rece.';
 
   COMMENT ON COLUMN "IDAF"."HST_LOV_FISC_INFR_COD_RECE"."CODIGO_ID" IS 'Chave estrangeira para lov_fisc_infracao_codigo_rece.';
 
   COMMENT ON COLUMN "IDAF"."HST_LOV_FISC_INFR_COD_RECE"."TEXTO" IS 'Texto de identifica��o. C�digo da receita.';
 
   COMMENT ON COLUMN "IDAF"."HST_LOV_FISC_INFR_COD_RECE"."DESCRICAO" IS 'Descri��o do c�digo da receita.';
 
   COMMENT ON COLUMN "IDAF"."HST_LOV_FISC_INFR_COD_RECE"."ATIVO" IS 'Ativo: 1 - sim | 0 - n�o.';
 
   COMMENT ON COLUMN "IDAF"."HST_LOV_FISC_INFR_COD_RECE"."TID" IS 'Id transacional. Esse valor garante a liga��o entre todas as tabelas relacionadas com essa transaction.';
 
   COMMENT ON COLUMN "IDAF"."HST_LOV_FISC_INFR_COD_RECE"."EXECUTOR_ID" IS 'Chave estrangeira para tab_funcionario. Campo (ID).';
 
   COMMENT ON COLUMN "IDAF"."HST_LOV_FISC_INFR_COD_RECE"."EXECUTOR_TID" IS 'Referencia ao campo (TID) da tabela tab_funcionario.';
 
   COMMENT ON COLUMN "IDAF"."HST_LOV_FISC_INFR_COD_RECE"."EXECUTOR_NOME" IS 'Nome do executor que executou a a��o.';
 
   COMMENT ON COLUMN "IDAF"."HST_LOV_FISC_INFR_COD_RECE"."EXECUTOR_LOGIN" IS 'Login do executor que executou a a��o.';
 
   COMMENT ON COLUMN "IDAF"."HST_LOV_FISC_INFR_COD_RECE"."EXECUTOR_TIPO_ID" IS 'Chave estrangeira para lov_executor_tipo. Campo(ID).';
 
   COMMENT ON COLUMN "IDAF"."HST_LOV_FISC_INFR_COD_RECE"."EXECUTOR_TIPO_TEXTO" IS 'Texto do tipo do executor que executou a a��o.';
 
   COMMENT ON COLUMN "IDAF"."HST_LOV_FISC_INFR_COD_RECE"."ACAO_EXECUTADA" IS 'A��o que foi disparada pelo sistema a qual gerou essa linha de hist�rico.';
 
   COMMENT ON COLUMN "IDAF"."HST_LOV_FISC_INFR_COD_RECE"."DATA_EXECUCAO" IS 'Data que foi gerada essa linha de hist�rico.';
 
   COMMENT ON TABLE "IDAF"."HST_LOV_FISC_INFR_COD_RECE"  IS 'Tabela do historico da Configuracao de Fiscalizacao - C�digos da Receita.';
 

  CREATE INDEX "IDAF"."IDX_HST_LOV_F_INF_CR_EXEC_ACAO" ON "IDAF"."HST_LOV_FISC_INFR_COD_RECE" ("ACAO_EXECUTADA") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
  TABLESPACE "IDAF_TBS" ;
 

  CREATE INDEX "IDAF"."IDX_HST_LOV_F_INF_CR_EXEC_DATA" ON "IDAF"."HST_LOV_FISC_INFR_COD_RECE" ("DATA_EXECUCAO") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
  TABLESPACE "IDAF_TBS" ;
 

  CREATE INDEX "IDAF"."IDX_HST_LOV_F_INF_CR_EXEC_TID1" ON "IDAF"."HST_LOV_FISC_INFR_COD_RECE" ("EXECUTOR_TID") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
  TABLESPACE "IDAF_TBS" ;
 

  CREATE INDEX "IDAF"."IDX_HST_LOV_F_INF_CR_EXE_TIP_I" ON "IDAF"."HST_LOV_FISC_INFR_COD_RECE" ("EXECUTOR_TIPO_ID") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
  TABLESPACE "IDAF_TBS" ;
 

  CREATE INDEX "IDAF"."IDX_HST_LOV_F_INF_CR_TID" ON "IDAF"."HST_LOV_FISC_INFR_COD_RECE" ("TID") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
  TABLESPACE "IDAF_TBS" ;
 

  CREATE INDEX "IDAF"."IDX_HST_LOV_F_INF_CR_TIPO_ID1" ON "IDAF"."HST_LOV_FISC_INFR_COD_RECE" ("CODIGO_ID") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
  TABLESPACE "IDAF_TBS" ;
 
