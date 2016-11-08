package models
{
	import controllers.FerramentaGeometriaControllerEvent;
	import controllers.IdentifyControllerEvent;
	
	import models.Esri.DesenhadorEsri;

	public class LayerFeicaoSelecionada
	{
		private static var instance:LayerFeicaoSelecionada;
		
		public var idProjeto:int;
		public var isDesenhando:Boolean;
		public var isDesativada:Boolean;
		
		public function LayerFeicaoSelecionada(enforcer:SingletonEnforcer)
		{
			if (enforcer == null)
				throw new Error("LayerFeicaoSelecionada é um Singleton, não é permitido outra instancia. Utilize LayerFeicaoSelecionada.getInstance().");
		}
		
		public static function getInstance():LayerFeicaoSelecionada {
			if (instance == null) {
				instance = new LayerFeicaoSelecionada( new SingletonEnforcer );
			}
			return instance;
		} 
		public function get layerFeicao():LayerFeicao
		{
			return _layerFeicao;
		}
		
		public function set layerFeicao(value:LayerFeicao):void
		{
			if(!isDesativada)
			{
			_layerFeicao = value;
			DesenhadorEsri.getInstance().map.dispatchEvent(new FerramentaGeometriaControllerEvent(FerramentaGeometriaControllerEvent.ATIVAR_DESATIVAR_BOTOES,null,null,true,true));
			}		
		}	
		private var _layerFeicao:LayerFeicao; 
	}
}
class SingletonEnforcer {
}