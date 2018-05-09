update TAB_ATIVIDADE ta
set ta.ATIVIDADE = 'Pilagem de gr�os (exclusivo para piladoras fixas), n�o associada � secagem mec�nica'
where ta.atividade = 'Pilagem de gr�o (exclusivo para pilagem fixas), n�o associada � secagem mec�nica'
;

update TAB_ATIVIDADE ta
set ta.ATIVIDADE = 'Terraplenagem (corte e aterro) quando vinculada � atividade n�o sujeita ao licenciamento ambiental (exclusivo para a terraplenagem executada no interior da propriedade rural e com objetivo agropecu�rio, inclusive carreadores)'
where ta.atividade = 'Terraplanagem (corte e aterro) quando vinculada � atividade n�o sujeita ao licenciamento ambiental (exclusivo para a terraplanagem executada no interior da propriedade rural e com objetivo agropecu�rio)'
;

update TAB_ATIVIDADE ta
set ta.ATIVIDADE = 'Posto e central de recebimento de embalagens de agrot�xicos'
where ta.atividade = 'Posto e central de recebimento de embalagens vazias de agrot�xicos'
;

update TAB_ATIVIDADE ta
set ta.ATIVIDADE = 'Implanta��o, manuten��o e/ou renova��o de pastagens e/ou de culturas anuais e/ou perenes, exceto silvicultura'
where ta.atividade = 'Irriga��o, implanta��o e/ou renova��o de pastagem e/ou de culturas anuais e/ou perenes inclusive para produ��o de combust�veis (cana-de-a��car, mamona e outras) exceto empreendimentos de silvicultura'
;

update TAB_ATIVIDADE ta
set ta.ATIVIDADE = 'Produ��o artesanal de alimentos e bebidas'
where ta.atividade = 'Empreendimentos rurais ou de agroturismo para produ��o artesanal/ n�o industrial de alimentos e bebidas (sem uso de equipamentos industriais)'
;

update TAB_ATIVIDADE ta
set ta.ATIVIDADE = 'Cria��o de animais de m�dio ou grande porte confinados em ambiente n�o aqu�tico, exceto atividades com enquadramento pr�prio e fauna silvestre'
where ta.atividade = 'Cria��o de animais de m�dio porte confinados em ambiente n�o aqu�tico, exceto fauna silvestre e/ou ex�tica'
;

update TAB_ATIVIDADE ta
set ta.ATIVIDADE = 'Cria��o de animais de pequeno porte confinados em ambiente n�o aqu�tico, exceto atividades com enquadramento pr�prio e fauna silvestre'
where ta.atividade = 'Cria��o de animais de pequeno porte confinados, em ambiente n�o aqu�tico, exceto fauna silvestre e/ou ex�tica (cunicultura e outros)'
;

update TAB_ATIVIDADE ta
set ta.ATIVIDADE = 'Central de sele��o, tratamento e embalagem de produtos vegetais (Packing House)'
where ta.atividade = 'Central de sele��o, tratamento e embalagem de produtos vegetais  (frutas, legumes, tub�rculos e outros) Packing House'
;

update TAB_ATIVIDADE ta
set ta.ATIVIDADE = 'Suinocultura (exclusivo para termina��o) com gera��o de efluente l�quido'
where ta.atividade = 'Suinocultura (exclusivo para termina��o) sem lan�amento de efluentes l�quidos em corpo h�drico e/ou em cama sobreposta'
;

update TAB_ATIVIDADE ta
set ta.ATIVIDADE = 'Suinocultura (exclusivo para produ��o de leit�es / maternidade) com gera��o de efluente l�quido'
where ta.atividade = 'Suinocultura (exclusivo para produ��o de leit�es / maternidade) sem lan�amento de efluentes l�quidos em corpo h�drico e/ou em cama sobreposta'
;

update TAB_ATIVIDADE ta
set ta.ATIVIDADE = 'Secagem mec�nica de gr�os, n�o associada � pilagem'
where ta.atividade = 'Secagem mec�nica de gr�os associado a pilagem'
;

update TAB_ATIVIDADE ta
set ta.ATIVIDADE = 'Secagem mec�nica de gr�os, associada ou n�o � pilagem'
where ta.atividade = 'Secagem mec�nica de gr�os'
;

update TAB_ATIVIDADE ta
set ta.ATIVIDADE = 'Produ��o de carv�o vegetal'
where ta.atividade = 'Produ��o de carv�o vegetal, exclusivo para fornos n�o industriais � Licenciamento'
;

update TAB_ATIVIDADE ta
set ta.ATIVIDADE = 'Fabrica��o de caixas de madeira para uso agropecu�rio e paletes'
where ta.atividade = 'Fabrica��o de estruturas de madeira com aplica��o rural (caixas, porteiras, batentes, carro�as, dentre outros) associada ou n�o � serraria'
;

update TAB_ATIVIDADE ta
set ta.ATIVIDADE = 'Serraria (somente desdobra de madeira)'
where ta.atividade = 'Serrarias, quando n�o associadas � fabrica��o de estruturas de madeira'
;

update TAB_ATIVIDADE ta
set ta.ATIVIDADE = 'Fabrica��o de aguardente associada ou n�o ao envase (inclusive de terceiros)'
where ta.atividade = 'Fabrica��o de aguardente'
;

update TAB_ATIVIDADE ta
set ta.ATIVIDADE = 'Fabrica��o de ra��es balanceadas para animais, sem cozimento e/ou digest�o (apenas mistura)'
where ta.atividade = 'Fabrica��o de ra��es balanceadas e de alimentos preparados para animais sem cozimento e/ou digest�o (apenas mistura)'
;

update TAB_ATIVIDADE ta
set ta.ATIVIDADE = 'Suinocultura (ciclo completo) com gera��o de efluente l�quido'
where ta.atividade = 'Suinocultura (ciclo completo) sem lan�amento de efluentes l�quidos em corpo h�drico e/ou em cama sobreposta'
;