package com.idaf.services
{
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

		private var getMunicipioCompleteCallback:Function;
		private var getLoteCompleteCallback:Function;
		
		public function GetMunicipios(municipio: String,callback:Function) : void {
			getMunicipioCompleteCallback = callback;
			
			var variables:URLVariables = new URLVariables();
			variables.municipio = municipio;

			var request: URLRequest = new URLRequest(this._serviceURL+"/LocalizarMunicipio");
			request.method = URLRequestMethod.GET;
			request.data = variables;
			
			var loader:URLLoader = new URLLoader(request);
			loader.addEventListener(Event.COMPLETE, GetMunicipiosComplete);
		}
		
		private function GetMunicipiosComplete(event:Event) : void {
			var obj:Object = JSON.parse(event.target.data);
			getMunicipioCompleteCallback(obj.Data);
		}
		
		public function GetLote(codfiscal: String,codquadra: String,codlote: String,callback:Function) : void {
			getLoteCompleteCallback = callback;
			
			var variables:URLVariables = new URLVariables();
			variables.codfiscal = codfiscal;
			variables.codquadra = codquadra;
			variables.codlote = codlote;
			
			var request: URLRequest = new URLRequest(this._serviceURL+"/LocalizarLote");
			request.method = URLRequestMethod.GET;
			request.data = variables;
			
			var loader:URLLoader = new URLLoader(request);
			loader.addEventListener(Event.COMPLETE, GetLoteComplete);
		}
		
		private function GetLoteComplete(event:Event) : void {
			var obj:Object = JSON.parse(event.target.data);
			getLoteCompleteCallback(obj);
		}

	}
}