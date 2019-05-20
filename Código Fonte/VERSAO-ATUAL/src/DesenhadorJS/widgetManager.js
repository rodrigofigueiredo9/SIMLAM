function callWidgetFunction(widgetName, functionName, params)
{
	var widget = getWidgetByName(widgetName);
	var functionReturn = widget[functionName](params);	
	return functionReturn;
}

function getWidgetByName(widgetName)
{
	var widget;
	$.each(this._widgetManager.loaded, function(i) {
	
		if(widgetName == this.widgetManager.loaded[i].name)
		{
			widget = this.widgetManager.loaded[i];
			return false; //break
		}
	});
	return widget;
}