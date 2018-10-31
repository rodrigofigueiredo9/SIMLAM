--CREATE TABLE LOV_TIPO_EXPLORACAO
CREATE TABLE "IDAFGEO"."LOV_TIPO_EXPLORACAO" 
   (	"CHAVE" VARCHAR2(10 BYTE), 
	"TEXTO" VARCHAR2(50 BYTE), 
	"TIPO_ATIVIDADE" NUMERIC(38,0),
	 CONSTRAINT "PK_LOV_TIPO_EXPLORACAO" PRIMARY KEY ("CHAVE")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
  TABLESPACE "IDAFGEO_TBS"  ENABLE
   ) PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
  TABLESPACE "IDAFGEO_TBS" ;
 

   COMMENT ON COLUMN "IDAFGEO"."LOV_TIPO_EXPLORACAO"."CHAVE" IS 'Chave primaria para LOV_TIPO_EXPLORACAO';
 
   COMMENT ON COLUMN "IDAFGEO"."LOV_TIPO_EXPLORACAO"."TEXTO" IS 'Valor da coluna';
   
   COMMENT ON COLUMN "IDAFGEO"."LOV_TIPO_EXPLORACAO"."TIPO_ATIVIDADE" IS 'Código do tipo de atividade cadastrado no IBAMA';
 
   COMMENT ON TABLE "IDAFGEO"."LOV_TIPO_EXPLORACAO"  IS 'Lista de valores para as tabelas do desenhador';
 
 
insert into idafgeo.LOV_TIPO_EXPLORACAO (chave, texto, tipo_atividade)
values ('AUS', 'AUS -  Uso Alternativo do Solo', 370);
insert into idafgeo.LOV_TIPO_EXPLORACAO (chave, texto, tipo_atividade)
values ('CAI', 'CAI - Corte de Árvore Isolada', 374);
insert into idafgeo.LOV_TIPO_EXPLORACAO (chave, texto, tipo_atividade)
values ('EFP', 'EFP - Exploração de Floresta Plantada', 929);


-- INSERT INTO TAB_FEICAO_COLUNAS
insert into idafgeo.tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) 
values (31,'TIPO_EXPLORACAO', 4,1,'Tipo de Exploração',1,1,1, 'LOV_TIPO_EXPLORACAO', 'CHAVE');

insert into idafgeo.tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) 
values (34,'TIPO_EXPLORACAO', 4,1,'Tipo de Exploração',1,1,1, 'LOV_TIPO_EXPLORACAO', 'CHAVE');

-- ADD COLUMN TIPO_EXPLORACAO INTO TABELAS DO GEO
alter table IDAFGEO.DES_PATIV add TIPO_EXPLORACAO VARCHAR2(50 BYTE);
COMMENT ON COLUMN "IDAFGEO"."DES_PATIV"."TIPO_EXPLORACAO" IS 'Texto do tipo de exploracao associado ao ponto de atividade.';

alter table IDAFGEO.FE_DES_AIATIV add TIPO_EXPLORACAO VARCHAR2(50 BYTE);
COMMENT ON COLUMN "IDAFGEO"."FE_DES_AIATIV"."TIPO_EXPLORACAO" IS 'Texto do tipo de exploracao associado ao ponto de atividade.';

alter table IDAFGEO.TMP_AATIV add TIPO_EXPLORACAO VARCHAR2(50 BYTE);
COMMENT ON COLUMN "IDAFGEO"."TMP_AATIV"."TIPO_EXPLORACAO" IS 'Texto do tipo de exploracao associado ao ponto de atividade.';

alter table IDAFGEO.DES_AIATIV add TIPO_EXPLORACAO VARCHAR2(50 BYTE);
COMMENT ON COLUMN "IDAFGEO"."DES_AIATIV"."TIPO_EXPLORACAO" IS 'Texto do tipo de exploracao associado ao ponto de atividade.';

alter table IDAFGEO.DES_LATIV add TIPO_EXPLORACAO VARCHAR2(50 BYTE);
COMMENT ON COLUMN "IDAFGEO"."DES_LATIV"."TIPO_EXPLORACAO" IS 'Texto do tipo de exploracao associado ao ponto de atividade.';

