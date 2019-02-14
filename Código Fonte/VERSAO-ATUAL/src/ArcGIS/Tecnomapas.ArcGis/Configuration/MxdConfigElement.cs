// Decompiled with JetBrains decompiler
// Type: Tecnomapas.ArcGis.Configuration.MxdConfigElement
// Assembly: Tecnomapas.ArcGis, Version=1.0.1.0, Culture=neutral, PublicKeyToken=a4bcaf9376653225
// MVID: 7081477B-1906-455F-8CA4-7D3E408B4443
// Assembly location: C:\Users\userdefault\Desktop\WindowsServiceGeo\ProcessOperacoesGeo\bin\Debug\Tecnomapas.ArcGis.dll

using System.Configuration;

namespace Tecnomapas.ArcGis.Configuration
{
  public class MxdConfigElement : ConfigurationElement
  {
    [ConfigurationProperty("nome", IsKey = true, IsRequired = true)]
    public string Nome
    {
      get
      {
        return (string) this["nome"];
      }
    }

    [ConfigurationProperty("tipo", IsRequired = true)]
    public string Tipo
    {
      get
      {
        return (string) this["tipo"];
      }
    }

    [ConfigurationProperty("texto", DefaultValue = null)]
    public string Texto
    {
      get
      {
        return (string) this["texto"];
      }
    }

    [ConfigurationProperty("valor", DefaultValue = null)]
    public string Valor
    {
      get
      {
        return (string) this["valor"];
      }
    }

    [ConfigurationProperty("definicoes", DefaultValue = null)]
    public MxdConfigElementPositionCollection Definicoes
    {
      get
      {
        return (MxdConfigElementPositionCollection) this["definicoes"];
      }
    }

    [ConfigurationProperty("alinhamentos", DefaultValue = null)]
    public MxdConfigAlignmentCollection Alinhamentos
    {
      get
      {
        return (MxdConfigAlignmentCollection) this["alinhamentos"];
      }
    }

    [ConfigurationProperty("posicoes", DefaultValue = null)]
    public MxdConfigOffsetCollection Posicoes
    {
      get
      {
        return (MxdConfigOffsetCollection) this["posicoes"];
      }
    }
  }
}
