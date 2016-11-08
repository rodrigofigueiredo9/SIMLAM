using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloAquicultura;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels
{
	public class AquiculturaAquicultVM
	{
		public bool IsVisualizar { get; set; }
		public bool IsEditar { get; set; }
		public String TextoAbrirModal { get; set; }
		public String TextoMerge { get; set; }
		public String AtualizarDependenciasModalTitulo { get; set; }

		public CoordenadaAtividadeVM CoodernadaAtividade { get; set; }

		private AquiculturaAquicult _caracterizacao = new AquiculturaAquicult();
		public AquiculturaAquicult Caracterizacao
		{
			get { return _caracterizacao; }
			set { _caracterizacao = value; }
		}

		public Decimal VolumeTotalCultivos
		{
			get
			{
				Decimal retorno = 0;
				if (Caracterizacao.Cultivos != null && Caracterizacao.Cultivos.Count > 0)
				{
					Decimal aux = 0;
					foreach (Cultivo cultivo in Caracterizacao.Cultivos)
					{
						if (Decimal.TryParse(cultivo.Volume, out aux))
						{
							retorno += aux;
						}
					}
				}

				return retorno;
			}
		}

		private List<SelectListItem> _atividade = new List<SelectListItem>();
		public List<SelectListItem> Atividade
		{
			get { return _atividade; }
			set { _atividade = value; }
		}

		public Boolean MostrarGrupo01
		{
			get
			{
				return ConfiguracaoAtividade.ObterId(new[]
				{
					(int)eAtividadeCodigo.PisciculturaCarciniculturaEspeciesAguaDoceViveirosEscavadosInclusivePolicultivo01, 
					(int)eAtividadeCodigo.PisciculturaEspeciesAguaDoceViveirosEscavadosUnidadePescaEsportivatipo02, 
					(int)eAtividadeCodigo.CarciniculturaEspeciesAguaDoceViveirosEscavados03 
				}).Any(x => x == Caracterizacao.Atividade);
			}
		}
		
		public Boolean MostrarGrupo03 { get { return Caracterizacao.Atividade == ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.CriacaoAnimaisConfinadosPequenoPorteAmbienteAquatico10);}}
		public Boolean MostrarGrupo02  { get { return (!MostrarGrupo01 && !MostrarGrupo03) && Caracterizacao.Atividade != 0; }}
		
		public AquiculturaAquicultVM(AquiculturaAquicult caracterizacao, List<ProcessoAtividadeItem> atividades, List<Lista> tipoGeometrico)
		{
			List<ProcessoAtividadeItem> _atvs = atividades.Where(x => 
										x.Codigo == (int)eAtividadeCodigo.PisciculturaCarciniculturaEspeciesAguaDoceViveirosEscavadosInclusivePolicultivo01 ||
										x.Codigo == (int)eAtividadeCodigo.PisciculturaEspeciesAguaDoceViveirosEscavadosUnidadePescaEsportivatipo02 ||
										x.Codigo == (int)eAtividadeCodigo.CarciniculturaEspeciesAguaDoceViveirosEscavados03 ||
										x.Codigo == (int)eAtividadeCodigo.PisciculturaEspeciesAguaDoceEmGaiolasTanquesAlvenaria04 ||
										x.Codigo == (int)eAtividadeCodigo.PisciculturaEspeciesAguaDoceGaiolasNaoIncluiProducaoLarvas05 ||
										x.Codigo == (int)eAtividadeCodigo.PisciculturaEspeciesAguaDoceTanquesBemComoCultivoPeixesOrnamentais06 ||
										x.Codigo == (int)eAtividadeCodigo.CarciniculturaEspeciesAguaDoceGaiolasTanquesAlvenariaOuMaterialIsolamento07 ||
										x.Codigo == (int)eAtividadeCodigo.CarciniculturaEspeciesAguaDoceEmGaiolasRaceway08 ||
										x.Codigo == (int)eAtividadeCodigo.CarciniculturaEspeciesAguaDoceEmGaiolasRacewayApenasEngorda09 ||
										x.Codigo == (int)eAtividadeCodigo.CriacaoAnimaisConfinadosPequenoPorteAmbienteAquatico10).ToList();

			Atividade = ViewModelHelper.CriarSelectList(_atvs, true, true);
			CoodernadaAtividade = new CoordenadaAtividadeVM(new CoordenadaAtividade(), new List<Lista>(), tipoGeometrico);
			Caracterizacao = caracterizacao;
		}

		public AquiculturaAquicultVM(AquiculturaAquicult caracterizacao, List<ProcessoAtividadeItem> atividades, List<Lista> coordenadaslst, List<Lista> tipoGeometrico, bool isVisualizar = false, bool isEditar = false)
		{

			IsVisualizar = isVisualizar;
			Caracterizacao = caracterizacao;
			IsEditar = isEditar;

			List<ProcessoAtividadeItem> _atvs = atividades.Where(x =>
										x.Codigo == (int)eAtividadeCodigo.PisciculturaCarciniculturaEspeciesAguaDoceViveirosEscavadosInclusivePolicultivo01 ||
										x.Codigo == (int)eAtividadeCodigo.PisciculturaEspeciesAguaDoceViveirosEscavadosUnidadePescaEsportivatipo02 ||
										x.Codigo == (int)eAtividadeCodigo.CarciniculturaEspeciesAguaDoceViveirosEscavados03 ||
										x.Codigo == (int)eAtividadeCodigo.PisciculturaEspeciesAguaDoceEmGaiolasTanquesAlvenaria04 ||
										x.Codigo == (int)eAtividadeCodigo.PisciculturaEspeciesAguaDoceGaiolasNaoIncluiProducaoLarvas05 ||
										x.Codigo == (int)eAtividadeCodigo.PisciculturaEspeciesAguaDoceTanquesBemComoCultivoPeixesOrnamentais06 ||
										x.Codigo == (int)eAtividadeCodigo.CarciniculturaEspeciesAguaDoceGaiolasTanquesAlvenariaOuMaterialIsolamento07 ||
										x.Codigo == (int)eAtividadeCodigo.CarciniculturaEspeciesAguaDoceEmGaiolasRaceway08 ||
										x.Codigo == (int)eAtividadeCodigo.CarciniculturaEspeciesAguaDoceEmGaiolasRacewayApenasEngorda09 ||
										x.Codigo == (int)eAtividadeCodigo.CriacaoAnimaisConfinadosPequenoPorteAmbienteAquatico10).ToList();

			Atividade = ViewModelHelper.CriarSelectList(_atvs, true, true, selecionado: caracterizacao.Atividade.ToString());
			CoodernadaAtividade = new CoordenadaAtividadeVM(caracterizacao.CoordenadaAtividade, coordenadaslst, tipoGeometrico, IsVisualizar);

		}

	}
}