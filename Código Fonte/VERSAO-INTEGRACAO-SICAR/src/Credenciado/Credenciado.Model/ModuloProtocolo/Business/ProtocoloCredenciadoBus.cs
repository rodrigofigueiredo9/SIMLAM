using System;
using System.Web;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.ModuloProcesso;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloPessoa.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloProtocolo.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloProtocolo.Business
{
	public class ProtocoloCredenciadoBus
	{
		#region Propriedades

		PessoaInternoBus _busPessoa;
		ProtocoloInternoBus _busInterno;
		CredenciadoBus _busCredenciado;
		ProtocoloCredenciadoValidar _validar;
		ProtocoloCredenciadoDa _da;

		#endregion Propriedades

		public ProtocoloCredenciadoBus()
		{
			_busInterno = new ProtocoloInternoBus();
			_busCredenciado = new CredenciadoBus();
			_busPessoa = new PessoaInternoBus();
			_validar = new ProtocoloCredenciadoValidar();
			_da = new ProtocoloCredenciadoDa();
		}

		public EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		public IProtocolo Obter(int id, bool validarPosse = true)
		{
			IProtocolo protocolo = null;

			try
			{
				protocolo = _busInterno.Obter(id);

				if (validarPosse)
				{
					if (!_validar.EmPosse(protocolo))
					{
						return null;
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return protocolo;
		}

		public IProtocolo ObterSimplificado(int id)
		{
			IProtocolo protocolo = null;

			try
			{
				protocolo = _busInterno.ObterSimplificado(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return protocolo;
		}

		public ProtocoloLocalizacao ObterLocalizacao(int protocoloId, ProtocoloLocalizacao localizacao = null)
		{
			ProtocoloLocalizacao loc = null;

			try
			{
				loc = _busInterno.ObterLocalizacao(protocoloId, localizacao);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return loc;
		}

		public Resultados<Protocolo> Filtrar(ListarProtocoloFiltro filtrosListar, Paginacao paginacao)
		{
			try
			{
				CredenciadoPessoa credenciado = _busCredenciado.Obter(User.FuncionarioId);
				Pessoa pessoa = _busPessoa.Obter(cpfCnpj: credenciado.Pessoa.CPFCNPJ, simplificado: true);
				if (pessoa != null && pessoa.Id <= 0 && credenciado.Tipo != (int)eCredenciadoTipo.OrgaoParceiroConveniado)
				{
					// O credenciado ainda nao está no interno, retorne lista vazia, se o credenciado não for do tipo Órgão Parceiro Conveniado.
					Validacao.Add(Mensagem.Padrao.NaoEncontrouRegistros);
					return new Resultados<Protocolo>();
				}
				filtrosListar.CredenciadoPessoaId = pessoa.Id;
				filtrosListar.AutorId = credenciado.Id;

				Filtro<ListarProtocoloFiltro> filtro = new Filtro<ListarProtocoloFiltro>(filtrosListar, paginacao);
				Resultados<Protocolo> resultados = _da.Filtrar(filtro);

				if (resultados.Quantidade < 1)
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

		public Resultados<NotificacaoPendencia> FiltrarNotificacaoPendencia(int protocolo)
		{
			try
			{
				Resultados<NotificacaoPendencia> resultados = _da.FiltrarNotificacaoPendencia(protocolo);

				if (resultados.Quantidade < 1)
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

		public Resultados<HistoricoProtocolo> FiltrarHistoricoAssociados(ListarProtocoloFiltro filtrosListar)
		{
			try
			{
				Filtro<ListarProtocoloFiltro> filtro = new Filtro<ListarProtocoloFiltro>(filtrosListar);
				Resultados<HistoricoProtocolo> resultados = _da.FiltrarHistoricoAssociados(filtro);

				return resultados;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

		public bool ExisteProtocoloAtividade(int protocolo)
		{
			if (!_busInterno.ExisteAtividade(protocolo))
			{
				Validacao.Add(Mensagem.Processo.ProtocoloSemAtividade);
			}

			return Validacao.EhValido;
		}
	}
}