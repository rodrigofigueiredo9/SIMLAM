  	----------------------------------------------------------------------------------------------------------------
		--  VRTE
		----------------------------------------------------------------------------------------------------------------
    
  procedure vrte(p_id               number,
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
		--  VRTE
		----------------------------------------------------------------------------------------------------------------
		for i in (select id,
											ano,
                      vrte,
											tid
					from tab_fisc_vrte t
				   where t.id = p_id) loop
			-- Inserindo na histórico
			insert into hst_fisc_vrte a
				(id,
         vrte_id,
         ano,
         vrte,
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
				(seq_hst_arquivo.nextval,
         i.id,
         i.ano,
         i.vrte,
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
									'Erro ao gerar o Historico de Parametrizacao.');
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Historico de Parametrizacao. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		
	end;