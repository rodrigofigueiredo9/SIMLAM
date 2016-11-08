create or replace package OperacoesProcessamentoGeo is

   spa_tolerancia constant number := 0.01001;
   spa_tolerancia_area constant number := 0.001;
   spa_tolerancia_overlap constant number := 500;
   
   spa_buffer_nascente constant number := 50;
   spa_buffer_escarpa  constant number := 100;
   spa_buffer_brejo    constant number := 30;

   type t_cursor is ref cursor;

   function  ErroSpatial(p_resposta varchar2) return varchar2 deterministic;
   function  ValidarGeometria(p_geo mdsys.sdo_geometry) return varchar2 deterministic;
   function  ValidarGtype(geo mdsys.sdo_geometry, gType number) return varchar2 deterministic;
   function  Concatenar(texto1 varchar2, texto2 varchar2) return varchar2 deterministic;
   
   function GerarBuffer(geo mdsys.sdo_geometry, dist number, tol number) return mdsys.sdo_geometry;

	procedure ApagarGeometriasTrackmaker(id_projeto number);

	procedure Processar(id_projeto number, tipo number);
   procedure ProcessarValidacao(id_projeto number, tipo number);

   procedure ImportarDoTrackmaker(id_projeto number, tipo number);
   procedure ImportarDoDesenhador(id_projeto number, tipo number);
   procedure ImportarParaDesenhProcessada(id_projeto number, tipo number);
    procedure ImportarParaDesenhFinalizada(id_projeto number, tipo number);
   
   procedure ExportarParaTabelasGEO(id_projeto number, tid_code varchar2);
   procedure GerarHistoricoGEO(id_projeto number, p_acao number, p_executor_id number, p_executor_nome varchar2, p_executor_login varchar2, p_executor_tipo_id number, p_executor_tid varchar2);
