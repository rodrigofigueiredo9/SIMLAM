---------------------------------------------------------
	-- Configuração de CFO/CFOC/PTV
	---------------------------------------------------------
	procedure configdocumentofitossanitario(p_id               number,
											p_acao             number,
											p_executor_id      number,
											p_executor_nome    varchar2,
											p_executor_login   varchar2,
											p_executor_tipo_id number,
											p_executor_tid     varchar2) is
		v_sucesso boolean := false;
		v_id      number := 0;
	begin
		for i in (select t.id, t.tid
					from cnf_doc_fitossanitario t
				   where t.id = p_id) loop
		
			insert into hst_cnf_doc_fitossanitario
				(id,
				 tid,
				 configuracao_id,
				 executor_id,
				 executor_tid,
				 executor_nome,
				 executor_login,
				 executor_tipo_id,
				 executor_tipo_texto,
				 acao_executada,
				 data_execucao)
			values
				(seq_hst_cnf_doc_fitossanitario.nextval,
				 i.tid,
				 i.id,
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
					 and la.artefato = 132),
				 systimestamp)
			returning id into v_id;
		
			insert into hst_cnf_doc_fito_intervalo
				(id,
				 tid,
				 id_hst,
				 configuracao_id,
				 intervalo_id,
				 tipo_documento_id,
				 tipo_documento_texto,
				 tipo,
				 numero_inicial,
				 numero_final,
         serie)
				(select seq_hst_cnf_doc_fito_intervalo.nextval,
						c.tid,
						v_id,
						c.configuracao,
						c.id,
						c.tipo_documento,
						l.texto,
						c.tipo,
						c.numero_inicial,
						c.numero_final,
            c.serie
				   from cnf_doc_fito_intervalo       c,
						lov_doc_fitossanitarios_tipo l
				  where l.id = c.tipo_documento
					and c.configuracao = p_id);
		
		end loop;
	
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Configuração de CFO/CFOC/PTV. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
	
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Praga. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
	---------------------------------------------------------