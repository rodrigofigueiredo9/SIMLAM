  CREATE TABLE "IDAF"."TAB_FISC_COBRANCA"
  (
    "ID"                NUMBER(38,0) NOT NULL ENABLE,
    "FISCALIZACAO"      NUMBER(38,0) NOT NULL ENABLE,
    "AUTUADO"           NUMBER(38,0) NOT NULL ENABLE,
    "CODIGORECEITA"     NUMBER(38,0),
    "SERIE"           	NUMBER(38,0),
    "IUF_NUMERO"        NUMBER(38,0),
    "IUF_DATA"        	DATE,
    "PROTOC_NUM"        NUMBER(38, 0),
    "AUTOS"             NUMBER(38,0),    
    "NOT_IUF_DATA"   	  DATE,
    "NOT_JIAPI_DATA"   	DATE,
    "NOT_CORE_DATA"   	DATE,
    "VALOR_MULTA"       NUMBER,
    "QTDPARCELAS"       NUMBER(2,0),    
    "VENCIMENTO_DATA" 	DATE,
    "DATAEMISSAO" 	    DATE,
    "TID"               VARCHAR2(36 BYTE) NOT NULL ENABLE,
    CONSTRAINT "PK_FIS_COBRANCA" PRIMARY KEY ("ID") 
    USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
    STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645 
    PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT) 
    TABLESPACE "IDAF_TBS" ENABLE, 
	 CONSTRAINT "FK_FIS_COB_AUT" FOREIGN KEY ("AUTUADO")
	  REFERENCES "IDAF"."TAB_PESSOA" ("ID") ENABLE, 
	 CONSTRAINT "FK_FIS_COB_REC" FOREIGN KEY ("CODIGORECEITA")
	  REFERENCES "IDAF"."LOV_FISC_INFRACAO_CODIGO_RECE" ("ID") ENABLE, 
	 CONSTRAINT "FK_FIS_COB_SERIE" FOREIGN KEY ("SERIE")
	  REFERENCES "IDAF"."LOV_FISCALIZACAO_SERIE" ("ID") ENABLE
  )
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 NOCOMPRESS LOGGING STORAGE
  (
    INITIAL 196608 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645 PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT
  )
  TABLESPACE "IDAF_TBS" ;
 
  COMMENT ON COLUMN "IDAF"."TAB_FISC_COBRANCA"."ID" IS 'Chave primaria. Utiliza a sequencia seq_fisc_cobranca.';
  
  COMMENT ON COLUMN "IDAF"."TAB_FISC_COBRANCA"."FISCALIZACAO" IS 'Informa o n�mero da fiscalizacao (poder� existir na tabela tab_fiscalizacao. Campo (ID))';
  
  COMMENT ON COLUMN "IDAF"."TAB_FISC_COBRANCA"."AUTUADO" IS 'Chave estrangeira com a tabela tab_pessoa. Campo (ID)';
  
  COMMENT ON COLUMN "IDAF"."TAB_FISC_COBRANCA"."CODIGORECEITA" IS 'Chave estrangeira para lov_fisc_infracao_codigo_rece. Campo(ID).';
  
  COMMENT ON COLUMN "IDAF"."TAB_FISC_COBRANCA"."SERIE" IS 'Chave estrangeira para lov_fiscalizacao_serie. Campo(ID).';
  
  COMMENT ON COLUMN "IDAF"."TAB_FISC_COBRANCA"."IUF_NUMERO" IS 'Informa o n�mero do IUF.';
  
  COMMENT ON COLUMN "IDAF"."TAB_FISC_COBRANCA"."IUF_DATA" IS 'Data do IUF..';
  
  COMMENT ON COLUMN "IDAF"."TAB_FISC_COBRANCA"."PROTOC_NUM" IS 'Informa o n�mero de protocolo.';
  
  COMMENT ON COLUMN "IDAF"."TAB_FISC_COBRANCA"."AUTOS" IS 'Numero de Autos/Temos de N? AI, N? TEI e N? TAD. Utiliza a sequencia seq_fiscalizacao_autos.';
  
  COMMENT ON COLUMN "IDAF"."TAB_FISC_COBRANCA"."NOT_IUF_DATA" IS 'Data da notifica��o IUF.';
  
  COMMENT ON COLUMN "IDAF"."TAB_FISC_COBRANCA"."NOT_JIAPI_DATA" IS 'Data da notifica��o JIAPI.';
  
  COMMENT ON COLUMN "IDAF"."TAB_FISC_COBRANCA"."NOT_CORE_DATA" IS 'Data da notifica��o CORE.';
  
  COMMENT ON COLUMN "IDAF"."TAB_FISC_COBRANCA"."VALOR_MULTA" IS 'Informa o valor da multa.';
  
  COMMENT ON COLUMN "IDAF"."TAB_FISC_COBRANCA"."QTDPARCELAS" IS 'Informa a quantidade de parcelas que ser� gerada para o parcelamento da cobran�a.';
  
  COMMENT ON COLUMN "IDAF"."TAB_FISC_COBRANCA"."VENCIMENTO_DATA" IS 'Data de vencimento da primeira parcela.';
  
  COMMENT ON COLUMN "IDAF"."TAB_FISC_COBRANCA"."DATAEMISSAO" IS 'Data de emiss�o da cobran�a.';
  
  COMMENT ON COLUMN "IDAF"."TAB_FISC_COBRANCA"."TID" IS 'Id transacional. Esse valor garante a liga��o entre todas as tabelas relacionadas com essa transaction.';
  
  COMMENT ON TABLE "IDAF"."TAB_FISC_COBRANCA"  IS 'Tabela de Cobran�a.';
 
  CREATE INDEX "IDAF"."IDX_FIS_COB_TID" ON "IDAF"."TAB_FISC_COBRANCA" ("TID") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
  TABLESPACE "IDAF_TBS" ;

  CREATE INDEX "IDAF"."IDX_FIS_COB_FIS" ON "IDAF"."TAB_FISC_COBRANCA" ("FISCALIZACAO") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
  TABLESPACE "IDAF_TBS" ;
  
  CREATE INDEX "IDAF"."IDX_FIS_COB_AUT" ON "IDAF"."TAB_FISC_COBRANCA" ("AUTUADO") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
  TABLESPACE "IDAF_TBS" ;
  
  CREATE INDEX "IDAF"."IDX_FIS_COB_REC" ON "IDAF"."TAB_FISC_COBRANCA" ("CODIGORECEITA") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
  TABLESPACE "IDAF_TBS" ;
  
  CREATE INDEX "IDAF"."IDX_FIS_COB_SERIE" ON "IDAF"."TAB_FISC_COBRANCA" ("SERIE") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
  TABLESPACE "IDAF_TBS" ;