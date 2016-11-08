using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCadastro;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloCadastro.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloCadastro.Business
{
	public class CadastroAmbientalRuralTituloValidar : EspecificidadeValidarBase, IEspecificiadeValidar
	{
		#region Propriedades

		CadastroAmbientalRuralTituloDa _da = new CadastroAmbientalRuralTituloDa();
		CaracterizacaoBus caracterizacaoBus = new CaracterizacaoBus();
		CaracterizacaoValidar caracterizacaoValidar = new CaracterizacaoValidar();

		#endregion Propriedades

		public bool Salvar(IEspecificidade especificidade)
		{
			CadastroAmbientalRuralTitulo esp = especificidade as CadastroAmbientalRuralTitulo;

			List<Dependencia> dependencias = new List<Dependencia>();
			List<Caracterizacao> caracterizacoes = caracterizacaoBus.ObterCaracterizacoesEmpreendimento(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault());
			int idCaracterizacao;

			RequerimentoAtividade(esp);

			if (Destinatario(especificidade.ProtocoloReq.Id, esp.Destinatario, "CadastroAmbientalRuralTitulo_Destinatario"))
			{
				if (!ValidarDestinatarioIsRepresentanteEmpreendimento(esp.ProtocoloReq.Id, esp.Destinatario))
				{
					Validacao.Add(Mensagem.CadastroAmbientalRuralTitulo.DestinatarioNaoEstaAssociadoEmpreendimento);
				}
			}

			if (esp.Atividades[0].Id != ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.CadastroAmbientalRural))
			{
				Validacao.Add(Mensagem.CadastroAmbientalRuralTitulo.AtividadeInvalida);
			}
			else {

				String tituloNumero = _da.ObterNumeroTituloCARExistente(esp.Titulo.EmpreendimentoId.GetValueOrDefault(0));

				if (!String.IsNullOrWhiteSpace(tituloNumero))
				{
					Validacao.Add(Mensagem.CadastroAmbientalRuralTitulo.AtividadeJaAssociadaOutroTitulo(tituloNumero));
				}
			
			}

			#region Caracterizacao

			idCaracterizacao = caracterizacaoBus.Existe(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault(), eCaracterizacao.CadastroAmbientalRural);
			if (idCaracterizacao > 0)
			{
				
				if (especificidade.Titulo.EmpreendimentoId.GetValueOrDefault() > 0)
				{
					if (_da.EmpreendimentoCaracterizacaoCARNaoFinalizada(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault()))
					{
						Validacao.Add(Mensagem.CadastroAmbientalRuralTitulo.EmpreendimentoCaracterizacaoCARNaoFinalizada);
						return Validacao.EhValido;
					}
				}

				dependencias = caracterizacaoBus.ObterDependencias(idCaracterizacao, eCaracterizacao.CadastroAmbientalRural, eCaracterizacaoDependenciaTipo.Caracterizacao);
				if (caracterizacaoValidar.DependenciasAlteradas(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault(), (int)eCaracterizacao.CadastroAmbientalRural, eCaracterizacaoDependenciaTipo.Caracterizacao, dependencias) != String.Empty)
				{
					Validacao.Add(Mensagem.CadastroAmbientalRuralTitulo.CaracterizacaoValida(caracterizacoes.Single(x => x.Tipo == eCaracterizacao.CadastroAmbientalRural).Nome));
				}
			}
			else
			{
				Validacao.Add(Mensagem.CadastroAmbientalRuralTitulo.CaracterizacaoNaoCadastrada);
			}

			idCaracterizacao = caracterizacaoBus.Existe(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault(), eCaracterizacao.Dominialidade);
			if (idCaracterizacao > 0)
			{
				if (_da.EmpreendimentoCaracterizacaoCARNaoFinalizada(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault()))
				{
					Validacao.Add(Mensagem.CadastroAmbientalRuralTitulo.EmpreendimentoCaracterizacaoCARNaoFinalizada);
					return Validacao.EhValido;
				}
				
				dependencias = caracterizacaoBus.ObterDependencias(idCaracterizacao, eCaracterizacao.Dominialidade, eCaracterizacaoDependenciaTipo.Caracterizacao);
				if (caracterizacaoValidar.DependenciasAlteradas(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault(), (int)eCaracterizacao.Dominialidade, eCaracterizacaoDependenciaTipo.Caracterizacao, dependencias) != String.Empty)
				{
					Validacao.Add(Mensagem.CadastroAmbientalRuralTitulo.CaracterizacaoValida(caracterizacoes.Single(x => x.Tipo == eCaracterizacao.Dominialidade).Nome));
				}
			}
			else
			{
				Validacao.Add(Mensagem.CadastroAmbientalRuralTitulo.DominialidadeInexistente);
			}

			#endregion

			return Validacao.EhValido;
		}

		public bool Emitir(IEspecificidade especificidade)
		{
			return Salvar(especificidade);
		}
	}
}