alter table IDAFGEO.FE_DES_PATIV add TIPO_EXPLORACAO VARCHAR2(50 BYTE);
COMMENT ON COLUMN "IDAFGEO"."FE_DES_PATIV"."TIPO_EXPLORACAO" IS 'Texto do tipo de exploracao associado ao ponto de atividade.';

alter table IDAFGEO.TMP_LATIV add TIPO_EXPLORACAO VARCHAR2(50 BYTE);
COMMENT ON COLUMN "IDAFGEO"."TMP_LATIV"."TIPO_EXPLORACAO" IS 'Texto do tipo de exploracao associado ao ponto de atividade.';

alter table IDAFGEO.TMP_PATIV add TIPO_EXPLORACAO VARCHAR2(50 BYTE);
COMMENT ON COLUMN "IDAFGEO"."TMP_PATIV"."TIPO_EXPLORACAO" IS 'Texto do tipo de exploracao associado ao ponto de atividade.';

alter table IDAFGEO.GEO_AIATIV add TIPO_EXPLORACAO VARCHAR2(50 BYTE);
COMMENT ON COLUMN "IDAFGEO"."GEO_AIATIV"."TIPO_EXPLORACAO" IS 'Texto do tipo de exploracao associado ao ponto de atividade.';

alter table IDAFGEO.FE_GEO_AATIV add TIPO_EXPLORACAO VARCHAR2(50 BYTE);
COMMENT ON COLUMN "IDAFGEO"."FE_GEO_AATIV"."TIPO_EXPLORACAO" IS 'Texto do tipo de exploracao associado ao ponto de atividade.';

alter table IDAFGEO.GEO_PATIV add TIPO_EXPLORACAO VARCHAR2(50 BYTE);
COMMENT ON COLUMN "IDAFGEO"."GEO_PATIV"."TIPO_EXPLORACAO" IS 'Texto do tipo de exploracao associado ao ponto de atividade.';

alter table IDAFGEO.GEO_AATIV add TIPO_EXPLORACAO VARCHAR2(50 BYTE);
COMMENT ON COLUMN "IDAFGEO"."GEO_AATIV"."TIPO_EXPLORACAO" IS 'Texto do tipo de exploracao associado ao ponto de atividade.';

alter table IDAFGEO.HST_PATIV add TIPO_EXPLORACAO VARCHAR2(50 BYTE);
COMMENT ON COLUMN "IDAFGEO"."HST_PATIV"."TIPO_EXPLORACAO" IS 'Texto do tipo de exploracao associado ao ponto de atividade.';

alter table IDAFGEO.TMP_AIATIV add TIPO_EXPLORACAO VARCHAR2(50 BYTE);
COMMENT ON COLUMN "IDAFGEO"."TMP_AIATIV"."TIPO_EXPLORACAO" IS 'Texto do tipo de exploracao associado ao ponto de atividade.';

alter table IDAFGEO.INV_TMP_AATIV add TIPO_EXPLORACAO VARCHAR2(50 BYTE);
COMMENT ON COLUMN "IDAFGEO"."INV_TMP_AATIV"."TIPO_EXPLORACAO" IS 'Texto do tipo de exploracao associado ao ponto de atividade.';

alter table IDAFGEO.GEO_LATIV add TIPO_EXPLORACAO VARCHAR2(50 BYTE);
COMMENT ON COLUMN "IDAFGEO"."GEO_LATIV"."TIPO_EXPLORACAO" IS 'Texto do tipo de exploracao associado ao ponto de atividade.';

alter table IDAFGEO.HST_AATIV add TIPO_EXPLORACAO VARCHAR2(50 BYTE);
COMMENT ON COLUMN "IDAFGEO"."HST_AATIV"."TIPO_EXPLORACAO" IS 'Texto do tipo de exploracao associado ao ponto de atividade.';

alter table IDAFGEO.HST_AIATIV add TIPO_EXPLORACAO VARCHAR2(50 BYTE);
COMMENT ON COLUMN "IDAFGEO"."HST_AIATIV"."TIPO_EXPLORACAO" IS 'Texto do tipo de exploracao associado ao ponto de atividade.';

alter table IDAFGEO.HST_LATIV add TIPO_EXPLORACAO VARCHAR2(50 BYTE);
COMMENT ON COLUMN "IDAFGEO"."HST_LATIV"."TIPO_EXPLORACAO" IS 'Texto do tipo de exploracao associado ao ponto de atividade.';

