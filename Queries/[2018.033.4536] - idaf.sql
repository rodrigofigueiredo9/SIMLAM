--ALTER TABLE CRT_EXPLORACAO_FLORESTAL
alter table IDAF.CRT_EXPLORACAO_FLORESTAL add TIPO_EXPLORACAO NUMBER(38,0);
alter table IDAF.CRT_EXPLORACAO_FLORESTAL add DATA_CADASTRO DATE;
alter table IDAF.CRT_EXPLORACAO_FLORESTAL modify finalidade null;
alter table IDAF.CRT_EXP_FLORESTAL_EXPLORACAO modify EXPLORACAO_TIPO NULL;

--ALTER TABLE CRT_EXP_FLORESTAL_EXPLORACAO
alter table IDAF.crt_exp_florestal_exploracao add finalidade numeric(38,0);
alter table IDAF.crt_exp_florestal_exploracao add parecer_favoravel numeric(1,0) default 0 not null;
alter table IDAF.crt_exp_florestal_exploracao add finalidade_outros	VARCHAR2(80 BYTE);
update IDAF.crt_exp_florestal_exploracao ef set ef.finalidade = (select e.finalidade from CRT_EXPLORACAO_FLORESTAL e 
where e.id = ef.exploracao_florestal),
ef.finalidade_outros = (select e.finalidade_outros from CRT_EXPLORACAO_FLORESTAL e 
where e.id = ef.exploracao_florestal);


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