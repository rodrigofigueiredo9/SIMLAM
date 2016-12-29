
Informações extraídas de 
	namespace Tecnomapas.EtramiteX.Interno.Model.ModuloCredenciado.Data
		public class LiberacaoNumeroCFOCFOCDa

++++++++++++++++++++++++++++++++++++++ if CFO:

declare
	v_aux            number := 0;
	v_maior          number := 0;
	v_quantidade_lib number := :quantidade_lib;
begin
	select nvl((select max(d.numero) from tab_numero_cfo_cfoc d where d.tipo_documento = :tipo_documento and d.tipo_numero = :tipo_numero),
		(select min(c.numero_inicial) - 1 from cnf_doc_fito_intervalo c where c.tipo_documento = :tipo_documento and c.tipo = :tipo_numero))
	into v_maior from dual;

	for j in 1..v_quantidade_lib loop 
		v_maior := v_maior + 1;

		select count(1) into v_aux from cnf_doc_fito_intervalo c where c.tipo_documento = :tipo_documento 
		and c.tipo = :tipo_numero and (v_maior between c.numero_inicial and c.numero_final);

		if (v_aux > 0) then
			insert into tab_numero_cfo_cfoc (id, numero, tipo_documento, tipo_numero, liberacao, situacao, utilizado, tid) 
			values (seq_tab_numero_cfo_cfoc.nextval, v_maior, :tipo_documento, :tipo_numero, :liberacao, 1, 0, :tid);
		else 
			v_aux := v_maior;

			select min(t.numero_inicial) into v_maior from cnf_doc_fito_intervalo t 
			where t.tipo_documento = :tipo_documento and t.tipo = :tipo_numero and t.numero_inicial > v_maior;

			if(v_maior is null or v_aux = v_maior) then 
				--Tratamento de exceção
				Raise_application_error(-20023, 'Número não configurado');
			else 
				insert into tab_numero_cfo_cfoc (id, numero, tipo_documento, tipo_numero, liberacao, situacao, utilizado, tid) 
				values (seq_tab_numero_cfo_cfoc.nextval, v_maior, :tipo_documento, :tipo_numero, :liberacao, 1, 0, :tid);
			end if;
		end if;
	end loop;
end;

++++++++++++++++++++++++++++++++++++++ if CFOC:


declare
	v_aux            number := 0;
	v_maior          number := 0;
	v_quantidade_lib number := :quantidade_lib;
begin
	select nvl((select max(d.numero) from tab_numero_cfo_cfoc d where d.tipo_documento = :tipo_documento and d.tipo_numero = :tipo_numero),
		(select min(c.numero_inicial) - 1 from cnf_doc_fito_intervalo c where c.tipo_documento = :tipo_documento and c.tipo = :tipo_numero))
	into v_maior from dual;

	for j in 1..v_quantidade_lib loop 
		v_maior := v_maior + 1;

		select count(1) into v_aux from cnf_doc_fito_intervalo c where c.tipo_documento = :tipo_documento 
		and c.tipo = :tipo_numero and (v_maior between c.numero_inicial and c.numero_final);

		if (v_aux > 0) then
			insert into tab_numero_cfo_cfoc (id, numero, tipo_documento, tipo_numero, liberacao, situacao, utilizado, tid) 
			values (seq_tab_numero_cfo_cfoc.nextval, v_maior, :tipo_documento, :tipo_numero, :liberacao, 1, 0, :tid);
		else 
			v_aux := v_maior;

			select min(t.numero_inicial) into v_maior from cnf_doc_fito_intervalo t 
			where t.tipo_documento = :tipo_documento and t.tipo = :tipo_numero and t.numero_inicial > v_maior;

			if(v_maior is null or v_aux = v_maior) then 
				--Tratamento de exceção
				Raise_application_error(-20023, 'Número não configurado');
			else 
				insert into tab_numero_cfo_cfoc (id, numero, tipo_documento, tipo_numero, liberacao, situacao, utilizado, tid) 
				values (seq_tab_numero_cfo_cfoc.nextval, v_maior, :tipo_documento, :tipo_numero, :liberacao, 1, 0, :tid);
			end if;
		end if;
	end loop;
