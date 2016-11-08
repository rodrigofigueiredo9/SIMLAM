package com.tecnomapas.navegadores
{
	import com.esri.ags.Graphic;
	import com.esri.ags.Map;
	import com.esri.ags.events.PanEvent;
	import com.esri.ags.events.ZoomEvent;
	import com.esri.ags.geometry.Extent;
	import com.esri.ags.geometry.MapPoint;
	import com.esri.ags.geometry.Polygon;
	import com.esri.ags.layers.GraphicsLayer;
	import com.esri.ags.symbol.SimpleFillSymbol;
	import com.esri.ags.symbol.SimpleLineSymbol;
	
	import flash.display.Bitmap;
	import flash.display.BitmapData;
	import flash.display.DisplayObject;
	import flash.events.Event;
	import flash.events.MouseEvent;
	import flash.geom.Point;
	import flash.geom.Rectangle;
	import flash.net.FileReference;
	
	import mx.controls.Button;
	import mx.controls.DateField;
	import mx.graphics.codec.JPEGEncoder;
	
	import org.alivepdf.colors.RGBColor;
	import org.alivepdf.display.Display;
	import org.alivepdf.fonts.CoreFont;
	import org.alivepdf.fonts.FontFamily;
	import org.alivepdf.images.ColorSpace;
	import org.alivepdf.layout.Layout;
	import org.alivepdf.layout.Orientation;
	import org.alivepdf.layout.Size;
	import org.alivepdf.layout.Unit;
	import org.alivepdf.pdf.PDF;
	import org.alivepdf.saving.Method;
		
	public class PrintMap
	{
		[Embed (source="assets/img/legenda_pdf_navegador.png" )]
		public var imgLegenda:Class;
		
		[Embed (source="assets/img/norte.png" )]
		public var imgNorte:Class;
		
		/*[Embed (source="assets/img/Logomarca-SEMASA_peq.png" )]
		public var imgLogo:Class;*/
		
		
		[Embed (source="assets/img/carimbo_pdf_navegador.png")]
		public var imgLateral:Class;

		
		private var mapa:Map;
		private var layer:GraphicsLayer;
        private var largura:Number;
        private var altura:Number;  
        private var btnGerarPdf:Button = new Button();
        private var btnCancelar:Button = new Button();
        private var areaImpressao:Rectangle;
        
        public var layersOcultasTexto:String;
        public var isHabilitado:Boolean = false;
	
		public function PrintMap(mapa:Map, largura:Number, altura:Number)
		{
			this.layer = new GraphicsLayer();
			
			this.largura = largura;
			this.altura = altura;
			
			this.btnGerarPdf.addEventListener(MouseEvent.CLICK, gerarPdf);
			this.btnCancelar.addEventListener(MouseEvent.CLICK, desativar);
			
			this.mapa = mapa;
			this.mapa.addLayer(this.layer);
			this.mapa.addEventListener(ZoomEvent.ZOOM_START, exibirBotoes);
			this.mapa.addEventListener(ZoomEvent.ZOOM_END, exibirBotoes);
			this.mapa.addEventListener(PanEvent.PAN_START, exibirBotoes);
			this.mapa.addEventListener(PanEvent.PAN_END, exibirBotoes);
		}  
		
		public function set styleNameBtnGerarPdf(value:String):void
		{
			this.btnGerarPdf.styleName = value;
		}
		
		public function get styleNameBtnGerarPdf():String
		{
			return this.btnGerarPdf.styleName.toString();
		}
		
		public function set styleNameBtnCancelar(value:String):void
		{
			this.btnCancelar.styleName = value;
		}
		
		public function get styleNameBtnCancelar():String
		{
			return this.btnCancelar.styleName.toString();
		}
		
		private function exibirBotoes(event:Event):void
		{
			this.btnGerarPdf.visible = !this.btnGerarPdf.visible;
			this.btnCancelar.visible = !this.btnCancelar.visible;
		}
		
		public function gerarPdf(event:Event):void
		{
			if(isHabilitado)
			{
	            var pdf:PDF= new PDF(Orientation.LANDSCAPE, org.alivepdf.layout.Unit.POINT, Size.A4 );
				pdf.setDisplayMode(Display.FULL_PAGE, Layout.SINGLE_PAGE );

				pdf.addPage();				
				
				//pdf.setXY(150,20);
				//pdf.addMultiCell(0,0,"Serviço Municipal de Saneamento de Santo André",0, Align.LEFT,0);
				
				//var logo:DisplayObject = new imgLogo(); 
				//pdf.addImage(logo,null,0,0,logo.width*0.5,logo.height*0.5,0,1,true,'PNG',100,'Normal');
				
					
			
				//pdf.setFontSize(11);
				

	            
	            
	            
	            var img:Bitmap = extrairImagem();
	            pdf.addImageStream( new JPEGEncoder(100).encode(img.bitmapData), ColorSpace.DEVICE_RGB,null, 20, 20, largura*0.9, altura*0.9, 0, 1, 'Normal');

				var lateral:DisplayObject = new imgLateral();
				//pdf.addImage(lateral,null, 20 + largura*0.85, 20, lateral.width*0.75, lateral.height*0.75,0,1,true,'PNG',100,'Normal');
				pdf.addImage(lateral,null, 20 + largura*0.9, 20, lateral.width*0.9, lateral.height*0.9);
				
				pdf.lineStyle( new org.alivepdf.colors.RGBColor(0x000000), 1);
				pdf.drawRect( new Rectangle(48,48,largura*0.9+1,lateral.height*0.9));
				pdf.drawRect( new Rectangle(49,49,largura*0.9-1,lateral.height*0.9-2));

				//pdf.addImage(lateral,null, 20 + largura*0.9, 20, 265, 451);
				
				/*
				var src:BitmapData = new BitmapData(lateral.width, lateral.height);
				src.draw(lateral);
				
				var quality:int = 75;
				var jpg:JPEGEncoder = new JPEGEncoder(100);
				var byteArray:ByteArray = jpg.encode(src);
				
				
				pdf.addImageStream(byteArray, ColorSpace.DEVICE_RGB,null, 20 + largura*0.85, 20, lateral.width, lateral.height);
				*/
				//pdf.addImage(new imgLegenda(),null, 40 + largura*0.85, 80,0,0,0,1,true,'PNG',100,'Normal');
				
				var font:CoreFont = new CoreFont(FontFamily.ARIAL);
				pdf.setFont(font, 9);
				pdf.textStyle(new org.alivepdf.colors.RGBColor(0x000000),1);
				
				var now:Date = new Date();
				//pdf.addText( '00/00/0000 - 00:00', 597, 489);
				var hour:String = now.hours.toString();
				if (hour.length==1)
					hour = '0'+ hour;
					
				var min:String = now.minutes.toString();
				if (min.length==1)
					min = '0'+ min;
				
				pdf.addText( DateField.dateToString(now, "DD/MM/YYYY - "+ hour +":"+ min), 597, 489);

				pdf.addText( "Escala - 1:"+ Math.round(mapa.scale/0.9/50)*50 , 55, 493);

	            var file:FileReference = new FileReference();
				file.save(pdf.save(Method.LOCAL), "Mapa.pdf");
   			}
		}		
		
		
		private function extrairImagem():Bitmap
		{
			this.layer.visible = false; 
			var rectangle:Rectangle = clipRectangle();			
			
			var myBitmapData:BitmapData = new BitmapData(this.mapa.width,this.mapa.height);
			myBitmapData.draw(this.mapa,null,null,null,rectangle,true);
			
			var newBitmapData:BitmapData = new BitmapData(rectangle.width, rectangle.height)
			newBitmapData.copyPixels(myBitmapData, rectangle, new Point(0, 0));
			
			this.layer.visible = true;

			return new Bitmap(newBitmapData);
		} 

		private function clipRectangle():Rectangle
		{
			var ratioWidth:Number;
			var ratioHeight:Number;
			var clipRect:Rectangle = new Rectangle();
			var ratio:Number;
			var mapRatio:Number;   		
			
			ratio = this.altura /this.largura;
			mapRatio = this.mapa.height / this.mapa.width;    
			ratioHeight = (this.mapa.height);
			ratioWidth = (this.mapa.width);		        	
			clipRect.x =  (this.mapa.width - largura)/2;
			clipRect.y =  (this.mapa.height - altura)/2;		
			clipRect.width = largura;
			clipRect.height = altura;

			return clipRect;
		} 
		
		
		
		
		
		public function desativar(event:Event):void{
			exibirAreaImpressao(false);
		}
		
		private function atualizarAreaImpressao(event:Event):void
		{
			exibirAreaImpressao(isHabilitado);
		}
		
		public function exibirAreaImpressao(habilitar:Boolean):void 
		{
			layer.clear();
			
			if (habilitar)
			{
				areaImpressao = obterAreaImpressao();
				
				isHabilitado = true;
			  	layer.add(desenharAreaImpressao());
			  	destacarAreaImpressao();
			  	
			  	if(!this.mapa.contains(btnGerarPdf))
			  	{
					this.mapa.addChild(btnGerarPdf);
			  	}
			  	
			  	if(!this.mapa.contains(btnCancelar))
			  	{
					this.mapa.addChild(btnCancelar);
			  	}
			  	
			  	mapa.addEventListener(ZoomEvent.ZOOM_END, atualizarAreaImpressao);
				mapa.addEventListener(PanEvent.PAN_END, atualizarAreaImpressao);
			  	
				this.btnGerarPdf.move((areaImpressao.x + (areaImpressao.width / 2)) - 90, (areaImpressao.y + areaImpressao.height) + 5);
				this.btnCancelar.move((areaImpressao.x + (areaImpressao.width / 2)) + 10, (areaImpressao.y + areaImpressao.height) + 5);
			}
			else
			{
				if(isHabilitado)
				{
					if(this.mapa.contains(btnGerarPdf))
					{
						this.mapa.removeChild(btnGerarPdf);
					}
					
					if(this.mapa.contains(btnCancelar))
					{
						this.mapa.removeChild(btnCancelar);
					}
				}
				
				mapa.removeEventListener(ZoomEvent.ZOOM_END, atualizarAreaImpressao);
				mapa.removeEventListener(PanEvent.PAN_END, atualizarAreaImpressao);
				
				isHabilitado = false;
			}
		}
		
		public function desenharAreaImpressao():Graphic
		{  
			var minMapPoint:MapPoint = mapa.toMap(new Point(areaImpressao.x,areaImpressao.y));
			var maxMapPoint:MapPoint = mapa.toMap(new Point((areaImpressao.x + areaImpressao.width),(areaImpressao.y + areaImpressao.height)));
			
			var symbol:SimpleFillSymbol = new SimpleFillSymbol(SimpleFillSymbol.STYLE_NULL,0,0.1);
	        symbol.outline = new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, parseInt("EAEAEB"), 1.0, 3);
			
			var graphic:Graphic = new Graphic(new Extent(minMapPoint.x,minMapPoint.y,maxMapPoint.x,maxMapPoint.y)); 
			graphic.symbol = symbol;
			
			layer.clear();
	        
	       	return graphic ;
		}	
			
		private function obterAreaImpressao():Rectangle
		{
			var areaImpressao:Rectangle = new Rectangle();
			       	
			areaImpressao.x = (this.mapa.width - largura)/2;
			areaImpressao.y = (this.mapa.height - altura)/2;		
			areaImpressao.width = this.largura;
			areaImpressao.height = this.altura;
			
			return areaImpressao;
		}        
	   
	    private function destacarAreaImpressao():void
	    {   
	        var graphic:Graphic = null;
	        var points:Array;
	        var areaExterna:Polygon = new Polygon(null, mapa.spatialReference);
			
			/*
			 points.addItem(mapa.toMap(new Point(-1200, 1200)));
					points.addItem(mapa.toMap(new Point(-1200, -(mapa.extent.height + 1200))));
					points.addItem(mapa.toMap(new Point(mapa.extent.width + 1200, -(mapa.extent.height + 1200))));
					points.addItem(mapa.toMap(new Point(mapa.extent.width + 1200, 1200)));
					points.addItem(mapa.toMap(new Point(-1200, 1200)));
					*/
	        	
	        points = [
	        	mapa.toMap(new Point(-1200, 1200)),
	        	mapa.toMap(new Point(-1200, -(mapa.extent.height + 1200))),  
	        	mapa.toMap(new Point(areaImpressao.x, -(mapa.extent.height + 1200))),
	        	mapa.toMap(new Point(areaImpressao.x, areaImpressao.y)),
	        	mapa.toMap(new Point(areaImpressao.x + areaImpressao.width, areaImpressao.y)),
	        	mapa.toMap(new Point(areaImpressao.x + areaImpressao.width, areaImpressao.y + areaImpressao.height)),
	        	mapa.toMap(new Point(areaImpressao.x, areaImpressao.y + areaImpressao.height)),
	        	mapa.toMap(new Point(areaImpressao.x, -(mapa.extent.height + 1200))),
	        	mapa.toMap(new Point(mapa.extent.width + 1200, -(mapa.extent.height + 1200))),
	        	mapa.toMap(new Point(mapa.extent.width + 1200, 1200)),
	        	mapa.toMap(new Point(-1200, 1200))]
	        
	        areaExterna.addRing(points);
	        
	        graphic = new Graphic(areaExterna);
	        graphic.symbol = new SimpleFillSymbol(SimpleFillSymbol.STYLE_SOLID, 0, 0.4);
	        
	        layer.add(graphic);
	    }
	} 
} 