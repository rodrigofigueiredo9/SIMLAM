﻿// Decompiled with JetBrains decompiler
// Type: Tecnomapas.ArcGis.Configuration.MxdConfigOffsetCollection
// Assembly: Tecnomapas.ArcGis, Version=1.0.1.0, Culture=neutral, PublicKeyToken=a4bcaf9376653225
// MVID: 7081477B-1906-455F-8CA4-7D3E408B4443
// Assembly location: C:\Users\userdefault\Desktop\WindowsServiceGeo\ProcessOperacoesGeo\bin\Debug\Tecnomapas.ArcGis.dll

using System.Configuration;

namespace Tecnomapas.ArcGis.Configuration
{
  [ConfigurationCollection(typeof (MxdConfigOffset), AddItemName = "posicao", CollectionType = ConfigurationElementCollectionType.BasicMap)]
  public class MxdConfigOffsetCollection : ConfigurationElementCollection
  {
    public override ConfigurationElementCollectionType CollectionType
    {
      get
      {
        return ConfigurationElementCollectionType.BasicMap;
      }
    }

    protected override string ElementName
    {
      get
      {
        return "posicao";
      }
    }

    protected override object GetElementKey(ConfigurationElement element)
    {
      return (object) (element as MxdConfigOffset).Valor;
    }

    protected override ConfigurationElement CreateNewElement()
    {
      return (ConfigurationElement) new MxdConfigOffset();
    }
  }
}