end;

++++++++++++++++++++++++++++++++++++++ if Cancelar:

update tab_numero_cfo_cfoc set motivo =:motivo, situacao = 0, tid = :tid where id = :id

++++++++++++++++++++++++++++++++++++++ CFOC numero bloco liberado:
internal bool NumeroLiberado(string numero)

select count(*) from tab_numero_cfo_cfoc n, tab_liberacao_cfo_cfoc l where 
	l.id = n.liberacao and l.responsavel_tecnico = :credenciado_id and n.numero = :numero and n.tipo_documento = 2 and n.tipo_numero = 1
	
++++++++++++++++++++++++++++++++++++++ CFOC numero digital liberado:
internal bool NumeroDigitalDisponivel()

select count(*) from tab_numero_cfo_cfoc n, tab_liberacao_cfo_cfoc l 
	where l.id = n.liberacao and n.tipo_documento = 2 and n.tipo_numero = 2 and n.situacao = 1 and n.utilizado = 0 
	and not exists (select null from cre_cfoc c where c.numero = n.numero) 
	and l.responsavel_tecnico = :credenciado 
	
++++++++++++++++++++++++++++++++++++++ CFOC setar numero utilizado:
public int SetarNumeroUtilizado(string numero, int tipoNumero, eDocumentoFitossanitarioTipo tipoDocumento)

update tab_numero_cfo_cfoc t set t.utilizado = 1 
	where t.tipo_documento = :tipo_documento and t.tipo_numero = :tipo_numero and t.numero = :numero 
	returning t.id into :id	


++++++++++++++++++++++++++++++++++++++ INFO AUX:

TAB_NUMERO_CFO_CFOC:
| Coluna         | Descrição                                                                                               | Tipo               |
|----------------|---------------------------------------------------------------------------------------------------------|--------------------|
| ID             | Chave primária de identificação da tabela.                                                              | NUMBER(38,0)       |
| NUMERO         | Número liberado.                                                                                        | NUMBER(38,0)       |
| TIPO_DOCUMENTO | Tipo do documento. 1 - CFO / 2 - CFOC.                                                                  | NUMBER(1,0)        |
| TIPO_NUMERO    | Tipo do número. 1 - Bloco / 2 - Digital.                                                                | NUMBER(1,0)        |
| LIBERACAO      | Chave estrangeira para tab_liberacao_cfo_cfoc, campo(ID).                                               | NUMBER(38,0)       |
| SITUACAO       | Situação do número. 1 - Válido / 0 - Inválido.                                                          | NUMBER(1,0)        |
| UTILIZADO      | Se o número ja foi utilizado. 1 - Sim / 0 - Não.                                                        | NUMBER(1,0)        |
| MOTIVO         | Motivo da situação.                                                                                     | VARCHAR2(500 BYTE) |
| TID            | Id transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction. | VARCHAR2(36 BYTE)  |


CNF_DOC_FITO_INTERVALO:
| Coluna         | Descrição                                                                                               | Tipo              |
|----------------|---------------------------------------------------------------------------------------------------------|-------------------|
| ID             | Chave primária de identificação da tabela.                                                              | NUMBER(38,0)      |
| TID            | Id transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction. | VARCHAR2(36 BYTE) |
| CONFIGURACAO   | Chave estrangeira da tabela tab_config_doc_fitossani. (CNF_DOC_FITOSSANITARIO)                          | NUMBER(38,0)      |
| TIPO_DOCUMENTO | Chave estrangeira na tabela lov_doc_fitossanitarios_tipo.                                               | NUMBER(2,0)       |
| TIPO           | Define o tipo de registro 1 - Bloco/2 - Digital.                                                        | NUMBER(2,0)       |
| NUMERO_INICIAL | Número inicial.                                                                                         | NUMBER(10,0)      |
| NUMERO_FINAL   | Número Final.                                                                                           | NUMBER(10,0)      |

