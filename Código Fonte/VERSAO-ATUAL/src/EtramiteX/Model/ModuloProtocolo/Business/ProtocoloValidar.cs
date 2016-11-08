using System;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloChecagemRoteiro.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloPessoa.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Business
{
	public class ProtocoloValidar
	{
		#region Propriedades

		ProtocoloDa _da;
		PessoaBus _busPessoa;		
		ChecagemRoteiroBus _busCheckList;

		#endregion

		public ProtocoloValidar()
		{
			_busCheckList = new ChecagemRoteiroBus();
			_busPessoa = new PessoaBus();
			_da = new ProtocoloDa();
		}

		#region Verificar / Validar

		public bool PosseProtocoloFuncionario(int protocoloId, int usuarioId)
		{
			return _da.EmPosse(protocoloId, usuarioId);
		}

		public bool ExisteRequerimento(int protocolo, bool somenteFilhos = false)
		{
			try
			{
				return _da.ExisteRequerimento(protocolo, somenteFilhos);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return false;
		}

		public bool ExisteProtocoloAtividade(int protocolo)
		{
			try
			{
				return _da.ExisteAtividade(protocolo);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return false;

		}

		public bool Associado(int protocolo)
		{
			ProtocoloNumero retorno = _da.VerificarProtocoloAssociado(protocolo);
			return (retorno != null);
		}

		public bool ValidarAssociarResponsavelTecnico(int id)
		{
			return _busPessoa.ValidarAssociarResponsavelTecnico(id);
		}

		public bool ValidarCheckList(int checkListId, int processoId)
		{
			return _busCheckList.ValidarAssociarCheckList(checkListId, processoId, true);
		}

		public int ExisteProtocolo(string numero, int excetoId = 0)
		{
			if (numero == null)
			{
				numero = string.Empty;
			}

			return _da.ExisteProtocolo(numero, excetoId);
		}

		public bool ExisteProtocolo(int id)
		{
			return _da.ExisteProtocolo(id);
		}

		public ProtocoloNumero ObterProtocolo(string numero)
		{
			return _da.ObterProtocolo(numero);
		}

		public int ProtocoloAssociado(int protocolo)
		{
			ProtocoloNumero retorno = _da.VerificarProtocoloAssociado(protocolo);
			if (retorno != null)
			{
				return retorno.Id;
			}
			return 0;
		}

		public bool VerificarProtocoloAssociado(int protocolo)
		{
			ProtocoloNumero retorno = _da.VerificarProtocoloAssociado(protocolo);
			return (retorno != null);
		}

		public string VerificarProtocoloAssociadoNumero(int protocolo)
		{
			ProtocoloNumero retorno = _da.VerificarProtocoloAssociado(protocolo);
			if (retorno != null)
			{
				return retorno.NumeroTexto;
			}
			return string.Empty;
		}

		public bool EmPosse(int protocolo, int funcionario = 0)
		{
			if (funcionario > 0)
			{
				return _da.EmPosse(protocolo, funcionario);
			}

			return _da.EmPosse(protocolo);
		}

		public bool ExisteAtividade(int protocolo)
		{
			return _da.ExisteAtividade(protocolo);
		}

		internal string ObterNumeroProcessoPai(int? id)
		{
			return _da.ObterNumeroProcessoPai(id);
		}

		#endregion
	}
}