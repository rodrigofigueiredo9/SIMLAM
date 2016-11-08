package models
{
	public class CategoriaQuadroDeArea
	{
		public var Id:int;
		public var Nome:String;
		public var Itens:Vector.<ItemQuadroDeArea>;
		public var IsAtivo:Boolean;
		public function CategoriaQuadroDeArea()
		{
			IsAtivo = true;
		}
	}
}