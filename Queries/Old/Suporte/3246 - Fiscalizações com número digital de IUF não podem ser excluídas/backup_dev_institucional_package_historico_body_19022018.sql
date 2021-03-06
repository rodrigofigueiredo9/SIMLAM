create or replace PACKAGE BODY        "HISTORICO" is
	---------------------------------------------------------
	-- Funcionário
	---------------------------------------------------------
	procedure funcionario(p_id               number,
						  p_acao             number,
						  p_executor_id      number,
						  p_executor_nome    varchar2,
						  p_executor_login   varchar2,
						  p_executor_tipo_id number,
						  p_executor_tid     varchar2) is
		v_sucesso boolean := false;
		v_id      number;
	begin
	
		----------------------------------------------------------------------------------------------------------------
	
		----------------------------------------------------------------------------------------------------------------
		-- Funcionário
		----------------------------------------------------------------------------------------------------------------
		for i in (select f.id,
						 f.usuario    usuario_id,
						 f.nome,
						 f.arquivo,
						 aa.tid       arquivo_tid,
						 f.cpf,
						 f.situacao   situacao_id,
						 ls.texto     situacao_texto,
						 f.email,
						 f.tipo       tipo_id,
						 lt.texto     tipo_texto,
						 f.tid,
						 u.tid        usuario_tid,
						 f.tentativa,
						 f.logado,
						 f.session_id
					from tab_funcionario          f,
						 tab_usuario              u,
						 lov_funcionario_situacao ls,
						 tab_arquivo              aa,
						 lov_funcionario_tipo     lt
				   where f.usuario = u.id
					 and f.situacao = ls.id
					 and f.tipo = lt.id
					 and f.arquivo = aa.id(+)
					 and f.id = p_id) loop
		
			-- Inserindo na histórico
			insert into hst_funcionario f
				(id,
				 funcionario_id,
				 tid,
				 usuario_id,
				 usuario_tid,
				 nome,
				 arquivo_id,
				 arquivo_tid,
				 cpf,
				 situacao_id,
				 situacao_texto,
				 email,
				 tipo_id,
				 tipo_texto,
				 tentativa,
				 logado,
				 session_id,
				 executor_id,
				 executor_tid,
				 executor_nome,
				 executor_login,
				 executor_tipo_id,
				 executor_tipo_texto,
				 acao_executada,
				 data_execucao)
			values
				(seq_hst_funcionario.nextval,
				 i.id,
				 i.tid,
				 i.usuario_id,
				 i.usuario_tid,
				 i.nome,
				 i.arquivo,
				 i.arquivo_tid,
				 i.cpf,
				 i.situacao_id,
				 i.situacao_texto,
				 i.email,
				 i.tipo_id,
				 i.tipo_texto,
				 i.tentativa,
				 i.logado,
				 i.session_id,
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
					 and la.artefato = 1),
				 systimestamp)
			returning f.id into v_id;
		
			----------------------------------------------------------------------------------------------------------------
			-- Funcionário/Cargos
			----------------------------------------------------------------------------------------------------------------
			for j in (select cargo cargo_id, tc.tid cargo_tid, c.tid
						from tab_funcionario_cargo c, tab_cargo tc
					   where tc.id = c.cargo
						 and c.funcionario = i.id) loop
				insert into hst_funcionario_cargo
					(id, id_hst, cargo_id, cargo_tid, tid)
				values
					(seq_hst_funcionario_cargo.nextval,
					 v_id,
					 j.cargo_id,
					 j.cargo_tid,
					 i.tid);
			end loop;
		
			----------------------------------------------------------------------------------------------------------------
			-- Funcionário/Papeis
			----------------------------------------------------------------------------------------------------------------
			for j in (select c.papel papel_id, tc.tid papel_tid, c.tid
						from tab_funcionario_papel  c,
							 tab_autenticacao_papel tc
					   where tc.id = c.papel
						 and c.funcionario = i.id) loop
				insert into hst_funcionario_papel
					(id, id_hst, papel_id, papel_tid, tid)
				values
					(seq_hst_funcionario_papel.nextval,
					 v_id,
					 j.papel_id,
					 j.papel_tid,
					 i.tid);
			end loop;
		
			----------------------------------------------------------------------------------------------------------------
			-- Funcionário/Permissões
			----------------------------------------------------------------------------------------------------------------
			for j in (select c.permissao permissao_id,
							 tc.tid      permissao_tid,
							 c.tid
						from tab_funcionario_permissao   c,
							 tab_autenticacao_papel_perm tc
					   where tc.id = c.permissao
						 and c.funcionario = i.id) loop
				insert into hst_funcionario_permissao
					(id, id_hst, permissao_id, permissao_tid, tid)
				values
					(seq_hst_funcionario_permissao.nextval,
					 v_id,
					 j.permissao_id,
					 j.permissao_tid,
					 i.tid);
			end loop;
		
			----------------------------------------------------------------------------------------------------------------
			-- Funcionário/Setores
			----------------------------------------------------------------------------------------------------------------
			for j in (select c.setor setor_id, tc.tid setor_tid, c.tid
						from tab_funcionario_setor c, tab_setor tc
					   where tc.id = c.setor
						 and c.funcionario = i.id) loop
				insert into hst_funcionario_setor
					(id, id_hst, setor_id, setor_tid, tid)
				values
					(seq_hst_funcionario_setor.nextval,
					 v_id,
					 j.setor_id,
					 j.setor_tid,
					 i.tid);
			end loop;
		
		end loop;
		----------------------------------------------------------------------------------------------------------------
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Funcionário. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Funcionário. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		
	end;
	---------------------------------------------------------

	---------------------------------------------------------
	-- Atividade de Protocolo/Requerimento
	---------------------------------------------------------
	procedure atividade(p_id               number,
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
		-- Atividade de Protocolo/Requerimento
		----------------------------------------------------------------------------------------------------------------
		for i in (select a.id,
						 a.tid,
						 a.setor,
						 s.tid                        setor_tid,
						 a.agrupador,
						 aa.tid                       agrupador_tid,
						 a.secao,
						 a.cod_cnae,
						 a.atividade,
						 a.situacao,
						 a.conclusao,
						 a.cod_categoria,
						 a.codigo,
						 a.exibir_credenciado,
						 a.empreendimento_obrigatorio
					from tab_atividade           a,
						 tab_setor               s,
						 tab_atividade_agrupador aa
				   where a.setor = s.id
					 and a.agrupador = aa.id
					 and a.id = p_id) loop
		
			-- Inserindo na histórico
			insert into hst_atividade a
				(id,
				 atividade_id,
				 tid,
				 setor_id,
				 setor_tid,
				 agrupador_id,
				 agrupador_tid,
				 secao,
				 cod_cnae,
				 atividade,
				 situacao,
				 conclusao,
				 cod_categoria,
				 codigo,
				 exibir_credenciado,
				 empreendimento_obrigatorio,
				 executor_id,
				 executor_tid,
				 executor_nome,
				 executor_login,
				 executor_tipo_id,
				 executor_tipo_texto,
				 acao_executada,
				 data_execucao)
			values
				(seq_hst_atividade.nextval,
				 i.id,
				 i.tid,
				 i.setor,
				 i.setor_tid,
				 i.agrupador,
				 i.agrupador_tid,
				 i.secao,
				 i.cod_cnae,
				 i.atividade,
				 i.situacao,
				 i.conclusao,
				 i.cod_categoria,
				 i.codigo,
				 i.exibir_credenciado,
				 i.empreendimento_obrigatorio,
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
					 and la.artefato = 24),
				 systimestamp);
		end loop;
		----------------------------------------------------------------------------------------------------------------
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Atividade.');
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Atividade. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		
	end;
	---------------------------------------------------------

	---------------------------------------------------------
	-- Arquivos do sistema
	---------------------------------------------------------
	procedure arquivo(p_id               number,
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
		--  Arquivos do sistema
		----------------------------------------------------------------------------------------------------------------
		for i in (select id,
						 nome,
						 caminho,
						 diretorio,
						 extensao,
						 tipo,
						 tamanho,
						 raiz,
						 tid
					from tab_arquivo ta
				   where ta.id = p_id) loop
			-- Inserindo na histórico
			insert into hst_arquivo a
				(id,
				 arquivo_id,
				 nome,
				 caminho,
				 diretorio,
				 extensao,
				 tipo,
				 tamanho,
				 raiz,
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
				 i.nome,
				 i.caminho,
				 i.diretorio,
				 i.extensao,
				 i.tipo,
				 i.tamanho,
				 i.raiz,
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
									'Erro ao gerar o Histórico de Arquivo.');
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Arquivo. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		
	end;
	---------------------------------------------------------

	---------------------------------------------------------
	-- Pessoa
	---------------------------------------------------------
	procedure pessoa(p_id               number,
					 p_acao             number,
					 p_executor_id      number,
					 p_executor_nome    varchar2,
					 p_executor_login   varchar2,
					 p_executor_tipo_id number,
					 p_executor_tid     varchar2) is
		v_sucesso boolean := false;
		v_id      number;
	begin
	
		----------------------------------------------------------------------------------------------------------------
	
		----------------------------------------------------------------------------------------------------------------
		-- Pessoa
		----------------------------------------------------------------------------------------------------------------
		for i in (select p.id,
						 tipo,
						 nome,
						 apelido,
						 cpf,
						 rg,
						 estado_civil    estado_civil_id,
						 le.texto        estado_civil_texto,
						 sexo,
						 nacionalidade,
						 naturalidade,
						 data_nascimento,
						 mae,
						 pai,
						 cnpj,
						 razao_social,
						 nome_fantasia,
						 ie,
						 ativa,
						 tid
					from tab_pessoa p, lov_pessoa_estado_civil le
				   where p.estado_civil = le.id(+)
					 and p.id = p_id) loop
		
			-- Inserindo na histórico
			insert into hst_pessoa p
				(id,
				 pessoa_id,
				 tid,
				 tipo,
				 nome,
				 apelido,
				 cpf,
				 rg,
				 estado_civil_id,
				 estado_civil_texto,
				 sexo,
				 nacionalidade,
				 naturalidade,
				 data_nascimento,
				 mae,
				 pai,
				 cnpj,
				 razao_social,
				 nome_fantasia,
				 ie,
				 ativa,
				 executor_id,
				 executor_tid,
				 executor_nome,
				 executor_login,
				 executor_tipo_id,
				 executor_tipo_texto,
				 acao_executada,
				 data_execucao)
			values
				(seq_hst_pessoa.nextval,
				 i.id,
				 i.tid,
				 i.tipo,
				 i.nome,
				 i.apelido,
				 i.cpf,
				 i.rg,
				 i.estado_civil_id,
				 i.estado_civil_texto,
				 i.sexo,
				 i.nacionalidade,
				 i.naturalidade,
				 i.data_nascimento,
				 i.mae,
				 i.pai,
				 i.cnpj,
				 i.razao_social,
				 i.nome_fantasia,
				 i.ie,
				 i.ativa,
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
					 and la.artefato = 4),
				 systimestamp)
			returning p.id into v_id;
		
			----------------------------------------------------------------------------------------------------------------
			-- Pessoa/Meio de Contato
			----------------------------------------------------------------------------------------------------------------
			for j in (select m.id,
							 pessoa,
							 tm.id  meio_contato_id,
							 tm.tid meio_contato_tid,
							 valor,
							 m.tid
						from tab_pessoa_meio_contato m, tab_meio_contato tm
					   where tm.id = m.meio_contato
						 and m.pessoa = i.id) loop
				insert into hst_pessoa_meio_contato
					(id,
					 id_hst,
					 pes_meio_cont_id,
					 meio_contato_id,
					 meio_contato_tid,
					 valor,
					 tid)
				values
					(seq_hst_pessoa_meio_contato.nextval,
					 v_id,
					 j.id,
					 j.meio_contato_id,
					 j.meio_contato_tid,
					 j.valor,
					 i.tid);
			end loop;
		
			----------------------------------------------------------------------------------------------------------------
			-- Pessoa/Cônjuge
			----------------------------------------------------------------------------------------------------------------
			for j in (select tr.id,
							 pessoa,
							 pp.id  conjuge_id,
							 pp.tid conjuge_tid,
							 tr.tid
						from tab_pessoa_conjuge tr, tab_pessoa pp
					   where pp.id = tr.conjuge
						 and tr.pessoa = i.id) loop
				insert into hst_pessoa_conjuge
					(id,
					 id_hst,
					 pessoa_conj_id,
					 pessoa_id,
					 pessoa_tid,
					 conjuge_id,
					 conjuge_tid,
					 tid)
				values
					(seq_hst_pessoa_conjuge.nextval,
					 v_id,
					 j.id,
					 i.id,
					 i.tid,
					 j.conjuge_id,
					 j.conjuge_tid,
					 i.tid);
			end loop;
		
			----------------------------------------------------------------------------------------------------------------
			-- Pessoa/Representantes
			----------------------------------------------------------------------------------------------------------------
			for j in (select tr.id,
							 pessoa,
							 pp.id  representante_id,
							 pp.tid representante_tid,
							 tr.tid
						from tab_pessoa_representante tr, tab_pessoa pp
					   where pp.id = tr.representante
						 and tr.pessoa = i.id) loop
				insert into hst_pessoa_representante
					(id,
					 id_hst,
					 pessoa_rep_id,
					 pessoa_id,
					 pessoa_tid,
					 representante_id,
					 representante_tid,
					 tid)
				values
					(seq_hst_pessoa_representante.nextval,
					 v_id,
					 j.id,
					 i.id,
					 i.tid,
					 j.representante_id,
					 j.representante_tid,
					 i.tid);
			end loop;
		
			----------------------------------------------------------------------------------------------------------------
			-- Pessoa/Profissão
			----------------------------------------------------------------------------------------------------------------
			for j in (select tpp.id,
							 tp.id    profissao_id,
							 tp.tid   profissao_tid,
							 toc.id   orgao_classe_id,
							 toc.tid  orgao_classe_tid,
							 registro,
							 tpp.tid
						from tab_pessoa_profissao tpp,
							 tab_profissao        tp,
							 tab_orgao_classe     toc
					   where tpp.profissao = tp.id
						 and tpp.orgao_classe = toc.id(+)
						 and tpp.pessoa = i.id) loop
				insert into hst_pessoa_profissao
					(id,
					 id_hst,
					 pes_profissao_id,
					 profissao_id,
					 profissao_tid,
					 orgao_classe_id,
					 orgao_classe_tid,
					 registro,
					 tid)
				values
					(seq_hst_pessoa_profissao.nextval,
					 v_id,
					 j.id,
					 j.profissao_id,
					 j.profissao_tid,
					 j.orgao_classe_id,
					 j.orgao_classe_tid,
					 j.registro,
					 i.tid);
			end loop;
		
			----------------------------------------------------------------------------------------------------------------
			-- Pessoa/Endereço
			----------------------------------------------------------------------------------------------------------------
			for j in (select te.id,
							 pessoa,
							 te.cep,
							 te.distrito,
							 logradouro,
							 bairro,
							 le.id       estado_id,
							 le.texto    estado_texto,
							 lm.id       municipio_id,
							 lm.texto    municipio_texto,
							 numero,
							 complemento,
							 te.tid
						from tab_pessoa_endereco te,
							 lov_estado          le,
							 lov_municipio       lm
					   where te.estado = le.id
						 and te.municipio = lm.id
						 and te.pessoa = p_id) loop
				insert into hst_pessoa_endereco
					(id,
					 id_hst,
					 endereco_id,
					 cep,
					 distrito,
					 logradouro,
					 bairro,
					 estado_id,
					 estado_texto,
					 municipio_id,
					 municipio_texto,
					 numero,
					 complemento,
					 tid)
				values
					(seq_hst_pessoa_endereco.nextval,
					 v_id,
					 j.id,
					 j.cep,
					 j.distrito,
					 j.logradouro,
					 j.bairro,
					 j.estado_id,
					 j.estado_texto,
					 j.municipio_id,
					 j.municipio_texto,
					 j.numero,
					 j.complemento,
					 i.tid);
			end loop;
		
		end loop;
		----------------------------------------------------------------------------------------------------------------
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Pessoa. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Pessoa. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		
	end;
	---------------------------------------------------------

	---------------------------------------------------------
	-- Roteiro
	---------------------------------------------------------
	procedure roteiro(p_id               number,
					  p_acao             number,
					  p_executor_id      number,
					  p_executor_nome    varchar2,
					  p_executor_login   varchar2,
					  p_executor_tipo_id number,
					  p_executor_tid     varchar2) is
		v_sucesso boolean := false;
		v_id      number;
	begin
	
		----------------------------------------------------------------------------------------------------------------
	
		----------------------------------------------------------------------------------------------------------------
		-- roteiro
		----------------------------------------------------------------------------------------------------------------
		for i in (select r.id,
						 r.numero,
						 r.versao,
						 r.nome,
						 s.id           setor_id,
						 s.tid          setor_tid,
						 r.situacao     situacao_id,
						 rs.texto       situacao_texto,
						 r.observacoes,
						 r.data_criacao,
						 r.finalidade,
						 r.tid
					from tab_roteiro r, tab_setor s, lov_roteiro_situacao rs
				   where r.setor = s.id
					 and r.situacao = rs.id
					 and r.id = p_id) loop
		
			-- Inserindo na histórico
			insert into hst_roteiro p
				(id,
				 roteiro_id,
				 tid,
				 numero,
				 versao,
				 nome,
				 setor_id,
				 setor_tid,
				 situacao_id,
				 situacao_texto,
				 observacoes,
				 data_criacao,
				 finalidade,
				 executor_id,
				 executor_tid,
				 executor_nome,
				 executor_login,
				 executor_tipo_id,
				 executor_tipo_texto,
				 acao_executada,
				 data_execucao)
			values
				(seq_hst_roteiro.nextval,
				 i.id,
				 i.tid,
				 i.numero,
				 i.versao,
				 i.nome,
				 i.setor_id,
				 i.setor_tid,
				 i.situacao_id,
				 i.situacao_texto,
				 i.observacoes,
				 i.data_criacao,
				 i.finalidade,
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
					 and la.artefato = 5),
				 systimestamp)
			returning p.id into v_id;
		
			----------------------------------------------------------------------------------------------------------------
			-- Roteiro/Itens
			----------------------------------------------------------------------------------------------------------------
			for j in (select te.id, it.id item_id, it.tid item_tid, te.ordem
						from tab_roteiro_itens te, tab_roteiro_item it
					   where it.id = te.item
						 and te.roteiro = i.id) loop
				insert into hst_roteiro_itens
					(id,
					 id_hst,
					 roteiro_itens_id,
					 roteiro_id,
					 roteiro_tid,
					 item_id,
					 item_tid,
					 ordem,
					 tid)
				values
					(seq_hst_roteiro_itens.nextval,
					 v_id,
					 j.id,
					 i.id,
					 i.tid,
					 j.item_id,
					 j.item_tid,
					 j.ordem,
					 i.tid);
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
						from tab_roteiro_arquivo tr, tab_arquivo ta
					   where tr.arquivo = ta.id
						 and tr.roteiro = i.id) loop
				insert into hst_roteiro_arquivo
					(id,
					 id_hst,
					 roteiro_arq_id,
					 roteiro_id,
					 roteiro_tid,
					 arquivo_id,
					 arquivo_tid,
					 ordem,
					 descricao,
					 tid)
				values
					(seq_hst_roteiro_arquivo.nextval,
					 v_id,
					 j.id,
					 i.id,
					 i.tid,
					 j.arquivo_id,
					 j.arquivo_tid,
					 j.ordem,
					 j.descricao,
					 i.tid);
			end loop;
		
			----------------------------------------------------------------------------------------------------------------
			-- Palavra Chave/Itens
			----------------------------------------------------------------------------------------------------------------
			for j in (select id, roteiro, chave, tid
						from tab_roteiro_chave tr
					   where tr.roteiro = i.id) loop
				insert into hst_roteiro_chave
					(id,
					 id_hst,
					 roteiro_cha_id,
					 chave_id,
					 roteiro_id,
					 roteiro_tid,
					 chave,
					 tid)
				values
					(seq_hst_roteiro_chave.nextval,
					 v_id,
					 j.id,
					 j.id,
					 i.id,
					 i.tid,
					 j.chave,
					 i.tid);
			end loop;
		
			----------------------------------------------------------------------------------------------------------------
			-- Modelos
			----------------------------------------------------------------------------------------------------------------
			--
			insert into hst_roteiro_modelos e
				(id,
				 id_hst,
				 roteiro_mod_id,
				 roteiro_id,
				 roteiro_tid,
				 modelo_id,
				 modelo_tid,
				 tid)
				(select seq_hst_roteiro_modelos.nextval,
						v_id,
						a.id,
						i.id,
						i.tid,
						t.id,
						t.tid,
						i.tid
				   from tab_roteiro_modelos a, tab_titulo_modelo t
				  where a.modelo = t.id
					and a.roteiro = i.id);
		
			----------------------------------------------------------------------------------------------------------------
			-- Atividades
			----------------------------------------------------------------------------------------------------------------
			--
			insert into hst_roteiro_atividades e
				(id,
				 id_hst,
				 roteiro_ativ_id,
				 roteiro_id,
				 roteiro_tid,
				 atividade_id,
				 atividade_tid,
				 tid)
				(select seq_hst_roteiro_atividades.nextval,
						v_id,
						a.id,
						i.id,
						i.tid,
						aa.id,
						aa.tid,
						i.tid
				   from tab_roteiro_atividades a, tab_atividade aa
				  where a.atividade = aa.id
					and a.roteiro = i.id);
		
		end loop;
		----------------------------------------------------------------------------------------------------------------
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Roteiro. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Roteiro. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		
	end;
	---------------------------------------------------------

	---------------------------------------------------------
	-- Checagem de item de roteiro
	---------------------------------------------------------
	procedure checagem(p_id               number,
					   p_acao             number,
					   p_executor_id      number,
					   p_executor_nome    varchar2,
					   p_executor_login   varchar2,
					   p_executor_tipo_id number,
					   p_executor_tid     varchar2) is
		v_sucesso boolean := false;
		v_id      number;
	begin
	
		----------------------------------------------------------------------------------------------------------------
	
		----------------------------------------------------------------------------------------------------------------
		-- Checagem
		----------------------------------------------------------------------------------------------------------------
		for i in (select c.id,
						 c.interessado,
						 c.situacao    situacao_id,
						 s.texto       situacao_texto,
						 c.tid
					from tab_checagem c, lov_checagem_situacao s
				   where c.situacao = s.id
					 and c.id = p_id) loop
		
			-- Inserindo na histórico
			insert into hst_checagem p
				(id,
				 checagem_id,
				 tid,
				 interessado,
				 situacao_id,
				 situacao_texto,
				 executor_id,
				 executor_tid,
				 executor_nome,
				 executor_login,
				 executor_tipo_id,
				 executor_tipo_texto,
				 acao_executada,
				 data_execucao)
			values
				(seq_hst_checagem.nextval,
				 i.id,
				 i.tid,
				 i.interessado,
				 i.situacao_id,
				 i.situacao_texto,
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
					 and la.artefato = 7),
				 systimestamp)
			returning p.id into v_id;
		
			----------------------------------------------------------------------------------------------------------------
			-- Checagem/Roteiros
			----------------------------------------------------------------------------------------------------------------
			for j in (select tc.id,
							 tc.roteiro roteiro_id,
							 tr.tid     roteiro_tid,
							 tc.tid
						from tab_checagem_roteiro tc, tab_roteiro tr
					   where tc.checagem = i.id
						 and tc.roteiro = tr.id) loop
				insert into hst_checagem_roteiro
					(id,
					 id_hst,
					 tid,
					 checagem_rot_id,
					 checagem_id,
					 checagem_tid,
					 roteiro_id,
					 roteiro_tid)
				values
					(seq_hst_checagem_roteiro.nextval,
					 v_id,
					 i.tid,
					 j.id,
					 i.id,
					 i.tid,
					 j.roteiro_id,
					 j.roteiro_tid);
			end loop;
		
			----------------------------------------------------------------------------------------------------------------
			-- Checagem/Itens
			----------------------------------------------------------------------------------------------------------------
			for j in (select ai.id,
							 ri.id     item_id,
							 ri.tid    item_tid,
							 ls.id     situacao_id,
							 ls.texto  situacao_texto,
							 ai.motivo,
							 ai.tid
						from tab_checagem_itens         ai,
							 tab_roteiro_item           ri,
							 lov_checagem_item_situacao ls
					   where ai.situacao = ls.id
						 and ai.item_id = ri.id
						 and ai.checagem = i.id) loop
				insert into hst_checagem_itens
					(id,
					 id_hst,
					 itens_id,
					 checagem_id,
					 checagem_tid,
					 item_id,
					 item_tid,
					 situacao_id,
					 situacao_texto,
					 motivo,
					 tid)
				values
					(seq_hst_checagem_itens.nextval,
					 v_id,
					 j.id,
					 i.id,
					 i.tid,
					 j.item_id,
					 j.item_tid,
					 j.situacao_id,
					 j.situacao_texto,
					 j.motivo,
					 i.tid);
			end loop;
		
		end loop;
		----------------------------------------------------------------------------------------------------------------
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Checagem de item de roteiro. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Checagem de item de roteiro. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		
	end;
	---------------------------------------------------------

	---------------------------------------------------------
	-- Checagem de Pendência
	---------------------------------------------------------
	procedure checagempendencia(p_id               number,
								p_acao             number,
								p_executor_id      number,
								p_executor_nome    varchar2,
								p_executor_login   varchar2,
								p_executor_tipo_id number,
								p_executor_tid     varchar2) is
		v_sucesso boolean := false;
		v_id      number;
	begin
	
		----------------------------------------------------------------------------------------------------------------
	
		----------------------------------------------------------------------------------------------------------------
		-- Checagem
		----------------------------------------------------------------------------------------------------------------
		for i in (select c.id,
						 c.numero,
						 c.titulo   titulo_id,
						 t.tid      titulo_tid,
						 c.situacao situacao_id,
						 s.texto    situacao_texto,
						 c.tid
					from tab_checagem_pend          c,
						 tab_titulo                 t,
						 lov_checagem_pend_situacao s
				   where c.situacao = s.id
					 and c.titulo = t.id
					 and c.id = p_id) loop
		
			-- Inserindo na histórico
			insert into hst_checagem_pend p
				(id,
				 checagem_pend_id,
				 tid,
				 numero,
				 titulo_id,
				 titulo_tid,
				 situacao_id,
				 situacao_texto,
				 executor_id,
				 executor_tid,
				 executor_nome,
				 executor_login,
				 executor_tipo_id,
				 executor_tipo_texto,
				 acao_executada,
				 data_execucao)
			values
				(seq_hst_checagem_pend.nextval,
				 i.id,
				 i.tid,
				 i.numero,
				 i.titulo_id,
				 i.titulo_tid,
				 i.situacao_id,
				 i.situacao_texto,
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
					 and la.artefato = 23),
				 systimestamp)
			returning p.id into v_id;
		
			----------------------------------------------------------------------------------------------------------------
			-- Checagem/Itens
			----------------------------------------------------------------------------------------------------------------
			for j in (select ai.id,
							 ai.nome,
							 ri.id    item_id,
							 ri.tid   item_tid,
							 ls.id    situacao_id,
							 ls.texto situacao_texto,
							 ai.tid
						from tab_checagem_pend_itens    ai,
							 tab_roteiro_item           ri,
							 lov_checagem_pend_item_sit ls
					   where ai.situacao = ls.id
						 and ai.item_id = ri.id
						 and ai.checagem = i.id) loop
				insert into hst_checagem_pend_itens
					(id,
					 id_hst,
					 pendencia_itens_id,
					 checagem_pend_id,
					 checagem_pend_tid,
					 item_id,
					 item_tid,
					 nome,
					 situacao_id,
					 situacao_texto,
					 tid)
				values
					(seq_hst_checagem_pend_itens.nextval,
					 v_id,
					 j.id,
					 i.id,
					 i.tid,
					 j.item_id,
					 j.item_tid,
					 j.nome,
					 j.situacao_id,
					 j.situacao_texto,
					 i.tid);
			end loop;
		
		end loop;
		----------------------------------------------------------------------------------------------------------------
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Checagem de Pendência. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Checagem de Pendência. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		
	end;
	---------------------------------------------------------

	---------------------------------------------------------
	-- Empreendimento
	---------------------------------------------------------
	procedure empreendimento(p_id               number,
							 p_acao             number,
							 p_executor_id      number,
							 p_executor_nome    varchar2,
							 p_executor_login   varchar2,
							 p_executor_tipo_id number,
							 p_executor_tid     varchar2) is
		v_sucesso boolean := false;
		v_id      number;
	begin
	
		----------------------------------------------------------------------------------------------------------------
	
		----------------------------------------------------------------------------------------------------------------
		-- Empreendimento
		----------------------------------------------------------------------------------------------------------------
		for i in (select e.id,
						 e.segmento segmento_id,
						 ls.texto segmento_texto,
						 e.cnpj,
						 e.codigo,
						 e.denominador,
						 e.nome_fantasia,
						 e.atividade atividade_id,
						 a.tid atividade_tid,
						 e.tid,
						 (select ee.zona
							from tab_empreendimento_endereco ee
						   where ee.correspondencia = 0
							 and ee.empreendimento = e.id) zona
					from tab_empreendimento           e,
						 lov_empreendimento_segmento  ls,
						 tab_empreendimento_atividade a
				   where e.segmento = ls.id
					 and e.atividade = a.id(+)
					 and e.id = p_id) loop
		
			-- Inserindo na histórico
			insert into hst_empreendimento p
				(id,
				 empreendimento_id,
				 tid,
				 codigo,
				 segmento_id,
				 segmento_texto,
				 cnpj,
				 denominador,
				 nome_fantasia,
				 atividade_id,
				 atividade_tid,
				 zona,
				 executor_id,
				 executor_tid,
				 executor_nome,
				 executor_login,
				 executor_tipo_id,
				 executor_tipo_texto,
				 acao_executada,
				 data_execucao)
			values
				(seq_hst_empreendimento.nextval,
				 i.id,
				 i.tid,
				 i.codigo,
				 i.segmento_id,
				 i.segmento_texto,
				 i.cnpj,
				 i.denominador,
				 i.nome_fantasia,
				 i.atividade_id,
				 i.atividade_tid,
				 i.zona,
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
					 and la.artefato = 8),
				 systimestamp)
			returning p.id into v_id;
		
			----------------------------------------------------------------------------------------------------------------
			-- Empreendimento/Responsaveis
			----------------------------------------------------------------------------------------------------------------
			for j in (select tr.id,
							 tr.empreendimento,
							 tr.tipo                   tipo_id,
							 lt.texto                  tipo_texto,
							 tr.data_vencimento,
							 tr.especificar,
							 pp.id                     responsavel_id,
							 pp.tid                    responsavel_tid,
							 tr.origem,
							 tr.origem_texto,
							 tr.credenciado_usuario_id,
							 tr.tid
						from tab_empreendimento_responsavel tr,
							 tab_pessoa                     pp,
							 lov_empreendimento_tipo_resp   lt
					   where pp.id = tr.responsavel
						 and tr.empreendimento = i.id
						 and lt.id = tr.tipo) loop
			
				insert into hst_empreendimento_responsavel
					(id,
					 id_hst,
					 emp_resp_id,
					 empreendimento_id,
					 empreendimento_tid,
					 tipo_id,
					 tipo_texto,
					 data_vencimento,
					 especificar,
					 responsavel_id,
					 responsavel_tid,
					 origem,
					 origem_texto,
					 credenciado_usuario_id,
					 tid)
				values
					(seq_hst_empreendimento_resp.nextval,
					 v_id,
					 j.id,
					 i.id,
					 i.tid,
					 j.tipo_id,
					 j.tipo_texto,
					 j.data_vencimento,
					 j.especificar,
					 j.responsavel_id,
					 j.responsavel_tid,
					 j.origem,
					 j.origem_texto,
					 j.credenciado_usuario_id,
					 i.tid);
			end loop;
		
			----------------------------------------------------------------------------------------------------------------
			-- Empreendimento/Endereço
			----------------------------------------------------------------------------------------------------------------
			for j in (select te.id,
							 te.empreendimento,
							 te.correspondencia,
							 te.zona,
							 te.cep,
							 te.caixa_postal,
							 te.distrito,
							 te.corrego,
							 logradouro,
							 bairro,
							 le.id              estado_id,
							 le.texto           estado_texto,
							 lm.id              municipio_id,
							 lm.texto           municipio_texto,
							 numero,
							 complemento,
							 te.tid
						from tab_empreendimento_endereco te,
							 lov_estado                  le,
							 lov_municipio               lm
					   where te.estado = le.id
						 and te.municipio = lm.id
						 and te.empreendimento = p_id) loop
			
				insert into hst_empreendimento_endereco
					(id,
					 id_hst,
					 endereco_id,
					 correspondencia,
					 zona,
					 cep,
					 caixa_postal,
					 distrito,
					 corrego,
					 logradouro,
					 bairro,
					 estado_id,
					 estado_texto,
					 municipio_id,
					 municipio_texto,
					 numero,
					 complemento,
					 tid)
				values
					(seq_hst_empreendimento_endere.nextval,
					 v_id,
					 j.id,
					 j.correspondencia,
					 j.zona,
					 j.cep,
					 j.caixa_postal,
					 j.distrito,
					 j.corrego,
					 j.logradouro,
					 j.bairro,
					 j.estado_id,
					 j.estado_texto,
					 j.municipio_id,
					 j.municipio_texto,
					 j.numero,
					 j.complemento,
					 i.tid);
			end loop;
		
			----------------------------------------------------------------------------------------------------------------
			-- Empreendimento/Coordenada
			----------------------------------------------------------------------------------------------------------------
			for j in (select tc.id,
							 tc.empreendimento,
							 tc.tipo_coordenada tipo_coordenada_id,
							 lt.texto           tipo_coordenada_texto,
							 tc.datum           datum_id,
							 ld.sigla           datum_sigla,
							 ld.texto           datum_texto,
							 tc.easting_utm,
							 tc.northing_utm,
							 tc.fuso_utm,
							 tc.hemisferio_utm  hemisferio_utm_id,
							 lh.texto           hemisferio_utm_texto,
							 tc.latitude_gms,
							 tc.longitude_gms,
							 tc.latitude_gdec,
							 tc.longitude_gdec,
							 tc.forma_coleta    forma_coleta_id,
							 fc.texto           forma_coleta_texto,
							 tc.local_coleta    local_coleta_id,
							 lc.texto           local_coleta_texto,
							 tc.tid
						from tab_empreendimento_coord       tc,
							 lov_coordenada_tipo            lt,
							 lov_empreendimento_forma_colet fc,
							 lov_empreendimento_local_colet lc,
							 lov_coordenada_datum           ld,
							 lov_coordenada_hemisferio      lh
					   where tc.tipo_coordenada = lt.id(+)
						 and tc.datum = ld.id(+)
						 and tc.hemisferio_utm = lh.id(+)
						 and tc.forma_coleta = fc.id(+)
						 and tc.local_coleta = lc.id(+)
						 and tc.empreendimento = i.id) loop
				--
				insert into hst_empreendimento_coord ac
					(id,
					 id_hst,
					 coordenada_id,
					 tid,
					 tipo_coordenada_id,
					 tipo_coordenada_texto,
					 datum_id,
					 datum_sigla,
					 datum_texto,
					 easting_utm,
					 northing_utm,
					 fuso_utm,
					 hemisferio_utm_id,
					 hemisferio_utm_texto,
					 latitude_gms,
					 longitude_gms,
					 latitude_gdec,
					 longitude_gdec,
					 forma_coleta_id,
					 forma_coleta_texto,
					 local_coleta_id,
					 local_coleta_texto)
				values
					(seq_hst_empreendimento_coord.nextval,
					 v_id,
					 j.id,
					 i.tid,
					 j.tipo_coordenada_id,
					 j.tipo_coordenada_texto,
					 j.datum_id,
					 j.datum_sigla,
					 j.datum_texto,
					 j.easting_utm,
					 j.northing_utm,
					 j.fuso_utm,
					 j.hemisferio_utm_id,
					 j.hemisferio_utm_texto,
					 j.latitude_gms,
					 j.longitude_gms,
					 j.latitude_gdec,
					 j.longitude_gdec,
					 j.forma_coleta_id,
					 j.forma_coleta_texto,
					 j.local_coleta_id,
					 j.local_coleta_texto);
			end loop;
		
			----------------------------------------------------------------------------------------------------------------
			-- Empreendimento/Meio de Contato
			----------------------------------------------------------------------------------------------------------------
			for j in (select m.id,
							 empreendimento,
							 tm.id          meio_contato_id,
							 tm.tid         meio_contato_tid,
							 valor,
							 m.tid
						from tab_empreendimento_contato m,
							 tab_meio_contato           tm
					   where tm.id = m.meio_contato
						 and m.empreendimento = i.id) loop
			
				insert into hst_empreendimento_contato
					(id,
					 id_hst,
					 emp_contato_id,
					 meio_contato_id,
					 meio_contato_tid,
					 valor,
					 tid)
				values
					(seq_hst_empreendimento_contato.nextval,
					 v_id,
					 j.id,
					 j.meio_contato_id,
					 j.meio_contato_tid,
					 j.valor,
					 i.tid);
			end loop;
		end loop;
		----------------------------------------------------------------------------------------------------------------
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Empreendimento. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Empreendimento. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		
	end;
	---------------------------------------------------------

	---------------------------------------------------------
	-- Requerimento
	---------------------------------------------------------
	procedure requerimento(p_id               number,
						   p_acao             number,
						   p_executor_id      number,
						   p_executor_nome    varchar2,
						   p_executor_login   varchar2,
						   p_executor_tipo_id number,
						   p_executor_tid     varchar2) is
		v_sucesso boolean := false;
		v_id      number;
		v_id_ativ number;
	begin
	
		----------------------------------------------------------------------------------------------------------------
	
		----------------------------------------------------------------------------------------------------------------
		-- Requerimento
		----------------------------------------------------------------------------------------------------------------
		for i in (select r.id,
						 r.tid,
						 r.numero,
						 r.data_criacao,
						 r.interessado    interessado_id,
						 tp.tid           interessado_tid,
						 r.empreendimento empreendimento_id,
						 e.tid            empreendimento_tid,
						 r.situacao       situacao_id,
						 ls.texto         situacao_texto,
						 r.agendamento    agendamento_id,
						 la.texto         agendamento_texto,
						 ts.id            setor_id,
						 ts.tid           setor_tid,
						 r.informacoes,
						 tc.id            credenciado_id,
						 tc.tid           credenciado_tid
					from tab_credenciado              tc,
						 tab_requerimento             r,
						 tab_pessoa                   tp,
						 tab_empreendimento           e,
						 tab_setor                    ts,
						 lov_requerimento_situacao    ls,
						 lov_requerimento_agendamento la
				   where r.autor = tc.id(+)
					 and r.setor = ts.id(+)
					 and r.empreendimento = e.id(+)
					 and r.agendamento = la.id(+)
					 and r.situacao = ls.id
					 and r.interessado = tp.id(+)
					 and r.id = p_id) loop
		
			-- Inserindo na histórico
			insert into hst_requerimento p
				(id,
				 requerimento_id,
				 tid,
				 numero,
				 data_criacao,
				 interessado_id,
				 interessado_tid,
				 empreendimento_id,
				 empreendimento_tid,
				 situacao_id,
				 situacao_texto,
				 agendamento_id,
				 agendamento_texto,
				 setor_id,
				 setor_tid,
				 informacoes,
				 executor_id,
				 executor_tid,
				 executor_nome,
				 executor_login,
				 executor_tipo_id,
				 executor_tipo_texto,
				 acao_executada,
				 data_execucao,
				 autor_id,
				 autor_tid)
			values
				(seq_hst_requerimento.nextval,
				 i.id,
				 i.tid,
				 i.numero,
				 i.data_criacao,
				 i.interessado_id,
				 i.interessado_tid,
				 i.empreendimento_id,
				 i.empreendimento_tid,
				 i.situacao_id,
				 i.situacao_texto,
				 i.agendamento_id,
				 i.agendamento_texto,
				 i.setor_id,
				 i.setor_tid,
				 i.informacoes,
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
					 and la.artefato = 9),
				 systimestamp,
				 i.credenciado_id,
				 i.credenciado_tid)
			returning p.id into v_id;
		
			----------------------------------------------------------------------------------------------------------------
			-- Atividades
			----------------------------------------------------------------------------------------------------------------
			for j in (select tc.id,
							 tc.atividade atividade_id,
							 pa.tid       atividade_tid,
							 tc.tid
						from tab_requerimento_atividade tc, tab_atividade pa
					   where tc.requerimento = i.id
						 and tc.atividade = pa.id) loop
				insert into hst_requerimento_atividade e
					(id,
					 id_hst,
					 tid,
					 requeri_ativ_id,
					 requerimento_id,
					 requerimento_tid,
					 atividade_id,
					 atividade_tid)
				values
					(seq_hst_requerimento_atividade.nextval,
					 v_id,
					 i.tid,
					 j.id,
					 i.id,
					 i.tid,
					 j.atividade_id,
					 j.atividade_tid)
				returning e.id into v_id_ativ;
			
				----------------------------------------------------------------------------------------------------------------
				-- Atividades/Finalidade/Modelos/Titulo Anterior
				----------------------------------------------------------------------------------------------------------------
				for k in (select ta.id,
								 ta.requerimento_ativ,
								 ta.finalidade,
								 tm.id modelo_id,
								 tm.tid modelo_tid,
								 ta.modelo_anterior_id,
								 case
									 when ta.titulo_anterior_tipo = 1 then
									  (select ttm.tid
										 from tab_titulo_modelo ttm
										where ttm.id = ta.modelo_anterior_id)
									 else
									  null
								 end modelo_anterior_tid,
								 ta.modelo_anterior_nome,
								 ta.modelo_anterior_sigla,
								 ta.titulo_anterior_numero,
								 ta.titulo_anterior_tipo titulo_anterior_tipo_id,
								 (select lt.texto
									from lov_titulo_anterior_tipo lt
								   where lt.id = ta.titulo_anterior_tipo) titulo_anterior_tipo_texto,
								 ta.titulo_anterior_id,
								 (select tt.tid
									from tab_titulo tt
								   where tt.id = ta.titulo_anterior_id) titulo_anterior_tid,
								 ta.orgao_expedidor
							from tab_requerimento_ativ_finalida ta,
								 tab_titulo_modelo              tm
						   where ta.modelo = tm.id
							 and ta.requerimento_ativ = j.id) loop
					insert into hst_requerimento_ativ_finalida
						(id,
						 id_hst,
						 tid,
						 requerimento_ativ_fin_id,
						 finalidade,
						 modelo_id,
						 modelo_tid,
						 modelo_anterior_id,
						 modelo_anterior_tid,
						 titulo_anterior_numero,
						 titulo_anterior_tipo_id,
						 titulo_anterior_tipo_texto,
						 titulo_anterior_id,
						 titulo_anterior_tid,
						 modelo_anterior_nome,
						 modelo_anterior_sigla,
						 orgao_expedidor)
					values
						(seq_hst_requerimento_ativ_fin.nextval,
						 v_id_ativ,
						 i.tid,
						 k.id,
						 k.finalidade,
						 k.modelo_id,
						 k.modelo_tid,
						 k.modelo_anterior_id,
						 k.modelo_anterior_tid,
						 k.titulo_anterior_numero,
						 k.titulo_anterior_tipo_id,
						 k.titulo_anterior_tipo_texto,
						 k.titulo_anterior_id,
						 k.titulo_anterior_tid,
						 k.modelo_anterior_nome,
						 k.modelo_anterior_sigla,
						 k.orgao_expedidor);
				end loop;
			end loop;
		
			----------------------------------------------------------------------------------------------------------------
			-- Requerimento/Responsaveis
			----------------------------------------------------------------------------------------------------------------
			for j in (select tr.id,
							 pp.id         responsavel_id,
							 pp.tid        responsavel_tid,
							 tr.funcao     funcao_id,
							 f.texto       funcao_texto,
							 tr.numero_art,
							 tr.tid
						from tab_requerimento_responsavel tr,
							 tab_pessoa                   pp,
							 lov_protocolo_resp_funcao    f
					   where pp.id = tr.responsavel
						 and tr.funcao = f.id
						 and tr.requerimento = i.id) loop
				insert into hst_requerimento_responsavel
					(id,
					 id_hst,
					 tid,
					 requeri_resp_id,
					 requerimento_id,
					 requerimento_tid,
					 responsavel_id,
					 responsavel_tid,
					 funcao_id,
					 funcao_texto,
					 numero_art)
				values
					(seq_hst_requerimento_resp.nextval,
					 v_id,
					 i.tid,
					 j.id,
					 i.id,
					 i.tid,
					 j.responsavel_id,
					 j.responsavel_tid,
					 j.funcao_id,
					 j.funcao_texto,
					 j.numero_art);
			end loop;
		
		end loop;
		----------------------------------------------------------------------------------------------------------------
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Requerimento. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Requerimento. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		
	end;
	---------------------------------------------------------

	---------------------------------------------------------
	-- Protocolo
	---------------------------------------------------------
	procedure protocolo(p_id               number,
						p_acao             number,
						p_executor_id      number,
						p_executor_nome    varchar2,
						p_executor_login   varchar2,
						p_executor_tipo_id number,
						p_executor_tid     varchar2) is
		v_sucesso boolean := false;
		v_id      number;
	begin
	
		----------------------------------------------------------------------------------------------------------------
	
		----------------------------------------------------------------------------------------------------------------
		-- Protocolo
		----------------------------------------------------------------------------------------------------------------
		for i in (select p.id,
						 p.tid,
						 p.numero,
						 p.ano,
						 p.numero_autuacao,
						 p.data_autuacao,
						 p.data_criacao,
						 p.protocolo           protocolo_id,
						 lo.texto              protocolo_texto,
						 p.tipo                tipo_id,
						 lp.texto              tipo_texto,
						 p.volume,
						 p.situacao            situacao_id,
						 ls.texto              situacao_texto,
						 p.interessado         interessado_id,
						 i.tid                 interessado_tid,
						 p.requerimento        requerimento_id,
						 r.tid                 requerimento_tid,
						 p.empreendimento      empreendimento_id,
						 e.tid                 empreendimento_tid,
						 p.checagem            checagem_id,
						 c.tid                 checagem_tid,
						 p.checagem_pendencia  checagem_pendencia_id,
						 cp.tid                checagem_pendencia_tid,
						 p.protocolo_associado protocolo_associado_id,
						 pro.tid               protocolo_associado_tid,
						 p.arquivo             arquivo_id,
						 ta.tid                arquivo_tid,
						 p.emposse             emposse_id,
						 f.tid                 emposse_tid,
						 ts.id                 setor_id,
						 ts.tid                setor_tid,
						 tsc.id                setor_criacao_id,
						 tsc.tid               setor_criacao_tid,
						 fis.id                fiscalizacao_id,
						 fis.tid               fiscalizacao_tid
					from tab_protocolo          p,
						 tab_protocolo          pro,
						 tab_arquivo            ta,
						 tab_setor              ts,
						 tab_setor              tsc,
						 lov_protocolo_tipo     lp,
						 lov_protocolo          lo,
						 lov_protocolo_situacao ls,
						 tab_checagem           c,
						 tab_checagem_pend      cp,
						 tab_requerimento       r,
						 tab_pessoa             i,
						 tab_funcionario        f,
						 tab_empreendimento     e,
						 tab_fiscalizacao       fis
				   where p.protocolo_associado = pro.id(+)
					 and p.arquivo = ta.id(+)
					 and p.setor = ts.id(+)
					 and p.setor_criacao = tsc.id(+)
					 and p.tipo = lp.id
					 and p.checagem = c.id(+)
					 and p.checagem_pendencia = cp.id(+)
					 and p.requerimento = r.id(+)
					 and p.interessado = i.id(+)
					 and p.emposse = f.id(+)
					 and p.empreendimento = e.id(+)
					 and p.fiscalizacao = fis.id(+)
					 and p.protocolo = lo.id
					 and p.situacao = ls.id
					 and p.id = p_id) loop
		
			-- Inserindo na histórico
			insert into hst_protocolo p
				(id,
				 id_protocolo,
				 tid,
				 numero,
				 ano,
				 numero_autuacao,
				 data_autuacao,
				 data_criacao,
				 protocolo_id,
				 protocolo_texto,
				 tipo_id,
				 tipo_texto,
				 volume,
				 situacao_id,
				 situacao_texto,
				 interessado_id,
				 interessado_tid,
				 requerimento_id,
				 requerimento_tid,
				 empreendimento_id,
				 empreendimento_tid,
				 checagem_id,
				 checagem_tid,
				 checagem_pendencia_id,
				 checagem_pendencia_tid,
				 setor_id,
				 setor_tid,
				 setor_criacao_id,
				 setor_criacao_tid,
				 protocolo_associado_id,
				 protocolo_associado_tid,
				 arquivo_id,
				 arquivo_tid,
				 emposse_id,
				 emposse_tid,
				 fiscalizacao_id,
				 fiscalizacao_tid,
				 executor_id,
				 executor_tid,
				 executor_nome,
				 executor_login,
				 executor_tipo_id,
				 executor_tipo_texto,
				 acao_executada,
				 data_execucao)
			
			values
				(seq_hst_protocolo.nextval,
				 i.id,
				 i.tid,
				 i.numero,
				 i.ano,
				 i.numero_autuacao,
				 i.data_autuacao,
				 i.data_criacao,
				 i.protocolo_id,
				 i.protocolo_texto,
				 i.tipo_id,
				 i.tipo_texto,
				 i.volume,
				 i.situacao_id,
				 i.situacao_texto,
				 i.interessado_id,
				 i.interessado_tid,
				 i.requerimento_id,
				 i.requerimento_tid,
				 i.empreendimento_id,
				 i.empreendimento_tid,
				 i.checagem_id,
				 i.checagem_tid,
				 i.checagem_pendencia_id,
				 i.checagem_pendencia_tid,
				 i.setor_id,
				 i.setor_tid,
				 i.setor_criacao_id,
				 i.setor_criacao_tid,
				 i.protocolo_associado_id,
				 i.protocolo_associado_tid,
				 i.arquivo_id,
				 i.arquivo_tid,
				 i.emposse_id,
				 i.emposse_tid,
				 i.fiscalizacao_id,
				 i.fiscalizacao_tid,
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
					 and la.artefato = 10),
				 systimestamp)
			returning p.id into v_id;
		
			----------------------------------------------------------------------------------------------------------------
			-- protocolo/Responsaveis
			----------------------------------------------------------------------------------------------------------------
			for j in (select tr.id,
							 pp.id         responsavel_id,
							 pp.tid        responsavel_tid,
							 tr.funcao     funcao_id,
							 f.texto       funcao_texto,
							 tr.numero_art,
							 tr.tid
						from tab_protocolo_responsavel tr,
							 tab_pessoa                pp,
							 lov_protocolo_resp_funcao f
					   where pp.id = tr.responsavel
						 and tr.funcao = f.id
						 and tr.protocolo = i.id) loop
				insert into hst_protocolo_responsavel
					(id,
					 id_hst,
					 tid,
					 protocolo_resp_id,
					 protocolo_id,
					 protocolo_tid,
					 responsavel_id,
					 responsavel_tid,
					 funcao_id,
					 funcao_texto,
					 numero_art)
				values
					(seq_hst_protocolo_resp.nextval,
					 v_id,
					 i.tid,
					 j.id,
					 i.id,
					 i.tid,
					 j.responsavel_id,
					 j.responsavel_tid,
					 j.funcao_id,
					 j.funcao_texto,
					 j.numero_art);
			end loop;
		
		end loop;
		----------------------------------------------------------------------------------------------------------------
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Protocolo. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Protocolo. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
	---------------------------------------------------------

	---------------------------------------------------------
	-- Atividade de Protocolo
	---------------------------------------------------------
	procedure protocoloatividade(p_id               number,
								 p_acao             number,
								 p_executor_id      number,
								 p_executor_nome    varchar2,
								 p_executor_login   varchar2,
								 p_executor_tipo_id number,
								 p_executor_tid     varchar2) is
		v_sucesso boolean := false;
		v_id      number;
	begin
	
		----------------------------------------------------------------------------------------------------------------
	
		----------------------------------------------------------------------------------------------------------------
		-- Atividade de Protocolo
		----------------------------------------------------------------------------------------------------------------
		for i in (select a.id,
						 p.id     protocolo_id,
						 p.tid    protocolo_tid,
						 r.id     requerimento_id,
						 r.tid    requerimento_tid,
						 t.id     atividade_id,
						 t.tid    atividade_tid,
						 ls.id    situacao_id,
						 ls.texto situacao_texto,
						 a.motivo,
						 a.tid
					from tab_protocolo_atividades a,
						 tab_atividade            t,
						 tab_protocolo            p,
						 tab_requerimento         r,
						 lov_atividade_situacao   ls
				   where a.protocolo = p.id
					 and a.requerimento = r.id
					 and a.atividade = t.id
					 and a.situacao = ls.id
					 and a.id = p_id) loop
		
			-- Inserindo na histórico
			insert into hst_protocolo_atividades a
				(id,
				 id_atividade,
				 tid,
				 protocolo_id,
				 protocolo_tid,
				 requerimento_id,
				 requerimento_tid,
				 atividade_id,
				 atividade_tid,
				 situacao_id,
				 situacao_texto,
				 motivo,
				 executor_id,
				 executor_tid,
				 executor_nome,
				 executor_login,
				 executor_tipo_id,
				 executor_tipo_texto,
				 acao_executada,
				 data_execucao)
			values
				(seq_hst_protocolo_atividades.nextval,
				 i.id,
				 i.tid,
				 i.protocolo_id,
				 i.protocolo_tid,
				 i.requerimento_id,
				 i.requerimento_tid,
				 i.atividade_id,
				 i.atividade_tid,
				 i.situacao_id,
				 i.situacao_texto,
				 i.motivo,
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
					 and la.artefato = 25),
				 systimestamp)
			returning a.id into v_id;
		
			----------------------------------------------------------------------------------------------------------------
			-- Atividades/Finalidade/Modelos/Titulo Anterior
			----------------------------------------------------------------------------------------------------------------
			for k in (select ta.id,
							 ta.protocolo_ativ,
							 ta.finalidade,
							 tm.id modelo_id,
							 tm.tid modelo_tid,
							 ta.modelo_anterior_id,
							 case
								 when ta.titulo_anterior_tipo = 1 then
								  (select ttm.tid
									 from tab_titulo_modelo ttm
									where ttm.id = ta.modelo_anterior_id)
								 else
								  null
							 end modelo_anterior_tid,
							 ta.modelo_anterior_nome,
							 ta.modelo_anterior_sigla,
							 ta.titulo_anterior_numero,
							 ta.titulo_anterior_tipo titulo_anterior_tipo_id,
							 (select lt.texto
								from lov_titulo_anterior_tipo lt
							   where lt.id = ta.titulo_anterior_tipo) titulo_anterior_tipo_texto,
							 ta.titulo_anterior_id,
							 (select ttm.tid
								from tab_titulo_modelo ttm
							   where ttm.id = ta.titulo_anterior_id) titulo_anterior_tid,
							 ta.orgao_expedidor
						from tab_protocolo_ativ_finalida ta,
							 tab_titulo_modelo           tm
					   where ta.modelo = tm.id
						 and ta.protocolo_ativ = i.id) loop
				insert into hst_protocolo_ativ_finalida
					(id,
					 id_hst,
					 tid,
					 protocolo_ativ_fin_id,
					 finalidade,
					 modelo_id,
					 modelo_tid,
					 modelo_anterior_id,
					 modelo_anterior_tid,
					 titulo_anterior_numero,
					 titulo_anterior_tipo_id,
					 titulo_anterior_tipo_texto,
					 titulo_anterior_id,
					 titulo_anterior_tid,
					 modelo_anterior_nome,
					 modelo_anterior_sigla,
					 orgao_expedidor)
				values
					(seq_hst_protocolo_ativ_fin.nextval,
					 v_id,
					 i.tid,
					 k.id,
					 k.finalidade,
					 k.modelo_id,
					 k.modelo_tid,
					 k.modelo_anterior_id,
					 k.modelo_anterior_tid,
					 k.titulo_anterior_numero,
					 k.titulo_anterior_tipo_id,
					 k.titulo_anterior_tipo_texto,
					 k.titulo_anterior_id,
					 k.titulo_anterior_tid,
					 k.modelo_anterior_nome,
					 k.modelo_anterior_sigla,
					 k.orgao_expedidor);
			end loop;
		end loop;
		----------------------------------------------------------------------------------------------------------------
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Atividade de Protocolo.');
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Atividade de Protocolo. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
	---------------------------------------------------------

	---------------------------------------------------------
	-- Apensar protocolo a outro
	---------------------------------------------------------
	procedure apensar(p_id               number,
					  p_acao             number,
					  p_executor_id      number,
					  p_executor_nome    varchar2,
					  p_executor_login   varchar2,
					  p_executor_tipo_id number,
					  p_executor_tid     varchar2) is
		v_sucesso boolean := false;
	begin
		----------------------------------------------------------------------------------------------------------------
		-- Apensar protocolo a outro
		----------------------------------------------------------------------------------------------------------------
		for i in (select p.id,
						 p.tid,
						 p.protocolo protocolo_id,
						 pp.tid      protocolo_tid,
						 pa.id       associado_id,
						 pa.tid      associado_tid,
						 lo.id       tipo_id,
						 lo.texto    tipo_texto
					from tab_protocolo_associado p,
						 tab_protocolo           pp,
						 tab_protocolo           pa,
						 lov_protocolo           lo
				   where pa.protocolo = lo.id
					 and pp.id = p.protocolo
					 and pa.id = p.associado
					 and p.associado = p_id) loop
			-- Inserindo na histórico
			insert into hst_protocolo_associado p
				(id,
				 tid,
				 protocolo_ass_id,
				 protocolo_id,
				 protocolo_tid,
				 associado_id,
				 associado_tid,
				 tipo_id,
				 tipo_texto,
				 executor_id,
				 executor_tid,
				 executor_nome,
				 executor_login,
				 executor_tipo_id,
				 executor_tipo_texto,
				 acao_executada,
				 data_execucao)
			values
				(seq_hst_protocolo_associado.nextval,
				 i.tid,
				 i.id,
				 i.protocolo_id,
				 i.protocolo_tid,
				 i.associado_id,
				 i.associado_tid,
				 i.tipo_id,
				 i.tipo_texto,
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
					 and la.artefato = 10),
				 systimestamp);
		end loop;
		----------------------------------------------------------------------------------------------------------------
	
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Apensar Protocolo. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Apensar Protocolo. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
	---------------------------------------------------------

	---------------------------------------------------------
	-- Juntar documento a protocolo
	---------------------------------------------------------
	procedure juntar(p_id               number,
					 p_acao             number,
					 p_executor_id      number,
					 p_executor_nome    varchar2,
					 p_executor_login   varchar2,
					 p_executor_tipo_id number,
					 p_executor_tid     varchar2) is
		v_sucesso boolean := false;
	begin
		----------------------------------------------------------------------------------------------------------------
		-- Apensar protocolo a outro
		----------------------------------------------------------------------------------------------------------------
		for i in (select p.id,
						 p.tid,
						 p.protocolo protocolo_id,
						 pp.tid      protocolo_tid,
						 pa.id       associado_id,
						 pa.tid      associado_tid,
						 lo.id       tipo_id,
						 lo.texto    tipo_texto
					from tab_protocolo_associado p,
						 tab_protocolo           pp,
						 tab_protocolo           pa,
						 lov_protocolo           lo
				   where pa.protocolo = lo.id
					 and pp.id = p.protocolo
					 and pa.id = p.associado
					 and p.associado = p_id) loop
			-- Inserindo na histórico
			insert into hst_protocolo_associado p
				(id,
				 tid,
				 protocolo_ass_id,
				 protocolo_id,
				 protocolo_tid,
				 associado_id,
				 associado_tid,
				 tipo_id,
				 tipo_texto,
				 executor_id,
				 executor_tid,
				 executor_nome,
				 executor_login,
				 executor_tipo_id,
				 executor_tipo_texto,
				 acao_executada,
				 data_execucao)
			values
				(seq_hst_protocolo_associado.nextval,
				 i.tid,
				 i.id,
				 i.protocolo_id,
				 i.protocolo_tid,
				 i.associado_id,
				 i.associado_tid,
				 i.tipo_id,
				 i.tipo_texto,
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
					 and la.artefato = 10),
				 systimestamp);
		end loop;
		----------------------------------------------------------------------------------------------------------------
	
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Apensar protocolo. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Apensar protocolo. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
	---------------------------------------------------------

	---------------------------------------------------------
	-- Análise de itens de protocolo/documento
	---------------------------------------------------------
	procedure analiseitens(p_id               number,
						   p_acao             number,
						   p_executor_id      number,
						   p_executor_nome    varchar2,
						   p_executor_login   varchar2,
						   p_executor_tipo_id number,
						   p_executor_tid     varchar2) is
		v_sucesso boolean := false;
		v_id      number;
	begin
	
		----------------------------------------------------------------------------------------------------------------
	
		----------------------------------------------------------------------------------------------------------------
		-- Análise de itens de protocolo/documento
		----------------------------------------------------------------------------------------------------------------
		for i in (select a.id,
						 a.protocolo protocolo_id,
						 p.tid       protocolo_tid,
						 a.situacao  situacao_id,
						 ls.texto    situacao_texto,
						 c.id        checagem_id,
						 c.tid       checagem_tid,
						 a.tid
					from tab_analise          a,
						 lov_analise_situacao ls,
						 tab_protocolo        p,
						 tab_checagem         c
				   where a.protocolo = p.id(+)
					 and a.checagem = c.id(+)
					 and a.situacao = ls.id
					 and a.id = p_id) loop
		
			-- Inserindo na histórico
			insert into hst_analise p
				(id,
				 analise_id,
				 tid,
				 protocolo_id,
				 protocolo_tid,
				 checagem_id,
				 checagem_tid,
				 situacao_id,
				 situacao_texto,
				 executor_id,
				 executor_tid,
				 executor_nome,
				 executor_login,
				 executor_tipo_id,
				 executor_tipo_texto,
				 acao_executada,
				 data_execucao)
			values
				(seq_hst_analise.nextval,
				 i.id,
				 i.tid,
				 i.protocolo_id,
				 i.protocolo_tid,
				 i.checagem_id,
				 i.checagem_tid,
				 i.situacao_id,
				 i.situacao_texto,
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
					 and la.artefato = 13),
				 systimestamp)
			returning p.id into v_id;
		
			----------------------------------------------------------------------------------------------------------------
			-- Análise/Roteiros
			----------------------------------------------------------------------------------------------------------------
			for j in (select tc.id,
							 tc.roteiro roteiro_id,
							 tr.tid     roteiro_tid,
							 tc.tid
						from tab_analise_roteiro tc, tab_roteiro tr
					   where tc.analise = i.id
						 and tc.roteiro = tr.id) loop
				insert into hst_analise_roteiro
					(id,
					 id_hst,
					 tid,
					 analise_rot_id,
					 analise_id,
					 analise_tid,
					 roteiro_id,
					 roteiro_tid)
				values
					(seq_hst_analise_roteiro.nextval,
					 v_id,
					 i.tid,
					 j.id,
					 i.id,
					 i.tid,
					 j.roteiro_id,
					 j.roteiro_tid);
			end loop;
		
			----------------------------------------------------------------------------------------------------------------
			-- Análise/Itens
			----------------------------------------------------------------------------------------------------------------
			for j in (select ai.id,
							 c.id            checagem_id,
							 c.tid           checagem_tid,
							 ri.id           item_id,
							 ri.tid          item_tid,
							 ls.id           situacao_id,
							 ls.texto        situacao_texto,
							 ts.id           setor_id,
							 ts.tid          setor_tid,
							 ai.descricao,
							 ai.motivo,
							 ai.data_analise,
							 ai.analista,
							 ai.recebido,
							 ai.tid,
							 ai.avulso
						from tab_analise_itens         ai,
							 tab_roteiro_item          ri,
							 lov_analise_item_situacao ls,
							 tab_checagem              c,
							 tab_setor                 ts
					   where ai.setor = ts.id(+)
						 and ai.item_id = ri.id
						 and ai.checagem = c.id(+)
						 and ai.situacao = ls.id
						 and ai.analise = i.id) loop
				insert into hst_analise_itens
					(id,
					 id_hst,
					 tid,
					 itens_id,
					 analise_id,
					 analise_tid,
					 checagem_id,
					 checagem_tid,
					 item_id,
					 item_tid,
					 situacao_id,
					 situacao_texto,
					 setor_id,
					 setor_tid,
					 descricao,
					 motivo,
					 data_analise,
					 analista,
					 recebido,
					 avulso)
				values
					(seq_hst_analise_itens.nextval,
					 v_id,
					 i.tid,
					 j.id,
					 i.id,
					 i.tid,
					 j.checagem_id,
					 j.checagem_tid,
					 j.item_id,
					 j.item_tid,
					 j.situacao_id,
					 j.situacao_texto,
					 j.setor_id,
					 j.setor_tid,
					 j.descricao,
					 j.motivo,
					 j.data_analise,
					 j.analista,
					 j.recebido,
					 j.avulso);
			
			end loop;
		
		end loop;
		----------------------------------------------------------------------------------------------------------------
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Análise de itens de protocolo/documento. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Análise de itens de protocolo/documento. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
	---------------------------------------------------------

	---------------------------------------------------------
	-- Item do roteiro
	---------------------------------------------------------
	procedure itemroteiro(p_id               number,
						  p_acao             number,
						  p_executor_id      number,
						  p_executor_nome    varchar2,
						  p_executor_login   varchar2,
						  p_executor_tipo_id number,
						  p_executor_tid     varchar2) is
		v_sucesso boolean := false;
	begin
		----------------------------------------------------------------------------------------------------------------
		-- Item do roteiro
		----------------------------------------------------------------------------------------------------------------
		for i in (select r.id,
						 r.tid,
						 r.nome,
						 r.procedimento,
						 r.tipo                tipo_id,
						 lt.texto              tipo_texto,
						 r.condicionante,
						 r.caracterizacao_tipo,
						 lc.texto              caracterizacao_tipo_texto
					from tab_roteiro_item        r,
						 lov_roteiro_item_tipo   lt,
						 lov_caracterizacao_tipo lc
				   where r.tipo = lt.id
					 and lc.id(+) = r.caracterizacao_tipo
					 and r.id = p_id) loop
			-- Inserindo na histórico
			insert into hst_roteiro_item p
				(id,
				 tid,
				 item_id,
				 nome,
				 procedimento,
				 tipo_id,
				 tipo_texto,
				 condicionante,
				 caracterizacao_tipo_id,
				 caracterizacao_tipo_texto,
				 executor_id,
				 executor_tid,
				 executor_nome,
				 executor_login,
				 executor_tipo_id,
				 executor_tipo_texto,
				 acao_executada,
				 data_execucao)
			values
				(seq_hst_roteiro_item.nextval,
				 i.tid,
				 i.id,
				 i.nome,
				 i.procedimento,
				 i.tipo_id,
				 i.tipo_texto,
				 i.condicionante,
				 i.caracterizacao_tipo,
				 i.caracterizacao_tipo_texto,
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
					 and la.artefato = 12),
				 systimestamp);
		end loop;
		----------------------------------------------------------------------------------------------------------------
	
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Item Roteiro. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Item Roteiro. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
	---------------------------------------------------------

	---------------------------------------------------------
	-- Tramitação
	---------------------------------------------------------
	procedure tramitacao(p_id               number,
						 p_acao             number,
						 p_executor_id      number,
						 p_executor_nome    varchar2,
						 p_executor_login   varchar2,
						 p_executor_tipo_id number,
						 p_executor_tid     varchar2) is
		v_sucesso boolean := false;
		v_id      number;
	begin
		----------------------------------------------------------------------------------------------------------------
		-- Tramitação
		----------------------------------------------------------------------------------------------------------------
	
		for i in (select t.id,
						 t.protocolo          protocolo_id,
						 p.tid                protocolo_tid,
						 t.tipo               tipo_id,
						 lt.texto             tipo_texto,
						 t.objetivo           objetivo_id,
						 lo.texto             objetivo_texto,
						 t.situacao           situacao_id,
						 ls.texto             situacao_texto,
						 t.despacho,
						 t.data_envio,
						 t.remetente          remetente_id,
						 fr.tid               remetente_tid,
						 t.remetente_setor    remetente_setor_id,
						 sr.tid               remetente_setor_tid,
						 t.destinatario       destinatario_id,
						 fd.tid               destinatario_tid,
						 t.destinatario_setor destinatario_setor_id,
						 sd.tid               destinatario_setor_tid,
						 t.tid
					from tab_tramitacao          t,
						 tab_protocolo           p,
						 lov_tramitacao_tipo     lt,
						 lov_tramitacao_objetivo lo,
						 lov_tramitacao_situacao ls,
						 tab_funcionario         fr,
						 tab_funcionario         fd,
						 tab_setor               sr,
						 tab_setor               sd
				   where t.protocolo = p.id(+)
					 and t.tipo = lt.id
					 and t.objetivo = lo.id(+)
					 and t.situacao = ls.id
					 and t.remetente_setor = sr.id(+)
					 and t.remetente = fr.id(+)
					 and t.destinatario_setor = sd.id(+)
					 and t.destinatario = fd.id(+)
					 and t.id = p_id) loop
		
			-- Inserindo na histórico
			insert into hst_tramitacao p
				(id,
				 tramitacao_id,
				 protocolo_id,
				 protocolo_tid,
				 tipo_id,
				 tipo_texto,
				 objetivo_id,
				 objetivo_texto,
				 situacao_id,
				 situacao_texto,
				 despacho,
				 data_envio,
				 remetente_id,
				 remetente_tid,
				 remetente_setor_id,
				 remetente_setor_tid,
				 destinatario_id,
				 destinatario_tid,
				 destinatario_setor_id,
				 destinatario_setor_tid,
				 executor_id,
				 executor_tid,
				 executor_nome,
				 executor_login,
				 executor_tipo_id,
				 executor_tipo_texto,
				 acao_executada,
				 data_execucao,
				 tid)
			values
				(seq_hst_tramitacao.nextval,
				 i.id,
				 i.protocolo_id,
				 i.protocolo_tid,
				 i.tipo_id,
				 i.tipo_texto,
				 i.objetivo_id,
				 i.objetivo_texto,
				 i.situacao_id,
				 i.situacao_texto,
				 i.despacho,
				 i.data_envio,
				 i.remetente_id,
				 i.remetente_tid,
				 i.remetente_setor_id,
				 i.remetente_setor_tid,
				 i.destinatario_id,
				 i.destinatario_tid,
				 i.destinatario_setor_id,
				 i.destinatario_setor_tid,
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
					 and la.artefato = 14),
				 systimestamp,
				 i.tid)
			returning p.id into v_id;
		
			--Atualizar consulta
			lst_consulta.tramitacao(v_id);
			lst_consulta.protocolo(i.protocolo_id);
		
			----------------------------------------------------------------------------------------------------------------
			-- Tramitação/Externo
			----------------------------------------------------------------------------------------------------------------
			for j in (select te.id          externo_id,
							 te.tid         externo_tid,
							 tog.id         orgao_id,
							 tog.tid        orgao_tid,
							 te.funcionario
						from tab_tramitacao_externo te,
							 tab_tramitacao_orgao   tog
					   where te.orgao = tog.id
						 and te.tramitacao = i.id) loop
				insert into hst_tramitacao_externo
					(id,
					 id_hst,
					 tid,
					 externo_id,
					 externo_tid,
					 orgao_id,
					 orgao_tid,
					 tramitacao_id,
					 tramitacao_tid,
					 funcionario)
				values
					(seq_hst_tramit_ext.nextval,
					 v_id,
					 i.tid,
					 j.externo_id,
					 j.externo_tid,
					 j.orgao_id,
					 j.orgao_tid,
					 i.id,
					 i.tid,
					 j.funcionario);
			end loop;
		
			----------------------------------------------------------------------------------------------------------------
			-- Tramitação/Arquivar
			----------------------------------------------------------------------------------------------------------------
			for j in (select ta.id,
							 ta.tid,
							 a.id   arquivo_id,
							 a.tid  arquivo_tid,
							 e.id   estante_id,
							 e.tid  estante_tid,
							 p.id   prateleira_id,
							 p.tid  prateleira_tid
						from tab_tramitacao_arquivar       ta,
							 tab_tramitacao_arquivo        a,
							 tab_tramitacao_arq_estante    e,
							 tab_tramitacao_arq_prateleira p
					   where ta.arquivo = a.id
						 and ta.estante = e.id
						 and ta.prateleira = p.id
						 and ta.tramitacao = i.id) loop
				--
				insert into hst_tramitacao_arquivar
					(id,
					 id_hst,
					 tid,
					 arquivar_id,
					 arquivar_tid,
					 tramitacao_id,
					 tramitacao_tid,
					 arquivo_id,
					 arquivo_tid,
					 estante_id,
					 estante_tid,
					 prateleira_id,
					 prateleira_tid)
				values
					(seq_hst_tramit_arquivar.nextval,
					 v_id,
					 i.tid,
					 j.id,
					 i.tid,
					 i.id,
					 i.tid,
					 j.arquivo_id,
					 j.arquivo_tid,
					 j.estante_id,
					 j.estante_tid,
					 j.prateleira_id,
					 j.prateleira_tid);
			end loop;
		
		end loop;
		----------------------------------------------------------------------------------------------------------------
	
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Tramitação. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Tramitação. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
	---------------------------------------------------------

	---------------------------------------------------------
	-- Tramitação Arquivo
	---------------------------------------------------------
	procedure tramitacaoarquivo(p_id               number,
								p_acao             number,
								p_executor_id      number,
								p_executor_nome    varchar2,
								p_executor_login   varchar2,
								p_executor_tipo_id number,
								p_executor_tid     varchar2) is
		v_sucesso boolean := false;
		v_id      number;
		v_est_id  number;
	begin
	
		----------------------------------------------------------------------------------------------------------------
		-- Tramitação Arquivo
		----------------------------------------------------------------------------------------------------------------
		for i in (select a.id,
						 a.nome,
						 a.setor                   setor_id,
						 s.tid                     setor_tid,
						 a.tipo                    tipo_id,
						 lp.texto                  tipo_texto,
						 a.protocolo_ativ_situacao,
						 a.tid
					from tab_tramitacao_arquivo  a,
						 tab_setor               s,
						 lov_tramitacao_arq_tipo lp
				   where a.setor = s.id
					 and a.tipo = lp.id
					 and a.id = p_id) loop
		
			-- Inserindo na histórico
			insert into hst_tramitacao_arquivo p
				(id,
				 arquivo_id,
				 tid,
				 nome,
				 setor_id,
				 setor_tid,
				 tipo_id,
				 tipo_texto,
				 protocolo_ativ_situacao,
				 executor_id,
				 executor_tid,
				 executor_nome,
				 executor_login,
				 executor_tipo_id,
				 executor_tipo_texto,
				 acao_executada,
				 data_execucao)
			values
				(seq_hst_tramit_arq.nextval,
				 i.id,
				 i.tid,
				 i.nome,
				 i.setor_id,
				 i.setor_tid,
				 i.tipo_id,
				 i.tipo_texto,
				 i.protocolo_ativ_situacao,
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
					 and la.artefato = 15),
				 systimestamp)
			returning p.id into v_id;
		
			----------------------------------------------------------------------------------------------------------------
			-- Estantes
			----------------------------------------------------------------------------------------------------------------
			for j in (select id, arquivo, nome, tid
						from tab_tramitacao_arq_estante tr
					   where tr.arquivo = i.id) loop
				insert into hst_tramitacao_arq_estante te
					(id,
					 id_hst,
					 arquivo_est_id,
					 arquivo_id,
					 arquivo_tid,
					 nome,
					 tid)
				values
					(seq_hst_tramitacao_arq_esta.nextval,
					 v_id,
					 j.id,
					 i.id,
					 i.tid,
					 j.nome,
					 i.tid)
				returning te.id into v_est_id;
				----------------------------------------------------------------------------------------------------------------
				-- prateleira
				----------------------------------------------------------------------------------------------------------------
				insert into hst_tramitacao_arq_prateleira
					(id,
					 id_hst,
					 arquivo_pra_id,
					 arquivo_id,
					 arquivo_tid,
					 estante_id,
					 estante_tid,
					 modo_id,
					 modo_texto,
					 identificacao,
					 tid)
					(select seq_hst_tramitacao_arq_prat.nextval,
							seq_hst_tramitacao_arq_esta.currval,
							tr.id,
							i.id,
							i.tid,
							j.id,
							i.tid,
							lm.id,
							lm.texto,
							tr.identificacao,
							i.tid
					   from tab_tramitacao_arq_prateleira tr,
							lov_tramitacao_arq_modo       lm
					  where tr.modo = lm.id(+)
						and tr.estante = j.id
						and tr.arquivo = i.id);
			end loop;
		end loop;
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Tramitação de Arquivo. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Tramitação de Arquivo. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
	---------------------------------------------------------

	---------------------------------------------------------
	-- Setor
	---------------------------------------------------------
	procedure setor(p_id               number,
					p_acao             number,
					p_executor_id      number,
					p_executor_nome    varchar2,
					p_executor_login   varchar2,
					p_executor_tipo_id number,
					p_executor_tid     varchar2) is
		v_sucesso boolean := false;
		v_id      number;
	begin
	
		----------------------------------------------------------------------------------------------------------------
		-- Setor
		----------------------------------------------------------------------------------------------------------------
		for i in (select s.id,
						 s.nome,
						 s.sigla,
						 s.responsavel      responsavel_id,
						 f.tid              responsavel_tid,
						 s.tid,
						 s.unidade_convenio
					from tab_setor s, tab_funcionario f
				   where s.responsavel = f.id(+)
					 and s.id = p_id) loop
		
			-- Inserindo na histórico
			insert into hst_setor p
				(id,
				 setor_id,
				 tid,
				 nome,
				 sigla,
				 responsavel_id,
				 responsavel_tid,
				 executor_id,
				 executor_tid,
				 executor_nome,
				 executor_login,
				 executor_tipo_id,
				 executor_tipo_texto,
				 acao_executada,
				 data_execucao,
				 unidade_convenio)
			values
				(seq_hst_setor.nextval,
				 i.id,
				 i.tid,
				 i.nome,
				 i.sigla,
				 i.responsavel_id,
				 i.responsavel_tid,
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
					 and la.artefato = 6),
				 systimestamp,
				 i.unidade_convenio)
			returning p.id into v_id;
		
			----------------------------------------------------------------------------------------------------------------
			-- Setor/Endereço
			----------------------------------------------------------------------------------------------------------------
			for j in (select te.id,
							 setor,
							 te.cep,
							 te.distrito,
							 logradouro,
							 bairro,
							 le.id       estado_id,
							 le.texto    estado_texto,
							 lm.id       municipio_id,
							 lm.texto    municipio_texto,
							 numero,
							 complemento,
							 fone,
							 fone_fax,
							 te.tid
						from tab_setor_endereco te,
							 lov_estado         le,
							 lov_municipio      lm
					   where te.estado = le.id
						 and te.municipio = lm.id
						 and te.setor = p_id) loop
				insert into hst_setor_endereco
					(id,
					 id_hst,
					 endereco_id,
					 cep,
					 distrito,
					 logradouro,
					 bairro,
					 estado_id,
					 estado_texto,
					 municipio_id,
					 municipio_texto,
					 numero,
					 complemento,
					 fone,
					 fone_fax,
					 tid)
				values
					(seq_hst_setor_endereco.nextval,
					 v_id,
					 j.id,
					 j.cep,
					 j.distrito,
					 j.logradouro,
					 j.bairro,
					 j.estado_id,
					 j.estado_texto,
					 j.municipio_id,
					 j.municipio_texto,
					 j.numero,
					 j.complemento,
					 j.fone,
					 j.fone_fax,
					 i.tid);
			end loop;
		
		end loop;
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Setor. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Setor. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
	---------------------------------------------------------

	---------------------------------------------------------
	-- Configuração de Tramitação de Setor
	---------------------------------------------------------
	procedure tramitacaosetor(p_id               number,
							  p_acao             number,
							  p_executor_id      number,
							  p_executor_nome    varchar2,
							  p_executor_login   varchar2,
							  p_executor_tipo_id number,
							  p_executor_tid     varchar2) is
		v_sucesso boolean := false;
		v_id      number;
	begin
	
		----------------------------------------------------------------------------------------------------------------
		-- Configuração de Tramitação de Setor
		----------------------------------------------------------------------------------------------------------------
		for i in (select ts.id,
						 ts.setor setor_id,
						 s.tid    setor_tid,
						 ts.tipo  tipo_id,
						 lt.texto tipo_texto,
						 ts.tid
					from tab_tramitacao_setor ts,
						 tab_setor            s,
						 lov_tramitacao_tipo  lt
				   where ts.setor = s.id
					 and ts.tipo = lt.id
					 and ts.id = p_id) loop
		
			-- Inserindo na histórico
			insert into hst_tramitacao_setor p
				(id,
				 tid,
				 tramit_setor_id,
				 setor_id,
				 setor_tid,
				 tipo_id,
				 tipo_texto,
				 executor_id,
				 executor_tid,
				 executor_nome,
				 executor_login,
				 executor_tipo_id,
				 executor_tipo_texto,
				 acao_executada,
				 data_execucao)
			values
				(seq_hst_tramitacao_setor.nextval,
				 i.tid,
				 i.id,
				 i.setor_id,
				 i.setor_tid,
				 i.tipo_id,
				 i.tipo_texto,
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
					 and la.artefato = 16),
				 systimestamp)
			returning p.id into v_id;
		
			----------------------------------------------------------------------------------------------------------------
			-- Funcionários
			----------------------------------------------------------------------------------------------------------------
			for j in (select c.id,
							 c.setor,
							 c.funcionario funcionario_id,
							 f.tid         funcionario_tid,
							 c.tid
						from tab_tramitacao_setor_func c, tab_funcionario f
					   where c.funcionario = f.id
						 and c.setor = i.id) loop
				insert into hst_tramitacao_setor_func
					(id,
					 id_hst,
					 setor_func_id,
					 setor_id,
					 setor_tid,
					 funcionario_id,
					 funcionario_tid,
					 tid)
				values
					(seq_hst_tramitacao_setor_func.nextval,
					 v_id,
					 j.id,
					 i.id,
					 i.setor_tid,
					 j.funcionario_id,
					 j.funcionario_tid,
					 i.tid);
			end loop;
		
		end loop;
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de configuração de tramitação de setor. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de configuração de tramitação de setor. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
	---------------------------------------------------------

	---------------------------------------------------------
	-- Papel
	---------------------------------------------------------
	procedure papel(p_id               number,
					p_acao             number,
					p_executor_id      number,
					p_executor_nome    varchar2,
					p_executor_login   varchar2,
					p_executor_tipo_id number,
					p_executor_tid     varchar2) is
		v_sucesso boolean := false;
		v_id      number;
	begin
	
		----------------------------------------------------------------------------------------------------------------
	
		----------------------------------------------------------------------------------------------------------------
		-- Papel
		----------------------------------------------------------------------------------------------------------------
		for i in (select tap.id,
						 tap.nome,
						 tap.funcionario_tipo funcionario_tipo_id,
						 lft.texto            funcionario_tipo_texto,
						 tap.tid
					from tab_autenticacao_papel tap,
						 lov_funcionario_tipo   lft
				   where tap.funcionario_tipo = lft.id
					 and tap.id = p_id) loop
		
			-- Inserindo na histórico
			insert into hst_autenticacao_papel
				(id,
				 papel_id,
				 tid,
				 nome,
				 funcionario_tipo_id,
				 funcionario_tipo_texto,
				 executor_id,
				 executor_tid,
				 executor_nome,
				 executor_login,
				 executor_tipo_id,
				 executor_tipo_texto,
				 acao_executada,
				 data_execucao)
			values
				(seq_hst_autenticacao_papel.nextval,
				 i.id,
				 i.tid,
				 i.nome,
				 i.funcionario_tipo_id,
				 i.funcionario_tipo_texto,
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
					 and la.artefato = 27),
				 systimestamp)
			returning id into v_id;
		
			----------------------------------------------------------------------------------------------------------------
			-- Permissao
			----------------------------------------------------------------------------------------------------------------
			for j in (select tapp.id        papel_perm_id,
							 tapp.papel     papel_id,
							 tapp.permissao permissao_id,
							 lap.nome       permissao_texto,
							 tapp.tid
						from tab_autenticacao_papel_perm tapp,
							 lov_autenticacao_permissao  lap
					   where tapp.permissao = lap.id
						 and tapp.papel = p_id) loop
				insert into hst_autenticacao_papel_perm
					(id,
					 id_hst,
					 papel_perm_id,
					 papel_id,
					 permissao_id,
					 permissao_texto,
					 tid)
				values
					(seq_hst_autenticacao_papel_per.nextval,
					 v_id,
					 j.papel_perm_id,
					 j.papel_id,
					 j.permissao_id,
					 j.permissao_texto,
					 i.tid);
			end loop;
		
		end loop;
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Papel. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Papel. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;

	---------------------------------------------------------
	-- Modelo de Título
	---------------------------------------------------------
	procedure titulomodelo(p_id               number,
						   p_acao             number,
						   p_executor_id      number,
						   p_executor_nome    varchar2,
						   p_executor_login   varchar2,
						   p_executor_tipo_id number,
						   p_executor_tid     varchar2) is
		v_sucesso boolean := false;
		v_id      number;
		v_id_aux  number;
	begin
	
		----------------------------------------------------------------------------------------------------------------
	
		----------------------------------------------------------------------------------------------------------------
		-- Modelo de Título
		----------------------------------------------------------------------------------------------------------------
		for i in (select m.id,
						 m.codigo,
						 m.tipo         tipo_id,
						 lt.texto       tipo_texto,
						 ls.id          situacao_id,
						 ls.texto       situacao_texto,
						 m.subtipo,
						 m.data_criacao,
						 m.nome,
						 m.sigla,
						 lp.id          tipo_protocolo_id,
						 lp.texto       tipo_protocolo_texto,
						 m.arquivo      arquivo_id,
						 a.tid          arquivo_tid,
						 m.tabela,
						 m.tid,
						 m.documento,
						 ltd.texto      tipo_doc_texto
					from tab_titulo_modelo           m,
						 tab_arquivo                 a,
						 lov_titulo_modelo_tipo      lt,
						 lov_titulo_modelo_prot_tipo lp,
						 lov_titulo_modelo_situacao  ls,
						 lov_titulo_modelo_tipo_doc  ltd
				   where m.arquivo = a.id(+)
					 and m.tipo = lt.id
					 and m.tipo_protocolo = lp.id
					 and m.situacao = ls.id
					 and m.documento = ltd.id
					 and m.id = p_id) loop
		
			-- Inserindo na histórico
			insert into hst_titulo_modelo m
				(id,
				 modelo_id,
				 tid,
				 codigo,
				 tipo_id,
				 tipo_texto,
				 situacao_id,
				 situacao_texto,
				 subtipo,
				 data_criacao,
				 nome,
				 sigla,
				 tipo_protocolo_id,
				 tipo_protocolo_texto,
				 arquivo_id,
				 arquivo_tid,
				 tabela,
				 executor_id,
				 executor_tid,
				 executor_nome,
				 executor_login,
				 executor_tipo_id,
				 executor_tipo_texto,
				 acao_executada,
				 data_execucao,
				 documento_id,
				 documento_texto)
			
			values
				(seq_hst_titulo_modelo.nextval,
				 i.id,
				 i.tid,
				 i.codigo,
				 i.tipo_id,
				 i.tipo_texto,
				 i.situacao_id,
				 i.situacao_texto,
				 i.subtipo,
				 i.data_criacao,
				 i.nome,
				 i.sigla,
				 i.tipo_protocolo_id,
				 i.tipo_protocolo_texto,
				 i.arquivo_id,
				 i.arquivo_tid,
				 i.tabela,
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
					 and la.artefato = 17),
				 systimestamp,
				 i.documento,
				 i.tipo_doc_texto)
			returning m.id into v_id;
		
			----------------------------------------------------------------------------------------------------------------
			-- Regras
			----------------------------------------------------------------------------------------------------------------
			for j in (select a.id,
							 a.tid,
							 lr.id    regra_id,
							 lr.texto regra_texto
						from tab_titulo_modelo_regras a,
							 lov_titulo_modelo_regras lr
					   where a.regra = lr.id
						 and a.modelo = i.id) loop
			
				insert into hst_titulo_modelo_regras e
					(id,
					 id_hst,
					 tid,
					 id_regra,
					 regra_id,
					 regra_texto,
					 modelo_id,
					 modelo_tid)
				values
					(seq_hst_titulo_modelo_regras.nextval,
					 v_id,
					 i.tid,
					 j.id,
					 j.regra_id,
					 j.regra_texto,
					 i.id,
					 i.tid)
				returning e.id into v_id_aux;
			
				----------------------------------------------------------------------------------------------------------------
				-- Respostas
				----------------------------------------------------------------------------------------------------------------
				for k in (select e.id,
								 lr.id    resposta_id,
								 lr.texto resposta_texto,
								 e.valor,
								 e.tid
							from tab_titulo_modelo_respostas e,
								 lov_titulo_modelo_respostas lr
						   where e.resposta = lr.id
							 and e.regra = j.id) loop
					--
					insert into hst_titulo_modelo_respostas mr
						(id,
						 id_hst,
						 tid,
						 id_resposta,
						 resposta_id,
						 resposta_texto,
						 modelo_id,
						 modelo_tid,
						 regra_id,
						 regra_tid,
						 valor)
					values
						(seq_hst_titulo_modelo_respost.nextval,
						 v_id_aux,
						 i.tid,
						 k.id,
						 k.resposta_id,
						 k.resposta_texto,
						 i.id,
						 i.tid,
						 j.id,
						 i.tid,
						 k.valor);
				end loop;
			end loop;
		
			----------------------------------------------------------------------------------------------------------------
			-- Setores
			----------------------------------------------------------------------------------------------------------------
			--
			insert into hst_titulo_modelo_setores e
				(id,
				 id_hst,
				 tid,
				 id_setor,
				 setor_id,
				 setor_tid,
				 hierarquia,
				 modelo_id,
				 modelo_tid)
				(select seq_hst_titulo_modelo_setores.nextval,
						v_id,
						i.tid,
						a.id,
						s.id,
						s.tid,
						a.hierarquia,
						i.id,
						i.tid
				   from tab_titulo_modelo_setores a, tab_setor s
				  where a.setor = s.id
					and a.modelo = i.id);
		
			----------------------------------------------------------------------------------------------------------------
			-- Assinantes
			----------------------------------------------------------------------------------------------------------------
			--
			insert into hst_titulo_modelo_assinantes e
				(id,
				 id_hst,
				 tid,
				 assinantes_id,
				 modelo_id,
				 modelo_tid,
				 setor_id,
				 setor_tid,
				 tipo_assinante_id,
				 tipo_assinante_texto)
				(select seq_hst_titulo_modelo_assina.nextval,
						v_id,
						i.tid,
						a.id,
						i.id,
						i.tid,
						s.id,
						s.tid,
						la.id,
						la.texto
				   from tab_titulo_modelo_assinantes a,
						tab_setor                    s,
						lov_titulo_modelo_assinante  la
				  where a.setor = s.id
					and a.tipo_assinante = la.id
					and a.modelo = i.id);
		
		end loop;
		----------------------------------------------------------------------------------------------------------------
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Modelo de Título. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Modelo de Título. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
	---------------------------------------------------------

	---------------------------------------------------------
	-- Título
	---------------------------------------------------------
	procedure titulo(p_id               number,
					 p_acao             number,
					 p_executor_id      number,
					 p_executor_nome    varchar2,
					 p_executor_login   varchar2,
					 p_executor_tipo_id number,
					 p_executor_tid     varchar2) is
		v_sucesso boolean := false;
		v_id      number;
	begin
		----------------------------------------------------------------------------------------------------------------
	
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
						 t.data_situacao
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
				 data_situacao)
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
				 i.data_situacao)
			returning m.id into v_id;
		
			----------------------------------------------------------------------------------------------------------------
			-- Número
			----------------------------------------------------------------------------------------------------------------
			--
			insert into hst_titulo_numero e
				(id,
				 id_hst,
				 tid,
				 id_numero,
				 numero,
				 ano,
				 modelo_id,
				 modelo_tid,
				 titulo_id,
				 titulo_tid)
				(select seq_hst_titulo_numero.nextval,
						v_id,
						i.tid,
						a.id,
						a.numero,
						a.ano,
						a.modelo,
						m.tid,
						i.id,
						i.tid
				   from tab_titulo_numero a, tab_titulo_modelo m
				  where a.modelo = m.id
					and a.titulo = i.id);
		
			----------------------------------------------------------------------------------------------------------------
			-- Setores
			----------------------------------------------------------------------------------------------------------------
			--
			insert into hst_titulo_setores e
				(id,
				 id_hst,
				 tid,
				 id_setor,
				 setor_id,
				 setor_tid,
				 titulo_id,
				 titulo_tid)
				(select seq_hst_titulo_setores.nextval,
						v_id,
						i.tid,
						a.id,
						s.id,
						s.tid,
						i.id,
						i.tid
				   from tab_titulo_setores a, tab_setor s
				  where a.setor = s.id
					and a.titulo = i.id);
		
			----------------------------------------------------------------------------------------------------------------
			-- Assinantes
			----------------------------------------------------------------------------------------------------------------
			--
			insert into hst_titulo_assinantes e
				(id,
				 id_hst,
				 tid,
				 assinantes_id,
				 titulo_id,
				 titulo_tid,
				 funcionario_id,
				 funcionario_tid,
				 cargo_id,
				 cargo_tid)
				(select seq_hst_titulo_assinantes.nextval,
						v_id,
						i.tid,
						a.id,
						i.id,
						i.tid,
						s.id,
						s.tid,
						g.id,
						g.tid
				   from tab_titulo_assinantes a,
						tab_funcionario       s,
						tab_cargo             g
				  where a.cargo = g.id(+)
					and a.funcionario = s.id
					and a.titulo = i.id);
		
			----------------------------------------------------------------------------------------------------------------
			-- Título Associado
			----------------------------------------------------------------------------------------------------------------
			--
			insert into hst_titulo_associados e
				(id,
				 id_hst,
				 tid,
				 id_anterior,
				 associado_id,
				 associado_tid,
				 titulo_id,
				 titulo_tid)
				(select seq_hst_titulo_associados.nextval,
						v_id,
						i.tid,
						a.id,
						m.id,
						m.tid,
						i.id,
						i.tid
				   from tab_titulo_associados a, tab_titulo m
				  where a.associado_id = m.id
					and a.titulo = i.id);
		
			----------------------------------------------------------------------------------------------------------------
			-- Atividade
			----------------------------------------------------------------------------------------------------------------
			--
			insert into hst_titulo_atividades e
				(id,
				 id_hst,
				 tid,
				 id_atividade,
				 atividade_id,
				 atividade_tid,
				 titulo_id,
				 titulo_tid,
				 protocolo_id,
				 protocolo_tid)
				(select seq_hst_titulo_atividades.nextval,
						v_id,
						i.tid,
						a.id,
						a.atividade,
						m.tid,
						i.id,
						i.tid,
						p.id,
						p.tid
				   from tab_titulo_atividades a,
						tab_atividade         m,
						tab_protocolo         p
				  where a.atividade = m.id
					and a.protocolo = p.id(+)
					and a.titulo = i.id);
		
			----------------------------------------------------------------------------------------------------------------
			-- Pessoas
			----------------------------------------------------------------------------------------------------------------
			--
			insert into hst_titulo_pessoas e
				(id,
				 id_hst,
				 tid,
				 id_pessoa,
				 pessoa_id,
				 pessoa_tid,
				 titulo_id,
				 titulo_tid,
				 tipo)
				(select seq_hst_titulo_pessoas.nextval,
						v_id,
						i.tid,
						a.id,
						a.pessoa,
						m.tid,
						i.id,
						i.tid,
						a.tipo
				   from tab_titulo_pessoas a, tab_pessoa m
				  where a.pessoa = m.id
					and a.titulo = i.id);
		
			----------------------------------------------------------------------------------------------------------------
			-- Arquivos
			----------------------------------------------------------------------------------------------------------------
			--
			insert into hst_titulo_arquivo e
				(id,
				 id_hst,
				 titulo_arq_id,
				 titulo_id,
				 titulo_tid,
				 arquivo_id,
				 arquivo_tid,
				 ordem,
				 descricao,
				 tid)
				(select seq_hst_titulo_arquivo.nextval,
						v_id,
						a.id,
						i.id,
						i.tid,
						m.id,
						m.tid,
						a.ordem,
						a.descricao,
						i.tid
				   from tab_titulo_arquivo a, tab_arquivo m
				  where a.arquivo = m.id
					and a.titulo = i.id);
		
		end loop;
		----------------------------------------------------------------------------------------------------------------
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Título. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Título. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
	---------------------------------------------------------

	---------------------------------------------------------
	-- Condicionante de Título
	---------------------------------------------------------
	procedure titulocondicionante(p_id               number,
								  p_acao             number,
								  p_executor_id      number,
								  p_executor_nome    varchar2,
								  p_executor_login   varchar2,
								  p_executor_tipo_id number,
								  p_executor_tid     varchar2) is
		v_sucesso boolean := false;
		v_id      number;
	begin
		----------------------------------------------------------------------------------------------------------------
	
		----------------------------------------------------------------------------------------------------------------
		-- Condicionante de Título
		----------------------------------------------------------------------------------------------------------------
		for i in (select c.id,
						 t.id               titulo_id,
						 t.tid              titulo_tid,
						 ls.id              situacao_id,
						 ls.texto           situacao_texto,
						 c.descricao,
						 c.possui_prazo,
						 c.prazo,
						 c.dias_prorrogados,
						 c.periodicidade,
						 c.periodo,
						 lp.id              periodo_tipo_id,
						 lp.texto           periodo_tipo_texto,
						 c.data_inicio,
						 c.data_vencimento,
						 c.ordem,
						 c.tid
					from tab_titulo_condicionantes    c,
						 tab_titulo                   t,
						 lov_titulo_cond_situacao     ls,
						 lov_titulo_cond_periodo_tipo lp
				   where c.titulo = t.id
					 and c.situacao = ls.id
					 and c.periodo_tipo = lp.id(+)
					 and c.id = p_id) loop
		
			-- Inserindo na histórico
			insert into hst_titulo_condicionantes c
				(id,
				 condicionante_id,
				 tid,
				 titulo_id,
				 titulo_tid,
				 situacao_id,
				 situacao_texto,
				 descricao,
				 possui_prazo,
				 prazo,
				 dias_prorrogados,
				 periodicidade,
				 periodo,
				 periodo_tipo_id,
				 periodo_tipo_texto,
				 data_inicio,
				 data_vencimento,
				 ordem,
				 executor_id,
				 executor_tid,
				 executor_nome,
				 executor_login,
				 executor_tipo_id,
				 executor_tipo_texto,
				 acao_executada,
				 data_execucao)
			values
				(seq_hst_titulo_condiciona.nextval,
				 i.id,
				 i.tid,
				 i.titulo_id,
				 i.titulo_tid,
				 i.situacao_id,
				 i.situacao_texto,
				 i.descricao,
				 i.possui_prazo,
				 i.prazo,
				 i.dias_prorrogados,
				 i.periodicidade,
				 i.periodo,
				 i.periodo_tipo_id,
				 i.periodo_tipo_texto,
				 i.data_inicio,
				 i.data_vencimento,
				 i.ordem,
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
					 and la.artefato = 19),
				 systimestamp)
			returning c.id into v_id;
		
			----------------------------------------------------------------------------------------------------------------
			-- Condicionante de Título/Periodicidade
			----------------------------------------------------------------------------------------------------------------
			--
			insert into hst_titulo_condicionantes_peri e
				(id,
				 id_hst,
				 tid,
				 periodicidade_id,
				 titulo_id,
				 titulo_tid,
				 condicionante_id,
				 condicionante_tid,
				 situacao_id,
				 situacao_texto,
				 dias_prorrogados,
				 data_inicio,
				 data_vencimento)
				(select seq_hst_titulo_cond_per.nextval,
						v_id,
						i.tid,
						a.id,
						i.titulo_id,
						i.titulo_tid,
						i.id,
						i.tid,
						a.situacao,
						ls.texto,
						a.dias_prorrogados,
						a.data_inicio,
						a.data_vencimento
				   from tab_titulo_condicionantes_peri a,
						lov_titulo_cond_situacao       ls
				  where a.situacao = ls.id
					and a.condicionante = i.id);
		
		end loop;
		----------------------------------------------------------------------------------------------------------------
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Condicionantes de Título. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Condicionantes de Título. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
	---------------------------------------------------------

	---------------------------------------------------------
	-- Descrição de Condicionante de Título
	---------------------------------------------------------
	procedure titulocondicionantedescricao(p_id               number,
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
		-- Descrição de Condicionante de Título
		----------------------------------------------------------------------------------------------------------------
		for i in (select c.id, c.descricao, c.tid
					from tab_titulo_condicionante_desc c
				   where c.id = p_id) loop
		
			-- Inserindo na histórico
			insert into hst_titulo_condicionante_desc c
				(id,
				 descricao_id,
				 tid,
				 descricao,
				 executor_id,
				 executor_tid,
				 executor_nome,
				 executor_login,
				 executor_tipo_id,
				 executor_tipo_texto,
				 acao_executada,
				 data_execucao)
			values
				(seq_hst_titulo_cond_desc.nextval,
				 i.id,
				 i.tid,
				 i.descricao,
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
					 and la.artefato = 20),
				 systimestamp);
		
		end loop;
		----------------------------------------------------------------------------------------------------------------
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Descrição de Condicionantes de Título. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Descrição de Condicionantes de Título. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
	---------------------------------------------------------

	---------------------------------------------------------
	-- Entrega de Título
	---------------------------------------------------------
	procedure tituloentrega(p_id               number,
							p_acao             number,
							p_executor_id      number,
							p_executor_nome    varchar2,
							p_executor_login   varchar2,
							p_executor_tipo_id number,
							p_executor_tid     varchar2) is
		v_sucesso boolean := false;
		v_id      number;
	begin
		----------------------------------------------------------------------------------------------------------------
	
		----------------------------------------------------------------------------------------------------------------
		-- Entrega de Título
		----------------------------------------------------------------------------------------------------------------
		for i in (select t.id,
						 t.protocolo    protocolo_id,
						 p.tid          protocolo_tid,
						 t.pessoa       pessoa_id,
						 p1.tid         pessoa_tid,
						 t.nome,
						 t.cpf,
						 t.data_entrega,
						 t.tid
					from tab_titulo_entrega t,
						 tab_pessoa         p1,
						 tab_protocolo      p
				   where t.protocolo = p.id(+)
					 and t.pessoa = p1.id(+)
					 and t.id = p_id) loop
		
			-- Inserindo na histórico
			insert into hst_titulo_entrega m
				(id,
				 entrega_id,
				 tid,
				 protocolo_id,
				 protocolo_tid,
				 pessoa_id,
				 pessoa_tid,
				 nome,
				 cpf,
				 data_entrega,
				 executor_id,
				 executor_tid,
				 executor_nome,
				 executor_login,
				 executor_tipo_id,
				 executor_tipo_texto,
				 acao_executada,
				 data_execucao)
			values
				(seq_hst_titulo_entrega.nextval,
				 i.id,
				 i.tid,
				 i.protocolo_id,
				 i.protocolo_tid,
				 i.pessoa_id,
				 i.pessoa_tid,
				 i.nome,
				 i.cpf,
				 i.data_entrega,
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
					 and la.artefato = 21),
				 systimestamp)
			returning m.id into v_id;
		
			----------------------------------------------------------------------------------------------------------------
			-- Títulos
			----------------------------------------------------------------------------------------------------------------
			--
			insert into hst_titulo_entrega_titulos e
				(id,
				 id_hst,
				 tid,
				 entrega_titulo_id,
				 entrega_id,
				 entrega_tid,
				 titulo_id,
				 titulo_tid)
				(select seq_hst_titulo_entre_tit.nextval,
						v_id,
						i.tid,
						a.id,
						i.id,
						i.tid,
						a.titulo,
						t.tid
				   from tab_titulo_entrega_titulos a, tab_titulo t
				  where a.titulo = t.id
					and a.entrega = i.id);
		
		end loop;
		----------------------------------------------------------------------------------------------------------------
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Entrega de Título. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Entrega de Título. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
	---------------------------------------------------------

	---------------------------------------------------------
	-- Configuração de Atividades
	---------------------------------------------------------
	procedure cnfatividades(p_id               number,
							p_acao             number,
							p_executor_id      number,
							p_executor_nome    varchar2,
							p_executor_login   varchar2,
							p_executor_tipo_id number,
							p_executor_tid     varchar2) is
		v_sucesso boolean := false;
		v_id      number;
	begin
		----------------------------------------------------------------------------------------------------------------
	
		----------------------------------------------------------------------------------------------------------------
		-- Configuração de Atividades
		----------------------------------------------------------------------------------------------------------------
		for i in (select a.id, a.nome, a.tid
					from cnf_atividade a
				   where a.id = p_id) loop
		
			-- Inserindo na histórico
			insert into hst_cnf_atividade a
				(id,
				 configuracao_id,
				 tid,
				 nome,
				 executor_id,
				 executor_tid,
				 executor_nome,
				 executor_login,
				 executor_tipo_id,
				 executor_tipo_texto,
				 acao_executada,
				 data_execucao)
			values
				(seq_hst_cnf_atividade.nextval,
				 i.id,
				 i.tid,
				 i.nome,
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
					 and la.artefato = 22),
				 systimestamp)
			returning a.id into v_id;
		
			----------------------------------------------------------------------------------------------------------------
			-- Modelos
			----------------------------------------------------------------------------------------------------------------
			--
			insert into hst_cnf_atividade_modelos e
				(id,
				 id_hst,
				 tid,
				 atividade_modelo_id,
				 configuracao_id,
				 configuracao_tid,
				 modelo_id,
				 modelo_tid)
				(select seq_hst_cnf_ativ_modelo.nextval,
						v_id,
						i.tid,
						a.id,
						i.id,
						i.tid,
						a.modelo,
						t.tid
				   from cnf_atividade_modelos a, tab_titulo_modelo t
				  where a.modelo = t.id
					and a.configuracao = i.id);
		
			----------------------------------------------------------------------------------------------------------------
			-- Atividades
			----------------------------------------------------------------------------------------------------------------
			--
			insert into hst_cnf_atividade_atividades e
				(id,
				 id_hst,
				 tid,
				 atividade_ativ_id,
				 configuracao_id,
				 configuracao_tid,
				 atividade_id,
				 atividade_tid)
				(select seq_hst_cnf_ativ_ativ.nextval,
						v_id,
						i.tid,
						a.id,
						i.id,
						i.tid,
						a.atividade,
						t.tid
				   from cnf_atividade_atividades a, tab_atividade t
				  where a.atividade = t.id
					and a.configuracao = i.id);
		
		end loop;
		----------------------------------------------------------------------------------------------------------------
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Configuração de Atividade. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Configuração de Atividade. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
	---------------------------------------------------------

	---------------------------------------------------------
	-- Configuração de Motivo de  Tramitação
	---------------------------------------------------------
	procedure tramitacaomotivo(p_id               number,
							   p_acao             number,
							   p_executor_id      number,
							   p_executor_nome    varchar2,
							   p_executor_login   varchar2,
							   p_executor_tipo_id number,
							   p_executor_tid     varchar2) is
		v_sucesso boolean := false;
		v_id      number;
	begin
	
		----------------------------------------------------------------------------------------------------------------
		-- Configuração de Motivo de Tramitação
		----------------------------------------------------------------------------------------------------------------
		for i in (select t.id, t.texto, t.tid, t.ativo
					from tab_tramitacao_motivo t
				   where t.id = p_id) loop
		
			-- Inserindo na histórico
			insert into hst_tramitacao_motivo p
				(id,
				 motivo_id,
				 motivo_texto,
				 tid,
				 ativo,
				 executor_id,
				 executor_tid,
				 executor_nome,
				 executor_login,
				 executor_tipo_id,
				 executor_tipo_texto,
				 acao_executada,
				 data_execucao)
			values
				(seq_hst_tramit_motivo.nextval,
				 i.id,
				 i.texto,
				 i.tid,
				 i.ativo,
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
					 and la.artefato = 31),
				 systimestamp)
			returning p.id into v_id;
		
		end loop;
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de configuração motivo de tramitação: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de configuração motivo de tramitação. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
	---------------------------------------------------------   

	---------------------------------------------------------
	-- Peça Técnica
	---------------------------------------------------------
	procedure pecatecnica(p_id               number,
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
		-- Peça Técnica
		----------------------------------------------------------------------------------------------------------------
		for i in (select seq_hst_peca_tecnica.nextval hst_peca_tecnica_id,
						 t.id,
						 t.tid,
						 t.protocolo,
						 p.tid                        protocolo_tid,
						 t.atividade,
						 a.tid                        atividade_tid,
						 t.elaborador_tipo,
						 et.texto                     elaborador_tipo_texto,
						 t.elaborador_pessoa,
						 e.tid                        elaborador_pessoa_tid,
						 t.elaborador_tecnico,
						 f.tid                        elaborador_tecnico_tid,
						 t.setor_cadastro,
						 s.tid                        setor_tid
					from tab_peca_tecnica             t,
						 tab_protocolo                p,
						 tab_protocolo_atividades     a,
						 tab_funcionario              f,
						 lov_peca_tec_elaborador_tipo et,
						 tab_pessoa                   e,
						 tab_setor                    s
				   where t.protocolo = p.id
					 and t.atividade = a.id
					 and t.elaborador_tipo = et.id
					 and t.elaborador_tecnico = f.id(+)
					 and t.elaborador_pessoa = e.id(+)
					 and t.setor_cadastro = s.id(+)
					 and t.id = p_id) loop
		
			-- Inserindo no histórico
			insert into hst_peca_tecnica p
				(id,
				 peca_tecnica_id,
				 tid,
				 protocolo_id,
				 protocolo_tid,
				 atividade_id,
				 atividade_tid,
				 elaborador_tipo_id,
				 elaborador_tipo_texto,
				 elaborador_pessoa_id,
				 elaborador_pessoa_tid,
				 elaborador_tecnico_id,
				 elaborador_tecnico_tid,
				 setor_cadastro_id,
				 setor_cadastro_tid,
				 executor_id,
				 executor_tid,
				 executor_nome,
				 executor_login,
				 executor_tipo_id,
				 executor_tipo_texto,
				 acao_executada,
				 data_execucao)
			values
				(i.hst_peca_tecnica_id,
				 i.id,
				 i.tid,
				 i.protocolo,
				 i.protocolo_tid,
				 i.atividade,
				 i.atividade_tid,
				 i.elaborador_tipo,
				 i.elaborador_tipo_texto,
				 i.elaborador_pessoa,
				 i.elaborador_pessoa_tid,
				 i.elaborador_tecnico,
				 i.elaborador_tecnico_tid,
				 i.setor_cadastro,
				 i.setor_tid,
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
					 and la.artefato = 39),
				 systimestamp);
		
			insert into hst_peca_tecnica_dest
				(id,
				 id_hst,
				 peca_tecnica_dest_id,
				 peca_tecnica_id,
				 peca_tecnica_tid,
				 destinatario_id,
				 destinatario_tid,
				 tid)
				(select seq_hst_peca_tecnica_dest.nextval,
						i.hst_peca_tecnica_id,
						d.id,
						i.id,
						i.tid,
						d.destinatario,
						(select p.tid
						   from tab_pessoa p
						  where p.id = d.destinatario),
						i.tid
				   from tab_peca_tecnica_dest d
				  where d.peca_tecnica = i.id);
		
		end loop;
		----------------------------------------------------------------------------------------------------------------
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Peça técnica. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Peça técnica. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
	---------------------------------------------------------

	
	---------------------------------------------------------
	-- Fiscalização 
	---------------------------------------------------------
	procedure fiscalizacao(p_id               number,
						   p_acao             number,
						   p_executor_id      number,
						   p_executor_nome    varchar2,
						   p_executor_login   varchar2,
						   p_executor_tipo_id number,
						   p_executor_tid     varchar2) is
		v_sucesso boolean := false;
		v_tid     varchar2(36) := '';
	begin
		---------------------------------------------------------
		-- Fiscalização
		---------------------------------------------------------
		select t.tid into v_tid from tab_fiscalizacao t where t.id = p_id;
	
		insert into hst_fiscalizacao
			(id,
			 fiscalizacao_id,
			 situacao_id,
			 situacao_texto,
			 motivo,
			 situacao_data,
			 situacao_anterior_id,
			 situacao_anterior_texto,
			 situacao_data_anterior,
			 autuante_id,
			 autuante_tid,
			 autos,
			 vencimento,
			 pdf_auto_termo,
			 pdf_laudo,
			 pdf_croqui,
       pdf_iuf,
       possui_projeto_geo,
			 tid,
			 executor_id,
			 executor_tid,
			 executor_nome,
			 executor_login,
			 executor_tipo_id,
			 executor_tipo_texto,
			 acao_executada,
			 data_execucao)
			(select seq_hst_fiscalizacao.nextval,
					t.id,
					t.situacao,
					ls.texto,
					t.motivo,
					t.situacao_data,
					t.situacao_anterior,
					lst.texto,
					t.situacao_data_anterior,
					t.autuante,
					tf.tid,
					t.autos,
					t.vencimento,
					t.pdf_auto_termo,
					t.pdf_laudo,
					t.pdf_croqui,
          t.pdf_iuf,
          t.possui_projeto_geo,
					v_tid,
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
						and la.artefato = 81),
					systimestamp
			   from tab_fiscalizacao          t,
					lov_fiscalizacao_situacao ls,
					lov_fiscalizacao_situacao lst,
					tab_funcionario           tf
			  where t.situacao = ls.id(+)
				and t.situacao_anterior = lst.id(+)
				and t.autuante = tf.id(+)
				and t.id = p_id);
		---------------------------------------------------------
		---------------------------------------------------------
		-- Fiscalização - Local da Infração
		---------------------------------------------------------
		insert into hst_fisc_local_infracao
			(id,
			 id_hst,
			 local_infracao_id,
			 fiscalizacao_id,
			 setor_id,
			 setor_sigla,
			 setor_texto,
       area_fiscalizacao,
			 sis_coord_id,
			 sis_coord_texto,
			 datum_id,
			 datum_texto,
			 fuso,
			 area_abrang,
			 lat_northing,
			 lon_easting,
			 hemisferio_id,
			 hemisferio_texto,
			 municipio_id,
			 municipio_texto,
			 pessoa_id,
			 pessoa_tid,
			 empreendimento_id,
			 empreendimento_tid,
			 responsavel_id,
			 responsavel_tid,
			 resp_propriedade_id,
			 resp_propriedade_tid,
			 tid,
			 data,
			 local)
			(select seq_hst_fisc_local_infracao.nextval,
					seq_hst_fiscalizacao.currval,
					t.id,
					t.fiscalizacao,
					t.setor,
					ts.sigla,
					ts.nome,
          t.area_fiscalizacao,
					t.sis_coord,
					lct.texto,
					t.datum,
					lcd.texto,
					t.fuso,
					t.area_abrang,
					t.lat_northing,
					t.lon_easting,
					t.hemisferio,
					lch.texto,
					t.municipio,
					lm.texto,
					t.pessoa,
					tb.tid,
					t.empreendimento,
					te.tid,
					t.responsavel,
					tba.tid,
					t.resp_propriedade,
					tpp.tid,
					v_tid,
					t.data,
					t.local
			   from tab_fisc_local_infracao   t,
					tab_setor                 ts,
					lov_coordenada_tipo       lct,
					lov_coordenada_datum      lcd,
					lov_coordenada_hemisferio lch,
					lov_municipio             lm,
					tab_pessoa                tb,
					tab_empreendimento        te,
					tab_pessoa                tba,
					tab_pessoa                tpp
			  where t.setor = ts.id(+)
				and t.sis_coord = lct.id(+)
				and t.datum = lcd.id(+)
				and t.hemisferio = lch.id(+)
				and t.municipio = lm.id(+)
				and t.pessoa = tb.id(+)
				and t.empreendimento = te.id(+)
				and t.responsavel = tba.id(+)
				and t.resp_propriedade = tpp.id(+)
				and t.fiscalizacao = p_id);
		---------------------------------------------------------  
	
		---------------------------------------------------------
		-- Fiscalização - Projeto Geografico
		--------------------------------------------------------- 
		for i in (select f.id,
						 f.fiscalizacao,
						 f.situacao             situacao_id,
						 s.texto                situacao_texto,
						 f.nivel_precisao       nivel_precisao_id,
						 n.texto                nivel_precisao_texto,
						 f.mecanismo_elaboracao mecanismo_elaboracao_id,
						 m.texto                mecanismo_elaboracao_texto,
						 f.menor_x,
						 f.menor_y,
						 f.maior_x,
						 f.maior_y,
						 f.tid
					from tab_fisc_prj_geo              f,
						 lov_crt_projeto_geo_nivel     n,
						 lov_crt_projeto_geo_mecanismo m,
						 lov_crt_projeto_geo_situacao  s
				   where f.fiscalizacao = p_id
					 and f.situacao = s.id
					 and f.nivel_precisao = n.id(+)
					 and f.mecanismo_elaboracao = m.id(+)) loop
		
			insert into hst_fisc_prj_geo
				(id,
				 fiscalizacao_id_hst,
				 fiscalizacao_id,
				 fiscalizacao_tid,
				 projeto_id,
				 projeto_tid,
				 situacao_id,
				 situacao_texto,
				 nivel_precisao_id,
				 nivel_precisao_texto,
				 mecanismo_elaboracao_id,
				 mecanismo_elaboracao_texto,
				 menor_x,
				 menor_y,
				 maior_x,
				 maior_y)
			values
				(seq_hst_fisc_prj_geo.nextval,
				 seq_hst_fiscalizacao.currval,
				 i.fiscalizacao,
				 v_tid,
				 i.id,
				 i.tid,
				 i.situacao_id,
				 i.situacao_texto,
				 i.nivel_precisao_id,
				 i.nivel_precisao_texto,
				 i.mecanismo_elaboracao_id,
				 i.mecanismo_elaboracao_texto,
				 i.menor_x,
				 i.menor_y,
				 i.maior_x,
				 i.maior_y);
		
			----------------------------------------------------------------------------------------------------------------
			-- Projeto Geográfico/Arquivos
			----------------------------------------------------------------------------------------------------------------
			--
			insert into hst_fisc_prj_geo_arquivos e
				(id,
				 id_hst,
				 fiscalizacao_id,
				 fiscalizacao_tid,
				 projeto_geo_arquivo_id,
				 projeto_id,
				 projeto_tid,
				 tipo_id,
				 tipo_texto,
				 arquivo_id,
				 arquivo_tid,
				 valido)
				(select seq_hst_fisc_prj_geo_arqui.nextval,
						seq_hst_fisc_prj_geo.currval,
						i.fiscalizacao,
						v_tid,
						a.id,
						a.projeto,
						a.tid,
						a.tipo,
						la.texto,
						a.arquivo,
						aa.tid,
						a.valido
				   from tab_fisc_prj_geo_arquivos    a,
						tab_arquivo                  aa,
						lov_crt_projeto_geo_arquivos la
				  where a.arquivo = aa.id
					and a.tipo = la.id
					and a.projeto = i.id);
		
			----------------------------------------------------------------------------------------------------------------
			-- Projeto Geográfico/Arquivos Ortofoto
			----------------------------------------------------------------------------------------------------------------
			--
			insert into hst_fisc_prj_geo_ortofoto e
				(id,
				 id_hst,
				 fiscalizacao_id,
				 fiscalizacao_tid,
				 ortofoto_id,
				 projeto_id,
				 projeto_tid,
				 caminho)
				(select seq_hst_fisc_prj_geo_orto.nextval,
						seq_hst_fisc_prj_geo.currval,
						i.fiscalizacao,
						v_tid,
						a.id,
						a.projeto,
						a.tid,
						a.caminho
				   from tab_fisc_prj_geo_ortofoto a
				  where a.projeto = i.id);
		
		end loop;
		------------------------------------------------------------
	
		------------------------------------------------------------
		-- Fiscalização - Enquadramento
		------------------------------------------------------------
		for i in (select f.id, f.fiscalizacao, f.tid
					from tab_fisc_enquadramento f
				   where f.fiscalizacao = p_id) loop
		
			insert into hst_fisc_enquadramento
				(id,
				 tid,
				 enquadramento_id,
				 fiscalizacao_id,
				 fiscalizacao_id_hst)
			values
				(seq_hst_fisc_enquadramento.nextval,
				 v_tid,
				 i.id,
				 i.fiscalizacao,
				 seq_hst_fiscalizacao.currval);
		
			for j in (select a.id,
							 a.enquadramento_id,
							 a.artigo,
							 a.artigo_paragrafo,
							 a.combinado_artigo,
							 a.combinado_artigo_paragrafo,
							 a.da_do,
							 a.tid
						from tab_fisc_enquadr_artig a
					   where a.enquadramento_id = i.id) loop
			
				insert into hst_fisc_enquadr_artig
					(id,
					 id_hst,
					 tid,
					 artigo_id,
					 artigo_tid,
					 enquadramento_id,
					 artigo,
					 artigo_paragrafo,
					 combinado_artigo,
					 combinado_artigo_paragrafo,
					 da_do)
				values
					(seq_hst_fisc_enquadr_artig.nextval,
					 seq_hst_fisc_enquadramento.currval,
					 j.tid,
					 j.id,
					 v_tid,
					 j.enquadramento_id,
					 j.artigo,
					 j.artigo_paragrafo,
					 j.combinado_artigo,
					 j.combinado_artigo_paragrafo,
					 j.da_do);
			end loop;
		
		end loop;
		------------------------------------------------------------
	
		------------------------------------------------------------
		-- Fiscalização - Infração
		------------------------------------------------------------
		for i in (select tfi.id,
						 tfi.classificacao,
						 c.texto                        classificacao_texto,
						 tfi.tipo,
						 cfit.tid                       tipo_tid,
						 tfi.item,
						 cfii.tid                       item_tid,
						 tfi.infracao_autuada,
						 tfi.gerado_sistema,
						 tfi.subitem,
						 cfis.tid                       subitem_tid,
						 tfi.serie,
						 lfs.texto                      serie_texto,
						 tfi.valor_multa,
						 tfi.data_lavratura_auto,
						 tfi.numero_auto_infracao_bloco,
						 tfi.descricao_infracao,
						 tfi.codigo_receita,
						 cr.texto                       codigo_receita_texto,
						 tfi.tid,
						 tfi.configuracao,
						 cfi.tid                        configuracao_tid,
						 tfi.arquivo,
						 a.tid                          arquivo_tid,
						 tfi.possui_infracao,
						 tfi.data_constatacao,
						 tfi.hora_constatacao,
						 tfi.classificacao_infracao
					from tab_fisc_infracao             tfi,
						 cnf_fisc_infracao_tipo        cfit,
						 cnf_fisc_infracao_item        cfii,
						 cnf_fisc_infracao_subitem     cfis,
						 lov_fiscalizacao_serie        lfs,
						 cnf_fisc_infracao             cfi,
						 lov_cnf_fisc_infracao_classif c,
						 lov_fisc_infracao_codigo_rece cr,
						 tab_arquivo                   a
				   where tfi.tipo = cfit.id
					 and tfi.item = cfii.id
					 and tfi.subitem = cfis.id(+)
					 and tfi.serie = lfs.id(+)
					 and tfi.configuracao = cfi.id
					 and tfi.classificacao = c.id
					 and tfi.codigo_receita = cr.id(+)
					 and tfi.arquivo = a.id(+)
					 and tfi.fiscalizacao = p_id) loop
		
			insert into hst_fisc_infracao
				(id,
				 tid,
				 infracao_id,
				 fiscalizacao_id,
				 fiscalizacao_id_hst,
				 classificacao_id,
				 classificacao_texto,
				 tipo_id,
				 tipo_tid,
				 item_id,
				 item_tid,
				 infracao_autuada,
				 gerado_sistema,
				 subitem_id,
				 subitem_tid,
				 serie_id,
				 serie_texto,
				 valor_multa,
				 data_lavratura_auto,
				 numero_auto_infracao_bloco,
				 descricao_infracao,
				 codigo_receita_id,
				 codigo_receita_texto,
				 configuracao_id,
				 configuracao_tid,
				 arquivo_id,
				 arquivo_tid,
				 possui_infracao,
				 data_constatacao,
				 hora_constatacao,
				 classificacao_infracao)
			values
				(seq_hst_fisc_infracao.nextval,
				 v_tid,
				 i.id,
				 p_id,
				 seq_hst_fiscalizacao.currval,
				 i.classificacao,
				 i.classificacao_texto,
				 i.tipo,
				 i.tipo_tid,
				 i.item,
				 i.item_tid,
				 i.infracao_autuada,
				 i.gerado_sistema,
				 i.subitem,
				 i.subitem_tid,
				 i.serie,
				 i.serie_texto,
				 i.valor_multa,
				 i.data_lavratura_auto,
				 i.numero_auto_infracao_bloco,
				 i.descricao_infracao,
				 i.codigo_receita,
				 i.codigo_receita_texto,
				 i.configuracao,
				 i.configuracao_tid,
				 i.arquivo,
				 i.arquivo_tid,
				 i.possui_infracao,
				 i.data_constatacao,
				 i.hora_constatacao,
				 i.classificacao_infracao);
		
			for j in (select tfic.id,
							 tfic.campo,
							 cfic.tid campo_tid,
							 tfic.texto,
							 tfic.tid
						from tab_fisc_infracao_campo tfic,
							 cnf_fisc_infracao_campo cfic
					   where tfic.campo = cfic.id
						 and tfic.infracao = i.id) loop
			
				insert into hst_fisc_infracao_campo
					(id,
					 tid,
					 infracao_campo_id,
					 infracao_id,
					 infracao_id_hst,
					 campo_id,
					 campo_tid,
					 texto)
				values
					(seq_hst_fisc_inf_campo.nextval,
					 v_tid,
					 j.id,
					 p_id,
					 seq_hst_fisc_infracao.currval,
					 j.campo,
					 j.campo_tid,
					 j.texto);
			
			end loop;
		
			for j in (select tfip.id,
							 tfip.tid,
							 tfip.pergunta,
							 cfip.tid           pergunta_tid,
							 tfip.resposta,
							 cfir.tid           resposta_tid,
							 tfip.especificacao
						from tab_fisc_infracao_pergunta tfip,
							 cnf_fisc_infracao_pergunta cfip,
							 cnf_fisc_infracao_resposta cfir
					   where tfip.pergunta = cfip.id
						 and tfip.resposta = cfir.id
						 and tfip.infracao = i.id) loop
			
				insert into hst_fisc_infracao_pergunta
					(id,
					 tid,
					 infracao_pergunta_id,
					 infracao_id,
					 infracao_id_hst,
					 pergunta_id,
					 pergunta_tid,
					 resposta_id,
					 resposta_tid,
					 especificacao)
				values
					(seq_hst_fisc_inf_pergunta.nextval,
					 v_tid,
					 j.id,
					 i.id,
					 seq_hst_fisc_infracao.currval,
					 j.pergunta,
					 j.pergunta_tid,
					 j.resposta,
					 j.resposta_tid,
					 j.especificacao);
			
			end loop;
			
			for j in (select tfopi.id,
							 tfopi.penalidade_outra penalidade_outra_id,
							 cfip.tid penalidade_outra_tid,
							 cfip.artigo penalidade_outra_artigo,
							 cfip.item penalidade_outra_item,
							 cfip.descricao penalidade_outra_descricao,
							 tfopi.tid
						from tab_fisc_outras_penalidad_infr tfopi,
							 cnf_fisc_infracao_penalidade cfip
					   where tfopi.penalidade_outra = cfip.id(+)
							 and tfopi.infracao = i.id) loop
			
				insert into hst_fisc_outras_penalidad_infr
					(id,
					 id_hst,
					 outras_pen_infr_id,
					 infracao_id,
					 pen_outr_id,
					 pen_outr_tid,
					 pen_outr_artigo,
					 pen_outr_item,
					 pen_outr_descricao,
					 tid)
				values
					(seq_hst_fisc_outr_penali_infr.nextval,
					 seq_hst_fisc_infracao.currval,
					 j.id,
					 i.id,
					 j.penalidade_outra_id,
					 j.penalidade_outra_tid,
					 j.penalidade_outra_artigo,
					 j.penalidade_outra_item,
					 j.penalidade_outra_descricao,
					 v_tid);
			
			end loop;
			
			for j in (select tfpi.id,
							 tfpi.penalidade penalidade_id,
							 lfpf.texto penalidade_texto,
							 tfpi.tid
						from tab_fisc_penalidades_infr tfpi,
							 lov_fisc_penalidades_fixas lfpf
					   where tfpi.penalidade = lfpf.id(+)
							 and tfpi.infracao = i.id) loop
			
				insert into hst_fisc_penalidades_infr
					(id,
					 id_hst,
					 penalidades_infr_id,
					 infracao_id,
					 penalidade_id,
					 penalidade_texto,
					 tid)
				values
					(seq_hst_fisc_penalidades_infr.nextval,
					 seq_hst_fisc_infracao.currval,
					 j.id,
					 i.id,
					 j.penalidade_id,
					 j.penalidade_texto,
					 v_tid);
			
			end loop;
		
		end loop;
		------------------------------------------------------------
	
		------------------------------------------------------------
		-- Fiscalização - Objeto infração
		------------------------------------------------------------ 
		for i in (select f.id,
						 f.fiscalizacao,
						 f.area_embargada_atv_intermed,
						 f.tei_gerado_pelo_sist,
						 f.tei_gerado_pelo_sist_serie,
						 f.num_tei_bloco,
						 f.arquivo,
						 f.data_lavratura_termo,
						 f.opniao_area_danificada,
						 f.desc_termo_embargo,
						 f.existe_atv_area_degrad,
						 f.existe_atv_area_degrad_especif,
						 f.fundament_infracao,
						 f.uso_solo_area_danif,
						 f.caract_solo_area_danif,
						 f.declividade_media_area,
						 f.infracao_result_erosao,
						 f.tid,
						 f.infr_result_er_especifique
					from tab_fisc_obj_infracao f
				   where f.fiscalizacao = p_id) loop
		
			insert into hst_fisc_obj_infracao
				(id,
				 tid,
				 obj_infracao_id,
				 fiscalizacao_id,
				 fiscalizacao_id_hst,
				 area_embargada_atv_intermed,
				 tei_gerado_pelo_sist,
				 tei_gerado_pelo_sist_serie,
				 num_tei_bloco,
				 arquivo,
				 data_lavratura_termo,
				 opniao_area_danificada,
				 desc_termo_embargo,
				 existe_atv_area_degrad,
				 existe_atv_area_degrad_especif,
				 fundament_infracao,
				 uso_solo_area_danif,
				 caract_solo_area_danif,
				 declividade_media_area,
				 infracao_result_erosao,
				 infr_result_er_especifique)
			values
				(seq_hst_fisc_obj_infracao.nextval,
				 v_tid,
				 i.id,
				 i.fiscalizacao,
				 seq_hst_fiscalizacao.currval,
				 i.area_embargada_atv_intermed,
				 i.tei_gerado_pelo_sist,
				 i.tei_gerado_pelo_sist_serie,
				 i.num_tei_bloco,
				 i.arquivo,
				 i.data_lavratura_termo,
				 i.opniao_area_danificada,
				 i.desc_termo_embargo,
				 i.existe_atv_area_degrad,
				 i.existe_atv_area_degrad_especif,
				 i.fundament_infracao,
				 i.uso_solo_area_danif,
				 i.caract_solo_area_danif,
				 i.declividade_media_area,
				 i.infracao_result_erosao,
				 i.infr_result_er_especifique);
		end loop;
		------------------------------------------------------------
	
		------------------------------------------------------------
		-- Fiscalização - Material Apreendido
		------------------------------------------------------------ 
		for i in (select tfma.id,
						 tfma.houve_material,
						 tfma.tad_gerado,
						 tfma.tad_numero,
						 tfma.tad_data,
						 tfma.serie,
						 lfmat.texto              serie_texto,
						 tfma.descricao,
						 tfma.valor_produtos,
						 tfma.depositario,
						 tp.tid                   depositario_tid,
						 tfma.endereco_logradouro,
						 tfma.endereco_bairro,
						 tfma.endereco_distrito,
						 tfma.endereco_estado,
						 le.texto                 estado_texto,
						 tfma.endereco_municipio,
						 lm.texto                 municipio_texto,
						 tfma.opiniao,
						 tfma.arquivo,
						 a.tid                    arquivo_tid,
						 tfma.tid
					from tab_fisc_material_apreendido  tfma,
						 lov_fisc_mate_apreendido_tipo lfmat,
						 lov_estado                    le,
						 lov_municipio                 lm,
						 tab_pessoa                    tp,
						 tab_arquivo                   a
				   where tfma.serie = lfmat.id(+)
					 and tfma.endereco_estado = le.id(+)
					 and tfma.endereco_municipio = lm.id(+)
					 and tfma.depositario = tp.id(+)
					 and tfma.arquivo = a.id(+)
					 and tfma.fiscalizacao = p_id) loop
		
			insert into hst_fisc_material_apreendido
				(id,
				 tid,
				 material_apreendido_id,
				 fiscalizacao_id,
				 fiscalizacao_id_hst,
				 houve_material,
				 tad_gerado,
				 tad_numero,
				 tad_data,
				 serie_id,
				 serie_texto,
				 descricao,
				 valor_produtos,
				 depositario_id,
				 depositario_tid,
				 endereco_logradouro,
				 endereco_bairro,
				 endereco_distrito,
				 endereco_estado_id,
				 endereco_estado_texto,
				 endereco_municipio_id,
				 endereco_municipio_texto,
				 opiniao,
				 arquivo_id,
				 arquivo_tid)
			values
				(seq_hst_fisc_material_apreend.nextval,
				 v_tid,
				 i.id,
				 p_id,
				 seq_hst_fiscalizacao.currval,
				 i.houve_material,
				 i.tad_gerado,
				 i.tad_numero,
				 i.tad_data,
				 i.serie,
				 i.serie_texto,
				 i.descricao,
				 i.valor_produtos,
				 i.depositario,
				 i.depositario_tid,
				 i.endereco_logradouro,
				 i.endereco_bairro,
				 i.endereco_distrito,
				 i.endereco_estado,
				 i.estado_texto,
				 i.endereco_municipio,
				 i.municipio_texto,
				 i.opiniao,
				 i.arquivo,
				 i.arquivo_tid);
		
			for j in (select tfmam.id,
							 tfmam.material_apreendido,
							 tfmam.tipo,
							 lfmat.texto tipo_texto,
							 tfmam.especificacao,
							 tfmam.tid
						from tab_fisc_mater_apree_material tfmam,
							 lov_fisc_mate_apreendido_tipo lfmat
					   where tfmam.tipo = lfmat.id
						 and tfmam.material_apreendido = i.id) loop
			
				insert into hst_fisc_mater_apree_material
					(id,
					 tid,
					 mater_apree_mater_id,
					 material_apreendido_id,
					 material_apreendido_id_hst,
					 tipo_id,
					 tipo_texto,
					 especificacao)
				values
					(seq_fisc_mater_apree_material.nextval,
					 v_tid,
					 j.id,
					 i.id,
					 seq_hst_fisc_material_apreend.currval,
					 j.tipo,
					 j.tipo_texto,
					 j.especificacao);
			
			end loop;
		
		end loop;
		------------------------------------------------------------  
	
		------------------------------------------------------------
		-- Fiscalização - Apreensão - NOVA 
		------------------------------------------------------------ 
		for i in (select tfa.id,
						 tfa.iuf_digital,
						 tfa.iuf_numero,
						 tfa.iuf_data,
						 tfa.numero_lacres,
						 tfa.arquivo arquivo_id,
						 ta.tid arquivo_tid,
						 tfa.serie serie_id,
						 lfs.texto serie_texto,
						 tfa.descricao,
						 tfa.valor_produtos,
						 tfa.depositario depositario_id,
						 tp.nome depositario_nome,
						 tp.cpf depositario_cpf,
						 tfa.endereco_logradouro,
						 tfa.endereco_bairro,
						 tfa.endereco_distrito,
						 tfa.endereco_estado endereco_estado_id,
						 le.texto endereco_estado_texto,
						 tfa.endereco_municipio endereco_municipio_id,
						 lm.texto endereco_municipio_texto,
						 tfa.opiniao,
						 tfa.valor_produtos_reais,
						 tfa.tid
					from tab_fisc_apreensao tfa,
						 tab_arquivo ta,
						 lov_fiscalizacao_serie lfs,
						 tab_pessoa tp,
						 lov_estado le,
						 lov_municipio lm
				   where tfa.arquivo = ta.id(+)
						 and tfa.serie = lfs.id(+)
						 and tfa.depositario = tp.id(+)
						 and tfa.endereco_estado = le.id(+)
						 and tfa.endereco_municipio = lm.id(+)
						 and tfa.fiscalizacao = p_id ) loop
		
			insert into hst_fisc_apreensao
				(id,
				 id_hst,
				 apreensao_id,
				 fiscalizacao_id,
				 iuf_digital,
				 iuf_numero,
				 iuf_data,
				 numero_lacres,
				 arquivo_id,
				 arquivo_tid,
				 serie_id,
				 serie_texto,
				 descricao,
				 valor_produtos,
				 depositario_id,
				 depositario_nome,
				 depositario_cpf,
				 endereco_logradouro,
				 endereco_bairro,
				 endereco_distrito,
				 endereco_estado_id,
				 endereco_estado,
				 endereco_municipio_id,
				 endereco_municipio,
				 opiniao,
				 valor_produtos_reais,
				 tid)
			values
				(seq_hst_fisc_apreensao.nextval,
				 seq_hst_fiscalizacao.currval,
				 i.id,
				 p_id,
				 i.iuf_digital,
				 i.iuf_numero,
				 i.iuf_data,
				 i.numero_lacres,
				 i.arquivo_id,
				 i.arquivo_tid,
				 i.serie_id,
				 i.serie_texto,
				 i.descricao,
				 i.valor_produtos,
				 i.depositario_id,
				 i.depositario_nome,
				 i.depositario_cpf,
				 i.endereco_logradouro,
				 i.endereco_bairro,
				 i.endereco_distrito,
				 i.endereco_estado_id,
				 i.endereco_estado_texto,
				 i.endereco_municipio_id,
				 i.endereco_municipio_texto,
				 i.opiniao,
				 i.valor_produtos_reais,
				 v_tid);
		
			for j in ( select tfap.id,
							 tfap.apreensao apreensao_id,
							 i.tid apreensao_tid,
							 tfap.produto produto_id,
							 cfip.tid produto_tid,
							 cfip.item produto_item,
							 cfip.unidade produto_unidade,
							 tfap.quantidade,
							 tfap.destinacao destinacao_id,
							 cfid.tid destinacao_tid,
							 cfid.destino destinacao_destino							 
					  from tab_fisc_apreensao_produto tfap,
						   cnf_fisc_infracao_produto cfip,
						   cnf_fisc_infr_destinacao cfid
					  where tfap.apreensao = i.id
							and tfap.produto = cfip.id(+)
							and tfap.destinacao = cfid.id(+)) loop
			
				insert into hst_fisc_apreensao_produto
					(id,
					 id_hst,
					 apreensao_prod_id,
					 apreensao_id,
					 apreensao_tid,
					 produto_id,
					 produto_tid,
					 produto_item,
					 produto_unidade,
					 quantidade,
					 destinacao_id,
					 destinacao_tid,
					 destinacao_destino,
					 tid)
				values
					(seq_hst_fisc_apreensao_prod.nextval,
					 seq_hst_fisc_apreensao.currval,
					 j.id,
					 j.apreensao_id,
					 j.apreensao_tid,
					 j.produto_id,
					 j.produto_tid,
					 j.produto_item,
					 j.produto_unidade,
					 j.quantidade,
					 j.destinacao_id,
					 j.destinacao_tid,
					 j.destinacao_destino,
					 v_tid);
			
			end loop;
		
		end loop;
		------------------------------------------------------------ 
		
		
		------------------------------------------------------------
		-- Fiscalização - Multa - NOVA 
		------------------------------------------------------------ 
		for i in (select tfm.id,
						 tfm.iuf_digital,
						 tfm.iuf_numero,
						 tfm.iuf_data,
						 tfm.serie serie_id,
						 lfs.texto serie_texto,
						 tfm.valor_multa,
						 tfm.arquivo arquivo_id,
						 ta.tid arquivo_tid,
						 tfm.justificar,
						 tfm.codigo_receita codigo_receita_id,
						 lficr.texto codigo_receita_texto,
						 lficr.descricao codigo_receita_descricao,
						 tfm.tid
					from tab_fisc_multa tfm,
						 lov_fiscalizacao_serie lfs,
						 tab_arquivo ta,
						 lov_fisc_infracao_codigo_rece lficr
				   where tfm.serie = lfs.id(+)
						 and tfm.arquivo = ta.id(+)
						 and tfm.codigo_receita = lficr.id(+)
						 and tfm.fiscalizacao = p_id ) loop
		
			insert into hst_fisc_multa
				(id,
				 id_hst,
				 multa_id,
				 fiscalizacao_id,
				 iuf_digital,
				 iuf_numero,
				 iuf_data,
				 arquivo_id,
				 arquivo_tid,
				 serie_id,
				 serie_texto,
				 valor_multa,
				 justificar,
				 codigo_receita_id,
				 cod_receita_texto,
				 cod_receita_descricao,
				 tid)
			values
				(seq_hst_fisc_multa.nextval,
				 seq_hst_fiscalizacao.currval,
				 i.id,
				 p_id,
				 i.iuf_digital,
				 i.iuf_numero,
				 i.iuf_data,
				 i.arquivo_id,
				 i.arquivo_tid,
				 i.serie_id,
				 i.serie_texto,
				 i.valor_multa,
				 i.justificar,
				 i.codigo_receita_id,
				 i.codigo_receita_texto,
				 i.codigo_receita_descricao,
				 v_tid);
		
		end loop;
		------------------------------------------------------------ 
		
		
		------------------------------------------------------------
		-- Fiscalização - Outras Penalidades - NOVA 
		------------------------------------------------------------ 
		for i in (select tfop.id,
						 tfop.iuf_digital,
						 tfop.iuf_numero,
						 tfop.iuf_data,
						 tfop.serie serie_id,
						 lfs.texto serie_texto,
						 tfop.descricao,
						 tfop.arquivo arquivo_id,
						 ta.tid arquivo_tid,
						 tfop.tid
					from tab_fisc_outras_penalidades tfop,
						 lov_fiscalizacao_serie lfs,
						 tab_arquivo ta
				   where tfop.serie = lfs.id(+)
						 and tfop.arquivo = ta.id(+)
						 and tfop.fiscalizacao = p_id ) loop
		
			insert into hst_fisc_outras_penalidades
				(id,
				 id_hst,
				 outras_penalidades_id,
				 fiscalizacao_id,
				 iuf_digital,
				 iuf_numero,
				 iuf_data,
				 arquivo_id,
				 arquivo_tid,
				 serie_id,
				 serie_texto,
				 descricao,
				 tid)
			values
				(seq_hst_fisc_outras_penal.nextval,
				 seq_hst_fiscalizacao.currval,
				 i.id,
				 p_id,
				 i.iuf_digital,
				 i.iuf_numero,
				 i.iuf_data,
				 i.arquivo_id,
				 i.arquivo_tid,
				 i.serie_id,
				 i.serie_texto,
				 i.descricao,
				 v_tid);
		
		end loop;
		------------------------------------------------------------ 
	
	
		---------------------------------------------------------
		-- Fiscalização - Consideração Final
		--------------------------------------------------------- 
	
		for i in (select t.id,
						 t.fiscalizacao,
						 t.justificar,
						 t.descrever,
						 t.tem_reparacao,
						 t.reparacao,
						 t.tem_termo_comp_justificar,
						 t.tid,
						 t.tem_termo_comp,
						 t.arquivo_termo,
						 ta.tid arquivo_termo_tid
					from tab_fisc_consid_final t, tab_arquivo ta
				   where t.arquivo_termo = ta.id(+)
					 and t.fiscalizacao = p_id) loop
		
			insert into hst_fisc_consid_final
				(id,
				 id_hst,
				 consid_final_id,
				 fiscalizacao_id,
				 justificar,
				 descrever,
				 tem_reparacao,
				 reparacao,
				 tem_termo_comp_justificar,
				 tid,
				 tem_termo_comp,
				 arquivo_termo_id,
				 arquivo_termo_tid)
			values
				(seq_hst_fisc_consid_final.nextval,
				 seq_hst_fiscalizacao.currval,
				 i.id,
				 i.fiscalizacao,
				 i.justificar,
				 i.descrever,
				 i.tem_reparacao,
				 i.reparacao,
				 i.tem_termo_comp_justificar,
				 v_tid,
				 i.tem_termo_comp,
				 i.arquivo_termo,
				 i.arquivo_termo_tid);
		
			insert into hst_fisc_consid_final_arq
				(id,
				 id_hst,
				 consid_final_arq_id,
				 consid_final_id,
				 arquivo_id,
				 arquivo_tid,
				 ordem,
				 descricao,
				 tid)
				(select seq_h_fisc_consid_final_arq.nextval,
						seq_hst_fisc_consid_final.currval,
						ta.id,
						ta.consid_final,
						ta.arquivo,
						tc.tid,
						ta.ordem,
						ta.descricao,
						v_tid
				   from tab_fisc_consid_final_arq ta, tab_arquivo tc
				  where ta.arquivo = tc.id
					and ta.consid_final = i.id);
					
			insert into hst_fisc_consid_final_iuf
				(id,
				 id_hst,
				 consid_final_iuf_id,
				 consid_final_id,
				 arquivo_id,
				 arquivo_tid,
				 ordem,
				 descricao,
				 tid)
				(select seq_hst_fisc_consid_final_iuf.nextval,
						seq_hst_fisc_consid_final.currval,
						ta.id,
						ta.consid_final,
						ta.arquivo,
						tc.tid,
						ta.ordem,
						ta.descricao,
						v_tid
				   from tab_fisc_consid_final_iuf ta, tab_arquivo tc
				  where ta.arquivo = tc.id
					and ta.consid_final = i.id);
		
			insert into hst_fisc_consid_final_test
				(id,
				 id_hst,
				 consid_final_test_id,
				 consid_final_id,
				 idaf,
				 testemunha_id,
				 testemunha_tid,
				 nome,
				 endereco,
				 tid,
				 testemunha_setor_id,
				 testemunha_setor_tid)
				(select seq_hst_fiscconsidfinaltest.nextval,
						seq_hst_fisc_consid_final.currval,
						t.id,
						t.consid_final,
						t.idaf,
						t.testemunha,
						tf.tid,
						t.nome,
						t.endereco,
						v_tid,
						t.testemunha_setor,
						ts.tid
				   from tab_fisc_consid_final_test t,
						tab_funcionario            tf,
						tab_setor                  ts
				  where t.testemunha = tf.id(+)
					and t.testemunha_setor = ts.id(+)
					and t.consid_final = i.id);
		
			insert into hst_fisc_consid_final_ass
				(id,
				 id_hst,
				 tid,
				 assinantes_id,
				 consid_final_id,
				 consid_final_tid,
				 funcionario_id,
				 funcionario_tid,
				 cargo_id,
				 cargo_tid)
				(select seq_hst_fisc_consid_final_ass.nextval,
						seq_hst_fisc_consid_final.currval,
						v_tid,
						t.id,
						t.consid_final,
						i.tid,
						t.funcionario,
						tf.tid,
						t.cargo,
						tc.tid
				   from tab_fisc_consid_final_ass t,
						tab_funcionario           tf,
						tab_cargo                 tc
				  where t.funcionario = tf.id(+)
					and t.cargo = tc.id(+)
					and t.consid_final = i.id);
		
		end loop;
		---------------------------------------------------------     
	
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Fiscalização. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Fiscalização. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;

	---------------------------------------------------------
	-- Configuração Fiscalização 
	---------------------------------------------------------
	procedure configfiscalizacao(p_id               number,
								 p_acao             number,
								 p_executor_id      number,
								 p_executor_nome    varchar2,
								 p_executor_login   varchar2,
								 p_executor_tipo_id number,
								 p_executor_tid     varchar2) is
		v_sucesso boolean := false;
		v_tid     varchar2(36) := '';
	begin
		---------------------------------------------------------
		-- Configuração
		---------------------------------------------------------
		select t.tid into v_tid from cnf_fisc_infracao t where t.id = p_id;
	
		insert into hst_cnf_fisc_infracao
			(id,
			 config_fisc_id,
			 classificacao_id,
			 classificacao_texto,
			 tipo_id,
			 tipo_tid,
			 item_id,
			 item_tid,
			 tid,
			 executor_id,
			 executor_tid,
			 executor_nome,
			 executor_login,
			 executor_tipo_id,
			 executor_tipo_texto,
			 acao_executada,
			 data_execucao)
			(select seq_hst_cnf_fisc_infracao.nextval,
					t.id,
					t.classificacao,
					lc.texto,
					t.tipo,
					cft.tid,
					t.item,
					cfi.tid,
					v_tid,
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
						and la.artefato = 84),
					systimestamp
			   from cnf_fisc_infracao             t,
					lov_cnf_fisc_infracao_classif lc,
					cnf_fisc_infracao_tipo        cft,
					cnf_fisc_infracao_item        cfi
			  where t.classificacao = lc.id(+)
				and t.tipo = cft.id(+)
				and t.item = cfi.id(+)
				and t.id = p_id);
	
		----------------------------------------------
		-- SubItens
		----------------------------------------------          
		insert into hst_cnf_fisc_infr_cnf_subit
			(id,
			 id_hst,
			 cnf_subitem_id,
			 configuracao_id,
			 subitem_id,
			 subitem_tid,
			 tid)
			(select seq_hst_cnffiscinfrcnfsubit.nextval,
					seq_hst_cnf_fisc_infracao.currval,
					t.id,
					t.configuracao,
					t.subitem,
					cs.tid,
					v_tid
			   from cnf_fisc_infr_cnf_subitem t,
					cnf_fisc_infracao_subitem cs
			  where t.subitem = cs.id(+)
				and t.configuracao = p_id);
		----------------------------------------------
		----------------------------------------------
		-- Perguntas
		----------------------------------------------          
		insert into hst_cnf_fisc_infr_cnf_pergu
			(id,
			 id_hst,
			 cnf_pergunta_id,
			 configuracao_id,
			 pergunta_id,
			 pergunta_tid,
			 tid)
			(select seq_hst_cnffiscinfrcnfpergu.nextval,
					seq_hst_cnf_fisc_infracao.currval,
					t.id,
					t.configuracao,
					t.pergunta,
					cp.tid,
					v_tid
			   from cnf_fisc_infr_cnf_pergunta t,
					cnf_fisc_infracao_pergunta cp
			  where t.pergunta = cp.id(+)
				and t.configuracao = p_id);
		----------------------------------------------          
		----------------------------------------------
		-- Campos
		----------------------------------------------          
		insert into hst_cnf_fisc_infr_cnf_campo
			(id,
			 id_hst,
			 cnf_campo_id,
			 configuracao_id,
			 campo_id,
			 campo_tid,
			 tid)
			(select seq_hst_cnffiscinfrcnfcampo.nextval,
					seq_hst_cnf_fisc_infracao.currval,
					t.id,
					t.configuracao,
					t.campo,
					cc.tid,
					v_tid
			   from cnf_fisc_infr_cnf_campo t, cnf_fisc_infracao_campo cc
			  where t.campo = cc.id(+)
				and t.configuracao = p_id);
		----------------------------------------------               
	
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico da Configuração da Fiscalização. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico da Configuração da Fiscalização. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
  
  
  	---------------------------------------------------------
	-- Configuracao Fiscalizacao - Tipo Infracao
	---------------------------------------------------------
	procedure penalidadeinfracao(p_id               number,
						   p_acao             number,
						   p_executor_id      number,
						   p_executor_nome    varchar2,
						   p_executor_login   varchar2,
						   p_executor_tipo_id number,
						   p_executor_tid     varchar2) is
		v_sucesso boolean := false;
	begin
		----------------------------------------------------------------------------------------------------------------
		-- Tipo Infracao
		----------------------------------------------------------------------------------------------------------------
		for i in (select t.id, t.artigo, t.item, t.descricao, t.tid
					from cnf_fisc_infracao_penalidade t
				   where t.id = p_id) loop
			-- Inserindo na histórico
			insert into hst_cnf_fisc_penalidade
				(id,
				 artigo,
				 item,
				 descricao,
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
				(SEQ_HST_CNF_INFR_PENALIDADE.nextval,
				 i.artigo,
				 i.item,
         i.descricao,
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
					 and la.artefato = 85),
				 systimestamp);
		end loop;
		----------------------------------------------------------------------------------------------------------------
	
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Configuração Fiscalizacao - Penalidade. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Configuração Fiscalizacao - Penalidade. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;

	---------------------------------------------------------
	-- Configuracao Fiscalizacao - Tipo Infracao
	---------------------------------------------------------
	procedure tipoinfracao(p_id               number,
						   p_acao             number,
						   p_executor_id      number,
						   p_executor_nome    varchar2,
						   p_executor_login   varchar2,
						   p_executor_tipo_id number,
						   p_executor_tid     varchar2) is
		v_sucesso boolean := false;
	begin
		----------------------------------------------------------------------------------------------------------------
		-- Tipo Infracao
		----------------------------------------------------------------------------------------------------------------
		for i in (select t.id, t.texto, t.ativo, t.tid
					from cnf_fisc_infracao_tipo t
				   where t.id = p_id) loop
			-- Inserindo na histórico
			insert into hst_cnf_fisc_infracao_tipo
				(id,
				 infracao_tipo_id,
				 texto,
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
				(seq_hst_cnf_fisc_infracao_tipo.nextval,
				 i.id,
				 i.texto,
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
					from lov_historico_artefatos_acoes la
				   where la.acao = p_acao
					 and la.artefato = 85),
				 systimestamp);
		end loop;
		----------------------------------------------------------------------------------------------------------------
	
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Configuração Fiscalizacao - Tipo Infração. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Configuração Fiscalizacao - Tipo Infração. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
	--------------------------------------------------------- 
	---------------------------------------------------------
	-- Configuracao Fiscalizacao - Item
	---------------------------------------------------------
	procedure iteminfracao(p_id               number,
						   p_acao             number,
						   p_executor_id      number,
						   p_executor_nome    varchar2,
						   p_executor_login   varchar2,
						   p_executor_tipo_id number,
						   p_executor_tid     varchar2) is
		v_sucesso boolean := false;
	begin
		----------------------------------------------------------------------------------------------------------------
		-- Item
		----------------------------------------------------------------------------------------------------------------
		for i in (select t.id, t.texto, t.ativo, t.tid
					from cnf_fisc_infracao_item t
				   where t.id = p_id) loop
			-- Inserindo na histórico
			insert into hst_cnf_fisc_infracao_item
				(id,
				 infracao_item_id,
				 texto,
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
				(seq_hst_cnf_fisc_infracao_item.nextval,
				 i.id,
				 i.texto,
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
					from lov_historico_artefatos_acoes la
				   where la.acao = p_acao
					 and la.artefato = 86),
				 systimestamp);
		end loop;
		----------------------------------------------------------------------------------------------------------------
	
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Configuração Fiscalizacao - Item. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Configuração Fiscalizacao - Item. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
	--------------------------------------------------------- 

	---------------------------------------------------------
	-- Configuracao Fiscalizacao - Subitem
	---------------------------------------------------------
	procedure subiteminfracao(p_id               number,
							  p_acao             number,
							  p_executor_id      number,
							  p_executor_nome    varchar2,
							  p_executor_login   varchar2,
							  p_executor_tipo_id number,
							  p_executor_tid     varchar2) is
		v_sucesso boolean := false;
	begin
		----------------------------------------------------------------------------------------------------------------
		-- Subitem
		----------------------------------------------------------------------------------------------------------------
		for i in (select t.id, t.texto, t.ativo, t.tid
					from cnf_fisc_infracao_subitem t
				   where t.id = p_id) loop
			-- Inserindo na histórico
			insert into hst_cnf_fisc_infracao_subitem
				(id,
				 infracao_subitem_id,
				 texto,
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
				(seq_hst_cnf_fisc_infr_subitem.nextval,
				 i.id,
				 i.texto,
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
					from lov_historico_artefatos_acoes la
				   where la.acao = p_acao
					 and la.artefato = 87),
				 systimestamp);
		end loop;
		----------------------------------------------------------------------------------------------------------------
	
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Configuração Fiscalizacao - Subitem. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Configuração Fiscalizacao - Subitem. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
	---------------------------------------------------------  

	---------------------------------------------------------
	-- Configuracao Fiscalizacao - Campo
	---------------------------------------------------------
	procedure campoinfracao(p_id               number,
							p_acao             number,
							p_executor_id      number,
							p_executor_nome    varchar2,
							p_executor_login   varchar2,
							p_executor_tipo_id number,
							p_executor_tid     varchar2) is
		v_sucesso boolean := false;
	begin
		----------------------------------------------------------------------------------------------------------------
		-- Campo
		----------------------------------------------------------------------------------------------------------------
		for i in (select t.id,
						 t.texto,
						 t.ativo,
						 t.tipo,
						 lt.texto  tipo_texto,
						 t.unidade,
						 lu.texto  unidade_texto,
						 t.tid
					from cnf_fisc_infracao_campo        t,
						 lov_cnf_fisc_infracao_camp_uni lu,
						 lov_cnf_fisc_infracao_camp_tip lt
				   where lu.id(+) = t.unidade
					 and lt.id = t.tipo
					 and t.id = p_id) loop
			-- Inserindo na histórico
			insert into hst_cnf_fisc_infracao_campo
				(id,
				 infracao_campo_id,
				 texto,
				 ativo,
				 tipo,
				 tipo_texto,
				 unidade,
				 unidade_texto,
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
				(seq_hst_cnf_fisc_infracao_camp.nextval,
				 i.id,
				 i.texto,
				 i.ativo,
				 i.tipo,
				 i.tipo_texto,
				 i.unidade,
				 i.unidade_texto,
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
					 and la.artefato = 88),
				 systimestamp);
		end loop;
		----------------------------------------------------------------------------------------------------------------
	
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Configuração Fiscalizacao - Campo. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Configuração Fiscalizacao - Campo. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
	--------------------------------------------------------- 

	---------------------------------------------------------
	-- Configuracao Fiscalizacao - Resposta
	---------------------------------------------------------
	procedure respostainfracao(p_id               number,
							   p_acao             number,
							   p_executor_id      number,
							   p_executor_nome    varchar2,
							   p_executor_login   varchar2,
							   p_executor_tipo_id number,
							   p_executor_tid     varchar2) is
		v_sucesso boolean := false;
	begin
		----------------------------------------------------------------------------------------------------------------
		-- resposta
		----------------------------------------------------------------------------------------------------------------
		for i in (select t.id, t.texto, t.ativo, t.tid
					from cnf_fisc_infracao_resposta t
				   where t.id = p_id) loop
			-- Inserindo na histórico
			insert into hst_cnf_fisc_infracao_resposta
				(id,
				 resposta_id,
				 texto,
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
				(seq_hst_cnf_fisc_infr_resposta.nextval,
				 i.id,
				 i.texto,
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
					from lov_historico_artefatos_acoes la
				   where la.acao = p_acao
					 and la.artefato = 93),
				 systimestamp);
		end loop;
		----------------------------------------------------------------------------------------------------------------
	
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Configuração Fiscalizacao - resposta. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Configuração Fiscalizacao - resposta. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
	---------------------------------------------------------

	---------------------------------------------------------
	-- Configuracao Fiscalizacao - Pergunta
	---------------------------------------------------------
	procedure perguntainfracao(p_id               number,
							   p_acao             number,
							   p_executor_id      number,
							   p_executor_nome    varchar2,
							   p_executor_login   varchar2,
							   p_executor_tipo_id number,
							   p_executor_tid     varchar2) is
		v_sucesso boolean := false;
		v_aux     number;
	begin
		----------------------------------------------------------------------------------------------------------------
		-- Pergunta
		----------------------------------------------------------------------------------------------------------------
		for i in (select t.id, t.texto, t.ativo, t.tid
					from cnf_fisc_infracao_pergunta t
				   where t.id = p_id) loop
			-- Inserindo na histórico
			insert into hst_cnf_fisc_infracao_pergunta h
				(id,
				 pergunta_id,
				 texto,
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
				(seq_hst_cnf_fisc_infr_pergunta.nextval,
				 i.id,
				 i.texto,
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
					from lov_historico_artefatos_acoes la
				   where la.acao = p_acao
					 and la.artefato = 92),
				 systimestamp)
			returning h.id into v_aux;
		
			----------------------------------------------------------------------------------------------------------------
			-- Pergunta/Respostas
			----------------------------------------------------------------------------------------------------------------
			for j in (select p.id,
							 p.pergunta,
							 p.resposta,
							 p.especificar,
							 p.resposta_tid,
							 p.tid
						from cnf_fisc_infracao_pergu_respo p
					   where p.pergunta = i.id) loop
				-- Inserindo na histórico
				insert into hst_cnf_fisc_infracao_pergu_re
					(id,
					 id_hst,
					 pergu_respo_id,
					 pergunta,
					 resposta,
					 resposta_tid,
					 especificar,
					 tid)
				values
					(seq_hst_cnf_fisc_infr_pergu_re.nextval,
					 v_aux,
					 j.id,
					 j.pergunta,
					 j.resposta,
					 j.resposta_tid,
					 j.especificar,
					 j.tid);
			end loop;
		
		end loop;
		----------------------------------------------------------------------------------------------------------------
	
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Configuração Fiscalizacao - Pergunta. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Configuração Fiscalizacao - Pergunta. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
	---------------------------------------------------------   

