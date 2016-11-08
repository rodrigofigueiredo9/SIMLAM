
--==================================================================================--
-- SCRIPTS DE CRIAÇÃO / ALTERAÇÃO DE TABELAS - IDAF
--==================================================================================--

set feedback off
set define off

prompt 
prompt ---------------------------------------------------------------------
prompt INICIANDO SCRIPTS DE CRIAÇÃO / ALTERAÇÃO DE TABELAS
prompt ---------------------------------------------------------------------

----------------------------------------------------------------------------
-- SEQUÊNCIAS
----------------------------------------------------------------------------

create sequence SEQ_FEICAO minvalue 1 maxvalue 999999999999999999999999999 start with 334 increment by 1 nocache;

----------------------------------------------------------------------------
-- TABELAS
----------------------------------------------------------------------------

prompt
prompt -------------------------------------------------
prompt LOV_TIPO_COLUNA
prompt -------------------------------------------------

create table LOV_TIPO_COLUNA
(
  ID    NUMBER(10) not null,
  texto VARCHAR2(80)
);

alter table LOV_TIPO_COLUNA add constraint PK_TIPO_COLUNA primary key (ID);

comment on table LOV_TIPO_COLUNA is 'Lista de valores de tipo de coluna';
comment on column LOV_TIPO_COLUNA.ID is 'Chave primária para lov_tipo_coluna';
comment on column LOV_TIPO_COLUNA.texto is 'Valor do tipo coluna';

prompt
prompt -------------------------------------------------
prompt LOV_TIPO_FEICAO
prompt -------------------------------------------------

create table LOV_TIPO_FEICAO
(
  ID  number(10),
  texto  varchar2(80)
);

alter table LOV_TIPO_FEICAO add constraint PK_TIPO_FEICAO primary key (ID);

comment on table LOV_TIPO_FEICAO is 'Lista de valores de tipo geometrico';
comment on column lov_tipo_feicao.id is 'Chave primária para lov_tipo_feicao';
comment on column lov_tipo_feicao.texto is 'Valor do tipo geometrico';

prompt
prompt -------------------------------------------------
prompt LOV_LAGOA_ZONA
prompt -------------------------------------------------

create table LOV_LAGOA_ZONA
(
       chave varchar2(10),
       texto varchar2(50)
);

alter table LOV_LAGOA_ZONA add constraint PK_LOV_LAGOA_ZONA primary key (chave);

comment on table LOV_LAGOA_ZONA is 'Lista de valores para tabela lagoa coluna zona';
comment on column LOV_LAGOA_ZONA.chave is 'Chave primária para lov_lagoa_zona';
comment on column LOV_LAGOA_ZONA.texto is 'Valor da coluna zona da tabela lagoa';

prompt
prompt -------------------------------------------------
prompt LOV_APMP_TIPO
prompt -------------------------------------------------

create table LOV_APMP_TIPO
(
       chave varchar2(10),
       texto varchar2(50)
);

alter table LOV_APMP_TIPO add constraint PK_LOV_APMP_TIPO primary key (chave);

comment on table LOV_APMP_TIPO is 'Lista de valores para tabela APMP coluna tipo';
comment on column LOV_APMP_TIPO.chave is 'Chave primária para lov_apmp_tipo';
comment on column LOV_APMP_TIPO.texto is 'Valor da coluna tipo da tabela APMP';

prompt
prompt -------------------------------------------------
prompt LOV_AVN_ESTAGIO
prompt -------------------------------------------------

create table LOV_AVN_ESTAGIO
(
       chave varchar2(10),
       texto varchar2(50)
);

alter table LOV_AVN_ESTAGIO add constraint PK_LOV_AVN_ESTAGIO primary key (chave);

comment on table LOV_AVN_ESTAGIO is 'Lista de valores para tabela AVN coluna estagio';
comment on column LOV_AVN_ESTAGIO.chave is 'Chave primária para lov_avn_estagio';
comment on column LOV_AVN_ESTAGIO.texto is 'Valor da coluna estagio da tabela AVN';

prompt
prompt -------------------------------------------------
prompt LOV_AA_TIPO
prompt -------------------------------------------------

create table LOV_AA_TIPO
(
       chave varchar2(10),
       texto varchar2(50)
);

alter table LOV_AA_TIPO add constraint PK_LOV_AA_TIPO primary key (chave);

comment on table LOV_AA_TIPO is 'Lista de valores para tabela AA coluna tipo';
comment on column LOV_AA_TIPO.chave is 'Chave primária para lov_aa_tipo';
comment on column LOV_AA_TIPO.texto is 'Valor da coluna tipo da tabela AA';

prompt
prompt -------------------------------------------------
prompt LOV_ACV_TIPO
prompt -------------------------------------------------

create table LOV_ACV_TIPO
(
       chave varchar2(50),
       texto varchar2(50)
);

alter table LOV_ACV_TIPO add constraint PK_LOV_ACV_TIPO primary key (chave);

comment on table LOV_ACV_TIPO is 'Lista de valores para tabela ACV coluna tipo';
comment on column LOV_ACV_TIPO.chave is 'Chave primária para lov_acv_tipo';
comment on column LOV_ACV_TIPO.texto is 'Valor da coluna tipo da tabela ACV';

prompt
prompt -------------------------------------------------
prompt LOV_ARL_COMPENSADA
prompt -------------------------------------------------

create table LOV_ARL_COMPENSADA
(
       chave varchar2(10),
       texto varchar2(50)
);

alter table LOV_ARL_COMPENSADA add constraint PK_LOV_ARL_COMPENSADA primary key (chave);

comment on table LOV_ARL_COMPENSADA is 'Lista de valores para tabela ARL coluna compensada';
comment on column LOV_ARL_COMPENSADA.chave is 'Chave primária para lov_arl_compensada';
comment on column LOV_ARL_COMPENSADA.texto is 'Valor da coluna compensada da tabela ARL';

prompt
prompt -------------------------------------------------
prompt LOV_SIM_NAO
prompt -------------------------------------------------

create table LOV_SIM_NAO
(
       chave varchar2(10),
       texto varchar2(50)
);

alter table LOV_SIM_NAO add constraint PK_LOV_SIM_NAO primary key (chave);

comment on table LOV_SIM_NAO is 'Lista de valores para as tabelas do desenhador';
comment on column LOV_SIM_NAO.chave is 'Chave primária para lov_sim_nao';
comment on column LOV_SIM_NAO.texto is 'Valor da coluna';

prompt 
prompt -------------------------------------------------
prompt TAB_CONFIGURACAO
prompt -------------------------------------------------

create table TAB_CONFIGURACAO
(
  CHAVE VARCHAR2(30) not null,
  VALOR VARCHAR2(100)
);
comment on table TAB_CONFIGURACAO  is 'Tabela de Configuração.';
comment on column TAB_CONFIGURACAO.CHAVE is 'Chave de identificação única.';
comment on column TAB_CONFIGURACAO.VALOR is 'Valor.';

prompt
prompt -------------------------------------------------
prompt TAB_CATEGORIA_FEICAO
prompt -------------------------------------------------

create table TAB_CATEGORIA_FEICAO
(
  ID    number(10),
  NOME    varchar2(80),
  ORDEM number(5)
);

alter table TAB_CATEGORIA_FEICAO add constraint PK_CATEGORIA_FEICAO primary key (ID);

comment on table TAB_CATEGORIA_FEICAO is 'Lista de categorias referentes às feições';
comment on column tab_categoria_feicao.id is 'Chave primária para tab_categoria_feicao';
comment on column tab_categoria_feicao.nome is 'Nome da categoria';
comment on column tab_categoria_feicao.ordem is 'Aplica ordem que deve aparecer a categoria';

prompt 
prompt -------------------------------------------------
prompt TAB_FEICAO
prompt -------------------------------------------------

create table TAB_FEICAO
(
  ID    number(10),
  NOME    varchar2(80),
  CATEGORIA  number(10),
  ESQUEMA    varchar2(30),
  TABELA    varchar2(30),
  TIPO    number(10),
  SEQUENCIA  varchar2(30),
  COLUNA_PK  varchar2(30)
);

alter table TAB_FEICAO add constraint PK_FEICAO primary key (ID);
alter table TAB_FEICAO add constraint FK_FEI_CATEGORIA foreign key (CATEGORIA) references TAB_CATEGORIA_FEICAO (ID);
alter table TAB_FEICAO add constraint FK_FEI_TIPO foreign key (TIPO) references LOV_TIPO_FEICAO (ID);

comment on table TAB_FEICAO is 'Dados cadastrados para relacionar as feições com metadados para o desenhador';
comment on column TAB_FEICAO.id is 'Chave primária para tab_feicao';
comment on column tab_feicao.nome is 'Nome da feição';
comment on column TAB_FEICAO.CATEGORIA is 'Chave estrangeira para tab_categoria_feicao';
comment on column TAB_FEICAO.ESQUEMA is 'Esquema da tabela';
comment on column TAB_FEICAO.tabela is 'Tabela da feição';
comment on column TAB_FEICAO.TIPO is 'Chave estrangeira para lov_tipo_feicao';
comment on column TAB_FEICAO.SEQUENCIA is 'Sequência utilizada para gerar identificadores';
comment on column TAB_FEICAO.COLUNA_PK is 'Coluna da feição que é chave primária';

prompt 
prompt -------------------------------------------------
prompt TAB_FEICAO_COLUNAS
prompt -------------------------------------------------

create table TAB_FEICAO_COLUNAS
(
  FEICAO      number(10),
  COLUNA      varchar2(30),
  TIPO        number(10),
  TAMANHO     number,
  ALIAS       varchar2(80),
  IS_OBRIGATORIO    number(1) default 0 not null ,
  TABELA_REFERENCIADA  varchar2(30),
  COLUNA_REFERENCIADA  varchar2(30),
  IS_VISIVEL           number(1) default 1 not null,
  IS_EDITAVEL          number(1) default 1 not null
);

alter table TAB_FEICAO_COLUNAS add constraint FK_FEI_COL_FEICAO foreign key (FEICAO) references TAB_FEICAO (ID);
alter table TAB_FEICAO_COLUNAS add constraint FK_FEI_COL_TIPO foreign key (TIPO) references LOV_TIPO_COLUNA (ID);

comment on table TAB_FEICAO_COLUNAS is 'Metadados das feições cadastradas';
comment on column TAB_FEICAO_COLUNAS.feicao is 'Chave estrangeira para tab_feicao';
comment on column TAB_FEICAO_COLUNAS.coluna is 'Coluna do metadados da feição cadastrada';
comment on column TAB_FEICAO_COLUNAS.tipo is 'Tipo do metadados da feição cadastrada';
comment on column TAB_FEICAO_COLUNAS.tamanho is 'Tamanho do metadados da feição cadastrada';
comment on column TAB_FEICAO_COLUNAS.alias is 'Alias do metadados da feição cadastrada';
comment on column TAB_FEICAO_COLUNAS.is_obrigatorio is 'Define se o metadados da feição cadastrada é obrigatório. 1- Sim/ 0- Não';
comment on column TAB_FEICAO_COLUNAS.tabela_referenciada is 'Tabela a qual a coluna referencia';
comment on column TAB_FEICAO_COLUNAS.coluna_referenciada is 'Coluna a qual a coluna referencia';
comment on column TAB_FEICAO_COLUNAS.is_visivel is 'Define se a coluna é visivel ou não. 1-Sim/ 0-Não';
comment on column TAB_FEICAO_COLUNAS.is_editavel is 'Define se a coluna é editavel ou não. 1-Sim/ 0-Não';

prompt 
prompt -------------------------------------------------
prompt TAB_SERVICO
prompt -------------------------------------------------

create table TAB_SERVICO
(
  ID          NUMBER(10) not null,
  NOME        VARCHAR2(200),
  URL         VARCHAR2(200),
  IS_CACHEADO NUMBER(1) default 0 not null,
  LOGIN       VARCHAR2(30),
  SENHA       RAW(16),
  MXD_ARQUIVO BLOB,
  SITUACAO    NUMBER(10),
  DATA_INICIO DATE,
  DATA_FINAL  DATE
);

alter table TAB_SERVICO  add constraint PK_SERVICO primary key (ID);

comment on table TAB_SERVICO is 'Cadastro dos serviços disponíveis para criação dos navegadores';
comment on column TAB_SERVICO.id   is 'Chave primária para tab_servico';
comment on column TAB_SERVICO.NOME   is 'Nome do serviço';
comment on column TAB_SERVICO.URL is 'Caminho onde se encontra o serviço do ArcGIS';
comment on column TAB_SERVICO.IS_CACHEADO is 'Define se o serviço é cacheado ou dinamico. 1- Sim/ 0- Não';
comment on column TAB_SERVICO.LOGIN is 'Usuário do serviço';
comment on column TAB_SERVICO.SENHA is 'Senha do serviço';
comment on column TAB_SERVICO.MXD_ARQUIVO is 'Arquivo mxd';
comment on column TAB_SERVICO.SITUACAO is 'Situação que indica se o arquivo mxd já foi interpretado pelo serviço ou não. Chave estrangeira para lov_situacao_servico';
comment on column TAB_SERVICO.DATA_INICIO is 'Data que iniciou a execução do serviço';
comment on column TAB_SERVICO.DATA_FINAL is 'Data que finalizou a execução do serviço';

prompt 
prompt -------------------------------------------------
prompt TAB_SERVICO_FEICAO
prompt -------------------------------------------------

create table TAB_SERVICO_FEICAO
(
  SERVICO    NUMBER(10),
  FEICAO     NUMBER(10),
  ID_LAYER   NUMBER(10),
  NOME_LAYER VARCHAR2(80),
  IS_VISIVEL NUMBER(1) default 1 not null,
  IS_EDITAVEL number(1) default 0 not null,
  FILTRO     VARCHAR2(200),
  ORDEM number(10)
);

alter table TAB_SERVICO_FEICAO add constraint FK_SERV_FEI_FEICAO foreign key (FEICAO) references TAB_FEICAO (ID);
alter table TAB_SERVICO_FEICAO add constraint FK_SERV_FEI_SERVICO foreign key (SERVICO) references TAB_SERVICO (ID);

comment on table TAB_SERVICO_FEICAO is 'Registra os dados do MXD para conferir com os dados do Serviço e relacionar o Serviço à feição';
comment on column TAB_SERVICO_FEICAO.SERVICO is 'Chave estrangeira para tab_servico';
comment on column TAB_SERVICO_FEICAO.FEICAO is 'Chave estrangeira para tab_feicao';
comment on column TAB_SERVICO_FEICAO.ID_LAYER is 'Id do Layer - Valor adquirido do MXD';
comment on column TAB_SERVICO_FEICAO.NOME_LAYER is 'Nome do Layer - Valor adquirido do MXD';
comment on column TAB_SERVICO_FEICAO.IS_VISIVEL is 'Define se a feição está visivel - Valor adquirido do MXD. 1- Sim/ 0- Não';
comment on column TAB_SERVICO_FEICAO.IS_EDITAVEL is 'Define se a feição é editavel ou não. 1- Sim/ 0- Não';
comment on column TAB_SERVICO_FEICAO.FILTRO is 'Filtro aplicado a tabela da feição. Utilizado principalmente para exibir quantidade de feições.';
comment on column TAB_SERVICO_FEICAO.ordem is 'Aplica ordem que deve aparecer as feições';

prompt 
prompt -------------------------------------------------
prompt TAB_NAVEGADOR
prompt -------------------------------------------------

create table TAB_NAVEGADOR
(
  ID                 NUMBER(10) not null,
  NOME               VARCHAR2(80),
  DATA_CADASTRO      DATE
);

alter table TAB_NAVEGADOR add constraint PK_NAVEGADOR primary key (ID);

comment on table TAB_NAVEGADOR is 'Tabela com o cadastro dos navegadores, objetivo e descrição';
comment on column TAB_NAVEGADOR.id is 'Chave primária de tab_navegador';
comment on column TAB_NAVEGADOR.NOME is 'Nome do navegador';
comment on column TAB_NAVEGADOR.DATA_CADASTRO is 'Data em que foi cadastrado o navegador';

prompt 
prompt -------------------------------------------------
prompt TAB_NAVEGADOR_SERVICO
prompt -------------------------------------------------

create table TAB_NAVEGADOR_SERVICO
(
  NAVEGADOR      NUMBER(10),
  SERVICO        NUMBER(10),
  IS_PRINCIPAL   NUMBER(1) default 0 not null,
  ORDEM_EXIBICAO NUMBER(10),
  IDENTIFICAR    NUMBER(1) default 0 not null
);

alter table TAB_NAVEGADOR_SERVICO add constraint FK_NAV_SERV_NAVEGADOR foreign key (NAVEGADOR) references TAB_NAVEGADOR (ID);
alter table TAB_NAVEGADOR_SERVICO add constraint FK_NAV_SERV_SERVICO foreign key (SERVICO) references TAB_SERVICO (ID);

comment on table TAB_NAVEGADOR_SERVICO is 'Serviços cadastrados para determinado navegador';
comment on column TAB_NAVEGADOR_SERVICO.NAVEGADOR is 'Chave estrangeira para tab_navegador';
comment on column TAB_NAVEGADOR_SERVICO.SERVICO is 'Chave estrangeira para tab_servico';
comment on column TAB_NAVEGADOR_SERVICO.IS_PRINCIPAL is 'Define se o serviço é principal. 1- Sim/ 0- Não';
comment on column TAB_NAVEGADOR_SERVICO.ORDEM_EXIBICAO is 'Define a ordem do serviço';
comment on column TAB_NAVEGADOR_SERVICO.IDENTIFICAR is 'Define se o serviço do navegador aparece na ferramenta de identificar ou não. 1-Sim/ 0-Não';

prompt 
prompt -------------------------------------------------
prompt TAB_NAVEGADOR_PROJETO
prompt -------------------------------------------------

create table TAB_NAVEGADOR_PROJETO
(
   PROJETO          NUMBER(10) not null,
   IS_VALIDO_PROCES NUMBER(1)
);

comment on table TAB_NAVEGADOR_PROJETO  is 'Tabela referente a situação do projeto no navegador';
comment on column TAB_NAVEGADOR_PROJETO.PROJETO  is 'Chave estrangeira para crt_projeto_geo';
comment on column TAB_NAVEGADOR_PROJETO.IS_VALIDO_PROCES is 'Define se os dados processados continuam válidos ou não. 1-Sim 0-Não';


prompt 
prompt -------------------------------------------------
prompt TAB_CENARIO_NAVEGADOR
prompt -------------------------------------------------

create table TAB_CENARIO_NAVEGADOR
(
  ID    number(10),
  NAVEGADOR  number(10),
  NOME    varchar2(80),
  ORDEM_EXIBICAO  number(10),
  IS_ATIVO  number(1) default 0 not null 
);

alter table TAB_CENARIO_NAVEGADOR add constraint PK_CENARIO_NAVEGADOR primary key (ID);
alter table TAB_CENARIO_NAVEGADOR add constraint FK_CEN_NAV_NAVEGADOR foreign key (NAVEGADOR) references TAB_NAVEGADOR (ID);

comment on table TAB_CENARIO_NAVEGADOR is 'Cenários cadastrados para o navegador ("Imagem", "Hibrido", "Vetor", ou outros) e a ordem de exibição';

comment on column tab_cenario_navegador.id is 'Chave primária para tab_cenario_navegador';
comment on column tab_cenario_navegador.navegador is 'Chave estrangeira para tab_navegador';
comment on column tab_cenario_navegador.nome is 'Nome do navegador';
comment on column tab_cenario_navegador.is_ativo is 'Define se o cenário está ativado. 1- Sim/ 0- Não';
comment on column tab_cenario_navegador.ordem_exibicao is 'Define a ordem do cenário';

prompt 
prompt -------------------------------------------------
prompt TAB_CENARIO_SERVICO
prompt -------------------------------------------------

create table TAB_CENARIO_SERVICO
(
  CENARIO_NAVEGADOR  number(10),
  SERVICO      number(10)
);

alter table TAB_CENARIO_SERVICO add constraint FK_CEN_SERV_CENARIO_NAVEGADOR foreign key (CENARIO_NAVEGADOR) references TAB_CENARIO_NAVEGADOR(ID);
alter table TAB_CENARIO_SERVICO add constraint FK_CEN_SERV_SERVICO foreign key (SERVICO) references TAB_SERVICO (ID);

comment on table TAB_CENARIO_SERVICO is 'Lista dos serviços relacionados a determinado Cenário(TrocaMapa)';

comment on column tab_cenario_servico.cenario_navegador is 'Chave estrangeira para tab_cenario_navegador';
comment on column tab_cenario_servico.servico is 'Chave estrangeira para tab_servico';

prompt 
prompt -------------------------------------------------
prompt TAB_LINHA
prompt -------------------------------------------------

create table TAB_LINHA
(
  ID NUMBER(10),
  PROJETO NUMBER(10),
  FEICAO   NUMBER(10),
  GEOMETRY SDO_GEOMETRY
);
comment on table TAB_LINHA is 'Tabela rascunho para validar geometrias do tipo LINHA .';
comment on column TAB_LINHA.ID is 'Identificador da geometria.';
comment on column TAB_LINHA.FEICAO is 'identificador da feicao';
comment on column TAB_LINHA.GEOMETRY is 'Geometria.';

prompt 
prompt -------------------------------------------------
prompt TAB_POLIGONO
prompt -------------------------------------------------

create table TAB_POLIGONO
(
  ID NUMBER,
  PROJETO NUMBER(10),
  FEICAO   NUMBER(10),
  GEOMETRY  SDO_GEOMETRY
);
comment on table TAB_POLIGONO is 'Tabela rascunho para validar geometrias do tipo POLÍGONO .';
comment on column TAB_POLIGONO.ID is 'Identificador da geometria.'; 
comment on column TAB_POLIGONO.FEICAO is 'identificador da feicao'; 
comment on column TAB_POLIGONO.GEOMETRY  is 'Geometria.';

prompt 
prompt -------------------------------------------------
prompt TAB_PONTO
prompt -------------------------------------------------

create table TAB_PONTO
(
  ID NUMBER(10),
  PROJETO NUMBER(10),
  FEICAO   NUMBER(10),
  GEOMETRY    SDO_GEOMETRY
);
comment on table TAB_PONTO is 'Tabela rascunho para validar geometrias do tipo PONTO .'; 
comment on column TAB_PONTO.ID is 'Identificador da geometria.'; 
comment on column TAB_PONTO.FEICAO is 'identificador da feicao';
comment on column TAB_PONTO.GEOMETRY is 'Geometria.';


prompt 
prompt -------------------------------------------------
prompt TABELAS DE DESENHO
prompt -------------------------------------------------

--Área de Abrangencia
create table DES_AREA_ABRANGENCIA
(
   id number(10)  constraint pk_des_area_abrang primary key,
   projeto      number(38),   
   geometry       mdsys.sdo_geometry
);

comment on table des_area_abrangencia is 'Tabela utilizada pelo desenhador para validar area de abrangencia.';
comment on column des_area_abrangencia.id is 'Chave primária de identificação da tabela. 1 - Area de abrangencia do estado.';
comment on column des_area_abrangencia.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial. Em caso de área de abrangencia do estado o projeto é nulo';
comment on column des_area_abrangencia.geometry is 'Campo geométrico.';

create sequence SEQ_DES_AREA_ABRANG start with 2;

--Área Total do Imóvel
create table DES_ATP
(
   id number(10)  constraint pk_des_atp primary key,
   projeto  		number(38),
   area_m2        number,
   geometry       mdsys.sdo_geometry
);

comment on table des_atp is 'Tabela que contem as geometrias de desenho de ATP.';
comment on column des_atp.id is 'Chave primária de identificação da tabela.';
comment on column des_atp.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column des_atp.area_m2 is 'Área em metros quadrados da geometria.';
comment on column des_atp.geometry is 'Campo geométrico.';

create sequence SEQ_DES_ATP;


--Matrícula ou Posse
create table DES_APMP(
   id number(10)  constraint pk_des_apmp primary key,
   projeto  		number(38),
   tipo				varchar2(100),
   nome           varchar2(100),
   cod_atp        number(10),
   area_m2        number,
   geometry       mdsys.sdo_geometry
);

comment on table des_apmp is 'Tabela que contem as geometrias de desenho de APMP.';
comment on column des_apmp.id is 'Chave primária de identificação da tabela.';
comment on column des_apmp.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column des_apmp.tipo is 'Identifica a tipo da APMP, podendo ser "M" para Matrícula, "P" para Posse ou "D" para Desconhecido.';
comment on column des_apmp.nome is 'Nome da APMP.';
comment on column des_apmp.cod_atp is 'Referencia o ID da ATP que interage com essa geometria.';
comment on column des_apmp.area_m2 is 'Área em metros quadrados da geometria.';
comment on column des_apmp.geometry is 'Campo geométrico.';

create sequence SEQ_DES_APMP;


--Área da faixa de dominio
create table DES_AFD
(
   id number(10)  constraint pk_des_afd primary key,
   projeto  		number(38),
   area_m2        number,
   geometry       mdsys.sdo_geometry
);

