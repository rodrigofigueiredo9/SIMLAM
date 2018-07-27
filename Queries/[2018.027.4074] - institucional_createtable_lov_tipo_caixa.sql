CREATE TABLE "IDAF"."LOV_TIPO_CAIXA" 
   (	"ID" NUMBER(38,0) NOT NULL ENABLE, 
	"TEXTO" VARCHAR2(60 BYTE) NOT NULL ENABLE, 
	 CONSTRAINT "PK_TIPO_CAIXA" PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
  TABLESPACE "IDAF_TBS"  ENABLE
   ) PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
  TABLESPACE "IDAF_TBS" ;

INSERT INTO LOV_TIPO_CAIXA VALUES(1, 'Madeira');
INSERT INTO LOV_TIPO_CAIXA VALUES(2, 'Pl�stico');
INSERT INTO LOV_TIPO_CAIXA VALUES(3, 'Papel�o');

COMMENT ON COLUMN "IDAF"."LOV_TIPO_CAIXA"."ID" IS 'Chave prim�ria de identifica��o da tabela.';
COMMENT ON COLUMN "IDAF"."LOV_TIPO_CAIXA"."TEXTO" IS 'Texto com os tipos de caixas.';
COMMENT ON TABLE "IDAF"."LOV_TIPO_CAIXA"  IS 'Tabela de tipos de caixas.';

CREATE TABLE "IDAF"."TAB_NF_CAIXA" 
   (	"ID" NUMBER(38,0) NOT NULL ENABLE, 
	"TID" VARCHAR2(36 BYTE) NOT NULL ENABLE, 
	"NUMERO" VARCHAR2(36 BYTE) NOT NULL ENABLE, 
	"TIPO_CAIXA" NUMBER(2,0) NOT NULL ENABLE, 
	"SALDO_INICIAL" NUMBER(38,0) NOT NULL ENABLE, 
	 CONSTRAINT "PK_TAB_NF_CAIXA" PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
  TABLESPACE "IDAF_TBS"  ENABLE, 
	 CONSTRAINT "FK_TIPO_CAIXA" FOREIGN KEY ("TIPO_CAIXA")
	  REFERENCES "IDAF"."LOV_TIPO_CAIXA" ("ID") ENABLE
   ) PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
  TABLESPACE "IDAF_TBS" ;
  
COMMENT ON COLUMN "IDAF"."TAB_NF_CAIXA"."ID" IS 'Chave prim�ria de identifica��o da tabela.';  
COMMENT ON COLUMN "IDAF"."TAB_NF_CAIXA"."TID" IS 'Id transacional.Esse valor garante a liga��o entre as tabelas relacionadas com essa transaction.';  
COMMENT ON COLUMN "IDAF"."TAB_NF_CAIXA"."NUMERO" IS 'N�mero da Nota Fiscal de caixa.';  
COMMENT ON COLUMN "IDAF"."TAB_NF_CAIXA"."TIPO_CAIXA" IS 'Chave estrangeira para lov_tipo_caixa. Campo(ID).';  
COMMENT ON COLUMN "IDAF"."TAB_NF_CAIXA"."SALDO_INICIAL" IS 'Total de saldo de caixas quando cadastrou a Nota Fiscal.';  
COMMENT ON TABLE "IDAF"."TAB_NF_CAIXA"  IS 'Tabela de Nota Fiscal de caixas.';

grant all on "IDAF"."TAB_NF_CAIXA" to "IDAFCREDENCIADO" ;

CREATE TABLE "IDAF"."TAB_PTV_NF_CAIXA" 
   (	"ID" NUMBER(38,0) NOT NULL ENABLE, 
	"TID" VARCHAR2(36 BYTE) NOT NULL ENABLE, 
	"PTV" NUMBER(38,0) NOT NULL ENABLE, 
	"NF_CAIXA" NUMBER(38,0) NOT NULL ENABLE, 
	"SALDO_ATUAL" NUMBER(38,0) NOT NULL ENABLE, 
  "NUMERO_CAIXAS" NUMBER(38,0) NOT NULL ENABLE,
	 CONSTRAINT "PK_TAB_PTV_NF_CAIXA" PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
  TABLESPACE "IDAF_TBS"  ENABLE, 
	 CONSTRAINT "FK_PTV_NF_CAIXA_PTV" FOREIGN KEY ("PTV")
	  REFERENCES "IDAF"."TAB_PTV" ("ID") ENABLE, 
	 CONSTRAINT "FK_NF_CAIXA" FOREIGN KEY ("NF_CAIXA")
	  REFERENCES "IDAF"."TAB_NF_CAIXA" ("ID") ENABLE
   ) PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
  TABLESPACE "IDAF_TBS" ;
  
COMMENT ON COLUMN "IDAF"."TAB_PTV_NF_CAIXA"."ID" IS 'Chave prim�ria de identifica��o da tabela.';  
COMMENT ON COLUMN "IDAF"."TAB_PTV_NF_CAIXA"."TID" IS 'Id transacional.Esse valor garante a liga��o entre as tabelas relacionadas com essa transaction.';  
COMMENT ON COLUMN "IDAF"."TAB_PTV_NF_CAIXA"."PTV" IS 'Chave estrangeira para tab_ptv. Campo(ID).';  
COMMENT ON COLUMN "IDAF"."TAB_PTV_NF_CAIXA"."NF_CAIXA" IS 'Chave estrangeira para tab_nf_caixa. Campo(ID).';  
COMMENT ON COLUMN "IDAF"."TAB_PTV_NF_CAIXA"."SALDO_ATUAL" IS 'Saldo de caixas antes de serem utilizadas.';  
COMMENT ON COLUMN "IDAF"."TAB_PTV_NF_CAIXA"."NUMERO_CAIXAS" IS 'Saldo de caixas utilizado.';  

