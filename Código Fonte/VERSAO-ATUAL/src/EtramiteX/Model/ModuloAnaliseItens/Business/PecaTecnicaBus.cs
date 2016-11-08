using System;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAnaliseItens;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAnaliseItens.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloAnaliseItens.Business
{
	public class PecaTecnicaBus
	{
		AnaliseItensDa _daAnalise = new AnaliseItensDa();
		PecaTecnicaDa _da = new PecaTecnicaDa();
		PecaTecnicaValidar _validar = new PecaTecnicaValidar();
		ProtocoloBus _busProtocolo = new ProtocoloBus();

		public PecaTecnica VerificarProtocolo(string numero)
		{
			PecaTecnica pecaTecnica = new PecaTecnica();

			ProtocoloNumero protocolo = _busProtocolo.ObterProtocolo(numero) ?? new ProtocoloNumero();

			pecaTecnica.Protocolo = new Protocolo(protocolo);

			try
			{
				if (_validar.VerificarProtocolo(protocolo))
				{
					pecaTecnica.Protocolo.Id = protocolo.Id;
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return pecaTecnica;
		}

		public int Salvar(PecaTecnica pecaTecnica)
		{
			try
			{
				if (_validar.Salvar(pecaTecnica))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						int id = _da.Salvar(pecaTecnica, bancoDeDados);

						bancoDeDados.Commit();

						Validacao.Add(Mensagem.AnaliseItem.PecaTecnicaSalvaComSucesso);

						return id;
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return 0;
		}

		public PecaTecnica Obter(int id, string tid = null, BancoDeDados banco = null)
		{
			PecaTecnica pecaTecnica = null;

			try
			{
				pecaTecnica = _da.Obter(id, banco, tid, false);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return pecaTecnica;
		}

		public PecaTecnica ObterSimplificado(int id, string tid = null, BancoDeDados banco = null)
		{
			PecaTecnica pecaTecnica = null;

			try
			{
				pecaTecnica = _da.Obter(id, banco, tid, true);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return pecaTecnica;
		}

		public int ExistePecaTecnica(int atividade, int protocoloId)
		{
			try
			{
				return _da.ExistePecaTecnica(atividade, protocoloId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return 0;
		}
	}
}