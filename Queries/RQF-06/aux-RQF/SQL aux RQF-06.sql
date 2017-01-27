      select * from IDAF.tab_numero_cfo_cfoc 
        where utilizado = 0 /*não utilizado*/
        and substring(to_char(numero), 3, 0 < to_char(sysdate, 'yy');
---

select * from idaf.tab_liberacao_cfo_cfoc; -- autoriza a liberação de números para a pessoa

select * from tab_numero_cfo_cfoc; -- insere os números que serão liberados

select * from lov_doc_fitossanitarios_tipo;
select * from hst_cnf_doc_fito_intervalo;
--update cnf_doc_fito_intervalo set numero_final = 3216006000 where ID = 5; commit;
select * from cnf_doc_fito_intervalo i 
  where not exists (select 1 from hst_cnf_doc_fito_intervalo h
      where h.NUMERO_INICIAL = i.NUMERO_INICIAL and h.NUMERO_FINAL = i.NUMERO_FINAL);
select * from cnf_doc_fito_intervalo;
select * from CNF_DOC_FITOSSANITARIO;
select * from hst_cnf_doc_fito_intervalo;

select * from tab_config_doc_fitossani

---
select * from LOV_CULTIVAR_DECLARA_ADICIONAL;
---

select max(numero) from tab_ptv;
select * from hst_ptv;
select * from idaf.tab_ptv 
select * from ins_ptv; --é a tab_ptv do Institucional no Credenciado
select * from idafcredenciado.tab_ptv;

---
select * from tab_numero_cfo_cfoc where LIBERACAO = 56; -- insere os números que serão liberados

select * from idaf.tab_liberacao_cfo_cfoc where RESPONSAVEL_TECNICO = 4812; -- autoriza a liberação de números para a pessoa
select * from idafcredenciado.TAB_CREDENCIADO where id = 4812; --- "Luiz Carlos de Melo Oliveira Serra", pessoa 55250, usuário	4212
select * from idafcredenciado.tab_usuario where id = 4212;
select * from idafcredenciado.tab_pessoa where cpf = '053.478.296-56' -- "Luiz Carlos de Melo Oliveira Serra"

select * from idaf.tab_liberacao_cfo_cfoc where RESPONSAVEL_TECNICO = 996; -- autoriza a liberação de números para a pessoa
select * from idafcredenciado.TAB_CREDENCIADO where id = 996; --- "Pedro Spósito Zamborlini Oliveira", pessoa 5498, usuário	873
select * from idafcredenciado.tab_usuario where id = 873;
select * from idafcredenciado.tab_pessoa where id = 5498;
      
---
select * from tab_cfoc;
select * from TAB_CFOC_PRODUTO; -- tab CFOC x Lote (n:n)
select * from TAB_PTV_PRODUTO;

--- Lote CFOC

select * from tab_lote where codigo_uc like '32050690055%';
select * from tab_cfoc_produto where lote in(12,6)
select * from tab_cfoc where ID in (3,4)
--select * from tab_usuario where id = 346
--select * from tab_credenciado where id = 416

--- CFO

select * from tab_cfo where numero = 3216020826;

--- CFOC

select * from tab_cfoc where numero = 3216005502;
select * from tab_cfoc_produto where cfoc in(select id from tab_cfoc where numero = 3216005502);
select * from tab_lote where id in(select lote from tab_cfoc_produto where cfoc in(select id from tab_cfoc where numero = 3216005502));

/* public bool Excluir(int id):
begin
  delete tab_cfoc_trata_fitossa where cfoc = 4;
  delete tab_cfoc_praga where cfoc = 4;
  delete tab_cfoc_produto where cfoc = 4;
  delete tab_cfoc a where a.id = 4;
end;
*/
--rollback;
--commit;
---

--++++++++++++++++++++++++++++++++++++++ numero digital liberado:
--internal bool NumeroDigitalDisponivel()

select * from tab_numero_cfo_cfoc n, tab_liberacao_cfo_cfoc l 
	where l.id = n.liberacao and n.tipo_numero = 2 and n.situacao = 1 and n.utilizado = 0 
	and not exists (select null from cre_cfoc c where c.numero = n.numero) 
	and l.responsavel_tecnico = 4812
order by l.id, n.id;

---

select sysdate -1,
  to_char(sysdate, 'yy') -1,
  '__'|| to_char(sysdate, 'yy') ||'%' as form1_este_ano,
  '__'|| to_char(to_char(sysdate, 'yy') -1) ||'%' as form1,
  '__'|| to_char(to_date('01/01/' || to_char(sysdate, 'yyyy'), 'dd/mm/yyyy') -1, 'yy')||'%' as form2,
  substr(EXTRACT(YEAR FROM sysdate), 3) as "ptv digital form", 
  substr(3215000000, 3, 2) as "ptv digital subst"
from dual

--- Numeros Liberados de CFO/CFOC

    select * from IDAF.tab_numero_cfo_cfoc 
		where utilizado = 0 /*não utilizado*/
		and to_char(numero) not like '__'|| to_char(sysdate, 'yy') ||'%';

---

--VerificarQuantidadeMaximaNumDigitalCadastradoCFO
select count(t.id) from tab_numero_cfo_cfoc t, tab_liberacao_cfo_cfoc l, tab_credenciado c where l.id = t.liberacao and c.id = l.responsavel_tecnico
				and t.tipo_documento = 1 and t.tipo_numero = 2 and t.situacao = 1 and t.utilizado = 0 and c.id = 4812
select * from tab_credenciado where id = 4812
select * from tab_pessoa where id = 55250

--- Aux: Lista de status de PTV: 
-- Institucional:
select l.id, l.texto from lov_ptv_situacao l order by l.id;
-- Credenciado:
select l.id, l.texto from idafcredenciado.lov_solicitacao_ptv_situacao l order by l.id;

--- Aux: Lista de status CFO/CFOC:
select * from lov_doc_fitossani_situacao;

--- Lotes

select * from lov_lote_situacao;

select * from tab_lote l;

---  
select * from tab_usuario where login like '%Woelpher%'

---select * from tab_usuario

/*
    update IDAF.tab_numero_cfo_cfoc set motivo = 'Cancelamento de números do ano anterior (script)', situacao = 0
    --select * from IDAF.tab_numero_cfo_cfoc
      where utilizado = 0 não utilizado
      and to_char(numero) like '__________';
    commit;
*/
---
/*
--- seta esse maldito pra ON:
SET SERVEROUTPUT ON;

  declare
    v_aux            number := 0;
    v_maior          number := 0;
    v_quantidade_lib number := 25;
  begin
    select nvl((select max(d.numero) from tab_numero_cfo_cfoc d where d.tipo_documento = 1 and d.tipo_numero = 2
            and to_char(numero) like '__'|| to_char(sysdate, 'yy') ||'%'),
      (select min(c.numero_inicial) - 1 from cnf_doc_fito_intervalo c where c.tipo_documento = 1 and c.tipo = 2
            and to_char(numero_inicial) like '__'|| to_char(sysdate, 'yy') ||'%'))
    into v_maior from dual;
    
    -- printando o valor:
    dbms_output.put_line(v_maior);
    
  end;

SET SERVEROUTPUT OFF;
*/

---------------------------------------------------------------------------------------------------------------------------
--- VerificarQuantidadeMaximaNumDigitalCadastradoCFOC

	select count(t.id) from tab_numero_cfo_cfoc t, tab_liberacao_cfo_cfoc l, tab_credenciado c where l.id = t.liberacao and c.id = l.responsavel_tecnico
		and t.tipo_documento = 2 and t.tipo_numero = 2 and t.situacao = 1 and t.utilizado = 0 and c.id = 416 /*:credenciado_id*/
    
---------------------------------------------------------------------------------------------------------------------------
--- DigitalPossuiNumeroCFOCNaoConfigurado
--- 	Dispara uma Mensagem.LiberacaoNumeroCFOCFOC.QuantidadeNumerosDigitaisCFOCNaoPodeExcederAConfiguradaNoSistema

--- retorna "ultimoAdicionado":
   select nvl((select max(t.numero) from tab_liberacao_cfo_cfoc l, tab_numero_cfo_cfoc t where t.liberacao = l.id and t.tipo_documento = 2 and t.tipo_numero = 2), 
      (select min(t.numero_inicial) - 1 from cnf_doc_fito_intervalo t where t.tipo_documento = 2 and t.tipo = 2)) from dual

--- verifica se "retorno" > 0 como retorno da função DigitalPossuiNumeroCFOCNaoConfigurado
SET SERVEROUTPUT ON;
declare 
	proximo number;
	v_aux   number := 1;
	v_saida number := 0;
begin 
	proximo := 3216005500/*:numero_inicial*/;

	for i in 1..25/*:quantidadeDigital*/ loop
		select count(*) into v_aux from cnf_doc_fito_intervalo c 
		where c.tipo = 2 and c.tipo_documento = 2 and proximo >= c.numero_inicial and proximo <= c.numero_final;

		if v_aux = 0 then
			select nvl(min(t.numero_inicial), 0) into proximo from cnf_doc_fito_intervalo t 
			where t.tipo_documento = 2 and t.tipo = 2 and t.numero_inicial > proximo;

			if proximo = 0 then
				v_saida := 1;/*Possui 1 não configurado*/
				exit;
			end if;
		else
			proximo := proximo + 1;
		end if;
	end loop;

	-- printando o valor:
  dbms_output.put_line('Valor de v_saida:');
  dbms_output.put_line(v_saida);
end;
SET SERVEROUTPUT OFF;
---------------------------------------------------------------------------------------------------------------------------
    
    
-- exec cancelar_cfocfocptv_anual;