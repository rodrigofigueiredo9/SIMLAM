alter table HST_PTV_COMUNI_CONVERSA
modify texto null;

alter table IDAFCREDENCIADO.tab_ptv add exibir_mensagem numeric(1,0);
alter table IDAFCREDENCIADO.tab_ptv add exibir_msg_credenciado numeric(1,0);
alter table IDAFCREDENCIADO.tab_ptv add local_fiscalizacao varchar2(500 byte);
alter table IDAFCREDENCIADO.tab_ptv add hora_fiscalizacao varchar2(5 byte);
alter table IDAFCREDENCIADO.tab_ptv add informacoes_adicionais varchar2(500 byte);
grant all on "IDAF"."TAB_NF_CAIXA" to "IDAFCREDENCIADO" ;

update lov_solicitacao_ptv_situacao set texto = 'Válido' where texto = 'Aprovado';

insert into lov_solicitacao_ptv_situacao
select (select max(id) + 1 from lov_solicitacao_ptv_situacao), 'Inválido' from dual
where not exists 
(select 1 from lov_solicitacao_ptv_situacao where texto = 'Inválido');

INSERT INTO LOV_SOLICITACAO_PTV_SITUACAO (ID, TEXTO) VALUES(8, 'Editado');

UPDATE LOV_SOLICITACAO_PTV_SITUACAO SET TEXTO = 'Rejeitado' WHERE ID = 4;



UPDATE CNF_IMPLANTACAO SET VALOR = '2018.007.001' WHERE CAMPO like 'ultimoscript';