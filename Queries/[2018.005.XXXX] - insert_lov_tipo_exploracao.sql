insert into TAB_FEICAO_COLUNAS (feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, 
coluna_referenciada, is_visivel, is_editavel)
values( 63, 'TPEXP', 4, 1, 'Tipo de Exploração', 1, 'LOV_TIPO_EXPLORACAO', 'CHAVE', 1, 1);

insert into LOV_TIPO_EXPLORACAO (chave, texto)
values (1, 'AUS -  Uso Alternativo do Solo');
insert into LOV_TIPO_EXPLORACAO (chave, texto)
values (2, 'CAI - Corte de Árvore Isolada');
insert into LOV_TIPO_EXPLORACAO (chave, texto)
values (3, 'EFP - Exploração de Floresta Plantada');