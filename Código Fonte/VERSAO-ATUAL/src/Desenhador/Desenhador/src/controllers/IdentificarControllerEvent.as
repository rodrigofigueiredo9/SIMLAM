package controllers
{
	import flash.events.Event;
	
	import models.Item;
	
	public class IdentificarControllerEvent extends Event
	{
		public static const IDENTIFICAR_RESULT:String = "IdentificarResultIdentificarControllerEvent";
		public static const LAYERS_SERVICOS_ATUALIZAR:String = "LayersServicosAtualizarIdentificarControllerEvent";
		public var servicosIdentificam:Vector.<Item>;
		
		public function IdentificarControllerEvent(type:String, _servicosIdentificam:Vector.<Item>,  bubbles:Boolean=false, cancelable:Boolean=false)
		{
			servicosIdentificam = _servicosIdentificam;
			
			super(type, bubbles, cancelable);  
		}		
	}
} 