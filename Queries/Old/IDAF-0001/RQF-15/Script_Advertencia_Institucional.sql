--Altera as habilita��es com situa��o = "advert�ncia" para situa��o = "ativo" com motivo = "advert�ncia"
update TAB_HAB_EMI_CFO_CFOC
set situacao = 1,
    motivo = 5
where situacao = 2;