comment on table des_afd is 'Tabela que contem as geometrias de desenho de AFD.';
comment on column des_afd.id is 'Chave primária de identificação da tabela.';
comment on column des_afd.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column des_afd.area_m2 is 'Área em metros quadrados da geometria.';
comment on column des_afd.geometry is 'Campo geométrico.';

create sequence SEQ_DES_AFD;

	
-- Rocha
create table DES_ROCHA(
   id number(10)  constraint pk_des_rocha primary key,
   projeto  		number(38),
   cod_apmp  		number(10),
   area_m2        number,
   geometry       mdsys.sdo_geometry
);

comment on table des_rocha is 'Tabela que contem as geometrias de desenho de Rocha.';
comment on column des_rocha.id is 'Chave primária de identificação da tabela.';
comment on column des_rocha.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column des_rocha.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column des_rocha.area_m2 is 'Área em metros quadrados da geometria.';
comment on column des_rocha.geometry is 'Campo geométrico.';

create sequence SEQ_DES_ROCHA;


-- Vertices de APMP
create table DES_VERTICE(
   id number(10)  constraint pk_des_vertice primary key,
   projeto  		number(38),
   nome           varchar2(100),
   geometry       mdsys.sdo_geometry
);

comment on table des_vertice is 'Tabela que contem as geometrias de desenho de VERTICE.';
comment on column des_vertice.id is 'Chave primária de identificação da tabela.';
comment on column des_vertice.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column des_vertice.nome is 'Nome do VERTICE.';
comment on column des_vertice.geometry is 'Campo geométrico.';

create sequence SEQ_DES_VERTICE;


-- Area de Reserva Legal
create table DES_ARL(
   id number(10)  constraint pk_des_arl primary key,
   projeto  		number(38),
   cod_apmp  		number(10),
   compensada     varchar2(1),
   situacao       varchar2(50),
   area_m2        number,
   geometry       mdsys.sdo_geometry
);

comment on table des_arl is 'Tabela que contem as geometrias de desenho de ARL.';
comment on column des_arl.id is 'Chave primária de identificação da tabela.';
comment on column des_arl.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column des_arl.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column des_arl.compensada is 'Identifica se a ARL é compensada. Assume "S" para sim e "N" para não.';
comment on column des_arl.situacao is 'Identifica a situação da ARL, podendo ser "AA" para ARLs em Formação, "AVN" para Preservadas ou "Não Informado" para Indefinidas.';
comment on column des_arl.area_m2 is 'Área em metros quadrados da geometria.';
comment on column des_arl.geometry is 'Campo geométrico.';

create sequence SEQ_DES_ARL;


-- Reserva Particular de Patrimônio Natural
create table DES_RPPN(
   id number(10)  constraint pk_des_rppn primary key,
   projeto  		number(38),
   cod_apmp  		number(10),
   area_m2        number,
   geometry       mdsys.sdo_geometry
);

comment on table des_rppn is 'Tabela que contem as geometrias de desenho de RPPN.';
comment on column des_rppn.id is 'Chave primária de identificação da tabela.';
comment on column des_rppn.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column des_rppn.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column des_rppn.area_m2 is 'Área em metros quadrados da geometria.';
comment on column des_rppn.geometry is 'Campo geométrico.';

create sequence SEQ_DES_RPPN;


-- Área de Faixa de Servidão
create table DES_AFS
(
   id number(10)  constraint pk_des_afs primary key,
   projeto  		number(38),
   cod_apmp  		number(10),
   area_m2        number,
   geometry       mdsys.sdo_geometry
);

comment on table des_afs is 'Tabela que contem as geometrias de desenho de AFS.';
comment on column des_afs.id is 'Chave primária de identificação da tabela.';
comment on column des_afs.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column des_afs.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column des_afs.area_m2 is 'Área em metros quadrados da geometria.';
comment on column des_afs.geometry is 'Campo geométrico.';

create sequence SEQ_DES_AFS;


-- Área de Vegetação Nativa
create table DES_AVN(
   id number(10)  constraint pk_des_avn primary key,
   projeto  		number(38),
   cod_apmp  		number(10),
   estagio        varchar2(100),
   area_m2        number,
   geometry       mdsys.sdo_geometry
);

comment on table des_avn is 'Tabela que contem as geometrias de desenho de AVN.';
comment on column des_avn.id is 'Chave primária de identificação da tabela.';
comment on column des_avn.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column des_avn.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column des_avn.estagio is 'Estagio da area, podendo ser "I" para Inicial, "M" para Médio, "A" para Avançado ou "D" para Desconhecido.';
comment on column des_avn.area_m2 is 'Área em metros quadrados da geometria.';
comment on column des_avn.geometry is 'Campo geométrico.';

create sequence SEQ_DES_AVN;


-- Área Aberta
create table DES_AA(
   id number(10)  constraint pk_des_aa primary key,
   projeto  		number(38),
   cod_apmp  		number(10),
   tipo        	varchar2(100),
   area_m2        number,
   geometry       mdsys.sdo_geometry
);

create sequence SEQ_DES_AA;

comment on table des_aa is 'Tabela que contem as geometrias de desenho de AA.';
comment on column des_aa.id is 'Chave primária de identificação da tabela.';
comment on column des_aa.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column des_aa.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column des_aa.tipo is 'Tipo da area, podendo ser "C" para Cultivada, "NC" para Não Cultivada ou "D" para Desconhecido.';
comment on column des_aa.area_m2 is 'Área em metros quadrados da geometria.';
comment on column des_aa.geometry is 'Campo geométrico.';


-- Área de Classificação de Vegetação
create table DES_ACV(
   id number(10)  constraint pk_des_acv primary key,
   projeto  		number(38),
   cod_apmp  		number(10),
   tipo        	varchar2(100),
   area_m2        number,
   geometry       mdsys.sdo_geometry
);

comment on table des_acv is 'Tabela que contem as geometrias de desenho de ACV.';
comment on column des_acv.id is 'Chave primária de identificação da tabela.';
comment on column des_acv.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column des_acv.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column des_acv.tipo is 'Tipo da area, podendo ser "MANGUE", "BREJO", "RESTINGA", "RESTINGA-APP", "FLORESTA-NATIVA", "FLORESTA-PLANTADA", "MACEGA", "CABRUCA" ou "OUTROS".';
comment on column des_acv.area_m2 is 'Área em metros quadrados da geometria.';
comment on column des_acv.geometry is 'Campo geométrico.';

create sequence SEQ_DES_ACV;



----------------------------------------------------
-- Outras 

-- Área Construída
create table DES_ACONSTRUIDA(
   id number(10)  constraint pk_des_aconstruida primary key,
   projeto  		number(38),
   area_m2        number,
   geometry       mdsys.sdo_geometry
);

comment on table des_aconstruida is 'Tabela que contem as geometrias de desenho de AConstruida.';
comment on column des_aconstruida.id is 'Chave primária de identificação da tabela.';
comment on column des_aconstruida.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column des_aconstruida.area_m2 is 'Área em metros quadrados da geometria.';
comment on column des_aconstruida.geometry is 'Campo geométrico.';

create sequence SEQ_DES_ACONSTRUIDA;


-- Linhas de DUTO
create table DES_DUTO(
   id number(10)  constraint pk_des_duto primary key,
   projeto  		number(38),
   geometry       mdsys.sdo_geometry
);

comment on table des_duto is 'Tabela que contem as geometrias de desenho de Duto.';
comment on column des_duto.id is 'Chave primária de identificação da tabela.';
comment on column des_duto.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column des_duto.geometry is 'Campo geométrico.';

create sequence SEQ_DES_DUTO;


-- Linhas de TRANSMISSAO
create table DES_LTRANSMISSAO(
   id number(10)  constraint pk_des_ltransmissao primary key,
   projeto  		number(38),
   geometry       mdsys.sdo_geometry
);

comment on table des_ltransmissao is 'Tabela que contem as geometrias de desenho de LTransmissao.';
comment on column des_ltransmissao.id is 'Chave primária de identificação da tabela.';
comment on column des_ltransmissao.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column des_ltransmissao.geometry is 'Campo geométrico.';

create sequence SEQ_DES_LTRANSMISSAO;


-- Linhas de ESTRADA
create table DES_ESTRADA(
   id number(10)  constraint pk_des_estrada primary key,
   projeto  		number(38),
   geometry       mdsys.sdo_geometry
);

comment on table des_estrada is 'Tabela que contem as geometrias de desenho de Estrada.';
comment on column des_estrada.id is 'Chave primária de identificação da tabela.';
comment on column des_estrada.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column des_estrada.geometry is 'Campo geométrico.';

create sequence SEQ_DES_ESTRADA;


-- Linhas de FERROVIA
create table DES_FERROVIA(
   id number(10)  constraint pk_des_ferrovia primary key,
   projeto  		number(38),
   geometry       mdsys.sdo_geometry
);

comment on table des_ferrovia is 'Tabela que contem as geometrias de desenho de Ferrovia.';
comment on column des_ferrovia.id is 'Chave primária de identificação da tabela.';
comment on column des_ferrovia.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column des_ferrovia.geometry is 'Campo geométrico.';

create sequence SEQ_DES_FERROVIA;

-- Tabelas de rascunho para o desenhador
create table DES_RASCUNHO_PONTO(
   id number(10)  constraint pk_DES_RASC_PONTO primary key,
   projeto      number(38),
   geometry       mdsys.sdo_geometry
);

comment on table DES_RASCUNHO_PONTO is 'Tabela que contem as geometrias de desenho de Rascunho - Ponto.';
comment on column DES_RASCUNHO_PONTO.id is 'Chave primária de identificação da tabela.';
comment on column DES_RASCUNHO_PONTO.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column DES_RASCUNHO_PONTO.geometry is 'Campo geométrico.';

create table DES_RASCUNHO_LINHA(
   id number(10)  constraint pk_DES_RASCUNHO_LINHA primary key,
   projeto      number(38),
   geometry       mdsys.sdo_geometry
);

comment on table DES_RASCUNHO_LINHA is 'Tabela que contem as geometrias de desenho de Rascunho - Linha.';
comment on column DES_RASCUNHO_LINHA.id is 'Chave primária de identificação da tabela.';
comment on column DES_RASCUNHO_LINHA.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column DES_RASCUNHO_LINHA.geometry is 'Campo geométrico.';

create table DES_RASCUNHO_POLIGONO(
   id number(10)  constraint pk_DES_RASCUNHO_POLIGONO primary key,
   projeto      number(38),
   geometry       mdsys.sdo_geometry
);

comment on table DES_RASCUNHO_POLIGONO is 'Tabela que contem as geometrias de desenho de Rascunho - Poligono.';
comment on column DES_RASCUNHO_POLIGONO.id is 'Chave primária de identificação da tabela.';
comment on column DES_RASCUNHO_POLIGONO.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column DES_RASCUNHO_POLIGONO.geometry is 'Campo geométrico.';

----------------------------------------------------
-- Areas que dao origem a APP

-- Nascente
create table DES_NASCENTE(
   id number(10)  constraint pk_des_nascente primary key,
   projeto  		number(38),
   geometry       mdsys.sdo_geometry
);

comment on table des_nascente is 'Tabela que contem as geometrias de desenho de Nascente.';
comment on column des_nascente.id is 'Chave primária de identificação da tabela.';
comment on column des_nascente.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column des_nascente.geometry is 'Campo geométrico.';

create sequence SEQ_DES_NASCENTE;

	
-- Linhas de Rio
create table DES_RIO_LINHA(
   id number(10)  constraint pk_des_rio_linha primary key,
   projeto  		number(38),
   nome           varchar2(100),
   largura        number,
   geometry       mdsys.sdo_geometry
);

comment on table des_rio_linha is 'Tabela que contem as geometrias de desenho de RIO_LINHA.';
comment on column des_rio_linha.id is 'Chave primária de identificação da tabela.';
comment on column des_rio_linha.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column des_rio_linha.nome is 'Nome do Rio.';
comment on column des_rio_linha.largura is 'Largura do rio usada para calcular a APP.';
comment on column des_rio_linha.geometry is 'Campo geométrico.';

create sequence SEQ_DES_RIO_LINHA;


-- Areas de Rio
create table DES_RIO_AREA(
   id number(10)  constraint pk_des_rio_area primary key,
   projeto  		number(38),
   cod_apmp  		number(10),
   area_m2        number,
   nome           varchar2(100),
   largura        number,
   geometry       mdsys.sdo_geometry
);

comment on table des_rio_area is 'Tabela que contem as geometrias de desenho de RIO_AREA.';
comment on column des_rio_area.id is 'Chave primária de identificação da tabela.';
comment on column des_rio_area.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column des_rio_area.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column des_rio_area.area_m2 is 'Área em metros quadrados da geometria.';
comment on column des_rio_area.nome is 'Nome do Rio.';
comment on column des_rio_area.largura is 'Largura do rio usada para calcular a APP.';
comment on column des_rio_area.geometry is 'Campo geométrico.';

create sequence SEQ_DES_RIO_AREA;


-- Areas de Lagos e Lagoas
create table DES_LAGOA(
   id number(10)  constraint pk_des_lagoa primary key,
   projeto  		number(38),
   cod_apmp  		number(10),
   area_m2        number,
   zona				varchar2(100),
   nome           varchar2(100),
   geometry       mdsys.sdo_geometry
);

comment on table des_lagoa is 'Tabela que contem as geometrias de desenho de Lagoa.';
comment on column des_lagoa.id is 'Chave primária de identificação da tabela.';
comment on column des_lagoa.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column des_lagoa.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column des_lagoa.area_m2 is 'Área em metros quadrados da geometria.';
comment on column des_lagoa.zona is 'Zona da Lagoa, podendo ser "U" para Urbano, "R" para Rural ou "A" para Abastecimento.';
comment on column des_lagoa.nome is 'Nome da Lagoa.';
comment on column des_lagoa.geometry is 'Campo geométrico.';

create sequence SEQ_DES_LAGOA;


-- Areas Inundadas de Represas
create table DES_REPRESA(
   id number(10)  constraint pk_des_represa primary key,
   projeto  		number(38),
   cod_apmp  		number(10),
   area_m2        number,
   amortecimento  number,
   nome           varchar2(100),
   geometry       mdsys.sdo_geometry
);

comment on table des_represa is 'Tabela que contem as geometrias de desenho de Represa.';
comment on column des_represa.id is 'Chave primária de identificação da tabela.';
comment on column des_represa.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column des_represa.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column des_represa.area_m2 is 'Área em metros quadrados da geometria.';
comment on column des_represa.amortecimento is 'Raio de buffer para geração de APP.';
comment on column des_represa.nome is 'Nome da Represa.';
comment on column des_represa.geometry is 'Campo geométrico.';

create sequence SEQ_DES_REPRESA;


-- Areas de Duna
create table DES_DUNA(
   id number(10)  constraint pk_des_duna primary key,
   projeto  		number(38),
   geometry       mdsys.sdo_geometry
);

comment on table des_duna is 'Tabela que contem as geometrias de desenho de Duna.';
comment on column des_duna.id is 'Chave primária de identificação da tabela.';
comment on column des_duna.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column des_duna.geometry is 'Campo geométrico.';

create sequence SEQ_DES_DUNA;


-- Restrição de declividade
create table DES_REST_DECLIVIDADE(
   id number(10)  constraint pk_des_rest_declividade primary key,
   projeto  		number(38),
   geometry       mdsys.sdo_geometry
);

comment on table des_rest_declividade is 'Tabela que contem as geometrias de desenho de Restrição de Declividade.';
comment on column des_rest_declividade.id is 'Chave primária de identificação da tabela.';
comment on column des_rest_declividade.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column des_rest_declividade.geometry is 'Campo geométrico.';

create sequence SEQ_DES_REST_DECLIVIDADE;


-- Escarpa
create table DES_ESCARPA(
   id number(10)  constraint pk_des_escarpa primary key,
   projeto  		number(38),
   geometry       mdsys.sdo_geometry
);

comment on table des_escarpa is 'Tabela que contem as geometrias de desenho de Escarpa.';
comment on column des_escarpa.id is 'Chave primária de identificação da tabela.';
comment on column des_escarpa.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column des_escarpa.geometry is 'Campo geométrico.';

create sequence SEQ_DES_ESCARPA;



----------------------------------------------------
-- Feições de Atividade

-- Ponto da Atividade
create table DES_PATIV(
   id number(10)  constraint pk_des_pativ primary key,
   projeto  		number(38),
   cod_apmp  		number(10),
   atividade      varchar2(150),
   rocha				varchar2(1),
   massa_dagua		varchar2(1),
   avn				varchar2(1),
   aa					varchar2(1),
   afs				varchar2(1),
   floresta_plantada	varchar2(1),
   arl				varchar2(1),
   rppn				varchar2(1),
   app				varchar2(1),
   geometry       mdsys.sdo_geometry
);

comment on table des_pativ is 'Tabela que contem as geometrias de desenho de PATIV ( Ponto da Atividade ).';
comment on column des_pativ.id is 'Chave primária de identificação da tabela.';
comment on column des_pativ.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column des_pativ.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column des_pativ.atividade is 'Texto da atividade associada ao projeto geo.';
comment on column des_pativ.rocha is 'Identifica se a geometria cruza com ROCHA. Assume "S" para sim e "N" para não.';
comment on column des_pativ.massa_dagua is 'Identifica se a geometria cruza com MASSA_DAGUA. Assume "S" para sim e "N" para não.';
comment on column des_pativ.avn is 'Identifica se a geometria cruza com AVN. Case não encontre relação assume "N" para não, caso contrário, assume a sigla do estágio da AVN que cruzou podendo ser "I", "M", "A" ou "D".';
comment on column des_pativ.aa is 'Identifica se a geometria cruza com AA. Assume "S" para sim e "N" para não.';
comment on column des_pativ.afs is 'Identifica se a geometria cruza com AFS. Assume "S" para sim e "N" para não.';
comment on column des_pativ.floresta_plantada is 'Identifica se a geometria cruza com ACV de tipo "FLORESTA_PLANTADA". Assume "S" para sim e "N" para não.';
comment on column des_pativ.arl is 'Identifica se a geometria cruza com ARL. Assume "S" para sim e "N" para não.';
comment on column des_pativ.rppn is 'Identifica se a geometria cruza com RPPN. Assume "S" para sim e "N" para não.';
comment on column des_pativ.app is 'Identifica se a geometria cruza com APP. Assume "S" para sim e "N" para não.';
comment on column des_pativ.geometry is 'Campo geométrico.';

create sequence SEQ_DES_PATIV;


-- Linha da Atividade
create table DES_LATIV(
   id number(10)  constraint pk_des_lativ primary key,
   projeto  		number(38),
   cod_apmp  		number(10),
   atividade      varchar2(150),
   rocha				varchar2(1),
   massa_dagua		varchar2(1),
   avn				varchar2(1),
   aa					varchar2(1),
   afs				varchar2(1),
   floresta_plantada	varchar2(1),
   arl				varchar2(1),
   rppn				varchar2(1),
   app				varchar2(1),
   comprimento		number,
   geometry       mdsys.sdo_geometry
);

comment on table des_lativ is 'Tabela que contem as geometrias de desenho de LATIV ( Linha da Atividade ).';
comment on column des_lativ.id is 'Chave primária de identificação da tabela.';
comment on column des_lativ.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column des_lativ.cod_apmp is 'Referencia o ID da APMP que interage com o ponto médio dessa geometria.';
comment on column des_lativ.atividade is 'Texto da atividade associada ao projeto geo.';
comment on column des_lativ.rocha is 'Identifica se a geometria cruza atravez do seu ponto médio com ROCHA. Assume "S" para sim e "N" para não.';
comment on column des_lativ.massa_dagua is 'Identifica se a geometria cruza atravez do seu ponto médio com MASSA_DAGUA. Assume "S" para sim e "N" para não.';
comment on column des_lativ.avn is 'Identifica se a geometria cruza atravez do seu ponto médio com AVN. Case não encontre relação assume "N" para não, caso contrário, assume a sigla do estágio da AVN que cruzou podendo ser "I", "M", "A" ou "D".';
comment on column des_lativ.aa is 'Identifica se a geometria cruza atravez do seu ponto médio com AA. Assume "S" para sim e "N" para não.';
comment on column des_lativ.afs is 'Identifica se a geometria cruza atravez do seu ponto médio com AFS. Assume "S" para sim e "N" para não.';
comment on column des_lativ.floresta_plantada is 'Identifica se a geometria cruza atravez do seu ponto médio com ACV de tipo "FLORESTA_PLANTADA". Assume "S" para sim e "N" para não.';
comment on column des_lativ.arl is 'Identifica se a geometria cruza atravez do seu ponto médio com ARL. Assume "S" para sim e "N" para não.';
comment on column des_lativ.rppn is 'Identifica se a geometria cruza atravez do seu ponto médio com RPPN. Assume "S" para sim e "N" para não.';
comment on column des_lativ.app is 'Identifica se a geometria cruza atravez do seu ponto médio com APP. Assume "S" para sim e "N" para não.';
comment on column des_lativ.comprimento is 'Comprimento em metros da geometria.';
comment on column des_lativ.geometry is 'Campo geométrico.';

create sequence SEQ_DES_LATIV;


-- Area da Atividade
create table DES_AATIV(
   id number(10)  constraint pk_des_aativ primary key,
   projeto  		number(38),
   cod_apmp  		number(10),
   atividade      varchar2(150),
   rocha				varchar2(1),
   massa_dagua		varchar2(1),
   avn				varchar2(1),
   aa					varchar2(1),
   afs				varchar2(1),
   floresta_plantada	varchar2(1),
   arl				varchar2(1),
   rppn				varchar2(1),
   app				varchar2(1),
   area_m2			number,
   geometry       mdsys.sdo_geometry
);

comment on table des_aativ is 'Tabela que contem as geometrias de desenho de AATIV ( Área da Atividade ).';
comment on column des_aativ.id is 'Chave primária de identificação da tabela.';
comment on column des_aativ.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column des_aativ.cod_apmp is 'Referencia o ID da APMP que interage com o ponto ideal dessa geometria.';
comment on column des_aativ.atividade is 'Texto da atividade associada ao projeto geo.';
comment on column des_aativ.rocha is 'Identifica se a geometria cruza atravez do seu ponto ideal com ROCHA. Assume "S" para sim e "N" para não.';
comment on column des_aativ.massa_dagua is 'Identifica se a geometria cruza atravez do seu ponto ideal com MASSA_DAGUA. Assume "S" para sim e "N" para não.';
comment on column des_aativ.avn is 'Identifica se a geometria cruza atravez do seu ponto ideal com AVN. Case não encontre relação assume "N" para não, caso contrário, assume a sigla do estágio da AVN que cruzou podendo ser "I", "M", "A" ou "D".';
comment on column des_aativ.aa is 'Identifica se a geometria cruza atravez do seu ponto ideal com AA. Assume "S" para sim e "N" para não.';
comment on column des_aativ.afs is 'Identifica se a geometria cruza atravez do seu ponto ideal com AFS. Assume "S" para sim e "N" para não.';
comment on column des_aativ.floresta_plantada is 'Identifica se a geometria cruza atravez do seu ponto ideal com ACV de tipo "FLORESTA_PLANTADA". Assume "S" para sim e "N" para não.';
comment on column des_aativ.arl is 'Identifica se a geometria cruza atravez do seu ponto ideal com ARL. Assume "S" para sim e "N" para não.';
comment on column des_aativ.rppn is 'Identifica se a geometria cruza atravez do seu ponto ideal com RPPN. Assume "S" para sim e "N" para não.';
comment on column des_aativ.app is 'Identifica se a geometria cruza atravez do seu ponto ideal com APP. Assume "S" para sim e "N" para não.';
comment on column des_aativ.area_m2 is 'Área em metros quadrados da geometria.';
comment on column des_aativ.geometry is 'Campo geométrico.';

create sequence SEQ_DES_AATIV;


-- Area de Influencia da Atividade
create table DES_AIATIV(
   id number(10)  constraint pk_des_aiativ primary key,
   projeto  		number(38),
   cod_apmp  		number(10),
   atividade      varchar2(150),
   rocha				varchar2(1),
   massa_dagua		varchar2(1),
   avn				varchar2(1),
   aa					varchar2(1),
   afs				varchar2(1),
   floresta_plantada	varchar2(1),
   arl				varchar2(1),
   rppn				varchar2(1),
   app				varchar2(1),
   area_m2			number,
   geometry       mdsys.sdo_geometry
);

