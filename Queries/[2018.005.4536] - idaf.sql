--INSERT INTO CRT_EXP_FLORESTAL_EXPLORACAO
alter table IDAF.crt_exp_florestal_exploracao add finalidade numeric(38,0);
alter table IDAF.crt_exp_florestal_exploracao add parecer_favoravel numeric(1,0) default 0 not null;


--ALTER TABLE CRT_EXP_FLORESTAL_PRODUTO
alter table IDAF.crt_exp_florestal_produto add taxonomia_id NUMBER(38,0);
alter table IDAF.crt_exp_florestal_produto add taxonomia_nome VARCHAR2(200 BYTE);

COMMENT ON COLUMN IDAF.crt_exp_florestal_produto.taxonomia_id  IS 'Descrição da taxonomia cadastrada no Ibama.';
COMMENT ON COLUMN IDAF.crt_exp_florestal_produto.taxonomia_nome  IS 'Identificador da Taxonomia Científica cadastrada no Ibama.';

--ALTER TABLE HST_CRT_EXP_FLORESTAL_PRODUTO
alter table IDAF.hst_crt_exp_florestal_produto add taxonomia_id NUMBER(38,0);
alter table IDAF.hst_crt_exp_florestal_produto add taxonomia_nome VARCHAR2(200 BYTE);

COMMENT ON COLUMN IDAF.hst_crt_exp_florestal_produto.taxonomia_id  IS 'Descrição da taxonomia cadastrada no Ibama.';
COMMENT ON COLUMN IDAF.hst_crt_exp_florestal_produto.taxonomia_nome  IS 'Identificador da Taxonomia Científica cadastrada no Ibama.';