package com.tm.spark.buttonext
{
	import mx.events.FlexEvent;
	
	import spark.components.Button;

	[Style(name="icon",type="*")]
	public class ButtonExt extends Button
	{
		
		[Bindable]
		public var padding:int = 5;
		[Bindable] 
		public var labelStyleName:String;
		
		private var _horizontalAlign:String;
		private var _verticalAlign:String = "middle";
		
		
		public function ButtonExt()
		{
			super();
			this.addEventListener(FlexEvent.CREATION_COMPLETE, onComplete);
			this.buttonMode = true;
		}
		
		public function onComplete(event:FlexEvent):void
		{
			this.setStyle("skinClass", Class(ButtonExtSkin));
		}
		


		
		
		//----------------------------------
		//  horizontalAlign
		//----------------------------------		
		[Inspectable(category="General", enumeration="left,right,center", defaultValue="left")]
		[Bindable]
		public function get horizontalAlign():String
		{
			return this._horizontalAlign;
		}

		public function set horizontalAlign(value:String):void
		{
			this._horizontalAlign = value;
		}
		
		//----------------------------------
		//  verticalAlign
		//----------------------------------
		
		[Inspectable(category="General", enumeration="top,bottom,middle,justify,contentJustify", defaultValue="middle")]
		[Bindable]
		public function get verticalAlign():String
		{
			return this._verticalAlign;
		}

		public function set verticalAlign(value:String):void
		{
			this._verticalAlign = value;
		}
		
	}
}