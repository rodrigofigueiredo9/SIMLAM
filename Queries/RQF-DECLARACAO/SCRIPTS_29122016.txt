--------------------------------------------------------
--  DDL for Sequence SEQ_DECLARACAO_ADICIONAL
--------------------------------------------------------

CREATE SEQUENCE  "IDAF"."SEQ_DECLARACAO_ADICIONAL"  MINVALUE 1 MAXVALUE 999999999999999999999999999 INCREMENT BY 1 START WITH 268 NOCACHE  NOORDER


ALTER TABLE "IDAF".lov_cultivar_declara_adicional
ADD OUTRO_ESTADO CHAR(1)
CONSTRAINT CONS_OUTRO_ESTADO CHECK (OUTRO_ESTADO IN ('1','0')) 

ALTER TABLE "IDAF".tab_ptv_outrouf
     add ( DECLARACAOCAO_ADICIONAL VARCHAR2(4000), DECLARACAO_ADICIONAL_FORMATADO VARCHAR2(4000) )



CREATE SEQUENCE  "IDAF"."SEQ_TAB_PTV_OUTROUF_DECLARACAO"  MINVALUE 1 MAXVALUE 999999999999999999999999999 INCREMENT BY 1 START WITH 268 NOCACHE  NOORDER  

CREATE TABLE "IDAF".tab_ptv_outrouf_declaracao
(
 ID number(38) not null,
 TID VARCHAR2(36) NOT null,
 PTV NUMBER(38) NOT NULL,
 DECLARACAO_ADICIONAL NUMBER(38) NOT NULL,
 CULTIVAR NUMBER(38) NOT NULL,
 PRAGA NUMBER(38) NOT NULL
);





insert into "IDAF".lov_autenticacao_permissao(id,nome,codigo,funcionario_tipo,descricao,grupo,tipo)
values(380,'Declaração Adicional','DeclaracaoAdicional', 3,'Declaração Adicional','Configurar Vegetal',1)


insert into  "IDAF".tab_autenticacao_papel_perm(id,papel,permissao,tid)
values(15413, 40, 380,'b76c74d3-cbdb-4253-b74b-e7eb23a3b10e')

insert into  "IDAF".tab_autenticacao_papel_perm(id,papel,permissao,tid)
values(15415, 12, 380,'070e172a-de24-4fef-85d7-b42d3a0f2c5a')


update "IDAF".lov_cultivar_declara_adicional set OUTRO_ESTADO = '0' where OUTRO_ESTADO is null




