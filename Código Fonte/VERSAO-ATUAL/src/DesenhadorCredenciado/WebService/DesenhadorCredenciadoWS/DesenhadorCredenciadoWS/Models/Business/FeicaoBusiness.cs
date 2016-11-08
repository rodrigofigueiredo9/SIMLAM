using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using Tecnomapas.DesenhadorWS.Models.Entities;
using Tecnomapas.DesenhadorWS.Models.DataAcess;

namespace Tecnomapas.DesenhadorWS.Models.Business
{
    public class FeicaoBusiness
    {
        FeicaoDa da;
        public FeicaoBusiness()
        {
            da = new FeicaoDa();
        }

        #region Cadastra geometria e grava atributos
        public Retorno Cadastrar(FeicaoObjeto feicao)
        {
            LayerFeicao layerFeicao;
            LayerFeicaoBusiness layerFeicaoBus = new LayerFeicaoBusiness();
            int id = 0;
            try
            {
                Hashtable filtros = new Hashtable();
                filtros.Add("id", feicao.IdLayerFeicao);
                layerFeicao = layerFeicaoBus.Buscar(filtros);
                if (feicao == null)
                    return new Retorno(false, "Layer Feição inválida", id, feicao.IdLayerFeicao);
                
                FeicaoGeometria geoFeicao = null;
                string mensagem;
                            
                mensagem = string.Empty;
                geoFeicao = InstanciarObjeto(layerFeicao.TipoGeometria, feicao.Vertices, feicao.Atributos, feicao.Aneis);
                id = da.GerarId(layerFeicao.Sequencia);

                AtributoFeicao atributo = geoFeicao.Atributos.Find(delegate(AtributoFeicao a) { return a.Nome.ToUpper() == layerFeicao.ColunaPK.ToUpper(); });
                if (atributo != null)
                {
                    atributo.Tipo = AtributoFeicao.TipoAtributo.Manual;
                    atributo.Valor = id;
                }
                else
                {
                    atributo = new AtributoFeicao(AtributoFeicao.TipoAtributo.Manual, layerFeicao.ColunaPK.ToUpper(), id);
                    geoFeicao.Atributos.Add(atributo);
                }

                if (!da.Cadastrar(geoFeicao, geoFeicao.TabelaRascunho, layerFeicao.Id))
                {
                    return new Retorno(false, "Erro ao cadastrar geometria", id, feicao.IdLayerFeicao);
                }
                if (!da.ValidarGeometria(id, geoFeicao.TabelaRascunho, layerFeicao.ColunaPK, layerFeicao.Id, feicao.IdProjeto, out mensagem))
                {
					da.Excluir(geoFeicao.ObjectId, geoFeicao.TabelaRascunho, layerFeicao.ColunaPK);
                    return new Retorno(false, mensagem, id, feicao.IdLayerFeicao);
                }

                da.Transferir(layerFeicao.Schema, layerFeicao.Tabela, geoFeicao.TabelaRascunho, id);

                da.AtualizarAtributos(geoFeicao.Atributos, layerFeicao.Schema, layerFeicao.Tabela, id, layerFeicao.ColunaPK);

                LayerFeicaoBusiness.LimparCacheLayers(feicao.IdProjeto);
               
               
            }
            catch (Exception exc)
            {
				LogDa.Gerar(new { Exc = exc, Feicao = feicao});
				return new Retorno(false, exc.Message);
            }
            return new Retorno(true, "Cadastrado com sucesso!", id, feicao.IdLayerFeicao);
        }
        #endregion

        #region Exclui geometria e atributos
        public Retorno Excluir(int idLayerFeicao, int objectId, int idProjeto)
        {
            LayerFeicao layerFeicao;
            LayerFeicaoBusiness feicaoBus = new LayerFeicaoBusiness();
            List<Retorno> retornos = new List<Retorno>();
            try
            {
                Hashtable filtros = new Hashtable();
                filtros.Add("id", idLayerFeicao);
                layerFeicao = feicaoBus.Buscar(filtros);
                if (layerFeicao == null) throw new Exception("Layer Feição inválida");
                
                if (!da.Excluir(objectId, layerFeicao.Tabela, layerFeicao.ColunaPK))
                {
                    retornos.Add(new Retorno(false, "Erro ao excluir geometria",objectId, idLayerFeicao));
                }
                else
                {
                    retornos.Add(new Retorno(true, "OBJECTID", objectId, idLayerFeicao));
                }
                LayerFeicaoBusiness.LimparCacheLayers(idProjeto);
               
            }
            catch (Exception exc)
            {
				LogDa.Gerar(new { Exc = exc, idLayerFeicao = idLayerFeicao, objectId = objectId, idProjeto = idProjeto });
                return new Retorno(false, exc.Message);
            }
            return new Retorno(true);
        }
        #endregion