alter table IDAFGEO.FE_DES_LATIV add TIPO_EXPLORACAO VARCHAR2(50 BYTE);
COMMENT ON COLUMN "IDAFGEO"."FE_DES_LATIV"."TIPO_EXPLORACAO" IS 'Texto do tipo de exploracao associado ao ponto de atividade.';

alter table IDAFGEO.FE_DES_AATIV add TIPO_EXPLORACAO VARCHAR2(50 BYTE);
COMMENT ON COLUMN "IDAFGEO"."FE_DES_AATIV"."TIPO_EXPLORACAO" IS 'Texto do tipo de exploracao associado ao ponto de atividade.';

alter table IDAFGEO.DES_AATIV add TIPO_EXPLORACAO VARCHAR2(50 BYTE);
COMMENT ON COLUMN "IDAFGEO"."DES_AATIV"."TIPO_EXPLORACAO" IS 'Texto do tipo de exploracao associado ao ponto de atividade.';

alter table IDAFGEO.FE_GEO_AIATIV add TIPO_EXPLORACAO VARCHAR2(50 BYTE);
COMMENT ON COLUMN "IDAFGEO"."FE_GEO_AIATIV"."TIPO_EXPLORACAO" IS 'Texto do tipo de exploracao associado ao ponto de atividade.';

alter table IDAFGEO.FE_GEO_LATIV add TIPO_EXPLORACAO VARCHAR2(50 BYTE);
COMMENT ON COLUMN "IDAFGEO"."FE_GEO_LATIV"."TIPO_EXPLORACAO" IS 'Texto do tipo de exploracao associado ao ponto de atividade.';

alter table IDAFGEO.FE_GEO_PATIV add TIPO_EXPLORACAO VARCHAR2(50 BYTE);
COMMENT ON COLUMN "IDAFGEO"."FE_GEO_PATIV"."TIPO_EXPLORACAO" IS 'Texto do tipo de exploracao associado ao ponto de atividade.';

alter table IDAFGEO.FE_TMP_AATIV add TIPO_EXPLORACAO VARCHAR2(50 BYTE);
COMMENT ON COLUMN "IDAFGEO"."FE_TMP_AATIV"."TIPO_EXPLORACAO" IS 'Texto do tipo de exploracao associado ao ponto de atividade.';

alter table IDAFGEO.FE_TMP_AIATIV add TIPO_EXPLORACAO VARCHAR2(50 BYTE);
COMMENT ON COLUMN "IDAFGEO"."FE_TMP_AIATIV"."TIPO_EXPLORACAO" IS 'Texto do tipo de exploracao associado ao ponto de atividade.';

alter table IDAFGEO.FE_TMP_LATIV add TIPO_EXPLORACAO VARCHAR2(50 BYTE);
COMMENT ON COLUMN "IDAFGEO"."FE_TMP_LATIV"."TIPO_EXPLORACAO" IS 'Texto do tipo de exploracao associado ao ponto de atividade.';

alter table IDAFGEO.FE_TMP_PATIV add TIPO_EXPLORACAO VARCHAR2(50 BYTE);
COMMENT ON COLUMN "IDAFGEO"."FE_TMP_PATIV"."TIPO_EXPLORACAO" IS 'Texto do tipo de exploracao associado ao ponto de atividade.';

alter table IDAFGEO.VAL_PATIV add TIPO_EXPLORACAO VARCHAR2(50 BYTE);
COMMENT ON COLUMN "IDAFGEO"."VAL_PATIV"."TIPO_EXPLORACAO" IS 'Texto do tipo de exploracao associado ao ponto de atividade.';

alter table IDAFGEO.VAL_LATIV add TIPO_EXPLORACAO VARCHAR2(50 BYTE);
COMMENT ON COLUMN "IDAFGEO"."VAL_LATIV"."TIPO_EXPLORACAO" IS 'Texto do tipo de exploracao associado ao ponto de atividade.';

alter table IDAFGEO.VAL_AATIV add TIPO_EXPLORACAO VARCHAR2(50 BYTE);
COMMENT ON COLUMN "IDAFGEO"."VAL_AATIV"."TIPO_EXPLORACAO" IS 'Texto do tipo de exploracao associado ao ponto de atividade.';

