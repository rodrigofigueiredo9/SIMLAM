==================================================================================--
-- SCRIPTS DE INSER플O / ALTERA플O DE DADOS 
==================================================================================--

set feedback off
set define off


==================================================================================--

set feedback off
set define off

prompt 
prompt ---------------------------------------------------------------------
prompt INICIANDO SCRIPTS DE CRIA플O / ALTERA플O DE TABELAS
prompt ---------------------------------------------------------------------

----------------------------------------------------------------------------

-------
-- TABELAS
----------------------------------------------------------------------------

-------

prompt
prompt -------------------------------------------------
prompt TAB_FEICAO
prompt -------------------------------------------------

update tab_feicao f set f.nome = 'Linha da atividade' where f.id in (32,64);
update tab_feicao f set f.nome = 'Ponto da atividade' where f.id in (31,63);

prompt
prompt -------------------------------------------------
prompt TAB_SERVICO_FEICAO
prompt -------------------------------------------------

update tab_servico_feicao sf set sf.is_finalizada = 1 where sf.servico = 2 and sf.feicao in (63,64,65,66);

prompt
prompt -------------------------------------------------
prompt TAB_NAVEGADOR_SERVICO
prompt -------------------------------------------------

update tab_navegador_servico ns set ns.gera_legenda =1, identificar =1 where ns.servico in(1, 2,5,6);
update tab_navegador_servico ns set ns.gera_legenda =0, identificar =0 where ns.servico not in(1, 2,5,6);

prompt
prompt -------------------------------------------------
prompt TAB_SERVICO
prompt -------------------------------------------------

update tab_servico set nome = 'Dominialidades' where id = 1;
update tab_servico set nome = 'Atividades cadastradas' where id = 2;
update tab_servico set nome = 'Imagem' where id = 3;
update tab_servico set nome = 'Base Geobases' where id = 4;
update tab_servico set nome = 'Outras propriedades' where id = 5;
update tab_servico set nome = 'Dados fiscaliza豫o' where id = 6;


prompt -------------------------------------------------

----------------------------------------------------------------------------

prompt 
prompt ---------------------------------------------------------------------
prompt FINALIZANDO SCRIPTS DE ALTERA플O DE TABELAS
prompt ---------------------------------------------------------------------
prompt 

set feedback on
set define on

--

============================================================================

======--