package controllers
{
	import flash.events.Event;
	
	import models.AtributoFeicao;

	public class AtributoFeicaoControllerEvent extends Event
	{
		public static const ATUALIZAR_ATRIBUTO:String = "atualizar_atributo";
		
		public var atributo:AtributoFeicao;
	
		public function AtributoFeicaoControllerEvent(type:String, _atributo:AtributoFeicao, bubbles:Boolean=false, cancelable:Boolean=false)
		{
			atributo = _atributo;
			super(type, bubbles, cancelable); 
		}
	}
}