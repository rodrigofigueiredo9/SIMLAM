package com.idaf.models
{
	public class Segmentos
	{
		private var _id		: Number;
		private var _texto	: String;
		
		public function Segmentos()
		{
		}

		public function get id():Number
		{
			return _id;
		}

		public function set id(value:Number):void
		{
			_id = value;
		}

		public function get texto():String
		{
			return _texto;
		}

		public function set texto(value:String):void
		{
			_texto = value;
		}


	}
}