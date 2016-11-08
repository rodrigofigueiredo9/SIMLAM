package com.tecnomapas.navegadores
{
	import flash.external.ExternalInterface;
	import flash.net.URLRequest;
	import flash.net.navigateToURL;
	
	public class PopupWin
	{
		public static var baseURL:String = '';
		private static var browserName:String = '';
		
		public static function openWindow(url:String, target:String = '_blank', features:String=""):void
		{
			target = "testejanelamapanavegacao";
			
			var WINDOW_OPEN_FUNCTION:String = "window.open";
			
			if (PopupWin.baseURL != '')
			{
				url = PopupWin.baseURL + url;
			}
			
			var myURL:URLRequest = new URLRequest(url);
			
			if (PopupWin.browserName == '')
			{
				PopupWin.browserName = PopupWin.getBrowserName();
			}
			
			switch (PopupWin.browserName)
			{
				case "Firefox":
					ExternalInterface.call(WINDOW_OPEN_FUNCTION, url, target, features);
				   break;
				
				case "IE":
					ExternalInterface.call("function setWMWindow() {window.open('" + url + "', '"+target+"', '"+features+"');}");
					break;
				
				case "Safari":
				case "Opera":
				default:
					navigateToURL(myURL, target);
					break;
			}
			/*Alternate methodology...
			   var popSuccess:Boolean = ExternalInterface.call(WINDOW_OPEN_FUNCTION, url, target, features);
			if(popSuccess == false){
				navigateToURL(myURL, target);
			}*/
		}
		
		private static function getBrowserName():String
		{
			var browser:String;
			
			var browserAgent:String = ExternalInterface.call("function getBrowser(){return navigator.userAgent;}");
			
			if(browserAgent != null && browserAgent.indexOf("Firefox")>= 0) 
			{
				browser = "Firefox";
			}
			else if(browserAgent != null && browserAgent.indexOf("Safari")>= 0)
			{
				browser = "Safari";
			}
			else if(browserAgent != null && browserAgent.indexOf("MSIE")>= 0)
			{
				browser = "IE";
			}
			else if(browserAgent != null && browserAgent.indexOf("Opera")>= 0)
			{
				browser = "Opera";
			}
			else 
			{
				browser = "Undefined";
			}
			
			return (browser);
		}
		
		public static function showHelp(screen:String = 'home') :void
		{
			//var features:String = "menubar=yes,status=no,toolbar=yes,location=1,scrollbars=yes,resizable=1";
			PopupWin.openWindow(screen, '_help');
		}
	}
}
