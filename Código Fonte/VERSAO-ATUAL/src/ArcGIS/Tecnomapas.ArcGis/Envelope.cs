// Decompiled with JetBrains decompiler
// Type: Tecnomapas.ArcGis.Envelope
// Assembly: Tecnomapas.ArcGis, Version=1.0.1.0, Culture=neutral, PublicKeyToken=a4bcaf9376653225
// MVID: 7081477B-1906-455F-8CA4-7D3E408B4443
// Assembly location: C:\Users\userdefault\Desktop\WindowsServiceGeo\ProcessOperacoesGeo\bin\Debug\Tecnomapas.ArcGis.dll

namespace Tecnomapas.ArcGis
{
  public struct Envelope
  {
    private double xmin;
    private double xmax;
    private double ymin;
    private double ymax;

    public Envelope(double xmin, double xmax, double ymin, double ymax)
    {
      this.xmin = xmin;
      this.xmax = xmax;
      this.ymin = ymin;
      this.ymax = ymax;
    }

    public double Xmin
    {
      get
      {
        return this.xmin;
      }
      set
      {
        this.xmin = value;
      }
    }

    public double Xmax
    {
      get
      {
        return this.xmax;
      }
      set
      {
        this.xmax = value;
      }
    }

    public double Ymin
    {
      get
      {
        return this.ymin;
      }
      set
      {
        this.ymin = value;
      }
    }

    public double Ymax
    {
      get
      {
        return this.ymax;
      }
      set
      {
        this.ymax = value;
      }
    }
  }
}
