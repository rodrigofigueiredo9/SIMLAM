package components
{
	import com.esri.ags.Graphic;
	import com.esri.ags.Map;
	import com.esri.ags.events.PanEvent;
	import com.esri.ags.events.ZoomEvent;
	import com.esri.ags.geometry.Extent;
	import com.esri.ags.geometry.MapPoint;
	import com.esri.ags.geometry.Polygon;
	import com.esri.ags.layers.GraphicsLayer;
	import com.esri.ags.symbols.SimpleFillSymbol;
	import com.esri.ags.symbols.SimpleLineSymbol;
	
	import flash.events.Event;
	import flash.geom.Point;
	import flash.geom.Rectangle;
	
	import tm.componentes.CenterLatLong;
		
	public class WorkspaceArea
	{
		private var _map:Map;
		private var layer:GraphicsLayer;
        private var largura:Number;
        private var altura:Number;  
        private var areaImpressao:Rectangle;
        
        public var layersOcultasTexto:String;
        public var isHabilitado:Boolean = false;
	
		private var _workspace: Extent;
		
		private static const MEDIDAPADRAO: Number = 400;
		
		[Bindable] 
		private var wkt1:String = 'PROJCS["SIRGAS 2000 / UTM zone 24S",GEOGCS["SIRGAS 2000",DATUM["D_SIRGAS_2000",SPHEROID["GRS_1980",6378137,298.257222101]],PRIMEM["Greenwich",0],UNIT["Degree",0.017453292519943295]],PROJECTION["Transverse_Mercator"],PARAMETER["latitude_of_origin",0],PARAMETER["central_meridian",-39],PARAMETER["scale_factor",0.9996],PARAMETER["false_easting",500000],PARAMETER["false_northing",10000000],UNIT["Meter",1]]';
		
		public function WorkspaceArea(map:Map)
		{
			this.layer = new GraphicsLayer();
			
			this._map = map;
			this._map.addLayer(this.layer);
			this.largura = MEDIDAPADRAO;
			this.altura = MEDIDAPADRAO;
		}  
		
		public function get workspace(): Extent {
			return this._workspace;
		}
		
		public function set workspace(extent: Extent): void {
			this._workspace = extent;
		}

		private function clipRectangle():Rectangle
		{
			var ratioWidth:Number;
			var ratioHeight:Number;
			var clipRect:Rectangle = new Rectangle();
			var ratio:Number;
			var mapRatio:Number;   		
			
			ratio = this.altura /this.largura;
			mapRatio = this._map.height / this._map.width;    
			ratioHeight = (this._map.height);
			ratioWidth = (this._map.width);		        	
			clipRect.x =  (this._map.width - largura)/2;
			clipRect.y =  (this._map.height - altura)/2;		
			clipRect.width = largura;
			clipRect.height = altura;

			return clipRect;
		} 
		
		public function desativar(event:Event):void{
			exibirAreaImpressao(false);
		}
		
		private function atualizarAreaImpressao(event:Event):void
		{
			exibirAreaImpressao(isHabilitado);
		}
		
		public function exibirAreaImpressao(habilitar:Boolean):void 
		{
			layer.clear();
			
			if (habilitar)
			{
				areaImpressao = obterAreaImpressao();
				
				isHabilitado = true;
			  	layer.add(desenharAreaImpressao());
			  	destacarAreaImpressao();
			  	
			  	_map.addEventListener(ZoomEvent.ZOOM_END, atualizarAreaImpressao);
				_map.addEventListener(PanEvent.PAN_END, atualizarAreaImpressao);
			}
			else
			{
				_map.removeEventListener(ZoomEvent.ZOOM_END, atualizarAreaImpressao);
				_map.removeEventListener(PanEvent.PAN_END, atualizarAreaImpressao);
				
				isHabilitado = false;
			}
		}
		
		public function desenharAreaImpressao():Graphic
		{  
			var minMapPoint:MapPoint = _map.toMap(new Point(Math.round(areaImpressao.x),Math.round(areaImpressao.y)));
			var maxMapPoint:MapPoint = _map.toMap(new Point(Math.round(areaImpressao.x + areaImpressao.width),Math.round(areaImpressao.y + areaImpressao.height)));
			
			var symbol:SimpleFillSymbol = new SimpleFillSymbol(SimpleFillSymbol.STYLE_NULL,0,0.1);
	        symbol.outline = new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, parseInt("EAEAEB"), 1.0, 3);
			
			var extent: Extent = new Extent(minMapPoint.x,minMapPoint.y,maxMapPoint.x,maxMapPoint.y);
			
			this.workspace = extent;
			
			var graphic:Graphic = new Graphic(extent); 
			graphic.symbol = symbol;
			
			layer.clear();
	        
	       	return graphic ;
		}	
			
		private function obterAreaImpressao():Rectangle
		{
			var mapCenter: CenterLatLong = new CenterLatLong();
			mapCenter.wktOrigem = wkt1;
			mapCenter.wktDestino = wkt1;
			mapCenter.showGMSFormat = true;
			mapCenter.updateCoordinateText();
			
			var center: Object = {};
			center.easting = mapCenter.EastingCoordinate;
			center.northing = mapCenter.NorthingCoordinate;

			var realWidth: Number = _map.extent.width;
			var pixelWidth: Number = _map.width;
			var deltaWith: Number = realWidth/pixelWidth;
			
			var larguraMax: Number = (10000/deltaWith);
			
			var larguraAtual: Number = MEDIDAPADRAO*deltaWith;
			
			this.largura = MEDIDAPADRAO;
			this.altura = MEDIDAPADRAO;
			
			if (larguraAtual > larguraMax) {
				this.largura = larguraMax;
				this.altura = larguraMax;
			}
			
			var areaImpressao:Rectangle = new Rectangle();
			areaImpressao.x = (this._map.width - largura)/2;
			areaImpressao.y = (this._map.height - altura)/2;		
			areaImpressao.width = this.largura;
			areaImpressao.height = this.altura;
			
			return areaImpressao;
		}        
	   
	    private function destacarAreaImpressao():void
	    {   
	        var graphic:Graphic = null;
	        var points:Array;
	        var areaExterna:Polygon = new Polygon(null, _map.spatialReference);
	        	
	        points = [
	        	_map.toMap(new Point(-5000, 5000)),
	        	_map.toMap(new Point(-5000, -(_map.height + 5000))),  
	        	_map.toMap(new Point(Math.round(areaImpressao.x), -Math.round(_map.height + 5000))),
	        	_map.toMap(new Point(Math.round(areaImpressao.x), Math.round(areaImpressao.y))),
	        	_map.toMap(new Point(Math.round(areaImpressao.x + areaImpressao.width), Math.round(areaImpressao.y))),
	        	_map.toMap(new Point(Math.round(areaImpressao.x + areaImpressao.width), Math.round(areaImpressao.y + areaImpressao.height))),
	        	_map.toMap(new Point(Math.round(areaImpressao.x), Math.round(areaImpressao.y + areaImpressao.height))),
	        	_map.toMap(new Point(Math.round(areaImpressao.x), -Math.round(_map.extent.height + 1200))),
	        	_map.toMap(new Point(Math.round(_map.extent.width + 5000), -Math.round(_map.extent.height + 5000))),
	        	_map.toMap(new Point(Math.round(_map.extent.width + 5000), 5000)),
	        	_map.toMap(new Point(-5000, 5000))]
	        
	        areaExterna.addRing(points);
	        
	        graphic = new Graphic(areaExterna);
	        graphic.symbol = new SimpleFillSymbol(SimpleFillSymbol.STYLE_SOLID, 0, 0.4);
	        
	        layer.add(graphic);
	    }
	} 
} 