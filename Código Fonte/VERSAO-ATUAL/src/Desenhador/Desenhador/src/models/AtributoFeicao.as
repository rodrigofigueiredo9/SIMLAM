package models
{
	public class AtributoFeicao
	{
		/*public function AtributoFeicao()
		{
		}*/
		public function AtributoFeicao(nome:String="", valor:String="", tipo:int=AtributoFeicao.Manual)
		{
			Nome = nome;
			Valor = valor;
			Tipo = tipo;
		}
		public var Nome:String;
		public var Alias:String;
		public var Valor:String;
		public var Tipo:int;
		
		public static const Manual:int=0;
		public static const Automatico:int=1;
		public static const Sequencia:int=2;
	}
}