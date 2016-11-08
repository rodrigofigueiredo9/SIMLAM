CREATE OR REPLACE PACKAGE "OPERACAOGIS" is

   -----------------------------------------
   -- Sigla do estado  (MT, AP, PA, RO, ES)
   -----------------------------------------
   EstadoLCC constant varchar2(2) := 'ES';
   -----------------------------------------
   
   spa_tolerancia    constant number := 0.01001;
   srid_base         constant number := 82308; --WGS 84 / UTM zona 24 S
   srid_base_real    constant number := 31999; --SIRGAS / UTM zona 24 S
   diminfo           constant mdsys.sdo_dim_array := mdsys.sdo_dim_array(mdsys.SDO_DIM_ELEMENT('X', -2147483648, 2147483647, 5e-6),mdsys.SDO_DIM_ELEMENT('Y', -2147483648, 2147483647, 5e-6));

   function  formataGMS(coordenada varchar2) return varchar2;
   function  gdec2spatialrect(lon1 number, lat1 number, lon2 number, lat2 number) return mdsys.sdo_geometry;   
   procedure gms2gdec(longitude_gms varchar2, latitude_gms varchar2, longitude out number, latitude out number);      
   procedure gdec2gms(longitude_gdec number, latitude_gdec number, longitude out varchar2, latitude out varchar2);    
   function gdec2spatial(datum varchar2, longitude number, latitude number) return mdsys.sdo_geometry;       
   function utm2spatialrect( minx number, miny number, maxx number, maxy number) return mdsys.sdo_geometry;                  
   procedure spatial2UTM(geo mdsys.sdo_geometry, easting out number, northing out number, fuso out number, hemisferiosul out number);   
   procedure spatial2GDec(geo mdsys.sdo_geometry, longitude_gdec out number, latitude_gdec out number);   
   procedure spatial2GMS(geo mdsys.sdo_geometry, longitude_gms out varchar2, latitude_gms out varchar2);
   function utm2spatial(datum varchar2, easting number, northing number, fuso number, hemisferiosul number) return mdsys.sdo_geometry;     
   function densificacao(p_geo mdsys.sdo_geometry) return mdsys.sdo_geometry;       
   function gdec2utmbaserealrect(lon1 number, lat1 number, lon2 number, lat2 number) return mdsys.sdo_geometry;
