using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business
{

	public class MaterialApreendidoValidar
	{
        public bool Salvar(MaterialApreendido materialApreendido)
        {
            if (materialApreendido.IsDigital == null)
            {
                Validacao.Add(Mensagem.MaterialApreendidoMsg.DigitalOuBlocoObrigatorio);
            }
            else
            {
                if (materialApreendido.IsDigital == false)
                {
                    if (String.IsNullOrWhiteSpace(materialApreendido.NumeroIUF))
                    {
                        Validacao.Add(Mensagem.MaterialApreendidoMsg.NumeroIUFObrigatorio);
                    }

                    if (materialApreendido.SerieId == null || materialApreendido.SerieId == 0)
                    {
                        Validacao.Add(Mensagem.MaterialApreendidoMsg.SerieObrigatorio);
                    }

                    ValidacoesGenericasBus.DataMensagem(materialApreendido.DataLavratura, "MaterialApreendido_DataLavratura", "lavratura do IUF");
                }

                if (string.IsNullOrWhiteSpace(materialApreendido.Descricao))
                {
                    Validacao.Add(Mensagem.MaterialApreendidoMsg.DescricaoObrigatorio);
                }

                if (materialApreendido.ValorProdutosReais == null || materialApreendido.ValorProdutosReais == 0)
                {
                    Validacao.Add(Mensagem.MaterialApreendidoMsg.ValorProdutosObrigatorio);
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

                if (materialApreendido.ProdutosApreendidos.Count == 0)
                {
                    Validacao.Add(Mensagem.MaterialApreendidoMsg.ProdutoApreendidoObrigatorio);
                }

                if (String.IsNullOrWhiteSpace(materialApreendido.Opiniao))
                {
                    Validacao.Add(Mensagem.MaterialApreendidoMsg.OpiniaoObrigatoria);
                }
            }

            return Validacao.EhValido;
        }
	}
}
