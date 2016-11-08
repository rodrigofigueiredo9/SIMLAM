delete tab_scheduler_fila f
 where f.requisitante in (select ff.requisitante
                            from tab_scheduler_fila ff
                           where ff.resultado like '{"codigoResposta":500,%'
                             and ff.tipo = 'enviar-car')
   and f.tipo = 'revisar-resposta-car'
   and f.resultado like '{"codigoResposta":500,%';

update tab_scheduler_fila f
   set f.data_criacao   = null,
       f.data_conclusao = null,
       f.resultado      = null,
       f.sucesso        = null
 where f.id in (select f.id
                  from tab_scheduler_fila f
                 where f.resultado like '{"codigoResposta":500,%'
                   and f.tipo = 'enviar-car');

commit;
