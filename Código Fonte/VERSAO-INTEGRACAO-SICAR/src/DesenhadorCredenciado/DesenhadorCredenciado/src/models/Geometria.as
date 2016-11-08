package models
{
	import com.gmaps.geom.AxisSegment;
	import com.gmaps.geom.GeomArea;
	import com.gmaps.geom.GeomLine;
	import com.gmaps.geom.GeomPoint;
	import com.gmaps.geom.GeomSegment;
	import com.gmaps.geom.Mbr;
	import com.gmaps.tools.flood.FloodEngine;
	
	import flash.geom.Point;
	
	import models.Esri.DesenhadorEsri;
	import models.Esri.GeometriaEsri;
	import models.Esri.GeometriaEsriGraphic;
	
	import mx.collections.ArrayCollection;
	import mx.controls.Alert;
	import mx.controls.List;
	
	import tm.spatialReference.Coordinate;
	import tm.spatialReference.CoordinateSystemConverter;
	import tm.spatialReference.CoordinateSystemConverterFactory;
	
	public class Geometria
	{
		public static const Poligono:int=1;
		public static const Linha:int=2;
		public static const Ponto:int=3;
		public static const MultiPontos:int=4;
		public static const MBR:int=5;
		public var idGraphic:String;
		public var idGraphicAneis:String;
		public var idGraphicTemporario:String;
		//public var idGraphicIdentificar:String;
		
		public var indicePrimeiro:Number;
		public var verticeSelecionado:Point;
		public var vertices:Vector.<Point>;
		public var aneis:Vector.<Vector.<Point>>;
		public var verticesOriginal:Vector.<Point>;
		public var aneisOriginal:Vector.<Vector.<Point>>;
		public var tipoGeometria:int;
		public var isAnelExterno:Boolean;
				
		public function Geometria(_tipoGeometria:int)
		{
			vertices = new Vector.<Point>();
			aneis = new Vector.<Vector.<Point>>();
			verticesOriginal = new Vector.<Point>();
			aneisOriginal = new Vector.<Vector.<Point>>();
			indicePrimeiro =0;
			tipoGeometria = _tipoGeometria;
			idGraphic = "-1";
			idGraphicTemporario = "-1";
			isAnelExterno = true;
		}
		
		public function get quantidade():Number
		{
			if(vertices)
			{
				return  vertices.length;
			}
			return 0;
		}
		public function converterVerticesParaArray(_geom:Vector.<Point>=null):Array
		{
			if(!_geom)
				_geom = vertices;
			
			var ar:Array;
			
			if(_geom!=null)
			{
				ar = new Array();
				for(var i:int; i<_geom.length; i++)
				{
					ar.push(_geom[i]);
				}
			}
			return ar;
		}
		public function converterVerticesParaGeomArea(_geom:Vector.<Point>=null):GeomArea
		{
			if(!_geom)
				_geom = vertices;
			
			var ar:Array;
			var geomArea:GeomArea;
			if(_geom && _geom.length>2)
			{
				ar = new Array();
				for(var i:int; i<_geom.length; i++)
				{
					ar.push(_geom[i]);
				}
				
				geomArea = new GeomArea("1",ar,null,null);				
				
			}
			return geomArea;
		}
		public function converterArrayParaVertices(ar:Array):Vector.<Point>
		{
			var _vertices:Vector.<Point>;
			if(ar!=null )
			{
				_vertices = new Vector.<Point>;
				for(var i:int; i<ar.length; i++)
				{
					_vertices.push(ar[i]);
				}	 
			}
			return _vertices;
		}
		public function converterVerticesParaGeomLine(geom:Vector.<Point>):GeomLine
		{
			if(!geom)
				geom = vertices;
			var ar:Array;
			var geomLine:GeomLine;
			if(geom!=null && geom.length>1)
			{
				ar = new Array();
				for(var i:int; i<geom.length; i++)
				{
					ar.push(geom[i]);
				}
				
				geomLine = new GeomLine("1",ar,null);				
			}
			return geomLine;
		}
		public function converterVerticesParaGeomPoints(geom:Vector.<Point>=null):Vector.<GeomPoint>
		{
			if(!geom)
				geom = vertices;
			var ar:Array;
			var lista:Vector.<GeomPoint>;
			if(geom!=null && geom.length>1)
			{
				lista =  new Vector.<GeomPoint>();
				var geomPoint:GeomPoint;
				for(var i:int; i<geom.length; i++)
				{
					geomPoint = new GeomPoint("pt", geom[i]);
					lista.push(geomPoint);
				}				
			}
			return lista;
		}
		private function verificaInterseccaoVertices(ponto:Point, range:Number, geometriaInterseccao:Vector.<Point>, somenteArestas:Boolean):Point
		{
			var verticeIntersectado:Point = null;
			if(geometriaInterseccao && geometriaInterseccao.length>0)
			{
				if(geometriaInterseccao.length >1)
				{
					var geomLine:GeomLine = converterVerticesParaGeomLine(geometriaInterseccao);			
					var ar:Array = geomLine.getVerticesOnPointRange(ponto, range);
					if(ar && ar.length>0)
						verticeIntersectado = ar[0];
				}
				else if(!somenteArestas)
				{
					var pontoInter:GeomPoint = new GeomPoint("1",geometriaInterseccao[0],null);
					if(pontoInter.interactWithPoint(ponto,range))
					{
						var pontoGeom:GeomPoint = new GeomPoint("1",ponto,null);
						verticeIntersectado = pontoInter.getNearestPoint(pontoGeom); 
					}
				}
			}
			
			return verticeIntersectado;
		}
		private function verificaInterseccaoArestas(ponto:Point, range:Number, geometriaInterseccao:Vector.<Point>, isPoligono:Boolean, ignorarArestaDesenho:Boolean=false):Array
		{
			
			var ar:Array = null;
			if(geometriaInterseccao !=null && geometriaInterseccao.length>1)
			{
				if(ignorarArestaDesenho)
				{
					if(geometriaInterseccao.length >2 && isPoligono)
					{
						var geomArea:GeomArea = converterVerticesParaGeomArea(geometriaInterseccao);	
						ar = geomArea.getSegmentsOnPointRange(ponto, range);
					}
					else
					{
						var geomLine:GeomLine = converterVerticesParaGeomLine(geometriaInterseccao);	
						ar = geomLine.getSegmentsOnPointRange(ponto, range);
					}
					
					if(geometriaInterseccao.length > indicePrimeiro)
					{
						var pt:Point = geometriaInterseccao [indicePrimeiro];
						
						if(pt)
						{
							var novoAr:Array = new Array();
							for(var k:int; k<ar.length; k++)
							{
								if(ar[k] is GeomSegment)
								{
									var segmento:GeomSegment = ar[k] as GeomSegment;
									if(segmento.segment.x2 == pt.x && segmento.segment.y2 == pt.y)
									{
										
									}
									else
										novoAr.push(ar[k]);
								}
							}
							return novoAr;
						}
					}
					
				}
				else
				{
					if(geometriaInterseccao.length >2 && isPoligono)
					{
						var geomArea:GeomArea = converterVerticesParaGeomArea(geometriaInterseccao);	
						ar = geomArea.getSegmentsOnPointRange(ponto, range);
					}
					else
					{
						var geomLine:GeomLine = converterVerticesParaGeomLine(geometriaInterseccao);	
						ar = geomLine.getSegmentsOnPointRange(ponto, range);
					}
				}
			}
			return ar;
		}
		public function verificaIntersecaoPontoNoMeioArestaMudandoIndice(ponto:Point, range:Number, geometriaInterseccao:Vector.<Point>=null, aneisInterseccao:Vector.<Vector.<Point>>=null, mudarAnel:Boolean=false, isPoligono:Boolean=false,ignorarArestaDesenho:Boolean=false):Boolean
		{
			if(!geometriaInterseccao)
				geometriaInterseccao = vertices;
			if(!aneisInterseccao)
				aneisInterseccao = aneis;
			
			var intersectou:Boolean = verificaIntersecaoPontoNoMeioArestaMudandoIndiceUnicaGeometria(ponto,range,geometriaInterseccao,isPoligono,ignorarArestaDesenho);
			
			if(intersectou)
			{
				return intersectou;
			}
			else
			{
				if(tipoGeometria == Geometria.Poligono && aneisInterseccao && aneisInterseccao.length>0 && mudarAnel) 
				{
					var anel:Vector.<Point> = null;
					for(var i:int =0; i<aneis.length; i++)
					{
						intersectou = verificaIntersecaoPontoNoMeioArestaMudandoIndiceUnicaGeometria(ponto,range,aneis[i]);
						if(intersectou)
						{
							anel = aneis[i];
							aneis.splice(i, 1);
							if(isAnelExterno)
							{
								aneis.splice(0, 0, vertices);
								isAnelExterno = false;
							}
							else
							{
								aneis.push(vertices);
								isAnelExterno = i==0;
							}
							
							vertices = anel;
							return true;
						}
					}
				}
			}
			
			
			return false;
		}
		public function verificaIntersecaoPontoNoMeioArestaMudandoIndiceUnicaGeometria(ponto:Point, range:Number, geometriaInterseccao:Vector.<Point>=null,isPoligono:Boolean=false,ignorarArestaDesenho:Boolean=false):Boolean
		{
			var arGeom:Array = verificaInterseccaoArestas(ponto,range, geometriaInterseccao,isPoligono,ignorarArestaDesenho);
			if(arGeom !=null && arGeom.length >0)
			{
				if(arGeom[0] is GeomSegment)
				{
					var segmento:GeomSegment = arGeom[0] as GeomSegment;
					
					var pontoProx:Point = segmento.getNearestPoint(new GeomPoint("1",ponto));
					if(pontoProx !=null) 
					{
						var geomLine:GeomLine = converterVerticesParaGeomLine(geometriaInterseccao);
						var verticesAr:Array =  geomLine.getVerticesOnPointRange(new Point(segmento.segment.x2, segmento.segment.y2),range);
						if(verticesAr !=null && verticesAr.length >0)
						{
							var pt:Point = verticesAr[0] as Point;
							var indice:int = geometriaInterseccao.indexOf(pt);
							if(indice >=0)
							{
								indicePrimeiro = indice;
								return true;
							}
						}
					}
				}
			}
			return false;
		}
		
		private function buscarPontoProximoNoMeioDaArestaUnicaGeometria(ponto:Point, range:Number, geometriaInterseccao:Vector.<Point>=null, isPoligono:Boolean=true, ignorarArestaDesenho:Boolean=false):Point
		{
			var arGeom:Array = verificaInterseccaoArestas(ponto,range, geometriaInterseccao,isPoligono,ignorarArestaDesenho);
			if(arGeom !=null && arGeom.length >0)
			{
				if(arGeom[0] is GeomSegment)
				{
					var segmento:GeomSegment = arGeom[0] as GeomSegment;
												
					var pontoProx:Point = segmento.getNearestPoint(new GeomPoint("1",ponto));
					return pontoProx;
				}
			}
			return null;
		}
		
		public function buscarPontoProximoNoMeioDaAresta(ponto:Point, range:Number, geometriaInterseccao:Vector.<Point>=null,  aneisInterseccao:Vector.<Vector.<Point>>=null,mudarAnel:Boolean=false, isPoligono:Boolean=true,ignorarArestaDesenho:Boolean=false):Point
		{
			//Alert.show(ignorarArestaDesenho.toString());
			
			if(!geometriaInterseccao)
				geometriaInterseccao = vertices;
			
			if(!aneisInterseccao)
				aneisInterseccao = aneis;
		
			var pontoProx:Point = buscarPontoProximoNoMeioDaArestaUnicaGeometria(ponto,range,geometriaInterseccao,isPoligono, ignorarArestaDesenho);
			
			if(pontoProx)
				return pontoProx;
					
			if(aneisInterseccao && aneisInterseccao.length>0) 
			{
				var anel:Vector.<Point> = null;
				for(var i:int =0; i<aneisInterseccao.length; i++)
				{
					pontoProx = buscarPontoProximoNoMeioDaArestaUnicaGeometria(ponto,range,aneisInterseccao[i]);
					
					if(pontoProx)
					{
						if(mudarAnel)
						{
							anel = aneis[i];
							aneis.splice(i, 1);
							if(isAnelExterno)
							{
								aneis.splice(0, 0, vertices); 
								isAnelExterno = false;
							}
							else
							{
								aneis.push(vertices);
								isAnelExterno = i ==0;
							}
							
							vertices = anel;
							
							var indice:int = vertices.indexOf(ponto);
							if(indice >=0)
									indicePrimeiro = indice;
							
						}
						return pontoProx;
					}
				}
			}
			return null;
		}
		
		public function moverAneis(distanciaX:Number, distanciaY:Number):Vector.<Vector.<Point>>
		{
			var aneisNovo:Vector.<Vector.<Point>> = new Vector.<Vector.<Point>>();
			if(aneis) 
			{
				var anelNovo:Vector.<Point>;
				var anel:Vector.<Point>;
				for(var i:int=0; i<aneis.length; i++)
				{
					anelNovo = new Vector.<Point>();
					anel = aneis[i] as Vector.<Point>
					for(var k:int=0; k<anel.length; k++)
					{
						anelNovo.push(new Point(anel[k].x+distanciaX, anel[k].y+distanciaY));
					}
					aneisNovo.push(anelNovo);
				}
			}
			return aneisNovo;
		}
		public function moverVertices(distanciaX:Number, distanciaY:Number):Vector.<Point>
		{
			var geoNova:Vector.<Point> = new Vector.<Point>();
			
			if(vertices)
			{
				for(var i:int=0; i<vertices.length; i++)
				{
					geoNova.push(new Point(vertices[i].x+distanciaX, vertices[i].y+distanciaY));
				}
			}
			return geoNova;
		}
		
		public function rotacionarAneis(pontoCentral:Point, pontoMouse:Point, pontoInicial:Point, _aneis:Vector.<Vector.<Point>>=null):Vector.<Vector.<Point>>
		{
			if(!_aneis)
				_aneis = aneis;
			
			var aneisNovo:Vector.<Vector.<Point>> = new Vector.<Vector.<Point>>();
			if(_aneis) 
			{
				var anelNovo:Vector.<Point>;
				var anel:Vector.<Point>;
				for(var i:int=0; i<_aneis.length; i++)
				{ 
					anelNovo = new Vector.<Point>();
					anel = _aneis[i] as Vector.<Point>					
					anelNovo = rotacionarVertices(pontoCentral, pontoMouse,pontoInicial, anel);
					
					if(anelNovo)
						aneisNovo.push(anelNovo);
				}
			} 
			return aneisNovo;
		}
		public function rotacionarVertices(pontoCentral:Point, pontoMouse:Point, pontoInicial:Point, _vertices:Vector.<Point>=null):Vector.<Point>
		{
			if(!_vertices)
				_vertices = vertices;
			
			var geoNova:Vector.<Point> = new Vector.<Point>();
			
			if(_vertices)
			{				
				var ptCentralGeom:GeomPoint = new GeomPoint("a", pontoCentral,null);
				var distanciaInicial:Number = ptCentralGeom.distanceFromPoint(pontoInicial);
				var grauPontoInicial:Number = calculargrau(pontoCentral.x, pontoCentral.y, pontoInicial.x, pontoInicial.y,distanciaInicial);
				var distanciaMouse:Number = ptCentralGeom.distanceFromPoint(pontoMouse);
				var grauMouse:Number = calculargrau(pontoCentral.x, pontoCentral.y, pontoMouse.x, pontoMouse.y,distanciaMouse);
				
				for(var i:int=0; i<_vertices.length; i++)
				{
					var distanciaVertice:Number = ptCentralGeom.distanceFromPoint(_vertices[i]) ;
					
					var grauVertice:Number = calculargrau(_vertices[i].x, _vertices[i].y, pontoCentral.x, pontoCentral.y, distanciaVertice);
					var novoGrau:Number = grauMouse+grauVertice-grauPontoInicial+180;					

					var Y2:Number =  (distanciaVertice)*Math.cos( novoGrau* (Math.PI / 180))+pontoCentral.y;						 
					var X2:Number = (distanciaVertice) *Math.sin(novoGrau * (Math.PI / 180))+pontoCentral.x;
					geoNova.push(new Point(X2, Y2));
				}
			}
			return geoNova;
		}
		
		public function calculargrau(x1:Number, y1:Number, x2:Number, y2:Number, distancia:Number):Number
		{
			var grau:Number = 0;
			if (x2 > x1)
			{
				if (y2 > y1)
				{
					grau = 90-(Math.asin(((y2-y1)/distancia)))/(Math.PI/180);
					
				}
				else
				{
					grau = 90;
					if (y2 == y1)
					{
						return grau;
						
					}
					grau = 180-(Math.asin(((x2 - x1) / distancia))) / (Math.PI / 180);
					
				}
			}
			else 
			{
				if (y2 > y1)
				{
					grau = ((Math.asin(((y2 - y1) / distancia))) / (Math.PI / 180));
					grau = 270+ (grau >0 ? grau : (-grau)); 
					
				}
				else
				{
					if (y2 == y1)
					{
						if (x2 == x1)
						{
							return 0;
						}
						else
						{
							return 270;
						}
					}
					else
					{
						if (x2 == x1)
						{
							if (y2 > y1)
							{
								return 0;
							}
							else
							{
								return 180;
							}
						}
					}
					grau = ((Math.asin(((x2 - x1) / distancia))) / (Math.PI / 180));
					grau = 180+(grau > 0 ? (grau) : (-grau));
				}
			}
			return grau;
		}
		
		public function adicionarPontoOrdem(ponto:Point):void
		{
			if(ponto)
			{
				vertices.push(ponto);
				indicePrimeiro = vertices.length-1;
			}
		}
		public function adicionarPonto(ponto:Point):void
		{
			if(tipoGeometria == Ponto)
			{
				vertices.pop();
				vertices.push(ponto);				
			}
			else
			{
				if(vertices)
				{
					var existePontoIgual:Boolean;
					for each(var pt:Point in vertices)
					{
						if(pt == ponto)
						{
							existePontoIgual = true;
							break;
						}
					}
					if(!existePontoIgual)
					{
						if(tipoGeometria == Geometria.Linha && vertices.length == indicePrimeiro)
						{
							vertices.push(ponto);
							indicePrimeiro++;
						}
						else
						{
							vertices.splice(indicePrimeiro,0, ponto);
						}
					}
				}
			}
		}
		
		private function removerUltimoVertice():void
		{
			if(tipoGeometria == Ponto)
			{
				vertices.pop();				
			}
			else
			{
				if(indicePrimeiro == vertices.length)
					indicePrimeiro--;
				
				vertices.splice(indicePrimeiro,1);
												
				if(indicePrimeiro+1 == vertices.length)
				{
					if(tipoGeometria != Geometria.Linha || vertices.length==1)
						indicePrimeiro = 0;
				}
				
			
				
			}
		}
		public function removerVertice(vertice:Point=null):int
		{
			var index:int;
			if(!vertice)
			{
				if(verticeSelecionado)
				{
					vertice = verticeSelecionado;
					verticeSelecionado = null;
				}
				else
				{
					removerUltimoVertice();
					return indicePrimeiro;
				}
				
			}
			if(vertice)
			{
				
				index = vertices.indexOf(vertice,0);
				if(index+1 == vertices.length)
				{
					if(tipoGeometria != Geometria.Linha || vertices.length==1)
						indicePrimeiro = 0;
					else
						indicePrimeiro = index;
				}
				else
				{
					indicePrimeiro = index;
				}
				vertices.splice(index,1);
			}
			
			return index;
		}
		
		public function buscarVerticeIntersectado(vertice:Point, range:Number, _vertices:Vector.<Point>=null, _aneis:Vector.<Vector.<Point>>=null, somenteArestas:Boolean=false, mudarAnel:Boolean=false):Point
		{
			if(!_vertices)
				_vertices = vertices;
			
			if(!_aneis)
				_aneis = aneis;
			
			var ptIn:Point =  verificaInterseccaoVertices(vertice,range, _vertices,somenteArestas);
			if(ptIn)
				return ptIn;
			else if(_aneis)
			{
				for(var i:int=0; i<_aneis.length; i++)
				{
					var anel:Vector.<Point> = _aneis[i] as Vector.<Point>;
					ptIn = verificaInterseccaoVertices(vertice,range, anel,somenteArestas);
					if(ptIn)
					{
						if(mudarAnel)
						{
							anel = aneis[i];
							aneis.splice(i, 1);
							if(isAnelExterno)
							{
								aneis.splice(0, 0, vertices);
								isAnelExterno = false;
							}
							else
							{
								aneis.push(vertices);
								isAnelExterno = i==0;
							}
							vertices = anel;						
						}
						
						return ptIn;
					}
				}
			}
			return ptIn;	
		}
		
		public function selecionarVertice(ponto:Point, range:Number, _vertices:Vector.<Point>=null, _aneis:Vector.<Vector.<Point>>=null, mudarAnel:Boolean=false):Boolean
		{
			var vertice:Point = buscarVerticeIntersectado(ponto,range,_vertices,_aneis,true,mudarAnel);
			
			if(vertice)
			{
				verticeSelecionado = vertice;
				return true;
			}
			return false;
		}
		
		
		public function desenharTemporario(ponto:Point, utilizarGeometria:Boolean=true,simbologia:int=2, desenharVertices:Boolean=true):void
		{
			if(!desenharVertices && tipoGeometria == Geometria.Ponto)
				return;
			var gem:Vector.<Point> = new Vector.<Point>();
			if(utilizarGeometria)
			{
				if(vertices && vertices.length >0 && tipoGeometria != Ponto)
				{
					if(indicePrimeiro == vertices.length)
					{
						gem.push(ponto);
						ponto = null;						
					}
				
					
					for(var i:int = vertices.length-1; i>=0; i--)
					{					
						gem.push(vertices[i]);
						if(ponto &&i==indicePrimeiro )
						{
							gem.push(ponto);
							ponto = null;
						}				
					}	
					
					//if(ponto)
						//gem.push(ponto);
					
				}
				else
				{
					gem.push(ponto);
				}
				
				idGraphic = DesenhadorEsri.getInstance().desenharFeicao(idGraphic,tipoGeometria, gem,aneis,simbologia,desenharVertices);
				if(idGraphicTemporario != "-1")
				{
					idGraphicTemporario = DesenhadorEsri.getInstance().excluirFeicao(idGraphicTemporario); 
				}
			}
			else
			{
				gem.push(ponto);
				idGraphicTemporario = DesenhadorEsri.getInstance().desenharFeicao(idGraphicTemporario,Geometria.Ponto, gem,new Vector.<Vector.<Point>>(),simbologia,desenharVertices);
			}
			
		}
		public function desenhar(_vertices:Vector.<Point>=null,_aneis:Vector.<Vector.<Point>>=null,simbologia:int=1,desenharVertices:Boolean=true):void
		{	
			if(!desenharVertices && tipoGeometria == Geometria.Ponto)
				return;
			
			if(!_aneis)
				_aneis = aneis;
			
			if(!_vertices)
				_vertices = vertices;
			
			if(_vertices && _vertices.length==0 && _aneis && _aneis.length ==0 && idGraphic != "-1")
				DesenhadorEsri.getInstance().excluirFeicao(idGraphic);
			else
				idGraphic = DesenhadorEsri.getInstance().desenharFeicao(idGraphic,tipoGeometria, _vertices, _aneis,simbologia,desenharVertices);
		}
		public function excluir(excluirVertices:Boolean=true):void
		{
			if(idGraphic != "-1")
				DesenhadorEsri.getInstance().excluirFeicao(idGraphic);
			if(idGraphicTemporario != "-1")
				DesenhadorEsri.getInstance().excluirFeicao(idGraphicTemporario);
			if(excluirVertices)
			{
				vertices = new Vector.<Point>();
				aneis = new Vector.<Vector.<Point>>();	
			}
		}
	}
}