using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.WindowsService.SVCImportarTituloDeclaratorio.Data;

namespace Tecnomapas.EtramiteX.WindowsService.SVCImportarTituloDeclaratorio.Business
{
    public class EmpreendimentoCredenciadoBus
    {
        EmpreendimentoCredenciadoDa _da = new EmpreendimentoCredenciadoDa();
        ConfiguracaoSistema _configSys = new ConfiguracaoSistema();

        public String UsuarioCredenciado
        {
            get { return _configSys.UsuarioCredenciado; }
        }

        public EmpreendimentoCredenciadoBus() { }

        public List<Pessoa> ObterResponsaveis(int empreendimento, BancoDeDados bancoCredenciado)
        {
            return _da.ObterResponsaveis(empreendimento, bancoCredenciado);
        }

        public Empreendimento Obter(int id, BancoDeDados bancoCredenciado, bool simplificado = false)
        {
            Empreendimento emp = _da.Obter(id, bancoCredenciado, simplificado: simplificado) ?? new Empreendimento();
            emp.Enderecos = emp.Enderecos ?? new List<Endereco>();

            if (emp != null && emp.Id > 0)
            {
                emp.SegmentoDenominador =
                    (_da.ObterSegmentos().First(x => x.Id == emp.Segmento.GetValueOrDefault().ToString()) ??
                     new Segmento()).Denominador;
                emp.SegmentoTexto =
                    (_da.ObterSegmentos().First(x => x.Id == emp.Segmento.GetValueOrDefault().ToString()) ??
                     new Segmento()).Texto;

                if (!simplificado)
                {
                    emp.Coordenada.Tipo.Texto =
                        ((_da.ObterTiposCoordenada().FirstOrDefault(x => x.Id == emp.Coordenada.Tipo.Id)) ??
                         new CoordenadaTipo()).Texto;
                    emp.Coordenada.Datum.Texto =
                        ((_da.ObterDatuns().FirstOrDefault(x => x.Id == emp.Coordenada.Datum.Id)) ?? new Datum()).Texto;
                    emp.Coordenada.HemisferioUtmTexto =
                        ((_da.ObterHemisferios().FirstOrDefault(x => x.Id == emp.Coordenada.HemisferioUtm)) ??
                         new CoordenadaHemisferio()).Texto;

                    emp.Coordenada.FormaColetaTexto =
                        ((_da.ObterFormaColetaPonto()
                            .FirstOrDefault(x => x.Id == emp.Coordenada.FormaColeta.GetValueOrDefault().ToString())) ??
                         new Lista()).Texto;
                    emp.Coordenada.LocalColetaTexto =
                        ((_da.ObterFormaColetaPonto()
                            .FirstOrDefault(x => x.Id == emp.Coordenada.LocalColeta.GetValueOrDefault().ToString())) ??
                         new Lista()).Texto;

                    foreach (var item in emp.Enderecos)
                    {
                        if (item.EstadoId > 0)
                        {
                            item.EstadoTexto = _da.ObterEstados().First(x => x.Id == item.EstadoId).Texto;
                            item.MunicipioTexto = _da.ObterMunicipio(item.MunicipioId).Texto;
                        }
                    }

                    foreach (var item in emp.Responsaveis)
                    {
                        item.TipoTexto =
                            (_da.ObterEmpTipoResponsavel().First(x => x.Id == item.Tipo.GetValueOrDefault()) ??
                             new TipoResponsavel()).Texto;
                    }

                    if (emp.Atividade.Id > 0)
                    {
                        AtividadeEmpreendimentoInternoBus atividadeEmpBus = new AtividadeEmpreendimentoInternoBus();

                        emp.Atividade.Atividade =
                            atividadeEmpBus.Filtrar(new AtividadeListarFiltro() {Id = emp.Atividade.Id},
                                new Paginacao() {PaginaAtual = 1, QuantPaginacao = 1}).Itens[0].Atividade;
                    }

                    ContatoLst contatoAux = new ContatoLst();
                    emp.MeiosContatos.ForEach(x =>
                    {
                        contatoAux = (_da.ObterMeiosContato().SingleOrDefault(y => y.Id == (int) x.TipoContato) ??
                                      new ContatoLst());
                        x.Mascara = contatoAux.Mascara;
                        x.TipoTexto = contatoAux.Texto;
                    });
                }
            }

            return emp;
        }

        public void SalvarInternoId(Empreendimento empreendimento, BancoDeDados banco = null)
        {
            GerenciadorTransacao.ObterIDAtual();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
            {
                bancoDeDados.IniciarTransacao();

                _da.SalvarInternoId(empreendimento, bancoDeDados);

                bancoDeDados.Commit();
            }
        }
    }
}
