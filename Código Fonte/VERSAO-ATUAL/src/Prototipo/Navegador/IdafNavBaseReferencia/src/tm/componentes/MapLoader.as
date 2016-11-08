package tm.componentes
{
	import com.esri.ags.Map;
	import com.esri.ags.events.LayerEvent;
	import com.esri.ags.layers.ArcGISDynamicMapServiceLayer;
	import com.esri.ags.layers.ArcGISTiledMapServiceLayer;
	import com.esri.ags.layers.Layer;
	
	import flash.display.Sprite;
	import flash.events.Event;
	import flash.utils.setTimeout;
	
	import mx.rpc.soap.LoadEvent;
	
	public class MapLoader extends Sprite
	{
		public static const TILEDMAPSERVICELAYER:String = "arcgis_tiled_map_service_layer";
		public static const DYNAMICMAPSERVICELAYER:String = "arcgis_dynamic_map_service_layer";

		private var _map:Map;
		private var _layers:Array;
		private var _initialLayer:Layer;
		

		public function MapLoader(map:Map)
		{
			_map = map;
			_layers = new Array();
		}

		public function get layers():Array{
			return _layers;
		}

		public function addLayer(type:String, url:String, proxyURL:String=null, token:String=null, id:String=null):void{
			_layers.push({type:type, url:url, proxyURL:proxyURL, token:token, id:id});
		}

		public function removeLayer():Object{
			return _layers.pop();
		}
		
		
		public function load():void{
			loadInitialLayer();
		}
		
		private function createLayer(type:String, url:String, proxyURL:String=null, token:String=null, id:String=null):Layer{
			var layer:Layer;
			if (type==TILEDMAPSERVICELAYER)
				layer = new ArcGISTiledMapServiceLayer(url,proxyURL,token);
			else
				layer = new ArcGISDynamicMapServiceLayer(url,proxyURL,token);

			layer.id = id;

			return layer;
		}

		private function loadInitialLayer(info:Object=null):void{
			_initialLayer = createLayer(_layers[0].type, _layers[0].url, _layers[0].proxyURL, _layers[0].token, _layers[0].id);
			_initialLayer.addEventListener(LayerEvent.LOAD, loadLayers);
			_initialLayer.addEventListener(LayerEvent.LOAD_ERROR, reloadInitialLayer);
		}

		private function loadLayers(event:Event):void {
			_map.addLayer(_initialLayer);

			for (var i:uint=1;i<_layers.length;i++){
				_map.addLayer(createLayer(_layers[i].type, _layers[i].url, _layers[i].proxyURL, _layers[i].token, _layers[i].id),i);
			}
			
			var loadEvent:LoadEvent = new LoadEvent(LoadEvent.LOAD);
			dispatchEvent(loadEvent);
			//new EventDispatcher().dispatchEvent(loadEvent);
		}

		private function reloadInitialLayer(event:Event):void {
			setTimeout(loadInitialLayer,1000);
		}
	}
}