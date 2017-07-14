
ALTER TABLE idaf.TAB_HAB_EMI_CFO_CFOC ADD NUMERO_PROCESSO varchar2(30);

commit;

ALTER TABLE idaf.hst_hab_emi_cfo_cfoc add NUMERO_PROCESSO varchar2(30);

commit;


ALTER TABLE idaf.tab_ptv
       ADD (
       	DUA_NUMERO VARCHAR2(80 BYTE), 
        DUA_TIPO_PESSOA NUMBER(38,0), 
        DUA_CPF_CNPJ VARCHAR2(20 BYTE)
      );

commit;

ALTER TABLE idaf.hst_ptv
       ADD (
       	DUA_NUMERO VARCHAR2(80 BYTE), 
        DUA_TIPO_PESSOA NUMBER(38,0), 
        DUA_CPF_CNPJ VARCHAR2(20 BYTE)
      );

commit;

ALTER TABLE idaf.tab_ptv
       ADD (
       	RESPONSAVEL_SEM_DOC VARCHAR2(3000 BYTE), 
        EMPREENDIMENTO_SEM_DOC VARCHAR2(3000 BYTE)
      );
      
commit;

ALTER TABLE idaf.hst_ptv
       ADD (
       	RESPONSAVEL_SEM_DOC VARCHAR2(3000 BYTE), 
        EMPREENDIMENTO_SEM_DOC VARCHAR2(3000 BYTE)
      );

commit;

ALTER TABLE idaf.tab_ptv MODIFY
   (empreendimento NULL);
   
commit;

ALTER TABLE idafcredenciado.tab_cfoc_produto
            ADD QUANTIDADE number null;


commit;

ALTER TABLE idafcredenciado.hst_cfoc_produto
            ADD QUANTIDADE number null;
            
commit;

ALTER TABLE idafcredenciado.tab_cfoc_produto
            ADD CULTURA number(38,0) null;    
		

commit;
        
ALTER TABLE idafcredenciado.tab_cfoc_produto
            ADD CULTIVAR number(38,0) null;  


commit;

ALTER TABLE idaf.tab_ptv_outrouf_produto
ADD ( 
      EXIBE_KILOS CHAR(1),
      CONSTRAINT CONS_KILOS4 CHECK (EXIBE_KILOS IN ('1','0'))
    );
    
commit;



ALTER TABLE idafcredenciado.tab_lote_item
ADD EXIBE_KILOS CHAR(1)
CONSTRAINT CONS_KILOS_LOTE CHECK (EXIBE_KILOS IN ('1','0'));

commit;

ALTER TABLE idafcredenciado.hst_lote_item
ADD EXIBE_KILOS CHAR(1)
CONSTRAINT CONS_KILOS_LOTE_HST CHECK (EXIBE_KILOS IN ('1','0'));

commit;

ALTER TABLE idafcredenciado.tab_cfoc_produto
ADD EXIBE_KILOS CHAR(1)
CONSTRAINT CONS_KILOS_CFOC_PRODUTO CHECK (EXIBE_KILOS IN ('1','0'));

commit;


ALTER TABLE idafcredenciado.hst_cfoc_produto
ADD EXIBE_KILOS CHAR(1)
CONSTRAINT CONS_KILOS_CFOC_HIST CHECK (EXIBE_KILOS IN ('1','0'));

commit;

ALTER TABLE idafcredenciado.tab_cfo_produto
ADD EXIBE_KILOS CHAR(1)
CONSTRAINT CONS_KILOS_CFO_PRODUTO CHECK (EXIBE_KILOS IN ('1','0'));

commit;

ALTER TABLE idaf.tab_ptv_produto
ADD EXIBE_KILOS CHAR(1)
CONSTRAINT CONS_KILOS_PTV CHECK (EXIBE_KILOS IN ('1','0'));

commit;

ALTER TABLE idaf.hst_ptv_produto
ADD EXIBE_KILOS CHAR(1)
CONSTRAINT CONS_KILOS_PTV_HIST CHECK (EXIBE_KILOS IN ('1','0'));

commit;


ALTER TABLE idafcredenciado.hst_cfo_produto
ADD EXIBE_KILOS CHAR(1)
CONSTRAINT CONS_KILOS_CFO_PRODUTO_HIST CHECK (EXIBE_KILOS IN ('1','0'));

