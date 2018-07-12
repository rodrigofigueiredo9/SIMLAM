update TAB_ATIVIDADE ta
set ta.ATIVIDADE = 'Pilagem de grãos (exclusivo para piladoras fixas), não associada à secagem mecânica'
where ta.atividade = 'Pilagem de grão (exclusivo para pilagem fixas), não associada à secagem mecânica'
;

update TAB_ATIVIDADE ta
set ta.ATIVIDADE = 'Terraplenagem (corte e aterro) quando vinculada à atividade não sujeita ao licenciamento ambiental (exclusivo para a terraplenagem executada no interior da propriedade rural e com objetivo agropecuário, inclusive carreadores)'
where ta.atividade = 'Terraplanagem (corte e aterro) quando vinculada à atividade não sujeita ao licenciamento ambiental (exclusivo para a terraplanagem executada no interior da propriedade rural e com objetivo agropecuário)'
;

update TAB_ATIVIDADE ta
set ta.ATIVIDADE = 'Posto e central de recebimento de embalagens de agrotóxicos'
where ta.atividade = 'Posto e central de recebimento de embalagens vazias de agrotóxicos'
;

update TAB_ATIVIDADE ta
set ta.ATIVIDADE = 'Implantação, manutenção e/ou renovação de pastagens e/ou de culturas anuais e/ou perenes, exceto silvicultura'
where ta.atividade = 'Irrigação, implantação e/ou renovação de pastagem e/ou de culturas anuais e/ou perenes inclusive para produção de combustíveis (cana-de-açúcar, mamona e outras) exceto empreendimentos de silvicultura'
;

update TAB_ATIVIDADE ta
set ta.ATIVIDADE = 'Produção artesanal de alimentos e bebidas'
where ta.atividade = 'Empreendimentos rurais ou de agroturismo para produção artesanal/ não industrial de alimentos e bebidas (sem uso de equipamentos industriais)'
;

update TAB_ATIVIDADE ta
set ta.ATIVIDADE = 'Criação de animais de médio ou grande porte confinados em ambiente não aquático, exceto atividades com enquadramento próprio e fauna silvestre'
where ta.atividade = 'Criação de animais de médio porte confinados em ambiente não aquático, exceto fauna silvestre e/ou exótica'
;

update TAB_ATIVIDADE ta
set ta.ATIVIDADE = 'Criação de animais de pequeno porte confinados em ambiente não aquático, exceto atividades com enquadramento próprio e fauna silvestre'
where ta.atividade = 'Criação de animais de pequeno porte confinados, em ambiente não aquático, exceto fauna silvestre e/ou exótica (cunicultura e outros)'
;

update TAB_ATIVIDADE ta
set ta.ATIVIDADE = 'Central de seleção, tratamento e embalagem de produtos vegetais (Packing House)'
where ta.atividade = 'Central de seleção, tratamento e embalagem de produtos vegetais  (frutas, legumes, tubérculos e outros) Packing House'
;

update TAB_ATIVIDADE ta
set ta.ATIVIDADE = 'Suinocultura (exclusivo para terminação) com geração de efluente líquido'
where ta.atividade = 'Suinocultura (exclusivo para terminação) sem lançamento de efluentes líquidos em corpo hídrico e/ou em cama sobreposta'
;

update TAB_ATIVIDADE ta
set ta.ATIVIDADE = 'Suinocultura (exclusivo para produção de leitões / maternidade) com geração de efluente líquido'
where ta.atividade = 'Suinocultura (exclusivo para produção de leitões / maternidade) sem lançamento de efluentes líquidos em corpo hídrico e/ou em cama sobreposta'
;

update TAB_ATIVIDADE ta
set ta.ATIVIDADE = 'Secagem mecânica de grãos, não associada à pilagem'
where ta.atividade = 'Secagem mecânica de grãos associado a pilagem'
;

update TAB_ATIVIDADE ta
set ta.ATIVIDADE = 'Secagem mecânica de grãos, associada ou não à pilagem'
where ta.atividade = 'Secagem mecânica de grãos'
;

update TAB_ATIVIDADE ta
set ta.ATIVIDADE = 'Produção de carvão vegetal'
where ta.atividade = 'Produção de carvão vegetal, exclusivo para fornos não industriais ¿ Licenciamento'
;

update TAB_ATIVIDADE ta
set ta.ATIVIDADE = 'Fabricação de caixas de madeira para uso agropecuário e paletes'
where ta.atividade = 'Fabricação de estruturas de madeira com aplicação rural (caixas, porteiras, batentes, carroças, dentre outros) associada ou não à serraria'
;

update TAB_ATIVIDADE ta
set ta.ATIVIDADE = 'Serraria (somente desdobra de madeira)'
where ta.atividade = 'Serrarias, quando não associadas à fabricação de estruturas de madeira'
;

update TAB_ATIVIDADE ta
set ta.ATIVIDADE = 'Fabricação de aguardente associada ou não ao envase (inclusive de terceiros)'
where ta.atividade = 'Fabricação de aguardente'
;

update TAB_ATIVIDADE ta
set ta.ATIVIDADE = 'Fabricação de rações balanceadas para animais, sem cozimento e/ou digestão (apenas mistura)'
where ta.atividade = 'Fabricação de rações balanceadas e de alimentos preparados para animais sem cozimento e/ou digestão (apenas mistura)'
;

update TAB_ATIVIDADE ta
set ta.ATIVIDADE = 'Suinocultura (ciclo completo) com geração de efluente líquido'
where ta.atividade = 'Suinocultura (ciclo completo) sem lançamento de efluentes líquidos em corpo hídrico e/ou em cama sobreposta'
;