
				declare
					v_aux   number := 0;
					v_maior number := 0;
				begin
					select nvl((select max(d.numero) from tab_ptv d where d.tipo_numero = 2
									and to_char(d.numero) like '__'|| to_char(sysdate, 'yy') ||'%'),
						(select min(c.numero_inicial) - 1 from cnf_doc_fito_intervalo c where c.tipo_documento = 3 and c.tipo = 2
									and to_char(c.numero_inicial) like '__'|| to_char(sysdate, 'yy') ||'%'))
					into v_maior from dual;

					v_maior := v_maior + 1;

					select count(1) into v_aux from cnf_doc_fito_intervalo c 
					where c.tipo_documento = 3 and c.tipo = 2 
					and substr(c.numero_inicial, 3, 2) = substr(EXTRACT(YEAR FROM sysdate), 3) 
					and substr(c.numero_final, 3, 2) = substr(EXTRACT(YEAR FROM sysdate), 3) 
					and (v_maior between c.numero_inicial and c.numero_final);

					if (v_aux <= 0) then
						v_aux := v_maior;

						select min(t.numero_inicial) into v_maior from cnf_doc_fito_intervalo t 
						where t.tipo_documento = 3 and t.tipo = 2 
						and substr(t.numero_inicial, 3, 2) = substr(EXTRACT(YEAR FROM sysdate), 3) 
						and substr(t.numero_final, 3, 2) = substr(EXTRACT(YEAR FROM sysdate), 3)
						and t.numero_inicial > v_maior;

						if(v_maior is null or v_aux = v_maior) then 
							v_maior := null;
						end if;
					end if;
					select v_maior into :numero_saida from dual;
				end;