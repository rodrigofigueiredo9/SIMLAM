select ts.NOME setor,
       ta.ATIVIDADE atividade,
       ca.NOME categoria
from TAB_ATIVIDADE ta,
     CNF_ATIVIDADE ca,
     CNF_ATIVIDADE_ATIVIDADES caa,
     TAB_SETOR ts
where caa.ATIVIDADE = ta.ID
      and caa.CONFIGURACAO = ca.ID
      and ca.NOME like '%CLAM%'
      and ta.SITUACAO = 1
      and ta.SETOR = ts.id
order by ts.nome, ca.nome, ta.COD_CATEGORIA
;