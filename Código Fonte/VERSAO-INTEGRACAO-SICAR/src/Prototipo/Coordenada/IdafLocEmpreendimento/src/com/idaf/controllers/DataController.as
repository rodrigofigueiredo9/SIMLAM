package com.idaf.controllers
{
	import flash.events.EventDispatcher;
	import flash.events.IEventDispatcher;
	
	import com.idaf.models.Municipio;
	import com.idaf.models.Lote;
	
	import com.idaf.services.DataService;
	
	public class DataController extends EventDispatcher
	{
		private static var instance: DataController;
		
		private var dataservice: DataService = null; 
		
		public function DataController(enforcer:SingletonEnforcer)
		{
			if (enforcer == null)
				throw new Error("DataController is a Singleton, no other instances allowed! Use \"DataController.getInstance()\" instead.");
		}
		
		public static function getInstance():DataController {
			if (instance == null) {
				instance = new DataController( new SingletonEnforcer );
			}
			return instance;
		}
		
		public function setDataServiceUrl(url: String): void {
			this.dataservice = new DataService(url);
		}
		
		public function getMunicipios(municipio: String): void {
			dataservice.GetMunicipios(municipio,function(data:Object) : void {
				var results: Array = data as Array;
				
				var municipios: Array = [];
				
				for each (var items: Object in results) {
					var result: Municipio = new Municipio();
					result.id = items.id;
					result.nome = items.nome;
					result.x = items.x;
					result.y = items.y;
					municipios.push(result);
				}
				
				DataController.getInstance().dispatchEvent(new DataControllerEvent(DataControllerEvent.MUNICIPIOS_ATUALIZADOS,municipios));
			});
		}
		
		public function getLote(codfiscal: String,codquadra: String,codlote: String): void {
			dataservice.GetLote(codfiscal,codquadra,codlote,function(data:Object) : void {
				var result: Object = data as Object;

				var lote: Lote = new Lote();
				
				if (result.id != 0) {
					lote.id = result.id;
					lote.codfiscal = result.codfiscal;
					lote.codquadras = result.codquadras;
					lote.codlotes = result.codlotes;
					lote.proprietario = result.proprietario;
					lote.tipo_logradouro = result.tipo_logradouro;
					lote.logradouro = result.logradouro;
					lote.numero = result.numero;
					lote.bairro = result.bairro;
					lote.x = result.x;
					lote.y = result.y;
				} else {
					lote = null;
				}
				
				
				var lotes: Array = [];
				lotes.push(lote);
				
				DataController.getInstance().dispatchEvent(new DataControllerEvent(DataControllerEvent.LOTES_ATUALIZADO,lotes));
			});
		}
	}
}

class SingletonEnforcer {
}