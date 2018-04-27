insert into tab_tramitacao_motivo (id, texto, ativo, tid) 
select (select max(id) + 1 from tab_tramitacao_motivo), 'Para conhecimento e Providências', 1, '6fe3950b-f2d2-410b-8212-a7e7cafcb077' from dual
where not exists (select 1 from tab_tramitacao_motivo where texto like 'Para conhecimento e Providências');

insert into tab_tramitacao_motivo (id, texto, ativo, tid) 
select (select max(id) + 1 from tab_tramitacao_motivo), 'Juntada Processo SEP', 1, '6fe3950b-f2d2-410b-8212-a7e7cafcb077' from dual
where not exists (select 1 from tab_tramitacao_motivo where texto like 'Juntada Processo SEP');