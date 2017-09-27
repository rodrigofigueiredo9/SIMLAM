insert into lov_fiscalizacao_serie(id, texto, tipo)
values ((select max(id)+1 from lov_fiscalizacao_serie),
        'E',
        0)
;

commit;