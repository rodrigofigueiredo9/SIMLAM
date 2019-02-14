// Decompiled with JetBrains decompiler
// Type: Tecnomapas.ArcGis.IMxdService
// Assembly: Tecnomapas.ArcGis, Version=1.0.1.0, Culture=neutral, PublicKeyToken=a4bcaf9376653225
// MVID: 7081477B-1906-455F-8CA4-7D3E408B4443
// Assembly location: C:\Users\userdefault\Desktop\WindowsServiceGeo\ProcessOperacoesGeo\bin\Debug\Tecnomapas.ArcGis.dll

using ESRI.ArcGIS.Carto;
using System.Collections.Generic;
using System.Drawing;

namespace Tecnomapas.ArcGis
{
  public interface IMxdService
  {
    string MxdFile { get; set; }

    IPageLayout PageLayout { get; }

    void Open();

    void Close();

    void ActivatePageLayout();

    void ActivateMap(string mapName);

    void ActivateMap(int mapId);

    void Refresh();

    void AddAGSLayer(string layerName, string service);

    void SetLayerVisibility(string layerName, bool visible);

    void SetLayerVisibility(int layerId, bool visible);

    bool GetLayerVisibility(string layer);

    bool GetLayerVisibility(int layer);

    void ConfigureLayerVisibility(Dictionary<string, bool> layers);

    void RefreshLegends();

    void RefreshLegends(bool scaleSymbols);

    void ConfigureLayerVisibility(Dictionary<int, bool> layers);

    void SetQueryDefinition(string layerName, string query);

    void SetQueryDefinition(int layerId, string query);

    void SetElementText(string elementName, string text);

    void SetElementPosition(string elementName, Envelope envelope);

    void SetHorizontalAlign(string elementName, string alignment, string relate);

    void SetVerticalAlign(string elementName, string alignment, string relate);

    void SetHorizontalOffset(string elementName, double offset, string relate);

    void SetVerticalOffset(string elementName, double offset, string relate);

    Envelope GetElementEnvelope(string elementName);

    void ZoomToFeature(string featureClassName, string field, string value);

    void ZoomToFeature(string featureClassName, string query);

    void ZoomPercentage(double percentage);

    void ZoomToScale(string frame, double scale);

    void ZoomToBestScale(string frame, double nearScale);

    void SetGridColor(string frame, Color color);

    void ZoomToEnvelope(Envelope zoomEnv);

    void AdjustGrid(string frame, int x, int y);

    void SetRasterLayer(string layer, string image);

    double GetMapScale(string elementName);

    void ExportToPDF(string fileName, double resolution);
  }
}
