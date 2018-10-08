  CREATE SEQUENCE seq_integracao_sinaflor INCREMENT BY 1 START WITH 1 NOMAXVALUE MINVALUE 1 NOCACHE;

  CREATE TABLE "IDAF"."TAB_INTEGRACAO_SINAFLOR"
  (
    "ID"                NUMBER(38,0) NOT NULL ENABLE,
    "DESCRICAO"         VARCHAR(4000) NOT NULL ENABLE,
    "SITUACAO"          VARCHAR(255) NOT NULL ENABLE,
    "SERVICO"           VARCHAR(255) NOT NULL ENABLE,
    "DATA"              TIMESTAMP NOT NULL ENABLE,
    CONSTRAINT "PK_INTEGRACAO_SINAFLOR" PRIMARY KEY ("ID") USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645 PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT) TABLESPACE "IDAF_TBS" ENABLE
  )
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 NOCOMPRESS LOGGING STORAGE
  (
    INITIAL 196608 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645 PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1 BUFFER_POOL DEFAULT
  )
  TABLESPACE "IDAF_TBS" ;
 
  COMMENT ON COLUMN "IDAF"."TAB_INTEGRACAO_SINAFLOR"."ID" IS 'Chave primaria. Utiliza a sequencia seq_integracao_sinaflor.';
  
  COMMENT ON COLUMN "IDAF"."TAB_INTEGRACAO_SINAFLOR"."DESCRICAO" IS 'Informa o resultado da integracao com o SINAFLOR.';
  
  COMMENT ON COLUMN "IDAF"."TAB_INTEGRACAO_SINAFLOR"."SITUACAO" IS 'Informa a situacao da integracao.';
  
  COMMENT ON COLUMN "IDAF"."TAB_INTEGRACAO_SINAFLOR"."SERVICO" IS 'Informa o servico que realizara integracao.';
  
  COMMENT ON COLUMN "IDAF"."TAB_INTEGRACAO_SINAFLOR"."DATA" IS 'Informa a data da integracao no SINAFLOR.';
  
  COMMENT ON TABLE "IDAF"."TAB_INTEGRACAO_SINAFLOR"  IS 'Tabela de integracao com SINAFLOR.';

  COMMENT ON COLUMN IDAF.CRT_EXPLORACAO_FLORESTAL.CODIGO_EXPLORACAO IS 'Codigo identificador acrescido de acordo com o tipo de atividade (tipo_exploracao).';
  COMMENT ON COLUMN IDAF.CRT_EXPLORACAO_FLORESTAL.TIPO_EXPLORACAO IS 'Código do tipo de atividade cadastrado no IBAMA (idafgeo.lov_tipo_exploracao CAMPO(tipo_atividade)).';
  COMMENT ON COLUMN IDAF.CRT_EXPLORACAO_FLORESTAL.DATA_CADASTRO IS 'Data de cadastro da exploracao no IDAF.';
  COMMENT ON COLUMN IDAF.CRT_EXPLORACAO_FLORESTAL.FINALIDADE IS 'Finalidade da exploracao (coluna criada na tabela filha) - não utilizada.';

begin
	
	update idaf.LOV_CRT_PRODUTO set texto = 'Lenha (st)' where id = 1;
	update idaf.LOV_CRT_PRODUTO set texto = 'Tora (m³)' where id = 2;	
	update idaf.LOV_CRT_PRODUTO set texto = 'Toretes (m³)' where id = 3;	
	update idaf.LOV_CRT_PRODUTO set texto = 'Mourões (m³)' where id = 4;	
	update idaf.LOV_CRT_PRODUTO set texto = 'Escoramento (m³)' where id = 5;	
	update idaf.LOV_CRT_PRODUTO set texto = 'Palmito (und)' where id = 6;
	
	insert into idaf.LOV_CRT_PRODUTO (id, texto)
	select 8, 'Casca (kg)' from dual where not exists (select 1 from idaf.LOV_CRT_PRODUTO where id = 8);
	
	insert into idaf.LOV_CRT_PRODUTO (id, texto)
	select 9, 'Folhas (kg)' from dual where not exists (select 1 from idaf.LOV_CRT_PRODUTO where id = 9);
	
	insert into idaf.LOV_CRT_PRODUTO (id, texto)
	select 10, 'Muda/Planta (und)' from dual where not exists (select 1 from idaf.LOV_CRT_PRODUTO where id = 10);
	
	update idaf.CRT_EXP_FLORESTAL_PRODUTO set PRODUTO = 8 where PRODUTO = 7;
	delete from idaf.LOV_CRT_PRODUTO where id = 7;

end;

begin 
	update idaf.LOV_CRT_EXP_FLORES_FINALIDADE set texto = 'Infraestrutura' where id = 4;
	update idaf.LOV_CRT_EXP_FLORES_FINALIDADE set texto = 'Mineração' where id = 7;

	delete from idaf.LOV_CRT_EXP_FLORES_FINALIDADE where id in (2, 3);
end;

begin 
	insert into idaf.lov_dest_material_lenhoso (id, texto) values (seq_dest_material_lenhoso.nextval, 'Lenha para carvão');
	insert into idaf.lov_dest_material_lenhoso (id, texto) values (seq_dest_material_lenhoso.nextval, 'Lenha para outros fins');
	insert into idaf.lov_dest_material_lenhoso (id, texto) values (seq_dest_material_lenhoso.nextval, 'Lenha para uso dentro da propriedade');
	insert into idaf.lov_dest_material_lenhoso (id, texto) values (seq_dest_material_lenhoso.nextval, 'Lenha para uso doméstico');
	insert into idaf.lov_dest_material_lenhoso (id, texto) values (seq_dest_material_lenhoso.nextval, 'Madeira para outros fins');
	insert into idaf.lov_dest_material_lenhoso (id, texto) values (seq_dest_material_lenhoso.nextval, 'Madeira para serraria');
	insert into idaf.lov_dest_material_lenhoso (id, texto) values (seq_dest_material_lenhoso.nextval, 'Maderia para uso dentro da propriedade');
	insert into idaf.lov_dest_material_lenhoso (id, texto) values (seq_dest_material_lenhoso.nextval, 'Outro produto florestal');
end;