package models
{
	import flash.geom.Point;
	
	import mx.controls.Alert;

	public class FeicaoObjeto
	{
		public function FeicaoObjeto(layerFeicao:LayerFeicao,  idProjeto:int, vertices:Vector.<Point>=null,  objectid:int=0, aneis:Vector.<Vector.<Point>>=null, atributos:Vector.<AtributoFeicao>=null)
		{
			Vertices = new Array();
			Atributos = new Array();
			Aneis = new Array();
			
			var a:String = "";
			if(vertices)
			{
				var vertice:Vertice;
				for(var i:int =0; i< vertices.length; i++)
				{
					vertice = new Vertice(); 
					vertice.X = (vertices[i] as Point).x;
					vertice.Y = (vertices[i] as Point).y;
					Vertices.push(vertice);	 
				}
			}
			if(aneis) 
			{
				var anel:Array = null;
				for(var i:int =0; i< aneis.length; i++)
				{
					if(aneis[i] is Vector.<Point>)
					{ 
						anel = new Array();
						var listaVertices:Vector.<Point> = aneis[i] as Vector.<Point>;
						var vertice:Vertice;
						for(var k:int=0; k< listaVertices.length; k++)
						{
							vertice = new Vertice();
							vertice.X = (listaVertices[k] as Point).x;
							vertice.Y = (listaVertices[k] as Point).y;
							anel.push(vertice);	
						}
						Aneis.push(anel);
					}
				}
			}
			ObjectId = objectid;
			IdProjeto = idProjeto;
			var atributo:AtributoFeicao = null;
			if(layerFeicao )
			{
				IdLayerFeicao = layerFeicao.Id;
				
				if(atributos)
				{
					for each(var atributo:AtributoFeicao in atributos)
					{
						Atributos.push(atributo);
					}
				}	
				else
				{
					atributo = new AtributoFeicao();
					atributo.Nome = layerFeicao.ColunaPk;
					atributo.Tipo = AtributoFeicao.Manual;
					atributo.Valor = ObjectId.toString();
					Atributos.push(atributo);
					
					atributo = new AtributoFeicao();
					atributo.Nome = "PROJETO";
					atributo.Tipo = AtributoFeicao.Manual;
					atributo.Valor = idProjeto.toString();
					Atributos.push(atributo);
				}
			}
		}
		private var _idLayerFeicao:int;
		private var _objectId:int;
		private var _idProjeto:int;
		private var _vertices:Array;
		private var _atributos:Array;
		private var _aneis:Array;

		public function get Aneis():Array
		{
			return _aneis;
		}

		public function set Aneis(value:Array):void
		{
			_aneis = value;
		}

		public function get Atributos():Array
		{
			return _atributos;
		}

		public function set Atributos(value:Array):void
		{
			_atributos = value;
		}

		public function get Vertices():Array
		{
			return _vertices;
		}

		public function set Vertices(value:Array):void
		{
			_vertices = value;
		}

		public function get ObjectId():int
		{
			return _objectId;
		}

		public function set ObjectId(value:int):void
		{
			_objectId = value;
		}
		
		public function get IdProjeto():int
		{
			return _idProjeto;
		}
		
		public function set IdProjeto(value:int):void
		{
			_idProjeto = value;
		}

		public function get IdLayerFeicao():int
		{
			return _idLayerFeicao;
		}

		public function set IdLayerFeicao(value:int):void
		{
			_idLayerFeicao = value;
		}
		
	}
}