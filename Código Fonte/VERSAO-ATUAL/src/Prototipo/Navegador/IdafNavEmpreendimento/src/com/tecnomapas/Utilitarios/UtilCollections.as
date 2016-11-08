package com.tecnomapas.Utilitarios
{
	import mx.collections.ArrayCollection;
	import mx.collections.IViewCursor;
	import mx.collections.Sort;
	import mx.collections.SortField;
	
	public class UtilCollections
	{
		public static function buscarItemArrayOrdenando(valor:String, propriedadeFiltro:String, propriedadeOrdenacao:String, arrayItens:ArrayCollection):Object
		{

			if(arrayItens != null)
			{
				var sort:Sort = new Sort();
				var cursor:IViewCursor = arrayItens.createCursor();
				var sortField:SortField = new SortField(propriedadeOrdenacao);
				
				sort.fields = new Array(sortField);
				arrayItens.sort = sort;
				arrayItens.refresh();
				
				for(var i:int = 0; i < arrayItens.length; i++)
				{
					if(arrayItens[i][propriedadeFiltro] == valor)
					{
						return arrayItens[i];
					}
				}
			}
			
			return null;
		}
		

		public static function buscarItemArray(valor:String, propriedadeFiltro:String, arrayItens:ArrayCollection):Object
		{
			if(arrayItens != null)
			{
				for(var i:int = 0; i < arrayItens.length; i++)
				{
					if(arrayItens[i][propriedadeFiltro] == valor)
					{
						return arrayItens[i];
					}
				}
			}

			return null;
		}
	}
}