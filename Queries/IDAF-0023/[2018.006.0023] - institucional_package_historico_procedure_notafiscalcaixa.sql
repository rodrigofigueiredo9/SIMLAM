--ATENÇÃO!!!!
--Lembrar de inserir a procedure no cabeçalho e no body.

	---------------------------------------------------------
	-- Cadastro de Nota Fiscal de Caixa
	---------------------------------------------------------
	procedure notafiscalcaixa(	p_id               number,
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
						 p.numero,
						 p.tipo_caixa tipo_caixa_id,
						 ltc.texto tipo_caixa_texto,
						 p.saldo_inicial,
             p.tipo_pessoa,
             p.cpf_cnpj_associado,
             p.saldo_retificado
				  from tab_nf_caixa p,
					   lov_tipo_caixa ltc
				  where p.id = p_id
						and ltc.id = p.tipo_caixa) loop
			
			-- insert historico PTV
			insert into hst_nf_caixa
				(id,
				 tid,
				 nf_caixa_id,
				 numero,
				 tipo_caixa_id,
				 tipo_caixa_texto,         
				 saldo_inicial,
         tipo_pessoa,
         cpf_cnpj_associado,
         saldo_retificado,
				 executor_id,
				 executor_tid,
				 executor_nome,
				 executor_login,
				 executor_tipo_id,
				 executor_tipo_texto,
				 acao_executada,
				 data_execucao
				)
			values
				(seq_hst_nf_caixa.nextval,
				 i.tid,
				 i.id,
				 i.numero,
				 i.tipo_caixa_id,
				 i.tipo_caixa_texto,
				 i.saldo_inicial,
         i.tipo_pessoa,
         i.cpf_cnpj_associado,
         i.saldo_retificado,
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
					 and la.artefato = (select id from Lov_Historico_Artefato where texto = 'notafiscalcaixa')),
				 systimestamp)
			returning id into v_id;
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