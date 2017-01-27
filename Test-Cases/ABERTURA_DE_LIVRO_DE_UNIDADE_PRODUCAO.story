Narrativa:
Como desenvolvedor de sistemas,
Desejo ter a capacidade de testar o procedimento de abertura de livro de unidade de producao,
Afim de poder reproduzir erros relacionados a este no futuro.

Titulo: Abrindo Livro de Unidade de Produção

Cenário:
Dado que a aplicação é o SIMLAM Institucional,
E o usuário logado é o victor.vicentini,
Ao abrir a tela de Título,
Clicar em Cadastrar Título,
Escolher Setor de Cadastro: Departamento de Defesa Sanitária e Inspeção Vegetal,
Escolher Local de emissão: Afonso Cláudio,
Escolher Modelo: Abertura de Livro de Unidade de Produção,
Clicar em Buscar Processo,
Buscar pelo Nº de registro do processo: 40027/2016,
Clicar em Associar,
Digitar 20 em "Total de páginas do livro",
Digitar 1 em "Nº de Pagina Inicial",
Digitar 20 em "Nº de Pagina Final",
Escolher Unidade de Produção: 32050022923160006,
Clicar no botão + para adicionar unidade de produção,
Escolher Setor Assinante: Seção de Defesa Sanitária Vegetal,
Escolher Cargo Assinante: Chefe de Departamento,
Escolher Nome Assinante: Adriana Kister Rodrigues,
Clicar no botão + para adicionar o assinante,
Escolher um destinátario de email,
Clicar em Salvar,
Então modal de mensagem do sistema deve ser mostrada com "Título salvo com sucesso",
E ao clicar no botão "Gerar PDF",
Então o arquivo "Abertura de Livro de Unidade de Produção.pdf" deve ser baixado,
E o codigo da UP no final deve ser o mesmo exibido no sistema.