import flash.events.MouseEvent;

import mx.containers.Canvas;
import mx.core.Application;
 
private var selectionRectangle:Canvas;
private var rectangle:Rectangle;

private var x1:Number = 0;
private var x2:Number = 0;
private var y1:Number = 0;
private var y2:Number = 0;

private function isEnableSelectionRectangle():Boolean
{
	return (selectionRectangle != null);
}

private function enableSelectionRectangle(listener:Function):void 
{ 
	mapa.addEventListener(MouseEvent.MOUSE_DOWN, onMouseDown); 
	mapa.addEventListener(MouseEvent.MOUSE_UP, onMouseUp);
	mapa.addEventListener(MouseEvent.MOUSE_UP, listener);  
	mapa.addEventListener(MouseEvent.MOUSE_MOVE, onMouseMove); 
	 
	selectionRectangle = new Canvas(); 
	selectionRectangle.setStyle("styleName", "selectionRectangleStyle"); 
	 
	rectangle = new Rectangle(0,0,0,0); 
}

private function disableSelectionRectangle(listener:Function):void 
{ 
	mapa.removeEventListener(MouseEvent.MOUSE_DOWN, onMouseDown); 
	mapa.removeEventListener(MouseEvent.MOUSE_UP, onMouseUp);
	mapa.removeEventListener(MouseEvent.MOUSE_UP, listener);
	mapa.removeEventListener(MouseEvent.MOUSE_MOVE, onMouseMove);   
	 
	selectionRectangle = null; 
	rectangle = null; 
}
 
private function onMouseDown(e:MouseEvent):void 
{ 
	x1 = e.stageX - ((Application.application.width - mapa.width) / 2);
	y1 = e.stageY - ((Application.application.height - mapa.height) / 2);
	
	rectangle = new Rectangle(Number(x1),Number(y1),0,0); 
	 
	selectionRectangle.x = rectangle.x; 
	selectionRectangle.y = rectangle.y; 
	selectionRectangle.width = rectangle.width; 
	selectionRectangle.height = rectangle.height; 
	 
	mapa.addChild(selectionRectangle); 
}
 
private function onMouseUp(e:MouseEvent):void 
{  
	if(mapa.contains(selectionRectangle))
	{
		mapa.removeChild(selectionRectangle); 
	}
}
 
private function onMouseMove(e:MouseEvent):void 
{ 
	x2 = e.stageX - ((Application.application.width - mapa.width) / 2);
	y2 = e.stageY - ((Application.application.height - mapa.height) / 2);
	
	rectangle.bottomRight = new Point(x2, y2); 
	 
	selectionRectangle.x = rectangle.x; 
	selectionRectangle.y = rectangle.y; 
	selectionRectangle.width = rectangle.width; 
	selectionRectangle.height = rectangle.height; 
}