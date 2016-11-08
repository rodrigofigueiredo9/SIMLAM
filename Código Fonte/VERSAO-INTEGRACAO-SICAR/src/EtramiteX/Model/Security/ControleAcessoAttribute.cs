using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.Model.Security
{
	[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
	public class ControleAcessoAttribute : ActionFilterAttribute
	{
		public int Acao { get; set; }
		public int Artefato { get; set; }

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			Int32 caracterizacaoTipo = 0;
			Int32 artefatoId = 0;

			if (filterContext.ActionParameters.ContainsKey("id"))
			{
				Int32.TryParse((filterContext.ActionParameters["id"]).ToString(), out artefatoId);
			}

			if (artefatoId == 0)
			{
				Validacao.Add(Mensagem.Sistema.ControleAcessoParametrosInsuficientes);
			}

			if (Artefato == (int)eHistoricoArtefatoCaracterizacao.projetogeografico)
			{
				if (filterContext.ActionParameters.ContainsKey("tipo"))
				{
					Int32.TryParse((filterContext.ActionParameters["tipo"]).ToString(), out caracterizacaoTipo);
				}

				if (caracterizacaoTipo == 0)
				{
					Validacao.Add(Mensagem.Sistema.ControleAcessoParametrosInsuficientes);
				}
			}

			if (Validacao.Erros.Exists(x => x.Tipo == eTipoMensagem.ControleAcesso)) 
			{
				return;
			}

			Controle controle = new Controle();
			controle.Acao = Acao;
			controle.ArtefatoTipo = Artefato;
			controle.ArtefatoId = artefatoId;
			controle.Caracterizacao = caracterizacaoTipo;
			controle.Ip = HttpContext.Current.Request.UserHostAddress;
			controle.Executor = Executor.Current;

			ControleAcessoBus controleBus = new ControleAcessoBus();
			controleBus.Gerar(controle);
		}

		public override void OnResultExecuting(ResultExecutingContext filterContext)
		{
			if (Validacao.Erros.Exists(x => x.Tipo == eTipoMensagem.ControleAcesso)) 
			{
				filterContext.Cancel = true;
			}
		}
	}
}
