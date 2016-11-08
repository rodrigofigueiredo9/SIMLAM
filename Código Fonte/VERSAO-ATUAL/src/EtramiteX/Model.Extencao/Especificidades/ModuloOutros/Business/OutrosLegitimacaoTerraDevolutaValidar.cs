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
	public class OutrosLegitimacaoTerraDevolutaValidar : EspecificidadeValidarBase, IEspecificiadeValidar
	{
		public bool Salvar(IEspecificidade especificidade)
		{
			#region Variáveis

			OutrosLegitimacaoTerraDevolutaDa _da = new OutrosLegitimacaoTerraDevolutaDa();
			CaracterizacaoBus caracterizacaoBus = new CaracterizacaoBus();
			CaracterizacaoValidar caracterizacaoValidar = new CaracterizacaoValidar();
			List<Dependencia> dependencias = new List<Dependencia>();
			OutrosLegitimacaoTerraDevoluta esp = especificidade as OutrosLegitimacaoTerraDevoluta;
			List<Caracterizacao> caracterizacoes = caracterizacaoBus.ObterCaracterizacoesEmpreendimento(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault());
			int idCaracterizacao;
			string titulo = string.Empty;

			#endregion

			#region Requerimento

			//O requerimento <nº> não está mais associado ao processo <nº>.
			//A atividade <nome> não está mais associada ao processo.
			//A atividade <nome> deve estar na situação ‘em andamento’.
			//Este modelo de título não foi solicitado para a atividade <nome>’.

			RequerimentoAtividade(esp, apenasObrigatoriedade: true);

			#endregion

			#region Atividade

			//O modelo de título <nome do modelo> não pode ser utilizado para atividade <nome da atividade selecionada na especificidade do título>.
			if (!ConfiguracaoAtividade.ObterId(new[] {
				(int)eAtividadeCodigo.RegularizacaoFundiariaRural,
				(int)eAtividadeCodigo.RegularizacaoFundiariaUrbana,
				(int)eAtividadeCodigo.RegularizacaoFundiariaRuralOriundaDeProcessoDiscriminatorio,
				(int)eAtividadeCodigo.RegularizacaoFundiariaUrbanaOriundaDeProcessoDiscriminatorio
			}).Any(x => x == esp.Atividades[0].Id))
			{
				Validacao.Add(Mensagem.OutrosLegitimacaoTerraDevolutaMsg.AtividadeInvalida(esp.Atividades[0].NomeAtividade));
			}

			#endregion

			#region Destinatários

			if (esp.Destinatarios.Count == 0)
			{
				Validacao.Add(Mensagem.Especificidade.DestinatarioObrigatorio("Outros_Destinatarios"));
			}
			else
			{
				esp.Destinatarios.ForEach(x =>
				{
					Destinatario(esp.ProtocoloReq.Id, x.Id, "Outros_Destinatarios");
				});
			}

			#endregion

			#region Caracterização

			//A caracterização de <nome da caracterização> deve estar cadastrada.
			//Para cadastrar este modelo de título é necessário ter os dados da caracterização <nome da caracterização dependente> válidos.

			idCaracterizacao = caracterizacaoBus.Existe(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault(), eCaracterizacao.RegularizacaoFundiaria);
			if (idCaracterizacao > 0)
			{
				dependencias = caracterizacaoBus.ObterDependencias(idCaracterizacao, eCaracterizacao.RegularizacaoFundiaria, eCaracterizacaoDependenciaTipo.Caracterizacao);
				if (caracterizacaoValidar.DependenciasAlteradas(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault(), (int)eCaracterizacao.RegularizacaoFundiaria, eCaracterizacaoDependenciaTipo.Caracterizacao, dependencias) != String.Empty)
				{
					Validacao.Add(Mensagem.OutrosLegitimacaoTerraDevolutaMsg.CaracterizacaoValida(caracterizacoes.Single(x => x.Tipo == eCaracterizacao.RegularizacaoFundiaria).Nome));
				}
			}
			else
			{
				Validacao.Add(Mensagem.OutrosLegitimacaoTerraDevolutaMsg.CaracterizacaoCadastrada);
			}

			#endregion

			#region Posse

			if (esp.Dominio == 0)
			{
				Validacao.Add(Mensagem.OutrosLegitimacaoTerraDevolutaMsg.DominioObrigatorio);
			}
			else
			{
				if (!_da.IsDominioCadastrado(esp.Dominio))
				{
					Validacao.Add(Mensagem.OutrosLegitimacaoTerraDevolutaMsg.DominioInexistente);
				}

				TituloEsp tituloAux = _da.IsDominioAssociado(esp.Dominio, esp.Titulo.Id);
				switch ((eTituloSituacao)tituloAux.SituacaoId)
				{
					case eTituloSituacao.Cadastrado:
					case eTituloSituacao.Emitido:
					case eTituloSituacao.Assinado:
						Validacao.Add(Mensagem.OutrosLegitimacaoTerraDevolutaMsg.PosseAssociadaSituacao(tituloAux.SituacaoTexto));
						break;

					case eTituloSituacao.Concluido:
					case eTituloSituacao.Prorrogado:
						Validacao.Add(Mensagem.OutrosLegitimacaoTerraDevolutaMsg.PosseAssociadaNumero(tituloAux.Numero.Texto));
						break;
				}
			}

			#endregion

			if (string.IsNullOrWhiteSpace(esp.ValorTerreno))
			{
				Validacao.Add(Mensagem.OutrosLegitimacaoTerraDevolutaMsg.ValorTerrenoObrigatorio);
			}

			if (!esp.IsInalienabilidade.HasValue)
			{
				Validacao.Add(Mensagem.OutrosLegitimacaoTerraDevolutaMsg.IsInalienabilidadeObrigatorio);
			}

			if(esp.MunicipioGlebaId <= 0)
			{
				Validacao.Add(Mensagem.OutrosLegitimacaoTerraDevolutaMsg.MunicipioGlebaObrigatorio);
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