package controllers
{
	import flash.events.Event;
	
	import models.Retorno;

	public class FeicaoControllerEvent extends Event
	{
		public static const ATUALIZAR_GEOMETRIA:String = "AtualizarGeometriaFeicaoControllerEvent";
		public static const CADASTRAR:String = "CadastrarFeicaoControllerEvent";
		public static const CADASTRARTODOS:String = "CadastrarTodosFeicaoControllerEvent";
		public static const EXCLUIR:String = "ExcluirFeicaoControllerEvent";
		public static const ATUALIZAR_ATRIBUTOS:String = "AtualizarAtributosFeicaoControllerEvent";
		public static const IMPORTAR_FEICOES:String = "ImportarFeicoesFeicaoControllerEvent";
		private var _resposta:Retorno;
		private var _respostas:Vector.<Retorno>;
	
		public function FeicaoControllerEvent(type:String, resposta:Retorno, respostas:Vector.<Retorno>=null, bubbles:Boolean=false, cancelable:Boolean=false)
		{
			_resposta = resposta; 
			_respostas = respostas;
			super(type, bubbles, cancelable); 
		}

		public function get respostas():Vector.<Retorno>
		{
			return _respostas;
		}

		public function set respostas(value:Vector.<Retorno>):void
		{
			_respostas = value;
		}

		public function get resposta():Retorno
		{
			return _resposta;
		}

		public function set resposta(value:Retorno):void
		{
			_resposta = value;
		}
	}
}