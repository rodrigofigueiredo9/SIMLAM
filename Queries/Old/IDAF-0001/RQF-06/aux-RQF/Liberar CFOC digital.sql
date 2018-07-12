
					declare
						v_aux            number := 0;
						v_maior          number := 0;
						v_quantidade_lib number := :quantidade_lib;
					begin
						select nvl((select max(d.numero) from tab_numero_cfo_cfoc d where d.tipo_documento = :tipo_documento and d.tipo_numero = :tipo_numero
										and to_char(numero) like '__'|| to_char(sysdate, 'yy') ||'%'),
							(select min(c.numero_inicial) - 1 from cnf_doc_fito_intervalo c where c.tipo_documento = :tipo_documento and c.tipo = :tipo_numero
										and to_char(numero_inicial) like '__'|| to_char(sysdate, 'yy') ||'%'))
						into v_maior from dual;

						for j in 1..v_quantidade_lib loop 
							v_maior := v_maior + 1;

							select count(1) into v_aux from cnf_doc_fito_intervalo c where c.tipo_documento = :tipo_documento 
							and c.tipo = :tipo_numero and (v_maior between c.numero_inicial and c.numero_final)
							and to_char(v_maior) like '__'|| to_char(sysdate, 'yy') ||'%';

							if (v_aux > 0) then
								insert into tab_numero_cfo_cfoc (id, numero, tipo_documento, tipo_numero, liberacao, situacao, utilizado, tid) 
								values (seq_tab_numero_cfo_cfoc.nextval, v_maior, :tipo_documento, :tipo_numero, :liberacao, 1, 0, :tid);
							else 
								v_aux := v_maior;

								select min(t.numero_inicial) into v_maior from cnf_doc_fito_intervalo t 
								where t.tipo_documento = :tipo_documento and t.tipo = :tipo_numero and t.numero_inicial > v_maior
								and to_char(numero_inicial) like '__'|| to_char(sysdate, 'yy') ||'%';

								if(v_maior is null or v_aux = v_maior) then 
									--Tratamento de exceção
									Raise_application_error(-20023, 'Número não configurado');
								else 
									insert into tab_numero_cfo_cfoc (id, numero, tipo_documento, tipo_numero, liberacao, situacao, utilizado, tid) 
									values (seq_tab_numero_cfo_cfoc.nextval, v_maior, :tipo_documento, :tipo_numero, :liberacao, 1, 0, :tid);
								end if;
							end if;
						end loop;
					end;