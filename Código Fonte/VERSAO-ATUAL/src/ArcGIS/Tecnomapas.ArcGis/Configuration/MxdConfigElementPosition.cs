// Decompiled with JetBrains decompiler
// Type: Tecnomapas.ArcGis.Configuration.MxdConfigElementPosition
// Assembly: Tecnomapas.ArcGis, Version=1.0.1.0, Culture=neutral, PublicKeyToken=a4bcaf9376653225
// MVID: 7081477B-1906-455F-8CA4-7D3E408B4443
// Assembly location: C:\Users\userdefault\Desktop\WindowsServiceGeo\ProcessOperacoesGeo\bin\Debug\Tecnomapas.ArcGis.dll

using System.Configuration;

namespace Tecnomapas.ArcGis.Configuration
{
  public class MxdConfigElementPosition : ConfigurationElement
  {
    [ConfigurationProperty("valor", IsKey = true, IsRequired = true)]
    public string Valor
    {
      get
      {
        return (string) this["valor"];
      }
    }

    [ConfigurationProperty("posicao", IsKey = true, IsRequired = true)]
    public string Posicao
    {
      get
      {
        return (string) this["posicao"];
      }
    }
  }
}
