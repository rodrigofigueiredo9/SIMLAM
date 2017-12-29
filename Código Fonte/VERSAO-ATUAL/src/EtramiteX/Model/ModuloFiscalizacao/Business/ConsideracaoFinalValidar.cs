using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business
{
	public class ConsideracaoFinalValidar
	{
		public EtramiteIdentity User
		{
			get
			{
				if (HttpContext.Current.User == null)
				{
					return null;
				}
				return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity;
			}
		}

		public bool Salvar(ConsideracaoFinal consideracao)
		{
            if (string.IsNullOrWhiteSpace(consideracao.Descrever))
            {
                Validacao.Add(Mensagem.ConsideracaoFinalMsg.DescreverObrigatorio);
            }

			if (!consideracao.HaReparacao.HasValue)
			{
				Validacao.Add(Mensagem.ConsideracaoFinalMsg.OpinarObrigatorio);
			}
			else
			{
				if (consideracao.HaReparacao.Value == 1)
				{
					if (string.IsNullOrWhiteSpace(consideracao.Reparacao))
					{
						Validacao.Add(Mensagem.ConsideracaoFinalMsg.OpinarReparacaoOpinarObrigatorio);	
					}

					if (!consideracao.HaTermoCompromisso.HasValue)
					{
						Validacao.Add(Mensagem.ConsideracaoFinalMsg.FirmarCompromissoObrigatorio);	
					}
					else
					{
						if (!consideracao.HaTermoCompromisso.Value && string.IsNullOrWhiteSpace(consideracao.TermoCompromissoJustificar))
						{
							Validacao.Add(Mensagem.ConsideracaoFinalMsg.JustificarFirmarCompromissoObrigatorio);	
						}
					}
				}
				else if (consideracao.HaReparacao.Value != -1 && string.IsNullOrWhiteSpace(consideracao.Reparacao))
				{
					Validacao.Add(Mensagem.ConsideracaoFinalMsg.OpinarReparacaoJustificarObrigatorio);
				}
			}

			#region Testemunhas

			foreach (var item in consideracao.Testemunhas)
			{
				if (item.TestemunhaIDAF.HasValue)
				{
					if (item.TestemunhaIDAF.Value)						
					{
						if (item.TestemunhaId.GetValueOrDefault() == 0)
						{
							AddFormarMsg(Mensagem.ConsideracaoFinalMsg.TestemunhaObrigatorio, item.Colocacao.ToString());
						}
					}
					else
					{
						if (string.IsNullOrWhiteSpace(item.TestemunhaNome))
						{
							AddFormarMsg(Mensagem.ConsideracaoFinalMsg.TestemunhaNomeObrigatorio, item.Colocacao.ToString());
						}
					}
				}
			}

			#endregion					

			#region Assinantes

			if (consideracao.Assinantes == null || consideracao.Assinantes.Count == 0)
			{
				Validacao.Add(Mensagem.ConsideracaoFinalMsg.AssinanteObrigatorio);
			}
			else
			{
				if (!consideracao.Assinantes.Exists(x => x.FuncionarioId == User.FuncionarioId))
				{
					Validacao.Add(Mensagem.ConsideracaoFinalMsg.AssinanteFuncionarioLogado);
				}

				if (consideracao.Assinantes.GroupBy(x => new { FuncionarioId = x.FuncionarioId, FuncionarioCargoId = x.FuncionarioCargoId }).Where(x => x.Count() > 1).Count() > 0)
				{
					Validacao.Add(Mensagem.ConsideracaoFinalMsg.AssinanteFuncionarioUnico);
				}
			}

			#endregion

			return Validacao.EhValido;
		}
		
		internal void AddFormarMsg(Mensagem msg, string colocacao)
		{
			int index = Validacao.Erros.FindIndex(x => !string.IsNullOrEmpty(x.Campo) && x.Campo.Contains(msg.Campo.Replace("{0}", "")));

			if (index > -1)
			{
				Validacao.Erros[index].Campo += "," + msg.Campo.Replace("{0}", colocacao);
			}
			else
			{
				Validacao.Add(new Mensagem { Tipo = msg.Tipo, Texto = msg.Texto, Campo = msg.Campo.Replace("{0}", colocacao) });
			}
		}
	}
}
