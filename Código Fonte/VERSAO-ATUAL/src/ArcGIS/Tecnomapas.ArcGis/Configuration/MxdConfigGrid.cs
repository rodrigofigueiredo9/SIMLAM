// Decompiled with JetBrains decompiler
// Type: Tecnomapas.ArcGis.Configuration.MxdConfigGrid
// Assembly: Tecnomapas.ArcGis, Version=1.0.1.0, Culture=neutral, PublicKeyToken=a4bcaf9376653225
// MVID: 7081477B-1906-455F-8CA4-7D3E408B4443
// Assembly location: C:\Users\userdefault\Desktop\WindowsServiceGeo\ProcessOperacoesGeo\bin\Debug\Tecnomapas.ArcGis.dll

using System.Configuration;

namespace Tecnomapas.ArcGis.Configuration
{
  public class MxdConfigGrid : ConfigurationElement
  {
    [ConfigurationProperty("x", DefaultValue = 2)]
    public int X
    {
      get
      {
        return (int) this["x"];
      }
    }

    [ConfigurationProperty("y", DefaultValue = 2)]
    public int Y
    {
      get
      {
        return (int) this["y"];
      }
    }
  }
}
