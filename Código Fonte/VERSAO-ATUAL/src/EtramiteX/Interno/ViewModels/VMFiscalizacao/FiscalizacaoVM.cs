using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMProjetoGeografico;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao
{
	public class FiscalizacaoVM
	{
		public int Id { get; set; }
		public Boolean IsVisualizar { get; set; }
        		
        public String PartialInicial { get; set; }

		public Fiscalizacao Fiscalizacao { get; set; }

		private LocalInfracaoVM _localInfracaoVM = new LocalInfracaoVM();
		public LocalInfracaoVM LocalInfracaoVM
		{
			get { return _localInfracaoVM; }
			set { _localInfracaoVM = value; }
		}

		private ProjetoGeograficoVM _projetoGeoVM = new ProjetoGeograficoVM();
		public ProjetoGeograficoVM ProjetoGeoVM
		{
			get { return _projetoGeoVM; }
			set { _projetoGeoVM = value; }
		}

		private ComplementacaoDadosVM _complementacaoDadosVM = new ComplementacaoDadosVM();
		public ComplementacaoDadosVM ComplementacaoDadosVM
		{
			get { return _complementacaoDadosVM; }
			set { _complementacaoDadosVM = value; }
		}

		private InfracaoVM _infracaoVM = new InfracaoVM();
		public InfracaoVM InfracaoVM
		{
			get { return _infracaoVM; }
			set { _infracaoVM = value; }
		}

		private MaterialApreendidoVM _materialapreendidoVM = new MaterialApreendidoVM();
		public MaterialApreendidoVM MaterialApreendidoVM
		{
			get { return _materialapreendidoVM; }
			set { _materialapreendidoVM = value; }
		}

		private EnquadramentoVM _enquadramentoVM = new EnquadramentoVM();
		public EnquadramentoVM EnquadramentoVM
		{
			get { return _enquadramentoVM; }
			set { _enquadramentoVM = value; }
		}

		private ObjetoInfracaoVM _objetoInfracaoVM = new ObjetoInfracaoVM();
		public ObjetoInfracaoVM ObjetoInfracaoVM
		{
			get { return _objetoInfracaoVM; }
			set { _objetoInfracaoVM = value; }
		}

		private ConsideracaoFinalVM _consideracaoFinalVM = new ConsideracaoFinalVM();
		public ConsideracaoFinalVM ConsideracaoFinalVM
		{
			get { return _consideracaoFinalVM; }
			set { _consideracaoFinalVM = value; }
		}

		private List<FiscalizacaoDocumento> _documentosCancelados = new List<FiscalizacaoDocumento>();
		public List<FiscalizacaoDocumento> DocumentosCancelados
		{
			get { return _documentosCancelados; }
			set { _documentosCancelados = value; }
		}

		private List<Acompanhamento> _acompanhamentos = new List<Acompanhamento>();
		public List<Acompanhamento> Acompanhamentos
		{
			get { return _acompanhamentos; }
			set { _acompanhamentos = value; }
		}

		public string NumeroAutos
		{
			get
			{
				if (this.Fiscalizacao.Situacao == eFiscalizacaoSituacao.EmAndamento)
				{
					return "Numero Automático";
				}
				else 
				{
					return this.Fiscalizacao.NumeroAutos.ToString();
				}
			}
		}

		#region N_AI

		public string N_AI 
		{
			get 
			{
				return this.InfracaoVM.Infracao.IsGeradaSistema.GetValueOrDefault() ? this.NumeroAutos : this.InfracaoVM.Infracao.NumeroAutoInfracaoBloco;
			}
		}

		public string N_AI_EmitidoPor
		{
			get
			{
				if (this.InfracaoVM.Infracao.IsGeradaSistema.HasValue)
				{
					return this.InfracaoVM.Infracao.IsGeradaSistema.Value ? "Sistema" : "Bloco";	
				}
				return string.Empty;
			}
		}

		public string N_AI_DataAuto
		{
			get
			{
				if (InfracaoVM.Infracao.IsGeradaSistema.GetValueOrDefault()) 
				{
					if (Fiscalizacao.SituacaoId != (int)eFiscalizacaoSituacao.EmAndamento) 
					{
						return Fiscalizacao.DataConclusao.DataTexto;
					}

					return String.Empty;
				}
				return this.InfracaoVM.Infracao.DataLavraturaAuto.DataTexto;
			}
		}

		#endregion

		#region N_TEI

		public string N_TEI
		{
			get
			{
				return this.ObjetoInfracaoVM.Entidade.TeiGeradoPeloSistema.GetValueOrDefault() > 0 ? this.NumeroAutos : this.ObjetoInfracaoVM.Entidade.NumTeiBloco;
			}
		}

		public string N_TEI_EmitidoPor
		{
			get
			{
				if (this.ObjetoInfracaoVM.Entidade.TeiGeradoPeloSistema.HasValue)
				{
					return this.ObjetoInfracaoVM.Entidade.TeiGeradoPeloSistema.Value > 0 ? "Sistema" : "Bloco";	
				}
				return string.Empty;
			}
		}

		public string N_TEI_DataTermo
		{
			get
			{
				if (ObjetoInfracaoVM.Entidade.TeiGeradoPeloSistema.GetValueOrDefault(0) == 1) 
				{
					if (Fiscalizacao.SituacaoId != (int)eFiscalizacaoSituacao.EmAndamento) 
					{
						return Fiscalizacao.DataConclusao.DataTexto;
					}

					return String.Empty;
				}
				return this.ObjetoInfracaoVM.Entidade.DataLavraturaTermo.DataTexto;
			}
		}

		#endregion

        //#region N_TAD

        //public string N_TAD
        //{
        //    get
        //    {
        //        return this.MaterialApreendidoVM.MaterialApreendido.IsTadGeradoSistema.GetValueOrDefault() ? this.NumeroAutos : this.MaterialApreendidoVM.MaterialApreendido.NumeroTad;
        //    }
        //}

        //public string N_TAD_EmitidoPor
        //{
        //    get
        //    {
        //        if (this.MaterialApreendidoVM.MaterialApreendido.IsTadGeradoSistema.HasValue)
        //        {
        //            return this.MaterialApreendidoVM.MaterialApreendido.IsTadGeradoSistema.Value ? "Sistema" : "Bloco";	
        //        }
        //        return string.Empty;
        //    }
        //}

        //public string N_TAD_DataTermo
        //{
        //    get
        //    {
        //        if (MaterialApreendidoVM.MaterialApreendido.IsTadGeradoSistema.GetValueOrDefault())
        //        {
        //            if (Fiscalizacao.SituacaoId != (int)eFiscalizacaoSituacao.EmAndamento)
        //            {
        //                return Fiscalizacao.DataConclusao.DataTexto;
        //            }

        //            return String.Empty;
        //        }
        //        return this.MaterialApreendidoVM.MaterialApreendido.DataLavratura.DataTexto;
        //    }
        //}

        //#endregion

		public string LabelTrDownload
		{
			get 
			{
				string label = this.InfracaoVM.Infracao.IsGeradaSistema.GetValueOrDefault() ? "Auto de infração" : "";
				label = this.MaterialApreendidoVM.MaterialApreendido.IsTadGeradoSistema.GetValueOrDefault() ? label + (string.IsNullOrEmpty(label) ? "" : " / ") + "Termo de apreensão e depósito" : label;
				label = this.ObjetoInfracaoVM.Entidade.TeiGeradoPeloSistema.GetValueOrDefault() > 0 ? label + (string.IsNullOrEmpty(label) ? "" : " / ") + "Termo de embargo e interdição" : label;
				return label;
			}
		}

		public FiscalizacaoVM()
		{
			this.PartialInicial = "ComplementacaoDados";
			this.Fiscalizacao = new Fiscalizacao();
		}

		public String JSON(object obj)
		{
			return ViewModelHelper.Json(obj);
		}	
	}
}