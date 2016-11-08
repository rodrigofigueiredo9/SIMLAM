package com.tecnomapas.scripts
{
	public class TipoMascara
	{
		 public var _value:int;    
		 
		 public function TipoMascara(val:int):void
		 {                    
		 	this._value = val;    
		 }    
		 
		 public static const Generica:TipoMascara = new TipoMascara(1);    
		 public static const Decimal:TipoMascara = new TipoMascara(2);
		 public static const Inteiro:TipoMascara = new TipoMascara(3); 
		 public static const AlphaNumerico:TipoMascara = new TipoMascara(4);
	}
}