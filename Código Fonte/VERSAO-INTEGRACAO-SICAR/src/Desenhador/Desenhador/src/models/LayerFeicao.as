package models
{
	import flash.utils.ByteArray;
	
	import mx.collections.IList;
	import mx.controls.Image;

	public class LayerFeicao
	{
		public var Id:int;
		public var IdLista:int;
		public var Nome:String;
		public var Descricao:String;
		public var TipoGeometria:int;
		public var IdLayer:int;
		public var Visivel:Boolean; 
		public var Quantidade:int;
		public var Categoria:int;
		public var ColunaPk:String;
		public var Colunas:Vector.<ColunaLayerFeicao>;
		private var _imagemData:ByteArray;
		private var _imagem:Image;
		private var _selecionado:Boolean;
		public var Feicoes:Vector.<Feicao>;
		public var IsAtivo:Boolean;
		public var IsFinalizada:Boolean;
		public var Selecionavel:Boolean; 
		public var ServicoId:int;
		public var ServicoUrlMxd:String;
		public var ServicoIsPrincipal:Boolean;
		
		public function LayerFeicao()
		{
			
		}

		public function get imagem():Image
		{
			return _imagem;
		}

		public function set imagem(value:Image):void
		{
			_imagem = value;
		}

		public function get imagemData():ByteArray
		{
			return _imagemData;
		}

		public function set imagemData(value:ByteArray):void
		{		
			_imagemData = value;
			if(_imagemData)
			{
				_imagem = new Image();
				_imagem.load(_imagemData);
			}
		}
		public function get selecionado():Boolean
		{
			return _selecionado;
		}
		
		public function set selecionado(value:Boolean):void
		{
			_selecionado = value;
		}
	}
}