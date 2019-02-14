// Decompiled with JetBrains decompiler
// Type: Tecnomapas.ArcGis.Configuration.MxdConfigOffset
// Assembly: Tecnomapas.ArcGis, Version=1.0.1.0, Culture=neutral, PublicKeyToken=a4bcaf9376653225
// MVID: 7081477B-1906-455F-8CA4-7D3E408B4443
// Assembly location: C:\Users\userdefault\Desktop\WindowsServiceGeo\ProcessOperacoesGeo\bin\Debug\Tecnomapas.ArcGis.dll

using System.Configuration;

namespace Tecnomapas.ArcGis.Configuration
{
  public class MxdConfigOffset : ConfigurationElement
  {
    [ConfigurationProperty("vertical", DefaultValue = 0.0)]
    public double Vertical
    {
      get
      {
        return (double) this["vertical"];
      }
    }

    [ConfigurationProperty("horizontal", DefaultValue = 0.0)]
    public double Horizontal
    {
      get
      {
        return (double) this["horizontal"];
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
