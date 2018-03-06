  	----------------------------------------------------------------------------------------------------------------
		--  COBRANCA
		----------------------------------------------------------------------------------------------------------------
    
  procedure cobranca(p_id               number,
					  p_acao             number,
					  p_executor_id      number,
					  p_executor_nome    varchar2,
					  p_executor_login   varchar2,
					  p_executor_tipo_id number,
					  p_executor_tid     varchar2) is
		v_sucesso boolean := false;
	begin
	
		----------------------------------------------------------------------------------------------------------------
	
		----------------------------------------------------------------------------------------------------------------
		--  COBRANCA
		----------------------------------------------------------------------------------------------------------------
		for i in (select id,
                      fiscalizacao,
                      autuado,
                      codigoreceita,
                      serie,
                      iuf_numero,
                      iuf_data,
                      protoc_num,
                      autos,
                      not_iuf_data,
                      not_jiapi_data,
                      not_core_data,
                      valor_multa,
                      qtdparcelas,
                      vencimento_data,
                      dataemissao,
											tid
					from tab_fisc_cobranca t
				   where t.id = p_id) loop
			-- Inserindo na histórico
			insert into hst_fisc_cobranca a
				(id,
         cobranca_id,
         fiscalizacao,
         autuado_id,
         autuado_nome,
         codigoreceita_id,
         codigoreceita_texto,
         serie_id,
         serie_texto,
         iuf_numero,
         iuf_data,
         protoc_num,
         autos,
         not_iuf_data,
         not_jiapi_data,
         not_core_data,
         valor_multa,
         qtdparcelas,
         vencimento_data,
         dataemissao,
         tid,
				 executor_id,
				 executor_tid,
				 executor_nome,
				 executor_login,
				 executor_tipo_id,
				 executor_tipo_texto,
				 acao_executada,
				 data_execucao)
			values
				(seq_hst_fisc_cobranca.nextval,
         i.id,
         i.fiscalizacao,
         i.autuado,
         (select p.nome
					from tab_pessoa p
				   where p.id = i.autuado),
         i.codigoreceita,
         (select lfc.texto
					from lov_fisc_infracao_codigo_rece lfc
				   where lfc.id = i.codigoreceita),
         i.serie,
         (select lfs.texto
					from lov_fiscalizacao_serie lfs
				   where lfs.id = i.serie),
         i.iuf_numero,
         i.iuf_data,
         i.protoc_num,
         i.autos,
         i.not_iuf_data,
         i.not_jiapi_data,
         i.not_core_data,
         i.valor_multa,
         i.qtdparcelas,
         i.vencimento_data,
         i.dataemissao,
         i.tid,
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
					 and la.artefato = 3),
				 systimestamp);
		
		end loop;
		----------------------------------------------------------------------------------------------------------------
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Historico de Cobranca.');
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Historico de Cobranca. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		
	end;