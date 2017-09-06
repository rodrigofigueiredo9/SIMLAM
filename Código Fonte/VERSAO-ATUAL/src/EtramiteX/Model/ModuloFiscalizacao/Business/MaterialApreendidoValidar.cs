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
            if (materialApreendido.IsDigital == false)
            {
                if (String.IsNullOrWhiteSpace(materialApreendido.NumeroIUF))
                {
                    Validacao.Add(Mensagem.MaterialApreendidoMsg.NumeroIUFObrigatorio);
                }

                if (materialApreendido.DataLavratura.Data == null
                || materialApreendido.DataLavratura.Data == DateTime.MinValue)
                {
                    Validacao.Add(Mensagem.MaterialApreendidoMsg.DataLavraturaObrigatorio);
                }
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

            if (!string.IsNullOrWhiteSpace(materialApreendido.NumeroLacre))
            {
                String[] lacresString = materialApreendido.NumeroLacre.Split(',');

                foreach (String lacre in lacresString)
                {
                    Int32 aux = 0;
                    if (!Int32.TryParse(lacre, out aux))
                    {
                        Validacao.Add(Mensagem.MaterialApreendidoMsg.NumeroLacreInvalido);
                    }
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

            if (materialApreendido.ProdutosApreendidos.Count == 0)
            {
                Validacao.Add(Mensagem.MaterialApreendidoMsg.ProdutoApreendidoObrigatorio);
            }

            return Validacao.EhValido;
        }
	}
}
