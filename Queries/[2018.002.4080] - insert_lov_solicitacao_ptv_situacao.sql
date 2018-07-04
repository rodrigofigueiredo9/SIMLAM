update lov_solicitacao_ptv_situacao set texto = 'Válido' where texto = 'Aprovado';

insert into lov_solicitacao_ptv_situacao
select (select max(id) + 1 from lov_solicitacao_ptv_situacao), 'Inválido' from dual
where not exists 
(select 1 from lov_solicitacao_ptv_situacao where texto = 'Inválido');