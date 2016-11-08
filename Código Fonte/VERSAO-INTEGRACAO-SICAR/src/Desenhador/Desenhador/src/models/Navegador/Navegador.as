package models.Navegador
{
	public class Navegador
	{
		public var Id:int;
		public var Nome:String;
		public var Servicos:Vector.<ServicoArcGis>;
		public var Cenarios:Vector.<CenarioServicoArcGis>;
		public var Filtros:Array;
		public var ProjetosAssociados:Array;
		public function Navegador()
		{
		}
		/*public function Navegador(id:int, nome:String)
		{
			Id= id;
			Nome = nome;
		}*/
	}
}