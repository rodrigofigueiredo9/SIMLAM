using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCadastroAmbientalRural.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloDominialidade.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCadastroAmbientalRural.Business
{
	public class CadastroAmbientalRuralValidar
	{
		CadastroAmbientalRuralDa _da = new CadastroAmbientalRuralDa();
		CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus(new CaracterizacaoValidar());

		GerenciadorConfiguracao<ConfiguracaoCaracterizacao> _caracterizacaoConfig = new GerenciadorConfiguracao<ConfiguracaoCaracterizacao>(new ConfiguracaoCaracterizacao());
		public string CaracterizacaoTipoTexto
		{
			get
			{
				return _caracterizacaoConfig
					.Obter<List<CaracterizacaoLst>>(ConfiguracaoCaracterizacao.KeyCaracterizacoes)
					.Single(x => x.Id == (int)eCaracterizacao.CadastroAmbientalRural).Texto;
			}
		}

		public bool Acessar(int empreendimentoId)
		{
			if (!_caracterizacaoBus.Validar.Dependencias(empreendimentoId, (int)eCaracterizacao.CadastroAmbientalRural))
			{
				return false;
			}

			return Validacao.EhValido;
		}

		public bool AbrirAcessar(CadastroAmbientalRural caracterizacao)
		{
			Dominialidade dominialidade = new DominialidadeBus().ObterDadosGeo(caracterizacao.EmpreendimentoId);
			if (dominialidade.AreaAPPNaoCaracterizada.MaiorToleranciaM2())
			{
				Validacao.Add(Mensagem.Caracterizacao.DominialidadeAreaAPPNaoCaracterizada(CaracterizacaoTipoTexto));
			}

			if (dominialidade.ARLNaoCaracterizada.MaiorToleranciaM2())
			{
				Validacao.Add(Mensagem.Caracterizacao.DominialidadeARLNaoCaracterizada(CaracterizacaoTipoTexto));
			}

			return Validacao.EhValido;
		}

		internal bool Salvar(CadastroAmbientalRural caracterizacao)
		{
			if (caracterizacao.EmpreendimentoId <= 0)
			{
				Validacao.Add(Mensagem.Caracterizacao.EmpreendimentoObrigatorio);
				return false;
			}

			if (caracterizacao.Id <= 0 && (_da.ObterPorEmpreendimento(caracterizacao.EmpreendimentoId, true, false) ?? new CadastroAmbientalRural()).Id > 0)
			{
				Validacao.Add(Mensagem.Caracterizacao.EmpreendimentoCaracterizacaoJaCriada);
				return false;
			}

			if (caracterizacao.MunicipioId <= 0)
			{
				Validacao.Add(Mensagem.CadastroAmbientalRural.MunicipioObrigatorio);
			}

			if (caracterizacao.ModuloFiscalId <= 0)
			{
				Validacao.Add(Mensagem.CadastroAmbientalRural.ModuloFiscalObrigatorio);
			}

			if(caracterizacao.OcorreuAlteracaoApos2008 < 0)
			{
				Validacao.Add(Mensagem.CadastroAmbientalRural.OcorreuAlteracaoApos2008Obrigatorio);
			}
			else if (caracterizacao.OcorreuAlteracaoApos2008 == 1 && caracterizacao.ATPDocumento2008.GetValueOrDefault() <= 0)
			{
				Validacao.Add(Mensagem.CadastroAmbientalRural.ATPDocumento2008Obrigatorio);
			}

			if (caracterizacao.ATPQuantidadeModuloFiscal < 0)
			{
				Validacao.Add(Mensagem.CadastroAmbientalRural.ATPQuantidadeModuloFiscalObrigatorio);
			}

			

			return Validacao.EhValido;
		}

		internal bool Processar(CadastroAmbientalRural caracterizacao)
		{
			if (caracterizacao.VistoriaAprovacaoCAR < 0)
			{
				Validacao.Add(Mensagem.CadastroAmbientalRural.VistoriaAprovacaoCARObrigatorio);
			}

			if (caracterizacao.VistoriaAprovacaoCAR > 0)
			{
				ValidacoesGenericasBus.DataMensagem(caracterizacao.DataVistoriaAprovacao, "DataVistoriaAprovacao", "vistoria");
			}

			if ((decimal.Round(caracterizacao.PercentARLEmpreendimento) < 20 && !caracterizacao.DispensaARL) && (caracterizacao.PercentARLEmpreendimento < 20 && !caracterizacao.ReservaLegalEmOutroCAR.GetValueOrDefault()))
			{
				Validacao.Add(Mensagem.CadastroAmbientalRural.DispensaARLNaoMarcado);
			}

			if (caracterizacao.DispensaARL && caracterizacao.ReservaLegalEmOutroCAR.GetValueOrDefault())
			{
				Validacao.Add(Mensagem.CadastroAmbientalRural.DispensaARL_ReservaLegalEmOutroCAR);
			}

			if (caracterizacao.IsCedente)
			{
				if (caracterizacao.ObterArea(eCadastroAmbientalRuralArea.ARL_CEDENTE_RECUPERAR).Valor > 0)
				{
					Validacao.Add(Mensagem.CadastroAmbientalRural.CedenteNaoPodeCederRLEmUso);
				}

				if (caracterizacao.ObterArea(eCadastroAmbientalRuralArea.ARL_CEDENTE_NAO_CARACTERIZADA).Valor > 0)
				{
					Validacao.Add(Mensagem.CadastroAmbientalRural.NaoPodeCederRLNaoCaracterizada);
				}
			}

			if (caracterizacao.IsReceptor)
			{
				if (caracterizacao.ObterArea(eCadastroAmbientalRuralArea.ARL_RECEPTORA_RECUPERAR).Valor > 0)
				{
					Validacao.Add(Mensagem.CadastroAmbientalRural.ReceptorNaoPodeReceberRLEmUso);
				}

				if (caracterizacao.ObterArea(eCadastroAmbientalRuralArea.ARL_RECEPTORA_NAO_CARACTERIZADA).Valor > 0)
				{
					Validacao.Add(Mensagem.CadastroAmbientalRural.ReceptorNaoPodeReceberRLNaoCaracterizada);
				}
			}

			if (caracterizacao.ReservaLegalEmOutroCAR.GetValueOrDefault() && caracterizacao.EmpreendimentoReceptorId.GetValueOrDefault() <=0 )
			{
				Validacao.Add(Mensagem.CadastroAmbientalRural.EmpreendimentoReceptorObrigatorio);
			}

			if (caracterizacao.ReservaLegalDeOutroCAR.GetValueOrDefault() && caracterizacao.EmpreendimentoCedenteId.GetValueOrDefault() <= 0)
			{
				Validacao.Add(Mensagem.CadastroAmbientalRural.EmpreendimentoCedenteObrigatorio);
			}

			if (caracterizacao.EmpreendimentoCedenteId.GetValueOrDefault() > 0)
			{
				if (!_da.PossuiTCPFRLCConcluido(caracterizacao.EmpreendimentoCedenteId.GetValueOrDefault()))
				{
					Validacao.Add(Mensagem.CadastroAmbientalRural.CedenteNaoPossuiTCPFRLCConcluido);
				}
			}

			if (caracterizacao.EmpreendimentoReceptorId.GetValueOrDefault() > 0)
			{
				if (!_da.PossuiTCPFRLCConcluido(caracterizacao.EmpreendimentoReceptorId.GetValueOrDefault()))
				{
					Validacao.Add(Mensagem.CadastroAmbientalRural.CedenteNaoPossuiTCPFRLCConcluido);
				}
			}

			if (!_caracterizacaoBus.Validar.Basicas(caracterizacao.EmpreendimentoId))
			{
				return false;
			}

			if (!Acessar(caracterizacao.EmpreendimentoId))
			{
				return false;
			}

			if (!AbrirAcessar(caracterizacao))
			{
				return false;
			}

			return Validacao.EhValido;
		}

		internal bool CancelarProcessamento(CadastroAmbientalRural caracterizacao)
		{
			return Validacao.EhValido;
		}

		internal bool Finalizar(CadastroAmbientalRural caracterizacao)
		{
			if (!Salvar(caracterizacao))
			{
				return false;
			}

			if (!Acessar(caracterizacao.EmpreendimentoId))
			{
				return false;
			}

			if (!AbrirAcessar(caracterizacao))
			{
				return false;
			}

			/*if (_da.Obter(caracterizacao.Id, simplificado: true, finalizado: true).Id > 0)
			{
				Validacao.Add(Mensagem.CadastroAmbientalRural.JaFinalizado);
			}*/

			return Validacao.EhValido;
		}

		internal bool Cancelar(CadastroAmbientalRural caracterizacao)
		{
			return Validacao.EhValido;
		}

		public bool VerificarMunicipioForaES(int MunicipioId, List<Municipio> Municipiolista)
		{
			if (!Municipiolista.Exists(x => x.Id == MunicipioId))
			{
				Validacao.Add(Mensagem.CadastroAmbientalRural.MunicipioForaES);
			}

			return Validacao.EhValido;
		}
	}
}