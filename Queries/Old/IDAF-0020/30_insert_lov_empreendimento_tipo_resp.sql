begin
  insert into lov_empreendimento_tipo_resp (id, texto) 
  select (select max(id) + 1 from lov_empreendimento_tipo_resp), 'Respons�vel T�cnico' from dual
  where not exists (select null from lov_empreendimento_tipo_resp where texto = 'Respons�vel T�cnico')
  union all
  select (select max(id) + 2 from lov_empreendimento_tipo_resp), 'Raz�o Social' from dual
  where not exists (select null from lov_empreendimento_tipo_resp where texto = 'Raz�o Social');
  commit;
end;