++++++++++++++++++++++++++++++++++++++ Tabelas de CFO e CFOC (Credenciado)

CFO:
| Coluna                         | Tipo               | Nulo | Descrição                                                                                                        |
|--------------------------------|--------------------|------|------------------------------------------------------------------------------------------------------------------|
| ID                             | NUMBER(38,0)       | No   | Chave primária de identificação da tabela.                                                                       |
| TID                            | VARCHAR2(36 BYTE)  | No   | Id transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.          |
| TIPO_NUMERO                    | NUMBER(1,0)        | No   | Chave estrangeira para lov_doc_fitossani_tipo_numero. Campo(ID).                                                 |
| NUMERO                         | NUMBER(10,0)       | No   | Número do CFO.                                                                                                   |
| DATA_EMISSAO                   | DATE               | No   | Data de emissão do CFO.                                                                                          |
| DATA_ATIVACAO                  | DATE               | Yes  | Data de ativação do CFO.                                                                                         |
| SITUACAO                       | NUMBER(1,0)        | No   | Chave estrangeira para lov_doc_fitossani_situacao. Campo(ID).                                                    |
| PRODUTOR                       | NUMBER(38,0)       | No   | Chave estrangeira para tab_pessoa do institucional. Campo(ID).                                                   |
| EMPREENDIMENTO                 | NUMBER(38,0)       | No   | Chave estrangeira para tab_empreendimento do institucional. Campo(ID).                                           |
| POSSUI_LAUDO_LABORATORIAL      | NUMBER(1,0)        | No   | Possui laudo laboratorial? 0 - Não / 1 - Sim.                                                                    |
| NOME_LABORATORIO               | VARCHAR2(100 BYTE) | Yes  | Nome do laboratório.                                                                                             |
| NUMERO_LAUDO_RESULTADO_ANALISE | VARCHAR2(15 BYTE)  | Yes  | Número do laudo com o resultado da análise.                                                                      |
| ESTADO                         | NUMBER(38,0)       | Yes  | Chave estrangeira para lov_estado. Campo(ID).                                                                    |
| MUNICIPIO                      | NUMBER(38,0)       | Yes  | Chave estrangeira para lov_municipio. Campo(ID).                                                                 |
| PRODUTO_ESPECIFICACAO          | NUMBER(10,0)       | No   | BITAND do certifico que, mediante acompanhamento técnico, o(s) produto(s) acima especificado(s) se apresenta(m). |
| POSSUI_TRAT_FITO_FINS_QUAREN   | NUMBER(1,0)        | No   | Possui tratamento fitossanitário para fins quarentenários? 0 - Não / 1 - Sim.                                    |
| PARTIDA_LACRADA_ORIGEM         | NUMBER(1,0)        | No   | Partida lacrada na origem? 0 - Não / 1 - Sim.                                                                    |
| NUMERO_LACRE                   | VARCHAR2(15 BYTE)  | Yes  | Número do lacre.                                                                                                 |
| NUMERO_PORAO                   | VARCHAR2(15 BYTE)  | Yes  | Número do porão.                                                                                                 |
| NUMERO_CONTAINER               | VARCHAR2(15 BYTE)  | Yes  | Número do container.                                                                                             |
| VALIDADE_CERTIFICADO           | NUMBER(3,0)        | No   | Validade do certificado (dias).                                                                                  |
| INFORMACOES_COMPLEMENTARES     | CLOB               | Yes  | Informações complementares.                                                                                      |
| CREDENCIADO                    | NUMBER(38,0)       | No   | Chave estrangeira para tab_credenciado. Campo(ID)                                                                |
| ESTADO_EMISSAO                 | NUMBER(38,0)       | Yes  | Chave estrangeira para lov_estado. Campo(ID).                                                                    |
| MUNICIPIO_EMISSAO              | NUMBER(38,0)       | Yes  | Chave estrangeira para lov_municipio. Campo(ID).                                                                 |
| INFORMACOES_COMPLEMENT_HTML    | CLOB               | Yes  | Informações complementares no formato HTML.                                                                      |


