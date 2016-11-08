using System.Collections.Generic;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.Blocos.Etx.ModuloCore.Entities
{
	public class ResponseJsonData<T>
	{
		public List<Mensagem> Erros { get; set; }
		public T Data { get; set; }
	}
}
