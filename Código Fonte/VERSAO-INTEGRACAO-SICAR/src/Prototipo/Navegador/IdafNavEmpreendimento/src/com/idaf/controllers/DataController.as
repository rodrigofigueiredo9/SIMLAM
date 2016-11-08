package com.idaf.controllers
{
	import com.idaf.models.Listas;
	import com.idaf.models.PontoEmpreendimento;
	import com.idaf.services.DataService;
	
	import flash.events.EventDispatcher;
	
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
		
		public function GetPontoEmpreendimento(empreendimento: String,pessoa: String,processo: String,segmento: String,municipio: String,atividade: String): void {
			dataservice.GetEmpreendimentos(empreendimento,pessoa,processo,segmento,municipio,atividade,function(data:Object) : void {
				var result: Object = data as Array;
				var empreendimentos: Array = [];

				for each (var item: Object in result) {
					var empreendimento: PontoEmpreendimento = new PontoEmpreendimento();
					
					if (item.id != 0) {
						empreendimento.id = item.id;
						empreendimento.denominador = item.denominador;
						empreendimento.segmento = item.segmento;
						empreendimento.atividade = item.atividade;
						empreendimento.processos = item.processos;
						empreendimento.municipio = item.municipio;
						empreendimento.x = item.x;
						empreendimento.y = item.y;
					} else {
						empreendimento = null;
					}
					empreendimentos.push(empreendimento);
				}
				
				DataController.getInstance().dispatchEvent(new DataControllerEvent(DataControllerEvent.EMPREENDIMENTOS_ATUALIZADOS,empreendimentos));
			});
		}

		public function IdentificarEmpreendimentos(empreendimentos: Array): void {
			dataservice.IdentificarEmpreendimentos(empreendimentos,function(data:Object) : void {
				var result: Object = data as Array;
				var empreendimentos: Array = [];
				
				for each (var item: Object in result) {
					var empreendimento: PontoEmpreendimento = new PontoEmpreendimento();
					
					if (item.id != 0) {
						empreendimento.id = item.id;
						empreendimento.denominador = item.denominador;
						empreendimento.segmento = item.segmento;
						empreendimento.atividade = item.atividade;
						empreendimento.processos = item.processos;
						empreendimento.municipio = item.municipio;
						empreendimento.x = item.x;
						empreendimento.y = item.y;
					} else {
						empreendimento = null;
					}
					empreendimentos.push(empreendimento);
				}
				
				DataController.getInstance().dispatchEvent(new DataControllerEvent(DataControllerEvent.EMPREENDIMENTOS_IDENTIFICADOS,empreendimentos));
			});
		}

		
		public function GetListas(): void {
			dataservice.GetListas(function(data:Object) : void {
				var result: Object = data as Object;
				
				var lista: Listas = new Listas();

				lista.atividades = result.atividades;
				lista.segmentos = result.segmentos;
				lista.municipios = result.municipios;
				
				var listas: Array = [];
				listas.push(lista);
				
				DataController.getInstance().dispatchEvent(new DataControllerEvent(DataControllerEvent.LISTAS_ATUALIZADAS,listas));
			});
		}
	}
}

class SingletonEnforcer {
}