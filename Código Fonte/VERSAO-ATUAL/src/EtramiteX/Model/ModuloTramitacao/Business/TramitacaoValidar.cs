using System;
using System.Collections.Generic;
using System.Web;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFuncionario.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTramitacao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloTramitacao.Business
{
	public class TramitacaoValidar
	{
		#region Propriedades

		TramitacaoDa _da = new TramitacaoDa();
		TramitacaoMsg Msg = new TramitacaoMsg();
		ListaBus _busLista = new ListaBus();
		FuncionarioBus _busFunc = new FuncionarioBus(new FuncionarioValidar());
		ProtocoloBus _busProtocolo = new ProtocoloBus();
		public EtramiteIdentity User
		{
			get
			{
				try
				{
					return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity;
				}
				catch (Exception exc)
				{
					Validacao.AddErro(exc);
					return null;
				}
			}
		}

		#endregion
		
		public bool Cancelar(Tramitacao tramitacao, bool permissaoTramitar)
		{
			if (!tramitacao.Protocolo.Id.HasValue)
			{
				Validacao.Add(Mensagem.Tramitacao.ProtocoloJaTramitado(tramitacao.Protocolo.IsProcesso ? "processo" : "documento", tramitacao.Protocolo.Numero, "cancelado"));
				return Validacao.EhValido;
			}

			if (tramitacao.Executor.Id != tramitacao.Remetente.Id && !_da.Registrador(User.FuncionarioId, tramitacao.RemetenteSetor.Id))
			{
				Validacao.Add(Msg.CancelarTramitacaoRegistro);
				return Validacao.EhValido;
			}

			RegraSetor(tramitacao.RemetenteSetor.Id, permissaoTramitar);

			return Validacao.EhValido;
		}

		public bool RegraSetor(int setorId, bool permissaoTramitar)
		{
			if (_da.ObterTipoSetor(setorId) == (int)eTramitacaoTipo.Registro)
			{
				if (!_da.Registrador(User.FuncionarioId, setorId))
				{
					Validacao.Add(Msg.TramitacaoRegistro);
				}
			}
			else
			{
				if (!permissaoTramitar)
				{
					Validacao.Add(Msg.TramitacaoSemPermissao);
				}
			}

			return Validacao.EhValido;
		}

		public bool EnviarPreValidar(Enviar enviarCampos)
		{
			if (enviarCampos.RemetenteSetor.Id <= 0)
			{
				Validacao.Add(Msg.SetorRemetenteObrigatorio);
				return Validacao.EhValido;
			}
			else
			{
				SetorSelecionadoMesmoTipoTramitacao(enviarCampos.TramitacaoTipo, enviarCampos.RemetenteSetor.Id);
			}

			if (enviarCampos.Remetente == null || enviarCampos.Remetente.Id <= 0)
			{
				Validacao.Add(Msg.RemetenteObrigatorio);
			}

			if (!Validacao.EhValido)
			{
				return Validacao.EhValido;
			}

			if (enviarCampos.ObjetivoId <= 0)
			{
				Validacao.Add(Msg.ObjetivoObrigratorio);
			}

			if (enviarCampos.ObjetivoId == 19)//Juntada Processo SEP
			{
				if (string.IsNullOrWhiteSpace(enviarCampos.NumeroAutuacao))
					Validacao.Add(Msg.NumeroAutuacaoObrigatorio);
			}

			if (enviarCampos.DestinatarioSetor.Id <= 0)
			{
				Validacao.Add(Msg.SetorDestinatarioObrigratorio);
			}

			if (enviarCampos.Remetente.Id == enviarCampos.Destinatario.Id && enviarCampos.RemetenteSetor.Id == enviarCampos.DestinatarioSetor.Id)
			{
				Validacao.Add(Msg.RemetenteDestinatarioIguais);
			}

			if(enviarCampos.DestinatarioSetor.Id == 258)//Outros
			{
				if(string.IsNullOrWhiteSpace(enviarCampos.DestinoExterno))
					Validacao.Add(Msg.DestinoExternoObrigatorio);

				if((enviarCampos.FormaEnvio ?? 0) == 0)
					Validacao.Add(Msg.FormaEnvioObrigatorio);

				if((enviarCampos.FormaEnvio == 1 || enviarCampos.FormaEnvio == 2) && string.IsNullOrWhiteSpace(enviarCampos.CodigoRastreio))
					Validacao.Add(Msg.CodigoRastreioObrigatorio);
			}

			return Validacao.EhValido;
		}

		public bool EnviarExternoPreValidar(Enviar enviarCampos)
		{
			if (enviarCampos.RemetenteSetor.Id <= 0)
			{
				Validacao.Add(Msg.SetorRemetenteObrigatorio);
				return Validacao.EhValido;
			}

			if (enviarCampos.Remetente == null || enviarCampos.Remetente.Id <= 0)
			{
				Validacao.Add(Msg.RemetenteObrigatorio);
			}

			if (enviarCampos.RemetenteSetor.Id <= 0)
			{
				Validacao.Add(Msg.SetorRemetenteObrigatorio);
				return Validacao.EhValido;
			}

			if (enviarCampos.ObjetivoId <= 0)
			{
				Validacao.Add(Msg.ObjetivoObrigratorio);
			}

			if (enviarCampos.OrgaoExterno.Id <= 0)
			{
				Validacao.Add(Msg.OrgaoExternoObrigratorio);
			}

			return Validacao.EhValido;
		}

		public bool Enviar(List<Tramitacao> tramitacoes)
		{
			if (tramitacoes == null || tramitacoes.Count <= 0)
			{
				Validacao.Add(Msg.ProtocoloObrigatorio);
			}

			if (!Validacao.EhValido)
			{
				return Validacao.EhValido;
			}

			foreach (Tramitacao item in tramitacoes)
			{
				if (item.Protocolo?.Tipo?.Texto == "Documento Avulso")
				{
					if (string.IsNullOrWhiteSpace(item.Despacho))
						Validacao.Add(Msg.DespachoObrigatorio);
				}
				RegraSetor(item.RemetenteSetor.Id, true);
				SetorOrigem(item);
				if (item.Protocolo.Id > 0)
				{
					if (!_da.NotificacaoIsValida(item.Protocolo.Id.Value))
						Validacao.Add(Msg.NaoExisteNotificacao);
				}
			}

			return Validacao.EhValido;
		}

		public bool EnviarExterno(List<Tramitacao> tramitacoes)
		{
			if (tramitacoes == null || tramitacoes.Count <= 0)
			{
				Validacao.Add(Msg.ProtocoloObrigatorio);
			}

			foreach (Tramitacao item in tramitacoes)
			{
				SetorOrigem(item);
				if (item.Protocolo.Id > 0)
				{
					if (!_da.NotificacaoIsValida(item.Protocolo.Id.Value))
						Validacao.Add(Msg.NaoExisteNotificacao);
				}
			}

			return Validacao.EhValido;
		}

		internal bool Receber(List<Tramitacao> tramitacoes)
		{
			if (tramitacoes == null || tramitacoes.Count <= 0)
			{
				Validacao.Add(Mensagem.Tramitacao.ReceberPeloMenosUmProcessoOuDocumentoDeveEstarSelecionado);
				return Validacao.EhValido;
			}

			foreach (Tramitacao tramitacao in tramitacoes)
			{
				bool existe = _da.ExisteTramitacao(tramitacao.Protocolo.Id.Value) > 0;

				if (!existe)
				{
					Validacao.Add(Mensagem.Tramitacao.ProtocoloJaTramitado(tramitacao.Protocolo.IsProcesso ? "processo" : "documento", tramitacao.Protocolo.Numero, "recebido"));
				}

				if (tramitacao.Protocolo.Id > 0)
				{
					if (!_da.NotificacaoIsValida(tramitacao.Protocolo.Id.Value))
						Validacao.Add(Msg.NaoExisteNotificacao);
				}
			}

			return Validacao.EhValido;
		}

		public bool ReceberRegistro(List<Tramitacao> enviadosParaFuncionario, List<Tramitacao> enviadosParaSetor, int FuncionarioDestinatarioId)
		{
			if (enviadosParaSetor.Count > 0 && FuncionarioDestinatarioId == 0)
			{
				Validacao.Add(Mensagem.Tramitacao.ReceberDestinatarioObrigratorio);
			}

			if ((enviadosParaFuncionario == null && enviadosParaSetor == null) || ((enviadosParaFuncionario.Count + enviadosParaSetor.Count) == 0))
			{
				Validacao.Add(Mensagem.Tramitacao.ReceberPeloMenosUmProcessoOuDocumentoDeveEstarSelecionado);
			}

			if (!Validacao.EhValido) return Validacao.EhValido;

			foreach (Tramitacao tramitacao in enviadosParaFuncionario)
			{
				bool existe = _da.ExisteTramitacao(tramitacao.Protocolo.Id.Value) > 0;

				if (!existe)
				{
					Validacao.Add(Mensagem.Tramitacao.ProtocoloJaTramitado(tramitacao.Protocolo.IsProcesso ? "processo" : "documento", tramitacao.Protocolo.Numero, "recebido"));
				}

				if (tramitacao.Protocolo.Id > 0)
				{
					if (!_da.NotificacaoIsValida(tramitacao.Protocolo.Id.Value))
						Validacao.Add(Msg.NaoExisteNotificacao);
				}
			}

			foreach (Tramitacao tramitacao in enviadosParaSetor)
			{
				bool existe = _da.ExisteTramitacao(tramitacao.Protocolo.Id.Value) > 0;

				if (!existe)
				{
					Validacao.Add(Mensagem.Tramitacao.ProtocoloJaTramitado(tramitacao.Protocolo.IsProcesso ? "processo" : "documento", tramitacao.Protocolo.Numero, "recebido"));
				}

				if (tramitacao.Protocolo.Id > 0)
				{
					if (!_da.NotificacaoIsValida(tramitacao.Protocolo.Id.Value))
						Validacao.Add(Msg.NaoExisteNotificacao);
				}
			}

			return Validacao.EhValido;
		}

		internal bool ReceberExterno(List<Tramitacao> tramitacoes, int destinatarioSetorId, int orgaoExternoId)
		{
			if (destinatarioSetorId <= 0)
			{
				Validacao.Add(Mensagem.Tramitacao.SetorDestinoObrigratorio);
				return Validacao.EhValido;
			}

			if (orgaoExternoId <= 0)
			{
				Validacao.Add(Mensagem.Tramitacao.OrgaoExternoObrigratorio);
			}

			if (tramitacoes == null || tramitacoes.Count <= 0)
			{
				Validacao.Add(Mensagem.Tramitacao.ReceberPeloMenosUmProcessoOuDocumentoDeveEstarSelecionado);
			}

			if (!Validacao.EhValido) return Validacao.EhValido;

			foreach (Tramitacao tramitacao in tramitacoes)
			{
				bool existe = _da.ExisteTramitacao(tramitacao.Protocolo.Id.Value) > 0;

				if (!existe)
				{
					Validacao.Add(Mensagem.Tramitacao.ProtocoloJaRetiradoExterno(tramitacao.Protocolo.IsProcesso ? "processo" : "documento", tramitacao.Protocolo.Numero));
				}

				if (tramitacao.Protocolo.Id > 0)
				{
					if (!_da.NotificacaoIsValida(tramitacao.Protocolo.Id.Value))
						Validacao.Add(Msg.NaoExisteNotificacao);
				}
			}

			return Validacao.EhValido;
		}

		public bool SetorSelecionadoMesmoTipoTramitacao(int tramitacaoTipo, int setorId)
		{
			int setorTipo = _da.ObterTipoSetor(setorId);

			if (tramitacaoTipo != setorTipo && setorTipo == (int)eTramitacaoTipo.Registro)
			{
				Validacao.Add(Msg.SetorRemetenteTipoRegistro);
			}

			if (tramitacaoTipo != setorTipo && setorTipo == (int)eTramitacaoTipo.Normal)
			{
				Validacao.Add(Msg.SetorRemetenteTipoNormal);
			}

			return Validacao.EhValido;
		}

		public void SetorOrigem(Tramitacao tramitacao)
		{
			if (_da.ObterSetorProtocolo(tramitacao.Protocolo.Id.Value) != tramitacao.RemetenteSetor.Id)
			{
				Validacao.Add(Mensagem.Tramitacao.SetorInvalido(tramitacao.Protocolo.Numero));
			}

			if (!_busProtocolo.EmPosse(tramitacao.Protocolo.Id.Value, tramitacao.Remetente.Id))
			{
				Validacao.Add(Mensagem.Tramitacao.FuncionarioNaoPossuiPosseProtocolo(tramitacao.Protocolo.Numero));
			}
		}

		public bool MudarTipoTramitacaoSetor(List<Setor> setores)
		{
			foreach (Setor setor in setores)
			{
				if (setor.TramitacaoTipoId == (int)eTramitacaoTipo.Registro && setor.Funcionarios.Count <= 0)
				{
					Validacao.Add(Msg.FuncionarioObrigatorio);
				}
			}

			FuncionarioContidoSetor(setores);

			return Validacao.EhValido;
		}

		public Protocolo AdicionarProtocolo(int? funcId, int funcSetorId, string numeroProcDoc)
		{
			Protocolo protocolo = new Protocolo();

			NumeroProtocoloFormato(numeroProcDoc);

			if (funcId <= 0)
			{
				Validacao.Add(Msg.RemetenteObrigatorio);
			}

			if (funcSetorId <= 0)
			{
				Validacao.Add(Msg.SetorRemetenteObrigatorio);
			}

			if (!Validacao.EhValido)
			{
				return protocolo;
			}

			protocolo = ProtocoloEmPosse(numeroProcDoc, funcId.Value);

			if (!Validacao.EhValido)
			{
				protocolo.Id = 0;
				return protocolo;
			}

			if (protocolo.Id > 0)
			{
				ProtocoloMesmoSetorSelecionado(protocolo, funcSetorId);
			}
			return protocolo;
		}

		public Protocolo ProtocoloEmPosse(string numeroProcDoc, int funcionario = 0)
		{
			Protocolo protocolo = _da.ExisteProtocolo(numeroProcDoc);
			if (!protocolo.Id.HasValue || protocolo.Id <= 0)
			{
				Validacao.Add(Msg.ProtocoloInexistente);
			}
			else
			{
				if (!_busProtocolo.EmPosse(protocolo.Id.Value, funcionario))
				{
					Validacao.Add(Msg.FuncionarioNaoPossuiPosseDoProtocolo);
				}
			}
			return protocolo;
		}

		public bool ProtocoloMesmoSetorSelecionado(Protocolo protocolo, int funcSetorId)
		{
			if (funcSetorId <= 0)
			{
				Validacao.Add(Msg.SetorRemetenteObrigatorio);
			}
			else
			{
				if (!_da.ProtocoloMesmoSetorSelecionado(protocolo, funcSetorId))
				{
					Validacao.Add(Msg.ProtocoloDeOutroSetor);
				}
			}
			return Validacao.EhValido;
		}

		public bool NumeroProtocoloFormato(string numeroProcDoc)
		{
			if (String.IsNullOrEmpty(numeroProcDoc))
			{
				Validacao.Add(Msg.NumeroProtocoloObrigatorio);
			}
			else
			{
				if (!ValidacoesGenericasBus.ValidarMaskNumeroBarraAno(numeroProcDoc))
				{
					Validacao.Add(Msg.NumeroProtocoloInvalido);
				}
			}
			return Validacao.EhValido;
		}

		public bool FuncionarioContidoSetor(List<Setor> setores)
		{
			foreach (Setor setor in setores)
			{
				foreach (var funcionario in setor.Funcionarios)
				{
					FuncionarioContidoSetor(funcionario.Id, setor.Id, funcionario.Texto, setor.Sigla);
				}
			}

			return Validacao.EhValido;
		}

		public bool FuncionarioContidoSetor(int funcionario, int setor, string funcionarioNome, string setorSigla)
		{
			try
			{
				if (!_busFunc.VerificarFuncionarioContidoSetor(funcionario, setor))
				{
					if (!string.IsNullOrWhiteSpace(funcionarioNome) && !string.IsNullOrWhiteSpace(setorSigla))
					{
						Validacao.Add(Msg.FuncionarioNaoAssociadoSetor(funcionarioNome, setorSigla));
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		internal bool BuscarTramitacoesEmPosse(int? setorId, Resultados<Tramitacao> resultados = null)
		{
			if (setorId.GetValueOrDefault() <= 0)
			{
				Validacao.Add(Msg.SetorRemetenteObrigatorio);
				return Validacao.EhValido;
			}

			if (resultados != null && resultados.Itens.Count <= 0)
			{
				Validacao.Add(Msg.FuncionarioNaoPossuiPosseNenhuma);
			}
			return Validacao.EhValido;
		}

		public bool RegistradorSetor(int funcionarioLogadoId, int setorId, int funcionarioId)
		{
			if ((!_da.Registrador(funcionarioLogadoId, setorId)) && funcionarioId != funcionarioLogadoId)
			{
				Validacao.Add(Msg.UsuarioNaoRegistrador);
			}

			return Validacao.EhValido;
		}

		public bool SetorPorRegistrado(int setorId)
		{
			try
			{
				if (_da.SetorPorRegistrado(setorId))
				{
					Validacao.Add(Mensagem.Tramitacao.SetorDestinoPorRegistro);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return !Validacao.EhValido;
		}

		internal bool SalvarMotivo(Motivo motivo)
		{
			if (string.IsNullOrWhiteSpace(motivo.Nome))
			{
				Validacao.Add(Mensagem.Tramitacao.MotivoObrigatorio);
			}
			else
			{
				if (motivo.Nome.Length > 100)
				{
					Validacao.Add(Mensagem.Tramitacao.MotivoTamanhoMaximo);
				}
			}

			if (_da.ExisteMotivo(motivo))
			{
				Validacao.Add(Mensagem.Tramitacao.NomeJaAdicionado);
			}

			return Validacao.EhValido;
		}
	}
}