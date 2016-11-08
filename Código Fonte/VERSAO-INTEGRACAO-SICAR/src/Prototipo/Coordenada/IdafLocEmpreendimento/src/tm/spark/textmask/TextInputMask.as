package tm.spark.textmask
{
	import flash.events.Event;
	import flash.events.TextEvent;
	
	import mx.controls.TextInput;
	import mx.events.FlexEvent;
	
	import spark.components.TextInput;
	import spark.events.TextOperationEvent;
	
	public class TextInputMask
	{
		private var _enabled:Boolean;
		private var _target:Object;
		private var _mask:RegExp;
		private var _autoCompleteValues:Array;
		private var _autoSwitchDotsAndComma:Boolean;
		
		private var _lastValue:String;
		private var _lastSelectionIdx:int;
		private var _validating:Boolean=false;
		
		public function TextInputMask(target:Object=null, mask:RegExp=null, autoCompleteValues:Array=null, autoSwitchDotsAndComma:Boolean=true)
		{
			this.target = target;
			this.mask = mask;
			this.autoCompleteValues = autoCompleteValues;
			this.enabled = true;
			this.autoSwitchDotsAndComma = autoSwitchDotsAndComma;
		}
		
		public function get enabled():Boolean{
			return this._enabled;
		}
		
		public function set enabled(value:Boolean):void{
			this._enabled = value;
		}
		
		public function get target():Object{
			return this._target;
		}
		
		public function set target(value:Object):void{
			
			if (this._target){
				if (this._target is spark.components.TextInput){
					this._target.removeEventListener(TextEvent.TEXT_INPUT, sparkTextInsertedAction);
					this._target.removeEventListener(Event.CHANGE, sparkTextRemovedAction);
					this._target.removeEventListener(FlexEvent.SELECTION_CHANGE, sparkSelectionChangeAction);
				}
				else if (this._target is mx.controls.TextInput){
					this._target.removeEventListener(TextEvent.TEXT_INPUT, mxTextInsertedAction);
					this._target.removeEventListener(Event.CHANGE, mxTextRemovedAction);
					this._target.removeEventListener(FlexEvent.SELECTION_CHANGE, mxSelectionChangeAction);
				}
			}

			this._target = value;

			if (this._target){
				if (this._target is spark.components.TextInput){
					this._target.addEventListener(TextEvent.TEXT_INPUT, sparkTextInsertedAction);
					this._target.addEventListener(Event.CHANGE, sparkTextRemovedAction);
					this._target.addEventListener(FlexEvent.SELECTION_CHANGE, sparkSelectionChangeAction);
				}
				else if (this._target is mx.controls.TextInput){
					this._target.addEventListener(TextEvent.TEXT_INPUT, mxTextInsertedAction);
					this._target.addEventListener(Event.CHANGE, mxTextRemovedAction);
					this._target.addEventListener(FlexEvent.SELECTION_CHANGE, mxSelectionChangeAction);
				}
				else{
					this._target = null;
				}
			}
		}
		
		public function get mask():RegExp{
			return this._mask;
		}
		
		public function set mask(value:RegExp):void{
			this._mask = value;
		}
		
		public function get autoCompleteValues():Array{
			return this._autoCompleteValues;
		}
		
		public function set autoCompleteValues(value:Array):void{
			this._autoCompleteValues = value;
		}
		
		public function get autoSwitchDotsAndComma():Boolean{
			return this._autoSwitchDotsAndComma;
		}
		
		public function set autoSwitchDotsAndComma(value:Boolean):void{
			this._autoSwitchDotsAndComma = value;
		}
		
		/*****************************************************************************
		********      SPARK Methods
		*****************************************************************************/
		
		private function sparkSelectionChangeAction(event:FlexEvent):void{
			this._lastSelectionIdx = this._target.selectionAnchorPosition;
		}
		
		private function sparkTextRemovedAction(event:Event):void{
			if (!this._enabled || !this._mask)
				return;


			if (!this._validating){
				this._validating = true;

				if (!this._mask.test(this._target.text)){
					var sizeDif:int = this._target.text.length-this._lastValue.length;
					this._target.text = _lastValue;
					this._target.selectRange(this._lastSelectionIdx - sizeDif, this._lastSelectionIdx - sizeDif);
					
					this._target.dispatchEvent( new TextOperationEvent(TextOperationEvent.CHANGE) );
				}
				else
					this._lastValue = event.target.text;
				
				this._validating= false;
			}
		}
		
		private function sparkTextInsertedAction(event:TextEvent):void{
			if (!this._enabled || !this._mask)
				return;

			event.preventDefault();
			if (!this._validating){

				this._validating = true;
				sparkValidateMask(event.text, this._target, this._autoCompleteValues, this._mask, this._autoSwitchDotsAndComma );
				
				this._lastValue = this._target.text;
				
				this._target.dispatchEvent( new TextOperationEvent(TextOperationEvent.CHANGE) );
				this._validating= false;
			}
		}
		
		private function sparkValidateMask(value:String, target:Object, autoCompleteChars:Array, maskRegExp:RegExp, trySwitchingDotsAndComma:Boolean):Boolean{
			if (!this._enabled || !this._mask)
				return true;

			value = value.replace(/[\r?\n?]/g, "");
			
			
			var selectionIni:int;
			var selectionEnd:int;
			if (target.selectionAnchorPosition>target.selectionActivePosition){
				selectionIni = target.selectionActivePosition;
				selectionEnd = target.selectionAnchorPosition;
			}
			else{
				selectionIni = target.selectionAnchorPosition;
				selectionEnd = target.selectionActivePosition;
			}
				
			var textoAnt:String = target.text.substring(0, selectionIni);
			var textoPos:String = target.text.substr(selectionEnd);
			
			var texto:String = textoAnt + value + textoPos;
			var pos:int = textoAnt.length + value.length;
			
			var valueIsValid:Boolean = false;
			
			
			//validando o texto
			if ( maskRegExp.test(texto) ){
				target.text = texto;
				target.selectRange(pos, pos);
				target.validateNow();
				valueIsValid = true;
			}
			else{
				if (autoCompleteChars!=null){
					var i:uint;
					for (i=0;i<autoCompleteChars.length; i++){
						texto = textoAnt + autoCompleteChars[i].toString() + value + textoPos;
						pos = textoAnt.length + value.length + autoCompleteChars[i].toString().length;
						if ( maskRegExp.test(texto) ){
							target.text = texto;
							target.selectRange(pos, pos);
							target.validateNow();
							valueIsValid = true;
							break;
						}			        		
					}
					
					if (!valueIsValid && value.length==1 && textoPos.length>0 ){
						texto = textoAnt + value + textoPos.substr(1);
						pos = textoAnt.length + 1;
						if ( maskRegExp.test(texto) ){
							target.text = texto;
							//target.setSelection(pos, pos);
							target.selectRange(pos, pos);
							target.validateNow();
							valueIsValid = true;
						}
						else if(textoPos.length>1){
							
							for (i=0;i<autoCompleteChars.length && !valueIsValid; i++){
								texto = textoAnt + autoCompleteChars[i].toString() + value + textoPos.substr(autoCompleteChars[i].toString().length + value.length);
								pos = textoAnt.length + value.length + autoCompleteChars[i].toString().length;
								if ( maskRegExp.test(texto) ){
									target.text = texto;
									//target.setSelection(pos, pos);
									target.selectRange(pos, pos);
									target.validateNow();
									valueIsValid = true;
								}			        		
							}
						}
					}
				}
			}
			
			if (!valueIsValid && trySwitchingDotsAndComma){
				if (value.indexOf(',')>=0)
					return sparkValidateMask(value.replace(",","."), target, autoCompleteChars, maskRegExp, false);
				else if (value.indexOf('.')>=0)
					return sparkValidateMask(value.replace(".",","), target, autoCompleteChars, maskRegExp, false);
			}
			
			return valueIsValid;
		}
		
		/*****************************************************************************
		 ********      MX Methods
		 *****************************************************************************/

		private function mxSelectionChangeAction(event:FlexEvent):void{
			this._lastSelectionIdx = this._target.selectionAnchorPosition;
		}
		
		private function mxTextRemovedAction(event:Event):void{
			if (!this._enabled || !this._mask)
				return;

			if (!this._mask.test(this._target.text)){
				this._target.text = _lastValue;
				this._target.setSelection(this._lastSelectionIdx, this._lastSelectionIdx);
			}
			else
				this._lastValue = event.target.text;
		}
		
		private function mxTextInsertedAction(event:TextEvent):void{
			if (!this._enabled || !this._mask)
				return;

			event.preventDefault();
			if (!this._validating){
				this._validating = true;
				mxValidateMask(event.text, this._target, this._autoCompleteValues, this._mask, this._autoSwitchDotsAndComma );
				
				this._lastValue = this._target.text;
				this._validating= false;
			}
		}

		private function mxValidateMask(value:String, target:Object, autoCompleteChars:Array, maskRegExp:RegExp, trySwitchingDotsAndComma:Boolean):Boolean{
			if (!this._enabled || !this._mask)
				return true;

			var textoAnt:String = target.text.substring(0, target.selectionBeginIndex);
			var textoPos:String = target.text.substr(target.selectionEndIndex);
			
			var texto:String = textoAnt + value + textoPos;
			var pos:int = textoAnt.length + value.length;
			
			var valueIsValid:Boolean = false;
			
			
			//validando o texto
			if ( maskRegExp.test(texto) ){
				target.text = texto;
				target.setSelection(pos, pos);
				target.validateNow();
				valueIsValid = true;
			}
			else{
				if (autoCompleteChars!=null){
					var i:uint
					for (i=0;i<autoCompleteChars.length && !valueIsValid; i++){
						texto = textoAnt + autoCompleteChars[i].toString() + value + textoPos;
						pos = textoAnt.length + value.length + autoCompleteChars[i].toString().length;
						if ( maskRegExp.test(texto) ){
							target.text = texto;
							target.setSelection(pos, pos);
							target.validateNow();
							valueIsValid = true;
						}			        		
					}
					
					if (!valueIsValid && value.length==1 && textoPos.length>0 ){
						texto = textoAnt + value + textoPos.substr(1);
						pos = textoAnt.length + 1;
						if ( maskRegExp.test(texto) ){
							target.text = texto;
							target.setSelection(pos, pos);
							target.validateNow();
							valueIsValid = true;
						}
						else if(textoPos.length>1){
							
							for (i=0;i<autoCompleteChars.length && !valueIsValid; i++){
								texto = textoAnt + autoCompleteChars[i].toString() + value + textoPos.substr(autoCompleteChars[i].toString().length + value.length);
								pos = textoAnt.length + value.length + autoCompleteChars[i].toString().length;
								if ( maskRegExp.test(texto) ){
									target.text = texto;
									target.setSelection(pos, pos);
									target.validateNow();
									valueIsValid = true;
								}			        		
							}
						}
					}
				}
			}
			
			if (!valueIsValid && trySwitchingDotsAndComma){
				if (value.indexOf(',')>=0)
					return mxValidateMask(value.replace(",","."), target, autoCompleteChars, maskRegExp, false);
				else if (value.indexOf('.')>=0)
					return mxValidateMask(value.replace(".",","), target, autoCompleteChars, maskRegExp, false);
			}
			
			return valueIsValid;
		}
		
		
		
		
		
		/*****************************************************************************
		 ********      CLASS Methods
		 *****************************************************************************/
		
		public static function generateMask(format:String, numberIdentifier:String = '9', alphaIdentifier:String = 'a', alphanumericIdentifier:String = '*' ):RegExp{
			var exp:String= "";
			for (var i:int=format.length; i>0;i--){
				var char:String = format.charAt(i-1);
				if (char == numberIdentifier)
					char = "[0-9]";
				else if (char == alphaIdentifier)
					char = "[A-Za-z]";
				else if (char == alphanumericIdentifier)
					char = "[0-9A-Za-z]";
				else
					if (isNaN(parseInt(char)))
						char = "\\"+ char; 
				
				exp= "("+ char+ exp +")?";
			}
			
			return new RegExp("^("+ exp +")$");
		}  
	}
	
}