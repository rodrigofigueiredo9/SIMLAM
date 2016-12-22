select
	pes.id,
	CASE pes.TIPO
		WHEN 1 THEN pes.NOME
		ELSE pes.RAZAO_SOCIAl
	END as Nome,
	cont.VALOR
from
	tab_pessoa pes
	inner join tab_pessoa_meio_contato cont on pes.ID = cont.PESSOA
	inner join tab_credenciado cred on cred.PESSOA = pes.ID
where
	cont.MEIO_CONTATO = 5;