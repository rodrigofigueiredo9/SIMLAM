package tm.componentes.scripts
{
	import flash.utils.Timer;
	
	public class TokenTimer extends Timer
	{
		private var _token: uint;
		private var _data : Object;
		
		public function TokenTimer(token: uint, delay:Number, repeatCount:int=0,data:Object = null)
		{
			super(delay, repeatCount);
			this._token = token;
			this._data = data;
		}
		
		public function get token(): uint {
			return this._token;
		}
		
		public function get data(): Object {
			return this._data;
		}
		
		public function set data(value: Object): void {
			this._data = value;
		}
	}
}