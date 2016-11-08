--==================================================================================--
-- SCRIPTS DE CRIAÇÃO / ALTERAÇÃO DE TABELAS 
--==================================================================================--

set feedback off
set define off

prompt 
prompt ---------------------------------------------------------------------
prompt INICIANDO SCRIPTS DE CRIAÇÃO / ALTERAÇÃO DE TABELAS
prompt ---------------------------------------------------------------------

-----------------------------------------------------------------------------------
-- TABELAS
-----------------------------------------------------------------------------------

prompt 
prompt -------------------------------------------------
prompt TAB_CONFIGURACAO
prompt -------------------------------------------------

insert into tab_configuracao (chave, valor) values ('SRID_BASE_REAL', 31999);
insert into tab_configuracao (chave, valor) values ('SRID_BASE', 82308);
insert into tab_configuracao (chave, valor) values ('SRID_GEOGRAFICO', 4326);


prompt 
prompt -------------------------------------------------
prompt TAB_CATEGORIA_FEICAO
prompt -------------------------------------------------

  insert into tab_categoria_feicao (id, nome, ordem) values (1, 'Hidrografia', 4); 
  insert into tab_categoria_feicao (id, nome, ordem) values (2, 'Limite/Edificação', 3); 
  insert into tab_categoria_feicao (id, nome, ordem) values (3, 'Preservação', 7); 
  insert into tab_categoria_feicao (id, nome, ordem) values (4, 'Rascunho', 10); 
  insert into tab_categoria_feicao (id, nome, ordem) values (5, 'Relevo', 5); 
  insert into tab_categoria_feicao (id, nome, ordem) values (6, 'Transportes', 8); 
  insert into tab_categoria_feicao (id, nome, ordem) values (7, 'Vegetação', 6);  
  insert into tab_categoria_feicao (id, nome, ordem) values (8, 'Atividade', 1); 
  insert into tab_categoria_feicao (id, nome, ordem) values (9, 'Outras', 9); 
  insert into tab_categoria_feicao (id, nome, ordem) values (10, 'Atividades finalizadas', 2); 
  
  
prompt 
prompt -------------------------------------------------
prompt LOV_TIPO_FEICAO
prompt -------------------------------------------------

  insert into lov_tipo_feicao (id, texto) values (1, 'Área');
  insert into lov_tipo_feicao (id, texto) values (2, 'Linha');
  insert into lov_tipo_feicao (id, texto) values (3, 'Ponto');
  
prompt 
prompt -------------------------------------------------
prompt LOV_TIPO_COLUNA
prompt -------------------------------------------------

  insert into lov_tipo_coluna (id, texto) values (1, 'Booleano');
  insert into lov_tipo_coluna (id, texto) values (2, 'Data');
  insert into lov_tipo_coluna (id, texto) values (3, 'Número');
  insert into lov_tipo_coluna (id, texto) values (4, 'Lista de Valores');
  insert into lov_tipo_coluna (id, texto) values (5, 'Texto');

prompt
prompt -------------------------------------------------
prompt LOV_LAGOA_ZONA
prompt -------------------------------------------------

insert into LOV_LAGOA_ZONA (chave, texto) values ('U', 'Urbana');
insert into LOV_LAGOA_ZONA (chave, texto) values ('R', 'Rural');
insert into LOV_LAGOA_ZONA (chave, texto) values ('A', 'Abastecimento');

prompt
prompt -------------------------------------------------
prompt LOV_APMP_TIPO
prompt -------------------------------------------------

insert into LOV_APMP_TIPO (chave, texto) values ('M', 'Matrícula');
insert into LOV_APMP_TIPO (chave, texto) values ('P', 'Posse');
insert into LOV_APMP_TIPO (chave, texto) values ('D', 'Desconhecido');

prompt
prompt -------------------------------------------------
prompt LOV_AVN_ESTAGIO
prompt -------------------------------------------------

insert into LOV_AVN_ESTAGIO (chave, texto) values ('M', 'Médio');
insert into LOV_AVN_ESTAGIO (chave, texto) values ('I', 'Inicial');
insert into LOV_AVN_ESTAGIO (chave, texto) values ('A', 'Avançado');
insert into LOV_AVN_ESTAGIO (chave, texto) values ('D', 'Desconhecido');

prompt
prompt -------------------------------------------------
prompt LOV_AA_TIPO
prompt -------------------------------------------------

insert into LOV_AA_TIPO (chave, texto) values ('C', 'Cultivada');
insert into LOV_AA_TIPO (chave, texto) values ('NC', 'Não cultivada');
insert into LOV_AA_TIPO (chave, texto) values ('D', 'Desconhecido');

prompt
prompt -------------------------------------------------
prompt LOV_ACV_TIPO
prompt -------------------------------------------------


insert into LOV_ACV_TIPO (chave, texto) values ('mangue', 'Mangue');
insert into LOV_ACV_TIPO (chave, texto) values ('brejo', 'Brejo');
insert into LOV_ACV_TIPO (chave, texto) values ('restinga', 'Restinga');
insert into LOV_ACV_TIPO (chave, texto) values ('restinga-APP', 'Restinga APP');
insert into LOV_ACV_TIPO (chave, texto) values ('floresta-nativa', 'Floresta nativa');
insert into LOV_ACV_TIPO (chave, texto) values ('macega', 'Macega');
insert into LOV_ACV_TIPO (chave, texto) values ('cabruca', 'Cabruca');
insert into LOV_ACV_TIPO (chave, texto) values ('floresta-plantada', 'Floresta plantada');
insert into LOV_ACV_TIPO (chave, texto) values ('outros', 'Outros');

prompt
prompt -------------------------------------------------
prompt LOV_ARL_COMPENSADA
prompt -------------------------------------------------

insert into LOV_ARL_COMPENSADA (chave, texto) values ('S', 'Sim');
insert into LOV_ARL_COMPENSADA (chave, texto) values ('N', 'Não');

prompt
prompt -------------------------------------------------
prompt LOV_SIM_NAO
prompt -------------------------------------------------

insert into LOV_SIM_NAO (chave, texto) values ('S', 'Sim');
insert into LOV_SIM_NAO (chave, texto) values ('N', 'Não');
  
