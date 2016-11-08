package com.idaf.controllers
{
	import flash.events.Event;
	
	public class DataControllerEvent extends Event
	{
		public static const EMPREENDIMENTOS_ATUALIZADOS: String = "AtualizadoEmpreendimentoDataControllerEvent";
		public static const EMPREENDIMENTOS_IDENTIFICADOS: String = "IdentificadosEmpreendimentoDataControllerEvent";
		public static const LISTAS_ATUALIZADAS: String = "AtualizadoListasDataControllerEvent";
		
		private var _result:Array;
		
		public function DataControllerEvent(type:String, result: Array, bubbles:Boolean=false, cancelable:Boolean=false)
		{
			this._result = result;
			
			super(type, bubbles, cancelable);
		}

		public function get result():Array
		{
			return _result;
		}

		public function set result(value:Array):void
		{
			_result = value;
		}

	}
}