package tm.spark
{
	import flash.events.MouseEvent;
	
	import mx.events.FlexEvent;
	
	import spark.components.ComboBox;
	import spark.events.IndexChangeEvent;
	
	public class ComboBoxExt extends ComboBox
	{
		private var _prompt:String = "";		
		private var _editable:Boolean = false;
		
		public function ComboBoxExt()
		{
			super();
			
			mouseChildren = false;

			addEventListener(FlexEvent.VALUE_COMMIT, valueCommitHandler);
			addEventListener(IndexChangeEvent.CHANGE, changeHandler);
			addEventListener(IndexChangeEvent.CHANGING, changingHandler);
			
			addEventListener(MouseEvent.ROLL_OVER, rolloverHandler);
			addEventListener(MouseEvent.ROLL_OUT, rolloutHandler);
			addEventListener(MouseEvent.MOUSE_DOWN, mousedownHandler);
		}
		
		protected override function partAdded(partName:String, instance:Object):void{
			super.partAdded(partName, instance);
			
			if (instance == textInput)
			{
				this.textInput.editable = this._editable;
				this.textInput.addEventListener(FlexEvent.SELECTION_CHANGE, deselect);
				
				updateLabel(false);
			}
		}

		
		private function deselect(event:FlexEvent):void{
			if (this.textInput.selectionActivePosition != this.textInput.selectionAnchorPosition){
				this.textInput.removeEventListener(FlexEvent.SELECTION_CHANGE, deselect);
				this.textInput.selectRange(-1, -1);
				this.textInput.addEventListener(FlexEvent.SELECTION_CHANGE, deselect);
			}
		}


		private function rolloverHandler(event:MouseEvent):void{
			this.openButton.dispatchEvent(event);
		}
		
		private function rolloutHandler(event:MouseEvent):void{
			this.openButton.dispatchEvent(event);
		}
		
		private function mousedownHandler(event:MouseEvent):void{
			this.openButton.dispatchEvent( new MouseEvent(MouseEvent.MOUSE_DOWN,false,true) );
		}


		protected function changingHandler(event:IndexChangeEvent):void{
			if (event.newIndex<0)
				event.preventDefault();
		}
		
		protected function changeHandler(event:IndexChangeEvent):void{
			updateLabel();
		}
		
		protected function valueCommitHandler(event:FlexEvent):void{
			updateLabel();
		}
		
		protected function updateLabel(calllater:Boolean=true):void{
			if (calllater)
				callLater(updateLabel,[false]);
			else{
				if (this.selectedIndex<0)
					this.textInput.text = this._prompt;
			}
		}
		
		public function get editable():Boolean{
			return this._editable;
		}
		
		public function set editable(value:Boolean):void{
			this._editable = value;
			
			if (initialized)			
				this.textInput.editable = value;
		}

		override public function get prompt():String{
			return this._prompt;
		}
		
		override public function set prompt(value:String):void{
			this._prompt = value;
			
			if ( initialized && this.selectedIndex<0)
				this.textInput.text = value;
		}

	}
}