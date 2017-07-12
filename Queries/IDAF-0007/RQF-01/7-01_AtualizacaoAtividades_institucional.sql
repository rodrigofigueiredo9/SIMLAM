begin

--DESATIVAR ATIVIDADES
delete
from CNF_ATIVIDADE_ATIVIDADES caa
where caa.CONFIGURACAO in (select ca.id
                           from cnf_atividade ca
                           where ca.nome = 'DRNRE/SCFL - Cadastro')
      and caa.ATIVIDADE in (select ta.id
                            from tab_atividade ta
                            where (ta.COD_CATEGORIA = '4.3.4.1' 
                                   or ta.COD_CATEGORIA = '4.3.4.2'
                                   or ta.COD_CATEGORIA = '4.3.4.3'))
;

update TAB_ATIVIDADE ta
set ta.situacao = 0
where ta.COD_CATEGORIA = '4.3.4.1' 
      or ta.COD_CATEGORIA = '4.3.4.2'
      or ta.COD_CATEGORIA = '4.3.4.3'
;

commit;

--CRIAR NOVA ATIVIDADE
insert into TAB_ATIVIDADE (id, setor, agrupador, atividade, situacao, tid, cod_categoria, codigo, exibir_credenciado, empreendimento_obrigatorio, licenca_obrigatoria)
values ((select max(id)+1 from tab_atividade),
         139,
         26,
         'Produtor de Mudas e Sementes Florestais',
         1,
         '3EF1CC35-2578-444F-BC76-7F53C5B61F26',
         '4.1.2',
         (select max(codigo)+1 from tab_atividade),
         1,
         1,
         1)
;

insert into cnf_atividade_atividades (id, configuracao, atividade, tid)
values ((select max(id)+1 from CNF_ATIVIDADE_ATIVIDADES),
        (select id from cnf_atividade where nome = 'DRNRE/SCFL - Cadastro'),
        (select id from tab_atividade where atividade='Produtor de Mudas e Sementes Florestais'),
        'ea0aa5a3-24a5-45a6-9768-bc22dd2567b2')
;

commit;

--RENOMEAR ATIVIDADES
update tab_atividade
set atividade = 'Indústria de Conserva e Beneficiamento de Palmito',
    situacao = 1,
    licenca_obrigatoria = 1
where cod_categoria = '4.3.4'
;

insert into cnf_atividade_atividades (id, configuracao, atividade, tid)
values ((select max(id)+1 from CNF_ATIVIDADE_ATIVIDADES),
        (select id from cnf_atividade where nome = 'DRNRE/SCFL - Cadastro'),
        (select id from tab_atividade where atividade='Indústria de Conserva e Beneficiamento de Palmito'),
        'ea0aa5a3-24a5-45a6-9768-bc22dd2567b2')
;

update tab_atividade
set atividade = 'Serraria quando não associada a fabricação de artefatos de madeiras - Classe I'
where atividade = 'Serraria - Classe I'
;

select * from tab_atividade
where atividade = 'Serraria quando não associada a fabricação de artefatos de madeiras - Classe I'
      or atividade = 'Indústria de Conserva e Beneficiamento de Palmito';

update tab_atividade
set atividade = 'Serraria quando não associada a fabricação de artefatos de madeiras - Classe II'
where atividade = 'Serraria - Classe II'
;

update tab_atividade
set atividade = 'Serraria quando não associada a fabricação de artefatos de madeiras - Classe III'
where atividade = 'Serraria - Classe III'
;

commit;

end;