-- Inserts e Updates para o histórico de nota fiscal de caixa funcionar (institucional)
insert into Lov_Historico_Artefato (id, texto)
values (
	(select max(id)+1 from lov_historico_artefatos),
	'notafiscalcaixa'
);

insert into LOV_HISTORICO_ARTEFATOS_ACOES (id, acao, artefato)
values(
	(select max(id)+1 from LOV_HISTORICO_ARTEFATOS_ACOES),
	(select id from lov_historico_acao where texto = 'Criar'),
	(select id from Lov_Historico_Artefato where texto = 'notafiscalcaixa')
);
insert into LOV_HISTORICO_ARTEFATOS_ACOES (id, acao, artefato)
values(
	(select max(id)+1 from LOV_HISTORICO_ARTEFATOS_ACOES),
	(select id from lov_historico_acao where texto = 'Atualizar'),
	(select id from Lov_Historico_Artefato where texto = 'notafiscalcaixa')
);
insert into LOV_HISTORICO_ARTEFATOS_ACOES (id, acao, artefato)
values(
	(select max(id)+1 from LOV_HISTORICO_ARTEFATOS_ACOES),
	(select id from lov_historico_acao where texto = 'Excluir'),
	(select id from Lov_Historico_Artefato where texto = 'notafiscalcaixa')
);


------ Criação de sequence e tabela de histórico de nota fiscal de caixa (institucional)

CREATE SEQUENCE  "IDAF"."SEQ_HST_NF_CAIXA"  MINVALUE 1 MAXVALUE 999999999999999999999999999 INCREMENT BY 1 START WITH 57542 NOCACHE  NOORDER  NOCYCLE ;

CREATE TABLE "IDAF"."HST_NF_CAIXA" 
( "ID" NUMBER(38,0) NOT NULL ENABLE,
  "TID" VARCHAR2(36 BYTE) NOT NULL ENABLE,
  "NF_CAIXA_ID" NUMBER(38,0) NOT NULL ENABLE,
  "NUMERO" VARCHAR2(36 BYTE) NOT NULL ENABLE, 
  "TIPO_CAIXA_ID" NUMBER(2,0) NOT NULL ENABLE, 
  "TIPO_CAIXA_TEXTO" VARCHAR2(60 BYTE) NOT NULL ENABLE, 
  "SALDO_INICIAL" NUMBER(38,0) NOT NULL ENABLE, 
  "EXECUTOR_ID" NUMBER(38,0) NOT NULL ENABLE,
  "EXECUTOR_TID" VARCHAR2(36 BYTE) NOT NULL ENABLE,
  "EXECUTOR_NOME" VARCHAR2(80 BYTE) NOT NULL ENABLE,
  "EXECUTOR_LOGIN" VARCHAR2(30 BYTE) NOT NULL ENABLE,
  "EXECUTOR_TIPO_ID" NUMBER(38,0) NOT NULL ENABLE,
  "EXECUTOR_TIPO_TEXTO" VARCHAR2(30 BYTE) NOT NULL ENABLE,
  "ACAO_EXECUTADA" NUMBER(38,0) NOT NULL ENABLE,
  "DATA_EXECUCAO" TIMESTAMP(6) NOT NULL ENABLE,
  
  CONSTRAINT "PK_HST_NF_CAIXA" PRIMARY KEY ("ID") USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645 PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT) TABLESPACE "IDAF_TBS" ENABLE, 
  
  CONSTRAINT "FK_HST_PTV_NFCAIXA_ACAO" FOREIGN KEY ("ACAO_EXECUTADA") REFERENCES "IDAF"."LOV_HISTORICO_ARTEFATOS_ACOES" ("ID") ENABLE
)
PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 NOCOMPRESS LOGGING STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645 PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT) TABLESPACE "IDAF_TBS" ;

COMMENT ON COLUMN "IDAF"."HST_NF_CAIXA"."ID" IS 'Chave primária de identificação da tabela.';
COMMENT ON COLUMN "IDAF"."HST_NF_CAIXA"."TID" IS 'Id transacional.Esse valor garante a ligação entre as tabelas relacionadas com essa transaction.';
COMMENT ON COLUMN "IDAF"."HST_NF_CAIXA"."NF_CAIXA_ID" IS 'Chave estrangeira para tab_nf_caixa (ID).';
COMMENT ON COLUMN "IDAF"."HST_NF_CAIXA"."NUMERO" IS 'Número da Nota Fiscal de caixa.';
COMMENT ON COLUMN "IDAF"."HST_NF_CAIXA"."TIPO_CAIXA_ID" IS 'Chave estrangeira para lov_tipo_caixa. Campo(ID).';
COMMENT ON COLUMN "IDAF"."HST_NF_CAIXA"."TIPO_CAIXA_TEXTO" IS 'Campo (Texto) da tabela lov_tipo_caixa.';
COMMENT ON COLUMN "IDAF"."HST_NF_CAIXA"."SALDO_INICIAL" IS 'Total de saldo de caixas quando cadastrou a Nota Fiscal.';
COMMENT ON COLUMN "IDAF"."HST_NF_CAIXA"."EXECUTOR_ID" IS 'Chave estrangeira para tab_funcionario. Campo(ID).';
COMMENT ON COLUMN "IDAF"."HST_NF_CAIXA"."EXECUTOR_TID" IS 'Referência ao campo (TID) da tabela tab_funcionario.';
COMMENT ON COLUMN "IDAF"."HST_NF_CAIXA"."EXECUTOR_NOME" IS 'Nome do funcionário que executou a ação.';
COMMENT ON COLUMN "IDAF"."HST_NF_CAIXA"."EXECUTOR_LOGIN" IS 'Login do funcionário que executou a ação.';
COMMENT ON COLUMN "IDAF"."HST_NF_CAIXA"."EXECUTOR_TIPO_ID" IS 'Chave estrangeira para lov_executor_tipo. Campo(ID).';
COMMENT ON COLUMN "IDAF"."HST_NF_CAIXA"."EXECUTOR_TIPO_TEXTO" IS 'Texto do tipo do funcionário que executou a ação.';
COMMENT ON COLUMN "IDAF"."HST_NF_CAIXA"."ACAO_EXECUTADA" IS 'Ação que foi disparada pelo sistema a qual gerou essa linha de histórico.';
COMMENT ON COLUMN "IDAF"."HST_NF_CAIXA"."DATA_EXECUCAO" IS 'Data em que foi gerada essa linha de histórico. Tabela lov_historico_artefatos_acoes.';

