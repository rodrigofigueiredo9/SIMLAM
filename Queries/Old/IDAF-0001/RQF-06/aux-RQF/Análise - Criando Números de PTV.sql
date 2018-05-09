
++++++++++++++++++++++++++++++++++++++ Institucional > PTV > Criar

--internal string ObterNumeroDigital()

declare
	v_aux   number := 0;
	v_maior number := 0;
begin
	select nvl((select max(d.numero) from tab_ptv d where d.tipo_numero = 2),
		(select min(c.numero_inicial) - 1 from cnf_doc_fito_intervalo c where c.tipo_documento = 3 and c.tipo = 2))
	into v_maior from dual;

	v_maior := v_maior + 1;

	select count(1) into v_aux from cnf_doc_fito_intervalo c 
	where c.tipo_documento = 3 and c.tipo = 2 
	and substr(c.numero_inicial, 3, 2) = substr(EXTRACT(YEAR FROM sysdate), 3) 
	and substr(c.numero_final, 3, 2) = substr(EXTRACT(YEAR FROM sysdate), 3) 
	and (v_maior between c.numero_inicial and c.numero_final);

	if (v_aux <= 0) then
		v_aux := v_maior;

		select min(t.numero_inicial) into v_maior from cnf_doc_fito_intervalo t 
		where t.tipo_documento = 3 and t.tipo = 2 
		and substr(t.numero_inicial, 3, 2) = substr(EXTRACT(YEAR FROM sysdate), 3) 
		and substr(t.numero_final, 3, 2) = substr(EXTRACT(YEAR FROM sysdate), 3)
		and t.numero_inicial > v_maior;

		if(v_maior is null or v_aux = v_maior) then 
			v_maior := null;
		end if;
	end if;
	select v_maior into :numero_saida from dual;
end;

++++++++++++++++++++++++++++++++++++++ tabela PTV - Credenciado

