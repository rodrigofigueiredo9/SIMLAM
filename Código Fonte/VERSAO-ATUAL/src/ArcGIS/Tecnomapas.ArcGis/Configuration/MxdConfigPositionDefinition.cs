// Decompiled with JetBrains decompiler
// Type: Tecnomapas.ArcGis.Configuration.MxdConfigPositionDefinition
// Assembly: Tecnomapas.ArcGis, Version=1.0.1.0, Culture=neutral, PublicKeyToken=a4bcaf9376653225
// MVID: 7081477B-1906-455F-8CA4-7D3E408B4443
// Assembly location: C:\Users\userdefault\Desktop\WindowsServiceGeo\ProcessOperacoesGeo\bin\Debug\Tecnomapas.ArcGis.dll

using System.Configuration;

namespace Tecnomapas.ArcGis.Configuration
{
  public class MxdConfigPositionDefinition : ConfigurationElement
  {
    [ConfigurationProperty("nome", IsKey = true, IsRequired = true)]
    public string Nome
    {
      get
      {
        return (string) this["nome"];
      }
    }

    [ConfigurationProperty("posicao", IsRequired = true)]
    public MxdConfigOffset Posicao
    {
      get
      {
        return (MxdConfigOffset) this["posicao"];
      }
    }

    [ConfigurationProperty("alinhamento", IsRequired = true)]
    public MxdConfigAlignment Alinhamento
    {
      get
      {
        return (MxdConfigAlignment) this["alinhamento"];
      }
    }
  }
}
