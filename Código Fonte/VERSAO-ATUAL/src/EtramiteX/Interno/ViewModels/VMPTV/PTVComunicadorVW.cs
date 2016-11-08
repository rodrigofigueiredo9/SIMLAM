using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV;
using Tecnomapas.Blocos.Etx.ModuloValidacao;


namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMPTV
{
    public class PTVComunicadorVW
    {
        public PTVComunicador Comunicador { get; set; }

        public PTVConversa Conversa { get; set; }

        public bool IsVisualizar { get; set; }
        public string Mensagens
        {
            get
			{
				return ViewModelHelper.Json(new
				{
                    @JustificativaObrigatoria = Mensagem.PTV.JustificativaObrigatoria,
                    @ArquivoObrigatorio = Mensagem.Arquivo.ArquivoObrigatorio,
                    @ArquivoTipoInvalido = Mensagem.Arquivo.ArquivoTipoInvalido("Anexo", new List<string>(new string[] { ".zip", ".rar" }))
				});
			}
        }

        public PTVComunicadorVW()
        {
            Comunicador = new PTVComunicador();
        }

        public String ObterJSon(Object objeto)
        {
            return ViewModelHelper.JsSerializer.Serialize(objeto);
        }
    }
}