|---------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Coluna                         | Tipo               | Nulo | Descrição                                                                                        |
|--------------------------------|--------------------|------|--------------------------------------------------------------------------------------------------|
| ID                             | NUMBER(38,0)       | No   | Chave primária de identificação da tabela.                                                       |
| TID                            | VARCHAR2(36 BYTE)  | No   | Id transacional.Esse valor garante a ligação entre as tabelas relacionadas com essa transaction. |
| SITUACAO                       | NUMBER(1,0)        | No   | Chave estrangeira tabela lov_solicitacao_ptv_situacao. Campo(ID).                                |
| SITUACAO_DATA                  | DATE               | No   | Data da situação.                                                                                |
| MOTIVO                         | VARCHAR2(500 BYTE) | Yes  | Motivo da situação.                                                                              |
| TIPO_NUMERO                    | NUMBER(1,0)        | No   | Chave estrangeira tabela lov_docfitossani_tipo_numero. Campo(ID).                                |
| NUMERO                         | NUMBER(10,0)       | Yes  | Número do PTV.                                                                                   |
| DUA_NUMERO                     | VARCHAR2(80 BYTE)  | No   | Número do DUA.                                                                                   |
| DUA_TIPO_PESSOA                | NUMBER(38,0)       | No   | Tipo de pessoa. 1 - Física/ 2 - Jurídica                                                         |
| DUA_CPF_CNPJ                   | VARCHAR2(20 BYTE)  | No   | CPF/CNPJ de validação do DUA.                                                                    |
| DATA_EMISSAO                   | DATE               | No   | Data de emissão do PTV.                                                                          |
| DATA_ATIVACAO                  | DATE               | Yes  | Data de ativação do PTV.                                                                         |
| DATA_CANCELAMENTO              | DATE               | Yes  | Data de cancelamento do PTV.                                                                     |
| EMPREENDIMENTO                 | NUMBER(38,0)       | No   | Chave estrangeira para tab_empreendimento. Campo(ID).                                            |
| RESPONSAVEL_EMP                | NUMBER(38,0)       | Yes  | Chave estrangeira para tab_pessoa. Campo(ID).                                                    |
| PARTIDA_LACRADA_ORIGEM         | NUMBER(1,0)        | No   | Partida lacrada na origem. 0/nulo-Não/1-Sim                                                      |
| NUMERO_LACRE                   | VARCHAR2(15 BYTE)  | Yes  | Número do lacre.                                                                                 |
| NUMERO_PORAO                   | VARCHAR2(15 BYTE)  | Yes  | Número do porão.                                                                                 |
| NUMERO_CONTAINER               | VARCHAR2(15 BYTE)  | Yes  | Número do contêiner.                                                                             |
| DESTINATARIO                   | NUMBER(38,0)       | No   | Chave estrangeira para tab_destinatario_ptv. Campo(ID).                                          |
| POSSUI_LAUDO_LABORATORIAL      | NUMBER(1,0)        | No   | Possui laudo laboratorio? 0/nulo - Não / 1 - Sim.                                                |
| NOME_LABORATORIO               | VARCHAR2(100 BYTE) | Yes  | Nome do Laboratório.                                                                             |
| NUMERO_LAUDO_RESULTADO_ANALISE | VARCHAR2(15 BYTE)  | Yes  | Número do laudo com o resultado da análise.                                                      |
| ESTADO                         | NUMBER(38,0)       | Yes  | Chave estrangeira para lov_estado. Campo(ID).                                                    |
| MUNICIPIO                      | NUMBER(38,0)       | Yes  | Chave estrangeira para lov_municipio. Campo(ID).                                                 |
| MUNICIPIO_EMISSAO              | NUMBER(38,0)       | Yes  | Chave estrangeira para lov_municipio. Campo(ID).                                                 |
| TIPO_TRANSPORTE                | NUMBER(2,0)        | No   | Chave estrangeira para lov_tipo_transporte. Campo(ID).                                           |
| VEICULO_IDENTIFICACAO_NUMERO   | VARCHAR2(15 BYTE)  | No   | Identificação do veículo.                                                                        |
| ROTA_TRANSITO_DEFINIDA         | NUMBER(1,0)        | No   | Rota de trnasito definida? 0/nulo - Não / 1 - Sim.                                               |
| ITINERARIO                     | VARCHAR2(200 BYTE) | Yes  | Itinerário para o endereço do destinatário.                                                      |
| APRESENTACAO_NOTA_FISCAL       | NUMBER(1,0)        | No   | Apresentação de nota fiscal? 0/nulo - Não/ 1 - sim.                                              |
| NUMERO_NOTA_FISCAL             | VARCHAR2(60 BYTE)  | Yes  | Número da nota fiscal.                                                                           |
| VALIDO_ATE                     | DATE               | Yes  | Data de validade.                                                                                |
| RESPONSAVEL_TECNICO            | NUMBER(38,0)       | Yes  | Chave estrangeira para tab_funcionario. Campo(ID)                                                |
| LOCAL_VISTORIA                 | NUMBER(38,0)       | No   | Chave estrangeira para tab_setor. Campo(ID)                                                      |
| DATA_HORA_VISTORIA             | NUMBER(38,0)       | No   | Chave estrangeira para cnf_local_vistoria. Campo(ID)                                             |
| CREDENCIADO                    | NUMBER(38,0)       | No   | Chave estrangeira para tab_credenciado. Campo(ID)                                                |
|---------------------------------------------------------------------------------------------------------------------------------------------------------------|

++++++++++++++++++++++++++++++++++++++ tabela PTV - Institucional

