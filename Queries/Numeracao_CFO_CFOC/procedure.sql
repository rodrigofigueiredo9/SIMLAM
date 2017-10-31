---------------------------------------------------------
    -- Emissão de CFO 
    ---------------------------------------------------------
    procedure emissaocfo(p_id               number,
                         p_acao             number,
                         p_executor_id      number,
                         p_executor_nome    varchar2,
                         p_executor_login   varchar2,
                         p_executor_tipo_id number,
                         p_executor_tid     varchar2) is
        v_sucesso boolean := false;
        v_id      number;
    begin
        for i in (select c.id,
                         c.tid,
                         c.tipo_numero,
                         lt.texto                         tipo_numero_texto,
                         c.numero,
                         c.data_emissao,
                         c.data_ativacao,
                         c.situacao,
                         ls.texto                         situacao_texto,
                         c.produtor,
                         p.tid                            produtor_tid,
                         c.empreendimento,
                         e.tid                            empreendimento_tid,
                         c.possui_laudo_laboratorial,
                         c.nome_laboratorio,
                         c.numero_laudo_resultado_analise,
                         c.estado,
                         le.texto                         estado_texto,
                         c.municipio,
                         lm.texto                         municipio_texto,
                         c.produto_especificacao,
                         c.possui_trat_fito_fins_quaren,
                         c.partida_lacrada_origem,
                         c.numero_lacre,
                         c.numero_porao,
                         c.numero_container,
                         c.validade_certificado,
                         c.informacoes_complementares,
                         c.informacoes_complement_html,
                         c.estado_emissao,
                         lee.texto                        estado_emissao_texto,
                         c.municipio_emissao,
                         lme.texto                        municipio_emissao_texto,
                         c.serie
                    from tab_cfo                       c,
                         lov_doc_fitossani_tipo_numero lt,
                         lov_doc_fitossani_situacao    ls,
                         ins_empreendimento            e,
                         ins_pessoa                    p,
                         lov_estado                    le,
                         lov_municipio                 lm,
                         lov_estado                    lee,
                         lov_municipio                 lme
                   where lt.id = c.tipo_numero
                     and ls.id = c.situacao
                     and e.id = c.empreendimento
                     and p.id = c.produtor
                     and le.id(+) = c.estado
                     and lm.id(+) = c.municipio
                     and lee.id(+) = c.estado_emissao
                     and lme.id(+) = c.municipio_emissao
                     and c.id = p_id) loop
            -- Inserindo na histórico
            insert into hst_cfo
                (id,
                 tid,
                 cfo_id,
                 tipo_numero_id,
                 tipo_numero_texto,
                 numero,
                 data_emissao,
                 data_ativacao,
                 situacao_id,
                 situacao_texto,
                 produtor_id,
                 produtor_tid,
                 empreendimento_id,
                 empreendimento_tid,
                 possui_laudo_laboratorial,
                 nome_laboratorio,
                 numero_laudo_resultado_analise,
                 estado_id,
                 estado_texto,
                 municipio_id,
                 municipio_texto,
                 produto_especificacao,
                 possui_trat_fito_fins_quaren,
                 partida_lacrada_origem,
                 numero_lacre,
                 numero_porao,
                 numero_container,
                 validade_certificado,
                 informacoes_complementares,
                 informacoes_complement_html,
                 estado_emissao_id,
                 estado_emissao_texto,
                 municipio_emissao_id,
                 municipio_emissao_texto,
                 serie,
                 executor_id,
                 executor_tid,
                 executor_nome,
                 executor_login,
                 executor_tipo_id,
                 executor_tipo_texto,
                 acao_executada,
                 data_execucao)
            values
                (seq_hst_cfo.nextval,
                 i.tid,
                 i.id,
                 i.tipo_numero,
                 i.tipo_numero_texto,
                 i.numero,
                 i.data_emissao,
                 i.data_ativacao,
                 i.situacao,
                 i.situacao_texto,
                 i.produtor,
                 i.produtor_tid,
                 i.empreendimento,
                 i.empreendimento_tid,
                 i.possui_laudo_laboratorial,
                 i.nome_laboratorio,
                 i.numero_laudo_resultado_analise,
                 i.estado,
                 i.estado_texto,
                 i.municipio,
                 i.municipio_texto,
                 i.produto_especificacao,
                 i.possui_trat_fito_fins_quaren,
                 i.partida_lacrada_origem,
                 i.numero_lacre,
                 i.numero_porao,
                 i.numero_container,
                 i.validade_certificado,
                 i.informacoes_complementares,
                 i.informacoes_complement_html,
                 i.estado_emissao,
                 i.estado_emissao_texto,
                 i.municipio_emissao,
                 i.municipio_emissao_texto,
                 i.serie,
                 p_executor_id,
                 p_executor_tid,
                 p_executor_nome,
                 p_executor_login,
                 p_executor_tipo_id,
                 (select texto
                    from lov_executor_tipo
                   where id = p_executor_tipo_id),
                 (select la.id
                    from lov_historico_artefatos_acoes la
                   where la.acao = p_acao
                     and la.artefato = 137),
                 systimestamp)
            returning id into v_id;
        
            insert into hst_cfo_produto
                (id,
                 tid,
                 id_hst,
                 unidade_producao_id,
                 unidade_producao_tid,
                 quantidade,
                 inicio_colheita,
                 fim_colheita)
                (select seq_hst_cfo_produto.nextval,
                        c.tid,
                        v_id,
                        c.unidade_producao,
                        u.tid,
                        c.quantidade,
                        c.inicio_colheita,
                        c.fim_colheita
                   from tab_cfo_produto c, ins_crt_unidade_prod_unidade u
                  where c.unidade_producao = u.id
                    and c.cfo = i.id);
        
            insert into hst_cfo_praga
                (id, tid, id_hst, praga_id, praga_tid)
                (select seq_hst_cfo_praga.nextval,
                        c.tid,
                        v_id,
                        c.praga,
                        p.tid
                   from tab_cfo_praga c, tab_praga p
                  where p.id = c.praga
                    and c.cfo = i.id);
        
            insert into hst_cfo_trata_fitossa
                (id,
                 tid,
                 id_hst,
                 produto_comercial,
                 ingrediente_ativo,
                 dose,
                 praga_produto,
                 modo_aplicacao)
                (select seq_hst_cfo_trata_fitossa.nextval,
                        c.tid,
                        v_id,
                        c.produto_comercial,
                        c.ingrediente_ativo,
                        c.dose,
                        c.praga_produto,
                        c.modo_aplicacao
                   from tab_cfo_trata_fitossa c
                  where c.cfo = i.id);
        
        end loop;
        ----------------------------------------------------------------------------------------------------------------
        v_sucesso := true;
    
        if (not v_sucesso) then
            Raise_application_error(-20000,
                                    'Erro ao gerar o Histórico de Emissão de CFO. Mensagem: ' ||
                                    dbms_utility.format_error_stack ||
                                    dbms_utility.format_call_stack);
        end if;
        --Tratamento de exceção
    exception
        when others then
            Raise_application_error(-20000,
                                    'Erro ao gerar o Histórico de Emissão de CFO. Mensagem: ' ||
                                    dbms_utility.format_error_stack ||
                                    dbms_utility.format_call_stack);
    end;
    
    
       ---------------------------------------------------------
    -- Emissão de CFOC
    ---------------------------------------------------------
    procedure emissaocfoc(p_id               number,
                          p_acao             number,
                          p_executor_id      number,
                          p_executor_nome    varchar2,
                          p_executor_login   varchar2,
                          p_executor_tipo_id number,
                          p_executor_tid     varchar2) is
        v_sucesso boolean := false;
        v_id      number;
    begin
        for i in (select c.id,
                         c.tid,
                         c.tipo_numero,
                         lt.texto                         tipo_numero_texto,
                         c.numero,
                         c.data_emissao,
                         c.data_ativacao,
                         c.situacao,
                         ls.texto                         situacao_texto,
                         c.empreendimento,
                         e.tid                            empreendimento_tid,
                         c.possui_laudo_laboratorial,
                         c.nome_laboratorio,
                         c.numero_laudo_resultado_analise,
                         c.estado,
                         le.texto                         estado_texto,
                         c.municipio,
                         lm.texto                         municipio_texto,
                         c.produto_especificacao,
                         c.possui_trat_fito_fins_quaren,
                         c.partida_lacrada_origem,
                         c.numero_lacre,
                         c.numero_porao,
                         c.numero_container,
                         c.validade_certificado,
                         c.informacoes_complementares,
                         c.informacoes_complement_html,
                         c.estado_emissao,
                         lee.texto                        estado_emissao_texto,
                         c.municipio_emissao,
                         lme.texto                        municipio_emissao_texto,
                         c.serie
                    from tab_cfoc                      c,
                         lov_doc_fitossani_tipo_numero lt,
                         lov_doc_fitossani_situacao    ls,
                         ins_empreendimento            e,
                         lov_estado                    le,
                         lov_municipio                 lm,
                         lov_estado                    lee,
                         lov_municipio                 lme
                   where lt.id = c.tipo_numero
                     and ls.id = c.situacao
                     and e.id = c.empreendimento
                     and le.id(+) = c.estado
                     and lm.id(+) = c.municipio
                     and lee.id(+) = c.estado_emissao
                     and lme.id(+) = c.municipio_emissao
                     and c.id = p_id) loop
            -- Inserindo na histórico
            insert into hst_cfoc
                (id,
                 tid,
                 cfoc_id,
                 tipo_numero_id,
                 tipo_numero_texto,
                 numero,
                 data_emissao,
                 data_ativacao,
                 situacao_id,
                 situacao_texto,
                 empreendimento_id,
                 empreendimento_tid,
                 possui_laudo_laboratorial,
                 nome_laboratorio,
                 numero_laudo_resultado_analise,
                 estado_id,
                 estado_texto,
                 municipio_id,
                 municipio_texto,
                 produto_especificacao,
                 possui_trat_fito_fins_quaren,
                 partida_lacrada_origem,
                 numero_lacre,
                 numero_porao,
                 numero_container,
                 validade_certificado,
                 informacoes_complementares,
                 informacoes_complement_html,
                 estado_emissao_id,
                 estado_emissao_texto,
                 municipio_emissao_id,
                 municipio_emissao_texto,
                 serie,
                 executor_id,
                 executor_tid,
                 executor_nome,
                 executor_login,
                 executor_tipo_id,
                 executor_tipo_texto,
                 acao_executada,
                 data_execucao)
            values
                (seq_hst_cfoc.nextval,
                 i.tid,
                 i.id,
                 i.tipo_numero,
                 i.tipo_numero_texto,
                 i.numero,
                 i.data_emissao,
                 i.data_ativacao,
                 i.situacao,
                 i.situacao_texto,
                 i.empreendimento,
                 i.empreendimento_tid,
                 i.possui_laudo_laboratorial,
                 i.nome_laboratorio,
                 i.numero_laudo_resultado_analise,
                 i.estado,
                 i.estado_texto,
                 i.municipio,
                 i.municipio_texto,
                 i.produto_especificacao,
                 i.possui_trat_fito_fins_quaren,
                 i.partida_lacrada_origem,
                 i.numero_lacre,
                 i.numero_porao,
                 i.numero_container,
                 i.validade_certificado,
                 i.informacoes_complementares,
                 i.informacoes_complement_html,
                 i.estado_emissao,
                 i.estado_emissao_texto,
                 i.municipio_emissao,
                 i.municipio_emissao_texto,
                 i.serie,
                 p_executor_id,
                 p_executor_tid,
                 p_executor_nome,
                 p_executor_login,
                 p_executor_tipo_id,
                 (select texto
                    from lov_executor_tipo
                   where id = p_executor_tipo_id),
                 (select la.id
                    from lov_historico_artefatos_acoes la
                   where la.acao = p_acao
                     and la.artefato = 138),
                 systimestamp)
            returning id into v_id;
        
            insert into hst_cfoc_produto
                (id, tid, id_hst, lote_id, lote_tid)
                (select seq_hst_cfoc_produto.nextval,
                        c.tid,
                        v_id,
                        c.lote,
                        l.tid
                   from tab_cfoc_produto c, tab_lote l
                  where c.lote = l.id
                    and c.cfoc = i.id);
        
            insert into hst_cfoc_praga
                (id, tid, id_hst, praga_id, praga_tid)
                (select seq_hst_cfoc_praga.nextval,
                        c.tid,
                        v_id,
                        c.praga,
                        p.tid
                   from tab_cfoc_praga c, tab_praga p
                  where p.id = c.praga
                    and c.cfoc = i.id);
        
            insert into hst_cfoc_trata_fitossa
                (id,
                 tid,
                 id_hst,
                 produto_comercial,
                 ingrediente_ativo,
                 dose,
                 praga_produto,
                 modo_aplicacao)
                (select seq_hst_cfoc_trata_fitossa.nextval,
                        c.tid,
                        v_id,
                        c.produto_comercial,
                        c.ingrediente_ativo,
                        c.dose,
                        c.praga_produto,
                        c.modo_aplicacao
                   from tab_cfoc_trata_fitossa c
                  where c.cfoc = i.id);
        
        end loop;
        ----------------------------------------------------------------------------------------------------------------
        v_sucesso := true;
    
        if (not v_sucesso) then
            Raise_application_error(-20000,
                                    'Erro ao gerar o Histórico de Emissão de CFOC. Mensagem: ' ||
                                    dbms_utility.format_error_stack ||
                                    dbms_utility.format_call_stack);
        end if;
        --Tratamento de exceção
    exception
        when others then
            Raise_application_error(-20000,
                                    'Erro ao gerar o Histórico de Emissão de CFOC. Mensagem: ' ||
                                    dbms_utility.format_error_stack ||
                                    dbms_utility.format_call_stack);
    end;