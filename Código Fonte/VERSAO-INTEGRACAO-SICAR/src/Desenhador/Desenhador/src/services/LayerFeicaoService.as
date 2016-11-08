package services
{
	import flash.events.Event;
	import flash.events.IOErrorEvent;
	import flash.net.URLLoader;
	import flash.net.URLRequest;
	import flash.net.URLRequestMethod;
	import flash.net.URLVariables;
	
	import models.CategoriaLayerFeicao;
	import models.ColunaLayerFeicao;
	import models.Item;
	import models.LayerFeicao;
	import models.LayerFeicaoQuantidade;
	
	import mx.collections.ArrayList;
	import mx.controls.Alert;
	
	public class LayerFeicaoService
	{
	
		private var _serviceURL:String;
		private var _categorias:Vector.<CategoriaLayerFeicao>;
		private var _layersQtde:Vector.<LayerFeicaoQuantidade>;
		private var completeCallback:Function;
		
		[Embed(source="../assets/erro.png")]
		private var ErroImg:Class;
		
		public function LayerFeicaoService()
		{	
			this._serviceURL = ConfiguracaoService.getInstance().serviceUrl +"/LayerFeicao";
		}
		
		public function get getServiceURL():String
		{
			if(_serviceURL.length>4&&_serviceURL.substr(0,4).toLowerCase() == "null")
				getServiceURL = ConfiguracaoService.getInstance().serviceUrl +"/LayerFeicao";
			return _serviceURL;
		}
		
		public function set getServiceURL(value:String):void
		{
			_serviceURL = value;
		}
		
		public function ListarCategoria(idNavegador:int, idProjeto:int, callback:Function) : void {
			completeCallback = callback;
			
			var variables:URLVariables = new URLVariables();
			variables.idNavegador = idNavegador;
			variables.idProjeto = idProjeto;
			
			var request: URLRequest = new URLRequest(this._serviceURL+"/ListarCategoria");
			request.method = URLRequestMethod.POST;
			request.data = variables;
			
			var loader:URLLoader = new URLLoader(request);
			loader.addEventListener(Event.COMPLETE, ListarCategoriaComplete);
			loader.addEventListener(IOErrorEvent.IO_ERROR, ErrorFunction);
		} 
		
		public function ListarQuantidadeLayerFeicao(idNavegador:int, idProjeto:int, callback:Function) : void {
			completeCallback = callback;
			
			var variables:URLVariables = new URLVariables();
			variables.idNavegador = idNavegador;
			variables.idProjeto = idProjeto;
			
			var request: URLRequest = new URLRequest(this._serviceURL+"/ListarQuantidadeLayerFeicao");
			request.method = URLRequestMethod.POST;
			request.data = variables;
			
			var loader:URLLoader = new URLLoader(request);
			loader.addEventListener(Event.COMPLETE, ListarQuantidadeLayerFeicaoComplete);
			loader.addEventListener(IOErrorEvent.IO_ERROR, ErrorFunction);
		}
		
		public function listarQuantidadeLayerFeicaoResult(value:Object):void
		{
			_layersQtde = new Vector.<LayerFeicaoQuantidade>();
			
			var ar:Array = value as Array;
			if(ar)
			{
				var layerQtde:LayerFeicaoQuantidade;
				var arLayersFeicao:Array;
				for(var i:int=0; i<ar.length; i++)
				{
					layerQtde = new LayerFeicaoQuantidade();
					layerQtde.Categoria = ar[i].Categoria;
					layerQtde.IdLayerFeicao = ar[i].LayerFeicao;
					layerQtde.Quantidade = ar[i].Quantidade;
					
					if(layerQtde)
						_layersQtde.push(layerQtde);
					else
						Alert.show('valor nulo');
				}	
			}
		}
		
		public function listarCategoriaResult(value:Object):void
		{
			_categorias = new Vector.<CategoriaLayerFeicao>();
			
			var ar:Array = value as Array;
			if(ar)
			{
				var categoria:CategoriaLayerFeicao;
				var arLayersFeicao:Array;
				for(var i:int=0; i<ar.length; i++)
				{
					categoria = new CategoriaLayerFeicao();
					categoria.Id = ar[i].Id;
					categoria.Nome = ar[i].Nome;
					categoria.LayersFeicao = new Vector.<LayerFeicao>();
					var layerFeicao:LayerFeicao;
					if(ar[i].LayersFeicoes)
					{
						arLayersFeicao = ar[i].LayersFeicoes;
						for(var f:int=0; f<arLayersFeicao.length; f++)
						{
							layerFeicao = new LayerFeicao();
							layerFeicao.Categoria = categoria.Id;
							layerFeicao.Id = arLayersFeicao[f].Id;
							layerFeicao.IdLista = f;
							layerFeicao.Nome = arLayersFeicao[f].Nome;
							layerFeicao.Descricao = arLayersFeicao[f].Descricao;
							layerFeicao.IdLayer = arLayersFeicao[f].IdLayer;
							layerFeicao.TipoGeometria = arLayersFeicao[f].TipoGeometria;
							layerFeicao.Visivel = arLayersFeicao[f].Visivel;
							layerFeicao.IsFinalizada = arLayersFeicao[f].IsFinalizada;
							layerFeicao.ColunaPk = arLayersFeicao[f].ColunaPK;
							layerFeicao.Quantidade = arLayersFeicao[f].Quantidade;
							layerFeicao.Selecionavel = arLayersFeicao[f].Selecionavel;
							layerFeicao.ServicoId = arLayersFeicao[f].ServicoId;
							layerFeicao.ServicoIsPrincipal = arLayersFeicao[f].ServicoIsPrincipal;
							layerFeicao.ServicoUrlMxd = arLayersFeicao[f].ServicoUrlMxd;
								
							layerFeicao.Colunas = new Vector.<ColunaLayerFeicao>();
							
							if(arLayersFeicao[f].Colunas)
							{
								var arColunas:Array = arLayersFeicao[f].Colunas as Array;
								var coluna:ColunaLayerFeicao;
								for(var k:int=0; k< arColunas.length; k++)
								{
									coluna = new ColunaLayerFeicao();
									coluna.Alias = arColunas[k].Alias;
									coluna.Coluna = arColunas[k].Coluna;
									coluna.Tipo = arColunas[k].Tipo;
									coluna.Referencia = arColunas[k].Referencia;
									coluna.IsObrigatorio = arColunas[k].IsObrigatorio;
									coluna.IsVisivel = arColunas[k].IsVisivel;
									coluna.IsEditavel = arColunas[k].IsEditavel;
									coluna.Tamanho = arColunas[k].Tamanho;
									coluna.IdLista = k;
									coluna.Operacao = arColunas[k].Operacao;
									coluna.ValorCondicao = arColunas[k].ValorCondicao;
									coluna.ColunaObrigada = arColunas[k].ColunaObrigada;
									var arItens:Array = arColunas[k].Itens as Array;
									coluna.Itens = new ArrayList();
									if(arItens)
									{
										for(var l:int=0; l< arItens.length; l++)
										{
											var item:Item = new Item();
											item.Chave = arItens[l].Chave;
											item.Texto = arItens[l].Texto;
											coluna.Itens.addItem(item);
										}
									}
									layerFeicao.Colunas.push(coluna);
								}
							}
							categoria.LayersFeicao.push(layerFeicao);
						}
					}
					_categorias.push(categoria);
				}	
			}
		}
		
		private function ListarCategoriaComplete(event:Event) : void {
			var obj:Object = JSON.parse(event.target.data);
			listarCategoriaResult(obj);	
			completeCallback(_categorias);
		}
				
		private function ListarQuantidadeLayerFeicaoComplete(event:Event) : void {
			var obj:Object = JSON.parse(event.target.data);
			
			listarQuantidadeLayerFeicaoResult(obj);	
			completeCallback(_layersQtde);
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