comment on table des_aiativ is 'Tabela que contem as geometrias de desenho de AIATIV ( Área de Influência da Atividade ).';
comment on column des_aiativ.id is 'Chave primária de identificação da tabela.';
comment on column des_aiativ.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column des_aiativ.cod_apmp is 'Referencia o ID da APMP que interage com o ponto ideal dessa geometria.';
comment on column des_aiativ.atividade is 'Texto da atividade associada ao projeto geo.';
comment on column des_aiativ.rocha is 'Identifica se a geometria cruza atravez do seu ponto ideal com ROCHA. Assume "S" para sim e "N" para não.';
comment on column des_aiativ.massa_dagua is 'Identifica se a geometria cruza atravez do seu ponto ideal com MASSA_DAGUA. Assume "S" para sim e "N" para não.';
comment on column des_aiativ.avn is 'Identifica se a geometria cruza atravez do seu ponto ideal com AVN. Case não encontre relação assume "N" para não, caso contrário, assume a sigla do estágio da AVN que cruzou podendo ser "I", "M", "A" ou "D".';
comment on column des_aiativ.aa is 'Identifica se a geometria cruza atravez do seu ponto ideal com AA. Assume "S" para sim e "N" para não.';
comment on column des_aiativ.afs is 'Identifica se a geometria cruza atravez do seu ponto ideal com AFS. Assume "S" para sim e "N" para não.';
comment on column des_aiativ.floresta_plantada is 'Identifica se a geometria cruza atravez do seu ponto ideal com ACV de tipo "FLORESTA_PLANTADA". Assume "S" para sim e "N" para não.';
comment on column des_aiativ.arl is 'Identifica se a geometria cruza atravez do seu ponto ideal com ARL. Assume "S" para sim e "N" para não.';
comment on column des_aiativ.rppn is 'Identifica se a geometria cruza atravez do seu ponto ideal com RPPN. Assume "S" para sim e "N" para não.';
comment on column des_aiativ.app is 'Identifica se a geometria cruza atravez do seu ponto ideal com APP. Assume "S" para sim e "N" para não.';
comment on column des_aiativ.area_m2 is 'Área em metros quadrados da geometria.';
comment on column des_aiativ.geometry is 'Campo geométrico.';

create sequence SEQ_DES_AIATIV;


----------------------------------------------------
-- Processamento
----------------------------------------------------

-- Create table
create table TMP_RASC_TRACKMAKER
(
   projeto  		number(38),
   nome     		varchar2(400),
   geometry 		mdsys.sdo_geometry
);


--Área Total do Imóvel
create table TMP_ATP
(
   id number(10)  constraint pk_tmp_atp primary key,
   projeto  		number(38),
   area_m2        number,
   geometry       mdsys.sdo_geometry
);

comment on table tmp_atp is 'Tabela que contem as geometrias temporárias de ATP.';
comment on column tmp_atp.id is 'Chave primária de identificação da tabela.';
comment on column tmp_atp.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column tmp_atp.area_m2 is 'Área em metros quadrados da geometria.';
comment on column tmp_atp.geometry is 'Campo geométrico.';

create sequence SEQ_TMP_ATP;


--Matrícula ou Posse
create table TMP_APMP(
   id number(10)  constraint pk_tmp_apmp primary key,
   projeto  		number(38),
   tipo				varchar2(100),
   nome           varchar2(100),
   cod_atp        number(10),
   area_m2        number,
   geometry       mdsys.sdo_geometry
);

comment on table tmp_apmp is 'Tabela que contem as geometrias temporárias de APMP.';
comment on column tmp_apmp.id is 'Chave primária de identificação da tabela.';
comment on column tmp_apmp.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column tmp_apmp.tipo is 'Identifica a tipo da APMP, podendo ser "M" para Matrícula, "P" para Posse ou "D" para Desconhecido.';
comment on column tmp_apmp.nome is 'Nome da APMP.';
comment on column tmp_apmp.cod_atp is 'Referencia o ID da ATP que interage com essa geometria.';
comment on column tmp_apmp.area_m2 is 'Área em metros quadrados da geometria.';
comment on column tmp_apmp.geometry is 'Campo geométrico.';

create sequence SEQ_TMP_APMP;


--Área da faixa de dominio
create table TMP_AFD
(
   id number(10)  constraint pk_tmp_afd primary key,
   projeto  		number(38),
   area_m2        number,
   geometry       mdsys.sdo_geometry
);

comment on table tmp_afd is 'Tabela que contem as geometrias temporárias de AFD.';
comment on column tmp_afd.id is 'Chave primária de identificação da tabela.';
comment on column tmp_afd.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column tmp_afd.area_m2 is 'Área em metros quadrados da geometria.';
comment on column tmp_afd.geometry is 'Campo geométrico.';

create sequence SEQ_TMP_AFD;

	
-- Rocha
create table TMP_ROCHA(
   id number(10)  constraint pk_tmp_rocha primary key,
   projeto  		number(38),
   cod_apmp  		number(10),
   area_m2        number,
   geometry       mdsys.sdo_geometry
);

comment on table tmp_rocha is 'Tabela que contem as geometrias temporárias de Rocha.';
comment on column tmp_rocha.id is 'Chave primária de identificação da tabela.';
comment on column tmp_rocha.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column tmp_rocha.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column tmp_rocha.area_m2 is 'Área em metros quadrados da geometria.';
comment on column tmp_rocha.geometry is 'Campo geométrico.';


create sequence SEQ_TMP_ROCHA;


-- Vertices de APMP
create table TMP_VERTICE
(
   id number(10)  constraint pk_tmp_vertice primary key,
   projeto  		number(38),
   nome           varchar2(100),
   geometry       mdsys.sdo_geometry
);

comment on table tmp_vertice is 'Tabela que contem as geometrias temporárias de VERTICE.';
comment on column tmp_vertice.id is 'Chave primária de identificação da tabela.';
comment on column tmp_vertice.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column tmp_vertice.nome is 'Nome do VERTICE.';
comment on column tmp_vertice.geometry is 'Campo geométrico.';

create sequence SEQ_TMP_VERTICE;


-- Area de Reserva Legal
create table TMP_ARL(
   id number(10)  constraint pk_tmp_arl primary key,
   projeto  		number(38),
   cod_apmp  		number(10),
   codigo			varchar2(100),
   compensada     varchar2(1),
   situacao       varchar2(50),
   area_m2        number,
   geometry       mdsys.sdo_geometry
);

comment on table tmp_arl is 'Tabela que contem as geometrias temporárias de ARL.';
comment on column tmp_arl.id is 'Chave primária de identificação da tabela.';
comment on column tmp_arl.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column tmp_arl.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column tmp_arl.codigo is 'Codigo gerado automaticamente para identificação da ARL.';
comment on column tmp_arl.compensada is 'Identifica se a ARL é compensada. Assume "S" para sim e "N" para não.';
comment on column tmp_arl.situacao is 'Identifica a situação da ARL, podendo ser "AA" para ARLs em Formação, "AVN" para Preservadas ou "Não Informado" para Indefinidas.';
comment on column tmp_arl.area_m2 is 'Área em metros quadrados da geometria.';
comment on column tmp_arl.geometry is 'Campo geométrico.';

create sequence SEQ_TMP_ARL;


-- Reserva Particular de Patrimônio Natural
create table TMP_RPPN(
   id number(10)  constraint pk_tmp_rppn primary key,
   projeto  		number(38),
   cod_apmp  		number(10),
   area_m2        number,
   geometry       mdsys.sdo_geometry
);

comment on table tmp_rppn is 'Tabela que contem as geometrias temporárias de RPPN.';
comment on column tmp_rppn.id is 'Chave primária de identificação da tabela.';
comment on column tmp_rppn.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column tmp_rppn.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column tmp_rppn.area_m2 is 'Área em metros quadrados da geometria.';
comment on column tmp_rppn.geometry is 'Campo geométrico.';

create sequence SEQ_TMP_RPPN;


-- Área de Faixa de Servidão
create table TMP_AFS(
   id number(10)  constraint pk_tmp_afs primary key,
   projeto  		number(38),
   cod_apmp  		number(10),
   area_m2        number,
   geometry       mdsys.sdo_geometry
);

comment on table tmp_afs is 'Tabela que contem as geometrias temporárias de AFS.';
comment on column tmp_afs.id is 'Chave primária de identificação da tabela.';
comment on column tmp_afs.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column tmp_afs.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column tmp_afs.area_m2 is 'Área em metros quadrados da geometria.';
comment on column tmp_afs.geometry is 'Campo geométrico.';

create sequence SEQ_TMP_AFS;


-- Área de Vegetação Nativa
create table TMP_AVN(
   id number(10)  constraint pk_tmp_avn primary key,
   projeto  		number(38),
   cod_apmp  		number(10),
   estagio        varchar2(100),
   area_m2        number,
   geometry       mdsys.sdo_geometry
);

comment on table tmp_avn is 'Tabela que contem as geometrias temporárias de AVN.';
comment on column tmp_avn.id is 'Chave primária de identificação da tabela.';
comment on column tmp_avn.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column tmp_avn.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column tmp_avn.estagio is 'Estagio da area, podendo ser "I" para Inicial, "M" para Médio, "A" para Avançado ou "D" para Desconhecido.';
comment on column tmp_avn.area_m2 is 'Área em metros quadrados da geometria.';
comment on column tmp_avn.geometry is 'Campo geométrico.';

create sequence SEQ_TMP_AVN;


-- Área Aberta
create table TMP_AA(
   id number(10)  constraint pk_tmp_aa primary key,
   projeto  		number(38),
   cod_apmp  		number(10),
   tipo        	varchar2(100),
   area_m2        number,
   geometry       mdsys.sdo_geometry
);

create sequence SEQ_TMP_AA;

comment on table tmp_aa is 'Tabela que contem as geometrias temporárias de AA.';
comment on column tmp_aa.id is 'Chave primária de identificação da tabela.';
comment on column tmp_aa.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column tmp_aa.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column tmp_aa.tipo is 'Tipo da area, podendo ser "C" para Cultivada, "NC" para Não Cultivada ou "D" para Desconhecido.';
comment on column tmp_aa.area_m2 is 'Área em metros quadrados da geometria.';
comment on column tmp_aa.geometry is 'Campo geométrico.';


-- Área de Classificação de Vegetação
create table TMP_ACV(
   id number(10)  constraint pk_tmp_acv primary key,
   projeto  		number(38),
   cod_apmp  		number(10),
   tipo        	varchar2(100),
   area_m2        number,
   geometry       mdsys.sdo_geometry
);

comment on table tmp_acv is 'Tabela que contem as geometrias temporárias de ACV.';
comment on column tmp_acv.id is 'Chave primária de identificação da tabela.';
comment on column tmp_acv.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column tmp_acv.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column tmp_acv.tipo is 'Tipo da area, podendo ser "MANGUE", "BREJO", "RESTINGA", "RESTINGA-APP", "FLORESTA-NATIVA", "FLORESTA-PLANTADA", "MACEGA", "CABRUCA" ou "OUTROS".';
comment on column tmp_acv.area_m2 is 'Área em metros quadrados da geometria.';
comment on column tmp_acv.geometry is 'Campo geométrico.';

create sequence SEQ_TMP_ACV;



----------------------------------------------------
-- Outras 

-- Área Construída
create table TMP_ACONSTRUIDA(
   id number(10)  constraint pk_tmp_aconstruida primary key,
   projeto  		number(38),
   area_m2        number,
   geometry       mdsys.sdo_geometry
);

comment on table tmp_aconstruida is 'Tabela que contem as geometrias temporárias de AConstruida.';
comment on column tmp_aconstruida.id is 'Chave primária de identificação da tabela.';
comment on column tmp_aconstruida.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column tmp_aconstruida.area_m2 is 'Área em metros quadrados da geometria.';
comment on column tmp_aconstruida.geometry is 'Campo geométrico.';

create sequence SEQ_TMP_ACONSTRUIDA;


-- Linhas de DUTO
create table TMP_DUTO(
   id number(10)  constraint pk_tmp_duto primary key,
   projeto  		number(38),
   geometry       mdsys.sdo_geometry
);

comment on table tmp_duto is 'Tabela que contem as geometrias temporárias de Duto.';
comment on column tmp_duto.id is 'Chave primária de identificação da tabela.';
comment on column tmp_duto.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column tmp_duto.geometry is 'Campo geométrico.';

create sequence SEQ_TMP_DUTO;


-- Linhas de TRANSMISSAO
create table TMP_LTRANSMISSAO(
   id number(10)  constraint pk_tmp_ltransmissao primary key,
   projeto  		number(38),
   geometry       mdsys.sdo_geometry
);

comment on table tmp_ltransmissao is 'Tabela que contem as geometrias temporárias de LTransmissao.';
comment on column tmp_ltransmissao.id is 'Chave primária de identificação da tabela.';
comment on column tmp_ltransmissao.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column tmp_ltransmissao.geometry is 'Campo geométrico.';

create sequence SEQ_TMP_LTRANSMISSAO;


-- Linhas de ESTRADA
create table TMP_ESTRADA(
   id number(10)  constraint pk_tmp_estrada primary key,
   projeto  		number(38),
   geometry       mdsys.sdo_geometry
);

comment on table tmp_estrada is 'Tabela que contem as geometrias temporárias de Estrada.';
comment on column tmp_estrada.id is 'Chave primária de identificação da tabela.';
comment on column tmp_estrada.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column tmp_estrada.geometry is 'Campo geométrico.';

create sequence SEQ_TMP_ESTRADA;


-- Linhas de FERROVIA
create table TMP_FERROVIA(
   id number(10)  constraint pk_tmp_ferrovia primary key,
   projeto  		number(38),
   geometry       mdsys.sdo_geometry
);

comment on table tmp_ferrovia is 'Tabela que contem as geometrias temporárias de Ferrovia.';
comment on column tmp_ferrovia.id is 'Chave primária de identificação da tabela.';
comment on column tmp_ferrovia.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column tmp_ferrovia.geometry is 'Campo geométrico.';

create sequence SEQ_TMP_FERROVIA;



----------------------------------------------------
-- Areas Calculadas pelo Sistema


-- Áreas Calculadas
create table TMP_AREAS_CALCULADAS(
   id number(10)  constraint pk_tmp_areas_calculadas primary key,
   projeto  		number(38),
   cod_apmp  		number(10),
   tipo        	varchar2(100),
   area_m2        number,
   geometry       mdsys.sdo_geometry
);

comment on table tmp_areas_calculadas is 'Tabela que contem as geometrias temporárias de Areas Calculadas.';
comment on column tmp_areas_calculadas.id is 'Chave primária de identificação da tabela.';
comment on column tmp_areas_calculadas.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column tmp_areas_calculadas.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column tmp_areas_calculadas.tipo is 'Tipo da area calculada, podendo ser "APMP_APMP", "APMP_AFD", "ARL_ARL", "ARL_ROCHA", "AA_AVN", "APP_APMP", "APP_AA", "APP_AVN", "APP_ARL", "MASSA_DAGUA_APMP" ou "AATIV_AATIV".';
comment on column tmp_areas_calculadas.area_m2 is 'Área em metros quadrados da geometria.';
comment on column tmp_areas_calculadas.geometry is 'Campo geométrico.';

create sequence SEQ_TMP_AREAS_CALCULADAS;


----------------------------------------------------
-- Areas que dao origem a APP

-- Nascente
create table TMP_NASCENTE(
   id number(10)  constraint pk_tmp_nascente primary key,
   projeto  		number(38),
   geometry       mdsys.sdo_geometry
);

comment on table tmp_nascente is 'Tabela que contem as geometrias temporárias de Nascente.';
comment on column tmp_nascente.id is 'Chave primária de identificação da tabela.';
comment on column tmp_nascente.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column tmp_nascente.geometry is 'Campo geométrico.';

create sequence SEQ_TMP_NASCENTE;


-- Linhas de Rio
create table TMP_RIO_LINHA(
   id number(10)  constraint pk_tmp_rio_linha primary key,
   projeto  		number(38),
   nome           varchar2(100),
   largura        number,
   geometry       mdsys.sdo_geometry
);

comment on table tmp_rio_linha is 'Tabela que contem as geometrias temporárias de RIO_LINHA.';
comment on column tmp_rio_linha.id is 'Chave primária de identificação da tabela.';
comment on column tmp_rio_linha.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column tmp_rio_linha.nome is 'Nome do Rio.';
comment on column tmp_rio_linha.largura is 'Largura do rio usada para calcular a APP.';
comment on column tmp_rio_linha.geometry is 'Campo geométrico.';

create sequence SEQ_TMP_RIO_LINHA;


-- Areas de Rio
create table TMP_RIO_AREA(
   id number(10)  constraint pk_tmp_rio_area primary key,
   projeto  		number(38),
   cod_apmp  		number(10),
   area_m2        number,
   nome           varchar2(100),
   largura        number,
   geometry       mdsys.sdo_geometry
);

comment on table tmp_rio_area is 'Tabela que contem as geometrias temporárias de RIO_AREA.';
comment on column tmp_rio_area.id is 'Chave primária de identificação da tabela.';
comment on column tmp_rio_area.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column tmp_rio_area.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column tmp_rio_area.area_m2 is 'Área em metros quadrados da geometria.';
comment on column tmp_rio_area.nome is 'Nome do Rio.';
comment on column tmp_rio_area.largura is 'Largura do rio usada para calcular a APP.';
comment on column tmp_rio_area.geometry is 'Campo geométrico.';

create sequence SEQ_TMP_RIO_AREA;


-- Areas de Lagos e Lagoas
create table TMP_LAGOA(
   id number(10)  constraint pk_tmp_lagoa primary key,
   projeto  		number(38),
   cod_apmp  		number(10),
   area_m2        number,
   zona				varchar2(100),
   nome           varchar2(100),
   geometry       mdsys.sdo_geometry
);

comment on table tmp_lagoa is 'Tabela que contem as geometrias temporárias de Lagoa.';
comment on column tmp_lagoa.id is 'Chave primária de identificação da tabela.';
comment on column tmp_lagoa.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column tmp_lagoa.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column tmp_lagoa.area_m2 is 'Área em metros quadrados da geometria.';
comment on column tmp_lagoa.zona is 'Zona da Lagoa, podendo ser "U" para Urbano, "R" para Rural ou "A" para Abastecimento.';
comment on column tmp_lagoa.nome is 'Nome da Lagoa.';
comment on column tmp_lagoa.geometry is 'Campo geométrico.';

create sequence SEQ_TMP_LAGOA;


-- Areas Inundadas de Represas
create table TMP_REPRESA(
   id number(10)  constraint pk_tmp_represa primary key,
   projeto  		number(38),
   cod_apmp  		number(10),
   area_m2        number,
   amortecimento  number,
   nome           varchar2(100),
   geometry       mdsys.sdo_geometry
);

comment on table tmp_represa is 'Tabela que contem as geometrias temporárias de Represa.';
comment on column tmp_represa.id is 'Chave primária de identificação da tabela.';
comment on column tmp_represa.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column tmp_represa.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column tmp_represa.area_m2 is 'Área em metros quadrados da geometria.';
comment on column tmp_represa.amortecimento is 'Raio de buffer para geração de APP.';
comment on column tmp_represa.nome is 'Nome da Represa.';
comment on column tmp_represa.geometry is 'Campo geométrico.';

create sequence SEQ_TMP_REPRESA;


-- Areas de Duna
create table TMP_DUNA(
   id number(10)  constraint pk_tmp_duna primary key,
   projeto  		number(38),
   geometry       mdsys.sdo_geometry
);

comment on table tmp_duna is 'Tabela que contem as geometrias temporárias de Duna.';
comment on column tmp_duna.id is 'Chave primária de identificação da tabela.';
comment on column tmp_duna.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column tmp_duna.geometry is 'Campo geométrico.';

create sequence SEQ_TMP_DUNA;


-- Restrição de declividade
create table TMP_REST_DECLIVIDADE(
   id number(10)  constraint pk_tmp_rest_declividade primary key,
   projeto  		number(38),
   geometry       mdsys.sdo_geometry
);

comment on table tmp_rest_declividade is 'Tabela que contem as geometrias temporárias de Restrição de Declividade.';
comment on column tmp_rest_declividade.id is 'Chave primária de identificação da tabela.';
comment on column tmp_rest_declividade.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column tmp_rest_declividade.geometry is 'Campo geométrico.';

create sequence SEQ_TMP_REST_DECLIVIDADE;


-- Escarpa
create table TMP_ESCARPA(
   id number(10)  constraint pk_tmp_escarpa primary key,
   projeto  		number(38),
   geometry       mdsys.sdo_geometry
);

create sequence SEQ_TMP_ESCARPA;

comment on table tmp_escarpa is 'Tabela que contem as geometrias temporárias de Escarpa.';
comment on column tmp_escarpa.id is 'Chave primária de identificação da tabela.';
comment on column tmp_escarpa.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column tmp_escarpa.geometry is 'Campo geométrico.';



----------------------------------------------------
-- Feições de Atividade

-- Ponto da Atividade
create table TMP_PATIV(
   id number(10)  constraint pk_tmp_pativ primary key,
   projeto  		number(38),
   cod_apmp  		number(10),
   codigo			varchar2(100),
   atividade      varchar2(150),
   rocha				varchar2(1),
   massa_dagua		varchar2(1),
   avn				varchar2(1),
   aa					varchar2(1),
   afs				varchar2(1),
   floresta_plantada	varchar2(1),
   arl				varchar2(1),
   rppn				varchar2(1),
   app				varchar2(1),
   geometry       mdsys.sdo_geometry
);

comment on table tmp_pativ is 'Tabela que contem as geometrias temporárias de PATIV ( Ponto da Atividade ).';
comment on column tmp_pativ.id is 'Chave primária de identificação da tabela.';
comment on column tmp_pativ.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column tmp_pativ.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column tmp_pativ.codigo is 'Codigo gerado automaticamente para identificação da PATIV.';
comment on column tmp_pativ.atividade is 'Texto da atividade associada ao projeto geo.';
comment on column tmp_pativ.rocha is 'Identifica se a geometria cruza com ROCHA. Assume "S" para sim e "N" para não.';
comment on column tmp_pativ.massa_dagua is 'Identifica se a geometria cruza com MASSA_DAGUA. Assume "S" para sim e "N" para não.';
comment on column tmp_pativ.avn is 'Identifica se a geometria cruza com AVN. Case não encontre relação assume "N" para não, caso contrário, assume a sigla do estágio da AVN que cruzou podendo ser "I", "M", "A" ou "D".';
comment on column tmp_pativ.aa is 'Identifica se a geometria cruza com AA. Assume "S" para sim e "N" para não.';
comment on column tmp_pativ.afs is 'Identifica se a geometria cruza com AFS. Assume "S" para sim e "N" para não.';
comment on column tmp_pativ.floresta_plantada is 'Identifica se a geometria cruza com ACV de tipo "FLORESTA_PLANTADA". Assume "S" para sim e "N" para não.';
comment on column tmp_pativ.arl is 'Identifica se a geometria cruza com ARL. Assume "S" para sim e "N" para não.';
comment on column tmp_pativ.rppn is 'Identifica se a geometria cruza com RPPN. Assume "S" para sim e "N" para não.';
comment on column tmp_pativ.app is 'Identifica se a geometria cruza com APP. Assume "S" para sim e "N" para não.';
comment on column tmp_pativ.geometry is 'Campo geométrico.';

create sequence SEQ_TMP_PATIV;


-- Linha da Atividade
create table TMP_LATIV(
   id number(10)  constraint pk_tmp_lativ primary key,
   projeto  		number(38),
   cod_apmp  		number(10),
   codigo			varchar2(100),
   atividade      varchar2(150),
   rocha				varchar2(1),
   massa_dagua		varchar2(1),
   avn				varchar2(1),
   aa					varchar2(1),
   afs				varchar2(1),
   floresta_plantada	varchar2(1),
   arl				varchar2(1),
   rppn				varchar2(1),
   app				varchar2(1),
   comprimento		number,
   geometry       mdsys.sdo_geometry
);

comment on table tmp_lativ is 'Tabela que contem as geometrias temporárias de LATIV ( Linha da Atividade ).';
comment on column tmp_lativ.id is 'Chave primária de identificação da tabela.';
comment on column tmp_lativ.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column tmp_lativ.cod_apmp is 'Referencia o ID da APMP que interage com o ponto médio dessa geometria.';
comment on column tmp_lativ.codigo is 'Codigo gerado automaticamente para identificação da LATIV.';
comment on column tmp_lativ.atividade is 'Texto da atividade associada ao projeto geo.';
comment on column tmp_lativ.rocha is 'Identifica se a geometria cruza atravez do seu ponto médio com ROCHA. Assume "S" para sim e "N" para não.';
comment on column tmp_lativ.massa_dagua is 'Identifica se a geometria cruza atravez do seu ponto médio com MASSA_DAGUA. Assume "S" para sim e "N" para não.';
comment on column tmp_lativ.avn is 'Identifica se a geometria cruza atravez do seu ponto médio com AVN. Case não encontre relação assume "N" para não, caso contrário, assume a sigla do estágio da AVN que cruzou podendo ser "I", "M", "A" ou "D".';
comment on column tmp_lativ.aa is 'Identifica se a geometria cruza atravez do seu ponto médio com AA. Assume "S" para sim e "N" para não.';
comment on column tmp_lativ.afs is 'Identifica se a geometria cruza atravez do seu ponto médio com AFS. Assume "S" para sim e "N" para não.';
comment on column tmp_lativ.floresta_plantada is 'Identifica se a geometria cruza atravez do seu ponto médio com ACV de tipo "FLORESTA_PLANTADA". Assume "S" para sim e "N" para não.';
comment on column tmp_lativ.arl is 'Identifica se a geometria cruza atravez do seu ponto médio com ARL. Assume "S" para sim e "N" para não.';
comment on column tmp_lativ.rppn is 'Identifica se a geometria cruza atravez do seu ponto médio com RPPN. Assume "S" para sim e "N" para não.';
comment on column tmp_lativ.app is 'Identifica se a geometria cruza atravez do seu ponto médio com APP. Assume "S" para sim e "N" para não.';
comment on column tmp_lativ.comprimento is 'Comprimento em metros da geometria.';
comment on column tmp_lativ.geometry is 'Campo geométrico.';

