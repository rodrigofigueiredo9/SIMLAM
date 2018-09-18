ALTER TABLE tab_tramitacao_arquivo ADD arquivar_automatico numeric(1,0);

insert into tab_tramitacao_arquivo 
select seq_tramitacao_arquivo.nextval, 'Arquivo SEP',	
(select id from tab_setor where nome = 'Processo SEP' and rownum = 1), 1,	1, '95808c27-e9db-43c4-9a5e-79a869069578', 1 from dual;

insert into tab_tramitacao_arq_estante 
select seq_tramitacao_arq_esta.nextval, seq_tramitacao_arquivo.currval, 'Estante SEP', '95808c27-e9db-43c4-9a5e-79a869069578' from dual;

insert into tab_tramitacao_arq_prateleira 
select seq_tramitacao_arq_prat.nextval, seq_tramitacao_arquivo.currval, seq_tramitacao_arq_esta.currval, 1,	'Prateleira SEP', '95808c27-e9db-43c4-9a5e-79a869069578' from dual;

select * from tab_setor where nome = 'Processo SEP';
update tab_tramitacao_arquivo set setor = 259 where id = (select max(id) from tab_tramitacao_arquivo);