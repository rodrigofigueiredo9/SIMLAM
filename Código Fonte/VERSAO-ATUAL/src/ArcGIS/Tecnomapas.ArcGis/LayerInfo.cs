// Decompiled with JetBrains decompiler
// Type: Tecnomapas.ArcGis.LayerInfo
// Assembly: Tecnomapas.ArcGis, Version=1.0.1.0, Culture=neutral, PublicKeyToken=a4bcaf9376653225
// MVID: 7081477B-1906-455F-8CA4-7D3E408B4443
// Assembly location: C:\Users\userdefault\Desktop\WindowsServiceGeo\ProcessOperacoesGeo\bin\Debug\Tecnomapas.ArcGis.dll

namespace Tecnomapas.ArcGis
{
  public class LayerInfo
  {
    private int id;
    private string name;
    private string source;
    private bool enabled;
    private string type;
    private double minScale;
    private double maxScale;

    public int ID
    {
      get
      {
        return this.id;
      }
      set
      {
        this.id = value;
      }
    }

    public string Name
    {
      get
      {
        return this.name;
      }
      set
      {
        this.name = value;
      }
    }

    public string Source
    {
      get
      {
        return this.source;
      }
      set
      {
        this.source = value;
      }
    }

    public bool Enabled
    {
      get
      {
        return this.enabled;
      }
      set
      {
        this.enabled = value;
      }
    }

    public string Type
    {
      get
      {
        return this.type;
      }
      set
      {
        this.type = value;
      }
    }

    public double MinScale
    {
      get
      {
        return this.minScale;
      }
      set
      {
        this.minScale = value;
      }
    }

    public double MaxScale
    {
      get
      {
        return this.maxScale;
      }
      set
      {
        this.maxScale = value;
      }
    }
  }
}
