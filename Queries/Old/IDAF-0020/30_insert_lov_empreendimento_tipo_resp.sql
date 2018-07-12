begin
  insert into lov_empreendimento_tipo_resp (id, texto) 
  select (select max(id) + 1 from lov_empreendimento_tipo_resp), 'Responsável Técnico' from dual
  where not exists (select null from lov_empreendimento_tipo_resp where texto = 'Responsável Técnico')
  union all
  select (select max(id) + 2 from lov_empreendimento_tipo_resp), 'Razão Social' from dual
  where not exists (select null from lov_empreendimento_tipo_resp where texto = 'Razão Social');
  commit;
end;
