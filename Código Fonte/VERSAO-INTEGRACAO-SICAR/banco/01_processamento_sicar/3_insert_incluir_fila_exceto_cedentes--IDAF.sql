declare 
    v_tid varchar2(500) := '3DE3D456-2258-4F39-A691-D33C8AE591A3';
    v_id_controle_sicar number;
begin

	for i in (select x.*,
					 '{"origem": "' || x.origem || '", "empreendimento":' ||
					 x.emp_id || ', "empreendimento_tid": "' || x.emp_tid ||
					 '","solicitacao_car": ' || x.solic_id ||
					 ',"solicitacao_car_tid": "' || x.solic_tid || '"}' requisicao
				from (select c.id solic_id,
							 c.tid solic_tid,
							 c.situacao solic_situacao,
							 c.empreendimento emp_id,
							 (select ht.empreendimento_tid
								from hst_titulo ht
							   where ht.titulo_id = t.id
								 and ht.tid = t.tid
                                 and rownum=1) emp_tid,
							 'institucional' origem
						from tab_car_solicitacao c, tab_titulo t
                       where c.empreendimento = t.empreendimento
                         and t.modelo = (select ttm.id
                                           from tab_titulo_modelo ttm
                                          where ttm.codigo = 49)
                         and t.situacao = 3
                         and c.situacao <> 3 /*Inválido*/
                         and not exists ( select d.empreendimento 
                                from crt_dominialidade d, 
                                     crt_dominialidade_dominio dd, 
                                     crt_dominialidade_reserva dr 
                               where d.id= dd.dominialidade
                                 and dd.id=dr.dominio
                                 and dr.localizacao=3
                                 AND D.EMPREENDIMENTO=c.empreendimento )
                         and c.empreendimento not in (200, 283, 601, 1290, 1712, 2972, 3547, 4235, 5264, 10516, 10061, 9350, 8854, 8574, 8095, 7653, 7579, 6787, 12761, 12148, 12016, 11981, 11861, 11154, 11109, 10874, 10531, 25734, 25602, 24646, 23059, 21300, 21069, 20905, 20616, 19851, 46665, 44482, 43037, 38964, 38597, 37757, 37636, 37491, 33090, 31027, 29969, 29834, 29593, 27347, 26559, 26280, 26220, 19136, 19090, 18712, 18029, 17866, 17706, 17090, 17036, 16844, 15667, 15157, 14095, 14046, 13468, 13079, 13007, 69449, 67771, 66396, 63790, 62277, 60868, 59337, 59085, 58111, 55642, 55227, 50054, 36188, 35191, 33638, 4862, 3588, 3381, 1769, 1286)

                      union all

                      select c.id solic_id,
                             c.tid solic_tid,
                             c.situacao solic_situacao,
                             c.empreendimento emp_id,
                             (select hc.empreendimento_tid
                                from hst_car_solicitacao hc
                               where hc.solicitacao_id = c.id
                                 and hc.tid = c.tid
                                 and rownum=1) emp_tid,
                             'institucional' origem
                        from tab_car_solicitacao c
                       where c.situacao = 2 /*Válido*/
                         and not exists
                             (select c1.id
                                from tab_car_solicitacao c1, tab_titulo t1
                               where c1.empreendimento = t1.empreendimento
                                 and t1.modelo =
                                     (select ttm.id
                                        from tab_titulo_modelo ttm
                                       where ttm.codigo = 49)
                                 and t1.situacao = 3
                                 and c1.id = c.id)
                         and not exists ( select d.empreendimento 
                                from crt_dominialidade d, 
                                     crt_dominialidade_dominio dd, 
                                     crt_dominialidade_reserva dr 
                               where d.id= dd.dominialidade
                                 and dd.id=dr.dominio
                                 and dr.localizacao=3
                                 AND d.empreendimentO=c.empreendimento)
                         and c.empreendimento not in (200, 283, 601, 1290, 1712, 2972, 3547, 4235, 5264, 10516, 10061, 9350, 8854, 8574, 8095, 7653, 7579, 6787, 12761, 12148, 12016, 11981, 11861, 11154, 11109, 10874, 10531, 25734, 25602, 24646, 23059, 21300, 21069, 20905, 20616, 19851, 46665, 44482, 43037, 38964, 38597, 37757, 37636, 37491, 33090, 31027, 29969, 29834, 29593, 27347, 26559, 26280, 26220, 19136, 19090, 18712, 18029, 17866, 17706, 17090, 17036, 16844, 15667, 15157, 14095, 14046, 13468, 13079, 13007, 69449, 67771, 66396, 63790, 62277, 60868, 59337, 59085, 58111, 55642, 55227, 50054, 36188, 35191, 33638, 4862, 3588, 3381, 1769, 1286)
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

        COMMIT;
    end loop;
end;
/
