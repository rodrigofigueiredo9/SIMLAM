--Melhorando o comentário da coluna Tipo na tabela de DUA
COMMENT ON COLUMN "IDAF"."CNF_VALOR_DUA"."TIPO" IS 'Tipo do DUA: 1=PTV, 2=CFO/CFOC';

--PTV (credenciado)
insert into cnf_valor_dua
values (
(select max(id)+1 from cnf_valor_dua),
(select sys_guid() from dual),
sysdate,
16.36,
1
);

--CFO/CFOC (Institucional)
insert into cnf_valor_dua
values (
(select max(id)+1 from cnf_valor_dua),
(select sys_guid() from dual),
sysdate,
24.54,
2
);

UPDATE IDAF.CNF_IMPLANTACAO SET VALOR = '2018.002.4641' WHERE CAMPO like 'ultimoscript';