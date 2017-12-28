alter table tab_fiscalizacao
add "PDF_IUF" NUMBER(38, 0)
;

alter table tab_fiscalizacao
add CONSTRAINT "FK_TAB_FISC_PDF_IUF" FOREIGN KEY ("PDF_IUF")
	  REFERENCES "IDAF"."TAB_ARQUIVO" ("ID") ENABLE
;

COMMENT ON COLUMN "IDAF"."TAB_FISCALIZACAO"."PDF_IUF" IS 'Chave estrangeira para tab_arquivo. Campo(ID).'
;

alter table tab_fiscalizacao
add POSSUI_PROJETO_GEO NUMBER(1, 0) NULL
;

COMMENT ON COLUMN "IDAF"."TAB_FISCALIZACAO"."POSSUI_PROJETO_GEO" IS 'Fiscalização possui projeto geográfico? 1 - sim; 0 - não';