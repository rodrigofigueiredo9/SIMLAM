// Decompiled with JetBrains decompiler
// Type: Tecnomapas.ArcGis.MxdService
// Assembly: Tecnomapas.ArcGis, Version=1.0.1.0, Culture=neutral, PublicKeyToken=a4bcaf9376653225
// MVID: 7081477B-1906-455F-8CA4-7D3E408B4443
// Assembly location: C:\Users\userdefault\Desktop\WindowsServiceGeo\ProcessOperacoesGeo\bin\Debug\Tecnomapas.ArcGis.dll

using ESRI.ArcGIS.ADF.COMSupport;
using ESRI.ArcGIS;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Catalog;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Output;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace Tecnomapas.ArcGis
{
  public class MxdService : IMxdService, IDisposable
  {
    private string _mxdService;
    private IMapDocument _mapDocument;
    private IActiveView _activeView;
    private IAoInitialize _aoInitialize;

    [DllImport("user32.dll")]
    private static extern int GetDesktopWindow();

    public MxdService()
      : this("")
    {
    }

    public MxdService(string mxdFile)
    {
      this.InitializeLicense();
      this._mxdService = mxdFile;
      this._mapDocument = (IMapDocument) new MapDocumentClass();
    }

    public string MxdFile
    {
      get
      {
        return this._mxdService;
      }
      set
      {
        this._mxdService = value;
      }
    }

    public IPageLayout PageLayout
    {
      get
      {
        return this._mapDocument.PageLayout;
      }
    }

    public void Open()
    {
      try
      {
        this._mapDocument.Open(this.MxdFile, "");
        this.ActivateMap(0);
      }
      catch (Exception ex)
      {
        throw new MxdException("Error openning file", ex);
      }
    }

    public void Close()
    {
      this._activeView = (IActiveView) null;
      this._mapDocument.Close();
    }

    public void ActivatePageLayout()
    {
      this._activeView = this._mapDocument.PageLayout as IActiveView;
      this._activeView.Activate(MxdService.GetDesktopWindow());
    }

    public void ActivateMap(string mapName)
    {
      this.ActivateMap(this.FindMap(mapName));
    }

    public void ActivateMap(int mapId)
    {
      this.ActivateMap(this._mapDocument.get_Map(mapId));
    }

    public void Refresh()
    {
      this._activeView.Refresh();
    }

    public void AddAGSLayer(string layerName, string service)
    {
      IMap activeView = this._activeView as IMap;
      IImageServerLayer imageServerLayer = (IImageServerLayer) new ImageServerLayerClass();
      imageServerLayer.Initialize(service);
      imageServerLayer.Name = layerName;
      activeView.AddLayer((ILayer) imageServerLayer);
      this._activeView.Refresh();
    }

    public void AddLayer(string layerPathFile)
    {
      this.EnsureMapView();
      if (this._activeView == null || layerPathFile == null || !layerPathFile.EndsWith(".lyr"))
        return;
      IGxLayer gxLayer = (IGxLayer) new GxLayerClass();
      ((IGxFile) gxLayer).Path = layerPathFile;
      if (gxLayer.Layer == null)
        return;
      this._activeView.FocusMap.AddLayer(gxLayer.Layer);
    }

    public void SetQueryDefinition(string layerName, string query)
    {
      this.EnsureMapView();
      this.SetQueryDefinition(this.FindFeatureLayer(layerName) as IFeatureLayerDefinition, query);
    }

    public void SetQueryDefinition(int layerId, string query)
    {
      this.EnsureMapView();
      this.SetQueryDefinition((this._activeView as IMap).get_Layer(layerId) as IFeatureLayerDefinition, query);
    }

    public void SetElementText(string elementName, string text)
    {
      this.EnsurePageView();
      ITextElement element = this.FindElement(elementName) as ITextElement;
      if (element == null)
        throw new MxdException("Element is not present");
      element.Text = text;
      this._activeView.Refresh();
    }

    public void SetElementPosition(string elementName, Envelope envelope)
    {
      IElement element = this.FindElement(elementName);
      if (element == null)
        throw new MxdException("Element " + elementName + " is not present");
      IEnvelope envelope1 = (IEnvelope) new EnvelopeClass();
      envelope1.PutCoords(envelope.Xmin, envelope.Ymin, envelope.Xmax, envelope.Ymax);
      element.Geometry = (IGeometry) envelope1;
      this._activeView.Refresh();
    }

    public void SetHorizontalAlign(string elementName, string alignment, string relate)
    {
      IElement element1 = this.FindElement(elementName);
      if (element1 == null)
        throw new MxdException("Element " + elementName + " is not present");
      IEnvelope envelope1 = element1.Geometry.Envelope;
      IEnvelope envelope2;
      if (relate.Trim() == string.Empty)
      {
        envelope2 = (this._activeView as ESRI.ArcGIS.Carto.PageLayout).Page.PrintableBounds.Envelope;
      }
      else
      {
        IElement element2 = this.FindElement(relate);
        if (element2 == null)
          throw new MxdException("Dataframe " + relate + " is not present");
        envelope2 = element2.Geometry.Envelope;
      }
      switch (alignment)
      {
        case "esquerda":
          envelope1.Offset(envelope2.XMin - envelope1.XMin, 0.0);
          break;
        case "direita":
          envelope1.Offset(envelope2.XMax - envelope1.XMax, 0.0);
          break;
        case "centro":
          double num1 = envelope1.Width / 2.0 + envelope1.XMin;
          double num2 = envelope2.Width / 2.0 + envelope2.XMin;
          envelope1.Offset(num2 - num1, 0.0);
          break;
        default:
          throw new MxdException("Alignment " + alignment + " is invalid");
      }
      element1.Geometry = (IGeometry) envelope1;
      this._activeView.Refresh();
    }

    public void SetVerticalAlign(string elementName, string alignment, string relate)
    {
      IElement element1 = this.FindElement(elementName);
      if (element1 == null)
        throw new MxdException("Dataframe " + elementName + " is not present");
      IEnvelope envelope1 = element1.Geometry.Envelope;
      IEnvelope envelope2;
      if (relate.Trim() == string.Empty)
      {
        envelope2 = (this._activeView as ESRI.ArcGIS.Carto.PageLayout).Page.PrintableBounds.Envelope;
      }
      else
      {
        IElement element2 = this.FindElement(relate);
        if (element2 == null)
          throw new MxdException("Dataframe " + relate + " is not present");
        envelope2 = element2.Geometry.Envelope;
      }
      switch (alignment)
      {
        case "topo":
          envelope1.Offset(0.0, envelope2.YMax - envelope1.YMax);
          break;
        case "fundo":
          envelope1.Offset(0.0, envelope2.YMin - envelope1.YMin);
          break;
        case "centro":
          double num1 = envelope1.Height / 2.0 + envelope1.YMin;
          double num2 = envelope2.Height / 2.0 + envelope2.YMin;
          envelope1.Offset(0.0, num2 - num1);
          break;
        default:
          throw new MxdException("Alignment " + alignment + " is invalid");
      }
      element1.Geometry = (IGeometry) envelope1;
      this._activeView.Refresh();
    }

    public void SetHorizontalOffset(string elementName, double offset, string relate)
    {
      IElement element1 = this.FindElement(elementName);
      if (element1 == null)
        throw new MxdException("Element " + elementName + " is not present");
      IEnvelope envelope1 = element1.Geometry.Envelope;
      switch (relate)
      {
        case "atual":
          envelope1.Offset(offset, 0.0);
          break;
        case "":
        case "absoluto":
          envelope1.Offset(-envelope1.XMin + offset, 0.0);
          break;
        default:
          IElement element2 = this.FindElement(relate);
          if (element2 == null)
            throw new MxdException("Element " + relate + " is not present");
          IEnvelope envelope2 = element2.Geometry.Envelope;
          envelope1.Offset(-envelope1.XMin + envelope2.XMin + offset, 0.0);
          break;
      }
      element1.Geometry = (IGeometry) envelope1;
      this._activeView.Refresh();
    }

    public void SetVerticalOffset(string elementName, double offset, string relate)
    {
      IElement element1 = this.FindElement(elementName);
      if (element1 == null)
        throw new MxdException("Element " + elementName + " is not present");
      IEnvelope envelope1 = element1.Geometry.Envelope;
      switch (relate)
      {
        case "atual":
          envelope1.Offset(0.0, offset);
          break;
        case "":
        case "absoluto":
          envelope1.Offset(0.0, -envelope1.YMin + offset);
          break;
        default:
          IElement element2 = this.FindElement(relate);
          if (element2 == null)
            throw new MxdException("Element " + relate + " is not present");
          IEnvelope envelope2 = element2.Geometry.Envelope;
          envelope1.Offset(0.0, -envelope1.YMin + envelope2.YMin + offset);
          break;
      }
      element1.Geometry = (IGeometry) envelope1;
      this._activeView.Refresh();
    }

    public Envelope GetElementEnvelope(string elementName)
    {
      IElement element = this.FindElement(elementName);
      if (element == null)
        return new Envelope();
      IEnvelope envelope = element.Geometry.Envelope;
      return new Envelope(envelope.XMin, envelope.XMax, envelope.YMin, envelope.YMax);
    }

    public void RefreshLegends()
    {
      this.EnsurePageView();
      IGraphicsContainer pageLayout = this._mapDocument.PageLayout as IGraphicsContainer;
      pageLayout.Reset();
      for (IElement element = pageLayout.Next(); element != null; element = pageLayout.Next())
      {
        if (element is IMapSurroundFrame)
          ((element as IMapSurroundFrame).MapSurround as ILegend)?.Refresh();
      }
    }

    public void RefreshLegends(bool scaleSymbols)
    {
      this.EnsurePageView();
      IGraphicsContainer pageLayout = this._mapDocument.PageLayout as IGraphicsContainer;
      pageLayout.Reset();
      for (IElement element = pageLayout.Next(); element != null; element = pageLayout.Next())
      {
        if (element is IMapSurroundFrame)
        {
          ILegend2 mapSurround = (element as IMapSurroundFrame).MapSurround as ILegend2;
          if (mapSurround != null)
          {
            mapSurround.ScaleSymbols = scaleSymbols;
            mapSurround.Refresh();
          }
        }
      }
    }

    public void ZoomToFeature(string featureClassName, string field, string value)
    {
      IFeatureCursor features = this.GetFeatures(featureClassName, field, value);
      IFeature feature = features.NextFeature();
      IEnvelope zoomEnv = (IEnvelope) new EnvelopeClass();
      for (; feature != null; feature = features.NextFeature())
        zoomEnv.Union(feature.Shape.Envelope);
      this.Release((object) features);
      this.ZoomToEnvelope(zoomEnv);
    }

    public void ZoomToFeature(string featureClassName, string query)
    {
      IFeatureCursor features = this.GetFeatures(featureClassName, query);
      IFeature feature = features.NextFeature();
      IEnvelope zoomEnv = (IEnvelope) new EnvelopeClass();
      for (; feature != null; feature = features.NextFeature())
        zoomEnv.Union(feature.Shape.Envelope);
      this.ZoomToEnvelope(zoomEnv);
    }

    public void ZoomPercentage(double percentage)
    {
      IEnvelope extent = this._activeView.Extent;
      extent.Expand(percentage, percentage, true);
      this.ZoomToEnvelope(extent);
    }

    public void ZoomToScale(string frame, double scale)
    {
      this.EnsurePageView();
      IPageLayout activeView = this._activeView as IPageLayout;
      (this.FindElement(frame) as IMapFrame).Map.MapScale = scale;
      this._activeView.Refresh();
    }

    public void ZoomToBestScale(string frame, double nearScale)
    {
      this.EnsurePageView();
      IPageLayout activeView = this._activeView as IPageLayout;
      IMapFrame element = this.FindElement(frame) as IMapFrame;
      IMap map = element.Map;
      int num1 = (int) (map.MapScale / nearScale);
      double num2 = (map.MapScale % nearScale > 0.0 ? (double) (num1 + 1) : (double) num1) * nearScale;
      map.MapScale = num2;
      element.MapScale = num2;
      this._activeView.Refresh();
    }

    public void ZoomToEnvelope(Envelope zoomEnv)
    {
      IEnvelope zoomEnv1 = (IEnvelope) new EnvelopeClass();
      zoomEnv1.XMax = zoomEnv.Xmax;
      zoomEnv1.XMin = zoomEnv.Xmin;
      zoomEnv1.YMax = zoomEnv.Ymax;
      zoomEnv1.YMin = zoomEnv.Ymin;
      this.ZoomToEnvelope(zoomEnv1);
    }

    public void SetGridColor(string frame, Color color)
    {
      IMapGrid mapGrid = (this.FindElement(frame) as IMapGrids).get_MapGrid(0);
      IRgbColor rgbColor = (IRgbColor) new RgbColorClass();
      rgbColor.Red = (int) color.R;
      rgbColor.Blue = (int) color.B;
      rgbColor.Green = (int) color.G;
      mapGrid.LineSymbol.Color = (IColor) rgbColor;
    }

    public void AdjustGrid(string frame, int x, int y)
    {
      IElement element = this.FindElement(frame);
      IMap map = (element as IMapFrame).Map;
      IMeasuredGrid measuredGrid = (element as IMapGrids).get_MapGrid(0) as IMeasuredGrid;
      if (measuredGrid.Units == esriUnits.esriMeters)
      {
        double intervaloP1 = element.Geometry.Envelope.Width * map.MapScale / (double) x / 100.0;
        double num1 = this.MelhorIntervaloP(intervaloP1);
        double intervaloP2 = element.Geometry.Envelope.Height * map.MapScale / (double) y / 100.0;
        double num2 = this.MelhorIntervaloP(intervaloP2);
        measuredGrid.XIntervalSize = num1 > 0.0 ? num1 : intervaloP1;
        measuredGrid.YIntervalSize = num2 > 0.0 ? num2 : intervaloP2;
      }
      else
      {
        double num1 = 11131884.502145;
        double intervalo1 = element.Geometry.Envelope.Width * map.MapScale / (double) x / num1;
        double num2 = this.MelhorIntervalo(intervalo1);
        double intervalo2 = element.Geometry.Envelope.Height * map.MapScale / (double) y / num1;
        double num3 = this.MelhorIntervalo(intervalo2);
        measuredGrid.XIntervalSize = num2 > 0.0 ? num2 : intervalo1;
        measuredGrid.YIntervalSize = num3 > 0.0 ? num3 : intervalo2;
      }
      this._activeView.Refresh();
    }

    public void SetRasterLayer(string layer, string image)
    {
      string directoryName = System.IO.Path.GetDirectoryName(image);
      string fileName = System.IO.Path.GetFileName(image);
      IDataLayer layer1 = this.FindLayer(layer) as IDataLayer;
      IEnumDatasetName enumDatasetName = ((IWorkspaceFactory) new RasterWorkspaceFactoryClass()).OpenFromFile(directoryName, 0).get_DatasetNames(esriDatasetType.esriDTRasterDataset);
      IDatasetName datasetName1 = enumDatasetName.Next();
      IDatasetName datasetName2 = (IDatasetName) null;
      for (; datasetName1 != null; datasetName1 = enumDatasetName.Next())
      {
        if (string.Equals(datasetName1.Name, fileName, StringComparison.InvariantCultureIgnoreCase))
        {
          datasetName2 = datasetName1;
          break;
        }
      }
      if (datasetName1 == null)
        throw new ApplicationException(string.Format("Raster {0} not found.", (object) image));
      IName name = datasetName2 as IName;
      name.Open();
      layer1.DataSourceName = name;
      this._activeView.Refresh();
    }

    public void SetFeatureLayer(string layer, string feature)
    {
      string directoryName = System.IO.Path.GetDirectoryName(feature);
      string withoutExtension = System.IO.Path.GetFileNameWithoutExtension(feature);
      IDataLayer layer1 = this.FindLayer(layer) as IDataLayer;
      IEnumDatasetName enumDatasetName = ((IWorkspaceFactory) new ShapefileWorkspaceFactoryClass()).OpenFromFile(directoryName, 0).get_DatasetNames(esriDatasetType.esriDTFeatureClass);
      IDatasetName datasetName1 = enumDatasetName.Next();
      IDatasetName datasetName2 = (IDatasetName) null;
      for (; datasetName1 != null; datasetName1 = enumDatasetName.Next())
      {
        if (string.Equals(datasetName1.Name, withoutExtension, StringComparison.InvariantCultureIgnoreCase))
        {
          datasetName2 = datasetName1;
          break;
        }
      }
      if (datasetName1 == null)
        throw new ApplicationException(string.Format("Shapefile {0} not found.", (object) feature));
      IName name = datasetName1 as IName;
      name.Open();
      layer1.DataSourceName = name;
      this._activeView.Refresh();
    }

    public string GetDataSource(string layer)
    {
      ILayer layer1 = this.FindLayer(layer);
      if (layer1 == null)
        throw new MxdException("Layer not found.");
      IDataLayer dataLayer = layer1 as IDataLayer;
      if (dataLayer == null)
        throw new MxdException("Layer not a Data Layer.");
      IDatasetName dataSourceName = dataLayer.DataSourceName as IDatasetName;
      if (dataSourceName == null)
        throw new MxdException("Layer not a Data Layer.");
      return dataSourceName.Name;
    }

    public string GetDataSource(int layer)
    {
      ILayer layer1 = this._mapDocument.get_Layer(0, layer);
      if (layer1 == null)
        throw new MxdException("Layer not found.");
      IDataLayer dataLayer = layer1 as IDataLayer;
      if (dataLayer == null)
        throw new MxdException("Layer not a Data Layer.");
      IDatasetName dataSourceName = dataLayer.DataSourceName as IDatasetName;
      if (dataSourceName == null)
        throw new MxdException("Layer not a Data Layer.");
      return dataSourceName.Name;
    }

    public List<LayerInfo> GetLayerInfos(int mapIndex)
    {
      List<LayerInfo> layerInfoList = new List<LayerInfo>();
      IMap map = this._mapDocument.get_Map(mapIndex);
      int layerCount = map.LayerCount;
      IEnumLayer enumLayer = map.get_Layers((UID) null, true);
      enumLayer.Reset();
      ILayer layer = enumLayer.Next();
      int num = 0;
      while (layer != null)
      {
        LayerInfo layerInfo = new LayerInfo();
        layerInfoList.Add(layerInfo);
        layerInfo.ID = num;
        layerInfo.Name = layer.Name;
        layerInfo.Enabled = layer.Visible;
        layerInfo.MinScale = layer.MinimumScale;
        layerInfo.MaxScale = layer.MaximumScale;
        IDataLayer dataLayer = layer as IDataLayer;
        if (dataLayer != null)
        {
          IDatasetName dataSourceName = dataLayer.DataSourceName as IDatasetName;
          if (dataSourceName != null)
          {
            layerInfo.Type = dataSourceName.WorkspaceName.BrowseName;
            layerInfo.Source = dataSourceName.Name;
          }
        }
        Console.WriteLine(num);
        layer = enumLayer.Next();
        ++num;
      }
      return layerInfoList;
    }

    public int GetLayerCount(int mapIndex)
    {
      int num1 = 0;
      IEnumLayer enumLayer = this._mapDocument.get_Map(mapIndex).get_Layers((UID) null, true);
      enumLayer.Reset();
      ILayer layer = enumLayer.Next();
      int num2 = 0;
      while (layer != null)
      {
        num1 = num2;
        layer = enumLayer.Next();
        ++num2;
      }
      return num1;
    }

    public List<DataFrameInfo> GetDataFrames()
    {
      List<DataFrameInfo> dataFrameInfoList = new List<DataFrameInfo>();
      int mapCount = this._mapDocument.MapCount;
      for (int mapIndex = 0; mapIndex < mapCount; ++mapIndex)
      {
        DataFrameInfo dataFrameInfo = new DataFrameInfo();
        dataFrameInfoList.Add(dataFrameInfo);
        dataFrameInfo.ID = mapIndex;
        IMap map = this._mapDocument.get_Map(mapIndex);
        dataFrameInfo.Name = map.Name;
        if (map.SpatialReference != null)
          dataFrameInfo.Reference = map.SpatialReference.Name;
      }
      return dataFrameInfoList;
    }

    public string GetQueryDefinition(string layerName)
    {
      this.EnsureMapView();
      return (this.FindFeatureLayer(layerName) as IFeatureLayerDefinition).DefinitionExpression;
    }

    public string GetQueryDefinition(int layerId)
    {
      this.EnsureMapView();
      return ((this._activeView as IMap).get_Layer(layerId) as IFeatureLayerDefinition).DefinitionExpression;
    }

    public int ObterQuantidade(string layerName)
    {
      string queryDefinition = this.GetQueryDefinition(layerName);
      IQueryFilter QueryFilter = (IQueryFilter) new QueryFilterClass();
      QueryFilter.WhereClause = queryDefinition;
      return this.FindFeatureClass(layerName).FeatureCount(QueryFilter);
    }

    public void SetLayerVisibility(string layerName, bool visible)
    {
      this.FindLayer(layerName).Visible = visible;
      this._activeView.Refresh();
    }

    public void SetLayerVisibility(int layerId, bool visible)
    {
      this.EnsureMapView();
      IEnumLayer enumLayer = (this._activeView as IMap).get_Layers((UID) null, true);
      enumLayer.Reset();
      ILayer layer = enumLayer.Next();
      int num = 0;
      while (layer != null)
      {
        if (num == layerId)
        {
          layer.Visible = visible;
          break;
        }
        layer = enumLayer.Next();
        ++num;
      }
      this._activeView.Refresh();
    }

    public bool GetLayerVisibility(string layer)
    {
      return this.FindLayer(layer).Visible;
    }

    public bool GetLayerVisibility(int layer)
    {
      this.EnsureMapView();
      IEnumLayer enumLayer = (this._activeView as IMap).get_Layers((UID) null, true);
      enumLayer.Reset();
      ILayer layer1 = enumLayer.Next();
      int num = 0;
      while (layer1 != null)
      {
        if (num == layer)
          return layer1.Visible;
        layer1 = enumLayer.Next();
        ++num;
      }
      throw new MxdException("Index of layer not found");
    }

    public void ConfigureLayerVisibility(Dictionary<string, bool> layers)
    {
      foreach (KeyValuePair<string, bool> layer in layers)
      {
        if (this.GetLayerVisibility(layer.Key) != layer.Value)
          this.SetLayerVisibility(layer.Key, layer.Value);
      }
    }

    public void ConfigureLayerVisibility(Dictionary<int, bool> layers)
    {
      foreach (KeyValuePair<int, bool> layer in layers)
      {
        if (this.GetLayerVisibility(layer.Key) != layer.Value)
          this.SetLayerVisibility(layer.Key, layer.Value);
      }
    }

    public void ExportToPDF(string fileName, double resolution)
    {
      IExportPDF exportPdf = (IExportPDF) new ExportPDFClass();
      exportPdf.EmbedFonts = true;
      exportPdf.ImageCompression = esriExportImageCompression.esriExportImageCompressionDeflate;
      exportPdf.Compressed = true;
      (exportPdf as IExportVectorOptions).PolygonizeMarkers = true;
      IExport export = exportPdf as IExport;
      export.ExportFileName = fileName;
      export.Resolution = resolution;
      if (File.Exists(fileName))
        File.Delete(fileName);
      (exportPdf as IOutputRasterSettings).ResampleRatio = 1;
      int num = 96;
      int int32 = Convert.ToInt32(resolution);
      IActiveView pageLayout = this.PageLayout as IActiveView;
      tagRECT pixelBounds = new tagRECT();
      pixelBounds.left = 0;
      pixelBounds.top = 0;
      pixelBounds.right = pageLayout.ExportFrame.right / num * int32;
      pixelBounds.bottom = pageLayout.ExportFrame.bottom / num * int32;
      IEnvelope envelope = (IEnvelope) new EnvelopeClass();
      envelope.PutCoords((double) pixelBounds.left, (double) pixelBounds.top, (double) pixelBounds.right, (double) pixelBounds.bottom);
      export.PixelBounds = envelope;
      int hDC = export.StartExporting();
      pageLayout.Output(hDC, 0, ref pixelBounds, (IEnvelope) null, (ITrackCancel) null);
      export.FinishExporting();
      export.Cleanup();
    }

    public void ExportToJPG(string fileName, int resolution)
    {
      IExportJPEG exportJpeg = (IExportJPEG) new ExportJPEGClass();
      exportJpeg.ProgressiveMode = false;
      exportJpeg.Quality = (short) 100;
      IExport export = exportJpeg as IExport;
      export.ExportFileName = fileName;
      export.Resolution = (double) resolution;
      if (File.Exists(fileName))
        File.Delete(fileName);
      int num = 96;
      int int32 = Convert.ToInt32(resolution);
      IActiveView pageLayout = this.PageLayout as IActiveView;
      tagRECT pixelBounds = new tagRECT();
      pixelBounds.left = 0;
      pixelBounds.top = 0;
      pixelBounds.right = pageLayout.ExportFrame.right / num * int32;
      pixelBounds.bottom = pageLayout.ExportFrame.bottom / num * int32;
      IEnvelope envelope = (IEnvelope) new EnvelopeClass();
      envelope.PutCoords((double) pixelBounds.left, (double) pixelBounds.top, (double) pixelBounds.right, (double) pixelBounds.bottom);
      export.PixelBounds = envelope;
      pageLayout.Refresh();
      int hDC = export.StartExporting();
      pageLayout.Output(hDC, 0, ref pixelBounds, (IEnvelope) null, (ITrackCancel) null);
      export.FinishExporting();
      export.Cleanup();
    }

    public void ExportToTIF(string fileName, int resolution)
    {
      IExportTIFF exportTiff = (IExportTIFF) new ExportTIFFClass();
      exportTiff.CompressionType = esriTIFFCompression.esriTIFFCompressionNone;
      IExport export = exportTiff as IExport;
      export.ExportFileName = fileName;
      export.Resolution = (double) resolution;
      if (File.Exists(fileName))
        File.Delete(fileName);
      int num = 96;
      int int32 = Convert.ToInt32(resolution);
      IActiveView pageLayout = this.PageLayout as IActiveView;
      tagRECT pixelBounds = new tagRECT();
      pixelBounds.left = 0;
      pixelBounds.top = 0;
      pixelBounds.right = pageLayout.ExportFrame.right / num * int32;
      pixelBounds.bottom = pageLayout.ExportFrame.bottom / num * int32;
      IEnvelope envelope = (IEnvelope) new EnvelopeClass();
      envelope.PutCoords((double) pixelBounds.left, (double) pixelBounds.top, (double) pixelBounds.right, (double) pixelBounds.bottom);
      export.PixelBounds = envelope;
      int hDC = export.StartExporting();
      pageLayout.Output(hDC, 0, ref pixelBounds, (IEnvelope) null, (ITrackCancel) null);
      export.FinishExporting();
      export.Cleanup();
    }

    public void ExportToPNG(string fileName, int resolution)
    {
      IExportPNG exportPng = (IExportPNG) new ExportPNGClass();
      exportPng.InterlaceMode = false;
      IExport export = exportPng as IExport;
      export.ExportFileName = fileName;
      export.Resolution = (double) resolution;
      if (File.Exists(fileName))
        File.Delete(fileName);
      int num = 96;
      int int32 = Convert.ToInt32(resolution);
      IActiveView pageLayout = this.PageLayout as IActiveView;
      tagRECT pixelBounds = new tagRECT();
      pixelBounds.left = 0;
      pixelBounds.top = 0;
      pixelBounds.right = pageLayout.ExportFrame.right / num * int32;
      pixelBounds.bottom = pageLayout.ExportFrame.bottom / num * int32;
      IEnvelope envelope = (IEnvelope) new EnvelopeClass();
      envelope.PutCoords((double) pixelBounds.left, (double) pixelBounds.top, (double) pixelBounds.right, (double) pixelBounds.bottom);
      export.PixelBounds = envelope;
      int hDC = export.StartExporting();
      pageLayout.Output(hDC, 0, ref pixelBounds, (IEnvelope) null, (ITrackCancel) null);
      export.FinishExporting();
      export.Cleanup();
    }

    private void ZoomToEnvelope(IEnvelope zoomEnv)
    {
      this.EnsureMapView();
      this._activeView.Extent = zoomEnv;
      (this._activeView as IMap).ClearSelection();
      this._activeView.Refresh();
    }

    public void UsarZoomAutomatico(string frame)
    {
      this.EnsurePageView();
      IPageLayout activeView = this._activeView as IPageLayout;
      (this.FindElement(frame) as IMapFrame).ExtentType = esriExtentTypeEnum.esriExtentDefault;
    }

    public void UsarZoomPorEscala(string frame)
    {
      this.EnsurePageView();
      IPageLayout activeView = this._activeView as IPageLayout;
      (this.FindElement(frame) as IMapFrame).ExtentType = esriExtentTypeEnum.esriExtentScale;
    }

    public double GetMapScale(string elementName)
    {
      this.EnsurePageView();
      IPageLayout activeView = this._activeView as IPageLayout;
      return (this.FindElement(elementName) as IMapFrame).Map.MapScale;
    }

    private void SetQueryDefinition(IFeatureLayerDefinition feature, string query)
    {
      if (feature == null)
        throw new MxdException("Layer is not present");
      feature.DefinitionExpression = query;
      this._activeView.Refresh();
    }

    private void ActivateMap(IMap map)
    {
      if (map == null)
        throw new MxdException("Error activating map");
      this._activeView = map as IActiveView;
      this._activeView.Activate(MxdService.GetDesktopWindow());
    }

    private void EnsureMapView()
    {
      if (this._activeView is IMap)
        return;
      this.ActivateMap(0);
    }

    private void EnsurePageView()
    {
      if (this._activeView is IPageLayout)
        return;
      this.ActivatePageLayout();
    }

    private double MelhorIntervalo(double intervalo)
    {
      double[] numArray1 = new double[12]
      {
        45.0,
        30.0,
        20.0,
        15.0,
        12.0,
        10.0,
        7.5,
        5.0,
        3.0,
        2.0,
        1.5,
        1.0
      };
      double[] numArray2 = new double[3]
      {
        1.0,
        60.0,
        3600.0
      };
      foreach (double num1 in numArray2)
      {
        foreach (double num2 in numArray1)
        {
          if (intervalo >= num2 / num1)
            return num2 / num1;
        }
      }
      return -1.0;
    }

    private double MelhorIntervaloP(double intervaloP)
    {
      double y = 0.0;
      while (intervaloP / Math.Pow(10.0, y) > 100.0)
        ++y;
      return Math.Floor(intervaloP / Math.Pow(10.0, y) * Math.Pow(10.0, y));
    }

    public void AdjustColumns(int legend)
    {
      this.EnsurePageView();
      IGraphicsContainer pageLayout = this._mapDocument.PageLayout as IGraphicsContainer;
      pageLayout.Reset();
      for (IElement element = pageLayout.Next(); element != null; element = pageLayout.Next())
      {
        if (element is IMapSurroundFrame)
          ((element as IMapSurroundFrame).MapSurround as ILegend2)?.Refresh();
      }
    }

    private IMap FindMap(string mapName)
    {
      int mapCount = this._mapDocument.MapCount;
      int mapIndex = 0;
      int num = -1;
      IMap map = (IMap) null;
      while (mapIndex < mapCount && num == -1)
      {
        map = this._mapDocument.get_Map(mapIndex);
        if (map.Name.ToLower() == mapName.ToLower())
          num = mapIndex;
        else
          ++mapIndex;
      }
      if (num == -1)
        throw new MxdException("Map " + mapName + " not found");
      return map;
    }

    private ILayer FindLayer(string layerName)
    {
      try
      {
        this.EnsureMapView();
        IEnumLayer enumLayer = (this._activeView as IMap).get_Layers((UID) null, true);
        enumLayer.Reset();
        for (ILayer layer = enumLayer.Next(); layer != null; layer = enumLayer.Next())
        {
          if (string.Equals(layer.Name, layerName, StringComparison.InvariantCultureIgnoreCase))
            return layer;
        }
        return (ILayer) null;
      }
      catch (Exception ex)
      {
        throw new MxdException("Error locating layer " + layerName, ex);
      }
    }

    private IFeatureLayer FindFeatureLayer(string featureLayerName)
    {
      try
      {
        this.EnsureMapView();
        IFeatureLayer featureLayer = (IFeatureLayer) null;
        IEnumLayer enumLayer = (this._activeView as IMap).get_Layers((UID) null, true);
        enumLayer.Reset();
        for (ILayer layer = enumLayer.Next(); layer != null; layer = enumLayer.Next())
        {
          if (layer is IFeatureLayer && layer.Name.ToLower() == featureLayerName.ToLower())
          {
            featureLayer = layer as IFeatureLayer;
            break;
          }
        }
        return featureLayer;
      }
      catch (Exception ex)
      {
        throw new MxdException("Error locating layer " + featureLayerName, ex);
      }
    }

    private IFeatureClass FindFeatureClass(string featureClassName)
    {
      try
      {
        return this.FindFeatureLayer(featureClassName).FeatureClass;
      }
      catch (Exception ex)
      {
        throw new MxdException("Error locating layer " + featureClassName, ex);
      }
    }

    private FrameElement FindFrame(string frameName)
    {
      this.EnsurePageView();
      bool flag = false;
      IGraphicsContainer activeView = this._activeView as IGraphicsContainer;
      activeView.Reset();
      IElementProperties elementProperties;
      for (elementProperties = activeView.Next() as IElementProperties; elementProperties != null; elementProperties = activeView.Next() as IElementProperties)
      {
        if (elementProperties.Name.ToLower() == frameName.ToLower() && elementProperties.Type == "Data Frame")
        {
          flag = true;
          break;
        }
      }
      if (flag)
        return elementProperties as FrameElement;
      return (FrameElement) null;
    }

    private IElement FindElement(string elementName)
    {
      this.EnsurePageView();
      bool flag = false;
      IGraphicsContainer activeView = this._activeView as IGraphicsContainer;
      activeView.Reset();
      IElementProperties elementProperties;
      for (elementProperties = activeView.Next() as IElementProperties; elementProperties != null; elementProperties = activeView.Next() as IElementProperties)
      {
        if (elementProperties.Name.ToLower() == elementName.ToLower())
        {
          flag = true;
          break;
        }
      }
      if (flag)
        return elementProperties as IElement;
      return (IElement) null;
    }

    private IFeatureCursor GetFeatures(
      string featureClassName,
      string field,
      string value)
    {
      if (string.IsNullOrEmpty(featureClassName))
        throw new ArgumentNullException(nameof (featureClassName));
      if (string.IsNullOrEmpty(field))
        throw new ArgumentNullException(nameof (field));
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      IQueryFilter filter = (IQueryFilter) new QueryFilterClass();
      filter.WhereClause = field + " = '" + value.Replace("'", "''") + "'";
      IFeatureClass featureClass = this.FindFeatureClass(featureClassName);
      if (featureClass == null)
        throw new NullReferenceException(string.Format("Feature class not found: {0}", (object) featureClassName));
      return featureClass.Search(filter, true);
    }

    private IFeatureCursor GetFeatures(string featureClassName, string query)
    {
      if (string.IsNullOrEmpty(featureClassName))
        throw new ArgumentNullException(nameof (featureClassName));
      if (query == null)
        throw new ArgumentNullException(nameof (query));
      IQueryFilter filter = (IQueryFilter) new QueryFilterClass();
      filter.WhereClause = query;
      IFeatureClass featureClass = this.FindFeatureClass(featureClassName);
      if (featureClass == null)
        throw new NullReferenceException(string.Format("Feature class not found: {0}", (object) featureClassName));
      return featureClass.Search(filter, true);
    }

    private IFeature GetFeature(string featureClassName, string field, string value)
    {
      IFeatureCursor features = this.GetFeatures(featureClassName, field, value);
      IFeature feature = (IFeature) null;
      if (features != null)
      {
        feature = features.NextFeature();
        this.Release((object) features);
      }
      return feature;
    }

    private void Release(object obj)
    {
      do
        ;
      while (Marshal.ReleaseComObject(obj) > 0);
    }

        private void ReleaseResources()
        { 
            AOUninitialize.Shutdown();
        }

        private void InitializeLicense()
    {
      if (RuntimeManager.ActiveRuntime == null)
        RuntimeManager.BindLicense(ESRI.ArcGIS.ProductCode.Desktop);

      //switch (this.CheckOutLicenses(esriLicenseProductCode.esriLicenseProductCodeAdvanced))
      //{
      //  case esriLicenseStatus.esriLicenseNotLicensed:
      //    throw new Exception("Not Licensed");
      //  case esriLicenseStatus.esriLicenseUnavailable:
      //    throw new Exception("No License Available");
      //  case esriLicenseStatus.esriLicenseFailure:
      //    throw new Exception("License Failed");
      //}
    }

    private esriLicenseStatus CheckOutLicenses(esriLicenseProductCode productCode)
    {
      this._aoInitialize = (IAoInitialize) new AoInitializeClass();
      esriLicenseStatus esriLicenseStatus = this._aoInitialize.IsProductCodeAvailable(productCode);
      if (esriLicenseStatus == esriLicenseStatus.esriLicenseAvailable)
        esriLicenseStatus = this._aoInitialize.Initialize(productCode);
      return esriLicenseStatus;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    ~MxdService()
    {
      this.Dispose(false);
    }

        protected virtual void Dispose(bool disposing)
        {
            this.ReleaseResources();
        }
    }
}
