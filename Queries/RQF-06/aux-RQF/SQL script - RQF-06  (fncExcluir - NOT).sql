/**
  RQF-06	Cancelar Documentos em Elaboração e Nº de Blocos e Digitais não utilizados
*/

/*Deixar como agendamento anual. Se repetirá a cada virada de ano.*/

begin

  savepoint cancelar_CFOCFOCPTV_savepoint;

  begin
  /*Cancelar todos os documentos de CFO, CFOC e PTV na situação em Elaboração no ano anterior, no dia 01 de Janeiro do ano atual, às 00:01. */
  --- CFO:
	begin
		delete IDAFCREDENCIADO.tab_cfo_trata_fitossa 
			where cfo in(select id from IDAFCREDENCIADO.TAB_CFO
			  where SITUACAO = 1 /*Em elaboração*/
			  and DATA_EMISSAO < to_date('01/01/' || to_char(sysdate, 'yyyy'), 'dd/mm/yyyy'));
		delete IDAFCREDENCIADO.tab_cfo_praga 
			where cfo in(select id from IDAFCREDENCIADO.TAB_CFO
			  where SITUACAO = 1 /*Em elaboração*/
			  and DATA_EMISSAO < to_date('01/01/' || to_char(sysdate, 'yyyy'), 'dd/mm/yyyy'));
		delete IDAFCREDENCIADO.tab_cfo_produto 
			where cfo in(select id from IDAFCREDENCIADO.TAB_CFO
			  where SITUACAO = 1 /*Em elaboração*/
			  and DATA_EMISSAO < to_date('01/01/' || to_char(sysdate, 'yyyy'), 'dd/mm/yyyy'));
		delete IDAFCREDENCIADO.tab_cfo a 
			where a.id in(select id from IDAFCREDENCIADO.TAB_CFO
			  where SITUACAO = 1 /*Em elaboração*/
			  and DATA_EMISSAO < to_date('01/01/' || to_char(sysdate, 'yyyy'), 'dd/mm/yyyy'));
	end;
  --- fim-CFO
  
  --- CFOC:	
	begin
	  /*Voltar o Lote do CFOC cancelado para Não Utilizado*/
	  update IDAFCREDENCIADO.tab_lote set situacao = 2 /*Não Utilizado*/
		where tab_lote.id in(
		  select tab_cfoc_produto.lote
		  from IDAFCREDENCIADO.tab_cfoc_produto
		  where cfoc in(select id from IDAFCREDENCIADO.TAB_CFOC
			  where SITUACAO = 1 /*Em elaboração*/
			  and DATA_EMISSAO < to_date('01/01/' || to_char(sysdate, 'yyyy'), 'dd/mm/yyyy')));
		delete IDAFCREDENCIADO.tab_cfoc_trata_fitossa 
			where cfoc in(select id from IDAFCREDENCIADO.TAB_CFOC
			  where SITUACAO = 1 /*Em elaboração*/
			  and DATA_EMISSAO < to_date('01/01/' || to_char(sysdate, 'yyyy'), 'dd/mm/yyyy'));
		delete IDAFCREDENCIADO.tab_cfoc_praga 
			where cfoc in(select id from IDAFCREDENCIADO.TAB_CFOC
			  where SITUACAO = 1 /*Em elaboração*/
			  and DATA_EMISSAO < to_date('01/01/' || to_char(sysdate, 'yyyy'), 'dd/mm/yyyy'));
		delete IDAFCREDENCIADO.tab_cfoc_produto 
			where cfoc in(select id from IDAFCREDENCIADO.TAB_CFOC
			  where SITUACAO = 1 /*Em elaboração*/
			  and DATA_EMISSAO < to_date('01/01/' || to_char(sysdate, 'yyyy'), 'dd/mm/yyyy'));
		delete IDAFCREDENCIADO.tab_cfoc a 
			where a.id in(select id from IDAFCREDENCIADO.TAB_CFOC
			  where SITUACAO = 1 /*Em elaboração*/
			  and DATA_EMISSAO < to_date('01/01/' || to_char(sysdate, 'yyyy'), 'dd/mm/yyyy'));
	end;
  --- fim-CFOC
	
  --- PTV (Institucional): 
	  /*Confirmado na função PTVDa.ObterOrigemQuantidade() chamada em PTVValidar.ValidarProduto() */
	  update IDAF.TAB_PTV 
		  set SITUACAO = 3, /*Cancelado*/
			  DATA_CANCELAMENTO = sysdate
		--select count(1) from IDAF.TAB_PTV
		where SITUACAO = 1 /*Em elaboração*/
		and DATA_EMISSAO < to_date('01/01/' || to_char(sysdate, 'yyyy'), 'dd/mm/yyyy');
  --- fim-PTV	
  
  --- ePTV:
	/*	
	begin
	?	delete {0}tab_ptv_comuni_conversa pcc where pcc.comunicador_id in (select c.id from {0}tab_ptv_comunicador c where c.ptv_id = :id);
	?	delete {0}tab_ptv_comunicador pc where pc.ptv_id = :id;
	?	delete {0}tab_ptv_produto pr where pr.ptv = :id;
	?	delete {0}tab_ptv_arquivo pr where pr.ptv = :id;
	?	delete {0}tab_ptv p where p.id = :id;
	end;
	
	Qual status de ePTV é o que devemos excluir?
	*/
  --- fim-ePTV
  
  --- PTV de Outro Estado:
	/*
	update IDAFCREDENCIADO.tab_ptv_outrouf p
		set p.tid			   = :tid,
			p.situacao		  = :situacao,
			p.data_cancelamento = :data_cancelamento
		where p.id = :id"
	*/
  --- fim PTV de Outro Estado
  
  --- Blocos CFO e CFOC:
	/*Também cancelar os nº de blocos e digitais não utilizados, inclusive os não liberados (não vendidos).*/
	  update IDAF.tab_numero_cfo_cfoc set situacao = 0, motivo = 'Cancelamento de números do ano anterior (automático)'
		where utilizado = 0 /*não utilizado*/
		and to_char(numero) not like '__'|| to_char(sysdate, 'yy') ||'%';
  --- fim Blocos CFO e CFOC:
  
  
  /*O Saldo das Culturas deve ser estornado conforme a quantidade especificada no documento cancelado.
		Ex:
		  PTV baseada em CFO/PTV – Devolver o Saldo da Cultura indicada no PTV para cada UP de Origem especificada no CFO ou PTV;
		  CFO - Devolver o Saldo da Cultura indicada no CFO para cada UP de Origem;
		  CFOC - Devolver o Saldo da Cultura indicada no CFOC para o Lote de Origem;
  */
  
  end;
  exception
	 when others then
	   rollback to cancelar_CFOCFOCPTV_savepoint;
	   
  commit;
end;  