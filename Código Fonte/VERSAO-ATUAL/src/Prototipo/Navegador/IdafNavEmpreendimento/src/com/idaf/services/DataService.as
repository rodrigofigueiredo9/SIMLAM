package com.idaf.services
{
	import com.adobe.serialization.json.JSON;
	
	import flash.events.Event;
	import flash.net.URLLoader;
	import flash.net.URLRequest;
	import flash.net.URLRequestMethod;
	import flash.net.URLVariables;

	public class DataService
	{
		private var _serviceURL:String;
		
		public function DataService(serviceUrl: String)
		{
			this._serviceURL = serviceUrl;
		}
		
		public function get getServiceURL():String
		{
			return _serviceURL;
		}

		public function set getServiceURL(value:String):void
		{
			_serviceURL = value;
		}

		private var getEmpreendimentosCompleteCallback:Function;
		
		public function GetEmpreendimentos(empreendimento: String,pessoa: String,processo: String,segmento: String,municipio: String,atividade: String,callback:Function) : void {
			getEmpreendimentosCompleteCallback = callback;
			
			var variables:URLVariables = new URLVariables();
			variables.empreendimento = empreendimento;
			variables.pessoa = pessoa;
			variables.processo = processo;
			variables.segmento = segmento;
			variables.municipio = municipio;
			variables.atividade = atividade;
			
			var request: URLRequest = new URLRequest(this._serviceURL+"/ListarEmpreendimentos");
			request.method = URLRequestMethod.GET;
			request.data = variables;
			
			var loader:URLLoader = new URLLoader(request);
			loader.addEventListener(Event.COMPLETE, GetEmpreendimentosComplete);
		}
		
		private function GetEmpreendimentosComplete(event:Event) : void {
			var obj:Object = JSON.decode(event.target.data);
			getEmpreendimentosCompleteCallback(obj);
		}

		private var identificarEmpreendimentosCompleteCallback:Function;
		
		public function IdentificarEmpreendimentos(empreendimentos: Array,callback:Function) : void {
			identificarEmpreendimentosCompleteCallback = callback;
			
			var variables:URLVariables = new URLVariables();
			variables.empreendimentos = empreendimentos;
			
			var request: URLRequest = new URLRequest(this._serviceURL+"/IdentificarEmpreendimentos");
			request.method = URLRequestMethod.GET;
			request.data = variables;
			
			var loader:URLLoader = new URLLoader(request);
			loader.addEventListener(Event.COMPLETE, IdentificarEmpreendimentosComplete);
		}
		
		private function IdentificarEmpreendimentosComplete(event:Event) : void {
			var obj:Object = JSON.decode(event.target.data);
			identificarEmpreendimentosCompleteCallback(obj);
		}
		
		private var getListasCompleteCallback:Function;
		
		public function GetListas(callback:Function) : void {
			getListasCompleteCallback = callback;
			
			var request: URLRequest = new URLRequest(this._serviceURL+"/ObterListas");
			request.method = URLRequestMethod.GET;
			
			var loader:URLLoader = new URLLoader(request);
			loader.addEventListener(Event.COMPLETE, GetListasComplete);
		}
		
		private function GetListasComplete(event:Event) : void {
			var obj:Object = JSON.decode(event.target.data);
			getListasCompleteCallback(obj);
		}
		
	}
}