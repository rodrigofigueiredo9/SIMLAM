package controllers
{
	import flash.events.EventDispatcher;
	import flash.events.IEventDispatcher;
	import flash.events.TimerEvent;
	import flash.geom.Point;
	import flash.utils.Timer;
	
	import models.Esri.DesenhadorEsri;
	import models.FeicaoObjeto;
	import models.LayerFeicao;
	import models.Retorno;
	
	import services.FeicaoService;
	
	public class FeicaoController extends EventDispatcher
	{
		private static var instance:FeicaoController;
		
		private var dataservice:FeicaoService; 
		
		public function FeicaoController(enforcer:SingletonEnforcer)
		{
			if (enforcer == null)
				throw new Error("FeicaoController é um Singleton, não é permitido outra instancia. Utilize FeicaoController.getInstance().");
		}
		
		public static function getInstance():FeicaoController {
			if (instance == null) {
				instance = new FeicaoController( new SingletonEnforcer );
				
			} 
			return instance;
		}
		
		public function AtualizarQtdLayerFeicao(): void
		{
			/*var timer:Timer = new Timer(1300, 1);
			
			timer.addEventListener(TimerEvent.TIMER, function(): void {*/
				LayerFeicaoController.getInstance().listarQuantidadeLayerFeicao(DesenhadorEsri.getInstance().idNavegador, DesenhadorEsri.getInstance().idProjeto);
			/*});
			
			timer.start();*/
		}
		
		public function CadastrarFeicao(feicaoObjeto:FeicaoObjeto): void {
			dataservice =  new FeicaoService(AtualizarQtdLayerFeicao);
			dataservice.CadastrarFeicao(feicaoObjeto, function(resposta:Retorno) : void {
				FeicaoController.getInstance().dispatchEvent(new FeicaoControllerEvent(FeicaoControllerEvent.CADASTRAR,resposta));
			});
		}
		public function CadastrarGeometrias(feicoes:Array): void {
			dataservice =  new FeicaoService(AtualizarQtdLayerFeicao);
			dataservice.CadastrarGeometrias(feicoes, function(respostas:Vector.<Retorno>) : void {
				FeicaoController.getInstance().dispatchEvent(new FeicaoControllerEvent(FeicaoControllerEvent.CADASTRARTODOS,null,respostas));
			});
		}
		public function AtualizarGeometria(feicaoObjeto:FeicaoObjeto): void {
			dataservice =  new FeicaoService(AtualizarQtdLayerFeicao);
			dataservice.AtualizarGeometria(feicaoObjeto, function(resposta:Retorno) : void {
				FeicaoController.getInstance().dispatchEvent(new FeicaoControllerEvent(FeicaoControllerEvent.ATUALIZAR_GEOMETRIA,resposta));
			});
		}
		public function AtualizarGeometrias(feicoes:Array): void {
			dataservice =  new FeicaoService(AtualizarQtdLayerFeicao);
			dataservice.AtualizarGeometrias(feicoes, function(respostas:Vector.<Retorno>) : void {
				FeicaoController.getInstance().dispatchEvent(new FeicaoControllerEvent(FeicaoControllerEvent.ATUALIZAR_GEOMETRIA,null,respostas));
			});
		}
		public function AtualizarAtributos(feicaoObjeto:FeicaoObjeto): void {
			dataservice =  new FeicaoService(AtualizarQtdLayerFeicao);
			dataservice.AtualizarAtributos(feicaoObjeto, function(resposta:Retorno) : void {
				FeicaoController.getInstance().dispatchEvent(new FeicaoControllerEvent(FeicaoControllerEvent.ATUALIZAR_ATRIBUTOS,resposta));
			});
		}
		public function ExcluirFeicao(idLayerFeicao:int, objectid:int, idProjeto:int): void {
			dataservice =  new FeicaoService(AtualizarQtdLayerFeicao);
			dataservice.ExcluirFeicao(idLayerFeicao, objectid, idProjeto, function(resposta:Retorno) : void {
				FeicaoController.getInstance().dispatchEvent(new FeicaoControllerEvent(FeicaoControllerEvent.EXCLUIR, resposta));
			});
		}
		public function ExcluirFeicoes(feicoes:Array): void {
			dataservice =  new FeicaoService(AtualizarQtdLayerFeicao);
			dataservice.ExcluirFeicoes(feicoes, function(respostas:Vector.<Retorno>) : void {
				FeicaoController.getInstance().dispatchEvent(new FeicaoControllerEvent(FeicaoControllerEvent.EXCLUIR,null,respostas));
			});
		}
		public function ImportarFeicoes(idNavegador:int, idProjeto:int, isFinalizadas:Boolean): void {
			dataservice =  new FeicaoService(AtualizarQtdLayerFeicao);
			dataservice.ImportarFeicoes(idNavegador, idProjeto, isFinalizadas, function(resposta:Retorno) : void {
				FeicaoController.getInstance().dispatchEvent(new FeicaoControllerEvent(FeicaoControllerEvent.IMPORTAR_FEICOES,resposta));
			});
		}
	}
}

class SingletonEnforcer {
}