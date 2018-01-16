
  CREATE TABLE "IDAF"."HST_FISC_MULTA" 
  (	"ID" 					NUMBER(38,0) NOT NULL ENABLE, 
	"ID_HST" 				NUMBER(38,0) NOT NULL ENABLE, 
	"MULTA_ID"				NUMBER(38,0), 
	"FISCALIZACAO_ID" 		NUMBER(38,0),
	"IUF_DIGITAL"			NUMBER(1,0),
	"IUF_NUMERO"			NUMBER(38,0),
	"IUF_DATA"				DATE,
	"ARQUIVO_ID"			NUMBER(38,0),
	"ARQUIVO_TID" 			VARCHAR2(36 BYTE),
	"SERIE_ID"				NUMBER(38,0),
	"SERIE_TEXTO"			VARCHAR2(50 BYTE),
	"VALOR_MULTA"			NUMBER,
	"JUSTIFICAR" 			VARCHAR2(1000 BYTE),
	"CODIGO_RECEITA_ID" 	NUMBER(38,0),
	"COD_RECEITA_TEXTO" 	VARCHAR2(50 BYTE),
	"COD_RECEITA_DESCRICAO"	VARCHAR2(100 BYTE),
	"TID"					VARCHAR2(36 BYTE)	NOT NULL,  
  CONSTRAINT "PK_HST_FISC_MULTA" PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 131072 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
  TABLESPACE "IDAF_TBS"  ENABLE, 
	 CONSTRAINT "FK_HST_FISC_MULTA_HST" FOREIGN KEY ("ID_HST")
	  REFERENCES "IDAF"."HST_FISCALIZACAO" ("ID") ENABLE
   ) PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 NOCOMPRESS LOGGING
  STORAGE(INITIAL 4194304 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
  TABLESPACE "IDAF_TBS" ;
 
	COMMENT ON COLUMN "IDAF"."HST_FISC_MULTA"."ID" 						IS 'Chave primaria. Utiliza a sequencia SEQ_HST_FISC_MULTA.';
	COMMENT ON COLUMN "IDAF"."HST_FISC_MULTA"."ID_HST" 				    IS 'Chave estrangeira para hst_fiscalizacao. Campo(ID).';
	COMMENT ON COLUMN "IDAF"."HST_FISC_MULTA"."MULTA_ID"				    IS 'Chave estrangeira para tab_fisc_multa. Campo (ID).';
	COMMENT ON COLUMN "IDAF"."HST_FISC_MULTA"."FISCALIZACAO_ID" 		    IS 'Chave estrangeira para tab_fiscalizacao. Campo(ID).';
	COMMENT ON COLUMN "IDAF"."HST_FISC_MULTA"."IUF_DIGITAL"			    IS 'Informa se IUF é digital ou bloco. 1 - digital | 0 - bloco.';
	COMMENT ON COLUMN "IDAF"."HST_FISC_MULTA"."IUF_NUMERO"			    IS 'Informa o número do IUF.';
	COMMENT ON COLUMN "IDAF"."HST_FISC_MULTA"."IUF_DATA"				    IS 'Data do IUF.';
	COMMENT ON COLUMN "IDAF"."HST_FISC_MULTA"."ARQUIVO_ID"			    IS 'Chave estrangeira para tab_arquivo. Campo(ID).';
	COMMENT ON COLUMN "IDAF"."HST_FISC_MULTA"."ARQUIVO_TID" 			    IS 'Referencia ao campo (TID) da tabela tab_arquivo.';
	COMMENT ON COLUMN "IDAF"."HST_FISC_MULTA"."SERIE_ID"				    IS 'Chave estrangeira para lov_fiscalizacao_serie. Campo(ID).';
	COMMENT ON COLUMN "IDAF"."HST_FISC_MULTA"."SERIE_TEXTO"			    IS 'Série utilizada. Campo (TEXTO) da tabela lov_fiscalizacao_serie.';
	COMMENT ON COLUMN "IDAF"."HST_FISC_MULTA"."VALOR_MULTA"			    IS 'Valor da multa.';
	COMMENT ON COLUMN "IDAF"."HST_FISC_MULTA"."JUSTIFICAR" 			    IS 'Justificar o valor da penalidade pecuniaria atribuida, levando-se em considerção os parametros legais.';
	COMMENT ON COLUMN "IDAF"."HST_FISC_MULTA"."CODIGO_RECEITA_ID" 	    IS 'Chave estrangeira para lov_fisc_infracao_codigo_rece. Campo(ID).';
	COMMENT ON COLUMN "IDAF"."HST_FISC_MULTA"."COD_RECEITA_TEXTO" 	    IS 'Campo texto do código da receita.';
	COMMENT ON COLUMN "IDAF"."HST_FISC_MULTA"."COD_RECEITA_DESCRICAO"	IS 'Campo descrição do código da receita.';
	COMMENT ON COLUMN "IDAF"."HST_FISC_MULTA"."TID"					    IS 'Id transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
 
	COMMENT ON TABLE "IDAF"."HST_FISC_MULTA"  IS 'Tabela com o histórico da tabela tab_fisc_multa da Fiscalização.';
 

  CREATE INDEX "IDAF"."IDX_HST_MULTA_ARQ" ON "IDAF"."HST_FISC_MULTA" ("ARQUIVO_ID") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
  TABLESPACE "IDAF_TBS" ;
 

  CREATE INDEX "IDAF"."IDX_HST_FISC_MULTA_FISC" ON "IDAF"."HST_FISC_MULTA" ("FISCALIZACAO_ID") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 131072 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
  TABLESPACE "IDAF_TBS" ;
 

  CREATE INDEX "IDAF"."IDX_HST_FISC_MULTA_HST" ON "IDAF"."HST_FISC_MULTA" ("ID_HST") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 131072 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
  TABLESPACE "IDAF_TBS" ;
  
  CREATE INDEX "IDAF"."IDX_HST_FISC_MULTA_CODREC" ON "IDAF"."HST_FISC_MULTA" ("CODIGO_RECEITA_ID") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 131072 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
  TABLESPACE "IDAF_TBS" ;
 

  CREATE INDEX "IDAF"."IDX_HST_FISC_MULTA_TID" ON "IDAF"."HST_FISC_MULTA" ("TID") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 262144 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
  TABLESPACE "IDAF_TBS" ;
 
