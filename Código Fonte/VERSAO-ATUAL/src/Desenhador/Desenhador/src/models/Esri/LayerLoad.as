package models.Esri
{
	public class LayerLoad
	{
		public function LayerLoad()
		{
		}
		
		public var Type:String;
		public var Nome:String;
		public var Url:String;
		public var Rank:String;
		public var Filtros:Array;
		public var Id:String;
		public var MaxScale:Number;
		public var MinScale:Number;
		public var ProxyURL:String=null;
		public var Token:String=null;
		public var Tentativas:Number=0;
		public var IsCarregado:Boolean;
		public var Posicao:Number=0;
	}
}