COMMENT ON TABLE "IDAF"."HST_NF_CAIXA"  IS 'Tabela de histórico de Nota Fiscal de caixas.';

grant all on "IDAF"."HST_NF_CAIXA" to "IDAFCREDENCIADO" ;



------ Criação de sequence e tabela de histórico de nota fiscal de caixa do PTV (hst_ptv_nf_caixa) (institucional)

CREATE SEQUENCE  "IDAF"."SEQ_HST_PTV_NF_CAIXA"  MINVALUE 1 MAXVALUE 999999999999999999999999999 INCREMENT BY 1 START WITH 57542 NOCACHE  NOORDER  NOCYCLE ;

CREATE TABLE "IDAF"."HST_PTV_NF_CAIXA" 
( "ID" NUMBER(38,0) NOT NULL ENABLE,
  "TID" VARCHAR2(36 BYTE) NOT NULL ENABLE,
  "ID_HST" NUMBER(38,0) NOT NULL ENABLE,
  "PTV_NF_CAIXA_ID" NUMBER(38,0) NOT NULL ENABLE,
  "PTV_ID" NUMBER(38,0) NOT NULL ENABLE,
  "NF_CAIXA_ID" NUMBER(38,0) NOT NULL ENABLE,
  "NF_CAIXA_TID" VARCHAR2(36 BYTE) NOT NULL ENABLE,
  "SALDO_ATUAL" NUMBER(38,0) NOT NULL ENABLE, 
  "NUMERO_CAIXAS" NUMBER(38,0) NOT NULL ENABLE,
  
  CONSTRAINT "PK_HST_PTV_NF_CAIXA" PRIMARY KEY ("ID") USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645 PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT) TABLESPACE "IDAF_TBS" ENABLE, 
  
  CONSTRAINT "FK_HST_PTV_NFCAIXA" FOREIGN KEY ("ID_HST") REFERENCES "IDAF"."HST_PTV" ("ID") ENABLE
)
PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 NOCOMPRESS LOGGING STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645 PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT) TABLESPACE "IDAF_TBS" ;

COMMENT ON COLUMN "IDAF"."HST_PTV_NF_CAIXA"."ID" IS 'Chave primária de identificação da tabela.';
COMMENT ON COLUMN "IDAF"."HST_PTV_NF_CAIXA"."TID" IS 'Id transacional.Esse valor garante a ligação entre as tabelas relacionadas com essa transaction.';
COMMENT ON COLUMN "IDAF"."HST_PTV_NF_CAIXA"."ID_HST" IS 'Chave estrangeira da tabela hst_ptv.';
COMMENT ON COLUMN "IDAF"."HST_PTV_NF_CAIXA"."PTV_NF_CAIXA_ID" IS 'Chave estrangeira para tab_ptv_nf_caixa. Campo(ID)';
COMMENT ON COLUMN "IDAF"."HST_PTV_NF_CAIXA"."PTV_ID" IS 'Chave estrangeira para tab_ptv. Campo(ID)';
COMMENT ON COLUMN "IDAF"."HST_PTV_NF_CAIXA"."NF_CAIXA_ID" IS 'Chave estrangeira para tab_nf_caixa. Campo(ID).';
COMMENT ON COLUMN "IDAF"."HST_PTV_NF_CAIXA"."NF_CAIXA_TID" IS 'Referência ao campo (TID) da tabela tab_nf_caixa.';
COMMENT ON COLUMN "IDAF"."HST_PTV_NF_CAIXA"."SALDO_ATUAL" IS 'Saldo de caixas antes de serem utilizadas.';
COMMENT ON COLUMN "IDAF"."HST_PTV_NF_CAIXA"."NUMERO_CAIXAS" IS 'Saldo de caixas utilizado.';

COMMENT ON TABLE "IDAF"."HST_PTV_NF_CAIXA"  IS 'Tabela de histórico de Nota Fiscal de caixas do PTV.';