create sequence SEQ_TMP_LATIV;


-- Area da Atividade
create table TMP_AATIV(
   id number(10)  constraint pk_tmp_aativ primary key,
   projeto  		number(38),
   cod_apmp  		number(10),
   codigo			varchar2(100),
   atividade      varchar2(150),
   rocha				varchar2(1),
   massa_dagua		varchar2(1),
   avn				varchar2(1),
   aa					varchar2(1),
   afs				varchar2(1),
   floresta_plantada	varchar2(1),
   arl				varchar2(1),
   rppn				varchar2(1),
   app				varchar2(1),
   area_m2			number,
   geometry       mdsys.sdo_geometry
);

comment on table tmp_aativ is 'Tabela que contem as geometrias temporárias de AATIV ( Área da Atividade ).';
comment on column tmp_aativ.id is 'Chave primária de identificação da tabela.';
comment on column tmp_aativ.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column tmp_aativ.cod_apmp is 'Referencia o ID da APMP que interage com o ponto ideal dessa geometria.';
comment on column tmp_aativ.codigo is 'Codigo gerado automaticamente para identificação da AATIV.';
comment on column tmp_aativ.atividade is 'Texto da atividade associada ao projeto geo.';
comment on column tmp_aativ.rocha is 'Identifica se a geometria cruza atravez do seu ponto ideal com ROCHA. Assume "S" para sim e "N" para não.';
comment on column tmp_aativ.massa_dagua is 'Identifica se a geometria cruza atravez do seu ponto ideal com MASSA_DAGUA. Assume "S" para sim e "N" para não.';
comment on column tmp_aativ.avn is 'Identifica se a geometria cruza atravez do seu ponto ideal com AVN. Case não encontre relação assume "N" para não, caso contrário, assume a sigla do estágio da AVN que cruzou podendo ser "I", "M", "A" ou "D".';
comment on column tmp_aativ.aa is 'Identifica se a geometria cruza atravez do seu ponto ideal com AA. Assume "S" para sim e "N" para não.';
comment on column tmp_aativ.afs is 'Identifica se a geometria cruza atravez do seu ponto ideal com AFS. Assume "S" para sim e "N" para não.';
comment on column tmp_aativ.floresta_plantada is 'Identifica se a geometria cruza atravez do seu ponto ideal com ACV de tipo "FLORESTA_PLANTADA". Assume "S" para sim e "N" para não.';
comment on column tmp_aativ.arl is 'Identifica se a geometria cruza atravez do seu ponto ideal com ARL. Assume "S" para sim e "N" para não.';
comment on column tmp_aativ.rppn is 'Identifica se a geometria cruza atravez do seu ponto ideal com RPPN. Assume "S" para sim e "N" para não.';
comment on column tmp_aativ.app is 'Identifica se a geometria cruza atravez do seu ponto ideal com APP. Assume "S" para sim e "N" para não.';
comment on column tmp_aativ.area_m2 is 'Área em metros quadrados da geometria.';
comment on column tmp_aativ.geometry is 'Campo geométrico.';

create sequence SEQ_TMP_AATIV;


-- Area de Influencia da Atividade
create table TMP_AIATIV(
   id number(10)  constraint pk_tmp_aiativ primary key,
   projeto  		number(38),
   cod_apmp  		number(10),
   codigo			varchar2(100),
   atividade      varchar2(150),
   rocha				varchar2(1),
   massa_dagua		varchar2(1),
   avn				varchar2(1),
   aa					varchar2(1),
   afs				varchar2(1),
   floresta_plantada	varchar2(1),
   arl				varchar2(1),
   rppn				varchar2(1),
   app				varchar2(1),
   area_m2			number,
   geometry       mdsys.sdo_geometry
);

comment on table tmp_aiativ is 'Tabela que contem as geometrias temporárias de AIATIV ( Área de Influência da Atividade ).';
comment on column tmp_aiativ.id is 'Chave primária de identificação da tabela.';
comment on column tmp_aiativ.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column tmp_aiativ.cod_apmp is 'Referencia o ID da APMP que interage com o ponto ideal dessa geometria.';
comment on column tmp_aiativ.codigo is 'Codigo gerado automaticamente para identificação da AIATIV.';
comment on column tmp_aiativ.atividade is 'Texto da atividade associada ao projeto geo.';
comment on column tmp_aiativ.rocha is 'Identifica se a geometria cruza atravez do seu ponto ideal com ROCHA. Assume "S" para sim e "N" para não.';
comment on column tmp_aiativ.massa_dagua is 'Identifica se a geometria cruza atravez do seu ponto ideal com MASSA_DAGUA. Assume "S" para sim e "N" para não.';
comment on column tmp_aiativ.avn is 'Identifica se a geometria cruza atravez do seu ponto ideal com AVN. Case não encontre relação assume "N" para não, caso contrário, assume a sigla do estágio da AVN que cruzou podendo ser "I", "M", "A" ou "D".';
comment on column tmp_aiativ.aa is 'Identifica se a geometria cruza atravez do seu ponto ideal com AA. Assume "S" para sim e "N" para não.';
comment on column tmp_aiativ.afs is 'Identifica se a geometria cruza atravez do seu ponto ideal com AFS. Assume "S" para sim e "N" para não.';
comment on column tmp_aiativ.floresta_plantada is 'Identifica se a geometria cruza atravez do seu ponto ideal com ACV de tipo "FLORESTA_PLANTADA". Assume "S" para sim e "N" para não.';
comment on column tmp_aiativ.arl is 'Identifica se a geometria cruza atravez do seu ponto ideal com ARL. Assume "S" para sim e "N" para não.';
comment on column tmp_aiativ.rppn is 'Identifica se a geometria cruza atravez do seu ponto ideal com RPPN. Assume "S" para sim e "N" para não.';
comment on column tmp_aiativ.app is 'Identifica se a geometria cruza atravez do seu ponto ideal com APP. Assume "S" para sim e "N" para não.';
comment on column tmp_aiativ.area_m2 is 'Área em metros quadrados da geometria.';
comment on column tmp_aiativ.geometry is 'Campo geométrico.';

create sequence SEQ_TMP_AIATIV;



----------------------------------------------------
-- Tabelas de base do Sistema
----------------------------------------------------

--Área Total do Imóvel
create table GEO_ATP(
   id number(10)  constraint pk_geo_atp primary key,
   projeto  		number(38) not null,
   area_m2        number,
   data				date,
   tid				varchar2(36) not null,
   geometry       mdsys.sdo_geometry
);

comment on table geo_atp is 'Tabela que contem as geometrias mais recentes de ATP.';
comment on column geo_atp.id is 'Chave primária de identificação da tabela.';
comment on column geo_atp.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column geo_atp.area_m2 is 'Área em metros quadrados da geometria.';
comment on column geo_atp.data is 'Data de inserção desse registro na tabela.';
comment on column geo_atp.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column geo_atp.geometry is 'Campo geométrico.';


--Matrícula ou Posse
create table GEO_APMP(
   id number(10)  constraint pk_geo_apmp primary key,
   projeto  		number(38) not null,
   tipo				varchar2(100),
   nome           varchar2(100),
   cod_atp        number(10),
   area_m2        number,
   data				date,
   tid				varchar2(36) not null,
   geometry       mdsys.sdo_geometry
);

comment on table geo_apmp is 'Tabela que contem as geometrias mais recentes de APMP.';
comment on column geo_apmp.id is 'Chave primária de identificação da tabela.';
comment on column geo_apmp.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column geo_apmp.tipo is 'Identifica a tipo da APMP, podendo ser "M" para Matrícula, "P" para Posse ou "D" para Desconhecido.';
comment on column geo_apmp.nome is 'Nome da APMP.';
comment on column geo_apmp.cod_atp is 'Referencia o ID da ATP que interage com essa geometria.';
comment on column geo_apmp.area_m2 is 'Área em metros quadrados da geometria.';
comment on column geo_apmp.data is 'Data de inserção desse registro na tabela.';
comment on column geo_apmp.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column geo_apmp.geometry is 'Campo geométrico.';


--Área da faixa de dominio
create table GEO_AFD(
   id number(10)  constraint pk_geo_afd primary key,
   projeto  		number(38) not null,
   area_m2        number,
   data				date,
   tid				varchar2(36) not null,
   geometry       mdsys.sdo_geometry
);

comment on table geo_afd is 'Tabela que contem as geometrias mais recentes de AFD.';
comment on column geo_afd.id is 'Chave primária de identificação da tabela.';
comment on column geo_afd.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column geo_afd.area_m2 is 'Área em metros quadrados da geometria.';
comment on column geo_afd.data is 'Data de inserção desse registro na tabela.';
comment on column geo_afd.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column geo_afd.geometry is 'Campo geométrico.';

	
-- Rocha
create table GEO_ROCHA(
   id number(10)  constraint pk_geo_rocha primary key,
   projeto  		number(38) not null,
   cod_apmp  		number(10),
   area_m2        number,
   data				date,
   tid				varchar2(36) not null,
   geometry       mdsys.sdo_geometry
);

comment on table geo_rocha is 'Tabela que contem as geometrias mais recentes de Rocha.';
comment on column geo_rocha.id is 'Chave primária de identificação da tabela.';
comment on column geo_rocha.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column geo_rocha.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column geo_rocha.area_m2 is 'Área em metros quadrados da geometria.';
comment on column geo_rocha.data is 'Data de inserção desse registro na tabela.';
comment on column geo_rocha.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column geo_rocha.geometry is 'Campo geométrico.';


-- Vertices de APMP
create table GEO_VERTICE(
   id number(10)  constraint pk_geo_vertice primary key,
   projeto  		number(38) not null,
   nome           varchar2(100),
   data				date,
   tid				varchar2(36) not null,
   geometry       mdsys.sdo_geometry
);

comment on table geo_vertice is 'Tabela que contem as geometrias mais recentes de VERTICE.';
comment on column geo_vertice.id is 'Chave primária de identificação da tabela.';
comment on column geo_vertice.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column geo_vertice.nome is 'Nome do VERTICE.';
comment on column geo_vertice.data is 'Data de inserção desse registro na tabela.';
comment on column geo_vertice.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column geo_vertice.geometry is 'Campo geométrico.';


-- Area de Reserva Legal
create table GEO_ARL(
   id number(10)  constraint pk_geo_arl primary key,
   projeto  		number(38) not null,
   cod_apmp  		number(10),
   codigo			varchar2(100),
   compensada     varchar2(1),
   situacao       varchar2(50),
   area_m2        number,
   data				date,
   tid				varchar2(36) not null,
   geometry       mdsys.sdo_geometry
);

comment on table geo_arl is 'Tabela que contem as geometrias mais recentes de ARL.';
comment on column geo_arl.id is 'Chave primária de identificação da tabela.';
comment on column geo_arl.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column geo_arl.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column geo_arl.codigo is 'Codigo gerado automaticamente para identificação da ARL.';
comment on column geo_arl.compensada is 'Identifica se a ARL é compensada. Assume "S" para sim e "N" para não.';
comment on column geo_arl.situacao is 'Identifica a situação da ARL, podendo ser "AA" para ARLs em Formação, "AVN" para Preservadas ou "Não Informado" para Indefinidas.';
comment on column geo_arl.area_m2 is 'Área em metros quadrados da geometria.';
comment on column geo_arl.data is 'Data de inserção desse registro na tabela.';
comment on column geo_arl.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column geo_arl.geometry is 'Campo geométrico.';


-- Reserva Particular de Patrimônio Natural
create table GEO_RPPN(
   id number(10)  constraint pk_geo_rppn primary key,
   projeto  		number(38) not null,
   cod_apmp  		number(10),
   area_m2        number,
   data				date,
   tid				varchar2(36) not null,
   geometry       mdsys.sdo_geometry
);

comment on table geo_rppn is 'Tabela que contem as geometrias mais recentes de RPPN.';
comment on column geo_rppn.id is 'Chave primária de identificação da tabela.';
comment on column geo_rppn.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column geo_rppn.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column geo_rppn.area_m2 is 'Área em metros quadrados da geometria.';
comment on column geo_rppn.data is 'Data de inserção desse registro na tabela.';
comment on column geo_rppn.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column geo_rppn.geometry is 'Campo geométrico.';


-- Área de Faixa de Servidão
create table GEO_AFS(
   id number(10)  constraint pk_geo_afs primary key,
   projeto  		number(38) not null,
   cod_apmp  		number(10),
   area_m2        number,
   data				date,
   tid				varchar2(36) not null,
   geometry       mdsys.sdo_geometry
);

comment on table geo_afs is 'Tabela que contem as geometrias mais recentes de AFS.';
comment on column geo_afs.id is 'Chave primária de identificação da tabela.';
comment on column geo_afs.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column geo_afs.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column geo_afs.area_m2 is 'Área em metros quadrados da geometria.';
comment on column geo_afs.data is 'Data de inserção desse registro na tabela.';
comment on column geo_afs.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column geo_afs.geometry is 'Campo geométrico.';


-- Área de Vegetação Nativa
create table GEO_AVN(
   id number(10)  constraint pk_geo_avn primary key,
   projeto  		number(38) not null,
   cod_apmp  		number(10),
   estagio        varchar2(100),
   area_m2        number,
   data				date,
   tid				varchar2(36) not null,
   geometry       mdsys.sdo_geometry
);

comment on table geo_avn is 'Tabela que contem as geometrias mais recentes de AVN.';
comment on column geo_avn.id is 'Chave primária de identificação da tabela.';
comment on column geo_avn.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column geo_avn.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column geo_avn.estagio is 'Estagio da area, podendo ser "I" para Inicial, "M" para Médio, "A" para Avançado ou "D" para Desconhecido.';
comment on column geo_avn.area_m2 is 'Área em metros quadrados da geometria.';
comment on column geo_avn.data is 'Data de inserção desse registro na tabela.';
comment on column geo_avn.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column geo_avn.geometry is 'Campo geométrico.';


-- Área Aberta
create table GEO_AA(
   id number(10)  constraint pk_geo_aa primary key,
   projeto  		number(38) not null,
   cod_apmp  		number(10),
   tipo        	varchar2(100),
   area_m2        number,
   data				date,
   tid				varchar2(36) not null,
   geometry       mdsys.sdo_geometry
);

comment on table geo_aa is 'Tabela que contem as geometrias mais recentes de AA.';
comment on column geo_aa.id is 'Chave primária de identificação da tabela.';
comment on column geo_aa.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column geo_aa.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column geo_aa.tipo is 'Tipo da area, podendo ser "C" para Cultivada, "NC" para Não Cultivada ou "D" para Desconhecido.';
comment on column geo_aa.area_m2 is 'Área em metros quadrados da geometria.';
comment on column geo_aa.data is 'Data de inserção desse registro na tabela.';
comment on column geo_aa.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column geo_aa.geometry is 'Campo geométrico.';


-- Área de Classificação de Vegetação
create table GEO_ACV(
   id number(10)  constraint pk_geo_acv primary key,
   projeto  		number(38) not null,
   cod_apmp  		number(10),
   tipo        	varchar2(100),
   area_m2        number,
   data				date,
   tid				varchar2(36) not null,
   geometry       mdsys.sdo_geometry
);

comment on table geo_acv is 'Tabela que contem as geometrias mais recentes de ACV.';
comment on column geo_acv.id is 'Chave primária de identificação da tabela.';
comment on column geo_acv.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column geo_acv.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column geo_acv.tipo is 'Tipo da area, podendo ser "MANGUE", "BREJO", "RESTINGA", "RESTINGA-APP", "FLORESTA-NATIVA", "FLORESTA-PLANTADA", "MACEGA", "CABRUCA" ou "OUTROS".';
comment on column geo_acv.area_m2 is 'Área em metros quadrados da geometria.';
comment on column geo_acv.data is 'Data de inserção desse registro na tabela.';
comment on column geo_acv.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column geo_acv.geometry is 'Campo geométrico.';



----------------------------------------------------
-- Outras 

-- Área Construída
create table GEO_ACONSTRUIDA(
   id number(10)  constraint pk_geo_aconstruida primary key,
   projeto  		number(38) not null,
   area_m2        number,
   data				date,
   tid				varchar2(36) not null,
   geometry       mdsys.sdo_geometry
);

comment on table geo_aconstruida is 'Tabela que contem as geometrias mais recentes de AConstruida.';
comment on column geo_aconstruida.id is 'Chave primária de identificação da tabela.';
comment on column geo_aconstruida.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column geo_aconstruida.area_m2 is 'Área em metros quadrados da geometria.';
comment on column geo_aconstruida.data is 'Data de inserção desse registro na tabela.';
comment on column geo_aconstruida.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column geo_aconstruida.geometry is 'Campo geométrico.';

-- Linhas de DUTO
create table GEO_DUTO(
   id number(10)  constraint pk_geo_duto primary key,
   projeto  		number(38) not null,
   data				date,
   tid				varchar2(36) not null,
   geometry       mdsys.sdo_geometry
);

comment on table geo_duto is 'Tabela que contem as geometrias mais recentes de Duto.';
comment on column geo_duto.id is 'Chave primária de identificação da tabela.';
comment on column geo_duto.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column geo_duto.data is 'Data de inserção desse registro na tabela.';
comment on column geo_duto.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column geo_duto.geometry is 'Campo geométrico.';


-- Linhas de TRANSMISSAO
create table GEO_LTRANSMISSAO(
   id number(10)  constraint pk_geo_ltransmissao primary key,
   projeto  		number(38) not null,
   data				date,
   tid				varchar2(36) not null,
   geometry       mdsys.sdo_geometry
);

comment on table geo_ltransmissao is 'Tabela que contem as geometrias mais recentes de LTransmissao.';
comment on column geo_ltransmissao.id is 'Chave primária de identificação da tabela.';
comment on column geo_ltransmissao.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column geo_ltransmissao.data is 'Data de inserção desse registro na tabela.';
comment on column geo_ltransmissao.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column geo_ltransmissao.geometry is 'Campo geométrico.';


-- Linhas de ESTRADA
create table GEO_ESTRADA(
   id number(10)  constraint pk_geo_estrada primary key,
   projeto  		number(38) not null,
   data				date,
   tid				varchar2(36) not null,
   geometry       mdsys.sdo_geometry
);

comment on table geo_estrada is 'Tabela que contem as geometrias mais recentes de Estrada.';
comment on column geo_estrada.id is 'Chave primária de identificação da tabela.';
comment on column geo_estrada.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column geo_estrada.data is 'Data de inserção desse registro na tabela.';
comment on column geo_estrada.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column geo_estrada.geometry is 'Campo geométrico.';


-- Linhas de FERROVIA
create table GEO_FERROVIA(
   id number(10)  constraint pk_geo_ferrovia primary key,
   projeto  		number(38) not null,
   data				date,
   tid				varchar2(36) not null,
   geometry       mdsys.sdo_geometry
);

comment on table geo_ferrovia is 'Tabela que contem as geometrias mais recentes de Ferrovia.';
comment on column geo_ferrovia.id is 'Chave primária de identificação da tabela.';
comment on column geo_ferrovia.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column geo_ferrovia.data is 'Data de inserção desse registro na tabela.';
comment on column geo_ferrovia.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column geo_ferrovia.geometry is 'Campo geométrico.';


----------------------------------------------------
-- Areas Calculadas pelo Sistema


-- Áreas Calculadas
create table GEO_AREAS_CALCULADAS(
   id number(10)  constraint pk_geo_areas_calculadas primary key,
   projeto  		number(38) not null,
   cod_apmp  		number(10),
   tipo        	varchar2(100),
   area_m2        number,
   data				date,
   tid				varchar2(36) not null,
   geometry       mdsys.sdo_geometry
);

comment on table geo_areas_calculadas is 'Tabela que contem as geometrias mais recentes de Areas Calculadas.';
comment on column geo_areas_calculadas.id is 'Chave primária de identificação da tabela.';
comment on column geo_areas_calculadas.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column geo_areas_calculadas.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column geo_areas_calculadas.tipo is 'Tipo da area calculada, podendo ser "APMP_APMP", "APMP_AFD", "ARL_ARL", "ARL_ROCHA", "AA_AVN", "APP_APMP", "APP_AA", "APP_AVN", "APP_ARL", "MASSA_DAGUA_APMP" ou "AATIV_AATIV".';
comment on column geo_areas_calculadas.area_m2 is 'Área em metros quadrados da geometria.';
comment on column geo_areas_calculadas.data is 'Data de inserção desse registro na tabela.';
comment on column geo_areas_calculadas.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column geo_areas_calculadas.geometry is 'Campo geométrico.';


----------------------------------------------------
-- Areas que dao origem a APP

-- Nascente
create table GEO_NASCENTE(
   id number(10)  constraint pk_geo_nascente primary key,
   projeto  		number(38) not null,
   data				date,
   tid				varchar2(36) not null,
   geometry       mdsys.sdo_geometry
);

comment on table geo_nascente is 'Tabela que contem as geometrias mais recentes de Nascente.';
comment on column geo_nascente.id is 'Chave primária de identificação da tabela.';
comment on column geo_nascente.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column geo_nascente.data is 'Data de inserção desse registro na tabela.';
comment on column geo_nascente.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column geo_nascente.geometry is 'Campo geométrico.';

	
-- Linhas de Rio
create table GEO_RIO_LINHA(
   id number(10)  constraint pk_geo_rio_linha primary key,
   projeto  		number(38) not null,
   nome           varchar2(100),
   largura        number,
   data				date,
   tid				varchar2(36) not null,
   geometry       mdsys.sdo_geometry
);

comment on table geo_rio_linha is 'Tabela que contem as geometrias mais recentes de RIO_LINHA.';
comment on column geo_rio_linha.id is 'Chave primária de identificação da tabela.';
comment on column geo_rio_linha.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column geo_rio_linha.nome is 'Nome do Rio.';
comment on column geo_rio_linha.largura is 'Largura do rio usada para calcular a APP.';
comment on column geo_rio_linha.data is 'Data de inserção desse registro na tabela.';
comment on column geo_rio_linha.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column geo_rio_linha.geometry is 'Campo geométrico.';


-- Areas de Rio
create table GEO_RIO_AREA(
   id number(10)  constraint pk_geo_rio_area primary key,
   projeto  		number(38) not null,
   cod_apmp  		number(10),
   area_m2        number,
   nome           varchar2(100),
   largura        number,
   data				date,
   tid				varchar2(36) not null,
   geometry       mdsys.sdo_geometry
);

comment on table geo_rio_area is 'Tabela que contem as geometrias mais recentes de RIO_AREA.';
comment on column geo_rio_area.id is 'Chave primária de identificação da tabela.';
comment on column geo_rio_area.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column geo_rio_area.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column geo_rio_area.area_m2 is 'Área em metros quadrados da geometria.';
comment on column geo_rio_area.nome is 'Nome do Rio.';
comment on column geo_rio_area.largura is 'Largura do rio usada para calcular a APP.';
comment on column geo_rio_area.data is 'Data de inserção desse registro na tabela.';
comment on column geo_rio_area.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column geo_rio_area.geometry is 'Campo geométrico.';


-- Areas de Lagos e Lagoas
create table GEO_LAGOA(
   id number(10)  constraint pk_geo_lagoa primary key,
   projeto  		number(38) not null,
   cod_apmp  		number(10),
   area_m2        number,
   zona				varchar2(100),
   nome           varchar2(100),
   data				date,
   tid				varchar2(36) not null,
   geometry       mdsys.sdo_geometry
);

comment on table geo_lagoa is 'Tabela que contem as geometrias mais recentes de Lagoa.';
comment on column geo_lagoa.id is 'Chave primária de identificação da tabela.';
comment on column geo_lagoa.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column geo_lagoa.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column geo_lagoa.area_m2 is 'Área em metros quadrados da geometria.';
comment on column geo_lagoa.zona is 'Zona da Lagoa, podendo ser "U" para Urbano, "R" para Rural ou "A" para Abastecimento.';
comment on column geo_lagoa.nome is 'Nome da Lagoa.';
comment on column geo_lagoa.data is 'Data de inserção desse registro na tabela.';
comment on column geo_lagoa.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column geo_lagoa.geometry is 'Campo geométrico.';


-- Areas Inundadas de Represas
create table GEO_REPRESA(
   id number(10)  constraint pk_geo_represa primary key,
   projeto  		number(38) not null,
   cod_apmp  		number(10),
   area_m2        number,
   amortecimento  number,
   nome           varchar2(100),
   data				date,
   tid				varchar2(36) not null,
   geometry       mdsys.sdo_geometry
);

