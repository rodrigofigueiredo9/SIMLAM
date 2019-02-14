// Decompiled with JetBrains decompiler
// Type: Tecnomapas.ArcGis.Configuration.MxdConfigZoom
// Assembly: Tecnomapas.ArcGis, Version=1.0.1.0, Culture=neutral, PublicKeyToken=a4bcaf9376653225
// MVID: 7081477B-1906-455F-8CA4-7D3E408B4443
// Assembly location: C:\Users\userdefault\Desktop\WindowsServiceGeo\ProcessOperacoesGeo\bin\Debug\Tecnomapas.ArcGis.dll

using System.Configuration;

namespace Tecnomapas.ArcGis.Configuration
{
  public class MxdConfigZoom : ConfigurationElement
  {
    [ConfigurationProperty("dataframe", DefaultValue = "", IsRequired = true)]
    public string DataFrame
    {
      get
      {
        return (string) this["dataframe"];
      }
    }

    [ConfigurationProperty("layer", IsRequired = true)]
    public string Layer
    {
      get
      {
        return (string) this["layer"];
      }
    }

    [ConfigurationProperty("restricao", IsRequired = true)]
    public string Restricao
    {
      get
      {
        return (string) this["restricao"];
      }
    }

    [ConfigurationProperty("posRestricao")]
    public string PosRestricao
    {
      get
      {
        return (string) this["posRestricao"];
      }
    }

    [ConfigurationProperty("zoom", DefaultValue = 1)]
    public int Zoom
    {
      get
      {
        return (int) this["zoom"];
      }
    }

    [ConfigurationProperty("grid")]
    public MxdConfigGrid Grid
    {
      get
      {
        return (MxdConfigGrid) this["grid"];
      }
    }
  }
}
