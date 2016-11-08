using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloOutros;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloOutros.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloOutros.Business
{
	public class OutrosInformacaoCorteValidar : EspecificidadeValidarBase, IEspecificiadeValidar
	{
		public bool Salvar(IEspecificidade especificidade)
		{
			#region Variáveis

			OutrosInformacaoCorteDa _da = new OutrosInformacaoCorteDa();
			CaracterizacaoBus caracterizacaoBus = new CaracterizacaoBus();
			CaracterizacaoValidar caracterizacaoValidar = new CaracterizacaoValidar();
			List<Dependencia> dependencias = new List<Dependencia>();
			OutrosInformacaoCorte esp = especificidade as OutrosInformacaoCorte;
			List<Caracterizacao> caracterizacoes = caracterizacaoBus.ObterCaracterizacoesEmpreendimento(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault());
			int idCaracterizacao;

			#endregion

			RequerimentoAtividade(esp);
			Destinatario(esp.ProtocoloReq.Id, esp.Destinatario, "Outros_Destinatarios");

			// Atividade Informação de corte
			if (esp.Atividades[0].Id != ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.InformacaoDeCorte))
			{
				Validacao.Add(Mensagem.OutrosInformacaoCorte.AtividadeInvalida(esp.Atividades[0].NomeAtividade));
			}

			idCaracterizacao = caracterizacaoBus.Existe(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault(), eCaracterizacao.InformacaoCorte);
			if (idCaracterizacao > 0)
			{
				dependencias = caracterizacaoBus.ObterDependencias(idCaracterizacao, eCaracterizacao.InformacaoCorte, eCaracterizacaoDependenciaTipo.Caracterizacao);
				if (caracterizacaoValidar.DependenciasAlteradas(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault(), (int)eCaracterizacao.InformacaoCorte, eCaracterizacaoDependenciaTipo.Caracterizacao, dependencias) != String.Empty)
				{
					Validacao.Add(Mensagem.OutrosInformacaoCorte.CaracterizacaoValida(caracterizacoes.Single(x => x.Tipo == eCaracterizacao.InformacaoCorte).Nome));
				}
			}
			else
			{
				Validacao.Add(Mensagem.OutrosInformacaoCorte.CaracterizacaoCadastrada);
			}

			if (esp.InformacaoCorte == 0)
			{
				Validacao.Add(Mensagem.OutrosInformacaoCorte.InformacaoCorteObrigatorio);
			}
			else if (!_da.IsInformacaoCorteCadastrado(esp.InformacaoCorte))
			{
				Validacao.Add(Mensagem.OutrosInformacaoCorte.InformacaoCorteInexistente);
			}
			else if (_da.IsInformacaoCorteAssociado(esp.InformacaoCorte, esp.Id))
			{
				Validacao.Add(Mensagem.OutrosInformacaoCorte.InformacaoCorteAssociado);
			}


			return Validacao.EhValido;
		}

		public bool Emitir(IEspecificidade especificidade)
		{
			if (ExisteProcDocFilhoQueFoiDesassociado(especificidade.Titulo.Id))
			{
				Validacao.Add(Mensagem.Especificidade.ProtocoloReqFoiDesassociado);
			}

			return Salvar(especificidade);
		}
	}
}