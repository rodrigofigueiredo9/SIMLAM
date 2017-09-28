insert into LOV_AUTENTICACAO_PERMISSAO (id, nome, codigo, funcionario_tipo, descricao, grupo, tipo)
values ( ( select max(id)+1 from LOV_AUTENTICACAO_PERMISSAO),
         'Produtos',
         'ConfigurarProdutosDestinacao',
         3,
         'Produtos Apreendidos/Destinação',
         'Configurar Fiscalizacao',
         1)
;

commit;

insert into TAB_AUTENTICACAO_PAPEL_PERM (id, papel, permissao, tid)
values ( ( select max(id)+1 from TAB_AUTENTICACAO_PAPEL_PERM ),
         ( select id from TAB_AUTENTICACAO_PAPEL tap where tap.nome = 'Configurar Fiscalização' ),
         ( select id from LOV_AUTENTICACAO_PERMISSAO lap where lap.codigo = 'ConfigurarProdutosDestinacao'),
         ( select max(tid) from TAB_AUTENTICACAO_PAPEL_PERM ) )
;

commit;