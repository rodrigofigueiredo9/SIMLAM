begin
  insert into LOV_HAB_EMISSAO_CFO_MOTIVO (id, texto)
       values ((select max(id) + 1 from LOV_HAB_EMISSAO_CFO_MOTIVO), 'Advertência');
  commit;
end;