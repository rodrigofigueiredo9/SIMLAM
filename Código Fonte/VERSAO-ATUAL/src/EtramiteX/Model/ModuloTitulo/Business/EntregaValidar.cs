using System;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloProcesso;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProcesso.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Business
{
	public class EntregaValidar
	{
		#region Propriedade

		EntregaDa _da = new EntregaDa();
		TituloBus _busTit = new TituloBus();
		ProcessoBus _busProc = new ProcessoBus();
		DocumentoBus _busDoc = new DocumentoBus();

		#endregion

		public bool Salvar(Entrega entrega)
		{
			ValidarProtocoloNumero(entrega.Protocolo.Numero);

			if (entrega.Titulos == null || entrega.Titulos.Count < 1)
			{
				Validacao.Add(Mensagem.Entrega.TituloObrigatorio);
			}

			if (entrega.DataEntrega.IsEmpty)
			{
				Validacao.Add(Mensagem.Entrega.DataEntregaObrigatria);
			}
			else
			{
				if(!entrega.DataEntrega.IsValido)
				{
					Validacao.Add(Mensagem.Entrega.DataEntregaInvalida);
				}

				if (entrega.DataEntrega.Data > DateTime.Today)
				{
					Validacao.Add(Mensagem.Entrega.DataMaiorDataAtual);
				}
			}

			ValidarCPF(entrega.CPF);

			if (string.IsNullOrWhiteSpace(entrega.Nome))
			{
				Validacao.Add(Mensagem.Entrega.NomeObrigatorio);
			}

			return Validacao.EhValido;
		}

		internal bool ValidarCPF(string cpf)
		{
			if (string.IsNullOrWhiteSpace(cpf))
			{
				Validacao.Add(Mensagem.Entrega.CPFObrigatorio);
			}
			else
			{
				if (!ValidacoesGenericasBus.Cpf(cpf))
				{
					Validacao.Add(Mensagem.Pessoa.CpfInvalido);
				}
			}

			return Validacao.EhValido;
		}

		public ProtocoloNumero ProcessoEntregaTitulo(string numero)
		{
			ProtocoloNumero protocolo = new ProtocoloNumero();

			ValidarProtocoloNumero(numero);

			if (!Validacao.EhValido)
			{
				return protocolo;
			}

			protocolo = _busProc.ObterProtocolo(numero);

			if (protocolo.Id == 0)
			{
				if (protocolo.IsProcesso)
				{
					Validacao.Add(Mensagem.Entrega.ProtocoloNaoExiste);
				}
				else
				{
					Validacao.Add(Mensagem.Entrega.ProtocoloNaoExiste);
				}
			}

			if (!Validacao.EhValido)
			{
				return protocolo;
			}

			int processoId = 0;

			if (protocolo.IsProcesso)
			{
				processoId = _busProc.ProcessoApensado(protocolo.Id);

				if (processoId != 0)
				{
					Processo processo = _busProc.ObterSimplificado(processoId);
					Validacao.Add(Mensagem.Entrega.ProcessoApenso(processo.Numero));
				}
			}
			else
			{
				ProtocoloNumero processoPai = _busDoc.ProtocoloAssociado(protocolo.Id);

				if (processoPai != null && processoPai.Id != 0)
				{
					Validacao.Add(Mensagem.Entrega.DocumentoJuntado(processoPai.NumeroTexto));
				}
			}

			return protocolo;
		}

		public void ValidarProtocoloNumero(string numero)
		{
			if (string.IsNullOrWhiteSpace(numero))
			{
				Validacao.Add(Mensagem.Entrega.ProcessoNumeroObrigatorio);
			}
			else
			{
				if (!ValidacoesGenericasBus.ValidarMaskNumeroBarraAno(numero))
				{
					Validacao.Add(Mensagem.Entrega.ProcessoNumeroInvalido);
				}
			}
		}

		public bool TituloEntrega(Titulo titulo)
		{
			//ver id da Situação "Prorrogado" e "Concluído"  Assiando na lov_titulo_situacao 
			if (titulo.Situacao.Id != 6 && titulo.Situacao.Id != 3 && titulo.Situacao.Id != 4)
			{
				return false;
			}

			if (_da.TituloEntregue(titulo.Id))
			{
				return false;
			}

			TituloModeloResposta resposta = titulo.Modelo.Resposta(eRegra.Prazo, eResposta.InicioPrazo);
			if (resposta != null && resposta.Valor.ToString() == "3")//início do prazo da data de entrega
			{
				if (titulo.Situacao.Id != 4)//"Assinado"
				{
					return false;
				}
			}
			else
			{
				if (titulo.Situacao.Id == 4)
				{
					return false;
				}
			}

			return true;
		}

		internal Resultados<Entrega> Filtrar(Resultados<Entrega> resultados)
		{
			if (resultados.Quantidade < 1)
			{
				Validacao.Add(Mensagem.Padrao.NaoEncontrouRegistros);
			}

			return resultados;
		}
	}
}