prompt
prompt -------------------------------------------------
prompt LOV_OBRIGATORIEDADES_OPERACAO
prompt -------------------------------------------------

insert into lov_obrigatoriedades_operacao (id,texto) values (1,'Igual');
insert into lov_obrigatoriedades_operacao (id,texto) values (2,'Diferente');

prompt 
prompt -------------------------------------------------
prompt TAB_FEICAO
prompt -------------------------------------------------

insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (1, 'AA',  7,  'IDAFGEO', 'VW_DES_AA', 1, 'SEQ_DES_AA','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (2, 'Área construida',  2,  'IDAFGEO', 'DES_ACONSTRUIDA', 1, 'SEQ_DES_ACONSTRUIDA','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (3, 'ACV',  7,  'IDAFGEO', 'DES_ACV', 1, 'SEQ_DES_ACV','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (4, 'AFD',  6,  'IDAFGEO', 'DES_AFD', 1, 'SEQ_DES_AFD','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (5, 'AFS',  6,  'IDAFGEO', 'DES_AFS', 1, 'SEQ_DES_AFS','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (6, 'APMP',  2,  'IDAFGEO', 'VW_DES_APMP', 1, 'SEQ_DES_APMP','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (7, 'ARL',  3,  'IDAFGEO', 'DES_ARL', 1, 'SEQ_DES_ARL','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (8, 'ATP',  2,  'IDAFGEO', 'VW_DES_ATP', 1, 'SEQ_DES_ATP','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (9, 'AVN',  7,  'IDAFGEO', 'VW_DES_AVN', 1, 'SEQ_DES_AVN','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (10, 'Duna',  5,  'IDAFGEO', 'DES_DUNA', 1, 'SEQ_DES_DUNA','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (11, 'Duto',  6,  'IDAFGEO', 'DES_DUTO', 2, 'SEQ_DES_DUTO','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (12, 'Escarpa',  5,  'IDAFGEO', 'DES_ESCARPA', 1, 'SEQ_DES_ESCARPA','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (13, 'Estrada',  6,  'IDAFGEO', 'DES_ESTRADA', 2, 'SEQ_DES_ESTRADA','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (14, 'Ferrovia',  6,  'IDAFGEO', 'DES_FERROVIA', 2, 'SEQ_DES_FERROVIA','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (15, 'Lagoa',  1,  'IDAFGEO', 'DES_LAGOA', 1, 'SEQ_DES_LAGOA','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (16, 'Linha de transmissão',  6,  'IDAFGEO', 'DES_LTRANSMISSAO', 2, 'SEQ_DES_LTRANSMISSAO','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (17, 'Nascente',  1,  'IDAFGEO', 'DES_NASCENTE', 3, 'SEQ_DES_NASCENTE','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (18, 'Represa',  1,  'IDAFGEO', 'DES_REPRESA', 1, 'SEQ_DES_REPRESA','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (19, 'Declividade',  5,  'IDAFGEO', 'DES_REST_DECLIVIDADE', 1, 'SEQ_DES_REST_DECLIVIDADE','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (20, 'Rio (área)',  1,  'IDAFGEO', 'DES_RIO_AREA', 1, 'SEQ_DES_RIO_AREA','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (21, 'Rio (linha)',  1,  'IDAFGEO', 'DES_RIO_LINHA', 2, 'SEQ_DES_RIO_LINHA','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (22, 'Rocha',  5,  'IDAFGEO', 'DES_ROCHA', 1, 'SEQ_DES_ROCHA','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (23, 'RPPN',  3,  'IDAFGEO', 'DES_RPPN', 1, 'SEQ_DES_RPPN','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (24, 'Vértice (APMP)',  2,  'IDAFGEO', 'DES_VERTICE', 3, 'SEQ_DES_VERTICE','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (25, 'Área Abrangência',  9,  'IDAFGEO', 'DES_AREA_ABRANGENCIA', 1, 'SEQ_DES_AREA_ABRANG','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (26, 'APP',  9,  'IDAFGEO', 'TMP_AREAS_CALCULADAS', 1, 'SEQ_TMP_AREAS_CALCULADAS','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (27, 'APP em ARL',  9,  'IDAFGEO', 'TMP_AREAS_CALCULADAS', 1, 'SEQ_TMP_AREAS_CALCULADAS','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (28, 'APP em formação',  9,  'IDAFGEO', 'TMP_AREAS_CALCULADAS', 1, 'SEQ_TMP_AREAS_CALCULADAS','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (29, 'APP preservada',  9,  'IDAFGEO', 'TMP_AREAS_CALCULADAS', 1, 'SEQ_TMP_AREAS_CALCULADAS','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (30, 'Ponto do empreendimento',  9,  'IDAFGEO', 'GEO_EMP_LOCALIZACAO', 1, '','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (31, 'PATIV',  8,  'IDAFGEO', 'DES_PATIV', 3, 'SEQ_DES_PATIV','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (32, 'LATIV',  8,  'IDAFGEO', 'DES_LATIV', 2, 'SEQ_DES_LATIV','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (33, 'Área de influência da atividade',  8,  'IDAFGEO', 'DES_AIATIV', 1, 'SEQ_DES_AIATIV','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (34, 'Área da atividade',  8,  'IDAFGEO', 'DES_AATIV', 1, 'SEQ_DES_AATIV','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (35, 'AA',  7,  'IDAFGEO', 'GEO_AA', 1, '','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (36, 'Área construida',  2,  'IDAFGEO', 'GEO_ACONSTRUIDA', 1, '','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (37, 'ACV',  7,  'IDAFGEO', 'GEO_ACV', 1, '','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (38, 'AFD',  6,  'IDAFGEO', 'GEO_AFD', 1, '','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (39, 'AFS',  6,  'IDAFGEO', 'GEO_AFS', 1, '','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (40, 'APMP',  2,  'IDAFGEO', 'GEO_APMP', 1, '','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (41, 'ARL',  3,  'IDAFGEO', 'GEO_ARL', 1, '','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (42, 'ATP',  2,  'IDAFGEO', 'GEO_ATP', 1, '','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (43, 'AVN',  7,  'IDAFGEO', 'GEO_AVN', 1, '','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (44, 'Duna',  5,  'IDAFGEO', 'GEO_DUNA', 1, '','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (45, 'Duto',  6,  'IDAFGEO', 'GEO_DUTO', 2, '','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (46, 'Escarpa',  5,  'IDAFGEO', 'GEO_ESCARPA', 1, '','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (47, 'Estrada',  6,  'IDAFGEO', 'GEO_ESTRADA', 2, '','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (48, 'Ferrovia',  6,  'IDAFGEO', 'GEO_FERROVIA', 2, '','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (49, 'Lagoa',  1,  'IDAFGEO', 'GEO_LAGOA', 1, '','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (50, 'Linha de transmissão',  6,  'IDAFGEO', 'GEO_LTRANSMISSAO', 2, '','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (51, 'Nascente',  1,  'IDAFGEO', 'GEO_NASCENTE', 3, '','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (52, 'Represa',  1,  'IDAFGEO', 'GEO_REPRESA', 1, '','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (53, 'Declividade',  5,  'IDAFGEO', 'GEO_REST_DECLIVIDADE', 1, '','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (54, 'Rio (área)',  1,  'IDAFGEO', 'GEO_RIO_AREA', 1, '','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (55, 'Rio (linha)',  1,  'IDAFGEO', 'GEO_RIO_LINHA', 2, '','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (56, 'Rocha',  5,  'IDAFGEO', 'GEO_ROCHA', 1, '','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (57, 'RPPN',  3,  'IDAFGEO', 'GEO_RPPN', 1, '','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (58, 'Vértice (APMP)',  2,  'IDAFGEO', 'GEO_VERTICE', 3, '','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (59, 'APP',  9,  'IDAFGEO', 'GEO_AREAS_CALCULADAS', 1, '','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (60, 'APP em ARL',  9,  'IDAFGEO', 'GEO_AREAS_CALCULADAS', 1, '','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (61, 'APP em formação',  9,  'IDAFGEO', 'GEO_AREAS_CALCULADAS', 1, '','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (62, 'APP preservada',  9,  'IDAFGEO', 'GEO_AREAS_CALCULADAS', 1, '','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (63, 'PATIV',  10,  'IDAFGEO', 'GEO_PATIV', 3, '','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (64, 'LATIV',  10,  'IDAFGEO', 'GEO_LATIV', 2, '','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (65, 'Área de influência da atividade',  10,  'IDAFGEO', 'GEO_AIATIV', 1, '','ID');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values  (66, 'Área da atividade',  10,  'IDAFGEO', 'GEO_AATIV', 1, '','ID');

prompt 
prompt -------------------------------------------------
prompt TAB_FEICAO_COLUNAS
prompt -------------------------------------------------

 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (1,'AREA_M2', 3,38.8,'Área (m2)',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (1,'COD_APMP', 3,10,'Código APMP',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (1,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (1,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada ) values (1,'TIPO', 4,100,'Tipo',1,1,1,'LOV_AA_TIPO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (2,'AREA_M2', 3,38.8,'Área (m2)',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (2,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (2,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (3,'AREA_M2', 3,38.8,'Área (m2)',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (3,'COD_APMP', 3,10,'Código APMP',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (3,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (3,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (3,'TIPO', 4,100,'Tipo',1,1,1, 'LOV_ACV_TIPO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (4,'AREA_M2', 3,38.8,'Área (m2)',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (4,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (4,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (5,'AREA_M2', 3,38.8,'Área (m2)',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (5,'COD_APMP', 3,10,'Código APMP',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (5,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (5,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (6,'AREA_M2', 3,38.8,'Área (m2)',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (6,'COD_ATP', 3,10,'Código ATP',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (6,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (6,'NOME', 5,100,'Nome',1,1,1);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (6,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel,tabela_referenciada, coluna_referenciada ) values (6,'TIPO', 4,100,'Tipo',1,1,1,'LOV_APMP_TIPO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (7,'AREA_M2', 3,38.8,'Área (m2)',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (7,'COD_APMP', 3,10,'Código APMP',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada ) values (7,'COMPENSADA', 4,1,'Compensada',0,1,1, 'LOV_ARL_COMPENSADA', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (7,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (7,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (7,'SITUACAO', 5,50,'Situação',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (8,'AREA_M2', 3,38.8,'Área (m2)',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (8,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (8,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (9,'AREA_M2', 3,38.8,'Área (m2)',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (9,'COD_APMP', 3,10,'Código APMP',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel,tabela_referenciada, coluna_referenciada ) values (9,'ESTAGIO', 4,100,'Estágio',1,1,1, 'LOV_AVN_ESTAGIO', 'ESTAGIO');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (9,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (9,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (10,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (10,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (11,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (11,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (12,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (12,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (13,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (13,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (14,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (14,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (15,'AREA_M2', 3,38.8,'Área (m2)',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (15,'COD_APMP', 3,10,'Código APMP',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (15,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (15,'NOME', 5,100,'Nome',0,1,1);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (15,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (15,'ZONA', 4,100,'Zona',1,1,1,'LOV_LAGOA_ZONA', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (16,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (16,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (17,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (17,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (18,'AMORTECIMENTO', 3,38.8,'Amortecimento (m)',1,1,1);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (18,'AREA_M2', 3,38.8,'Área (m2)',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (18,'COD_APMP', 3,10,'Código APMP',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (18,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (18,'NOME', 5,100,'Nome',0,1,1);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (18,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (19,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (19,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (20,'AREA_M2', 3,38.8,'Área (m2)',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (20,'COD_APMP', 3,10,'Código APMP',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (20,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (20,'LARGURA', 3,38.8,'Largura (m)',1,1,1);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (20,'NOME', 5,100,'Nome',0,1,1);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (20,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (21,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (21,'LARGURA', 3,38.8,'Largura (m)',1,1,1);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (21,'NOME', 5,100,'Nome',0,1,1);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (21,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (22,'AREA_M2', 3,38.8,'Área (m2)',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (22,'COD_APMP', 3,10,'Código APMP',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (22,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (22,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (23,'AREA_M2', 3,38.8,'Área (m2)',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (23,'COD_APMP', 3,10,'Código APMP',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (23,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (23,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (24,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (24,'NOME', 5,100,'Nome',0,1,1);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (24,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (25,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (25,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (26,'AREA_M2', 3,0,'Área (m2)',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (26,'COD_APMP', 3,10,'Código APMP',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (26,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (26,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (26,'TIPO', 5,100,'Tipo',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (27,'AREA_M2', 3,38.8,'Área (m2)',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (27,'COD_APMP', 3,10,'Código APMP',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (27,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (27,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (27,'TIPO', 5,100,'Tipo',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (28,'AREA_M2', 3,38.8,'Área (m2)',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (28,'COD_APMP', 3,10,'Código APMP',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (28,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (28,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (28,'TIPO', 5,100,'Tipo',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (29,'AREA_M2', 3,38.8,'Área (m2)',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (29,'COD_APMP', 3,10,'Código APMP',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (29,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (29,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (29,'TIPO', 5,100,'Tipo',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (30,'DATA', 5,10,'Data',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (30,'EMPREENDIMENTO', 3,38,'Empreendimento',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (30,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (30,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (30,'TID', 5,36,'TID',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (31,'AA', 4,1,'AA',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (31,'AFS', 4,1,'AFS',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (31,'APP', 4,1,'APP',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (31,'ARL', 4,1,'ARL',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (31,'ATIVIDADE', 5,150,'Atividade',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (31,'AVN', 4,1,'AVN',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (31,'COD_APMP', 3,10,'Código APMP',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (31,'FLORESTA_PLANTADA', 4,1,'Floresta plantada',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (31,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (31,'MASSA_DAGUA', 4,1,'Massa d´água',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (31,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (31,'ROCHA', 4,1,'Rocha',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (31,'RPPN', 4,1,'RPPN',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (32,'AA', 4,1,'AA',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (32,'AFS', 4,1,'AFS',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (32,'APP', 4,1,'APP',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (32,'ARL', 4,1,'ARL',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (32,'ATIVIDADE', 5,150,'Atividade',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (32,'AVN', 4,1,'AVN',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (32,'COD_APMP', 3,10,'Código APMP',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (32,'COMPRIMENTO', 3,38.8,'Comprimento',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (32,'FLORESTA_PLANTADA', 4,1,'Floresta plantada',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (32,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (32,'MASSA_DAGUA', 4,1,'Massa d´água',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (32,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (32,'ROCHA', 4,1,'Rocha',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (32,'RPPN', 4,1,'RPPN',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (33,'AA', 4,1,'AA',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (33,'AFS', 4,1,'AFS',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (33,'APP', 4,1,'APP',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (33,'AREA_M2', 3,0,'Área (m2)',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (33,'ARL', 4,1,'ARL',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (33,'ATIVIDADE', 5,150,'Atividade',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (33,'AVN', 4,1,'AVN',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (33,'COD_APMP', 3,10,'Código APMP',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (33,'FLORESTA_PLANTADA', 4,1,'Floresta plantada',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (33,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (33,'MASSA_DAGUA', 4,1,'Massa d´água',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (33,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (33,'ROCHA', 4,1,'Rocha',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (33,'RPPN', 4,1,'RPPN',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (34,'AA', 4,1,'AA',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (34,'AFS', 4,1,'AFS',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (34,'APP', 4,1,'APP',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (34,'AREA_M2', 3,0,'Área (m2)',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (34,'ARL', 4,1,'ARL',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (34,'ATIVIDADE', 5,150,'Atividade',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (34,'AVN', 4,1,'AVN',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (34,'COD_APMP', 3,10,'Código APMP',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (34,'FLORESTA_PLANTADA', 4,1,'Floresta plantada',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (34,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (34,'MASSA_DAGUA', 4,1,'Massa d´água',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (34,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (34,'ROCHA', 4,1,'Rocha',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (34,'RPPN', 4,1,'RPPN',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (35,'AREA_M2', 3,38.8,'Área (m2)',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (35,'COD_APMP', 3,10,'Código APMP',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (35,'DATA', 5,10,'Data',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (35,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (35,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (35,'TID', 5,36,'TID',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (35,'TIPO', 4,100,'Tipo',0,1,0, 'LOV_AA_TIPO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (36,'AREA_M2', 3,38.8,'Área (m2)',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (36,'DATA', 5,10,'Data',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (36,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (36,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (36,'TID', 5,36,'TID',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (37,'AREA_M2', 3,38.8,'Área (m2)',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (37,'COD_APMP', 3,10,'Código APMP',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (37,'DATA', 5,10,'Data',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (37,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (37,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (37,'TID', 5,36,'TID',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (37,'TIPO', 4,100,'Tipo',0,1,0, 'LOV_ACV_TIPO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (38,'AREA_M2', 3,38.8,'Área (m2)',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (38,'DATA', 5,10,'Data',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (38,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (38,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (38,'TID', 5,36,'TID',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (39,'AREA_M2', 3,38.8,'Área (m2)',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (39,'COD_APMP', 3,10,'Código APMP',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (39,'DATA', 5,10,'Data',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (39,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (39,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (39,'TID', 5,36,'TID',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (40,'AREA_M2', 3,38.8,'Área (m2)',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (40,'COD_ATP', 3,10,'Código ATP',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (40,'DATA', 5,10,'Data',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (40,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (40,'NOME', 5,100,'Nome',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (40,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (40,'TID', 5,36,'TID',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (40,'TIPO',4,100,'Tipo',1,1,1,'LOV_APMP_TIPO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (41,'AREA_M2', 3,38.8,'Área (m2)',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (41,'COD_APMP', 3,10,'Código APMP',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (41,'CODIGO', 5,100,'Código',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (41,'COMPENSADA', 4,1,'Compensada',0,1,0, 'LOV_ARL_COMPENSADA', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (41,'DATA', 5,10,'Data',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (41,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (41,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (41,'SITUACAO', 5,50,'Situação',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (41,'TID', 5,36,'TID',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (42,'AREA_M2', 3,38.8,'Área (m2)',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (42,'DATA', 5,10,'Data',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (42,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (42,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (42,'TID', 5,36,'TID',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (43,'AREA_M2', 3,38.8,'Área (m2)',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (43,'COD_APMP', 3,10,'Código APMP',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (43,'DATA', 5,10,'Data',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada  ) values (43,'ESTAGIO', 4,100,'Estágio',0,1,0, 'LOV_AVN_ESTAGIO', 'ESTAGIO');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (43,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (43,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (43,'TID', 5,36,'TID',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (44,'DATA', 5,10,'Data',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (44,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (44,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (44,'TID', 5,36,'TID',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (45,'DATA', 5,10,'Data',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (45,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (45,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (45,'TID', 5,36,'TID',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (46,'DATA', 5,10,'Data',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (46,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (46,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (46,'TID', 5,36,'TID',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (47,'DATA', 5,10,'Data',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (47,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (47,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (47,'TID', 5,36,'TID',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (48,'DATA', 5,10,'Data',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (48,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (48,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (48,'TID', 5,36,'TID',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (49,'AREA_M2', 3,38.8,'Área (m2)',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (49,'COD_APMP', 3,10,'Código APMP',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (49,'DATA', 5,10,'Data',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (49,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (49,'NOME', 5,100,'Nome',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (49,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (49,'TID', 5,36,'TID',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (49,'ZONA',4,100,'Zona',0,1,0,'LOV_LAGOA_ZONA', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (50,'DATA', 5,10,'Data',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (50,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (50,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (50,'TID', 5,36,'TID',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (51,'DATA', 5,10,'Data',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (51,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (51,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (51,'TID', 5,36,'TID',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (52,'AMORTECIMENTO', 3,0,'Amortecimento (m)',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (52,'AREA_M2', 3,38.8,'Área (m2)',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (52,'COD_APMP', 3,10,'Código APMP',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (52,'DATA', 5,10,'Data',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (52,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (52,'NOME', 5,100,'Nome',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (52,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (52,'TID', 5,36,'TID',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (53,'DATA', 5,10,'Data',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (53,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (53,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (53,'TID', 5,36,'TID',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (54,'AREA_M2', 3,0,'Área (m2)',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (54,'COD_APMP', 3,10,'Código APMP',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (54,'DATA', 5,10,'Data',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (54,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (54,'LARGURA', 3,0,'Largura (m)',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (54,'NOME', 5,100,'Nome',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (54,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (54,'TID', 5,36,'TID',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (55,'DATA', 5,10,'Data',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (55,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (55,'LARGURA', 3,0,'Largura (m)',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (55,'NOME', 5,100,'Nome',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (55,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (55,'TID', 5,36,'TID',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (56,'AREA_M2', 3,38.8,'Área (m2)',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (56,'COD_APMP', 3,10,'Código APMP',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (56,'DATA', 5,10,'Data',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (56,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (56,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (56,'TID', 5,36,'TID',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (57,'AREA_M2', 3,38.8,'Área (m2)',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (57,'COD_APMP', 3,10,'Código APMP',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (57,'DATA', 5,10,'Data',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (57,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (57,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (57,'TID', 5,36,'TID',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (58,'DATA', 5,10,'Data',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (58,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (58,'NOME', 5,100,'Nome',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (58,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (58,'TID', 5,36,'TID',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (59,'AREA_M2', 3,0,'Área (m2)',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (59,'COD_APMP', 3,10,'Código APMP',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (59,'DATA', 5,10,'Data',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (59,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (59,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (59,'TID', 5,36,'TID',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (59,'TIPO', 5,100,'Tipo',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (60,'AREA_M2', 3,38.8,'Área (m2)',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (60,'COD_APMP', 3,10,'Código APMP',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (60,'DATA', 5,10,'Data',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (60,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (60,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (60,'TID', 5,36,'TID',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (60,'TIPO', 5,100,'Tipo',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (61,'AREA_M2', 3,38.8,'Área (m2)',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (61,'COD_APMP', 3,10,'Código APMP',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (61,'DATA', 5,10,'Data',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (61,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (61,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (61,'TID', 5,36,'TID',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (61,'TIPO', 5,100,'Tipo',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (62,'AREA_M2', 3,38.8,'Área (m2)',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (62,'COD_APMP', 3,10,'Código APMP',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (62,'DATA', 5,10,'Data',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (62,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (62,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (62,'TID', 5,36,'TID',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (62,'TIPO', 5,100,'Tipo',0,1,0);

insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (63,'AA', 4,1,'AA',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (63,'AFS', 4,1,'AFS',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (63,'APP', 4,1,'APP',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (63,'ARL', 4,1,'ARL',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (63,'ATIVIDADE', 5,150,'Atividade',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (63,'AVN', 4,1,'AVN',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (63,'COD_APMP', 3,10,'Código APMP',0,1,0);
  insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (63,'CODIGO', 5,100,'Código',0,1,0);
  insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (63,'DATA', 2,10,'Data',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (63,'FLORESTA_PLANTADA', 4,1,'Floresta plantada',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (63,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (63,'MASSA_DAGUA', 4,1,'Massa d´água',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (63,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (63,'ROCHA', 4,1,'Rocha',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (63,'RPPN', 4,1,'RPPN',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (63,'TID', 5,36,'TID',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (64,'AA', 4,1,'AA',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (64,'AFS', 4,1,'AFS',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (64,'APP', 4,1,'APP',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (64,'ARL', 4,1,'ARL',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (64,'ATIVIDADE', 5,150,'Atividade',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (64,'AVN', 4,1,'AVN',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (64,'COD_APMP', 3,10,'Código APMP',0,1,0);
  insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (64,'CODIGO', 5,100,'Código',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (64,'COMPRIMENTO', 3,38.8,'Comprimento',0,1,0);
  insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (64,'DATA', 2,10,'Data',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (64,'FLORESTA_PLANTADA', 4,1,'Floresta plantada',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (64,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (64,'MASSA_DAGUA', 4,1,'Massa d´água',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (64,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (64,'ROCHA', 4,1,'Rocha',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (64,'RPPN', 4,1,'RPPN',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (64,'TID', 5,36,'TID',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (65,'AA', 4,1,'AA',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (65,'AFS', 4,1,'AFS',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (65,'APP', 4,1,'APP',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (65,'AREA_M2', 3,0,'Área (m2)',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (65,'ARL', 4,1,'ARL',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (65,'ATIVIDADE', 5,150,'Atividade',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (65,'AVN', 4,1,'AVN',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (65,'COD_APMP', 3,10,'Código APMP',0,1,0);
  insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (65,'CODIGO', 5,100,'Código',0,1,0);
   insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (65,'DATA', 2,10,'Data',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (65,'FLORESTA_PLANTADA', 4,1,'Floresta plantada',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (65,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (65,'MASSA_DAGUA', 4,1,'Massa d´água',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (65,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (65,'ROCHA', 4,1,'Rocha',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (65,'RPPN', 4,1,'RPPN',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (65,'TID', 5,36,'TID',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (66,'AA', 4,1,'AA',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (66,'AFS', 4,1,'AFS',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (66,'APP', 4,1,'APP',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (66,'AREA_M2', 3,0,'Área (m2)',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (66,'ARL', 4,1,'ARL',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (66,'ATIVIDADE', 5,150,'Atividade',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (66,'AVN', 4,1,'AVN',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (66,'COD_APMP', 3,10,'Código APMP',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (66,'CODIGO', 5,100,'Código',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (66,'DATA', 2,10,'Data',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (66,'FLORESTA_PLANTADA', 4,1,'Floresta plantada',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (66,'ID', 3,10,'Id',0,1,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (66,'MASSA_DAGUA', 4,1,'Massa d´água',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (66,'PROJETO', 3,10,'Projeto',0,0,0);
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (66,'ROCHA', 4,1,'Rocha',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (66,'RPPN', 4,1,'RPPN',0,1,0, 'LOV_SIM_NAO', 'CHAVE');
 insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (66,'TID', 5,36,'TID',0,0,0);

prompt
prompt -------------------------------------------------
prompt TAB_FEICAO_COL_OBRIGATOR
prompt -------------------------------------------------

insert into tab_feicao_col_obrigator (feicao, coluna, operacao, valor, coluna_obrigada) values (1, 'TIPO', 1, 'USO','VEGETACAO');
insert into tab_feicao_col_obrigator (feicao, coluna, operacao, valor, coluna_obrigada) values (9, 'ESTAGIO', 2, 'D','VEGETACAO');


prompt 
prompt -------------------------------------------------
prompt TAB_SERVICO
prompt -------------------------------------------------

insert into tab_servico (id, nome, url, is_cacheado, situacao) values (1, 'dominialidade', 'http://devap3/ArcGIS/rest/services/IDAF/D_DOMINIALIDADE/MapServer', 0, null);
insert into tab_servico (id, nome, url, is_cacheado, situacao) values (2, 'atividade', 'http://devap3/ArcGIS/rest/services/IDAF/D_ATIVIDADE/MapServer', 0, null); 
insert into tab_servico (id, nome, url, is_cacheado, situacao) values (3, 'imagem', 'http://poseidon/ArcGIS/rest/services/Geobases-LandSat_Cbers/Imagem_Aerolevantamento/MapServer', 1, null); 
insert into tab_servico (id, nome, url, is_cacheado, situacao) values (4, 'vetor', 'http://poseidon/ArcGIS/rest/services/Geobases-LandSat_Cbers/Vetor_Aerolevantamento/MapServer', 1, null);
insert into tab_servico (id, nome, url, is_cacheado) values (5, 'areas_visinhas', 'http://devap3/ArcGIS/rest/services/IDAF/DEV_D_VIZINHOS/MapServer',0);

prompt 
prompt -------------------------------------------------
prompt TAB_SERVICO_FEICAO
prompt -------------------------------------------------

insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, Is_Editavel, Ordem) values (1, 30, 0, 'Ponto_Empreendimento', 1,0,29);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, Is_Editavel, filtro, Ordem) values (1, 27, 1, 'APP_ARL', 0,0,'tipo = ''APP_ARL''',  26);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, Is_Editavel, filtro, Ordem) values (1, 28, 2, 'APP_Formacao', 0,0,'tipo = ''APP_AA''',27);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, Is_Editavel, filtro, Ordem) values (1, 29, 3, 'APP_Preservada', 0,0,'tipo = ''APP_AVN''',28);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, Is_Editavel, filtro, Ordem) values (1, 26, 4, 'APP', 0,0,'tipo = ''APP_APMP''',25);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, Is_Editavel, Ordem) values (1, 17, 5, 'Nascente', 1,1,5);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, Is_Editavel, Ordem) values (1, 24, 6, 'Vertice', 1,1,3);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, Is_Editavel, Ordem) values (1, 11, 7, 'Duto', 1,1,24);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, Is_Editavel, Ordem) values (1, 13, 8, 'Estrada', 1,1,21);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, Is_Editavel, Ordem) values (1, 14, 9, 'Ferrovia', 1,1,22);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, Is_Editavel, Ordem) values (1, 16, 10, 'LTransmissao', 1,1,23);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, Is_Editavel, Ordem) values (1, 21,11, 'Rio_Linha', 1,1,6);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, Is_Editavel, Ordem) values (1, 2, 12, 'Aconstruida', 1,1,4);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, Is_Editavel, Ordem) values (1, 10, 13, 'Duna', 1,1,12);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, Is_Editavel, Ordem) values (1, 12, 14, 'Escarpa', 1,1,13);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, Is_Editavel, Ordem) values (1, 19, 15, 'Declividade', 1,1,11);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, Is_Editavel, Ordem) values (1, 15, 16, 'Lagoa', 1,1,8);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, Is_Editavel, Ordem) values (1, 18, 17, 'Represa', 1,1,9);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, Is_Editavel, Ordem) values (1, 20, 18, 'Rio_Area', 1,1,7);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, Is_Editavel, Ordem) values (1, 22, 19, 'Rocha', 1,1,10);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, Is_Editavel, Ordem) values (1, 3, 20, 'ACV', 1,1,16);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, Is_Editavel, Ordem) values (1, 5, 21, 'AFS', 1,1,20);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, Is_Editavel, Ordem) values (1, 7, 22, 'ARL', 1,1,17);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, Is_Editavel, Ordem) values (1, 23, 23, 'RPPN', 1,1,18);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, Is_Editavel, Ordem) values (1, 9, 24, 'AVN', 1,1,15);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, Is_Editavel, Ordem) values (1, 1, 25, 'AA', 1,1,14);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, Is_Editavel, Ordem) values (1, 4, 26, 'AFD', 1,1,19);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, Is_Editavel, Ordem) values (1, 6, 27, 'APMP', 1,1,2);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, Is_Editavel, Ordem) values (1, 8, 28, 'ATP', 1,1,1);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, Is_Editavel, Ordem) values (1, 25, 29, 'Area_Abrangencia', 1,0,30);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, is_editavel, Ordem) values  (2, 31, 0, 'PATIV', 1,1,1);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, is_editavel, Ordem) values  (2, 32, 1, 'LATIV', 1,1,2);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, is_editavel, Ordem) values  (2, 33, 2, 'AIATIV', 1,1,3);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, is_editavel, Ordem) values  (2, 34, 3, 'AATIV', 1,1,4);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, is_editavel, Ordem) values  (2, 63, 4, 'PATIV', 1,0,1);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, is_editavel, Ordem) values  (2, 64, 5, 'LATIV', 1,0,2);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, is_editavel, Ordem) values  (2, 65, 6, 'AIATIV', 1,0,3);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, is_editavel, Ordem) values  (2, 66, 7, 'AATIV', 1,0,4);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, is_editavel, Ordem) values  (2, 30, 8, 'Ponto_Empreendimento', 1,0,33);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, is_editavel, filtro, Ordem) values  (2, 60, 9, 'APP_ARL',1,0, 'tipo = ''APP_ARL''',30);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, is_editavel,filtro, Ordem) values  (2, 61, 10, 'APP_Formacao', 1,0,'tipo = ''APP_AA''',31);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, is_editavel,filtro, Ordem) values  (2, 62, 11, 'APP_preservada', 1,0,'tipo = ''APP_AVN''',32);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, is_editavel, filtro, Ordem) values  (2, 59, 12, 'APP', 1,0,'tipo = ''APP_APMP''',29);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, is_editavel, Ordem) values  (2, 51, 13, 'Nascente', 1,0,9);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, is_editavel, Ordem) values  (2, 58, 14, 'Vertice', 1,0,7);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, is_editavel, Ordem) values  (2, 45, 15, 'Duto', 1,0,28);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, is_editavel, Ordem) values  (2, 47, 16, 'Estrada', 1,0,25);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, is_editavel, Ordem) values  (2, 48, 17, 'Ferrovia', 1,0,26);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, is_editavel, Ordem) values  (2, 50, 18, 'LTransmissao',1,0,27);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, is_editavel, Ordem) values  (2, 55, 19, 'Rio_Linha',1,0,10);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, is_editavel, Ordem) values  (2, 36, 20, 'Aconstruida',1,0,8);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, is_editavel, Ordem) values  (2, 44, 21, 'Duna', 1,0,16);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, is_editavel, Ordem) values  (2, 46, 22, 'Escarpa', 1,0,17);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, is_editavel, Ordem) values  (2, 53, 23, 'Declividade', 1,0,15);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, is_editavel, Ordem) values  (2, 49, 24, 'Lagoa', 1,0,12);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, is_editavel, Ordem) values  (2, 52, 25, 'Represa', 1,0,13);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, is_editavel, Ordem) values  (2, 54, 26, 'Rio_Area', 1,0,11);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, is_editavel, Ordem) values  (2, 56, 27, 'Rocha', 1,0,14);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, is_editavel, Ordem) values  (2, 37, 28, 'ACV', 1,0,20);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, is_editavel, Ordem) values  (2, 39, 29, 'AFS', 1,0,24);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, is_editavel, Ordem) values  (2, 41, 30, 'ARL', 1,0,21);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, is_editavel, Ordem) values  (2, 57, 31, 'RPPN', 1,0,22);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, is_editavel, Ordem) values  (2, 43, 32, 'AVN', 1,0,19);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, is_editavel, Ordem) values  (2, 35, 33, 'AA', 1,0,18);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, is_editavel, Ordem) values  (2, 38, 34, 'AFD', 1,0,23);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, is_editavel, Ordem) values  (2, 40, 35, 'APMP', 1,0,6);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, is_editavel, Ordem) values  (2, 42, 36, 'ATP', 1,0,5);
insert into Tab_Servico_Feicao (Servico, Feicao, Id_Layer, Nome_Layer, Is_Visivel, is_editavel, Ordem) values  (2, 25, 37, 'Area_Abrangencia', 1,0,34);

prompt 
prompt -------------------------------------------------
prompt TAB_NAVEGADOR
prompt -------------------------------------------------

insert into tab_navegador (id, nome, data_cadastro) values (1, 'Dominialidade', sysdate);
insert into tab_navegador (id, nome, data_cadastro) values (2, 'Atividade', sysdate);

prompt 
prompt -------------------------------------------------
prompt TAB_NAVEGADOR_SERVICO
prompt -------------------------------------------------

insert into tab_navegador_servico (navegador, servico, is_principal, ordem_exibicao, identificar) values (1, 1, 1, 4, 1);
insert into tab_navegador_servico (navegador, servico, is_principal, ordem_exibicao, identificar) values (1, 5, 0, 3, 1);
insert into tab_navegador_servico (navegador, servico, is_principal, ordem_exibicao, identificar) values (1, 4, 0, 2, 1);
insert into tab_navegador_servico (navegador, servico, is_principal, ordem_exibicao, identificar) values (1, 3, 0, 1, 1);

insert into tab_navegador_servico (navegador, servico, is_principal, ordem_exibicao, identificar) values (2, 2, 1, 4, 1);
insert into tab_navegador_servico (navegador, servico, is_principal, ordem_exibicao, identificar) values (2, 5, 0, 3, 1);
insert into tab_navegador_servico (navegador, servico, is_principal, ordem_exibicao, identificar) values (2, 4, 0, 2, 1);
insert into tab_navegador_servico (navegador, servico, is_principal, ordem_exibicao, identificar) values (2, 3, 0, 1, 1);

prompt 
prompt -------------------------------------------------
prompt TAB_CENARIO_NAVEGADOR
prompt -------------------------------------------------

insert into tab_cenario_navegador (id, navegador, nome, ordem_exibicao, is_ativo) values (1, 1, 'Imagem', 1, 0);
insert into tab_cenario_navegador (id, navegador, nome, ordem_exibicao, is_ativo) values (2, 1, 'Híbrido', 2, 1);
insert into tab_cenario_navegador (id, navegador, nome, ordem_exibicao, is_ativo) values (3, 1, 'Vetor', 3, 0);

insert into tab_cenario_navegador (id, navegador, nome, ordem_exibicao, is_ativo) values (4, 2, 'Imagem', 1, 0);
insert into tab_cenario_navegador (id, navegador, nome, ordem_exibicao, is_ativo) values (5, 2, 'Híbrido', 2, 1);
insert into tab_cenario_navegador (id, navegador, nome, ordem_exibicao, is_ativo) values (6, 2, 'Vetor', 3, 0);

prompt 
prompt -------------------------------------------------
prompt TAB_CENARIO_SERVICO
prompt -------------------------------------------------

insert into tab_cenario_servico (cenario_navegador, servico) values (1, 3);
insert into tab_cenario_servico (cenario_navegador, servico) values (2, 3);
insert into tab_cenario_servico (cenario_navegador, servico) values (2, 4);
insert into tab_cenario_servico (cenario_navegador, servico) values (3, 4);

insert into tab_cenario_servico (cenario_navegador, servico) values (4, 3);
insert into tab_cenario_servico (cenario_navegador, servico) values (5, 3);
insert into tab_cenario_servico (cenario_navegador, servico) values (5, 4);
insert into tab_cenario_servico (cenario_navegador, servico) values (6, 4);

prompt
prompt -------------------------------------------------
prompt FEICAO
prompt -------------------------------------------------

insert into tab_categoria_feicao (id, nome, ordem) values (12, 'Outras propriedades', 12);
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values (seq_feicao.nextval, 'Emprendimento', 12, 'IDAFGEO', 'GEO_EMP_LOCALIZACAO', 3, 'SEQ_GEO_EMP_LOCALIZACAO', 'ID');
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (seq_feicao.currval, 'EMPREENDIMENTO', 3, 38, 'Empreendimento', 0, 0, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (seq_feicao.currval, 'ID', 3, 10, 'Id', 0, 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (seq_feicao.currval, 'PROJETO', 3, 10, 'Projeto', 0, 0, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (seq_feicao.currval, 'TID', 5, 36, 'TID', 0, 0, 0);
insert into tab_servico_feicao (servico, feicao, id_layer, nome_layer, is_visivel, is_editavel, ordem) values (5, seq_feicao.currval, 0, 'Empreendimento', 1,0,1);
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values (seq_feicao.nextval, 'APP', 12, 'IDAFGEO', 'GEO_AREAS_CALCULADAS', 1, null, 'ID');
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (seq_feicao.currval, 'AREA_M2', 3, 38.8, 'Área (m2)', 0, 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (seq_feicao.currval, 'COD_APMP', 3, 10, 'Código APMP', 0, 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (seq_feicao.currval, 'DATA', 5, 10, 'Data', 0, 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (seq_feicao.currval, 'ID', 3, 10, 'Id', 0, 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (seq_feicao.currval, 'PROJETO', 3, 10, 'Projeto', 0, 0, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (seq_feicao.currval, 'TID', 5, 36, 'TID', 0, 0, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (seq_feicao.currval, 'TIPO', 5, 100, 'Tipo', 0, 1, 0);
insert into tab_servico_feicao (servico, feicao, id_layer, nome_layer, is_visivel, is_editavel, ordem) values (5, seq_feicao.currval, 1, 'APP', 1,0,2);
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values (seq_feicao.nextval, 'ARL', 12, 'IDAFGEO', 'GEO_ARL', 1, null, 'ID');
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (seq_feicao.currval, 'SITUACAO', 4, 100, 'Situação', 0, 1, 0, 'LOV_ARL_SITUACAO', 'CHAVE');
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (seq_feicao.currval, 'AREA_M2', 3, 38.8, 'Área (m2)', 0, 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (seq_feicao.currval, 'COD_APMP', 3, 10, 'Código APMP', 0, 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (seq_feicao.currval, 'CODIGO', 5, 100, 'Código', 0, 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel, tabela_referenciada, coluna_referenciada) values (seq_feicao.currval, 'COMPENSADA', 4, 1, 'Compensada', 0, 1, 0, 'LOV_ARL_COMPENSADA', 'CHAVE');
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (seq_feicao.currval, 'DATA', 5, 10, 'Data', 0, 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (seq_feicao.currval, 'ID', 3, 10, 'Id', 0, 1, 0);
insert into tab_servico_feicao (servico, feicao, id_layer, nome_layer, is_visivel, is_editavel, ordem) values (5, seq_feicao.currval, 2, 'ARL', 1,0,3);
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk) values (seq_feicao.nextval, 'ATP', 12, 'IDAFGEO', 'GEO_ATP', 1, null, 'ID');
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (seq_feicao.currval, 'AREA_M2', 3, 38.8, 'Área (m2)', 0, 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (seq_feicao.currval, 'DATA', 5, 10, 'Data', 0, 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, is_visivel, is_editavel) values (seq_feicao.currval, 'ID', 3, 10, 'Id', 0, 1, 0);
insert into tab_servico_feicao (servico, feicao, id_layer, nome_layer, is_visivel, is_editavel, ordem) values (5, seq_feicao.currval, 3, 'ATP', 1,0,4);

prompt
prompt -------------------------------------------------
prompt TAB_NAVEGADOR_CAMADA
prompt -------------------------------------------------

   insert into tab_navegador_camada (navegador, servico) values (1,5);
   insert into tab_navegador_camada (navegador, servico) values (2,5);

prompt 
prompt -------------------------------------------------
prompt DES_AREA_ABRANGENCIA
prompt -------------------------------------------------

insert into des_area_abrangencia (id, projeto, geometry) values 
(1, null, mdsys.sdo_geometry( 2003, 31999 ,  null, mdsys.sdo_elem_info_array(1,1003,1), 
mdsys.sdo_ordinate_array(135453.588619015, 7622261.36779103, 579954.477620793, 7623584.28710354, 574662.800370772, 8133569.68207432, 137437.967587772, 8136215.52069934,
135453.588619015, 7622261.36779103)));

prompt -------------------------------------------------

----------------------------------------------------------------------------

prompt 
prompt ---------------------------------------------------------------------
prompt FINALIZANDO SCRIPTS DE ALTERAÇÃO DE TABELAS
prompt ---------------------------------------------------------------------
prompt 

set feedback on
set define on

--==================================================================================--
