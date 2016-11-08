package com.tecnomapas.Utilitarios
{
	public class UtilNumber
	{
		public static function formatDecimals(number:Number, digits:int):Number
		{ 
			if (digits <= 0) 
			{ 
				return Math.round(number); 
			} 
			
			var tenToPower:Number = Math.pow(10, digits);
			 
			var cropped:String = String(Math.round(number * tenToPower) / tenToPower); 
			
			if (cropped.indexOf(".") == -1) 
			{ 
				cropped += ".0"; 
			} 
			
			var halves:Array = cropped.split("."); 
			 
			var zerosNeeded:int = digits - halves[1].length; 
			
			for (var i:int = 1; i <= zerosNeeded; i++) 
			{ 
				cropped += "0"; 
			} 
			
			return Number(cropped); 
		}
	}
}