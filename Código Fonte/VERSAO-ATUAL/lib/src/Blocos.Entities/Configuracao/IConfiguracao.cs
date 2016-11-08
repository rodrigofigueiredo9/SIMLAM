

using System;

namespace Tecnomapas.Blocos.Entities.Configuracao
{
	public interface IConfiguracao
	{
		object this[string idx] { get; }
		T Obter<T>(String key);
		object Atual(String key);
	}
}