    ---------------------------------------------------------
    -- Solicitar Emissão de Permissão de Transito de Vegetais
    ---------------------------------------------------------
    procedure emitirptv(p_id               number,
                        p_acao             number,
                        p_executor_id      number,
                        p_executor_nome    varchar2,
                        p_executor_login   varchar2,
                        p_executor_tipo_id number,
                        p_executor_tid     varchar2) is
        v_sucesso boolean := false;
        v_id      number := 0;
    begin
        for i in (select p.tid,
                         p.id,
                         (case when (p.situacao = 1 and (select count(1) from hst_ptv where PTV_ID = p_id and situacao_id = 1) > 0 ) then 8 /*Editado*/ else p.situacao END) situacao,
                         (case when (p.situacao = 1 and (select count(1) from hst_ptv where PTV_ID = p_id and situacao_id = 1) > 0 ) then 'Editado' else ls.texto END) situacao_texto,
                         p.situacao_data,
                         p.motivo,
                         p.tipo_numero,
                         tn.texto tipo_numero_texto,
                         p.numero,
                         p.dua_numero,
                         p.dua_tipo_pessoa,
                         p.dua_cpf_cnpj,
                         p.data_emissao,
                         p.data_ativacao,
                         p.data_cancelamento,
                         p.empreendimento,
                         e.tid empreendimento_tid,
                         p.responsavel_emp,
                         re.tid responsavel_emp_tid,
                         p.partida_lacrada_origem,
                         p.numero_lacre,
                         p.numero_porao,
                         p.numero_container,
                         p.destinatario,
                         d.tid destinatario_tid,
                         p.possui_laudo_laboratorial,
                         p.nome_laboratorio,
                         p.numero_laudo_resultado_analise,
                         p.estado,
                         le.sigla estado_texto,
                         p.municipio,
                         lm.texto municipio_texto,
                         p.tipo_transporte,
                         tp.texto tipo_transporte_texto,
                         p.veiculo_identificacao_numero,
                         p.rota_transito_definida,
                         p.itinerario,
                         p.apresentacao_nota_fiscal,
                         p.numero_nota_fiscal,
                         p.local_vistoria,
                         (select s.tid
                            from tab_setor s
                           where s.id = p.local_vistoria) local_vistoria_tid,
                         p.data_hora_vistoria,
                         (select c.tid
                            from cnf_local_vistoria c
                           where c.id = p.data_hora_vistoria) data_hora_vistoria_tid,
                         p.credenciado,
                         (select c.tid
                            from tab_credenciado c
                           where c.id = p.credenciado) credenciado_tid
                    from tab_ptv                       p,
                         lov_doc_fitossani_tipo_numero tn,
                         lov_solicitacao_ptv_situacao  ls,
                         ins_empreendimento            e,
                         ins_pessoa                    re,
                         tab_destinatario_ptv          d,
                         lov_estado                    le,
                         lov_municipio                 lm,
                         lov_tipo_transporte           tp
                   where tn.id = p.tipo_numero
                     and ls.id = p.situacao
                     and e.id = p.empreendimento
                     and re.id(+) = p.responsavel_emp
                     and d.id = p.destinatario
                     and le.id(+) = p.estado
                     and lm.id(+) = p.municipio
                     and tp.id = p.tipo_transporte
                     and p.id = p_id) loop
        
            -- insert historico PTV
            insert into hst_ptv
                (id,
                 tid,
                 ptv_id,
                 situacao_id,
                 situacao_texto,
                 situacao_data,
                 motivo,
                 tipo_numero_id,
                 tipo_numero_texto,
                 numero,
                 dua_numero,
                 dua_tipo_pessoa,
                 dua_cpf_cnpj,
                 data_emissao,
                 data_ativacao,
                 data_cancelamento,
                 empreendimento_id,
                 empreendimento_tid,
                 responsavel_emp_id,
                 responsavel_emp_tid,
                 partida_lacrada_origem,
                 numero_lacre,
                 numero_porao,
                 numero_container,
                 destinatario_id,
                 destinatario_tid,
                 possui_laudo_laboratorial,
                 nome_laboratorio,
                 numero_laudo_resultado_analise,
                 estado_id,
                 estado_texto,
                 municipio_id,
                 municipio_texto,
                 tipo_transporte_id,
                 tipo_transporte_texto,
                 veiculo_identificacao_numero,
                 rota_transito_definida,
                 itinerario,
                 apresentacao_nota_fiscal,
                 numero_nota_fiscal,
                 local_vistoria_id,
                 local_vistoria_tid,
                 data_hora_vistoria_id,
                 data_hora_vistoria_tid,
                 credenciado_id,
                 credenciado_tid,
                 executor_id,
                 executor_tid,
                 executor_nome,
                 executor_login,
                 executor_tipo_id,
                 executor_tipo_texto,
                 acao_executada,
                 data_execucao)
            values
                (seq_hst_ptv.nextval,
                 i.tid,
                 i.id,
                 i.situacao,
                 i.situacao_texto,
                 i.situacao_data,
                 i.motivo,
                 i.tipo_numero,
                 i.tipo_numero_texto,
                 i.numero,
                 i.dua_numero,
                 i.dua_tipo_pessoa,
                 i.dua_cpf_cnpj,
                 i.data_emissao,
                 i.data_ativacao,
                 i.data_cancelamento,
                 i.empreendimento,
                 i.empreendimento_tid,
                 i.responsavel_emp,
                 i.responsavel_emp_tid,
                 i.partida_lacrada_origem,
                 i.numero_lacre,
                 i.numero_porao,
                 i.numero_container,
                 i.destinatario,
                 i.destinatario_tid,
                 i.possui_laudo_laboratorial,
                 i.nome_laboratorio,
                 i.numero_laudo_resultado_analise,
                 i.estado,
                 i.estado_texto,
                 i.municipio,
                 i.municipio_texto,
                 i.tipo_transporte,
                 i.tipo_transporte_texto,
                 i.veiculo_identificacao_numero,
                 i.rota_transito_definida,
                 i.itinerario,
                 i.apresentacao_nota_fiscal,
                 i.numero_nota_fiscal,
                 i.local_vistoria,
                 i.local_vistoria_tid,
                 i.data_hora_vistoria,
                 i.data_hora_vistoria_tid,
                 i.credenciado,
                 i.credenciado_tid,
                 p_executor_id,
                 p_executor_tid,
                 p_executor_nome,
                 p_executor_login,
                 p_executor_tipo_id,
                 (select ltf.texto
                    from lov_executor_tipo ltf
                   where ltf.id = p_executor_tipo_id),
                 (select la.id
                    from lov_historico_artefatos_acoes la
                   where la.acao = p_acao
                     and la.artefato = 140),
                 systimestamp)
            returning id into v_id;
        
            -- insert historico Produto PTV
            for j in (select p.id,
                             p.tid,
                             p.ptv,
                             p.origem_tipo,
                             lt.texto origem_tipo_texto,
                             p.origem,
                             p.numero_origem,
                             case p.origem_tipo
                                 when 1 then
                                  (select t.tid
                                     from tab_cfo t
                                    where t.id = p.origem)
                                 when 2 then
                                  (select t.tid
                                     from tab_cfoc t
                                    where t.id = p.origem)
                                 when 3 then
                                  (select t.tid
                                     from ins_ptv t
                                    where t.id = p.origem)
                                 else
                                  null
                             end as origem_tid,
                             p.cultura,
                             c.tid cultura_tid,
                             p.cultivar,
                             cc.tid cultivar_tid,
                             p.quantidade,
                             p.unidade_medida,
                             u.texto unidade_medida_texto
                        from tab_ptv_produto              p,
                             lov_doc_fitossanitarios_tipo lt,
                             tab_cultura                  c,
                             tab_cultura_cultivar         cc,
                             lov_crt_uni_prod_uni_medida  u
                       where lt.id = p.origem_tipo
                         and c.id = p.cultura
                         and cc.id = p.cultivar
                         and u.id(+) = p.unidade_medida
                         and p.ptv = p_id) loop
            
                insert into hst_ptv_produto
                    (id,
                     tid,
                     id_hst,
                     ptv_id,
                     origem_tipo_id,
                     origem_tipo_texto,
                     origem_id,
                     numero_origem,
                     origem_tid,
                     cultura_id,
                     cultura_tid,
                     cultivar_id,
                     cultivar_tid,
                     quantidade,
                     unidade_medida_id,
                     unidade_medida_texto)
                values
                    (seq_hst_ptv_produto.nextval,
                     j.tid,
                     v_id,
                     j.ptv,
                     j.origem_tipo,
                     j.origem_tipo_texto,
                     j.origem,
                     j.numero_origem,
                     j.origem_tid,
                     j.cultura,
                     j.cultura_tid,
                     j.cultivar,
                     j.cultivar_tid,
                     j.quantidade,
                     j.unidade_medida,
                     j.unidade_medida_texto);
            end loop;
			
			----------------------------------------------------------------------------------------------------------------
			-- Notas Fiscais de Caixa
			----------------------------------------------------------------------------------------------------------------
			for j in (select tpnf.id,
							 tpnf.tid,
							 tnf.id nf_caixa_id,
							 tnf.tid nf_caixa_tid,
							 tpnf.saldo_atual,
							 tpnf.numero_caixas
						from tab_ptv_nf_caixa tpnf,
							 IDAF.tab_nf_caixa tnf
					   where tpnf.nf_caixa = tnf.id
							 and tpnf.ptv = p_id) loop
				insert into hst_ptv_nf_caixa
					(id,
					 tid,
					 id_hst,
					 ptv_nf_caixa_id,
					 ptv_id,
					 nf_caixa_id,
					 nf_caixa_tid,
					 saldo_atual,
					 numero_caixas)
				values
					(seq_hst_ptv_nf_caixa.nextval,
					 i.tid,
					 v_id,
					 j.id,
					 i.id,
					 j.nf_caixa_id,
					 j.nf_caixa_tid,
					 j.saldo_atual,
					 j.numero_caixas);
			end loop;
		
            ----------------------------------------------------------------------------------------------------------------
            -- Arquivos/Itens
            ----------------------------------------------------------------------------------------------------------------
            for j in (select tr.id,
                             tr.arquivo   arquivo_id,
                             ta.tid       arquivo_tid,
                             tr.ordem,
                             tr.descricao,
                             tr.tid
                        from tab_ptv_arquivo tr, tab_arquivo ta
                       where tr.arquivo = ta.id
                         and tr.ptv = p_id) loop
                insert into hst_ptv_arquivo
                    (id,
                     id_hst,
                     ptv_arq_id,
                     ptv_id,
                     arquivo_id,
                     arquivo_tid,
                     ordem,
                     descricao,
                     tid)
                values
                    (seq_hst_ptv_arquivo.nextval,
                     v_id,
                     j.id,
                     i.id,
                     j.arquivo_id,
                     j.arquivo_tid,
                     j.ordem,
                     j.descricao,
                     i.tid);
            end loop;
        
            -- roda Historico de Conversa
            if (p_acao = 3 /*Excluir*/
               ) then
                for h in (select tpc.id
                            from tab_ptv_comunicador tpc
                           where tpc.ptv_id = p_id) loop
                    ptvcomunicador(h.id,
                                   p_acao,
                                   p_executor_id,
                                   p_executor_nome,
                                   p_executor_login,
                                   p_executor_tipo_id,
                                   p_executor_tid);
                end loop;
            end if;
        
        end loop;
    
        v_sucesso := true;
    
        if (not v_sucesso) then
            Raise_application_error(-20000,
                                    'Erro ao gerar o Histórico de PTV. Mensagem: ' ||
                                    dbms_utility.format_error_stack ||
                                    dbms_utility.format_call_stack);
        end if;
    
        --Tratamento de exceção
    exception
        when others then
            Raise_application_error(-20000,
                                    'Erro ao gerar o Histórico de PTV. Mensagem: ' ||
                                    dbms_utility.format_error_stack ||
                                    dbms_utility.format_call_stack);
    end;