insert into tab_setor (id, nome, sigla, tid) 
select (select max(r.id) + 1 from tab_setor r), 'Outros', 'OUTROS', '12F49ABE-08E0-4C40-B9BD-158210239449' from dual
where not exists(select 1 from tab_setor s where s.sigla like 'OUTROS');

