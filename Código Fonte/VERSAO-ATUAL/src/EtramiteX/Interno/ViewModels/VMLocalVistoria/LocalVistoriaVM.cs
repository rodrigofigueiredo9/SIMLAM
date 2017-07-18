using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloLocalVistoria;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMLocalVistoria
{
	public class LocalVistoriaVM
	{

        private DiaHoraVistoria _diaHoraLocalVistoria = new DiaHoraVistoria();

        private List<BloqueioLocalVistoria> _lstBloqueios = new List<BloqueioLocalVistoria>();

        public List<BloqueioLocalVistoria> ListBloqueios
        {
            get { return _lstBloqueios; }
            set { _lstBloqueios = value; }
        }

        
        public bool IsEdicao { get; set; }
        public bool IsVisualizar { get; set; }
        

        private List<SelectListItem> _listadiasSemana = new List<SelectListItem>();

        public List<SelectListItem> ListaDiasSemana 
        {
            get { return _listadiasSemana; }
            set { _listadiasSemana = value; }
        }

        private List<SelectListItem> _setores = new List<SelectListItem>();
        public List<SelectListItem> Setores
        {
            get { return _setores; }
            set { _setores = value; }
        }


		private bool _mostrarGrid = true;
		public bool MostrarGrid
		{
			get { return _mostrarGrid; }
			set { _mostrarGrid = value; }
		}

        public DiaHoraVistoria DiaHoraLocalVistoria
        {
            get { return _diaHoraLocalVistoria; }
            set { _diaHoraLocalVistoria = value;}

        }

        private List<DiaHoraVistoria> _diasHorasVistoria = new List<DiaHoraVistoria>();
        public List<DiaHoraVistoria> DiasHorasVistoria
        {
            get { return _diasHorasVistoria; }
            set { _diasHorasVistoria = value; }
        }

        public string Mensagens
        {
            get
            {
                return ViewModelHelper.Json(new
                {
                    @HoraInicioObrigatorio = Mensagem.LocalVistoria.HoraFimObrigatorio,
                    @HoraFimObrigatorio = Mensagem.LocalVistoria.HoraFimObrigatorio,
                    @SetorObrigatorio = Mensagem.LocalVistoria.SetorObrigatorio,
                    @DiaSemanaObrigatorio = Mensagem.LocalVistoria.DiaSemanaObrigatorio,
                    @HoraInicioInvalida = Mensagem.LocalVistoria.HoraInicioInvalida,
                    @HoraFimInvalida = Mensagem.LocalVistoria.HoraFimInvalida,
                    @HoraInicialMenorHoraFinal = Mensagem.LocalVistoria.HoraInicialMenorHoraFinal("")
                });

            }
        }


        public LocalVistoriaVM(List<Setor> setores, int setorSelecionado, List<Lista> diasSemana) 
        {
            Setores = ViewModelHelper.CriarSelectList(setores, true, true, setorSelecionado.ToString());
            ListaDiasSemana = ViewModelHelper.CriarSelectList(diasSemana, true, true);


        }
        public LocalVistoriaVM(List<Setor> setores, List<Lista> diasSemana, List<DiaHoraVistoria> diasHorasVistoria)
        {
            Setores = ViewModelHelper.CriarSelectList(setores, true, true);
            ListaDiasSemana = ViewModelHelper.CriarSelectList(diasSemana, true, true);
            DiasHorasVistoria = diasHorasVistoria;

        }
	}
}