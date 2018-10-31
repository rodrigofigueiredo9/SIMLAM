using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloAutorizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloAutorizacao.Business
{
	public class AutorizacaoExploracaoFlorestalValidar : EspecificidadeValidarBase, IEspecificiadeValidar
	{
		EspecificidadeDa _daEspecificidade = new EspecificidadeDa();

		public bool Salvar(IEspecificidade especificidade)
		{
			AutorizacaoExploracaoFlorestal esp = especificidade as AutorizacaoExploracaoFlorestal;
			CaracterizacaoValidar caracterizacaoValidar = new CaracterizacaoValidar();
			CaracterizacaoBus caracterizacaoBus = new CaracterizacaoBus();
			List<Caracterizacao> caracterizacoes = caracterizacaoBus.ObterCaracterizacoesEmpreendimento(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault());
			List<Dependencia> dependencias = new List<Dependencia>();
			String caracterizacoesAlteradas = String.Empty;
			int idCaracterizacao;

			RequerimentoAtividade(esp, solicitado: false, jaAssociado: false, atividadeAndamento: false);

			if (!String.IsNullOrWhiteSpace(esp.Observacao) && esp.Observacao.Length > 500)
				Validacao.Add(Mensagem.AutorizacaoExploracaoFlorestal.ObservacaoMuitoGrande);

			Destinatario(especificidade.ProtocoloReq.Id, esp.Destinatario, "Autorizacao_Destinatario");

			if ((esp.TitulosAssociado.FirstOrDefault() ?? new TituloAssociadoEsp()).Id <= 0)
			{
				Validacao.Add(Mensagem.AutorizacaoExploracaoFlorestal.LaudoVistoriaObrigatorio);
			}
			else if (_daEspecificidade.ObterTituloAssociado(esp.TitulosAssociado.FirstOrDefault().Id).Situacao != (int) eTituloSituacao.Concluido)
			{
				Validacao.Add(Mensagem.AutorizacaoExploracaoFlorestal.LaudoVIstoriaDeveEstarConcluiddo);
			}

			idCaracterizacao = caracterizacaoBus.Existe(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault(), eCaracterizacao.Dominialidade);
			if (idCaracterizacao > 0)
			{
				dependencias = caracterizacaoBus.ObterDependencias(idCaracterizacao, eCaracterizacao.Dominialidade, eCaracterizacaoDependenciaTipo.Caracterizacao);
				if (caracterizacaoValidar.DependenciasAlteradas(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault(), (int)eCaracterizacao.Dominialidade, eCaracterizacaoDependenciaTipo.Caracterizacao, dependencias) != String.Empty)
				{
					Validacao.Add(Mensagem.AutorizacaoExploracaoFlorestal.CaracterizacaoDeveEstarValida(caracterizacoes.Single(x => x.Tipo == eCaracterizacao.Dominialidade).Nome));
				}
			}
			else
			{
				Validacao.Add(Mensagem.AutorizacaoExploracaoFlorestal.DominialidadeInexistente);
			}

			idCaracterizacao = caracterizacaoBus.Existe(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault(), eCaracterizacao.ExploracaoFlorestal);
			if (idCaracterizacao > 0)
			{
				dependencias = caracterizacaoBus.ObterDependencias(idCaracterizacao, eCaracterizacao.ExploracaoFlorestal, eCaracterizacaoDependenciaTipo.Caracterizacao);
				if (caracterizacaoValidar.DependenciasAlteradas(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault(), (int)eCaracterizacao.ExploracaoFlorestal, eCaracterizacaoDependenciaTipo.Caracterizacao, dependencias) != String.Empty)
				{
					Validacao.Add(Mensagem.AutorizacaoExploracaoFlorestal.CaracterizacaoDeveEstarValida(caracterizacoes.Single(x => x.Tipo == eCaracterizacao.ExploracaoFlorestal).Nome));
				}
			}
			else
			{
				Validacao.Add(Mensagem.AutorizacaoExploracaoFlorestal.ExploracaoInexistente);
			}

			if (caracterizacoesAlteradas != String.Empty)
			{
				Validacao.Add(Mensagem.Caracterizacao.CaracterizacaoAlterada);
			}

			return Validacao.EhValido;
		}

		public bool Emitir(IEspecificidade especificidade)
		{
			Salvar(especificidade);

			return Validacao.EhValido;
		}
	}
}
