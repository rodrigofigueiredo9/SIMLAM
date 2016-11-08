using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloSilviculturaPPFF;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels
{
	public class SilviculturaPPFFVM
	{
		public bool IsVisualizar { get; set; }
		public bool IsEditar { get; set; }
        
		private SilviculturaPPFF _caracterizacao = new SilviculturaPPFF();
		public SilviculturaPPFF Caracterizacao
		{
			get { return _caracterizacao; }
			set { _caracterizacao = value; }
		}

		private List<SelectListItem> _atividade = new List<SelectListItem>();
		public List<SelectListItem> Atividade
		{
			get { return _atividade; }
			set { _atividade = value; }
		}

		private List<SelectListItem> _municipios = new List<SelectListItem>();
		public List<SelectListItem> Municipios
		{
			get { return _municipios; }
			set { _municipios = value; }
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
                    @MunicipioObrigatorio = Mensagem.SilviculturaPPFF.MunicipioObrigatorio,
                    @MunicipioDuplicado = Mensagem.SilviculturaPPFF.MunicipioDuplicado
				});
			}
		}

		public SilviculturaPPFFVM(SilviculturaPPFF caracterizacao, List<ProcessoAtividadeItem> atividades, List<Municipio> municipios, bool isVisualizar = false, bool isEditar = false)
		{
			IsVisualizar = isVisualizar;
			Caracterizacao = caracterizacao;
			IsEditar = isEditar;
			Municipios = ViewModelHelper.CriarSelectList(municipios, true, true);

			int silviculturaProgramaProdutorFlorestalFomento = ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.SilviculturaProgramaProdutorFlorestalFomento);

			Atividade = ViewModelHelper.CriarSelectList(atividades.Where(x => x.Id == silviculturaProgramaProdutorFlorestalFomento).ToList(), true, true, selecionado: (silviculturaProgramaProdutorFlorestalFomento).ToString());
		}
	}
}