--ATEN��O!!! QUANDO FOR COPIAR ISSO PARA O PACKAGE HISTORICO BODY, O CABE�ALHO TAMB�M TEM QUE SER COPIADO PARA O PACKAGE HISTORICO!

---------------------------------------------------------
	-- Configuracao Fiscalizacao - C�digo da Receita
---------------------------------------------------------
procedure codigoreceita(p_id     number,
							p_acao             number,
							p_executor_id      number,
							p_executor_nome    varchar2,
							p_executor_login   varchar2,
							p_executor_tipo_id number,
							p_executor_tid     varchar2) is
		v_sucesso boolean := false;
	begin
		----------------------------------------------------------------------------------------------------------------
		-- C�digo da Receita
		----------------------------------------------------------------------------------------------------------------
		for i in (select t.id,
						 t.texto,
						 t.descricao,
						 t.ativo,
						 t.tid
					from lov_fisc_infracao_codigo_rece t
				   where t.id = p_id) loop
			-- Inserindo na hist�rico
			insert into hst_lov_fisc_infr_cod_rece
				(id,
				 codigo_id,
				 texto,
				 descricao,
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
				(seq_hst_lov_fisc_infr_cod_rece.nextval,
				 i.id,
				 i.texto,
				 i.descricao,
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
                and art.texto like 'codigoreceita'),
				 systimestamp);
		end loop;
		----------------------------------------------------------------------------------------------------------------
	
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Hist�rico de Configura��o Fiscalizacao - C�digo da Receita. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
		--Tratamento de exce��o
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Hist�rico de Configura��o Fiscalizacao - C�digo da Receita. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
---------------------------------------------------------   