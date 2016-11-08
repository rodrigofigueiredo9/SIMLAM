using System;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;

namespace Tecnomapas.Blocos.Etx.ModuloCore.Business
{
	public class EnderecoBus
	{
		IEnderecoValidar _validar = null;

		public EnderecoBus(IEnderecoValidar validacao)
		{
			_validar = validacao;
		}

		public EnderecoBus(){}

		public Int32 ObterSetorId(String sigla) 
		{
			return new EnderecoDa().ObterSetorId(sigla);
		}
	}
}