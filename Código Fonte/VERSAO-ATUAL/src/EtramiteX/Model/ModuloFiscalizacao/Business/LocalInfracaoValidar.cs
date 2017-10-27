using System;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business
{
	public class LocalInfracaoValidar
	{
		public bool Salvar(LocalInfracao localInfracao)
		{
			if (localInfracao.SetorId == 0)
			{
				Validacao.Add(Mensagem.LocalInfracaoMsg.SelecioneSetor);
			}

            //if (localInfracao.Data.IsEmpty || !localInfracao.Data.IsValido)
            //{
            //    Validacao.Add(Mensagem.LocalInfracaoMsg.DataFiscalizacaoObrigatoria);
            //} 
            //else if (localInfracao.Data.Data.GetValueOrDefault() > DateTime.Now)
            //{
            //    Validacao.Add(Mensagem.LocalInfracaoMsg.DataFiscalizacaoMenorAtual);
            //}

            if (localInfracao.AreaFiscalizacao == null)
            {
                Validacao.Add(Mensagem.LocalInfracaoMsg.AreaFiscalizacaoObrigatoria);
            }

            //if (!localInfracao.LonEastingToDecimal.HasValue)
            //{
            //    Validacao.Add(Mensagem.LocalInfracaoMsg.EastingUtmObrigatorio);
            //}

            //if (!localInfracao.LatNorthingToDecimal.HasValue)
            //{
            //    Validacao.Add(Mensagem.LocalInfracaoMsg.NorthingUtmObrigatorio);
            //}

            //if (localInfracao.MunicipioId == 0)
            //{
            //    Validacao.Add(Mensagem.LocalInfracaoMsg.MunicipioObrigatorio);				
            //}

            //if (string.IsNullOrWhiteSpace(localInfracao.AreaAbrangencia))
            //{
            //    Validacao.Add(Mensagem.LocalInfracaoMsg.AreaAbrangenciaObrigatoria);
            //}
            //else
            //{
            //    int areaAbran = 0;
            //    if (!int.TryParse(localInfracao.AreaAbrangencia, out areaAbran))
            //    {
            //        Validacao.Add(Mensagem.LocalInfracaoMsg.AreaAbrangenciaInvalida);
            //    }
            //    else if (areaAbran == 0)				
            //    {
            //        Validacao.Add(Mensagem.LocalInfracaoMsg.AreaAbrangenciaMaiorZero);
            //    }
            //}

            //if (string.IsNullOrWhiteSpace(localInfracao.Local))
            //{
            //    Validacao.Add(Mensagem.LocalInfracaoMsg.LocalObrigatorio);
            //}

			if (localInfracao.PessoaId.GetValueOrDefault() == 0 && localInfracao.EmpreendimentoId.GetValueOrDefault() == 0)
			{
				Validacao.Add(Mensagem.LocalInfracaoMsg.PessoaEmpreendimentoObrigatorio);
			}

			if (localInfracao.EmpreendimentoId.GetValueOrDefault() > 0 && localInfracao.ResponsavelId.GetValueOrDefault() == 0)
			{
				Validacao.Add(Mensagem.LocalInfracaoMsg.ResponsavelObrigatorio);
			}

			if (localInfracao.EmpreendimentoId.GetValueOrDefault() > 0 && localInfracao.ResponsavelPropriedadeId.GetValueOrDefault() == 0)
			{
				Validacao.Add(Mensagem.LocalInfracaoMsg.ResponsavelPropriedadeObrigatorio);
			}

			return Validacao.EhValido;
		}
	}
}
