package models
{
	import com.gmaps.geom.Mbr;
	
	import controllers.FerramentaGeometriaControllerEvent;
	import controllers.IdentifyControllerEvent;
	
	import flash.events.Event;
	import flash.geom.Point;
	
	import models.Esri.DesenhadorEsri;
	import models.Esri.GeometriaEsriGraphic;
	
	import mx.collections.ArrayList;
	import mx.controls.Alert;
	import mx.utils.ObjectUtil;

	public class FeicoesSelecionadas
	{
		private static var instance:FeicoesSelecionadas;
		
		public var idProjeto:int;
		public var idGraphic:String ="-1";
		private var _mbr:Mbr;
		private var _layersQuantidade:Vector.<LayerFeicaoQuantidade>;
		private var _layersQuantidadeSelecionaveis:Vector.<LayerFeicaoQuantidade>;
		private var _feicoes:Vector.<Feicao>; 
		private var _feicoesSelecionaveis:Vector.<Feicao>; 
		private var _layers:Vector.<LayerFeicao>;
		public function FeicoesSelecionadas(enforcer:SingletonEnforcer)
		{
			if (enforcer == null)
				throw new Error("FeicoesSelecionadas é um Singleton, não é permitido outra instancia. Utilize FeicoesSelecionadas.getInstance().");
		}

		public function get feicoesSelecionaveis():Vector.<Feicao>
		{
			return _feicoesSelecionaveis;
		}

		public function set feicoesSelecionaveis(value:Vector.<Feicao>):void
		{
			_feicoesSelecionaveis = value;
		}

		public function get mbr():Mbr
		{
			if(!_mbr)
				_mbr = calcularMbrTodasFeicoes();
			
			return _mbr;
		}

		public static function getInstance():FeicoesSelecionadas {
			if (instance == null) {
				instance = new FeicoesSelecionadas( new SingletonEnforcer );
			}
			return instance;
		} 

		public function get feicoes():Vector.<Feicao>
		{
			return _feicoes;
		}
		
		public function set feicoes(value:Vector.<Feicao>):void
		{
			limpar(true);
			_feicoes = value;
			_feicoesSelecionaveis = new Vector.<Feicao>();
			if(_feicoes)
			{
				
				for each(var feicao:Feicao in _feicoes)
				{
					if(feicao && feicao.layerFeicao && feicao.layerFeicao.Selecionavel && (feicao.projetoId == DesenhadorEsri.getInstance().idProjeto))
						_feicoesSelecionaveis.push(feicao);
				}
			}
			
			_mbr = null;
			_layersQuantidade = null;
			_layersQuantidadeSelecionaveis = null;
			_layers = null;
			DesenhadorEsri.getInstance().map.dispatchEvent(new FerramentaGeometriaControllerEvent(FerramentaGeometriaControllerEvent.ATIVAR_DESATIVAR_BOTOES,null,null,true,true));
		}
		public function get layers():Vector.<LayerFeicao>
		{
			if(!_layers)
			{
				_layers = agruparPorLayer();
			}
			return _layers;
		}
	
		public function get layersQuantidade()
		{
			if(!_layersQuantidade)
			{
				_layersQuantidade = new Vector.<LayerFeicaoQuantidade>();
				if(_feicoes)
				{
					var encontrou:Boolean;
					for each(var feicao:Feicao in _feicoes)
					{
						encontrou = false;
						for each(var lyQtde:LayerFeicaoQuantidade in _layersQuantidade)
						{
							if(lyQtde.IdLayerFeicao ==feicao.layerFeicao.Id && lyQtde.Categoria == feicao.layerFeicao.Categoria)
							{
								lyQtde.Quantidade++;
								encontrou = true;
								break;
							}
						}
						if(!encontrou)
						{
							var layer:LayerFeicaoQuantidade = new LayerFeicaoQuantidade();
							layer.Categoria = feicao.layerFeicao.Categoria;
							layer.IdLayerFeicao = feicao.layerFeicao.Id;
							layer.NomeLayerFeicao = feicao.layerFeicao.Nome;
							layer.Quantidade++;
							_layersQuantidade.push(layer);
						}
					}
				}
			}
			return _layersQuantidade;
		}
		
		public function get layersQuantidadeSelecionaveis()
		{
			if(!_layersQuantidadeSelecionaveis)
			{
				_layersQuantidadeSelecionaveis = new Vector.<LayerFeicaoQuantidade>();
				if(_feicoes)
				{
					var encontrou:Boolean;
					for each(var feicao:Feicao in _feicoesSelecionaveis)
					{
						encontrou = false;
						for each(var lyQtde:LayerFeicaoQuantidade in _layersQuantidadeSelecionaveis)
						{
							if(lyQtde.IdLayerFeicao ==feicao.layerFeicao.Id && lyQtde.Categoria == feicao.layerFeicao.Categoria)
							{
								lyQtde.Quantidade++;
								encontrou = true;
								break;
							}
						}
						if(!encontrou)
						{
							var layer:LayerFeicaoQuantidade = new LayerFeicaoQuantidade();
							layer.Categoria = feicao.layerFeicao.Categoria;
							layer.IdLayerFeicao = feicao.layerFeicao.Id;
							layer.NomeLayerFeicao = feicao.layerFeicao.Nome;
							layer.Quantidade++;
							_layersQuantidadeSelecionaveis.push(layer);
						}
					}
				}
			}
			return _layersQuantidadeSelecionaveis;
		}
		
		public function reiniciarGeometria():void
		{
			if(_feicoesSelecionaveis)
			{
				for each (var feicao:Feicao in _feicoesSelecionaveis)
				{
					if(feicao.geometria)
					{
						feicao.geometria.excluir(false);
						
						feicao.geometria.vertices = new Vector.<Point>();
						
						for each(var ponto:Point in feicao.geometria.verticesOriginal)
						feicao.geometria.vertices.push(ponto);
						
						feicao.geometria.aneis = new Vector.<Vector.<Point>>();
						
						for each(var anel:Vector.<Point> in feicao.geometria.aneisOriginal)
						{
							var novoAnel:Vector.<Point> = new Vector.<Point>();					
							
							for each(var ponto:Point in anel)
							{
								novoAnel.push(ponto);
							}
							feicao.geometria.aneis.push(novoAnel);
						}
					}
				}
			}
		}
		public function limpar(excluirFeicoes:Boolean):void
		{
			if(_feicoes )
			{
				for each(var feicao:Feicao in _feicoes)
				{
					feicao.layerFeicao.selecionado = false;
					if(feicao.geometria)
						feicao.geometria.excluir(excluirFeicoes);					
				}
				
				for each(var feicao:Feicao in _feicoesSelecionaveis)
				{
					feicao.layerFeicao.selecionado = false;
					if(feicao.geometria)
						feicao.geometria.excluir(excluirFeicoes);					
				}
				
				if(excluirFeicoes)
				{
					_feicoes = new Vector.<Feicao>();
					_feicoesSelecionaveis = new Vector.<Feicao>();
				}
			}
			if(idGraphic!="-1")
			{
				DesenhadorEsri.getInstance().excluirFeicao(idGraphic);
			}
			DesenhadorEsri.getInstance().map.dispatchEvent(new FerramentaGeometriaControllerEvent(FerramentaGeometriaControllerEvent.ATIVAR_DESATIVAR_BOTOES,null,null,true,true));
			
			_mbr = null;
		}
				
		public function exclirSelecaoFeicao(id:int, layer:int):void
		{
			if(_feicoes )
			{
				for each(var feicao:Feicao in _feicoes)
				{
					if(feicao.objectId == id && feicao.layerFeicao.Id == layer)
					{
						_feicoes.splice(_feicoes.indexOf(feicao),1);
						break;
					}
				}	
			}
			if(_feicoesSelecionaveis)
			{
				for each(var feicao:Feicao in _feicoesSelecionaveis)
				{
					if(feicao.objectId == id && feicao.layerFeicao.Id == layer)
					{
						_feicoesSelecionaveis.splice(_feicoesSelecionaveis.indexOf(feicao),1);
						break;
					}
				}
			}
		}
		
		public function contemFeicao(id:int, layer:int):Boolean
		{
			for each(var feicao:Feicao in _feicoes)
			{
				if(feicao.objectId == id && feicao.layerFeicao.Id == layer)
				{
					return true;					
				}
			}	
			return false;
		}
		public function desenhar(simbologia:int=5, apenasAsSelecionaveis:Boolean=false):void
		{
			if(_feicoes)
			{
				idGraphic = DesenhadorEsri.getInstance().excluirFeicao(idGraphic);
				
				if(apenasAsSelecionaveis)
				{
					for(var i:int=0; i<_feicoesSelecionaveis.length; i++)
					{
						idGraphic = DesenhadorEsri.getInstance().desenharGeometry(idGraphic, _feicoesSelecionaveis[i].geometry,simbologia,_feicoesSelecionaveis[i].tipoGeometria,false);
					}
				}
				else
				{
				
					for(var i:int=0; i<_feicoes.length; i++)
					{
						idGraphic = DesenhadorEsri.getInstance().desenharGeometry(idGraphic, _feicoes[i].geometry,simbologia,_feicoes[i].tipoGeometria,false);
					}
				}
			}
			else
				idGraphic = DesenhadorEsri.getInstance().excluirFeicao(idGraphic);
		}
		public function calcularMbrTodasFeicoes():Mbr
		{
			var minx:Number = 9999999999999;
			var miny:Number = 9999999999999;
			var maxx:Number =  0;
			var maxy:Number =  0;
			
			if(_feicoes)
			{
				for each (var feicao:Feicao in _feicoes)
				{
					var vertices:Vector.<Point>;
					if(feicao.tipoGeometria == Geometria.Poligono && feicao.geometria.aneis && feicao.geometria.aneis.length>0)
						vertices = feicao.geometria.aneis[0];
					else
						vertices = feicao.geometria.vertices;
					
					if(vertices)
					{
						for each(var ponto:Point in vertices)
						{
							if(ponto.x < minx)
								minx = ponto.x;
							if(ponto.x >maxx)
								maxx = ponto.x;
							if(ponto.y<miny)
								miny = ponto.y;
							if(ponto.y > maxy)
								maxy = ponto.y;
						}
					}
				}
			}
		
			if(minx != 9999999999999 &&  miny != 9999999999999 && maxx !=  0 && maxy !=  0)
				return new Mbr(minx, miny, maxx, maxy);
			else
				return null;
		}
		private function agruparPorLayer():Vector.<LayerFeicao>
		{
			_layers = new Vector.<LayerFeicao>();
			
			if(feicoes)
			{
				
				feicoes.sort(function (itemA:Object, itemB:Object):int {
					try{
						
						return ObjectUtil.stringCompare(itemA.layerFeicao.Nome, itemB.layerFeicao.Nome);
					}
					catch(evt:Object){}
					
					return -1;
					
				});
				
				for each(var feicao:Feicao in feicoes)
				{
					if(feicao)
					{
						var encontrou:Boolean = false;
						
						for(var k:int =0; k<_layers.length;k++)
						{
							if(_layers[k].Id == feicao.layerFeicao.Id)
							{
								_layers[k].Quantidade++;
								feicao.IdLista =_layers[k].Quantidade;
								
								_layers[k].Feicoes.push(feicao);
								encontrou = true;
								break;
							}
						}
						if(!encontrou)
						{
							var layer:LayerFeicao = new LayerFeicao();
							layer.Categoria = feicao.layerFeicao.Categoria;
							layer.ColunaPk = feicao.layerFeicao.ColunaPk;
							layer.Colunas = feicao.layerFeicao.Colunas;
							layer.Id = feicao.layerFeicao.Id;
							layer.IdLayer = feicao.layerFeicao.IdLayer;
							layer.Nome = feicao.layerFeicao.Nome;
							layer.Descricao = feicao.layerFeicao.Descricao;
							layer.TipoGeometria = feicao.tipoGeometria;
							layer.Feicoes = new Vector.<Feicao>();
							feicao.IdLista = 0;
							layer.Feicoes.push(feicao);
							layer.Quantidade = 1;
							layers.push(layer);
							encontrou = false;
						}
					}
				}
				
				if(_layers)
				{
					for each(var ly:LayerFeicao in _layers)
					{	
						ly.Feicoes.sort(function (itemA:Object, itemB:Object):int {
							try{
								
								return ObjectUtil.compare(itemA.objectId, itemB.objectId);
							}
							catch(evt:Object){}
							
							return -1;
							
						});
					 }
				}
			}
			return _layers;
		}
		public function ativarDesativarLayer(ativar:Boolean,id:int):void
		{
			if(layers)
			{
				for each (var ly:LayerFeicao in layers)
				{
					if(id == ly.Id)
					{
						ly.IsAtivo = ativar;
					}
				}
			}
		}
		
	}
}
class SingletonEnforcer {
}