--===================================================================================
-- SCRIPTS DE CRIA플O FUN합ES E PROCEDURES - IDAF
--===================================================================================

set feedback off
set define off

/* 
 * <idaf>	  = Substituir para o esquema do Idaf.
 */


prompt 
prompt ------------------------------------------------------------------------
prompt INICIANDO SCRIPTS DE CRIA플O - IDAF
prompt ------------------------------------------------------------------------

prompt 
prompt -------------------------------------------------

-----------------------------------------------------------------------------------
-- FUN합ES
-----------------------------------------------------------------------------------

create or replace function GerarEnvelope(v_geo mdsys.sdo_geometry) return clob is
 coordenadas clob;
begin
  if(v_geo is not null) then
        if(v_geo.SDO_ORDINATES is not null) then
          for k in 1..v_geo.sdo_ordinates.count loop
              if(k=1) then
                coordenadas := to_char(v_geo.sdo_ordinates(k));
              else
                coordenadas := coordenadas ||';'||to_char(v_geo.sdo_ordinates(k));
              end if;
          end loop;
        else
            if(v_geo.SDO_POINT is not null) then
                 coordenadas := to_char(v_geo.SDO_POINT.x)||';'||to_char(v_geo.SDO_POINT.y);             
            end if;
        end if;
  end if;
  return coordenadas;
end;
/

--------------------------------------------------------------------------------------------

prompt 
prompt --------------------------------------------------------------------------
prompt FINALIZANDO SCRIPTS DE CRIA플O FUN합ES E PROCEDURES - IDAF
prompt --------------------------------------------------------------------------
prompt 

set feedback on
set define on

--===================================================================================