comment on table geo_represa is 'Tabela que contem as geometrias mais recentes de Represa.';
comment on column geo_represa.id is 'Chave primária de identificação da tabela.';
comment on column geo_represa.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column geo_represa.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column geo_represa.area_m2 is 'Área em metros quadrados da geometria.';
comment on column geo_represa.amortecimento is 'Raio de buffer para geração de APP.';
comment on column geo_represa.nome is 'Nome da Represa.';
comment on column geo_represa.data is 'Data de inserção desse registro na tabela.';
comment on column geo_represa.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column geo_represa.geometry is 'Campo geométrico.';



-- Areas de Duna
create table GEO_DUNA(
   id number(10)  constraint pk_geo_duna primary key,
   projeto  		number(38) not null,
   data				date,
   tid				varchar2(36) not null,
   geometry       mdsys.sdo_geometry
);

comment on table geo_duna is 'Tabela que contem as geometrias mais recentes de Duna.';
comment on column geo_duna.id is 'Chave primária de identificação da tabela.';
comment on column geo_duna.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column geo_duna.data is 'Data de inserção desse registro na tabela.';
comment on column geo_duna.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column geo_duna.geometry is 'Campo geométrico.';


-- Restrição de declividade
create table GEO_REST_DECLIVIDADE(
   id number(10)  constraint pk_geo_rest_declividade primary key,
   projeto  		number(38) not null,
   data				date,
   tid				varchar2(36) not null,
   geometry       mdsys.sdo_geometry
);

comment on table geo_rest_declividade is 'Tabela que contem as geometrias mais recentes de Restrição de Declividade.';
comment on column geo_rest_declividade.id is 'Chave primária de identificação da tabela.';
comment on column geo_rest_declividade.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column geo_rest_declividade.data is 'Data de inserção desse registro na tabela.';
comment on column geo_rest_declividade.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column geo_rest_declividade.geometry is 'Campo geométrico.';


-- Escarpa
create table GEO_ESCARPA(
   id number(10)  constraint pk_geo_escarpa primary key,
   projeto  		number(38) not null,
   data				date,
   tid				varchar2(36) not null,
   geometry       mdsys.sdo_geometry
);

comment on table geo_escarpa is 'Tabela que contem as geometrias mais recentes de Escarpa.';
comment on column geo_escarpa.id is 'Chave primária de identificação da tabela.';
comment on column geo_escarpa.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column geo_escarpa.data is 'Data de inserção desse registro na tabela.';
comment on column geo_escarpa.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column geo_escarpa.geometry is 'Campo geométrico.';



----------------------------------------------------
-- Feições de Atividade

-- Ponto da Atividade
create table GEO_PATIV(
   id number(10)  constraint pk_geo_pativ primary key,
   projeto  		number(38) not null,
   cod_apmp  		number(10),
   codigo			varchar2(100),
   atividade      varchar2(150),
   rocha				varchar2(1),
   massa_dagua		varchar2(1),
   avn				varchar2(1),
   aa					varchar2(1),
   afs				varchar2(1),
   floresta_plantada	varchar2(1),
   arl				varchar2(1),
   rppn				varchar2(1),
   app				varchar2(1),
   data				date,
   tid				varchar2(36) not null,
   geometry       mdsys.sdo_geometry
);

comment on table geo_pativ is 'Tabela que contem as geometrias mais recentes de PATIV ( Ponto da Atividade ).';
comment on column geo_pativ.id is 'Chave primária de identificação da tabela.';
comment on column geo_pativ.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column geo_pativ.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column geo_pativ.codigo is 'Codigo gerado automaticamente para identificação da PATIV.';
comment on column geo_pativ.atividade is 'Texto da atividade associada ao projeto geo.';
comment on column geo_pativ.rocha is 'Identifica se a geometria cruza com ROCHA. Assume "S" para sim e "N" para não.';
comment on column geo_pativ.massa_dagua is 'Identifica se a geometria cruza com MASSA_DAGUA. Assume "S" para sim e "N" para não.';
comment on column geo_pativ.avn is 'Identifica se a geometria cruza com AVN. Case não encontre relação assume "N" para não, caso contrário, assume a sigla do estágio da AVN que cruzou podendo ser "I", "M", "A" ou "D".';
comment on column geo_pativ.aa is 'Identifica se a geometria cruza com AA. Assume "S" para sim e "N" para não.';
comment on column geo_pativ.afs is 'Identifica se a geometria cruza com AFS. Assume "S" para sim e "N" para não.';
comment on column geo_pativ.floresta_plantada is 'Identifica se a geometria cruza com ACV de tipo "FLORESTA_PLANTADA". Assume "S" para sim e "N" para não.';
comment on column geo_pativ.arl is 'Identifica se a geometria cruza com ARL. Assume "S" para sim e "N" para não.';
comment on column geo_pativ.rppn is 'Identifica se a geometria cruza com RPPN. Assume "S" para sim e "N" para não.';
comment on column geo_pativ.app is 'Identifica se a geometria cruza com APP. Assume "S" para sim e "N" para não.';
comment on column geo_pativ.data is 'Data de inserção desse registro na tabela.';
comment on column geo_pativ.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column geo_pativ.geometry is 'Campo geométrico.';


-- Linha da Atividade
create table GEO_LATIV(
   id number(10)  constraint pk_geo_lativ primary key,
   projeto  		number(38) not null,
   cod_apmp  		number(10),
   codigo			varchar2(100),
   atividade      varchar2(150),
   rocha				varchar2(1),
   massa_dagua		varchar2(1),
   avn				varchar2(1),
   aa					varchar2(1),
   afs				varchar2(1),
   floresta_plantada	varchar2(1),
   arl				varchar2(1),
   rppn				varchar2(1),
   app				varchar2(1),
   comprimento		number,
   data				date,
   tid				varchar2(36) not null,
   geometry       mdsys.sdo_geometry
);

comment on table geo_lativ is 'Tabela que contem as geometrias mais recentes de LATIV ( Linha da Atividade ).';
comment on column geo_lativ.id is 'Chave primária de identificação da tabela.';
comment on column geo_lativ.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column geo_lativ.cod_apmp is 'Referencia o ID da APMP que interage com o ponto médio dessa geometria.';
comment on column geo_lativ.codigo is 'Codigo gerado automaticamente para identificação da LATIV.';
comment on column geo_lativ.atividade is 'Texto da atividade associada ao projeto geo.';
comment on column geo_lativ.rocha is 'Identifica se a geometria cruza atravez do seu ponto médio com ROCHA. Assume "S" para sim e "N" para não.';
comment on column geo_lativ.massa_dagua is 'Identifica se a geometria cruza atravez do seu ponto médio com MASSA_DAGUA. Assume "S" para sim e "N" para não.';
comment on column geo_lativ.avn is 'Identifica se a geometria cruza atravez do seu ponto médio com AVN. Case não encontre relação assume "N" para não, caso contrário, assume a sigla do estágio da AVN que cruzou podendo ser "I", "M", "A" ou "D".';
comment on column geo_lativ.aa is 'Identifica se a geometria cruza atravez do seu ponto médio com AA. Assume "S" para sim e "N" para não.';
comment on column geo_lativ.afs is 'Identifica se a geometria cruza atravez do seu ponto médio com AFS. Assume "S" para sim e "N" para não.';
comment on column geo_lativ.floresta_plantada is 'Identifica se a geometria cruza atravez do seu ponto médio com ACV de tipo "FLORESTA_PLANTADA". Assume "S" para sim e "N" para não.';
comment on column geo_lativ.arl is 'Identifica se a geometria cruza atravez do seu ponto médio com ARL. Assume "S" para sim e "N" para não.';
comment on column geo_lativ.rppn is 'Identifica se a geometria cruza atravez do seu ponto médio com RPPN. Assume "S" para sim e "N" para não.';
comment on column geo_lativ.app is 'Identifica se a geometria cruza atravez do seu ponto médio com APP. Assume "S" para sim e "N" para não.';
comment on column geo_lativ.comprimento is 'Comprimento em metros da geometria.';
comment on column geo_lativ.data is 'Data de inserção desse registro na tabela.';
comment on column geo_lativ.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column geo_lativ.geometry is 'Campo geométrico.';

-- Area da Atividade
create table GEO_AATIV(
   id number(10)  constraint pk_geo_aativ primary key,
   projeto  		number(38) not null,
   cod_apmp  		number(10),
   codigo			varchar2(100),
   atividade      varchar2(150),
   rocha				varchar2(1),
   massa_dagua		varchar2(1),
   avn				varchar2(1),
   aa					varchar2(1),
   afs				varchar2(1),
   floresta_plantada	varchar2(1),
   arl				varchar2(1),
   rppn				varchar2(1),
   app				varchar2(1),
   area_m2			number,
   data				date,
   tid				varchar2(36) not null,
   geometry       mdsys.sdo_geometry
);

comment on table geo_aativ is 'Tabela que contem as geometrias mais recentes de AATIV ( Área da Atividade ).';
comment on column geo_aativ.id is 'Chave primária de identificação da tabela.';
comment on column geo_aativ.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column geo_aativ.cod_apmp is 'Referencia o ID da APMP que interage com o ponto ideal dessa geometria.';
comment on column geo_aativ.codigo is 'Codigo gerado automaticamente para identificação da AATIV.';
comment on column geo_aativ.atividade is 'Texto da atividade associada ao projeto geo.';
comment on column geo_aativ.rocha is 'Identifica se a geometria cruza atravez do seu ponto ideal com ROCHA. Assume "S" para sim e "N" para não.';
comment on column geo_aativ.massa_dagua is 'Identifica se a geometria cruza atravez do seu ponto ideal com MASSA_DAGUA. Assume "S" para sim e "N" para não.';
comment on column geo_aativ.avn is 'Identifica se a geometria cruza atravez do seu ponto ideal com AVN. Case não encontre relação assume "N" para não, caso contrário, assume a sigla do estágio da AVN que cruzou podendo ser "I", "M", "A" ou "D".';
comment on column geo_aativ.aa is 'Identifica se a geometria cruza atravez do seu ponto ideal com AA. Assume "S" para sim e "N" para não.';
comment on column geo_aativ.afs is 'Identifica se a geometria cruza atravez do seu ponto ideal com AFS. Assume "S" para sim e "N" para não.';
comment on column geo_aativ.floresta_plantada is 'Identifica se a geometria cruza atravez do seu ponto ideal com ACV de tipo "FLORESTA_PLANTADA". Assume "S" para sim e "N" para não.';
comment on column geo_aativ.arl is 'Identifica se a geometria cruza atravez do seu ponto ideal com ARL. Assume "S" para sim e "N" para não.';
comment on column geo_aativ.rppn is 'Identifica se a geometria cruza atravez do seu ponto ideal com RPPN. Assume "S" para sim e "N" para não.';
comment on column geo_aativ.app is 'Identifica se a geometria cruza atravez do seu ponto ideal com APP. Assume "S" para sim e "N" para não.';
comment on column geo_aativ.area_m2 is 'Área em metros quadrados da geometria.';
comment on column geo_aativ.data is 'Data de inserção desse registro na tabela.';
comment on column geo_aativ.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column geo_aativ.geometry is 'Campo geométrico.';


-- Area de Influencia da Atividade
create table GEO_AIATIV(
   id number(10)  constraint pk_geo_aiativ primary key,
   projeto  		number(38) not null,
   cod_apmp  		number(10),
   codigo			varchar2(100),
   atividade      varchar2(150),
   rocha				varchar2(1),
   massa_dagua		varchar2(1),
   avn				varchar2(1),
   aa					varchar2(1),
   afs				varchar2(1),
   floresta_plantada	varchar2(1),
   arl				varchar2(1),
   rppn				varchar2(1),
   app				varchar2(1),
   area_m2			number,
   data				date,
   tid				varchar2(36) not null,
   geometry       mdsys.sdo_geometry
);

comment on table geo_aiativ is 'Tabela que contem as geometrias mais recentes de AIATIV ( Área de Influência da Atividade ).';
comment on column geo_aiativ.id is 'Chave primária de identificação da tabela.';
comment on column geo_aiativ.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column geo_aiativ.cod_apmp is 'Referencia o ID da APMP que interage com o ponto ideal dessa geometria.';
comment on column geo_aiativ.codigo is 'Codigo gerado automaticamente para identificação da AIATIV.';
comment on column geo_aiativ.atividade is 'Texto da atividade associada ao projeto geo.';
comment on column geo_aiativ.rocha is 'Identifica se a geometria cruza atravez do seu ponto ideal com ROCHA. Assume "S" para sim e "N" para não.';
comment on column geo_aiativ.massa_dagua is 'Identifica se a geometria cruza atravez do seu ponto ideal com MASSA_DAGUA. Assume "S" para sim e "N" para não.';
comment on column geo_aiativ.avn is 'Identifica se a geometria cruza atravez do seu ponto ideal com AVN. Case não encontre relação assume "N" para não, caso contrário, assume a sigla do estágio da AVN que cruzou podendo ser "I", "M", "A" ou "D".';
comment on column geo_aiativ.aa is 'Identifica se a geometria cruza atravez do seu ponto ideal com AA. Assume "S" para sim e "N" para não.';
comment on column geo_aiativ.afs is 'Identifica se a geometria cruza atravez do seu ponto ideal com AFS. Assume "S" para sim e "N" para não.';
comment on column geo_aiativ.floresta_plantada is 'Identifica se a geometria cruza atravez do seu ponto ideal com ACV de tipo "FLORESTA_PLANTADA". Assume "S" para sim e "N" para não.';
comment on column geo_aiativ.arl is 'Identifica se a geometria cruza atravez do seu ponto ideal com ARL. Assume "S" para sim e "N" para não.';
comment on column geo_aiativ.rppn is 'Identifica se a geometria cruza atravez do seu ponto ideal com RPPN. Assume "S" para sim e "N" para não.';
comment on column geo_aiativ.app is 'Identifica se a geometria cruza atravez do seu ponto ideal com APP. Assume "S" para sim e "N" para não.';
comment on column geo_aiativ.area_m2 is 'Área em metros quadrados da geometria.';
comment on column geo_aiativ.data is 'Data de inserção desse registro na tabela.';
comment on column geo_aiativ.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column geo_aiativ.geometry is 'Campo geométrico.';









----------------------------------------------------
-- Tabelas de Histórico do Sistema
----------------------------------------------------

--Área Total do Imóvel
create sequence SEQ_HST_ATP;

create table HST_ATP(
   id number(10)  		constraint pk_hst_atp primary key,
   feature_id				number(10),
   projeto					number(38) not null,
   area_m2					number,
   data						date,
   tid						varchar2(36) not null,
   geometry					mdsys.sdo_geometry,
   executor_id				number(38) not null,
   executor_tid			varchar2(36),
   executor_nome			varchar2(80) not null,
   executor_login			varchar2(30) not null,
   executor_tipo_id		number(38) not null,
   executor_tipo_texto	varchar2(30) not null,
   acao_executada			number(38) not null,
   data_execucao			timestamp(6) not null
);

comment on table hst_atp is 'Tabela do histórico de ATP.';
comment on column hst_atp.id is 'Chave primária de identificação da tabela.';
comment on column hst_atp.feature_id is 'Identificador original da tabela oficial.';
comment on column hst_atp.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column hst_atp.area_m2 is 'Área em metros quadrados da geometria.';
comment on column hst_atp.data is 'Data de inserção desse registro na tabela.';
comment on column hst_atp.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column hst_atp.geometry is 'Campo geométrico.';
comment on column hst_atp.executor_id is 'Chave estrangeira para tab_funcionario no esquema oficial. Campo(ID).';
comment on column hst_atp.executor_tid is 'Referência ao campo (TID) da tabela tab_funcionario no esquema oficial.';
comment on column hst_atp.executor_nome is 'Nome do funcionário que executou a ação.';
comment on column hst_atp.executor_login is 'Login do funcionário que executou a ação.';
comment on column hst_atp.executor_tipo_id is 'Chave estrangeira para lov_executor_tipo no esquema oficial. Campo(ID).';
comment on column hst_atp.executor_tipo_texto is 'Texto do tipo do funcionário que executou a ação.';
comment on column hst_atp.acao_executada is 'Ação que foi disparada pelo sistema a qual gerou essa linha de histórico.';
comment on column hst_atp.data_execucao is 'Data que foi gerada essa linha de histórico.';


--Matrícula ou Posse
create sequence SEQ_HST_APMP;

create table HST_APMP(
   id number(10)  		constraint pk_hst_apmp primary key,
   feature_id				number(10),
   projeto  				number(38) not null,
   tipo						varchar2(100),
   nome           		varchar2(100),
   cod_atp        		number(10),
   area_m2        		number,
   data						date,
   tid						varchar2(36) not null,
   geometry       		mdsys.sdo_geometry,
   executor_id				number(38) not null,
   executor_tid			varchar2(36),
   executor_nome			varchar2(80) not null,
   executor_login			varchar2(30) not null,
   executor_tipo_id		number(38) not null,
   executor_tipo_texto	varchar2(30) not null,
   acao_executada			number(38) not null,
   data_execucao			timestamp(6) not null
);

comment on table hst_apmp is 'Tabela do histórico de APMP.';
comment on column hst_apmp.id is 'Chave primária de identificação da tabela.';
comment on column hst_apmp.feature_id is 'Identificador original da tabela oficial.';
comment on column hst_apmp.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column hst_apmp.tipo is 'Identifica a tipo da APMP, podendo ser "M" para Matrícula, "P" para Posse ou "D" para Desconhecido.';
comment on column hst_apmp.nome is 'Nome da APMP.';
comment on column hst_apmp.cod_atp is 'Referencia o ID da ATP que interage com essa geometria.';
comment on column hst_apmp.area_m2 is 'Área em metros quadrados da geometria.';
comment on column hst_apmp.data is 'Data de inserção desse registro na tabela.';
comment on column hst_apmp.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column hst_apmp.geometry is 'Campo geométrico.';
comment on column hst_apmp.executor_id is 'Chave estrangeira para tab_funcionario no esquema oficial. Campo(ID).';
comment on column hst_apmp.executor_tid is 'Referência ao campo (TID) da tabela tab_funcionario no esquema oficial.';
comment on column hst_apmp.executor_nome is 'Nome do funcionário que executou a ação.';
comment on column hst_apmp.executor_login is 'Login do funcionário que executou a ação.';
comment on column hst_apmp.executor_tipo_id is 'Chave estrangeira para lov_executor_tipo no esquema oficial. Campo(ID).';
comment on column hst_apmp.executor_tipo_texto is 'Texto do tipo do funcionário que executou a ação.';
comment on column hst_apmp.acao_executada is 'Ação que foi disparada pelo sistema a qual gerou essa linha de histórico.';
comment on column hst_apmp.data_execucao is 'Data que foi gerada essa linha de histórico.';


--Área da faixa de dominio
create sequence SEQ_HST_AFD;

create table HST_AFD(
   id number(10)  		constraint pk_hst_afd primary key,
   feature_id				number(10),
   projeto  				number(38) not null,
   area_m2        		number,
   data						date,
   tid						varchar2(36) not null,
   geometry       		mdsys.sdo_geometry,
   executor_id				number(38) not null,
   executor_tid			varchar2(36),
   executor_nome			varchar2(80) not null,
   executor_login			varchar2(30) not null,
   executor_tipo_id		number(38) not null,
   executor_tipo_texto	varchar2(30) not null,
   acao_executada			number(38) not null,
   data_execucao			timestamp(6) not null
);

comment on table hst_afd is 'Tabela do histórico de AFD.';
comment on column hst_afd.id is 'Chave primária de identificação da tabela.';
comment on column hst_afd.feature_id is 'Identificador original da tabela oficial.';
comment on column hst_afd.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column hst_afd.area_m2 is 'Área em metros quadrados da geometria.';
comment on column hst_afd.data is 'Data de inserção desse registro na tabela.';
comment on column hst_afd.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column hst_afd.geometry is 'Campo geométrico.';
comment on column hst_afd.executor_id is 'Chave estrangeira para tab_funcionario no esquema oficial. Campo(ID).';
comment on column hst_afd.executor_tid is 'Referência ao campo (TID) da tabela tab_funcionario no esquema oficial.';
comment on column hst_afd.executor_nome is 'Nome do funcionário que executou a ação.';
comment on column hst_afd.executor_login is 'Login do funcionário que executou a ação.';
comment on column hst_afd.executor_tipo_id is 'Chave estrangeira para lov_executor_tipo no esquema oficial. Campo(ID).';
comment on column hst_afd.executor_tipo_texto is 'Texto do tipo do funcionário que executou a ação.';
comment on column hst_afd.acao_executada is 'Ação que foi disparada pelo sistema a qual gerou essa linha de histórico.';
comment on column hst_afd.data_execucao is 'Data que foi gerada essa linha de histórico.';

	
-- Rocha
create sequence SEQ_HST_ROCHA;

create table HST_ROCHA(
   id number(10)  		constraint pk_hst_rocha primary key,
   feature_id				number(10),
   projeto  				number(38) not null,
   cod_apmp  				number(10),
   area_m2        		number,
   data						date,
   tid						varchar2(36) not null,
   geometry       		mdsys.sdo_geometry,
   executor_id				number(38) not null,
   executor_tid			varchar2(36),
   executor_nome			varchar2(80) not null,
   executor_login			varchar2(30) not null,
   executor_tipo_id		number(38) not null,
   executor_tipo_texto	varchar2(30) not null,
   acao_executada			number(38) not null,
   data_execucao			timestamp(6) not null
);

comment on table hst_rocha is 'Tabela do histórico de Rocha.';
comment on column hst_rocha.id is 'Chave primária de identificação da tabela.';
comment on column hst_rocha.feature_id is 'Identificador original da tabela oficial.';
comment on column hst_rocha.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column hst_rocha.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column hst_rocha.area_m2 is 'Área em metros quadrados da geometria.';
comment on column hst_rocha.data is 'Data de inserção desse registro na tabela.';
comment on column hst_rocha.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column hst_rocha.geometry is 'Campo geométrico.';
comment on column hst_rocha.executor_id is 'Chave estrangeira para tab_funcionario no esquema oficial. Campo(ID).';
comment on column hst_rocha.executor_tid is 'Referência ao campo (TID) da tabela tab_funcionario no esquema oficial.';
comment on column hst_rocha.executor_nome is 'Nome do funcionário que executou a ação.';
comment on column hst_rocha.executor_login is 'Login do funcionário que executou a ação.';
comment on column hst_rocha.executor_tipo_id is 'Chave estrangeira para lov_executor_tipo no esquema oficial. Campo(ID).';
comment on column hst_rocha.executor_tipo_texto is 'Texto do tipo do funcionário que executou a ação.';
comment on column hst_rocha.acao_executada is 'Ação que foi disparada pelo sistema a qual gerou essa linha de histórico.';
comment on column hst_rocha.data_execucao is 'Data que foi gerada essa linha de histórico.';


-- Vertices de APMP
create sequence SEQ_HST_VERTICE;

create table HST_VERTICE(
   id number(10)  		constraint pk_hst_vertice primary key,
   feature_id				number(10),
   projeto  				number(38) not null,
   nome           		varchar2(100),
   data						date,
   tid						varchar2(36) not null,
   geometry       		mdsys.sdo_geometry,
   executor_id				number(38) not null,
   executor_tid			varchar2(36),
   executor_nome			varchar2(80) not null,
   executor_login			varchar2(30) not null,
   executor_tipo_id		number(38) not null,
   executor_tipo_texto	varchar2(30) not null,
   acao_executada			number(38) not null,
   data_execucao			timestamp(6) not null
);

comment on table hst_vertice is 'Tabela do histórico de VERTICE.';
comment on column hst_vertice.id is 'Chave primária de identificação da tabela.';
comment on column hst_vertice.feature_id is 'Identificador original da tabela oficial.';
comment on column hst_vertice.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column hst_vertice.nome is 'Nome do VERTICE.';
comment on column hst_vertice.data is 'Data de inserção desse registro na tabela.';
comment on column hst_vertice.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column hst_vertice.geometry is 'Campo geométrico.';
comment on column hst_vertice.executor_id is 'Chave estrangeira para tab_funcionario no esquema oficial. Campo(ID).';
comment on column hst_vertice.executor_tid is 'Referência ao campo (TID) da tabela tab_funcionario no esquema oficial.';
comment on column hst_vertice.executor_nome is 'Nome do funcionário que executou a ação.';
comment on column hst_vertice.executor_login is 'Login do funcionário que executou a ação.';
comment on column hst_vertice.executor_tipo_id is 'Chave estrangeira para lov_executor_tipo no esquema oficial. Campo(ID).';
comment on column hst_vertice.executor_tipo_texto is 'Texto do tipo do funcionário que executou a ação.';
comment on column hst_vertice.acao_executada is 'Ação que foi disparada pelo sistema a qual gerou essa linha de histórico.';
comment on column hst_vertice.data_execucao is 'Data que foi gerada essa linha de histórico.';


-- Area de Reserva Legal
create sequence SEQ_HST_ARL;

