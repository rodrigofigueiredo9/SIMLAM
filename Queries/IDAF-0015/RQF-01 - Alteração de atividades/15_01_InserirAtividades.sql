--select sys_guid() from dual;

insert into TAB_ATIVIDADE (id, setor, agrupador, atividade, situacao, tid, codigo, exibir_credenciado, empreendimento_obrigatorio)
values (seq_atividade.nextval,
        139,
        23,
        'Suinocultura sem geração de efluente líquido',
        1,
        '538566A7-A929-FA2B-E050-A8C060020C8B',
        (select max(codigo)+1 from tab_atividade),
        1,
        1)
;

insert into cnf_atividade_atividades (id, configuracao, atividade, tid)
values (seq_cnf_atividade_atividades.nextval,
        15,
        (select id from tab_atividade where atividade='Suinocultura sem geração de efluente líquido'),
        '538566A7-A929-FA2B-E050-A8C060020C8B')
;

--------------------------------------

insert into TAB_ATIVIDADE (id, setor, agrupador, atividade, situacao, tid, codigo, exibir_credenciado, empreendimento_obrigatorio)
values (seq_atividade.nextval,
        139,
        23,
        'Classificação de ovos',
        1,
        '538566A7-A929-FA2B-E050-A8C060020C8B',
        (select max(codigo)+1 from tab_atividade),
        1,
        1)
;

insert into cnf_atividade_atividades (id, configuracao, atividade, tid)
values (seq_cnf_atividade_atividades.nextval,
        15,
        (select id from tab_atividade where atividade='Classificação de ovos'),
        '538566A7-A929-FA2B-E050-A8C060020C8B')
;

--------------------------------------

insert into TAB_ATIVIDADE (id, setor, agrupador, atividade, situacao, tid, codigo, exibir_credenciado, empreendimento_obrigatorio)
values (seq_atividade.nextval,
        139,
        23,
        'Compostagem de resíduos orgânicos provenientes exclusivamente de atividades agropecuárias',
        1,
        '538566A7-A929-FA2B-E050-A8C060020C8B',
        (select max(codigo)+1 from tab_atividade),
        1,
        1)
;

insert into cnf_atividade_atividades (id, configuracao, atividade, tid)
values (seq_cnf_atividade_atividades.nextval,
        15,
        (select id from tab_atividade where atividade='Compostagem de resíduos orgânicos provenientes exclusivamente de atividades agropecuárias'),
        '538566A7-A929-FA2B-E050-A8C060020C8B')
;

--------------------------------------

insert into TAB_ATIVIDADE (id, setor, agrupador, atividade, situacao, tid, codigo, exibir_credenciado, empreendimento_obrigatorio)
values (seq_atividade.nextval,
        139,
        36,
        'Fabricação de fécula, amido e seus derivados',
        1,
        '538566A7-A929-FA2B-E050-A8C060020C8B',
        (select max(codigo)+1 from tab_atividade),
        1,
        1)
;

insert into cnf_atividade_atividades (id, configuracao, atividade, tid)
values (seq_cnf_atividade_atividades.nextval,
        15,
        (select id from tab_atividade where atividade='Fabricação de fécula, amido e seus derivados'),
        '538566A7-A929-FA2B-E050-A8C060020C8B')
;