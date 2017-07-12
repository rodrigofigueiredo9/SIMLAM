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

  select '--- Blocos CFO e CFOC:' as msg from dual;
      select * from IDAF.tab_numero_cfo_cfoc 
        where utilizado = 0 /*não utilizado*/
        and substr(to_char(numero), 3, 2) < to_char(sysdate, 'yy');
  select '--- fim Blocos CFO e CFOC:' as msg from dual;

  select '--- CFO:' as msg from dual;
	 select '/*Números Liberados */' as msg from dual;
	  select * from IDAF.tab_numero_cfo_cfoc 
        where numero in(select numero from IDAFCREDENCIADO.TAB_CFO
              where SITUACAO = 1 /*Em elaboração*/
              and substr(to_char(numero), 3, 2) < to_char(sysdate, 'yy')
           );
     select '/*Marcar o CFO como cancelado:*/' as msg from dual;
      select * from IDAFCREDENCIADO.TAB_CFO 
        where SITUACAO = 1 /*Em elaboração*/
        and substr(to_char(numero), 3, 2) < to_char(sysdate, 'yy');
  select '--- fim-CFO' as msg from dual;
  
  
  select '--- CFOC:    ' as msg from dual;
	 select '/*Números Liberados */' as msg from dual;
	  select * from IDAF.tab_numero_cfo_cfoc 
        where numero in(select numero from IDAFCREDENCIADO.TAB_CFOC
              where SITUACAO = 1 /*Em elaboração*/
              and substr(to_char(numero), 3, 2) < to_char(sysdate, 'yy')
           );
     select '/*Voltar o Lote do CFOC cancelado para Não Utilizado*/' as msg from dual;
      select * from IDAFCREDENCIADO.tab_lote 
        where tab_lote.id in(
          select tab_cfoc_produto.lote
          from IDAFCREDENCIADO.tab_cfoc_produto
          where cfoc in(select id from IDAFCREDENCIADO.TAB_CFOC
              where SITUACAO = 1 /*Em elaboração*/
              and substr(to_char(numero), 3, 2) < to_char(sysdate, 'yy')
           )
        );
     select '/*Apagar os Lotes inseridos no CFOC cancelado*/' as msg from dual;
      select * from IDAFCREDENCIADO.tab_cfoc_produto
      where cfoc in(select id from IDAFCREDENCIADO.TAB_CFOC
          where SITUACAO = 1 /*Em elaboração*/
          and substr(to_char(numero), 3, 2) < to_char(sysdate, 'yy')
        );
     select '/*Marcar o CFOC como cancelado:*/' as msg from dual;
      select * from IDAFCREDENCIADO.TAB_CFOC 
        where SITUACAO = 1 /*Em elaboração*/
        and substr(to_char(numero), 3, 2) < to_char(sysdate, 'yy');
  select '--- fim-CFOC' as msg from dual;
    
  select '--- PTV (Institucional):' as msg from dual;
      select * from IDAF.TAB_PTV 
        where SITUACAO = 1 /*Em elaboração*/
        and substr(to_char(numero), 3, 2) < to_char(sysdate, 'yy');
  select '--- fim-PTV    ' as msg from dual;
  
  select '--- ePTV:' as msg from dual;
      select '-- Conforme acordado com a DDSIV-SDSV, não será necessário.' as msg from dual;
  select '--- fim-ePTV' as msg from dual;
  
  select '--- PTV Outro Estado:' as msg from dual;
      select '-- Conforme acordado com a DDSIV-SDSV, não será necessário.' as msg from dual;
  select '--- fim PTV Outro Estado' as msg from dual;
  
