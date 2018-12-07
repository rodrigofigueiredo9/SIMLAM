
begin
	delete from idafgeo.tab_feicao_colunas where feicao in (31, 34) and coluna = 'RPPN';
	delete from idafgeo.tab_feicao_colunas where feicao in (31, 34) and coluna = 'ROCHA';	
	delete from idafgeo.tab_feicao_colunas where feicao in (31, 34) and coluna = 'MASSA_DAGUA';	
	
	insert into idafgeo.tab_feicao_colunas 
	(feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) 
		values(31, 'RPPN', 4, 1, 'RPPN', 0, 'LOV_SIM_NAO', 'CHAVE', 1, 0);
		
	insert into idafgeo.tab_feicao_colunas
	(feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel)
		values(31, 'ROCHA', 4, 1, 'Rocha', 0, 'LOV_SIM_NAO',	'CHAVE', 1, 0);
	
	insert into idafgeo.tab_feicao_colunas 
	(feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel)
		values(31, 'MASSA_DAGUA', 4, 1, 'Massa d´água', 0, 'LOV_SIM_NAO', 'CHAVE', 1, 0);
	
	insert into idafgeo.tab_feicao_colunas 
	(feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel) 
		values(34, 'RPPN', 4, 1, 'RPPN', 0, 'LOV_SIM_NAO', 'CHAVE', 1, 0);
		
	insert into idafgeo.tab_feicao_colunas
	(feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel)
		values(34, 'ROCHA', 4, 1, 'Rocha', 0, 'LOV_SIM_NAO',	'CHAVE', 1, 0);
	
	insert into idafgeo.tab_feicao_colunas 
	(feicao, coluna, tipo, tamanho, alias, is_obrigatorio, tabela_referenciada, coluna_referenciada, is_visivel, is_editavel)
		values(34, 'MASSA_DAGUA', 4, 1, 'Massa d´água', 0, 'LOV_SIM_NAO', 'CHAVE', 1, 0);
end;

update idafgeo.TAB_SERVICO_FEICAO set IS_EDITAVEL = 0 where FEICAO in (32, 33);

update idafgeo.LOV_TIPO_EXPLORACAO set CHAVE = 'UAS', TEXTO = replace(TEXTO, 'AUS -', 'UAS -') where CHAVE = 'AUS';

alter table idafgeo.tab_fila add TITULO NUMBER(38,0);
COMMENT ON COLUMN "IDAFGEO"."TAB_FILA"."TITULO" IS 'Referencia o ID do titulo na tabela tab_titulo no esquema oficial.';