
alter table IDAFCREDENCIADOGEO.GEO_AIATIV add TIPO_EXPLORACAO VARCHAR2(50 BYTE);
COMMENT ON COLUMN "IDAFCREDENCIADOGEO"."GEO_AIATIV"."TIPO_EXPLORACAO" IS 'Texto do tipo de exploracao associado ao ponto de atividade.';

alter table IDAFCREDENCIADOGEO.GEO_LATIV add TIPO_EXPLORACAO VARCHAR2(50 BYTE);
COMMENT ON COLUMN "IDAFCREDENCIADOGEO"."GEO_LATIV"."TIPO_EXPLORACAO" IS 'Texto do tipo de exploracao associado ao ponto de atividade.';

alter table IDAFCREDENCIADOGEO.GEO_PATIV add TIPO_EXPLORACAO VARCHAR2(50 BYTE);
COMMENT ON COLUMN "IDAFCREDENCIADOGEO"."GEO_PATIV"."TIPO_EXPLORACAO" IS 'Texto do tipo de exploracao associado ao ponto de atividade.';