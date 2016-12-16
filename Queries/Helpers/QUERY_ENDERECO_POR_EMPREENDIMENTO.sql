select 
	te.id,
	te.empreendimento,
	te.correspondencia,
	te.zona,
	te.cep,
	te.logradouro,
	te.bairro,
	le.id estado_id,
	le.texto estado_texto,
	lm.id municipio_id,
	lm.texto municipio_texto,
	te.numero,
	te.distrito,
	te.corrego,
	te.caixa_postal,
	te.complemento,
	te.tid 
from 
	IDAF.tab_empreendimento_endereco te,
	IDAF.lov_estado le,
	IDAF.lov_municipio lm 
where 
	te.estado = le.id(+) 
and te.municipio = lm.id(+) 
and te.empreendimento = 37171 
and te.correspondencia = 0