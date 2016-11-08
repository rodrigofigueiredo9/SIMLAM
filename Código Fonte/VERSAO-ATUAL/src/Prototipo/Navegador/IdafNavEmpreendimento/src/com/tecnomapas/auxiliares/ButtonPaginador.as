package com.tecnomapas.auxiliares 
{
	import flash.events.MouseEvent;
	
	import mx.controls.Button;
	
	import com.tecnomapas.events.PaginadorExibirPaginaEvent;

	[Event(name="exibirPagina", type="com.tecnomapas.events.PaginadorExibirPaginaEvent")]
	public class ButtonPaginador extends Button 
	{

		[Bindable] private var _pagina:int;
		[Bindable] private var _intervaloInicial:int;
		[Bindable] private var _intervaloFinal:int;
		
		public function ButtonPaginador() 
		{
			super();
		}
		
		override protected function childrenCreated():void
		{
			super.childrenCreated();
			
			this.buttonMode = true;
			this.useHandCursor = true;
			
			if(this.label == '')
			{
				this.label = String(_pagina);
			}
		}
		
		public function set pagina(value:int):void 
		{
			_pagina = value;
		}

		public function get pagina():int 
		{
			return _pagina;
		}

		public function set intervaloInicial(value:int):void 
		{
			_intervaloInicial = value;
		}

		public function get intervaloInicial():int 
		{
			return _intervaloInicial;
		}
		
		public function set intervaloFinal(value:int):void 
		{
			_intervaloFinal = value;
		}

		public function get intervaloFinal():int 
		{
			return _intervaloFinal;
		}
		
		override protected function clickHandler(event:MouseEvent):void 
		{
			super.clickHandler(event);
			
			if(this.selected)
			{
				this.dispatchEvent(new PaginadorExibirPaginaEvent(PaginadorExibirPaginaEvent.EXIBIR_PAGINA, this));
			} 
		}
	}
}