using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloLaudo;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloLaudo.Business
{
	public class LaudoAuditoriaFomentoFlorestalValidar : EspecificidadeValidarBase, IEspecificiadeValidar
	{
		LaudoAuditoriaFomentoFlorestal _da = new LaudoAuditoriaFomentoFlorestal();
		GerenciadorConfiguracao<ConfiguracaoCaracterizacao> _caracterizacaoConfig = new GerenciadorConfiguracao<ConfiguracaoCaracterizacao>(new ConfiguracaoCaracterizacao());
		GerenciadorConfiguracao<ConfiguracaoProcesso> _atividadeConfig = new GerenciadorConfiguracao<ConfiguracaoProcesso>(new ConfiguracaoProcesso());

		public bool Salvar(IEspecificidade especificidade)
		{
			#region Validacoes Genericas

			LaudoAuditoriaFomentoFlorestal esp = especificidade as LaudoAuditoriaFomentoFlorestal;
			RequerimentoAtividade(esp, false, true);

			Destinatario(especificidade.ProtocoloReq.Id, esp.Destinatario, "Laudo_Destinatario");

			ValidacoesGenericasBus.DataMensagem(esp.DataVistoria, "Laudo_DataVistoria_DataTexto", "vistoria");

			#endregion

			#region Objetivo

			if (String.IsNullOrWhiteSpace(esp.Objetivo))
			{
				Validacao.Add(Mensagem.LaudoAuditoriaFomentoFlorestal.ObjetivoObrigatorio);
			}

			#endregion

			#region PlantioAPP

			if (!esp.PlantioAPP.HasValue)
			{
				Validacao.Add(Mensagem.LaudoAuditoriaFomentoFlorestal.PlantioAPPObrigatorio);
			}
			else 
			{
				if (esp.PlantioAPP.Value == 1)
				{

					if (!String.IsNullOrWhiteSpace(esp.PlantioAPPArea))
					{
						Decimal aux = 0;
						if (!Decimal.TryParse(esp.PlantioAPPArea, out aux)) 
						{
							Validacao.Add(Mensagem.LaudoAuditoriaFomentoFlorestal.PlantioAPPAreaInvalida);
						}

						if (aux <= 0) 
						{
							Validacao.Add(Mensagem.LaudoAuditoriaFomentoFlorestal.PlantioAPPAreaMaiorZero);
						}
					}
					else 
					{
						Validacao.Add(Mensagem.LaudoAuditoriaFomentoFlorestal.PlantioAPPAreaObrigatoria);
					}
				}
			}

			#endregion

			#region PlantioMudasEspeciesFlorestNativas

			if (!esp.PlantioMudasEspeciesFlorestNativas.HasValue)
			{
				Validacao.Add(Mensagem.LaudoAuditoriaFomentoFlorestal.PlantioMudasEspeciesFlorestNativasObrigatorio);
			}
			else 
			{
				if (esp.PlantioMudasEspeciesFlorestNativas.Value == 1) 
				{
					if (!String.IsNullOrWhiteSpace(esp.PlantioMudasEspeciesFlorestNativasArea))
					{
						Decimal aux = 0;
						if (!Decimal.TryParse(esp.PlantioMudasEspeciesFlorestNativasArea, out aux)) 
						{
							Validacao.Add(Mensagem.LaudoAuditoriaFomentoFlorestal.PlantioMudasEspeciesFlorestNativasAreaInvalida);
						}

						if (aux <= 0) 
						{
							Validacao.Add(Mensagem.LaudoAuditoriaFomentoFlorestal.PlantioMudasEspeciesFlorestNativasAreaMaiorZero);
						}
					}
					else 
					{
						Validacao.Add(Mensagem.LaudoAuditoriaFomentoFlorestal.PlantioMudasEspeciesFlorestNativasAreaObrigatoria);
					}

					if (!String.IsNullOrWhiteSpace(esp.PlantioMudasEspeciesFlorestNativasQtd))
					{
						Decimal aux = 0;
						if (!Decimal.TryParse(esp.PlantioMudasEspeciesFlorestNativasQtd, out aux))
						{
							Validacao.Add(Mensagem.LaudoAuditoriaFomentoFlorestal.PlantioMudasEspeciesFlorestNativasQtdInvalida);
						}

						if (aux <= 0)
						{
							Validacao.Add(Mensagem.LaudoAuditoriaFomentoFlorestal.PlantioMudasEspeciesFlorestNativasQtdMaiorZero);
						}
					}
					else
					{
						Validacao.Add(Mensagem.LaudoAuditoriaFomentoFlorestal.PlantioMudasEspeciesFlorestNativasQtdObrigatoria);
					}
				}
			}

			#endregion

			#region PreparoSolo

			if (!esp.PreparoSolo.HasValue)
			{
				Validacao.Add(Mensagem.LaudoAuditoriaFomentoFlorestal.PreparoSoloObrigatorio);
			}
			else 
			{
				if (esp.PreparoSolo.Value == 1) 
				{
					if (!String.IsNullOrWhiteSpace(esp.PreparoSoloArea))
					{
						Decimal aux = 0;
						if (!Decimal.TryParse(esp.PreparoSoloArea, out aux))
						{
							Validacao.Add(Mensagem.LaudoAuditoriaFomentoFlorestal.PreparoSoloAreaInvalida);
						}

						if (aux <= 0)
						{
							Validacao.Add(Mensagem.LaudoAuditoriaFomentoFlorestal.PreparoSoloAreaMaiorZero);
						}
					}
					else
					{
						Validacao.Add(Mensagem.LaudoAuditoriaFomentoFlorestal.PreparoSoloAreaObrigatoria);
					}
				}
			}

			#endregion

			#region Resultado 

			if (esp.ResultadoTipo <= 0)
			{
				Validacao.Add(Mensagem.LaudoAuditoriaFomentoFlorestal.ResultadoObrigatorio);
			}
			else
			{
				if (esp.ResultadoTipo == (int)eEspecificidadeResultado.NaoConforme)
				{
					if (String.IsNullOrWhiteSpace(esp.ResultadoQuais))
					{
						Validacao.Add(Mensagem.LaudoAuditoriaFomentoFlorestal.ResultadoQuaisObrigatorio);
					}
				}
			}

			#endregion

			#region Caracterizacao

			CaracterizacaoBus caracterizacaoBus = new CaracterizacaoBus();
			int caracterizacao = caracterizacaoBus.Existe(esp.Titulo.EmpreendimentoId.GetValueOrDefault(), eCaracterizacao.SilviculturaATV);

			if (caracterizacao <= 0)
			{
				Validacao.Add(Mensagem.LaudoAuditoriaFomentoFlorestal.CaracterizacaoCadastrada);
			}
			else
			{
				CaracterizacaoValidar caracterizacaoValidar = new CaracterizacaoValidar();
				List<Dependencia> dependencias = caracterizacaoBus.ObterDependencias(caracterizacao, eCaracterizacao.SilviculturaATV,eCaracterizacaoDependenciaTipo.Caracterizacao);

				string retorno = caracterizacaoValidar.DependenciasAlteradas(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault(),
					(int)eCaracterizacao.SilviculturaATV,
					eCaracterizacaoDependenciaTipo.Caracterizacao,
					dependencias);

				if (!string.IsNullOrEmpty(retorno))
				{
					List<CaracterizacaoLst> caracterizacoes = _caracterizacaoConfig.Obter<List<CaracterizacaoLst>>(ConfiguracaoCaracterizacao.KeyCaracterizacoes);
					Validacao.Add(Mensagem.LaudoAuditoriaFomentoFlorestal.CaracterizacaoInvalida(caracterizacoes.SingleOrDefault(x => x.Id == (int)eCaracterizacao.SilviculturaATV).Texto));
				}
			}

			#endregion

			#region Atividade

			foreach (var atividade in esp.Atividades)
			{
				if (atividade.Id != 0 && atividade.Id != ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.ImplantacaoAtividadeSilviculturaReferenteAoFomentoFlorestal))
				{
					List<ProcessoAtividadeItem> atividades = _atividadeConfig.Obter<List<ProcessoAtividadeItem>>(ConfiguracaoProcesso.KeyAtividadesProcesso);
					Validacao.Add(Mensagem.LaudoAuditoriaFomentoFlorestal.AtividadeInvalida(atividades.SingleOrDefault(x => x.Id == atividade.Id).Texto));
				}
			}

			#endregion

			return Validacao.EhValido;
		}

		public bool Emitir(IEspecificidade especificidade)
		{
			Salvar(especificidade);

			return Validacao.EhValido;
		}
	}
}