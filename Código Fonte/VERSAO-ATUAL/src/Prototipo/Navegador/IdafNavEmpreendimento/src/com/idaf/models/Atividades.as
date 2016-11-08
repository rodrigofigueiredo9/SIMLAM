package com.idaf.models
{
	public class Atividades
	{
		private var _id: Number;
		private var _atividade: String;
		
		public function Atividades()
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

		public function get atividade():String
		{
			return _atividade;
		}

		public function set atividade(value:String):void
		{
			_atividade = value;
		}


	}
}