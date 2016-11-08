package controllers
{
	import flash.events.Event;

	public class FerramentaGeometriaControllerEvent  extends Event
	{
		public static const DESLIGAR:String = "DesligarFerramentaGeometriaControllerEvent";
		public static const DESLIGAR_OUTRAS_FERRAMENTAS:String = "DesligarOutrasFerramentaGeometriaControllerEvent";
		public static const PAUSAR_FERRAMENTA:String = "DesligarFerramentaGeometriaControllerEvent";
		public static const LIGAR_DESLIGAR_PAN:String = "LigarDesligarPanFerramentaGeometriaControllerEvent";
		public static const FINALIZAR_GEOMETRIA:String = "finalizar_geometria";
		public static const HABILITAR_SNAP:String = "HabilitarSnapFerramentaGeometriaControllerEvent";
		public static const SNAP_ISATIVADO:String = "SnapIsAtivadoFerramentaGeometriaControllerEvent";
		public static const DESATIVAR_TODAS_FERRAMENTAS:String = "DesativarTodasFerramentaGeometriaControllerEvent";
		public static const ATIVAR_DESATIVAR_BOTOES:String = "AtivarDesativarBotoesFerramentaGeometriaControllerEvent";
		public var completeCallback:Function;
		public var targetObj:Object;
		public var ativar:Boolean;
		public function FerramentaGeometriaControllerEvent(type:String, callback:Function, _target:Object, _ativar:Boolean=true, bubbles:Boolean=false, cancelable:Boolean=false)
		{
			completeCallback = callback;
			targetObj = _target;
			ativar = _ativar;
			super(type, bubbles, cancelable); 
		}
	}
}