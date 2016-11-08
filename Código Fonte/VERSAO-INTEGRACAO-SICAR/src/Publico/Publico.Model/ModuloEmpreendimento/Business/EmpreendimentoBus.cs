using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloEmpreendimento.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;

namespace Tecnomapas.EtramiteX.Publico.Model.ModuloEmpreendimento.Business
{
	public class EmpreendimentoBus
	{
		#region Propriedades

		ListaBus _busLista = new ListaBus();
		EmpreendimentoDa _da = new EmpreendimentoDa();

		public List<EmpreendimentoAtividade> Atividades
		{
			get { return _da.Atividades(); }
		}

		#endregion

		#region Obter / Filtrar

		public Resultados<Empreendimento> Filtrar(ListarEmpreendimentoFiltro filtrosListar, Paginacao paginacao, bool validarEncontrouRegistros = true)
		{
			try
			{
				if (!string.IsNullOrEmpty(filtrosListar.AreaAbrangencia))
				{
					filtrosListar.Coordenada.Datum.Sigla = _busLista.Datuns.SingleOrDefault(x => Equals(x.Id, filtrosListar.Coordenada.Datum.Id)).Sigla;
				}

				Filtro<ListarEmpreendimentoFiltro> filtro = new Filtro<ListarEmpreendimentoFiltro>(filtrosListar, paginacao);
				Resultados<Empreendimento> resultados = _da.Filtrar(filtro);

				if (validarEncontrouRegistros && resultados.Quantidade <= 0)
				{
					Validacao.Add(Mensagem.Padrao.NaoEncontrouRegistros);
				}

				return resultados;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public Empreendimento Obter(int id)
		{
			try
			{
				Empreendimento emp = _da.Obter(id);

				if (emp.Id == 0)
				{
					Validacao.Add(Mensagem.Empreendimento.Inexistente);
				}

				return emp;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		#endregion
	}
}