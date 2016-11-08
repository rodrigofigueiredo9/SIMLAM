package controllers
{
	import flash.events.Event;
	
	import models.Navegador.CenarioServicoArcGis;

	public class CenarioServicoArcGisControllerEvent  extends Event
	{
		public static const SELECIONOU_CENARIO:String = "CenarioServicoSelecionouControllerEvent";
		public var cenario:CenarioServicoArcGis;
		public function CenarioServicoArcGisControllerEvent(type:String, _cenario:CenarioServicoArcGis, bubbles:Boolean=false, cancelable:Boolean=false)
		{
			cenario = _cenario;
			super(type, bubbles, cancelable); 
		}
	}
}