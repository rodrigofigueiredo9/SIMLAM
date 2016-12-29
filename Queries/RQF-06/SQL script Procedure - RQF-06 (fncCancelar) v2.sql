CREATE OR REPLACE PROCEDURE cancelar_CFOCFOCPTV_anual AS
BEGIN
/*
  RQF-06	Cancelar Documentos em Elaboração e Nº de Blocos e Digitais não utilizados

	Cancelar todos os documentos de CFO, CFOC e PTV na situação em Elaboração no ano anterior, no dia 01 de Janeiro do ano atual, às 00:01. 
	Também cancelar os nº de blocos e digitais não utilizados, inclusive os não liberados (não vendidos).
	
	> Deixar como agendamento anual. Se repetirá a cada virada de ano.

	O Saldo das Culturas deve ser estornado conforme a quantidade especificada no documento cancelado.

	Ex:
	- PTV baseada em CFO/PTV – Devolver o Saldo da Cultura indicada no PTV  para cada UP de Origem especificada no CFO ou PTV;
	- CFO - Devolver o Saldo da Cultura indicada no CFO para cada UP de Origem;
	- CFOC - Devolver o Saldo da Cultura indicada no CFOC para o Lote de Origem;
*/

  savepoint cancelar_CFOCFOCPTV_savepoint;

  begin
  
  --- Blocos CFO e CFOC:
      update IDAF.tab_numero_cfo_cfoc set situacao = 0, utilizado = 1, motivo = 'Cancelamento de números do ano anterior (automático)'
        where utilizado = 0 /*não utilizado*/
        and substr(to_char(numero), 3, 2) < to_char(sysdate, 'yy');
  --- fim Blocos CFO e CFOC:

  --- CFO:
	 /*Números Liberados */
	  update IDAF.tab_numero_cfo_cfoc set situacao = 0, utilizado = 1, motivo = 'Cancelamento de números do ano anterior (automático)'
        where numero in(select numero from IDAFCREDENCIADO.TAB_CFO
              where SITUACAO = 1 /*Em elaboração*/
              and substr(to_char(numero), 3, 2) < to_char(sysdate, 'yy')
           );
     /*Marcar o CFO como cancelado:*/
      update IDAFCREDENCIADO.TAB_CFO set SITUACAO = 4 /*Cancelado*/
        where SITUACAO = 1 /*Em elaboração*/
        and substr(to_char(numero), 3, 2) < to_char(sysdate, 'yy');
  --- fim-CFO
  
  
  --- CFOC:    
	 /*Números Liberados */
	  update IDAF.tab_numero_cfo_cfoc set situacao = 0, utilizado = 1, motivo = 'Cancelamento de números do ano anterior (automático)'
        where numero in(select numero from IDAFCREDENCIADO.TAB_CFOC
              where SITUACAO = 1 /*Em elaboração*/
              and substr(to_char(numero), 3, 2) < to_char(sysdate, 'yy')
           );
     /*Voltar o Lote do CFOC cancelado para Não Utilizado*/
      update IDAFCREDENCIADO.tab_lote set situacao = 2 /*Não Utilizado*/
        where tab_lote.id in(
          select tab_cfoc_produto.lote
          from IDAFCREDENCIADO.tab_cfoc_produto
          where cfoc in(select id from IDAFCREDENCIADO.TAB_CFOC
              where SITUACAO = 1 /*Em elaboração*/
              and substr(to_char(numero), 3, 2) < to_char(sysdate, 'yy')
           )
        );
     /*Apagar os Lotes inseridos no CFOC cancelado*/
      delete from IDAFCREDENCIADO.tab_cfoc_produto
      where cfoc in(select id from IDAFCREDENCIADO.TAB_CFOC
          where SITUACAO = 1 /*Em elaboração*/
          and substr(to_char(numero), 3, 2) < to_char(sysdate, 'yy')
        );
     /*Marcar o CFOC como cancelado:*/
      update IDAFCREDENCIADO.TAB_CFOC set SITUACAO = 4 /*Cancelado*/
        where SITUACAO = 1 /*Em elaboração*/
        and substr(to_char(numero), 3, 2) < to_char(sysdate, 'yy');
  --- fim-CFOC
    
  --- PTV (Institucional):
      update IDAF.TAB_PTV 
          set SITUACAO = 3, /*Cancelado*/
              DATA_CANCELAMENTO = sysdate
        where SITUACAO = 1 /*Em elaboração*/
        and substr(to_char(numero), 3, 2) < to_char(sysdate, 'yy');
  --- fim-PTV    
  
  --- ePTV:
      -- Conforme acordado com a DDSIV-SDSV, não será necessário.
  --- fim-ePTV
  
  --- PTV Outro Estado:
      -- Conforme acordado com a DDSIV-SDSV, não será necessário.
  --- fim PTV Outro Estado
  
  end;
  exception
     when others then
       rollback to cancelar_CFOCFOCPTV_savepoint;
	   
  commit;
END cancelar_CFOCFOCPTV_anual;  
