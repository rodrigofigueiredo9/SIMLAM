--==================================================================================--
-- SCRIPTS DE CRIAÇÃO / ALTERAÇÃO DE TABELAS 
--==================================================================================--

set feedback off
set define off
/* 
 * <IDAFCREDENCIADOGEO>	  = Substituir para o esquema do IDAFCREDENCIADOGEO.
 * <http://devap3/arcgis/rest/services/IDAF/DEV_D_DOMINIALIDADE_CRED/MapServer>  = Substituir para o endereço onde será publicado o serviço de dominialidade do credenciado
 * <http://mapas.geobases.es.gov.br/ArcGIS/rest/services/IMAGEM/Aerolevantamento/MapServer> = Substituir para o endereço do serviço de imagem do Geobases
 * <http://mapas.geobases.es.gov.br/ArcGIS/rest/services/Vetor_Aerolevantamento/MapServer> = Substituir para o endereço serviço de vetor do Geobases
 * <http://devap3/ArcGIS/rest/services/IDAF/DEV_D_VIZINHOS/MapServer> = Substituir para o endereço onde está o serviço de vizinhos do interno
 *
 */

prompt 
prompt ---------------------------------------------------------------------
prompt INICIANDO SCRIPTS DE CRIAÇÃO / ALTERAÇÃO DE TABELAS
prompt ---------------------------------------------------------------------

-----------------------------------------------------------------------------------
-- TABELAS
-----------------------------------------------------------------------------------

prompt 
prompt -------------------------------------------------
prompt TAB_CATEGORIA_FEICAO
prompt -------------------------------------------------

insert into tab_categoria_feicao (id, nome, ordem) values (1, 'Hidrografia', 4);
insert into tab_categoria_feicao (id, nome, ordem) values (2, 'Limite/Edificação', 3);
insert into tab_categoria_feicao (id, nome, ordem) values (3, 'Preservação', 7);
insert into tab_categoria_feicao (id, nome, ordem) values (5, 'Relevo', 5);
insert into tab_categoria_feicao (id, nome, ordem) values (6, 'Transportes', 8);
insert into tab_categoria_feicao (id, nome, ordem) values (7, 'Vegetação', 6);
insert into tab_categoria_feicao (id, nome, ordem) values (9, 'Áreas calculadas', 12);
insert into tab_categoria_feicao (id, nome, ordem) values (12, 'Outras propriedades', 11);

prompt 
prompt -------------------------------------------------
prompt TAB_FEICAO
prompt -------------------------------------------------

insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk, descricao) values (1, 'AA', 7, '<IDAFCREDENCIADOGEO>', 'VW_DES_AA',1, 'SEQ_DES_AA', 'ID', 'Área Alterada');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk, descricao) values (2, 'Área construida', 2, '<IDAFCREDENCIADOGEO>', 'DES_ACONSTRUIDA',1, 'SEQ_DES_ACONSTRUIDA', 'ID', 'Área construída');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk, descricao) values (4, 'AFD', 6, '<IDAFCREDENCIADOGEO>', 'DES_AFD',1, 'SEQ_DES_AFD', 'ID', 'Área de Faixa de Domínio');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk, descricao) values (5, 'AFS', 6, '<IDAFCREDENCIADOGEO>', 'DES_AFS',1, 'SEQ_DES_AFS', 'ID', 'Área de Faixa de Servidão');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk, descricao) values (6, 'APMP', 2, '<IDAFCREDENCIADOGEO>', 'VW_DES_APMP',1, 'SEQ_TMP_APMP', 'ID', 'Área da Propriedade por Matrícula ou Posse');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk, descricao) values (7, 'ARL', 3, '<IDAFCREDENCIADOGEO>', 'DES_ARL',1, 'SEQ_DES_ARL', 'ID', 'Área de Reserva Legal');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk, descricao) values (8, 'ATP', 2, '<IDAFCREDENCIADOGEO>', 'VW_DES_ATP',1, 'SEQ_DES_ATP', 'ID', 'Área Total da Propriedade');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk, descricao) values (9, 'AVN', 7, '<IDAFCREDENCIADOGEO>', 'VW_DES_AVN',1, 'SEQ_DES_AVN', 'ID', 'Área de Vegetação Nativa');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk, descricao) values (10, 'Duna', 5, '<IDAFCREDENCIADOGEO>', 'DES_DUNA',1, 'SEQ_DES_DUNA', 'ID', 'Duna');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk, descricao) values (11, 'Duto', 6, '<IDAFCREDENCIADOGEO>', 'DES_DUTO',2, 'SEQ_DES_DUTO', 'ID', 'Duto');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk, descricao) values (12, 'Escarpa', 5, '<IDAFCREDENCIADOGEO>', 'DES_ESCARPA',1, 'SEQ_DES_ESCARPA', 'ID', 'Escarpa');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk, descricao) values (13, 'Estrada', 6, '<IDAFCREDENCIADOGEO>', 'DES_ESTRADA',2, 'SEQ_DES_ESTRADA', 'ID', 'Estrada');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk, descricao) values (14, 'Ferrovia', 6, '<IDAFCREDENCIADOGEO>', 'DES_FERROVIA',2, 'SEQ_DES_FERROVIA', 'ID', 'Ferrovia');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk, descricao) values (15, 'Lagoa', 1, '<IDAFCREDENCIADOGEO>', 'DES_LAGOA',1, 'SEQ_DES_LAGOA', 'ID', 'Lago, lagoa');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk, descricao) values (16, 'Linha de transmissão', 6, '<IDAFCREDENCIADOGEO>', 'DES_LTRANSMISSAO',2, 'SEQ_DES_LTRANSMISSAO', 'ID', 'Linha de transmissão');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk, descricao) values (17, 'Nascente', 1, '<IDAFCREDENCIADOGEO>', 'DES_NASCENTE',3, 'SEQ_DES_NASCENTE', 'ID', 'Nascente');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk, descricao) values (18, 'Represa', 1, '<IDAFCREDENCIADOGEO>', 'DES_REPRESA',1, 'SEQ_DES_REPRESA', 'ID', 'Represa');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk, descricao) values (19, 'Declividade', 5, '<IDAFCREDENCIADOGEO>', 'DES_REST_DECLIVIDADE',1, 'SEQ_DES_REST_DECLIVIDADE', 'ID', 'Declividade');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk, descricao) values (20, 'Rio (área)', 1, '<IDAFCREDENCIADOGEO>', 'DES_RIO_AREA',1, 'SEQ_DES_RIO_AREA', 'ID', 'Rio (área)');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk, descricao) values (21, 'Rio (linha)', 1, '<IDAFCREDENCIADOGEO>', 'DES_RIO_LINHA',2, 'SEQ_DES_RIO_LINHA', 'ID', 'Rio (linha)');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk, descricao) values (22, 'Rocha', 5, '<IDAFCREDENCIADOGEO>', 'DES_ROCHA',1, 'SEQ_DES_ROCHA', 'ID', 'Rocha');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk, descricao) values (23, 'RPPN', 3, '<IDAFCREDENCIADOGEO>', 'DES_RPPN',1, 'SEQ_DES_RPPN', 'ID', 'Área de Reserva Particular do Patrimônio Natural');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk, descricao) values (24, 'Vértice (APMP)', 2, '<IDAFCREDENCIADOGEO>', 'DES_VERTICE',3, 'SEQ_DES_VERTICE', 'ID', 'Vértice da APMP');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk, descricao) values (25, 'Área Abrangência', 9, '<IDAFCREDENCIADOGEO>', 'DES_AREA_ABRANGENCIA',1, 'SEQ_DES_AREA_ABRANG', 'ID', 'Área de abrangência do projeto geográfico');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk, descricao) values (26, 'APP', 9, '<IDAFCREDENCIADOGEO>', 'TMP_AREAS_CALCULADAS',1, 'SEQ_TMP_AREAS_CALCULADAS', 'ID', 'Área de Preservação Permanente');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk, descricao) values (27, 'APP em ARL', 9, '<IDAFCREDENCIADOGEO>', 'TMP_AREAS_CALCULADAS',1, 'SEQ_TMP_AREAS_CALCULADAS', 'ID', 'Área de Preservação Permanente em Reserva Legal');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk, descricao) values (28, 'APP em recuperação', 9, '<IDAFCREDENCIADOGEO>', 'TMP_AREAS_CALCULADAS',1, 'SEQ_TMP_AREAS_CALCULADAS', 'ID', 'Área de Preservação Permanente em Recuperação');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk, descricao) values (29, 'APP preservada', 9, '<IDAFCREDENCIADOGEO>', 'TMP_AREAS_CALCULADAS',1, 'SEQ_TMP_AREAS_CALCULADAS', 'ID', 'Área de Preservação Permanente Preservada');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk, descricao) values (30, 'Ponto do empreendimento', 9, '<IDAFCREDENCIADOGEO>', 'GEO_EMP_LOCALIZACAO',1, '', 'ID', 'Ponto do empreendimento');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk, descricao) values (67, 'APP em uso', 9, '<IDAFCREDENCIADOGEO>', 'TMP_AREAS_CALCULADAS',1, 'SEQ_TMP_AREAS_CALCULADAS', 'ID', 'Área de Preservação Permanente em Uso');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk, descricao) values (337, 'Emprendimento', 12, '<IDAFCREDENCIADOGEO>', 'GEO_EMP_LOCALIZACAO',3, 'SEQ_GEO_EMP_LOCALIZACAO', 'ID', 'Empreendimento');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk, descricao) values (338, 'APP', 12, '<IDAFCREDENCIADOGEO>', 'GEO_AREAS_CALCULADAS',1, '', 'ID', 'Área de Preservação Permanente');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk, descricao) values (339, 'ARL', 12, '<IDAFCREDENCIADOGEO>', 'GEO_ARL',1, '', 'ID', 'Área de Reserva Legal');
insert into tab_feicao (id, nome, categoria, esquema, tabela, tipo, sequencia, coluna_pk, descricao) values (340, 'ATP', 12, '<IDAFCREDENCIADOGEO>', 'GEO_ATP',1, '', 'ID', 'Área Total da Propriedade');