CREATE TABLE "IDAF"."TAB_PTV_SEM_DOC_ORIGEM" 
   (	"ID" NUMBER(38,0) NOT NULL ENABLE, 
	"TID" VARCHAR2(36 BYTE) NOT NULL ENABLE, 	
  "PTV" NUMBER(38,0) NULL, 
	"EMPREENDIMENTO" VARCHAR2(150 BYTE) NULL, 
	"ENDERECO" VARCHAR2(150 BYTE) NULL, 
	"UF" NUMBER(38,0) NULL, 
	"MUNICIPIO" NUMBER(38,0) NULL, 
  "PRODUTOR" VARCHAR2(50 BYTE) NULL, 
  "RESPONSAVEL" VARCHAR2(50 BYTE) NULL, 
	"CPF_CNPJ" VARCHAR2(20 BYTE) NULL, 
	 CONSTRAINT "PK_TAB_PTV_SEM_DOC_ORIGEM" PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
  TABLESPACE "IDAF_TBS"  ENABLE, 
	 CONSTRAINT "FK_PTV_SEM_DOC_PTV" FOREIGN KEY ("PTV")
	  REFERENCES "IDAF"."TAB_PTV" ("ID") ENABLE, 
	 CONSTRAINT "FK_PTV_SEM_DOC_ESTADO" FOREIGN KEY ("UF")
	  REFERENCES "IDAF"."LOV_ESTADO" ("ID") ENABLE, 
	 CONSTRAINT "FK_PTV_SEM_DOC_MUNICIPIO" FOREIGN KEY ("MUNICIPIO")
	  REFERENCES "IDAF"."LOV_MUNICIPIO" ("ID") ENABLE
   ) PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
  TABLESPACE "IDAF_TBS" ;
  
COMMENT ON TABLE "IDAF"."TAB_PTV_SEM_DOC_ORIGEM"  IS 'Tabela dos documentos de origem tipo CF/CR, TR e sem documento de origem.';  
COMMENT ON COLUMN "IDAF"."TAB_PTV_SEM_DOC_ORIGEM"."ID" IS 'Chave prim�ria de identifica��o da tabela.';  
COMMENT ON COLUMN "IDAF"."TAB_PTV_SEM_DOC_ORIGEM"."TID" IS 'Id transacional.Esse valor garante a liga��o entre as tabelas relacionadas com essa transaction.';  
COMMENT ON COLUMN "IDAF"."TAB_PTV_SEM_DOC_ORIGEM"."PTV" IS 'Chave estrangeira para tab_ptv. Campo(ID).'; 
COMMENT ON COLUMN "IDAF"."TAB_PTV_SEM_DOC_ORIGEM"."EMPREENDIMENTO" IS 'Nome do empreendimento quando for TF ou CF/CFR.';    
COMMENT ON COLUMN "IDAF"."TAB_PTV_SEM_DOC_ORIGEM"."ENDERECO" IS 'Endere�o do empreendimento.';    
COMMENT ON COLUMN "IDAF"."TAB_PTV_SEM_DOC_ORIGEM"."UF" IS 'Chave estrangeira para lov_estado. Campo(ID).';   
COMMENT ON COLUMN "IDAF"."TAB_PTV_SEM_DOC_ORIGEM"."MUNICIPIO" IS 'Chave estrangeira para lov_municipio. Campo(ID).';   
COMMENT ON COLUMN "IDAF"."TAB_PTV_SEM_DOC_ORIGEM"."PRODUTOR" IS 'Nome do produtor.';   
COMMENT ON COLUMN "IDAF"."TAB_PTV_SEM_DOC_ORIGEM"."RESPONSAVEL" IS 'Nome do responsavel quando for TF ou CF/CFR.';   
COMMENT ON COLUMN "IDAF"."TAB_PTV_SEM_DOC_ORIGEM"."CPF_CNPJ" IS 'CPF/CNPJ do produtor.';   
	
	
CREATE SEQUENCE  "IDAF"."SEQ_NF_CAIXA"  MINVALUE 1 MAXVALUE 999999999999999999999999999 INCREMENT BY 1 START WITH 747 NOCACHE  NOORDER  NOCYCLE ;
CREATE SEQUENCE  "IDAF"."SEQ_PTV_NF_CAIXA"  MINVALUE 1 MAXVALUE 999999999999999999999999999 INCREMENT BY 1 START WITH 747 NOCACHE  NOORDER  NOCYCLE ;
CREATE SEQUENCE  "IDAF"."SEQ_PTV_SEM_DOC_ORIGEM"  MINVALUE 1 MAXVALUE 999999999999999999999999999 INCREMENT BY 1 START WITH 747 NOCACHE  NOORDER  NOCYCLE ;
 
ALTER TABLE TAB_CULTURA_CULTIVAR ADD(NF_CAIXA_OBRIGATORIA NUMBER(1,0));

UPDATE TAB_CULTURA_CULTIVAR SET NF_CAIXA_OBRIGATORIA = 0;
UPDATE TAB_CULTURA_CULTIVAR SET NF_CAIXA_OBRIGATORIA = 1 WHERE CULTURA = 33;

ALTER TABLE HST_PTV_PRODUTO MODIFY ORIGEM_TIPO_TEXTO	VARCHAR2(25 BYTE);
   