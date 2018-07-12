--Altera as habilitações com situação = "advertência" para situação = "ativo" com motivo = "advertência"
update TAB_HAB_EMI_CFO_CFOC
set situacao = 1,
    motivo = 5
where situacao = 2;