---------------------------------------------------------
	-- Configuracao Fiscalizacao - Produto Apreendido
---------------------------------------------------------
procedure produtoapreendido(p_id               number,
							p_acao             number,
							p_executor_id      number,
							p_executor_nome    varchar2,
							p_executor_login   varchar2,
							p_executor_tipo_id number,
							p_executor_tid     varchar2) is
		v_sucesso boolean := false;
	begin
		----------------------------------------------------------------------------------------------------------------
		-- Produto Apreendido
		----------------------------------------------------------------------------------------------------------------
		for i in (select t.id,
						 t.item,
						 t.unidade,
						 t.ativo,
						 t.tid
					from cnf_fisc_infracao_produto t
				   where t.id = p_id) loop
			-- Inserindo na histórico
			insert into hst_cnf_fisc_infracao_produto
				(id,
				 produto_id,
				 item,
				 unidade,
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
				(seq_hst_cnf_fisc_infr_produto.nextval,
				 i.id,
				 i.item,
				 i.unidade,
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
                and art.texto like 'produtoapreendido'),
				 systimestamp);
		end loop;
		----------------------------------------------------------------------------------------------------------------
	
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Configuração Fiscalizacao - Produto Apreendido. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Configuração Fiscalizacao - Produto Apreendido. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
---------------------------------------------------------   

