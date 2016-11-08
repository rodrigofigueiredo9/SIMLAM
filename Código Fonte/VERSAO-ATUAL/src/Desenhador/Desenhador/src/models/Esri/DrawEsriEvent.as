package models.Esri
{
	import com.esri.ags.geometry.Geometry;
	
	import flash.events.Event;
	
	import models.Geometria;

	public class DrawEsriEvent extends Event
	{
		public static const DRAW_END:String = "DrawEndControllerEvent";
		public var geometry:Geometry;
		public var geometria:Geometria;
		public function DrawEsriEvent(type:String, _geometry:Geometry, _geometria:Geometria, bubbles:Boolean=false, cancelable:Boolean=false)
		{
			geometry = _geometry;
			geometria = _geometria;
			super(type, bubbles, cancelable); 
		}
	}
}