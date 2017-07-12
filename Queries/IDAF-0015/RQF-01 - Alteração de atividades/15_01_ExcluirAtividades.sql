delete
from CNF_ATIVIDADE_ATIVIDADES caa
where caa.CONFIGURACAO in (select ca.id
                           from cnf_atividade ca
                           where ca.nome = 'DRNRE - CLAM: AA')
      and caa.ATIVIDADE in (select ta.id
                            from tab_atividade ta
                            where ta.atividade = 'Pulveriza��o a�rea de produtos agrot�xicos, seus componentes e afins')
;

update TAB_ATIVIDADE ta
set ta.situacao = 0
where ta.atividade = 'Pulveriza��o a�rea de produtos agrot�xicos, seus componentes e afins'
;

-----------------------------------------

delete
from CNF_ATIVIDADE_ATIVIDADES caa
where caa.CONFIGURACAO in (select ca.id
                           from cnf_atividade ca
                           where ca.nome = 'DRNRE - CLAM: LS, LP, LI, LO, LAR')
      and caa.ATIVIDADE in (select ta.id
                            from tab_atividade ta
                            where ta.atividade = 'P�tio de lavagem, abastecimento e descontamina��o de aeronave agr�cola')
;

update TAB_ATIVIDADE ta
set ta.situacao = 0
where ta.atividade = 'P�tio de lavagem, abastecimento e descontamina��o de aeronave agr�cola'
;

-----------------------------------------

delete
from CNF_ATIVIDADE_ATIVIDADES caa
where caa.CONFIGURACAO in (select ca.id
                           from cnf_atividade ca
                           where ca.nome = 'DRNRE - CLAM: LS, LP, LI, LO, LAR, AA')
      and caa.ATIVIDADE in (select ta.id
                            from tab_atividade ta
                            where ta.atividade = 'Cria��o de animais de grande porte confinados em ambiente n�o aqu�tico, exceto fauna silvestre e/ou ex�tica')
;

update TAB_ATIVIDADE ta
set ta.situacao = 0
where ta.atividade = 'Cria��o de animais de grande porte confinados em ambiente n�o aqu�tico, exceto fauna silvestre e/ou ex�tica'
;

-----------------------------------------

delete
from CNF_ATIVIDADE_ATIVIDADES caa
where caa.CONFIGURACAO in (select ca.id
                           from cnf_atividade ca
                           where ca.nome = 'DRNRE - CLAM: LS, LP, LI, LO, LAR, AA')
      and caa.ATIVIDADE in (select ta.id
                            from tab_atividade ta
                            where ta.atividade = 'Cria��o de animais de grande porte semi-confinados em ambiente n�o aqu�tico, exceto fauna silvestre e/ou ex�tica')
;

update TAB_ATIVIDADE ta
set ta.situacao = 0
where ta.atividade = 'Cria��o de animais de grande porte semi-confinados em ambiente n�o aqu�tico, exceto fauna silvestre e/ou ex�tica'
;

-----------------------------------------

delete
from CNF_ATIVIDADE_ATIVIDADES caa
where caa.CONFIGURACAO in (select ca.id
                           from cnf_atividade ca
                           where ca.nome = 'DRNRE - CLAM: LS, LP, LI, LO, LAR, AA')
      and caa.ATIVIDADE in (select ta.id
                            from tab_atividade ta
                            where ta.atividade = 'Cria��o de animais de m�dio porte semi-confinados em ambiente n�o aqu�tico, exceto fauna silvestre e/ou ex�tica')
;

update TAB_ATIVIDADE ta
set ta.situacao = 0
where ta.atividade = 'Cria��o de animais de m�dio porte semi-confinados em ambiente n�o aqu�tico, exceto fauna silvestre e/ou ex�tica'
;

-----------------------------------------

delete
from CNF_ATIVIDADE_ATIVIDADES caa
where caa.CONFIGURACAO in (select ca.id
                           from cnf_atividade ca
                           where ca.nome = 'DRNRE - CLAM: LS, LP, LI, LO, LAR, AA')
      and caa.ATIVIDADE in (select ta.id
                            from tab_atividade ta
                            where ta.atividade = 'Suinocultura com lan�amento de efluentes l�quidos, exclusivo para subsist�ncia')
;

update TAB_ATIVIDADE ta
set ta.situacao = 0
where ta.atividade = 'Suinocultura com lan�amento de efluentes l�quidos, exclusivo para subsist�ncia'
;