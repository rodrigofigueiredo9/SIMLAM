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

  select '--- Blocos CFO e CFOC:' as msg from dual;
      select * from IDAF.tab_numero_cfo_cfoc 
        where utilizado = 0 /*n�o utilizado*/
        and substr(to_char(numero), 3, 2) < to_char(sysdate, 'yy');
  select '--- fim Blocos CFO e CFOC:' as msg from dual;

  select '--- CFO:' as msg from dual;
	 select '/*N�meros Liberados */' as msg from dual;
	  select * from IDAF.tab_numero_cfo_cfoc 
        where numero in(select numero from IDAFCREDENCIADO.TAB_CFO
              where SITUACAO = 1 /*Em elabora��o*/
              and substr(to_char(numero), 3, 2) < to_char(sysdate, 'yy')
           );
     select '/*Marcar o CFO como cancelado:*/' as msg from dual;
      select * from IDAFCREDENCIADO.TAB_CFO 
        where SITUACAO = 1 /*Em elabora��o*/
        and substr(to_char(numero), 3, 2) < to_char(sysdate, 'yy');
  select '--- fim-CFO' as msg from dual;
  
  
  select '--- CFOC:    ' as msg from dual;
	 select '/*N�meros Liberados */' as msg from dual;
	  select * from IDAF.tab_numero_cfo_cfoc 
        where numero in(select numero from IDAFCREDENCIADO.TAB_CFOC
              where SITUACAO = 1 /*Em elabora��o*/
              and substr(to_char(numero), 3, 2) < to_char(sysdate, 'yy')
           );
     select '/*Voltar o Lote do CFOC cancelado para N�o Utilizado*/' as msg from dual;
      select * from IDAFCREDENCIADO.tab_lote 
        where tab_lote.id in(
          select tab_cfoc_produto.lote
          from IDAFCREDENCIADO.tab_cfoc_produto
          where cfoc in(select id from IDAFCREDENCIADO.TAB_CFOC
              where SITUACAO = 1 /*Em elabora��o*/
              and substr(to_char(numero), 3, 2) < to_char(sysdate, 'yy')
           )
        );
     select '/*Apagar os Lotes inseridos no CFOC cancelado*/' as msg from dual;
      select * from IDAFCREDENCIADO.tab_cfoc_produto
      where cfoc in(select id from IDAFCREDENCIADO.TAB_CFOC
          where SITUACAO = 1 /*Em elabora��o*/
          and substr(to_char(numero), 3, 2) < to_char(sysdate, 'yy')
        );
     select '/*Marcar o CFOC como cancelado:*/' as msg from dual;
      select * from IDAFCREDENCIADO.TAB_CFOC 
        where SITUACAO = 1 /*Em elabora��o*/
        and substr(to_char(numero), 3, 2) < to_char(sysdate, 'yy');
  select '--- fim-CFOC' as msg from dual;
    
  select '--- PTV (Institucional):' as msg from dual;
      select * from IDAF.TAB_PTV 
        where SITUACAO = 1 /*Em elabora��o*/
        and substr(to_char(numero), 3, 2) < to_char(sysdate, 'yy');
  select '--- fim-PTV    ' as msg from dual;
  
  select '--- ePTV:' as msg from dual;
      select '-- Conforme acordado com a DDSIV-SDSV, n�o ser� necess�rio.' as msg from dual;
  select '--- fim-ePTV' as msg from dual;
  
  select '--- PTV Outro Estado:' as msg from dual;
      select '-- Conforme acordado com a DDSIV-SDSV, n�o ser� necess�rio.' as msg from dual;
  select '--- fim PTV Outro Estado' as msg from dual;
  
