package models
{
	public class FeicaoAreaAbrangencia
	{
		public function FeicaoAreaAbrangencia()
		{
			
		}
		
		private var _objectId:int;
		private var _idProjeto:int;
		private var _minX:Number;
		private var _minY:Number;
		private var _maxX:Number;
		private var _maxY:Number;
		
		public function get ObjectId():int
		{
			return _objectId;
		}
		
		public function set ObjectId(value:int):void
		{
			_objectId = value;
		}
		
		public function get IdProjeto():int
		{
			return _idProjeto;
		}
		
		public function set IdProjeto(value:int):void
		{
			_idProjeto = value;
		}
		public function get MaxY():Number
		{
			return _maxY;
		}
		
		public function set MaxY(value:Number):void
		{
			_maxY = value;
		}
		
		public function get MaxX():Number
		{
			return _maxX;
		}
		
		public function set MaxX(value:Number):void
		{
			_maxX = value;
		}
		
		public function get MinY():Number
		{
			return _minY;
		}
		
		public function set MinY(value:Number):void
		{
			_minY = value;
		}
		
		public function get MinX():Number
		{
			return _minX;
		}
		
		public function set MinX(value:Number):void
		{
			_minX = value;
		}
	}
}