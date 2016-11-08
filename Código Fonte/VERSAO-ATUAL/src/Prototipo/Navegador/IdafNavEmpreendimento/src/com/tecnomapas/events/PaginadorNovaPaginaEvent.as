package com.tecnomapas.events 
{
	import flash.events.Event;
	
	public class PaginadorNovaPaginaEvent extends Event
	{
		public static const NOVA_PAGINA:String = "novaPagina";
		
		public var intervaloInicial:int;
		public var intervaloFinal:int;
		
		public function PaginadorNovaPaginaEvent(type:String, intervaloInicial:int, intervaloFinal:int, bubbles:Boolean = false, cancelable:Boolean = false)
		{
			this.intervaloInicial = intervaloInicial;
			this.intervaloFinal = intervaloFinal;
			
			super(type, bubbles, cancelable);
		}
	}
}