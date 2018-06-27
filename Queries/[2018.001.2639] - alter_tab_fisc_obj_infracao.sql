alter table tab_fisc_obj_infracao modify DESC_TERMO_EMBARGO VARCHAR2(2000 BYTE);
alter table hst_fisc_obj_infracao modify DESC_TERMO_EMBARGO VARCHAR2(2000 BYTE);
alter table tab_fisc_obj_infracao modify opniao_area_danificada VARCHAR2(2000 BYTE);
alter table hst_fisc_obj_infracao modify opniao_area_danificada VARCHAR2(2000 BYTE);

UPDATE IDAF.CNF_IMPLANTACAO SET VALOR = '2018.001.2639' WHERE CAMPO like 'ultimoscript';