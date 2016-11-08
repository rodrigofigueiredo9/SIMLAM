package controllers
{
	import flash.events.EventDispatcher;
	import flash.events.IEventDispatcher;
	
	import models.Navegador.Navegador;
	
	import services.NavegadorService;
	
	public class NavegadorController extends EventDispatcher
	{
		private static var instance:NavegadorController;
		
		private var dataservice:NavegadorService = new NavegadorService(); 
			
		public function NavegadorController(enforcer:SingletonEnforcer)
		{
			if (enforcer == null)
				throw new Error("NavegadorController é um Singleton, não é permitido outra instancia. Utilize NavegadorController.getInstance().");
		}

		public static function getInstance():NavegadorController {
			if (instance == null) {
				instance = new NavegadorController( new SingletonEnforcer );
				
			}
			return instance;
		}
		
		public function buscarNavegador(idNavegador:int, idProjeto:int): void {
			dataservice.BuscarNavegador(idNavegador,idProjeto, function(navegador:Navegador) : void {
			NavegadorController.getInstance().dispatchEvent(new NavegadorControllerEvent(NavegadorControllerEvent.NAVEGADOR_BUSCAR,navegador));
			});
		}
	}
}

class SingletonEnforcer {
}