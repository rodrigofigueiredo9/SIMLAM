using System;
using System.Linq;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmpreendimento.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmpreendimento.Business
{
	public class EmpreendimentoInternoBus
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys;
		EmpreendimentoInternoDa _da;

		public String UsuarioInterno
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioInterno); }
		}

		#endregion

		public EmpreendimentoInternoBus()
		{
			_configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
			_da = new EmpreendimentoInternoDa(UsuarioInterno, _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioGeo));
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

		public Empreendimento ObterSimplificado(int id)
		{
			try
			{
				Empreendimento emp = _da.ObterSimplificado(id);

				if (emp == null)
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

		public Resultados<Empreendimento> Filtrar(ListarEmpreendimentoFiltro filtrosListar, Paginacao paginacao, bool validarEncontrouRegistros = true)
		{
			try
			{
				Resultados<Empreendimento> resultados = new Resultados<Empreendimento>();

				if (!string.IsNullOrEmpty(filtrosListar.AreaAbrangencia))
				{
					filtrosListar.Coordenada.Datum.Sigla = ListaCredenciadoBus.Datuns.SingleOrDefault(x => Equals(x.Id, filtrosListar.Coordenada.Datum.Id)).Sigla;
				}

				Filtro<ListarEmpreendimentoFiltro> filtro = new Filtro<ListarEmpreendimentoFiltro>(filtrosListar, paginacao);
				resultados = _da.Filtrar(filtro);

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

		public bool ValidarEmpreendimentoCnpjLocalizar(string cnpj, int? id = null)
		{
			if (!ValidacoesGenericasBus.Cnpj(cnpj))
			{
				Validacao.Add(Mensagem.Empreendimento.CnpjInvalido);
				return Validacao.EhValido;
			}

			bool valor = ExisteEmpreendimentoCnpj(cnpj, id);

			if (!valor)
			{
				Validacao.Add(Mensagem.Empreendimento.CnpjDisponivel);
			}

			return valor;
		}

		public bool ExisteEmpreendimentoCnpj(string cnpj, int? id = null)
		{
			return _da.ExisteCnpj(cnpj, id);
		}
	}
}