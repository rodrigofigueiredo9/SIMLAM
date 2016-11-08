package componentes.scripts
{
	import flash.events.Event;
	
	import mx.controls.TextInput;
	
	public class TextInputMasked extends TextInput
	{
		[Bindable] public var mascara:String = "";
		[Bindable] public var tipoMascara:TipoMascara = TipoMascara.Generica;
		[Bindable] public var tamanho:int = 0;
		[Bindable] public var casasDecAposVirgula:int = 0;
		[Bindable] public var casasDecAntesVirgula:int = 0;
		
		private var strTextoAtual:String = "";
		
		public function TextInputMasked()
		{
			super();
			
			addEventListener(Event.CHANGE,changeHandler);
		}
		
		private function changeHandler(event:Event):void
		{
			var qtdAtual:int = this.text.length;
			var posAtual:int = this.selectionEndIndex;
			var diferenca:int = 0;
			
			switch(tipoMascara)
			{
				case TipoMascara.Generica:
					mascaraGenerica();
					break;
					
				case TipoMascara.Decimal:
					validarExpressaoRegular("^(([+]\\d{1," + casasDecAntesVirgula + "},\\d{0," + casasDecAposVirgula + "})|([-]\\d{1," + casasDecAntesVirgula + "},\\d{0," + casasDecAposVirgula + "})|(\\d{1," + casasDecAntesVirgula + "},\\d{0," + casasDecAposVirgula + "})|([+]\\d{0," + casasDecAntesVirgula + "})|([-]\\d{0," + casasDecAntesVirgula + "})|(\\d{0," + casasDecAntesVirgula + "}))$");
					break;
					
				case TipoMascara.AlphaNumerico:
					validarExpressaoRegular("^([^\"\"\'\'&#\\\\]{0," + this.tamanho + "})$");
					break;
					
				case TipoMascara.Inteiro:
					mascaraInteiro();
					break;
			}
			
			diferenca = this.text.length - qtdAtual;
			
			this.setSelection(posAtual + diferenca, posAtual + diferenca);
			this.strTextoAtual = this.text;
		}
		
		private function validarExpressaoRegular(strRegExp:String, flags:String = ""):void
		{
			var regExp:RegExp = new RegExp(strRegExp, flags);
			
			if(regExp.exec(this.text) == null)
			{
				this.text = this.strTextoAtual;
			}
		}
		
		private function mascaraGenerica():void
		{
			var ehValido:Boolean = false;
			var isSair:Boolean = false;
			var strTextoFinal:String = "";
			var arrayTexto:Array = this.text.split("");
			var arrayMask:Array = this.mascara.split("");
			
			var charTexto:String = "";
			var charMask:String = "";
			
			var j:int = 0;
			
			for(var i:int = 0; arrayMask.length > i; i++)
			{
				charMask = arrayMask[i].toString();
				
				while(arrayTexto.length > j)
				{
					charTexto = arrayTexto[j];
					
					switch(charMask)
					{
						case "L":
							ehValido = isLetter(charTexto);
							break;
							
						case "A":
							ehValido = isLetter(charTexto);
							
							if(ehValido)
							{
								this.text.toUpperCase();
							}
							break;
							
						case "a":
							ehValido = isLetter(charTexto);
							
							if(ehValido)
							{
								this.text.toLowerCase()();
							}
							break;				
							
						case "~":
							ehValido = (charTexto == "+" || charTexto == "-");
							
							if(!ehValido && isDigit(charTexto))
							{
								strTextoFinal = strTextoFinal + "+";
								
								isSair = true;
								j--;
							} 
							
							break;
							
						default:
						
							ehValido = false;
							
							if(isDigit(charMask))
							{
								ehValido = (isDigit(charTexto) && (charTexto >= "0" && charTexto <= charMask));
							}
							else
							{
								isSair = true;
								j--;
								strTextoFinal = strTextoFinal + charMask;
							}
							break;
					}
					
					j++;
					
					if(ehValido)
					{
						strTextoFinal = strTextoFinal + charTexto;
						isSair = true;
					}
					
					if(isSair)
					{
						isSair = false;
						break;
					}
				}
			}
			
			this.text = strTextoFinal;
		}

		private function mascaraInteiro():void
		{
			var strTextoFinal:String = "";
			var arrayTexto:Array = this.text.split("");
			var charTexto:String = "";
				
			for(var i:int = 0; i < arrayTexto.length && i < tamanho; i++)
			{
				charTexto = arrayTexto[i];
				
				if(isDigit(charTexto))
				{
					strTextoFinal = strTextoFinal + charTexto;
				}
			}
			
			this.text = strTextoFinal;
		}
		
		private function isDigit(c:String) : Boolean
		{
			return ((c >= "0" && c <= "9"));
		}
		
		private function isLetter(c:String):Boolean
		{
			return (((c >= "a") && (c <= "z")) || ((c >= "A") && (c <= "Z")));
		}
		
		public function desfazerConfiguracoes():void
		{
			mascara = "";
			tipoMascara = TipoMascara.Generica;
			tamanho = 0;
			casasDecAntesVirgula = 0;
			casasDecAposVirgula = 0;
		}
	}
}