end;
/
create or replace package body OperacoesProcessamentoGeo is

   type t_array_geo is table of mdsys.sdo_geometry;
	
   TIPO_DOMINIALIDADE constant number := 3;
	TIPO_ATIVIDADE constant number := 4;


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
            return('A geometria de polígono tem menos que quatro coordenadas ');
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


   function ValidarGeometria(p_geo mdsys.sdo_geometry) return varchar2 deterministic is
      v_erro varchar2(4000);
      i number := 3;
   begin
      v_erro := sdo_geom.validate_geometry_with_context(p_geo, spa_tolerancia);

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



   function Concatenar(texto1 varchar2, texto2 varchar2) return varchar2 deterministic as
      resposta varchar2(32767);
   begin
      if ((texto2 is not null) and (length(trim(texto2))>0)) then
         if ((texto1 is not null) and (length(trim(texto1))>0)) then
            resposta:= texto1 ||', '|| texto2;
         else
            resposta:= texto2;
         end if;
      else
         resposta:= texto1;
      end if;

      return resposta;
   end;

	function parse_number(numero varchar2) return number as
   begin
      begin
         return to_number(numero);
      exception
         when others then
            begin
	            return to_number( replace(numero,'.',',') );
            exception
		         when others then
		            return to_number( replace(numero,',','.') );
            end;
      end;
   end;


 --------------------------------------------------------------------------------
   ------  Função que une espacialmente todas as geometrias de uma lista com uma outra
   --------------------------------------------------------------------------------
   procedure UnirGeometrias(p_geo in out mdsys.sdo_geometry, p_lista t_array_geo) as
      v_lst_geo t_array_geo := p_lista;
      v_lst_aux t_array_geo;

      idx number;

   begin
      if ((v_lst_geo is not null) and (v_lst_geo.count>0)) then

         for i in 1 ..v_lst_geo.count loop
            begin
               if ( sdo_geom.validate_geometry_with_context(  v_lst_geo(i), spa_tolerancia ) = 'TRUE' ) then
                  v_lst_geo(i) := sdo_geom.sdo_arc_densify( v_lst_geo(i), spa_tolerancia, 'arc_tolerance=0,00001' );
               end if;
            exception
               when others then 
                  null;
            end;
         end loop;
         
         while (v_lst_geo.count>1) loop
            v_lst_aux:= t_array_geo();
            v_lst_aux.extend(ceil(v_lst_geo.count/4));

            for i in 1 .. v_lst_geo.count loop
               idx := ceil(i/4);
               v_lst_aux(idx) := sdo_geom.sdo_union( v_lst_aux(idx), v_lst_geo(i), spa_tolerancia );
            end loop;
            v_lst_geo := v_lst_aux;

         end loop;

         p_geo := sdo_geom.sdo_union( p_geo, v_lst_geo(1), spa_tolerancia );
      end if;
   end;
   --------------------------------------------------------------------------------


   --------------------------------------------------------------------------------
   ------  Função que separa os elementos contidos em uma geometria de tipo complexo.
   --------------------------------------------------------------------------------
   function SepararGeometrias(p_geo mdsys.sdo_geometry, p_gtype number default null) return t_array_geo as
      v_lst_geo t_array_geo:= t_array_geo();
      v_geo mdsys.sdo_geometry := p_geo;
      tamanho number;
   begin
      
      if p_geo is not null then 
      
         if (p_gtype is not null) then
            v_geo.sdo_gtype := p_gtype;
         end if;

         tamanho := geometria9i.subElementos(v_geo);
         
         for i in 1 .. tamanho loop
            v_lst_geo.extend();
            v_lst_geo(v_lst_geo.count):= geometria9i.extrair(v_geo, i);
            
            if ( v_lst_geo(v_lst_geo.count).sdo_point is not null ) then
               v_lst_geo(v_lst_geo.count).sdo_gtype:= 2001;
            else
               v_lst_geo(v_lst_geo.count).sdo_gtype:= (case (v_lst_geo(v_lst_geo.count).sdo_elem_info(2))
                  when 1 then
                     2001
                  when 2 then
                     2002
                  when 4 then
                     2002
                  when 1003 then
                     2003
                  when 1005 then
                     2003
                  else
                     2000
               end);
               
            end if;
            
         end loop;
      
      end if;
      
      return v_lst_geo;
   end;
   --------------------------------------------------------------------------------


   --------------------------------------------------------------------------------
   ------  Limpa o lixo dos polígonos
   --------------------------------------------------------------------------------
   procedure LimparPoligono(p_geo in out mdsys.sdo_geometry) is
   begin
      if (p_geo is not null) then
         if (p_geo.sdo_gtype <> 2003) then
            p_geo.sdo_gtype := 2007;
         end if;
   
         begin
            p_geo := geometria9i.extrair(p_geo);
            if ( sdo_geom.validate_geometry_with_context( p_geo, spa_tolerancia ) <> 'TRUE' ) then
               p_geo:= null;
            else
               p_geo := sdo_geom.sdo_arc_densify( p_geo, spa_tolerancia, 'arc_tolerance=0,00001' );
               if ( sdo_geom.validate_geometry_with_context( p_geo, spa_tolerancia ) <> 'TRUE' ) then
                  p_geo:= null;
               end if;
            end if;
         exception
            when others then
               p_geo:= null;
         end;

      end if;
   end;
   --------------------------------------------------------------------------------


   --------------------------------------------------------------------------------
   ------  Função que gera o buffer 
   --------------------------------------------------------------------------------   
   function GerarBuffer(geo mdsys.sdo_geometry, dist number, tol number) return mdsys.sdo_geometry is
      Result mdsys.sdo_geometry;
   begin
      begin
         return sdo_geom.sdo_buffer(geo, dist, tol);
      exception
         when others then begin
            return sdo_geom.sdo_buffer(geo, dist, tol*10);
         exception
            when others then begin
               return sdo_geom.sdo_buffer(geo, dist, tol*100);
            exception
               when others then begin
                  return sdo_geom.sdo_buffer(geo, dist, tol/10);
               exception
                  when others then 
                     return sdo_geom.sdo_buffer(geo, dist, tol/100);
               end;
            end;
         end;
      end;
      
     return(Result);
   end;
   --------------------------------------------------------------------------------




   procedure CalcularAPP(id_projeto number) is
      v_lst_app t_array_geo;
      v_lst_geo t_array_geo;
      v_lst_ma  t_array_geo;
      v_app     mdsys.sdo_geometry;
      v_ma      mdsys.sdo_geometry;

		v_intersect mdsys.sdo_geometry;
   begin

      ----------------------------------------------------
      -- Nascentes
      select GerarBuffer(t.geometry, spa_buffer_nascente, spa_tolerancia ) geo
         bulk collect into v_lst_geo
      from TMP_NASCENTE t
         where t.projeto = id_projeto;

      unirGeometrias(v_app, v_lst_geo);
      ----------------------------------------------------
      
      
      ----------------------------------------------------
      -- Rio Linha
      select GerarBuffer(t.geometry, (case when (t.largura<10) then 30 else 50 end), spa_tolerancia ) geo
         bulk collect into v_lst_geo
      from TMP_RIO_LINHA t 
         where t.projeto = id_projeto;

      unirGeometrias(v_app, v_lst_geo);
      ----------------------------------------------------


      ----------------------------------------------------
      -- Rio Area
      select GerarBuffer(t.geometry, (case when (t.largura<10) then 30 when (t.largura<=50) then 50 when (t.largura<=200) then 100 when (t.largura<=600) then 200 else 500 end), spa_tolerancia ) geo, t.geometry
         bulk collect into v_lst_geo, v_lst_ma
      from TMP_RIO_AREA t
         where t.projeto = id_projeto;

      unirGeometrias(v_app, v_lst_geo);
      unirGeometrias(v_ma, v_lst_ma);
      ----------------------------------------------------


      ----------------------------------------------------
      -- Lagoas
      select GerarBuffer(t.geometry, (case when (upper(t.zona)='A') then 100 when (upper(t.zona)='U') then 30 when (t.area_m2<=200000) then 50 else 100 end), spa_tolerancia ) geo, t.geometry
         bulk collect into v_lst_geo, v_lst_ma
      from TMP_LAGOA t
         where t.projeto = id_projeto;

      unirGeometrias(v_app, v_lst_geo);
      unirGeometrias(v_ma, v_lst_ma);
      ----------------------------------------------------


      ----------------------------------------------------
      -- Represa
      select (case when (t.amortecimento>0) then GerarBuffer(t.geometry, t.amortecimento, spa_tolerancia ) else null end) geo, t.geometry
         bulk collect into v_lst_geo, v_lst_ma
      from TMP_REPRESA t
         where t.projeto = id_projeto;

      unirGeometrias(v_app, v_lst_geo);
      unirGeometrias(v_ma, v_lst_ma);
      ----------------------------------------------------

      
      ----------------------------------------------------
      -- Dunas
      select t.geometry geo
         bulk collect into v_lst_geo
      from TMP_DUNA t
         where t.projeto = id_projeto;

      unirGeometrias(v_app, v_lst_geo);
      ----------------------------------------------------
      
      
      ----------------------------------------------------
      -- Restrição de declividade
      select t.geometry geo
         bulk collect into v_lst_geo
      from TMP_REST_DECLIVIDADE t
         where t.projeto = id_projeto;

      unirGeometrias(v_app, v_lst_geo);
      ---------------------------------------------------- 


      ----------------------------------------------------
      -- Escarpa
      select GerarBuffer(t.geometry, spa_buffer_escarpa, spa_tolerancia ) geo
         bulk collect into v_lst_geo
      from TMP_ESCARPA t
         where t.projeto = id_projeto;

      unirGeometrias(v_app, v_lst_geo);
      ----------------------------------------------------


      ----------------------------------------------------
      -- Área de Classificação Vegetal
      select (case when tipo='BREJO' then GerarBuffer(t.geometry, spa_buffer_brejo, spa_tolerancia ) else t.geometry end) geo
         bulk collect into v_lst_geo
      from TMP_ACV t
         where t.projeto = id_projeto and upper(t.tipo) in ('MANGUE', 'BREJO', 'RESTINGA-APP');

      unirGeometrias(v_app, v_lst_geo);
      ----------------------------------------------------


      v_app := sdo_geom.sdo_difference( v_app, v_ma, spa_tolerancia );

      v_lst_app := new t_array_geo();
      ----------------------------------------------------
      -- Matriculas/Posses
      for i in ( select t.geometry geo, t.id cod_apmp from TMP_APMP t 
                  where t.projeto = id_projeto and sdo_relate(t.geometry, v_app, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE') loop

			-- APP por APMP
         v_intersect := sdo_geom.sdo_intersection(v_app, i.geo, spa_tolerancia);

         v_lst_geo := SepararGeometrias(v_intersect, 2007);
         if ((v_lst_geo is not null) and (v_lst_geo.count>0)) then
            for j in 1 ..v_lst_geo.count loop
               insert into TMP_AREAS_CALCULADAS (id, projeto, cod_apmp, tipo, area_m2, geometry) 
               	values (SEQ_TMP_AREAS_CALCULADAS.nextval, id_projeto, i.cod_apmp, 'APP_APMP', sdo_geom.sdo_area(v_lst_geo(j), spa_tolerancia_area), v_lst_geo(j) );

               v_lst_app.extend();
               v_lst_app(v_lst_app.count) := v_lst_geo(j);
            end loop;
         end if;


         -- MA por APMP
         v_intersect := sdo_geom.sdo_intersection(v_ma, i.geo, spa_tolerancia);

         v_lst_geo := SepararGeometrias(v_intersect, 2007);
         if ((v_lst_geo is not null) and (v_lst_geo.count>0)) then
            for j in 1 ..v_lst_geo.count loop
               insert into TMP_AREAS_CALCULADAS (id, projeto, cod_apmp, tipo, area_m2, geometry) 
               	values (SEQ_TMP_AREAS_CALCULADAS.nextval, id_projeto, i.cod_apmp, 'MASSA_DAGUA_APMP', sdo_geom.sdo_area(v_lst_geo(j), spa_tolerancia_area), v_lst_geo(j) );
            end loop;
         end if;

      end loop;

   end;


   procedure ApagarGeometriasTMP(id_projeto number) is
   begin
      -------------------------------------------
      -- Cadastro da Propriedade
      
      -- Limites
      delete TMP_ATP t where t.projeto = id_projeto;
		delete TMP_APMP t where t.projeto = id_projeto;
		delete TMP_AFD t where t.projeto = id_projeto;
		delete TMP_ROCHA t where t.projeto = id_projeto;
		delete TMP_VERTICE t where t.projeto = id_projeto;
		delete TMP_ARL t where t.projeto = id_projeto;
		delete TMP_RPPN t where t.projeto = id_projeto;
		delete TMP_AFS t where t.projeto = id_projeto;
		delete TMP_AVN t where t.projeto = id_projeto;
		delete TMP_AA t where t.projeto = id_projeto;
		delete TMP_ACV t where t.projeto = id_projeto;
      
      -- Outros
		delete TMP_ACONSTRUIDA t where t.projeto = id_projeto;
		delete TMP_DUTO t where t.projeto = id_projeto;
		delete TMP_LTRANSMISSAO t where t.projeto = id_projeto;
		delete TMP_ESTRADA t where t.projeto = id_projeto;
		delete TMP_FERROVIA t where t.projeto = id_projeto;
      
      -- Itens da APP
		delete TMP_NASCENTE t where t.projeto = id_projeto;
		delete TMP_RIO_LINHA t where t.projeto = id_projeto;
		delete TMP_RIO_AREA t where t.projeto = id_projeto;
		delete TMP_LAGOA t where t.projeto = id_projeto;
		delete TMP_REPRESA t where t.projeto = id_projeto;
		delete TMP_DUNA t where t.projeto = id_projeto;
		delete TMP_REST_DECLIVIDADE t where t.projeto = id_projeto;
		delete TMP_ESCARPA t where t.projeto = id_projeto;
      
      -- Calculadas
		delete TMP_AREAS_CALCULADAS t where t.projeto = id_projeto;
      
      
      -------------------------------------------
      -- Atividade

      delete TMP_PATIV t where t.projeto = id_projeto;
		delete TMP_LATIV t where t.projeto = id_projeto;
		delete TMP_AATIV t where t.projeto = id_projeto;
      delete TMP_AIATIV t where t.projeto = id_projeto;

		commit;
   end;
   

   procedure ApagarGeometriasDES(id_projeto number) is
   begin
      -------------------------------------------
      -- Cadastro da Propriedade
      
      -- Limites
      delete DES_ATP t where t.projeto = id_projeto;
		delete DES_APMP t where t.projeto = id_projeto;
		delete DES_AFD t where t.projeto = id_projeto;
		delete DES_ROCHA t where t.projeto = id_projeto;
		delete DES_VERTICE t where t.projeto = id_projeto;
		delete DES_ARL t where t.projeto = id_projeto;
		delete DES_RPPN t where t.projeto = id_projeto;
		delete DES_AFS t where t.projeto = id_projeto;
		delete DES_AVN t where t.projeto = id_projeto;
		delete DES_AA t where t.projeto = id_projeto;
		delete DES_ACV t where t.projeto = id_projeto;
      
      -- Outros
		delete DES_ACONSTRUIDA t where t.projeto = id_projeto;
		delete DES_DUTO t where t.projeto = id_projeto;
		delete DES_LTRANSMISSAO t where t.projeto = id_projeto;
		delete DES_ESTRADA t where t.projeto = id_projeto;
		delete DES_FERROVIA t where t.projeto = id_projeto;
      
      -- Itens da APP
		delete DES_NASCENTE t where t.projeto = id_projeto;
		delete DES_RIO_LINHA t where t.projeto = id_projeto;
		delete DES_RIO_AREA t where t.projeto = id_projeto;
		delete DES_LAGOA t where t.projeto = id_projeto;
		delete DES_REPRESA t where t.projeto = id_projeto;
		delete DES_DUNA t where t.projeto = id_projeto;
		delete DES_REST_DECLIVIDADE t where t.projeto = id_projeto;
		delete DES_ESCARPA t where t.projeto = id_projeto;

		-------------------------------------------
      -- Atividade

      delete DES_PATIV t where t.projeto = id_projeto;
		delete DES_LATIV t where t.projeto = id_projeto;
		delete DES_AATIV t where t.projeto = id_projeto;
      delete DES_AIATIV t where t.projeto = id_projeto;
      
      commit;
   end;
   
   procedure ApagarGeometriasTrackmaker(id_projeto number) is
   begin
	   delete TMP_RASC_TRACKMAKER t where t.projeto=id_projeto;
      commit;
   end;

	procedure ProcessarTopologia(id_projeto number, tipo number) is
      v_lst_geo t_array_geo;
      v_geo mdsys.sdo_geometry;

		valor_avn varchar2(1);
   begin

      if (tipo = TIPO_DOMINIALIDADE) then
         
      	----------------------------------------------------------------
         -- APMP
         update TMP_APMP a set a.cod_atp = null where a.projeto = id_projeto;
         for i in (select t.id, geometria9i.pontoIdeal(t.geometry,3) geometry from TMP_APMP t where t.projeto = id_projeto) loop
            for j in (select p.id from TMP_ATP p where p.projeto = id_projeto and sdo_relate(p.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE') loop
               update TMP_APMP a set a.cod_atp = j.id where a.id = i.id;
               exit;
            end loop;
         end loop;
         
         -- AA
         update TMP_AA a set a.cod_apmp = null where a.projeto = id_projeto;
         for i in (select t.id, geometria9i.pontoIdeal(t.geometry,3) geometry from TMP_AA t where t.projeto = id_projeto) loop
            for j in (select p.id from TMP_APMP p where p.projeto = id_projeto and sdo_relate(p.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE') loop
               update TMP_AA a set a.cod_apmp = j.id where a.id = i.id;
               exit;
            end loop;
         end loop;
         
         -- ACV
         update TMP_ACV a set a.cod_apmp = null where a.projeto = id_projeto;
         for i in (select t.id, geometria9i.pontoIdeal(t.geometry,3) geometry from TMP_ACV t where t.projeto = id_projeto) loop
            for j in (select p.id from TMP_APMP p where p.projeto = id_projeto and sdo_relate(p.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE') loop
               update TMP_ACV a set a.cod_apmp = j.id where a.id = i.id;
               exit;
            end loop;
         end loop;
      
         -- AFS
         update TMP_AFS a set a.cod_apmp = null where a.projeto = id_projeto;
         for i in (select t.id, geometria9i.pontoIdeal(t.geometry,3) geometry from TMP_AFS t where t.projeto = id_projeto) loop
            for j in (select p.id from TMP_APMP p where p.projeto = id_projeto and sdo_relate(p.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE') loop
               update TMP_AFS a set a.cod_apmp = j.id where a.id = i.id;
               exit;
            end loop;
         end loop;
         
         -- ARL
         update TMP_ARL a set a.cod_apmp = null, a.situacao = null where a.projeto = id_projeto;
         for i in (select t.id, geometria9i.pontoIdeal(t.geometry,3) geometry, null cod_apmp, t.situacao from TMP_ARL t where t.projeto = id_projeto) loop
            i.situacao := 'Não Informado';

            --APMP
            for j in (select p.id from TMP_APMP p where p.projeto = id_projeto and sdo_relate(p.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE') loop
               i.cod_apmp := j.id;
               exit;
            end loop;
            --AA
            for j in (select p.id from TMP_AA p where p.projeto = id_projeto and sdo_relate(p.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE') loop
               i.situacao := 'AA';
               exit;
            end loop;
            --AVN
            if (i.situacao = 'Não Informado') then
               for j in (select p.id from TMP_AVN p where p.projeto = id_projeto and sdo_relate(p.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE') loop
                  i.situacao := 'AVN';
                  exit;
               end loop;
            end if;
            
            update TMP_ARL a set a.cod_apmp = i.cod_apmp, a.situacao=i.situacao where a.id = i.id;
         end loop;
         
         -- AVN
         update TMP_AVN a set a.cod_apmp = null where a.projeto = id_projeto;
         for i in (select t.id, geometria9i.pontoIdeal(t.geometry,3) geometry from TMP_AVN t where t.projeto = id_projeto) loop
            for j in (select p.id from TMP_APMP p where p.projeto = id_projeto and sdo_relate(p.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE') loop
               update TMP_AVN a set a.cod_apmp = j.id where a.id = i.id;
               exit;
            end loop;
         end loop;

         -- Lagoa
         update TMP_LAGOA a set a.cod_apmp = null where a.projeto = id_projeto;
         for i in (select t.id, geometria9i.pontoIdeal(t.geometry,3) geometry from TMP_LAGOA t where t.projeto = id_projeto) loop
            for j in (select p.id from TMP_APMP p where p.projeto = id_projeto and sdo_relate(p.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE') loop
               update TMP_LAGOA a set a.cod_apmp = j.id where a.id = i.id;
               exit;
            end loop;
         end loop;

         -- Represa
         update TMP_REPRESA a set a.cod_apmp = null where a.projeto = id_projeto;
         for i in (select t.id, geometria9i.pontoIdeal(t.geometry,3) geometry from TMP_REPRESA t where t.projeto = id_projeto) loop
            for j in (select p.id from TMP_APMP p where p.projeto = id_projeto and sdo_relate(p.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE') loop
               update TMP_REPRESA a set a.cod_apmp = j.id where a.id = i.id;
               exit;
            end loop;
         end loop;

         -- Rio Area
         update TMP_RIO_AREA a set a.cod_apmp = null where a.projeto = id_projeto;
         for i in (select t.id, geometria9i.pontoIdeal(t.geometry,3) geometry from TMP_RIO_AREA t where t.projeto = id_projeto) loop
            for j in (select p.id from TMP_APMP p where p.projeto = id_projeto and sdo_relate(p.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE') loop
               update TMP_RIO_AREA a set a.cod_apmp = j.id where a.id = i.id;
               exit;
            end loop;
         end loop;

         -- Rocha
         update TMP_ROCHA a set a.cod_apmp = null where a.projeto = id_projeto;
         for i in (select t.id, geometria9i.pontoIdeal(t.geometry,3) geometry from TMP_ROCHA t where t.projeto = id_projeto) loop
            for j in (select p.id from TMP_APMP p where p.projeto = id_projeto and sdo_relate(p.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE') loop
               update TMP_ROCHA a set a.cod_apmp = j.id where a.id = i.id;
               exit;
            end loop;
         end loop;

         -- RPPN
         update TMP_RPPN a set a.cod_apmp = null where a.projeto = id_projeto;
         for i in (select t.id, geometria9i.pontoIdeal(t.geometry,3) geometry from TMP_RPPN t where t.projeto = id_projeto) loop
            for j in (select p.id from TMP_APMP p where p.projeto = id_projeto and sdo_relate(p.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE') loop
               update TMP_RPPN a set a.cod_apmp = j.id where a.id = i.id;
               exit;
            end loop;
         end loop;
      
      
      
      	----------------------------------------------------------------
         -- APMP_APMP
         for i in (select a.geometry geo1, c.geometry geo2 from tmp_apmp a, tmp_apmp c 
            			where a.projeto = id_projeto and a.projeto=c.projeto and a.id!=c.id and 
								sdo_relate(c.geometry, a.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE' ) loop

            v_geo := mdsys.sdo_geom.sdo_intersection(i.geo1, i.geo2, spa_tolerancia);
            v_lst_geo := SepararGeometrias(v_geo, 2007);

            for j in 1 .. v_lst_geo.count loop
               v_geo := v_lst_geo(j);
               LimparPoligono(v_geo);

					if (v_geo is not null) then
                  insert into TMP_AREAS_CALCULADAS (id, projeto, cod_apmp, tipo, area_m2, geometry) 
	               	values (SEQ_TMP_AREAS_CALCULADAS.nextval, id_projeto, null, 'APMP_APMP', sdo_geom.sdo_area(v_lst_geo(j), spa_tolerancia_area), v_lst_geo(j) );
               end if;
            end loop;
         end loop;
         
         -- APMP_AFD
         for i in (select a.geometry geo1, c.geometry geo2 from tmp_apmp a, tmp_afd c 
            			where a.projeto = id_projeto and a.projeto=c.projeto and 
								sdo_relate(c.geometry, a.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE' ) loop

            v_geo := mdsys.sdo_geom.sdo_intersection(i.geo1, i.geo2, spa_tolerancia);
            v_lst_geo := SepararGeometrias(v_geo, 2007);

            for j in 1 .. v_lst_geo.count loop
               v_geo := v_lst_geo(j);
               LimparPoligono(v_geo);

					if (v_geo is not null) then
                  insert into TMP_AREAS_CALCULADAS (id, projeto, cod_apmp, tipo, area_m2, geometry) 
	               	values (SEQ_TMP_AREAS_CALCULADAS.nextval, id_projeto, null, 'APMP_AFD', sdo_geom.sdo_area(v_lst_geo(j), spa_tolerancia_area), v_lst_geo(j) );
               end if;
            end loop;
         end loop;
         
         -- ARL_ARL
         for i in (select a.cod_apmp, a.geometry geo1, c.geometry geo2 from tmp_arl a, tmp_arl c 
            			where a.projeto = id_projeto and a.projeto=c.projeto and a.id!=c.id and 
								sdo_relate(c.geometry, a.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE' ) loop

            v_geo := mdsys.sdo_geom.sdo_intersection(i.geo1, i.geo2, spa_tolerancia);
            v_lst_geo := SepararGeometrias(v_geo, 2007);

            for j in 1 .. v_lst_geo.count loop
               v_geo := v_lst_geo(j);
               LimparPoligono(v_geo);

					if (v_geo is not null) then
                  insert into TMP_AREAS_CALCULADAS (id, projeto, cod_apmp, tipo, area_m2, geometry) 
	               	values (SEQ_TMP_AREAS_CALCULADAS.nextval, id_projeto, null, 'ARL_ARL', sdo_geom.sdo_area(v_lst_geo(j), spa_tolerancia_area), v_lst_geo(j) );
               end if;
            end loop;
         end loop;

         -- ARL_ROCHA
         for i in (select a.geometry geo1, c.geometry geo2 from tmp_arl a, tmp_rocha c 
            			where a.projeto = id_projeto and a.projeto=c.projeto and 
								sdo_relate(c.geometry, a.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE' ) loop

            v_geo := mdsys.sdo_geom.sdo_intersection(i.geo1, i.geo2, spa_tolerancia);
            v_lst_geo := SepararGeometrias(v_geo, 2007);

            for j in 1 .. v_lst_geo.count loop
               v_geo := v_lst_geo(j);
               LimparPoligono(v_geo);

					if (v_geo is not null) then
                  insert into TMP_AREAS_CALCULADAS (id, projeto, cod_apmp, tipo, area_m2, geometry) 
	               	values (SEQ_TMP_AREAS_CALCULADAS.nextval, id_projeto, null, 'ARL_ROCHA', sdo_geom.sdo_area(v_lst_geo(j), spa_tolerancia_area), v_lst_geo(j) );
               end if;
            end loop;
         end loop;
         
         -- AA_AVN
         for i in (select a.geometry geo1, c.geometry geo2 from tmp_aa a, tmp_avn c 
            			where a.projeto = id_projeto and a.projeto=c.projeto and 
								sdo_relate(c.geometry, a.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE' ) loop

            v_geo := mdsys.sdo_geom.sdo_intersection(i.geo1, i.geo2, spa_tolerancia);
            v_lst_geo := SepararGeometrias(v_geo, 2007);

            for j in 1 .. v_lst_geo.count loop
               v_geo := v_lst_geo(j);
               LimparPoligono(v_geo);

					if (v_geo is not null) then
                  insert into TMP_AREAS_CALCULADAS (id, projeto, cod_apmp, tipo, area_m2, geometry) 
	               	values (SEQ_TMP_AREAS_CALCULADAS.nextval, id_projeto, null, 'AA_AVN', sdo_geom.sdo_area(v_lst_geo(j), spa_tolerancia_area), v_lst_geo(j) );
               end if;
            end loop;
         end loop;
         
         ----------------------------------------------------------------
         
         
         
		elsif (tipo = TIPO_ATIVIDADE) then
         
      	-- Setando a atividade e inicializando valores
      	for i in (select c.texto atividade from idaf.tmp_projeto_geo t, idaf.lov_caracterizacao_tipo c where t.id = id_projeto and t.caracterizacao = c.id) loop

            update TMP_PATIV a set 
               a.cod_apmp = null,
               a.atividade = i.atividade,
               a.rocha = 'N',
               a.massa_dagua = 'N',
               a.avn = 'N',
               a.aa = 'N',
               a.afs = 'N',
               a.floresta_plantada = 'N',
               a.arl = 'N',
               a.rppn = 'N',
               a.app = 'N'
            where 
               a.projeto = id_projeto;

            update TMP_LATIV a set 
               a.cod_apmp = null,
               a.atividade = i.atividade,
               a.rocha = 'N',
               a.massa_dagua = 'N',
               a.avn = 'N',
               a.aa = 'N',
               a.afs = 'N',
               a.floresta_plantada = 'N',
               a.arl = 'N',
               a.rppn = 'N',
               a.app = 'N'
            where 
               a.projeto = id_projeto;               

            update TMP_AATIV a set 
               a.cod_apmp = null,
               a.atividade = i.atividade,
               a.rocha = 'N',
               a.massa_dagua = 'N',
               a.avn = 'N',
               a.aa = 'N',
               a.afs = 'N',
               a.floresta_plantada = 'N',
               a.arl = 'N',
               a.rppn = 'N',
               a.app = 'N'
            where 
               a.projeto = id_projeto;
               
            update TMP_AIATIV a set 
               a.cod_apmp = null,
               a.atividade = i.atividade,
               a.rocha = 'N',
               a.massa_dagua = 'N',
               a.avn = 'N',
               a.aa = 'N',
               a.afs = 'N',
               a.floresta_plantada = 'N',
               a.arl = 'N',
               a.rppn = 'N',
               a.app = 'N'
            where
               a.projeto = id_projeto;

         end loop;
      
      
         for k in (select t.id projeto from idaf.crt_projeto_geo t, idaf.tmp_projeto_geo a where t.caracterizacao=1 and t.empreendimento=a.empreendimento and a.id=id_projeto) loop
            ----------------------------------------------------------------
            -- PATIV
            for i in (select t.id, t.geometry geometry from TMP_PATIV t where t.projeto = id_projeto) loop
               for j in (select p.id from GEO_APMP p where p.projeto = k.projeto and sdo_relate(p.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE') loop
                  update TMP_PATIV a set a.cod_apmp = j.id where a.id = i.id;
                  exit;
               end loop;
               
               valor_avn := 'N';
               for j in (select p.estagio from GEO_AVN p where p.projeto = k.projeto and sdo_relate(p.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE') loop
                  if (j.estagio='I') then
                     valor_avn := j.estagio;
                     exit;
                  elsif ( (j.estagio='M') or (valor_avn<>'A') ) then
                     valor_avn := j.estagio;
                  end if;
               end loop;

               
               update TMP_PATIV a set 
                  a.rocha = (select (case when count(t.id)>0 then 'S' else 'N' end) from GEO_ROCHA t where t.projeto = k.projeto and sdo_relate(t.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE'),
                  a.massa_dagua = (select (case when count(t.id)>0 then 'S' else 'N' end) from GEO_AREAS_CALCULADAS t where t.projeto = k.projeto and t.tipo='MASSA_DAGUA_APMP' and sdo_relate(t.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE'),
                  a.avn = valor_avn,
                  a.aa = (select (case when count(t.id)>0 then 'S' else 'N' end) from GEO_AA t where t.projeto = k.projeto and sdo_relate(t.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE'),
                  a.afs = (select (case when count(t.id)>0 then 'S' else 'N' end) from GEO_AFS t where t.projeto = k.projeto and sdo_relate(t.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE'),
                  a.floresta_plantada = (select (case when count(t.id)>0 then 'S' else 'N' end) from GEO_ACV t where t.projeto = k.projeto and t.tipo='FLORESTA_PLANTADA' and sdo_relate(t.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE'),
                  a.arl = (select (case when count(t.id)>0 then 'S' else 'N' end) from GEO_ARL t where t.projeto = k.projeto and sdo_relate(t.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE'),
                  a.rppn = (select (case when count(t.id)>0 then 'S' else 'N' end) from GEO_RPPN t where t.projeto = k.projeto and sdo_relate(t.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE'),
                  a.app = (select (case when count(t.id)>0 then 'S' else 'N' end) from GEO_AREAS_CALCULADAS t where t.projeto = k.projeto and t.tipo='APP_APMP' and sdo_relate(t.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE')
               where
                  a.id = i.id;
               
            end loop;


            -- LATIV
            for i in (select t.id, sdo_lrs.convert_to_std_geom( sdo_lrs.locate_pt( sdo_lrs.convert_to_lrs_geom(t.geometry), sdo_geom.sdo_length(t.geometry, spa_tolerancia_area)/2) ) geometry from TMP_LATIV t where t.projeto = id_projeto) loop
               for j in (select p.id from GEO_APMP p where p.projeto = k.projeto and sdo_relate(p.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE') loop
                  update TMP_LATIV a set a.cod_apmp = j.id where a.id = i.id;
                  exit;
               end loop;
               
               valor_avn := 'N';
               for j in (select p.estagio from GEO_AVN p where p.projeto = k.projeto and sdo_relate(p.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE') loop
                  if (j.estagio='I') then
                     valor_avn := j.estagio;
                     exit;
                  elsif ( (j.estagio='M') or (valor_avn<>'A') ) then
                     valor_avn := j.estagio;
                  end if;
               end loop;
               
               update TMP_LATIV a set 
                  a.rocha = (select (case when count(t.id)>0 then 'S' else 'N' end) from GEO_ROCHA t where t.projeto = k.projeto and sdo_relate(t.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE'),
                  a.massa_dagua = (select (case when count(t.id)>0 then 'S' else 'N' end) from GEO_AREAS_CALCULADAS t where t.projeto = k.projeto and t.tipo='MASSA_DAGUA_APMP' and sdo_relate(t.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE'),
                  a.avn = valor_avn,
                  a.aa = (select (case when count(t.id)>0 then 'S' else 'N' end) from GEO_AA t where t.projeto = k.projeto and sdo_relate(t.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE'),
                  a.afs = (select (case when count(t.id)>0 then 'S' else 'N' end) from GEO_AFS t where t.projeto = k.projeto and sdo_relate(t.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE'),
                  a.floresta_plantada = (select (case when count(t.id)>0 then 'S' else 'N' end) from GEO_ACV t where t.projeto = k.projeto and t.tipo='FLORESTA_PLANTADA' and sdo_relate(t.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE'),
                  a.arl = (select (case when count(t.id)>0 then 'S' else 'N' end) from GEO_ARL t where t.projeto = k.projeto and sdo_relate(t.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE'),
                  a.rppn = (select (case when count(t.id)>0 then 'S' else 'N' end) from GEO_RPPN t where t.projeto = k.projeto and sdo_relate(t.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE'),
                  a.app = (select (case when count(t.id)>0 then 'S' else 'N' end) from GEO_AREAS_CALCULADAS t where t.projeto = k.projeto and t.tipo='APP_APMP' and sdo_relate(t.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE')
               where
                  a.id = i.id;
                  
            end loop;
              

            -- AATIV
            for i in (select t.id, geometria9i.pontoIdeal(t.geometry,3) geometry from TMP_AATIV t where t.projeto = id_projeto) loop
               for j in (select p.id from GEO_APMP p where p.projeto = k.projeto and sdo_relate(p.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE') loop
                  update TMP_AATIV a set a.cod_apmp = j.id where a.id = i.id;
                  exit;
               end loop;
               
               valor_avn := 'N';
               for j in (select p.estagio from GEO_AVN p where p.projeto = k.projeto and sdo_relate(p.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE') loop
                  if (j.estagio='I') then
                     valor_avn := j.estagio;
                     exit;
                  elsif ( (j.estagio='M') or (valor_avn<>'A') ) then
                     valor_avn := j.estagio;
                  end if;
               end loop;
               
               update TMP_AATIV a set 
                  a.rocha = (select (case when count(t.id)>0 then 'S' else 'N' end) from GEO_ROCHA t where t.projeto = k.projeto and sdo_relate(t.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE'),
                  a.massa_dagua = (select (case when count(t.id)>0 then 'S' else 'N' end) from GEO_AREAS_CALCULADAS t where t.projeto = k.projeto and t.tipo='MASSA_DAGUA_APMP' and sdo_relate(t.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE'),
                  a.avn = valor_avn,
                  a.aa = (select (case when count(t.id)>0 then 'S' else 'N' end) from GEO_AA t where t.projeto = k.projeto and sdo_relate(t.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE'),
                  a.afs = (select (case when count(t.id)>0 then 'S' else 'N' end) from GEO_AFS t where t.projeto = k.projeto and sdo_relate(t.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE'),
                  a.floresta_plantada = (select (case when count(t.id)>0 then 'S' else 'N' end) from GEO_ACV t where t.projeto = k.projeto and t.tipo='FLORESTA_PLANTADA' and sdo_relate(t.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE'),
                  a.arl = (select (case when count(t.id)>0 then 'S' else 'N' end) from GEO_ARL t where t.projeto = k.projeto and sdo_relate(t.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE'),
                  a.rppn = (select (case when count(t.id)>0 then 'S' else 'N' end) from GEO_RPPN t where t.projeto = k.projeto and sdo_relate(t.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE'),
                  a.app = (select (case when count(t.id)>0 then 'S' else 'N' end) from GEO_AREAS_CALCULADAS t where t.projeto = k.projeto and t.tipo='APP_APMP' and sdo_relate(t.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE')
               where
                  a.id = i.id;
                  
            end loop;


            -- AIATIV
            for i in (select t.id, geometria9i.pontoIdeal(t.geometry,3) geometry from TMP_AIATIV t where t.projeto = id_projeto) loop
               for j in (select p.id from GEO_APMP p where p.projeto = k.projeto and sdo_relate(p.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE') loop
                  update TMP_AIATIV a set a.cod_apmp = j.id where a.id = i.id;
                  exit;
               end loop;
               
               valor_avn := 'N';
               for j in (select p.estagio from GEO_AVN p where p.projeto = k.projeto and sdo_relate(p.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE') loop
                  if (j.estagio='I') then
                     valor_avn := j.estagio;
                     exit;
                  elsif ( (j.estagio='M') or (valor_avn<>'A') ) then
                     valor_avn := j.estagio;
                  end if;
               end loop;
               
               update TMP_AIATIV a set 
                  a.rocha = (select (case when count(t.id)>0 then 'S' else 'N' end) from GEO_ROCHA t where t.projeto = k.projeto and sdo_relate(t.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE'),
                  a.massa_dagua = (select (case when count(t.id)>0 then 'S' else 'N' end) from GEO_AREAS_CALCULADAS t where t.projeto = k.projeto and t.tipo='MASSA_DAGUA_APMP' and sdo_relate(t.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE'),
                  a.avn = valor_avn,
                  a.aa = (select (case when count(t.id)>0 then 'S' else 'N' end) from GEO_AA t where t.projeto = k.projeto and sdo_relate(t.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE'),
                  a.afs = (select (case when count(t.id)>0 then 'S' else 'N' end) from GEO_AFS t where t.projeto = k.projeto and sdo_relate(t.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE'),
                  a.floresta_plantada = (select (case when count(t.id)>0 then 'S' else 'N' end) from GEO_ACV t where t.projeto = k.projeto and t.tipo='FLORESTA_PLANTADA' and sdo_relate(t.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE'),
                  a.arl = (select (case when count(t.id)>0 then 'S' else 'N' end) from GEO_ARL t where t.projeto = k.projeto and sdo_relate(t.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE'),
                  a.rppn = (select (case when count(t.id)>0 then 'S' else 'N' end) from GEO_RPPN t where t.projeto = k.projeto and sdo_relate(t.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE'),
                  a.app = (select (case when count(t.id)>0 then 'S' else 'N' end) from GEO_AREAS_CALCULADAS t where t.projeto = k.projeto and t.tipo='APP_APMP' and sdo_relate(t.geometry, i.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE')
               where
                  a.id = i.id;
                  
            end loop;

            ----------------------------------------------------------------
			end loop;
         
         
         -- AATIV_AATIV
         for i in (select a.cod_apmp, a.geometry geo1, c.geometry geo2 from TMP_AATIV a, TMP_AATIV c 
            			where a.projeto = id_projeto and a.projeto=c.projeto and a.id!=c.id and 
								sdo_relate(c.geometry, a.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE' ) loop

            v_geo := mdsys.sdo_geom.sdo_intersection(i.geo1, i.geo2, spa_tolerancia);
            v_lst_geo := SepararGeometrias(v_geo, 2007);

            for j in 1 .. v_lst_geo.count loop
               v_geo := v_lst_geo(j);
               LimparPoligono(v_geo);

					if (v_geo is not null) then
                  insert into TMP_AREAS_CALCULADAS (id, projeto, cod_apmp, tipo, area_m2, geometry) 
	               	values (SEQ_TMP_AREAS_CALCULADAS.nextval, id_projeto, null, 'AATIV_AATIV', sdo_geom.sdo_area(v_lst_geo(j), spa_tolerancia_area), v_lst_geo(j) );
               end if;
            end loop;
         end loop;
         
      end if;
   end;

   procedure Processar(id_projeto number, tipo number) is
      v_lst_geo t_array_geo;
      v_geo mdsys.sdo_geometry;
      v_tipo number := tipo;

   begin

      if (tipo = TIPO_DOMINIALIDADE) then
         
         -- Limites
         update TMP_ATP t set t.area_m2 = sdo_geom.sdo_area(t.geometry, spa_tolerancia_area) where t.projeto = id_projeto;
         update TMP_APMP t set t.area_m2 = sdo_geom.sdo_area(t.geometry, spa_tolerancia_area) where t.projeto = id_projeto;
         update TMP_AFD t set t.area_m2 = sdo_geom.sdo_area(t.geometry, spa_tolerancia_area) where t.projeto = id_projeto;
         update TMP_ROCHA t set t.area_m2 = sdo_geom.sdo_area(t.geometry, spa_tolerancia_area) where t.projeto = id_projeto;
         update TMP_ARL t set t.codigo='ARL-'||rownum, t.area_m2 = sdo_geom.sdo_area(t.geometry, spa_tolerancia_area) where t.projeto = id_projeto;
         update TMP_RPPN t set t.area_m2 = sdo_geom.sdo_area(t.geometry, spa_tolerancia_area) where t.projeto = id_projeto;
         update TMP_AFS t set t.area_m2 = sdo_geom.sdo_area(t.geometry, spa_tolerancia_area) where t.projeto = id_projeto;
         update TMP_AVN t set t.area_m2 = sdo_geom.sdo_area(t.geometry, spa_tolerancia_area) where t.projeto = id_projeto;
         update TMP_AA t set t.area_m2 = sdo_geom.sdo_area(t.geometry, spa_tolerancia_area) where t.projeto = id_projeto;
         update TMP_ACV t set t.area_m2 = sdo_geom.sdo_area(t.geometry, spa_tolerancia_area) where t.projeto = id_projeto;
         
         -- Outros
         update TMP_ACONSTRUIDA t set t.area_m2 = sdo_geom.sdo_area(t.geometry, spa_tolerancia_area) where t.projeto = id_projeto;
         
         -- Itens da APP
         update TMP_RIO_AREA t set t.area_m2 = sdo_geom.sdo_area(t.geometry, spa_tolerancia_area) where t.projeto = id_projeto;
         update TMP_LAGOA t set t.area_m2 = sdo_geom.sdo_area(t.geometry, spa_tolerancia_area) where t.projeto = id_projeto;
         update TMP_REPRESA t set t.area_m2 = sdo_geom.sdo_area(t.geometry, spa_tolerancia_area) where t.projeto = id_projeto;
         
         
         
         CalcularAPP(id_projeto);
         
         
         -- APP_AA
         for i in (select a.cod_apmp, a.geometry geo1, c.geometry geo2 from tmp_aa a, tmp_areas_calculadas c 
            			where a.projeto = id_projeto and a.cod_apmp=c.cod_apmp and c.tipo='APP_APMP' and 
								sdo_relate(c.geometry, a.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE' ) loop

            v_geo := mdsys.sdo_geom.sdo_intersection(i.geo1, i.geo2, spa_tolerancia);
            v_lst_geo := SepararGeometrias(v_geo, 2007);

            for j in 1 .. v_lst_geo.count loop
               v_geo := v_lst_geo(j);
               LimparPoligono(v_geo);

					if (v_geo is not null) then
                  insert into TMP_AREAS_CALCULADAS (id, projeto, cod_apmp, tipo, area_m2, geometry) 
	               	values (SEQ_TMP_AREAS_CALCULADAS.nextval, id_projeto, i.cod_apmp, 'APP_AA', sdo_geom.sdo_area(v_lst_geo(j), spa_tolerancia_area), v_lst_geo(j) );
               end if;
            end loop;
         end loop;
         
         
         -- APP_AVN
         for i in (select a.cod_apmp, a.geometry geo1, c.geometry geo2 from tmp_avn a, tmp_areas_calculadas c 
            			where a.projeto = id_projeto and a.cod_apmp=c.cod_apmp and c.tipo='APP_APMP' and 
								sdo_relate(c.geometry, a.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE' ) loop

            v_geo := mdsys.sdo_geom.sdo_intersection(i.geo1, i.geo2, spa_tolerancia);
            v_lst_geo := SepararGeometrias(v_geo, 2007);

            for j in 1 .. v_lst_geo.count loop
               v_geo := v_lst_geo(j);
               LimparPoligono(v_geo);

					if (v_geo is not null) then
                  insert into TMP_AREAS_CALCULADAS (id, projeto, cod_apmp, tipo, area_m2, geometry) 
	               	values (SEQ_TMP_AREAS_CALCULADAS.nextval, id_projeto, i.cod_apmp, 'APP_AVN', sdo_geom.sdo_area(v_lst_geo(j), spa_tolerancia_area), v_lst_geo(j) );
               end if;
            end loop;
         end loop;
         
         -- APP_ARL
         for i in (select a.cod_apmp, a.geometry geo1, c.geometry geo2 from tmp_arl a, tmp_areas_calculadas c 
            			where a.projeto = id_projeto and a.cod_apmp=c.cod_apmp and c.tipo='APP_APMP' and 
								sdo_relate(c.geometry, a.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE' ) loop

            v_geo := mdsys.sdo_geom.sdo_intersection(i.geo1, i.geo2, spa_tolerancia);
            v_lst_geo := SepararGeometrias(v_geo, 2007);

            for j in 1 .. v_lst_geo.count loop
               v_geo := v_lst_geo(j);
               LimparPoligono(v_geo);

					if (v_geo is not null) then
                  insert into TMP_AREAS_CALCULADAS (id, projeto, cod_apmp, tipo, area_m2, geometry) 
	               	values (SEQ_TMP_AREAS_CALCULADAS.nextval, id_projeto, i.cod_apmp, 'APP_ARL', sdo_geom.sdo_area(v_lst_geo(j), spa_tolerancia_area), v_lst_geo(j) );
               end if;
            end loop;
         end loop;



		elsif (tipo = TIPO_ATIVIDADE) then

      	update TMP_PATIV t set t.codigo='P-'||rownum where t.projeto=id_projeto;
         update TMP_LATIV t set t.codigo='L-'||rownum, t.comprimento=sdo_geom.sdo_length(t.geometry, spa_tolerancia_area) where t.projeto=id_projeto;
         update TMP_AATIV t set t.codigo='A-'||rownum, t.area_m2=sdo_geom.sdo_area(t.geometry, spa_tolerancia_area) where t.projeto=id_projeto;
         update TMP_AIATIV t set t.codigo='AI-'||rownum, t.area_m2=sdo_geom.sdo_area(t.geometry, spa_tolerancia_area) where t.projeto=id_projeto;
         
      end if;

		update tab_navegador_projeto p set p.is_valido_proces = (select count(*) from tab_fila t where t.projeto = id_projeto and t.tipo = v_tipo and t.mecanismo_elaboracao=2)
      	where p.projeto = id_projeto;
		commit;
   end;
   
   procedure ProcessarValidacao (id_projeto number, tipo number) is
   begin
      delete from tab_validacao_geo t where t.projeto=id_projeto;
      
      ProcessarTopologia(id_projeto, tipo);
      
      if (tipo = TIPO_DOMINIALIDADE) then
         
         --============================================================================================
         -- Erros Espaciais
         --============================================================================================
         for i in (select * from (

					--Área Total da Propriedade
               select 'ATP' tabela, t.projeto, '' codigo_mensagem, ErroSpatial(ValidarGeometria(t.geometry)) descricao_mensagem from TMP_ATP t where ValidarGeometria(t.geometry)<>'TRUE'
               union all
               select 'ATP' tabela, t.projeto, '' codigo_mensagem, ValidarGtype(t.geometry, 2003) descricao_mensagem from TMP_ATP t where ValidarGtype(t.geometry, 2003)<>'TRUE'
               
               union all

					--Matrícula ou Posse
               select 'APMP' tabela, t.projeto, '' codigo_mensagem,  ErroSpatial(ValidarGeometria(t.geometry)) descricao_mensagem from TMP_APMP t where ValidarGeometria(t.geometry)<>'TRUE'
               union all
               select 'APMP' tabela, t.projeto, '' codigo_mensagem,   ValidarGtype(t.geometry, 2003) descricao_mensagem from TMP_APMP t where ValidarGtype(t.geometry, 2003)<>'TRUE'
               
               union all

					--Área da faixa de dominio
               select 'AFD' tabela, t.projeto, '' codigo_mensagem,  ErroSpatial(ValidarGeometria(t.geometry)) descricao_mensagem from TMP_AFD t where ValidarGeometria(t.geometry)<>'TRUE'
               union all
               select 'AFD' tabela, t.projeto, '' codigo_mensagem,   ValidarGtype(t.geometry, 2003) descricao_mensagem from TMP_AFD t where ValidarGtype(t.geometry, 2003)<>'TRUE'
               
               union all

					-- Rocha
               select 'ROCHA' tabela, t.projeto, '' codigo_mensagem,  ErroSpatial(ValidarGeometria(t.geometry)) descricao_mensagem from TMP_ROCHA t where ValidarGeometria(t.geometry)<>'TRUE'
               union all
               select 'ROCHA' tabela, t.projeto, '' codigo_mensagem,   ValidarGtype(t.geometry, 2003) descricao_mensagem from TMP_ROCHA t where ValidarGtype(t.geometry, 2003)<>'TRUE'
               
               union all

					-- Vertices de APMP
               select 'VERTICE' tabela, t.projeto, '' codigo_mensagem,  ErroSpatial(ValidarGeometria(t.geometry)) descricao_mensagem from TMP_VERTICE t where ValidarGeometria(t.geometry)<>'TRUE'
               union all
               select 'VERTICE' tabela, t.projeto, '' codigo_mensagem,   ValidarGtype(t.geometry, 2001) descricao_mensagem from TMP_VERTICE t where ValidarGtype(t.geometry, 2001)<>'TRUE'
               
               union all

					-- Area de Reserva Legal
               select 'ARL' tabela, t.projeto, '' codigo_mensagem,  ErroSpatial(ValidarGeometria(t.geometry)) descricao_mensagem from TMP_ARL t where ValidarGeometria(t.geometry)<>'TRUE'
               union all
               select 'ARL' tabela, t.projeto, '' codigo_mensagem,   ValidarGtype(t.geometry, 2003) descricao_mensagem from TMP_ARL t where ValidarGtype(t.geometry, 2003)<>'TRUE'
               
               union all

					-- Reserva Particular de Patrimônio Natural
               select 'RPPN' tabela, t.projeto, '' codigo_mensagem,  ErroSpatial(ValidarGeometria(t.geometry)) descricao_mensagem from TMP_RPPN t where ValidarGeometria(t.geometry)<>'TRUE'
               union all
               select 'RPPN' tabela, t.projeto, '' codigo_mensagem,   ValidarGtype(t.geometry, 2003) descricao_mensagem from TMP_RPPN t where ValidarGtype(t.geometry, 2003)<>'TRUE'
               
               union all

					-- Área de Faixa de Servidão
               select 'AFS' tabela, t.projeto, '' codigo_mensagem,  ErroSpatial(ValidarGeometria(t.geometry)) descricao_mensagem from TMP_AFS t where ValidarGeometria(t.geometry)<>'TRUE'
               union all
               select 'AFS' tabela, t.projeto, '' codigo_mensagem,   ValidarGtype(t.geometry, 2003) descricao_mensagem from TMP_AFS t where ValidarGtype(t.geometry, 2003)<>'TRUE'
               
               union all

					-- Área de Vegetação Nativa
               select 'AVN' tabela, t.projeto, '' codigo_mensagem,  ErroSpatial(ValidarGeometria(t.geometry)) descricao_mensagem from TMP_AVN t where ValidarGeometria(t.geometry)<>'TRUE'
               union all
               select 'AVN' tabela, t.projeto, '' codigo_mensagem,   ValidarGtype(t.geometry, 2003) descricao_mensagem from TMP_AVN t where ValidarGtype(t.geometry, 2003)<>'TRUE'
               
               union all

					-- Área Aberta
               select 'AA' tabela, t.projeto, '' codigo_mensagem,  ErroSpatial(ValidarGeometria(t.geometry)) descricao_mensagem from TMP_AA t where ValidarGeometria(t.geometry)<>'TRUE'
               union all
               select 'AA' tabela, t.projeto, '' codigo_mensagem,   ValidarGtype(t.geometry, 2003) descricao_mensagem from TMP_AA t where ValidarGtype(t.geometry, 2003)<>'TRUE'
               
               union all

					-- Área de Classificação de Vegetação
               select 'ACV' tabela, t.projeto, '' codigo_mensagem,  ErroSpatial(ValidarGeometria(t.geometry)) descricao_mensagem from TMP_ACV t where ValidarGeometria(t.geometry)<>'TRUE'
               union all
               select 'ACV' tabela, t.projeto, '' codigo_mensagem,   ValidarGtype(t.geometry, 2003) descricao_mensagem from TMP_ACV t where ValidarGtype(t.geometry, 2003)<>'TRUE'
               
               union all


					----------------------------------------------------
					-- Outras 

					-- Área Construída
               select 'ACONSTRUIDA' tabela, t.projeto, '' codigo_mensagem,  ErroSpatial(ValidarGeometria(t.geometry)) descricao_mensagem from TMP_ACONSTRUIDA t where ValidarGeometria(t.geometry)<>'TRUE'
               union all
               select 'ACONSTRUIDA' tabela, t.projeto, '' codigo_mensagem,   ValidarGtype(t.geometry, 2003) descricao_mensagem from TMP_ACONSTRUIDA t where ValidarGtype(t.geometry, 2003)<>'TRUE'
               
               union all

					-- Linhas de DUTO
               select 'DUTO' tabela, t.projeto, '' codigo_mensagem,  ErroSpatial(ValidarGeometria(t.geometry)) descricao_mensagem from TMP_DUTO t where ValidarGeometria(t.geometry)<>'TRUE'
               union all
               select 'DUTO' tabela, t.projeto, '' codigo_mensagem,   ValidarGtype(t.geometry, 2002) descricao_mensagem from TMP_DUTO t where ValidarGtype(t.geometry, 2002)<>'TRUE'
               
               union all

					-- Linhas de TRANSMISSAO
               select 'LTRANSMISSAO' tabela, t.projeto, '' codigo_mensagem,  ErroSpatial(ValidarGeometria(t.geometry)) descricao_mensagem from TMP_LTRANSMISSAO t where ValidarGeometria(t.geometry)<>'TRUE'
               union all
               select 'LTRANSMISSAO' tabela, t.projeto, '' codigo_mensagem,   ValidarGtype(t.geometry, 2002) descricao_mensagem from TMP_LTRANSMISSAO t where ValidarGtype(t.geometry, 2002)<>'TRUE'
               
               union all

					-- Linhas de ESTRADA
               select 'ESTRADA' tabela, t.projeto, '' codigo_mensagem,  ErroSpatial(ValidarGeometria(t.geometry)) descricao_mensagem from TMP_ESTRADA t where ValidarGeometria(t.geometry)<>'TRUE'
               union all
               select 'ESTRADA' tabela, t.projeto, '' codigo_mensagem,   ValidarGtype(t.geometry, 2002) descricao_mensagem from TMP_ESTRADA t where ValidarGtype(t.geometry, 2002)<>'TRUE'
               
               union all

					-- Linhas de FERROVIA
               select 'FERROVIA' tabela, t.projeto, '' codigo_mensagem,  ErroSpatial(ValidarGeometria(t.geometry)) descricao_mensagem from TMP_FERROVIA t where ValidarGeometria(t.geometry)<>'TRUE'
               union all
               select 'FERROVIA' tabela, t.projeto, '' codigo_mensagem,   ValidarGtype(t.geometry, 2002) descricao_mensagem from TMP_FERROVIA t where ValidarGtype(t.geometry, 2002)<>'TRUE'
               
               union all

					----------------------------------------------------
					-- Areas que dao origem a APP

					-- Nascente
               select 'NASCENTE' tabela, t.projeto, '' codigo_mensagem,  ErroSpatial(ValidarGeometria(t.geometry)) descricao_mensagem from TMP_NASCENTE t where ValidarGeometria(t.geometry)<>'TRUE'
               union all
               select 'NASCENTE' tabela, t.projeto, '' codigo_mensagem,   ValidarGtype(t.geometry, 2001) descricao_mensagem from TMP_NASCENTE t where ValidarGtype(t.geometry, 2001)<>'TRUE'
               
               union all

					-- Linhas de Rio
               select 'RIO_LINHA' tabela, t.projeto, '' codigo_mensagem,  ErroSpatial(ValidarGeometria(t.geometry)) descricao_mensagem from TMP_RIO_LINHA t where ValidarGeometria(t.geometry)<>'TRUE'
               union all
               select 'RIO_LINHA' tabela, t.projeto, '' codigo_mensagem,   ValidarGtype(t.geometry, 2002) descricao_mensagem from TMP_RIO_LINHA t where ValidarGtype(t.geometry, 2002)<>'TRUE'
               
               union all

					-- Areas de Rio
               select 'RIO_AREA' tabela, t.projeto, '' codigo_mensagem,  ErroSpatial(ValidarGeometria(t.geometry)) descricao_mensagem from TMP_RIO_AREA t where ValidarGeometria(t.geometry)<>'TRUE'
               union all
               select 'RIO_AREA' tabela, t.projeto, '' codigo_mensagem,   ValidarGtype(t.geometry, 2003) descricao_mensagem from TMP_RIO_AREA t where ValidarGtype(t.geometry, 2003)<>'TRUE'
               
               union all

					-- Areas de Lagos e Lagoas
               select 'LAGOA' tabela, t.projeto, '' codigo_mensagem,  ErroSpatial(ValidarGeometria(t.geometry)) descricao_mensagem from TMP_LAGOA t where ValidarGeometria(t.geometry)<>'TRUE'
               union all
               select 'LAGOA' tabela, t.projeto, '' codigo_mensagem,   ValidarGtype(t.geometry, 2003) descricao_mensagem from TMP_LAGOA t where ValidarGtype(t.geometry, 2003)<>'TRUE'
               
               union all

					-- Areas Inundadas de Represas
               select 'REPRESA' tabela, t.projeto, '' codigo_mensagem,  ErroSpatial(ValidarGeometria(t.geometry)) descricao_mensagem from TMP_REPRESA t where ValidarGeometria(t.geometry)<>'TRUE'
               union all
               select 'REPRESA' tabela, t.projeto, '' codigo_mensagem,   ValidarGtype(t.geometry, 2003) descricao_mensagem from TMP_REPRESA t where ValidarGtype(t.geometry, 2003)<>'TRUE'
               
               union all

					-- Areas de Duna
               select 'DUNA' tabela, t.projeto, '' codigo_mensagem,  ErroSpatial(ValidarGeometria(t.geometry)) descricao_mensagem from TMP_DUNA t where ValidarGeometria(t.geometry)<>'TRUE'
               union all
               select 'DUNA' tabela, t.projeto, '' codigo_mensagem,   ValidarGtype(t.geometry, 2003) descricao_mensagem from TMP_DUNA t where ValidarGtype(t.geometry, 2003)<>'TRUE'
               
               union all

					-- Restrição de declividade
               select 'REST_DECLIVIDADE' tabela, t.projeto, '' codigo_mensagem,  ErroSpatial(ValidarGeometria(t.geometry)) descricao_mensagem from TMP_REST_DECLIVIDADE t where ValidarGeometria(t.geometry)<>'TRUE'
               union all
               select 'REST_DECLIVIDADE' tabela, t.projeto, '' codigo_mensagem,   ValidarGtype(t.geometry, 2003) descricao_mensagem from TMP_REST_DECLIVIDADE t where ValidarGtype(t.geometry, 2003)<>'TRUE'
               
               union all

					-- Escarpa
               select 'ESCARPA' tabela, t.projeto, '' codigo_mensagem,  ErroSpatial(ValidarGeometria(t.geometry)) descricao_mensagem from TMP_ESCARPA t where ValidarGeometria(t.geometry)<>'TRUE'
               union all
               select 'ESCARPA' tabela, t.projeto, '' codigo_mensagem,   ValidarGtype(t.geometry, 2003) descricao_mensagem from TMP_ESCARPA t where ValidarGtype(t.geometry, 2003)<>'TRUE'

               ----------------------------------------------------
            ) a where a.projeto = id_projeto) loop
            
            -- tipo=1 , erro espacial
            insert into tab_validacao_geo (projeto, tipo, sigla_tabela, codigo_mensagem, descricao_mensagem)
            	values(id_projeto, 1, i.tabela, i.codigo_mensagem, i.descricao_mensagem);

      	end loop;
         --============================================================================================
         
         
         
         --============================================================================================
         -- Obrigatoriedades
         --============================================================================================
         for i in (
               -- ATP
               select 'ATP' tabela, (case when num=0 then 'Nenhuma ATP encontrada.' else 'Mais de 1 ATP encontrada.' end) descricao_mensagem, '' codigo_mensagem from (select count(*) num from TMP_ATP t where t.projeto = id_projeto) where num <> 1
               
               union all

               select 'ATP' tabela, 'ATP deve sobrepor o ponto do Empreendimento.' descricao_mensagem, '' codigo_mensagem from (select count(*) num from idaf.tmp_projeto_geo p, TMP_ATP t, GEO_EMP_LOCALIZACAO a where t.projeto = id_projeto and p.id = id_projeto and p.empreendimento=a.empreendimento and sdo_relate(t.geometry, a.geometry, 'MASK=ANYINTERACT QUERYTYPE=WINDOW') = 'TRUE' ) where num = 0
               
               union all

               -- APMP
               select 'APMP' tabela, 'Nenhuma APMP encontrada.' descricao_mensagem, '' codigo_mensagem from (select count(*) num from TMP_APMP t where t.projeto = id_projeto) where num = 0

               union all
               
               select 'APMP' tabela, 'APMP deve sobrepor ATP.' descricao_mensagem, '' codigo_mensagem from (select count(*) num from TMP_APMP t where t.projeto = id_projeto and t.cod_atp is null) where num > 0

               union all
         
               -- Rocha
               select  'ROCHA' tabela, 'ROCHA deve sobrepor APMP.' descricao_mensagem, '' codigo_mensagem from (select count(*) num from TMP_ROCHA t where t.projeto = id_projeto and t.cod_apmp is null) where num > 0
               
               union all
         
               -- ARL
               select  'ROCHA' tabela, 'ARL deve sobrepor APMP.' descricao_mensagem, '' codigo_mensagem from (select count(*) num from TMP_ARL t where t.projeto = id_projeto and t.cod_apmp is null) where num > 0
               
               union all
         
               -- RPPN
               select  'RPPN' tabela, 'RPPN deve sobrepor APMP.' descricao_mensagem, '' codigo_mensagem from (select count(*) num from TMP_RPPN t where t.projeto = id_projeto and t.cod_apmp is null) where num > 0
               
               union all
         
               -- AFS
               select  'AFS' tabela, 'AFS deve sobrepor APMP.' descricao_mensagem, '' codigo_mensagem from (select count(*) num from TMP_AFS t where t.projeto = id_projeto and t.cod_apmp is null) where num > 0
               
               union all
               
               -- AVN
               select  'AVN' tabela, 'AVN deve sobrepor APMP.' descricao_mensagem, '' codigo_mensagem from (select count(*) num from TMP_AVN t where t.projeto = id_projeto and t.cod_apmp is null) where num > 0

               union all

               -- AA
               select  'AA' tabela, 'AA deve sobrepor APMP.' descricao_mensagem, '' codigo_mensagem from (select count(*) num from TMP_AA t where t.projeto = id_projeto and t.cod_apmp is null) where num > 0
               
               union all
         
               -- ACV
               select  'ACV' tabela, 'ACV deve sobrepor APMP.' descricao_mensagem, '' codigo_mensagem from (select count(*) num from TMP_ACV t where t.projeto = id_projeto and t.cod_apmp is null) where num > 0

               union all

               -- APMP_APMP
               select  'APMP_APMP' tabela, 'APMP não deve sobrepor APMP.' descricao_mensagem, '' codigo_mensagem from (select sum(t.area_m2) num from TMP_AREAS_CALCULADAS t where t.projeto = id_projeto and t.tipo='APMP_APMP') where num > spa_tolerancia_overlap

               union all

               -- APMP_AFD
               select  'APMP_AFD' tabela, 'APMP não deve sobrepor AFD.' descricao_mensagem, '' codigo_mensagem from (select sum(t.area_m2) num from TMP_AREAS_CALCULADAS t where t.projeto = id_projeto and t.tipo='APMP_AFD') where num > spa_tolerancia_overlap

               union all

               -- ARL_ARL
               select  'ARL_ARL' tabela, 'ARL não deve sobrepor ARL.' descricao_mensagem, '' codigo_mensagem from (select sum(t.area_m2) num from TMP_AREAS_CALCULADAS t where t.projeto = id_projeto and t.tipo='ARL_ARL') where num > spa_tolerancia_overlap

               union all

               -- ARL_ROCHA
               select  'ARL_ROCHA' tabela, 'ARL não deve sobrepor ROCHA.' descricao_mensagem, '' codigo_mensagem from (select sum(t.area_m2) num from TMP_AREAS_CALCULADAS t where t.projeto = id_projeto and t.tipo='ARL_ROCHA') where num > spa_tolerancia_overlap

               union all

               -- AA_AVN
               select  'AA_AVN' tabela, 'AA não deve sobrepor AVN.' descricao_mensagem, '' codigo_mensagem from (select sum(t.area_m2) num from TMP_AREAS_CALCULADAS t where t.projeto = id_projeto and t.tipo='AA_AVN') where num > spa_tolerancia_overlap
				) loop
            
            -- tipo=2 , obrigatoriedades
            insert into tab_validacao_geo (projeto, tipo, sigla_tabela, codigo_mensagem, descricao_mensagem)
            	values(id_projeto, 2, i.tabela, i.codigo_mensagem, i.descricao_mensagem);

      	end loop;
         --============================================================================================
         
         
         
         --============================================================================================
         -- Atributos
         --============================================================================================
         for i in (
               select * from (         

                  --APMP
                  select 'APMP' tabela, t.projeto, 'Tipo Inválido. O Tipo deve ser ''M'' para Matrícula, ''P'' para Posse ou ''D'' para Desconhecido.' descricao_mensagem, '' codigo_mensagem from TMP_APMP t where t.tipo is null or upper(t.tipo) not in ('M','P','D')
                  
                  union all
                  
						select 'APMP' tabela, t.projeto, 'Nome Inválido. O Nome deve ser único.' descricao_mensagem, '' codigo_mensagem from TMP_APMP t where t.nome is not null and trim(t.nome) is not null and t.projeto=id_projeto group by trim(upper(t.nome)), t.projeto having count(upper(trim(t.nome)))>1
                  
                  union all
                  
                  select 'APMP' tabela, t.projeto, 'Nome Inválido. O Nome deve ser preenchido.' descricao_mensagem, '' codigo_mensagem from TMP_APMP t where t.nome is null or trim(t.nome) is null
                  
                  union all

                  --AVN
                  select 'AVN' tabela, t.projeto, 'Estágio Inválido. O Estágio deve ser ''I'' para Inicial, ''M'' para Médio, ''A'' para Avançado ou ''D'' para Desconhecido.' descricao_mensagem, '' codigo_mensagem from TMP_AVN t where t.estagio is null or upper(t.estagio) not in ('I','M','A','D')
                  
                  union all
                                 
                  --AA
                  select 'AA' tabela, t.projeto, 'Tipo Inválido. O Tipo deve ser ''C'' para Cultivada, ''NC'' para Não Cultivada ou ''D'' para Desconhecido.' descricao_mensagem, '' codigo_mensagem from TMP_AA t where t.tipo is null or upper(t.tipo) not in ('C','NC','D')

                  union all
                  
                  --ACV
                  select 'ACV' tabela, t.projeto, 'Tipo Inválido. O Tipo deve ser ''MANGUE'', ''BREJO'', ''RESTINGA'', ''RESTINGA-APP'', ''FLORESTA-NATIVA'', ''FLORESTA-PLANTADA'', ''MACEGA'', ''CABRUCA'' ou ''OUTROS''.' descricao_mensagem, '' codigo_mensagem from TMP_ACV t where t.tipo is null or upper(t.tipo) not in ('MANGUE', 'BREJO', 'RESTINGA', 'RESTINGA-APP', 'FLORESTA-NATIVA', 'MACEGA', 'CABRUCA', 'FLORESTA-PLANTADA', 'OUTROS')
                  
                  union all
                  
                  --RIO_LINHA
                  select 'RIO_LINHA' tabela, t.projeto, 'Largura Inválida. A Largura deve ser maior do que 0 e menor ou igual a 50.' descricao_mensagem, '' codigo_mensagem from TMP_RIO_LINHA t where t.largura is null or t.largura<=0 or t.largura>50
                  
                  union all
                  
                  --RIO_AREA
                  select 'RIO_AREA' tabela, t.projeto, 'Largura Inválida. A Largura dever ser maior do que 0.' descricao_mensagem, '' codigo_mensagem from TMP_RIO_AREA t where t.largura is null or t.largura<=0
                  
                  union all

                  --LAGOA
                  select 'LAGOA' tabela, t.projeto, 'Zona Inválida. A Zona dever ser ''U'' para Urbano, ''R'' para Rural ou ''A'' para Abastecimento.' descricao_mensagem, '' codigo_mensagem from TMP_LAGOA t where t.zona is null or upper(t.zona) not in ('U','R','A')

               ) a where a.projeto = id_projeto
				) loop
            
            -- tipo=3 , atributos
            insert into tab_validacao_geo (projeto, tipo, sigla_tabela, codigo_mensagem, descricao_mensagem)
            	values(id_projeto, 3, i.tabela, i.codigo_mensagem, i.descricao_mensagem);

      	end loop;
         --============================================================================================
         



         --============================================================================================
         -- Contabilizar Geometrias
         --============================================================================================
         for i in (
   				-- Área Total da Propriedade
               select 'ATP' tabela, 'Área Total da Propriedade' tabela_nome, count(*) descricao_mensagem, '' codigo_mensagem from TMP_ATP t where t.projeto = id_projeto

               -- Matrícula ou Posse
               union all 
               select 'APMP' tabela, 'Área da Propriedade por Matrícula ou Posse' tabela_nome, count(*) descricao_mensagem, '' codigo_mensagem from TMP_APMP t where t.projeto = id_projeto

               -- Área da faixa de dominio
               union all 
               select 'AFD' tabela, 'Área de Faixa de Domínio' tabela_nome, count(*) descricao_mensagem, '' codigo_mensagem from TMP_AFD t where t.projeto = id_projeto

               -- Rocha
               union all 
               select 'ROCHA' tabela, 'Área de afloramentos rochosos' tabela_nome, count(*) descricao_mensagem, '' codigo_mensagem from TMP_ROCHA t where t.projeto = id_projeto

               -- Vertices de APMP
               union all 
               select 'VERTICE' tabela, 'Pontos vértices da área da propriedade por matrícula ou posse' tabela_nome, count(*) descricao_mensagem, '' codigo_mensagem from TMP_VERTICE t where t.projeto = id_projeto

               -- Area de Reserva Legal
               union all 
               select 'ARL' tabela, 'Área de Reserva Legal' tabela_nome, count(*) descricao_mensagem, '' codigo_mensagem from TMP_ARL t where t.projeto = id_projeto

               -- Reserva Particular de Patrimônio Natural
               union all 
               select 'RPPN' tabela, 'Área de Reserva Particular do Patrimônio Natural' tabela_nome, count(*) descricao_mensagem, '' codigo_mensagem from TMP_RPPN t where t.projeto = id_projeto

               -- Área de Faixa de Servidão
               union all 
               select 'AFS' tabela, 'Área de Faixa de Servidão' tabela_nome, count(*) descricao_mensagem, '' codigo_mensagem from TMP_AFS t where t.projeto = id_projeto

               -- Área de Vegetação Nativa
               union all 
               select 'AVN' tabela, 'Área de Vegetação Nativa' tabela_nome, count(*) descricao_mensagem, '' codigo_mensagem from TMP_AVN t where t.projeto = id_projeto

               -- Área Aberta
               union all 
               select 'AA' tabela, 'Área Aberta' tabela_nome, count(*) descricao_mensagem, '' codigo_mensagem from TMP_AA t where t.projeto = id_projeto

               -- Área de Classificação de Vegetação
               union all 
               select 'ACV' tabela, 'Área de Classificação de Vegetação' tabela_nome, count(*) descricao_mensagem, '' codigo_mensagem from TMP_ACV t where t.projeto = id_projeto

               -- Área Construída
               union all 
               select 'ACONSTRUIDA' tabela, 'Área Construída' tabela_nome, count(*) descricao_mensagem, '' codigo_mensagem from TMP_ACONSTRUIDA t where t.projeto = id_projeto
               
               -- Linhas de DUTO
               union all 
               select 'DUTO' tabela, 'Linha de duto' tabela_nome, count(*) descricao_mensagem, '' codigo_mensagem from TMP_DUTO t where t.projeto = id_projeto

               -- Linhas de TRANSMISSAO
               union all 
               select 'LTRANSMISSAO' tabela, 'Linha de transmissão de energia' tabela_nome, count(*) descricao_mensagem, '' codigo_mensagem from TMP_LTRANSMISSAO t where t.projeto = id_projeto

               -- Linhas de ESTRADA
               union all 
               select 'ESTRADA' tabela, 'Linha de estrada' tabela_nome, count(*) descricao_mensagem, '' codigo_mensagem from TMP_ESTRADA t where t.projeto = id_projeto

               -- Linhas de FERROVIA
               union all 
               select 'FERROVIA' tabela, 'Linha de ferrovias' tabela_nome, count(*) descricao_mensagem, '' codigo_mensagem from TMP_FERROVIA t where t.projeto = id_projeto

               -- Nascente
               union all 
               select 'NASCENTE' tabela, 'Ponto de nascente' tabela_nome, count(*) descricao_mensagem, '' codigo_mensagem from TMP_NASCENTE t where t.projeto = id_projeto

               -- Linhas de Rio
               union all 
               select 'RIO_LINHA' tabela, 'Linha de curso de água' tabela_nome, count(*) descricao_mensagem, '' codigo_mensagem from TMP_RIO_LINHA t where t.projeto = id_projeto

               -- Areas de Rio
               union all 
               select 'RIO_AREA' tabela, 'Área de curso de água' tabela_nome, count(*) descricao_mensagem, '' codigo_mensagem from TMP_RIO_AREA t where t.projeto = id_projeto

               -- Areas de Lagos e Lagoas
               union all 
               select 'LAGOA' tabela, 'Área de lagoa' tabela_nome, count(*) descricao_mensagem, '' codigo_mensagem from TMP_LAGOA t where t.projeto = id_projeto

               -- Areas Inundadas de Represas
               union all 
               select 'REPRESA' tabela, 'Área de represa' tabela_nome, count(*) descricao_mensagem, '' codigo_mensagem from TMP_REPRESA t where t.projeto = id_projeto

               -- Areas de Duna            
               union all 
               select 'DUNA' tabela, 'Área de duna' tabela_nome, count(*) descricao_mensagem, '' codigo_mensagem from TMP_DUNA t where t.projeto = id_projeto

               -- Restrição de declividade            
               union all 
               select 'REST_DECLIVIDADE' tabela, 'Área de restrições de declividade' tabela_nome, count(*) descricao_mensagem, '' codigo_mensagem from TMP_REST_DECLIVIDADE t where t.projeto = id_projeto

               -- Escarpa
               union all 
               select 'ESCARPA' tabela, 'Área de escarpa' tabela_nome, count(*) descricao_mensagem, '' codigo_mensagem from TMP_ESCARPA t where t.projeto = id_projeto
				) loop
            
            -- tipo=4 , contabilizacao
            insert into tab_validacao_geo (projeto, tipo, sigla_tabela, nome_tabela, codigo_mensagem, descricao_mensagem)
            	values(id_projeto, 4, i.tabela, i.tabela_nome, i.codigo_mensagem, i.descricao_mensagem);

      	end loop;
         --============================================================================================






		elsif (tipo = TIPO_ATIVIDADE) then

         --============================================================================================
         -- Erros Espaciais
         --============================================================================================
         for i in (select * from (

					-- PATIV
               select 'PATIV' tabela, t.projeto, '' codigo_mensagem,  ErroSpatial(ValidarGeometria(t.geometry)) descricao_mensagem from TMP_PATIV t where ValidarGeometria(t.geometry)<>'TRUE'
               union all
               select 'PATIV' tabela, t.projeto, '' codigo_mensagem,   ValidarGtype(t.geometry, 2001) descricao_mensagem from TMP_PATIV t where ValidarGtype(t.geometry, 2001)<>'TRUE'
               
               union all

					-- LATIV
               select 'LATIV' tabela, t.projeto, '' codigo_mensagem,  ErroSpatial(ValidarGeometria(t.geometry)) descricao_mensagem from TMP_LATIV t where ValidarGeometria(t.geometry)<>'TRUE'
               union all
               select 'LATIV' tabela, t.projeto, '' codigo_mensagem,   ValidarGtype(t.geometry, 2002) descricao_mensagem from TMP_LATIV t where ValidarGtype(t.geometry, 2002)<>'TRUE'
               
               union all

					-- AATIV
               select 'AATIV' tabela, t.projeto, '' codigo_mensagem,  ErroSpatial(ValidarGeometria(t.geometry)) descricao_mensagem from TMP_AATIV t where ValidarGeometria(t.geometry)<>'TRUE'
               union all
               select 'AATIV' tabela, t.projeto, '' codigo_mensagem,   ValidarGtype(t.geometry, 2003) descricao_mensagem from TMP_AATIV t where ValidarGtype(t.geometry, 2003)<>'TRUE'
               
               union all

					-- AIATIV
               select 'AIATIV' tabela, t.projeto, '' codigo_mensagem,  ErroSpatial(ValidarGeometria(t.geometry)) descricao_mensagem from TMP_AIATIV t where ValidarGeometria(t.geometry)<>'TRUE'
               union all
               select 'AIATIV' tabela, t.projeto, '' codigo_mensagem,   ValidarGtype(t.geometry, 2003) descricao_mensagem from TMP_AIATIV t where ValidarGtype(t.geometry, 2003)<>'TRUE'
				) a where a.projeto = id_projeto) loop
            
            -- tipo=1 , erro espacial
            insert into tab_validacao_geo (projeto, tipo, sigla_tabela, codigo_mensagem, descricao_mensagem)
            	values(id_projeto, 1, i.tabela, i.codigo_mensagem, i.descricao_mensagem);

      	end loop;

         --============================================================================================
         
         
         
         --============================================================================================
         -- Obrigatoriedades
         --============================================================================================

			for i in (
               -- Geral
               select 'ATIV' tabela, 'Nenhuma PATIV, LATIV ou AATIV encontrada' descricao_mensagem, '' codigo_mensagem from (select (select count(*) num from TMP_PATIV t where t.projeto = id_projeto) + (select count(*) num from TMP_LATIV t where t.projeto = id_projeto) + (select count(*) num from TMP_AATIV t where t.projeto = id_projeto) num from dual) where num = 0
               
               union all
               
               -- AATIV_AATIV
               select  'AATIV_AATIV' tabela, 'AATIV não deve sobrepor AATIV.' descricao_mensagem, '' codigo_mensagem from (select sum(t.area_m2) num from TMP_AREAS_CALCULADAS t where t.projeto = id_projeto and t.tipo='AATIV_AATIV') where num > spa_tolerancia_overlap

               union all

               -- PATIV
               select  'PATIV' tabela, 'PATIV deve sobrepor APMP.' descricao_mensagem, '' codigo_mensagem from (select count(*) num from TMP_PATIV t where t.projeto = id_projeto and t.cod_apmp is null) where num > 0
               
               union all
               
               -- LATIV
               select  'LATIV' tabela, 'LATIV deve sobrepor APMP.' descricao_mensagem, '' codigo_mensagem from (select count(*) num from TMP_LATIV t where t.projeto = id_projeto and t.cod_apmp is null) where num > 0
               
               union all
               
               -- AATIV
               select  'AATIV' tabela, 'AATIV deve sobrepor APMP.' descricao_mensagem, '' codigo_mensagem from (select count(*) num from TMP_AATIV t where t.projeto = id_projeto and t.cod_apmp is null) where num > 0

				) loop
            
            -- tipo=2 , obrigatoriedades
            insert into tab_validacao_geo (projeto, tipo, sigla_tabela, codigo_mensagem, descricao_mensagem)
            	values(id_projeto, 2, i.tabela, i.codigo_mensagem, i.descricao_mensagem);

      	end loop;

         --============================================================================================
         
         
         
         --============================================================================================
         -- Atributos
         --============================================================================================         

      	-- Não possui erros de Atributo
      
         --============================================================================================
         



         --============================================================================================
         -- Contabilizar Geometrias
         --============================================================================================
         for i in (
   				-- PATIV
               select 'PATIV' tabela, 'Ponto da Atividade' tabela_nome, count(*) descricao_mensagem, '' codigo_mensagem from TMP_PATIV t where t.projeto = id_projeto

               -- LATIV
               union all 
               select 'LATIV' tabela, 'Linha da Atividade' tabela_nome, count(*) descricao_mensagem, '' codigo_mensagem from TMP_LATIV t where t.projeto = id_projeto

               -- AATIV
               union all 
               select 'AATIV' tabela, 'Área da Atividade' tabela_nome, count(*) descricao_mensagem, '' codigo_mensagem from TMP_AATIV t where t.projeto = id_projeto

               -- AIATIV
               union all 
               select 'AIATIV' tabela, 'Área de Influência da Atividade' tabela_nome, count(*) descricao_mensagem, '' codigo_mensagem from TMP_AIATIV t where t.projeto = id_projeto
				) loop
            
            -- tipo=4 , contabilizacao
            insert into tab_validacao_geo (projeto, tipo, sigla_tabela, nome_tabela, codigo_mensagem, descricao_mensagem)
            	values(id_projeto, 4, i.tabela, i.tabela_nome, i.codigo_mensagem, i.descricao_mensagem);

      	end loop;
         --============================================================================================

      end if;
      
      commit;
   end;

   
   procedure ImportarDoTrackmaker(id_projeto number, tipo number) is
      srid    number := 31999;

      numero  number;
      texto1  varchar2(400);
      texto2  varchar2(400);
      
      idx1 number;
      idx2 number;

   begin
      ApagarGeometriasTMP(id_projeto);

      if (tipo = TIPO_DOMINIALIDADE) then

         for i in (select upper(t.nome) nome, t.geometry from TMP_RASC_TRACKMAKER t where t.projeto=id_projeto) loop
            i.geometry.sdo_srid := srid;
            i.geometry.sdo_gtype := 2004;
            i.geometry := geometria9i.extrair(i.geometry);

            i.geometry.sdo_gtype := case (i.geometry.sdo_elem_info(2))
                                       when 1 then
                                          2001
                                       when 2 then
                                          2002
                                       when 4 then
                                          2002
                                       when 1003 then
                                          2003
                                       when 1005 then
                                          2003
                                       else
                                          2000
                                    end;

            if ( (i.nome='RIO') or (instr(i.nome, 'RIO=')>0) ) then
               begin
                  idx1 := instr(i.nome, '=');
                  if (idx1=0) then
                     idx1 := length(i.nome);
                  end if;
                  idx1 := idx1+1;
                  
                  idx2 := instr(i.nome, '=', idx1);
                  
                  if (idx2>0) then
	                  numero := parse_number( substr(i.nome, idx1, idx2-idx1) );
                     texto1 := substr(i.nome, idx2+1);
                  else
                     numero := parse_number( substr(i.nome, idx1) );
                     texto1 := null;
                  end if;

                  numero := floor(numero);
               exception
                  when others then 
                     numero := null;
	                  texto1 := null;
               end;
               
               if (i.geometry.sdo_gtype = 2002) then
                  insert into TMP_RIO_LINHA (id, projeto, nome, largura, geometry) values (SEQ_TMP_RIO_LINHA.nextval, id_projeto, texto1, numero, i.geometry);
               elsif (i.geometry.sdo_gtype = 2003) then
                  insert into TMP_RIO_AREA (id, projeto, nome, largura, geometry) values (SEQ_TMP_RIO_AREA.nextval, id_projeto, texto1, numero, i.geometry);
               end if;
            
            elsif (i.nome='NASCENTE') then
               if (i.geometry.sdo_gtype = 2001) then
                  insert into TMP_NASCENTE (id, projeto, geometry) values (SEQ_TMP_NASCENTE.nextval, id_projeto, i.geometry);
               end if;

  				elsif ( (i.nome='LAGOA') or (instr(i.nome, 'LAGOA=')>0) ) then
	            if (i.geometry.sdo_gtype = 2003) then
                  begin
                     idx1 := instr(i.nome, '=');
                     if (idx1=0) then
                        idx1 := length(i.nome);
                     end if;
                     idx1 := idx1+1;

                     idx2 := instr(i.nome, '=', idx1);
                     
                     if (idx2>0) then
                        texto1 := substr(i.nome, idx1, idx2-idx1);
                        texto2 := substr(i.nome, idx2+1);
                     else
                        texto1 := substr(i.nome, idx1);
                        texto2 := null;
                     end if;
                  exception
                     when others then 
                        texto1 := null;
                        texto2 := null;
                  end;
                  
                  insert into TMP_LAGOA (id, projeto, zona, nome, geometry) values (SEQ_TMP_LAGOA.nextval, id_projeto, texto1, texto2, i.geometry);
               end if;
               
            elsif ( (i.nome='REPRESA') or (instr(i.nome, 'REPRESA=')>0) ) then
	            if (i.geometry.sdo_gtype = 2003) then
                  begin
                     idx1 := instr(i.nome, '=');
                     if (idx1=0) then
                        idx1 := length(i.nome);
                     end if;
                     idx1 := idx1+1;
                  
                     idx2 := instr(i.nome, '=', idx1);
                     
                     if (idx2>0) then
                        numero := parse_number( substr(i.nome, idx1, idx2-idx1) );
                        texto1 := substr(i.nome, idx2+1);
                     else
                        numero := parse_number( substr(i.nome, idx1) );
                        texto1 := null;
                     end if;

                     numero := floor(numero);
                  exception
                     when others then 
                        numero := null;
                        texto1 := null;
                  end;

                  insert into TMP_REPRESA (id, projeto, nome, amortecimento, geometry) values (SEQ_TMP_RIO_AREA.nextval, id_projeto, texto1, numero, i.geometry);
               end if;
               
            elsif (i.nome='REST_DECLIVIDADE') then
               if (i.geometry.sdo_gtype = 2003) then
                  insert into TMP_REST_DECLIVIDADE (id, projeto, geometry) values (SEQ_TMP_REST_DECLIVIDADE.nextval, id_projeto, i.geometry);
               end if;
               
            elsif (i.nome='ESCARPA') then
               if (i.geometry.sdo_gtype = 2003) then
                  insert into TMP_ESCARPA (id, projeto, geometry) values (SEQ_TMP_ESCARPA.nextval, id_projeto, i.geometry);
               end if;
               
            elsif (i.nome='DUNA') then
               if (i.geometry.sdo_gtype = 2003) then
                  insert into TMP_DUNA (id, projeto, geometry) values (SEQ_TMP_DUNA.nextval, id_projeto, i.geometry);
               end if;
               
            elsif (i.nome='ROCHA') then
               if (i.geometry.sdo_gtype = 2003) then
                  insert into TMP_ROCHA (id, projeto, geometry) values (SEQ_TMP_ROCHA.nextval, id_projeto, i.geometry);
               end if;

            elsif (i.nome='ATP') then
               if (i.geometry.sdo_gtype = 2003) then
                  insert into TMP_ATP (id, projeto, geometry) values (SEQ_TMP_ATP.nextval, id_projeto, i.geometry);
               end if;

  				elsif ( (i.nome='APMP') or (instr(i.nome, 'APMP=')>0) ) then
	            if (i.geometry.sdo_gtype = 2003) then
                  begin
                     idx1 := instr(i.nome, '=');
                     if (idx1=0) then
                        idx1 := length(i.nome);
                     end if;
                     idx1 := idx1+1;

                     idx2 := instr(i.nome, '=', idx1);
                     
                     if (idx2>0) then
                        texto1 := substr(i.nome, idx1, idx2-idx1);
                        texto2 := substr(i.nome, idx2+1);
                     else
                        texto1 := substr(i.nome, idx1);
                        texto2 := null;
                     end if;
                  exception
                     when others then 
                        texto1 := null;
                        texto2 := null;
                  end;
                  
                  insert into TMP_APMP (id, projeto, tipo, nome, geometry) values (SEQ_TMP_APMP.nextval, id_projeto, texto1, texto2, i.geometry);
               end if;  

				elsif ( (i.nome='VERTICE') or (instr(i.nome, 'VERTICE=')>0) ) then
	            if (i.geometry.sdo_gtype = 2001) then
                  begin
                     idx1 := instr(i.nome, '=');
	                  if (idx1=0) then
   	                  idx1 := length(i.nome);
      	            end if;
                     idx1 := idx1+1;

                     texto1 := substr(i.nome, idx1);
                  exception
                     when others then 
                        texto1 := null;
                  end;
                  
                  insert into TMP_VERTICE (id, projeto, nome, geometry) values (SEQ_TMP_VERTICE.nextval, id_projeto, texto1, i.geometry);
               end if;

				elsif ( (i.nome='ARL') or (instr(i.nome, 'ARL=')>0) ) then
               if (i.geometry.sdo_gtype = 2003) then
                  begin
                     idx1 := instr(i.nome, '=');
	                  if (idx1=0) then
   	                  idx1 := length(i.nome);
      	            end if;
                     idx1 := idx1+1;

                     texto1 := substr(i.nome, idx1);
                  exception
                     when others then 
                        texto1 := null;
                  end;
                  
                  if (texto1 is not null and (upper(texto1)='S' or upper(texto1)='C') ) then
                     texto1:= 'S';
                  else
                     texto1:= 'N';
                  end if;

						insert into TMP_ARL (id, projeto, compensada, geometry) values (SEQ_TMP_ARL.nextval, id_projeto, texto1, i.geometry);
               end if;

				elsif (i.nome='RPPN') then
               if (i.geometry.sdo_gtype = 2003) then
                  insert into TMP_RPPN (id, projeto, geometry) values (SEQ_TMP_RPPN.nextval, id_projeto, i.geometry);
               end if;

				elsif (i.nome='AFD') then
               if (i.geometry.sdo_gtype = 2003) then
                  insert into TMP_AFD (id, projeto, geometry) values (SEQ_TMP_AFD.nextval, id_projeto, i.geometry);
               end if;

				elsif ( (i.nome='AVN') or (instr(i.nome, 'AVN=')>0) ) then
               if (i.geometry.sdo_gtype = 2003) then
                  begin
                     idx1 := instr(i.nome, '=');
	                  if (idx1=0) then
   	                  idx1 := length(i.nome);
      	            end if;
                     idx1 := idx1+1;

                     texto1 := substr(i.nome, idx1);
                  exception
                     when others then 
                        texto1 := null;
                  end;

						insert into TMP_AVN (id, projeto, estagio, geometry) values (SEQ_TMP_AVN.nextval, id_projeto, texto1, i.geometry);
               end if;

				elsif ( (i.nome='AA') or (instr(i.nome, 'AA=')>0) ) then
               if (i.geometry.sdo_gtype = 2003) then
                  begin
                     idx1 := instr(i.nome, '=');
	                  if (idx1=0) then
   	                  idx1 := length(i.nome);
      	            end if;
                     idx1 := idx1+1;

                     texto1 := substr(i.nome, idx1);
                  exception
                     when others then 
                        texto1 := null;
                  end;

						insert into TMP_AA (id, projeto, tipo, geometry) values (SEQ_TMP_AA.nextval, id_projeto, texto1, i.geometry);
               end if;

				elsif (i.nome='AFS') then
               if (i.geometry.sdo_gtype = 2003) then
                  insert into TMP_AFS (id, projeto, geometry) values (SEQ_TMP_AFS.nextval, id_projeto, i.geometry);
               end if;

				elsif ( (i.nome='ACV') or (instr(i.nome, 'ACV=')>0) ) then
               if (i.geometry.sdo_gtype = 2003) then
                  begin
                     idx1 := instr(i.nome, '=');
	                  if (idx1=0) then
   	                  idx1 := length(i.nome);
      	            end if;
                     idx1 := idx1+1;

                     texto1 := substr(i.nome, idx1);
                  exception
                     when others then 
                        texto1 := null;
                  end;

						insert into TMP_ACV (id, projeto, tipo, geometry) values (SEQ_TMP_ACV.nextval, id_projeto, upper(texto1), i.geometry);
               end if;

				elsif (i.nome='ACONSTRUIDA') then
               if (i.geometry.sdo_gtype = 2003) then
                  insert into TMP_ACONSTRUIDA (id, projeto, geometry) values (SEQ_TMP_ACONSTRUIDA.nextval, id_projeto, i.geometry);
               end if;

				elsif (i.nome='DUTO') then
               if (i.geometry.sdo_gtype = 2002) then
                  insert into TMP_DUTO (id, projeto, geometry) values (SEQ_TMP_DUTO.nextval, id_projeto, i.geometry);
               end if;

				elsif (i.nome='LTRANSMISSAO') then
               if (i.geometry.sdo_gtype = 2002) then
                  insert into TMP_LTRANSMISSAO (id, projeto, geometry) values (SEQ_TMP_LTRANSMISSAO.nextval, id_projeto, i.geometry);
               end if;

				elsif (i.nome='ESTRADA') then
               if (i.geometry.sdo_gtype = 2002) then
                  insert into TMP_ESTRADA (id, projeto, geometry) values (SEQ_TMP_ESTRADA.nextval, id_projeto, i.geometry);
               end if;

				elsif (i.nome='FERROVIA') then
               if (i.geometry.sdo_gtype = 2002) then
                  insert into TMP_FERROVIA (id, projeto, geometry) values (SEQ_TMP_FERROVIA.nextval, id_projeto, i.geometry);
               end if;

            end if;

         end loop;
         
         
		elsif (tipo = TIPO_ATIVIDADE) then
      	for i in (select upper(t.nome) nome, t.geometry from TMP_RASC_TRACKMAKER t where t.projeto=id_projeto) loop
            i.geometry.sdo_srid := srid;
            i.geometry.sdo_gtype := 2004;
            i.geometry := geometria9i.extrair(i.geometry);

            i.geometry.sdo_gtype := case (i.geometry.sdo_elem_info(2))
                                       when 1 then
                                          2001
                                       when 2 then
                                          2002
                                       when 4 then
                                          2002
                                       when 1003 then
                                          2003
                                       when 1005 then
                                          2003
                                       else
                                          2000
                                    end;

				if (i.nome='PATIV') then
               if (i.geometry.sdo_gtype = 2001) then
                  insert into TMP_PATIV (id, projeto, geometry) values (SEQ_TMP_PATIV.nextval, id_projeto, i.geometry);
               end if;

				elsif (i.nome='LATIV') then
               if (i.geometry.sdo_gtype = 2002) then
                  insert into TMP_LATIV (id, projeto, geometry) values (SEQ_TMP_LATIV.nextval, id_projeto, i.geometry);
               end if;

				elsif (i.nome='AATIV') then
               if (i.geometry.sdo_gtype = 2003) then
                  insert into TMP_AATIV (id, projeto, geometry) values (SEQ_TMP_AATIV.nextval, id_projeto, i.geometry);
               end if;
               
				elsif (i.nome='AIATIV') then
               if (i.geometry.sdo_gtype = 2003) then
                  insert into TMP_AIATIV (id, projeto, geometry) values (SEQ_TMP_AIATIV.nextval, id_projeto, i.geometry);
               end if;

            end if;

         end loop;

      end if;
      
      commit;
   end;
   
   procedure ImportarDoDesenhador(id_projeto number, tipo number) is
   begin
      ApagarGeometriasTMP(id_projeto);

      if (tipo = TIPO_DOMINIALIDADE) then
         -- Limites
         insert into TMP_ATP (id, projeto, geometry) (select SEQ_TMP_ATP.nextval, id_projeto, a.geometry from DES_ATP a where a.projeto = id_projeto);
         insert into TMP_APMP (id, projeto, tipo, nome, geometry) (select SEQ_TMP_APMP.nextval, id_projeto, a.tipo, a.nome, a.geometry from DES_APMP a where a.projeto = id_projeto);
         insert into TMP_AFD (id, projeto, geometry) (select SEQ_TMP_AFD.nextval, id_projeto, a.geometry from DES_AFD a where a.projeto = id_projeto);
			insert into TMP_ROCHA (id, projeto, geometry) (select SEQ_TMP_ROCHA.nextval, id_projeto, a.geometry from DES_ROCHA a where a.projeto = id_projeto);
			insert into TMP_VERTICE (id, projeto, nome, geometry) (select SEQ_TMP_VERTICE.nextval, id_projeto, a.nome, a.geometry from DES_VERTICE a where a.projeto = id_projeto);
			insert into TMP_ARL (id, projeto, compensada, geometry) (select SEQ_TMP_ARL.nextval, id_projeto, (case when upper(a.compensada)='S' then 'S' else 'N' end), a.geometry from DES_ARL a where a.projeto = id_projeto);
			insert into TMP_RPPN (id, projeto, geometry) (select SEQ_TMP_RPPN.nextval, id_projeto, a.geometry from DES_RPPN a where a.projeto = id_projeto);
			insert into TMP_AFS (id, projeto, geometry) (select SEQ_TMP_AFS.nextval, id_projeto, a.geometry from DES_AFS a where a.projeto = id_projeto);
			insert into TMP_AVN (id, projeto, estagio, geometry) (select SEQ_TMP_AVN.nextval, id_projeto, a.estagio, a.geometry from DES_AVN a where a.projeto = id_projeto);
			insert into TMP_AA (id, projeto, tipo, geometry) (select SEQ_TMP_AA.nextval, id_projeto, a.tipo, a.geometry from DES_AA a where a.projeto = id_projeto);
			insert into TMP_ACV (id, projeto, tipo, geometry) (select SEQ_TMP_ACV.nextval, id_projeto, upper(a.tipo), a.geometry from DES_ACV a where a.projeto = id_projeto);
         
         -- Outros
			insert into TMP_ACONSTRUIDA (id, projeto, geometry) (select SEQ_TMP_ACONSTRUIDA.nextval, id_projeto, a.geometry from DES_ACONSTRUIDA a where a.projeto = id_projeto);
         insert into TMP_DUTO (id, projeto, geometry) (select SEQ_TMP_DUTO.nextval, id_projeto, a.geometry from DES_DUTO a where a.projeto = id_projeto);
         insert into TMP_LTRANSMISSAO (id, projeto, geometry) (select SEQ_TMP_LTRANSMISSAO.nextval, id_projeto, a.geometry from DES_LTRANSMISSAO a where a.projeto = id_projeto);
         insert into TMP_ESTRADA (id, projeto, geometry) (select SEQ_TMP_ESTRADA.nextval, id_projeto, a.geometry from DES_ESTRADA a where a.projeto = id_projeto);
         insert into TMP_FERROVIA (id, projeto, geometry) (select SEQ_TMP_FERROVIA.nextval, id_projeto, a.geometry from DES_FERROVIA a where a.projeto = id_projeto);
         
         -- Itens da APP
         insert into TMP_NASCENTE (id, projeto, geometry) (select SEQ_TMP_NASCENTE.nextval, id_projeto, a.geometry from DES_NASCENTE a where a.projeto = id_projeto);
         insert into TMP_RIO_LINHA (id, projeto, nome, largura, geometry) (select SEQ_TMP_RIO_LINHA.nextval, id_projeto, a.nome, a.largura, a.geometry from DES_RIO_LINHA a where a.projeto = id_projeto);
         insert into TMP_RIO_AREA (id, projeto, nome, largura, geometry) (select SEQ_TMP_RIO_AREA.nextval, id_projeto, a.nome, a.largura, a.geometry from DES_RIO_AREA a where a.projeto = id_projeto);
			insert into TMP_LAGOA (id, projeto, zona, nome, geometry) (select SEQ_TMP_LAGOA.nextval, id_projeto, a.zona, a.nome, a.geometry from DES_LAGOA a where a.projeto = id_projeto);
			insert into TMP_REPRESA (id, projeto, amortecimento, nome, geometry) (select SEQ_TMP_REPRESA.nextval, id_projeto, a.amortecimento, a.nome, a.geometry from DES_REPRESA a where a.projeto = id_projeto);
         insert into TMP_DUNA (id, projeto, geometry) (select SEQ_TMP_DUNA.nextval, id_projeto, a.geometry from DES_DUNA a where a.projeto = id_projeto);
         insert into TMP_REST_DECLIVIDADE (id, projeto, geometry) (select SEQ_TMP_REST_DECLIVIDADE.nextval, id_projeto, a.geometry from DES_REST_DECLIVIDADE a where a.projeto = id_projeto);
         insert into TMP_ESCARPA (id, projeto, geometry) (select SEQ_TMP_ESCARPA.nextval, id_projeto, a.geometry from DES_ESCARPA a where a.projeto = id_projeto);

      
      elsif (tipo = TIPO_ATIVIDADE) then
         insert into TMP_PATIV (id, projeto, geometry) (select SEQ_TMP_PATIV.nextval, id_projeto, a.geometry from DES_PATIV a where a.projeto = id_projeto);
         insert into TMP_LATIV (id, projeto, geometry) (select SEQ_TMP_LATIV.nextval, id_projeto, a.geometry from DES_LATIV a where a.projeto = id_projeto);
         insert into TMP_AATIV (id, projeto, geometry) (select SEQ_TMP_AATIV.nextval, id_projeto, a.geometry from DES_AATIV a where a.projeto = id_projeto);
         insert into TMP_AIATIV (id, projeto, geometry) (select SEQ_TMP_AIATIV.nextval, id_projeto, a.geometry from DES_AIATIV a where a.projeto = id_projeto);
         
      end if;
      
      commit;
   end;
   
   procedure ImportarParaDesenhProcessada(id_projeto number, tipo number) is
   begin     
      ApagarGeometriasDES(id_projeto);

		if (tipo = TIPO_DOMINIALIDADE) then
         -- Limites
         insert into VW_DES_ATP (id, projeto, geometry) (select SEQ_DES_ATP.nextval, id_projeto, a.geometry from TMP_ATP a where a.projeto = id_projeto);
         insert into VW_DES_APMP (id, projeto, tipo, nome, geometry) (select SEQ_DES_APMP.nextval, id_projeto, a.tipo, a.nome, a.geometry from TMP_APMP a where a.projeto = id_projeto);
         insert into DES_AFD (id, projeto, geometry) (select SEQ_DES_AFD.nextval, id_projeto, a.geometry from TMP_AFD a where a.projeto = id_projeto);
			insert into DES_ROCHA (id, projeto, geometry) (select SEQ_DES_ROCHA.nextval, id_projeto, a.geometry from TMP_ROCHA a where a.projeto = id_projeto);
			insert into DES_VERTICE (id, projeto, nome, geometry) (select SEQ_DES_VERTICE.nextval, id_projeto, a.nome, a.geometry from TMP_VERTICE a where a.projeto = id_projeto);
			insert into DES_ARL (id, projeto, compensada, geometry) (select SEQ_DES_ARL.nextval, id_projeto, a.compensada, a.geometry from TMP_ARL a where a.projeto = id_projeto);
			insert into DES_RPPN (id, projeto, geometry) (select SEQ_DES_RPPN.nextval, id_projeto, a.geometry from TMP_RPPN a where a.projeto = id_projeto);
			insert into DES_AFS (id, projeto, geometry) (select SEQ_DES_AFS.nextval, id_projeto, a.geometry from TMP_AFS a where a.projeto = id_projeto);
			insert into VW_DES_AVN (id, projeto, estagio, geometry) (select SEQ_DES_AVN.nextval, id_projeto, a.estagio, a.geometry from TMP_AVN a where a.projeto = id_projeto);
			insert into VW_DES_AA (id, projeto, tipo, geometry) (select SEQ_DES_AA.nextval, id_projeto, a.tipo, a.geometry from TMP_AA a where a.projeto = id_projeto);
			insert into DES_ACV (id, projeto, tipo, geometry) (select SEQ_DES_ACV.nextval, id_projeto, a.tipo, a.geometry from TMP_ACV a where a.projeto = id_projeto);
         
         -- Outros
			insert into DES_ACONSTRUIDA (id, projeto, geometry) (select SEQ_DES_ACONSTRUIDA.nextval, id_projeto, a.geometry from TMP_ACONSTRUIDA a where a.projeto = id_projeto);
         insert into DES_DUTO (id, projeto, geometry) (select SEQ_DES_DUTO.nextval, id_projeto, a.geometry from TMP_DUTO a where a.projeto = id_projeto);
         insert into DES_LTRANSMISSAO (id, projeto, geometry) (select SEQ_DES_LTRANSMISSAO.nextval, id_projeto, a.geometry from TMP_LTRANSMISSAO a where a.projeto = id_projeto);
         insert into DES_ESTRADA (id, projeto, geometry) (select SEQ_DES_ESTRADA.nextval, id_projeto, a.geometry from TMP_ESTRADA a where a.projeto = id_projeto);
         insert into DES_FERROVIA (id, projeto, geometry) (select SEQ_DES_FERROVIA.nextval, id_projeto, a.geometry from TMP_FERROVIA a where a.projeto = id_projeto);
         
         -- Itens da APP
         insert into DES_NASCENTE (id, projeto, geometry) (select SEQ_DES_NASCENTE.nextval, id_projeto, a.geometry from TMP_NASCENTE a where a.projeto = id_projeto);
         insert into DES_RIO_LINHA (id, projeto, nome, largura, geometry) (select SEQ_DES_RIO_LINHA.nextval, id_projeto, a.nome, a.largura, a.geometry from TMP_RIO_LINHA a where a.projeto = id_projeto);
         insert into DES_RIO_AREA (id, projeto, nome, largura, geometry) (select SEQ_DES_RIO_AREA.nextval, id_projeto, a.nome, a.largura, a.geometry from TMP_RIO_AREA a where a.projeto = id_projeto);
			insert into DES_LAGOA (id, projeto, zona, nome, geometry) (select SEQ_DES_LAGOA.nextval, id_projeto, a.zona, a.nome, a.geometry from TMP_LAGOA a where a.projeto = id_projeto);
			insert into DES_REPRESA (id, projeto, amortecimento, nome, geometry) (select SEQ_DES_REPRESA.nextval, id_projeto, a.amortecimento, a.nome, a.geometry from TMP_REPRESA a where a.projeto = id_projeto);
         insert into DES_DUNA (id, projeto, geometry) (select SEQ_DES_DUNA.nextval, id_projeto, a.geometry from TMP_DUNA a where a.projeto = id_projeto);
         insert into DES_REST_DECLIVIDADE (id, projeto, geometry) (select SEQ_DES_REST_DECLIVIDADE.nextval, id_projeto, a.geometry from TMP_REST_DECLIVIDADE a where a.projeto = id_projeto);
         insert into DES_ESCARPA (id, projeto, geometry) (select SEQ_DES_ESCARPA.nextval, id_projeto, a.geometry from TMP_ESCARPA a where a.projeto = id_projeto);

      elsif (tipo = TIPO_ATIVIDADE) then
      	insert into DES_PATIV (id, projeto, geometry) (select SEQ_DES_PATIV.nextval, id_projeto, a.geometry from TMP_PATIV a where a.projeto = id_projeto);
         insert into DES_LATIV (id, projeto, geometry) (select SEQ_DES_LATIV.nextval, id_projeto, a.geometry from TMP_LATIV a where a.projeto = id_projeto);
         insert into DES_AATIV (id, projeto, geometry) (select SEQ_DES_AATIV.nextval, id_projeto, a.geometry from TMP_AATIV a where a.projeto = id_projeto);
         insert into DES_AIATIV (id, projeto, geometry) (select SEQ_DES_AIATIV.nextval, id_projeto, a.geometry from TMP_AIATIV a where a.projeto = id_projeto);
      end if;

     	commit;
   end;
   
   procedure ImportarParaDesenhFinalizada(id_projeto number, tipo number) is
   begin     
      ApagarGeometriasDES(id_projeto);

		if (tipo = TIPO_DOMINIALIDADE) then
         -- Limites
         insert into VW_DES_ATP (id, projeto, geometry) (select SEQ_DES_ATP.nextval, id_projeto, a.geometry from GEO_ATP a where a.projeto = id_projeto);
         insert into VW_DES_APMP (id, projeto, tipo, nome, geometry) (select SEQ_DES_APMP.nextval, id_projeto, a.tipo, a.nome, a.geometry from GEO_APMP a where a.projeto = id_projeto);
         insert into DES_AFD (id, projeto, geometry) (select SEQ_DES_AFD.nextval, id_projeto, a.geometry from GEO_AFD a where a.projeto = id_projeto);
			insert into DES_ROCHA (id, projeto, geometry) (select SEQ_DES_ROCHA.nextval, id_projeto, a.geometry from GEO_ROCHA a where a.projeto = id_projeto);
			insert into DES_VERTICE (id, projeto, nome, geometry) (select SEQ_DES_VERTICE.nextval, id_projeto, a.nome, a.geometry from GEO_VERTICE a where a.projeto = id_projeto);
			insert into DES_ARL (id, projeto, compensada, geometry) (select SEQ_DES_ARL.nextval, id_projeto, a.compensada, a.geometry from GEO_ARL a where a.projeto = id_projeto);
			insert into DES_RPPN (id, projeto, geometry) (select SEQ_DES_RPPN.nextval, id_projeto, a.geometry from GEO_RPPN a where a.projeto = id_projeto);
			insert into DES_AFS (id, projeto, geometry) (select SEQ_DES_AFS.nextval, id_projeto, a.geometry from GEO_AFS a where a.projeto = id_projeto);
			insert into VW_DES_AVN (id, projeto, estagio, geometry) (select SEQ_DES_AVN.nextval, id_projeto, a.estagio, a.geometry from GEO_AVN a where a.projeto = id_projeto);
			insert into VW_DES_AA (id, projeto, tipo, geometry) (select SEQ_DES_AA.nextval, id_projeto, a.tipo, a.geometry from GEO_AA a where a.projeto = id_projeto);
			insert into DES_ACV (id, projeto, tipo, geometry) (select SEQ_DES_ACV.nextval, id_projeto, a.tipo, a.geometry from GEO_ACV a where a.projeto = id_projeto);
         
         -- Outros
			insert into DES_ACONSTRUIDA (id, projeto, geometry) (select SEQ_DES_ACONSTRUIDA.nextval, id_projeto, a.geometry from GEO_ACONSTRUIDA a where a.projeto = id_projeto);
         insert into DES_DUTO (id, projeto, geometry) (select SEQ_DES_DUTO.nextval, id_projeto, a.geometry from GEO_DUTO a where a.projeto = id_projeto);
         insert into DES_LTRANSMISSAO (id, projeto, geometry) (select SEQ_DES_LTRANSMISSAO.nextval, id_projeto, a.geometry from GEO_LTRANSMISSAO a where a.projeto = id_projeto);
         insert into DES_ESTRADA (id, projeto, geometry) (select SEQ_DES_ESTRADA.nextval, id_projeto, a.geometry from GEO_ESTRADA a where a.projeto = id_projeto);
         insert into DES_FERROVIA (id, projeto, geometry) (select SEQ_DES_FERROVIA.nextval, id_projeto, a.geometry from GEO_FERROVIA a where a.projeto = id_projeto);
         
         -- Itens da APP
         insert into DES_NASCENTE (id, projeto, geometry) (select SEQ_DES_NASCENTE.nextval, id_projeto, a.geometry from GEO_NASCENTE a where a.projeto = id_projeto);
         insert into DES_RIO_LINHA (id, projeto, nome, largura, geometry) (select SEQ_DES_RIO_LINHA.nextval, id_projeto, a.nome, a.largura, a.geometry from GEO_RIO_LINHA a where a.projeto = id_projeto);
         insert into DES_RIO_AREA (id, projeto, nome, largura, geometry) (select SEQ_DES_RIO_AREA.nextval, id_projeto, a.nome, a.largura, a.geometry from GEO_RIO_AREA a where a.projeto = id_projeto);
			insert into DES_LAGOA (id, projeto, zona, nome, geometry) (select SEQ_DES_LAGOA.nextval, id_projeto, a.zona, a.nome, a.geometry from GEO_LAGOA a where a.projeto = id_projeto);
			insert into DES_REPRESA (id, projeto, amortecimento, nome, geometry) (select SEQ_DES_REPRESA.nextval, id_projeto, a.amortecimento, a.nome, a.geometry from GEO_REPRESA a where a.projeto = id_projeto);
         insert into DES_DUNA (id, projeto, geometry) (select SEQ_DES_DUNA.nextval, id_projeto, a.geometry from GEO_DUNA a where a.projeto = id_projeto);
         insert into DES_REST_DECLIVIDADE (id, projeto, geometry) (select SEQ_DES_REST_DECLIVIDADE.nextval, id_projeto, a.geometry from GEO_REST_DECLIVIDADE a where a.projeto = id_projeto);
         insert into DES_ESCARPA (id, projeto, geometry) (select SEQ_DES_ESCARPA.nextval, id_projeto, a.geometry from GEO_ESCARPA a where a.projeto = id_projeto);

      elsif (tipo = TIPO_ATIVIDADE) then
      	insert into DES_PATIV (id, projeto, geometry) (select SEQ_DES_PATIV.nextval, id_projeto, a.geometry from GEO_PATIV a where a.projeto = id_projeto);
         insert into DES_LATIV (id, projeto, geometry) (select SEQ_DES_LATIV.nextval, id_projeto, a.geometry from GEO_LATIV a where a.projeto = id_projeto);
         insert into DES_AATIV (id, projeto, geometry) (select SEQ_DES_AATIV.nextval, id_projeto, a.geometry from GEO_AATIV a where a.projeto = id_projeto);
         insert into DES_AIATIV (id, projeto, geometry) (select SEQ_DES_AIATIV.nextval, id_projeto, a.geometry from GEO_AIATIV a where a.projeto = id_projeto);
      end if;

     	commit;
   end;


	procedure ExportarParaTabelasGEO(id_projeto number, tid_code varchar2) is
   begin
		delete from GEO_ATP a where a.projeto = id_projeto;
      delete from GEO_APMP a where a.projeto = id_projeto;
      delete from GEO_AFD a where a.projeto = id_projeto;
      delete from GEO_ROCHA a where a.projeto = id_projeto;
      delete from GEO_VERTICE a where a.projeto = id_projeto;
      delete from GEO_ARL a where a.projeto = id_projeto;
      delete from GEO_RPPN a where a.projeto = id_projeto;
      delete from GEO_AFS a where a.projeto = id_projeto;
      delete from GEO_AVN a where a.projeto = id_projeto;
      delete from GEO_AA a where a.projeto = id_projeto;
      delete from GEO_ACV a where a.projeto = id_projeto;
         
      -- Outros
      delete from GEO_ACONSTRUIDA a where a.projeto = id_projeto;

      delete from GEO_DUTO a where a.projeto = id_projeto;
      delete from GEO_LTRANSMISSAO a where a.projeto = id_projeto;
      delete from GEO_ESTRADA a where a.projeto = id_projeto;
      delete from GEO_FERROVIA a where a.projeto = id_projeto;
         
      -- Itens da APP
      delete from GEO_NASCENTE a where a.projeto = id_projeto;
      delete from GEO_RIO_LINHA a where a.projeto = id_projeto;
      delete from GEO_RIO_AREA a where a.projeto = id_projeto;
      delete from GEO_LAGOA a where a.projeto = id_projeto;
      delete from GEO_REPRESA a where a.projeto = id_projeto;
      delete from GEO_DUNA a where a.projeto = id_projeto;
      delete from GEO_REST_DECLIVIDADE a where a.projeto = id_projeto;
      delete from GEO_ESCARPA a where a.projeto = id_projeto;

      delete from GEO_PATIV a where a.projeto = id_projeto;
      delete from GEO_LATIV a where a.projeto = id_projeto;
      delete from GEO_AATIV a where a.projeto = id_projeto;
      delete from GEO_AIATIV a where a.projeto = id_projeto;

   
   
      -- Limites
      insert into GEO_ATP (id, projeto, area_m2, geometry, data, tid) (select a.id, a.projeto, a.area_m2, a.geometry, sysdate, tid_code from TMP_ATP a where a.projeto = id_projeto);
      insert into GEO_APMP (id, projeto, tipo, nome, cod_atp, area_m2, geometry, data, tid) (select a.id, a.projeto, a.tipo, a.nome, a.cod_atp, a.area_m2, a.geometry, sysdate, tid_code from TMP_APMP a where a.projeto = id_projeto);
      insert into GEO_AFD (id, projeto, area_m2, geometry, data, tid) (select a.id, a.projeto, a.area_m2, a.geometry, sysdate, tid_code from TMP_AFD a where a.projeto = id_projeto);
      insert into GEO_ROCHA (id, projeto, cod_apmp, area_m2, geometry, data, tid) (select a.id, a.projeto, a.cod_apmp, a.area_m2, a.geometry, sysdate, tid_code from TMP_ROCHA a where a.projeto = id_projeto);
      insert into GEO_VERTICE (id, projeto, nome, geometry, data, tid) (select a.id, a.projeto, a.nome, a.geometry, sysdate, tid_code from TMP_VERTICE a where a.projeto = id_projeto);
      insert into GEO_ARL (id, projeto, cod_apmp, compensada, situacao, codigo, area_m2, geometry, data, tid) (select a.id, a.projeto, a.cod_apmp, a.compensada, a.situacao, a.codigo, a.area_m2, a.geometry, sysdate, tid_code from TMP_ARL a where a.projeto = id_projeto);
      insert into GEO_RPPN (id, projeto, cod_apmp, area_m2, geometry, data, tid) (select a.id, a.projeto, a.cod_apmp, a.area_m2, a.geometry, sysdate, tid_code from TMP_RPPN a where a.projeto = id_projeto);
      insert into GEO_AFS (id, projeto, cod_apmp, area_m2, geometry, data, tid) (select a.id, a.projeto, a.cod_apmp, a.area_m2, a.geometry, sysdate, tid_code from TMP_AFS a where a.projeto = id_projeto);
      insert into GEO_AVN (id, projeto, estagio, cod_apmp, area_m2, geometry, data, tid) (select a.id, a.projeto, a.estagio, a.cod_apmp, a.area_m2, a.geometry, sysdate, tid_code from TMP_AVN a where a.projeto = id_projeto);
      insert into GEO_AA (id, projeto, tipo, cod_apmp, area_m2, geometry, data, tid) (select a.id, a.projeto, a.tipo, a.cod_apmp, a.area_m2, a.geometry, sysdate, tid_code from TMP_AA a where a.projeto = id_projeto);
      insert into GEO_ACV (id, projeto, tipo, cod_apmp, area_m2, geometry, data, tid) (select a.id, a.projeto, a.tipo, a.cod_apmp, a.area_m2, a.geometry, sysdate, tid_code from TMP_ACV a where a.projeto = id_projeto);
         
      -- Outros
      insert into GEO_ACONSTRUIDA (id, projeto, area_m2, geometry, data, tid) (select a.id, a.projeto, a.area_m2, a.geometry, sysdate, tid_code from TMP_ACONSTRUIDA a where a.projeto = id_projeto);
      insert into GEO_DUTO (id, projeto, geometry, data, tid) (select a.id, a.projeto, a.geometry, sysdate, tid_code from TMP_DUTO a where a.projeto = id_projeto);
      insert into GEO_LTRANSMISSAO (id, projeto, geometry, data, tid) (select a.id, a.projeto, a.geometry, sysdate, tid_code from TMP_LTRANSMISSAO a where a.projeto = id_projeto);
      insert into GEO_ESTRADA (id, projeto, geometry, data, tid) (select a.id, a.projeto, a.geometry, sysdate, tid_code from TMP_ESTRADA a where a.projeto = id_projeto);
      insert into GEO_FERROVIA (id, projeto, geometry, data, tid) (select a.id, a.projeto, a.geometry, sysdate, tid_code from TMP_FERROVIA a where a.projeto = id_projeto);
         
      -- Itens da APP
      insert into GEO_NASCENTE (id, projeto, geometry, data, tid) (select a.id, a.projeto, a.geometry, sysdate, tid_code from TMP_NASCENTE a where a.projeto = id_projeto);
      insert into GEO_RIO_LINHA (id, projeto, nome, largura, geometry, data, tid) (select a.id, a.projeto, a.nome, a.largura, a.geometry, sysdate, tid_code from TMP_RIO_LINHA a where a.projeto = id_projeto);
      insert into GEO_RIO_AREA (id, projeto, cod_apmp, area_m2, nome, largura, geometry, data, tid) (select a.id, a.projeto, a.cod_apmp, a.area_m2, a.nome, a.largura, a.geometry, sysdate, tid_code from TMP_RIO_AREA a where a.projeto = id_projeto);
      insert into GEO_LAGOA (id, projeto, cod_apmp, area_m2, zona, nome, geometry, data, tid) (select a.id, a.projeto, a.cod_apmp, a.area_m2, a.zona, a.nome, a.geometry, sysdate, tid_code from TMP_LAGOA a where a.projeto = id_projeto);
      insert into GEO_REPRESA (id, projeto, cod_apmp, area_m2, amortecimento, nome, geometry, data, tid) (select a.id, a.projeto, a.cod_apmp, a.area_m2, a.amortecimento, a.nome, a.geometry, sysdate, tid_code from TMP_REPRESA a where a.projeto = id_projeto);
      insert into GEO_DUNA (id, projeto, geometry, data, tid) (select a.id, a.projeto, a.geometry, sysdate, tid_code from TMP_DUNA a where a.projeto = id_projeto);
      insert into GEO_REST_DECLIVIDADE (id, projeto, geometry, data, tid) (select a.id, a.projeto, a.geometry, sysdate, tid_code from TMP_REST_DECLIVIDADE a where a.projeto = id_projeto);
      insert into GEO_ESCARPA (id, projeto, geometry, data, tid) (select a.id, a.projeto, a.geometry, sysdate, tid_code from TMP_ESCARPA a where a.projeto = id_projeto);

      insert into GEO_PATIV (id, projeto, cod_apmp, codigo, atividade, rocha, massa_dagua, avn, aa, afs, floresta_plantada, arl, rppn, app, geometry, data, tid) (select a.id, a.projeto, a.cod_apmp, a.codigo, a.atividade, a.rocha, a.massa_dagua, a.avn, a.aa, a.afs, a.floresta_plantada, a.arl, a.rppn, a.app, a.geometry, sysdate, tid_code from TMP_PATIV a where a.projeto = id_projeto);
      insert into GEO_LATIV (id, projeto, comprimento, cod_apmp, codigo, atividade, rocha, massa_dagua, avn, aa, afs, floresta_plantada, arl, rppn, app, geometry, data, tid) (select a.id, a.projeto, a.comprimento, a.cod_apmp, a.codigo, a.atividade, a.rocha, a.massa_dagua, a.avn, a.aa, a.afs, a.floresta_plantada, a.arl, a.rppn, a.app, a.geometry, sysdate, tid_code from TMP_LATIV a where a.projeto = id_projeto);
      insert into GEO_AATIV (id, projeto, area_m2, cod_apmp, codigo, atividade, rocha, massa_dagua, avn, aa, afs, floresta_plantada, arl, rppn, app, geometry, data, tid) (select a.id, a.projeto, a.area_m2, a.cod_apmp, a.codigo, a.atividade, a.rocha, a.massa_dagua, a.avn, a.aa, a.afs, a.floresta_plantada, a.arl, a.rppn, a.app, a.geometry, sysdate, tid_code from TMP_AATIV a where a.projeto = id_projeto);
      insert into GEO_AIATIV (id, projeto, area_m2, cod_apmp, codigo, atividade, rocha, massa_dagua, avn, aa, afs, floresta_plantada, arl, rppn, app, geometry, data, tid) (select a.id, a.projeto, a.area_m2, a.cod_apmp, a.codigo, a.atividade, a.rocha, a.massa_dagua, a.avn, a.aa, a.afs, a.floresta_plantada, a.arl, a.rppn, a.app, a.geometry, sysdate, tid_code from TMP_AIATIV a where a.projeto = id_projeto);
         
   end;
   
   procedure GerarHistoricoGEO(id_projeto number, p_acao number, p_executor_id number, p_executor_nome varchar2, p_executor_login varchar2, p_executor_tipo_id number, p_executor_tid varchar2) is
      v_executor_tipo_texto varchar2(30);
      v_acao_executada number;
      v_data_execucao timestamp(6);
   begin
      select ltf.texto into v_executor_tipo_texto from idaf.lov_executor_tipo ltf where ltf.id = p_executor_tipo_id;
      select la.id into v_acao_executada from idaf.lov_historico_artefatos_acoes la where la.acao = p_acao and la.artefato = 1;
      v_data_execucao := systimestamp;

   	-- Limites
      insert into HST_ATP (id, feature_id, projeto, area_m2, geometry, data, tid, executor_id, executor_tid, executor_nome, executor_login, executor_tipo_id, executor_tipo_texto, acao_executada, data_execucao) (select seq_hst_atp.nextval, a.id, a.projeto, a.area_m2, a.geometry, a.data, a.tid, p_executor_id, p_executor_tid, p_executor_nome, p_executor_login, p_executor_tipo_id, v_executor_tipo_texto, v_acao_executada, v_data_execucao from GEO_ATP a where a.projeto = id_projeto);
      insert into HST_APMP (id, feature_id, projeto, tipo, nome, cod_atp, area_m2, geometry, data, tid, executor_id, executor_tid, executor_nome, executor_login, executor_tipo_id, executor_tipo_texto, acao_executada, data_execucao) (select seq_hst_apmp.nextval, a.id, a.projeto, a.tipo, a.nome, a.cod_atp, a.area_m2, a.geometry, a.data, a.tid, p_executor_id, p_executor_tid, p_executor_nome, p_executor_login, p_executor_tipo_id, v_executor_tipo_texto, v_acao_executada, v_data_execucao from GEO_APMP a where a.projeto = id_projeto);
      insert into HST_AFD (id, feature_id, projeto, area_m2, geometry, data, tid, executor_id, executor_tid, executor_nome, executor_login, executor_tipo_id, executor_tipo_texto, acao_executada, data_execucao) (select seq_hst_afd.nextval, a.id, a.projeto, a.area_m2, a.geometry, a.data, a.tid, p_executor_id, p_executor_tid, p_executor_nome, p_executor_login, p_executor_tipo_id, v_executor_tipo_texto, v_acao_executada, v_data_execucao from GEO_AFD a where a.projeto = id_projeto);
      insert into HST_ROCHA (id, feature_id, projeto, cod_apmp, area_m2, geometry, data, tid, executor_id, executor_tid, executor_nome, executor_login, executor_tipo_id, executor_tipo_texto, acao_executada, data_execucao) (select seq_hst_rocha.nextval, a.id, a.projeto, a.cod_apmp, a.area_m2, a.geometry, a.data, a.tid, p_executor_id, p_executor_tid, p_executor_nome, p_executor_login, p_executor_tipo_id, v_executor_tipo_texto, v_acao_executada, v_data_execucao from GEO_ROCHA a where a.projeto = id_projeto);
      insert into HST_VERTICE (id, feature_id, projeto, nome, geometry, data, tid, executor_id, executor_tid, executor_nome, executor_login, executor_tipo_id, executor_tipo_texto, acao_executada, data_execucao) (select seq_hst_vertice.nextval, a.id, a.projeto, a.nome, a.geometry, a.data, a.tid, p_executor_id, p_executor_tid, p_executor_nome, p_executor_login, p_executor_tipo_id, v_executor_tipo_texto, v_acao_executada, v_data_execucao from GEO_VERTICE a where a.projeto = id_projeto);
      insert into HST_ARL (id, feature_id, projeto, compensada, situacao, codigo, area_m2, geometry, data, tid, executor_id, executor_tid, executor_nome, executor_login, executor_tipo_id, executor_tipo_texto, acao_executada, data_execucao) (select seq_hst_arl.nextval, a.id, a.projeto, a.compensada, a.situacao, a.codigo, a.area_m2, a.geometry, a.data, a.tid, p_executor_id, p_executor_tid, p_executor_nome, p_executor_login, p_executor_tipo_id, v_executor_tipo_texto, v_acao_executada, v_data_execucao from GEO_ARL a where a.projeto = id_projeto);
      insert into HST_RPPN (id, feature_id, projeto, cod_apmp, area_m2, geometry, data, tid, executor_id, executor_tid, executor_nome, executor_login, executor_tipo_id, executor_tipo_texto, acao_executada, data_execucao) (select seq_hst_rppn.nextval, a.id, a.projeto, a.cod_apmp, a.area_m2, a.geometry, a.data, a.tid, p_executor_id, p_executor_tid, p_executor_nome, p_executor_login, p_executor_tipo_id, v_executor_tipo_texto, v_acao_executada, v_data_execucao from GEO_RPPN a where a.projeto = id_projeto);
      insert into HST_AFS (id, feature_id, projeto, cod_apmp, area_m2, geometry, data, tid, executor_id, executor_tid, executor_nome, executor_login, executor_tipo_id, executor_tipo_texto, acao_executada, data_execucao) (select seq_hst_afs.nextval, a.id, a.projeto, a.cod_apmp, a.area_m2, a.geometry, a.data, a.tid, p_executor_id, p_executor_tid, p_executor_nome, p_executor_login, p_executor_tipo_id, v_executor_tipo_texto, v_acao_executada, v_data_execucao from GEO_AFS a where a.projeto = id_projeto);
      insert into HST_AVN (id, feature_id, projeto, estagio, cod_apmp, area_m2, geometry, data, tid, executor_id, executor_tid, executor_nome, executor_login, executor_tipo_id, executor_tipo_texto, acao_executada, data_execucao) (select seq_hst_avn.nextval, a.id, a.projeto, a.estagio, a.cod_apmp, a.area_m2, a.geometry, a.data, a.tid, p_executor_id, p_executor_tid, p_executor_nome, p_executor_login, p_executor_tipo_id, v_executor_tipo_texto, v_acao_executada, v_data_execucao from GEO_AVN a where a.projeto = id_projeto);
      insert into HST_AA (id, feature_id, projeto, tipo, cod_apmp, area_m2, geometry, data, tid, executor_id, executor_tid, executor_nome, executor_login, executor_tipo_id, executor_tipo_texto, acao_executada, data_execucao) (select seq_hst_aa.nextval, a.id, a.projeto, a.tipo, a.cod_apmp, a.area_m2, a.geometry, a.data, a.tid, p_executor_id, p_executor_tid, p_executor_nome, p_executor_login, p_executor_tipo_id, v_executor_tipo_texto, v_acao_executada, v_data_execucao from GEO_AA a where a.projeto = id_projeto);
      insert into HST_ACV (id, feature_id, projeto, tipo, cod_apmp, area_m2, geometry, data, tid, executor_id, executor_tid, executor_nome, executor_login, executor_tipo_id, executor_tipo_texto, acao_executada, data_execucao) (select seq_hst_acv.nextval, a.id, a.projeto, a.tipo, a.cod_apmp, a.area_m2, a.geometry, a.data, a.tid, p_executor_id, p_executor_tid, p_executor_nome, p_executor_login, p_executor_tipo_id, v_executor_tipo_texto, v_acao_executada, v_data_execucao from GEO_ACV a where a.projeto = id_projeto);
         
      -- Outros
      insert into HST_ACONSTRUIDA (id, feature_id, projeto, area_m2, geometry, data, tid, executor_id, executor_tid, executor_nome, executor_login, executor_tipo_id, executor_tipo_texto, acao_executada, data_execucao) (select seq_hst_aconstruida.nextval, a.id, a.projeto, a.area_m2, a.geometry, a.data, a.tid, p_executor_id, p_executor_tid, p_executor_nome, p_executor_login, p_executor_tipo_id, v_executor_tipo_texto, v_acao_executada, v_data_execucao from GEO_ACONSTRUIDA a where a.projeto = id_projeto);
      insert into HST_DUTO (id, feature_id, projeto, geometry, data, tid, executor_id, executor_tid, executor_nome, executor_login, executor_tipo_id, executor_tipo_texto, acao_executada, data_execucao) (select seq_hst_duto.nextval, a.id, a.projeto, a.geometry, a.data, a.tid, p_executor_id, p_executor_tid, p_executor_nome, p_executor_login, p_executor_tipo_id, v_executor_tipo_texto, v_acao_executada, v_data_execucao from GEO_DUTO a where a.projeto = id_projeto);
      insert into HST_LTRANSMISSAO (id, feature_id, projeto, geometry, data, tid, executor_id, executor_tid, executor_nome, executor_login, executor_tipo_id, executor_tipo_texto, acao_executada, data_execucao) (select seq_hst_ltransmissao.nextval, a.id, a.projeto, a.geometry, a.data, a.tid, p_executor_id, p_executor_tid, p_executor_nome, p_executor_login, p_executor_tipo_id, v_executor_tipo_texto, v_acao_executada, v_data_execucao from GEO_LTRANSMISSAO a where a.projeto = id_projeto);
      insert into HST_ESTRADA (id, feature_id, projeto, geometry, data, tid, executor_id, executor_tid, executor_nome, executor_login, executor_tipo_id, executor_tipo_texto, acao_executada, data_execucao) (select seq_hst_estrada.nextval, a.id, a.projeto, a.geometry, a.data, a.tid, p_executor_id, p_executor_tid, p_executor_nome, p_executor_login, p_executor_tipo_id, v_executor_tipo_texto, v_acao_executada, v_data_execucao from GEO_ESTRADA a where a.projeto = id_projeto);
      insert into HST_FERROVIA (id, feature_id, projeto, geometry, data, tid, executor_id, executor_tid, executor_nome, executor_login, executor_tipo_id, executor_tipo_texto, acao_executada, data_execucao) (select seq_hst_ferrovia.nextval, a.id, a.projeto, a.geometry, a.data, a.tid, p_executor_id, p_executor_tid, p_executor_nome, p_executor_login, p_executor_tipo_id, v_executor_tipo_texto, v_acao_executada, v_data_execucao from GEO_FERROVIA a where a.projeto = id_projeto);
         
      -- Itens da APP
      insert into HST_NASCENTE (id, feature_id, projeto, geometry, data, tid, executor_id, executor_tid, executor_nome, executor_login, executor_tipo_id, executor_tipo_texto, acao_executada, data_execucao) (select seq_hst_nascente.nextval, a.id, a.projeto, a.geometry, a.data, a.tid, p_executor_id, p_executor_tid, p_executor_nome, p_executor_login, p_executor_tipo_id, v_executor_tipo_texto, v_acao_executada, v_data_execucao from GEO_NASCENTE a where a.projeto = id_projeto);
      insert into HST_RIO_LINHA (id, feature_id, projeto, nome, largura, geometry, data, tid, executor_id, executor_tid, executor_nome, executor_login, executor_tipo_id, executor_tipo_texto, acao_executada, data_execucao) (select seq_hst_rio_linha.nextval, a.id, a.projeto, a.nome, a.largura, a.geometry, a.data, a.tid, p_executor_id, p_executor_tid, p_executor_nome, p_executor_login, p_executor_tipo_id, v_executor_tipo_texto, v_acao_executada, v_data_execucao from GEO_RIO_LINHA a where a.projeto = id_projeto);
      insert into HST_RIO_AREA (id, feature_id, projeto, cod_apmp, area_m2, nome, largura, geometry, data, tid, executor_id, executor_tid, executor_nome, executor_login, executor_tipo_id, executor_tipo_texto, acao_executada, data_execucao) (select seq_hst_rio_area.nextval, a.id, a.projeto, a.cod_apmp, a.area_m2, a.nome, a.largura, a.geometry, a.data, a.tid, p_executor_id, p_executor_tid, p_executor_nome, p_executor_login, p_executor_tipo_id, v_executor_tipo_texto, v_acao_executada, v_data_execucao from GEO_RIO_AREA a where a.projeto = id_projeto);
      insert into HST_LAGOA (id, feature_id, projeto, cod_apmp, area_m2, zona, nome, geometry, data, tid, executor_id, executor_tid, executor_nome, executor_login, executor_tipo_id, executor_tipo_texto, acao_executada, data_execucao) (select seq_hst_lagoa.nextval, a.id, a.projeto, a.cod_apmp, a.area_m2, a.zona, a.nome, a.geometry, a.data, a.tid, p_executor_id, p_executor_tid, p_executor_nome, p_executor_login, p_executor_tipo_id, v_executor_tipo_texto, v_acao_executada, v_data_execucao from GEO_LAGOA a where a.projeto = id_projeto);
      insert into HST_REPRESA (id, feature_id, projeto, cod_apmp, area_m2, amortecimento, nome, geometry, data, tid, executor_id, executor_tid, executor_nome, executor_login, executor_tipo_id, executor_tipo_texto, acao_executada, data_execucao) (select seq_hst_represa.nextval, a.id, a.projeto, a.cod_apmp, a.area_m2, a.amortecimento, a.nome, a.geometry, a.data, a.tid, p_executor_id, p_executor_tid, p_executor_nome, p_executor_login, p_executor_tipo_id, v_executor_tipo_texto, v_acao_executada, v_data_execucao from GEO_REPRESA a where a.projeto = id_projeto);
      insert into HST_DUNA (id, feature_id, projeto, geometry, data, tid, executor_id, executor_tid, executor_nome, executor_login, executor_tipo_id, executor_tipo_texto, acao_executada, data_execucao) (select seq_hst_duna.nextval, a.id, a.projeto, a.geometry, a.data, a.tid, p_executor_id, p_executor_tid, p_executor_nome, p_executor_login, p_executor_tipo_id, v_executor_tipo_texto, v_acao_executada, v_data_execucao from GEO_DUNA a where a.projeto = id_projeto);
      insert into HST_REST_DECLIVIDADE (id, feature_id, projeto, geometry, data, tid, executor_id, executor_tid, executor_nome, executor_login, executor_tipo_id, executor_tipo_texto, acao_executada, data_execucao) (select seq_hst_rest_declividade.nextval, a.id, a.projeto, a.geometry, a.data, a.tid, p_executor_id, p_executor_tid, p_executor_nome, p_executor_login, p_executor_tipo_id, v_executor_tipo_texto, v_acao_executada, v_data_execucao from GEO_REST_DECLIVIDADE a where a.projeto = id_projeto);
      insert into HST_ESCARPA (id, feature_id, projeto, geometry, data, tid, executor_id, executor_tid, executor_nome, executor_login, executor_tipo_id, executor_tipo_texto, acao_executada, data_execucao) (select seq_hst_escarpa.nextval, a.id, a.projeto, a.geometry, a.data, a.tid, p_executor_id, p_executor_tid, p_executor_nome, p_executor_login, p_executor_tipo_id, v_executor_tipo_texto, v_acao_executada, v_data_execucao from GEO_ESCARPA a where a.projeto = id_projeto);

      insert into HST_PATIV (id, feature_id, projeto, cod_apmp, codigo, atividade, rocha, massa_dagua, avn, aa, afs, floresta_plantada, arl, rppn, app, geometry, data, tid, executor_id, executor_tid, executor_nome, executor_login, executor_tipo_id, executor_tipo_texto, acao_executada, data_execucao) (select seq_hst_pativ.nextval, a.id, a.projeto, a.cod_apmp, a.codigo, a.atividade, a.rocha, a.massa_dagua, a.avn, a.aa, a.afs, a.floresta_plantada, a.arl, a.rppn, a.app, a.geometry, a.data, a.tid, p_executor_id, p_executor_tid, p_executor_nome, p_executor_login, p_executor_tipo_id, v_executor_tipo_texto, v_acao_executada, v_data_execucao from GEO_PATIV a where a.projeto = id_projeto);
      insert into HST_LATIV (id, feature_id, projeto, comprimento, cod_apmp, codigo, atividade, rocha, massa_dagua, avn, aa, afs, floresta_plantada, arl, rppn, app, geometry, data, tid, executor_id, executor_tid, executor_nome, executor_login, executor_tipo_id, executor_tipo_texto, acao_executada, data_execucao) (select seq_hst_lativ.nextval, a.id, a.projeto, a.comprimento, a.cod_apmp, a.codigo, a.atividade, a.rocha, a.massa_dagua, a.avn, a.aa, a.afs, a.floresta_plantada, a.arl, a.rppn, a.app, a.geometry, a.data, a.tid, p_executor_id, p_executor_tid, p_executor_nome, p_executor_login, p_executor_tipo_id, v_executor_tipo_texto, v_acao_executada, v_data_execucao from GEO_LATIV a where a.projeto = id_projeto);
      insert into HST_AATIV (id, feature_id, projeto, area_m2, cod_apmp, codigo, atividade, rocha, massa_dagua, avn, aa, afs, floresta_plantada, arl, rppn, app, geometry, data, tid, executor_id, executor_tid, executor_nome, executor_login, executor_tipo_id, executor_tipo_texto, acao_executada, data_execucao) (select seq_hst_aativ.nextval, a.id, a.projeto, a.area_m2, a.cod_apmp, a.codigo, a.atividade, a.rocha, a.massa_dagua, a.avn, a.aa, a.afs, a.floresta_plantada, a.arl, a.rppn, a.app, a.geometry, a.data, a.tid, p_executor_id, p_executor_tid, p_executor_nome, p_executor_login, p_executor_tipo_id, v_executor_tipo_texto, v_acao_executada, v_data_execucao from GEO_AATIV a where a.projeto = id_projeto);
      insert into HST_AIATIV (id, feature_id, projeto, area_m2, cod_apmp, codigo, atividade, rocha, massa_dagua, avn, aa, afs, floresta_plantada, arl, rppn, app, geometry, data, tid, executor_id, executor_tid, executor_nome, executor_login, executor_tipo_id, executor_tipo_texto, acao_executada, data_execucao) (select seq_hst_aiativ.nextval, a.id, a.projeto, a.area_m2, a.cod_apmp, a.codigo, a.atividade, a.rocha, a.massa_dagua, a.avn, a.aa, a.afs, a.floresta_plantada, a.arl, a.rppn, a.app, a.geometry, a.data, a.tid, p_executor_id, p_executor_tid, p_executor_nome, p_executor_login, p_executor_tipo_id, v_executor_tipo_texto, v_acao_executada, v_data_execucao from GEO_AIATIV a where a.projeto = id_projeto);

   end;
   
end;
/
