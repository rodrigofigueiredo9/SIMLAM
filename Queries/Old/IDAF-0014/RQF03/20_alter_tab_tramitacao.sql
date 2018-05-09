ALTER TABLE tab_tramitacao
  ADD( destino_externo VARCHAR2(100 BYTE) );
  
ALTER TABLE tab_tramitacao
  ADD( codigo_rastreio VARCHAR2(13 BYTE) );
  
ALTER TABLE tab_tramitacao
  ADD( forma_envio NUMBER(38,0) );
  
ALTER TABLE tab_tramitacao
  ADD( numero_autuacao VARCHAR2(15 BYTE) );  
  
ALTER TABLE hst_tramitacao
  ADD( destino_externo VARCHAR2(100 BYTE) );
  
ALTER TABLE hst_tramitacao
  ADD( codigo_rastreio VARCHAR2(13 BYTE) );
  
ALTER TABLE hst_tramitacao
  ADD( forma_envio NUMBER(38,0) );
  
ALTER TABLE hst_tramitacao
  ADD( numero_autuacao VARCHAR2(15 BYTE) );  