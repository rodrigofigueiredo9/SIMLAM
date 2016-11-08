package com.tecnomapas.renderers
{
	import flash.events.Event;
	
	import mx.controls.treeClasses.TreeItemRenderer;
	import mx.controls.treeClasses.TreeListData;
	
	import com.tecnomapas.auxiliares.CheckBoxIndeterminate;
	import com.tecnomapas.events.TreeCheckBoxEvent;
	
	[Event(name=TreeCheckBoxEvent.ITEM_CHANGE, type="tecnomapas.events.TreeCheckBoxEvent")]
	
	public class CheckIndeterminateTreeItemRenderer extends TreeItemRenderer
	{	
		public var check:CheckBoxIndeterminate;
		
		private const SPACE:Number = 17;
		private var selected:Boolean = false;
		private var dataChild:Object = null;
		
        public function CheckIndeterminateTreeItemRenderer()
        {
            super();
        }
        
        /**
        * Método sobrescrito da classe mx.controls.treeClasses.TreeItemRenderer
        * para retirar os ícones padrões do Tree e setar a proprieade data.
        * 
        * Esse método é chamado pela API de itemRenderer do Flex
        * 
        * @value - Object
        * */
        
        override public function set data(value:Object):void 
        {
            super.data = value;
        }
        
        private function onDispatchChange(event:Event = null):void
		{
			dispatchEvent(new TreeCheckBoxEvent(data, check, TreeCheckBoxEvent.ITEM_CHANGE));
		} 
		
		private function onDispatchChildChange(event:Event = null):void
		{
			dispatchEvent(new TreeCheckBoxEvent(dataChild, check, TreeCheckBoxEvent.ITEM_CHANGE));
		} 
     
       /**
        * Método sobrescrito da classe mx.controls.treeClasses.TreeItemRenderer
        * para adicionar a criação do CheckBoxIndeterminate
        * 
        * */
     
       override protected function updateDisplayList(unscaledWidth:Number,unscaledHeight:Number):void 
       {
            super.updateDisplayList(unscaledWidth, unscaledHeight);
            
            if(super.data) 
            {
                super.label.text = TreeListData(super.listData).label;
                
                var node:XMLList = new XMLList(TreeListData(super.listData).item);
                
                var selected:Boolean = (node[0].@selected == "true");
                var indeterminate:Boolean = (node[0].@indeterminate == "true");
                
                this.check.addEventListener(Event.CHANGE, onDispatchChange);
                this.check.indeterminate = indeterminate;
                this.check.selected = selected;
                
                this.toolTip = node[0].@toolTip;
                this.check.y = 9;
                this.check.x = super.icon.x + 2;
     
                super.label.x = this.check.x + SPACE;           
                super.icon.visible = false;
            }
        }
        
        /**
        * Método sobrescrito da classe mx.controls.treeClasses.TreeItemRenderer
        * para adicionar a criação do CheckBoxIndeterminate
        * 
        * */
        
        override protected function createChildren():void
        {
        	super.createChildren();	
        	
        	check = new CheckBoxIndeterminate();
        	check.addEventListener("click", onCheckBoxClick);
        	addChild(check);
        }
        
        private function onCheckBoxClick(event:Event):void
        {
        	var node:XMLList = new XMLList(TreeListData(super.listData).item);

            node[0].@selectedAnterior = node[0].@selected;
            node[0].@selected = String(event.currentTarget.selected);
            
            if (node.children().length())
            {
	            node[0].@indeterminate = String(event.currentTarget.selected);
            }
            
            recursiveCheck(node.children(), event.currentTarget.selected);
            recursiveIndeterminate(XMLList(node.parent()));
        }
        
         /**
        * Método seleciona e desseleciona os nos de forma recursiva
        * 
        * @param none - XMLList
        * @param selected - Boolen
        * */
        
        private function recursiveCheck(node:XMLList, selected:Boolean):void
        {
        	for each (var prop:XML in node) 
        	{
        		prop.@selectedAnterior = prop.@selected;
        		prop.@selected = selected;
        		
        		if (prop.children().length())
        		{
        			prop.@indeterminate = selected;
        		}
        		else if(prop.@selectedAnterior != prop.@selected)
	            {
	            	dataChild = prop;
	            	onDispatchChildChange();
	            }
        		
        		recursiveCheck(prop.children(), selected);
        	}
        }
        
        /**
        * Método que seta se os nos pais precisam estar com o estado indeterminate
        * 
        * @param none - XMLList
        * */
        
        private function recursiveIndeterminate(node:XMLList):void
        {
        	var obj:Object;
        	
        	if (node.children().length())
        	{
        		obj = calculateNodes(node);
        	
	        	if (node.children().length() == obj.trueCount)
	        	{
	        		node.@selected = true;
	        		node.@indeterminate = false;
	        	}
	        	else if (node.children().length() == obj.falseCount)
	        	{
	        		node.@selected = false;
	        		
	        		if (obj.indeterminateCount)
	        		{
	        			node.@indeterminate = true;
	        		}
	        		else
	        		{
	        			node.@indeterminate = false;
	        		}
	        	}
	        	else
	        	{
	        		node.@selected = false;
	        		node.@indeterminate = true;
	        	}
	        	
        		recursiveIndeterminate(XMLList(node.parent()));
        	}
        }
        
        /**
        * Método que calcula  o numero de nos filhos true, false de indeterminate 
        * de um determinda no
        * 
        * @param none - XMLList
        * */
        
        private function calculateNodes(node:XMLList):Object
        {
        	var obj:Object = new Object();
        	obj.falseCount = 0;
        	obj.trueCount = 0;
        	obj.indeterminateCount = 0;
        	
        	for each (var prop:XML in node.children()) 
        	{
        		if (prop.@selected == "false" || prop.@selected == undefined)
        		{
        			obj.falseCount = obj.falseCount + 1;
        		}
        		else
        		{
        			obj.trueCount = obj.trueCount + 1;
        		}
        		
        		if (prop.@indeterminate == "true")
        		{
        			obj.indeterminateCount = obj.indeterminateCount + 1;
        		}
        	}
        	
        	return obj;
        }
        
        /**
        * Método que seta se os nos pais precisam estar com o estado indeterminate
        * 
        * @param none - XMLList
        * */
        
        private function recursiveIndeterminateDown(node:XMLList):void
        {
        	for each (var prop:XML in node) 
        	{
        		prop.@selected = selected;

	        	var obj:Object;
	        	
	        	//pode ser um no que nao tem pai, como o 1o
	        	if (prop.children().length())
	        	{
	        		obj = calculateNodes(node);
	        	
	        		//condicoes para setar o indeterminate
		        	if (prop.children().length() == obj.trueCount)
		        	{
		        		prop.@selected = true;
		        		prop.@indeterminate = false;
		        	}
		        	else if (prop.children().length() == obj.falseCount)
		        	{
		        		prop.@selected = false;
		        		//se nao tiver nenhum indeterminate é pa não existe nenhum
		        		//nó abaixo checado
		        		if (obj.indeterminateCount)
		        		{
		        			prop.@indeterminate = true;
		        		}else
		        		{
		        			prop.@indeterminate = false;
		        		}
		        	}
		        	else
		        	{
		        		prop.@selected = false;
		        		prop.@indeterminate = true;
		        	}
		        	
	        		recursiveIndeterminate(prop.children());
	        	}
	        }
        }
    }
}