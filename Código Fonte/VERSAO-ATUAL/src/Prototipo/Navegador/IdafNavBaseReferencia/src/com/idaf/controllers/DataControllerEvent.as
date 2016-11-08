package com.idaf.controllers
{
	import flash.events.Event;
	
	public class DataControllerEvent extends Event
	{
		public static const MUNICIPIOS_ATUALIZADOS: String = "AtualizadosDataControllerEvent";
		public static const LOTES_ATUALIZADO: String = "AtualizadoLotesDataControllerEvent";
		
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