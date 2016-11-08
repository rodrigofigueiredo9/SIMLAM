package services
{	
	import flash.events.Event;
	import flash.events.IOErrorEvent;
	import flash.net.URLLoader;
	import flash.net.URLRequest;
	import flash.net.URLRequestMethod;
	import flash.net.URLVariables;
	
	import models.Esri.DesenhadorEsri;
	import models.FeicaoObjeto;
	import models.Retorno;
	
	import mx.controls.Alert;
	import mx.managers.CursorManager;

	public class FeicaoService
	{
		private var _serviceURL:String;
		private var completeCallback:Function;
		private var _projetoService:ProjetoService;
		
		private var _atualizarQtdLayerFeicaoCallback:Function;
		
		[Embed(source="../assets/erro.png")]
		private var ErroImg:Class;
		
		public function FeicaoService(qtdGeometriasCallback:Function)
		{	
			this._serviceURL = ConfiguracaoService.getInstance().serviceUrl +"/Feicao";
			this._atualizarQtdLayerFeicaoCallback = qtdGeometriasCallback;
		}
		
		public function get getServiceURL():String
		{
			if(_serviceURL.length>4&&_serviceURL.substr(0,4).toLowerCase() == "null")
				getServiceURL = ConfiguracaoService.getInstance().serviceUrl +"/Feicao";
			return _serviceURL;
		}
		
		public function set getServiceURL(value:String):void
		{
			_serviceURL = value;
		}
		
		public function CadastrarFeicao(feicao:FeicaoObjeto, callback:Function) : void {
			
			completeCallback = callback;
			if(!feicao)
			{
				Alert.show('Feição não pode ser nulo.');
				return;
			}
			if(feicao.IdLayerFeicao <=0)
			{
				Alert.show('Id Layer Feição deve ser maior que zero.');
				return;
			}
			
			var request: URLRequest = new URLRequest(this._serviceURL+"/Cadastrar");
			request.contentType = 'application/json';
			request.method = URLRequestMethod.POST;		
			request.data =  JSON.stringify(feicao);
			
			var loader:URLLoader = new URLLoader(request);
			loader.addEventListener(Event.COMPLETE, FeicaoComplete);
			loader.addEventListener(IOErrorEvent.IO_ERROR, ErrorFunction);
		}
				
		public function CadastrarGeometrias(feicoes:Array, callback:Function) : void {
			completeCallback = callback;
			if(!feicoes)
			{
				Alert.show('Feições não pode ser nulo.');
				return;
			}
			
			var request: URLRequest = new URLRequest(this._serviceURL+"/CadastrarGeometrias");
			request.contentType = 'application/json';
			request.method = URLRequestMethod.POST;		
			request.data =  JSON.stringify(feicoes);
			
			var loader:URLLoader = new URLLoader(request);
			loader.addEventListener(Event.COMPLETE, FeicoesComplete);
			loader.addEventListener(IOErrorEvent.IO_ERROR, ErrorFunction);
		}
		
		public function AtualizarGeometria(feicao:FeicaoObjeto, callback:Function) : void {
			completeCallback = callback;
			if(!feicao)
			{
				Alert.show('Feição não pode ser nulo.');
				return;
			}
			if(feicao.IdLayerFeicao<=0)
			{
				Alert.show('Id de Layer Feição deve ser maior que 0.');
				return;
			}
			if(feicao.ObjectId<=0)
			{
				Alert.show('Id deve ser maior que 0.');
				return;
			}
			
			var request: URLRequest = new URLRequest(this._serviceURL+"/AtualizarGeometria");
			request.contentType = 'application/json';
			request.method = URLRequestMethod.POST;		
			request.data = JSON.stringify(feicao);
			
			var loader:URLLoader = new URLLoader(request);
			loader.addEventListener(Event.COMPLETE, FeicaoComplete);
			loader.addEventListener(IOErrorEvent.IO_ERROR, ErrorFunction);
		}
		
		public function AtualizarGeometrias(feicoes:Array, callback:Function) : void {
			completeCallback = callback;
			if(!feicoes)
			{
				Alert.show('Feições não pode ser nulo.');
				return;
			}
						
			var request: URLRequest = new URLRequest(this._serviceURL+"/AtualizarGeometrias");
			request.contentType = 'application/json';
			request.method = URLRequestMethod.POST;		
			request.data = JSON.stringify(feicoes);
			
			var loader:URLLoader = new URLLoader(request);
			loader.addEventListener(Event.COMPLETE, FeicoesComplete);
			loader.addEventListener(IOErrorEvent.IO_ERROR, ErrorFunction);
		}
		
		public function AtualizarAtributos(feicao:FeicaoObjeto, callback:Function) : void {
			completeCallback = callback;
			if(!feicao)
			{
				Alert.show('Feição não pode ser nulo.');
				return;
			}
			if(feicao.IdLayerFeicao <=0)
			{
				Alert.show('Id Layer Feição deve ser maior que zero.');
				return;
			}
			
			var request: URLRequest = new URLRequest(this._serviceURL+"/AtualizarAtributos");
			request.contentType = 'application/json';
			request.method = URLRequestMethod.POST;		
			request.data = JSON.stringify(feicao);
			
			var loader:URLLoader = new URLLoader(request);
			loader.addEventListener(Event.COMPLETE, FeicaoComplete);
			loader.addEventListener(IOErrorEvent.IO_ERROR, ErrorFunction);
		}
		
		public function ExcluirFeicao(idLayerFeicao:int, objectid:int, idProjeto:int, callback:Function) : void {
			completeCallback = callback;
			if(idLayerFeicao<=0)
			{
				Alert.show('Id Layer Feição deve ser maior que 0.');
				return;
			}
			if(objectid<=0)
			{
				Alert.show('Id deve ser maior que 0.');
				return;
			}
			if(idProjeto<=0)
			{
				Alert.show('Id projeto deve ser maior que 0.');
				return;
			}
			
			var variables:URLVariables = new URLVariables();
			variables.idLayerFeicao = idLayerFeicao;
			variables.objectid = objectid;
			variables.idProjeto = idProjeto;
			
			var request: URLRequest = new URLRequest(this._serviceURL+"/Excluir");
			request.method = URLRequestMethod.POST;		
			request.data =  variables;
			
			var loader:URLLoader = new URLLoader(request);
			loader.addEventListener(Event.COMPLETE, FeicaoComplete);
			loader.addEventListener(IOErrorEvent.IO_ERROR, ErrorFunction);
		}
		
		public function ExcluirFeicoes(feicoes:Array, callback:Function) : void {
			completeCallback = callback;
			if(!feicoes)
			{
				Alert.show('Feições não pode ser nulo.');
				return;
			}
			
			var request: URLRequest = new URLRequest(this._serviceURL+"/ExcluirFeicoes");
			request.contentType = 'application/json';
			request.method = URLRequestMethod.POST;		
			request.data = JSON.stringify(feicoes);
			
			var loader:URLLoader = new URLLoader(request);
			loader.addEventListener(Event.COMPLETE, FeicoesComplete);
			loader.addEventListener(IOErrorEvent.IO_ERROR, ErrorFunction);
		}
		
		public function ImportarFeicoes(idNavegador:int, idProjeto:int, isFinalizadas:Boolean, callback:Function) : void {
			completeCallback = callback;
			if(idNavegador<=0)
			{
				Alert.show('Id Navegador deve ser maior que 0.');
				return;
			}
			if(idProjeto<=0)
			{
				Alert.show('Id projeto deve ser maior que 0.');
				return;
			}
									
			var variables:URLVariables = new URLVariables();
			variables.idNavegador = idNavegador;
			variables.idProjeto = idProjeto;
			variables.isFinalizadas = isFinalizadas;
			
			var request: URLRequest = new URLRequest(this._serviceURL+"/ImportarFeicoes");
			request.method = URLRequestMethod.POST;		
			request.data =  variables;
			
			var loader:URLLoader = new URLLoader(request);
			loader.addEventListener(Event.COMPLETE, FeicaoComplete);
			loader.addEventListener(IOErrorEvent.IO_ERROR, ErrorFunction);
		}
		
		private function setRetorno(obj:Object):Retorno
		{
			var retorno:Retorno = new Retorno();
			retorno.Mensagem = obj.Mensagem;
			retorno.Sucesso = obj.Sucesso;
			retorno.Objectid = obj.Objectid;		
			retorno.IdLayerFeicao = obj.IdLayerFeicao;		
			return retorno;
		}
		
		private function setRetornos(obj:Object):Vector.<Retorno>
		{
			var respostas:Vector.<Retorno> = new Vector.<Retorno>();
			if(obj is Array)
			{
				for(var i:int=0; i<(obj as Array).length; i++)
				{
					var retorno:Retorno = new Retorno();
					retorno.Mensagem = obj[i].Mensagem;
					retorno.Sucesso = obj[i].Sucesso;
					retorno.Objectid = obj[i].Objectid;		
					retorno.IdLayerFeicao = obj[i].IdLayerFeicao
					respostas.push(retorno);
				}
			}
			return respostas;
		}
		
		private function FeicaoComplete(event:Event) : void {
			var obj:Object = JSON.parse(event.target.data);			
			completeCallback(setRetorno(obj));
			
			DesenhadorCredenciado.AtualizarSessao();
			
			_atualizarQtdLayerFeicaoCallback();
		}
		
		private function FeicoesComplete(event:Event) : void {
			var obj:Object = JSON.parse(event.target.data);		
			completeCallback(setRetornos(obj));
			
			DesenhadorCredenciado.AtualizarSessao();
			
			_atualizarQtdLayerFeicaoCallback();
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