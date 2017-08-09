--ATEN��O!!! QUANDO FOR COPIAR ISSO PARA O PACKAGE HISTORICO BODY, O CABE�ALHO TAMB�M TEM QUE SER COPIADO PARA O PACKAGE HISTORICO!

---------------------------------------------------------
	-- Configuracao Fiscalizacao - Destina��o
---------------------------------------------------------
procedure fiscdestinacao(p_id               number,
							p_acao             number,
							p_executor_id      number,
							p_executor_nome    varchar2,
							p_executor_login   varchar2,
							p_executor_tipo_id number,
							p_executor_tid     varchar2) is
		v_sucesso boolean := false;
	begin
		----------------------------------------------------------------------------------------------------------------
		-- Destina��o
		----------------------------------------------------------------------------------------------------------------
		for i in (select t.id,
						 t.destino,
						 t.ativo,
						 t.tid
					from cnf_fisc_infr_destinacao t
				   where t.id = p_id) loop
			-- Inserindo na hist�rico
			insert into hst_cnf_fisc_infr_destinacao
				(id,
				 destinacao_id,
				 destino,
				 ativo,
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
				(seq_hst_cnf_fisc_infr_destin.nextval,
				 i.id,
				 i.destino,
				 i.ativo,
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
					from lov_historico_artefatos_acoes la,
                lov_historico_artefato art
				  where la.acao = p_acao
                and la.artefato = art.id
                and art.texto like 'fiscdestinacao'),
				 systimestamp);
		end loop;
		----------------------------------------------------------------------------------------------------------------
	
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Hist�rico de Configura��o Fiscalizacao - Destina��o. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
		--Tratamento de exce��o
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Hist�rico de Configura��o Fiscalizacao - Destina��o. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
---------------------------------------------------------   