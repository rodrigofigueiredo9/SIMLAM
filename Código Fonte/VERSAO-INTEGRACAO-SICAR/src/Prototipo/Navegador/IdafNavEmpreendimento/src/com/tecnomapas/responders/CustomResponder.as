package com.tecnomapas.responders
{
	import mx.rpc.IResponder;

	public class CustomResponder implements IResponder
	{
		private var _result:Function;
		private var _customResultParams:Array;
		private var _customFault:Function;
		private var _customFaultParams:Array;
		
		public function CustomResponder(customResult:Function, customResultParams:Array, customFault:Function, customFaultParams:Array)
		{
			_result = customResult;
			_customResultParams = customResultParams;

			_customFault = customFault;
			_customFaultParams = customFaultParams;
		}
		
		public function result(data:Object):void
		{
			if(_result != null)
			{
				_result(data, _customResultParams);
			}
		}
		
		public function fault(info:Object):void
		{
			if (_customFault != null)
			{
				_customFault(info, _customFaultParams);
			}
		}
	}
}