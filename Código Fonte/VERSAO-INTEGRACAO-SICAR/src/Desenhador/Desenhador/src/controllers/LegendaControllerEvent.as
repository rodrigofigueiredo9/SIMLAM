package controllers
{
	import flash.events.Event;
	
	public class LegendaControllerEvent extends Event
	{
		public static const LAYERS_SERVICOS_ATUALIZAR:String = "LayersServicosAtualizarLegendaControllerEvent";
		public var idsLayersServicos:Array;
		public function LegendaControllerEvent(type:String, _idsLayersServicos:Array, bubbles:Boolean=false, cancelable:Boolean=false)
		{
			idsLayersServicos = _idsLayersServicos;
			super(type, bubbles, cancelable); 
		}
	}
}