|----------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Coluna                         | Tipo                | Nulo | Descrição                                                                                        |
|--------------------------------|---------------------|------|--------------------------------------------------------------------------------------------------|
| ID                             | NUMBER(38,0)        | No   | Chave primária de identificação da tabela.                                                       |
| TID                            | VARCHAR2(36 BYTE)   | No   | Id transacional.Esse valor garante a ligação entre as tabelas relacionadas com essa transaction. |
| TIPO_NUMERO                    | NUMBER(1,0)         | No   | Chave estrangeira tabela lov_docfitossani_tipo_numero. Campo(ID).                                |
| NUMERO                         | NUMBER(10,0)        | Yes  | Número do PTV.                                                                                   |
| DATA_EMISSAO                   | DATE                | No   | Data de emissão do PTV.                                                                          |
| DATA_ATIVACAO                  | DATE                | Yes  | Data de ativação do PTV.                                                                         |
| DATA_CANCELAMENTO              | DATE                | Yes  | Data de cancelamento do PTV.                                                                     |
| SITUACAO                       | NUMBER(1,0)         | No   | Chave estrangeira para lov_ptv_situacao. Campo(ID).                                              |
| EMPREENDIMENTO                 | NUMBER(38,0)        | No   | Chave estrangeira para tab_empreendimento. Campo(ID).                                            |
| RESPONSAVEL_EMP                | NUMBER(38,0)        | Yes  | Chave estrangeira para tab_pessoa. Campo(ID).                                                    |
| PARTIDA_LACRADA_ORIGEM         | NUMBER(1,0)         | No   | Partida lacrada na origem. 0/nulo-Não/1-Sim                                                      |
| NUMERO_LACRE                   | VARCHAR2(15 BYTE)   | Yes  | Número do lacre.                                                                                 |
| NUMERO_PORAO                   | VARCHAR2(15 BYTE)   | Yes  | Número do porão.                                                                                 |
| NUMERO_CONTAINER               | VARCHAR2(15 BYTE)   | Yes  | Número do contêiner.                                                                             |
| DESTINATARIO                   | NUMBER(38,0)        | No   | Chave estrangeira para tab_destinatario_ptv. Campo(ID).                                          |
| POSSUI_LAUDO_LABORATORIAL      | NUMBER(1,0)         | No   | Possui laudo laboratorio? 0/nulo - Não / 1 - Sim.                                                |
| NOME_LABORATORIO               | VARCHAR2(100 BYTE)  | Yes  | Nome do Laboratório.                                                                             |
| NUMERO_LAUDO_RESULTADO_ANALISE | VARCHAR2(15 BYTE)   | Yes  | Número do laudo com o resultado da análise.                                                      |
| ESTADO                         | NUMBER(38,0)        | Yes  | Chave estrangeira para lov_estado. Campo(ID).                                                    |
| MUNICIPIO                      | NUMBER(38,0)        | Yes  | Chave estrangeira para lov_municipio. Campo(ID).                                                 |
| TIPO_TRANSPORTE                | NUMBER(2,0)         | No   | Chave estrangeira para lov_tipo_transporte. Campo(ID).                                           |
| VEICULO_IDENTIFICACAO_NUMERO   | VARCHAR2(15 BYTE)   | No   | Identificação do veículo.                                                                        |
| ROTA_TRANSITO_DEFINIDA         | NUMBER(1,0)         | No   | Rota de trnasito definida? 0/nulo - Não / 1 - Sim.                                               |
| ITINERARIO                     | VARCHAR2(200 BYTE)  | Yes  | Itinerário para o endereço do destinatário.                                                      |
| APRESENTACAO_NOTA_FISCAL       | NUMBER(1,0)         | No   | Apresentação de nota fiscal? 0/nulo - Não/ 1 - sim.                                              |
| NUMERO_NOTA_FISCAL             | VARCHAR2(60 BYTE)   | Yes  | Número da nota fiscal.                                                                           |
| VALIDO_ATE                     | DATE                | No   | Data de validade.                                                                                |
| RESPONSAVEL_TECNICO            | NUMBER(38,0)        | No   | Chave estrangeira para tab_funcionario. Campo(ID)                                                |
| MUNICIPIO_EMISSAO              | NUMBER(38,0)        | Yes  | Chave estrangeira para lov_municipio. Campo(ID).                                                 |
| EPTV_ID                        | NUMBER(38,0)        | Yes  | Chave estrangeira para a idafcredenciado.tab_ptv. Campo(ID).                                     |
| DECLARACAO_ADICIONAL           | VARCHAR2(4000 BYTE) | Yes  | Texto da declaração Adicional.                                                                   |
| DECLARACAO_ADICIONAL_FORMATADO | VARCHAR2(4000 BYTE) | Yes  | Texto da declaração Adicional formatado em HTML.                                                 |
|----------------------------------------------------------------------------------------------------------------------------------------------------------------|

