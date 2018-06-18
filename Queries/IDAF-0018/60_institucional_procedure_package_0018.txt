create or replace PACKAGE "HISTORICO" is
	---------------------------------------------------------
	-- Funcionário
	---------------------------------------------------------
	procedure funcionario(p_id               number,
						  p_acao             number,
						  p_executor_id      number,
						  p_executor_nome    varchar2,
						  p_executor_login   varchar2,
						  p_executor_tipo_id number,
						  p_executor_tid     varchar2);
	---------------------------------------------------------

	---------------------------------------------------------
	-- Atividade de Processo/Documento/Requerimento
	---------------------------------------------------------
	procedure atividade(p_id               number,
						p_acao             number,
						p_executor_id      number,
						p_executor_nome    varchar2,
						p_executor_login   varchar2,
						p_executor_tipo_id number,
						p_executor_tid     varchar2);
	---------------------------------------------------------

	---------------------------------------------------------
	-- Arquivos do sistema
	---------------------------------------------------------
	procedure arquivo(p_id               number,
					  p_acao             number,
					  p_executor_id      number,
					  p_executor_nome    varchar2,
					  p_executor_login   varchar2,
					  p_executor_tipo_id number,
					  p_executor_tid     varchar2);
	---------------------------------------------------------

	---------------------------------------------------------
	-- Funcionário
	---------------------------------------------------------
	procedure pessoa(p_id               number,
					 p_acao             number,
					 p_executor_id      number,
					 p_executor_nome    varchar2,
					 p_executor_login   varchar2,
					 p_executor_tipo_id number,
					 p_executor_tid     varchar2);
	---------------------------------------------------------
  ---------------------------------------------------------
	-- Funcionário
	---------------------------------------------------------
  procedure penalidadeinfracao(p_id               number,
						   p_acao             number,
						   p_executor_id      number,
						   p_executor_nome    varchar2,
						   p_executor_login   varchar2,
						   p_executor_tipo_id number,
						   p_executor_tid     varchar2);

	---------------------------------------------------------
	-- Roteiro
	---------------------------------------------------------
	procedure roteiro(p_id               number,
					  p_acao             number,
					  p_executor_id      number,
					  p_executor_nome    varchar2,
					  p_executor_login   varchar2,
					  p_executor_tipo_id number,
					  p_executor_tid     varchar2);
	---------------------------------------------------------

	---------------------------------------------------------
	-- Checagem de item de roteiro
	---------------------------------------------------------
	procedure checagem(p_id               number,
					   p_acao             number,
					   p_executor_id      number,
					   p_executor_nome    varchar2,
					   p_executor_login   varchar2,
					   p_executor_tipo_id number,
					   p_executor_tid     varchar2);
	---------------------------------------------------------

	---------------------------------------------------------
	-- Checagem de Pendência
	---------------------------------------------------------
	procedure checagempendencia(p_id               number,
								p_acao             number,
								p_executor_id      number,
								p_executor_nome    varchar2,
								p_executor_login   varchar2,
								p_executor_tipo_id number,
								p_executor_tid     varchar2);
	---------------------------------------------------------

	---------------------------------------------------------
	-- Empreendimento
	---------------------------------------------------------
	procedure empreendimento(p_id               number,
							 p_acao             number,
							 p_executor_id      number,
							 p_executor_nome    varchar2,
							 p_executor_login   varchar2,
							 p_executor_tipo_id number,
							 p_executor_tid     varchar2);
	---------------------------------------------------------

	---------------------------------------------------------
	-- Requerimento
	---------------------------------------------------------
	procedure requerimento(p_id               number,
						   p_acao             number,
						   p_executor_id      number,
						   p_executor_nome    varchar2,
						   p_executor_login   varchar2,
						   p_executor_tipo_id number,
						   p_executor_tid     varchar2);
	---------------------------------------------------------

	---------------------------------------------------------
	-- Protocolo
	---------------------------------------------------------
	procedure protocolo(p_id               number,
						p_acao             number,
						p_executor_id      number,
						p_executor_nome    varchar2,
						p_executor_login   varchar2,
						p_executor_tipo_id number,
						p_executor_tid     varchar2);
	---------------------------------------------------------

	---------------------------------------------------------
	-- Atividade de Protocolo
	---------------------------------------------------------
	procedure protocoloatividade(p_id               number,
								 p_acao             number,
								 p_executor_id      number,
								 p_executor_nome    varchar2,
								 p_executor_login   varchar2,
								 p_executor_tipo_id number,
								 p_executor_tid     varchar2);
	---------------------------------------------------------

	---------------------------------------------------------
	-- Apensar protocolo a outro
	---------------------------------------------------------
	procedure apensar(p_id               number,
					  p_acao             number,
					  p_executor_id      number,
					  p_executor_nome    varchar2,
					  p_executor_login   varchar2,
					  p_executor_tipo_id number,
					  p_executor_tid     varchar2);
	---------------------------------------------------------

	---------------------------------------------------------
	-- Juntar protocolo a outro
	---------------------------------------------------------
	procedure juntar(p_id               number,
					 p_acao             number,
					 p_executor_id      number,
					 p_executor_nome    varchar2,
					 p_executor_login   varchar2,
					 p_executor_tipo_id number,
					 p_executor_tid     varchar2);
	---------------------------------------------------------

	---------------------------------------------------------
	-- Análise de itens de processo/documento
	---------------------------------------------------------
	procedure analiseitens(p_id               number,
						   p_acao             number,
						   p_executor_id      number,
						   p_executor_nome    varchar2,
						   p_executor_login   varchar2,
						   p_executor_tipo_id number,
						   p_executor_tid     varchar2);
	---------------------------------------------------------

	---------------------------------------------------------
	-- Item de roteiro
	---------------------------------------------------------
	procedure itemroteiro(p_id               number,
						  p_acao             number,
						  p_executor_id      number,
						  p_executor_nome    varchar2,
						  p_executor_login   varchar2,
						  p_executor_tipo_id number,
						  p_executor_tid     varchar2);
	---------------------------------------------------------

	---------------------------------------------------------
	-- Tramitação
	---------------------------------------------------------
	procedure tramitacao(p_id               number,
						 p_acao             number,
						 p_executor_id      number,
						 p_executor_nome    varchar2,
						 p_executor_login   varchar2,
						 p_executor_tipo_id number,
						 p_executor_tid     varchar2);
	---------------------------------------------------------

	---------------------------------------------------------
	-- Tramitação Arquivo
	---------------------------------------------------------
	procedure tramitacaoarquivo(p_id               number,
								p_acao             number,
								p_executor_id      number,
								p_executor_nome    varchar2,
								p_executor_login   varchar2,
								p_executor_tipo_id number,
								p_executor_tid     varchar2);
	---------------------------------------------------------

	---------------------------------------------------------
	-- Setor
	---------------------------------------------------------
	procedure setor(p_id               number,
					p_acao             number,
					p_executor_id      number,
					p_executor_nome    varchar2,
					p_executor_login   varchar2,
					p_executor_tipo_id number,
					p_executor_tid     varchar2);
	---------------------------------------------------------

	---------------------------------------------------------
	-- Configuração de Tramitação de Setor
	---------------------------------------------------------

	procedure tramitacaosetor(p_id               number,
							  p_acao             number,
							  p_executor_id      number,
							  p_executor_nome    varchar2,
							  p_executor_login   varchar2,
							  p_executor_tipo_id number,
							  p_executor_tid     varchar2);

	---------------------------------------------------------

	---------------------------------------------------------
	-- Configuração de Motivo de Tramitação
	---------------------------------------------------------

	procedure tramitacaomotivo(p_id               number,
							   p_acao             number,
							   p_executor_id      number,
							   p_executor_nome    varchar2,
							   p_executor_login   varchar2,
							   p_executor_tipo_id number,
							   p_executor_tid     varchar2);

	---------------------------------------------------------   

	---------------------------------------------------------
	-- Papel
	---------------------------------------------------------

	procedure papel(p_id               number,
					p_acao             number,
					p_executor_id      number,
					p_executor_nome    varchar2,
					p_executor_login   varchar2,
					p_executor_tipo_id number,
					p_executor_tid     varchar2);

	---------------------------------------------------------

	---------------------------------------------------------
	-- Modelo de Título
	---------------------------------------------------------

	procedure titulomodelo(p_id               number,
						   p_acao             number,
						   p_executor_id      number,
						   p_executor_nome    varchar2,
						   p_executor_login   varchar2,
						   p_executor_tipo_id number,
						   p_executor_tid     varchar2);

	---------------------------------------------------------

	---------------------------------------------------------
	-- Título
	---------------------------------------------------------

	procedure titulo(p_id               number,
					 p_acao             number,
					 p_executor_id      number,
					 p_executor_nome    varchar2,
					 p_executor_login   varchar2,
					 p_executor_tipo_id number,
					 p_executor_tid     varchar2);

	---------------------------------------------------------

	---------------------------------------------------------
	-- Condicionante de Título
	---------------------------------------------------------

	procedure titulocondicionante(p_id               number,
								  p_acao             number,
								  p_executor_id      number,
								  p_executor_nome    varchar2,
								  p_executor_login   varchar2,
								  p_executor_tipo_id number,
								  p_executor_tid     varchar2);

	---------------------------------------------------------

	---------------------------------------------------------
	-- Descrição de Condicionante de Título
	---------------------------------------------------------

	procedure titulocondicionantedescricao(p_id               number,
										   p_acao             number,
										   p_executor_id      number,
										   p_executor_nome    varchar2,
										   p_executor_login   varchar2,
										   p_executor_tipo_id number,
										   p_executor_tid     varchar2);

	---------------------------------------------------------

	---------------------------------------------------------
	-- Entrega de Título
	---------------------------------------------------------

	procedure tituloentrega(p_id               number,
							p_acao             number,
							p_executor_id      number,
							p_executor_nome    varchar2,
							p_executor_login   varchar2,
							p_executor_tipo_id number,
							p_executor_tid     varchar2);

	---------------------------------------------------------

	---------------------------------------------------------
	-- Configuração de Atividades
	---------------------------------------------------------
	procedure cnfatividades(p_id               number,
							p_acao             number,
							p_executor_id      number,
							p_executor_nome    varchar2,
							p_executor_login   varchar2,
							p_executor_tipo_id number,
							p_executor_tid     varchar2);
	---------------------------------------------------------

	---------------------------------------------------------
	-- Peça Técnica
	---------------------------------------------------------
	procedure pecatecnica(p_id               number,
						  p_acao             number,
						  p_executor_id      number,
						  p_executor_nome    varchar2,
						  p_executor_login   varchar2,
						  p_executor_tipo_id number,
						  p_executor_tid     varchar2);
	---------------------------------------------------------

	---------------------------------------------------------
	-- Fiscalização
	---------------------------------------------------------
	procedure fiscalizacao(p_id               number,
						   p_acao             number,
						   p_executor_id      number,
						   p_executor_nome    varchar2,
						   p_executor_login   varchar2,
						   p_executor_tipo_id number,
						   p_executor_tid     varchar2);
	---------------------------------------------------------

	---------------------------------------------------------
	-- Configuração Fiscalização
	---------------------------------------------------------
	procedure configfiscalizacao(p_id               number,
								 p_acao             number,
								 p_executor_id      number,
								 p_executor_nome    varchar2,
								 p_executor_login   varchar2,
								 p_executor_tipo_id number,
								 p_executor_tid     varchar2);
	---------------------------------------------------------

	---------------------------------------------------------
	-- Configuração Fiscalização - Tipo Infração
	---------------------------------------------------------
	procedure tipoinfracao(p_id               number,
						   p_acao             number,
						   p_executor_id      number,
						   p_executor_nome    varchar2,
						   p_executor_login   varchar2,
						   p_executor_tipo_id number,
						   p_executor_tid     varchar2);
	---------------------------------------------------------

	---------------------------------------------------------
	-- Configuração Fiscalização - Item
	---------------------------------------------------------
	procedure iteminfracao(p_id               number,
						   p_acao             number,
						   p_executor_id      number,
						   p_executor_nome    varchar2,
						   p_executor_login   varchar2,
						   p_executor_tipo_id number,
						   p_executor_tid     varchar2);
	---------------------------------------------------------

	---------------------------------------------------------
	-- Configuração Fiscalização - Subitem
	---------------------------------------------------------
	procedure subiteminfracao(p_id               number,
							  p_acao             number,
							  p_executor_id      number,
							  p_executor_nome    varchar2,
							  p_executor_login   varchar2,
							  p_executor_tipo_id number,
							  p_executor_tid     varchar2);
	---------------------------------------------------------

	---------------------------------------------------------
	-- Configuração Fiscalização - Campo
	---------------------------------------------------------
	procedure campoinfracao(p_id               number,
							p_acao             number,
							p_executor_id      number,
							p_executor_nome    varchar2,
							p_executor_login   varchar2,
							p_executor_tipo_id number,
							p_executor_tid     varchar2);
	---------------------------------------------------------

	---------------------------------------------------------
	-- Configuração Fiscalização - Resposta
	---------------------------------------------------------
	procedure respostainfracao(p_id               number,
							   p_acao             number,
							   p_executor_id      number,
							   p_executor_nome    varchar2,
							   p_executor_login   varchar2,
							   p_executor_tipo_id number,
							   p_executor_tid     varchar2);
	---------------------------------------------------------

	---------------------------------------------------------
	-- Configuração Fiscalização - Pergunta
	---------------------------------------------------------
	procedure perguntainfracao(p_id               number,
							   p_acao             number,
							   p_executor_id      number,
							   p_executor_nome    varchar2,
							   p_executor_login   varchar2,
							   p_executor_tipo_id number,
							   p_executor_tid     varchar2);
	---------------------------------------------------------

