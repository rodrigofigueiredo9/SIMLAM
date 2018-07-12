/**
  RQF-06	Cancelar Documentos em Elaboração e Nº de Blocos e Digitais não utilizados
*/

/*Deixar como agendamento anual. Se repetirá a cada virada de ano.*/

begin

  savepoint cancelar_CFOCFOCPTV_savepoint;

  begin
  /*Cancelar todos os documentos de CFO, CFOC e PTV na situação em Elaboração no ano anterior, no dia 01 de Janeiro do ano atual, às 00:01. */
  --- CFO:
      update IDAFCREDENCIADO.TAB_CFO set SITUACAO = 4 /*Cancelado*/
        --select * from IDAFCREDENCIADO.TAB_CFO 
        where SITUACAO = 1 /*Em elaboração*/
        and DATA_EMISSAO < to_date('01/01/' || to_char(sysdate, 'yyyy'), 'dd/mm/yyyy');
  --- fim-CFO
  
  
  --- CFOC:    
      /*Voltar o Lote do CFOC cancelado para Não Utilizado*/
      update IDAFCREDENCIADO.tab_lote set situacao = 2 /*Não Utilizado*/
        where tab_lote.id in(
          select tab_cfoc_produto.lote
          from IDAFCREDENCIADO.tab_cfoc_produto
          where cfoc in(select id from IDAFCREDENCIADO.TAB_CFOC
              where SITUACAO = 1 /*Em elaboração*/
              and DATA_EMISSAO < to_date('01/01/' || to_char(sysdate, 'yyyy'), 'dd/mm/yyyy')
           )
        );
      /*Apagar os Lotes inseridos no CFOC cancelado*/
      delete from IDAFCREDENCIADO.tab_cfoc_produto
      where cfoc in(select id from IDAFCREDENCIADO.TAB_CFOC
          where SITUACAO = 1 /*Em elaboração*/
          and DATA_EMISSAO < to_date('01/01/' || to_char(sysdate, 'yyyy'), 'dd/mm/yyyy')
        );
      /*Marcar o CFOC como cancelado:*/
      update IDAFCREDENCIADO.TAB_CFOC set SITUACAO = 4 /*Cancelado*/
        --select count(1) from IDAFCREDENCIADO.TAB_CFOC
        where SITUACAO = 1 /*Em elaboração*/
        and DATA_EMISSAO < to_date('01/01/' || to_char(sysdate, 'yyyy'), 'dd/mm/yyyy');
  --- fim-CFOC
    
  --- PTV (Institucional):
      update IDAF.TAB_PTV 
          set SITUACAO = 3, /*Cancelado*/
              DATA_CANCELAMENTO = sysdate
        --select count(1) from IDAF.TAB_PTV
        where SITUACAO = 1 /*Em elaboração*/
        and DATA_EMISSAO < to_date('01/01/' || to_char(sysdate, 'yyyy'), 'dd/mm/yyyy');
  --- fim-PTV    
  
  --- ePTV:
  /*    update IDAFCREDENCIADO.TAB_PTV set SITUACAO = ? 4	Recusado / 6	Bloqueado ?
        --select count(1) from IDAFCREDENCIADO.TAB_PTV
        where SITUACAO in(
              1, --Cadastrado
              2  --Aguardando análise
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
    /*Também cancelar os nº de blocos e digitais não utilizados, inclusive os não liberados (não vendidos).*/
      update IDAF.tab_numero_cfo_cfoc set situacao = 0, motivo = 'Cancelamento de números do ano anterior (automático)'
      --select count(1) from IDAF.tab_numero_cfo_cfoc
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
  /* commitar todas as alterações se não teve rollback implicito */
  commit;
end;  