using System;
using System.Web;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloDUA;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno;

namespace Tecnomapas.EtramiteX.Credenciado.ViewModels.VMDUA
{
	public class DUAVM
	{
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		public List<Dua> DuaLst { get; set; }
		public Titulo Titulo { get; set; }

		public String ObterJSon(Dua dua)
		{
			object objeto = new
			{
				@codigo = dua.Codigo,
				@valor = dua.Valor,
				@numeroDua = dua.Numero,
				@situacao = dua.Situacao,
				@situacaoTexto = dua.SituacaoTexto,
				@validade = dua.Validade,
				@cpfCnpj = dua.CpfCnpj
			};

			return HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(objeto));
		}

		public DUAVM()
		{
			Titulo = new Titulo();
		}
	}
}