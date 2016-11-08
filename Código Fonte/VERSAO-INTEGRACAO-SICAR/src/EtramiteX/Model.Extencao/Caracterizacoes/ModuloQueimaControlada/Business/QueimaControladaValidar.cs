using System;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloQueimaControlada;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloDominialidade.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloQueimaControlada.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloQueimaControlada.Business
{
	public class QueimaControladaValidar
	{
		#region Propriedades

		QueimaControladaDa _da = new QueimaControladaDa();
		CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();

		#endregion

		internal bool Salvar(QueimaControlada caracterizacao)
		{
			if (!_caracterizacaoValidar.Basicas(caracterizacao.EmpreendimentoId))
			{
				return false;
			}

			if (caracterizacao.Id <= 0 && (_da.ObterPorEmpreendimento(caracterizacao.EmpreendimentoId, true) ?? new QueimaControlada()).Id > 0)
			{
				Validacao.Add(Mensagem.Caracterizacao.EmpreendimentoCaracterizacaoJaCriada);
				return false;
			}

			if (!Acessar(caracterizacao.EmpreendimentoId))
			{
				return false;
			}

			#region Queimas Controladas

			foreach (var queima in caracterizacao.QueimasControladas)
			{
				if (!String.IsNullOrWhiteSpace(queima.AreaRequerida))
				{
					if (!ValidacoesGenericasBus.ValidarDecimal(queima.AreaRequerida, 7, 4))
					{
						Validacao.Add(Mensagem.QueimaControlada.AreaRequeridaInvalida(queima.Identificacao));
					}
					else if(Convert.ToDecimal(queima.AreaRequerida) <= 0)
					{
						Validacao.Add(Mensagem.QueimaControlada.AreaRequiridaMaiorZero(queima.Identificacao));
					}
				}
				else
				{
					Validacao.Add(Mensagem.QueimaControlada.AreaRequeridaObrigatoria(queima.Identificacao));
				}

				if (queima.Cultivos.Count <= 0) 
				{
					Validacao.Add(Mensagem.QueimaControlada.QueimaControladaCultivoObrigatoria(queima.Identificacao));
				}
			}

			#endregion


			return Validacao.EhValido;
		}

		public bool Acessar(int empreendimentoId)
		{
			if (!_caracterizacaoValidar.Dependencias(empreendimentoId, (int)eCaracterizacao.QueimaControlada))
			{
				return false;
			}

			return Validacao.EhValido;
		}

		public string AbrirModalAcessar(QueimaControlada caracterizacao)
		{
			EmpreendimentoCaracterizacao empreendimento = _caracterizacaoBus.ObterEmpreendimentoSimplificado(caracterizacao.EmpreendimentoId);

			if (empreendimento.ZonaLocalizacao == eZonaLocalizacao.Rural)
			{
				DominialidadeDa dominialidadeDa = new DominialidadeDa();
				Dominialidade dominialidade = dominialidadeDa.ObterPorEmpreendimento(caracterizacao.EmpreendimentoId);

				foreach (Dominio dominio in dominialidade.Dominios)
				{
					if (dominio.ReservasLegais.Exists(x => x.SituacaoId == (int)eReservaLegalSituacao.NaoInformada))
					{
						return Mensagem.QueimaControlada.EmpreendimentoRuralReservaIndefinida.Texto;
					}
				}
			}

			return string.Empty;
		}
	}
}