using Exiges.Negocios.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicenca;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicensa.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Bussiness;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicensa.Business
{
	public class BarragemDispensaLicencaValidar
	{
		BarragemDispensaLicencaDa _da = new BarragemDispensaLicencaDa();
		CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();

        internal bool Salvar(BarragemDispensaLicenca caracterizacao, int projetoDigitalId)
        {
            if (!_caracterizacaoValidar.Basicas(caracterizacao.EmpreendimentoID))
            {
                return false;
            }

			//BarragemDispensaLicenca auxiliar = _da.ObterPorEmpreendimento(caracterizacao.EmpreendimentoID, true) ?? new BarragemDispensaLicenca();

			//if (caracterizacao.Id <= 0 && auxiliar.Id > 0)
			//{
			//	Validacao.Add(Mensagem.Caracterizacao.EmpreendimentoCaracterizacaoJaCriada);
			//	return false;
			//}

			if (!Acessar(caracterizacao.EmpreendimentoID, projetoDigitalId))
			{
				return false;
			}

			if (caracterizacao.AtividadeID <= 0)
			{
				Validacao.Add(Mensagem.BarragemDispensaLicenca.SelecioneAtividade);
			}

			if ((int)caracterizacao.BarragemTipo <= 0)
			{
				Validacao.Add(Mensagem.BarragemDispensaLicenca.SelecioneTipoBarragem);
			}

			if (AreaAlagadaValida(caracterizacao.areaAlagada))
			{
				Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeAreaAlagadaZero);
			}
			
			if (VolumeArmazenadoValida(caracterizacao.volumeArmazanado))
			{
				Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeAreaAlagadaZero);
			}

			if(caracterizacao.alturaBarramento <= 0)
				Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeAlturaBarramento);
			
			if(caracterizacao.comprimentoBarramento <= 0)
				Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeComprimentoBarramento);
			
			if(caracterizacao.larguraBaseBarramento <= 0)
				Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeLarguraBaseBarramento);
			
			if(caracterizacao.larguraCristaBarramento <= 0)
				Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeLarguraCristaBarramento);

			if (caracterizacao.vazaoEnchente <= 0)
				Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeVazaoInchenteZero);

			if (caracterizacao.areaBaciaContribuicao <= 0)
				Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeAreaBaciaContribuicaoZero);

			if (caracterizacao.periodoRetorno <= 0)
				Validacao.Add(Mensagem.BarragemDispensaLicenca.InformePeriodoRetornoZero);

			if (caracterizacao.tempoConcentracao <= 0)
				Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeTempoConcentracao);

			if (string.IsNullOrWhiteSpace(caracterizacao.tempoConcentracaoEquacaoUtilizada))
				Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeTempoConcentracao);

			if (caracterizacao.coeficienteEscoamento <= 0)
				Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeCoeficienteEscoamentoZero);
			
			if (String.IsNullOrWhiteSpace(caracterizacao.fonteDadosCoeficienteEscoamento))
				Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeCoeficienteEscoamentoZeroFonteDados);

			if (caracterizacao.finalidade.Count() < 1)
				Validacao.Add(Mensagem.BarragemDispensaLicenca.SelecioneFinalidade);

			if (caracterizacao.precipitacao <= 0)
				Validacao.Add(Mensagem.BarragemDispensaLicenca.InformePrecipitacaoZero);

			if (string.IsNullOrWhiteSpace(caracterizacao.fonteDadosPrecipitacao))
				Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeTempoConcentracao);

			if (string.IsNullOrWhiteSpace(caracterizacao.tempoConcentracaoEquacaoUtilizada))
				Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeTempoConcentracao);

			if (string.IsNullOrWhiteSpace(caracterizacao.cursoHidrico))
				Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeCursoHidrico);

			caracterizacao.coordenadas.ForEach(x =>
			{
				if(x.northing <= 0)
					Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeCoordNorthing(x.tipo.Description()));
				if(x.easting <= 0)
					Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeCoordEasting(x.tipo.Description()));
			});
			
			if (caracterizacao.Fase == (int)eFase.Construida)
			{
				if (caracterizacao.construidaConstruir.isDemarcacaoAPP == 1)
				{
					if (caracterizacao.construidaConstruir.larguraDemarcada <= 0)
						Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeLarguraDemarcada);

					if (!caracterizacao.construidaConstruir.faixaCercada.HasValue)
						Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeFaixaCercada);

					if (String.IsNullOrWhiteSpace(caracterizacao.construidaConstruir.descricacaoDesenvolvimentoAPP))
						Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeDescricaoApp);
				}

				if (!caracterizacao.construidaConstruir.barramentoNormas.HasValue)
				{
					Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeBarramentoNormas);
				}
				else
				{
					if (String.IsNullOrWhiteSpace(caracterizacao.construidaConstruir.barramentoAdequacoes))
						Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeBarramentoAdequacoes);
				}

				if (!Validacao.EhValido) return false;

				if (caracterizacao.construidaConstruir.vazaoMinTipo <= 0)
					Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeVazaoMinTipo);

				if (caracterizacao.construidaConstruir.vazaoMinDiametro <= 0)
					Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeVazaoMinDiametro);

				if (!caracterizacao.construidaConstruir.vazaoMinInstalado.HasValue)
				{
					Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeVazaoMinInstalado);
				}
				else if(caracterizacao.construidaConstruir.vazaoMinInstalado == true)
				{
					if (!caracterizacao.construidaConstruir.vazaoMinNormas.HasValue)
						Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeVazaoMinNormas);
					else if (caracterizacao.construidaConstruir.vazaoMinNormas == true)
					{
						if (String.IsNullOrWhiteSpace(caracterizacao.construidaConstruir.vazaoMinAdequacoes))
							Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeVazaoMinAdequacoes);
					}
				}

				if (caracterizacao.construidaConstruir.vazaoMaxTipo <= 0)
					Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeVazaoMaxTipo);

				if (caracterizacao.construidaConstruir.vazaoMaxDiametro <= 0)
					Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeVazaoMaxDiametro);

				if (!caracterizacao.construidaConstruir.vazaoMaxInstalado.HasValue)
				{
					Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeVazaoMaxInstalado);
				}
				else if(caracterizacao.construidaConstruir.vazaoMaxInstalado == true)
				{
					if (!caracterizacao.construidaConstruir.vazaoMaxNormas.HasValue)
						Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeVazaoMaxNormas);
					else if (caracterizacao.construidaConstruir.vazaoMaxNormas == true)
					{
						if (String.IsNullOrWhiteSpace(caracterizacao.construidaConstruir.vazaoMaxAdequacoes))
							Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeVazaoMaxAdequacoes);
					}
				}
			}
			else
			{
				if (caracterizacao.construidaConstruir.vazaoMinTipo <= 0)
					Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeVazaoMinTipo);

				if (caracterizacao.construidaConstruir.vazaoMinDiametro <= 0)
					Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeVazaoMinDiametro);

				if (caracterizacao.construidaConstruir.vazaoMaxTipo <= 0)
					Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeVazaoMaxTipo);

				if (caracterizacao.construidaConstruir.vazaoMaxDiametro <= 0)
					Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeVazaoMaxDiametro);

				if (caracterizacao.construidaConstruir.mesInicioObra <= 0)
					Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeMesInicio);
				else
				{
					if (caracterizacao.construidaConstruir.mesInicioObra > 12 || caracterizacao.construidaConstruir.mesInicioObra < 0)
						Validacao.Add(Mensagem.BarragemDispensaLicenca.MesInicioInvalido);
				}

				if (caracterizacao.construidaConstruir.anoInicioObra <= 0)
					Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeAnoInicio);
				else
				{
					if (caracterizacao.construidaConstruir.anoInicioObra > 2100 || caracterizacao.construidaConstruir.anoInicioObra < 1900)
						Validacao.Add(Mensagem.BarragemDispensaLicenca.AnoInicioInvalido);
				}
			}

			if (!Validacao.EhValido) return false;
			var profissoesSemAutorizacao = new List<int>() { 15, 37, 38};

			caracterizacao.responsaveisTecnicos.ForEach(x =>
			{
				if(x.tipo == eTipoRT.ElaboracaoDeclaracao || x.tipo == eTipoRT.ElaboracaoProjeto || x.tipo == eTipoRT.ElaboracaoEstudoAmbiental)
				{
					if(String.IsNullOrWhiteSpace(x.nome))
						Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeNomeRT(x.tipo.Description()));
					if(x.profissao.Id <= 0)
						Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeProfissapRT(x.tipo.Description()));
					if (String.IsNullOrWhiteSpace(x.registroCREA))
						Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeCREART(x.tipo.Description()));
					if (String.IsNullOrWhiteSpace(x.numeroART))
						Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeNumeroART(x.tipo.Description()));
				}
				if (x.tipo == eTipoRT.ElaboracaoProjeto &&
					!profissoesSemAutorizacao.Contains(x.profissao.Id) &&
					x.autorizacaoCREA.Id <= 0)
						Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeNumeroART(x.tipo.Description()));
			});

			return Validacao.EhValido;
        }

		public bool Acessar(int empreendimentoId, int projetoDigitalId)
		{
			return _caracterizacaoValidar.Dependencias(empreendimentoId, projetoDigitalId, (int)eCaracterizacao.BarragemDispensaLicenca);
		}

        internal bool CopiarDadosInstitucional(BarragemDispensaLicenca caracterizacao)
        {
            if (caracterizacao.InternoID <= 0)
            {
                Validacao.Add(Mensagem.BarragemDispensaLicenca.CopiarCaractizacaoCadastrada);
            }

            return Validacao.EhValido;
        }

		internal bool AreaAlagadaValida(decimal area) => 
			area < Convert.ToDecimal(0.01) || area > _da.AreaAlagadaConfiguracao(area);

		internal bool VolumeArmazenadoValida(decimal area) => 
			area < Convert.ToDecimal(0.01) && area > _da.VolumeArmazenadoConfiguracao(area);
			
		
	}
}