// Decompiled with JetBrains decompiler
// Type: Tecnomapas.ArcGis.Configuration.MxdConfigAlignment
// Assembly: Tecnomapas.ArcGis, Version=1.0.1.0, Culture=neutral, PublicKeyToken=a4bcaf9376653225
// MVID: 7081477B-1906-455F-8CA4-7D3E408B4443
// Assembly location: C:\Users\userdefault\Desktop\WindowsServiceGeo\ProcessOperacoesGeo\bin\Debug\Tecnomapas.ArcGis.dll

using System.Configuration;

namespace Tecnomapas.ArcGis.Configuration
{
  public class MxdConfigAlignment : ConfigurationElement
  {
    [ConfigurationProperty("vertical", DefaultValue = "")]
    public string Vertical
    {
      get
      {
        return (string) this["vertical"];
      }
    }

    [ConfigurationProperty("horizontal", DefaultValue = "")]
    public string Horizontal
    {
      get
      {
        return (string) this["horizontal"];
      }
    }

    [ConfigurationProperty("relacao")]
    public string Relacao
    {
      get
      {
        return (string) this["relacao"];
      }
    }

    [ConfigurationProperty("valor")]
    public string Valor
    {
      get
      {
        return (string) this["valor"];
      }
    }
  }
}
