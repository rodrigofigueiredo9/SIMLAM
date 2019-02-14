// Decompiled with JetBrains decompiler
// Type: Tecnomapas.ArcGis.Configuration.MxdConfigExporter
// Assembly: Tecnomapas.ArcGis, Version=1.0.1.0, Culture=neutral, PublicKeyToken=a4bcaf9376653225
// MVID: 7081477B-1906-455F-8CA4-7D3E408B4443
// Assembly location: C:\Users\userdefault\Desktop\WindowsServiceGeo\ProcessOperacoesGeo\bin\Debug\Tecnomapas.ArcGis.dll

using System.Configuration;

namespace Tecnomapas.ArcGis.Configuration
{
  public class MxdConfigExporter : ConfigurationElement
  {
    [ConfigurationProperty("tipo", IsRequired = true)]
    public string Tipo
    {
      get
      {
        return (string) this["tipo"];
      }
    }

    [ConfigurationProperty("destino", IsRequired = true)]
    public string Destino
    {
      get
      {
        return (string) this["destino"];
      }
    }

    [ConfigurationProperty("nome", IsRequired = true)]
    public string NomeArquivo
    {
      get
      {
        return (string) this["nome"];
      }
    }

    [ConfigurationProperty("dpi")]
    public int Dpi
    {
      get
      {
        return (int) this["dpi"];
      }
    }
  }
}
