package com.tecnomapas.events 
{
	import flash.events.Event;
	
	import com.tecnomapas.auxiliares.ButtonPaginador;

	public class PaginadorExibirPaginaEvent extends Event 
	{
		public static const EXIBIR_PAGINA:String = "exibirPagina";
		
		public var botaoPagina:ButtonPaginador;
		
		public function PaginadorExibirPaginaEvent(type:String, buttonPage:ButtonPaginador, bubbles:Boolean = false, cancelable:Boolean = false)
		{
			this.botaoPagina = buttonPage;
			super(type, bubbles, cancelable);
		}

	}
}