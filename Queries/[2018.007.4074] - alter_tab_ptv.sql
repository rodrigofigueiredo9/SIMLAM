alter table IDAFCREDENCIADO.tab_ptv add local_fiscalizacao varchar2(500 byte);
alter table IDAFCREDENCIADO.tab_ptv add hora_fiscalizacao varchar2(5 byte);
alter table IDAFCREDENCIADO.tab_ptv add informacoes_adicionais varchar2(500 byte);
grant all on "IDAF"."TAB_NF_CAIXA" to "IDAFCREDENCIADO" ;