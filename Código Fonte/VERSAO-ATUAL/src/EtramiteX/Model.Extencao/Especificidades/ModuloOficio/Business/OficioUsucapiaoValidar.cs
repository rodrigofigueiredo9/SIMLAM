using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloOficio;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloOficio.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloOficio.Business
{
	public class OficioUsucapiaoValidar : EspecificidadeValidarBase, IEspecificiadeValidar
	{
		OficioUsucapiaoDa _da = new OficioUsucapiaoDa();
		GerenciadorConfiguracao<ConfiguracaoProcesso> _atividadeConfig = new GerenciadorConfiguracao<ConfiguracaoProcesso>(new ConfiguracaoProcesso());

		public bool Salvar(IEspecificidade especificidade)
		{
			OficioUsucapiao esp = especificidade as OficioUsucapiao;

			if (!RequerimentoAtividade(esp, false, false))
			{
				return false;
			}

			if (!String.IsNullOrWhiteSpace(esp.Dimensao))
			{
				Decimal aux = 0;
				if (Decimal.TryParse(esp.Dimensao, out aux))
				{
					if (aux <= 0)
					{
						Validacao.Add(Mensagem.OficioUsucapiao.DimensaoMaiorZero);
					}
				}
				else
				{
					Validacao.Add(Mensagem.OficioUsucapiao.DimensaoInvalida);
				}
			}
			else
			{
				Validacao.Add(Mensagem.OficioUsucapiao.DimensaoObrigatoria);
			}

			if (String.IsNullOrWhiteSpace(esp.Destinatario))
			{
				Validacao.Add(Mensagem.OficioUsucapiao.DestinatarioPGEObrigatorio);
			}

			if (String.IsNullOrWhiteSpace(esp.Descricao))
			{
				Validacao.Add(Mensagem.OficioUsucapiao.DescricaoOficioObrigatorio);
			}

			if (!esp.EmpreendimentoTipo.HasValue)
			{
				Validacao.Add(Mensagem.OficioUsucapiao.EmpreedimentoTipoObrigatorio);
			}


			#region Atividade

			foreach (var atividade in esp.Atividades)
			{
				if (atividade.Id != 0)
				{
					if (atividade.Id != ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.UsucapiaoRural) && atividade.Id != ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.UsucapiaoUrbano))
					{
						List<ProcessoAtividadeItem> atividades = _atividadeConfig.Obter<List<ProcessoAtividadeItem>>(ConfiguracaoProcesso.KeyAtividadesProcesso);
						Validacao.Add(Mensagem.OficioUsucapiao.AtividadeInvalida(atividades.SingleOrDefault(x => x.Id == atividade.Id).Texto));
					}
				}
			}

			#endregion

			return Validacao.EhValido;
		}

		public bool Emitir(IEspecificidade especificidade)
		{
			if (ExisteProcDocFilhoQueFoiDesassociado(especificidade.Titulo.Id))
			{
				Validacao.Add(Mensagem.Especificidade.ProtocoloReqFoiDesassociado);
			}

			return Salvar(especificidade);
		}
	}
}