create table HST_ARL(
   id number(10)  		constraint pk_hst_arl primary key,
   feature_id				number(10),
   projeto  				number(38) not null,
   cod_apmp  				number(10),
   codigo					varchar2(100),
   compensada     		varchar2(1),
   situacao       		varchar2(50),
   area_m2        		number,
   data						date,
   tid						varchar2(36) not null,
   geometry       		mdsys.sdo_geometry,
   executor_id				number(38) not null,
   executor_tid			varchar2(36),
   executor_nome			varchar2(80) not null,
   executor_login			varchar2(30) not null,
   executor_tipo_id		number(38) not null,
   executor_tipo_texto	varchar2(30) not null,
   acao_executada			number(38) not null,
   data_execucao			timestamp(6) not null
);

comment on table hst_arl is 'Tabela do histórico de ARL.';
comment on column hst_arl.id is 'Chave primária de identificação da tabela.';
comment on column hst_arl.feature_id is 'Identificador original da tabela oficial.';
comment on column hst_arl.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column hst_arl.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column hst_arl.codigo is 'Codigo gerado automaticamente para identificação da ARL.';
comment on column hst_arl.compensada is 'Identifica se a ARL é compensada. Assume "S" para sim e "N" para não.';
comment on column hst_arl.situacao is 'Identifica a situação da ARL, podendo ser "AA" para ARLs em Formação, "AVN" para Preservadas ou "Não Informado" para Indefinidas.';
comment on column hst_arl.area_m2 is 'Área em metros quadrados da geometria.';
comment on column hst_arl.data is 'Data de inserção desse registro na tabela.';
comment on column hst_arl.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column hst_arl.geometry is 'Campo geométrico.';
comment on column hst_arl.executor_id is 'Chave estrangeira para tab_funcionario no esquema oficial. Campo(ID).';
comment on column hst_arl.executor_tid is 'Referência ao campo (TID) da tabela tab_funcionario no esquema oficial.';
comment on column hst_arl.executor_nome is 'Nome do funcionário que executou a ação.';
comment on column hst_arl.executor_login is 'Login do funcionário que executou a ação.';
comment on column hst_arl.executor_tipo_id is 'Chave estrangeira para lov_executor_tipo no esquema oficial. Campo(ID).';
comment on column hst_arl.executor_tipo_texto is 'Texto do tipo do funcionário que executou a ação.';
comment on column hst_arl.acao_executada is 'Ação que foi disparada pelo sistema a qual gerou essa linha de histórico.';
comment on column hst_arl.data_execucao is 'Data que foi gerada essa linha de histórico.';


-- Reserva Particular de Patrimônio Natural
create sequence SEQ_HST_RPPN;

create table HST_RPPN(
   id number(10)  		constraint pk_hst_rppn primary key,
   feature_id				number(10),
   projeto  				number(38) not null,
   cod_apmp  				number(10),
   area_m2        		number,
   data						date,
   tid						varchar2(36) not null,
   geometry       		mdsys.sdo_geometry,
   executor_id				number(38) not null,
   executor_tid			varchar2(36),
   executor_nome			varchar2(80) not null,
   executor_login			varchar2(30) not null,
   executor_tipo_id		number(38) not null,
   executor_tipo_texto	varchar2(30) not null,
   acao_executada			number(38) not null,
   data_execucao			timestamp(6) not null
);

comment on table hst_rppn is 'Tabela do histórico de RPPN.';
comment on column hst_rppn.id is 'Chave primária de identificação da tabela.';
comment on column hst_rppn.feature_id is 'Identificador original da tabela oficial.';
comment on column hst_rppn.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column hst_rppn.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column hst_rppn.area_m2 is 'Área em metros quadrados da geometria.';
comment on column hst_rppn.data is 'Data de inserção desse registro na tabela.';
comment on column hst_rppn.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column hst_rppn.geometry is 'Campo geométrico.';
comment on column hst_rppn.executor_id is 'Chave estrangeira para tab_funcionario no esquema oficial. Campo(ID).';
comment on column hst_rppn.executor_tid is 'Referência ao campo (TID) da tabela tab_funcionario no esquema oficial.';
comment on column hst_rppn.executor_nome is 'Nome do funcionário que executou a ação.';
comment on column hst_rppn.executor_login is 'Login do funcionário que executou a ação.';
comment on column hst_rppn.executor_tipo_id is 'Chave estrangeira para lov_executor_tipo no esquema oficial. Campo(ID).';
comment on column hst_rppn.executor_tipo_texto is 'Texto do tipo do funcionário que executou a ação.';
comment on column hst_rppn.acao_executada is 'Ação que foi disparada pelo sistema a qual gerou essa linha de histórico.';
comment on column hst_rppn.data_execucao is 'Data que foi gerada essa linha de histórico.';


-- Área de Faixa de Servidão
create sequence SEQ_HST_AFS;

create table HST_AFS(
   id number(10)  		constraint pk_hst_afs primary key,
   feature_id				number(10),
   projeto  				number(38) not null,
   cod_apmp  				number(10),
   area_m2       			number,
   data						date,
   tid						varchar2(36) not null,
   geometry       		mdsys.sdo_geometry,
   executor_id				number(38) not null,
   executor_tid			varchar2(36),
   executor_nome			varchar2(80) not null,
   executor_login			varchar2(30) not null,
   executor_tipo_id		number(38) not null,
   executor_tipo_texto	varchar2(30) not null,
   acao_executada			number(38) not null,
   data_execucao			timestamp(6) not null
);

comment on table hst_afs is 'Tabela do histórico de AFS.';
comment on column hst_afs.id is 'Chave primária de identificação da tabela.';
comment on column hst_afs.feature_id is 'Identificador original da tabela oficial.';
comment on column hst_afs.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column hst_afs.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column hst_afs.area_m2 is 'Área em metros quadrados da geometria.';
comment on column hst_afs.data is 'Data de inserção desse registro na tabela.';
comment on column hst_afs.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column hst_afs.geometry is 'Campo geométrico.';
comment on column hst_afs.executor_id is 'Chave estrangeira para tab_funcionario no esquema oficial. Campo(ID).';
comment on column hst_afs.executor_tid is 'Referência ao campo (TID) da tabela tab_funcionario no esquema oficial.';
comment on column hst_afs.executor_nome is 'Nome do funcionário que executou a ação.';
comment on column hst_afs.executor_login is 'Login do funcionário que executou a ação.';
comment on column hst_afs.executor_tipo_id is 'Chave estrangeira para lov_executor_tipo no esquema oficial. Campo(ID).';
comment on column hst_afs.executor_tipo_texto is 'Texto do tipo do funcionário que executou a ação.';
comment on column hst_afs.acao_executada is 'Ação que foi disparada pelo sistema a qual gerou essa linha de histórico.';
comment on column hst_afs.data_execucao is 'Data que foi gerada essa linha de histórico.';


-- Área de Vegetação Nativa
create sequence SEQ_HST_AVN;

create table HST_AVN(
   id number(10)  		constraint pk_hst_avn primary key,
   feature_id				number(10),
   projeto  				number(38) not null,
   cod_apmp  				number(10),
   estagio        		varchar2(100),
   area_m2        		number,
   data						date,
   tid						varchar2(36) not null,
   geometry       		mdsys.sdo_geometry,
   executor_id				number(38) not null,
   executor_tid			varchar2(36),
   executor_nome			varchar2(80) not null,
   executor_login			varchar2(30) not null,
   executor_tipo_id		number(38) not null,
   executor_tipo_texto	varchar2(30) not null,
   acao_executada			number(38) not null,
   data_execucao			timestamp(6) not null
);

comment on table hst_avn is 'Tabela do histórico de AVN.';
comment on column hst_avn.id is 'Chave primária de identificação da tabela.';
comment on column hst_avn.feature_id is 'Identificador original da tabela oficial.';
comment on column hst_avn.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column hst_avn.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column hst_avn.estagio is 'Estagio da area, podendo ser "I" para Inicial, "M" para Médio, "A" para Avançado ou "D" para Desconhecido.';
comment on column hst_avn.area_m2 is 'Área em metros quadrados da geometria.';
comment on column hst_avn.data is 'Data de inserção desse registro na tabela.';
comment on column hst_avn.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column hst_avn.geometry is 'Campo geométrico.';
comment on column hst_avn.executor_id is 'Chave estrangeira para tab_funcionario no esquema oficial. Campo(ID).';
comment on column hst_avn.executor_tid is 'Referência ao campo (TID) da tabela tab_funcionario no esquema oficial.';
comment on column hst_avn.executor_nome is 'Nome do funcionário que executou a ação.';
comment on column hst_avn.executor_login is 'Login do funcionário que executou a ação.';
comment on column hst_avn.executor_tipo_id is 'Chave estrangeira para lov_executor_tipo no esquema oficial. Campo(ID).';
comment on column hst_avn.executor_tipo_texto is 'Texto do tipo do funcionário que executou a ação.';
comment on column hst_avn.acao_executada is 'Ação que foi disparada pelo sistema a qual gerou essa linha de histórico.';
comment on column hst_avn.data_execucao is 'Data que foi gerada essa linha de histórico.';


-- Área Aberta
create sequence SEQ_HST_AA;

create table HST_AA(
   id number(10) 			constraint pk_hst_aa primary key,
   feature_id				number(10),
   projeto  				number(38) not null,
   cod_apmp  				number(10),
   tipo        			varchar2(100),
   area_m2        		number,
   data						date,
   tid						varchar2(36) not null,
   geometry       		mdsys.sdo_geometry,
   executor_id				number(38) not null,
   executor_tid			varchar2(36),
   executor_nome			varchar2(80) not null,
   executor_login			varchar2(30) not null,
   executor_tipo_id		number(38) not null,
   executor_tipo_texto	varchar2(30) not null,
   acao_executada			number(38) not null,
   data_execucao			timestamp(6) not null
);

comment on table hst_aa is 'Tabela do histórico de AA.';
comment on column hst_aa.id is 'Chave primária de identificação da tabela.';
comment on column hst_aa.feature_id is 'Identificador original da tabela oficial.';
comment on column hst_aa.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column hst_aa.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column hst_aa.tipo is 'Tipo da area, podendo ser "C" para Cultivada, "NC" para Não Cultivada ou "D" para Desconhecido.';
comment on column hst_aa.area_m2 is 'Área em metros quadrados da geometria.';
comment on column hst_aa.data is 'Data de inserção desse registro na tabela.';
comment on column hst_aa.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column hst_aa.geometry is 'Campo geométrico.';
comment on column hst_aa.executor_id is 'Chave estrangeira para tab_funcionario no esquema oficial. Campo(ID).';
comment on column hst_aa.executor_tid is 'Referência ao campo (TID) da tabela tab_funcionario no esquema oficial.';
comment on column hst_aa.executor_nome is 'Nome do funcionário que executou a ação.';
comment on column hst_aa.executor_login is 'Login do funcionário que executou a ação.';
comment on column hst_aa.executor_tipo_id is 'Chave estrangeira para lov_executor_tipo no esquema oficial. Campo(ID).';
comment on column hst_aa.executor_tipo_texto is 'Texto do tipo do funcionário que executou a ação.';
comment on column hst_aa.acao_executada is 'Ação que foi disparada pelo sistema a qual gerou essa linha de histórico.';
comment on column hst_aa.data_execucao is 'Data que foi gerada essa linha de histórico.';


-- Área de Classificação de Vegetação
create sequence SEQ_HST_ACV;

create table HST_ACV(
   id number(10)  		constraint pk_hst_acv primary key,
   feature_id				number(10),
   projeto  				number(38) not null,
   cod_apmp  				number(10),
   tipo        			varchar2(100),
   area_m2        		number,
   data						date,
   tid						varchar2(36) not null,
   geometry       		mdsys.sdo_geometry,
   executor_id				number(38) not null,
   executor_tid			varchar2(36),
   executor_nome			varchar2(80) not null,
   executor_login			varchar2(30) not null,
   executor_tipo_id		number(38) not null,
   executor_tipo_texto	varchar2(30) not null,
   acao_executada			number(38) not null,
   data_execucao			timestamp(6) not null
);

comment on table hst_acv is 'Tabela do histórico de ACV.';
comment on column hst_acv.id is 'Chave primária de identificação da tabela.';
comment on column hst_acv.feature_id is 'Identificador original da tabela oficial.';
comment on column hst_acv.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column hst_acv.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column hst_acv.tipo is 'Tipo da area, podendo ser "MANGUE", "BREJO", "RESTINGA", "RESTINGA-APP", "FLORESTA-NATIVA", "FLORESTA-PLANTADA", "MACEGA", "CABRUCA" ou "OUTROS".';
comment on column hst_acv.area_m2 is 'Área em metros quadrados da geometria.';
comment on column hst_acv.data is 'Data de inserção desse registro na tabela.';
comment on column hst_acv.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column hst_acv.geometry is 'Campo geométrico.';
comment on column hst_acv.executor_id is 'Chave estrangeira para tab_funcionario no esquema oficial. Campo(ID).';
comment on column hst_acv.executor_tid is 'Referência ao campo (TID) da tabela tab_funcionario no esquema oficial.';
comment on column hst_acv.executor_nome is 'Nome do funcionário que executou a ação.';
comment on column hst_acv.executor_login is 'Login do funcionário que executou a ação.';
comment on column hst_acv.executor_tipo_id is 'Chave estrangeira para lov_executor_tipo no esquema oficial. Campo(ID).';
comment on column hst_acv.executor_tipo_texto is 'Texto do tipo do funcionário que executou a ação.';
comment on column hst_acv.acao_executada is 'Ação que foi disparada pelo sistema a qual gerou essa linha de histórico.';
comment on column hst_acv.data_execucao is 'Data que foi gerada essa linha de histórico.';



----------------------------------------------------
-- Outras 

-- Área Construída
create sequence SEQ_HST_ACONSTRUIDA;

create table HST_ACONSTRUIDA(
   id number(10)  		constraint pk_hst_aconstruida primary key,
   feature_id				number(10),
   projeto  				number(38) not null,
   area_m2       			number,
   data						date,
   tid						varchar2(36) not null,
   geometry       		mdsys.sdo_geometry,
   executor_id				number(38) not null,
   executor_tid			varchar2(36),
   executor_nome			varchar2(80) not null,
   executor_login			varchar2(30) not null,
   executor_tipo_id		number(38) not null,
   executor_tipo_texto	varchar2(30) not null,
   acao_executada			number(38) not null,
   data_execucao			timestamp(6) not null
);

comment on table hst_aconstruida is 'Tabela do histórico de AConstruida.';
comment on column hst_aconstruida.id is 'Chave primária de identificação da tabela.';
comment on column hst_aconstruida.feature_id is 'Identificador original da tabela oficial.';
comment on column hst_aconstruida.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column hst_aconstruida.area_m2 is 'Área em metros quadrados da geometria.';
comment on column hst_aconstruida.data is 'Data de inserção desse registro na tabela.';
comment on column hst_aconstruida.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column hst_aconstruida.geometry is 'Campo geométrico.';
comment on column hst_aconstruida.executor_id is 'Chave estrangeira para tab_funcionario no esquema oficial. Campo(ID).';
comment on column hst_aconstruida.executor_tid is 'Referência ao campo (TID) da tabela tab_funcionario no esquema oficial.';
comment on column hst_aconstruida.executor_nome is 'Nome do funcionário que executou a ação.';
comment on column hst_aconstruida.executor_login is 'Login do funcionário que executou a ação.';
comment on column hst_aconstruida.executor_tipo_id is 'Chave estrangeira para lov_executor_tipo no esquema oficial. Campo(ID).';
comment on column hst_aconstruida.executor_tipo_texto is 'Texto do tipo do funcionário que executou a ação.';
comment on column hst_aconstruida.acao_executada is 'Ação que foi disparada pelo sistema a qual gerou essa linha de histórico.';
comment on column hst_aconstruida.data_execucao is 'Data que foi gerada essa linha de histórico.';

-- Linhas de DUTO
create sequence SEQ_HST_DUTO;

create table HST_DUTO(
   id number(10)  		constraint pk_hst_duto primary key,
   feature_id				number(10),
   projeto  				number(38) not null,
   data						date,
   tid						varchar2(36) not null,
   geometry       		mdsys.sdo_geometry,
   executor_id				number(38) not null,
   executor_tid			varchar2(36),
   executor_nome			varchar2(80) not null,
   executor_login			varchar2(30) not null,
   executor_tipo_id		number(38) not null,
   executor_tipo_texto	varchar2(30) not null,
   acao_executada			number(38) not null,
   data_execucao			timestamp(6) not null
);

comment on table hst_duto is 'Tabela do histórico de Duto.';
comment on column hst_duto.id is 'Chave primária de identificação da tabela.';
comment on column hst_duto.feature_id is 'Identificador original da tabela oficial.';
comment on column hst_duto.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column hst_duto.data is 'Data de inserção desse registro na tabela.';
comment on column hst_duto.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column hst_duto.geometry is 'Campo geométrico.';
comment on column hst_duto.executor_id is 'Chave estrangeira para tab_funcionario no esquema oficial. Campo(ID).';
comment on column hst_duto.executor_tid is 'Referência ao campo (TID) da tabela tab_funcionario no esquema oficial.';
comment on column hst_duto.executor_nome is 'Nome do funcionário que executou a ação.';
comment on column hst_duto.executor_login is 'Login do funcionário que executou a ação.';
comment on column hst_duto.executor_tipo_id is 'Chave estrangeira para lov_executor_tipo no esquema oficial. Campo(ID).';
comment on column hst_duto.executor_tipo_texto is 'Texto do tipo do funcionário que executou a ação.';
comment on column hst_duto.acao_executada is 'Ação que foi disparada pelo sistema a qual gerou essa linha de histórico.';
comment on column hst_duto.data_execucao is 'Data que foi gerada essa linha de histórico.';


-- Linhas de TRANSMISSAO
create sequence SEQ_HST_LTRANSMISSAO;

create table HST_LTRANSMISSAO(
   id number(10)  		constraint pk_hst_ltransmissao primary key,
   feature_id				number(10),
   projeto  				number(38) not null,
   data						date,
   tid						varchar2(36) not null,
   geometry       		mdsys.sdo_geometry,
   executor_id				number(38) not null,
   executor_tid			varchar2(36),
   executor_nome			varchar2(80) not null,
   executor_login			varchar2(30) not null,
   executor_tipo_id		number(38) not null,
   executor_tipo_texto	varchar2(30) not null,
   acao_executada			number(38) not null,
   data_execucao			timestamp(6) not null
);

comment on table hst_ltransmissao is 'Tabela do histórico de LTransmissao.';
comment on column hst_ltransmissao.id is 'Chave primária de identificação da tabela.';
comment on column hst_ltransmissao.feature_id is 'Identificador original da tabela oficial.';
comment on column hst_ltransmissao.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column hst_ltransmissao.data is 'Data de inserção desse registro na tabela.';
comment on column hst_ltransmissao.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column hst_ltransmissao.geometry is 'Campo geométrico.';
comment on column hst_ltransmissao.executor_id is 'Chave estrangeira para tab_funcionario no esquema oficial. Campo(ID).';
comment on column hst_ltransmissao.executor_tid is 'Referência ao campo (TID) da tabela tab_funcionario no esquema oficial.';
comment on column hst_ltransmissao.executor_nome is 'Nome do funcionário que executou a ação.';
comment on column hst_ltransmissao.executor_login is 'Login do funcionário que executou a ação.';
comment on column hst_ltransmissao.executor_tipo_id is 'Chave estrangeira para lov_executor_tipo no esquema oficial. Campo(ID).';
comment on column hst_ltransmissao.executor_tipo_texto is 'Texto do tipo do funcionário que executou a ação.';
comment on column hst_ltransmissao.acao_executada is 'Ação que foi disparada pelo sistema a qual gerou essa linha de histórico.';
comment on column hst_ltransmissao.data_execucao is 'Data que foi gerada essa linha de histórico.';


-- Linhas de ESTRADA
create sequence SEQ_HST_ESTRADA;

create table HST_ESTRADA(
   id number(10)  		constraint pk_hst_estrada primary key,
   feature_id				number(10),
   projeto  				number(38) not null,
   data						date,
   tid						varchar2(36) not null,
   geometry       		mdsys.sdo_geometry,
   executor_id				number(38) not null,
   executor_tid			varchar2(36),
   executor_nome			varchar2(80) not null,
   executor_login			varchar2(30) not null,
   executor_tipo_id		number(38) not null,
   executor_tipo_texto	varchar2(30) not null,
   acao_executada			number(38) not null,
   data_execucao			timestamp(6) not null
);

comment on table hst_estrada is 'Tabela do histórico de Estrada.';
comment on column hst_estrada.id is 'Chave primária de identificação da tabela.';
comment on column hst_estrada.feature_id is 'Identificador original da tabela oficial.';
comment on column hst_estrada.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column hst_estrada.data is 'Data de inserção desse registro na tabela.';
comment on column hst_estrada.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column hst_estrada.geometry is 'Campo geométrico.';
comment on column hst_estrada.executor_id is 'Chave estrangeira para tab_funcionario no esquema oficial. Campo(ID).';
comment on column hst_estrada.executor_tid is 'Referência ao campo (TID) da tabela tab_funcionario no esquema oficial.';
comment on column hst_estrada.executor_nome is 'Nome do funcionário que executou a ação.';
comment on column hst_estrada.executor_login is 'Login do funcionário que executou a ação.';
comment on column hst_estrada.executor_tipo_id is 'Chave estrangeira para lov_executor_tipo no esquema oficial. Campo(ID).';
comment on column hst_estrada.executor_tipo_texto is 'Texto do tipo do funcionário que executou a ação.';
comment on column hst_estrada.acao_executada is 'Ação que foi disparada pelo sistema a qual gerou essa linha de histórico.';
comment on column hst_estrada.data_execucao is 'Data que foi gerada essa linha de histórico.';


-- Linhas de FERROVIA
create sequence SEQ_HST_FERROVIA;

create table HST_FERROVIA(
   id number(10)  		constraint pk_hst_ferrovia primary key,
   feature_id				number(10),
   projeto  				number(38) not null,
   data						date,
   tid						varchar2(36) not null,
   geometry       		mdsys.sdo_geometry,
   executor_id				number(38) not null,
   executor_tid			varchar2(36),
   executor_nome			varchar2(80) not null,
   executor_login			varchar2(30) not null,
   executor_tipo_id		number(38) not null,
   executor_tipo_texto	varchar2(30) not null,
   acao_executada			number(38) not null,
   data_execucao			timestamp(6) not null
);

comment on table hst_ferrovia is 'Tabela do histórico de Ferrovia.';
comment on column hst_ferrovia.id is 'Chave primária de identificação da tabela.';
comment on column hst_ferrovia.feature_id is 'Identificador original da tabela oficial.';
comment on column hst_ferrovia.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column hst_ferrovia.data is 'Data de inserção desse registro na tabela.';
comment on column hst_ferrovia.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column hst_ferrovia.geometry is 'Campo geométrico.';
comment on column hst_ferrovia.executor_id is 'Chave estrangeira para tab_funcionario no esquema oficial. Campo(ID).';
comment on column hst_ferrovia.executor_tid is 'Referência ao campo (TID) da tabela tab_funcionario no esquema oficial.';
comment on column hst_ferrovia.executor_nome is 'Nome do funcionário que executou a ação.';
comment on column hst_ferrovia.executor_login is 'Login do funcionário que executou a ação.';
comment on column hst_ferrovia.executor_tipo_id is 'Chave estrangeira para lov_executor_tipo no esquema oficial. Campo(ID).';
comment on column hst_ferrovia.executor_tipo_texto is 'Texto do tipo do funcionário que executou a ação.';
comment on column hst_ferrovia.acao_executada is 'Ação que foi disparada pelo sistema a qual gerou essa linha de histórico.';
comment on column hst_ferrovia.data_execucao is 'Data que foi gerada essa linha de histórico.';


----------------------------------------------------
-- Areas Calculadas pelo Sistema


-- Áreas Calculadas
create sequence SEQ_HST_AREAS_CALCULADAS;

create table HST_AREAS_CALCULADAS(
   id number(10)  		constraint pk_hst_areas_calculadas primary key,
   feature_id				number(10),
   projeto  				number(38) not null,
   cod_apmp  				number(10),
   tipo        			varchar2(100),
   area_m2        		number,
   data						date,
   tid						varchar2(36) not null,
   geometry       		mdsys.sdo_geometry,
   executor_id				number(38) not null,
   executor_tid			varchar2(36),
   executor_nome			varchar2(80) not null,
   executor_login			varchar2(30) not null,
   executor_tipo_id		number(38) not null,
   executor_tipo_texto	varchar2(30) not null,
   acao_executada			number(38) not null,
   data_execucao			timestamp(6) not null
);