commit;

ALTER TABLE idafcredenciado.tab_ptv_produto
ADD EXIBE_KILOS CHAR(1)
CONSTRAINT CONS_KILOS_PTV CHECK (EXIBE_KILOS IN ('1','0'));

commit;

ALTER TABLE idafcredenciado.hst_lote_item
ADD EXIBE_KILOS CHAR(1)
CONSTRAINT CONS_KILOS_LOTE_HST CHECK (EXIBE_KILOS IN ('1','0'));

commit;

ALTER TABLE idafcredenciado.hst_ptv_produto
ADD EXIBE_KILOS CHAR(1)
CONSTRAINT CONS_KILOS_PTV_HIST CHECK (EXIBE_KILOS IN ('1','0'));


commit;

ALTER TABLE idaf.lov_cultivar_declara_adicional
ADD OUTRO_ESTADO CHAR(1)
CONSTRAINT CONS_OUTRO_ESTADO CHECK (OUTRO_ESTADO IN ('1','0')) 

commit;

CREATE SEQUENCE idaf.seq_cnf_local_vistoria_bloq NOCACHE;

commit;

CREATE TABLE idaf.cnf_local_vistoria_bloqueio (
  "ID" NUMBER(38) NOT NULL,
  dia_inicio DATE,
  dia_fim DATE,
  tid VARCHAR2(36 BYTE),
  setor NUMBER(38) NOT NULL,
  CONSTRAINT cnf_local_vistoria_bloquei_pk PRIMARY KEY ("ID"),
  CONSTRAINT fk_local_vistoria_bloque FOREIGN KEY (setor) REFERENCES idaf.tab_setor ("ID")
);

commit;

-- Table properties

COMMENT ON COLUMN idaf.cnf_local_vistoria_bloqueio."ID" IS 'Chave primária de indentificação da tabela.';
COMMENT ON COLUMN idaf.cnf_local_vistoria_bloqueio.dia_inicio IS 'Dia Início.
';
COMMENT ON COLUMN idaf.cnf_local_vistoria_bloqueio.dia_fim IS 'Dia Fim.';
COMMENT ON COLUMN idaf.cnf_local_vistoria_bloqueio.tid IS 'Id transacional. Esse valor garante a ligação entre todas as tabelas relacionadas.';
COMMENT ON COLUMN idaf.cnf_local_vistoria_bloqueio.setor IS 'Chave estrangeira para tabela tab_setor. coluna(ID).';

commit;

 CREATE TABLE idaf.tab_ptv_outro_arquivo
   (	"ID" NUMBER(38,0), 
	PTV NUMBER(38,0), 
	ARQUIVO NUMBER(38,0), 
	ORDEM NUMBER(3,0), 
	DESCRICAO VARCHAR2(100 BYTE), 
	TID VARCHAR2(36 BYTE),
    CONSTRAINT pk_TAB_PTV_OUTRO_ARQUIVO PRIMARY KEY ("ID") USING INDEX (CREATE UNIQUE INDEX idaf.TAB_PTV_OUTRO_ARQUIVO_pk ON idaf.TAB_PTV_OUTRO_ARQUIVO("ID"))
   ); 
 

   COMMENT ON COLUMN "IDAF"."TAB_PTV_OUTRO_ARQUIVO"."ID" IS 'ID';
 
   COMMENT ON COLUMN "IDAF"."TAB_PTV_OUTRO_ARQUIVO"."PTV" IS 'Chave estrangeira para tab_ptv_outrouf';
 
   COMMENT ON COLUMN "IDAF"."TAB_PTV_OUTRO_ARQUIVO"."ARQUIVO" IS 'Chave estrangeira para tab_arquivo';
 
   COMMENT ON COLUMN "IDAF"."TAB_PTV_OUTRO_ARQUIVO"."ORDEM" IS 'Orderm';
 
   COMMENT ON COLUMN "IDAF"."TAB_PTV_OUTRO_ARQUIVO"."DESCRICAO" IS 'Descrição do arquivo';
 
   COMMENT ON COLUMN "IDAF"."TAB_PTV_OUTRO_ARQUIVO"."TID" IS 'Id transacional';

commit;