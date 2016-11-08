package models.Esri
{	
	import com.esri.ags.geometry.Geometry;
	import com.esri.ags.geometry.MapPoint;
	import com.esri.ags.geometry.Multipoint;
	import com.esri.ags.geometry.Polygon;
	import com.esri.ags.geometry.Polyline;
	import com.esri.ags.virtualearth.VEAddress;
	
	import flash.geom.Point;
	
	public class GeometriaEsri 
	{
		public var Tipo:int;
		public var Vertices:Array;
		public var geometria:Geometry;
		public var Aneis:Vector.<Vector.<Point>>;
		public function GeometriaEsri(tipo:int, vertices:Vector.<Point>, aneis:Vector.<Vector.<Point>>=null)
		{
			switch(tipo)
			{
				case 1:
					geometria = new Polygon();
					break;
				case 2:
					geometria = new Polyline();
					break;
				case 3:
					geometria = new MapPoint(0,0);
					break;
				case 4:
					geometria = new Multipoint();
					break;
			}
			Tipo = tipo;	
			Aneis = aneis;
			AtualizarGeometria(vertices, aneis);
		}
		public function AtualizarGeometria(listaVertice:Vector.<Point>, aneis:Vector.<Vector.<Point>>=null):void
		{
			var novoAnel:Array;
			var lista:Array = new Array();
			
			if(!aneis)
				aneis= Aneis;
			
			var rings:Array = new Array();
			Vertices = new Array();
			
			if(listaVertice !=null && listaVertice.length >0)
			{
				for(var n:int=0; n<listaVertice.length; n++)
				{
					 Vertices.push(new MapPoint(listaVertice[n].x, listaVertice[n].y)); 
					 lista.push(new MapPoint(listaVertice[n].x, listaVertice[n].y)); 
				}
				rings.push(Vertices);
			} 
			
			if(aneis && aneis.length>0)
			{
				var anel:Vector.<Point>;
				for(var n:int=0; n<aneis.length; n++)
				{
					novoAnel = new Array();
					anel = aneis[n] as Vector.<Point>;
					if(anel)
					{
						for(var i:int=0; i<anel.length; i++)
						{
							novoAnel.push(new MapPoint(anel[i].x, anel[i].y)); 
							lista.push(new MapPoint(anel[i].x, anel[i].y)); 
							//Vertices.push(new MapPoint(lista[i].x, lista[i].y)); 
						}
						
						rings.push(novoAnel);
					}
				}
			}
				
			switch(Tipo)
			{
				case 1:
					(geometria as Polygon).rings = rings;
					break;
				case 2:
					(geometria as Polyline).paths = [Vertices];
					break;
				case 3:
					if(listaVertice && listaVertice.length>0)
					{
						(geometria as MapPoint).x = listaVertice[0].x;
						(geometria as MapPoint).y = listaVertice[0].y;
					}
					else
					{
						(geometria as MapPoint).x = 0;
						(geometria as MapPoint).y =0;
					}
					break;
				case 4:
					(geometria as Multipoint).points =lista;
					break;
				
			}
		}
	}
}