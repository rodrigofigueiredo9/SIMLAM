using System;
using System.Linq;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade.PDF;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloRegularizacaoFundiaria;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Model.Business;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class PossePDF
	{
		private List<Lista> _lstLimite = new List<Lista>();
		private List<RelacaoTrabalho> _relacoesTrabalho = new List<RelacaoTrabalho>();

		public Int32? DominioId { get; set; }
		public String ComprovacaoTexto { get; set; }
		public Decimal AreaCroquiDecimal { get; set; }
		public String AreaCroqui { get { return AreaCroquiDecimal.ToStringTrunc(); } }
		public String AreaPosse { get { return AreaCroquiDecimal.ToStringTrunc(); } }
		public Decimal AreaPerimetroDecimal { get; set; }
		public String AreaPerimetro { get { return AreaPerimetroDecimal.ToStringTrunc(3); } }
		public String AreaRequerida { set; get; }
		public Int32 Zona { get; set; }
		public Int32 RelacaoTrabalho { set; get; }
		public String Observacoes { get; set; }
		public String PertenceLimite { get; set; }
		public String DistanciaComercial { get; set; }
		public String DistanciaBR { get; set; }
		public String DistanciaES { get; set; }
		public String BenfeitoriasEdificacoes { get; set; }

		public String Identificacao { get; set; }
		public Int32 ComprovacaoId { get; set; }
		public Decimal Perimetro { get; set; }
		public Int32 RegularizacaoTipo { set; get; }
		public String Benfeitorias { get; set; }

		public Decimal AreaPosseDocumento { get; set; }
		public String DescricaoComprovacao { get; set; }

		public String TrabalhoPredominante
		{
			get
			{
				return String.Join(", ", _relacoesTrabalho.Where(x => (x.Codigo & RelacaoTrabalho) != 0).Select(x => x.Texto).ToArray());
			}
		}

		/*Dominio*/
		public long? NumeroCCIR { get; set; }
		public String AreaCCIR { get; set; }
		public String DataAtualizacao { get; set; }
		public String ConfrontacaoNorte { get; set; }
		public String ConfrontacaoLeste { get; set; }
		public String ConfrontacaoOeste { get; set; }
		public String ConfrontacaoSul { get; set; }

		private String _naoPossuiAreaTituladaAnexa = "Não possui área titulada anexa à posse";
		public String NaoPossuiAreaTituladaAnexa
		{
			get { return _naoPossuiAreaTituladaAnexa; }
			set { _naoPossuiAreaTituladaAnexa = value; }
		}

		private String _naoPossuiEdificacoes = "Não possui edificações";
		public String NaoPossuiEdificacoes
		{
			get { return _naoPossuiEdificacoes; }
			set { _naoPossuiEdificacoes = value; }
		}

		private String _naoPossuiTransmitentes = "Não possui transmitente";
		public String NaoPossuiTransmitentes
		{
			get { return _naoPossuiTransmitentes; }
			set { _naoPossuiTransmitentes = value; }
		}

		private DominioPDF _dominio = new DominioPDF();
		public DominioPDF Dominio
		{
			get { return _dominio; }
			set { _dominio = value; }
		}

		private List<DominioPDF> _matriculas = new List<DominioPDF>();
		public List<DominioPDF> Matriculas
		{
			get { return _matriculas; }
			set { _matriculas = value; }
		}

		private List<EdificacaoPDF> _edificacoes = new List<EdificacaoPDF>();
		public List<EdificacaoPDF> Edificacoes
		{
			get { return _edificacoes; }
			set { _edificacoes = value; }
		}

		private List<TransmitentePDF> _trasmitentes = new List<TransmitentePDF>();
		public List<TransmitentePDF> Transmitentes
		{
			get { return _trasmitentes; }
			set { _trasmitentes = value; }
		}

		private List<UsoAtualSoloPDF> _usosSolo = new List<UsoAtualSoloPDF>();
		public List<UsoAtualSoloPDF> UsosSolo
		{
			get { return _usosSolo; }
			set { _usosSolo = value; }
		}

		private List<Opcao> _opcoes = new List<Opcao>();
		public List<Opcao> Opcoes
		{
			get { return _opcoes; }
			set { _opcoes = value; }
		}

		#region Opcoes

		public String TerrenoDevolutivo { get; set; }
		public String TerrenoDevolutivoEspecificar { get; set; }
		public String RequerenteResideNaPosse { get; set; }
		public String RequerenteResideNaPosseDataAquisicao { get; set; }
		public String ExisteLitigio { get; set; }
		public String ExisteLitigioNome { get; set; }
		public String SobrepoeSeDivisa { get; set; }
		public String SobrepoeSeDivisaPertenceLimite { get; set; }
		public String BanhadoPorRioCorrego { get; set; }
		public String BanhadoPorRioCorregoNome { get; set; }
		public String PossuiNascente { get; set; }
		public String RedeAgua { get; set; }
		public String RedeEsgoto { get; set; }
		public String LuzEletrica { get; set; }
		public String IluminacaoPublica { get; set; }
		public String RedeTelefonica { get; set; }
		public String Calcada { get; set; }
		public String Pavimentacao { get; set; }
		public String PavimentacaoTipo { get; set; }

		private String OpcaoResposta(int? valor = 0)
		{
			return Convert.ToBoolean(valor) ? "Sim" : "Não";
		}

		#endregion

		#region TotalAreaUsoSolo

		public String TotalAreaUsoSolo { get { return TotalAreaUsoSoloDecimal.ToStringTrunc(); } }
		public Decimal TotalAreaUsoSoloDecimal
		{
			get
			{
				Decimal total = 0;
				if (UsosSolo != null && UsosSolo.Count > 0)
				{
					Decimal aux = 0;
					foreach (var item in UsosSolo) { if (Decimal.TryParse(item.AreaPorcentagem, out aux)) { total += aux; } }
				}

				return total;
			}
		}

		#endregion

		public PossePDF() { }

		public PossePDF(Posse posse, List<Dominio> matriculasAnexas = null)
		{
			_lstLimite = EntitiesBus.ObterRegularizacaoFundiariaTipoLimite();
			_relacoesTrabalho = EntitiesBus.ObterRegularizacaoFundiariaRelacaoTrabalho();

			ComprovacaoTexto = posse.ComprovacaoTexto;
			AreaRequerida = posse.AreaRequerida.ToStringTrunc();
			Zona = posse.Zona;
			AreaCroquiDecimal = posse.AreaCroqui;
			AreaPerimetroDecimal = posse.Perimetro;
			Opcoes = posse.Opcoes;
			Observacoes = posse.Observacoes;
			BenfeitoriasEdificacoes = posse.Benfeitorias;
			DominioId = posse.Dominio;
			Dominio = new DominioPDF(posse.DominioPosse);
			RelacaoTrabalho = posse.RelacaoTrabalho;

			Identificacao = posse.Identificacao;
			NumeroCCIR = posse.NumeroCCIR;
			//ComprovacaoTexto = posse.DescricaoComprovacao;
			AreaCCIR = posse.AreaCCIR.ToString();
			DataAtualizacao = posse.DataUltimaAtualizacaoCCIR.DataTexto;
			ConfrontacaoNorte = posse.ConfrontacoesNorte;
			ConfrontacaoSul = posse.ConfrontacoesSul;
			ConfrontacaoLeste = posse.ConfrontacoesLeste;
			ConfrontacaoOeste = posse.ConfrontacoesOeste;

			#region Lists

			if (matriculasAnexas != null)
			{
				foreach (var item in matriculasAnexas)
				{
					Matriculas.Add(new DominioPDF(item));
				}

				foreach (var item in posse.DominiosAvulsos)
				{
					Matriculas.Add(new DominioPDF(item));
				}
			}

			foreach (var item in posse.Transmitentes)
			{
				Transmitentes.Add(new TransmitentePDF(item));
			}

			foreach (var item in posse.UsoAtualSolo)
			{
				UsosSolo.Add(new UsoAtualSoloPDF(item));
			}

			foreach (var item in posse.Edificacoes)
			{
				Edificacoes.Add(new EdificacaoPDF(item));
			}

			#endregion

			#region Opcoes

			Opcao opcaoTerrenoDevolutivo = (Opcoes.FirstOrDefault(x => x.TipoEnum == eTipoOpcao.TerrenoDevoluto) ?? new Opcao());
			TerrenoDevolutivo = OpcaoResposta(opcaoTerrenoDevolutivo.Valor);

			if (Convert.ToBoolean(opcaoTerrenoDevolutivo.Valor))
			{
				TerrenoDevolutivoEspecificar = string.Empty;
			}
			else
			{
				TerrenoDevolutivoEspecificar = opcaoTerrenoDevolutivo.Outro;
			}

			RequerenteResideNaPosse = OpcaoResposta((Opcoes.FirstOrDefault(x => x.TipoEnum == eTipoOpcao.RequerenteResideNaPosse) ?? new Opcao()).Valor);
			RequerenteResideNaPosseDataAquisicao = (Opcoes.FirstOrDefault(x => x.TipoEnum == eTipoOpcao.RequerenteResideNaPosse) ?? new Opcao()).Outro;
			ExisteLitigio = OpcaoResposta((Opcoes.FirstOrDefault(x => x.TipoEnum == eTipoOpcao.ExisteLitigio)?? new Opcao()).Valor);
			ExisteLitigioNome = (Opcoes.FirstOrDefault(x => x.TipoEnum == eTipoOpcao.ExisteLitigio) ?? new Opcao()).Outro;
			SobrepoeSeDivisa = OpcaoResposta((Opcoes.FirstOrDefault(x => x.TipoEnum == eTipoOpcao.SobrepoeSeDivisa) ?? new Opcao()).Valor);
			SobrepoeSeDivisaPertenceLimite = (_lstLimite.FirstOrDefault(y => y.Id == (Opcoes.FirstOrDefault(x => x.TipoEnum == eTipoOpcao.SobrepoeSeDivisa) ?? new Opcao()).Outro) ?? new Lista()).Texto;
			BanhadoPorRioCorrego = OpcaoResposta((Opcoes.FirstOrDefault(x => x.TipoEnum == eTipoOpcao.BanhadoPorRioCorrego) ?? new Opcao()).Valor);
			BanhadoPorRioCorregoNome = (Opcoes.FirstOrDefault(x => x.TipoEnum == eTipoOpcao.BanhadoPorRioCorrego) ?? new Opcao()).Outro;
			PossuiNascente = OpcaoResposta((Opcoes.FirstOrDefault(x => x.TipoEnum == eTipoOpcao.PossuiNascente) ?? new Opcao()).Valor);
			RedeAgua = OpcaoResposta((Opcoes.FirstOrDefault(x => x.TipoEnum == eTipoOpcao.RedeAgua) ?? new Opcao()).Valor);
			RedeEsgoto = OpcaoResposta((Opcoes.FirstOrDefault(x => x.TipoEnum == eTipoOpcao.RedeEsgoto) ?? new Opcao()).Valor);
			RedeTelefonica = OpcaoResposta((Opcoes.FirstOrDefault(x => x.TipoEnum == eTipoOpcao.RedeTelefonica) ?? new Opcao()).Valor);
			LuzEletrica = OpcaoResposta((Opcoes.FirstOrDefault(x => x.TipoEnum == eTipoOpcao.LuzEletrica) ?? new Opcao()).Valor);
			IluminacaoPublica = OpcaoResposta((Opcoes.FirstOrDefault(x => x.TipoEnum == eTipoOpcao.IluminacaoPublica) ?? new Opcao()).Valor);
			Calcada = OpcaoResposta((Opcoes.FirstOrDefault(x => x.TipoEnum == eTipoOpcao.Calcada) ?? new Opcao()).Valor);
			Pavimentacao = OpcaoResposta((Opcoes.FirstOrDefault(x => x.TipoEnum == eTipoOpcao.Pavimentacao) ?? new Opcao()).Valor);
			PavimentacaoTipo = (Opcoes.FirstOrDefault(x => x.TipoEnum == eTipoOpcao.Pavimentacao) ?? new Opcao()).Outro;

			#endregion
		}
	}
}