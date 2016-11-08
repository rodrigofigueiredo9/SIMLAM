using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloDominialidade.Da;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloDominialidade.Business
{
	public class DominialidadeInternoBus
	{
		#region Propriedade

		DominialidadeInternoDa _da = new DominialidadeInternoDa();

		#endregion

		public DominialidadeInternoBus() { }

		#region Obter

		public Dominialidade ObterPorEmpreendimento(int empreendimento, bool simplificado = false, BancoDeDados banco = null)
		{
			Dominialidade caracterizacao = null;

			try
			{
				caracterizacao = _da.ObterPorEmpreendimento(empreendimento, simplificado: simplificado);

				foreach (Dominio dominio in caracterizacao.Dominios)
				{
					foreach (ReservaLegal reserva in dominio.ReservasLegais)
					{
						if (!string.IsNullOrEmpty(reserva.MatriculaIdentificacao))
						{
							Dominio dominioAux = caracterizacao.Dominios.SingleOrDefault(x => x.Identificacao == reserva.MatriculaIdentificacao);

							if (dominioAux == null)
							{
								continue;
							}

							reserva.MatriculaTexto = dominioAux.Matricula + " - " + dominioAux.Folha + " - " + dominioAux.Livro;
						}
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return caracterizacao;
		}

		#endregion

		public List<Lista> ObterDominiosLista(int empreendimentoInternoId, bool somenteMatriculas = false)
		{
			return _da.ObterDominiosLista(empreendimentoInternoId, somenteMatriculas);
		}

		public List<Lista> ObterARLCompensacaoLista(int empreendimentoReceptor, int dominio)
		{
			return _da.ObterARLCompensacaoLista(empreendimentoReceptor, dominio);
		}

		public ReservaLegal ObterReservaLegal(int reservaLegalId)
		{
			try
			{
				return _da.ObterReservaLegal(reservaLegalId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}
	}
}