using System;
using System.Collections.Generic;
using System.Web;
using Tecnomapas.Blocos.Arquivo.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloProcesso;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTramitacao.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Business
{
	public class ProtocoloBus
	{
		#region Propriedades

		ProtocoloValidar _validar;
		ProtocoloDa _da;

		public static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		#endregion

		public ProtocoloBus()
		{
			_da = new ProtocoloDa();
			_validar = new ProtocoloValidar();
		}

		#region Obter / Filtrar

		public IProtocolo ObterProcessosDocumentos(int processo)
		{
			try
			{
				return _da.ObterProcessosDocumentos(processo);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public Resultados<Protocolo> Filtrar(ListarProtocoloFiltro filtrosListar, Paginacao paginacao)
		{
			try
			{
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

		public IProtocolo Obter(int id)
		{
			IProtocolo protocolo = null;

			try
			{
				protocolo = _da.Obter(id);

				if ((protocolo.Id ?? 0) <= 0)
				{
					Validacao.Add(Mensagem.Processo.Inexistente);
				}
				else
				{
					if (protocolo.Arquivo != null && protocolo.Arquivo.Id > 0)
					{
						ArquivoDa _arquivoDa = new ArquivoDa();
						protocolo.Arquivo = _arquivoDa.Obter(protocolo.Arquivo.Id.Value);
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return protocolo;
		}

		public IProtocolo Obter(string numero)
		{
			IProtocolo protocolo = null;

			try
			{
				protocolo = _da.Obter(_da.ExisteProtocolo(numero));
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return protocolo;
		}

		public IProtocolo ObterAtividades(int id)
		{
			IProtocolo protocolo = null;
			try
			{
				protocolo = _da.ObterAtividades(id);
				if ((protocolo.Id ?? 0) <= 0)
				{
					Validacao.Add(Mensagem.Processo.Inexistente);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return protocolo;
		}

		public IProtocolo ObterSimplificado(int id, bool suprimirMensagemInexistente = false)
		{
			IProtocolo protocolo = null;

			try
			{
				protocolo = _da.ObterSimplificado(id);

				if (!suprimirMensagemInexistente && (protocolo.Id ?? 0) <= 0)
				{
					Validacao.Add(Mensagem.Processo.Inexistente);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return protocolo;
		}

		public Finalidade ObterTituloAnteriorAtividade(int atividade, int processo, int modelo)
		{
			try
			{
				return _da.ObterTituloAnteriorAtividade(atividade, processo, modelo);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

		public ProtocoloLocalizacao ObterLocalizacao(int protocoloId, ProtocoloLocalizacao localizacao = null)
		{
			ProtocoloLocalizacao loc = localizacao == null ? new ProtocoloLocalizacao() : localizacao;
			IProtocolo protocolo = ObterSimplificado(protocoloId);

			if (loc.ProcessoPaiId <= 0)
			{
				int ApensadoEmProcessoId = ProcessoApensado(protocoloId);
				if (ApensadoEmProcessoId > 0)
				{
					loc.ProcessoPaiId = ApensadoEmProcessoId;
					loc.ProcessoPaiNumero = ObterSimplificado(loc.ProcessoPaiId).Numero;
					return ObterLocalizacao(loc.ProcessoPaiId, loc);
				}
			}

			TramitacaoBus _busTramitacao = new TramitacaoBus(new TramitacaoValidar());

			loc.Tramitacao.Id = _busTramitacao.ObterTramitacaoProtocolo(protocoloId);
			if (loc.Tramitacao.Id > 0)
			{
				loc.Tramitacao = _busTramitacao.Obter(loc.Tramitacao.Id);
				if (loc.Tramitacao.SituacaoId == (int)eTramitacaoSituacao.Arquivado)
				{
					loc.Localizacao = eLocalizacaoProtocolo.Arquivado;
					ArquivarBus _arquivarBus = new ArquivarBus();
					Arquivar arquivamento = _arquivarBus.ObterArquivamento(loc.Tramitacao.Id);
					loc.ArquivoNome = arquivamento.ArquivoNome;
				}
				else if (loc.Tramitacao.SituacaoId == (int)eTramitacaoSituacao.ParaOrgaoExterno)
				{
					loc.Localizacao = eLocalizacaoProtocolo.OrgaoExterno;
					loc.OrgaoExternoNome = loc.Tramitacao.OrgaoExterno.Texto;
				}
				else if (loc.Tramitacao.SituacaoId == (int)eTramitacaoSituacao.Tramitando)
				{
					if (loc.Tramitacao.Destinatario.Id == 0)
					{
						loc.Localizacao = eLocalizacaoProtocolo.EnviadoParaSetor;
					}
					else
					{
						loc.Localizacao = eLocalizacaoProtocolo.EnviadoParaFuncionario;
						loc.FuncionarioDestinatarioNome = loc.Tramitacao.Destinatario.Nome;
					}
				}
			}
			else // se não existir tramitação, ele está na posse de algum funcionário
			{
				loc.Localizacao = eLocalizacaoProtocolo.PosseFuncionario;
				TramitacaoPosse posse = _busTramitacao.ObterProtocoloPosse(protocoloId);
				loc.FuncionarioDestinatarioNome = posse.FuncionarioNome;
				loc.SetorDestinatarioNome = posse.SetorNome;
			}
			return loc;
		}

		public List<PessoaLst> ObterInteressadoRepresentantes(int protocolo)
		{
			try
			{
				return _da.ObterInteressadoRepresentantes(protocolo);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public IProtocolo ObterJuntadosApensados(int id, BancoDeDados banco = null)
		{
			return _da.ObterJuntadosApensados(id, banco);
		}

		public IProtocolo ObterProcessosDocumentos(int protocolo, BancoDeDados banco = null)
		{
			return _da.ObterProcessosDocumentos(protocolo, banco);
		}

		internal int ObterSetor(int? protocolo)
		{
			return _da.ObterSetor(protocolo ?? 0);
		}

		public List<PessoaLst> ObterResponsaveisTecnicos(int id)
		{
			try
			{
				return _da.ObterResposaveisTecnicos(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<PessoaLst> ObterResponsaveisTecnicosPorRequerimento(int id)
		{
			try
			{
				return _da.ObterResponsaveisTecnicosPorRequerimento(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<PessoaLst> ObterResponsaveisTecnicos(string numero)
		{
			try
			{
				return _da.ObterResposaveisTecnicos(_da.ExisteProtocolo(numero));
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}
		
		public List<Requerimento> ObterProtocoloRequerimentos(int id)
		{
			try
			{
				return _da.ObterProtocoloRequerimentos(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<Requerimento> ObterProtocoloRequerimentos(string numero)
		{
			try
			{
				return _da.ObterProtocoloRequerimentos(_da.ExisteProtocolo(numero));
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<Requerimento> ObterRequerimentosPorProtocolo(int protocoloId)
		{
			List<Requerimento> requerimentos = new List<Requerimento>();

			try
			{
				requerimentos = _da.ObterProtocoloRequerimentos(protocoloId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return requerimentos;

		}

		public List<Atividade> ObterAtividadesSolicitadas(int protocoloId)
		{
			List<Atividade> atividades = new List<Atividade>();

			try
			{
				atividades = _da.ObterAtividadesSolicitadas(protocoloId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return atividades;
		}

		public IProtocolo ObterAtividadesProtocolo(int id, BancoDeDados banco = null)
		{
			try
			{
				return _da.ObterAtividades(id, banco);
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return null;
		}

		public List<Atividade> ObterProtocoloAtividades(int requerimentoId, BancoDeDados banco = null)
		{
			try
			{
				return _da.ObterProtocoloAtividades(requerimentoId, banco);
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return null;
		}

		public List<Atividade> ObterListAtividadesProtocolo(int processoId, BancoDeDados banco = null) 
		{
			try
			{
				return _da.ObterAtividadesProtocolo(processoId, banco);
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return null;
		}

		public ProtocoloNumero ObterProtocolo(int id)
		{
			try
			{
				return _da.ObterProtocolo(id);
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return null;
		}

		#endregion

		#region Verificar / Validar

		internal string ObterNumeroProcessoPai(int? id)
		{
			return _validar.ObterNumeroProcessoPai(id);
		}

		public bool ValidarAssociarResponsavelTecnico(int id)
		{
			return _validar.ValidarAssociarResponsavelTecnico(id);
		}

		public bool ValidarCheckList(int checkListId, int protocoloId)
		{
			return _validar.ValidarCheckList(checkListId, protocoloId);
		}

		public int ExisteProtocolo(string numero, int excetoId = 0, int? protocoloTipo = null)
		{
			if (numero == null)
			{
				numero = string.Empty;
			}

			return _da.ExisteProtocolo(numero, excetoId, protocoloTipo);
		}

		public bool ExisteProtocolo(int id)
		{
			return _validar.ExisteProtocolo(id);
		}

		public ProtocoloNumero ObterProtocolo(string numero)
		{
			return _validar.ObterProtocolo(numero);
		}

		public int ProcessoApensado(int protocolo)
		{
			return _validar.ProtocoloAssociado(protocolo);
		}

		public bool ExisteProcessoAtividade(int processo)
		{
			return _validar.ExisteProtocolo(processo);
		}

		public bool ExisteRequerimento(int processo, bool somenteFilhos = false, bool exibirMsg = true)
		{
			if (!_validar.ExisteRequerimento(processo, somenteFilhos))
			{
				if (exibirMsg)
				{
					Validacao.Add(Mensagem.Processo.ProcessoSemRequerimentos);
				}

				return false;
			}

			return Validacao.EhValido;
		}

		public bool VerificarProtocoloAssociado(int protocolo)
		{
			return _validar.VerificarProtocoloAssociado(protocolo);
		}

		public string VerificarProtocoloAssociadoNumero(int protocolo)
		{
			return _validar.VerificarProtocoloAssociadoNumero(protocolo);
		}

		public bool EmPosse(int protocolo, int funcionario = 0)
		{
			return _validar.EmPosse(protocolo, funcionario);
		}

		public bool ExisteAtividade(int protocolo)
		{
			return _validar.ExisteAtividade(protocolo);
		}

		public bool ExisteProtocoloAtividade(int processo)
		{
			return _validar.ExisteProtocoloAtividade(processo);
		}

		#endregion
	}
}