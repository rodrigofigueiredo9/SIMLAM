package controllers
{
	import flash.events.Event;
	
	import models.Feicao;
	import models.LayerFeicao;

	public class SelecionarControllerEvent extends Event
	{
		public static const PEDIR_CONFIRMACAO_SELECAO:String = "ConfirmarSelecaoControllerEvent";
		public static const RESPOSTA_CONFIRMACAO_SELECAO:String = "RespostaConfirmacaoSelecaoControllerEvent";
		private var _confirmado:Boolean;
		private var _callbackFunction:Function;
		
		public function SelecionarControllerEvent(type:String, confirma:Boolean, callback:Function, bubbles:Boolean=false, cancelable:Boolean=false)
		{
			_callbackFunction = callback;
			confirmado = confirma;
			super(type, bubbles, cancelable);  
		}
		
		public function get confirmado():Boolean
		{
			return _confirmado;
		}

		public function set confirmado(value:Boolean):void
		{
			_confirmado = value;
		}

		public function get callbackFunction():Function
		{
			return _callbackFunction;
		}
		
		public function set callbackFunction(value:Function):void
		{
			_callbackFunction = value;
		}
	}
}