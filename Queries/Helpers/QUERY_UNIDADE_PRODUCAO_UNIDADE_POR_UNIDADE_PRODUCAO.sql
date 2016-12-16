select 
	up.id,
	up.tid,
	up.unidade_producao,
	up.possui_cod_up,
	up.codigo_up,
	up.tipo_producao,
	up.renasem,
	up.renasem_data_validade,
	up.area,
	up.cultura,
	up.cultivar,
	tc.texto cultura_texto,
	cc.cultivar cultivar_nome,
	up.data_plantio_ano_producao,
	up.estimativa_quant_ano,
	up.estimativa_unid_medida 
from 
	IDAF.crt_unidade_producao_unidade up,
	IDAF.tab_cultura_cultivar cc,
	IDAF.tab_cultura tc 
where 
	cc.id(+) = up.cultivar 
and tc.id = up.cultura 
and up.unidade_producao = 121  -- codigo da unidade de producao