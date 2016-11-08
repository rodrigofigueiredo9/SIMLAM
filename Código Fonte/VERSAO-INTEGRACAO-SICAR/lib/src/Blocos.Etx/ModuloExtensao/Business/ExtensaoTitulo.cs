using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;

namespace Tecnomapas.Blocos.Etx.ModuloExtensao.Business
{
	public static class ExtensaoTitulo
	{
		public static bool Regra(this TituloModelo modelo, eRegra regra)
		{
			if (modelo == null || modelo.Regras == null || !modelo.Regras.Exists(x => x.TipoEnum == regra))
			{
				return false;
			}

			return modelo.Regras.Single(x => x.TipoEnum == regra).Valor;
		}

		public static TituloModeloResposta Resposta(this TituloModelo modelo, eRegra regra, eResposta resposta)
		{
			if (modelo.Regras == null || !modelo.Regras.Exists(x => x.TipoEnum == regra))
			{
				return null;
			}

			return modelo.Regras.Single(x => x.TipoEnum == regra).Respostas.SingleOrDefault(y => y.TipoEnum == resposta);
		}

		public static List<TituloModeloResposta> Respostas(this TituloModelo modelo, eRegra regra, eResposta resposta)
		{
			if (modelo.Regras == null || !modelo.Regras.Exists(x => x.TipoEnum == regra))
			{
				return null;
			}

			return modelo.Regras.Single(x => x.TipoEnum == regra).Respostas.Where(x => x.TipoEnum == resposta).ToList();
		}

		public static Especificidade ToEspecificidade(this Titulo titulo)
		{
			if (titulo.Modelo == null || titulo.Modelo.Id == 0)
			{
				return null;
			}

			if (titulo.Especificidade == null)
				throw new Exception("Objeto da especificidade é nulo");

			Especificidade especificidade = titulo.Especificidade;

			especificidade.RequerimentoId = titulo.RequerimetoId.GetValueOrDefault();
			especificidade.Titulo.Id = titulo.Id;
			especificidade.Titulo.SetorId = titulo.Setor.Id;
			especificidade.Titulo.SituacaoId = (titulo.Situacao != null) ? titulo.Situacao.Id : 0;
			especificidade.Titulo.ExisteAnexos = (titulo.Anexos != null && titulo.Anexos.Count > 0);
			especificidade.Titulo.EmpreendimentoId = titulo.EmpreendimentoId;
			especificidade.Titulo.Modelo = titulo.Modelo.Id.ToString();
			especificidade.Titulo.Protocolo.Id = titulo.Protocolo.Id ?? 0;
			especificidade.Titulo.Protocolo.IsProcesso = titulo.Protocolo.IsProcesso;
			especificidade.Titulo.Protocolo.Numero = titulo.Protocolo.Numero;
			especificidade.Titulo.RepresentanteId = titulo.Representante.Id;

			if (!string.IsNullOrEmpty(titulo.LocalEmissao.Texto))
			{
				especificidade.Titulo.LocalEmissao = titulo.LocalEmissao.Texto;
			}

			if (!string.IsNullOrEmpty(titulo.Modelo.Sigla))
			{
				especificidade.Titulo.ModeloSigla = titulo.Modelo.Sigla;
			}

			especificidade.Titulo.RequerimentoAtividades = titulo.RequerimentoAtividades;
			if (especificidade.ProtocoloReq.RequerimentoId == 0)
			{
				especificidade.ProtocoloReq.RequerimentoId = titulo.RequerimentoAtividades ?? 0;
			}

			foreach (var item in titulo.Atividades)
			{
				item.Protocolo = item.Protocolo ?? new Protocolo();
                especificidade.ProtocoloReq.Id = item.Protocolo.Id.GetValueOrDefault();
				especificidade.ProtocoloReq.IsProcesso = item.Protocolo.IsProcesso;
				especificidade.ProtocoloReq.Numero = item.Protocolo.Numero;

				ProcessoAtividadeEsp atividade = new ProcessoAtividadeEsp();
				atividade.Id = item.Id;
				especificidade.Atividades.Add(atividade);
			}

			foreach (var item in titulo.Associados)
			{
				TituloAssociadoEsp tituloAssociado = new TituloAssociadoEsp();

				tituloAssociado.Id = item.Id;
				tituloAssociado.IdRelacionamento = item.IdRelacionamento;
				tituloAssociado.TituloNumero = item.Numero.Texto;
				tituloAssociado.ModeloSigla = item.Modelo.Sigla;

				especificidade.TitulosAssociado.Add(tituloAssociado);
			}

			return especificidade;
		}

		public static Protocolo ToProtocolo(this ProtocoloEsp ProtocoloEsp)
		{
			Protocolo protocolo = new Protocolo();
			protocolo.Id = ProtocoloEsp.Id;
			protocolo.IsProcesso = ProtocoloEsp.IsProcesso;
			protocolo.Requerimento.Id = ProtocoloEsp.RequerimentoId;
			return protocolo;
		}
	}
}