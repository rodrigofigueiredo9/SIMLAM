
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
						 a.tid                          arquivo_tid
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
				 arquivo_tid)
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
				 i.arquivo_tid);
		
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
						 tfm.serie,
						 tfm.valor_multa,
						 tfm.arquivo,
						 tfm.justificar,
						 tfm.codigo_receita,
						 tfm.tid
					from tab_fisc_multa tfm
				   where 
						 and tfm.fiscalizacao = p_id ) loop
		
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
