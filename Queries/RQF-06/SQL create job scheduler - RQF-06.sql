BEGIN
DBMS_SCHEDULER.CREATE_JOB (
   job_name             => 'job_cancelar_CFOCFOCPTV_anual',
   job_type             => 'STORED_PROCEDURE',
   job_action           => 'cancelar_CFOCFOCPTV_anual',
   start_date           => '01/01/17 00:01:00,000000000 AMERICA/SAO_PAULO',
   repeat_interval      => 'FREQ=YEARLY', 
--   end_date             => TO_TIMESTAMP('2999/12/31 23:59:59', 'YYYY/MM/DD HH24:MI:SS'),
   enabled              =>  TRUE,
   comments             => 'RQF-06 - Cancelar Documentos em Elaboração e Nº de Blocos e Digitais não utilizados (anualmente)');
END;

-- Ver se o JOB foi criado => SELECT * FROM DBA_SCHEDULER_JOBS WHERE JOB_NAME = upper('job_cancelar_CFOCFOCPTV_anual'); 

-- Formato TimeStamp => select TO_TIMESTAMP('2017/01/01 00:01:00', 'YYYY/MM/DD HH24:MI:SS'), SYSTIMESTAMP AT LOCAL ZONE from dual;

-- Apagar JOB => BEGIN DBMS_SCHEDULER.drop_job (job_name => upper('job_cancelar_CFOCFOCPTV_anual')); END;