---------------------------------------------------------
	-- Configuracao Fiscalizacao - Destinação
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
		-- Destinação
		----------------------------------------------------------------------------------------------------------------
		for i in (select t.id,
						 t.destino,
						 t.ativo,
						 t.tid
					from cnf_fisc_infr_destinacao t
				   where t.id = p_id) loop
			-- Inserindo na histórico
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
									'Erro ao gerar o Histórico de Configuração Fiscalizacao - Destinação. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Configuração Fiscalizacao - Destinação. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
---------------------------------------------------------   

---------------------------------------------------------
	-- Configuracao Fiscalizacao - Código da Receita
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
		-- Código da Receita
		----------------------------------------------------------------------------------------------------------------
		for i in (select t.id,
						 t.texto,
						 t.descricao,
						 t.ativo,
						 t.tid
					from lov_fisc_infracao_codigo_rece t
				   where t.id = p_id) loop
			-- Inserindo na histórico
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
									'Erro ao gerar o Histórico de Configuração Fiscalizacao - Código da Receita. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Configuração Fiscalizacao - Código da Receita. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
---------------------------------------------------------   

	---------------------------------------------------------
	-- Motosserra
	---------------------------------------------------------
	procedure motosserra(p_id               number,
						 p_acao             number,
						 p_executor_id      number,
						 p_executor_nome    varchar2,
						 p_executor_login   varchar2,
						 p_executor_tipo_id number,
						 p_executor_tid     varchar2) is
		v_sucesso boolean := false;
	begin
		----------------------------------------------------------------------------------------------------------------
		-- Motosserra
		----------------------------------------------------------------------------------------------------------------
		for i in (select t.id,
						 t.tid,
						 t.possui_registro,
						 t.registro_numero,
						 t.serie_numero,
						 t.modelo,
						 t.nota_fiscal_numero,
						 t.proprietario,
						 p.tid proprietario_tid,
						 t.situacao
					from tab_motosserra t, tab_pessoa p
				   where t.proprietario = p.id
					 and t.id = p_id) loop
		
			-- Inserindo na histórico
			insert into hst_motosserra p
				(id,
				 tid,
				 motosserra,
				 possui_registro,
				 registro_numero,
				 serie_numero,
				 modelo,
				 nota_fiscal_numero,
				 proprietario_id,
				 proprietario_tid,
				 situacao,
				 executor_id,
				 executor_tid,
				 executor_nome,
				 executor_login,
				 executor_tipo_id,
				 executor_tipo_texto,
				 acao_executada,
				 data_execucao)
			values
				(seq_hst_motosserra.nextval,
				 i.tid,
				 i.id,
				 i.possui_registro,
				 i.registro_numero,
				 i.serie_numero,
				 i.modelo,
				 i.nota_fiscal_numero,
				 i.proprietario,
				 i.proprietario_tid,
				 i.situacao,
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
					 and la.artefato = 96),
				 systimestamp);
		end loop;
		----------------------------------------------------------------------------------------------------------------
	
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Motosserra. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Motosserra. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
	---------------------------------------------------------

	---------------------------------------------------------
	-- Ficha Fundiária
	---------------------------------------------------------
	procedure fichafundiaria(p_id               number,
							 p_acao             number,
							 p_executor_id      number,
							 p_executor_nome    varchar2,
							 p_executor_login   varchar2,
							 p_executor_tipo_id number,
							 p_executor_tid     varchar2) is
		v_sucesso boolean := false;
	begin
		----------------------------------------------------------------------------------------------------------------
		-- Ficha Fundiária
		----------------------------------------------------------------------------------------------------------------
		for i in (select f.id,
						 f.codigo,
						 f.requerente,
						 f.idoc,
						 f.ndoc,
						 f.pai,
						 f.mae,
						 f.endereco,
						 f.municipio,
						 f.distrito,
						 f.lugar,
						 f.tipo_area,
						 f.data_med,
						 f.area_med,
						 f.perimetro,
						 f.topografo,
						 f.prot_reg,
						 f.prot_ger,
						 f.lote,
						 f.quadra,
						 f.l_sul,
						 f.l_norte,
						 f.l_leste,
						 f.l_oeste,
						 f.data_ec,
						 f.lv_ec,
						 f.fl_ec,
						 f.data_ed,
						 f.lv_ed,
						 f.fl_ed,
						 f.observacao,
						 f.tid
					from tab_acervo_ficha_fund f
				   where f.id = p_id) loop
		
			-- Inserindo no histórico
			insert into hst_acervo_ficha_fund
				(id,
				 ficha_id,
				 tid,
				 codigo,
				 requerente,
				 idoc,
				 ndoc,
				 pai,
				 mae,
				 endereco,
				 municipio,
				 distrito,
				 lugar,
				 tipo_area,
				 data_med,
				 area_med,
				 perimetro,
				 topografo,
				 prot_reg,
				 prot_ger,
				 lote,
				 quadra,
				 l_sul,
				 l_norte,
				 l_leste,
				 l_oeste,
				 data_ec,
				 lv_ec,
				 fl_ec,
				 data_ed,
				 lv_ed,
				 fl_ed,
				 observacao,
				 executor_id,
				 executor_tid,
				 executor_nome,
				 executor_login,
				 executor_tipo_id,
				 executor_tipo_texto,
				 acao_executada,
				 data_execucao)
			values
				(seq_hst_acervo_ficha_fund.nextval,
				 i.id,
				 i.tid,
				 i.codigo,
				 i.requerente,
				 i.idoc,
				 i.ndoc,
				 i.pai,
				 i.mae,
				 i.endereco,
				 i.municipio,
				 i.distrito,
				 i.lugar,
				 i.tipo_area,
				 i.data_med,
				 i.area_med,
				 i.perimetro,
				 i.topografo,
				 i.prot_reg,
				 i.prot_ger,
				 i.lote,
				 i.quadra,
				 i.l_sul,
				 i.l_norte,
				 i.l_leste,
				 i.l_oeste,
				 i.data_ec,
				 i.lv_ec,
				 i.fl_ec,
				 i.data_ed,
				 i.lv_ed,
				 i.fl_ed,
				 i.observacao,
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
					 and la.artefato = 97),
				 systimestamp);
		end loop;
		----------------------------------------------------------------------------------------------------------------
	
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Ficha Fundiária. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Ficha Fundiária. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
	---------------------------------------------------------

	---------------------------------------------------------
	-- Acompanhamento da Fiscalização 
	---------------------------------------------------------
	procedure acompanhamento(p_id               number,
							 p_acao             number,
							 p_executor_id      number,
							 p_executor_nome    varchar2,
							 p_executor_login   varchar2,
							 p_executor_tipo_id number,
							 p_executor_tid     varchar2) is
		v_sucesso boolean := false;
		v_id      number;
	begin
		---------------------------------------------------------
		-- Acompanhamento da Fiscalização
		---------------------------------------------------------
	
		for i in (select a.id,
						 a.fiscalizacao,
						 f.tid                            fiscalizacao_tid,
						 a.numero_sufixo,
						 a.data_vistoria,
						 a.situacao,
						 ls.texto                         situacao_texto,
						 a.data_situacao,
						 a.agente_fiscal,
						 af.tid                           agente_fiscal_tid,
						 a.setor,
						 s.tid                            setor_tid,
						 a.area_total,
						 a.area_florestal_nativa,
						 a.reserva_legal,
						 a.possui_area_embargada,
						 a.opniao_area_embargo,
						 a.ativ_area_embargada,
						 a.atviv_area_embargada_especific,
						 a.uso_area_solo,
						 a.caract_solo_area_danificada,
						 a.declividade_media_area,
						 a.infr_resultou_erosao,
						 a.infr_resultou_erosao_especific,
						 a.houve_apreensao_material,
						 a.opniao_destin_material_apreend,
						 a.houve_desrespeito_tad,
						 a.houve_desrespeito_tad_especifi,
						 a.informacoes_relevante_processo,
						 a.neces_repar_dano_amb,
						 a.neces_repar_dano_amb_especific,
						 a.firmou_termo_comprom,
						 a.firmou_termo_comprom_especific,
						 a.arquivo,
						 ta.tid                           arquivo_tid,
						 a.tid
					from tab_acompanhamento_fisc a,
						 lov_acomp_fisc_situacao ls,
						 tab_fiscalizacao        f,
						 tab_funcionario         af,
						 tab_setor               s,
						 tab_arquivo             ta
				   where a.situacao = ls.id
					 and a.fiscalizacao = f.id
					 and a.agente_fiscal = af.id
					 and a.setor = s.id
					 and a.arquivo = ta.id(+)
					 and a.id = p_id) loop
		
			insert into hst_acompanhamento_fisc
				(id,
				 acompanhamento,
				 fiscalizacao_id,
				 fiscalizacao_tid,
				 numero_sufixo,
				 data_vistoria,
				 situacao_id,
				 situacao_texto,
				 data_situacao,
				 agente_fiscal_id,
				 agente_fiscal_tid,
				 setor_id,
				 setor_tid,
				 area_total,
				 area_florestal_nativa,
				 reserva_legal,
				 possui_area_embargada,
				 opniao_area_embargo,
				 ativ_area_embargada,
				 atviv_area_embargada_especific,
				 uso_area_solo,
				 caract_solo_area_danificada,
				 declividade_media_area,
				 infr_resultou_erosao,
				 infr_resultou_erosao_especific,
				 houve_apreensao_material,
				 opniao_destin_material_apreend,
				 houve_desrespeito_tad,
				 houve_desrespeito_tad_especifi,
				 informacoes_relevante_processo,
				 neces_repar_dano_amb,
				 neces_repar_dano_amb_especific,
				 firmou_termo_comprom,
				 firmou_termo_comprom_especific,
				 arquivo_id,
				 arquivo_tid,
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
				(seq_hst_acompanhamento_fisc.nextval,
				 i.id,
				 i.fiscalizacao,
				 i.fiscalizacao_tid,
				 i.numero_sufixo,
				 i.data_vistoria,
				 i.situacao,
				 i.situacao_texto,
				 i.data_situacao,
				 i.agente_fiscal,
				 i.agente_fiscal_tid,
				 i.setor,
				 i.setor_tid,
				 i.area_total,
				 i.area_florestal_nativa,
				 i.reserva_legal,
				 i.possui_area_embargada,
				 i.opniao_area_embargo,
				 i.ativ_area_embargada,
				 i.atviv_area_embargada_especific,
				 i.uso_area_solo,
				 i.caract_solo_area_danificada,
				 i.declividade_media_area,
				 i.infr_resultou_erosao,
				 i.infr_resultou_erosao_especific,
				 i.houve_apreensao_material,
				 i.opniao_destin_material_apreend,
				 i.houve_desrespeito_tad,
				 i.houve_desrespeito_tad_especifi,
				 i.informacoes_relevante_processo,
				 i.neces_repar_dano_amb,
				 i.neces_repar_dano_amb_especific,
				 i.firmou_termo_comprom,
				 i.firmou_termo_comprom_especific,
				 i.arquivo,
				 i.arquivo_tid,
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
					 and la.artefato = 101),
				 systimestamp)
			returning id into v_id;
		
			---------------------------------------------------------
			-- Acompanhamento da Fiscalização - Assinante
			--------------------------------------------------------- 
		
			for j in (select t.id,
							 t.funcionario,
							 f.tid         funcionario_tid,
							 t.cargo,
							 c.tid         cargo_tid
						from tab_acomp_fisc_assinante t,
							 tab_funcionario          f,
							 tab_cargo                c
					   where t.funcionario = f.id
						 and t.cargo = c.id
						 and t.acompanhamento = i.id) loop
			
				insert into hst_acomp_fisc_assinante
					(id,
					 id_hst,
					 tid,
					 assinantes_id,
					 acompanhamento_id,
					 funcionario_id,
					 funcionario_tid,
					 cargo_id,
					 cargo_tid)
				values
					(seq_hst_acomp_fisc_assinante.nextval,
					 v_id,
					 i.tid,
					 j.id,
					 i.id,
					 j.funcionario,
					 j.funcionario_tid,
					 j.cargo,
					 j.cargo_tid);
			end loop;
			---------------------------------------------------------
		
			---------------------------------------------------------
			-- Acompanhamento da Fiscalização - Arquivo
			--------------------------------------------------------- 
		
			for j in (select t.id,
							 t.arquivo,
							 a.tid arquivo_tid,
							 t.ordem,
							 t.descricao
						from tab_acomp_fisc_arquivo t, tab_arquivo a
					   where t.arquivo = a.id
						 and t.acompanhamento = i.id) loop
			
				insert into hst_acomp_fisc_arquivo
					(id,
					 id_hst,
					 tid,
					 acompanhamento_arq_id,
					 acompanhamento_id,
					 arquivo_id,
					 arquivo_tid,
					 ordem,
					 descricao)
				values
					(seq_hst_acomp_fisc_arquivo.nextval,
					 v_id,
					 i.tid,
					 j.id,
					 i.id,
					 j.arquivo,
					 j.arquivo_tid,
					 j.ordem,
					 j.descricao);
			end loop;
			---------------------------------------------------------
		
		end loop;
		---------------------------------------------------------     
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Acompanhamento de Fiscalização. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Acompanhamento de Fiscalização. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
	---------------------------------------------------------

	---------------------------------------------------------
	-- Cadastro Ambiental Rural - Solicitacao 
	---------------------------------------------------------
	procedure carsolicitacao(p_id               number,
							 p_acao             number,
							 p_executor_id      number,
							 p_executor_nome    varchar2,
							 p_executor_login   varchar2,
							 p_executor_tipo_id number,
							 p_executor_tid     varchar2) is
		v_sucesso           boolean := false;
		v_id                number;
		v_dominialidade_tid varchar(36);
		v_dominialidade_id  number;
		v_projeto_geo_id    number;
		v_projeto_geo_tid   varchar(36);
	
	begin
		---------------------------------------------------------
		-- Solicitacao
		---------------------------------------------------------
	
		for i in (select s.id,
						 s.tid,
						 s.numero,
						 s.data_emissao,
						 l.id                     situacao_id,
						 l.texto                  situacao_texto,
						 proc.id                  protocolo_id,
						 proc.tid                 protocolo_tid,
						 r.id                     requerimento_id,
						 r.tid                    requerimento_tid,
						 proc_s.id                protocolo_selecionado_id,
						 proc_s.tid               protocolo_selecionado_tid,
						 a.id                     atividade_id,
						 a.tid                    atividade_tid,
						 e.id                     empreendimento_id,
						 e.tid                    empreendimento_tid,
						 p.id                     declarante_id,
						 p.tid                    declarante_tid,
						 f.id                     autor_id,
						 f.tid                    autor_tid,
						 s.situacao_data,
						 s.situacao_anterior_data,
						 s.situacao_anterior      situacao_anterior_id,
						 ls.texto                 situacao_anterior_texto,
						 s.motivo,
						 d.id                     dominialidade_id,
						 d.tid                    dominialidade_tid,
						 pg.id                    projeto_geo_id,
						 pg.tid                   projeto_geo_tid
					from tab_car_solicitacao          s,
						 lov_car_solicitacao_situacao l,
						 lov_car_solicitacao_situacao ls,
						 tab_protocolo                proc,
						 tab_protocolo                proc_s,
						 tab_requerimento             r,
						 tab_empreendimento           e,
						 tab_atividade                a,
						 tab_pessoa                   p,
						 tab_funcionario              f,
						 crt_dominialidade            d,
						 crt_projeto_geo              pg
				   where s.situacao = l.id
					 and s.situacao_anterior = ls.id(+)
					 and s.protocolo = proc.id
					 and s.empreendimento = e.id
					 and proc_s.id = s.protocolo_selecionado
					 and a.id = s.atividade
					 and r.id = s.requerimento
					 and p.id = s.declarante
					 and f.id = s.autor
					 and e.id = d.empreendimento(+)
					 and e.id = pg.empreendimento(+)
					 and pg.caracterizacao = 1
					 and s.id = p_id) loop
		
			select i.dominialidade_tid,
				   i.dominialidade_id,
				   i.projeto_geo_id,
				   i.projeto_geo_tid
			  into v_dominialidade_tid,
				   v_dominialidade_id,
				   v_projeto_geo_id,
				   v_projeto_geo_tid
			  from dual;
		
			if (i.situacao_id != 1 and i.situacao_id != 2) then
				select h.dominialidade_tid,
					   h.dominialidade_id,
					   h.projeto_geo_id,
					   h.projeto_geo_tid
				  into v_dominialidade_tid,
					   v_dominialidade_id,
					   v_projeto_geo_id,
					   v_projeto_geo_tid
				  from hst_car_solicitacao h
				 where h.solicitacao_id = i.id
				   and h.id = (select id
								 from (select last_value(hh.id) over(order by hh.data_execucao) id
										 from hst_car_solicitacao hh
										where hh.solicitacao_id = i.id) a
								where rownum = 1);
			end if;
		
			insert into hst_car_solicitacao
				(id,
				 solicitacao_id,
				 tid,
				 numero,
				 data_emissao,
				 situacao_id,
				 situacao_texto,
				 protocolo_id,
				 protocolo_tid,
				 requerimento_id,
				 requerimento_tid,
				 protocolo_selecionado_id,
				 protocolo_selecionado_tid,
				 atividade_id,
				 atividade_tid,
				 empreendimento_id,
				 empreendimento_tid,
				 declarante_id,
				 declarante_tid,
				 autor_id,
				 autor_tid,
				 situacao_data,
				 situacao_anterior_data,
				 situacao_anterior_id,
				 situacao_anterior_texto,
				 motivo,
				 dominialidade_id,
				 dominialidade_tid,
				 projeto_geo_id,
				 projeto_geo_tid,
				 executor_id,
				 executor_tid,
				 executor_nome,
				 executor_login,
				 executor_tipo_id,
				 executor_tipo_texto,
				 acao_executada,
				 data_execucao)
			values
				(seq_hst_car_solicitacao.nextval,
				 i.id,
				 i.tid,
				 i.numero,
				 i.data_emissao,
				 i.situacao_id,
				 i.situacao_texto,
				 i.protocolo_id,
				 i.protocolo_tid,
				 i.requerimento_id,
				 i.requerimento_tid,
				 i.protocolo_selecionado_id,
				 i.protocolo_selecionado_tid,
				 i.atividade_id,
				 i.atividade_tid,
				 i.empreendimento_id,
				 i.empreendimento_tid,
				 i.declarante_id,
				 i.declarante_tid,
				 i.autor_id,
				 i.autor_tid,
				 i.situacao_data,
				 i.situacao_anterior_data,
				 i.situacao_anterior_id,
				 i.situacao_anterior_texto,
				 i.motivo,
				 v_dominialidade_id,
				 v_dominialidade_tid,
				 v_projeto_geo_id,
				 v_projeto_geo_tid,
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
					 and la.artefato = 102),
				 systimestamp)
			returning id into v_id;
		end loop;
		---------------------------------------------------------     
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Solicitacao de Cadastro Ambiental Rural. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Solicitacao de Cadastro Ambiental Rural. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
	---------------------------------------------------------

	---------------------------------------------------------
	-- Órgãos Parceiros/ Conveniados 
	---------------------------------------------------------
	procedure orgaoparceiroconveniado(p_id               number,
									  p_acao             number,
									  p_executor_id      number,
									  p_executor_nome    varchar2,
									  p_executor_login   varchar2,
									  p_executor_tipo_id number,
									  p_executor_tid     varchar2) is
		v_sucesso boolean := false;
		v_id      number;
	begin
		---------------------------------------------------------
		-- Órgãos Parceiros/ Conveniados
		---------------------------------------------------------
	
		for i in (select t.id                  orgao_parc_conv_id,
						 t.orgao_sigla,
						 t.orgao_nome,
						 t.termo_numero_ano,
						 t.diario_oficial_data,
						 t.situacao            situacao_id,
						 l.texto               situacao_texto,
						 t.situacao_motivo,
						 t.situacao_data,
						 t.tid
					from tab_orgao_parc_conv          t,
						 lov_orgao_parc_conv_situacao l
				   where l.id = t.situacao
					 and t.id = p_id) loop
		
			insert into hst_orgao_parc_conv
				(id,
				 orgao_parc_conv_id,
				 tid,
				 orgao_sigla,
				 orgao_nome,
				 termo_numero_ano,
				 diario_oficial_data,
				 situacao_id,
				 situacao_texto,
				 situacao_motivo,
				 situacao_data,
				 executor_id,
				 executor_tid,
				 executor_nome,
				 executor_login,
				 executor_tipo_id,
				 executor_tipo_texto,
				 acao_executada,
				 data_execucao)
			values
				(seq_hst_orgao_parc_conv.nextval,
				 i.orgao_parc_conv_id,
				 i.tid,
				 i.orgao_sigla,
				 i.orgao_nome,
				 i.termo_numero_ano,
				 i.diario_oficial_data,
				 i.situacao_id,
				 i.situacao_texto,
				 i.situacao_motivo,
				 i.situacao_data,
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
					 and la.artefato = 106),
				 systimestamp)
			returning id into v_id;
		
			---------------------------------------------------------
			-- Sigla/ Unidades
			--------------------------------------------------------- 
		
			for j in (select u.id,
							 u.orgao_parc_conv,
							 u.sigla,
							 u.nome_local,
							 u.tid
						from tab_orgao_parc_conv_sigla_unid u
					   where u.orgao_parc_conv = i.orgao_parc_conv_id) loop
			
				insert into hst_orgao_parc_conv_sigla_unid
					(id,
					 id_hst,
					 tid,
					 sigla_unid_id,
					 orgao_parc_conv_id,
					 orgao_parc_conv_tid,
					 sigla,
					 nome_local)
				values
					(seq_hst_orgao_parc_conv_sigl_u.nextval,
					 v_id,
					 j.tid,
					 j.id,
					 j.orgao_parc_conv,
					 i.tid,
					 j.sigla,
					 j.nome_local);
			end loop;
			---------------------------------------------------------
		
		end loop;
		---------------------------------------------------------     
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Órgãos Parceiros/ Conveniados. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Órgãos Parceiros/ Conveniados. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;

	---------------------------------------------------------

	---------------------------------------------------------
	-- Profissão 
	---------------------------------------------------------
	procedure profissao(p_id               number,
						p_acao             number,
						p_executor_id      number,
						p_executor_nome    varchar2,
						p_executor_login   varchar2,
						p_executor_tipo_id number,
						p_executor_tid     varchar2) is
		v_sucesso boolean := false;
	begin
		for i in (select p.id,
						 p.texto,
						 p.codigo,
						 p.tid,
						 p.origem,
						 o.texto origem_texto
					from tab_profissao p, lov_profissao_origem o
				   where p.origem = o.id(+)
					 and p.id = p_id) loop
		
			insert into hst_profissao p
				(p.id,
				 p.profissao_id,
				 p.texto,
				 p.codigo,
				 p.origem_id,
				 p.origem_texto,
				 p.tid,
				 p.executor_id,
				 p.executor_tid,
				 p.executor_nome,
				 p.executor_login,
				 p.executor_tipo_id,
				 p.executor_tipo_texto,
				 p.acao_executada,
				 p.data_execucao)
			VALUES
				(seq_hst_profissao.nextval,
				 i.id,
				 i.texto,
				 i.codigo,
				 i.origem,
				 i.origem_texto,
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
					 and la.artefato = 109),
				 systimestamp);
		end loop;
	
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Profissão. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
	
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Profissão. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
	---------------------------------------------------------

	---------------------------------------------------------
	-- Configuração Vegetal - Grupo Químico 
	---------------------------------------------------------
	procedure grupoquimico(p_id               number,
						   p_acao             number,
						   p_executor_id      number,
						   p_executor_nome    varchar2,
						   p_executor_login   varchar2,
						   p_executor_tipo_id number,
						   p_executor_tid     varchar2) is
		v_sucesso boolean := false;
	begin
		for i in (select t.id, t.texto, t.tid
					from tab_grupo_quimico t
				   where t.id = p_id) loop
			insert into hst_grupo_quimico
				(id,
				 grupo_quimico_id,
				 texto,
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
				(seq_hst_grupo_quimico.nextval,
				 i.id,
				 i.texto,
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
					 and la.artefato = 113),
				 systimestamp);
		end loop;
	
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Grupo Químico. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
	
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Grupo Químico. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
	---------------------------------------------------------

	---------------------------------------------------------
	-- Configuração Vegetal - Classe de Uso
	---------------------------------------------------------
	procedure classeuso(p_id               number,
						p_acao             number,
						p_executor_id      number,
						p_executor_nome    varchar2,
						p_executor_login   varchar2,
						p_executor_tipo_id number,
						p_executor_tid     varchar2) is
		v_sucesso boolean := false;
	begin
		for i in (select t.id, t.texto, t.tid
					from tab_classe_uso t
				   where t.id = p_id) loop
			insert into hst_classe_uso
				(id,
				 classe_uso_id,
				 texto,
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
				(seq_hst_classe_uso.nextval,
				 i.id,
				 i.texto,
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
					 and la.artefato = 114),
				 systimestamp);
		
		end loop;
	
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Classe de Uso. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
	
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Classe de Uso. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
	---------------------------------------------------------

	---------------------------------------------------------
	-- Configuração Vegetal - Periculosidade Ambiental
	---------------------------------------------------------
	procedure periculosidadeambiental(p_id               number,
									  p_acao             number,
									  p_executor_id      number,
									  p_executor_nome    varchar2,
									  p_executor_login   varchar2,
									  p_executor_tipo_id number,
									  p_executor_tid     varchar2) is
		v_sucesso boolean := false;
	begin
		for i in (select t.id, t.texto, t.tid
					from tab_peric_ambiental t
				   where t.id = p_id) loop
			insert into hst_peric_ambiental
				(id,
				 peric_ambiental_id,
				 texto,
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
				(seq_hst_peric_ambiental.nextval,
				 i.id,
				 i.texto,
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
					 and la.artefato = 115),
				 systimestamp);
		
		end loop;
	
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Periculosidade Ambiental. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
	
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Periculosidade Ambiental. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
	---------------------------------------------------------

	---------------------------------------------------------
	-- Configuração Vegetal - Classificação Toxicológica
	---------------------------------------------------------
	procedure classificacaotoxicologica(p_id               number,
										p_acao             number,
										p_executor_id      number,
										p_executor_nome    varchar2,
										p_executor_login   varchar2,
										p_executor_tipo_id number,
										p_executor_tid     varchar2) is
		v_sucesso boolean := false;
	begin
		for i in (select t.id, t.texto, t.tid
					from tab_class_toxicologica t
				   where t.id = p_id) loop
			insert into hst_class_toxicologica
				(id,
				 class_toxicologica_id,
				 texto,
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
				(seq_hst_class_toxicologica.nextval,
				 i.id,
				 i.texto,
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
					 and la.artefato = 116),
				 systimestamp);
		
		end loop;
	
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Classificação Toxicológica. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
	
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Classificação Toxicológica. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
	---------------------------------------------------------

	---------------------------------------------------------
	-- Configuração Vegetal - Modalidade de Aplicação
	---------------------------------------------------------
	procedure modalidadeaplicacao(p_id               number,
								  p_acao             number,
								  p_executor_id      number,
								  p_executor_nome    varchar2,
								  p_executor_login   varchar2,
								  p_executor_tipo_id number,
								  p_executor_tid     varchar2) is
		v_sucesso boolean := false;
	begin
		for i in (select t.id, t.texto, t.tid
					from tab_modalidade_aplicacao t
				   where t.id = p_id) loop
			insert into hst_modalidade_aplicacao
				(id,
				 modalidade_aplicacao_id,
				 texto,
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
				(seq_hst_modalidade_aplicacao.nextval,
				 i.id,
				 i.texto,
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
					 and la.artefato = 117),
				 systimestamp);
		end loop;
	
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Modalidade de Aplicação. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
	
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Modalidade de Aplicação. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
	---------------------------------------------------------

	---------------------------------------------------------
	-- Configuração Vegetal - Forma apresentação
	---------------------------------------------------------
	procedure formaapresentacao(p_id               number,
								p_acao             number,
								p_executor_id      number,
								p_executor_nome    varchar2,
								p_executor_login   varchar2,
								p_executor_tipo_id number,
								p_executor_tid     varchar2) is
		v_sucesso boolean := false;
	begin
		for i in (select t.id, t.texto, t.tid
					from tab_forma_apresentacao t
				   where t.id = p_id) loop
			insert into hst_forma_apresentacao
				(id,
				 forma_apresentacao_id,
				 texto,
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
				(seq_hst_forma_apresentacao.nextval,
				 i.id,
				 i.texto,
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
					 and la.artefato = 118),
				 systimestamp);
		end loop;
	
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Forma apresentação. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
	
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Forma apresentação. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
	---------------------------------------------------------

	---------------------------------------------------------
	-- Configuração Vegetal - Ingrediente Ativo
	---------------------------------------------------------
	procedure ingredienteativo(p_id               number,
							   p_acao             number,
							   p_executor_id      number,
							   p_executor_nome    varchar2,
							   p_executor_login   varchar2,
							   p_executor_tipo_id number,
							   p_executor_tid     varchar2) is
		v_sucesso boolean := false;
	begin
		for i in (select t.id,
						 t.tid,
						 t.situacao,
						 ls.texto situacao_texto,
						 t.motivo,
						 t.texto
					from tab_ingrediente_ativo          t,
						 lov_ingrediente_ativo_situacao ls
				   where t.situacao = ls.id
					 and t.id = p_id) loop
			insert into hst_ingrediente_ativo
				(id,
				 tid,
				 ingrediente_ativo_id,
				 texto,
				 situacao_id,
				 situacao_texto,
				 motivo,
				 executor_id,
				 executor_tid,
				 executor_nome,
				 executor_login,
				 executor_tipo_id,
				 executor_tipo_texto,
				 acao_executada,
				 data_execucao)
			values
				(seq_hst_ingrediente_ativo.nextval,
				 i.tid,
				 i.id,
				 i.texto,
				 i.situacao,
				 i.situacao_texto,
				 i.motivo,
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
					 and la.artefato = 119),
				 systimestamp);
		end loop;
	
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Ingrediente Ativo. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
	
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Ingrediente Ativo. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
	---------------------------------------------------------

	---------------------------------------------------------
	-- Configuração Vegetal - Cultura
	---------------------------------------------------------
	procedure cultura(p_id               number,
					  p_acao             number,
					  p_executor_id      number,
					  p_executor_nome    varchar2,
					  p_executor_login   varchar2,
					  p_executor_tipo_id number,
					  p_executor_tid     varchar2) is
		v_sucesso boolean := false;
		v_id      number := 0;
	begin
	
		for i in (select t.id, t.texto, t.tid
					from tab_cultura t
				   where t.id = p_id) loop
			insert into hst_cultura
				(id,
				 cultura_id,
				 texto,
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
				(seq_hst_cultura.nextval,
				 i.id,
				 i.texto,
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
					 and la.artefato = 120),
				 systimestamp)
			returning id into v_id;
		
			for j in (select cc.id,
							 cc.cultivar,
							 cc.cultura,
							 c.texto as cultura_texto,
							 cc.tid
						from tab_cultura_cultivar cc, tab_cultura c
					   where c.id = cc.cultura
						 and cc.cultura = p_id) loop
				insert into hst_cultura_cultivar
					(id,
					 tid,
					 id_hst,
					 cultivar_id,
					 cultivar_nome,
					 cultura_id,
					 cultura_tid)
				values
					(seq_hst_cultura_cultivar.nextval,
					 j.tid,
					 v_id,
					 j.id,
					 j.cultivar,
					 j.cultura,
					 i.tid);
			
				for x in (select cf.id,
								 cf.tid,
								 cf.cultivar,
								 cc.cultivar as cultivar_texto,
								 cf.praga,
								 pg.nome_cientifico || '-' || pg.nome_comum as praga_texto,
								 cf.tipo_producao,
								 tp.texto as tipo_producao_texto,
								 cf.declaracao_adicional,
								 dd.texto as declaracao_adicional_texto
							from tab_cultivar_configuracao      cf,
								 tab_cultura_cultivar           cc,
								 tab_praga                      pg,
								 lov_cultivar_tipo_producao     tp,
								 lov_cultivar_declara_adicional dd
						   where cc.id = cf.cultivar
							 and pg.id = cf.praga
							 and tp.id = cf.tipo_producao
							 and dd.id = cf.declaracao_adicional
							 and cf.cultivar = j.id) loop
					insert into hst_cultivar_configuracao
						(id,
						 tid,
						 id_hst,
						 cultivar_conf_id,
						 cultivar_id,
						 praga_id,
						 praga_texto,
						 tipo_producao_id,
						 tipo_producao_texto,
						 declaracao_adicional_id,
						 declaracao_adicional_texto)
					values
						(seq_hst_cultivar_config.nextval,
						 x.tid,
						 seq_hst_cultura_cultivar.currval,
						 x.id,
						 x.cultivar,
						 x.praga,
						 x.praga_texto,
						 x.tipo_producao,
						 x.tipo_producao_texto,
						 x.declaracao_adicional,
						 x.declaracao_adicional_texto);
				end loop;
			end loop;
		
		end loop;
	
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Cultura. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
	
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Cultura. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;

	---------------------------------------------------------

	---------------------------------------------------------
	-- Configuração Vegetal - Praga
	---------------------------------------------------------
	procedure praga(p_id               number,
					p_acao             number,
					p_executor_id      number,
					p_executor_nome    varchar2,
					p_executor_login   varchar2,
					p_executor_tipo_id number,
					p_executor_tid     varchar2) is
		v_sucesso boolean := false;
		v_id      number := 0;
	begin
		for i in (select t.id, t.nome_cientifico, t.nome_comum, t.tid
					from tab_praga t
				   where t.id = p_id) loop
			insert into hst_praga
				(id,
				 praga_id,
				 nome_cientifico,
				 nome_comum,
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
				(seq_hst_praga.nextval,
				 i.id,
				 i.nome_cientifico,
				 i.nome_comum,
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
					 and la.artefato = 121),
				 systimestamp)
			returning id into v_id;
		
			for j in (select pc.id, pc.praga, pc.cultura, pc.tid
						from tab_praga_cultura pc
					   where pc.praga = p_id) loop
				insert into hst_praga_cultura
					(id,
					 tid,
					 id_hst,
					 praga_cultura_id,
					 praga_id,
					 praga_tid,
					 cultura_id,
					 cultura_tid)
				values
					(seq_hst_praga_cultura.nextval,
					 j.tid,
					 v_id,
					 j.praga,
					 j.id,
					 (select tid from tab_praga where id = j.praga),
					 j.cultura,
					 (select tid from tab_cultura where id = j.cultura));
			end loop;
		end loop;
	
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Praga. Mensagem: ' ||
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
	-- Credenciado - Habilitar Emissão de CFO/CFOC
	---------------------------------------------------------
	procedure habilitaremissaocfocfoc(p_id               number,
									  p_acao             number,
									  p_executor_id      number,
									  p_executor_nome    varchar2,
									  p_executor_login   varchar2,
									  p_executor_tipo_id number,
									  p_executor_tid     varchar2) is
		v_sucesso boolean := false;
		v_id      number := 0;
	begin
		for i in (select t.id,
						 responsavel,
						 c.tid                  responsavel_tid,
						 responsavel_arquivo,
						 a.tid                  responsavel_arquivo_tid,
						 numero_habilitacao,
						 validade_registro,
						 t.situacao,
             t.situacao_data,
						 ls.texto               situacao_texto,
						 motivo,
						 lm.texto               motivo_texto,
						 observacao,
						 numero_dua,
						 extensao_habilitacao,
						 numero_habilitacao_ori,
						 registro_crea,
						 uf,
						 numero_visto_crea,
             numero_processo,
						 t.tid
					from tab_hab_emi_cfo_cfoc         t,
						 tab_credenciado              c,
						 tab_arquivo                  a,
						 lov_hab_emissao_cfo_situacao ls,
						 lov_hab_emissao_cfo_motivo   lm
				   where t.situacao = ls.id
					 and c.id = t.responsavel
					 and t.responsavel_arquivo = a.id(+)
					 and t.motivo = lm.id(+)
					 and t.id = p_id) loop
		
			insert into hst_hab_emi_cfo_cfoc
				(id,
				 tid,
				 habilitar_emissao_id,
				 responsavel_id,
				 responsavel_tid,
				 responsavel_arquivo_id,
				 responsavel_arquivo_tid,
				 numero_habilitacao,
				 validade_registro,
				 situacao_id,
				 situacao_texto,
         situacao_data,
				 motivo_id,
				 motivo_texto,
				 observacao,
				 numero_dua,
				 extensao_habilitacao,
				 numero_habilitacao_ori,
				 registro_crea,
				 uf,
				 numero_visto_crea,
				 executor_id,
				 executor_tid,
				 executor_nome,
				 executor_login,
				 executor_tipo_id,
				 executor_tipo_texto,
				 acao_executada,
				 data_execucao,
         numero_processo)
			values
				(seq_hst_hab_emi_cfo_cfoc.nextval,
				 i.tid,
				 i.id,
				 i.responsavel,
				 i.responsavel_tid,
				 i.responsavel_arquivo,
				 i.responsavel_arquivo_tid,
				 i.numero_habilitacao,
				 i.validade_registro,
				 i.situacao,
				 i.situacao_texto,
         i.situacao_data,
				 i.motivo,
				 i.motivo_texto,
				 i.observacao,
				 i.numero_dua,
				 i.extensao_habilitacao,
				 i.numero_habilitacao_ori,
				 i.registro_crea,
				 i.uf,
				 i.numero_visto_crea,
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
					 and la.artefato = 122),
				 systimestamp,
         i.numero_processo)
			returning id into v_id;
		
			for j in (select id,
							 tid,
							 habilitar_emi_id,
							 praga,
							 data_habilitacao_inicial,
							 data_habilitacao_final
						from tab_hab_emi_cfo_cfoc_praga c
					   where c.habilitar_emi_id = p_id) loop
				insert into hst_hab_emi_cfo_cfoc_praga
					(id,
					 tid,
					 id_hst,
					 hab_emi_praga_id,
					 habilitar_emi_id,
					 praga_id,
					 praga_tid,
					 data_habilitacao_inicial,
					 data_habilitacao_final)
				values
					(seq_hst_hab_emi_cfo_cfoc_praga.nextval,
					 i.tid,
					 v_id,
					 j.id,
					 i.id,
					 j.praga,
					 (select tid from tab_praga where id = j.praga),
					 j.data_habilitacao_inicial,
					 j.data_habilitacao_final);
			end loop;
		
		end loop;
	
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Habilitar Emissão de CFO/CFOC. Mensagem: ' ||
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
	-- Agrotóxico 
	---------------------------------------------------------
	procedure agrotoxico(p_id               number,
						 p_acao             number,
						 p_executor_id      number,
						 p_executor_nome    varchar2,
						 p_executor_login   varchar2,
						 p_executor_tipo_id number,
						 p_executor_tid     varchar2) is
		v_sucesso boolean := false;
		v_id      number;
		v_aux     number;
	begin
		---------------------------------------------------------
		-- Agrotóxico
		---------------------------------------------------------
	
		for i in (select t.id                         agrotoxico_id,
						 t.tid                        agrotoxico_tid,
						 t.possui_cadastro,
						 t.numero_cadastro,
						 t.cadastro_ativo,
						 t.motivo,
						 lm.texto                     motivo_texto,
						 t.nome_comercial,
						 t.numero_registro_ministerio,
						 t.numero_processo_sep,
						 tr.id                        titular_registro_id,
						 tr.tid                       titular_registro_tid,
						 ct.id                        classificacao_toxicologica_id,
						 ct.tid                       classificacao_toxicologica_tid,
						 pa.id                        periculosidade_ambiental_id,
						 pa.tid                       periculosidade_ambiental_tid,
						 fa.id                        forma_apresentacao_id,
						 fa.tid                       forma_apresentacao_tid,
						 t.observacao_interna,
						 t.observacao_geral,
						 a.id                         arquivo_id,
						 a.tid                        arquivo_tid
					from tab_agrotoxico         t,
						 lov_agrotoxico_motivo  lm,
						 tab_pessoa             tr,
						 tab_class_toxicologica ct,
						 tab_peric_ambiental    pa,
						 tab_forma_apresentacao fa,
						 tab_arquivo            a
				   where lm.id(+) = t.motivo
					 and tr.id = t.titular_registro
					 and ct.id = t.classificacao_toxicologica
					 and pa.id = t.periculosidade_ambiental
					 and fa.id = t.forma_apresentacao
					 and a.id(+) = t.arquivo
					 and t.id = p_id) loop
		
			insert into hst_agrotoxico
				(id,
				 tid,
				 agrotoxico_id,
				 possui_cadastro,
				 numero_cadastro,
				 cadastro_ativo,
				 motivo_id,
				 motivo_texto,
				 nome_comercial,
				 numero_registro_ministerio,
				 numero_processo_sep,
				 titular_registro_id,
				 titular_registro_tid,
				 classificacao_toxicologica_id,
				 classificacao_toxicologica_tid,
				 periculosidade_ambiental_id,
				 periculosidade_ambiental_tid,
				 forma_apresentacao_id,
				 forma_apresentacao_tid,
				 observacao_interna,
				 observacao_geral,
				 arquivo_id,
				 executor_id,
				 executor_tid,
				 executor_nome,
				 executor_login,
				 executor_tipo_id,
				 executor_tipo_texto,
				 acao_executada,
				 data_execucao)
			
			values
				(seq_hst_agrotoxico.nextval,
				 i.agrotoxico_tid,
				 i.agrotoxico_id,
				 i.possui_cadastro,
				 i.numero_cadastro,
				 i.cadastro_ativo,
				 i.motivo,
				 i.motivo_texto,
				 i.nome_comercial,
				 i.numero_registro_ministerio,
				 i.numero_processo_sep,
				 i.titular_registro_id,
				 i.titular_registro_tid,
				 i.classificacao_toxicologica_id,
				 i.classificacao_toxicologica_tid,
				 i.periculosidade_ambiental_id,
				 i.periculosidade_ambiental_tid,
				 i.forma_apresentacao_id,
				 i.forma_apresentacao_tid,
				 i.observacao_interna,
				 i.observacao_geral,
				 i.arquivo_id,
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
					 and la.artefato = 129),
				 systimestamp)
			returning id into v_id;
		
			---------------------------------------------------------
			-- Ingrediente Ativo
			---------------------------------------------------------         
			for j in (select t.id,
							 t.tid,
							 ia.id                  ingrediente_ativo_id,
							 ia.tid                 ingrediente_ativo_tid,
							 t.concentracao,
							 t.unidade_medida,
							 lu.texto               unidade_medida_texto,
							 t.unidade_medida_outro
						from tab_agrotoxico_ing_ativo  t,
							 tab_ingrediente_ativo     ia,
							 lov_agrotoxico_uni_medida lu
					   where ia.id = t.ingrediente_ativo
						 and t.unidade_medida = lu.id
						 and agrotoxico = i.agrotoxico_id) loop
			
				insert into hst_agrotoxico_ing_ativo
					(id,
					 id_hst,
					 tid,
					 agrotox_ingrediente_ativo_id,
					 agrotoxico_id,
					 ingrediente_ativo_id,
					 ingrediente_ativo_tid,
					 concentracao,
					 unidade_medida_id,
					 unidade_medida_texto,
					 unidade_medida_outro)
				values
					(seq_hst_agrotoxico_ing_ativo.nextval,
					 v_id,
					 j.tid,
					 j.id,
					 i.agrotoxico_id,
					 j.ingrediente_ativo_id,
					 j.ingrediente_ativo_tid,
					 j.concentracao,
					 j.unidade_medida,
					 j.unidade_medida_texto,
					 j.unidade_medida_outro);
			end loop;
			---------------------------------------------------------  
		
			---------------------------------------------------------
			-- Classe Uso
			---------------------------------------------------------         
			for j in (select t.id,
							 cl.id  classe_uso_id,
							 cl.tid classe_uso_tid,
							 t.tid
						from tab_agrotoxico_classe_uso t, tab_classe_uso cl
					   where cl.id = t.classe_uso
						 and t.agrotoxico = i.agrotoxico_id) loop
			
				insert into hst_agrotoxico_classe_uso
					(id,
					 id_hst,
					 agrotoxico_classe_uso_id,
					 agrotoxico_id,
					 classe_uso_id,
					 classe_uso_tid,
					 tid)
				values
					(seq_hst_agrotoxico_classe_uso.nextval,
					 v_id,
					 j.id,
					 i.agrotoxico_id,
					 j.classe_uso_id,
					 j.classe_uso_tid,
					 j.tid);
			end loop;
			---------------------------------------------------------  
		
			---------------------------------------------------------
			-- Grupo Quimico
			---------------------------------------------------------         
			for j in (select t.id,
							 gq.id  grupo_quimico_id,
							 gq.tid grupo_quimico_tid,
							 t.tid
						from tab_agrotoxico_grupo_quimico t,
							 tab_grupo_quimico            gq
					   where gq.id = t.grupo_quimico
						 and t.agrotoxico = i.agrotoxico_id) loop
			
				insert into hst_agrotoxico_grupo_quimico
					(id,
					 id_hst,
					 agrotoxico_grupo_quimico_id,
					 agrotoxico_id,
					 grupo_quimico_id,
					 grupo_quimico_tid,
					 tid)
				values
					(seq_hst_agrotox_grupo_quimico.nextval,
					 v_id,
					 j.id,
					 i.agrotoxico_id,
					 j.grupo_quimico_id,
					 j.grupo_quimico_tid,
					 j.tid);
			end loop;
			---------------------------------------------------------  
		
			---------------------------------------------------------
			-- Cultura
			---------------------------------------------------------         
			for j in (select t.id,
							 c.id                  cultura_id,
							 c.tid                 cultura_tid,
							 t.intervalo_seguranca,
							 t.tid
						from tab_agrotoxico_cultura t, tab_cultura c
					   where c.id = t.cultura
						 and t.agrotoxico = i.agrotoxico_id) loop
			
				insert into hst_agrotoxico_cultura
					(id,
					 id_hst,
					 agrotoxico_cultura_id,
					 agrotoxico_id,
					 cultura_id,
					 cultura_tid,
					 intervalo_seguranca,
					 tid)
				values
					(seq_hst_agrotoxico_cultura.nextval,
					 v_id,
					 j.id,
					 i.agrotoxico_id,
					 j.cultura_id,
					 j.cultura_tid,
					 j.intervalo_seguranca,
					 j.tid)
				returning id into v_aux;
			
				---------------------------------------------------------
				-- Cultura/ Praga
				---------------------------------------------------------         
				for x in (select t.id, p.id praga_id, p.tid praga_tid, t.tid
							from tab_agrotoxico_cultura_praga t, tab_praga p
						   where p.id = t.praga
							 and t.agrotoxico_cultura = j.id) loop
				
					insert into hst_agrotoxico_cultura_praga
						(id,
						 id_hst,
						 agrotoxico_cultura_praga_id,
						 agrotoxico_cultura_id,
						 praga_id,
						 praga_tid,
						 tid)
					values
						(seq_hst_agrotox_cultura_praga.nextval,
						 v_aux,
						 x.id,
						 j.id,
						 x.praga_id,
						 x.praga_tid,
						 x.tid);
				end loop;
				---------------------------------------------------------
			
				---------------------------------------------------------
				-- Cultura/ Modalidades de Aplicações
				---------------------------------------------------------         
				for x in (select t.id,
								 ma.id  modalidade_aplicacao_id,
								 ma.tid modalidade_aplicacao_tid,
								 t.tid
							from tab_agro_cult_moda_aplicacao t,
								 tab_modalidade_aplicacao     ma
						   where ma.id = t.modalidade_aplicacao
							 and t.agrotoxico_cultura = j.id) loop
				
					insert into hst_agro_cult_moda_aplicacao
						(id,
						 id_hst,
						 agrotoxico_cultura_mod_apli_id,
						 agrotoxico_cultura_id,
						 modalidade_aplicacao_id,
						 modalidade_aplicacao_tid,
						 tid)
					values
						(seq_hst_agro_cult_moda_aplic.nextval,
						 v_aux,
						 x.id,
						 j.id,
						 x.modalidade_aplicacao_id,
						 x.modalidade_aplicacao_tid,
						 x.tid);
				end loop;
				---------------------------------------------------------                
			
			end loop;
			---------------------------------------------------------        
		
		end loop;
		---------------------------------------------------------     
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Agrotóxico. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Agrotóxico. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
	---------------------------------------------------------

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

	---------------------------------------------------------
	-- Liberacao de CFO/CFOC
	---------------------------------------------------------
	procedure liberacaocfocfoc(p_id               number,
							   p_acao             number,
							   p_executor_id      number,
							   p_executor_nome    varchar2,
							   p_executor_login   varchar2,
							   p_executor_tipo_id number,
							   p_executor_tid     varchar2) is
		v_sucesso boolean := false;
		v_id      number := 0;
	begin
		for i in (select id,
						 tid,
						 responsavel_tecnico,
						 (select c.tid
							from tab_credenciado c
						   where c.id = l.responsavel_tecnico) responsavel_tid,
						 liberar_bloco_cfo,
						 numero_inicial_cfo,
						 numero_final_cfo,
						 liberar_bloco_cfoc,
						 numero_inicial_cfoc,
						 numero_final_cfoc,
						 liberar_num_digital_cfo,
						 qtd_num_cfo,
						 liberar_num_digital_cfoc,
						 qtd_num_cfoc
					from tab_liberacao_cfo_cfoc l
				   where l.id = p_id) loop
		
			insert into hst_liberacao_cfo_cfoc
				(id,
				 tid,
				 liberacao_id,
				 responsavel_tecnico_id,
				 responsavel_tecnico_tid,
				 liberar_bloco_cfo,
				 numero_inicial_cfo,
				 numero_final_cfo,
				 liberar_bloco_cfoc,
				 numero_inicial_cfoc,
				 numero_final_cfoc,
				 liberar_num_digital_cfo,
				 qtd_num_cfo,
				 liberar_num_digital_cfoc,
				 qtd_num_cfoc,
				 executor_id,
				 executor_tid,
				 executor_nome,
				 executor_login,
				 executor_tipo_id,
				 executor_tipo_texto,
				 acao_executada,
				 data_execucao)
			values
				(seq_hst_liberacao_cfo_cfoc.nextval,
				 i.tid,
				 i.id,
				 i.responsavel_tecnico,
				 i.responsavel_tid,
				 i.liberar_bloco_cfo,
				 i.numero_inicial_cfo,
				 i.numero_final_cfo,
				 i.liberar_bloco_cfoc,
				 i.numero_inicial_cfoc,
				 i.numero_final_cfoc,
				 i.liberar_num_digital_cfo,
				 i.qtd_num_cfo,
				 i.liberar_num_digital_cfoc,
				 i.qtd_num_cfoc,
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
					 and la.artefato = 133),
				 systimestamp)
			returning id into v_id;
		
			for j in (select c.id,
							 c.tid,
							 c.tipo_numero,
							 c.tipo_documento,
							 c.numero,
							 c.situacao,
							 c.motivo,
							 c.utilizado
						from tab_numero_cfo_cfoc c
					   where c.liberacao = p_id) loop
			
				insert into hst_numero_cfo_cfoc
					(id,
					 numero_id,
					 tid,
					 tipo_numero,
					 tipo_documento,
					 numero,
					 liberacao_id,
					 liberacao_tid,
					 situacao,
					 utilizado,
					 motivo,
					 executor_id,
					 executor_tid,
					 executor_nome,
					 executor_login,
					 executor_tipo_id,
					 executor_tipo_texto,
					 acao_executada,
					 data_execucao)
				values
					(seq_hst_numero_cfo_cfoc.nextval,
					 j.id,
					 j.tid,
					 j.tipo_numero,
					 j.tipo_documento,
					 j.numero,
					 i.id,
					 i.tid,
					 j.situacao,
					 j.utilizado,
					 j.motivo,
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
						 and la.artefato = 135),
					 systimestamp);
			end loop;
		
		end loop;
	
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Liberação de CFO/CFOC. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
	
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Liberação de CFO/CFOC. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
	---------------------------------------------------------

	---------------------------------------------------------
	-- Números CFO e CFOC
	---------------------------------------------------------
	procedure numerocfocfoc(p_id               number,
							p_acao             number,
							p_executor_id      number,
							p_executor_nome    varchar2,
							p_executor_login   varchar2,
							p_executor_tipo_id number,
							p_executor_tid     varchar2) is
		v_sucesso boolean := false;
	begin
		for i in (select c.id,
						 c.tid,
						 c.tipo_numero,
						 c.tipo_documento,
						 c.numero,
						 c.situacao,
						 c.utilizado,
						 c.motivo,
						 c.liberacao,
						 l.tid liberacao_tid
					from tab_numero_cfo_cfoc c, tab_liberacao_cfo_cfoc l
				   where c.liberacao = l.id
					 and c.id = p_id) loop
		
			insert into hst_numero_cfo_cfoc
				(id,
				 numero_id,
				 tid,
				 tipo_numero,
				 tipo_documento,
				 numero,
				 liberacao_id,
				 liberacao_tid,
				 situacao,
				 utilizado,
				 motivo,
				 executor_id,
				 executor_tid,
				 executor_nome,
				 executor_login,
				 executor_tipo_id,
				 executor_tipo_texto,
				 acao_executada,
				 data_execucao)
			values
				(seq_hst_numero_cfo_cfoc.nextval,
				 i.id,
				 i.tid,
				 i.tipo_numero,
				 i.tipo_documento,
				 i.numero,
				 i.liberacao,
				 i.liberacao_tid,
				 i.situacao,
				 i.utilizado,
				 i.motivo,
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
					 and la.artefato = 135),
				 systimestamp);
		end loop;
	
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Liberação de CFO/CFOC. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
	
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Liberação de CFO/CFOC. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
	---------------------------------------------------------

	---------------------------------------------------------
	-- Destinatario PTV
	---------------------------------------------------------
	procedure destinatarioptv(p_id               number,
							  p_acao             number,
							  p_executor_id      number,
							  p_executor_nome    varchar2,
							  p_executor_login   varchar2,
							  p_executor_tipo_id number,
							  p_executor_tid     varchar2) is
		v_sucesso boolean := false;
		v_id      number := 0;
	begin
		for i in (select d.id,
						 d.tid,
						 d.tipo_pessoa,
						 d.nome,
						 d.cpf_cnpj,
						 d.endereco,
						 d.uf,
						 e.texto             uf_texto,
						 d.municipio,
						 m.texto             municipio_texto,
						 d.itinerario,
						 d.empreendimento_id,
						 te.tid              empreendimento_tid,
						 d.codigo_uc
					from tab_destinatario_ptv d,
						 lov_municipio        m,
						 lov_estado           e,
						 tab_empreendimento   te
				   where m.id = d.municipio
					 and e.id = d.uf
					 and d.empreendimento_id = te.id(+)
					 and d.id = p_id) loop
		
			insert into hst_destinatario_ptv
				(id,
				 tid,
				 destinatario_ptv_id,
				 tipo_pessoa,
				 cpf_cnpj,
				 nome,
				 endereco,
				 uf_id,
				 uf_texto,
				 municipio_id,
				 municipio_texto,
				 itinerario,
				 executor_id,
				 executor_tid,
				 executor_nome,
				 executor_login,
				 executor_tipo_id,
				 executor_tipo_texto,
				 acao_executada,
				 data_execucao,
				 empreendimento_id,
				 empreendimento_tid,
				 codigo_uc)
			values
				(seq_hst_destinatario_ptv.nextval,
				 i.tid,
				 i.id,
				 i.tipo_pessoa,
				 i.cpf_cnpj,
				 i.nome,
				 i.endereco,
				 i.uf,
				 i.uf_texto,
				 i.municipio,
				 i.municipio_texto,
				 i.itinerario,
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
					 and la.artefato = 134),
				 systimestamp,
				 i.empreendimento_id,
				 i.empreendimento_tid,
				 i.codigo_uc)
			returning id into v_id;
		end loop;
	
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de destinatario ptv. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
	
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de destinatario ptv. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;
	---------------------------------------------------------

	---------------------------------------------------------
	-- Habilitação para Emissão de PTV
	---------------------------------------------------------
	procedure habilitacaoemissaoptv(p_id               number,
									p_acao             number,
									p_executor_id      number,
									p_executor_nome    varchar2,
									p_executor_login   varchar2,
									p_executor_tipo_id number,
									p_executor_tid     varchar2) is
		v_sucesso boolean := false;
		v_id      number := 0;
	begin
		for i in (select t.tid,
						 t.id,
						 t.situacao,
						 t.funcionario,
						 tf.tid                  funcionario_tid,
						 t.arquivo,
						 ta.tid                  arquivo_tid,
						 t.numero_habilitacao,
						 t.rg,
						 t.numero_matricula,
						 t.profissao,
						 tp.tid                  profissao_tid,
						 t.orgao_classe,
						 toc.tid                 orgao_classe_tid,
						 t.registro_orgao_classe,
						 t.uf_habilitacao,
						 leh.texto               uf_habilitacao_texto,
						 t.numero_visto_crea,
						 t.numero_crea,
						 t.telefone_comercial,
						 t.telefone_residencial,
						 t.telefone_celular,
						 t.cep,
						 t.logradouro,
						 t.bairro_gleba,
						 t.estado,
						 le.texto                estado_texto,
						 t.municipio,
						 lm.texto                municipio_texto,
						 t.numero,
						 t.distrito_localidade,
						 t.complemento
					from tab_hab_emi_ptv  t,
						 tab_funcionario  tf,
						 tab_arquivo      ta,
						 tab_profissao    tp,
						 tab_orgao_classe toc,
						 lov_estado       leh,
						 lov_estado       le,
						 lov_municipio    lm
				   where tf.id = t.funcionario
					 and ta.id(+) = t.arquivo
					 and tp.id(+) = t.profissao
					 and toc.id(+) = t.orgao_classe
					 and leh.id = t.uf_habilitacao
					 and le.id = t.estado
					 and lm.id = t.municipio
					 and t.id = p_id) loop
		
			insert into hst_hab_emi_ptv
				(id,
				 tid,
				 habilitacao_id,
				 situacao,
				 funcionario_id,
				 funcionario_tid,
				 arquivo_id,
				 arquivo_tid,
				 numero_habilitacao,
				 rg,
				 numero_matricula,
				 profissao_id,
				 profissao_tid,
				 orgao_classe_id,
				 orgao_classe_tid,
				 registro_orgao_classe,
				 uf_habilitacao_id,
				 uf_habilitacao_texto,
				 numero_visto_crea,
				 numero_crea,
				 telefone_comercial,
				 telefone_residencial,
				 telefone_celular,
				 cep,
				 logradouro,
				 bairro_gleba,
				 estado_id,
				 estado_texto,
				 municipio_id,
				 municipio_texto,
				 numero,
				 distrito_localidade,
				 complemento,
				 executor_id,
				 executor_tid,
				 executor_nome,
				 executor_login,
				 executor_tipo_id,
				 executor_tipo_texto,
				 acao_executada,
				 data_execucao)
			values
				(seq_hst_hab_emi_ptv.nextval,
				 i.tid,
				 i.id,
				 i.situacao,
				 i.funcionario,
				 i.funcionario_tid,
				 i.arquivo,
				 i.arquivo_tid,
				 i.numero_habilitacao,
				 i.rg,
				 i.numero_matricula,
				 i.profissao,
				 i.profissao_tid,
				 i.orgao_classe,
				 i.orgao_classe_tid,
				 i.registro_orgao_classe,
				 i.uf_habilitacao,
				 i.uf_habilitacao_texto,
				 i.numero_visto_crea,
				 i.numero_crea,
				 i.telefone_comercial,
				 i.telefone_residencial,
				 i.telefone_celular,
				 i.cep,
				 i.logradouro,
				 i.bairro_gleba,
				 i.estado,
				 i.estado_texto,
				 i.municipio,
				 i.municipio_texto,
				 i.numero,
				 i.distrito_localidade,
				 i.complemento,
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
					 and la.artefato = 136),
				 systimestamp)
			returning id into v_id;
		
			for j in (select h.id,
							 h.tid,
							 h.funcionario,
							 f.tid funcionario_tid
						from tab_hab_emi_ptv_operador h, tab_funcionario f
					   where f.id = h.funcionario
						 and h.habilitacao = p_id) loop
				insert into hst_hab_emi_ptv_operador
					(id,
					 tid,
					 id_hst,
					 habilitacao_operador_id,
					 habilitacao_id,
					 funcionario_id,
					 funcionario_tid)
				values
					(seq_hst_hab_emi_ptv_operador.nextval,
					 j.tid,
					 v_id,
					 j.id,
					 i.id,
					 j.funcionario,
					 j.funcionario_tid);
			end loop;
		end loop;
	
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de habilitacao de emissao de ptv. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
	
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de habilitacao de emissao de ptv. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;

	---------------------------------------------------------
	-- Emitir Permissão de Transito de Vegetais
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
						 p.tipo_numero,
						 tn.texto                         tipo_numero_texto,
						 p.numero,
						 p.data_emissao,
						 p.data_ativacao,
						 p.data_cancelamento,
						 p.situacao,
						 ls.texto                         situacao_texto,
						 p.empreendimento,
						 e.tid                            empreendimento_tid,
						 p.responsavel_emp,
						 re.tid                           responsavel_emp_tid,
						 p.partida_lacrada_origem,
						 p.numero_lacre,
						 p.numero_porao,
						 p.numero_container,
						 p.destinatario,
						 d.tid                            destinatario_tid,
						 p.possui_laudo_laboratorial,
						 p.nome_laboratorio,
						 p.numero_laudo_resultado_analise,
						 p.estado,
						 le.sigla                         estado_texto,
						 p.municipio,
						 lm.texto                         municipio_texto,
						 p.tipo_transporte,
						 tp.texto                         tipo_transporte_texto,
						 p.veiculo_identificacao_numero,
						 p.rota_transito_definida,
						 p.itinerario,
						 p.apresentacao_nota_fiscal,
						 p.numero_nota_fiscal,
						 p.valido_ate,
						 p.responsavel_tecnico,
						 rt.tid                           responsavel_tecnico_tid,
						 p.municipio_emissao,
						 lme.texto                        municipio_emissao_texto,
						 p.eptv_id,
						 eptv.tid                         eptv_tid,
						 p.declaracao_adicional,
						 p.declaracao_adicional_formatado
					from tab_ptv                       p,
						 CRED_EPTV                     eptv,
						 lov_doc_fitossani_tipo_numero tn,
						 lov_ptv_situacao              ls,
						 tab_empreendimento            e,
						 tab_pessoa                    re,
						 tab_destinatario_ptv          d,
						 lov_estado                    le,
						 lov_municipio                 lm,
						 lov_tipo_transporte           tp,
						 tab_funcionario               rt,
						 lov_municipio                 lme
				   where tn.id = p.tipo_numero
					 and ls.id = p.situacao
					 and e.id(+) = p.empreendimento
					 and re.id(+) = p.responsavel_emp
					 and d.id = p.destinatario
					 and le.id(+) = p.estado
					 and lm.id(+) = p.municipio
					 and tp.id = p.tipo_transporte
					 and lme.id(+) = p.municipio_emissao
					 and rt.id = p.responsavel_tecnico
					 and eptv.id(+) = p.eptv_id
					 and p.id = p_id) loop
		
			-- insert historico PTV
			insert into hst_ptv
				(id,
				 tid,
				 ptv_id,
				 tipo_numero_id,
				 tipo_numero_texto,
				 numero,
				 data_emissao,
				 data_ativacao,
				 data_cancelamento,
				 situacao_id,
				 situacao_texto,
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
				 valido_ate,
				 responsavel_tecnico_id,
				 responsavel_tecnico_tid,
				 municipio_emissao_id,
				 municipio_emissao_texto,
				 executor_id,
				 executor_tid,
				 executor_nome,
				 executor_login,
				 executor_tipo_id,
				 executor_tipo_texto,
				 acao_executada,
				 data_execucao,
				 eptv_id,
				 eptv_tid,
				 declaracao_adicional,
				 declaracao_adicional_formatado)
			values
				(seq_hst_ptv.nextval,
				 i.tid,
				 i.id,
				 i.tipo_numero,
				 i.tipo_numero_texto,
				 i.numero,
				 i.data_emissao,
				 i.data_ativacao,
				 i.data_cancelamento,
				 i.situacao,
				 i.situacao_texto,
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
				 i.valido_ate,
				 i.responsavel_tecnico,
				 i.responsavel_tecnico_tid,
				 i.municipio_emissao,
				 i.municipio_emissao_texto,
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
				 systimestamp,
				 i.eptv_id,
				 i.eptv_tid,
				 i.declaracao_adicional,
				 i.declaracao_adicional_formatado)
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
									 from cre_cfo t
									where t.id = p.origem)
								 when 2 then
								  (select t.tid
									 from cre_cfoc t
									where t.id = p.origem)
								 when 3 then
								  (select t.tid
									 from tab_ptv t
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
							 u.texto unidade_medida_texto,
               p.exibe_kilos
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
					 unidade_medida_texto,
           exibe_kilos)
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
					 j.unidade_medida_texto,
           j.exibe_kilos);
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

	---------------------------------------------------------
	-- Emitir Permissão de Transito de Vegetais de outro estado
	---------------------------------------------------------
	procedure emitirptvoutro(p_id               number,
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
						 p.data_emissao,
						 p.data_ativacao,
						 p.data_cancelamento,
						 p.situacao,
						 ls.texto                situacao_texto,
						 p.interessado,
						 p.interessado_cnpj_cpf,
						 p.interessado_endereco,
						 p.interessado_estado,
						 lee.texto               int_estado_texto,
						 p.interessado_municipio,
						 lme.texto               int_municipio_texto,
						 p.destinatario,
						 d.tid                   destinatario_tid,
						 p.valido_ate,
						 p.resp_tecnico,
						 p.resp_tecnico_num_hab,
						 p.estado,
						 le.texto                estado_texto,
						 p.municipio,
						 lm.texto                municipio_texto
					from tab_ptv_outrouf      p,
						 lov_ptv_situacao     ls,
						 tab_destinatario_ptv d,
						 lov_estado           le,
						 lov_estado           lee,
						 lov_municipio        lm,
						 lov_municipio        lme
				   where ls.id = p.situacao
					 and d.id = p.destinatario
					 and le.id(+) = p.estado
					 and lee.id(+) = p.interessado_estado
					 and lm.id(+) = p.municipio
					 and lme.id(+) = p.interessado_municipio
					 and p.id = p_id) loop
		
			-- insert historico PTV
			insert into hst_ptv_outrouf
				(id,
				 tid,
				 ptv_id,
				 numero,
				 data_emissao,
				 data_ativacao,
				 data_cancelamento,
				 situacao_id,
				 situacao_texto,
				 interessado,
				 interessado_cnpj_cpf,
				 interessado_endereco,
				 interessado_estado_id,
				 interessado_estado_texto,
				 interessado_municipio_id,
				 interessado_municipio_texto,
				 destinatario_id,
				 destinatario_tid,
				 valido_ate,
				 resp_tecnico,
				 resp_tecnico_num_hab,
				 estado_id,
				 estado_texto,
				 municipio_id,
				 municipio_texto,
				 executor_id,
				 executor_tid,
				 executor_nome,
				 executor_login,
				 executor_tipo_id,
				 executor_tipo_texto,
				 acao_executada,
				 data_execucao)
			values
				(seq_hst_ptv_outrouf.nextval,
				 i.tid,
				 i.id,
				 i.numero,
				 i.data_emissao,
				 i.data_ativacao,
				 i.data_cancelamento,
				 i.situacao,
				 i.situacao_texto,
				 i.interessado,
				 i.interessado_cnpj_cpf,
				 i.interessado_endereco,
				 i.interessado_estado,
				 i.int_estado_texto,
				 i.interessado_municipio,
				 i.int_municipio_texto,
				 i.destinatario,
				 i.destinatario_tid,
				 i.valido_ate,
				 i.resp_tecnico,
				 i.resp_tecnico_num_hab,
				 i.estado,
				 i.estado_texto,
				 i.municipio,
				 i.municipio_texto,
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
					 and la.artefato = 141),
				 systimestamp)
			returning id into v_id;
		
			-- insert historico Produto PTV
			for j in (select p.id,
							 p.tid,
							 p.ptv,
							 p.origem_tipo,
							 lt.texto         origem_tipo_texto,
							 p.numero_origem,
							 p.cultura,
							 c.tid            cultura_tid,
							 p.cultivar,
							 cc.tid           cultivar_tid,
							 p.quantidade,
							 p.unidade_medida,
							 u.texto          unidade_medida_texto,
							 p.tipo_producao  tipo_producao,
							 l.texto          tipo_producao_texto
						from tab_ptv_outrouf_produto      p,
							 lov_doc_fitossanitarios_tipo lt,
							 tab_cultura                  c,
							 tab_cultura_cultivar         cc,
							 lov_crt_uni_prod_uni_medida  u,
							 lov_tipo_producao            l
					   where lt.id = p.origem_tipo
						 and c.id = p.cultura
						 and cc.id = p.cultivar
						 and u.id(+) = p.unidade_medida
						 and p.tipo_producao = l.id(+)
						 and p.ptv = p_id) loop
				insert into hst_ptv_outrouf_prod
					(id,
					 tid,
					 id_hst,
					 ptv_id,
					 origem_tipo_id,
					 origem_tipo_texto,
					 numero_origem,
					 cultura_id,
					 cultura_tid,
					 cultivar_id,
					 cultivar_tid,
					 quantidade,
					 unidade_medida_id,
					 unidade_medida_texto,
					 tipo_producao,
					 tipo_producao_texto)
				values
					(seq_hst_ptv_outrouf_prod.nextval,
					 j.tid,
					 v_id,
					 j.ptv,
					 j.origem_tipo,
					 j.origem_tipo_texto,
					 j.numero_origem,
					 j.cultura,
					 j.cultura_tid,
					 j.cultivar,
					 j.cultivar_tid,
					 j.quantidade,
					 j.unidade_medida,
					 j.unidade_medida_texto,
					 j.tipo_producao,
					 j.tipo_producao_texto);
			end loop;
		end loop;
	
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de PTV de outro estado. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		end if;
	
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de PTV de outro estado. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
	end;

	---------------------------------------------------------
	-- Local Vistoria
	---------------------------------------------------------
	procedure localvistoria(p_id               number,
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
		--  Local Vistoria
		----------------------------------------------------------------------------------------------------------------
		for i in (select lv.id,
						 lv.tid,
						 lv.setor       setor_id,
						 s.tid          setor_tid,
						 lv.dia_semana  dia_semana_id,
						 dia.texto      dia_semana_texto,
						 lv.hora_inicio,
						 lv.hora_fim,
						 lv.situacao
					from cnf_local_vistoria lv,
						 lov_dia_semana     dia,
						 tab_setor          s
				   where dia.id = lv.dia_semana
					 and s.id = lv.setor
					 and lv.id = p_id) loop
		
			-- Inserindo na histórico
		
			insert into hst_local_vistoria h
				(id,
				 local_id,
				 tid,
				 setor_id,
				 setor_tid,
				 dia_semana_id,
				 dia_semana_texto,
				 hora_inicio,
				 hora_fim,
				 situacao,
				 executor_id,
				 executor_tid,
				 executor_nome,
				 executor_login,
				 executor_tipo_id,
				 executor_tipo_texto,
				 acao_executada,
				 data_execucao)
			values
				(seq_hst_local_vistoria.nextval,
				 i.id,
				 i.tid,
				 i.setor_id,
				 i.setor_tid,
				 i.dia_semana_id,
				 i.dia_semana_texto,
				 i.hora_inicio,
				 i.hora_fim,
				 i.situacao,
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
					 and la.artefato = 144),
				 systimestamp);
		
		end loop;
		----------------------------------------------------------------------------------------------------------------
		v_sucesso := true;
	
		if (not v_sucesso) then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Arquivo.');
		end if;
		--Tratamento de exceção
	exception
		when others then
			Raise_application_error(-20000,
									'Erro ao gerar o Histórico de Arquivo. Mensagem: ' ||
									dbms_utility.format_error_stack ||
									dbms_utility.format_call_stack);
		
	end;
	---------------------------------------------------------

end;