// Decompiled with JetBrains decompiler
// Type: Tecnomapas.ArcGis.Configuration.MxdConfigZoomCollection
// Assembly: Tecnomapas.ArcGis, Version=1.0.1.0, Culture=neutral, PublicKeyToken=a4bcaf9376653225
// MVID: 7081477B-1906-455F-8CA4-7D3E408B4443
// Assembly location: C:\Users\userdefault\Desktop\WindowsServiceGeo\ProcessOperacoesGeo\bin\Debug\Tecnomapas.ArcGis.dll

using System.Configuration;

namespace Tecnomapas.ArcGis.Configuration
{
  [ConfigurationCollection(typeof (MxdConfigZoom), AddItemName = "zoom", CollectionType = ConfigurationElementCollectionType.BasicMap)]
  public class MxdConfigZoomCollection : ConfigurationElementCollection
  {
    public override ConfigurationElementCollectionType CollectionType
    {
      get
      {
        return ConfigurationElementCollectionType.BasicMap;
      }
    }

    protected override string ElementName
    {
      get
      {
        return "zoom";
      }
    }

    protected override object GetElementKey(ConfigurationElement element)
    {
      MxdConfigZoom mxdConfigZoom = element as MxdConfigZoom;
      return (object) (mxdConfigZoom.DataFrame + "." + mxdConfigZoom.Layer);
    }

    protected override ConfigurationElement CreateNewElement()
    {
      return (ConfigurationElement) new MxdConfigZoom();
    }
  }
}