        #region Atualiza apenas a geometria
        public Retorno AtualizarGeometria(FeicaoObjeto feicao)
        {
            LayerFeicao layerFeicao;
            LayerFeicaoBusiness layerFeicaoBus = new LayerFeicaoBusiness();
            try
            {
                Hashtable filtros = new Hashtable();
                filtros.Add("id", feicao.IdLayerFeicao);
                layerFeicao = layerFeicaoBus.Buscar(filtros);
                if (layerFeicao == null) throw new Exception("Layer Feição inválida");

                FeicaoGeometria geoFeicao;

                string mensagem = string.Empty;

                geoFeicao = InstanciarObjeto(layerFeicao.TipoGeometria, feicao.Vertices, feicao.Atributos, feicao.Aneis);

                AtributoFeicao atributo = geoFeicao.Atributos.Find(delegate(AtributoFeicao a) { return a.Nome.ToUpper() == layerFeicao.ColunaPK.ToUpper(); });
                if (atributo == null)
                {
                    return new Retorno(true, "Para atualizar uma geometria é obrigatório enviar o campo ID como atributo", feicao.ObjectId, feicao.IdLayerFeicao); 
                }

                if (!da.AtualizarRascunho(geoFeicao, feicao.ObjectId, geoFeicao.TabelaRascunho, layerFeicao.ColunaPK, layerFeicao.Id))
                {
                    return new Retorno(false, "Erro ao atualizar geometria", feicao.ObjectId, feicao.IdLayerFeicao); 
                }

                if (!da.ValidarGeometria(feicao.ObjectId, geoFeicao.TabelaRascunho, layerFeicao.ColunaPK, layerFeicao.Id, feicao.IdProjeto, out mensagem))
                {
                    return new Retorno(false, mensagem, feicao.ObjectId, feicao.IdLayerFeicao); 
                }

                da.TransferirAtualizarGeometria(layerFeicao.Tabela, geoFeicao.TabelaRascunho, feicao.ObjectId);

                LayerFeicaoBusiness.LimparCacheLayers(feicao.IdProjeto);
                
            }
            catch (Exception exc)
            {
				LogDa.Gerar(new { Exc = exc, feicao = feicao });
                return new Retorno(false, exc.Message, feicao.ObjectId, feicao.IdLayerFeicao); 
            }

            return new Retorno(true, "OBJECTID", feicao.ObjectId, feicao.IdLayerFeicao); 
        }
        #endregion

        #region Atualiza apenas os atributos
        public Retorno AtualizarAtributos(List<AtributoFeicao> atributos, int idLayerFeicao, int objectId, int idProjeto)
        {
            try
            {
                LayerFeicao layerFeicao;
                LayerFeicaoBusiness layerFeicaoBus = new LayerFeicaoBusiness();
                Hashtable filtros = new Hashtable();
                filtros.Add("id", idLayerFeicao);
                layerFeicao = layerFeicaoBus.Buscar(filtros);
                if (layerFeicao == null) throw new Exception("Layer Feição inválida");

				var colunasEditaveis = layerFeicao.Colunas.Where(x => x.IsEditavel).ToList();
				atributos.RemoveAll(x => !colunasEditaveis.Any(ed => String.Equals(ed.Coluna, x.Nome, StringComparison.InvariantCultureIgnoreCase)));

                da.AtualizarAtributos(atributos, layerFeicao.Schema, layerFeicao.Tabela, objectId, layerFeicao.ColunaPK);
               
                return new Retorno(true, "OBJECTID", objectId, idLayerFeicao);
            }
            catch (Exception exc)
            {
				LogDa.Gerar(new { Exc = exc });
				LogDa.Gerar(new { atributos = atributos });
				LogDa.Gerar(new { idLayerFeicao = idLayerFeicao, objectId = objectId, idProjeto = idProjeto });
                return new Retorno(false, exc.Message, objectId, idLayerFeicao);
            }
        }
        #endregion

        #region Importar Feicoes do Projeto Processado ou Finalizado
        public Retorno ImportarFeicoes(int idNavegador, int idProjeto, bool isFinalizadas)
        {
            try
            {
                Retorno resposta = da.ImportarFeicoes(idNavegador, idProjeto, isFinalizadas);
                             
                LayerFeicaoBusiness.LimparCacheLayers(idProjeto);
                return resposta;
            }
            catch (Exception exc)
            {
				LogDa.Gerar(new { Exc = exc, idNavegador = idNavegador, idProjeto = idProjeto, isFinalizadas = isFinalizadas});
                return new Retorno(false, exc.Message);
            }
        }
        #endregion

        private FeicaoGeometria InstanciarObjeto(TipoGeometriaFeicao tipo, List<Vertice> vertices, List<AtributoFeicao> atributos, List<List<Vertice>> aneis = null)
        {
            FeicaoGeometria geoFeicao = null;
            switch (tipo)
            {
                case TipoGeometriaFeicao.Linha:
                    geoFeicao = new Linha(vertices, atributos);
                    break;
                case TipoGeometriaFeicao.Poligono:
                    geoFeicao = new Poligono(vertices, atributos, aneis);
                    break;
                case TipoGeometriaFeicao.Ponto:
                    geoFeicao = new Ponto(vertices, atributos);
                    break;
            }
            return geoFeicao;
        }

		public string ObterCultura()
		{
			List<string> lstCultura = new List<string>();

			System.Collections.Generic.Dictionary<string, string> dic = da.ObterParameters();

			foreach (var item in dic)
			{
				lstCultura.Add(string.Format("\n{0}: {1}", item.Key, item.Value));
			}

			return string.Format("Cultura da Thread: {0}\n {1}",
				System.Threading.Thread.CurrentThread.CurrentCulture.DisplayName,
				string.Concat(lstCultura));
		}
    }
}