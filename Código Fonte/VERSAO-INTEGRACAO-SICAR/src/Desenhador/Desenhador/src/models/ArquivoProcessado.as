package models
{
	public class ArquivoProcessado
	{
		public function ArquivoProcessado()
		{
			IsPDF = false;
		}
		public var Id:Number;
		public var Texto:String;
		public var Tipo:Number;
		public var IsPDF:Boolean;
	}
}