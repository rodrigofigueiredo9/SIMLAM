declare 
    v_tid varchar2(500) := '3DE3D456-2258-4F39-A691-D33C8AE591A3';
    v_id_controle_sicar number;
begin

	for i in (select x.*,
                     '{"origem": "' || x.origem || '", "empreendimento":' ||
                     x.emp_id || ', "empreendimento_tid": "' || x.emp_tid ||
                     '","solicitacao_car": ' || x.solic_id ||
                     ',"solicitacao_car_tid": "' || x.solic_tid || '"}' requisicao
                from (select cc.id solic_id,
                             cc.tid solic_tid,
                             cc.situacao solic_situacao,
                             cc.empreendimento emp_id,
                             (select hc.empreendimento_tid
                                from hst_car_solicitacao hc
                               where hc.solicitacao_id = cc.id
                                 and hc.tid = cc.tid
                                 and rownum=1) emp_tid,
                             'credenciado' origem
                        from tab_car_solicitacao cc,
                             tab_empreendimento tec
                       where cc.empreendimento = tec.id
                         and cc.situacao = 2 /*Válido*/
                         and tec.codigo is null
                         and exists ( select d.empreendimento 
                                from crt_dominialidade d, 
                                     crt_dominialidade_dominio dd, 
                                     crt_dominialidade_reserva dr 
                               where d.id= dd.dominialidade
                                 and dd.id=dr.dominio
                                 and dr.localizacao=3
                                 AND D.EMPREENDIMENTO=tec.id)
                         and tec.id not in (1127, 1650, 37283, 30820, 28122, 26546, 25967, 25331, 24906, 3077, 3064, 2746, 2245, 2233, 2143, 2087, 1983, 1804, 19549, 16752, 16298, 15288, 14933, 14735, 12359, 12197, 12032, 11709, 11451, 11410, 11072, 10652, 9251, 12635, 8966, 8519, 7605, 7546, 7359, 7343, 7305, 7006, 36405, 36357, 34795, 34102, 33555, 33380, 9574, 9459, 9403, 6791, 6640, 6535, 6361, 6031, 5869, 5845, 5790, 5351, 5100, 4779, 4643, 3797, 3509, 3468, 3412, 1684, 1647, 1257, 1135, 1406, 1139)
                      union all
                      
                      select cc.id solic_id,
                             cc.tid solic_tid,
                             cc.situacao solic_situacao,
                             cc.empreendimento emp_id,
                             (select hc.empreendimento_tid
                                from hst_car_solicitacao hc
                               where hc.solicitacao_id = cc.id
                                 and hc.tid = cc.tid
                                 and rownum=1) emp_tid,
                             'credenciado' origem
                        from tab_car_solicitacao cc,
                             tab_empreendimento tec,
                             ins_empreendimento te
                       where cc.empreendimento = tec.id
                         and cc.situacao = 2 /*Válido*/
                         and tec.codigo = te.codigo
                         and exists ( select d.empreendimento 
                                from crt_dominialidade d, 
                                     crt_dominialidade_dominio dd, 
                                     crt_dominialidade_reserva dr 
                               where d.id= dd.dominialidade
                                 and dd.id=dr.dominio
                                 and dr.localizacao=3
                                 AND D.EMPREENDIMENTO=tec.id)
                         and te.id not in
                             (select c.empreendimento
                                from INS_CAR_SOLICITACAO c, tab_titulo t
                               where c.empreendimento = t.empreendimento
                               and c.situacao <> 3 /*Inválido*/
                                 and t.modelo =
                                     (select ttm.id
                                        from tab_titulo_modelo ttm
                                       where ttm.codigo = 49)
                                 and t.situacao = 3)
                         and te.id not in
                             (select c.empreendimento
                                from INS_CAR_SOLICITACAO c
                               where c.situacao = 2 /*Válido*/
                                 and c.id not in
                                     (select c.id
                                        from INS_CAR_SOLICITACAO c,
                                             tab_titulo          t
                                       where c.empreendimento =
                                             t.empreendimento
                                         and t.modelo =
                                             (select ttm.id
                                                from tab_titulo_modelo ttm
                                               where ttm.codigo = 49)
                                         and t.situacao = 3))
                         and tec.id not in (1127, 1650, 37283, 30820, 28122, 26546, 25967, 25331, 24906, 3077, 3064, 2746, 2245, 2233, 2143, 2087, 1983, 1804, 19549, 16752, 16298, 15288, 14933, 14735, 12359, 12197, 12032, 11709, 11451, 11410, 11072, 10652, 9251, 12635, 8966, 8519, 7605, 7546, 7359, 7343, 7305, 7006, 36405, 36357, 34795, 34102, 33555, 33380, 9574, 9459, 9403, 6791, 6640, 6535, 6361, 6031, 5869, 5845, 5790, 5351, 5100, 4779, 4643, 3797, 3509, 3468, 3412, 1684, 1647, 1257, 1135, 1406, 1139)                
                                         ) x) loop
		select SEQ_TAB_CONTROLE_SICAR.NEXTVAL
		  into v_id_controle_sicar
		  from dual;
	
		--*CRIAR REGISTRO DE CONTROLE SICAR
		insert into TAB_CONTROLE_SICAR
			(id,tid,empreendimento,empreendimento_tid,solicitacao_car,solicitacao_car_tid,situacao_envio,solicitacao_car_esquema,solicitacao_passivo,solicitacao_situacao_aprovado)
		values
			(v_id_controle_sicar,v_tid,i.emp_id,i.emp_tid,i.solic_id,i.solic_tid,1 /*Aguardando Envio*/,(case when i.origem = 'institucional' then 1 when i.origem = 'credenciado' then 2 end),1 /*solicitacao passiva*/,i.solic_situacao);

		--*CRIAR HISTORICO DE CONTROLE SICAR
		insert into HST_CONTROLE_SICAR
			(id,controle_sicar_id,tid,empreendimento,empreendimento_tid,solicitacao_car,solicitacao_car_tid,situacao_envio,solicitacao_car_esquema,data_execucao,solicitacao_passivo,solicitacao_situacao_aprovado)
		values
			(SEQ_HST_CONTROLE_SICAR.nextval,v_id_controle_sicar,v_tid,i.emp_id,i.emp_tid,i.solic_id,i.solic_tid,1 /*Aguardando Envio*/,(case when i.origem = 'institucional' then 1 when i.origem = 'credenciado' then 2 end), CURRENT_TIMESTAMP,1 /*solicitacao passiva*/,i.solic_situacao);

		--*CRIAR REGISTRO NA FILA DO SCHEDULER
		insert into TAB_SCHEDULER_FILA
			(id,tipo,requisitante,requisicao,empreendimento,data_criacao,data_conclusao,resultado,sucesso)
		values
			(seq_tab_scheduler_fila.nextval,'gerar-car',0,i.requisicao,0,NULL,NULL,'','');

	end loop;
end;
/
