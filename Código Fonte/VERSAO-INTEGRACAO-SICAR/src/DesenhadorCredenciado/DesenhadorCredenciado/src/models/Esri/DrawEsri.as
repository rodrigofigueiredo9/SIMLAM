package models.Esri
{
	import com.esri.ags.Map;
	import com.esri.ags.events.MapEvent;
	import com.esri.ags.events.MapMouseEvent;
	import com.esri.ags.events.PanEvent;
	import com.esri.ags.geometry.Extent;
	import com.esri.ags.geometry.Geometry;
	import com.esri.ags.geometry.MapPoint;
	import com.esri.ags.geometry.Polygon;
	import com.esri.ags.geometry.Polyline;
	import com.gmaps.geom.Mbr;
	
	import flash.events.Event;
	import flash.events.KeyboardEvent;
	import flash.events.MouseEvent;
	import flash.geom.Point;
	import flash.ui.Keyboard;
	
	import models.Geometria;
	
	import mx.controls.Alert;

	public class DrawEsri
	{
		public var Tipo:int;
		public var pontoInicial:Point;
		public var pontoFinal:Point;
		public var idGraphic:String = "-1";
		public var simbologia:int;
		public var geometria:Geometria =null;
		public function DrawEsri()
		{
			
		}
		public function activate(_tipoGeometria:int,_simbologia:int):void
		{
			simbologia = _simbologia;
			geometria = new Geometria(_tipoGeometria);
			Tipo = _tipoGeometria;
			DesenhadorEsri.getInstance().map.panEnabled = false;
			if(Tipo == Geometria.MBR)
			{
				DesenhadorEsri.getInstance().map.addEventListener(MouseEvent.MOUSE_DOWN, definirPontoInicial);
				DesenhadorEsri.getInstance().map.addEventListener(MouseEvent.MOUSE_MOVE, desenhoTemporario);
				DesenhadorEsri.getInstance().map.addEventListener(MouseEvent.MOUSE_UP, end);
				DesenhadorEsri.getInstance().map.addEventListener(MouseEvent.CLICK, end);
			}
			else
			{
				DesenhadorEsri.getInstance().map.addEventListener(MouseEvent.CLICK, desenharClickMouse);
				DesenhadorEsri.getInstance().map.addEventListener(MouseEvent.MOUSE_MOVE, desenharMoveMouse);
				DesenhadorEsri.getInstance().map.addEventListener(KeyboardEvent.KEY_UP, acaoTeclado);
			}
		}
		protected function acaoTeclado(event:KeyboardEvent):void
		{	
			switch(event.keyCode) 
			{
				case Keyboard.DELETE:// delete
				case Keyboard.BACKSPACE:
					geometria.removerVertice();
					geometria.desenhar(null,null,simbologia,false);	
					break;
				case Keyboard.ENTER:
					end();	
					geometria.desenhar(null,null,simbologia,false);
					break;
				
			}	
		}
		protected function desenharClickMouse(ev:MouseEvent):void
		{	
			//if(DesenhadorEsri.getInstance().comparaPosicaoMouseComMapa(ev))
		//	{
				var range:Number = DesenhadorEsri.getInstance().range();
				var ar:Vector.<Point> = null;
				
					var pt:Point = DesenhadorEsri.getInstance().converterMouseParaPoint(ev);
					
					geometria.adicionarPonto(pt);
					geometria.desenhar(null,null,simbologia,false);
					if(geometria.tipoGeometria == Geometria.Ponto)
					{
						end();	
						DesenhadorEsri.getInstance().map.removeEventListener(MouseEvent.MOUSE_MOVE, desenharMoveMouse);
					}
			//}
		}
		
		protected function desenharMoveMouse(ev:MouseEvent):void
		{				
			var pt:Point = DesenhadorEsri.getInstance().converterMouseParaPoint(ev);
			geometria.desenharTemporario(pt,true,simbologia,false);
		}
		protected function desenhoTemporario(event:MouseEvent):void
		{
			if(pontoInicial)
			{
				pontoFinal = DesenhadorEsri.getInstance().converterMouseParaPoint(event);
				idGraphic = DesenhadorEsri.getInstance().desenharGeometry(idGraphic,criarMBR(),simbologia,Geometria.Poligono, true);
			}	
		}
		
		public function end(ev:Event=null):void
		{
			
			if(pontoInicial)
				DesenhadorEsri.getInstance().pontoCliqueInicial = pontoInicial;
			
			var geometry:Geometry = new Geometry();
			var _geometria:Geometria = geometria;
			
			if(Tipo == Geometria.MBR)
			{
				if(pontoInicial && pontoFinal)
				{
					geometry = criarMBR();
				}
				else
				{	
					geometry = DesenhadorEsri.getInstance().map.toMapFromStage(DesenhadorEsri.getInstance().mouseX, DesenhadorEsri.getInstance().mouseY);
					
				}
			}
			else
			{
				var arVert:Array = converterVerticesArrayMapPoint(geometria.vertices);
				
				if(arVert && arVert.length>0)
				{
					switch(Tipo)
					{
						case Geometria.Ponto:
								geometry = arVert[0];
							break;
						case Geometria.Linha:
							geometry = new Polyline();
							(geometry as Polyline).paths = [arVert];
							break;
						case Geometria.Poligono:
							var aneis:Array = new Array();
							aneis.push(arVert);
							geometry = new Polygon(aneis);
							break;
					}
				}
			}
				
			DesenhadorEsri.getInstance().map.dispatchEvent(new DrawEsriEvent(DrawEsriEvent.DRAW_END,geometry,_geometria));
			geometria.excluir(true);
			pontoInicial = null;
			pontoFinal = null;
			idGraphic = DesenhadorEsri.getInstance().excluirFeicao(idGraphic);
			
		}
		
		public function converterVerticesArrayMapPoint(vertices:Vector.<Point>):Array
		{
			var geo:Array = new Array();
			if(vertices)
			{
				for each(var ponto:Point in vertices)
				{
					geo.push(new MapPoint(ponto.x,ponto.y));
				}
			}
			return geo;
		}
		
		public function deactivate():void
		{
			DesenhadorEsri.getInstance().map.panEnabled = true;
			DesenhadorEsri.getInstance().map.removeEventListener(MouseEvent.CLICK, desenharClickMouse);
			DesenhadorEsri.getInstance().map.removeEventListener(MouseEvent.MOUSE_MOVE, desenharMoveMouse);
			DesenhadorEsri.getInstance().map.removeEventListener(MouseEvent.DOUBLE_CLICK, end);
			DesenhadorEsri.getInstance().map.removeEventListener(KeyboardEvent.KEY_UP, acaoTeclado);
			DesenhadorEsri.getInstance().map.removeEventListener(MouseEvent.MOUSE_DOWN, definirPontoInicial);
			DesenhadorEsri.getInstance().map.removeEventListener(MouseEvent.MOUSE_MOVE, desenhoTemporario);
			DesenhadorEsri.getInstance().map.removeEventListener(MouseEvent.MOUSE_UP, end);
			DesenhadorEsri.getInstance().map.removeEventListener(MouseEvent.CLICK, end);
			if(geometria)
				geometria.excluir(true);
			idGraphic = DesenhadorEsri.getInstance().excluirFeicao(idGraphic);
		}
		
		protected function definirPontoInicial(event:MouseEvent):void
		{
			pontoInicial =  DesenhadorEsri.getInstance().converterMouseParaPoint(event);
		}
		public function criarMBR():Extent
		{
			return new Extent(pontoInicial.x, pontoInicial.y, pontoFinal.x, pontoFinal.y);
		}
		
	}
}