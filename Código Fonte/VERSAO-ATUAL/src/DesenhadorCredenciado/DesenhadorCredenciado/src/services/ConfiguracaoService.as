package services
{
	public class ConfiguracaoService
	{
 		private var _serviceUrl:String;
 			
		private static var instance:ConfiguracaoService;
		
		public function ConfiguracaoService(enforcer:SingletonEnforcer)
		{
			if (enforcer == null)
				throw new Error("ConfiguracaoService é um Singleton, não é permitido outra instancia. Utilize ConfiguracaoService.getInstance().");
			
		}
	
		public function get serviceUrl():String
		{
			return _serviceUrl;
		}
		
		public function set serviceUrl(value:String):void
		{
			_serviceUrl = value;
		}
		
		public static function getInstance():ConfiguracaoService {
			if (instance == null) {
				instance = new ConfiguracaoService( new SingletonEnforcer );
				
			}
			return instance;
		}
	}
}
class SingletonEnforcer {
}