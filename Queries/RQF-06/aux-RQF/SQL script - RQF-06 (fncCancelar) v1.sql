/**
  RQF-06	Cancelar Documentos em Elabora��o e N� de Blocos e Digitais n�o utilizados
*/

/*Deixar como agendamento anual. Se repetir� a cada virada de ano.*/

begin

  savepoint cancelar_CFOCFOCPTV_savepoint;

  begin
  /*Cancelar todos os documentos de CFO, CFOC e PTV na situa��o em Elabora��o no ano anterior, no dia 01 de Janeiro do ano atual, �s 00:01. */
  --- CFO:
      update IDAFCREDENCIADO.TAB_CFO set SITUACAO = 4 /*Cancelado*/
        --select * from IDAFCREDENCIADO.TAB_CFO 
        where SITUACAO = 1 /*Em elabora��o*/
        and DATA_EMISSAO < to_date('01/01/' || to_char(sysdate, 'yyyy'), 'dd/mm/yyyy');
  --- fim-CFO
  
  
  --- CFOC:    
      /*Voltar o Lote do CFOC cancelado para N�o Utilizado*/
      update IDAFCREDENCIADO.tab_lote set situacao = 2 /*N�o Utilizado*/
        where tab_lote.id in(
          select tab_cfoc_produto.lote
          from IDAFCREDENCIADO.tab_cfoc_produto
          where cfoc in(select id from IDAFCREDENCIADO.TAB_CFOC
              where SITUACAO = 1 /*Em elabora��o*/
              and DATA_EMISSAO < to_date('01/01/' || to_char(sysdate, 'yyyy'), 'dd/mm/yyyy')
           )
        );
      /*Apagar os Lotes inseridos no CFOC cancelado*/
      delete from IDAFCREDENCIADO.tab_cfoc_produto
      where cfoc in(select id from IDAFCREDENCIADO.TAB_CFOC
          where SITUACAO = 1 /*Em elabora��o*/
          and DATA_EMISSAO < to_date('01/01/' || to_char(sysdate, 'yyyy'), 'dd/mm/yyyy')
        );
      /*Marcar o CFOC como cancelado:*/
      update IDAFCREDENCIADO.TAB_CFOC set SITUACAO = 4 /*Cancelado*/
        --select count(1) from IDAFCREDENCIADO.TAB_CFOC
        where SITUACAO = 1 /*Em elabora��o*/
        and DATA_EMISSAO < to_date('01/01/' || to_char(sysdate, 'yyyy'), 'dd/mm/yyyy');
  --- fim-CFOC
    
  --- PTV (Institucional):
      update IDAF.TAB_PTV 
          set SITUACAO = 3, /*Cancelado*/
              DATA_CANCELAMENTO = sysdate
        --select count(1) from IDAF.TAB_PTV
        where SITUACAO = 1 /*Em elabora��o*/
        and DATA_EMISSAO < to_date('01/01/' || to_char(sysdate, 'yyyy'), 'dd/mm/yyyy');
  --- fim-PTV    
  
  --- ePTV:
  /*    update IDAFCREDENCIADO.TAB_PTV set SITUACAO = ? 4	Recusado / 6	Bloqueado ?
        --select count(1) from IDAFCREDENCIADO.TAB_PTV
        where SITUACAO in(
              1, --Cadastrado
              2  --Aguardando an�lise
          )
        and DATA_EMISSAO < to_date('01/01/' || to_char(sysdate, 'yyyy'), 'dd/mm/yyyy');
  */
  --- fim-ePTV
  
  --- PTV Outro Estado:
  /*    update {0}tab_ptv_outrouf p
        set p.tid               = :tid,
            p.situacao          = :situacao,
            p.data_cancelamento = :data_cancelamento
        where p.id = :id"
  */
  --- fim PTV Outro Estado
  
  --- Blocos CFO e CFOC:
    /*Tamb�m cancelar os n� de blocos e digitais n�o utilizados, inclusive os n�o liberados (n�o vendidos).*/
      update IDAF.tab_numero_cfo_cfoc set situacao = 0, motivo = 'Cancelamento de n�meros do ano anterior (autom�tico)'
      --select count(1) from IDAF.tab_numero_cfo_cfoc
        where utilizado = 0 /*n�o utilizado*/
        and to_char(numero) not like '__'|| to_char(sysdate, 'yy') ||'%';
  --- fim Blocos CFO e CFOC:
  
  
  /*O Saldo das Culturas deve ser estornado conforme a quantidade especificada no documento cancelado.
        Ex:
          PTV baseada em CFO/PTV � Devolver o Saldo da Cultura indicada no PTV para cada UP de Origem especificada no CFO ou PTV;
          CFO - Devolver o Saldo da Cultura indicada no CFO para cada UP de Origem;
          CFOC - Devolver o Saldo da Cultura indicada no CFOC para o Lote de Origem;
  */
  
  end;
  exception
     when others then
       rollback to cancelar_CFOCFOCPTV_savepoint;
  /* commitar todas as altera��es se n�o teve rollback implicito */
  commit;
end;  