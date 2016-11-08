package com.tecnomapas.componentes
{
	import com.adobe.serialization.json.JSON;
	
	import flash.events.Event;
	import flash.net.URLLoader;
	import flash.net.URLRequest;
	import flash.net.URLRequestMethod;
	import flash.net.URLVariables;

	public class DataRepository
	{

		private var _getInventoryURL:String;

		
		public function DataRepository(url: String)
		{
			this._getInventoryURL = url + "Home/ListarPontosEmpreendimentoFiltrados";
		}
		
		public function get getInventoryURL():String
		{
			return _getInventoryURL;
		}

		public function set getInventoryURL(value:String):void
		{
			_getInventoryURL = value;
		}

		private var completeCallback:Function;
		
		public function GetInventory(nome: String, segmento: String, municipio: String, levantamento: Boolean, queima: Boolean, supressao: Boolean,callback:Function) : void {
			completeCallback = callback;
			
			var variables:URLVariables = new URLVariables();
			variables.nome = nome;
			variables.segmento = segmento;
			variables.municipio = municipio;
			variables.levantamento = (levantamento)? "Sim": "";
			variables.queima = (queima)? "Sim": "";
			variables.supressao = (supressao)? "Sim": "";

			var request: URLRequest = new URLRequest(this._getInventoryURL);
			request.method = URLRequestMethod.POST;
			request.data = variables;
			
			var loader:URLLoader = new URLLoader(request);
			loader.addEventListener(Event.COMPLETE, getInventoryComplete);
		}
		
		private function getInventoryComplete(event:Event) : void {
			var obj:Object = JSON.decode(event.target.data);
			completeCallback(obj);
		}
	}
}