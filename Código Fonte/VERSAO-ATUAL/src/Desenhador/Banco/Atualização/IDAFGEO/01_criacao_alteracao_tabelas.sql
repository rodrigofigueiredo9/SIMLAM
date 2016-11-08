
--==================================================================================--
-- SCRIPTS DE CRIAÇÃO / ALTERAÇÃO DE TABELAS - IDAF
--==================================================================================--

set feedback off
set define off

prompt 
prompt ---------------------------------------------------------------------
prompt INICIANDO SCRIPTS DE CRIAÇÃO / ALTERAÇÃO DE TABELAS
prompt ---------------------------------------------------------------------

----------------------------------------------------------------------------
-- TABELAS
----------------------------------------------------------------------------

prompt
prompt -------------------------------------------------
prompt TAB_SERVICO_FEICAO
prompt -------------------------------------------------

alter table tab_servico_feicao add is_finalizada number(1);

prompt
prompt -------------------------------------------------
prompt TAB_NAVEGADOR_SERVICO
prompt -------------------------------------------------


alter table tab_navegador_servico add gera_legenda number(1) default 0;
comment on column TAB_NAVEGADOR_SERVICO.GERA_LEGENDA  is 'Define se o serviço do navegador gera legenda ou não. 1-Sim/ 0-Não';


prompt -------------------------------------------------

----------------------------------------------------------------------------

set feedback on
set define on
