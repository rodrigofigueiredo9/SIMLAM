---Tabelas de Nota Fiscal de Caixa
CREATE SEQUENCE  "IDAFCREDENCIADO"."SEQ_PTV_NF_CAIXA"  MINVALUE 1 MAXVALUE 999999999999999999999999999 INCREMENT BY 1 START WITH 11 NOCACHE  NOORDER  NOCYCLE ;

CREATE TABLE "IDAFCREDENCIADO"."TAB_PTV_NF_CAIXA" 
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
	  REFERENCES "IDAFCREDENCIADO"."TAB_PTV" ("ID") ENABLE, 
	 CONSTRAINT "FK_NF_CAIXA" FOREIGN KEY ("NF_CAIXA")
	  REFERENCES "IDAF"."TAB_NF_CAIXA" ("ID") ENABLE
   ) PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT)
  TABLESPACE "IDAF_TBS" ;
 

   COMMENT ON COLUMN "IDAFCREDENCIADO"."TAB_PTV_NF_CAIXA"."ID" IS 'Chave primária de identificação da tabela.';
 
   COMMENT ON COLUMN "IDAFCREDENCIADO"."TAB_PTV_NF_CAIXA"."TID" IS 'Id transacional.Esse valor garante a ligação entre as tabelas relacionadas com essa transaction.';
 
   COMMENT ON COLUMN "IDAFCREDENCIADO"."TAB_PTV_NF_CAIXA"."PTV" IS 'Chave estrangeira para tab_ptv. Campo(ID).';
 
   COMMENT ON COLUMN "IDAFCREDENCIADO"."TAB_PTV_NF_CAIXA"."NF_CAIXA" IS 'Chave estrangeira para tab_nf_caixa. Campo(ID).';
 
   COMMENT ON COLUMN "IDAFCREDENCIADO"."TAB_PTV_NF_CAIXA"."SALDO_ATUAL" IS 'Saldo de caixas antes de serem utilizadas.';
 
   COMMENT ON COLUMN "IDAFCREDENCIADO"."TAB_PTV_NF_CAIXA"."NUMERO_CAIXAS" IS 'Saldo de caixas utilizado.';
   
   grant SELECT on "IDAFCREDENCIADO"."TAB_PTV_NF_CAIXA" to "IDAF" ;
 
 
---------------------------------

---Alterações no comunicador

alter table HST_PTV_COMUNI_CONVERSA
modify texto null;

alter table IDAFCREDENCIADO.tab_ptv add exibir_mensagem numeric(1,0);
alter table IDAFCREDENCIADO.tab_ptv add exibir_msg_credenciado numeric(1,0);
alter table IDAFCREDENCIADO.tab_ptv add local_fiscalizacao varchar2(500 byte);
alter table IDAFCREDENCIADO.tab_ptv add hora_fiscalizacao varchar2(5 byte);
alter table IDAFCREDENCIADO.tab_ptv add informacoes_adicionais varchar2(500 byte);


---------------------------------

---Alterações nas situações da EPTV

update lov_solicitacao_ptv_situacao set texto = 'Válido' where texto = 'Aprovado';

insert into lov_solicitacao_ptv_situacao
select (select max(id) + 1 from lov_solicitacao_ptv_situacao), 'Inválido' from dual
where not exists 
(select 1 from lov_solicitacao_ptv_situacao where texto = 'Inválido');

INSERT INTO LOV_SOLICITACAO_PTV_SITUACAO (ID, TEXTO) VALUES(8, 'Editado');

UPDATE LOV_SOLICITACAO_PTV_SITUACAO SET TEXTO = 'Rejeitado' WHERE ID = 4;



UPDATE CNF_IMPLANTACAO SET VALOR = '2018.026.0023' WHERE CAMPO like 'ultimoscript';