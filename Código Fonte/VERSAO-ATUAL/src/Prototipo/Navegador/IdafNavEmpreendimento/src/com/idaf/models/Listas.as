package com.idaf.models
{
	public class Listas
	{
		private var _atividades	: Array;
		private var _segmentos	: Array;
		private var _municipios	: Array;
		
		public function Listas()
		{
		}

		public function get atividades():Array
		{
			return _atividades;
		}

		public function set atividades(value:Array):void
		{
			_atividades = value;
		}

		public function get segmentos():Array
		{
			return _segmentos;
		}

		public function set segmentos(value:Array):void
		{
			_segmentos = value;
		}

		public function get municipios():Array
		{
			return _municipios;
		}

		public function set municipios(value:Array):void
		{
			_municipios = value;
		}


	}
}