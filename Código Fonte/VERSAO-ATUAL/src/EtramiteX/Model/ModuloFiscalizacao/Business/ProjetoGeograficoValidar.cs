using System;
using System.Globalization;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business
{
	public class ProjetoGeograficoValidar
	{
		ProjetoGeograficoDa _da = new ProjetoGeograficoDa();
		GerenciadorConfiguracao<ConfiguracaoCaracterizacao> _caracterizacaoConfig = new GerenciadorConfiguracao<ConfiguracaoCaracterizacao>(new ConfiguracaoCaracterizacao());

		internal bool EnviarArquivo(ArquivoProjeto arquivoEnviado)
		{
			if (arquivoEnviado == null)
			{
				Validacao.Add(Mensagem.ProjetoGeografico.ArquivoObrigatorio);
				return false;
			}

			if (arquivoEnviado.Extensao != ".zip")
			{
				Validacao.Add(Mensagem.ProjetoGeografico.ArquivoAnexoNaoEhZip);
				return false;
			}
			return true;
		}

		internal bool Finalizar(ProjetoGeografico projeto)
		{
			if (_da.ObterSitacaoProjetoGeografico(projeto.Id) == (int)eProjetoGeograficoSituacao.Finalizado)
			{
				Validacao.Add(Mensagem.ProjetoGeografico.SituacaoProjetoFinalizado);
				return false;
			}
			return Validacao.EhValido;
		}

		private bool ValidarDataVerificacao(Sobreposicoes sobreposicoes)
		{
			if (string.IsNullOrEmpty(sobreposicoes.DataVerificacao))
				return false;

			DateTime dataVerificacao = DateTime.ParseExact(sobreposicoes.DataVerificacao, "dd/MM/yyyy - HH:mm", CultureInfo.CurrentCulture.DateTimeFormat);

			if (dataVerificacao < DateTime.Now.AddMinutes(-30d))
			{
				return false;
			}
			return true;
		}

		internal bool Salvar(ProjetoGeografico projeto)
		{
            if (projeto.PossuiProjetoGeo == true)
            {
                if (projeto.NivelPrecisaoId <= 0)
                {
                    Validacao.Add(Mensagem.ProjetoGeografico.NivelPrecisaoObrigatorio);
                    return false;
                }

                if (projeto.MaiorX <= 0 || projeto.MaiorY <= 0 || projeto.MenorX <= 0 || projeto.MenorY <= 0)
                {
                    Validacao.Add(Mensagem.ProjetoGeografico.AreaDeAbrangenciaObrigatorio);
                    return false;
                }

                if (projeto.MecanismoElaboracaoId == 0)
                {
                    Validacao.Add(Mensagem.ProjetoGeografico.MecanismoObrigatorio);
                    return false;
                }
            }

			return Validacao.EhValido;
		}
	}
}