package controllers
{
	import com.adobe.serialization.json.JSON;
	import com.esri.ags.utils.JSON;
	import com.gmaps.geom.Mbr;
	
	import flash.events.Event;
	import flash.events.EventDispatcher;
	
	import models.CategoriaQuadroDeArea;
	import models.Esri.DesenhadorEsri;
	import models.FeicaoAreaAbrangencia;
	import models.FeicaoObjeto;
	import models.Projeto;
	import models.Retorno;
	import models.SituacaoProcessamento;
	
	import mx.controls.Alert;
	
	import services.ProjetoService;

	public class ProjetoController extends EventDispatcher
	{
		private static var instance:ProjetoController;
		
		
		private var dataservice:ProjetoService = new ProjetoService(); 
		public var categorias:Vector.<CategoriaQuadroDeArea>;
		public var projeto:Projeto;
		public function ProjetoController(enforcer:SingletonEnforcer)
		{
			if (enforcer == null)
				throw new Error("ProjetoController é um Singleton, não é permitido outra instancia. Utilize ProjetoController.getInstance().");
		}
		
		public static function getInstance():ProjetoController {
			if (instance == null) {
				instance = new ProjetoController( new SingletonEnforcer );
				
			} 
			return instance;
		}
		
		public function ativarDesativarCategoria(ativar:Boolean,idCategoria:int):void
		{
			if(categorias )
			{
				for each (var ct:CategoriaQuadroDeArea in categorias)
				{
					if(idCategoria == ct.Id)
					{
						ct.IsAtivo = ativar;
					}
				} 
			}
		}
		public function ListarQuadroAreas(idNavegador:int, idProjeto:int): void {
			dataservice.ListarQuadroAreas(idNavegador, idProjeto, function(_categorias:Vector.<CategoriaQuadroDeArea>) : void {
				if(categorias && _categorias)
				{
					for each(var cat:CategoriaQuadroDeArea in categorias)
					{
						for each(var categoria:CategoriaQuadroDeArea in _categorias)
						{
							if(cat.Nome.toUpperCase() == categoria.Nome.toUpperCase())
							{
								categoria.IsAtivo = cat.IsAtivo;
								break;
							}
						}
					}
				}
				
				categorias = _categorias;
				ProjetoController.getInstance().dispatchEvent(new ProjetoControllerEvent(ProjetoControllerEvent.LISTAR_QUADROAREAS,categorias,null));
			});
		}
		
		public function buscarDadosProjeto(idProjeto:int, idFilaTipo:int):void
		{
				dataservice.BuscarDadosProjeto(idProjeto, idFilaTipo, function(_projeto:Projeto) : void {
				projeto = _projeto;
				ProjetoController.getInstance().dispatchEvent(new ProjetoControllerEvent(ProjetoControllerEvent.BUSCAR_DADOS_PROJETO,null,null,projeto));
			});
		}
		
		public function buscarSituacaoProcessamento(objeto:Object):void
		{
			var situacao:SituacaoProcessamento = dataservice.buscarSituacaoProcessamento(objeto);
			dispatchEvent(new ProjetoControllerEvent(ProjetoControllerEvent.ATUALIZAR_SITUACAOPROCESSAMENTO,null,situacao));
		}
		
		public function buscarAreaAbrangencia(objeto:Object):void
		{
			var areaAbrangencia:Mbr = dataservice.buscarAreaAbrangencia(objeto);
			if(areaAbrangencia)
			{
				DesenhadorEsri.getInstance().zoomAreaAbrangencia(areaAbrangencia);
				if(areaAbrangencia && areaAbrangencia.maxX> 0 && areaAbrangencia.minX &&
					areaAbrangencia.maxY >0 && areaAbrangencia.minY)
				{
					var feicaoArea:FeicaoAreaAbrangencia = new FeicaoAreaAbrangencia();
					feicaoArea.IdProjeto = DesenhadorEsri.getInstance().idProjeto;
					feicaoArea.MaxX = areaAbrangencia.maxX;
					feicaoArea.MinX = areaAbrangencia.minX;
					feicaoArea.MaxY = areaAbrangencia.maxY;
					feicaoArea.MinY = areaAbrangencia.minY;
					 
					/*Alert.show(feicaoArea.MaxX.toString() + " "+ feicaoArea.MinX.toString()+ " "+
						feicaoArea.MaxY.toString()+ " "+feicaoArea.MinY.toString());*/
					
					dataservice.SalvarAreaAbrangencia(feicaoArea,function(resposta:Retorno) : void {
							if(resposta)
							{
								if(!resposta.Sucesso)
									Alert.show(resposta.Mensagem);
							}
						});
					}
				}
			}
	}
}

class SingletonEnforcer {
}