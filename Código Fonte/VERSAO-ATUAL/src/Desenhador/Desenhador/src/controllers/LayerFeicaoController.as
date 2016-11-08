package controllers
{
	import flash.events.EventDispatcher;
	import flash.events.IEventDispatcher;
	
	import models.CategoriaLayerFeicao;
	import models.LayerFeicao;
	import models.LayerFeicaoQuantidade;
	
	import services.LayerFeicaoService;

	public class LayerFeicaoController extends EventDispatcher
	{
		private static var instance:LayerFeicaoController;
		
		
		private var dataservice:LayerFeicaoService = new LayerFeicaoService(); 
		public var categorias:Vector.<CategoriaLayerFeicao>;
		public function LayerFeicaoController(enforcer:SingletonEnforcer)
		{
			if (enforcer == null)
				throw new Error("LayerFeicaoController é um Singleton, não é permitido outra instancia. Utilize LayerFeicaoController.getInstance().");
		}
		
		public static function getInstance():LayerFeicaoController {
			if (instance == null) {
				instance = new LayerFeicaoController( new SingletonEnforcer );
				
			} 
			return instance;
		}
		
		public function listarQuantidadeLayerFeicao(idNavegador:int, idProjeto:int): void {
			dataservice.ListarQuantidadeLayerFeicao(idNavegador, idProjeto, function(_layersQtde:Vector.<LayerFeicaoQuantidade>) : void {
				if(categorias && _layersQtde)
				{
					for each(var lyQtde:LayerFeicaoQuantidade in _layersQtde)
					{
						for each (var ct:CategoriaLayerFeicao in categorias)
						{
							if(lyQtde.Categoria == ct.Id)
							{
								for each(var layer:LayerFeicao in ct.LayersFeicao)
								{
									if(layer.Id == lyQtde.IdLayerFeicao)
									{
										layer.Quantidade = lyQtde.Quantidade;
										break;
									}
								}
								break;
							}
						}
					}
				}
				LayerFeicaoController.getInstance().dispatchEvent(new LayerFeicaoControllerEvent(LayerFeicaoControllerEvent.LISTAR_QUANTIDADE,categorias));
			});
		}
		public function ativarDesativarCategoria(ativar:Boolean,idCategoria:int):void
		{
			if(categorias )
			{
				for each (var ct:CategoriaLayerFeicao in categorias)
				{
					if(idCategoria == ct.Id)
					{
						ct.IsAtivo = ativar;
					}
				}
			}
		}
		public function listarCategorias(idNavegador:int, idProjeto:int): void {
			dataservice.ListarCategoria(idNavegador, idProjeto, function(_categorias:Vector.<CategoriaLayerFeicao>) : void {
				categorias = _categorias;
				LayerFeicaoController.getInstance().dispatchEvent(new LayerFeicaoControllerEvent(LayerFeicaoControllerEvent.LISTAR_CATEGORIAS,categorias));
			});
		}
		public function buscarLayerFeicao(idLayer:int, idServico:int):LayerFeicao
		{
			if(categorias)
			{ 
				for each(var c:CategoriaLayerFeicao in categorias)
				{
					for each (var f:LayerFeicao in c.LayersFeicao)
					{
						if(f.IdLayer == idLayer && f.ServicoId == idServico)
						{
							return f;
						}
					}
				}
			}
			return null;
		}
		public function buscarLayerFeicaoServicoPrincipal(idLayer:int):LayerFeicao
		{
			if(categorias)
			{ 
				for each(var c:CategoriaLayerFeicao in categorias)
				{
					for each (var f:LayerFeicao in c.LayersFeicao)
					{
						if(f.IdLayer == idLayer && f.ServicoIsPrincipal)
						{
							return f;
						}
					}
				}
			}
			return null;
		}
		public function buscarLayersVisiveis(somenteServicoPrincipal:Boolean=true):Array
		{
			var ar:Array = new Array();
			if(categorias)
			{ 
				for each(var c:CategoriaLayerFeicao in categorias)
				{
					for each (var f:LayerFeicao in c.LayersFeicao)
					{
						if(f.Visivel)
						{
							if(somenteServicoPrincipal)
							{
								if(f.ServicoIsPrincipal)
									ar.push(f.IdLayer);
							}
							else
							{
								ar.push(f.IdLayer);
							}
						}
					}
				}
			}
			return ar;
		}
		public function buscarLayersFeicoesMesmoTipoGeometria(tipoGeometria:int, apenasAsSelecionaveis:Boolean=false):Vector.<LayerFeicao>
		{
			var listaLayers:Vector.<LayerFeicao> = new Vector.<LayerFeicao>();
			if(categorias)
			{ 
				for each(var c:CategoriaLayerFeicao in categorias)
				{
					for each (var f:LayerFeicao in c.LayersFeicao)
					{
						if(f.TipoGeometria == tipoGeometria)
						{
							if(apenasAsSelecionaveis )
							{
								if(f.Selecionavel)
									listaLayers.push(f);
							}
							else
							{
								listaLayers.push(f);
							}
						}
					}
				}
			}
			return listaLayers;
		}
	}
}

class SingletonEnforcer {
}