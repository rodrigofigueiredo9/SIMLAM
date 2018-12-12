insert into tab_titulo_modelo (id, codigo, tipo, situacao, subtipo, data_criacao, 
nome, sigla, tipo_protocolo, arquivo, tabela, tid, documento)
select seq_titulo_modelo.nextval, 60, tipo, situacao, subtipo, sysdate, 
'Laudo de Vistoria de Queima Controlada', 'LVQC', tipo_protocolo, arquivo, tabela, tid, documento 
from tab_titulo_modelo where codigo = 4
and not exists (select 1 from tab_titulo_modelo where codigo = 60);

insert into tab_titulo_modelo_regras (id, modelo, regra, tid)
select seq_titulo_modelo_regras.nextval, (select id from tab_titulo_modelo where codigo = 60), regra, tid 
from tab_titulo_modelo_regras r
where modelo = 12 and not exists
(select 1 from tab_titulo_modelo_regras t 
where t.regra = r.regra and t.modelo = r.modelo);

insert into tab_titulo_modelo_respostas (id, modelo, regra, resposta, valor, tid)
select seq_titulo_modelo_respostas.nextval, (select id from tab_titulo_modelo where codigo = 60), regra, resposta, valor, tid 
from tab_titulo_modelo_respostas r
where modelo = 12 and not exists
(select 1 from tab_titulo_modelo_respostas t 
where t.regra = r.regra and t.modelo = r.modelo and t.resposta = r.resposta);