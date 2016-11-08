CREATE OR REPLACE PACKAGE "TECNOGEO" is

   spa_tolerancia  constant number := 0.01001;

   function CLOB2Geometry(bytes1 clob) return mdsys.sdo_geometry;
   function Extrair(geometria mdsys.sdo_geometry, gtype number default null, simplificar varchar2 default 'FALSE') return mdsys.sdo_geometry;
   function Transformar(geometria mdsys.sdo_geometry, srid_origem number, srid_destino number) return mdsys.sdo_geometry;

end;

 
 

 

 
/
CREATE OR REPLACE PACKAGE BODY "TECNOGEO" is

   type t_reader is record (
      pos_ini    number := 0,
      pos_fim    number := 0,
      tam_buffer number := 0,
      buffer     varchar2(32767) := '',
      tam_clob   number,
      clob_      clob
   );


   ----------------------------------------------------------------------------
   -- Pega proximo numero de uma string com numeros separados por espacos
   ----------------------------------------------------------------------------
   function pegaNumero(pos in out nocopy number, reader in out nocopy t_reader) return number is
      tam_char number;
      buf_char varchar2(50);
      tam_lido number;
      ini_leitura number;
      fim_leitura number;

   begin

      -- pega buffer
      if (reader.tam_buffer < 1) or (pos > reader.pos_fim) then
         tam_lido := reader.tam_clob - reader.pos_fim;

         if tam_lido > 32767 then
            tam_lido := dbms_lob.instr(reader.clob_, ' ', reader.pos_fim + 32767 - 50);
            tam_lido :=  tam_lido - reader.pos_fim;
         end if;
         
         dbms_lob.read(reader.clob_, tam_lido, reader.pos_fim+1, reader.buffer);
         reader.tam_buffer := tam_lido;
         reader.pos_ini := reader.pos_fim+1;
         reader.pos_fim := reader.pos_ini + tam_lido-1;
      end if;

      -- traduz posicao
      ini_leitura := pos - reader.pos_ini + 1;
      fim_leitura := instr(reader.buffer, ' ', ini_leitura);
      tam_char := fim_leitura - ini_leitura;

      buf_char := substr(reader.buffer, ini_leitura, tam_char);
      pos := pos + tam_char + 1;

      return to_number(buf_char);
   end;
   ----------------------------------------------------------------------------



   ----------------------------------------------------------------------------
   -- Converte CLOB para Geometria
   ----------------------------------------------------------------------------
   function CLOB2Geometry(bytes1 clob) return mdsys.sdo_geometry is 
      geo mdsys.sdo_geometry := new mdsys.sdo_geometry(null,null,null,null,null);
      pos number;
      i   number;
      tam binary_integer;
      valor_number number;

      elem_info mdsys.sdo_elem_info_array;
      ordinate mdsys.sdo_ordinate_array;
      reader t_reader;
      bytes clob;

   begin
      bytes := replace(bytes1, '.', ',');
      reader.tam_clob := dbms_lob.getlength(bytes);
      reader.clob_ := bytes;

      pos := 1;
      
      -- gtype
      valor_number := pegaNumero(pos, reader);
      geo.SDO_GTYPE := valor_number;

      -- srid
      valor_number := pegaNumero(pos, reader);
      if (valor_number <> -1) then
         geo.SDO_SRID := valor_number;
      end if;

      -- point
      valor_number := pegaNumero(pos, reader);

      -- tem point
      if valor_number <> 0 then
         geo.SDO_POINT := new mdsys.sdo_point_type(null,null,null);

         -- x
         valor_number := pegaNumero(pos, reader);
         geo.SDO_POINT.X := valor_number;

         -- y
         valor_number := pegaNumero(pos, reader);
         geo.SDO_POINT.Y := valor_number;

         -- z
         valor_number := pegaNumero(pos, reader);
         geo.SDO_POINT.Z := valor_number;
      end if;

      -- eleminfo
      tam := pegaNumero(pos, reader);

      if (tam >0) then
         elem_info := new mdsys.sdo_elem_info_array();
         elem_info.Extend(tam);
         i := 1;

         while i <= tam loop
            valor_number := pegaNumero(pos, reader);
            elem_info(i) := valor_number;
            i := i + 1;
         end loop;
   
         geo.SDO_ELEM_INFO := elem_info;
   
         -- ordinates
         tam := pegaNumero(pos, reader); 
   
         if (tam > 0) then    
            ordinate := new mdsys.sdo_ordinate_array();
            ordinate.extend(tam);
            i := 1;

            while i <= tam loop
               valor_number := pegaNumero(pos, reader);
               ordinate(i) := valor_number;
               i := i + 1;
            end loop;

            geo.SDO_ORDINATES := ordinate;
         end if;

      end if;
      
      return geo;

   exception
      when others then
         return null;
   end;
   ----------------------------------------------------------------------------


  ----------------------------------------------------------------------------
   -- Extrai apenas o que for espacialmente invalido
   ----------------------------------------------------------------------------
   function Extrair(geometria mdsys.sdo_geometry, gtype number default null, simplificar varchar2 default 'FALSE') return mdsys.sdo_geometry is
      geo mdsys.sdo_geometry:= geometria;
   begin
      if (geo is not null) then

         if (gtype is not null) then
            geo.sdo_gtype := gtype;
         end if;
         
         geo:= geometria9i.extrair(geo);
   
         if (geometria9i.validar(geo)='TRUE') then
            
            begin

               if ((simplificar = 'TRUE') and (geo.SDO_GTYPE = 2006 or geo.SDO_GTYPE = 2007)) then 
                  if (geometria9i.subElementos(geo) = 1) then 
                     geo.SDO_GTYPE := (case when geo.SDO_GTYPE = 2006 then 2002 else 2003 end);
                  end if;
               end if;
            
            exception
            when others then
               null;
            end;
         
            return geo;
         end if;

      end if;
      return null;
   end;
   ----------------------------------------------------------------------------


   ----------------------------------------------------------------------------
   -- Transformar o sistema de coordenadas da geometria
   ----------------------------------------------------------------------------
   function Transformar(geometria mdsys.sdo_geometry, srid_origem number, srid_destino number) return mdsys.sdo_geometry is
      geo mdsys.sdo_geometry:= geometria;
   begin
      if (geo is not null) then
         if (geometria9i.validar(geo)='TRUE') then
            geo.sdo_srid := srid_origem;

            if (srid_origem <> srid_destino) then
               geo := sdo_cs.transform(geo, spa_tolerancia, srid_destino);
            end if;
         else
            geo.SDO_SRID := srid_destino;
         end if;
      end if;

      return geo;
   end;
   ----------------------------------------------------------------------------

end;
/
