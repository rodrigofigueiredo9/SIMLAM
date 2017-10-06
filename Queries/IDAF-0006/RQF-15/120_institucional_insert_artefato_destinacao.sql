insert into lov_historico_artefato (id, texto)
values ((select max(id)+1 from lov_historico_artefato),
        'fiscdestinacao')
;

commit;

insert into LOV_HISTORICO_ARTEFATOS_ACOES (id, acao, artefato)
values ((select max(id)+1 from LOV_HISTORICO_ARTEFATOS_ACOES),
        (select id from LOV_HISTORICO_ACAO where codigo like 'criar'),
        (select id from LOV_HISTORICO_ARTEFATO where texto like 'fiscdestinacao'));
        
commit;

insert into LOV_HISTORICO_ARTEFATOS_ACOES (id, acao, artefato)
values ((select max(id)+1 from LOV_HISTORICO_ARTEFATOS_ACOES),
        (select id from LOV_HISTORICO_ACAO where codigo like 'excluir'),
        (select id from LOV_HISTORICO_ARTEFATO where texto like 'fiscdestinacao'));

commit;

insert into LOV_HISTORICO_ARTEFATOS_ACOES (id, acao, artefato)
values ((select max(id)+1 from LOV_HISTORICO_ARTEFATOS_ACOES),
        (select id from LOV_HISTORICO_ACAO where codigo like 'atualizar'),
        (select id from LOV_HISTORICO_ARTEFATO where texto like 'fiscdestinacao'));

commit;