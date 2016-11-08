package services
{
	import com.gmaps.geom.Mbr;
	
	import flash.debugger.enterDebugger;
	import flash.events.Event;
	import flash.events.IOErrorEvent;
	import flash.net.URLLoader;
	import flash.net.URLRequest;
	import flash.net.URLRequestMethod;
	import flash.net.URLVariables;
	
	import models.ArquivoProcessado;
	import models.CategoriaQuadroDeArea;
	import models.FeicaoAreaAbrangencia;
	import models.ItemQuadroDeArea;
	import models.Projeto;
	import models.Retorno;
	import models.SituacaoProcessamento;
	
	import mx.controls.Alert;
	import mx.managers.*;

	public class ProjetoService
	{
		private var _serviceURL:String;
		private var _projeto:Projeto;
		private var _quadrosArea:Vector.<CategoriaQuadroDeArea>;
		private var _situacao:SituacaoProcessamento;
		private var _areaAbrangencia:Mbr;
		private var completeCallbackQuadro:Function;
		private var completeCallback:Function;
		
		[Embed(source="../assets/erro.png")]
		private var ErroImg:Class;
		
		public function ProjetoService()
		{	
			this._serviceURL = ConfiguracaoService.getInstance().serviceUrl +"/Projeto";
		}
		
		public function get getServiceURL():String
		{
			if(_serviceURL.length>4&&_serviceURL.substr(0,4).toLowerCase() == "null")
				getServiceURL = ConfiguracaoService.getInstance().serviceUrl +"/Projeto";
			return _serviceURL;
		}
		
		public function set getServiceURL(value:String):void
		{
			_serviceURL = value;
		}
		
		public function BuscarDadosProjeto(idProjeto:int, idFilaTipo:int, callback:Function) : void {
			completeCallback = callback;
			
			var variables:URLVariables = new URLVariables();
			variables.idProjeto = idProjeto;
			variables.idFilaTipo = idFilaTipo;
			
			var request: URLRequest = new URLRequest(this._serviceURL+"/BuscarDadosProjeto");
			request.method = URLRequestMethod.POST;
			request.data = variables;
			
			var loader:URLLoader = new URLLoader(request);
			loader.addEventListener(Event.COMPLETE, BuscarDadosProjetoComplete);
			loader.addEventListener(IOErrorEvent.IO_ERROR, ErrorFunction);
		}
		
		public function ListarQuadroAreas(idNavegador:int, idProjeto:int, callback:Function) : void {
			completeCallbackQuadro = callback;
			
			var variables:URLVariables = new URLVariables();
			variables.idNavegador = idNavegador;
			variables.idProjeto = idProjeto;
			
			var request: URLRequest = new URLRequest(this._serviceURL+"/ListarQuadroAreas");
			request.method = URLRequestMethod.POST;
			request.data = variables;
			
			var loader:URLLoader = new URLLoader(request);
			loader.addEventListener(Event.COMPLETE, ListarQuadroAreasComplete);
			loader.addEventListener(IOErrorEvent.IO_ERROR, ErrorFunction);
		}
		
		public function SalvarAreaAbrangencia(feicao:FeicaoAreaAbrangencia, callback:Function) : void {
			completeCallback = callback;
			if(!feicao)
			{
				Alert.show('Feição não pode ser nulo.');
				return;
			}
			if(feicao.IdProjeto <=0)
			{
				Alert.show('Id Projeto deve ser maior que zero.');
				return;
			}
			
			var request: URLRequest = new URLRequest(this._serviceURL+"/SalvarAreaAbrangenciaProjeto");
			request.contentType = 'application/json';
			request.method = URLRequestMethod.POST;		
			request.data =  JSON.stringify(feicao);
			
			var loader:URLLoader = new URLLoader(request);
			loader.addEventListener(Event.COMPLETE, SalvarAreaAbrangenciaComplete);
			loader.addEventListener(IOErrorEvent.IO_ERROR, ErrorFunction);
		}
		
		public function buscarAreaAbrangencia(objeto:Object) :Mbr {
			var obj:Object = JSON.parse(objeto.toString());
			setAreaAbrangencia(obj);			
			return _areaAbrangencia;
		}
		
		private function BuscarDadosProjetoComplete(event:Event) : void {
			var obj:Object = JSON.parse(event.target.data);
			setProjeto(obj);	
			completeCallback(_projeto);
		}
		
		public function buscarSituacaoProcessamento(objeto:Object) :SituacaoProcessamento {
			var obj:Object = JSON.parse(objeto.toString());
			setSituacaoProcessamento(obj);			
			return _situacao;
		}
		
		
		
		private function SalvarAreaAbrangenciaComplete(event:Event) : void {
			var obj:Object = JSON.parse(event.target.data);			
			completeCallback(setRetorno(obj));
		}
		
		
		public function setQuadroAreas(value:Object):void
		{
			_quadrosArea = new Vector.<CategoriaQuadroDeArea>();
			
			var ar:Array = value as Array;
			if(ar)
			{
				var categoria:CategoriaQuadroDeArea;
				for(var i:int =0; i< ar.length; i++)
				{
					categoria = new CategoriaQuadroDeArea();
					categoria.Id = i+1;
					categoria.IsAtivo = false;
					categoria.Nome = ar[i].Nome;
					
					categoria.Itens = new Vector.<ItemQuadroDeArea>();
					if(ar[i].Itens)
					{
						var arItens:Array = ar[i].Itens as Array;
						if(arItens)
						{
							var item:ItemQuadroDeArea;
							for(var k:int =0; k< arItens.length; k++)
							{
								item = new ItemQuadroDeArea();
								item.Id = k+1;
								item.Nome = arItens[k].Nome;
								item.Descricao = arItens[k].Descricao;
								item.Area = arItens[k].Area;
								item.IsProcessada = arItens[k].IsProcessada;
								item.IsSubArea = arItens[k].IsSubArea;
								categoria.Itens.push(item);
							}
							
						}
						
					}
					_quadrosArea.push(categoria);
				}
			}		
		}
		
		public function setSituacaoProcessamento(value:Object):void
		{
			_situacao = new SituacaoProcessamento();
			if(value)
			{
				_situacao.SituacaoTexto = value.SituacaoTexto;
				_situacao.SituacaoId = value.SituacaoId;
				_situacao.ArquivosProcessados = new Vector.<ArquivoProcessado>();
				if(value.ArquivosProcessados)
				{
					var ar:Array = value.ArquivosProcessados as Array;
					if(ar)
					{
						var arquivo:ArquivoProcessado;
						for(var i:int =0; i< ar.length; i++)
						{
							arquivo = new ArquivoProcessado();
							arquivo.Id = ar[i].Id;
							arquivo.Texto = ar[i].Texto;
							arquivo.IsPDF = ar[i].IsPDF;
							
							_situacao.ArquivosProcessados.push(arquivo);
						}
					}
				}
			}	
		}
		
		public function setProjeto(value:Object):void
		{
			_projeto = new Projeto();
			if(value)
			{
				_projeto.Id = value.Id;
				_projeto.Empreendimento = value.Empreendimento;
				_projeto.TipoNavegador = value.TipoNavegador;
			}	
		}
		
		public function setAreaAbrangencia(value:Object):void
		{
			if(value)
			{
				_areaAbrangencia = new Mbr( value.MenorX, value.MenorY, value.MaiorX,value.MaiorY);
			}	
		}
		
		
		private function setRetorno(obj:Object):Retorno
		{
			var retorno:Retorno = new Retorno();
			retorno.Mensagem = obj.Mensagem;
			retorno.Sucesso = obj.Sucesso;
			retorno.Objectid = obj.Objectid;		
			retorno.IdLayerFeicao = obj.IdLayerFeicao;		
			return retorno;
		}
		
		private function ListarQuadroAreasComplete(event:Event) : void {
			var obj:Object = JSON.parse(event.target.data);
			setQuadroAreas(obj);	
			completeCallbackQuadro(_quadrosArea);
		}
		
		public function AtualizarSessao():void		
		{			
			var browser:IBrowserManager = BrowserManager.getInstance(); 
			browser.init();
			var browserUrl:String = browser.url; // full url in the browser
			
			
			if (browserUrl.indexOf("file:") > -1)
			{
				browserUrl = "http://localhost:21625/Caracterizacoes/ProjetoGeografico/Editar?id=57&empreendimento=14&tipo=1&isCadastrarCaracterizacao=true";					
			}
			var urlExterna: String = browserUrl.substring(0, browserUrl.indexOf("/", browserUrl.indexOf("://")+3 ));	
			var request: URLRequest = new URLRequest(urlExterna+"/mapa/AtualizarSessao");
			request.contentType = 'application/json';
			request.method = URLRequestMethod.GET;
			//request.data =  JSON.stringify(feicao);
			
			var loader:URLLoader = new URLLoader(request);
			loader.addEventListener(IOErrorEvent.IO_ERROR, ErrorFunction);
			loader.addEventListener(Event.COMPLETE, function (event:Event):void {
				
				var obj:Object = JSON.parse(event.target.data);
				
				
				
				
			});
		}
		
		protected function ErrorFunction(ev:IOErrorEvent):void
		{
			if(ev) 
			{
				Alert.show(ev.text +" \n\nTente novamente executar a operação, caso não resolva consulte o administrador.","Ocorreu o seguinte erro",4,null,null,ErroImg);
			}
		}
	}
}