---------------------------------------------------------
	-- Configuracao Fiscalizacao - Produto Apreendido
---------------------------------------------------------
procedure produtoapreendido(p_id               number,
							p_acao             number,
							p_executor_id      number,
							p_executor_nome    varchar2,
							p_executor_login   varchar2,
							p_executor_tipo_id number,
							p_executor_tid     varchar2);
  ---------------------------------------------------------

---------------------------------------------------------
	-- Configuracao Fiscalizacao - Código da Receita
---------------------------------------------------------
procedure codigoreceita(p_id     number,
							p_acao             number,
							p_executor_id      number,
							p_executor_nome    varchar2,
							p_executor_login   varchar2,
							p_executor_tipo_id number,
							p_executor_tid     varchar2);
  ---------------------------------------------------------
  
---------------------------------------------------------
	-- Configuracao Fiscalizacao - Destinação
---------------------------------------------------------
procedure fiscdestinacao(p_id               number,
							p_acao             number,
							p_executor_id      number,
							p_executor_nome    varchar2,
							p_executor_login   varchar2,
							p_executor_tipo_id number,
							p_executor_tid     varchar2);
  ---------------------------------------------------------

	---------------------------------------------------------
	-- Motosserra
	---------------------------------------------------------
	procedure motosserra(p_id               number,
						 p_acao             number,
						 p_executor_id      number,
						 p_executor_nome    varchar2,
						 p_executor_login   varchar2,
						 p_executor_tipo_id number,
						 p_executor_tid     varchar2);
	---------------------------------------------------------

	---------------------------------------------------------
	-- Ficha Fundiária
	---------------------------------------------------------
	procedure fichafundiaria(p_id               number,
							 p_acao             number,
							 p_executor_id      number,
							 p_executor_nome    varchar2,
							 p_executor_login   varchar2,
							 p_executor_tipo_id number,
							 p_executor_tid     varchar2);
	---------------------------------------------------------

	---------------------------------------------------------
	-- Acompanhamento da Fiscalização
	---------------------------------------------------------
	procedure acompanhamento(p_id               number,
							 p_acao             number,
							 p_executor_id      number,
							 p_executor_nome    varchar2,
							 p_executor_login   varchar2,
							 p_executor_tipo_id number,
							 p_executor_tid     varchar2);
	---------------------------------------------------------

	---------------------------------------------------------
	-- Cadastro Ambiental Rural - Solicitacao
	---------------------------------------------------------
	procedure carsolicitacao(p_id               number,
							 p_acao             number,
							 p_executor_id      number,
							 p_executor_nome    varchar2,
							 p_executor_login   varchar2,
							 p_executor_tipo_id number,
							 p_executor_tid     varchar2);
	---------------------------------------------------------

	---------------------------------------------------------
	-- Órgãos Parceiros/ Conveniados
	---------------------------------------------------------
	procedure orgaoparceiroconveniado(p_id               number,
									  p_acao             number,
									  p_executor_id      number,
									  p_executor_nome    varchar2,
									  p_executor_login   varchar2,
									  p_executor_tipo_id number,
									  p_executor_tid     varchar2);
	---------------------------------------------------------

	---------------------------------------------------------
	-- Profissão
	---------------------------------------------------------
	procedure profissao(p_id               number,
						p_acao             number,
						p_executor_id      number,
						p_executor_nome    varchar2,
						p_executor_login   varchar2,
						p_executor_tipo_id number,
						p_executor_tid     varchar2);
	---------------------------------------------------------   

	---------------------------------------------------------
	-- Configuração Vegetal - Grupo Químico
	---------------------------------------------------------
	procedure grupoquimico(p_id               number,
						   p_acao             number,
						   p_executor_id      number,
						   p_executor_nome    varchar2,
						   p_executor_login   varchar2,
						   p_executor_tipo_id number,
						   p_executor_tid     varchar2);
	---------------------------------------------------------

	---------------------------------------------------------
	-- Configuração Vegetal - Classe de Uso
	---------------------------------------------------------
	procedure classeuso(p_id               number,
						p_acao             number,
						p_executor_id      number,
						p_executor_nome    varchar2,
						p_executor_login   varchar2,
						p_executor_tipo_id number,
						p_executor_tid     varchar2);
	---------------------------------------------------------

	---------------------------------------------------------
	-- Configuração Vegetal - Periculosidade Ambiental
	---------------------------------------------------------
	procedure periculosidadeambiental(p_id               number,
									  p_acao             number,
									  p_executor_id      number,
									  p_executor_nome    varchar2,
									  p_executor_login   varchar2,
									  p_executor_tipo_id number,
									  p_executor_tid     varchar2);
	---------------------------------------------------------

	---------------------------------------------------------
	-- Configuração Vegetal - Classificação Toxicológica
	---------------------------------------------------------
	procedure classificacaotoxicologica(p_id               number,
										p_acao             number,
										p_executor_id      number,
										p_executor_nome    varchar2,
										p_executor_login   varchar2,
										p_executor_tipo_id number,
										p_executor_tid     varchar2);
	---------------------------------------------------------

	---------------------------------------------------------
	-- Configuração Vegetal - Modalidade de aplicação
	---------------------------------------------------------
	procedure modalidadeaplicacao(p_id               number,
								  p_acao             number,
								  p_executor_id      number,
								  p_executor_nome    varchar2,
								  p_executor_login   varchar2,
								  p_executor_tipo_id number,
								  p_executor_tid     varchar2);
	---------------------------------------------------------

	---------------------------------------------------------
	-- Configuração Vegetal - Forma de apresentação
	---------------------------------------------------------
	procedure formaapresentacao(p_id               number,
								p_acao             number,
								p_executor_id      number,
								p_executor_nome    varchar2,
								p_executor_login   varchar2,
								p_executor_tipo_id number,
								p_executor_tid     varchar2);
	---------------------------------------------------------

	---------------------------------------------------------
	-- Configuração Vegetal - Ingrediente Ativo
	---------------------------------------------------------
	procedure ingredienteativo(p_id               number,
							   p_acao             number,
							   p_executor_id      number,
							   p_executor_nome    varchar2,
							   p_executor_login   varchar2,
							   p_executor_tipo_id number,
							   p_executor_tid     varchar2);
	---------------------------------------------------------

	---------------------------------------------------------
	-- Configuração Vegetal - Cultura
	---------------------------------------------------------
	procedure cultura(p_id               number,
					  p_acao             number,
					  p_executor_id      number,
					  p_executor_nome    varchar2,
					  p_executor_login   varchar2,
					  p_executor_tipo_id number,
					  p_executor_tid     varchar2);
	---------------------------------------------------------

	---------------------------------------------------------
	-- Configuração Vegetal - Praga
	---------------------------------------------------------
	procedure praga(p_id               number,
					p_acao             number,
					p_executor_id      number,
					p_executor_nome    varchar2,
					p_executor_login   varchar2,
					p_executor_tipo_id number,
					p_executor_tid     varchar2);
	---------------------------------------------------------

	---------------------------------------------------------
	-- Credenciado - Habilitar Emissão de CFO/CFOC
	---------------------------------------------------------
	procedure habilitaremissaocfocfoc(p_id               number,
									  p_acao             number,
									  p_executor_id      number,
									  p_executor_nome    varchar2,
									  p_executor_login   varchar2,
									  p_executor_tipo_id number,
									  p_executor_tid     varchar2);
	--------------------------------------------------------- 

	---------------------------------------------------------
	-- Agrotoxico
	---------------------------------------------------------
	procedure agrotoxico(p_id               number,
						 p_acao             number,
						 p_executor_id      number,
						 p_executor_nome    varchar2,
						 p_executor_login   varchar2,
						 p_executor_tipo_id number,
						 p_executor_tid     varchar2);
	---------------------------------------------------------   

	---------------------------------------------------------
	-- Configuração cfo cfoc ptv
	---------------------------------------------------------
	procedure configdocumentofitossanitario(p_id               number,
											p_acao             number,
											p_executor_id      number,
											p_executor_nome    varchar2,
											p_executor_login   varchar2,
											p_executor_tipo_id number,
											p_executor_tid     varchar2);
	---------------------------------------------------------

	---------------------------------------------------------
	-- Liberação cfo cfoc
	---------------------------------------------------------
	procedure liberacaocfocfoc(p_id               number,
							   p_acao             number,
							   p_executor_id      number,
							   p_executor_nome    varchar2,
							   p_executor_login   varchar2,
							   p_executor_tipo_id number,
							   p_executor_tid     varchar2);
	---------------------------------------------------------

	---------------------------------------------------------
	-- Números cfo cfoc
	---------------------------------------------------------
	procedure numerocfocfoc(p_id               number,
							p_acao             number,
							p_executor_id      number,
							p_executor_nome    varchar2,
							p_executor_login   varchar2,
							p_executor_tipo_id number,
							p_executor_tid     varchar2);
	---------------------------------------------------------

	---------------------------------------------------------
	-- Destinatario ptv
	---------------------------------------------------------
	procedure destinatarioptv(p_id               number,
							  p_acao             number,
							  p_executor_id      number,
							  p_executor_nome    varchar2,
							  p_executor_login   varchar2,
							  p_executor_tipo_id number,
							  p_executor_tid     varchar2);
	---------------------------------------------------------

	---------------------------------------------------------
	-- Habilitacao emissao ptv
	---------------------------------------------------------
	procedure habilitacaoemissaoptv(p_id               number,
									p_acao             number,
									p_executor_id      number,
									p_executor_nome    varchar2,
									p_executor_login   varchar2,
									p_executor_tipo_id number,
									p_executor_tid     varchar2);
	--------------------------------------------------------- 

	---------------------------------------------------------
	-- Emitir Permissão de Transito de Vegetais
	--------------------------------------------------------- 
	procedure emitirptv(p_id               number,
						p_acao             number,
						p_executor_id      number,
						p_executor_nome    varchar2,
						p_executor_login   varchar2,
						p_executor_tipo_id number,
						p_executor_tid     varchar2);
	---------------------------------------------------------

	---------------------------------------------------------
	-- Emitir Permissão de Transito de Vegetais de outro estado
	--------------------------------------------------------- 
	procedure emitirptvoutro(p_id               number,
							 p_acao             number,
							 p_executor_id      number,
							 p_executor_nome    varchar2,
							 p_executor_login   varchar2,
							 p_executor_tipo_id number,
							 p_executor_tid     varchar2);
	---------------------------------------------------------     

	---------------------------------------------------------
	-- Local Vistoria
	--------------------------------------------------------- 
	procedure localvistoria(p_id               number,
							p_acao             number,
							p_executor_id      number,
							p_executor_nome    varchar2,
							p_executor_login   varchar2,
							p_executor_tipo_id number,
							p_executor_tid     varchar2);
	---------------------------------------------------------  
  
  ---------------------------------------------------------
	-- Parametrizacao do sistema
	--------------------------------------------------------- 
	procedure parametrizacao(p_id               number,
							p_acao             number,
							p_executor_id      number,
							p_executor_nome    varchar2,
							p_executor_login   varchar2,
							p_executor_tipo_id number,
							p_executor_tid     varchar2);
	--------------------------------------------------------- 

 	---------------------------------------------------------
	--  VRTE
	---------------------------------------------------------
    
  procedure vrte(p_id               number,
							p_acao             number,
							p_executor_id      number,
							p_executor_nome    varchar2,
							p_executor_login   varchar2,
							p_executor_tipo_id number,
							p_executor_tid     varchar2);
	--------------------------------------------------------- 
  
  ---------------------------------------------------------
	--  COBRANCA
	---------------------------------------------------------
    
  procedure cobranca(p_id               number,
							p_acao             number,
							p_executor_id      number,
							p_executor_nome    varchar2,
							p_executor_login   varchar2,
							p_executor_tipo_id number,
							p_executor_tid     varchar2);
	--------------------------------------------------------- 
end;