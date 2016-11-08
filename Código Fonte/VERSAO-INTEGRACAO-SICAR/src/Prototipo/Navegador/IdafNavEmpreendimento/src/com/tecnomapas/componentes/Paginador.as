package com.tecnomapas.componentes
{
	import flash.events.MouseEvent;
	
	import mx.collections.ArrayCollection;
	import mx.controls.listClasses.ListBase;
	import mx.events.ListEvent;
	
	import com.tecnomapas.auxiliares.ButtonPaginador;
	import com.tecnomapas.auxiliares.PaginacaoUI;
	import com.tecnomapas.events.PaginadorExibirPaginaEvent;
	import com.tecnomapas.events.PaginadorNovaPaginaEvent;

	[Event(name="novaPagina", type="com.tecnomapas.events.PaginadorNovaPaginaEvent")]
	
	public class Paginador extends PaginacaoUI
	{
		private var _listaAlvo:ListBase;
		private var _isTotalVirtual:Boolean = false;
		private var _totalDados:int = 0;
		private var _styleNameBtnPagina:String = "";
		private var _totalItensPorPagina:int = 0;
		
		private var btnAtual:ButtonPaginador;
		private var botoesPaginador:Array = [];
		private var totalBotoes:int;
		private var indexNovaPagina:int = 1;
		private var indexGrupoBotoesAtual:int = 1;
		private var totalGrupoBotoes:int;
		
		[Bindable] private var listaResultados:ArrayCollection;
		[Bindable] private var listaResultadosAux:ArrayCollection;
		
		public function Paginador() 
		{
			super();
		}
		
		/**
		 * Define o nome do style dos botões das páginas
		 * @param value
		 */
		public function set styleNameBtnPagina(value:String):void
		{
			_styleNameBtnPagina = value;
		}

		/**
		 * Retorna o nome do style dos botões das páginas
		 * @return 
		 */
		public function get styleNameBtnPagina():String
		{
			return _styleNameBtnPagina;
		}
		
		/**
		 * Define se o total de dados dados será virtual
		 * @param value
		 */
		public function set isTotalVirtual(value:Boolean):void
		{
			_isTotalVirtual = value;
		}

		/**
		 * Retorna se o total de dados dados é virtual
		 * @return 
		 */
		public function get isTotalVirtual():Boolean
		{
			return _isTotalVirtual;
		}
		
		/**
		 * Define o total de dados que serão amostrados em cada pagina
		 * @param value
		 */
		public function set totalPorPagina(value:int):void
		{
			_totalItensPorPagina = value;
		}

		/**
		 * Retorna o numero de dados que serão amostrados na tela. 
		 * @return 
		 */
		public function get totalPorPagina():int
		{
			if(_totalItensPorPagina <= 0)
			{
				_totalItensPorPagina = cbIntevalo.selectedItem.value;
			}
			
			return _totalItensPorPagina;
		}

		/**
		 * Define o total de dados que serão amostrados em cada pagina
		 * @param value
		 */
		public function set totalDados(value:int):void
		{
			_totalDados = value;
		}

		/**
		 * Retorna o numero de dados que serão amostrados na tela. 
		 * @return 
		 */
		public function get totalDados():int
		{
			return _totalDados;
		}
		
		/**
		* Define o componente que será utilizado para exibir os dados e a paginação 
		* @param value
		*/
		public function set componenteDestino(value:ListBase):void
		{
			_listaAlvo = value;
			_listaAlvo.rowCount = 1;
		}

		/**
		* Retorna o componente que será utilizado para exibir os dados e a paginação 
		* @return 
		*/
		public function get componenteDestino():ListBase
		{
			return _listaAlvo;
		}

		/**
		 * Define a lista que será utilizada para exibir na paginacao 
		 * @param value
		 */
		public function set resultados(value:ArrayCollection):void
		{
			listaResultados = value;
			
			if(!isTotalVirtual && listaResultados)
			{
				totalDados = listaResultados.length;
			}
		}

		/**
		 * Retorna a lista que será utilizada para exibir na paginacao 
		 * @return 
		 */
		public function get resultados():ArrayCollection
		{
			return listaResultados;
		}
		
		/**
		 * Retorna o intervalo inicial da paginação atual
		 * @return 
		 */
		public function get intervaloAtualInicial():int
		{
			return (btnAtual != null)? btnAtual.intervaloInicial : 1;
		}
		
		/**
		 * Retorna o intervalo final da paginação atual
		 * @return 
		 */
		public function get intervaloAtualFinal():int
		{
			return (btnAtual != null)? btnAtual.intervaloFinal : cbIntevalo.selectedItem.value;
		}
		
		override protected function childrenCreated():void
		{
			super.childrenCreated();
			
			this.btnPaginaAnterior.addEventListener(MouseEvent.CLICK, onExibirPaginaAnterior, false, 0, true);
			this.btnPaginaProxima.addEventListener(MouseEvent.CLICK, onExibirProximaPagina, false, 0, true);
			this.btnGrupoBotoesAnterior.addEventListener(MouseEvent.CLICK, onExibirGrupoBotoesAnterior, false, 0, true);
			this.btnGrupoBotoesProximo.addEventListener(MouseEvent.CLICK, onExibirProximoGrupoBotoes, false, 0, true);
			this.cbIntevalo.addEventListener(ListEvent.CHANGE, onTrocaIntervalo, false, 0, true);
		}
		
		override public function invalidateProperties():void
		{
			super.invalidateProperties();
		}
		
		/**
		 * @private
		 * Fica escutando quando o usuario trocou o total de interva de dados a ser amostrado na lista 
		 * @param evt
		 */
		private function onTrocaIntervalo(evt:ListEvent):void
		{
			indexGrupoBotoesAtual = 1;
			
			totalPorPagina = cbIntevalo.selectedItem.value;
			
			dispatchEvent(new PaginadorNovaPaginaEvent(PaginadorNovaPaginaEvent.NOVA_PAGINA, 1, totalPorPagina));
		}
		
		/**
		 * @private
		 * Responsavel por ficar escutando quando o usuario deseja visualizar o conteudo de cada pagina 
		 */
		private function onExibirPagina(evt:PaginadorExibirPaginaEvent):void
		{
			indexNovaPagina = evt.botaoPagina.pagina;
			
			dispatchEvent(new PaginadorNovaPaginaEvent(PaginadorNovaPaginaEvent.NOVA_PAGINA, evt.botaoPagina.intervaloInicial, evt.botaoPagina.intervaloFinal));
		}
		
		/**
		 * @private 
		 * Responsavel por exibir os dados do grupo de páginas anterior.
		 */
		private function onExibirGrupoBotoesAnterior(evt:MouseEvent):void
		{
			indexGrupoBotoesAtual--;
			 
			indexNovaPagina = (indexGrupoBotoesAtual * 7);
			
			dispatchEvent(new PaginadorNovaPaginaEvent(PaginadorNovaPaginaEvent.NOVA_PAGINA, botoesPaginador[indexNovaPagina].intervaloInicial, botoesPaginador[indexNovaPagina].intervaloFinal));
		}

		/**
		 * @private 
		 * Responsavel por exibir os dados da pagina anterior.
		 */
		private function onExibirPaginaAnterior(evt:MouseEvent):void
		{
			indexNovaPagina = (btnAtual.pagina > 1)? btnAtual.pagina - 1 : 1;
			
			if(indexNovaPagina <= (indexGrupoBotoesAtual * 7) - 7)
			{
				indexGrupoBotoesAtual--;
			}
			
			dispatchEvent(new PaginadorNovaPaginaEvent(PaginadorNovaPaginaEvent.NOVA_PAGINA, botoesPaginador[indexNovaPagina].intervaloInicial, botoesPaginador[indexNovaPagina].intervaloFinal));
		}
		
		/**
		 * @private 
		 * Responsavel por amostrar os dados do proximo grupo de páginas
		 */
		private function onExibirProximoGrupoBotoes(evt:MouseEvent):void
		{
			indexNovaPagina = (indexGrupoBotoesAtual * 7) + 1;
			
			indexGrupoBotoesAtual++;
			
			dispatchEvent(new PaginadorNovaPaginaEvent(PaginadorNovaPaginaEvent.NOVA_PAGINA, botoesPaginador[indexNovaPagina].intervaloInicial, botoesPaginador[indexNovaPagina].intervaloFinal));
		}

		/**
		 * @private 
		 * Responsavel por amostrar os dados da proxima pagina
		 */
		private function onExibirProximaPagina(evt:MouseEvent):void
		{
			indexNovaPagina = (btnAtual.pagina < botoesPaginador.length) ? btnAtual.pagina + 1 : botoesPaginador.length;
			
			if(indexNovaPagina >= (indexGrupoBotoesAtual * 7) + 1)
			{
				indexGrupoBotoesAtual++;
			}
			
			dispatchEvent(new PaginadorNovaPaginaEvent(PaginadorNovaPaginaEvent.NOVA_PAGINA, botoesPaginador[indexNovaPagina].intervaloInicial, botoesPaginador[indexNovaPagina].intervaloFinal));
		}
		
		/**
		 * @public 
		 * Responsavel por atualizar os botões e as paginas que serão exibidos de acordo com os novos resultados.
		 */
		public function atualizar():void
		{
			configurarBotoesPaginacao();
			
			btnAtual = null;
			
			if(botoesPaginador != null && botoesPaginador.length > 0)
			{
				btnAtual = botoesPaginador[indexNovaPagina];
				btnAtual.selected = true;
			}
			
			indexNovaPagina = 1;
		
			carregarResultadosNaPagina(btnAtual);
		}
		
		/**
		 * @private 
		 * Responsavel por configurar os botoes de paginação na tela.
		 */
		private function configurarBotoesPaginacao():void
		{
			var indexBotaoInicial:int = 0;
			var btnAux:ButtonPaginador;
			
			if(totalPorPagina <= 0)
			{
				totalPorPagina = this.cbIntevalo.selectedItem.value;
			}
			
			totalBotoes = Math.ceil(totalDados/totalPorPagina);
			totalGrupoBotoes = Math.ceil(totalBotoes/7);
			
			if(containerBpIntermadiarios != null && containerBpIntermadiarios.getChildren() != null)
			{
				containerBpIntermadiarios.removeAllChildren();
				botoesPaginador = [];
			}
			
			if(indexGrupoBotoesAtual > 0)
			{
				indexBotaoInicial = (indexGrupoBotoesAtual * 7) - 7;
			}
			
			for(var i:int = 0; i < totalBotoes; i++)
			{
				btnAux = new ButtonPaginador();
				btnAux.styleName = styleNameBtnPagina;
				btnAux.toggle = true;
				btnAux.pagina = i + 1;
				btnAux.intervaloInicial = (i * totalPorPagina) + 1;
				btnAux.intervaloFinal = (i * totalPorPagina) + totalPorPagina;
				btnAux.addEventListener(PaginadorExibirPaginaEvent.EXIBIR_PAGINA, onExibirPagina);
				
				botoesPaginador[btnAux.pagina] = btnAux;
				
				if(i >= indexBotaoInicial && containerBpIntermadiarios.getChildren().length < 7)
				{
					containerBpIntermadiarios.addChild(btnAux);
				}
			}
			
			/*for(var i:int = 0; i < totalBotoes; i++)
			{
				btnAux = new ButtonPaginador();
				btnAux.styleName = styleNameBtnPagina;
				btnAux.toggle = true;
				btnAux.pagina = i + 1;
				btnAux.intervaloInicial = (i * totalPorPagina) + 1;
				btnAux.intervaloFinal = (i * totalPorPagina) + totalPorPagina;
				btnAux.addEventListener(PaginadorExibirPaginaEvent.EXIBIR_PAGINA, onExibirPagina);
				
				botoesPaginador[btnAux.pagina] = btnAux;
				
				containerBpIntermadiarios.addChild(btnAux);
			}*/
		}
		
		/**
		 * @private 
		 * Configura e renderiza os dados Base para ser amostrado na tela levendo-se em consideração
		 * o intervalo passado.
		 * @param intervloIncial
		 */
		private function carregarResultadosNaPagina(bpAtual:ButtonPaginador):void
		{
			listaResultadosAux = new ArrayCollection();
			
			if(!isTotalVirtual)
			{
				for(var j:int = 0; j < totalPorPagina; j++)
				{
					if(bpAtual.intervaloInicial + j < totalDados)
					{
						listaResultadosAux.addItem(listaResultados.getItemAt(bpAtual.intervaloInicial + j));
					}
				}
			}
			else
			{
				listaResultadosAux = listaResultados;
			}
			
			btnGrupoBotoesAnterior.enabled = (indexGrupoBotoesAtual > 1) && (indexGrupoBotoesAtual <= Math.ceil(totalBotoes/7));
			btnGrupoBotoesProximo.enabled = (indexGrupoBotoesAtual < Math.ceil(totalBotoes/7));
			configBtnPaginaAnterior((bpAtual != null && bpAtual.pagina > 1));
			configBtnProximaPagina(!(bpAtual == null || bpAtual.pagina == totalBotoes));
			
			this.componenteDestino.dataProvider = listaResultadosAux;
			this.componenteDestino.invalidateDisplayList();
			this.componenteDestino.validateNow();
			
			if(listaResultadosAux.length > 0)
			{
				this.componenteDestino.rowCount = listaResultadosAux.length;
			}
			else
			{
				this.componenteDestino.rowCount = 1;
			}
		}
		
		private function configBtnPaginaAnterior(value:Boolean = true):void 
		{
			this.btnPaginaAnterior.enabled = value;
		}

		private function configBtnProximaPagina(value:Boolean = true):void 
		{
			this.btnPaginaProxima.enabled = value;
		}
	}
}