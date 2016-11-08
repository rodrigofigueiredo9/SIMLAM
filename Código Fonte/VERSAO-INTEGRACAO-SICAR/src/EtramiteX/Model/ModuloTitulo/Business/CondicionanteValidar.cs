using System;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Business
{
	public class CondicionanteValidar
	{
		CondicionanteDa _da = new CondicionanteDa();

		internal bool DescricaoSalvar(TituloCondicionanteDescricao descricao)
		{
			if (String.IsNullOrWhiteSpace(descricao.Texto))
			{
				Validacao.Add(Mensagem.Condicionante.DescricaoObrigatorio);
			}
			return Validacao.EhValido;
		}

		internal bool Salvar(TituloCondicionante condicionante)
		{
			// Só validar data de criação se for uma nova condicionante
			if (condicionante.Id <= 0)
			{
				if (condicionante.DataCriacao == null || condicionante.DataCriacao.IsEmpty)
				{
					Validacao.Add(Mensagem.Condicionante.DataCriacaoObrigatoria);
				}
				else if (!condicionante.DataCriacao.IsValido)
				{
					Validacao.Add(Mensagem.Condicionante.DataCriacaoInvalida);
				}				
			}

			if (String.IsNullOrWhiteSpace(condicionante.Descricao))
			{
				Validacao.Add(Mensagem.Condicionante.DescricaoObrigatorio);
			}
			else if (condicionante.Descricao.Length > 4000)
			{
				Validacao.Add(Mensagem.Condicionante.DescricaoMuitoGrande(4000));
			}
			
			if (condicionante.PossuiPrazo)
			{
				if (condicionante.Situacao.Id == 2 || condicionante.Situacao.Id == 3)
				{
					if (condicionante.DataVencimento.IsEmpty)
					{
						Validacao.Add(Mensagem.Condicionante.DataVencimentoObrigatoria);
					}
					else if (!condicionante.DataVencimento.IsValido)
					{
						Validacao.Add(Mensagem.Condicionante.DataVencimentoInvalida);
					}
				}
			
				if (condicionante.Prazo == null || !condicionante.Prazo.HasValue || condicionante.Prazo.Value <= 0)
				{
					Validacao.Add(Mensagem.Condicionante.PrazoValorObrigatorio);
				}

				if (condicionante.PossuiPeriodicidade)
				{
					if (condicionante.PeriodicidadeValor == null || !condicionante.PeriodicidadeValor.HasValue || condicionante.PeriodicidadeValor.Value <= 0)
					{
						Validacao.Add(Mensagem.Condicionante.PeriodicidadeValorObrigatorio);
					}
					if (condicionante.PeriodicidadeTipo == null || condicionante.PeriodicidadeTipo.Id <= 0)
					{
						Validacao.Add(Mensagem.Condicionante.PeriodicidadeTipoObrigatorio);
					}
				}
			}
			
			return Validacao.EhValido;
		}

		internal bool Prorrogar(TituloCondicionante cond, TituloCondicionantePeriodicidade periodicidade, int? diasProrrogados)
		{
			if (cond == null)
			{
				Validacao.Add(Mensagem.Condicionante.Inexistente);
				return false;
			}

			if (!IdValido(cond.Id))
			{
				return Validacao.EhValido;
			}

			if (!cond.PossuiPrazo)
			{
				Validacao.Add(Mensagem.Condicionante.ProrrogarNaoPossuiPrazo);
				return Validacao.EhValido;
			}

			if (cond.Situacao.Id == 1 || !cond.DataVencimento.Data.HasValue)
			{
				Validacao.Add(Mensagem.Condicionante.NaoAtiva);
			}

			if (diasProrrogados == null || !diasProrrogados.HasValue || diasProrrogados.Value == 0)
			{
				Validacao.Add(Mensagem.Condicionante.ProrrogarDiasObrigatorio);
			}

			if (cond.Situacao.Id == 4 || cond.Situacao.Id == 5)
			{
				Validacao.Add(Mensagem.Condicionante.ProrrogarAtentidaEncerrada);
			}

			if (cond.PossuiPeriodicidade)
			{
				if (periodicidade != null && periodicidade.Situacao.Id == 4 || periodicidade.Situacao.Id == 5)
				{
					Validacao.Add(Mensagem.Condicionante.ProrrogarAtentidaEncerrada);
				}
			}

			return Validacao.EhValido;
		}

		internal bool IdValido(int? condicionanteId)
		{
			if (condicionanteId == null || !condicionanteId.HasValue || condicionanteId <= 0)
			{
				Validacao.Add(Mensagem.Condicionante.Invalida);
			}
			return Validacao.EhValido;
		}

		internal bool Existe(int condicionanteId)
		{
			TituloCondicionante cond = _da.Obter(condicionanteId);

			if (cond == null || cond.Id <= 0)
			{
				Validacao.Add(Mensagem.Condicionante.Inexistente);
			}
			return Validacao.EhValido;
		}

		internal bool Atender(TituloCondicionante cond, TituloCondicionantePeriodicidade periodicidade)
		{
			if (cond == null || cond.Id <= 0)
			{
				Validacao.Add(Mensagem.Condicionante.Inexistente);
				return false;
			}

			IdValido(cond.Id);

			if (cond.Situacao.Id == 4)//Atendida
			{
				Validacao.Add(Mensagem.Condicionante.JaAtendida);
				return false;
			}

			if (cond.Situacao.Id != 2)//Ativa
			{
				Validacao.Add(Mensagem.Condicionante.AtenderNaoAtiva(cond.Situacao.Texto));
				return false;
			}

			if (periodicidade != null)
			{
				if (periodicidade.Situacao.Id == 5)
				{
					Validacao.Add(Mensagem.Condicionante.AtenderEncerrada);
				}
			}

			return Validacao.EhValido;
		}

		internal bool AssociarTitulo(TituloCondicionante condicionante)
		{
			Salvar(condicionante);
			return Validacao.EhValido;
		}

		internal bool DescricaoExcluir(int id)
		{
			TituloCondicionanteDescricao condDesc = new TituloCondicionanteDescricao() { Id = id };

			return DescricaoExcluir(condDesc);
		}

		internal bool DescricaoExcluir(TituloCondicionanteDescricao condDesc)
		{
			if (condDesc == null || condDesc.Id <= 0)
			{
				Validacao.Add(Mensagem.Condicionante.DescricaoInexistente);
			}
			else
			{
				condDesc = _da.DescricaoObter(condDesc.Id);
				if (condDesc == null || condDesc.Id <= 0)
				{
					Validacao.Add(Mensagem.Condicionante.DescricaoInexistente);
				}
			}
			return Validacao.EhValido;
		}

		internal bool DescricaoEditar(TituloCondicionanteDescricao condDesc)
		{
			if (condDesc == null || condDesc.Id <= 0)
			{
				Validacao.Add(Mensagem.Condicionante.DescricaoInexistente);
			}
			else
			{
				condDesc = _da.DescricaoObter(condDesc.Id);
				if (condDesc == null || condDesc.Id <= 0)
				{
					Validacao.Add(Mensagem.Condicionante.DescricaoInexistente);
				}
			}

			return Validacao.EhValido;
		}

		internal bool DescricaoEditar(int id)
		{
			return DescricaoEditar(new TituloCondicionanteDescricao() { Id = id });
		}
	}
}
