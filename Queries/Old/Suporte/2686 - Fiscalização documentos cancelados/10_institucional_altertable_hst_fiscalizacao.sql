alter table hst_fiscalizacao
add (PDF_IUF	NUMBER(38,0),
     POSSUI_PROJETO_GEO	NUMBER(1,0));
     
COMMENT ON COLUMN "IDAF"."HST_FISCALIZACAO"."PDF_IUF" IS 'Chave estrangeira para tab_arquivo. Campo(ID).';
COMMENT ON COLUMN "IDAF"."HST_FISCALIZACAO"."POSSUI_PROJETO_GEO" IS 'Fiscalização possui projeto geográfico? 1 - sim; 0 - não';
     
begin

for i in ( select tf.id,
                  tf.pdf_iuf,
                  tf.possui_projeto_geo
           from tab_fiscalizacao tf
           where tf.pdf_iuf is not null
                 or tf.possui_projeto_geo is not null
          ) loop
          update hst_fiscalizacao hf
          set hf.pdf_iuf = i.pdf_iuf,
              hf.possui_projeto_geo = i.possui_projeto_geo
          where hf.fiscalizacao_id = i.id;
end loop;

end;