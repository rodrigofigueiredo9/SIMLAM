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

            if (localInfracao.AreaFiscalizacao == null)
            {
                Validacao.Add(Mensagem.LocalInfracaoMsg.AreaFiscalizacaoObrigatoria);
            }

            if (localInfracao.DentroEmpreendimento == 1 && (localInfracao.EmpreendimentoId == null || localInfracao.EmpreendimentoId == 0))
            {
                Validacao.Add(Mensagem.LocalInfracaoMsg.EmpreendimentoObrigatorio);
            }
            else if (localInfracao.DentroEmpreendimento == 0 && (localInfracao.PessoaId == 0 || localInfracao.PessoaId == null))
            {
                Validacao.Add(Mensagem.LocalInfracaoMsg.PessoaObrigatorio);
            }
            else if (localInfracao.PessoaId.GetValueOrDefault() == 0 && localInfracao.EmpreendimentoId.GetValueOrDefault() == 0)
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

            if (localInfracao.DentroEmpreendimento == 0)
            {
                if (string.IsNullOrWhiteSpace(localInfracao.AreaAbrangencia))
                {
                    Validacao.Add(Mensagem.LocalInfracaoMsg.AreaAbrangenciaObrigatoria);
                }

                if (string.IsNullOrWhiteSpace(localInfracao.LonEasting))
                {
                    Validacao.Add(Mensagem.LocalInfracaoMsg.EastingUtmObrigatorio);
                }

                if (string.IsNullOrWhiteSpace(localInfracao.LatNorthing))
                {
                    Validacao.Add(Mensagem.LocalInfracaoMsg.NorthingUtmObrigatorio);
                }

                if (localInfracao.MunicipioId == null || localInfracao.MunicipioId < 1)
                {
                    Validacao.Add(Mensagem.LocalInfracaoMsg.MunicipioObrigatorio);
                }

                if (string.IsNullOrWhiteSpace(localInfracao.Local))
                {
                    Validacao.Add(Mensagem.LocalInfracaoMsg.LocalObrigatorio);
                }
            }

			return Validacao.EhValido;
		}
	}
}
