select
	up.id,
	up.tid,
	up.empreendimento,
	up.possui_cod_propriedade,
	up.propriedade_codigo,
	emp.codigo empreendimento_codigo,
	up.local_livro
from
	IDAF.crt_unidade_producao up,
	IDAF.tab_empreendimento emp
where
	up.empreendimento = emp.id
and up.empreendimento = 37171 -- codigo do empreendimento