package models
{
	import com.esri.ags.events.DrawEvent;
	import com.esri.ags.events.MapMouseEvent;
	import com.esri.ags.geometry.Geometry;
	import com.esri.ags.layers.GraphicsLayer;
	import com.esri.ags.tools.DrawTool;
	import com.gmaps.geom.AxisSegment;
	import com.gmaps.geom.GeomArea;
	import com.gmaps.geom.GeomLine;
	import com.gmaps.geom.GeomPoint;
	import com.gmaps.geom.GeomSegment;
	
	import controllers.IdentifyControllerEvent;
	
	import flash.display.Graphics;
	import flash.display.Sprite;
	import flash.events.MouseEvent;
	import flash.events.TimerEvent; 
	import flash.geom.Point;
	import flash.utils.Timer;
	
	import models.Esri.DesenhadorEsri;
	import models.Esri.DrawEsriEvent;
	import models.Esri.GeometriaEsriGraphic;
	
	import mx.controls.Alert;
	import mx.graphics.SolidColor;
	import mx.managers.CursorManager;
	import mx.utils.ObjectUtil;
	
	import spark.primitives.Ellipse;
	import spark.primitives.Graphic;
	
	
	public class FeicoesSnapSelecionadas
	{
		private static var instance:FeicoesSnapSelecionadas;
		
		public var idProjeto:int;
		public var idGraphic:String ="-1";
		public var idGraphicVertices:String ="-1";
		private var _vertices:Vector.<Point>;
		public var pontoMouse:Point;
		private var _buscarPontosProximos:Boolean;
		[Bindable] public var graphic:GraphicsLayer = new GraphicsLayer();		

		private var completeCallback:Function;		
		public var ligado:Boolean = false;
		[Embed( source="../assets/cursor_selecionar.png")]
		private var cursorMouse:Class;
		public var graphics:Graphic;
		private var geomPoints:Vector.<GeomPoint>;
		public var verticesMaisProximos:Vector.<Point>;
		public var segmentos:Array;
		public var timer:Timer;
		public function FeicoesSnapSelecionadas(enforcer:SingletonEnforcer)
		{
			if (enforcer == null)
				throw new Error("FeicoesSnapSelecionadas é um Singleton, não é permitido outra instancia. Utilize FeicoesSnapSelecionadas.getInstance().");
		}
		
		public static function getInstance():FeicoesSnapSelecionadas {
			if (instance == null) {
				instance = new FeicoesSnapSelecionadas( new SingletonEnforcer );
			}
			return instance;
		} 
		
		private var _feicoes:Vector.<Feicao>; 
		
		public function get feicoes():Vector.<Feicao>
		{
			return _feicoes;
		}
		
		public function set feicoes(value:Vector.<Feicao>):void
		{
			_feicoes = value;
			limpar(false);
			desenhar();
			_vertices = null;
			graphics = null;
			geomPoints = null;
		}
		public function get vertices():Vector.<Point>
		{
			//return _vertices;
			if(!_vertices && _feicoes)
			{
				_vertices = new Vector.<Point>();
				for each(var feicao:Feicao in _feicoes)
				{
					if(feicao.geometria)
					{
						if(feicao.geometria.vertices)
						{
							for each(var ponto:Point in feicao.geometria.vertices)
							{
								_vertices.push(ponto);
							}
						}
						if(feicao.geometria.aneis)
						{
							for each(var anel:Vector.<Point> in feicao.geometria.aneis)
							{
								if(anel)
								{
									for each(var ponto:Point in anel)
									{
										_vertices.push(ponto);
									}
								}
							}
						}
					}
				}
			}
			return _vertices;
		}
		
		public function set vertices(value:Vector.<Point>):void
		{	
			_vertices = value;
		}
		public function get buscarPontosProximos():Boolean
		{
			return _buscarPontosProximos;
		}
		
		public function set buscarPontosProximos(value:Boolean):void
		{
			_buscarPontosProximos = value;
			if(_buscarPontosProximos)
				ligarPontosProximos();
		}
		
		public function limpar(excluirFeicoes:Boolean):void
		{
			if(_feicoes )
			{
				for each(var feicao:Feicao in _feicoes)
				{
					if(feicao.geometria)
						feicao.geometria.excluir(excluirFeicoes);
				}
				
				if(excluirFeicoes)
				{
					feicoes = new Vector.<Feicao>();
						
				}
			}
		
				idGraphic = DesenhadorEsri.getInstance().excluirFeicao(idGraphic);
		
		
				idGraphicVertices = DesenhadorEsri.getInstance().excluirFeicao(idGraphicVertices);
			
			
		}
		
		public function ligar():void
		{	
			DesenhadorEsri.getInstance().addEventListener(IdentifyControllerEvent.IDENTIFICAR_RESULT, resultadoIdentificar);
			DesenhadorEsri.getInstance().ativarDesativarDraw(true,Geometria.MBR, GeometriaEsriGraphic.SnapSelecionar);	
			DesenhadorEsri.getInstance().map.addEventListener(DrawEsriEvent.DRAW_END, DesenhadorEsri.getInstance().identificarFeicoes);
			ligado = true;	
			CursorManager.removeAllCursors();
			CursorManager.setCursor(cursorMouse,1,-1, 0);
			ligarPontosProximos();
		}
		private function ligarPontosProximos():void
		{
			if(!timer)
			{		
				timer = new Timer(1000);
				timer.addEventListener(TimerEvent.TIMER,buscarPontosProximosSnap);
			}
			timer.start();
		}
		
		private function buscarPontosProximosSnap(event:TimerEvent = null):void 
		{
			if(buscarPontosProximos)
			{
				if(pontoMouse)
				{
					timer.stop();
					FeicoesSnapSelecionadas.getInstance().desenharVerticesMaisProximos(10);
					timer.start();
				}
			}
			else
				timer.stop();
		}
		public function desligar():void
		{	
			DesenhadorEsri.getInstance().ativarDesativarDraw(false, Geometria.MBR, GeometriaEsriGraphic.SnapSelecionar);
			DesenhadorEsri.getInstance().map.removeEventListener(DrawEsriEvent.DRAW_END, DesenhadorEsri.getInstance().identificarFeicoes);
			DesenhadorEsri.getInstance().removeEventListener(IdentifyControllerEvent.IDENTIFICAR_RESULT, resultadoIdentificar);
			ligado = false;
			
		}
		private function resultadoIdentificar(event:IdentifyControllerEvent):void
		{	
			if (event && event.listaFeicoes && event.listaFeicoes.length > 0)
			{							
				FeicoesSnapSelecionadas.getInstance().feicoes = event.listaFeicoes;
				DesenhadorEsri.getInstance().ativarDesativarDraw(false, Geometria.MBR,GeometriaEsriGraphic.SnapSelecionar);
				DesenhadorEsri.getInstance().removeEventListener(IdentifyControllerEvent.IDENTIFICAR_RESULT, resultadoIdentificar);
			}			
		}	
		public function desenhar():void
		{
			if(_feicoes)
			{
				idGraphic = DesenhadorEsri.getInstance().excluirFeicao(idGraphic);
				for(var i:int=0; i<_feicoes.length; i++)
				{
					idGraphic = DesenhadorEsri.getInstance().desenharGeometry(idGraphic, _feicoes[i].geometry,GeometriaEsriGraphic.Snap,_feicoes[i].tipoGeometria,false);
				}
			}
			else
				idGraphic = DesenhadorEsri.getInstance().excluirFeicao(idGraphic);
			
			if(vertices && idGraphicVertices == "-1")
			{
			//	idGraphicVertices = DesenhadorEsri.getInstance().desenharFeicao(idGraphicVertices,Geometria.MultiPontos,vertices,null,GeometriaEsriGraphic.Snap,false,true);
			}
				
		}
		public function desenharVerticesMaisProximos(quantidade:int):void
		{
			if(!pontoMouse)
				return;
			
			if(!geomPoints)
				geomPoints = converterVerticesParaGeomPoints(vertices);
			
			if(geomPoints)
			{
				verticesMaisProximos = new Vector.<Point>();
				var geomPointsDistancia:Vector.<Vertice> = new Vector.<Vertice>();
				if(geomPoints !=null && geomPoints.length >0)
				{
					var vertice:Vertice;
					for(var k:int =0; k<geomPoints.length;k++)
					{
						vertice = new Vertice(geomPoints[k].point.x, geomPoints[k].point.y);
						vertice.distancia = geomPoints[k].distanceFromPoint(pontoMouse);					
						geomPointsDistancia.push(vertice);					
					}
				}
				
				if(geomPointsDistancia)
				{
					geomPointsDistancia.sort(function (itemA:Object, itemB:Object):int {
						try{
							
							return ObjectUtil.compare(itemA.distancia, itemB.distancia);
						}
						catch(evt:Object){}
						
						return -1;
						
					});
					
					for(var i:int=0; i< geomPointsDistancia.length && i<quantidade; i++)
					{
						verticesMaisProximos.push(new Point((geomPointsDistancia[i] as Vertice).X, (geomPointsDistancia[i] as Vertice).Y));
					}
				}
				idGraphicVertices = DesenhadorEsri.getInstance().desenharFeicao(idGraphicVertices,Geometria.MultiPontos,verticesMaisProximos,null,GeometriaEsriGraphic.SnapVerticeProximo,false,true);
			}
		}
		
		private function converterVerticesParaGeomPoints(geom:Vector.<Point>):Vector.<GeomPoint>
		{
			var ar:Array;
			var lista:Vector.<GeomPoint>;
			if(geom!=null && geom.length>1)
			{
				lista =  new Vector.<GeomPoint>();
				var geomPoint:GeomPoint;
				for(var i:int; i<geom.length; i++)
				{
					geomPoint = new GeomPoint("pt", geom[i]);
					lista.push(geomPoint);
				}				
			}
			return lista;
		}
		public function buscarPontoProximoNoMeioDaAresta(ponto:Point, range:Number):void
		{
			if(!segmentos && feicoes)
			{
				for each(var feicao:Feicao in feicoes)
				{
					if(feicao.tipoGeometria != Geometria.Ponto)
					{
						var geomArea:GeomArea= feicao.geometria.converterVerticesParaGeomArea(feicao.geometria.vertices);
						var seg:GeomSegment = new GeomSegment("a",  new AxisSegment(0,0,0,0));
					}
				}
			}
		}
	}
}
class SingletonEnforcer {
}