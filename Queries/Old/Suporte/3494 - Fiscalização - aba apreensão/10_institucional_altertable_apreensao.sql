-- Alterando o tipo da coluna opiniao para CLOB, na tabela transacional.
alter table tab_fisc_apreensao
rename column OPINIAO	to OPINIAO_OLD;

alter table tab_fisc_apreensao
add OPINIAO CLOB;

COMMENT ON COLUMN "IDAF"."TAB_FISC_APREENSAO"."OPINIAO" IS 'Opinar pelo destino (permanencia no local, doação, uso pela instituição, entre outros) do material e/ou bens apreendido, levando-se em considerac?o os seguintes itens: localizac?o e sua dispers?o no local, potencial impacto que a retirada do material possa causar a area, valor economico, diametro medio das especies, entre outros.';

begin

for i in ( select tfa.id,
                  tfa.opiniao_old
           from tab_fisc_apreensao tfa
          ) loop
		  
          update tab_fisc_apreensao
          set opiniao = i.opiniao_old
          where id = i.id;
end loop;

end;

alter table tab_fisc_apreensao
drop column opiniao_old;

--------------------------------------------

-- Alterando o tipo da coluna opiniao para CLOB, na tabela historico
alter table hst_fisc_apreensao
rename column OPINIAO	to OPINIAO_OLD;

alter table hst_fisc_apreensao
add OPINIAO CLOB;

COMMENT ON COLUMN "IDAF"."HST_FISC_APREENSAO"."OPINIAO" IS 'Opinar pelo destino (permanencia no local, doação, uso pela instituição, entre outros) do material e/ou bens apreendido, levando-se em considerac?o os seguintes itens: localizac?o e sua dispers?o no local, potencial impacto que a retirada do material possa causar a area, valor economico, diametro medio das especies, entre outros.';

begin

for i in ( select tfa.id,
                  tfa.opiniao_old
           from hst_fisc_apreensao tfa
          ) loop
		  
          update hst_fisc_apreensao
          set opiniao = i.opiniao_old
          where id = i.id;
end loop;

end;

alter table hst_fisc_apreensao
drop column opiniao_old;