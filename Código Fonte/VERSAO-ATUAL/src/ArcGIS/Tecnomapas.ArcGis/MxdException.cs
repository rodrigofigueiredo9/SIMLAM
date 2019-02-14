// Decompiled with JetBrains decompiler
// Type: Tecnomapas.ArcGis.MxdException
// Assembly: Tecnomapas.ArcGis, Version=1.0.1.0, Culture=neutral, PublicKeyToken=a4bcaf9376653225
// MVID: 7081477B-1906-455F-8CA4-7D3E408B4443
// Assembly location: C:\Users\userdefault\Desktop\WindowsServiceGeo\ProcessOperacoesGeo\bin\Debug\Tecnomapas.ArcGis.dll

using System;

namespace Tecnomapas.ArcGis
{
  internal class MxdException : Exception
  {
    public MxdException()
    {
    }

    public MxdException(string message)
      : base(message)
    {
    }

    public MxdException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
