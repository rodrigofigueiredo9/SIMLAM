CREATE OR REPLACE PROCEDURE cancelar_CFOCFOCPTV_anual AS
BEGIN
/*
  RQF-06	Cancelar Documentos em Elabora��o e N� de Blocos e Digitais n�o utilizados

	Cancelar todos os documentos de CFO, CFOC e PTV na situa��o em Elabora��o no ano anterior, no dia 01 de Janeiro do ano atual, �s 00:01. 
	Tamb�m cancelar os n� de blocos e digitais n�o utilizados, inclusive os n�o liberados (n�o vendidos).
	
	> Deixar como agendamento anual. Se repetir� a cada virada de ano.

	O Saldo das Culturas deve ser estornado conforme a quantidade especificada no documento cancelado.

	Ex:
	- PTV baseada em CFO/PTV � Devolver o Saldo da Cultura indicada no PTV  para cada UP de Origem especificada no CFO ou PTV;
	- CFO - Devolver o Saldo da Cultura indicada no CFO para cada UP de Origem;
	- CFOC - Devolver o Saldo da Cultura indicada no CFOC para o Lote de Origem;
*/

  savepoint cancelar_CFOCFOCPTV_savepoint;

  begin
  
  --- Blocos CFO e CFOC:
      update IDAF.tab_numero_cfo_cfoc set situacao = 0, utilizado = 1, motivo = 'Cancelamento de n�meros do ano anterior (autom�tico)'
        where utilizado = 0 /*n�o utilizado*/
        and substr(to_char(numero), 3, 2) < to_char(sysdate, 'yy');
  --- fim Blocos CFO e CFOC:

  --- CFO:
	 /*N�meros Liberados */
	  update IDAF.tab_numero_cfo_cfoc set situacao = 0, utilizado = 1, motivo = 'Cancelamento de n�meros do ano anterior (autom�tico)'
        where numero in(select numero from IDAFCREDENCIADO.TAB_CFO
              where SITUACAO = 1 /*Em elabora��o*/
              and substr(to_char(numero), 3, 2) < to_char(sysdate, 'yy')
           );
     /*Marcar o CFO como cancelado:*/
      update IDAFCREDENCIADO.TAB_CFO set SITUACAO = 4 /*Cancelado*/
        where SITUACAO = 1 /*Em elabora��o*/
        and substr(to_char(numero), 3, 2) < to_char(sysdate, 'yy');
  --- fim-CFO
  
  
  --- CFOC:    
	 /*N�meros Liberados */
	  update IDAF.tab_numero_cfo_cfoc set situacao = 0, utilizado = 1, motivo = 'Cancelamento de n�meros do ano anterior (autom�tico)'
        where numero in(select numero from IDAFCREDENCIADO.TAB_CFOC
              where SITUACAO = 1 /*Em elabora��o*/
              and substr(to_char(numero), 3, 2) < to_char(sysdate, 'yy')
           );
     /*Voltar o Lote do CFOC cancelado para N�o Utilizado*/
      update IDAFCREDENCIADO.tab_lote set situacao = 2 /*N�o Utilizado*/
        where tab_lote.id in(
          select tab_cfoc_produto.lote
          from IDAFCREDENCIADO.tab_cfoc_produto
          where cfoc in(select id from IDAFCREDENCIADO.TAB_CFOC
              where SITUACAO = 1 /*Em elabora��o*/
              and substr(to_char(numero), 3, 2) < to_char(sysdate, 'yy')
           )
        );
     /*Apagar os Lotes inseridos no CFOC cancelado*/
      delete from IDAFCREDENCIADO.tab_cfoc_produto
      where cfoc in(select id from IDAFCREDENCIADO.TAB_CFOC
          where SITUACAO = 1 /*Em elabora��o*/
          and substr(to_char(numero), 3, 2) < to_char(sysdate, 'yy')
        );
     /*Marcar o CFOC como cancelado:*/
      update IDAFCREDENCIADO.TAB_CFOC set SITUACAO = 4 /*Cancelado*/
        where SITUACAO = 1 /*Em elabora��o*/
        and substr(to_char(numero), 3, 2) < to_char(sysdate, 'yy');
  --- fim-CFOC
    
  --- PTV (Institucional):
      update IDAF.TAB_PTV 
          set SITUACAO = 3, /*Cancelado*/
              DATA_CANCELAMENTO = sysdate
        where SITUACAO = 1 /*Em elabora��o*/
        and substr(to_char(numero), 3, 2) < to_char(sysdate, 'yy');
  --- fim-PTV    
  
  --- ePTV:
      -- Conforme acordado com a DDSIV-SDSV, n�o ser� necess�rio.
  --- fim-ePTV
  
  --- PTV Outro Estado:
      -- Conforme acordado com a DDSIV-SDSV, n�o ser� necess�rio.
  --- fim PTV Outro Estado
  
  end;
  exception
     when others then
       rollback to cancelar_CFOCFOCPTV_savepoint;
	   
  commit;
END cancelar_CFOCFOCPTV_anual;  
