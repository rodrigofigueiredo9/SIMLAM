using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloAquicultura;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels
{
	public class AquiculturaVM
	{
		public bool IsVisualizar { get; set; }
		public bool IsEditar { get; set; }
		public String TextoAbrirModal { get; set; }
		public String TextoMerge { get; set; }
		public String AtualizarDependenciasModalTitulo { get; set; }

		private List<AquiculturaAquicultVM> _aquiculturaAquicultVM = new List<AquiculturaAquicultVM>();
		public List<AquiculturaAquicultVM> AquiculturaAquicultVM
		{
			get { return _aquiculturaAquicultVM; }
			set { _aquiculturaAquicultVM = value; }
		}

		public AquiculturaAquicultVM AquiculturaAquicultTemplateVM {get;set;}

		private Aquicultura _caracterizacao = new Aquicultura();
		public Aquicultura Caracterizacao
		{
			get { return _caracterizacao; }
			set { _caracterizacao = value; }
		}

		public Int32 QtdAtividade
		{
			get 
			{
				return 10;
			}
		}

		public String IdsTela
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@Atividade01 = ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.PisciculturaCarciniculturaEspeciesAguaDoceViveirosEscavadosInclusivePolicultivo01),
					@Atividade02 = ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.PisciculturaEspeciesAguaDoceViveirosEscavadosUnidadePescaEsportivatipo02),
					@Atividade03 = ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.CarciniculturaEspeciesAguaDoceViveirosEscavados03),
					@Atividade04 = ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.PisciculturaEspeciesAguaDoceEmGaiolasTanquesAlvenaria04),
					@Atividade05 = ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.PisciculturaEspeciesAguaDoceGaiolasNaoIncluiProducaoLarvas05),
					@Atividade06 = ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.PisciculturaEspeciesAguaDoceTanquesBemComoCultivoPeixesOrnamentais06),
					@Atividade07 = ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.CarciniculturaEspeciesAguaDoceGaiolasTanquesAlvenariaOuMaterialIsolamento07),
					@Atividade08 = ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.CarciniculturaEspeciesAguaDoceEmGaiolasRaceway08),
					@Atividade09 = ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.CarciniculturaEspeciesAguaDoceEmGaiolasRacewayApenasEngorda09),
					@Atividade10 = ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.CriacaoAnimaisConfinadosPequenoPorteAmbienteAquatico10)
				});
			}
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@ListaAtividadeObrigatoria = Mensagem.Aquicultura.ListaAtividadeObrigatoria,

					@VolumeCultivoObrigatorio = Mensagem.Aquicultura.VolumeCultivoObrigatorio,
					@VolumeCultivoInvalido = Mensagem.Aquicultura.VolumeCultivoInvalido,
					@VolumeCultivoMaiorZero = Mensagem.Aquicultura.VolumeCultivoMaiorZero,

					@CultivosJaAdicionados  = Mensagem.Aquicultura.CultivosJaAdicionados,
					@NumUnidadeCultivosObrigatorio = Mensagem.Aquicultura.NumUnidadeCultivosObrigatorio(String.Empty),
					@NumUnidadeCultivosInvalido = Mensagem.Aquicultura.NumUnidadeCultivosInvalido(String.Empty),
					@NumUnidadeCultivosMaiorZero = Mensagem.Aquicultura.NumUnidadeCultivosMaiorZero(String.Empty)
				});
			}
		}

		public AquiculturaVM(Aquicultura caracterizacao, bool isVisualizar = false, bool isEditar = false)
		{
			Caracterizacao = caracterizacao;
			IsVisualizar = isVisualizar;
			IsEditar = isEditar;
		}
		
	}
}