comment on table hst_areas_calculadas is 'Tabela do histórico de Areas Calculadas.';
comment on column hst_areas_calculadas.id is 'Chave primária de identificação da tabela.';
comment on column hst_areas_calculadas.feature_id is 'Identificador original da tabela oficial.';
comment on column hst_areas_calculadas.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column hst_areas_calculadas.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column hst_areas_calculadas.tipo is 'Tipo da area calculada, podendo ser "APMP_APMP", "APMP_AFD", "ARL_ARL", "ARL_ROCHA", "AA_AVN", "APP_APMP", "APP_AA", "APP_AVN", "APP_ARL", "MASSA_DAGUA_APMP" ou "AATIV_AATIV".';
comment on column hst_areas_calculadas.area_m2 is 'Área em metros quadrados da geometria.';
comment on column hst_areas_calculadas.data is 'Data de inserção desse registro na tabela.';
comment on column hst_areas_calculadas.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column hst_areas_calculadas.geometry is 'Campo geométrico.';
comment on column hst_areas_calculadas.executor_id is 'Chave estrangeira para tab_funcionario no esquema oficial. Campo(ID).';
comment on column hst_areas_calculadas.executor_tid is 'Referência ao campo (TID) da tabela tab_funcionario no esquema oficial.';
comment on column hst_areas_calculadas.executor_nome is 'Nome do funcionário que executou a ação.';
comment on column hst_areas_calculadas.executor_login is 'Login do funcionário que executou a ação.';
comment on column hst_areas_calculadas.executor_tipo_id is 'Chave estrangeira para lov_executor_tipo no esquema oficial. Campo(ID).';
comment on column hst_areas_calculadas.executor_tipo_texto is 'Texto do tipo do funcionário que executou a ação.';
comment on column hst_areas_calculadas.acao_executada is 'Ação que foi disparada pelo sistema a qual gerou essa linha de histórico.';
comment on column hst_areas_calculadas.data_execucao is 'Data que foi gerada essa linha de histórico.';


----------------------------------------------------
-- Areas que dao origem a APP

-- Nascente
create sequence SEQ_HST_NASCENTE;

create table HST_NASCENTE(
   id number(10)  		constraint pk_hst_nascente primary key,
   feature_id				number(10),
   projeto  				number(38) not null,
   data						date,
   tid						varchar2(36) not null,
   geometry       		mdsys.sdo_geometry,
   executor_id				number(38) not null,
   executor_tid			varchar2(36),
   executor_nome			varchar2(80) not null,
   executor_login			varchar2(30) not null,
   executor_tipo_id		number(38) not null,
   executor_tipo_texto	varchar2(30) not null,
   acao_executada			number(38) not null,
   data_execucao			timestamp(6) not null
);

comment on table hst_nascente is 'Tabela do histórico de Nascente.';
comment on column hst_nascente.id is 'Chave primária de identificação da tabela.';
comment on column hst_nascente.feature_id is 'Identificador original da tabela oficial.';
comment on column hst_nascente.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column hst_nascente.data is 'Data de inserção desse registro na tabela.';
comment on column hst_nascente.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column hst_nascente.geometry is 'Campo geométrico.';
comment on column hst_nascente.executor_id is 'Chave estrangeira para tab_funcionario no esquema oficial. Campo(ID).';
comment on column hst_nascente.executor_tid is 'Referência ao campo (TID) da tabela tab_funcionario no esquema oficial.';
comment on column hst_nascente.executor_nome is 'Nome do funcionário que executou a ação.';
comment on column hst_nascente.executor_login is 'Login do funcionário que executou a ação.';
comment on column hst_nascente.executor_tipo_id is 'Chave estrangeira para lov_executor_tipo no esquema oficial. Campo(ID).';
comment on column hst_nascente.executor_tipo_texto is 'Texto do tipo do funcionário que executou a ação.';
comment on column hst_nascente.acao_executada is 'Ação que foi disparada pelo sistema a qual gerou essa linha de histórico.';
comment on column hst_nascente.data_execucao is 'Data que foi gerada essa linha de histórico.';

	
-- Linhas de Rio
create sequence SEQ_HST_RIO_LINHA;

create table HST_RIO_LINHA(
   id number(10)  		constraint pk_hst_rio_linha primary key,
   feature_id				number(10),
   projeto  				number(38) not null,
   nome           		varchar2(100),
   largura        		number,
   data						date,
   tid						varchar2(36) not null,
   geometry       		mdsys.sdo_geometry,
   executor_id				number(38) not null,
   executor_tid			varchar2(36),
   executor_nome			varchar2(80) not null,
   executor_login			varchar2(30) not null,
   executor_tipo_id		number(38) not null,
   executor_tipo_texto	varchar2(30) not null,
   acao_executada			number(38) not null,
   data_execucao			timestamp(6) not null
);

comment on table hst_rio_linha is 'Tabela do histórico de RIO_LINHA.';
comment on column hst_rio_linha.id is 'Chave primária de identificação da tabela.';
comment on column hst_rio_linha.feature_id is 'Identificador original da tabela oficial.';
comment on column hst_rio_linha.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column hst_rio_linha.nome is 'Nome do Rio.';
comment on column hst_rio_linha.largura is 'Largura do rio usada para calcular a APP.';
comment on column hst_rio_linha.data is 'Data de inserção desse registro na tabela.';
comment on column hst_rio_linha.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column hst_rio_linha.geometry is 'Campo geométrico.';
comment on column hst_rio_linha.executor_id is 'Chave estrangeira para tab_funcionario no esquema oficial. Campo(ID).';
comment on column hst_rio_linha.executor_tid is 'Referência ao campo (TID) da tabela tab_funcionario no esquema oficial.';
comment on column hst_rio_linha.executor_nome is 'Nome do funcionário que executou a ação.';
comment on column hst_rio_linha.executor_login is 'Login do funcionário que executou a ação.';
comment on column hst_rio_linha.executor_tipo_id is 'Chave estrangeira para lov_executor_tipo no esquema oficial. Campo(ID).';
comment on column hst_rio_linha.executor_tipo_texto is 'Texto do tipo do funcionário que executou a ação.';
comment on column hst_rio_linha.acao_executada is 'Ação que foi disparada pelo sistema a qual gerou essa linha de histórico.';
comment on column hst_rio_linha.data_execucao is 'Data que foi gerada essa linha de histórico.';


-- Areas de Rio
create sequence SEQ_HST_RIO_AREA;

create table HST_RIO_AREA(
   id number(10)  		constraint pk_hst_rio_area primary key,
   feature_id				number(10),
   projeto  				number(38) not null,
   cod_apmp  				number(10),
   area_m2        		number,
   nome           		varchar2(100),
   largura        		number,
   data						date,
   tid						varchar2(36) not null,
   geometry       		mdsys.sdo_geometry,
   executor_id				number(38) not null,
   executor_tid			varchar2(36),
   executor_nome			varchar2(80) not null,
   executor_login			varchar2(30) not null,
   executor_tipo_id		number(38) not null,
   executor_tipo_texto	varchar2(30) not null,
   acao_executada			number(38) not null,
   data_execucao			timestamp(6) not null
);

comment on table hst_rio_area is 'Tabela do histórico de RIO_AREA.';
comment on column hst_rio_area.id is 'Chave primária de identificação da tabela.';
comment on column hst_rio_area.feature_id is 'Identificador original da tabela oficial.';
comment on column hst_rio_area.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column hst_rio_area.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column hst_rio_area.area_m2 is 'Área em metros quadrados da geometria.';
comment on column hst_rio_area.nome is 'Nome do Rio.';
comment on column hst_rio_area.largura is 'Largura do rio usada para calcular a APP.';
comment on column hst_rio_area.data is 'Data de inserção desse registro na tabela.';
comment on column hst_rio_area.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column hst_rio_area.geometry is 'Campo geométrico.';
comment on column hst_rio_area.executor_id is 'Chave estrangeira para tab_funcionario no esquema oficial. Campo(ID).';
comment on column hst_rio_area.executor_tid is 'Referência ao campo (TID) da tabela tab_funcionario no esquema oficial.';
comment on column hst_rio_area.executor_nome is 'Nome do funcionário que executou a ação.';
comment on column hst_rio_area.executor_login is 'Login do funcionário que executou a ação.';
comment on column hst_rio_area.executor_tipo_id is 'Chave estrangeira para lov_executor_tipo no esquema oficial. Campo(ID).';
comment on column hst_rio_area.executor_tipo_texto is 'Texto do tipo do funcionário que executou a ação.';
comment on column hst_rio_area.acao_executada is 'Ação que foi disparada pelo sistema a qual gerou essa linha de histórico.';
comment on column hst_rio_area.data_execucao is 'Data que foi gerada essa linha de histórico.';


-- Areas de Lagos e Lagoas
create sequence SEQ_HST_LAGOA;

create table HST_LAGOA(
   id number(10)  		constraint pk_hst_lagoa primary key,
   feature_id				number(10),
   projeto  				number(38) not null,
   cod_apmp  				number(10),
   area_m2        		number,
   zona						varchar2(100),
   nome           		varchar2(100),
   data						date,
   tid						varchar2(36) not null,
   geometry       		mdsys.sdo_geometry,
   executor_id				number(38) not null,
   executor_tid			varchar2(36),
   executor_nome			varchar2(80) not null,
   executor_login			varchar2(30) not null,
   executor_tipo_id		number(38) not null,
   executor_tipo_texto	varchar2(30) not null,
   acao_executada			number(38) not null,
   data_execucao			timestamp(6) not null
);

comment on table hst_lagoa is 'Tabela do histórico de Lagoa.';
comment on column hst_lagoa.id is 'Chave primária de identificação da tabela.';
comment on column hst_lagoa.feature_id is 'Identificador original da tabela oficial.';
comment on column hst_lagoa.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column hst_lagoa.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column hst_lagoa.area_m2 is 'Área em metros quadrados da geometria.';
comment on column hst_lagoa.zona is 'Zona da Lagoa, podendo ser "U" para Urbano, "R" para Rural ou "A" para Abastecimento.';
comment on column hst_lagoa.nome is 'Nome da Lagoa.';
comment on column hst_lagoa.data is 'Data de inserção desse registro na tabela.';
comment on column hst_lagoa.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column hst_lagoa.geometry is 'Campo geométrico.';
comment on column hst_lagoa.executor_id is 'Chave estrangeira para tab_funcionario no esquema oficial. Campo(ID).';
comment on column hst_lagoa.executor_tid is 'Referência ao campo (TID) da tabela tab_funcionario no esquema oficial.';
comment on column hst_lagoa.executor_nome is 'Nome do funcionário que executou a ação.';
comment on column hst_lagoa.executor_login is 'Login do funcionário que executou a ação.';
comment on column hst_lagoa.executor_tipo_id is 'Chave estrangeira para lov_executor_tipo no esquema oficial. Campo(ID).';
comment on column hst_lagoa.executor_tipo_texto is 'Texto do tipo do funcionário que executou a ação.';
comment on column hst_lagoa.acao_executada is 'Ação que foi disparada pelo sistema a qual gerou essa linha de histórico.';
comment on column hst_lagoa.data_execucao is 'Data que foi gerada essa linha de histórico.';


-- Areas Inundadas de Represas
create sequence SEQ_HST_REPRESA;

create table HST_REPRESA(
   id number(10)  		constraint pk_hst_represa primary key,
   feature_id				number(10),
   projeto  				number(38) not null,
   cod_apmp  				number(10),
   area_m2        		number,
   amortecimento  		number,
   nome           		varchar2(100),
   data						date,
   tid						varchar2(36) not null,
   geometry       		mdsys.sdo_geometry,
   executor_id				number(38) not null,
   executor_tid			varchar2(36),
   executor_nome			varchar2(80) not null,
   executor_login			varchar2(30) not null,
   executor_tipo_id		number(38) not null,
   executor_tipo_texto	varchar2(30) not null,
   acao_executada			number(38) not null,
   data_execucao			timestamp(6) not null
);

comment on table hst_represa is 'Tabela do histórico de Represa.';
comment on column hst_represa.id is 'Chave primária de identificação da tabela.';
comment on column hst_represa.feature_id is 'Identificador original da tabela oficial.';
comment on column hst_represa.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column hst_represa.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column hst_represa.area_m2 is 'Área em metros quadrados da geometria.';
comment on column hst_represa.amortecimento is 'Raio de buffer para geração de APP.';
comment on column hst_represa.nome is 'Nome da Represa.';
comment on column hst_represa.data is 'Data de inserção desse registro na tabela.';
comment on column hst_represa.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column hst_represa.geometry is 'Campo geométrico.';
comment on column hst_represa.executor_id is 'Chave estrangeira para tab_funcionario no esquema oficial. Campo(ID).';
comment on column hst_represa.executor_tid is 'Referência ao campo (TID) da tabela tab_funcionario no esquema oficial.';
comment on column hst_represa.executor_nome is 'Nome do funcionário que executou a ação.';
comment on column hst_represa.executor_login is 'Login do funcionário que executou a ação.';
comment on column hst_represa.executor_tipo_id is 'Chave estrangeira para lov_executor_tipo no esquema oficial. Campo(ID).';
comment on column hst_represa.executor_tipo_texto is 'Texto do tipo do funcionário que executou a ação.';
comment on column hst_represa.acao_executada is 'Ação que foi disparada pelo sistema a qual gerou essa linha de histórico.';
comment on column hst_represa.data_execucao is 'Data que foi gerada essa linha de histórico.';



-- Areas de Duna
create sequence SEQ_HST_DUNA;

create table HST_DUNA(
   id number(10)  		constraint pk_hst_duna primary key,
   feature_id				number(10),
   projeto  				number(38) not null,
   data						date,
   tid						varchar2(36) not null,
   geometry       		mdsys.sdo_geometry,
   executor_id				number(38) not null,
   executor_tid			varchar2(36),
   executor_nome			varchar2(80) not null,
   executor_login			varchar2(30) not null,
   executor_tipo_id		number(38) not null,
   executor_tipo_texto	varchar2(30) not null,
   acao_executada			number(38) not null,
   data_execucao			timestamp(6) not null
);

comment on table hst_duna is 'Tabela do histórico de Duna.';
comment on column hst_duna.id is 'Chave primária de identificação da tabela.';
comment on column hst_duna.feature_id is 'Identificador original da tabela oficial.';
comment on column hst_duna.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column hst_duna.data is 'Data de inserção desse registro na tabela.';
comment on column hst_duna.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column hst_duna.geometry is 'Campo geométrico.';
comment on column hst_duna.executor_id is 'Chave estrangeira para tab_funcionario no esquema oficial. Campo(ID).';
comment on column hst_duna.executor_tid is 'Referência ao campo (TID) da tabela tab_funcionario no esquema oficial.';
comment on column hst_duna.executor_nome is 'Nome do funcionário que executou a ação.';
comment on column hst_duna.executor_login is 'Login do funcionário que executou a ação.';
comment on column hst_duna.executor_tipo_id is 'Chave estrangeira para lov_executor_tipo no esquema oficial. Campo(ID).';
comment on column hst_duna.executor_tipo_texto is 'Texto do tipo do funcionário que executou a ação.';
comment on column hst_duna.acao_executada is 'Ação que foi disparada pelo sistema a qual gerou essa linha de histórico.';
comment on column hst_duna.data_execucao is 'Data que foi gerada essa linha de histórico.';


-- Restrição de declividade
create sequence SEQ_HST_REST_DECLIVIDADE;

create table HST_REST_DECLIVIDADE(
   id number(10)  		constraint pk_hst_rest_declividade primary key,
   feature_id				number(10),
   projeto  				number(38) not null,
   data						date,
   tid						varchar2(36) not null,
   geometry       		mdsys.sdo_geometry,
   executor_id				number(38) not null,
   executor_tid			varchar2(36),
   executor_nome			varchar2(80) not null,
   executor_login			varchar2(30) not null,
   executor_tipo_id		number(38) not null,
   executor_tipo_texto	varchar2(30) not null,
   acao_executada			number(38) not null,
   data_execucao			timestamp(6) not null
);

comment on table hst_rest_declividade is 'Tabela do histórico de Restrição de Declividade.';
comment on column hst_rest_declividade.id is 'Chave primária de identificação da tabela.';
comment on column hst_rest_declividade.feature_id is 'Identificador original da tabela oficial.';
comment on column hst_rest_declividade.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column hst_rest_declividade.data is 'Data de inserção desse registro na tabela.';
comment on column hst_rest_declividade.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column hst_rest_declividade.geometry is 'Campo geométrico.';
comment on column hst_rest_declividade.executor_id is 'Chave estrangeira para tab_funcionario no esquema oficial. Campo(ID).';
comment on column hst_rest_declividade.executor_tid is 'Referência ao campo (TID) da tabela tab_funcionario no esquema oficial.';
comment on column hst_rest_declividade.executor_nome is 'Nome do funcionário que executou a ação.';
comment on column hst_rest_declividade.executor_login is 'Login do funcionário que executou a ação.';
comment on column hst_rest_declividade.executor_tipo_id is 'Chave estrangeira para lov_executor_tipo no esquema oficial. Campo(ID).';
comment on column hst_rest_declividade.executor_tipo_texto is 'Texto do tipo do funcionário que executou a ação.';
comment on column hst_rest_declividade.acao_executada is 'Ação que foi disparada pelo sistema a qual gerou essa linha de histórico.';
comment on column hst_rest_declividade.data_execucao is 'Data que foi gerada essa linha de histórico.';


-- Escarpa
create sequence SEQ_HST_ESCARPA;

create table HST_ESCARPA(
   id number(10)  		constraint pk_hst_escarpa primary key,
   feature_id				number(10),
   projeto  				number(38) not null,
   data						date,
   tid						varchar2(36) not null,
   geometry       		mdsys.sdo_geometry,
   executor_id				number(38) not null,
   executor_tid			varchar2(36),
   executor_nome			varchar2(80) not null,
   executor_login			varchar2(30) not null,
   executor_tipo_id		number(38) not null,
   executor_tipo_texto	varchar2(30) not null,
   acao_executada			number(38) not null,
   data_execucao			timestamp(6) not null
);

comment on table hst_escarpa is 'Tabela do histórico de Escarpa.';
comment on column hst_escarpa.id is 'Chave primária de identificação da tabela.';
comment on column hst_escarpa.feature_id is 'Identificador original da tabela oficial.';
comment on column hst_escarpa.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column hst_escarpa.data is 'Data de inserção desse registro na tabela.';
comment on column hst_escarpa.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column hst_escarpa.geometry is 'Campo geométrico.';
comment on column hst_escarpa.executor_id is 'Chave estrangeira para tab_funcionario no esquema oficial. Campo(ID).';
comment on column hst_escarpa.executor_tid is 'Referência ao campo (TID) da tabela tab_funcionario no esquema oficial.';
comment on column hst_escarpa.executor_nome is 'Nome do funcionário que executou a ação.';
comment on column hst_escarpa.executor_login is 'Login do funcionário que executou a ação.';
comment on column hst_escarpa.executor_tipo_id is 'Chave estrangeira para lov_executor_tipo no esquema oficial. Campo(ID).';
comment on column hst_escarpa.executor_tipo_texto is 'Texto do tipo do funcionário que executou a ação.';
comment on column hst_escarpa.acao_executada is 'Ação que foi disparada pelo sistema a qual gerou essa linha de histórico.';
comment on column hst_escarpa.data_execucao is 'Data que foi gerada essa linha de histórico.';



----------------------------------------------------
-- Feições de Atividade

-- Ponto da Atividade
create sequence SEQ_HST_PATIV;

create table HST_PATIV(
   id number(10)  		constraint pk_hst_pativ primary key,
   feature_id				number(10),
   projeto  				number(38) not null,
   cod_apmp  				number(10),
   codigo					varchar2(100),
   atividade      		varchar2(150),
   rocha						varchar2(1),
   massa_dagua				varchar2(1),
   avn						varchar2(1),
   aa							varchar2(1),
   afs						varchar2(1),
   floresta_plantada		varchar2(1),
   arl						varchar2(1),
   rppn						varchar2(1),
   app						varchar2(1),
   data						date,
   tid						varchar2(36) not null,
   geometry       		mdsys.sdo_geometry,
   executor_id				number(38) not null,
   executor_tid			varchar2(36),
   executor_nome			varchar2(80) not null,
   executor_login			varchar2(30) not null,
   executor_tipo_id		number(38) not null,
   executor_tipo_texto	varchar2(30) not null,
   acao_executada			number(38) not null,
   data_execucao			timestamp(6) not null
);

comment on table hst_pativ is 'Tabela do histórico de PATIV ( Ponto da Atividade ).';
comment on column hst_pativ.id is 'Chave primária de identificação da tabela.';
comment on column hst_pativ.feature_id is 'Identificador original da tabela oficial.';
comment on column hst_pativ.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column hst_pativ.cod_apmp is 'Referencia o ID da APMP que interage com essa geometria.';
comment on column hst_pativ.codigo is 'Codigo gerado automaticamente para identificação da PATIV.';
comment on column hst_pativ.atividade is 'Texto da atividade associada ao projeto geo.';
comment on column hst_pativ.rocha is 'Identifica se a geometria cruza com ROCHA. Assume "S" para sim e "N" para não.';
comment on column hst_pativ.massa_dagua is 'Identifica se a geometria cruza com MASSA_DAGUA. Assume "S" para sim e "N" para não.';
comment on column hst_pativ.avn is 'Identifica se a geometria cruza com AVN. Case não encontre relação assume "N" para não, caso contrário, assume a sigla do estágio da AVN que cruzou podendo ser "I", "M", "A" ou "D".';
comment on column hst_pativ.aa is 'Identifica se a geometria cruza com AA. Assume "S" para sim e "N" para não.';
comment on column hst_pativ.afs is 'Identifica se a geometria cruza com AFS. Assume "S" para sim e "N" para não.';
comment on column hst_pativ.floresta_plantada is 'Identifica se a geometria cruza com ACV de tipo "FLORESTA_PLANTADA". Assume "S" para sim e "N" para não.';
comment on column hst_pativ.arl is 'Identifica se a geometria cruza com ARL. Assume "S" para sim e "N" para não.';
comment on column hst_pativ.rppn is 'Identifica se a geometria cruza com RPPN. Assume "S" para sim e "N" para não.';
comment on column hst_pativ.app is 'Identifica se a geometria cruza com APP. Assume "S" para sim e "N" para não.';
comment on column hst_pativ.data is 'Data de inserção desse registro na tabela.';
comment on column hst_pativ.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column hst_pativ.geometry is 'Campo geométrico.';
comment on column hst_pativ.executor_id is 'Chave estrangeira para tab_funcionario no esquema oficial. Campo(ID).';
comment on column hst_pativ.executor_tid is 'Referência ao campo (TID) da tabela tab_funcionario no esquema oficial.';
comment on column hst_pativ.executor_nome is 'Nome do funcionário que executou a ação.';
comment on column hst_pativ.executor_login is 'Login do funcionário que executou a ação.';
comment on column hst_pativ.executor_tipo_id is 'Chave estrangeira para lov_executor_tipo no esquema oficial. Campo(ID).';
comment on column hst_pativ.executor_tipo_texto is 'Texto do tipo do funcionário que executou a ação.';
comment on column hst_pativ.acao_executada is 'Ação que foi disparada pelo sistema a qual gerou essa linha de histórico.';
comment on column hst_pativ.data_execucao is 'Data que foi gerada essa linha de histórico.';


-- Linha da Atividade
create sequence SEQ_HST_LATIV;

create table HST_LATIV(
   id number(10)  		constraint pk_hst_lativ primary key,
   feature_id				number(10),
   projeto  				number(38) not null,
   cod_apmp  				number(10),
   codigo					varchar2(100),
   atividade      		varchar2(150),
   rocha						varchar2(1),
   massa_dagua				varchar2(1),
   avn						varchar2(1),
   aa							varchar2(1),
   afs						varchar2(1),
   floresta_plantada		varchar2(1),
   arl						varchar2(1),
   rppn						varchar2(1),
   app						varchar2(1),
   comprimento				number,
   data						date,
   tid						varchar2(36) not null,
   geometry       		mdsys.sdo_geometry,
   executor_id				number(38) not null,
   executor_tid			varchar2(36),
   executor_nome			varchar2(80) not null,
   executor_login			varchar2(30) not null,
   executor_tipo_id		number(38) not null,
   executor_tipo_texto	varchar2(30) not null,
   acao_executada			number(38) not null,
   data_execucao			timestamp(6) not null
);

