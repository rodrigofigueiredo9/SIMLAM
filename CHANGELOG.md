# My app - Changelog



## Breaking changes

  - **rqf-21**
    - due to [07a8fdf7](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/07a8fdf781758a91dcbc98c36e29e3b1973698fc),
  A partir deste commit, o campo CodigoPropriedade da Classe UnidadeProducao precisa

de ser um Int64 e não mais um Int32, pois ele tem um tamanho maior que este. O campo atual no banco

é um Number(38, 0), incompatível com Int64. É necessário executar um alter table no banco caso a

tabela CRT_UNIDADE_PRODUCAO na coluna PROPRIEDADE_CODIGO não se comporte como deve.

  ([07a8fdf7](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/07a8fdf781758a91dcbc98c36e29e3b1973698fc))




## Bug Fixes

  - **bus**
    - UnidadeProducaoBus
  ([01c1441a](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/01c1441a533f33c90c96e44713b2dcb30958ebf9))

  - **ci**
    - Credenciado.mono.sln
  ([ca859b66](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/ca859b66488d185fa86af717d080e90a8b880be7))

  - **data-access**
    - UnidadeProducaoDa/Institucional
  ([3f7de76e](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/3f7de76e83fb354bdae1f7f6cd33922990e560a1))
    - UnidadeProducaoDa
  ([f4c08799](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/f4c0879990ebedbd0b0207f40971f825d5b4294d))

  - **epermissao**
    - Corrige problema de Enum
  ([5cefe2a5](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/5cefe2a58d7c016fbc075abda68a81e094ee5618),
   [#2](http://local.pentago.com.br:9995/idaf/SIMLAM/issues/2))

  - **input-length**
    - Length do input de CodigoUP
  ([f520a116](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/f520a116f71772e85fece4e19bf25d4e55530639))

  - **proj idaf**
    - Colocando o Web.Config de novo para o banco Oracle 12c (amb dev).
  ([521bf74f](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/521bf74f5010afdbf4a27fcb35fdcad547c52dce))

  - **rqf-02**
    - - Consertando as mensagens de validação ao Salvar.
  ([6118b375](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/6118b37536620fa94aa9b4b1c82dc7c3dab3fe21))

  - **rqf-06**
    - Correção do ano no SQL (números digitais)
  ([e7e71310](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/e7e713106bbddbb5f067cb80a0b2ab9a858898f5))
    - Block trazer números digitais dos anos anteriores de CFO e CFOC
  ([b6f87308](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/b6f87308a8e1213689a75c2dd7a57f922f129d16))

  - **rqf-21**
    - Unidade de Producao no Credenciado
  ([237eeba4](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/237eeba4948ba4d4aba0482b2f8b186b6b731866))

  - **solution**
    - Removes unnecessary references in Interno.csproj
  ([97a7ef4a](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/97a7ef4a400bb6646df1531a9f940515e3222492))

  - **unidade-producao-interno**
    - Credenciado: Int to Int64
  ([55497107](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/554971077d0ab27d24491cce3ae576c323d259c0))

  - **unidade-producao/data-access**
    - DbType.Int32 to DbType.Int64
  ([d24f73f1](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/d24f73f11c104538f6690b4ef2694862283787d8))




## Features

  - **rqf-06**
    - Melhoria de pesquisas em LiberacaoNumeroCFOCFOCDa.cs
  ([f0416240](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/f04162400c9bc61dd2eaea99a069d6a386c2d6a0))
    - CFOC - Saldo das Culturas
  ([c3cd178e](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/c3cd178e48c9a49097226b21f143a0bdcd065935))
    - Tela de Emissão de ePTV (blocos e digital)
  ([2402f758](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/2402f758dfdb1e1e14a3da1ebef7ae1b78bcb0bd))
    - Tela de Emissão de PTV (blocos e digital)
  ([ebec136c](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/ebec136c5e92c05219c8aec3f7f7cc19ddc0fc3c))
    - Tela de liberação de números CFO e CFOC
  ([ce9f0d16](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/ce9f0d16c584277209339b4dc11def6beec68e4b))

  - **rqf-21**
    - Numero do IBGE em codigos de propriedade
  ([bbe70500](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/bbe70500b0cee8c4096045a4d3a2aa028252616f))
    - Sequencia do Codigo da UP com 4 digitos
  ([07a8fdf7](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/07a8fdf781758a91dcbc98c36e29e3b1973698fc))

  - **unidade-producao-generator**
    - Método CodigoUpHasCodigoPropriedade
  ([845c5fbc](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/845c5fbcd285c90cf8d0fb025c9138b37b41990b))

  - **unidade-producao/data-access**
    - Métodos ObterEndereco e ObterMunicipio
  ([d904e23b](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/d904e23b443f230828de45e3336105ba0edadd4e))

  - **unidadeproducaogenerator**
    - Generator de Chaves de Unidade de Producao e CodigoPropriedade
  ([941a1d75](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/941a1d75ce399f2d2b1b717de29941020535aa54))




## Documentation

  - **queries**
    - Buscar email de credenciados
  ([1a3b7c83](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/1a3b7c8347202a25e9b0df1401083f992d79fc17))
    - Queries formatadas
  ([47e4e061](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/47e4e061a848b38af4d0e5ccb639f92b8fb78f27))
    - Alter Tables com dados de homologação
  ([92f2a965](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/92f2a9654c9bd5a5af4ea08184468f1dd963ffe0))
    - Queries da RQF-21
  ([ea5f3b6f](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/ea5f3b6f3f6a286548f80a5a465de77a1b1b7943))
    - Pasta de Queries
  ([d446d6f0](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/d446d6f04151a5e50647f71034989990dd39948c))

  - **readme**
    - README.md
  ([5fe457a0](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/5fe457a065a13265e382a83a1120f310cc411cca))




## Refactor

  - **cultura-interno/data-access**
    - Variavel inutilizada
  ([8f8106ea](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/8f8106ea32cc5a16fbf8cdbecc891b35a3b019d9))

  - **modularizacao**
    - Projeto Pentago.Utilities
  ([14640ba2](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/14640ba2966b4626598590a66f273ffc19cc0714))

  - **rqf-02**
    - Mensagens de CFO/CFOC no PTV de Outro Estado
  ([872a2ae5](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/872a2ae5f1435be413e510e144b6e284c45c2c0b))

  - **warning**
    - Removendo warning
  ([45f4fb33](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/45f4fb3371449febef9b9a0f47b2d1c52e9dd75c))




## Style

  - **project**
    - Identação com TABs
  ([12e55260](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/12e55260a74ce3760fe2d6e197ba1a8b20705c62))

  - **view-unidade-producao**
    - Tamanho de Coluna de Codigo UP
  ([01243707](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/0124370784e1fa0bd5095745c7652a8af0b75199))




## Test

  - **epermissao**
    - Projeto de Testes
  ([72b09d2c](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/72b09d2c9895b4e7f70f10b17c502c7fe8286348))

  - **project**
    - Projeto de testes unitários
  ([f236f62a](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/f236f62a76a82fca06d622d170dd07a956d6cbf9))

  - **rqf-21**
    - UnidadeProducaoGenerator
  ([530d1787](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/530d1787165461bbe5b872ce054209a8b89bb844))

  - **unidade-producao-generator**
    - Testes para validação
  ([d7f83a02](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/d7f83a025fff30fe69b3162471460e6875a4a306))

  - **unidadeproducaogenerator**
    - Testes de geração de códigos
  ([a220ecf5](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/a220ecf568abeca2568258ddd16f7a9eab8c8668))




## Chore

  - **binaries**
    - Removendo arquivo .suo do versionamento
  ([6c6cdd63](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/6c6cdd631440cd8374c0c096029c9bd83b258e8b))

  - **credenciado.model.extensoes**
    - csproj
  ([26e55f0b](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/26e55f0b3f46766219b86cdeb478640942692b9f))

  - **editorconfig**
    - Arquivo .editorconfig
  ([cd828a47](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/cd828a47474b9cc920d259f9ccc9331b34d46991))

  - **gitignore**
    - remove comments
  ([4a635ac7](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/4a635ac7af3d9ddacf42273a258429868f94258e))

  - **project**
    - .gitignore ignoring lib/*.dll.config
  ([910ebeef](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/910ebeefcf27707f53dae3a691f909bd49c51f1a))
    - Removes System.Net.Http.xml generated file from versioning
  ([f896a5e2](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/f896a5e22b85b901bc8892edd68f1bd9f074b926))
    - Ignores System.Net.Http.xml
  ([1967c988](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/1967c9881a812eaab2a8f8179d3b5bb664be5217))
    - .gitignore
  ([e8d429fc](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/e8d429fcc683a415a3d9bc32abb1edcd04b40acf))
    - Removendo arquivos não versionaveis
  ([55ca4a9f](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/55ca4a9f366f315a9b4c0381443de733a7c42df8))
    - Arquivos binarios funcionando.
  ([fe1ee492](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/fe1ee492c6c69a12968bcc634f509e82f32605a8))

  - **queries**
    - Queries de Alter table
  ([e3087868](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/e3087868bce09d8231f2df5b661c46831b725a28))

  - **suo-files**
    - Removes all .suo files
  ([67f334f4](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/67f334f4c0da3fe5172d8f70e126ee78859d77fd))

  - **web.config**
    - Conexões com banco de homologação
  ([6d91e00a](http://local.pentago.com.br:9995/idaf/SIMLAM/commits/6d91e00a8a6d3423c6aeebe8d87af18e7b0a6f53))





---
<sub><sup>*Generated with [git-changelog](https://github.com/rafinskipg/git-changelog). If you have any problems or suggestions, create an issue.* :) **Thanks** </sub></sup>
