package models
{
	import com.gmaps.geom.Mbr;
	
	import controllers.FerramentaGeometriaControllerEvent;
	import controllers.IdentifyControllerEvent;
	
	import flash.geom.Point;
	
	import models.Esri.DesenhadorEsri;

	public class FeicaoSelecionada
	{
		private static var instance:FeicaoSelecionada;
		
		public var idProjeto:int;
		private var _mbr:Mbr;
		private var _layerFeicao:LayerFeicao; 
		public var objectId:int;
		private var _geometria:Geometria;
		private var _isSelecionavel:Boolean;
		
		public function FeicaoSelecionada(enforcer:SingletonEnforcer)
		{
			if (enforcer == null)
				throw new Error("FeicaoSelecionada é um Singleton, não é permitido outra instancia. Utilize FeicaoSelecionada.getInstance().");
		}

		public function get isSelecionavel():Boolean
		{
			return _isSelecionavel;
		}

		public function set isSelecionavel(value:Boolean):void
		{
			_isSelecionavel = value;
		}

		public static function getInstance():FeicaoSelecionada {
			if (instance == null) {
				instance = new FeicaoSelecionada( new SingletonEnforcer );
			}
			return instance;
		} 
		public function get layerFeicao():LayerFeicao
		{
			return _layerFeicao;
		}
		
		public function set layerFeicao(value:LayerFeicao):void
		{
			_layerFeicao = value;
			if(_layerFeicao && !_geometria)
			{
			
					_geometria = new Geometria(_layerFeicao.TipoGeometria);
				
			}
			DesenhadorEsri.getInstance().map.dispatchEvent(new FerramentaGeometriaControllerEvent(FerramentaGeometriaControllerEvent.ATIVAR_DESATIVAR_BOTOES,null,null,true,true));	
		}
		public function get mbr():Mbr
		{
			if(!_mbr)
				_mbr = calcularMbr();
			
			return _mbr;
		}
		
		public function limpar(excluirVertices:Boolean):void
		{
			if(geometria)
			{
				geometria.excluir(excluirVertices);
			}
			if(excluirVertices)
			{
				geometria =null;
				_layerFeicao = null;
				objectId = 0;
				_mbr = null;
			}
			DesenhadorEsri.getInstance().map.dispatchEvent(new FerramentaGeometriaControllerEvent(FerramentaGeometriaControllerEvent.ATIVAR_DESATIVAR_BOTOES,null,null,true,true));
			
		}
		public function get geometria():Geometria
		{
			return _geometria;
		}
		
		public function set geometria(value:Geometria):void
		{
			_geometria = value;
			DesenhadorEsri.getInstance().map.dispatchEvent(new FerramentaGeometriaControllerEvent(FerramentaGeometriaControllerEvent.ATIVAR_DESATIVAR_BOTOES,null,null,true,true));
			_mbr = null;
		}
		public function reiniciarGeometria():void
		{
			if(_geometria)
			{
				_geometria.excluir(false);
				_geometria.vertices = new Vector.<Point>();
				
				for each(var ponto:Point in _geometria.verticesOriginal)
					_geometria.vertices.push(ponto);
								
				_geometria.aneis = new Vector.<Vector.<Point>>();
				
				for each(var anel:Vector.<Point> in _geometria.aneisOriginal)
				{
					var novoAnel:Vector.<Point> = new Vector.<Point>();					
					
					for each(var ponto:Point in anel)
					{
						novoAnel.push(ponto);
					}
					_geometria.aneis.push(novoAnel);
				}
				/*
				if(_geometria.tipoGeometria == Geometria.Poligono && _geometria.vertices 
					&& _geometria.vertices.length>0)
				{
					_geometria.aneis.splice(0,0,_geometria.vertices);
					_geometria.vertices = new Vector.<Point>();
				}*/
			}
		}
		public function calcularMbr():Mbr
		{
			var minx:Number = 9999999999999;
			var miny:Number = 9999999999999;
			var maxx:Number =  0;
			var maxy:Number =  0;
			
			if(geometria)
			{
				var vertices:Vector.<Point>;
				if(layerFeicao.TipoGeometria == Geometria.Poligono && geometria.aneis && geometria.aneis.length>0)
					vertices = geometria.aneis[0];
				else
					vertices = geometria.vertices;
				
				if(vertices)
				{
					for each(var ponto:Point in vertices)
					{
						if(ponto.x < minx)
							minx = ponto.x;
						if(ponto.x >maxx)
							maxx = ponto.x;
						if(ponto.y<miny)
							miny = ponto.y;
						if(ponto.y > maxy)
							maxy = ponto.y;
					}
				}
			}
			
			if(minx != 9999999999999 &&  miny != 9999999999999 && maxx !=  0 && maxy !=  0)
				return new Mbr(minx, miny, maxx, maxy);
			else
				return null;
		}
	}
}
class SingletonEnforcer {
}