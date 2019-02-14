// Decompiled with JetBrains decompiler
// Type: Tecnomapas.ArcGis.MxdServiceExporter
// Assembly: Tecnomapas.ArcGis, Version=1.0.1.0, Culture=neutral, PublicKeyToken=a4bcaf9376653225
// MVID: 7081477B-1906-455F-8CA4-7D3E408B4443
// Assembly location: C:\Users\userdefault\Desktop\WindowsServiceGeo\ProcessOperacoesGeo\bin\Debug\Tecnomapas.ArcGis.dll

using ESRI.ArcGIS.Carto;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Tecnomapas.ArcGis
{
  public class MxdServiceExporter : IMxdService
  {
    private IMxdService _mxdService;

    public MxdServiceExporter(string mxdFile)
    {
      this._mxdService = (IMxdService) new MxdService(mxdFile);
    }

    public MxdServiceExporter(IMxdService mxdService)
    {
      if (mxdService == null)
        throw new ArgumentNullException();
      this._mxdService = mxdService;
    }

    public string MxdFile
    {
      get
      {
        return this._mxdService.MxdFile;
      }
      set
      {
        this._mxdService.MxdFile = value;
      }
    }

    public IPageLayout PageLayout
    {
      get
      {
        return this._mxdService.PageLayout;
      }
    }

    public void Open()
    {
      this._mxdService.Open();
    }

    public void Close()
    {
      this._mxdService.Close();
    }

    public void ActivatePageLayout()
    {
      this._mxdService.ActivatePageLayout();
    }

    public void ActivateMap(string mapName)
    {
      this._mxdService.ActivateMap(mapName);
    }

    public void ActivateMap(int mapId)
    {
      this._mxdService.ActivateMap(mapId);
    }

    public void AddAGSLayer(string layerName, string service)
    {
      this._mxdService.AddAGSLayer(layerName, service);
    }

    public void SetQueryDefinition(string layerName, string query)
    {
      this._mxdService.SetQueryDefinition(layerName, query);
    }

    public void SetQueryDefinition(int layerId, string query)
    {
      this._mxdService.SetQueryDefinition(layerId, query);
    }

    public void SetElementText(string elementName, string text)
    {
      this._mxdService.SetElementText(elementName, text);
    }

    public void SetElementPosition(string elementName, Envelope envelope)
    {
      this._mxdService.SetElementPosition(elementName, envelope);
    }

    public void SetHorizontalAlign(string dataFrameName, string alignment, string relate)
    {
      this._mxdService.SetHorizontalAlign(dataFrameName, alignment, relate);
    }

    public void SetVerticalAlign(string dataFrameName, string alignment, string relate)
    {
      this._mxdService.SetVerticalAlign(dataFrameName, alignment, relate);
    }

    public void SetHorizontalOffset(string elementName, double offset, string relate)
    {
      this._mxdService.SetHorizontalOffset(elementName, offset, relate);
    }

    public void SetVerticalOffset(string elementName, double offset, string relate)
    {
      this._mxdService.SetVerticalOffset(elementName, offset, relate);
    }

    public Envelope GetElementEnvelope(string elementName)
    {
      return this._mxdService.GetElementEnvelope(elementName);
    }

    public void ZoomToFeature(string featureClassName, string field, string value)
    {
      this._mxdService.ZoomToFeature(featureClassName, field, value);
    }

    public void ZoomToFeature(string featureClassName, string query)
    {
      this._mxdService.ZoomToFeature(featureClassName, query);
    }

    public void ZoomPercentage(double percentage)
    {
      this._mxdService.ZoomPercentage(percentage);
    }

    public void ZoomToScale(string frame, double scale)
    {
      this._mxdService.ZoomToScale(frame, scale);
    }

    public void ZoomToBestScale(string frame, double nearScale)
    {
      this._mxdService.ZoomToBestScale(frame, nearScale);
    }

    public void ZoomToEnvelope(Envelope zoomEnv)
    {
      this._mxdService.ZoomToEnvelope(zoomEnv);
    }

    public void SetGridColor(string frame, Color color)
    {
      this._mxdService.SetGridColor(frame, color);
    }

    public void AdjustGrid(string frame, int x, int y)
    {
      this._mxdService.AdjustGrid(frame, x, y);
    }

    public void SetRasterLayer(string layer, string image)
    {
      this._mxdService.SetRasterLayer(layer, image);
    }

    public double GetMapScale(string elementName)
    {
      return this._mxdService.GetMapScale(elementName);
    }

    public void SetLayerVisibility(string layerName, bool visible)
    {
      this._mxdService.SetLayerVisibility(layerName, visible);
    }

    public void SetLayerVisibility(int layerId, bool visible)
    {
      this._mxdService.SetLayerVisibility(layerId, visible);
    }

    public bool GetLayerVisibility(string layer)
    {
      return this._mxdService.GetLayerVisibility(layer);
    }

    public bool GetLayerVisibility(int layer)
    {
      return this._mxdService.GetLayerVisibility(layer);
    }

    public void ConfigureLayerVisibility(Dictionary<string, bool> layers)
    {
      this._mxdService.ConfigureLayerVisibility(layers);
    }

    public void ConfigureLayerVisibility(Dictionary<int, bool> layers)
    {
      this._mxdService.ConfigureLayerVisibility(layers);
    }

    public void ExportToPDF(string fileName, double resolution)
    {
      this._mxdService.ExportToPDF(fileName, resolution);
    }

    public void Refresh()
    {
      this._mxdService.Refresh();
    }

    public void RefreshLegends()
    {
      this._mxdService.RefreshLegends();
    }

    public void RefreshLegends(bool scaleSymbols)
    {
      this._mxdService.RefreshLegends(scaleSymbols);
    }
  }
}
