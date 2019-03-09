		----------------------------------------------------------------------------------------------------------------
		-- Título
		----------------------------------------------------------------------------------------------------------------
		for i in (select t.id,
						 t.tid,
						 t.autor,
						 (case
							 when t.credenciado is not null then
							  (select c.tid
								 from tab_credenciado c
								where c.id = t.autor)
							 else
							  (select c.tid
								 from tab_funcionario c
								where c.id = t.autor)
						 end) autor_tid,
						 t.setor,
						 s.tid setor_tid,
						 t.modelo,
						 m.tid modelo_tid,
						 t.situacao,
						 ls.texto situacao_texto,
						 t.situacao_motivo,
						 lsm.texto situacao_motivo_texto,
						 t.local_emissao,
						 le.texto local_emissao_texto,
						 t.protocolo,
						 p.tid protocolo_tid,
						 t.empreendimento,
						 (case
							 when p_executor_tipo_id = 2 then
							  (select c.tid
								 from cre_empreendimento c
								where c.id = t.empreendimento)
							 else
							  (select c.tid
								 from tab_empreendimento c
								where c.id = t.empreendimento)
						 end) empreendimento_tid,
						 t.prazo,
						 t.prazo_unidade,
						 t.dias_prorrogados,
						 t.data_criacao,
						 t.data_inicio,
						 t.data_emissao,
						 t.data_assinatura,
						 t.data_vencimento,
						 t.data_encerramento,
						 t.email,
						 t.arquivo,
						 ar.tid arquivo_tid,
						 t.requerimento,
						 (case
							 when p_executor_tipo_id = 2 then
							  (select c.tid
								 from cre_requerimento c
								where c.id = t.requerimento)
							 else
							  (select c.tid
								 from tab_requerimento c
								where c.id = t.requerimento)
						 end) requerimento_tid,
						 t.credenciado,
						 tc.tid credenciado_tid,
						 t.motivo_suspensao,
						 t.data_situacao,
						 (SELECT tu.LOGON_IP
						  FROM tab_usuario tu
						  WHERE tu.LOGIN = p_executor_login) ip_inst,
						 (SELECT tuc.LOGON_IP
						  FROM idafcredenciado.tab_usuario tuc
						  WHERE tuc.LOGIN = p_executor_login) ip_cred
					from tab_titulo                 t,
						 tab_setor                  s,
						 tab_titulo_modelo          m,
						 lov_titulo_situacao        ls,
						 lov_titulo_situacao_motivo lsm,
						 lov_municipio              le,
						 tab_protocolo              p,
						 tab_arquivo                ar,
						 tab_credenciado            tc
				   where t.setor = s.id(+)
					 and t.modelo = m.id
					 and t.situacao = ls.id
					 and t.situacao_motivo = lsm.id(+)
					 and t.local_emissao = le.id
					 and t.protocolo = p.id(+)
					 and t.arquivo = ar.id(+)
					 and t.credenciado = tc.id(+)
					 and t.id = p_id) loop
		
			-- Inserindo na histórico
			insert into hst_titulo m
				(id,
				 titulo_id,
				 tid,
				 autor_id,
				 autor_tid,
				 setor_id,
				 setor_tid,
				 modelo_id,
				 modelo_tid,
				 situacao_id,
				 situacao_texto,
				 situacao_motivo_id,
				 situacao_motivo_texto,
				 local_emissao_id,
				 local_emissao_texto,
				 protocolo_id,
				 protocolo_tid,
				 empreendimento_id,
				 empreendimento_tid,
				 prazo,
				 prazo_unidade,
				 dias_prorrogados,
				 data_criacao,
				 data_inicio,
				 data_emissao,
				 data_assinatura,
				 data_vencimento,
				 data_encerramento,
				 email,
				 arquivo_id,
				 arquivo_tid,
				 executor_id,
				 executor_tid,
				 executor_nome,
				 executor_login,
				 executor_tipo_id,
				 executor_tipo_texto,
				 acao_executada,
				 data_execucao,
				 requerimento_id,
				 requerimento_tid,
				 credenciado_id,
				 credenciado_tid,
				 motivo_suspensao,
				 data_situacao,
				 executor_ip)
			values
				(seq_hst_titulo.nextval,
				 i.id,
				 i.tid,
				 i.autor,
				 i.autor_tid,
				 i.setor,
				 i.setor_tid,
				 i.modelo,
				 i.modelo_tid,
				 i.situacao,
				 i.situacao_texto,
				 i.situacao_motivo,
				 i.situacao_motivo_texto,
				 i.local_emissao,
				 i.local_emissao_texto,
				 i.protocolo,
				 i.protocolo_tid,
				 i.empreendimento,
				 i.empreendimento_tid,
				 i.prazo,
				 i.prazo_unidade,
				 i.dias_prorrogados,
				 i.data_criacao,
				 i.data_inicio,
				 i.data_emissao,
				 i.data_assinatura,
				 i.data_vencimento,
				 i.data_encerramento,
				 i.email,
				 i.arquivo,
				 i.arquivo_tid,
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
					 and la.artefato = 18),
				 systimestamp,
				 i.requerimento,
				 i.requerimento_tid,
				 i.credenciado,
				 i.credenciado_tid,
				 i.motivo_suspensao,
				 i.data_situacao,
				 nvl(i.ip_inst, i.ip_cred))
			returning m.id into v_id;