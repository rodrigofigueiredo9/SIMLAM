package controllers
{
	import flash.events.Event;
	
	import models.Feicao;
	import models.LayerFeicao;
	
	import mx.controls.Alert;

	public class ItemFeicaoControllerEvent extends Event
	{
		public static const SELECIONOU_ITEM:String = "selecionou_item";
		public static const ADICIONOU_ITEM:String = "adicionou_item";
		public static const RECARREGAR_LISTA:String = "recarregar_lista";
		
		private var _feicao:Feicao;
		private var _itemSelecionado:Boolean;
		private var _ctrlIsLigado:Boolean;
		public function ItemFeicaoControllerEvent(type:String, feicaoRecebida:Feicao, itemIsSelecionado:Boolean,  ctrlLigado:Boolean, bubbles:Boolean=false, cancelable:Boolean=false)
		{
			_ctrlIsLigado = ctrlLigado; 
			if(feicaoRecebida)
			{
				this._feicao = feicaoRecebida;
				this._itemSelecionado = !itemIsSelecionado;
			}
			
			super(type, bubbles, cancelable); 
		}
		
		public function get ctrlIsLigado():Boolean
		{
			return _ctrlIsLigado;
		}

		public function set ctrlIsLigado(value:Boolean):void
		{
			_ctrlIsLigado = value;
		}

		public function get feicao():Feicao
		{
			return _feicao;
		}
		
		public function set feicao(value:Feicao):void
		{
			_feicao = value;
		}
		public function get itemIsSelecionado():Boolean
		{
			return _itemSelecionado;
		}
		
		public function set itemIsSelecionado(value:Boolean):void
		{
			_itemSelecionado = value;
		}
	}
}