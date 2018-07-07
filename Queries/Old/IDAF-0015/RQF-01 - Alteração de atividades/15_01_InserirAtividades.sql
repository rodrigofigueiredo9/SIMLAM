--select sys_guid() from dual;

insert into TAB_ATIVIDADE (id, setor, agrupador, atividade, situacao, tid, codigo, exibir_credenciado, empreendimento_obrigatorio)
values (seq_atividade.nextval,
        139,
        23,
        'Suinocultura sem gera��o de efluente l�quido',
        1,
        '538566A7-A929-FA2B-E050-A8C060020C8B',
        (select max(codigo)+1 from tab_atividade),
        1,
        1)
;

insert into cnf_atividade_atividades (id, configuracao, atividade, tid)
values (seq_cnf_atividade_atividades.nextval,
        15,
        (select id from tab_atividade where atividade='Suinocultura sem gera��o de efluente l�quido'),
        '538566A7-A929-FA2B-E050-A8C060020C8B')
;

--------------------------------------

insert into TAB_ATIVIDADE (id, setor, agrupador, atividade, situacao, tid, codigo, exibir_credenciado, empreendimento_obrigatorio)
values (seq_atividade.nextval,
        139,
        23,
        'Classifica��o de ovos',
        1,
        '538566A7-A929-FA2B-E050-A8C060020C8B',
        (select max(codigo)+1 from tab_atividade),
        1,
        1)
;

insert into cnf_atividade_atividades (id, configuracao, atividade, tid)
values (seq_cnf_atividade_atividades.nextval,
        15,
        (select id from tab_atividade where atividade='Classifica��o de ovos'),
        '538566A7-A929-FA2B-E050-A8C060020C8B')
;

--------------------------------------

insert into TAB_ATIVIDADE (id, setor, agrupador, atividade, situacao, tid, codigo, exibir_credenciado, empreendimento_obrigatorio)
values (seq_atividade.nextval,
        139,
        23,
        'Compostagem de res�duos org�nicos provenientes exclusivamente de atividades agropecu�rias',
        1,
        '538566A7-A929-FA2B-E050-A8C060020C8B',
        (select max(codigo)+1 from tab_atividade),
        1,
        1)
;

insert into cnf_atividade_atividades (id, configuracao, atividade, tid)
values (seq_cnf_atividade_atividades.nextval,
        15,
        (select id from tab_atividade where atividade='Compostagem de res�duos org�nicos provenientes exclusivamente de atividades agropecu�rias'),
        '538566A7-A929-FA2B-E050-A8C060020C8B')
;

--------------------------------------

insert into TAB_ATIVIDADE (id, setor, agrupador, atividade, situacao, tid, codigo, exibir_credenciado, empreendimento_obrigatorio)
values (seq_atividade.nextval,
        139,
        36,
        'Fabrica��o de f�cula, amido e seus derivados',
        1,
        '538566A7-A929-FA2B-E050-A8C060020C8B',
        (select max(codigo)+1 from tab_atividade),
        1,
        1)
;

insert into cnf_atividade_atividades (id, configuracao, atividade, tid)
values (seq_cnf_atividade_atividades.nextval,
        15,
        (select id from tab_atividade where atividade='Fabrica��o de f�cula, amido e seus derivados'),
        '538566A7-A929-FA2B-E050-A8C060020C8B')
;