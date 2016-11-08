package com.tecnomapas.scripts
{
	
   import flash.events.MouseEvent;
   
   import mx.containers.Panel;
   import mx.controls.Image;
   import mx.core.IFlexDisplayObject;
   import mx.effects.Move;
   
	public class PanelMinMax extends Panel
	{
        private var imgMaxMin:Image;
        private var imgFechar:Image;
        private var isPopWindow:IFlexDisplayObject;
       
        [Embed("assets/img/minimizar_1.png")]
        [Bindable] private var minimizarClass:Class;

        [Embed("assets/img/maximizar_1.png")]
        [Bindable] public var maximizarClass:Class;
        
        [Embed("assets/img/fechar_1.png")]
        [Bindable] public var fechar:Class;
        
        [Bindable] public var isExibirMaxmizar:Boolean = true;
        [Bindable] public var isExibirFechar:Boolean = false;
        
        public var setNovo:Boolean = true;
        public var heightDefault:Number;
        public var widthDefault:Number;
        public var isMaxMin:Boolean = false;
        
   		public function PanelMinMax() 
   		{
			super();
            super.isPopUp = false;
		}

 		public override function set height(value:Number):void 
 		{ 
        	super.height = value;
        	
        	if(!isMaxMin)
        	{
        		this.heightDefault = value;
        	}
    	}	 

  		protected override function createChildren(): void
  		{
       		super.createChildren();
       		
       		if(isExibirFechar)
        	{
	    	 	imgFechar = new Image();
	       
	     		imgFechar.source = fechar;
	       		imgFechar.width = 30;
	       		imgFechar.height = 20;
	       		imgFechar.addEventListener(MouseEvent.CLICK, fecharPanel);
	        	rawChildren.addChild(imgFechar);
        	}
       		
       		if(isExibirMaxmizar)
       		{
	       		imgMaxMin = new Image();
	       
	     		imgMaxMin.source = maximizarClass;
	       		imgMaxMin.width = 30;
	       		imgMaxMin.height = 20;
	       		imgMaxMin.addEventListener(MouseEvent.CLICK, onEventoJanela);
	        	rawChildren.addChild(imgMaxMin);
        	}
        	
        	this.widthDefault = super.width;
            this.heightDefault = super.height;
        	
        	verificarAcaoJanela();
    	}
    	
    	public function onEventoJanela(event:MouseEvent):void
    	{
    		verificarAcaoJanela();
    	}
    	
    	public function verificarAcaoJanela():void
    	{
    		if(imgMaxMin == null)
    		{
    			return;
    		}
    		
    		if(this.height != heightDefault)
        	{ 
        		maximizar();
        	}
        	else
        	{
        		minimizar();
        	}
    	}
   
  		public function minimizar():void
  		{
  			this.isMaxMin = true;
  			
  			imgMaxMin.source = maximizarClass;
    		imgMaxMin.toolTip = "Maximizar";

			this.height = 40;
			
			this.isMaxMin = false;
  		}
 		
 		public function maximizar():void
  		{
  			this.isMaxMin = true;
  			
         	imgMaxMin.source = minimizarClass;
            imgMaxMin.toolTip = "Minimizar";
            
            var effectAumenta:Move = new Move(this);
            
            effectAumenta.duration = 20;
            effectAumenta.xTo = 10;
            effectAumenta.yTo = 80;
            effectAumenta.play();
            
            this.height = heightDefault;
            
            this.isMaxMin = false;
 		}
    
    	public function fecharPanel(event:MouseEvent):void
    	{
       		this.visible = false;
    	}
 
    	private function layoutBotoes():void
    	{
    		var aux:int = 0;
    		
       		if(imgMaxMin != null)
       		{
       			aux = aux + 22;
           		imgMaxMin.move(titleBar.width - aux, (titleBar.height - 20) / 2);
        	}
        	
        	if(imgFechar != null)
        	{
        		aux = aux + 22;
        		imgFechar.move(titleBar.width - aux, (titleBar.height - 20) / 2);
        	}
    	}
 
    	override protected function layoutChrome(unscaledWidth:Number, unscaledHeight:Number):void
    	{
        	super.layoutChrome(unscaledWidth, unscaledHeight);
        	layoutBotoes();
    	}
	}
}
