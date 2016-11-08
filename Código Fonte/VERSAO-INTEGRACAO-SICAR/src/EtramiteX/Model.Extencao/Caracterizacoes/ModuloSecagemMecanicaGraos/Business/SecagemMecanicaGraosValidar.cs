using System;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloSecagemMecanicaGraos;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloSecagemMecanicaGraos.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloSecagemMecanicaGraos.Business
{
	public class SecagemMecanicaGraosValidar
	{
		#region Propriedades

		SecagemMecanicaGraosDa _da = new SecagemMecanicaGraosDa();
		CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
		CoordenadaAtividadeValidar _coordenadaValidar = new CoordenadaAtividadeValidar();

		#endregion

		internal bool Salvar(SecagemMecanicaGraos caracterizacao)
		{
			if (!_caracterizacaoValidar.Basicas(caracterizacao.EmpreendimentoId))
			{
				return false;
			}

			if (caracterizacao.Id <= 0 && (_da.ObterPorEmpreendimento(caracterizacao.EmpreendimentoId, true) ?? new SecagemMecanicaGraos()).Id > 0)
			{
				Validacao.Add(Mensagem.Caracterizacao.EmpreendimentoCaracterizacaoJaCriada);
				return false;
			}

			if (!Acessar(caracterizacao.EmpreendimentoId))
			{
				return false;
			}

			if (caracterizacao.Atividade <= 0)
			{
				Validacao.Add(Mensagem.SecagemMecanicaGraos.AtividadeObrigatoria);
			}

			_coordenadaValidar.Salvar(caracterizacao.CoordenadaAtividade);

			#region Secadores

			if (caracterizacao.Secadores.Count <= 0)
			{
				Validacao.Add(Mensagem.SecagemMecanicaGraos.SecadorObrigatorio);
			}

			if (!String.IsNullOrWhiteSpace(caracterizacao.NumeroSecadores))
			{
				int aux = 0;
				Int32.TryParse(caracterizacao.NumeroSecadores, out aux);

				if (aux <= 0) 
				{
					Validacao.Add(Mensagem.SecagemMecanicaGraos.NumeroSecadorMaiorZero);
				}
			}
			else 
			{
				Validacao.Add(Mensagem.SecagemMecanicaGraos.NumeroSecadorObrigatorio);
			}

			int numeroSecadores = 0;
			Int32.TryParse(caracterizacao.NumeroSecadores, out numeroSecadores);

			if (numeroSecadores < caracterizacao.Secadores.Count)
			{
				Validacao.Add(Mensagem.SecagemMecanicaGraos.NumeroSecadorMenorQueSecadoresAdicionados);
			}

			#endregion 

			#region Materias

			if (caracterizacao.MateriasPrimasFlorestais.Count <= 0) 
			{
				Validacao.Add(Mensagem.MateriaPrimaFlorestalConsumida.MateriaObrigatoria);
			}

			#endregion

			return Validacao.EhValido;

		}

		public bool Acessar(int empreendimentoId)
		{
			if (!_caracterizacaoValidar.Dependencias(empreendimentoId, (int)eCaracterizacao.SecagemMecanicaGraos))
			{
				return false;
			}

			return Validacao.EhValido;
		}
	}
}
