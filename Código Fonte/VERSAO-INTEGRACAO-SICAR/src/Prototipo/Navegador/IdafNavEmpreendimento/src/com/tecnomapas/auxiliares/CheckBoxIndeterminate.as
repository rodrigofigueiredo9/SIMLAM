package com.tecnomapas.auxiliares
{
	import mx.controls.CheckBox;
	
	public class CheckBoxIndeterminate extends CheckBox
	{
		private var _indeterminate:Boolean = false;
		
		public function CheckBoxIndeterminate():void
		{
			setIndeterminate();
		}
		
		public function set indeterminate(b:Boolean):void
		{
			this._indeterminate = b;
			setIndeterminate();
		}
		
		public function get indeterminate():Boolean
		{
			return this._indeterminate;
		}
		
		private function setIndeterminate():void
		{
			if (indeterminate)
			{
				this.setStyle('upIcon', CheckBoxIndeterminateIcon);
				this.setStyle('overIcon', CheckBoxIndeterminateIcon);
				this.setStyle('downIcon', CheckBoxIndeterminateIcon);
			}
			else
			{
				this.setStyle('upIcon', undefined);
				this.setStyle('overIcon', undefined);
				this.setStyle('downIcon', undefined);
			}
			
		}
		
	}
}