end operacaoGIS;

 

 
/
CREATE OR REPLACE PACKAGE BODY "OPERACAOGIS" is

   PI constant number:= 3.1415926535897932384626433832795;

   --------------------------------------------------------------------------------
   -- Funcao de conversao de grau decimal para radiano
   --------------------------------------------------------------------------------
   function gdec2radiano(coordenada number) return number is
      v_sentido number:= 1;
      v_grau number:= coordenada;
   begin
      if (v_grau<0) then
         v_grau:= - v_grau;
         v_sentido:= -1;
      end if;

      return transformacoes.radiano(v_grau, v_sentido);
   end;

   --------------------------------------------------------------------------------
   -- Funcao de formatacao de grau minuto segundo
   --------------------------------------------------------------------------------
   function formataGMS(coordenada varchar2) return varchar2 is
      v_sentido varchar2(1):= '';
      v_grau number;
      v_minuto number;
      v_segundo number;
      v_string_minuto varchar2(15);
      v_aux number;

      v_coord varchar2(15):= replace(replace(coordenada,' '), '.', ',');
      v_num number:= 1;
      v_pos number;
   begin

      v_pos := instr(v_coord,':');
      if (v_pos <> 0)  then
         v_grau := to_number(substr(v_coord, v_num, v_pos - v_num));
         if (v_grau is null) then
            v_grau := 0;
         end if;
      else
         v_grau := v_coord;
      end if;
      
      if ( instr(v_coord, '-')>0 ) then
         v_grau:= - v_grau;
         v_sentido:= '-';
      end if;

      v_num := v_pos+1;
      v_pos := instr(v_coord,':', v_num);
      
      if (v_pos <> 0 ) then
         v_minuto:= to_number( substr(v_coord, v_num, v_pos - v_num) );
         if (v_minuto is null) then
            v_minuto := 0;
         end if;
         
         v_num := v_pos+1;
         if ( v_num <= length(v_coord) )  then
            v_string_minuto := substr(v_coord, v_num);
            v_aux := instr(v_string_minuto, ',');
            
            if (v_aux <> 0) then
               if (v_aux < length(v_string_minuto)) then
                  v_segundo := to_number( v_string_minuto );
               else
                  if (v_aux <> 1) then
                     v_segundo := to_number( v_string_minuto+'0' );
                  else
                     v_segundo := 0;
                  end if;
               end if;
            else
               v_segundo := to_number( v_string_minuto );
            end if;
            
         else      
            v_segundo := 0;
         end if;
        
      else
         v_minuto:= to_number( substr(v_coord, v_num) );
         if (v_minuto is null) then
           v_minuto := 0;
         end if;
         v_segundo := 0;         
      end if;

      return replace( replace( v_sentido || to_char(v_grau,'900') ||':'|| to_char(v_minuto,'00') ||':'|| to_char(v_segundo,'00.00'), ' '), '.', ',');
   end;   
   ---------------------------------------------------------------------
   
   ---------------------------------------------------------------------
   --  Funcoes que busca o srid de um fuso UTM
   ---------------------------------------------------------------------
   function BuscarSridUtm(datum varchar2, fuso number, hemisferiosul number) return number is
      v_srid number;
   begin
   
      if hemisferiosul <> 1 then
         Raise_application_error(-20000, 'Hemisfério norte não tratado na busca de Srid.');
      end if;
         
      if datum = 'SAD69' then
            
         case fuso 
            when 18 then v_srid := 82258;
            when 19 then v_srid := 82267;
            when 20 then v_srid := 82279;
            when 21 then v_srid := 82287;
            when 22 then v_srid := 82295;
            when 23 then v_srid := 82301;
            when 24 then v_srid := 82307;
            when 25 then v_srid := 82313;
            else Raise_application_error(-20000, 'Fuso utm SAD69 não encontrado no buscar Srid.');
         end case;
            
      elsif datum = 'WGS84' then
      
          case fuso 
            when 18 then v_srid := 82259;
            when 19 then v_srid := 82268;
            when 20 then v_srid := 82280;
            when 21 then v_srid := 82288;
            when 22 then v_srid := 82296;
            when 23 then v_srid := 82302;
            when 24 then v_srid := 82308;
            when 25 then v_srid := 82314;
            else Raise_application_error(-20000, 'Fuso utm WGS84 não encontrado no buscar Srid.');
         end case;
         
      elsif datum = 'SIRGAS' then
         case fuso
              when 24 then v_srid := 31999;
                else Raise_application_error(-20000, 'Fuso utm SIRGAS não encontrado no buscar Srid.');
         end case;
     
      else
      
         Raise_application_error(-20000, 'Datum diferente de ''SAD69'' e ''WGS84''.');         
      end if;
      
      return v_srid;
   end;
   ---------------------------------------------------------------------

   ---------------------------------------------------------------------
   --  Funcoes publicas que montar a geometria
   ---------------------------------------------------------------------

   -- Funcao de geracao a partir de coordenadas em utm
   function utm2spatial(datum varchar2, easting number, northing number, fuso number, hemisferiosul number) return mdsys.sdo_geometry is
      v_geo       mdsys.sdo_geometry;
      srid_coord  number;
   begin
   
      srid_coord := BuscarSridUtm(datum, fuso, hemisferiosul);
      
      v_geo := mdsys.sdo_geometry(2001, srid_coord, sdo_point_type(easting, northing, null), null, null);   

      if srid_coord <> srid_base then
         v_geo := mdsys.sdo_cs.transform(v_geo, spa_tolerancia, srid_base);
      end if;
      
      return v_geo;
   end;   
   
   ---------------------------------------------------------------------
   
   -- coordenadas em Grau Decimal
   function gdec2Spatial(datum varchar2, longitude number, latitude number) return mdsys.sdo_geometry is
      srid number;
   begin
      srid := case datum when 'SAD69' then 8292 else 4326 end;
      return mdsys.sdo_cs.transform(mdsys.sdo_geometry(2001, srid, mdsys.sdo_point_type(longitude, latitude, null), null, null), spa_tolerancia, srid_base);
   end;
   ---------------------------------------------------------------------
   
   -- coordenadas em Grau Minuto Segundo
   function gms2Spatial(datum varchar2, longitude varchar2, latitude varchar2) return mdsys.sdo_geometry is
      v_longitude number;
      v_latitude number;
   begin
      gms2gdec(formataGMS(longitude), formataGMS(latitude), v_longitude, v_latitude);
      return gdec2Spatial(datum, v_longitude, v_latitude);
   end;
   ---------------------------------------------------------------------

   ---------------------------------------------------------------------
   -- Funcao de conversao de grau minuto segundo para radiano
   ---------------------------------------------------------------------
   function gdec2spatialrect(lon1 number, lat1 number, lon2 number, lat2 number) return mdsys.sdo_geometry is
      v_x1 number;
      v_y1 number;
      v_x2 number;
      v_y2 number;
      
      v_srid_geo  number;
      v_srid_base number;
      rect mdsys.sdo_geometry;
   begin

      if (lon2<lon1) then
         v_x1 := lon2;
         v_x2 := lon1;
      else
         v_x1 := lon1;
         v_x2 := lon2;
      end if;

      if (lat2<lat1) then
         v_y1 := lat2;
         v_y2 := lat1;
      else
         v_y1 := lat1;
         v_y2 := lat2;
      end if;

      select to_number(a.valor) into v_srid_geo from tab_configuracao a where a.chave='SRID_GEOGRAFICO';
      select to_number(a.valor) into v_srid_base from tab_configuracao a where a.chave='SRID_BASE';


      rect:= mdsys.sdo_geometry(2003,v_srid_geo, null, mdsys.sdo_elem_info_array(1,1003,3), mdsys.sdo_ordinate_array(v_x1, v_y1, v_x2, v_y2));

      if (v_srid_base = v_srid_geo) then
         return rect;
      else
         return sdo_cs.transform(rect, spa_tolerancia, v_srid_base);
      end if;
   end;
   --------------------------------------------------------------------------------
   function utm2spatialrect( minx number, miny number, maxx number, maxy number) return mdsys.sdo_geometry is
      rect mdsys.sdo_geometry;
   begin
      rect:= mdsys.sdo_geometry(2003,srid_base_real, null, mdsys.sdo_elem_info_array(1,1003,3), mdsys.sdo_ordinate_array(minx, maxy, maxx, maxy,
                                                                                             maxx,miny, minx,miny,minx,maxy));
      return rect;
    end;
   
   --------------------------------------------------------------------------------
   -- Funcao de conversao de radiano para grau decimal
   --------------------------------------------------------------------------------
   function limitarRadiano(coordenada number, longitude boolean) return number deterministic is
      v_valor   number;
      v_acresc  number;
      v_sentido number;
      v_radiano number:= coordenada;

   begin
      v_sentido := sign(v_radiano);
      v_radiano := abs(v_radiano);

      v_valor   := case when(longitude) then PI else PI/2 end;
      v_acresc  := -(2*v_valor);

      v_radiano:= mod (v_radiano, PI*2);

      while(v_radiano>v_valor) loop
         v_radiano:= v_radiano + v_acresc;
      end loop;

      v_radiano:= v_radiano * v_sentido;

      return v_radiano;
   end;   
   --------------------------------------------------------------------------------
      
   function radiano2gdec(coordenada number, longitude boolean) return number is
      v_grau number;
      v_sentido number;
   begin
      transformacoes.graudec( limitarRadiano(coordenada, longitude), v_grau, v_sentido);
      return round(v_grau*v_sentido, 6);
   end;
   
   --------------------------------------------------------------------------------
   -- Funcao de conversao de grau minuto segundo para radiano 
   --------------------------------------------------------------------------------   
   function gms2radiano(coordenada varchar2) return number is
      v_sentido number:= 1;
      v_grau number;
      v_minuto number;
      v_segundo number;
      
      v_coord varchar2(15):= replace(replace(coordenada,' '), '.', ',');
      v_num number:= 1;
      v_pos number;
   begin

      v_pos := instr(v_coord,':');
      v_grau := to_number(substr(v_coord, v_num, v_pos - v_num));

      if ( instr(v_coord, '-')>0 ) then
         v_sentido:= -1;
         v_grau:= -v_grau;
      end if;

      v_num := v_pos+1;
      v_pos := instr(v_coord,':', v_num);
      v_minuto:= to_number( substr(v_coord, v_num, v_pos - v_num) );
      v_num := v_pos+1;
      v_segundo:= to_number( substr(v_coord, v_num) );

      return transformacoes.radiano(v_grau, v_minuto, v_segundo, v_sentido);
   end;
   ---------------------------------------------------------------------   
   
   ---------------------------------------------------------------------
   --  Funcoes de construcao de geometria spatial em LCC
   ---------------------------------------------------------------------

   -- Funcao de geracao a partir de coordenadas em radianos
   function geo2spatial(datum varchar2, longitude number, latitude number) return mdsys.sdo_geometry is
      v_x number;
      v_y number;
   begin
      transformacoes.lcc(EstadoLCC, datum);
      transformacoes.geo2lcc( limitarRadiano(longitude, true), limitarRadiano(latitude, false), v_x, v_y);

      return mdsys.sdo_geometry(2001, null, mdsys.sdo_point_type(v_x, v_y, null), null, null);
   end;


  
   ---------------------------------------------------------------------   
   
   
   ---------------------------------------------------------------------
   --  Funcoes de verificacao Spatial
   ---------------------------------------------------------------------

   -- Funcao que verifica se uma geometria esta dentro do estado
   /*function noEstado(geo mdsys.sdo_geometry) return number is
      v_qtd number;
   begin
      select count(*) into v_qtd from estados_sad69 e where sdo_relate(e.geometry, geo, 'MASK=ANYINTERACT QUERYTYPE=WINDOW')='TRUE';
      return v_qtd;
   end;*/
   ---------------------------------------------------------------------   
      
   ---------------------------------------------------------------------
   --  Funcoes publicas que verificam se as coordenadas do ponto estam dentro do estado
   ---------------------------------------------------------------------

   -- coordenadas em UTM
  /*function utmNoEstado(datum varchar2, easting number, northing number, fuso number, hemisferiosul number) return number is
   begin
      return noEstado(utm2spatial(datum, easting, northing, fuso, hemisferiosul));
   end;

   -- coordenadas em Grau Minuto Segundo
   function gmsNoEstado(datum varchar2, longitude varchar2, latitude varchar2) return number is
   begin
      return noEstado(geo2spatial(datum, gms2radiano(formataGMS(longitude)), gms2radiano(formataGMS(latitude))));
   end;

   -- coordenadas em Grau Decimal
   function gdecNoEstado(datum varchar2, longitude number, latitude number) return number is
   begin
      return noEstado(geo2spatial(datum, gdec2radiano(longitude), gdec2radiano(latitude)));
   end;*/

   ---------------------------------------------------------------------
      
   --------------------------------------------------------------------------------
   -- GMS para GDec 
   --------------------------------------------------------------------------------
   procedure gms2gdec(longitude_gms varchar2, latitude_gms varchar2, longitude out number, latitude out number) is
   begin
      longitude:= radiano2gdec(gms2radiano(longitude_gms), true);
      latitude:= radiano2gdec(gms2radiano(latitude_gms), false);
   end;  
   
   --------------------------------------------------------------------------------
   -- Funcao de conversao de radiano para grau minuto segundo
   --------------------------------------------------------------------------------
   function radiano2gms(coordenada number, longitude boolean) return varchar2 is
      v_sentido number;
      v_grau number;
      v_minuto number;
      v_segundo number;

   begin
      transformacoes.grau(limitarRadiano(coordenada, longitude), v_grau, v_minuto, v_segundo, v_sentido);

      return replace( replace( to_char(v_grau*v_sentido,'900') ||':'|| to_char(v_minuto,'00') ||':'|| to_char(v_segundo,'00.00'), ' '), '.', ',');
   end;
   
   
   --------------------------------------------------------------------------------
   -- GDec para GMS
   --------------------------------------------------------------------------------
   procedure gdec2gms(longitude_gdec number, latitude_gdec number, longitude out varchar2, latitude out varchar2) is
   begin
      longitude:= radiano2gms(gdec2radiano(longitude_gdec), true);
      latitude:= radiano2gms(gdec2radiano(latitude_gdec), false);
   end;   
   ---------------------------------------------------------------------
   
   ---------------------------------------------------------------------
   --  Função que retorna a coordenada em radianos de um ponto da geometria
   ---------------------------------------------------------------------

   procedure spatial2Geo(geo mdsys.sdo_geometry, longitude out number, latitude out number) is
      v_geo mdsys.sdo_geometry;
   begin 
   
      if (geometria9i.validar(geo) <> 'TRUE') then
         Raise_application_error(-20000, 'Geometria inválida.');
      end if;
      
      v_geo:= geometria9i.extrair(geo);
      
      v_geo := mdsys.sdo_cs.transform(geo, spa_tolerancia, 31999);
      
     -- transformacoes.LCC(EstadoLCC);
      
      if (v_geo.sdo_point is null) then
         transformacoes.LCC2Geo(v_geo.sdo_ordinates(1), v_geo.sdo_ordinates(2), longitude, latitude);
      else
         transformacoes.LCC2Geo(v_geo.sdo_point.X, v_geo.sdo_point.Y, longitude, latitude);
      end if;
      
      longitude:= limitarRadiano(longitude, true);
      latitude:= limitarRadiano(latitude, false);
   end;

   ---------------------------------------------------------------------
   
   
   ---------------------------------------------------------------------
   --  Função que retorna a coordenada UTM de um ponto da geometria
   ---------------------------------------------------------------------

   procedure spatial2UTM(geo mdsys.sdo_geometry, easting out number, northing out number, fuso out number, hemisferiosul out number) is 
      v_lon number;
      v_lat number;

      v_grau number;
      v_sentido number;
      
   begin 
   
      spatial2Geo(geo, v_lon, v_lat);

      transformacoes.utm();
      transformacoes.geo2utm(v_lon, v_lat, easting, northing);

      transformacoes.graudec(v_lon, v_grau, v_sentido);
      fuso:= transformacoes.zonautm( floor(v_grau*v_sentido) );
      
      if (v_lat<0) then
         hemisferiosul:= 1;
      else
         hemisferiosul:= 0;
      end if;

      easting:= round(easting, 4);
      northing:= round(northing, 4);
   end;
   
   ---------------------------------------------------------------------


   ---------------------------------------------------------------------
   --  Função que retorna a coordenada UTM de um ponto da geometria
   ---------------------------------------------------------------------

   procedure spatial2GDec(geo mdsys.sdo_geometry, longitude_gdec out number, latitude_gdec out number) is 
      v_lon number;
      v_lat number;

      v_grau number;
      v_sentido number;
      
   begin 
      spatial2Geo(geo, v_lon, v_lat);

      transformacoes.graudec( v_lon, v_grau, v_sentido);
      longitude_gdec := round(v_grau*v_sentido, 6);
      
      transformacoes.graudec( v_lat, v_grau, v_sentido);
      latitude_gdec := round(v_grau*v_sentido, 6);
   end;
   
   ---------------------------------------------------------------------

   ---------------------------------------------------------------------
   -- Funcao de conversao de radiano para grau minuto segundo
   ---------------------------------------------------------------------

   procedure spatial2GMS(geo mdsys.sdo_geometry, longitude_gms out varchar2, latitude_gms out varchar2) is 
   
      v_lon number;
      v_lat number;

      v_sentido number;
      v_grau number;
      v_minuto number;
      v_segundo number;

   begin
      spatial2Geo(geo, v_lon, v_lat);

      transformacoes.grau(v_lon, v_grau, v_minuto, v_segundo, v_sentido);
      longitude_gms:= (case when (v_sentido<0 and (v_grau + v_minuto + v_segundo)>0) then '-' else '' end) || replace( replace( to_char(v_grau,'900') ||':'|| to_char(v_minuto,'00') ||':'|| to_char(v_segundo,'00.00'), ' '), '.', ',');
      
      transformacoes.grau(v_lat, v_grau, v_minuto, v_segundo, v_sentido);
      latitude_gms:= (case when (v_sentido<0 and (v_grau + v_minuto + v_segundo)>0) then '-' else '' end) || replace( replace( to_char(v_grau,'900') ||':'|| to_char(v_minuto,'00') ||':'|| to_char(v_segundo,'00.00'), ' '), '.', ',');
   end;
   
   ---------------------------------------------------------------------
   
   ---------------------------------------------------------------------
   -- Funcao de densificação
   ---------------------------------------------------------------------

   function densificacao(p_geo mdsys.sdo_geometry) return mdsys.sdo_geometry is
      v_tol number;
      v_count number:=0;
      v_geo mdsys.sdo_geometry := p_geo;
      geo mdsys.sdo_geometry;
      valido varchar2(4000);
   begin
   
      v_tol := 6;
      geo := null;
      
      -- 6, 7 e 8
      loop 
         v_count:=v_count+1;
         begin 
            geo := mdsys.sdo_geom.sdo_arc_densify(v_geo, diminfo, 'arc_tolerance='||(v_tol*10)||'e-1 unit=MM');
         exception
            when others then 
            geo := null;
         end;
         valido:= mdsys.sdo_geom.validate_geometry_with_context(geo, spa_tolerancia);
         exit when valido = 'TRUE' or v_count > 3;
         v_tol:=v_tol+1;
      end loop;
      
      -- 5, 4, 3, 2, 1
      if valido <> 'TRUE' then   
         v_tol := 5;
         v_count:= 0;
         loop 
            v_count:=v_count+1;
            begin 
               geo := mdsys.sdo_geom.sdo_arc_densify(v_geo, diminfo, 'arc_tolerance='||(v_tol*10)||'e-1 unit=MM');
            exception
               when others then 
               geo := null;
            end;
            valido:= mdsys.sdo_geom.validate_geometry_with_context(geo, spa_tolerancia);
            exit when valido = 'TRUE' or v_count=5;
            v_tol:=v_tol-1;
         end loop;
      end if;
      
      if valido <> 'TRUE' then
         geo := mdsys.sdo_geom.sdo_arc_densify( v_geo, spa_tolerancia, 'arc_tolerance=0,000001' );
      end if;
      
      if (v_geo is not null) then
         v_geo.SDO_SRID:= p_geo.SDO_SRID;
      end if;
      
      return geo; 
   end;
   
   ---------------------------------------------------------------------   
   
    function gdec2utmbaserealrect(lon1 number, lat1 number, lon2 number, lat2 number) return mdsys.sdo_geometry is       
         geoDec    mdsys.sdo_geometry;         
         geobase   mdsys.sdo_geometry;
         geobaseReal mdsys.sdo_geometry;
    begin
         geoDec := operacaogis.gdec2spatialrect(lon1, lat1, lon2, lat2);
         
         geobase :=  mdsys.sdo_cs.transform(geoDec, spa_tolerancia, srid_base);
         
         if(srid_base = srid_base_real) then
             return geobase;
         else         
            geobaseReal := mdsys.sdo_geometry(geobase.SDO_GTYPE, srid_base_real, null, geobase.SDO_ELEM_INFO, geobase.SDO_ORDINATES);         
         end if;
         
         return geobaseReal;
    end;
   

end operacaoGIS;
/
