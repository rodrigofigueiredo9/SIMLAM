using System;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business
{

	public class MaterialApreendidoValidar
	{
		public bool Salvar(MaterialApreendido materialApreendido)
		{
            //if (!materialApreendido.IsApreendido.HasValue)
            //{
            //    Validacao.Add(Mensagem.MaterialApreendidoMsg.IsApreendidoObritatorio);
            //    return Validacao.EhValido;
            //}

            //if (materialApreendido.IsApreendido.Value)
            //{
				if (!materialApreendido.IsTadGeradoSistema.HasValue)
				{
					Validacao.Add(Mensagem.MaterialApreendidoMsg.IsTadGeradoSistemaObrigatorio);
				}
				else if (!materialApreendido.IsTadGeradoSistema.Value)
				{
					
                    //if (string.IsNullOrWhiteSpace(materialApreendido.NumeroTad))
                    //{
                    //    if (materialApreendido.SerieId == (int)eSerie.C)
                    //    {
                    //        Validacao.Add(Mensagem.MaterialApreendidoMsg.TadNumeroObrigatorio);
                    //    }
                    //    else
                    //    {
                    //        Validacao.Add(Mensagem.MaterialApreendidoMsg.TadNumeroBlocoObrigatorio);
                    //    }
                    //}

					ValidacoesGenericasBus.DataMensagem(materialApreendido.DataLavratura, "MaterialApreendido_DataLavratura", "Data da lavratura do termo");

				}

				if (materialApreendido.SerieId.GetValueOrDefault() == 0)
				{
					Validacao.Add(Mensagem.MaterialApreendidoMsg.SerieObrigatorio);
				}

				if (string.IsNullOrWhiteSpace(materialApreendido.Descricao))
				{
					Validacao.Add(Mensagem.MaterialApreendidoMsg.DescricaoObrigatorio);
				}

				if (!string.IsNullOrWhiteSpace(materialApreendido.ValorProdutos))
				{
					Decimal aux = 0;
					if (!Decimal.TryParse(materialApreendido.ValorProdutos, out aux)) 
					{
						Validacao.Add(Mensagem.MaterialApreendidoMsg.ValorProdutosInvalido);
					}
				}

				if (materialApreendido.Depositario.Id.GetValueOrDefault() == 0)
				{
					Validacao.Add(Mensagem.MaterialApreendidoMsg.DepositarioObrigatorio);
				}

				if (string.IsNullOrWhiteSpace(materialApreendido.Depositario.Logradouro))
				{
					Validacao.Add(Mensagem.MaterialApreendidoMsg.DepositarioLogradouroObrigatorio);
				}

				if (string.IsNullOrWhiteSpace(materialApreendido.Depositario.Bairro))
				{
					Validacao.Add(Mensagem.MaterialApreendidoMsg.DepositarioBairroObrigatorio);
				}

				if (string.IsNullOrWhiteSpace(materialApreendido.Depositario.Distrito))
				{
					Validacao.Add(Mensagem.MaterialApreendidoMsg.DepositarioDistritoObrigatorio);
				}

				if (materialApreendido.Depositario.Estado.GetValueOrDefault() == 0)
				{
					Validacao.Add(Mensagem.MaterialApreendidoMsg.DepositarioEstadoObrigatorio);
				}

				if (materialApreendido.Depositario.Municipio.GetValueOrDefault() == 0)
				{
					Validacao.Add(Mensagem.MaterialApreendidoMsg.DepositarioMunicipioObrigatorio);
				}

				if (materialApreendido.Materiais.Count == 0)
				{
					Validacao.Add(Mensagem.MaterialApreendidoMsg.MaterialApreendidoObrigatorio);
				}
            //}

			return Validacao.EhValido;
		}
	}
}
