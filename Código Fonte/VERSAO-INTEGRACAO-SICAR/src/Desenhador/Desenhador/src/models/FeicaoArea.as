package models
{
	import com.gmaps.geom.GeomArea;
	import com.gmaps.geom.GeomPoint;
	
	import flash.geom.Point;
	
	import mx.controls.Alert;

	public class FeicaoArea
	{
		public function FeicaoArea()
		{
		}
		
		public static var TOLERANCIA:Number = 0.01001;
		private var _geomArea:GeomArea;
		private var _area:Number;
		private var _verticesArray:Array;
		private var _vertices:Vector.<Point>

		public function get area():Number
		{
			if(geomArea)
				return geomArea.getArea();
			else
				return 0;
		}

		public function get geomArea():GeomArea
		{
			return _geomArea;
		}

		public function set geomArea(value:GeomArea):void
		{
			_geomArea = value;
		}

		public function get verticesArray():Array
		{
			return _verticesArray;
		}

		public function set verticesArray(value:Array):void
		{
			_verticesArray = value;
			if(_verticesArray)
			{
				_geomArea = new GeomArea(null,_verticesArray,null,null);
				_vertices = new Vector.<Point>();
				for(var i:int=0; i < _verticesArray.length; i++)
				{
					var ponto:Point = _verticesArray[i];
					var proxPt:Point;
					if(i+1 ==_verticesArray.length)
						proxPt = _verticesArray[0];
					else
						proxPt = _verticesArray[i+1];
					
					var geoPt:GeomPoint = new GeomPoint(i,ponto);
					if(geoPt && proxPt)
					{
					 	if(geoPt.distanceFromPoint(proxPt) <=TOLERANCIA)
						{
							continue;
						}
					}
					
					_vertices.push(ponto);
				}
				
				
				/*for each(var ponto:Point in _verticesArray)
				{
					_vertices.push(ponto);
				}*/
			}
		}

		public function set vertices(value:Vector.<Point>):void
		{
			_vertices = value;
		}
		public function get vertices():Vector.<Point>
		{
			return _vertices;
		}	
	}
}