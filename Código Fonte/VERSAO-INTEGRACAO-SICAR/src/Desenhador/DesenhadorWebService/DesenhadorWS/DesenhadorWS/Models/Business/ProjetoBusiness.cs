using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.DesenhadorWS.Models.Entities;
using Tecnomapas.DesenhadorWS.Models.DataAcess;

namespace Tecnomapas.DesenhadorWS.Models.Business
{
    public class ProjetoBusiness
    {
        ProjetoDa _da;

        public ProjetoBusiness()
        {
            _da = new ProjetoDa();
        }

        #region Busca dados do projeto
        public Projeto BuscarDadosProjeto(int idProjeto, int idFilaTipo)
        {
            return _da.BuscarDadosProjeto(idProjeto, idFilaTipo);
        }
        #endregion

        #region Listar Quadro de áreas
        public List<CategoriaQuadroDeArea> ListarQuadroAreas(int idNavegador, int idProjeto)
        {
            List<CategoriaQuadroDeArea> layersQtde = null;

            switch(idNavegador)
            {
                case 1:
					if (_da.VerficiarProjetoFinalizado(idProjeto))
					{
						layersQtde = _da.ListarQuadroAreasDominialidadeFinalizado(idProjeto);
					}
					else
					{
						layersQtde = _da.ListarQuadroAreasDominialidade(idProjeto);
					}
                    break;
                case 2:
					if (_da.VerficiarProjetoFinalizado(idProjeto))
					{
						layersQtde = _da.ListarQuadroAreasAtividadeFinalizado(idProjeto);
					}
					else
					{
						layersQtde = _da.ListarQuadroAreasAtividade(idProjeto);
					}
                    break;
                case 3:
					if (_da.VerficiarProjetoFinalizado(idProjeto,true))
					{
						layersQtde = _da.ListarQuadroAreasFiscalizacaoFinalizado(idProjeto);
					}
					else
					{
						layersQtde = _da.ListarQuadroAreasFiscalizacao(idProjeto);
					}
                    break;
				case 4:
					if (_da.VerficiarProjetoCARFinalizado(idProjeto))
					{
						layersQtde = _da.ListarQuadroAreasCARFinalizado(idProjeto);
					}
					else
					{
						layersQtde = _da.ListarQuadroAreasCAR(idProjeto);
					}
					break;
            }

			var lstDescricao = _da.ObterFeicaoDescricao();

			if (layersQtde != null && layersQtde.Count > 0)
			{
				layersQtde.ForEach(layer =>
				{
					layer.Itens.ForEach(feicao =>
					{

						var feicaoNome = feicao.Nome;

						if (feicaoNome.IndexOf("APP", StringComparison.InvariantCultureIgnoreCase) > -1 && feicaoNome.IndexOf("{") > -1)
						{
							feicaoNome = "APP";
						}

						feicao.Descricao = (lstDescricao.FirstOrDefault(x => x.Nome == feicaoNome) ?? new ItemQuadroDeArea()).Descricao;

					});
				});
			}

            return layersQtde;
        }
        #endregion
        
        #region Salva Área de Abrangência

		public Retorno SalvarAreaAbrangencia(FeicaoAreaAbrangencia feicao)
        {
            try
            {
                bool sucesso = false;

                if (feicao != null && feicao.IdProjeto > 0 && feicao.MinX >0 && feicao.MinY >0&&
                                                feicao.MaxX>0 && feicao.MaxY>0)
                {
					#region Corrigir Mbr
					double maxX = feicao.MaxX;
					double maxY = feicao.MaxY;
					double minX = feicao.MinX;
					double minY = feicao.MinY;

					if (minX > maxX)
					{
						feicao.MaxX = minX;
						feicao.MinX = maxX;
					}

					if (minY > maxY)
					{
						feicao.MaxY = minY;
						feicao.MinY = maxY;
					} 
					#endregion
					
                    sucesso = _da.SalvarAreaAbrangencia(feicao);
                }
                return new Retorno(sucesso);
            }
            catch (Exception exc)
            {
                return new Retorno(false, exc.Message);
            }
        }
        #endregion

        #region Invalidar Processamento
        public static void InvalidarProcessamento(int idProjeto)
        {
            ProjetoDa.InvalidarProcessamento(idProjeto);
        }
        #endregion
    }
}