prompt 
prompt -------------------------------------------------
prompt TAB_FEICAO_COLUNAS
prompt -------------------------------------------------

insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(1, 'AREA_M2', 3, '38,8', 'Área (m2)', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(1, 'COD_APMP', 3, '10', 'Código APMP', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(1, 'ID', 3, '10', 'Id', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(1, 'PROJETO', 3, '10', 'Projeto', 0, '', '', 0, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(1, 'TIPO', 4, '100', 'Tipo', 1, 'LOV_AA_TIPO', 'CHAVE', 1, 1);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(1, 'VEGETACAO', 4, '100', 'Vegetação', 0, 'LOV_AA_VEGETACAO', 'CHAVE', 1, 1);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(2, 'AREA_M2', 3, '38,8', 'Área (m2)', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(2, 'ID', 3, '10', 'Id', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(2, 'PROJETO', 3, '10', 'Projeto', 0, '', '', 0, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(4, 'AREA_M2', 3, '38,8', 'Área (m2)', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(4, 'ID', 3, '10', 'Id', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(4, 'PROJETO', 3, '10', 'Projeto', 0, '', '', 0, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(5, 'AREA_M2', 3, '38,8', 'Área (m2)', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(5, 'COD_APMP', 3, '10', 'Código APMP', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(5, 'ID', 3, '10', 'Id', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(5, 'PROJETO', 3, '10', 'Projeto', 0, '', '', 0, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(6, 'AREA_M2', 3, '38,8', 'Área (m2)', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(6, 'COD_ATP', 3, '10', 'Código ATP', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(6, 'ID', 3, '10', 'Id', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(6, 'NOME', 5, '100', 'Nome', 1, '', '', 1, 1);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(6, 'PROJETO', 3, '10', 'Projeto', 0, '', '', 0, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(6, 'TIPO', 4, '100', 'Tipo', 1, 'LOV_APMP_TIPO', 'CHAVE', 1, 1);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(7, 'AREA_M2', 3, '38,8', 'Área (m2)', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(7, 'COD_APMP', 3, '10', 'Código APMP', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(7, 'CODIGO', 5, '6', 'Código', 1, '', '', 1, 1);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(7, 'COMPENSADA', 4, '1', 'Compensada', 0, 'LOV_ARL_COMPENSADA', 'CHAVE', 1, 1);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(7, 'ID', 3, '10', 'Id', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(7, 'PROJETO', 3, '10', 'Projeto', 0, '', '', 0, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(7, 'SITUACAO', 4, '1', 'Situação', 0, 'LOV_ARL_SITUACAO', 'CHAVE', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(8, 'AREA_M2', 3, '38,8', 'Área (m2)', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(8, 'ID', 3, '10', 'Id', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(8, 'PROJETO', 3, '10', 'Projeto', 0, '', '', 0, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(9, 'AREA_M2', 3, '38,8', 'Área (m2)', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(9, 'COD_APMP', 3, '10', 'Código APMP', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(9, 'ESTAGIO', 4, '100', 'Estágio', 1, 'LOV_AVN_ESTAGIO', 'ESTAGIO', 1, 1);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(9, 'ID', 3, '10', 'Id', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(9, 'PROJETO', 3, '10', 'Projeto', 0, '', '', 0, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(9, 'VEGETACAO', 4, '100', 'Vegetação', 0, 'LOV_AVN_VEGETACAO', 'CHAVE', 1, 1);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(10, 'ID', 3, '10', 'Id', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(10, 'PROJETO', 3, '10', 'Projeto', 0, '', '', 0, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(11, 'ID', 3, '10', 'Id', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(11, 'PROJETO', 3, '10', 'Projeto', 0, '', '', 0, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(12, 'ID', 3, '10', 'Id', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(12, 'PROJETO', 3, '10', 'Projeto', 0, '', '', 0, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(13, 'ID', 3, '10', 'Id', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(13, 'PROJETO', 3, '10', 'Projeto', 0, '', '', 0, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(14, 'ID', 3, '10', 'Id', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(14, 'PROJETO', 3, '10', 'Projeto', 0, '', '', 0, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(15, 'AREA_M2', 3, '38,8', 'Área (m2)', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(15, 'COD_APMP', 3, '10', 'Código APMP', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(15, 'ID', 3, '10', 'Id', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(15, 'NOME', 5, '100', 'Nome', 0, '', '', 1, 1);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(15, 'PROJETO', 3, '10', 'Projeto', 0, '', '', 0, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(15, 'ZONA', 4, '100', 'Zona', 1, 'LOV_LAGOA_ZONA', 'CHAVE', 1, 1);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(16, 'ID', 3, '10', 'Id', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(16, 'PROJETO', 3, '10', 'Projeto', 0, '', '', 0, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(17, 'ID', 3, '10', 'Id', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(17, 'PROJETO', 3, '10', 'Projeto', 0, '', '', 0, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(18, 'AMORTECIMENTO', 3, '38,8', 'Largura APP (m)', 1, '', '', 1, 1);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(18, 'AREA_M2', 3, '38,8', 'Área (m2)', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(18, 'COD_APMP', 3, '10', 'Código APMP', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(18, 'ID', 3, '10', 'Id', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(18, 'NOME', 5, '100', 'Nome', 0, '', '', 1, 1);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(18, 'PROJETO', 3, '10', 'Projeto', 0, '', '', 0, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(19, 'ID', 3, '10', 'Id', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(19, 'PROJETO', 3, '10', 'Projeto', 0, '', '', 0, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(19, 'TIPO', 4, '50', 'Tipo', 1, 'LOV_REST_DECLIV_TIPO', 'CHAVE', 1, 1);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(20, 'AREA_M2', 3, '38,8', 'Área (m2)', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(20, 'COD_APMP', 3, '10', 'Código APMP', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(20, 'ID', 3, '10', 'Id', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(20, 'LARGURA', 3, '38,8', 'Largura (m)', 1, '', '', 1, 1);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(20, 'NOME', 5, '100', 'Nome', 0, '', '', 1, 1);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(20, 'PROJETO', 3, '10', 'Projeto', 0, '', '', 0, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(21, 'ID', 3, '10', 'Id', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(21, 'LARGURA', 3, '38,8', 'Largura (m)', 1, '', '', 1, 1);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(21, 'NOME', 5, '100', 'Nome', 0, '', '', 1, 1);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(21, 'PROJETO', 3, '10', 'Projeto', 0, '', '', 0, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(22, 'AREA_M2', 3, '38,8', 'Área (m2)', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(22, 'COD_APMP', 3, '10', 'Código APMP', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(22, 'ID', 3, '10', 'Id', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(22, 'PROJETO', 3, '10', 'Projeto', 0, '', '', 0, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(23, 'AREA_M2', 3, '38,8', 'Área (m2)', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(23, 'COD_APMP', 3, '10', 'Código APMP', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(23, 'ID', 3, '10', 'Id', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(23, 'PROJETO', 3, '10', 'Projeto', 0, '', '', 0, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(24, 'ID', 3, '10', 'Id', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(24, 'NOME', 5, '100', 'Nome', 0, '', '', 1, 1);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(24, 'PROJETO', 3, '10', 'Projeto', 0, '', '', 0, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(25, 'ID', 3, '10', 'Id', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(25, 'PROJETO', 3, '10', 'Projeto', 0, '', '', 0, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(26, 'AREA_M2', 3, '38,8', 'Área (m2)', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(26, 'COD_APMP', 3, '10', 'Código APMP', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(26, 'ID', 3, '10', 'Id', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(26, 'PROJETO', 3, '10', 'Projeto', 0, '', '', 0, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(26, 'TIPO', 5, '100', 'Tipo', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(27, 'AREA_M2', 3, '38,8', 'Área (m2)', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(27, 'COD_APMP', 3, '10', 'Código APMP', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(27, 'ID', 3, '10', 'Id', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(27, 'PROJETO', 3, '10', 'Projeto', 0, '', '', 0, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(27, 'TIPO', 5, '100', 'Tipo', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(28, 'AREA_M2', 3, '38,8', 'Área (m2)', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(28, 'COD_APMP', 3, '10', 'Código APMP', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(28, 'ID', 3, '10', 'Id', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(28, 'PROJETO', 3, '10', 'Projeto', 0, '', '', 0, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(28, 'TIPO', 5, '100', 'Tipo', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(29, 'AREA_M2', 3, '38,8', 'Área (m2)', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(29, 'COD_APMP', 3, '10', 'Código APMP', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(29, 'ID', 3, '10', 'Id', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(29, 'PROJETO', 3, '10', 'Projeto', 0, '', '', 0, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(29, 'TIPO', 5, '100', 'Tipo', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(30, 'EMPREENDIMENTO', 3, '38', 'Empreendimento', 0, '', '', 0, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(30, 'ID', 3, '10', 'Id', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(30, 'PROJETO', 3, '10', 'Projeto', 0, '', '', 0, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(30, 'TID', 5, '36', 'TID', 0, '', '', 0, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(67, 'AREA_M2', 3, '38,8', 'Área (m2)', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(67, 'COD_APMP', 3, '10', 'Código APMP', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(67, 'ID', 3, '10', 'Id', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(67, 'PROJETO', 3, '10', 'Projeto', 0, '', '', 0, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(67, 'TIPO', 5, '100', 'Tipo', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(337, 'EMPREENDIMENTO', 3, '38', 'Empreendimento', 0, '', '', 0, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(337, 'ID', 3, '10', 'Id', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(337, 'PROJETO', 3, '10', 'Projeto', 0, '', '', 0, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(337, 'TID', 5, '36', 'TID', 0, '', '', 0, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(338, 'AREA_M2', 3, '38,8', 'Área (m2)', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(338, 'COD_APMP', 3, '10', 'Código APMP', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(338, 'DATA', 5, '10', 'Data', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(338, 'ID', 3, '10', 'Id', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(338, 'PROJETO', 3, '10', 'Projeto', 0, '', '', 0, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(338, 'TID', 5, '36', 'TID', 0, '', '', 0, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(338, 'TIPO', 5, '100', 'Tipo', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(339, 'AREA_M2', 3, '38,8', 'Área (m2)', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(339, 'COD_APMP', 3, '10', 'Código APMP', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(339, 'CODIGO', 5, '100', 'Código', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(339, 'COMPENSADA', 4, '1', 'Compensada', 0, 'LOV_ARL_COMPENSADA', 'CHAVE', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(339, 'DATA', 5, '10', 'Data', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(339, 'ID', 3, '10', 'Id', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(339, 'SITUACAO', 4, '100', 'Situação', 0, 'LOV_ARL_SITUACAO', 'CHAVE', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(340, 'AREA_M2', 3, '38,8', 'Área (m2)', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(340, 'DATA', 5, '10', 'Data', 0, '', '', 1, 0);
insert into tab_feicao_colunas (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) values(340, 'ID', 3, '10', 'Id', 0, '', '', 1, 0);

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

insert into tab_servico (id, nome, url, is_cacheado) values (1, 'Dominialidades', '<http://devap3/arcgis/rest/services/IDAF/DEV_D_DOMINIALIDADE_CRED/MapServer>', 0);
insert into tab_servico (id, nome, url, is_cacheado) values (3, 'Imagem', '<http://mapas.geobases.es.gov.br/ArcGIS/rest/services/IMAGEM/Aerolevantamento/MapServer>', 1);
insert into tab_servico (id, nome, url, is_cacheado) values (4, 'Base Geobases', '<http://mapas.geobases.es.gov.br/ArcGIS/rest/services/Vetor_Aerolevantamento/MapServer>', 1);
insert into tab_servico (id, nome, url, is_cacheado) values (5, 'Outras propriedades', '<http://devap3/ArcGIS/rest/services/IDAF/DEV_D_VIZINHOS/MapServer>', 0);

prompt 
prompt -------------------------------------------------
prompt TAB_SERVICO_FEICAO
prompt -------------------------------------------------

insert into tab_servico_feicao (servico, feicao, id_layer, nome_layer, is_visivel, is_editavel, filtro, ordem, is_finalizada) values (1, 30, 0, 'Ponto_Empreendimento', 1, 0, '', 30, 0); 
insert into tab_servico_feicao (servico, feicao, id_layer, nome_layer, is_visivel, is_editavel, filtro, ordem, is_finalizada) values (1, 27, 1, 'APP_ARL', 0, 0, 'TIPO = ''APP_ARL''', 26, 0); 
insert into tab_servico_feicao (servico, feicao, id_layer, nome_layer, is_visivel, is_editavel, filtro, ordem, is_finalizada) values (1, 29, 2, 'APP_preservada', 0, 0, 'TIPO = ''APP_AVN''', 27, 0); 
insert into tab_servico_feicao (servico, feicao, id_layer, nome_layer, is_visivel, is_editavel, filtro, ordem, is_finalizada) values (1, 28, 3, 'APP_em_recuperacao', 0, 0, 'TIPO = ''APP_AA_REC''', 28, 0); 
insert into tab_servico_feicao (servico, feicao, id_layer, nome_layer, is_visivel, is_editavel, filtro, ordem, is_finalizada) values (1, 67, 4, 'APP_em_uso', 0, 0, 'TIPO = ''APP_AA_USO''', 29, 0); 
insert into tab_servico_feicao (servico, feicao, id_layer, nome_layer, is_visivel, is_editavel, filtro, ordem, is_finalizada) values (1, 26, 5, 'APP', 0, 0, 'TIPO = ''APP_APMP''', 25, 0); 
insert into tab_servico_feicao (servico, feicao, id_layer, nome_layer, is_visivel, is_editavel, filtro, ordem, is_finalizada) values (1, 17, 6, 'Nascente', 1, 1, '', 5, 0); 
insert into tab_servico_feicao (servico, feicao, id_layer, nome_layer, is_visivel, is_editavel, filtro, ordem, is_finalizada) values (1, 24, 7, 'Vertice', 1, 1, '', 3, 0); 
insert into tab_servico_feicao (servico, feicao, id_layer, nome_layer, is_visivel, is_editavel, filtro, ordem, is_finalizada) values (1, 11, 8, 'Duto', 1, 1, '', 24, 0); 
insert into tab_servico_feicao (servico, feicao, id_layer, nome_layer, is_visivel, is_editavel, filtro, ordem, is_finalizada) values (1, 13, 9, 'Estrada', 1, 1, '', 21, 0); 
insert into tab_servico_feicao (servico, feicao, id_layer, nome_layer, is_visivel, is_editavel, filtro, ordem, is_finalizada) values (1, 14, 10, 'Ferrovia', 1, 1, '', 22, 0); 
insert into tab_servico_feicao (servico, feicao, id_layer, nome_layer, is_visivel, is_editavel, filtro, ordem, is_finalizada) values (1, 16, 11, 'LTransmissao', 1, 1, '', 23, 0); 
insert into tab_servico_feicao (servico, feicao, id_layer, nome_layer, is_visivel, is_editavel, filtro, ordem, is_finalizada) values (1, 21, 12, 'Rio_Linha', 1, 1, '', 6, 0); 
insert into tab_servico_feicao (servico, feicao, id_layer, nome_layer, is_visivel, is_editavel, filtro, ordem, is_finalizada) values (1, 2, 13, 'Aconstruida', 1, 1, '', 4, 0); 
insert into tab_servico_feicao (servico, feicao, id_layer, nome_layer, is_visivel, is_editavel, filtro, ordem, is_finalizada) values (1, 10, 14, 'Duna', 1, 1, '', 12, 0); 
insert into tab_servico_feicao (servico, feicao, id_layer, nome_layer, is_visivel, is_editavel, filtro, ordem, is_finalizada) values (1, 12, 15, 'Escarpa', 1, 1, '', 13, 0); 
insert into tab_servico_feicao (servico, feicao, id_layer, nome_layer, is_visivel, is_editavel, filtro, ordem, is_finalizada) values (1, 19, 16, 'Declividade', 1, 1, '', 11, 0); 
insert into tab_servico_feicao (servico, feicao, id_layer, nome_layer, is_visivel, is_editavel, filtro, ordem, is_finalizada) values (1, 15, 17, 'Lagoa', 1, 1, '', 8, 0); 
insert into tab_servico_feicao (servico, feicao, id_layer, nome_layer, is_visivel, is_editavel, filtro, ordem, is_finalizada) values (1, 18, 18, 'Represa', 1, 1, '', 9, 0); 
insert into tab_servico_feicao (servico, feicao, id_layer, nome_layer, is_visivel, is_editavel, filtro, ordem, is_finalizada) values (1, 20, 19, 'Rio_Area', 1, 1, '', 7, 0); 
insert into tab_servico_feicao (servico, feicao, id_layer, nome_layer, is_visivel, is_editavel, filtro, ordem, is_finalizada) values (1, 22, 20, 'Rocha', 1, 1, '', 10, 0); 
insert into tab_servico_feicao (servico, feicao, id_layer, nome_layer, is_visivel, is_editavel, filtro, ordem, is_finalizada) values (1, 5, 21, 'AFS', 1, 1, '', 20, 0); 
insert into tab_servico_feicao (servico, feicao, id_layer, nome_layer, is_visivel, is_editavel, filtro, ordem, is_finalizada) values (1, 7, 22, 'ARL', 1, 1, '', 17, 0); 
insert into tab_servico_feicao (servico, feicao, id_layer, nome_layer, is_visivel, is_editavel, filtro, ordem, is_finalizada) values (1, 23, 23, 'RPPN', 1, 1, '', 18, 0); 
insert into tab_servico_feicao (servico, feicao, id_layer, nome_layer, is_visivel, is_editavel, filtro, ordem, is_finalizada) values (1, 9, 24, 'AVN', 1, 1, '', 15, 0); 
insert into tab_servico_feicao (servico, feicao, id_layer, nome_layer, is_visivel, is_editavel, filtro, ordem, is_finalizada) values (1, 1, 25, 'AA', 1, 1, '', 14, 0); 
insert into tab_servico_feicao (servico, feicao, id_layer, nome_layer, is_visivel, is_editavel, filtro, ordem, is_finalizada) values (1, 4, 26, 'AFD', 1, 1, '', 19, 0); 
insert into tab_servico_feicao (servico, feicao, id_layer, nome_layer, is_visivel, is_editavel, filtro, ordem, is_finalizada) values (1, 6, 27, 'APMP', 1, 1, '', 2, 0); 
insert into tab_servico_feicao (servico, feicao, id_layer, nome_layer, is_visivel, is_editavel, filtro, ordem, is_finalizada) values (1, 8, 28, 'ATP', 1, 1, '', 1, 0); 
insert into tab_servico_feicao (servico, feicao, id_layer, nome_layer, is_visivel, is_editavel, filtro, ordem, is_finalizada) values (1, 25, 29, 'Area_Abrangencia', 1, 0, '', 30, 0); 
insert into tab_servico_feicao (servico, feicao, id_layer, nome_layer, is_visivel, is_editavel, filtro, ordem, is_finalizada) values (5, 337, 0, 'Empreendimento', 1, 0, '', 1, 0); 
insert into tab_servico_feicao (servico, feicao, id_layer, nome_layer, is_visivel, is_editavel, filtro, ordem, is_finalizada) values (5, 338, 1, 'APP', 1, 0, '', 2, 0); 
insert into tab_servico_feicao (servico, feicao, id_layer, nome_layer, is_visivel, is_editavel, filtro, ordem, is_finalizada) values (5, 339, 2, 'ARL', 1, 0, '', 3, 0); 
insert into tab_servico_feicao (servico, feicao, id_layer, nome_layer, is_visivel, is_editavel, filtro, ordem, is_finalizada) values (5, 340, 3, 'ATP', 1, 0, '', 4, 0); 

prompt 
prompt -------------------------------------------------
prompt TAB_NAVEGADOR
prompt -------------------------------------------------

insert into tab_navegador (id, nome, data_cadastro, fila_tipo) values (1, 'Dominialidade', sysdate, 3);

prompt 
prompt -------------------------------------------------
prompt TAB_NAVEGADOR_SERVICO
prompt -------------------------------------------------

insert into tab_navegador_servico (navegador, servico, is_principal, ordem_exibicao, identificar, gera_legenda) values (1, 1, 1, 4, 1, 1);
insert into tab_navegador_servico (navegador, servico, is_principal, ordem_exibicao, identificar, gera_legenda) values (1, 4, 0, 2, 0, 0);
insert into tab_navegador_servico (navegador, servico, is_principal, ordem_exibicao, identificar, gera_legenda) values (1, 3, 0, 1, 0, 0);
insert into tab_navegador_servico (navegador, servico, is_principal, ordem_exibicao, identificar, gera_legenda) values (1, 5, 0, 3, 1, 1);


prompt 
prompt -------------------------------------------------
prompt TAB_CENARIO_NAVEGADOR
prompt -------------------------------------------------

insert into tab_cenario_navegador (id, navegador, nome, ordem_exibicao, is_ativo) values ( 1, 1, 'Imagem', 1, 1);
insert into tab_cenario_navegador (id, navegador, nome, ordem_exibicao, is_ativo) values ( 2, 1, 'Híbrido', 2, 0);
insert into tab_cenario_navegador (id, navegador, nome, ordem_exibicao, is_ativo) values ( 3, 1, 'Vetor', 3, 0);

prompt 
prompt -------------------------------------------------
prompt TAB_CENARIO_SERVICO
prompt -------------------------------------------------

insert into tab_cenario_servico (cenario_navegador, servico) values ( 1, 3);
insert into tab_cenario_servico (cenario_navegador, servico) values ( 2, 3);
insert into tab_cenario_servico (cenario_navegador, servico) values ( 2, 4);
insert into tab_cenario_servico (cenario_navegador, servico) values ( 3, 4);

prompt
prompt -------------------------------------------------
prompt TAB_NAVEGADOR_CAMADA
prompt -------------------------------------------------

insert into tab_navegador_camada (navegador, servico) values (1, 5);

prompt 
prompt -------------------------------------------------
prompt DES_AREA_ABRANGENCIA
prompt -------------------------------------------------

insert into des_area_abrangencia (id, projeto, geometry) values 
(1, null, mdsys.sdo_geometry( 2003, 31984 ,  null, mdsys.sdo_elem_info_array(1,1003,1), 
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
