insert into lov_cnf_fisc_infracao_classif(id, texto)
values ((select max(id)+1 from lov_cnf_fisc_infracao_classif),
        'Defesa Vegetal')
;

insert into lov_cnf_fisc_infracao_classif(id, texto)
values ((select max(id)+1 from lov_cnf_fisc_infracao_classif),
        'Defesa Animal')
;

insert into lov_cnf_fisc_infracao_classif(id, texto)
values ((select max(id)+1 from lov_cnf_fisc_infracao_classif),
        'Inspe��o Animal')
;

insert into lov_cnf_fisc_infracao_classif(id, texto)
values ((select max(id)+1 from lov_cnf_fisc_infracao_classif),
        'Agrot�xicos')
;