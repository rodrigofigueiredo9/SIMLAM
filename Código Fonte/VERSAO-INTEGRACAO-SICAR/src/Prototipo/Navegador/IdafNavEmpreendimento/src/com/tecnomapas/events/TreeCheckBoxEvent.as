package com.tecnomapas.events
{
	import flash.events.Event;
	
	import mx.controls.CheckBox;
	
	public class TreeCheckBoxEvent extends Event
	{
		public static var ITEM_CHANGE:String = "itemChange"; 
		
		public var itemCheckBox:CheckBox;
		public var itemDataSelected:Object;
		
		public function TreeCheckBoxEvent(itemData:Object, checkBox:CheckBox, type:String)
		{
			super(type);
			
			this.itemCheckBox = checkBox;
			this.itemDataSelected = itemData;
		}
	}
}