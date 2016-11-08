create or replace package DesenhadorWS is
   spa_tolerancia constant number := 0.01001;
   spa_tolerancia_area constant number := 0.001;
   spa_tolerancia_overlap constant number := 500;
   function ValidarGeometria(p_geo mdsys.sdo_geometry, idProjeto number) return varchar2 deterministic;
   function ValidarGeoDentroAreaAbrang(p_geo mdsys.sdo_geometry, idProjeto number) return varchar2;
   function  ErroSpatial(p_resposta varchar2) return varchar2 deterministic;
   function  ValidarGtype(geo mdsys.sdo_geometry, gType number) return varchar2 deterministic;
   procedure InserirFeicao(f_esquema varchar2, f_tabela varchar2, objectid number, f_rascunho varchar2);
   procedure AtualizarGeometriaFeicao(f_esquema varchar2, f_tabela varchar2, objectid number, f_rascunho varchar2);
end DesenhadorWS;
/
create or replace package body DesenhadorWS is
   
   function ErroSpatial(p_resposta varchar2) return varchar2 deterministic is

      v_aux      varchar2(5);

   begin
      v_aux      := substr(p_resposta,0,5);
      case (v_aux)
         when '13011' then
            return('Valor está acima do limite ');
         when '13013' then
            return('A area não especifica se é borda ou buraco ');
         when '13018' then
            return('A distancia especificada é invalida ');
         when '13019' then
            return('As coordenadas dos vertices estão fora do limite especificado ');
         when '13020' then
            return('Coordenada nula ');
         when '13021' then
            return('As coordenadas do elemento geometrico não estão conectadas ');
         when '13022' then
            return('O poligono possui segmentos que se cruzam ');
         when '13023' then
            return('Um elemento interior (buraco) cruza com a borda do poligono');
         when '13024' then
            return('O poligono tem menos de três segmentos');
         when '13025' then
            return('O poligono não esta fechado ');
         when '13026' then
            return('SDO_ETYPE invalido');
         when '13028' then
            return('SDO_GTYPE invalido');
         when '13029' then
            return('SDO_SRID invalido');
         when '13030' then
            return('Conflito do SDO_GTYPE com o SDO_GEOM_METADATA para o objeto geometrico');
         when '13031' then
            return('SDO_ELEM_INFO e SDO_ORDINATES são nulos e o SDO_GTYPE não é ponto ');
         when '13032' then
            return('Geometria invalida devido ao(s) campo(s) SDO_POINT, SDO_ELEM_INFO ou SDO_ORDINATES ');
         when '13033' then
            return('Dado(s) invalido(s) no SDO_ELEM_INFO da geometria ');
         when '13034' then
            return('Dado(s) invalido(s) no SDO_ORDINATES da geometria ');
         when '13035' then
            return('A geometria possui arcos em coordenadas geograficas ');
         when '13221' then
            return('SDO_GTYPE invalido ');
         when '13331' then
            return('Segmento LRS invalido ');
         when '13332' then
            return('Ponto LRS invalido ');
         when '13333' then
            return('Medida LRS alem do limite ');
         when '13334' then
            return('Segmentos LRS não conectados ');
         when '13335' then
            return('Medida LRS nula ');
         when '13340' then
            return('A geometria de ponto tem mais que uma coordenada ');
         when '13341' then
            return('A geometria de linha tem menos que duas coordenadas ');
         when '13342' then
            return('A geometria de arco tem menos de três coordenadas ');
         when '13343' then
            return('A geometria de polígono tem menos que três coordenadas ');
         when '13344' then
            return('O polígono de arcos tem menos que cinco coordenadas ');
         when '13345' then
            return('O polígono composto tem menos que cinco coordenadas ');
         when '13346' then
            return('As coordenadas usadas no arco são colineares ');
         when '13347' then
            return('As coordenadas que definem o arco não são distintas ');
         when '13348' then
            return('O ultimo ponto do polígono deve ser igual ao primeiro ');
         when '13349' then
            return('Borda do polígono se cruza ');
         when '13350' then
            return('Duas ou mais bordas ou buracos da geometria de poligono complexo se tocam ');
         when '13351' then
            return('Duas ou mais bordas ou buracos da geometria de poligono complexo se sobrepõem ');
         when '13352' then
            return('As coordenadas que definem o circulo estão incorretas ');
         when '13353' then
            return('O numero de dados no SDO_ELEM_INFO não é multiplo de três ');
         when '13354' then
            return('O SDO_ELEM_INFO referencia uma posicao incorreta no SDO_ORDINATES da geometria ');
         when '13355' then
            return('O numero de dados no SDO_ORDINATES não é multiplo das dimensoes da geometria ');
         when '13356' then
            return('Pontos repetidos na geometria ');
         when '13356' then
            return('O retangulo deve ter 2 pontos: superior direito e inferior esquerdo');
         when '13358' then
            return('Um circulo deve ter 3 pontos ');
         when '13359' then
            return('Os pontos do retangulo são coincidentes');
         when '13360' then
            return('Subtipo invalido para tipo composto');
         when '13361' then
            return('O tipo composto declara mais subtipos do que estão especificados no SDO_ELEM_INFO ');
         when '13362' then
            return('Os subtipos do tipo composto são desconexos ');
         when '13363' then
            return('Nenhum SDO_ETYPE é valido na geometria ');
         when '13366' then
            return('Combinação invalida de borda e buracos no poligono ');
         when '13367' then
            return('Orientação dos pontos da borda ou dos buracos está errada ');
         when '13368' then
            return('Polígono simples não pode ter mais de um anel exterior ');
         when '13369' then
            return('Valor do SDO_ETYPE de quatro digitos usado errado ');
         when '13371' then
            return('Valores de medida da geometria LRS posicionados errados no SDO_ORDINATES ');
         when '13373' then
            return('Retangulo formado por 2 pontos com dados geograficos não é suportado ');
   
         else
            return(p_resposta);
      end case;
   end;


   function ValidarGeometria(p_geo mdsys.sdo_geometry, idProjeto number) return varchar2 deterministic is
      v_erro varchar2(4000);
      i number := 3;
   begin
      v_erro := sdo_geom.validate_geometry_with_context(p_geo, spa_tolerancia);

      if (v_erro = 'TRUE') then
        v_erro := ValidarGeoDentroAreaAbrang(p_geo, idProjeto);
          if (v_erro = 'TRUE') then
           if (p_geo.sdo_elem_info is not null) then
              while ( i<= p_geo.sdo_elem_info.count) loop
                 if ( (p_geo.sdo_elem_info(i) in (2,4)) and (p_geo.sdo_elem_info(i-1) in (1003,2003,2)) ) then
                    v_erro:= 'A geometria contém arcos';
                    exit;
                 end if;
                 i:= i+3;
              end loop;
           end if;
          end if;
      else
          v_erro := ErroSpatial(v_erro);      
      end if;

      return v_erro;
   end;

   
   function ValidarGtype(geo mdsys.sdo_geometry, gType number) return varchar2 deterministic as
      resposta varchar2(100);
   begin
      
      if (geo.SDO_GTYPE != gType) then 
      
         resposta := 'Era esperado ';
         -----------------------------------------------------------------------------
            case
   
               when gType = 2000 then 
                  resposta := resposta||'uma geometria ignorada espacialmente';
               
               when gType = 2001 then 
                  resposta := resposta||'um ponto';
                  
               when gType = 2002 then 
                  resposta := resposta||'uma linha simples';
                  
               when gType = 2003 then 
                  resposta := resposta||'um polígono simples';
                  
               when gType = 2004 then 
                  resposta := resposta||'geomtrias complexas';
                  
               when gType = 2005 then 
                  resposta := resposta||'um conjunto de pontos';
                  
               when gType = 2006 then 
                  resposta := resposta||'linha complexa';
                  
               when gType = 2007 then 
                  resposta := resposta||'polígono complexo';
            
            end case;
         -----------------------------------------------------------------------------         
         resposta := resposta||', porém veio ';
         -----------------------------------------------------------------------------               
           case
   
               when geo.SDO_GTYPE = 2000 then 
                  resposta := resposta||'uma geometria ignorada espacialmente';
               
               when geo.SDO_GTYPE = 2001 then 
                  resposta := resposta||'um ponto';
                  
               when geo.SDO_GTYPE = 2002 then 
                  resposta := resposta||'uma linha simples';
                  
               when geo.SDO_GTYPE = 2003 then 
                  resposta := resposta||'um polígono simples';
                  
               when geo.SDO_GTYPE = 2004 then 
                  resposta := resposta||'geomtrias complexas';
                  
               when geo.SDO_GTYPE = 2005 then 
                  resposta := resposta||'um conjunto de pontos';
                  
               when geo.SDO_GTYPE = 2006 then 
                  resposta := resposta||'linha complexa';
                  
               when geo.SDO_GTYPE = 2007 then 
                  resposta := resposta||'polígono complexo';
            
            end case;
         -----------------------------------------------------------------------------         
      
      else
         resposta := 'TRUE';         
      end if;
      
      return resposta;
   end;
   
   function ValidarGeoDentroAreaAbrang(p_geo mdsys.sdo_geometry, idProjeto number) return varchar2 is
      v_erro varchar2(250) := 'TRUE';      
   begin
       for i in(select 'FALSE' validado from des_area_abrangencia a
                  where a.id = 1 and sdo_relate(a.geometry, p_geo, 'MASK=CONTAINS QUERYTYPE=WINDOW') != 'TRUE') loop
           v_erro := 'A geometria excedeu o limite da área de abrangência do estado.';
      end loop;
     
     -- if(v_erro = 'TRUE') then
     --  for i in(select 'FALSE' validado from des_area_abrangencia a
     --             where a.projeto = idProjeto and idProjeto > 0 and sdo_relate(a.geometry, p_geo, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') != 'TRUE') loop
    --       v_erro := 'A geometria excedeu o limite da área de abrangência do projeto geográfico.';
      --  end loop;
     
    --  end if;
      return v_erro;
   end;

  procedure InserirFeicao(f_esquema varchar2, f_tabela varchar2, objectid number, f_rascunho varchar2) is
      script varchar2(4000);
   begin
      for i in (select t.column_name, (case t.char_length when 0 then t.data_type else t.data_type||'('|| t.char_length ||')' end) data_type from all_tab_cols t where t.owner=upper(f_esquema) and t.table_name = upper(f_rascunho) and t.column_name not like '%$%' and t.data_type<>'BLOB' and t.column_name not like 'FEICAO') loop
         script:= script || ', '|| i.column_name;
      end loop;

      if (script is not null) then
          script:= substr(script,3);
         for f in (select a.Coluna_Pk, a.id layer_feicao from tab_feicao a where upper(a.tabela) = upper(f_tabela) and upper(a.esquema) = upper(f_esquema) and rownum =1 ) loop
           script:= 'insert into '|| f_esquema ||'.'|| f_tabela ||' ('|| script ||') (select '|| script ||' from '|| f_rascunho ||' where '|| f.coluna_pk ||' = '|| objectid ||' and feicao  = ' || f.layer_feicao||')' ;
           execute immediate script;
         end loop;
      end if;
   end;
   
   procedure AtualizarGeometriaFeicao(f_esquema varchar2, f_tabela varchar2, objectid number, f_rascunho varchar2) is
      script varchar2(4000);
   begin
     for f in ( select a.Coluna_Pk, a.id layer_feicao from tab_feicao a where upper(a.tabela) = upper(f_tabela) and upper(a.esquema) = upper(f_esquema) and rownum =1) loop
              script:= 'update '|| f_esquema ||'.'|| f_tabela ||' set  GEOMETRY = (select GEOMETRY from '|| f_rascunho ||' where '|| f.coluna_pk ||' = '|| objectid ||' and feicao  = ' || f.layer_feicao||') where '|| f.coluna_pk ||' = '|| objectid;
               execute immediate script;  
     end loop;
  end;
end DesenhadorWS;
/
