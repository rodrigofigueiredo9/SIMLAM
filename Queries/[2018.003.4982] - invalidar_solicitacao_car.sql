ALTER TABLE HST_AUTENTICACAO_PAPEL_PERM
MODIFY PERMISSAO_TEXTO VARCHAR2(60 BYTE);

INSERT INTO lov_autenticacao_permissao (ID, NOME, CODIGO, FUNCIONARIO_TIPO, DESCRICAO, GRUPO, TIPO)
VALUES(385, 'Alterar Situação Solicitação – sem posse/credenciado',
'CadastroAmbientalRuralSolicitacaoInvalida',
3,
'Alterar Situação Solicitação para inválida',
'Cadastro Ambiental Rural',
1
);
INSERT INTO lov_situacao_envio_sicar (ID, TEXTO)
VALUES (9, 'Cancelado');

UPDATE TAB_CAR_SOLICITACAO SET MOTIVO = NULL where id not in (
  SELECT ID FROM TAB_CAR_SOLICITACAO WHERE MOTIVO LIKE '%retifi%');
UPDATE IDAFCREDENCIADO.TAB_CAR_SOLICITACAO SET MOTIVO = NULL where id not in (
  SELECT ID FROM IDAFCREDENCIADO.TAB_CAR_SOLICITACAO WHERE MOTIVO LIKE '%retifi%');