alter table IDAFGEO.VAL_AIATIV add TIPO_EXPLORACAO VARCHAR2(50 BYTE);
COMMENT ON COLUMN "IDAFGEO"."VAL_AIATIV"."TIPO_EXPLORACAO" IS 'Texto do tipo de exploracao associado ao ponto de atividade.';

alter table IDAFGEO.FEC_GEO_AATIV add TIPO_EXPLORACAO VARCHAR2(50 BYTE);
COMMENT ON COLUMN "IDAFGEO"."FEC_GEO_AATIV"."TIPO_EXPLORACAO" IS 'Texto do tipo de exploracao associado ao ponto de atividade.';

alter table IDAFGEO.FEC_GEO_AIATIV add TIPO_EXPLORACAO VARCHAR2(50 BYTE);
COMMENT ON COLUMN "IDAFGEO"."FEC_GEO_AIATIV"."TIPO_EXPLORACAO" IS 'Texto do tipo de exploracao associado ao ponto de atividade.';

alter table IDAFGEO.FEC_GEO_LATIV add TIPO_EXPLORACAO VARCHAR2(50 BYTE);
COMMENT ON COLUMN "IDAFGEO"."FEC_GEO_LATIV"."TIPO_EXPLORACAO" IS 'Texto do tipo de exploracao associado ao ponto de atividade.';

alter table IDAFGEO.FEC_GEO_PATIV add TIPO_EXPLORACAO VARCHAR2(50 BYTE);
COMMENT ON COLUMN "IDAFGEO"."FEC_GEO_PATIV"."TIPO_EXPLORACAO" IS 'Texto do tipo de exploracao associado ao ponto de atividade.';

--PERMISSÃO DE CONSULTA PARA IDAF
grant SELECT on "IDAFGEO"."LOV_TIPO_EXPLORACAO" to "IDAF" ;

alter table IDAFGEO.TMP_RASC_TRACKMAKER add TIPOEXP VARCHAR2(50 BYTE);
COMMENT ON COLUMN "IDAFGEO"."TMP_RASC_TRACKMAKER"."TIPOEXP" IS 'Texto do tipo de exploracao associado ao ponto de atividade.';


--CRIA TABELA PARA PONTO
 CREATE TABLE "IDAFGEO"."LOV_TIPO_EXPLORACAO_PONTO" 
   (	"CHAVE" VARCHAR2(10 BYTE), 
	"TEXTO" VARCHAR2(50 BYTE), 
	"TIPO_ATIVIDADE" NUMBER(38,0), 
	 CONSTRAINT "PK_LOV_TIPO_EXPLORACAO_PONTO" PRIMARY KEY ("CHAVE")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
  TABLESPACE "IDAFGEO_TBS"  ENABLE
   ) PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
  TABLESPACE "IDAFGEO_TBS" ;
 

   COMMENT ON COLUMN "IDAFGEO"."LOV_TIPO_EXPLORACAO_PONTO"."CHAVE" IS 'Chave primaria para LOV_TIPO_EXPLORACAO';
 
   COMMENT ON COLUMN "IDAFGEO"."LOV_TIPO_EXPLORACAO_PONTO"."TEXTO" IS 'Valor da coluna';
 
   COMMENT ON COLUMN "IDAFGEO"."LOV_TIPO_EXPLORACAO_PONTO"."TIPO_ATIVIDADE" IS 'Código do tipo de atividade cadastrado no IBAMA';
 
   COMMENT ON TABLE "IDAFGEO"."LOV_TIPO_EXPLORACAO_PONTO"  IS 'Lista de valores para as tabelas do desenhador';
 
 INSERT INTO IDAFGEO.LOV_TIPO_EXPLORACAO_PONTO
 SELECT * FROM IDAFGEO.LOV_TIPO_EXPLORACAO WHERE TIPO_ATIVIDADE = 374 
 AND NOT EXISTS (SELECT 1 FROM IDAFGEO.LOV_TIPO_EXPLORACAO_PONTO);
 
 UPDATE IDAFGEO.TAB_FEICAO_COLUNAS SET TABELA_REFERENCIADA = 'LOV_TIPO_EXPLORACAO_PONTO' WHERE COLUNA = 'TIPO_EXPLORACAO' AND FEICAO = 34;