// Decompiled with JetBrains decompiler
// Type: Tecnomapas.ArcGis.Configuration.MxdConfigSearch
// Assembly: Tecnomapas.ArcGis, Version=1.0.1.0, Culture=neutral, PublicKeyToken=a4bcaf9376653225
// MVID: 7081477B-1906-455F-8CA4-7D3E408B4443
// Assembly location: C:\Users\userdefault\Desktop\WindowsServiceGeo\ProcessOperacoesGeo\bin\Debug\Tecnomapas.ArcGis.dll

using System.Configuration;

namespace Tecnomapas.ArcGis.Configuration
{
  public class MxdConfigSearch : ConfigurationElement
  {
    [ConfigurationProperty("conexao", IsRequired = true)]
    public string Conexao
    {
      get
      {
        return (string) this["conexao"];
      }
    }

    [ConfigurationProperty("tabela", IsRequired = true)]
    public string Tabela
    {
      get
      {
        return (string) this["tabela"];
      }
    }

    [ConfigurationProperty("campo", IsRequired = true)]
    public string Campo
    {
      get
      {
        return (string) this["campo"];
      }
    }

    [ConfigurationProperty("restricao")]
    public string Restricao
    {
      get
      {
        return (string) this["restricao"];
      }
    }

    [ConfigurationProperty("zooms")]
    public MxdConfigZoomCollection Zooms
    {
      get
      {
        return (MxdConfigZoomCollection) this["zooms"];
      }
    }
  }
}
