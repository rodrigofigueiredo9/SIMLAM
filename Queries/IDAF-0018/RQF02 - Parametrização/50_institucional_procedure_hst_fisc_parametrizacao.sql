procedure parametrizacao(p_id               number,
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
		--  Parametrizacao do sistema
		----------------------------------------------------------------------------------------------------------------
		for i in (select id,
											codigoreceita,
											iniciovigencia,
											fimvigencia,
											vrtre,
											multa_perc,
											juros_perc,
											juros_decor,
											desconto_perc,
											desconto_und,
											desconto_decor,
											tid
					from tab_fisc_parametrizacao t
				   where t.id = p_id) loop
			-- Inserindo na histórico
			insert into hst_fisc_parametrizacao a
				(id,
         parametrizacao_id,
         codigoreceita_id,
         codigoreceita_texto,
         iniciovigencia,
         fimvigencia,
         vrtre,
         multa_perc,
         juros_perc,
         juros_decor,
         desconto_perc,
         desconto_und,
         desconto_decor,
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
         i.codigoreceita,
         i.iniciovigencia,
         i.fimvigencia,
         i.vrtre,
         i.multa_perc,
         i.juros_perc,
         i.juros_decor,
         i.desconto_perc,
         i.desconto_und,
         i.desconto_decor,
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