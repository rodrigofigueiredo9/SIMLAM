package controllers
{
	import flash.events.Event;
	
	import models.Navegador.Navegador;
	
	public class NavegadorControllerEvent extends Event
	{
		public static const NAVEGADOR_BUSCAR: String = "NavegadorBuscarNavegadorControllerEvent";
		
		private var _navegador:Navegador;
				
		public function NavegadorControllerEvent(type:String, navegador:Navegador, bubbles:Boolean=false, cancelable:Boolean=false)
		{
			this._navegador = navegador; // as Navegador;
			
			super(type, bubbles, cancelable); 
		}
				
		
		public function get navegador():Navegador
		{
			return _navegador;
		}

		public function set navegador(value:Navegador):void
		{
			_navegador = value;
		}

	}
}