CFOC:
| Coluna                         | Tipo               | Nulo | Descrição                                                                                                        |
|--------------------------------|--------------------|------|------------------------------------------------------------------------------------------------------------------|
| ID                             | NUMBER(38,0)       | No   | Chave primária de identificação da tabela.                                                                       |
| TID                            | VARCHAR2(36 BYTE)  | No   | Id transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.          |
| TIPO_NUMERO                    | NUMBER(1,0)        | No   | Chave estrangeira para lov_doc_fitossani_tipo_numero. Campo(ID).                                                 |
| NUMERO                         | NUMBER(10,0)       | No   | Número do CFOC.                                                                                                  |
| DATA_EMISSAO                   | DATE               | No   | Data de emissão do CFOC.                                                                                         |
| DATA_ATIVACAO                  | DATE               | Yes  | Data de ativação do CFOC.                                                                                        |
| SITUACAO                       | NUMBER(1,0)        | No   | Chave estrangeira para lov_doc_fitossani_situacao. Campo(ID).                                                    |
| EMPREENDIMENTO                 | NUMBER(38,0)       | No   | Chave estrangeira para tab_empreendimento do institucional. Campo(ID).                                           |
| POSSUI_LAUDO_LABORATORIAL      | NUMBER(1,0)        | No   | Possui laudo laboratorial? 0 - Não / 1 - Sim.                                                                    |
| NOME_LABORATORIO               | VARCHAR2(100 BYTE) | Yes  | Nome do laboratório.                                                                                             |
| NUMERO_LAUDO_RESULTADO_ANALISE | VARCHAR2(15 BYTE)  | Yes  | Número do laudo com o resultado da análise.                                                                      |
| ESTADO                         | NUMBER(38,0)       | Yes  | Chave estrangeira para lov_estado. Campo(ID).                                                                    |
| MUNICIPIO                      | NUMBER(38,0)       | Yes  | Chave estrangeira para lov_municipio. Campo(ID).                                                                 |
| PRODUTO_ESPECIFICACAO          | NUMBER(10,0)       | No   | BITAND do certifico que, mediante acompanhamento técnico, o(s) produto(s) acima especificado(s) se apresenta(m). |
| POSSUI_TRAT_FITO_FINS_QUAREN   | NUMBER(1,0)        | No   | Possui tratamento fitossanitário para fins quarentenários? 0 - Não / 1 - Sim.                                    |
| PARTIDA_LACRADA_ORIGEM         | NUMBER(1,0)        | No   | Partida lacrada na origem? 0 - Não / 1 - Sim.                                                                    |
| NUMERO_LACRE                   | VARCHAR2(15 BYTE)  | Yes  | Número do lacre.                                                                                                 |
| NUMERO_PORAO                   | VARCHAR2(15 BYTE)  | Yes  | Número do porão.                                                                                                 |
| NUMERO_CONTAINER               | VARCHAR2(15 BYTE)  | Yes  | Número do container.                                                                                             |
| VALIDADE_CERTIFICADO           | NUMBER(3,0)        | No   | Validade do certificado (dias).                                                                                  |
| INFORMACOES_COMPLEMENTARES     | CLOB               | Yes  | Informações complementares.                                                                                      |
| CREDENCIADO                    | NUMBER(38,0)       | No   | Chave estrangeira para tab_credenciado. Campo(ID)                                                                |
| ESTADO_EMISSAO                 | NUMBER(38,0)       | Yes  | Chave estrangeira para lov_estado. Campo(ID).                                                                    |
| MUNICIPIO_EMISSAO              | NUMBER(38,0)       | Yes  | Chave estrangeira para lov_municipio. Campo(ID).                                                                 |
| INFORMACOES_COMPLEMENT_HTML    | CLOB               | Yes  | Informações complementares no formato HTML.                                                                      |

