package models
{
	import mx.collections.IList;

	public class ColunaLayerFeicao
	{		
		public static const Booleano:int=1;
		public static const Data:int=2;
		public static const Numero:int=3;
		public static const ListaDeValores:int=4;
		public static const Texto:int=5;
		
		public var Coluna:String;
		public var Alias:String;
		public var Tipo:int;
		public var Tamanho:Number;
		public var Referencia:String;
		public var Itens:IList;
		public var IsObrigatorio:Boolean;
		public var IsVisivel:Boolean;
		public var IsEditavel:Boolean;
		public var Valor:String;
		public var IdLista:int;
		public var Operacao:int;
		public var ValorCondicao:String;
		public var ColunaObrigada:String;
				
		public function ColunaLayerFeicao()
		{
			Valor = "";	
		}
	}
}