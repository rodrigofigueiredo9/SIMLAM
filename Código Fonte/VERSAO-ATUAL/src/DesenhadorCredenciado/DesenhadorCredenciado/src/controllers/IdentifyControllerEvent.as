package controllers
{
	import flash.events.Event;
	import flash.geom.Point;
	
	import models.Feicao;

	public class IdentifyControllerEvent extends Event
	{
		public static const IDENTIFICAR_RESULT:String = "IdentificarControllerEvent";
		public static const SELECIONOU_DESELECIONOU:String = "SelecionouControllerEvent";
		private var _listaFeicoes:Vector.<Feicao>;
		private var _ptCliqueInicial:Point;
		public function IdentifyControllerEvent(type:String, _feicoes:Vector.<Feicao>, _ptInicial:Point, bubbles:Boolean=false, cancelable:Boolean=false)
		{
			ptCliqueInicial = _ptInicial;
			listaFeicoes = _feicoes; 
			super(type, bubbles, cancelable); 
		}
		
		public function get listaFeicoes():Vector.<Feicao>
		{
			return _listaFeicoes;
		}
		
		public function set listaFeicoes(value:Vector.<Feicao>):void
		{
			_listaFeicoes = value;
		}
		
		public function get ptCliqueInicial():Point
		{
			return _ptCliqueInicial;
		}
		
		public function set ptCliqueInicial(value:Point):void
		{
			_ptCliqueInicial = value
		}
		
	}
}