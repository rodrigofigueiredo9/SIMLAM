// Decompiled with JetBrains decompiler
// Type: Tecnomapas.ArcGis.Configuration.MxdConfig
// Assembly: Tecnomapas.ArcGis, Version=1.0.1.0, Culture=neutral, PublicKeyToken=a4bcaf9376653225
// MVID: 7081477B-1906-455F-8CA4-7D3E408B4443
// Assembly location: C:\Users\userdefault\Desktop\WindowsServiceGeo\ProcessOperacoesGeo\bin\Debug\Tecnomapas.ArcGis.dll

using System.Configuration;

namespace Tecnomapas.ArcGis.Configuration
{
  public class MxdConfig : ConfigurationElement
  {
    [ConfigurationProperty("nome", IsRequired = true)]
    public string Nome
    {
      get
      {
        return (string) this["nome"];
      }
    }

    [ConfigurationProperty("localMxd", IsRequired = true)]
    public string LocalMxd
    {
      get
      {
        return (string) this["localMxd"];
      }
    }

    [ConfigurationProperty("exportador", IsRequired = true)]
    public MxdConfigExporter Exportador
    {
      get
      {
        return (MxdConfigExporter) this["exportador"];
      }
    }

    [ConfigurationProperty("posicoes")]
    public MxdConfigPositionDefinitionCollection Posicoes
    {
      get
      {
        return (MxdConfigPositionDefinitionCollection) this["posicoes"];
      }
    }

    [ConfigurationProperty("queries")]
    public MxdConfigQueryCollection Queries
    {
      get
      {
        return (MxdConfigQueryCollection) this["queries"];
      }
    }

    [ConfigurationProperty("pesquisa")]
    public MxdConfigSearch Pesquisa
    {
      get
      {
        return (MxdConfigSearch) this["pesquisa"];
      }
    }

    [ConfigurationProperty("elementos")]
    public MxdConfigElementCollection Elementos
    {
      get
      {
        return (MxdConfigElementCollection) this["elementos"];
      }
    }
  }
}
