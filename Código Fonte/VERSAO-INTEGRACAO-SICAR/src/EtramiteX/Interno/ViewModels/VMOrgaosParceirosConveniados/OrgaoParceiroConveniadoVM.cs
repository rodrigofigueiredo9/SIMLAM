using System;
using Tecnomapas.Blocos.Entities.Interno.ModuloOrgaoParceiroConveniado;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMOrgaosParceirosConveniados
{
	public class OrgaoParceiroConveniadoVM
	{
		public bool IsVisualizar { get; set; }
		private OrgaoParceiroConveniado _orgaoParceiroConveniado;
		public OrgaoParceiroConveniado OrgaoParceiroConveniado
		{
			get { return _orgaoParceiroConveniado; }
			set { _orgaoParceiroConveniado = value; }
		}

        public String Mensagens
        {
            get
            {
                return ViewModelHelper.Json(new
                {
                    UnidadeSiglaNomeLocalObrigatorio = Mensagem.OrgaoParceiroConveniado.UnidadeSiglaNomeLocalObrigatorio,
                    UnidadeJaAdicionada = Mensagem.OrgaoParceiroConveniado.UnidadeJaAdicionada,
                    ConfirmExcluirUnidade = Mensagem.OrgaoParceiroConveniado.ConfirmExcluirUnidade
                });
            }
        }
    }
}