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

            BarragemDispensaLicenca auxiliar = _da.ObterPorEmpreendimento(caracterizacao.EmpreendimentoID, true) ?? new BarragemDispensaLicenca();

            //if (caracterizacao.Id <= 0 && auxiliar.Id > 0)
            //{
            //    Validacao.Add(Mensagem.Caracterizacao.EmpreendimentoCaracterizacaoJaCriada);
            //    return false;
            //}

            if (!Acessar(caracterizacao.EmpreendimentoID, projetoDigitalId))
            {
                return false;
            }

            if (caracterizacao.AtividadeID <= 0)
            {
                Validacao.Add(Mensagem.BarragemDispensaLicenca.SelecioneAtividade);
            }

            if (!caracterizacao.BarragemTipo.HasValue)
            {
                Validacao.Add(Mensagem.BarragemDispensaLicenca.SelecioneTipoBarragem);
            }

            if (caracterizacao.FinalidadeAtividade <= 0)
            {
                Validacao.Add(Mensagem.BarragemDispensaLicenca.SelecioneFinalidade);
            }

            if (string.IsNullOrWhiteSpace(caracterizacao.CursoHidrico))
            {
                Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeCursoHidrico);
            }

            if (caracterizacao.VazaoEnchente.GetValueOrDefault() <= 0)
            {
                Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeVazaoInchenteZero);
            }

            if (caracterizacao.AreaBaciaContribuicao.GetValueOrDefault() <= 0)
            {
                Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeAreaBaciaContribuicaoZero);
            }

            if (caracterizacao.Precipitacao.GetValueOrDefault() <= 0)
            {
                Validacao.Add(Mensagem.BarragemDispensaLicenca.InformePrecipitacaoZero);
            }

            if (caracterizacao.PeriodoRetorno.GetValueOrDefault() <= 0)
            {
                Validacao.Add(Mensagem.BarragemDispensaLicenca.InformePeriodoRetornoZero);
            }

            if (string.IsNullOrWhiteSpace(caracterizacao.CoeficienteEscoamento))
            {
                Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeCoeficienteEscoamentoZero);
            }

            if (string.IsNullOrWhiteSpace(caracterizacao.TempoConcentracao))
            {
                Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeTempoConcentracao);
            }

            if (string.IsNullOrWhiteSpace(caracterizacao.EquacaoCalculo))
            {
                Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeEquacaoCalculo);
            }

            if (caracterizacao.AreaAlagada.GetValueOrDefault() <= 0)
            {
                Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeAreaAlagadaZero);
            }

            if (caracterizacao.VolumeArmazanado.GetValueOrDefault() <= 0)
            {
                Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeVolumeArmazenadoZero);
            }

            if (caracterizacao.Fase.GetValueOrDefault() <= 0)
            {
                Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeFase);
            }
            if (caracterizacao.Fase.HasValue)
            {
                switch (caracterizacao.Fase.GetValueOrDefault())
                {
                    case (int)eFase.Construida:
                        #region FASE: Construida
                        if (!caracterizacao.PossuiMonge.HasValue)
                        {
                            Validacao.Add(Mensagem.BarragemDispensaLicenca.InformePossuiMonge);
                        }
                        else if (Convert.ToBoolean(caracterizacao.PossuiMonge))
                        {
                            if (!caracterizacao.MongeTipo.HasValue || caracterizacao.MongeTipo.GetValueOrDefault() <= 0)
                            {
                                Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeTipoMonge);
                            }
                            else if (caracterizacao.MongeTipo.GetValueOrDefault() == (int)eMongeTipo.Outros && string.IsNullOrWhiteSpace(caracterizacao.EspecificacaoMonge))
                            {
                                Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeEspecificacaoMonge);
                            }
                        }

                        if (!caracterizacao.PossuiVertedouro.HasValue)
                        {
                            Validacao.Add(Mensagem.BarragemDispensaLicenca.InformePossuiVertedouro);
                        }
                        else if (Convert.ToBoolean(caracterizacao.PossuiVertedouro))
                        {
                            if (!caracterizacao.VertedouroTipo.HasValue || caracterizacao.VertedouroTipo <= 0)
                            {
                                Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeTipoVertedouro);
                            }
                            else if (caracterizacao.VertedouroTipo == (int)eVertedouroTipo.Outros && string.IsNullOrWhiteSpace(caracterizacao.EspecificacaoVertedouro))
                            {
                                Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeEspecificacaoVertedouro);
                            }
                        }

                        if ((caracterizacao.PossuiMonge.HasValue && !Convert.ToBoolean(caracterizacao.PossuiMonge)) ||
                            (caracterizacao.PossuiVertedouro.HasValue && !Convert.ToBoolean(caracterizacao.PossuiVertedouro)))
                        {
                            if (!caracterizacao.PossuiEstruturaHidraulica.HasValue)
                            {
                                Validacao.Add(Mensagem.BarragemDispensaLicenca.InformePossuiEstruturaHidraulica);
                            }
                        }

                        if ((caracterizacao.PossuiMonge.HasValue && !Convert.ToBoolean(caracterizacao.PossuiMonge)) ||
                            (caracterizacao.PossuiVertedouro.HasValue && !Convert.ToBoolean(caracterizacao.PossuiVertedouro)) ||
                            (caracterizacao.PossuiEstruturaHidraulica.HasValue && !Convert.ToBoolean(caracterizacao.PossuiEstruturaHidraulica)))
                        {
                            if (string.IsNullOrWhiteSpace(caracterizacao.AdequacoesRealizada))
                            {
                                Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeAdequacoesRealizada);
                            }
                        }
                        #endregion
                        break;
                    case (int)eFase.AConstruir:
                        #region FASE: A construir
                        if (!caracterizacao.MongeTipo.HasValue || caracterizacao.MongeTipo.GetValueOrDefault() <= 0)
                        {
                            Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeTipoMonge);
                        }
                        else if (caracterizacao.MongeTipo == (int)eMongeTipo.Outros && string.IsNullOrWhiteSpace(caracterizacao.EspecificacaoMonge))
                        {
                            Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeEspecificacaoMonge);
                        }

                        if (!caracterizacao.VertedouroTipo.HasValue || caracterizacao.VertedouroTipo <= 0)
                        {
                            Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeTipoVertedouro);
                        }
                        else if (caracterizacao.VertedouroTipo == (int)eVertedouroTipo.Outros && string.IsNullOrWhiteSpace(caracterizacao.EspecificacaoVertedouro))
                        {
                            Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeEspecificacaoVertedouro);
                        }

                        #region datas da obra
                        DateTime testeDataInicio;
                        if (string.IsNullOrWhiteSpace(caracterizacao.DataInicioObra))
                        {
                            Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeDataInicioObra);
                        }
                        else
                        {
                            if (!DateTime.TryParse(caracterizacao.DataInicioObra, out testeDataInicio))
                            {
                                Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeDataInicioObraFormatoValido);
                            }
                        }

                        DateTime testeDataTermino;
                        if (string.IsNullOrWhiteSpace(caracterizacao.DataPrevisaoTerminoObra))
                        {
                            Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeDataPrevisaoTerminoObra);
                        }
                        else
                        {
                            if (!DateTime.TryParse(caracterizacao.DataPrevisaoTerminoObra, out testeDataTermino))
                            {
                                Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeDataPrevisaoTerminoObraFormatoValido);
                            }
                        }

                        if (DateTime.TryParse(caracterizacao.DataInicioObra, out testeDataInicio) && DateTime.TryParse(caracterizacao.DataPrevisaoTerminoObra, out testeDataTermino))
                        {
                            if (testeDataInicio > testeDataTermino)
                            {
                                Validacao.Add(Mensagem.BarragemDispensaLicenca.PeriodoObraValido);
                            }
                        }
                        #endregion

                        #endregion
                        break;
                }
            }

            if (string.IsNullOrWhiteSpace(caracterizacao.Coordenada.EastingUtmTexto))
            {
                Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeCoordEasting);
            }

            if (string.IsNullOrWhiteSpace(caracterizacao.Coordenada.NorthingUtmTexto))
            {
                Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeCoordNorthing);
            }

            if (caracterizacao.FormacaoRT <= 0)
            {
                Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeFormacaoRT);
            }

            if (((Convert.ToInt32(eFormacaoRTCodigo.Outros) & caracterizacao.FormacaoRT) != 0) && string.IsNullOrWhiteSpace(caracterizacao.EspecificacaoRT))
            {
                Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeEspecificacaoRT);
            }

            if (caracterizacao.Autorizacao == null || string.IsNullOrWhiteSpace(caracterizacao.Autorizacao.Nome))
            {
                Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeArquivo);
            }

            if (string.IsNullOrWhiteSpace(caracterizacao.NumeroARTElaboracao))
            {
                Validacao.Add(Mensagem.BarragemDispensaLicenca.InformeNumeroARTElaboracao);
            }

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
	}
}