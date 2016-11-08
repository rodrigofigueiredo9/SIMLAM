package models
{
	import com.esri.ags.geometry.Geometry;
	
	import models.Esri.DesenhadorEsri;

	public class Feicao 
	{
		public function Feicao(_tipo:int)
		{
		//	atributos = new Vector.<AtributoFeicao>();
			tipoGeometria = _tipo;
			
		}
		private var _layerFeicao:LayerFeicao; 
		private var _objectId:int;
		private var _projetoId:int;
		private var _geometry:Geometry;
		//private var _atributos:Vector.<AtributoFeicao>;
		private var _tipoGeometria:int;
		private var _geometria:Geometria;
		public var IdLista:int;
		public var Selecionado:Boolean;
		
		public function get tipoGeometria():int
		{
			return _tipoGeometria;
		}

		public function set tipoGeometria(value:int):void
		{
			_tipoGeometria = value;
		}

		/*public function get atributos():Vector.<AtributoFeicao>
		{
			return _atributos;
		}

		public function set atributos(value:Vector.<AtributoFeicao>):void
		{
			_atributos = value;
		}*/

		public function get geometry():Geometry
		{
			return _geometry;
		}

		public function set geometry(value:Geometry):void
		{
			_geometry = value;
			_geometria = null;
		}
		
		public function get geometria():Geometria
		{
			if(!_geometria && geometry&& layerFeicao)
			 	_geometria =	DesenhadorEsri.getInstance().converterGeometryParaGeometria(_geometry, layerFeicao.TipoGeometria);
				
				return _geometria;
		}
		public function set geometria(value:Geometria):void
		{
			_geometria = value;
		}

		public function get objectId():int
		{
			return _objectId;
		}

		public function set objectId(value:int):void
		{
			_objectId = value;
		}
		
		public function get projetoId():int
		{
			return _projetoId;
		}
		
		public function set projetoId(value:int):void
		{
			_projetoId = value;
		}

		public function get layerFeicao():LayerFeicao
		{
			return _layerFeicao;
		}

		public function set layerFeicao(value:LayerFeicao):void
		{
			_layerFeicao = value;
		}
	}
}