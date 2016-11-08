package services
{

	import flash.events.Event;
	import flash.events.IOErrorEvent;
	import flash.net.URLLoader;
	import flash.net.URLRequest;
	import flash.net.URLRequestMethod;
	import flash.net.URLVariables;
	
	import models.Navegador.CenarioServicoArcGis;
	import models.Navegador.Navegador;
	import models.Navegador.ServicoArcGis;
	
	import mx.controls.Alert;
		 
	public class NavegadorService
	{
		private var _serviceURL:String;
		private var _navegador:Navegador;
		private var completeCallback:Function;
		
		[Embed(source="../assets/erro.png")]
		private var ErroImg:Class;
		
		public function NavegadorService()
		{	
			this._serviceURL = ConfiguracaoService.getInstance().serviceUrl +"/Navegador";
		}
		
		public function get getServiceURL():String
		{
			if(_serviceURL.length>4&&_serviceURL.substr(0,4).toLowerCase() == "null")
				getServiceURL = ConfiguracaoService.getInstance().serviceUrl +"/Navegador";
			return _serviceURL;
		}
		
		public function set getServiceURL(value:String):void
		{
			_serviceURL = value;
		}
		
		public function BuscarNavegador(idNavegador:int, idProjeto:int, callback:Function) : void {
			completeCallback = callback;
			
			var variables:URLVariables = new URLVariables();
			variables.idNavegador = idNavegador;
			variables.idProjeto = idProjeto;
			
			var request: URLRequest = new URLRequest(this._serviceURL+"/Buscar");
			request.method = URLRequestMethod.POST;
			request.data = variables;
			
			var loader:URLLoader = new URLLoader(request);
			loader.addEventListener(Event.COMPLETE, BuscarNavegadorComplete);
			loader.addEventListener(IOErrorEvent.IO_ERROR, ErrorFunction);
		}
				
		private function BuscarNavegadorComplete(event:Event) : void {
			var obj:Object = JSON.parse(event.target.data);
			setNavegador(obj);	
			completeCallback(_navegador);
		}
		
		public function setNavegador(value:Object):void
		{
			_navegador = new Navegador();
			_navegador.Id =  value.Id;
			_navegador.Nome = value.Nome;
			_navegador.Servicos = new Vector.<ServicoArcGis>();
			_navegador.Cenarios = new Vector.<CenarioServicoArcGis>();
			var ar:Array = value.Servicos as Array;
			if(ar)
			{
				var servico:ServicoArcGis;
				for(var i:int =0; i< ar.length; i++)
				{
					servico = new ServicoArcGis();
					servico.Nome = ar[i].Nome;
					servico.Url = ar[i].Url;
					servico.Id = ar[i].Id;
					servico.IsPrincipal = ar[i].IsPrincipal;
					servico.IsCacheado = ar[i].IsCacheado;
					servico.Identificar = ar[i].Identificar;
					servico.GeraLegenda = ar[i].GeraLegenda;
					servico.UltimoIdLayer = ar[i].UltimoIdLayer;
					_navegador.Servicos.push(servico);
				}
			}
			ar = value.Cenarios as Array;
			if(ar)
			{
				var cenario:CenarioServicoArcGis;
				for(var i:int =0; i< ar.length; i++)
				{
					cenario = new CenarioServicoArcGis();
					cenario.Id  = ar[i].Id;
					cenario.Indice = ar[i].Indice;
					cenario.IsPrincipal = ar[i].IsPrincipal;
					cenario.Nome  = ar[i].Nome;
					cenario.Servicos  = ar[i].Servicos;
					cenario.ExibirLogotipo = ar[i].ExibirLogotipo;
					_navegador.Cenarios.push(cenario);
				}
			}
			
			ar = value.Filtros as Array;
			if(ar)
			{
				_navegador.Filtros = ar;
			}
			
			ar = value.ProjetosAssociados as Array;
			if(ar)
			{
				_navegador.ProjetosAssociados = ar;
			}
		}
		
		protected function ErrorFunction(ev:IOErrorEvent):void
		{
			if(ev) 
			{
				Alert.show(ev.text +" \n\nTente novamente executar a operação, caso não resolva consulte o administrador.","Ocorreu o seguinte erro",4,null,null,ErroImg);
			}
		}
	}
}