comment on table hst_lativ is 'Tabela do histórico de LATIV ( Linha da Atividade ).';
comment on column hst_lativ.id is 'Chave primária de identificação da tabela.';
comment on column hst_lativ.feature_id is 'Identificador original da tabela oficial.';
comment on column hst_lativ.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column hst_lativ.cod_apmp is 'Referencia o ID da APMP que interage com o ponto médio dessa geometria.';
comment on column hst_lativ.codigo is 'Codigo gerado automaticamente para identificação da LATIV.';
comment on column hst_lativ.atividade is 'Texto da atividade associada ao projeto geo.';
comment on column hst_lativ.rocha is 'Identifica se a geometria cruza atravez do seu ponto médio com ROCHA. Assume "S" para sim e "N" para não.';
comment on column hst_lativ.massa_dagua is 'Identifica se a geometria cruza atravez do seu ponto médio com MASSA_DAGUA. Assume "S" para sim e "N" para não.';
comment on column hst_lativ.avn is 'Identifica se a geometria cruza atravez do seu ponto médio com AVN. Case não encontre relação assume "N" para não, caso contrário, assume a sigla do estágio da AVN que cruzou podendo ser "I", "M", "A" ou "D".';
comment on column hst_lativ.aa is 'Identifica se a geometria cruza atravez do seu ponto médio com AA. Assume "S" para sim e "N" para não.';
comment on column hst_lativ.afs is 'Identifica se a geometria cruza atravez do seu ponto médio com AFS. Assume "S" para sim e "N" para não.';
comment on column hst_lativ.floresta_plantada is 'Identifica se a geometria cruza atravez do seu ponto médio com ACV de tipo "FLORESTA_PLANTADA". Assume "S" para sim e "N" para não.';
comment on column hst_lativ.arl is 'Identifica se a geometria cruza atravez do seu ponto médio com ARL. Assume "S" para sim e "N" para não.';
comment on column hst_lativ.rppn is 'Identifica se a geometria cruza atravez do seu ponto médio com RPPN. Assume "S" para sim e "N" para não.';
comment on column hst_lativ.app is 'Identifica se a geometria cruza atravez do seu ponto médio com APP. Assume "S" para sim e "N" para não.';
comment on column hst_lativ.comprimento is 'Comprimento em metros da geometria.';
comment on column hst_lativ.data is 'Data de inserção desse registro na tabela.';
comment on column hst_lativ.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column hst_lativ.geometry is 'Campo geométrico.';
comment on column hst_lativ.executor_id is 'Chave estrangeira para tab_funcionario no esquema oficial. Campo(ID).';
comment on column hst_lativ.executor_tid is 'Referência ao campo (TID) da tabela tab_funcionario no esquema oficial.';
comment on column hst_lativ.executor_nome is 'Nome do funcionário que executou a ação.';
comment on column hst_lativ.executor_login is 'Login do funcionário que executou a ação.';
comment on column hst_lativ.executor_tipo_id is 'Chave estrangeira para lov_executor_tipo no esquema oficial. Campo(ID).';
comment on column hst_lativ.executor_tipo_texto is 'Texto do tipo do funcionário que executou a ação.';
comment on column hst_lativ.acao_executada is 'Ação que foi disparada pelo sistema a qual gerou essa linha de histórico.';
comment on column hst_lativ.data_execucao is 'Data que foi gerada essa linha de histórico.';

-- Area da Atividade
create sequence SEQ_HST_AATIV;

create table HST_AATIV(
   id number(10)  		constraint pk_hst_aativ primary key,
   feature_id				number(10),
   projeto  				number(38) not null,
   cod_apmp  				number(10),
   codigo					varchar2(100),
   atividade      		varchar2(150),
   rocha						varchar2(1),
   massa_dagua				varchar2(1),
   avn						varchar2(1),
   aa							varchar2(1),
   afs						varchar2(1),
   floresta_plantada		varchar2(1),
   arl						varchar2(1),
   rppn						varchar2(1),
   app						varchar2(1),
   area_m2					number,
   data						date,
   tid						varchar2(36) not null,
   geometry       		mdsys.sdo_geometry,
   executor_id				number(38) not null,
   executor_tid			varchar2(36),
   executor_nome			varchar2(80) not null,
   executor_login			varchar2(30) not null,
   executor_tipo_id		number(38) not null,
   executor_tipo_texto	varchar2(30) not null,
   acao_executada			number(38) not null,
   data_execucao			timestamp(6) not null
);

comment on table hst_aativ is 'Tabela do histórico de AATIV ( Área da Atividade ).';
comment on column hst_aativ.id is 'Chave primária de identificação da tabela.';
comment on column hst_aativ.feature_id is 'Identificador original da tabela oficial.';
comment on column hst_aativ.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column hst_aativ.cod_apmp is 'Referencia o ID da APMP que interage com o ponto ideal dessa geometria.';
comment on column hst_aativ.codigo is 'Codigo gerado automaticamente para identificação da AATIV.';
comment on column hst_aativ.atividade is 'Texto da atividade associada ao projeto geo.';
comment on column hst_aativ.rocha is 'Identifica se a geometria cruza atravez do seu ponto ideal com ROCHA. Assume "S" para sim e "N" para não.';
comment on column hst_aativ.massa_dagua is 'Identifica se a geometria cruza atravez do seu ponto ideal com MASSA_DAGUA. Assume "S" para sim e "N" para não.';
comment on column hst_aativ.avn is 'Identifica se a geometria cruza atravez do seu ponto ideal com AVN. Case não encontre relação assume "N" para não, caso contrário, assume a sigla do estágio da AVN que cruzou podendo ser "I", "M", "A" ou "D".';
comment on column hst_aativ.aa is 'Identifica se a geometria cruza atravez do seu ponto ideal com AA. Assume "S" para sim e "N" para não.';
comment on column hst_aativ.afs is 'Identifica se a geometria cruza atravez do seu ponto ideal com AFS. Assume "S" para sim e "N" para não.';
comment on column hst_aativ.floresta_plantada is 'Identifica se a geometria cruza atravez do seu ponto ideal com ACV de tipo "FLORESTA_PLANTADA". Assume "S" para sim e "N" para não.';
comment on column hst_aativ.arl is 'Identifica se a geometria cruza atravez do seu ponto ideal com ARL. Assume "S" para sim e "N" para não.';
comment on column hst_aativ.rppn is 'Identifica se a geometria cruza atravez do seu ponto ideal com RPPN. Assume "S" para sim e "N" para não.';
comment on column hst_aativ.app is 'Identifica se a geometria cruza atravez do seu ponto ideal com APP. Assume "S" para sim e "N" para não.';
comment on column hst_aativ.area_m2 is 'Área em metros quadrados da geometria.';
comment on column hst_aativ.data is 'Data de inserção desse registro na tabela.';
comment on column hst_aativ.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column hst_aativ.geometry is 'Campo geométrico.';
comment on column hst_aativ.executor_id is 'Chave estrangeira para tab_funcionario no esquema oficial. Campo(ID).';
comment on column hst_aativ.executor_tid is 'Referência ao campo (TID) da tabela tab_funcionario no esquema oficial.';
comment on column hst_aativ.executor_nome is 'Nome do funcionário que executou a ação.';
comment on column hst_aativ.executor_login is 'Login do funcionário que executou a ação.';
comment on column hst_aativ.executor_tipo_id is 'Chave estrangeira para lov_executor_tipo no esquema oficial. Campo(ID).';
comment on column hst_aativ.executor_tipo_texto is 'Texto do tipo do funcionário que executou a ação.';
comment on column hst_aativ.acao_executada is 'Ação que foi disparada pelo sistema a qual gerou essa linha de histórico.';
comment on column hst_aativ.data_execucao is 'Data que foi gerada essa linha de histórico.';


-- Area de Influencia da Atividade
create sequence SEQ_HST_AIATIV;

create table HST_AIATIV(
   id number(10)  		constraint pk_hst_aiativ primary key,
   feature_id				number(10),
   projeto  				number(38) not null,
   cod_apmp  				number(10),
   codigo					varchar2(100),
   atividade      		varchar2(150),
   rocha						varchar2(1),
   massa_dagua				varchar2(1),
   avn						varchar2(1),
   aa							varchar2(1),
   afs						varchar2(1),
   floresta_plantada		varchar2(1),
   arl						varchar2(1),
   rppn						varchar2(1),
   app						varchar2(1),
   area_m2					number,
   data						date,
   tid						varchar2(36) not null,
   geometry       		mdsys.sdo_geometry,
   executor_id				number(38) not null,
   executor_tid			varchar2(36),
   executor_nome			varchar2(80) not null,
   executor_login			varchar2(30) not null,
   executor_tipo_id		number(38) not null,
   executor_tipo_texto	varchar2(30) not null,
   acao_executada			number(38) not null,
   data_execucao			timestamp(6) not null
);

comment on table hst_aiativ is 'Tabela do histórico de AIATIV ( Área de Influência da Atividade ).';
comment on column hst_aiativ.id is 'Chave primária de identificação da tabela.';
comment on column hst_aiativ.feature_id is 'Identificador original da tabela oficial.';
comment on column hst_aiativ.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column hst_aiativ.cod_apmp is 'Referencia o ID da APMP que interage com o ponto ideal dessa geometria.';
comment on column hst_aiativ.codigo is 'Codigo gerado automaticamente para identificação da AIATIV.';
comment on column hst_aiativ.atividade is 'Texto da atividade associada ao projeto geo.';
comment on column hst_aiativ.rocha is 'Identifica se a geometria cruza atravez do seu ponto ideal com ROCHA. Assume "S" para sim e "N" para não.';
comment on column hst_aiativ.massa_dagua is 'Identifica se a geometria cruza atravez do seu ponto ideal com MASSA_DAGUA. Assume "S" para sim e "N" para não.';
comment on column hst_aiativ.avn is 'Identifica se a geometria cruza atravez do seu ponto ideal com AVN. Case não encontre relação assume "N" para não, caso contrário, assume a sigla do estágio da AVN que cruzou podendo ser "I", "M", "A" ou "D".';
comment on column hst_aiativ.aa is 'Identifica se a geometria cruza atravez do seu ponto ideal com AA. Assume "S" para sim e "N" para não.';
comment on column hst_aiativ.afs is 'Identifica se a geometria cruza atravez do seu ponto ideal com AFS. Assume "S" para sim e "N" para não.';
comment on column hst_aiativ.floresta_plantada is 'Identifica se a geometria cruza atravez do seu ponto ideal com ACV de tipo "FLORESTA_PLANTADA". Assume "S" para sim e "N" para não.';
comment on column hst_aiativ.arl is 'Identifica se a geometria cruza atravez do seu ponto ideal com ARL. Assume "S" para sim e "N" para não.';
comment on column hst_aiativ.rppn is 'Identifica se a geometria cruza atravez do seu ponto ideal com RPPN. Assume "S" para sim e "N" para não.';
comment on column hst_aiativ.app is 'Identifica se a geometria cruza atravez do seu ponto ideal com APP. Assume "S" para sim e "N" para não.';
comment on column hst_aiativ.area_m2 is 'Área em metros quadrados da geometria.';
comment on column hst_aiativ.data is 'Data de inserção desse registro na tabela.';
comment on column hst_aiativ.tid  is 'Id Transacional. Esse valor garante a ligação entre todas as tabelas relacionadas com essa transaction.';
comment on column hst_aiativ.geometry is 'Campo geométrico.';
comment on column hst_aiativ.executor_id is 'Chave estrangeira para tab_funcionario no esquema oficial. Campo(ID).';
comment on column hst_aiativ.executor_tid is 'Referência ao campo (TID) da tabela tab_funcionario no esquema oficial.';
comment on column hst_aiativ.executor_nome is 'Nome do funcionário que executou a ação.';
comment on column hst_aiativ.executor_login is 'Login do funcionário que executou a ação.';
comment on column hst_aiativ.executor_tipo_id is 'Chave estrangeira para lov_executor_tipo no esquema oficial. Campo(ID).';
comment on column hst_aiativ.executor_tipo_texto is 'Texto do tipo do funcionário que executou a ação.';
comment on column hst_aiativ.acao_executada is 'Ação que foi disparada pelo sistema a qual gerou essa linha de histórico.';
comment on column hst_aiativ.data_execucao is 'Data que foi gerada essa linha de histórico.';






----------------------------------------------------
-- Tabelas de Fila e Validacao
----------------------------------------------------

create table lov_fila_etapa(
   id		number(38) constraint pk_fila_etapa primary key,
   texto varchar2(50)
);

comment on table lov_fila_etapa is 'Lista das etapas de processamento.';
comment on column lov_fila_etapa.id is 'Chave primária de identificação da tabela.';
comment on column lov_fila_etapa.texto is 'Texto com o nome da etapa de processamento da fila.';

insert into lov_fila_etapa values(1, 'Validação');
insert into lov_fila_etapa values(2, 'Processamento');
insert into lov_fila_etapa values(3, 'Geração de PDF');



create table lov_fila_situacao(
   id		number(38) constraint pk_fila_situacao primary key,
   texto varchar2(50)
);

comment on table lov_fila_situacao is 'Lista das situações da fila.';
comment on column lov_fila_situacao.id is 'Chave primária de identificação da tabela.';
comment on column lov_fila_situacao.texto is 'Texto com o nome da situação da fila.';

insert into lov_fila_situacao values(1, 'Aguardando');
insert into lov_fila_situacao values(2, 'Executando');
insert into lov_fila_situacao values(3, 'Erro');
insert into lov_fila_situacao values(4, 'Concluído');
insert into lov_fila_situacao values(5, 'Cancelado');



create table lov_fila_tipo(
   id		number(38) constraint pk_fila_tipo primary key,
   texto varchar2(50)
);

comment on table lov_fila_tipo is 'Lista do tipo da fila.';
comment on column lov_fila_tipo.id is 'Chave primária de identificação da tabela.';
comment on column lov_fila_tipo.texto is 'Texto com o nome do tipo da fila.';

insert into lov_fila_tipo values(1, 'Base de Referência Interna');
insert into lov_fila_tipo values(2, 'Base de Referência GEOBASES');
insert into lov_fila_tipo values(3, 'Dominialidade');
insert into lov_fila_tipo values(4, 'Atividade');


create table lov_validacao_geo_tipo(
   id		number(38) constraint pk_validacao_geo_tipo primary key,
   texto varchar2(50)
);

comment on table lov_validacao_geo_tipo is 'Lista do tipo das mensagens de validação que constam no relatório.';
comment on column lov_validacao_geo_tipo.id is 'Chave primária de identificação da tabela.';
comment on column lov_validacao_geo_tipo.texto is 'Texto com o nome do tipo das mensagens de validação que constam no relatório.';

insert into lov_validacao_geo_tipo values(1, 'erro espacial');
insert into lov_validacao_geo_tipo values(2, 'obrigatoriedade nao atendida');
insert into lov_validacao_geo_tipo values(3, 'atributo invalido');
insert into lov_validacao_geo_tipo values(4, 'contabilizacao de geometrias');


create table tab_fila(
   id 						number(38) constraint pk_fila primary key,
   empreendimento 		number(38),
   projeto     			number(38),
   tipo        			number(38) constraint fk_lov_fila_tipo references lov_fila_tipo (id),
   mecanismo_elaboracao	number(38),
   etapa       			number(38) constraint fk_lov_fila_etapa references lov_fila_etapa (id),
   situacao    			number(38) constraint fk_lov_fila_situacao references lov_fila_situacao (id),
   data_fila   			date,
   data_inicio 			date,
   data_fim    			date
);

comment on table tab_fila is 'Tabela que controla a fila de pedidos e processamento usada pelo serviço windows de processamento geo.';
comment on column tab_fila.id is 'Chave primária de identificação da tabela.';
comment on column tab_fila.empreendimento is 'Referencia o ID do empreendimento na tabela tab_empreendimento no esquema oficial.';
comment on column tab_fila.projeto is 'Referencia o ID do projeto geo na tabela crt_projeto_geo no esquema oficial.';
comment on column tab_fila.tipo is 'Chave estrangeira para lov_fila_tipo. Campo(ID).';
comment on column tab_fila.mecanismo_elaboracao is 'Referencia o ID do projeto geo na tabela lov_crt_projeto_geo_mecanismo no esquema oficial.';
comment on column tab_fila.etapa is 'Chave estrangeira para lov_fila_etapa. Campo(ID).';
comment on column tab_fila.situacao is 'Chave estrangeira para lov_fila_situacao. Campo(ID).';
comment on column tab_fila.data_fila is 'Data de entrada na fila. A ordem de execução é baseada nesta coluna. Os pedidos mais antigos são executados primeiro.';
comment on column tab_fila.data_inicio is 'Data de inicio da execução. Data em que o windows service de processamento geo inicia o processamento.';
comment on column tab_fila.data_fim is 'Data de fim da execução. Data em que o windows service de processamento geo finaliza o processamento.';

create sequence seq_fila;

create table tab_validacao_geo(
   projeto     			number(38),
   tipo						number(38) constraint fk_lov_validacao_geo_tipo references lov_validacao_geo_tipo(id),
   sigla_tabela			varchar2(50),
   nome_tabela	   		varchar2(100),
   codigo_mensagem		varchar2(100),
   descricao_mensagem	varchar2(400)
);

comment on table tab_validacao_geo is 'Lista de mensagens gerada pela etapa de validação que constarão no relatório de importação.';
comment on column tab_validacao_geo.tipo is 'Chave estrangeira para lov_validacao_geo_tipo. Campo(ID).';
comment on column tab_validacao_geo.sigla_tabela is 'Sigla da feição a que se aplica a mensagem.';
comment on column tab_validacao_geo.nome_tabela is 'Descrição da sigla da feição a que se aplica a mensagem.';
comment on column tab_validacao_geo.codigo_mensagem is 'Código identificador da mensagem. Coluna criada para uso futuro na padronização das mensagens do sistema.';
comment on column tab_validacao_geo.descricao_mensagem is 'Descrição da mensagem que atualmente aparece no relatório de importação, sendo futuramente substituída pelo codigo da mensagem.';


create table cnf_sistema_geo
(
  campo varchar2(80) not null unique,
  valor varchar2(4000)
);

-- comentarios
comment on table cnf_sistema_geo is 'Tabela de configurações de valores usados pelo sistema geo';
comment on column cnf_sistema_geo.campo is 'Nome do campo que será guardada o valor de configuração.';
comment on column cnf_sistema_geo.valor is 'Valor do campo de configuração.';

insert into cnf_sistema_geo(campo, valor) values('ESQUEMA_OFICIAL', 'IDAF');
insert into cnf_sistema_geo(campo, valor) values('SERVICO_GEO_-_INTERVALO_DO_TIMER', '10');
insert into cnf_sistema_geo(campo, valor) values('SERVICO_GEO_-_MAXIMA_DURACAO_DO_PROCESSO', '3600');
insert into cnf_sistema_geo(campo, valor) values('SERVICO_GEO_-_CONFIGURACOES_DA_FILA', '0.0');
insert into cnf_sistema_geo(campo, valor) values('SERVICO_GEO_-_DIRETORIO_DE_ARQUIVOS', 'D:\arquivos\');

insert into cnf_sistema_geo(campo, valor) values('SERVICO_GEO_-_BASEREF_GEOBASES_-_FEATURE_NAMES', 'LIM_MUNICIPIO15,LIM_TERRA_INDIGENA,HID_BACIA_HIDROGRAFICA,ENC_TRECHO_ENERGIA,HID_TRECHO_DRENAGEM,HID_TRECHO_MASSA_DAGUA,LIM_LIMITE_URBANO,LIM_AREA_ESPECIAL,LIM_OUTRAS_UNID_PROTEGIDAS,LIM_UNIDADE_CONSERV_NAO_SNUC,LIM_UNIDADE_PROTECAO_INTEGRAL,LIM_UNIDADE_USO_SUSTENTAVEL,LIM_UNIDADES_CONSERV_ARACRUZ,REL_CURVA_NIVEL,REL_ROCHA,TRA_TRECHO_DUTO,TRA_TRECHO_FERROVIARIO,TRA_TRECHO_RODOVIARIO,VEG_USO_DO_SOLO');
insert into cnf_sistema_geo(campo, valor) values('SERVICO_GEO_-_BASEREF_GEOBASES_-_FEATURE_TYPES', 'MultiPoligono,MultiPoligono,MultiPoligono,MultiLinha,MultiLinha,MultiPoligono,MultiPoligono,MultiPoligono,MultiPoligono,MultiPoligono,MultiPoligono,MultiPoligono,MultiPoligono,MultiLinha,MultiPoligono,MultiLinha,MultiLinha,MultiLinha,MultiPoligono');
insert into cnf_sistema_geo(campo, valor) values('SERVICO_GEO_-_BASEREF_GEOBASES_-_FEATURE_ALIASES', 'LIM_MUNICIPIO15,LIM_TERRA_INDIGENA,HID_BACIA_HIDROGRAFICA,ENC_TRECHO_ENERGIA,HID_TRECHO_DRENAGEM,HID_TRECHO_MASSA_DAGUA,LIM_LIMITE_URBANO,LIM_AREA_ESPECIAL,LIM_OUTRAS_UNID_PROTEGIDAS,LIM_UNIDADE_CONSERV_NAO_SNUC,LIM_UNIDADE_PROTECAO_INTEGRAL,LIM_UNIDADE_USO_SUSTENTAVEL,LIM_UNIDADES_CONSERV_ARACRUZ,REL_CURVA_NIVEL,REL_ROCHA,TRA_TRECHO_DUTO,TRA_TRECHO_FERROVIARIO,TRA_TRECHO_RODOVIARIO,VEG_USO_DO_SOLO');
insert into cnf_sistema_geo(campo, valor) values('SERVICO_GEO_-_BASEREF_GEOBASES_-_CONNECTION_KEY', 'geobases');

insert into cnf_sistema_geo(campo, valor) values('SERVICO_GEO_-_BASEREF_INTERNO_-_FEATURE_NAMES', 'TIT_ATP,TIT_ARL,GEO_ATP,GEO_ARL');
insert into cnf_sistema_geo(campo, valor) values('SERVICO_GEO_-_BASEREF_INTERNO_-_FEATURE_TYPES', 'MultiPoligono,MultiPoligono,MultiPoligono,MultiPoligono');
insert into cnf_sistema_geo(campo, valor) values('SERVICO_GEO_-_BASEREF_INTERNO_-_FEATURE_ALIASES', 'ATP,ARL,ATP_ANDAMENTO,ARL_ANDAMENTO');
insert into cnf_sistema_geo(campo, valor) values('SERVICO_GEO_-_BASEREF_INTERNO_-_CONNECTION_KEY', 'default');

prompt
prompt -------------------------------------------------
prompt LOV_OBRIGATORIEDADES_OPERACAO
prompt -------------------------------------------------

create table lov_obrigatoriedades_operacao
(
       id number(10),
       TEXTO varchar2(30)
);

alter table lov_obrigatoriedades_operacao add constraint PK_lov_obrig primary key (ID);
comment on table lov_obrigatoriedades_operacao is 'Tabela utilizada pela tab_feicao_col_obrigator para descrever a operação de comparação de colunas';
comment on column lov_obrigatoriedades_operacao.id  is 'Chave primária para lov_obrigatoriedades_operacao.';
comment on column lov_obrigatoriedades_operacao.id  is 'Valor da operação para a tabela tab_feicao_col_obrigator.operacao.';

prompt
prompt -------------------------------------------------
prompt TAB_FEICAO_COL_OBRIGATOR
prompt -------------------------------------------------

create table tab_feicao_col_obrigator
(
       FEICAO number(38),
       COLUNA varchar2(30),
       OPERACAO number(2),
       VALOR varchar2(50),
       COLUNA_OBRIGADA varchar2(30)
);

alter table tab_feicao_col_obrigator add constraint FK_FEI_COL_OBRIG_FEIC foreign key (FEICAO) references TAB_FEICAO (ID);
alter table tab_feicao_col_obrigator add constraint FK_FEI_COL_OBRIG_OPER foreign key (OPERACAO) references lov_obrigatoriedades_operacao (ID);
alter table TAB_FEICAO_COL_OBRIGATOR add constraint UN_TB_FEIC_COL_OBR unique (FEICAO, COLUNA);

comment on table tab_feicao_col_obrigator is 'Tabela utilizada pela tab_feicao_coluna para determinar a obrigatoriedade condicional de colunas';
comment on column tab_feicao_col_obrigator.feicao  is 'Chave estrangeira para tab_feicao.id. Feição que terá uma obrigação condicional de coluna.';
comment on column tab_feicao_col_obrigator.coluna  is 'Chave estrangeira para tab_feicao_coluna.coluna. Coluna da feição que ocasionará a obrigação de outra coluna.';
comment on column tab_feicao_col_obrigator.operacao  is 'Chave estrangeira para lov_obrigatoriedades_operacao.id';
comment on column tab_feicao_col_obrigator.valor  is 'Valor escolhido da coluna que irá obrigar outra coluna ser obrigatória';
comment on column tab_feicao_col_obrigator.coluna_obrigada  is 'Coluna estrangeira para tab_feicao_coluna.coluna - Coluna que passará a ser obrigatória';

prompt
prompt -------------------------------------------------
prompt TAB_NAVEGADOR_CAMADA
prompt -------------------------------------------------
                
create table tab_navegador_camada
(
       navegador number(10),
       servico number(10)
);            
                
alter table tab_navegador_camada add constraint FK_NAVEGADOR_CAMADA_NAV foreign key (navegador) references TAB_NAVEGADOR (ID);
alter table tab_navegador_camada add constraint FK_NAVEGADOR_CAMADA_SERV foreign key (servico) references TAB_SERVICO (ID);

comment on table tab_navegador_camada is 'Tabela de serviços de camada do navegador';
comment on column tab_navegador_camada.navegador  is 'Chave estrangeira para tab_navegador.id. ';
comment on column tab_navegador_camada.servico  is 'Chave estrangeira para tab_servico.id. Serviço de camada para o navegador';

commit;

prompt -------------------------------------------------

----------------------------------------------------------------------------

set feedback on
set define on
