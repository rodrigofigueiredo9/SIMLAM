package models.Esri
{
	import com.esri.ags.Graphic;
	import com.esri.ags.geometry.Geometry;
	import com.esri.ags.symbols.SimpleFillSymbol;
	import com.esri.ags.symbols.SimpleLineSymbol;
	import com.esri.ags.symbols.SimpleMarkerSymbol;
	import com.esri.ags.symbols.Symbol;
	
	import flash.geom.Point;
	
	public class GeometriaEsriGraphic extends Graphic
	{
		public static const Desenho:int=1;
		public static const Inundar:int=2;
		public static const InundarOver:int=3;
		public static const Rotacionar:int=4;
		public static const Selecionado:int=5; //nao alterar
		public static const Selecionar:int=6; 
		public static const Snap:int=7;
		public static const SnapSelecionar:int=8;
		public static const SnapVerticeProximo:int =9;
		public static const MedirPosicaoAreaDistancia:int =10;
		
		var geometriaEsri:GeometriaEsri;
		public function GeometriaEsriGraphic(tipoSymbol:int, geometria:GeometriaEsri=null, geometry:Geometry=null, tipoGeometria:int=1):void
		{
			if(geometria)
			{
				geometriaEsri = geometria;
				DefinirSymbol(tipoSymbol, geometria.Tipo);
				super.geometry = geometriaEsri.geometria;
			}
			else
			{
				super.geometry = geometry;
				DefinirSymbol(tipoSymbol, tipoGeometria);
			}
		}
		public function AtualizarGeometria(vertices:Vector.<Point>, aneis:Vector.<Vector.<Point>>=null):void
		{			
			geometriaEsri.AtualizarGeometria(vertices, aneis);
			super.geometry = geometriaEsri.geometria;			
		}
		public function AtualizarGeometry(geometry:Geometry):void
		{	
			super.geometry = geometry;			
		}
		private function DefinirSymbol(tipoSymbol:int, tipoGeometria:int):void
		{
			var simpleLine:SimpleLineSymbol = new SimpleLineSymbol("solid",0x00FF00,1,2);
			switch(tipoGeometria)
			{	
				case 1:
					switch(tipoSymbol)
					{
						case Desenho:
							simpleLine = new SimpleLineSymbol("solid",0x0080FF,1,2);
							super.symbol = new  SimpleFillSymbol("solid",0xFFFFFF,0, simpleLine);
							break;
						case Inundar:
							simpleLine = new SimpleLineSymbol("solid",0xFF0000,1,2);
							super.symbol = new  SimpleFillSymbol("solid",0xFFFFFF,0, simpleLine);
							break;
						case InundarOver:
							simpleLine = new SimpleLineSymbol("solid",0xFF0000,1,2);
							super.symbol = new  SimpleFillSymbol("solid",0xFF0000,0.4, simpleLine);						
							break;
						case Rotacionar:
							simpleLine = new SimpleLineSymbol("solid",0xB35900,1,2);
							super.symbol = new  SimpleFillSymbol("solid",0xFFFFFF,0, simpleLine);
							break;
						case Selecionado:
							simpleLine = new SimpleLineSymbol("solid",0xFFFF00,1,2);
							super.symbol = new  SimpleFillSymbol("solid",0xFFFF00,0.4, simpleLine);
							break;
						case Selecionar:
							simpleLine = new SimpleLineSymbol("solid",0xFFFF00,1,2);
							super.symbol = new SimpleFillSymbol("diagonalcross",0xFFFF00,0.7, simpleLine);
							break;
						case Snap:
							simpleLine = new SimpleLineSymbol("solid",0x66CC00,1,2);
							super.symbol = new  SimpleFillSymbol("solid",0xFF0000,0, simpleLine);
							break;
						case SnapSelecionar:
							simpleLine = new SimpleLineSymbol("solid",0x66CC00,1,2);
							super.symbol = new SimpleFillSymbol("diagonalcross",0xFF0000,0.7, simpleLine);
							break;
						case MedirPosicaoAreaDistancia:
							simpleLine = new SimpleLineSymbol("solid",0xFF0000,1,2);
							super.symbol = new  SimpleFillSymbol("solid",0xFF0000,0.4, simpleLine);	
							break;
					}
					break
				case 2:
					switch(tipoSymbol)
					{
						case Desenho:
							super.symbol = new SimpleLineSymbol("solid",0x0080FF,1,2);
							break;
						case Rotacionar:
							super.symbol = new SimpleLineSymbol("solid",0xB35900,1,2);
							break;
						case Selecionado:
							super.symbol = new SimpleLineSymbol("solid",0xFFFF00,1,2);
							break;
						case Selecionar:
							super.symbol =new SimpleLineSymbol("solid",0xFFFF00,1,2);						
							break;
						case Snap:
						case SnapSelecionar:
							super.symbol = new SimpleLineSymbol("solid",0x66CC00,1,2);
							break;
						case MedirPosicaoAreaDistancia:
							super.symbol = new SimpleLineSymbol("solid",0xFF0000,1,2);
							break;
					}
					break;
				case 3:
				case 4:
					switch(tipoSymbol)
					{
						case Desenho:
							simpleLine = new SimpleLineSymbol("solid",0x0080FF,1,2);
							super.symbol = new  SimpleMarkerSymbol("square",10,0xFFFFFF,0.7,0,0,0,simpleLine);
							break;
						case Rotacionar:
							simpleLine = new SimpleLineSymbol("solid",0xB35900,1,2);
							super.symbol = new  SimpleMarkerSymbol("square",10,0xFFFFFF,0.7,0,0,0,simpleLine);
							break;
						case Selecionado: 
							simpleLine = new SimpleLineSymbol("solid",0xFFFF00,1,2);
							super.symbol = new  SimpleMarkerSymbol("square",10,0xFFFFFF,0.7,0,0,0,simpleLine);
							break;
						case Selecionar:
							simpleLine = new SimpleLineSymbol("solid",0xFFFF00,1,2);
							super.symbol = new  SimpleMarkerSymbol("square",10,0xFFFF00,0.7,0,0,0,simpleLine);
							break;
						case Snap:
						case SnapSelecionar:
							simpleLine = new SimpleLineSymbol("solid",0x66CC00,1,2);
							super.symbol = new SimpleMarkerSymbol("square",10,0xFF0000,0.7,0,0,0,simpleLine);
							break;
						case SnapVerticeProximo:
							simpleLine = new SimpleLineSymbol("solid",0x000000,1,2);
							super.symbol = new SimpleMarkerSymbol("circle",20,0xFF0000,0.2,0,0,0,simpleLine);
							break;
						case MedirPosicaoAreaDistancia:
							simpleLine = new SimpleLineSymbol("solid",0xFF0000,1,2);
							super.symbol = new  SimpleMarkerSymbol("square",10,0xFF0000,0.7,0,0,0,simpleLine);
							break;
					}
					break;			
			}	
		}
	}
}