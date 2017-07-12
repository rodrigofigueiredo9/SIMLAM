delete
from CNF_ATIVIDADE_ATIVIDADES caa
where caa.CONFIGURACAO in (select ca.id
                           from cnf_atividade ca
                           where ca.nome = 'DRNRE - CLAM: AA')
      and caa.ATIVIDADE in (select ta.id
                            from tab_atividade ta
                            where ta.atividade = 'Pulverização aérea de produtos agrotóxicos, seus componentes e afins')
;

update TAB_ATIVIDADE ta
set ta.situacao = 0
where ta.atividade = 'Pulverização aérea de produtos agrotóxicos, seus componentes e afins'
;

-----------------------------------------

delete
from CNF_ATIVIDADE_ATIVIDADES caa
where caa.CONFIGURACAO in (select ca.id
                           from cnf_atividade ca
                           where ca.nome = 'DRNRE - CLAM: LS, LP, LI, LO, LAR')
      and caa.ATIVIDADE in (select ta.id
                            from tab_atividade ta
                            where ta.atividade = 'Pátio de lavagem, abastecimento e descontaminação de aeronave agrícola')
;

update TAB_ATIVIDADE ta
set ta.situacao = 0
where ta.atividade = 'Pátio de lavagem, abastecimento e descontaminação de aeronave agrícola'
;

-----------------------------------------

delete
from CNF_ATIVIDADE_ATIVIDADES caa
where caa.CONFIGURACAO in (select ca.id
                           from cnf_atividade ca
                           where ca.nome = 'DRNRE - CLAM: LS, LP, LI, LO, LAR, AA')
      and caa.ATIVIDADE in (select ta.id
                            from tab_atividade ta
                            where ta.atividade = 'Criação de animais de grande porte confinados em ambiente não aquático, exceto fauna silvestre e/ou exótica')
;

update TAB_ATIVIDADE ta
set ta.situacao = 0
where ta.atividade = 'Criação de animais de grande porte confinados em ambiente não aquático, exceto fauna silvestre e/ou exótica'
;

-----------------------------------------

delete
from CNF_ATIVIDADE_ATIVIDADES caa
where caa.CONFIGURACAO in (select ca.id
                           from cnf_atividade ca
                           where ca.nome = 'DRNRE - CLAM: LS, LP, LI, LO, LAR, AA')
      and caa.ATIVIDADE in (select ta.id
                            from tab_atividade ta
                            where ta.atividade = 'Criação de animais de grande porte semi-confinados em ambiente não aquático, exceto fauna silvestre e/ou exótica')
;

update TAB_ATIVIDADE ta
set ta.situacao = 0
where ta.atividade = 'Criação de animais de grande porte semi-confinados em ambiente não aquático, exceto fauna silvestre e/ou exótica'
;

-----------------------------------------

delete
from CNF_ATIVIDADE_ATIVIDADES caa
where caa.CONFIGURACAO in (select ca.id
                           from cnf_atividade ca
                           where ca.nome = 'DRNRE - CLAM: LS, LP, LI, LO, LAR, AA')
      and caa.ATIVIDADE in (select ta.id
                            from tab_atividade ta
                            where ta.atividade = 'Criação de animais de médio porte semi-confinados em ambiente não aquático, exceto fauna silvestre e/ou exótica')
;

update TAB_ATIVIDADE ta
set ta.situacao = 0
where ta.atividade = 'Criação de animais de médio porte semi-confinados em ambiente não aquático, exceto fauna silvestre e/ou exótica'
;

-----------------------------------------

delete
from CNF_ATIVIDADE_ATIVIDADES caa
where caa.CONFIGURACAO in (select ca.id
                           from cnf_atividade ca
                           where ca.nome = 'DRNRE - CLAM: LS, LP, LI, LO, LAR, AA')
      and caa.ATIVIDADE in (select ta.id
                            from tab_atividade ta
                            where ta.atividade = 'Suinocultura com lançamento de efluentes líquidos, exclusivo para subsistência')
;

update TAB_ATIVIDADE ta
set ta.situacao = 0
where ta.atividade = 'Suinocultura com lançamento de efluentes líquidos, exclusivo para subsistência'
;