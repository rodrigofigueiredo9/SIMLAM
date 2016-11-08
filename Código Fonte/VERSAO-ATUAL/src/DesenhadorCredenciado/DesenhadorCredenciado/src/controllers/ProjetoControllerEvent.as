package controllers
{
	import flash.events.Event;
	
	import models.CategoriaQuadroDeArea;
	import models.Projeto;
	import models.SituacaoProcessamento;

	public class ProjetoControllerEvent extends Event
	{
		public static const ATIVAR_DESATIVAR_CATEGORIA:String = "AtivarDesativarCategoriaProjetoControllerEvent";
		public static const LISTAR_QUADROAREAS:String = "ListarQuadroAreasProjetoControllerEvent";
		public static const ATUALIZAR_SITUACAOPROCESSAMENTO:String = "AtualizarSituacaoProcessamentoProjetoControllerEvent";
		public static const BUSCAR_DADOS_PROJETO:String = "BuscarDadosProjetoProjetoControllerEvent";
		private var _categorias:Vector.<CategoriaQuadroDeArea>;
		private var _situacao:SituacaoProcessamento;
		private var _projeto:Projeto;
		public function ProjetoControllerEvent(type:String, categorias:Vector.<CategoriaQuadroDeArea>, situacao:SituacaoProcessamento, projeto:Projeto=null, bubbles:Boolean=false, cancelable:Boolean=false)
		{
			if(projeto)
				this._projeto = projeto;
			if(categorias)
				this._categorias = categorias;
			if(situacao)
				this._situacao = situacao;
			super(type, bubbles, cancelable); 
		}
		
		public function get projeto():Projeto
		{
			return _projeto;
		}
		
		public function set projeto(value:Projeto):void
		{
			_projeto = value;
		}
		
		public function get situacao():SituacaoProcessamento
		{
			return _situacao;
		}

		public function set situacao(value:SituacaoProcessamento):void
		{
			_situacao = value;
		}

		public function get categorias():Vector.<CategoriaQuadroDeArea>
		{
			return _categorias;
		}
		
		public function set categorias(value:Vector.<CategoriaQuadroDeArea>):void
		{
			_categorias = value;
		}
	}
}