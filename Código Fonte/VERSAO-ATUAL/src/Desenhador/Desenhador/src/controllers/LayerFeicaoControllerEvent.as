package controllers
{
	import flash.events.Event;
	
	import models.CategoriaLayerFeicao;

	public class LayerFeicaoControllerEvent extends Event
	{
		public static const ATIVAR_DESATIVAR_CATEGORIA:String = "AtivarDesativarCategoriaLayerFeicaoControllerEvent";
		public static const SELECIONOU_LAYER:String = "selecionou_layer";
		public static const LISTAR_CATEGORIAS:String = "ListarCategoriasLayerFeicaoControllerEvent";
		public static const LISTAR_QUANTIDADE:String = "ListarQuantidadeLayerFeicaoControllerEvent";
		private var _categorias:Vector.<CategoriaLayerFeicao>;
		public function LayerFeicaoControllerEvent(type:String, categorias:Vector.<CategoriaLayerFeicao>, bubbles:Boolean=false, cancelable:Boolean=false)
		{
			if(categorias)
				this._categorias = categorias;
			super(type, bubbles, cancelable); 
		}
		
		public function get categorias():Vector.<CategoriaLayerFeicao>
		{
			return _categorias;
		}
		
		public function set categorias(value:Vector.<CategoriaLayerFeicao>):void
		{
			_categorias = value;
		}
	}
}