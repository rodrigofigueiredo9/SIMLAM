// Decompiled with JetBrains decompiler
// Type: Tecnomapas.ArcGis.Configuration.MxdConfigCollection
// Assembly: Tecnomapas.ArcGis, Version=1.0.1.0, Culture=neutral, PublicKeyToken=a4bcaf9376653225
// MVID: 7081477B-1906-455F-8CA4-7D3E408B4443
// Assembly location: C:\Users\userdefault\Desktop\WindowsServiceGeo\ProcessOperacoesGeo\bin\Debug\Tecnomapas.ArcGis.dll

using System.Configuration;

namespace Tecnomapas.ArcGis.Configuration
{
  public class MxdConfigCollection : ConfigurationElementCollection
  {
    public MxdConfigCollection()
    {
      this.AddElementName = "mxd";
    }

    public MxdConfig this[int index]
    {
      get
      {
        return (MxdConfig) this.BaseGet(index);
      }
    }

    public MxdConfig this[string key]
    {
      get
      {
        return (MxdConfig) this.BaseGet((object) key);
      }
    }

    protected override void BaseAdd(ConfigurationElement element)
    {
      this.BaseAdd(element, true);
    }

    protected override object GetElementKey(ConfigurationElement element)
    {
      return (object) (element as MxdConfig).Nome;
    }

    protected override ConfigurationElement CreateNewElement()
    {
      return (ConfigurationElement) new MxdConfig();
    }
  }
}
