package models.Esri
{
	import com.esri.ags.Map;
	import com.esri.ags.events.IdentifyEvent;
	import com.esri.ags.events.LayerEvent;
	import com.esri.ags.geometry.Extent;
	import com.esri.ags.geometry.Geometry;
	import com.esri.ags.geometry.MapPoint;
	import com.esri.ags.geometry.Polygon;
	import com.esri.ags.geometry.Polyline;
	import com.esri.ags.layers.ArcGISDynamicMapServiceLayer;
	import com.esri.ags.layers.ArcGISImageServiceLayer;
	import com.esri.ags.layers.ArcGISTiledMapServiceLayer;
	import com.esri.ags.layers.GraphicsLayer;
	import com.esri.ags.layers.Layer;
	import com.esri.ags.layers.WMSLayer;
	import com.esri.ags.tasks.IdentifyTask;
	import com.esri.ags.tasks.supportClasses.IdentifyParameters;
	import com.esri.ags.tasks.supportClasses.IdentifyResult;
	import com.gmaps.geom.Mbr;
	
	import controllers.IdentificarControllerEvent;
	import controllers.IdentifyControllerEvent;
	import controllers.LayerFeicaoController;
	
	import flash.display.Sprite;
	import flash.events.Event;
	import flash.events.MouseEvent;
	import flash.geom.Point;
	import flash.ui.ContextMenu;
	import flash.utils.setTimeout;
	
	import models.AtributoFeicao;
	import models.ColunaLayerFeicao;
	import models.Feicao;
	import models.FeicaoSelecionada;
	import models.FeicoesSelecionadas;
	import models.Geometria;
	import models.Item;
	import models.LayerFeicao;
	
	import mx.controls.Alert;
	import mx.managers.CursorManager;
	import mx.rpc.Fault;
	import mx.rpc.IResponder;
	import mx.rpc.Responder;
	import mx.rpc.soap.LoadEvent;
	
	import tm.spatialReference.Coordinate;
	import tm.spatialReference.CoordinateSystemConverter;
	import tm.spatialReference.CoordinateSystemConverterFactory;
		
	public class DesenhadorEsri extends Sprite
	{
		public static const TILEDMAPSERVICELAYER:String = "arcgis_tiled_map_service_layer";
		public static const DYNAMICMAPSERVICELAYER:String = "arcgis_dynamic_map_service_layer";
		public static const WMSMAPSERVICELAYER:String = "arcgis_wms_map_service_layer";
		public static const IMAGEMAPSERVICELAYER:String = "arcgis_image_map_service_layer";
		
		private var wktProjetado:String = 'PROJCS["SIRGAS 2000 / UTM zone 24S",GEOGCS["SIRGAS 2000",DATUM["D_SIRGAS_2000",SPHEROID["GRS_1980",6378137,298.257222101]],PRIMEM["Greenwich",0],UNIT["Degree",0.017453292519943295]],PROJECTION["Transverse_Mercator"],PARAMETER["latitude_of_origin",0],PARAMETER["central_meridian",-39],PARAMETER["scale_factor",0.9996],PARAMETER["false_easting",500000],PARAMETER["false_northing",10000000],UNIT["Meter",1]]';
		private var wktGeografico:String = 'GEOGCS["SIRGAS 2000",DATUM["D_SIRGAS_2000",SPHEROID["GRS_1980",6378137,298.257222101]],PRIMEM["Greenwich",0],UNIT["Degree",0.017453292519943295]]';
		
		private static var instance:DesenhadorEsri;
		
		private var _map:Map;
		private var _initialLayer:Layer;
		
		public var QUANTIDADE_MAX_RELOAD:uint = 3;
		public var layersJaAdicionadasMapa:Boolean = false; 
		
		public var pontoCliqueInicial:Point;
		public var draw:DrawEsri;		
		public var layersMap:Vector.<Layer>;
		public var layersLoad:Vector.<LayerLoad>;
		public var idLayerMapDesenho:String;
		public var contador:int=0;
		public var idProjeto:int;
		public var idNavegador:int;
		public var projetosAssociados:Array;
		private var carregouMapa:Boolean = false;
		public var servicosIdentificam:Vector.<Item>;
		[Bindable]private var xClique:Number;
		[Bindable]private var yClique:Number;	
		public var graphicDraw:GraphicsLayer; 
		public var areaAbrangencia:Mbr;
		private var numeroTentativas:int = 0;		
		public var modo:int = 1;
		 
		[Embed(source="../assets/erro.png")]
		private var ErroImg:Class;
		
		public function DesenhadorEsri(enforcer:SingletonEnforcer)
		{
			if (enforcer == null)
				throw new Error("DesenhadorEsri é um Singleton, não é permitido outra instancia. Utilize o DesenhadorEsri.getInstance().");
		}

		public function get map():Map
		{
			return _map;
		}

		public function set map(value:Map):void
		{
			_map = value;
			init();
		}

		public static function getInstance():DesenhadorEsri {
			if (instance == null) {
				instance = new DesenhadorEsri( new SingletonEnforcer );	
			}			
			return instance;
		}
		
		public function init():void
		{
			map.addEventListener(MouseEvent.MOUSE_DOWN, pegarPosicaoMouse);
			graphicDraw = new GraphicsLayer();
			draw = new DrawEsri();
		}
		
		protected function pegarPosicaoMouse(event:MouseEvent):void
		{
			xClique = event.stageX;
			yClique = event.stageY;	
		}	
		
		public function comparaPosicaoMouseComMapa(ev:MouseEvent):Boolean
		{
			return (ev.stageX == xClique && ev.stageY == yClique);
		}
		
		public function addServicoMap(layerLoad:LayerLoad):Layer{
			
			var layer:Layer = new Layer();
			
			switch(layerLoad.Type)
			{
				case TILEDMAPSERVICELAYER:
					layer = new ArcGISTiledMapServiceLayer(layerLoad.Url,layerLoad.ProxyURL,layerLoad.Token);
					break;
				case DYNAMICMAPSERVICELAYER:
					layer = new ArcGISDynamicMapServiceLayer(layerLoad.Url,layerLoad.ProxyURL,layerLoad.Token);
					layer.alpha = 0.6;
					if(layerLoad.Filtros)				
						(layer as ArcGISDynamicMapServiceLayer).layerDefinitions = layerLoad.Filtros;
					break;
				case WMSMAPSERVICELAYER:
					layer = new WMSLayer(layerLoad.Url,layerLoad.ProxyURL);
					break;
				case IMAGEMAPSERVICELAYER:
					layer = new ArcGISImageServiceLayer(layerLoad.Url,layerLoad.ProxyURL,layerLoad.Token);
			}
			
			layer.addEventListener(LayerEvent.LOAD_ERROR, loadLayerError);
			layer.addEventListener(LayerEvent.LOAD, loadLayerConcluido);
			
			layer.minScale = layerLoad.MinScale;
			layer.maxScale = layerLoad.MaxScale;
			layer.id = layerLoad.Id; 
			layer.name = layerLoad.Url;
			
			layersMap.splice(layerLoad.Posicao,0,layer);
			return layer;
		}
		
		public function limparMapa():void
		{
			map.removeAllLayers();
			layersLoad = new Vector.<LayerLoad>();
			layersMap = new Vector.<Layer>();
		}
		
		protected function loadLayerConcluido(ev:LayerEvent):void
		{
			if(ev.layer)
			{
				for(var i:int; i<layersLoad.length; i++)
				{
					if(layersLoad[i].Id == ev.layer.id)
					{
						layersLoad[i].IsCarregado = true;
					}
				}	
			}
		}
		
		protected function loadLayerError(ev:LayerEvent):void
		{
			if(ev && ev.fault)
			{
				for(var i:int; i<layersLoad.length; i++)
				{
					if(layersLoad[i].Id == ev.layer.id)
					{
						layersLoad[i].Tentativas++;
						if(layersLoad[i].Tentativas <= QUANTIDADE_MAX_RELOAD)
						{
							var layerRemove:Layer = map.getLayer(layersLoad[i].Id);
							if(layerRemove)
							{
								map.removeLayer(layerRemove);
							}
							
							layersMap.splice(layersLoad[i].Posicao,1);
							var layerMap:Layer = DesenhadorEsri.getInstance().addServicoMap(layersLoad[i]);
							
							if(layersJaAdicionadasMapa)
							{
								map.addLayer(layerMap, layersLoad[i].Posicao);
								ligarDesligarLayer(layerMap.id, layerRemove.visible);
							}
						}
						else
						{
							Alert.show("Ocorreu um erro ao tentar carregar o serviço do mapa '"+ev.layer.name +"'. \n\nTente novamente reabrir o navegador, caso não resolva consulte o administrador.","",4,null,null);
						}
						break;
					}
				}
			}
		}
		
		public function load():void{
			if(layersLoad && layersLoad.length> 0 && (layersLoad[0].IsCarregado || layersLoad[0].Tentativas >= QUANTIDADE_MAX_RELOAD))
			{
				_initialLayer = layersMap[0] as Layer;  
				
				map.addLayer(_initialLayer);
				
				for (var i:uint=1;i<layersMap.length;i++){
					map.addLayer(layersMap[i] as Layer,i); 
				}
				layersJaAdicionadasMapa = true;
				var loadEvent:LoadEvent = new LoadEvent(LoadEvent.LOAD);
				dispatchEvent(loadEvent);
			}
			else
			{
				setTimeout(load,1000);
			}
		}
		
		public function refreshMapDesenho():void
		{
			if(map && idLayerMapDesenho)
			{
				var layer:Layer = map.getLayer(idLayerMapDesenho);
				if(layer)
					layer.refresh();
			}
		}
		
		public function desligarLayersMap():void
		{				
			if(layersMap !=null )
			{
				for each (var layer:Layer in layersMap) {
					layer.visible = false;
				} 
			}
		}
		
		public function ligarLayersMap(layersMap:Array):void
		{				
			if(layersMap !=null)
			{
				for each (var l:String in layersMap) {
					var layer:Layer = map.getLayer(l);
					if (layer == null) continue;
					layer.visible = true;
				} 
			}
			if(idLayerMapDesenho)
			{
				var layer:Layer = map.getLayer(idLayerMapDesenho);
				if(layer)
					layer.visible = true;
			}
		}
		
		public function ligarDesligarLayerFeicao(idLayer:int, isVisivel:Boolean, idServico:int):void
		{	
			if(map && map.getLayer(idServico.toString()) is ArcGISDynamicMapServiceLayer)
			{
				var dyn:ArcGISDynamicMapServiceLayer = map.getLayer(idServico.toString()) as ArcGISDynamicMapServiceLayer;
				if(dyn && dyn.visibleLayers)
				{
					if(isVisivel)
					{
						if (!dyn.visibleLayers.contains(idLayer))
							dyn.visibleLayers.addItem(idLayer);
					}
					else
					{
						if (dyn.visibleLayers.contains(idLayer))
							dyn.visibleLayers.removeItemAt(dyn.visibleLayers.getItemIndex(idLayer));
					}		
				}
			}
		}
		
		public function ligarDesligarLayerFeicaoMap(idLayer:int, idServico:int):void
		{	
			
			if(map.getLayer(idServico.toString()) is ArcGISDynamicMapServiceLayer)
			{
				var dyn:ArcGISDynamicMapServiceLayer = map.getLayer(idServico.toString()) as ArcGISDynamicMapServiceLayer;
				if (dyn.visibleLayers.contains(idLayer))
					dyn.visibleLayers.removeItemAt(dyn.visibleLayers.getItemIndex(idLayer));
				else
					dyn.visibleLayers.addItem(idLayer);
				
				dyn.refresh();
				dispatchEvent(new IdentificarControllerEvent(IdentificarControllerEvent.LAYERS_SERVICOS_ATUALIZAR, servicosIdentificam));
			}
		}
		
		public function ligarDesligarLayer(idServicoMap:String, isLigado:Boolean = true):void
		{
			if (map.getLayer(idServicoMap) && map.getLayer(idServicoMap) is ArcGISDynamicMapServiceLayer)
			{
				var layer:Layer = map.getLayer(idServicoMap);
				if(layer)
				{ 
					layer.visible = isLigado;
				}
			}
		}
		
		public function atualizarTodasLayersFeicaoMap():void
		{	
			if(map.getLayer(idLayerMapDesenho) is ArcGISDynamicMapServiceLayer)
			{
				var dyn:ArcGISDynamicMapServiceLayer = map.getLayer(idLayerMapDesenho) as ArcGISDynamicMapServiceLayer;
				if(layersMap)
				{
					for each(var ly:Layer in layersMap)
					{
						if (dyn.visibleLayers.contains(ly.id))
						{
							dyn.visibleLayers.removeItemAt(dyn.visibleLayers.getItemIndex(ly.id));
							dyn.visibleLayers.addItem(ly.id);
						}
					}
				}
			}
		}
		
		public function buscarImagensLayersMap():void
		{
			if(map.getLayer(idLayerMapDesenho) is ArcGISDynamicMapServiceLayer)
			{
				var resp:IResponder = new mx.rpc.Responder(buscarImagensLayersMapResult,loadError);
				//resp.result(buscarImagensLayersMapResult);
		//		(buscarImagensLayersMapResult);
			
				var dyn:ArcGISDynamicMapServiceLayer = map.getLayer(idLayerMapDesenho) as ArcGISDynamicMapServiceLayer;
				//var token:AsyncToken = 
				dyn.getLegendInfos(resp);
				//token.currentLayer = 1;
				//token.addEventListener("result",buscarImagensLayersMapResult);
				//token.addEventListener("fault", loadError);
			}
		}
		private function loadError(event:Fault):void{
			
			Alert.show(event.faultString + " "+ event.message.toString()  );
			//setTimeout(requestLoadLayer, retryTime, event.target.currentLayer, event.target.currentScale);
		}

		public function buscarImagensLayersMapResult(event:Object):void
		{
			for (var i:int=0;i<event.result.length; i++)
			{
				for (var j:int=0;j<event.result[i].LegendGroups.length; j++){
					if (event.result[i].LegendGroups[j].LegendClasses){
						for (var k:int=0;k<event.result[i].LegendGroups[j].LegendClasses.length; k++){
							Alert.show(event.result[i].LayerID.toString());
							//	var item:ItemLegenda =  new ItemLegenda(k, event.result[i].LayerID, event.target.currentLayer, (event.result[i].LegendGroups[j].LegendClasses.length==1)? event.result[i].Name: event.result[i].Name +" - "+ event.result[i].LegendGroups[j].LegendClasses[k].Label, -1, -1, event.result[i].LegendGroups[j].LegendClasses[k].SymbolImage.ImageData);
						//	getDetails(item);
						}
					}
				}
			}
		}
		
		
		public function converterMouseParaPoint(ev:MouseEvent):Point
		{
			var pt:MapPoint = new MapPoint(0,0);
			if(map)
				pt= map.toMapFromStage(ev.stageX, ev.stageY);
			return new Point(pt.x, pt.y);
		}
		public function converterMouseXYParaPoint(mouseX:Number,mouseY:Number):Point
		{
			var pt:MapPoint = new MapPoint(0,0);
			if(map)
				pt= map.toMapFromStage(mouseX, mouseY);
			return new Point(pt.x, pt.y);
		}
		public function converterPointParaMouse(pt:Point):Point
		{
			var ponto:Point;
			var mpPoint:MapPoint = new MapPoint(pt.x,pt.y);
			if(map)
				ponto= map.toScreen(mpPoint); 
			return ponto;
		}
		public function range(diferenca:Number=4):Number
		{
			var num:Number =0;
			if(map)
				num= (map.extent.width/map.width)*diferenca+(map.extent.height/map.height)*diferenca;
			return num;
		}
		public function desenharFeicao(idGraphic:String, tipo:int, vertices:Vector.<Point>, aneis:Vector.<Vector.<Point>>=null, simbologia:int=1,desenharVertices:Boolean=true,substituir:Boolean=true):String
		{
			var lyGraphics:GraphicsLayer;	
			if(idGraphic != "-1" && map.getLayer(idGraphic.toString()))				
			{
				lyGraphics  = map.getLayer(idGraphic.toString()) as GraphicsLayer;
			}
			else
			{
				contador++;
				idGraphic = "ly"+contador.toString();
				lyGraphics = new GraphicsLayer();
				lyGraphics.id = idGraphic;	
				map.addLayer(lyGraphics);
			}
			
			if(substituir && (lyGraphics.getChildByName(idGraphic.toString()) is GeometriaEsriGraphic))
			{
				(lyGraphics.getChildByName(idGraphic.toString()) as GeometriaEsriGraphic).AtualizarGeometria(vertices, aneis);
				var obj:Object = lyGraphics.getChildByName("vt"+idGraphic.toString());
				if(obj && obj is GeometriaEsriGraphic)
				{
					(obj as GeometriaEsriGraphic).AtualizarGeometria(vertices,aneis);
				}	
			}
			else
			{
				var geoEsri:GeometriaEsri = new GeometriaEsri(tipo, vertices,aneis);
				var geoGrap:GeometriaEsriGraphic;
				geoGrap = new GeometriaEsriGraphic(simbologia,geoEsri);
				geoGrap.name = idGraphic;
				lyGraphics.add(geoGrap);	
				
				if(desenharVertices && tipo != Geometria.Ponto)
				{
					var geoVertices:GeometriaEsriGraphic  = new GeometriaEsriGraphic(simbologia,new GeometriaEsri(Geometria.MultiPontos,vertices, aneis));
					geoVertices.name = "vt"+idGraphic;
					lyGraphics.add(geoVertices);
				}
				
				map.removeLayer(lyGraphics);
				map.addLayer(lyGraphics);
				
			}
			return idGraphic;
		}
		
		public function desenharGeometry(idGraphic:String, geometry:Geometry,
										   tipoSymbol:int,  tipoGeometria:int, substituir:Boolean=true):String
		{
			var lyGraphics:GraphicsLayer;			
			if(idGraphic != "-1" && map.getLayer(idGraphic.toString()))				
			{
				lyGraphics  = map.getLayer(idGraphic.toString()) as GraphicsLayer;
			}
			else
			{
				contador++;
				idGraphic = "ly"+contador.toString();
				lyGraphics = new GraphicsLayer();
				lyGraphics.id = idGraphic;	
				map.addLayer(lyGraphics);
			}
				
			if(substituir && (lyGraphics.getChildByName(idGraphic.toString()) is GeometriaEsriGraphic))
			{
				(lyGraphics.getChildByName(idGraphic.toString()) as GeometriaEsriGraphic).AtualizarGeometry(geometry);
				var obj:Object = lyGraphics.getChildByName("vt"+idGraphic.toString());
				if(obj && obj is GeometriaEsriGraphic)
				{
					(obj as GeometriaEsriGraphic).AtualizarGeometry(geometry);
				}	
			}
			else
			{
				var geoGrap:GeometriaEsriGraphic;				
				geoGrap = new GeometriaEsriGraphic(tipoSymbol,null,geometry,tipoGeometria);
				geoGrap.name = idGraphic;
				lyGraphics.add(geoGrap);					
				map.removeLayer(lyGraphics);
				map.addLayer(lyGraphics);
				
			}
			return idGraphic;
		}
		
		public function excluirFeicao(idGraphic:String):String
		{
			var lyGraphics:GraphicsLayer;
			if(idGraphic != "-1" && map.getLayer(idGraphic.toString()))				
			{
				lyGraphics  = map.getLayer(idGraphic.toString()) as GraphicsLayer;
				map.removeLayer(lyGraphics);
			}
			return "-1";
		}
		
		
		public function identificarFeicoes(ev:DrawEsriEvent):void
		{
			FeicaoSelecionada.getInstance().limpar(false);
			FeicoesSelecionadas.getInstance().limpar(false);
			
			var ar:Array = LayerFeicaoController.getInstance().buscarLayersVisiveis();
			if(ar && ar.length>0)
			{
			
				if(!ev.geometry)
					Alert.show('ev.geometry nulo');
				if(ev && ev.geometry)
				{
					var dyn:ArcGISDynamicMapServiceLayer = map.getLayer(idLayerMapDesenho) as ArcGISDynamicMapServiceLayer;
					if(dyn != null)
					{
						var identifyTask:IdentifyTask = new IdentifyTask();
						identifyTask.concurrency = "last";			
						identifyTask.url = dyn.url;
						
						var identify:IdentifyParameters = new IdentifyParameters();			
						identify.geometry = ev.geometry;	
						
						identify.returnGeometry = true;
						
						identify.layerIds = ar;
						
						identify.tolerance = 3;		
						identify.width = map.width;
						identify.height = map.height;				
						identify.mapExtent = map.extent;
						identify.spatialReference = map.spatialReference;
						identify.layerOption = IdentifyParameters.LAYER_OPTION_ALL;	
						identifyTask.addEventListener(IdentifyEvent.EXECUTE_COMPLETE, identificarResultFuncion);
						identifyTask.execute(identify);	
						CursorManager.removeAllCursors();
						CursorManager.setBusyCursor();
					}
				}
			}
		}
		
		private function identificarResultFuncion(ev:IdentifyEvent):void
		{	
			FeicaoSelecionada.getInstance().limpar(false);
			FeicoesSelecionadas.getInstance().limpar(false);
			
			var  lista:Vector.<Feicao> = new Vector.<Feicao>();
			if(ev && ev.identifyResults)
			{				
				var arResults:Array = ev.identifyResults;
				var result:IdentifyResult;
				var feicao:Feicao;
				var regexp:RegExp = /[^[A-Za-z]]*/ig;
				var isProjeto:Boolean;
				for(var i:int =0; i< arResults.length; i++)
				{
					isProjeto = true;					
					result = arResults[i];
					var tipo:int;
					switch (result.feature.geometry.type)
					{
						case Geometry.MAPPOINT:
						{	
							tipo = Geometria.Ponto;
							break;
						}
						case Geometry.POLYLINE:
						{	
							tipo = Geometria.Linha;
							break;
						}
						case Geometry.POLYGON: 
						{
							tipo = Geometria.Poligono;
							break;
						} 
					}
					feicao = new Feicao(tipo);
					var atributos:Vector.<AtributoFeicao> = new Vector.<AtributoFeicao>();
					for (var atributo:String in result.feature.attributes)
					{	
						var atr:String = atributo.toString().replace(regexp, "");
						if(atr.toString().toUpperCase() == "PROJETO" )
						{
							if(result.feature.attributes[atributo]== idProjeto.toString())
							{
								isProjeto = true;
								feicao.projetoId = idProjeto;
								continue;
							}
							else
							{
							  	if(projetosAssociados)
								{
									isProjeto = false;
									for each(var idProjetoAssociado:String in projetosAssociados)
									{
										if(result.feature.attributes[atributo]== idProjetoAssociado.toString())
										{
											feicao.projetoId =  parseInt(idProjetoAssociado);
											isProjeto = true;
											continue;
										}
									}
								}
								else
								{
									isProjeto = false;
									break;
								}
							}
						}
						
	
						if(atr.toString().toUpperCase() == "ID")
						{
							feicao.objectId = result.feature.attributes[atributo];
						}
						
						
						if(atr.toString().toUpperCase() != "GEOMETRY" && atr.toString().toUpperCase() != "GEOMETRY.LEN" && atr.toString().toUpperCase() != "GEOMETRYAREA" )
						{
							if(atributo.toString().toUpperCase() == "AREA_M2")
							{
								atributos.push(new AtributoFeicao(atributo.toString(), (result.feature.attributes[atributo]).toString() == "Null" ? "": Number(result.feature.attributes[atributo].replace(",",".")).toFixed(2).replace(".",",") ));
							}
							else
								atributos.push(new AtributoFeicao(atributo.toString(), (result.feature.attributes[atributo].toString() == "Null" ? "": result.feature.attributes[atributo]) ));

						}
						
					}
					//Alert.show(feicao.objectId.toString());
					if(isProjeto)
					{
						if(result.feature.geometry)
						{
							feicao.Selecionado = true;
							feicao.geometry = result.feature.geometry;	
							var lyFeicao:LayerFeicao = LayerFeicaoController.getInstance().buscarLayerFeicaoServicoPrincipal(result.layerId);
							feicao.layerFeicao= new LayerFeicao(); // é necessário recriar o objeto para não trabalhar com a mesma referencia
							if(lyFeicao)		
							{
								feicao.layerFeicao.Categoria = lyFeicao.Categoria;
								feicao.layerFeicao.ColunaPk = lyFeicao.ColunaPk;
								feicao.layerFeicao.Id = lyFeicao.Id;
								feicao.layerFeicao.IdLayer = lyFeicao.IdLayer;
								feicao.layerFeicao.Nome = lyFeicao.Nome + (lyFeicao.IsFinalizada ? " - finalizada":"") ;
								feicao.layerFeicao.Selecionavel = lyFeicao.Selecionavel;
								feicao.layerFeicao.TipoGeometria = lyFeicao.TipoGeometria;
								feicao.layerFeicao.Visivel = lyFeicao.Visivel;	
								feicao.layerFeicao.Descricao = lyFeicao.Descricao;
								feicao.layerFeicao.Colunas = new Vector.<ColunaLayerFeicao>();
								var coluna:ColunaLayerFeicao;
								for each(var col:ColunaLayerFeicao in lyFeicao.Colunas)
								{
									coluna = new ColunaLayerFeicao();
									coluna.Alias = col.Alias;
									coluna.Coluna = col.Coluna;
									coluna.ColunaObrigada = col.ColunaObrigada;
									coluna.IdLista = col.IdLista;
									coluna.IsEditavel = col.IsEditavel;
									coluna.IsObrigatorio = col.IsObrigatorio;
									coluna.IsVisivel = col.IsVisivel;
									coluna.Itens = col.Itens;
									coluna.Operacao = col.Operacao;
									coluna.Referencia = col.Referencia;
									coluna.Tamanho = col.Tamanho;
									coluna.Tipo = col.Tipo;
									for(var a:int=atributos.length-1; a>=0; a--)
									{
										if(atributos[a].Nome.toUpperCase() == coluna.Coluna.toUpperCase())
										{
											coluna.Valor = atributos[a].Valor;
											break;
										}
									}
									
									coluna.ValorCondicao = col.ValorCondicao;
									
									feicao.layerFeicao.Colunas.push(coluna);
								}
															
						
								lista.push(feicao);
							}
						}							
					}
				}
			}
			dispatchEvent(new IdentifyControllerEvent(IdentifyControllerEvent.IDENTIFICAR_RESULT, lista as Vector.<Feicao>, pontoCliqueInicial));
					
			//CursorManager.removeBusyCursor();
		}
		
		public function converterGeometryParaGeometria(geometry:Geometry, tipoGeometria:int):Geometria
		{
			var geometria:Geometria = new Geometria(tipoGeometria); 
			switch(tipoGeometria)
			{
				case Geometria.Ponto: 
					geometria.adicionarPonto(new Point((geometry as MapPoint).x, (geometry as MapPoint).y));
					break
				case Geometria.Linha:
					var linhaGeo:Polyline = geometry as Polyline;
					if(linhaGeo)
					{
						for(var i:int=0; i<linhaGeo.paths.length; i++)
						{
							var ar:Array = linhaGeo.paths[i] as Array;
							if(ar)
							{
								for(var k:int=0; k<ar.length; k++)
								{
									geometria.adicionarPonto(new Point((ar[k] as MapPoint).x, (ar[k] as MapPoint).y));
								}
							}
						}
					}
					break;
				case Geometria.Poligono:
					var poligono:Polygon = geometry as Polygon;
					if(poligono)
					{
						var anel:Vector.<Point>;
						geometria.aneis = new Vector.<Vector.<Point>>();
						for(var i:int=0; i<poligono.rings.length; i++)
						{
							var ar:Array = poligono.rings[i] as Array;
							anel = new Vector.<Point>();
							if(ar)
							{ 
								for(var k:int=0; k<ar.length-1; k++)
								{									
									anel.push(new Point((ar[k] as MapPoint).x, (ar[k] as MapPoint).y));
								}
								
								geometria.aneis.push(anel);								
							}
						}
					}		
					break;
			}
			geometria.aneisOriginal = geometria.aneis;
			geometria.verticesOriginal = geometria.vertices;
			return geometria;
		}
		
		public function converterCoordenadaProjParaGeografico(x:Number, y:Number):Coordinate
		{		
			var conversor:CoordinateSystemConverter = CoordinateSystemConverterFactory.getConverter(wktProjetado, wktGeografico);
			return conversor.transform(new Coordinate(x,y));
		}
		public function converterCoordenadaGeografParaProjetado(x:Number, y:Number):Coordinate
		{		
			var conversor:CoordinateSystemConverter = CoordinateSystemConverterFactory.getConverter(wktGeografico,wktProjetado);
			return conversor.transform(new Coordinate(x,y));
		}		
		public function ConverterPontoGeograficoParaGMS(gDec:Number, lat:Boolean = true):String {
			var positivo:Boolean = gDec >= 0;
			gDec = Math.abs(gDec);				
			var grau:Number = Math.floor(gDec);
			var min:Number = Math.floor((gDec - grau)*60);
			var seg:Number = (gDec - grau - min/60)*3600;
			var result:String = grau + "º "+min.toFixed(0)+"' "+seg.toFixed(4)+'"';
			if (lat)
			{
				return positivo ? result + " N" : result + " S"; 
			}
			else
			{
				return positivo ? result + " E" : result + " W";
			}
		}
		
		public function ativarDesativarDraw(ativar:Boolean,tipoGeometria:int, simbologia:int):void
		{
			if(ativar)
			{
				
				draw.activate(tipoGeometria, simbologia);	
			}
			else
			{
				draw.deactivate();			
			}
		}
		
		public function zoomGeometryFeicao(feicao:Feicao):void
		{		
			if(feicao)
			{ 
				if(feicao.tipoGeometria == Geometria.Ponto)
				{
					var pt:MapPoint = (feicao.geometry as MapPoint);
					DesenhadorEsri.getInstance().map.extent = new Extent((pt.x-1), (pt.y - 1), (pt.x+1),(pt.y + 1));					
				}
				else
				{
					DesenhadorEsri.getInstance().map.extent = feicao.geometry.extent;		
				}
			}
		}
		public function zoomMBR(mbr:Mbr):void
		{		
			if(mbr)
			{ 
				DesenhadorEsri.getInstance().map.extent = new Extent(mbr.minX, mbr.minY, mbr.maxX, mbr.maxY);		
			}
		}
		public function zoomAreaAbrangencia(mbr:Mbr=null):void
		{		
			if(!mbr)
				mbr = areaAbrangencia;
			else
				areaAbrangencia = mbr;
			
			if(mbr)
			{ 
				DesenhadorEsri.getInstance().map.extent = new Extent(mbr.minX, mbr.minY, mbr.maxX, mbr.maxY);		
			}
		}
	}
}
class SingletonEnforcer {
}