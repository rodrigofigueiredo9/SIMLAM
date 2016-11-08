package models
{
	public class SituacaoProcessamento
	{
		public function SituacaoProcessamento()
		{
		}
		
		public var SituacaoId:Number;
		public var SituacaoTexto:String;
		public var ArquivosProcessados:Vector.<ArquivoProcessado>;
	}
}